using ERPSEI.Data;
using ERPSEI.Data.Entities.Empresas;
using ERPSEI.Data.Entities.ExpedientesBancarios;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using ERPSEI.Data.Managers.Empresas;

namespace ERPSEI.Services.Compliance
{
    public sealed class DocumentoEmpresasComplianceService
        : IDocumentoEmpresasComplianceService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DocumentoEmpresasComplianceService>
            _logger;
        private readonly IArchivoEmpresaManager _archivoEmpresaManager;

        public DocumentoEmpresasComplianceService(
            ApplicationDbContext context,
            IConfiguration configuration,
            ILogger<DocumentoEmpresasComplianceService> logger,
            IArchivoEmpresaManager archivoEmpresaManager)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _archivoEmpresaManager = archivoEmpresaManager;
        }

        public async Task<ResultadoSincronizacionDocumental>
            SincronizarDesdeEmpresaAsync(
                int empresaMaestraId,
                int complianceId,
                string usuarioId,
                CancellationToken cancellationToken =
                    default)
        {
            /*
             * =====================================================
             * VALIDACIONES
             * =====================================================
             */

            if (empresaMaestraId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(empresaMaestraId)
                );
            }

            if (complianceId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(complianceId)
                );
            }

            if (string.IsNullOrWhiteSpace(usuarioId))
            {
                throw new ArgumentException(
                    "El usuario de sincronización es obligatorio.",
                    nameof(usuarioId)
                );
            }

            /*
             * =====================================================
             * VALIDAR EMPRESA MAESTRA
             * =====================================================
             */

            bool empresaExiste =
                await _context.Set<Empresa>()
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.Id == empresaMaestraId,
                        cancellationToken
                    );

            if (!empresaExiste)
            {
                throw new InvalidOperationException(
                    "No se encontró la empresa maestra."
                );
            }

            /*
             * =====================================================
             * VALIDAR EMPRESA COMPLIANCE
             * =====================================================
             */

            bool empresaComplianceExiste =
                await _context.EbEmpresas
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.Id == complianceId &&
                            !x.Eliminado,
                        cancellationToken
                    );

            if (!empresaComplianceExiste)
            {
                throw new InvalidOperationException(
                    "No se encontró la empresa de Compliance."
                );
            }

            /*
             * =====================================================
             * RUTA DOCUMENTAL DE COMPLIANCE
             * =====================================================
             */

            string? rutaBaseDocumentos =
                _configuration[
                    "ExpedientesBancarios:RutaDocumentos"
                ];

            if (string.IsNullOrWhiteSpace(
                rutaBaseDocumentos))
            {
                throw new InvalidOperationException(
                    "No se encuentra configurada la ruta " +
                    "de almacenamiento documental de Compliance."
                );
            }

            rutaBaseDocumentos =
                Path.GetFullPath(
                    rutaBaseDocumentos
                );

            /*
             * =====================================================
             * OBTENER DOCUMENTOS DE EMPRESAS
             * =====================================================
             *
             * IMPORTANTE:
             *
             * Aquí leemos directamente ArchivoEmpresa porque
             * necesitamos el contenido binario completo para
             * comparar el archivo.
             *
             * NO modificamos ningún ArchivoEmpresa.
             * =====================================================
             */

            List<ArchivoEmpresa> archivosEmpresa =
                await _context.Set<ArchivoEmpresa>()
                    .AsNoTracking()
                    .Where(x =>
                        x.EmpresaId ==
                        empresaMaestraId
                    )
                    .OrderBy(x =>
                        x.TipoArchivoId
                    )
                    .ThenBy(x =>
                        x.Nombre
                    )
                    .ToListAsync(
                        cancellationToken
                    );

            HashSet<int> tiposComplianceUnicosProcesados = new();

            /*
             * =====================================================
             * CATÁLOGO DE TIPOS DE COMPLIANCE
             * =====================================================
             */

            Dictionary<int, EbTipoDocumento>
                tiposCompliance =
                    await _context.EbTiposDocumento
                        .IgnoreQueryFilters()
                        .AsNoTracking()
                        .Where(x =>
                            !x.Eliminado &&
                            !x.Deshabilitado
                        )
                        .ToDictionaryAsync(
                            x => x.Id,
                            cancellationToken
                        );

            int revisados = 0;
            int sincronizados = 0;
            int sinCambios = 0;
            int ignorados = 0;
            int vinculosExistentesCreados = 0;

            /*
             * Guardaremos aquí únicamente los archivos físicos
             * creados durante ESTA sincronización.
             *
             * Si ocurre una excepción antes de guardar la BD,
             * podremos eliminarlos.
             */
            List<string> archivosFisicosCreados =
                new();

            List<(
                EbDocumento Documento,
                EbDocumentoVinculoEmpresa Vinculo
            )> vinculosPendientes =
                new();

            try
            {
                foreach (
                    ArchivoEmpresa archivoEmpresa
                    in archivosEmpresa)
                {
                    revisados++;

                    /*
                     * =============================================
                     * OBTENER EQUIVALENCIA
                     * =============================================
                     */

                    /*
                     * =============================================
                     * VALIDAR TIPO DE ARCHIVO DE EMPRESAS
                     * =============================================
                     *
                     * TipoArchivoId es nullable.
                     *
                     * Si el documento no tiene un tipo asignado,
                     * no puede existir una equivalencia segura
                     * con Compliance, por lo que se ignora.
                     */
                    if (!archivoEmpresa.TipoArchivoId.HasValue)
                    {
                        ignorados++;

                        _logger.LogWarning(
                            "El archivo {ArchivoId} de la empresa {EmpresaId} " +
                            "no tiene TipoArchivoId y no puede sincronizarse.",
                            archivoEmpresa.Id,
                            empresaMaestraId
                        );

                        continue;
                    }

                    /*
                     * =============================================
                     * OBTENER EQUIVALENCIA
                     * =============================================
                     */
                    int tipoArchivoEmpresaId =
                        archivoEmpresa.TipoArchivoId.Value;

                    int? tipoComplianceId =
                        MapeoDocumentalEmpresasCompliance
                            .ObtenerTipoCompliance(
                                tipoArchivoEmpresaId
                            );

                    /*
                     * No existe equivalencia.
                     *
                     * Ejemplos:
                     * Logo
                     * KEY
                     * RFC
                     * INE, etc.
                     *
                     * Se ignoran completamente.
                     */
                    if (!tipoComplianceId.HasValue)
                    {
                        ignorados++;
                        continue;
                    }

                    /*
                     * Verificar que el catálogo real de Compliance
                     * sigue conteniendo ese tipo.
                     */
                    if (!tiposCompliance.TryGetValue(
                    tipoComplianceId.Value,
                    out EbTipoDocumento?
                        tipoDocumento))
                    {
                        ignorados++;

                        _logger.LogWarning(
                            "No existe el tipo documental " +
                            "Compliance {TipoDocumentoId} " +
                            "para TipoArchivoId {TipoArchivoId}.",
                            tipoComplianceId.Value,
                            tipoArchivoEmpresaId
                        );

                        continue;
                    }

                    /*
                     * Validar contenido.
                     */
                    if (
                        archivoEmpresa.Archivo == null ||
                        archivoEmpresa.Archivo.Length == 0
                    )
                    {
                        ignorados++;

                        continue;
                    }

                    /*
                     * =============================================
                     * CONTROL DE TIPOS NO MÚLTIPLES
                     * =============================================
                     */
                    if (!tipoDocumento.PermiteMultiplesArchivos)
                    {
                        if (!tiposComplianceUnicosProcesados.Add(
                                tipoComplianceId.Value))
                        {
                            ignorados++;

                            _logger.LogWarning(
                                "Se encontró más de un archivo de Empresas " +
                                "para el tipo único de Compliance " +
                                "{TipoDocumentoId}. " +
                                "El archivo {ArchivoEmpresaId} fue ignorado.",
                                tipoComplianceId.Value,
                                archivoEmpresa.Id
                            );

                            continue;
                        }
                    }

                    /*
                     * =============================================
                     * HASH DEL ARCHIVO DE EMPRESAS
                     * =============================================
                     */
                    string hashEmpresa =
                        CalcularSha256(
                            archivoEmpresa.Archivo
                        );

                    /*
                     * =============================================
                     * DOCUMENTOS YA EXISTENTES EN COMPLIANCE
                     * =============================================
                     */

                    List<EbDocumento>
                        documentosCompliance =
                            await _context.EbDocumentos
                                .IgnoreQueryFilters()
                                .Where(x =>
                                    x.EmpresaId ==
                                        complianceId &&
                                    x.TipoDocumentoId ==
                                        tipoComplianceId.Value &&
                                    !x.Eliminado
                                )
                                .OrderByDescending(x =>
                                    x.Version
                                )
                                .ToListAsync(
                                    cancellationToken
                                );

                    /*
                     * =============================================
                     * COMPROBAR SI YA EXISTE EL MISMO ARCHIVO
                     * =============================================
                     *
                     * No usamos el nombre.
                     *
                     * Comparamos el SHA-256 real del contenido.
                     */

                    /*
 * =============================================
 * COMPROBAR SI YA EXISTE EL MISMO ARCHIVO
 * =============================================
 *
 * IMPORTANTE:
 * Comparamos únicamente contra documentos que
 * actualmente están vigentes/activos.
 *
 * No usamos versiones históricas para decidir
 * si el documento actual ya está sincronizado.
 */

                    EbDocumento? documentoCoincidente =
                        null;

                    foreach (
                        EbDocumento documentoCompliance
                        in documentosCompliance.Where(x =>
                            x.EsVersionActual &&
                            !x.Eliminado
                        ))
                    {
                        string rutaDocumento =
                            ObtenerRutaFisicaSegura(
                                rutaBaseDocumentos,
                                documentoCompliance
                                    .RutaArchivo
                            );

                        if (!File.Exists(
                            rutaDocumento))
                        {
                            continue;
                        }

                        string hashCompliance =
                            await CalcularSha256ArchivoAsync(
                                rutaDocumento,
                                cancellationToken
                            );

                        if (string.Equals(
                            hashEmpresa,
                            hashCompliance,
                            StringComparison.OrdinalIgnoreCase
                        ))
                        {
                            documentoCoincidente =
                                documentoCompliance;

                            break;
                        }
                    }

                    /*
                     * =============================================
                     * DOCUMENTO YA EXISTE EN COMPLIANCE
                     * =============================================
                     *
                     * No creamos una copia ni una nueva versión.
                     *
                     * Pero verificamos que exista el vínculo en
                     * EB_DocumentosVinculosEmpresa.
                     */
                    if (documentoCoincidente != null)
                    {
                        string archivoEmpresaId =
                            archivoEmpresa.Id.ToString();

                        bool vinculoYaExiste =
                            await _context
                                .EbDocumentosVinculosEmpresa
                                .AsNoTracking()
                                .AnyAsync(
                                    x =>
                                        x.EmpresaMaestraId ==
                                            empresaMaestraId &&

                                        x.EmpresaComplianceId ==
                                            complianceId &&

                                        x.TipoArchivoEmpresaId ==
                                            tipoArchivoEmpresaId &&

                                        x.TipoDocumentoComplianceId ==
                                            tipoComplianceId.Value &&

                                        x.DocumentoComplianceId ==
                                            documentoCoincidente.Id &&

                                        x.HashContenido ==
                                            hashEmpresa &&

                                        x.Activo,
                                    cancellationToken
                                );

                        /*
                         * El documento existe pero todavía no
                         * estaba registrado en nuestra capa
                         * de integración.
                         */
                        if (!vinculoYaExiste)
                        {
                            EbDocumentoVinculoEmpresa
                                vinculoExistente =
                                    new()
                                    {
                                        EmpresaMaestraId =
                                            empresaMaestraId,

                                        EmpresaComplianceId =
                                            complianceId,

                                        TipoArchivoEmpresaId =
                                            tipoArchivoEmpresaId,

                                        TipoDocumentoComplianceId =
                                            tipoComplianceId.Value,

                                        ArchivoEmpresaId =
                                            archivoEmpresaId,

                                        DocumentoComplianceId =
                                            documentoCoincidente.Id,

                                        HashContenido =
                                            hashEmpresa,

                                        Origen =
                                            "Empresas",

                                        Activo =
                                            true,

                                        FechaCreacion =
                                            DateTime.Now,

                                        FechaActualizacion =
                                            null
                                    };

                            _context
                                .EbDocumentosVinculosEmpresa
                                .Add(
                                    vinculoExistente
                                );

                            vinculosExistentesCreados++;
                        }

                        sinCambios++;

                        continue;
                    }

                    /*
                     * =============================================
                     * CALCULAR NUEVA VERSIÓN
                     * =============================================
                     */

                    int nuevaVersion =
                        documentosCompliance.Any()
                            ? documentosCompliance
                                .Max(x =>
                                    x.Version) + 1
                            : 1;

                    /*
                     * =============================================
                     * VERSIONADO
                     * =============================================
                     *
                     * Si el tipo NO permite múltiples archivos,
                     * la nueva importación se convierte en la
                     * versión actual.
                     *
                     * Si permite múltiples archivos, dejamos
                     * los demás documentos actuales intactos.
                     */

                    if (!tipoDocumento
                        .PermiteMultiplesArchivos)
                    {
                        foreach (
                            EbDocumento documentoAnterior
                            in documentosCompliance.Where(x =>
                                x.EsVersionActual &&
                                !x.Eliminado
                            ))
                        {
                            documentoAnterior.EsVersionActual =
                                false;
                        }
                    }

                    /*
                     * =============================================
                     * EXTENSIÓN
                     * =============================================
                     */

                    string extension =
                        NormalizarExtension(
                            archivoEmpresa.Extension
                        );

                    /*
                     * Si por alguna razón Extension estuviera vacía,
                     * intentar recuperarla desde Nombre.
                     */
                    if (string.IsNullOrWhiteSpace(
                        extension))
                    {
                        extension =
                            NormalizarExtension(
                                Path.GetExtension(
                                    archivoEmpresa.Nombre ??
                                    string.Empty
                                )
                            );
                    }

                    /*
                     * =============================================
                     * NOMBRE ORIGINAL
                     * =============================================
                     */

                    string nombreOriginal =
                        Path.GetFileName(
                            archivoEmpresa.Nombre ??
                            string.Empty
                        );

                    if (string.IsNullOrWhiteSpace(
                        nombreOriginal))
                    {
                        nombreOriginal =
                            $"Documento.{extension}";
                    }

                    /*
                     * =============================================
                     * NOMBRE FÍSICO ÚNICO
                     * =============================================
                     */

                    string nombreAlmacenado =
                        string.IsNullOrWhiteSpace(
                            extension)
                            ? $"{Guid.NewGuid():N}"
                            : $"{Guid.NewGuid():N}.{extension}";

                    /*
                     * =============================================
                     * CARPETA COMPLIANCE
                     * =============================================
                     */

                    string directorioFisico =
                        Path.Combine(
                            rutaBaseDocumentos,
                            complianceId.ToString(),
                            tipoComplianceId.Value
                                .ToString()
                        );

                    Directory.CreateDirectory(
                        directorioFisico
                    );

                    string rutaFisica =
                        Path.Combine(
                            directorioFisico,
                            nombreAlmacenado
                        );

                    /*
                     * =============================================
                     * ESCRIBIR ARCHIVO
                     * =============================================
                     */

                    await File.WriteAllBytesAsync(
                        rutaFisica,
                        archivoEmpresa.Archivo,
                        cancellationToken
                    );

                    archivosFisicosCreados.Add(
                        rutaFisica
                    );

                    /*
                     * Ruta relativa compatible con el sistema
                     * actual de Compliance.
                     */
                    string rutaRelativa =
                        Path.Combine(
                            complianceId.ToString(),
                            tipoComplianceId.Value
                                .ToString(),
                            nombreAlmacenado
                        )
                        .Replace(
                            "\\",
                            "/"
                        );

                    /*
                     * =============================================
                     * CREAR EbDocumento
                     * =============================================
                     *
                     * No inventamos fecha de vencimiento porque
                     * Empresas actualmente no proporciona ese dato.
                     */

                    EbDocumento nuevoDocumento =
                        new()
                        {
                            EmpresaId =
                                complianceId,

                            TipoDocumentoId =
                                tipoComplianceId.Value,

                            NombreOriginal =
                                nombreOriginal,

                            NombreAlmacenado =
                                nombreAlmacenado,

                            RutaArchivo =
                                rutaRelativa,

                            Extension =
                                extension,

                            MimeType =
                                ObtenerMimeType(
                                    extension
                                ),

                            TamanoBytes =
                                archivoEmpresa
                                    .Archivo
                                    .LongLength,

                            Version =
                                nuevaVersion,

                            FechaCarga =
                                DateTime.Now,

                            /*
                             * Empresas no tiene este dato.
                             * No inventamos vencimientos.
                             */
                            FechaVencimiento =
                                null,

                            Estado =
                                "Cargado",

                            Observaciones =
                                "Sincronizado desde el módulo Empresas.",

                            EsVersionActual =
                                true,

                            Eliminado =
                                false,

                            UsuarioCargaId =
                                usuarioId
                        };

                    _context.EbDocumentos.Add(
                        nuevoDocumento
                    );

                    EbDocumentoVinculoEmpresa nuevoVinculo =
                        new()
                        {
                            EmpresaMaestraId =
                                empresaMaestraId,

                            EmpresaComplianceId =
                                complianceId,

                            TipoArchivoEmpresaId =
                                tipoArchivoEmpresaId,

                            TipoDocumentoComplianceId =
                                tipoComplianceId.Value,

                            ArchivoEmpresaId =
                                archivoEmpresa.Id.ToString(),

                            DocumentoComplianceId =
                                null,

                            HashContenido =
                                hashEmpresa,

                            Origen =
                                "Empresas",

                            Activo =
                                true,

                            FechaCreacion =
                                DateTime.Now
                        };

                    vinculosPendientes.Add(
                        (
                            nuevoDocumento,
                            nuevoVinculo
                        )
                    );

                    sincronizados++;
                }

                /*
                 * =================================================
                 * GUARDAR EN UNA SOLA OPERACIÓN
                 * =================================================
                 */

                if (
                    sincronizados > 0 ||
                    vinculosExistentesCreados > 0
                )
                {
                    /*
                     * Primer guardado:
                     *
                     * - genera los Id de los nuevos EbDocumento
                     * - persiste vínculos de documentos que
                     *   ya existían previamente en Compliance
                     */
                    await _context.SaveChangesAsync(
                        cancellationToken
                    );

                    /*
                     * Para documentos recién creados,
                     * ahora ya tenemos Documento.Id.
                     */
                    foreach (
                        (
                            EbDocumento Documento,
                            EbDocumentoVinculoEmpresa Vinculo
                        ) pendiente
                        in vinculosPendientes)
                    {
                        pendiente.Vinculo.DocumentoComplianceId =
                            pendiente.Documento.Id;

                        _context.EbDocumentosVinculosEmpresa.Add(
                            pendiente.Vinculo
                        );
                    }

                    /*
                     * Segundo guardado:
                     * persiste los vínculos correspondientes
                     * a documentos nuevos.
                     */
                    if (vinculosPendientes.Count > 0)
                    {
                        await _context.SaveChangesAsync(
                            cancellationToken
                        );
                    }
                }

                return new ResultadoSincronizacionDocumental
                {
                    DocumentosRevisados =
                        revisados,

                    DocumentosSincronizados =
                        sincronizados,

                    DocumentosSinCambios =
                        sinCambios,

                    DocumentosIgnorados =
                        ignorados
                };
            }
            catch
            {
                /*
                 * =================================================
                 * COMPENSACIÓN DE ARCHIVOS FÍSICOS
                 * =================================================
                 *
                 * Si la BD falla, no dejamos archivos huérfanos
                 * creados por esta operación.
                 */

                foreach (
                    string archivoFisico
                    in archivosFisicosCreados)
                {
                    try
                    {
                        if (File.Exists(
                            archivoFisico))
                        {
                            File.Delete(
                                archivoFisico
                            );
                        }
                    }
                    catch (
                        Exception errorEliminacion)
                    {
                        _logger.LogError(
                            errorEliminacion,
                            "No fue posible eliminar el archivo " +
                            "físico incompleto {Archivo}.",
                            archivoFisico
                        );
                    }
                }

                throw;
            }
        }

        public async Task SincronizarDesdeComplianceAsync(
    int documentoComplianceId,
    CancellationToken cancellationToken = default)
        {
            if (documentoComplianceId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(documentoComplianceId)
                );
            }

            /*
             * ==========================================================
             * OBTENER DOCUMENTO DE COMPLIANCE
             * ==========================================================
             */
            EbDocumento? documentoCompliance =
                await _context.EbDocumentos
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id == documentoComplianceId &&
                            !x.Eliminado &&
                            x.EsVersionActual,
                        cancellationToken
                    );

            if (documentoCompliance == null)
            {
                return;
            }

            /*
             * ==========================================================
             * VALIDAR MAPEO COMPLIANCE → EMPRESAS
             * ==========================================================
             *
             * Si el tipo documental no tiene equivalencia,
             * permanece únicamente en Compliance.
             *
             * Ejemplos:
             * - INE de accionistas
             * - Opinión SAT
             * - Prueba de vida
             * - Poder notarial
             */
            int? tipoArchivoEmpresaId =
                MapeoDocumentalEmpresasCompliance
                    .ObtenerTipoEmpresa(
                        documentoCompliance.TipoDocumentoId
                    );

            if (!tipoArchivoEmpresaId.HasValue)
            {
                return;
            }

            /*
             * ==========================================================
             * OBTENER EMPRESA DE COMPLIANCE
             * ==========================================================
             */
            EbEmpresa? empresaCompliance =
                await _context.EbEmpresas
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id == documentoCompliance.EmpresaId &&
                            !x.Eliminado,
                        cancellationToken
                    );

            if (empresaCompliance == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(
                empresaCompliance.Rfc))
            {
                return;
            }

            /*
             * ==========================================================
             * LOCALIZAR EMPRESA MAESTRA POR RFC
             * ==========================================================
             */
            string rfcNormalizado =
                empresaCompliance.Rfc
                    .Trim()
                    .ToUpperInvariant();

            Empresa? empresaMaestra =
                await _context.Set<Empresa>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.RFC != null &&
                            x.RFC.Trim().ToUpper() ==
                                rfcNormalizado,
                        cancellationToken
                    );

            if (empresaMaestra == null)
            {
                return;
            }

            /*
             * ==========================================================
             * OBTENER RUTA DOCUMENTAL DE COMPLIANCE
             * ==========================================================
             */
            string? rutaBaseDocumentos =
                _configuration[
                    "ExpedientesBancarios:RutaDocumentos"
                ];

            if (string.IsNullOrWhiteSpace(
                rutaBaseDocumentos))
            {
                throw new InvalidOperationException(
                    "No se encuentra configurada la ruta " +
                    "documental de Compliance."
                );
            }

            rutaBaseDocumentos =
                Path.GetFullPath(
                    rutaBaseDocumentos
                );

            string rutaFisica =
                ObtenerRutaFisicaSegura(
                    rutaBaseDocumentos,
                    documentoCompliance.RutaArchivo
                );

            if (!File.Exists(
                rutaFisica))
            {
                throw new FileNotFoundException(
                    "No se encontró el archivo físico " +
                    "del documento de Compliance.",
                    rutaFisica
                );
            }

            /*
             * ==========================================================
             * LEER CONTENIDO
             * ==========================================================
             */
            byte[] contenido =
                await File.ReadAllBytesAsync(
                    rutaFisica,
                    cancellationToken
                );

            if (contenido.Length == 0)
            {
                return;
            }

            /*
             * ==========================================================
             * CALCULAR SHA-256
             * ==========================================================
             */
            string hashCompliance =
                CalcularSha256(
                    contenido
                );

            /*
             * ==========================================================
             * BUSCAR ARCHIVO EQUIVALENTE EN EMPRESAS
             * ==========================================================
             *
             * Para los tipos que estamos sincronizando de regreso,
             * manejamos un ArchivoEmpresa principal por tipo.
             *
             * INE NO entra aquí porque no tiene mapeo inverso.
             */
            ArchivoEmpresa? archivoEmpresa =
                await _context.Set<ArchivoEmpresa>()
                    .FirstOrDefaultAsync(
                        x =>
                            x.EmpresaId ==
                                empresaMaestra.Id &&
                            x.TipoArchivoId ==
                                tipoArchivoEmpresaId.Value,
                        cancellationToken
                    );

            /*
             * ==========================================================
             * SI YA EXISTE EL MISMO CONTENIDO
             * ==========================================================
             */
            if (
                archivoEmpresa != null &&
                archivoEmpresa.Archivo != null &&
                archivoEmpresa.Archivo.Length > 0
            )
            {
                string hashEmpresa =
                    CalcularSha256(
                        archivoEmpresa.Archivo
                    );

                if (string.Equals(
                    hashEmpresa,
                    hashCompliance,
                    StringComparison.OrdinalIgnoreCase
                ))
                {
                    /*
                     * El documento ya está sincronizado.
                     *
                     * Solo aseguramos que exista el vínculo.
                     */
                    await RegistrarOActualizarVinculoDesdeComplianceAsync(
                        empresaMaestra.Id,
                        empresaCompliance.Id,
                        tipoArchivoEmpresaId.Value,
                        documentoCompliance,
                        archivoEmpresa,
                        hashCompliance,
                        cancellationToken
                    );

                    return;
                }
            }

            /*
             * ==========================================================
             * ACTUALIZAR ARCHIVO EXISTENTE
             * ==========================================================
             */
            if (archivoEmpresa != null)
            {
                ArchivoEmpresa archivoActualizado =
                    new ArchivoEmpresa
                    {
                        Id =
                            archivoEmpresa.Id,

                        EmpresaId =
                            empresaMaestra.Id,

                        TipoArchivoId =
                            tipoArchivoEmpresaId.Value,

                        Archivo =
                            contenido,

                        Nombre =
                            documentoCompliance.NombreOriginal,

                        Extension =
                            NormalizarExtension(
                                documentoCompliance.Extension
                            )
                    };

                await _archivoEmpresaManager.UpdateAsync(
                    archivoActualizado
                );

                archivoEmpresa =
                    archivoActualizado;
            }
            else
            {
                archivoEmpresa =
                    new ArchivoEmpresa
                    {
                        Id = string.Empty,

                        EmpresaId =
                            empresaMaestra.Id,

                        TipoArchivoId =
                            tipoArchivoEmpresaId.Value,

                        Archivo =
                            contenido,

                        Nombre =
                            documentoCompliance.NombreOriginal,

                        Extension =
                            NormalizarExtension(
                                documentoCompliance.Extension
                            )
                    };

                /*
                 * El manager genera el Guid correspondiente
                 * para ArchivoEmpresa.Id.
                 */
                await _archivoEmpresaManager.CreateAsync(
                    archivoEmpresa
                );
            }

            /*
             * ==========================================================
             * REGISTRAR / ACTUALIZAR VÍNCULO
             * ==========================================================
             */
            await RegistrarOActualizarVinculoDesdeComplianceAsync(
                empresaMaestra.Id,
                empresaCompliance.Id,
                tipoArchivoEmpresaId.Value,
                documentoCompliance,
                archivoEmpresa,
                hashCompliance,
                cancellationToken
            );
        }

        /*
         * =========================================================
         * SHA-256 DE BYTE[]
         * =========================================================
         */

        private async Task RegistrarOActualizarVinculoDesdeComplianceAsync(
            int empresaMaestraId,
            int empresaComplianceId,
            int tipoArchivoEmpresaId,
            EbDocumento documentoCompliance,
            ArchivoEmpresa archivoEmpresa,
            string hashContenido,
            CancellationToken cancellationToken)
        {
            EbDocumentoVinculoEmpresa? vinculoExistente =
                await _context.EbDocumentosVinculosEmpresa
                    .FirstOrDefaultAsync(
                        x =>
                            x.EmpresaMaestraId == empresaMaestraId &&
                            x.EmpresaComplianceId == empresaComplianceId &&
                            x.TipoArchivoEmpresaId == tipoArchivoEmpresaId &&
                            x.TipoDocumentoComplianceId ==
                                documentoCompliance.TipoDocumentoId &&
                            x.Activo,
                        cancellationToken
                    );

            if (vinculoExistente == null)
            {
                EbDocumentoVinculoEmpresa nuevoVinculo =
                    new()
                    {
                        EmpresaMaestraId =
                            empresaMaestraId,

                        EmpresaComplianceId =
                            empresaComplianceId,

                        TipoArchivoEmpresaId =
                            tipoArchivoEmpresaId,

                        TipoDocumentoComplianceId =
                            documentoCompliance.TipoDocumentoId,

                        ArchivoEmpresaId =
                            archivoEmpresa.Id.ToString(),

                        DocumentoComplianceId =
                            documentoCompliance.Id,

                        HashContenido =
                            hashContenido,

                        Origen =
                            "Compliance",

                        Activo =
                            true,

                        FechaCreacion =
                            DateTime.Now,

                        FechaActualizacion =
                            null
                    };

                _context.EbDocumentosVinculosEmpresa.Add(
                    nuevoVinculo
                );
            }
            else
            {
                vinculoExistente.ArchivoEmpresaId =
                    archivoEmpresa.Id.ToString();

                vinculoExistente.DocumentoComplianceId =
                    documentoCompliance.Id;

                vinculoExistente.HashContenido =
                    hashContenido;

                vinculoExistente.Origen =
                    "Compliance";

                vinculoExistente.Activo =
                    true;

                vinculoExistente.FechaActualizacion =
                    DateTime.Now;
            }

            await _context.SaveChangesAsync(
                cancellationToken
            );
        }

        private static string CalcularSha256(
            byte[] contenido)
        {
            byte[] hash =
                SHA256.HashData(
                    contenido
                );

            return Convert.ToHexString(
                hash
            );
        }

        /*
         * =========================================================
         * SHA-256 DE ARCHIVO FÍSICO
         * =========================================================
         */

        private static async Task<string>
            CalcularSha256ArchivoAsync(
                string rutaArchivo,
                CancellationToken cancellationToken)
        {
            await using FileStream stream =
                new(
                    rutaArchivo,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: 81920,
                    useAsync: true
                );

            using SHA256 sha256 =
                SHA256.Create();

            byte[] hash =
                await sha256.ComputeHashAsync(
                    stream,
                    cancellationToken
                );

            return Convert.ToHexString(
                hash
            );
        }

        /*
         * =========================================================
         * SEGURIDAD DE RUTA
         * =========================================================
         */

        private static string ObtenerRutaFisicaSegura(
            string rutaBase,
            string rutaRelativa)
        {
            string baseCompleta =
                Path.GetFullPath(
                    rutaBase
                );

            string rutaCompleta =
                Path.GetFullPath(
                    Path.Combine(
                        baseCompleta,
                        rutaRelativa.Replace(
                            "/",
                            Path.DirectorySeparatorChar
                                .ToString()
                        )
                    )
                );

            string baseConSeparador =
                baseCompleta.TrimEnd(
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar
                ) +
                Path.DirectorySeparatorChar;

            if (!rutaCompleta.StartsWith(
                baseConSeparador,
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "La ruta documental calculada no es válida."
                );
            }

            return rutaCompleta;
        }

        /*
         * =========================================================
         * EXTENSIÓN
         * =========================================================
         */

        private static string NormalizarExtension(
            string? extension)
        {
            return (
                extension ??
                string.Empty
            )
            .Trim()
            .TrimStart('.')
            .ToLowerInvariant();
        }

        /*
         * =========================================================
         * MIME TYPE
         * =========================================================
         */

        private static string ObtenerMimeType(
            string extension)
        {
            return extension.ToLowerInvariant()
                switch
            {
                "pdf" =>
                    "application/pdf",

                "jpg" or "jpeg" =>
                    "image/jpeg",

                "png" =>
                    "image/png",

                "webp" =>
                    "image/webp",

                "cer" =>
                    "application/pkix-cert",

                _ =>
                    "application/octet-stream"
            };
        }
    }
}