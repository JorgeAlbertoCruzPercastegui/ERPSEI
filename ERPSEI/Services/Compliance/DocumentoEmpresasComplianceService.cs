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
        CancellationToken cancellationToken = default)
        {
            /*
             * ==========================================================
             * VALIDACIONES
             * ==========================================================
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
             * ==========================================================
             * VALIDAR EMPRESA MAESTRA
             * ==========================================================
             */
            bool empresaExiste =
                await _context.Set<Empresa>()
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.Id == empresaMaestraId,
                        cancellationToken
                    );

            if (!empresaExiste)
            {
                throw new InvalidOperationException(
                    "No se encontró la empresa maestra."
                );
            }

            /*
             * ==========================================================
             * VALIDAR EMPRESA COMPLIANCE
             * ==========================================================
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
             * ==========================================================
             * RUTA DOCUMENTAL
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
                    "de almacenamiento documental de Compliance."
                );
            }

            rutaBaseDocumentos =
                Path.GetFullPath(
                    rutaBaseDocumentos
                );

            /*
             * ==========================================================
             * DOCUMENTOS DEL MÓDULO EMPRESAS
             * ==========================================================
             *
             * Únicamente se consultan.
             * Este método NO modifica ArchivoEmpresa.
             */
            List<ArchivoEmpresa> archivosEmpresa =
                await _context.Set<ArchivoEmpresa>()
                    .AsNoTracking()
                    .Where(x =>
                        x.EmpresaId == empresaMaestraId
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

            /*
             * ==========================================================
             * CATÁLOGO COMPLIANCE
             * ==========================================================
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

            int revisados =
                archivosEmpresa.Count;

            int sincronizados = 0;
            int sinCambios = 0;
            int ignorados = 0;
            int cambiosVinculos = 0;

            /*
             * ==========================================================
             * PREPARAR CANDIDATOS MAPEADOS
             * ==========================================================
             */
            List<(
                ArchivoEmpresa Archivo,
                int TipoArchivoEmpresaId,
                int TipoComplianceId,
                EbTipoDocumento TipoDocumento
            )> candidatos = new();

            foreach (
                ArchivoEmpresa archivoEmpresa
                in archivosEmpresa)
            {
                if (!archivoEmpresa.TipoArchivoId.HasValue)
                {
                    ignorados++;

                    continue;
                }

                /*
                 * Un archivo sin contenido no puede
                 * sincronizarse.
                 */
                if (
                    archivoEmpresa.Archivo == null ||
                    archivoEmpresa.Archivo.Length == 0)
                {
                    ignorados++;

                    continue;
                }

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
                 * - Logo
                 * - RFC
                 * - KEY
                 * - INE2
                 * - INE3
                 */
                if (!tipoComplianceId.HasValue)
                {
                    ignorados++;

                    continue;
                }

                if (!tiposCompliance.TryGetValue(
                    tipoComplianceId.Value,
                    out EbTipoDocumento? tipoDocumento))
                {
                    ignorados++;

                    _logger.LogWarning(
                        "No existe el tipo documental Compliance " +
                        "{TipoDocumentoId} para TipoArchivoId " +
                        "{TipoArchivoId}.",
                        tipoComplianceId.Value,
                        tipoArchivoEmpresaId
                    );

                    continue;
                }

                candidatos.Add(
                    (
                        archivoEmpresa,
                        tipoArchivoEmpresaId,
                        tipoComplianceId.Value,
                        tipoDocumento
                    )
                );
            }

            /*
             * ==========================================================
             * ARCHIVOS FÍSICOS CREADOS
             * ==========================================================
             */
            List<string> archivosFisicosCreados =
                new();

            /*
             * Los nuevos vínculos deben esperar hasta que
             * EbDocumento tenga Id.
             */
            List<(
                EbDocumento Documento,
                EbDocumentoVinculoEmpresa Vinculo
            )> vinculosPendientes =
                new();

            try
            {
                /*
                 * ======================================================
                 * PROCESAR POR TIPO DE COMPLIANCE
                 * ======================================================
                 */
                foreach (
                    IGrouping<
                        int,
                        (
                            ArchivoEmpresa Archivo,
                            int TipoArchivoEmpresaId,
                            int TipoComplianceId,
                            EbTipoDocumento TipoDocumento
                        )
                    > grupo
                    in candidatos.GroupBy(x =>
                        x.TipoComplianceId))
                {
                    List<(
                        ArchivoEmpresa Archivo,
                        int TipoArchivoEmpresaId,
                        int TipoComplianceId,
                        EbTipoDocumento TipoDocumento
                    )> candidatosTipo =
                        grupo.ToList();

                    if (candidatosTipo.Count == 0)
                    {
                        continue;
                    }

                    EbTipoDocumento tipoDocumento =
                        candidatosTipo[0].TipoDocumento;

                    int tipoComplianceId =
                        candidatosTipo[0].TipoComplianceId;

                    /*
                     * ==================================================
                     * DOCUMENTOS EXISTENTES DE COMPLIANCE
                     * ==================================================
                     */
                    List<EbDocumento> documentosCompliance =
                        await _context.EbDocumentos
                            .IgnoreQueryFilters()
                            .Where(x =>
                                x.EmpresaId ==
                                    complianceId &&
                                x.TipoDocumentoId ==
                                    tipoComplianceId &&
                                !x.Eliminado
                            )
                            .OrderByDescending(x =>
                                x.Version
                            )
                            .ToListAsync(
                                cancellationToken
                            );

                    /*
                     * ==================================================
                     * DEFINIR QUÉ ARCHIVOS DE EMPRESAS PROCESAR
                     * ==================================================
                     */
                    List<(
                        ArchivoEmpresa Archivo,
                        int TipoArchivoEmpresaId,
                        int TipoComplianceId,
                        EbTipoDocumento TipoDocumento
                    )> archivosAProcesar =
                        new();

                    /*
                     * ==================================================
                     * TIPO ÚNICO
                     * ==================================================
                     *
                     * Aquí está la corrección principal.
                     *
                     * Ya NO tomamos simplemente el primer
                     * ArchivoEmpresa del tipo.
                     *
                     * Primero intentamos recuperar el ArchivoEmpresa
                     * relacionado con el documento ACTUAL de Compliance.
                     */
                    if (!tipoDocumento.PermiteMultiplesArchivos)
                    {
                        EbDocumento? documentoActual =
                            documentosCompliance
                                .Where(x =>
                                    x.EsVersionActual &&
                                    !x.Eliminado
                                )
                                .OrderByDescending(x =>
                                    x.Version
                                )
                                .FirstOrDefault();

                        (
                            ArchivoEmpresa Archivo,
                            int TipoArchivoEmpresaId,
                            int TipoComplianceId,
                            EbTipoDocumento TipoDocumento
                        )? candidatoSeleccionado =
                            null;

                        /*
                         * ==============================================
                         * 1. BUSCAR VÍNCULO ACTIVO DEL DOCUMENTO ACTUAL
                         * ==============================================
                         */
                        if (documentoActual != null)
                        {
                            EbDocumentoVinculoEmpresa?
                                vinculoActual =
                                    await _context
                                        .EbDocumentosVinculosEmpresa
                                        .AsNoTracking()
                                        .Where(x =>
                                            x.EmpresaMaestraId ==
                                                empresaMaestraId &&

                                            x.EmpresaComplianceId ==
                                                complianceId &&

                                            x.TipoDocumentoComplianceId ==
                                                tipoComplianceId &&

                                            x.DocumentoComplianceId ==
                                                documentoActual.Id &&

                                            x.Activo
                                        )
                                        .OrderByDescending(x =>
                                            x.FechaActualizacion ??
                                            x.FechaCreacion
                                        )
                                        .FirstOrDefaultAsync(
                                            cancellationToken
                                        );

                            /*
                             * El vínculo nos dice exactamente
                             * qué ArchivoEmpresa representa al
                             * documento actual.
                             */
                            if (
                                vinculoActual != null &&
                                !string.IsNullOrWhiteSpace(
                                    vinculoActual.ArchivoEmpresaId))
                            {
                                candidatoSeleccionado =
                                    candidatosTipo
                                        .Where(x =>
                                            string.Equals(
                                                x.Archivo.Id,
                                                vinculoActual
                                                    .ArchivoEmpresaId,
                                                StringComparison
                                                    .OrdinalIgnoreCase
                                            )
                                        )
                                        .Cast<(
                                            ArchivoEmpresa Archivo,
                                            int TipoArchivoEmpresaId,
                                            int TipoComplianceId,
                                            EbTipoDocumento TipoDocumento
                                        )?>()
                                        .FirstOrDefault();
                            }
                        }

                        /*
                         * ==============================================
                         * 2. SI NO HAY VÍNCULO, BUSCAR POR SHA
                         * ==============================================
                         *
                         * Esto permite recuperar correctamente relaciones
                         * antiguas que todavía no tienen vínculo.
                         */
                        if (
                            candidatoSeleccionado == null &&
                            documentoActual != null)
                        {
                            string rutaDocumentoActual =
                                ObtenerRutaFisicaSegura(
                                    rutaBaseDocumentos,
                                    documentoActual.RutaArchivo
                                );

                            if (File.Exists(
                                rutaDocumentoActual))
                            {
                                string hashDocumentoActual =
                                    await CalcularSha256ArchivoAsync(
                                        rutaDocumentoActual,
                                        cancellationToken
                                    );

                                foreach (
                                    var candidato
                                    in candidatosTipo)
                                {
                                    string hashCandidato =
                                        CalcularSha256(
                                            candidato
                                                .Archivo
                                                .Archivo
                                        );

                                    if (string.Equals(
                                        hashCandidato,
                                        hashDocumentoActual,
                                        StringComparison
                                            .OrdinalIgnoreCase))
                                    {
                                        candidatoSeleccionado =
                                            candidato;

                                        break;
                                    }
                                }
                            }
                        }

                        /*
                         * ==============================================
                         * 3. SI EXISTE UN SOLO ARCHIVO, ES SEGURO
                         * ==============================================
                         */
                        if (
                            candidatoSeleccionado == null &&
                            candidatosTipo.Count == 1)
                        {
                            candidatoSeleccionado =
                                candidatosTipo[0];
                        }

                        /*
                         * ==============================================
                         * 4. BUSCAR ALGÚN VÍNCULO ACTIVO CONOCIDO
                         * ==============================================
                         *
                         * Útil para relaciones antiguas donde el
                         * DocumentoComplianceId no estaba correctamente
                         * actualizado.
                         */
                        if (candidatoSeleccionado == null)
                        {
                            List<EbDocumentoVinculoEmpresa>
                                vinculosConocidos =
                                    await _context
                                        .EbDocumentosVinculosEmpresa
                                        .AsNoTracking()
                                        .Where(x =>
                                            x.EmpresaMaestraId ==
                                                empresaMaestraId &&

                                            x.EmpresaComplianceId ==
                                                complianceId &&

                                            x.TipoDocumentoComplianceId ==
                                                tipoComplianceId &&

                                            x.Activo &&

                                            x.ArchivoEmpresaId != null
                                        )
                                        .OrderByDescending(x =>
                                            x.FechaActualizacion ??
                                            x.FechaCreacion
                                        )
                                        .ToListAsync(
                                            cancellationToken
                                        );

                            foreach (
                                EbDocumentoVinculoEmpresa vinculo
                                in vinculosConocidos)
                            {
                                var candidato =
                                    candidatosTipo
                                        .FirstOrDefault(x =>
                                            string.Equals(
                                                x.Archivo.Id,
                                                vinculo.ArchivoEmpresaId,
                                                StringComparison
                                                    .OrdinalIgnoreCase
                                            )
                                        );

                                if (candidato.Archivo != null)
                                {
                                    candidatoSeleccionado =
                                        candidato;

                                    break;
                                }
                            }
                        }

                        /*
                         * ==============================================
                         * 5. AMBIGÜEDAD
                         * ==============================================
                         *
                         * Si existen varios archivos históricos y
                         * ninguno puede identificarse mediante vínculo
                         * ni SHA, NO elegimos uno al azar.
                         *
                         * Es más seguro ignorarlo que crear una
                         * versión incorrecta.
                         */
                        if (candidatoSeleccionado == null)
                        {
                            ignorados +=
                                candidatosTipo.Count;

                            _logger.LogWarning(
                                "No fue posible determinar de forma " +
                                "segura cuál ArchivoEmpresa corresponde " +
                                "al tipo único de Compliance " +
                                "{TipoDocumentoId} para la empresa " +
                                "{EmpresaId}. Se encontraron " +
                                "{CantidadArchivos} candidatos.",
                                tipoComplianceId,
                                empresaMaestraId,
                                candidatosTipo.Count
                            );

                            continue;
                        }

                        archivosAProcesar.Add(
                            candidatoSeleccionado.Value
                        );

                        /*
                         * Los demás son históricos/duplicados del
                         * módulo Empresas y NO deben generar versiones.
                         */
                        ignorados +=
                            candidatosTipo.Count - 1;
                    }
                    else
                    {
                        /*
                         * ==================================================
                         * TIPO MÚLTIPLE
                         * ==================================================
                         *
                         * Todos pueden coexistir.
                         *
                         * Ejemplo:
                         * INE de accionistas.
                         */
                        archivosAProcesar.AddRange(
                            candidatosTipo
                        );
                    }

                    /*
                     * ==================================================
                     * PROCESAR ARCHIVOS SELECCIONADOS
                     * ==================================================
                     */
                    foreach (
                        var candidato
                        in archivosAProcesar)
                    {
                        ArchivoEmpresa archivoEmpresa =
                            candidato.Archivo;

                        int tipoArchivoEmpresaId =
                            candidato.TipoArchivoEmpresaId;

                        string archivoEmpresaId =
                            archivoEmpresa.Id;

                        string hashEmpresa =
                            CalcularSha256(
                                archivoEmpresa.Archivo
                            );

                        /*
                         * ==============================================
                         * BUSCAR SI EL CONTENIDO YA ES ACTUAL
                         * ==============================================
                         */
                        EbDocumento? documentoCoincidente =
                            null;

                        foreach (
                            EbDocumento documentoCompliance
                            in documentosCompliance
                                .Where(x =>
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
                                StringComparison.OrdinalIgnoreCase))
                            {
                                documentoCoincidente =
                                    documentoCompliance;

                                break;
                            }
                        }

                        /*
                         * ==============================================
                         * YA ES EL MISMO DOCUMENTO
                         * ==============================================
                         */
                        if (documentoCoincidente != null)
                        {
                            /*
                             * Para tipos únicos debe quedar solamente
                             * un vínculo activo.
                             */
                            if (!tipoDocumento
                                .PermiteMultiplesArchivos)
                            {
                                List<EbDocumentoVinculoEmpresa>
                                    vinculosAnteriores =
                                        await _context
                                            .EbDocumentosVinculosEmpresa
                                            .Where(x =>
                                                x.EmpresaMaestraId ==
                                                    empresaMaestraId &&

                                                x.EmpresaComplianceId ==
                                                    complianceId &&

                                                x.TipoDocumentoComplianceId ==
                                                    tipoComplianceId &&

                                                x.Activo &&

                                                (
                                                    x.DocumentoComplianceId !=
                                                        documentoCoincidente.Id ||

                                                    x.ArchivoEmpresaId !=
                                                        archivoEmpresaId
                                                )
                                            )
                                            .ToListAsync(
                                                cancellationToken
                                            );

                                foreach (
                                    EbDocumentoVinculoEmpresa
                                        vinculoAnterior
                                    in vinculosAnteriores)
                                {
                                    vinculoAnterior.Activo =
                                        false;

                                    vinculoAnterior
                                        .FechaActualizacion =
                                            DateTime.Now;

                                    cambiosVinculos++;
                                }
                            }

                            /*
                             * Buscar vínculo EXACTO.
                             */
                            EbDocumentoVinculoEmpresa?
                                vinculoExacto =
                                    await _context
                                        .EbDocumentosVinculosEmpresa
                                        .FirstOrDefaultAsync(
                                            x =>
                                                x.EmpresaMaestraId ==
                                                    empresaMaestraId &&

                                                x.EmpresaComplianceId ==
                                                    complianceId &&

                                                x.TipoArchivoEmpresaId ==
                                                    tipoArchivoEmpresaId &&

                                                x.TipoDocumentoComplianceId ==
                                                    tipoComplianceId &&

                                                x.ArchivoEmpresaId ==
                                                    archivoEmpresaId &&

                                                x.DocumentoComplianceId ==
                                                    documentoCoincidente.Id,
                                            cancellationToken
                                        );

                            if (vinculoExacto == null)
                            {
                                EbDocumentoVinculoEmpresa
                                    nuevoVinculo =
                                        new()
                                        {
                                            EmpresaMaestraId =
                                                empresaMaestraId,

                                            EmpresaComplianceId =
                                                complianceId,

                                            TipoArchivoEmpresaId =
                                                tipoArchivoEmpresaId,

                                            TipoDocumentoComplianceId =
                                                tipoComplianceId,

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
                                        nuevoVinculo
                                    );

                                cambiosVinculos++;
                            }
                            else
                            {
                                bool huboCambioVinculo =
                                    !vinculoExacto.Activo ||

                                    !string.Equals(
                                        vinculoExacto
                                            .HashContenido,
                                        hashEmpresa,
                                        StringComparison
                                            .OrdinalIgnoreCase
                                    );

                                vinculoExacto.HashContenido =
                                    hashEmpresa;

                                vinculoExacto.Activo =
                                    true;

                                if (huboCambioVinculo)
                                {
                                    vinculoExacto
                                        .FechaActualizacion =
                                            DateTime.Now;

                                    cambiosVinculos++;
                                }
                            }

                            sinCambios++;

                            continue;
                        }

                        /*
                         * ==============================================
                         * NUEVA VERSIÓN
                         * ==============================================
                         */
                        int nuevaVersion =
                            documentosCompliance.Any()
                                ? documentosCompliance
                                    .Max(x => x.Version) + 1
                                : 1;

                        /*
                         * ==============================================
                         * TIPO ÚNICO:
                         * DESACTIVAR VERSIÓN Y VÍNCULO ANTERIOR
                         * ==============================================
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
                                documentoAnterior
                                    .EsVersionActual =
                                        false;
                            }

                            List<EbDocumentoVinculoEmpresa>
                                vinculosAnteriores =
                                    await _context
                                        .EbDocumentosVinculosEmpresa
                                        .Where(x =>
                                            x.EmpresaMaestraId ==
                                                empresaMaestraId &&

                                            x.EmpresaComplianceId ==
                                                complianceId &&

                                            x.TipoDocumentoComplianceId ==
                                                tipoComplianceId &&

                                            x.Activo
                                        )
                                        .ToListAsync(
                                            cancellationToken
                                        );

                            foreach (
                                EbDocumentoVinculoEmpresa
                                    vinculoAnterior
                                in vinculosAnteriores)
                            {
                                vinculoAnterior.Activo =
                                    false;

                                vinculoAnterior
                                    .FechaActualizacion =
                                        DateTime.Now;

                                cambiosVinculos++;
                            }
                        }

                        /*
                         * ==============================================
                         * EXTENSIÓN
                         * ==============================================
                         */
                        string extension =
                            NormalizarExtension(
                                archivoEmpresa.Extension
                            );

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
                         * ==============================================
                         * NOMBRE ORIGINAL
                         * ==============================================
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
                                string.IsNullOrWhiteSpace(extension)
                                    ? "Documento"
                                    : $"Documento.{extension}";
                        }

                        /*
                         * ==============================================
                         * NOMBRE FÍSICO ÚNICO
                         * ==============================================
                         */
                        string nombreAlmacenado =
                            string.IsNullOrWhiteSpace(extension)
                                ? $"{Guid.NewGuid():N}"
                                : $"{Guid.NewGuid():N}.{extension}";

                        /*
                         * ==============================================
                         * DIRECTORIO
                         * ==============================================
                         */
                        string directorioFisico =
                            Path.Combine(
                                rutaBaseDocumentos,
                                complianceId.ToString(),
                                tipoComplianceId.ToString()
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
                         * ==============================================
                         * ESCRIBIR ARCHIVO
                         * ==============================================
                         */
                        await File.WriteAllBytesAsync(
                            rutaFisica,
                            archivoEmpresa.Archivo,
                            cancellationToken
                        );

                        archivosFisicosCreados.Add(
                            rutaFisica
                        );

                        string rutaRelativa =
                            Path.Combine(
                                complianceId.ToString(),
                                tipoComplianceId.ToString(),
                                nombreAlmacenado
                            )
                            .Replace(
                                "\\",
                                "/"
                            );

                        /*
                         * ==============================================
                         * CREAR EbDocumento
                         * ==============================================
                         */
                        EbDocumento nuevoDocumento =
                            new()
                            {
                                EmpresaId =
                                    complianceId,

                                TipoDocumentoId =
                                    tipoComplianceId,

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
                                 * Empresas no proporciona
                                 * fecha de vencimiento.
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

                        /*
                         * Mantenerlo también en memoria para que
                         * la siguiente evaluación de este tipo conozca
                         * la nueva versión.
                         */
                        documentosCompliance.Add(
                            nuevoDocumento
                        );

                        EbDocumentoVinculoEmpresa
                            nuevoVinculoPendiente =
                                new()
                                {
                                    EmpresaMaestraId =
                                        empresaMaestraId,

                                    EmpresaComplianceId =
                                        complianceId,

                                    TipoArchivoEmpresaId =
                                        tipoArchivoEmpresaId,

                                    TipoDocumentoComplianceId =
                                        tipoComplianceId,

                                    ArchivoEmpresaId =
                                        archivoEmpresaId,

                                    DocumentoComplianceId =
                                        null,

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

                        vinculosPendientes.Add(
                            (
                                nuevoDocumento,
                                nuevoVinculoPendiente
                            )
                        );

                        sincronizados++;
                    }
                }

                /*
                 * ======================================================
                 * PRIMER GUARDADO
                 * ======================================================
                 *
                 * Guarda:
                 * - cambios de EsVersionActual
                 * - vínculos desactivados
                 * - vínculos existentes actualizados
                 * - nuevos EbDocumento
                 */
                if (
                    sincronizados > 0 ||
                    cambiosVinculos > 0)
                {
                    await _context.SaveChangesAsync(
                        cancellationToken
                    );
                }

                /*
                 * ======================================================
                 * VÍNCULOS DE DOCUMENTOS NUEVOS
                 * ======================================================
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

                    _context
                        .EbDocumentosVinculosEmpresa
                        .Add(
                            pendiente.Vinculo
                        );
                }

                if (vinculosPendientes.Count > 0)
                {
                    await _context.SaveChangesAsync(
                        cancellationToken
                    );
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
                 * ======================================================
                 * COMPENSACIÓN DE ARCHIVOS FÍSICOS
                 * ======================================================
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
            /*
             * ==========================================================
             * OBTENER CONFIGURACIÓN DEL TIPO DOCUMENTAL
             * ==========================================================
             */
            EbTipoDocumento? tipoDocumento =
                await _context.EbTiposDocumento
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                documentoCompliance.TipoDocumentoId &&
                            !x.Eliminado,
                        cancellationToken
                    );

            if (tipoDocumento == null)
            {
                throw new InvalidOperationException(
                    "No se encontró el tipo documental de Compliance."
                );
            }

            string archivoEmpresaId =
                archivoEmpresa.Id;

            DateTime fechaActual =
                DateTime.Now;

            /*
             * ==========================================================
             * TIPOS DOCUMENTALES ÚNICOS
             * ==========================================================
             *
             * Acta constitutiva, CSF, FIEL, Hoja membretada,
             * Organigrama, etc.
             *
             * Solamente debe existir UN vínculo activo para la
             * combinación Empresa + Tipo documental.
             *
             * Cuando Compliance reemplaza el documento:
             *
             * vínculo anterior → Activo = false
             * vínculo nuevo    → Activo = true
             * ==========================================================
             */
            if (!tipoDocumento.PermiteMultiplesArchivos)
            {
                List<EbDocumentoVinculoEmpresa>
                    vinculosActivosAnteriores =
                        await _context
                            .EbDocumentosVinculosEmpresa
                            .Where(x =>
                                x.EmpresaMaestraId ==
                                    empresaMaestraId &&

                                x.EmpresaComplianceId ==
                                    empresaComplianceId &&

                                x.TipoArchivoEmpresaId ==
                                    tipoArchivoEmpresaId &&

                                x.TipoDocumentoComplianceId ==
                                    documentoCompliance
                                        .TipoDocumentoId &&

                                x.Activo &&

                                (
                                    x.DocumentoComplianceId !=
                                        documentoCompliance.Id ||

                                    x.ArchivoEmpresaId !=
                                        archivoEmpresaId
                                )
                            )
                            .ToListAsync(
                                cancellationToken
                            );

                foreach (
                    EbDocumentoVinculoEmpresa vinculoAnterior
                    in vinculosActivosAnteriores)
                {
                    vinculoAnterior.Activo =
                        false;

                    vinculoAnterior.FechaActualizacion =
                        fechaActual;
                }
            }

            /*
             * ==========================================================
             * BUSCAR EL VÍNCULO EXACTO
             * ==========================================================
             *
             * Ya no buscamos simplemente cualquier vínculo activo
             * del mismo tipo.
             *
             * Debe corresponder exactamente a:
             *
             * - Empresa
             * - Tipo
             * - ArchivoEmpresa
             * - DocumentoCompliance
             * ==========================================================
             */
            EbDocumentoVinculoEmpresa? vinculoExistente =
                await _context
                    .EbDocumentosVinculosEmpresa
                    .FirstOrDefaultAsync(
                        x =>
                            x.EmpresaMaestraId ==
                                empresaMaestraId &&

                            x.EmpresaComplianceId ==
                                empresaComplianceId &&

                            x.TipoArchivoEmpresaId ==
                                tipoArchivoEmpresaId &&

                            x.TipoDocumentoComplianceId ==
                                documentoCompliance
                                    .TipoDocumentoId &&

                            x.ArchivoEmpresaId ==
                                archivoEmpresaId &&

                            x.DocumentoComplianceId ==
                                documentoCompliance.Id,
                        cancellationToken
                    );

            /*
             * ==========================================================
             * CREAR NUEVO VÍNCULO
             * ==========================================================
             */
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
                            documentoCompliance
                                .TipoDocumentoId,

                        ArchivoEmpresaId =
                            archivoEmpresaId,

                        DocumentoComplianceId =
                            documentoCompliance.Id,

                        HashContenido =
                            hashContenido,

                        Origen =
                            "Compliance",

                        Activo =
                            true,

                        FechaCreacion =
                            fechaActual,

                        FechaActualizacion =
                            null
                    };

                _context
                    .EbDocumentosVinculosEmpresa
                    .Add(
                        nuevoVinculo
                    );
            }
            else
            {
                /*
                 * ======================================================
                 * REACTIVAR / ACTUALIZAR VÍNCULO EXACTO
                 * ======================================================
                 */
                vinculoExistente.ArchivoEmpresaId =
                    archivoEmpresaId;

                vinculoExistente.DocumentoComplianceId =
                    documentoCompliance.Id;

                vinculoExistente.HashContenido =
                    hashContenido;

                vinculoExistente.Origen =
                    "Compliance";

                vinculoExistente.Activo =
                    true;

                vinculoExistente.FechaActualizacion =
                    fechaActual;
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