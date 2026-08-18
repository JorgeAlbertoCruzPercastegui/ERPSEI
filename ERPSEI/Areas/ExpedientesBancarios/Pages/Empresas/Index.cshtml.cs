using ERPSEI.Data;
using ERPSEI.Data.Entities.Empresas;
using ERPSEI.Data.Entities.ExpedientesBancarios;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers.Usuarios;
using ERPSEI.Services.Compliance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ERPSEI.Data.Entities.SAT.Catalogos;
using ERPSEI.Data.Managers.Empresas;

namespace ERPSEI.Areas.ExpedientesBancarios.Pages.Empresas
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly AppUserManager _userManager;
        private readonly IEmpresaManager _empresaManager;
        private readonly IPermisosComplianceService _permisosComplianceService;
        private readonly IDocumentoEmpresasComplianceService _documentoEmpresasComplianceService;

        public IndexModel(
            ApplicationDbContext context,
            IWebHostEnvironment environment,
            IConfiguration configuration,
            AppUserManager userManager,
            IEmpresaManager empresaManager,
            IPermisosComplianceService permisosComplianceService,
            IDocumentoEmpresasComplianceService documentoEmpresasComplianceService)
        {
            _context = context;
            _environment = environment;
            _configuration = configuration;
            _userManager = userManager;
            _empresaManager = empresaManager;
            _permisosComplianceService = permisosComplianceService;
            _documentoEmpresasComplianceService = documentoEmpresasComplianceService;
        }

        public PermisosComplianceResultado
    PermisosCompliance
        {
            get;
            private set;
        } =
    PermisosComplianceResultado.SinAcceso();

        public async Task<IActionResult> OnGetAsync()
        {
            PermisosCompliance =
                await _permisosComplianceService
                    .ObtenerPermisosAsync(User);

            if (!PermisosCompliance.TieneAccesoModulo)
            {
                return Forbid();
            }

            return Page();
        }

        private static string
    ObtenerNombreUsuarioCompliance(
        AppUser usuario)
        {
            /*
             * En esta primera versión usamos el nombre
             * de usuario y después el correo como respaldo.
             */
            if (!string.IsNullOrWhiteSpace(
                    usuario.UserName))
            {
                return usuario.UserName;
            }

            if (!string.IsNullOrWhiteSpace(
                    usuario.Email))
            {
                return usuario.Email;
            }

            return "Usuario sin nombre";
        }

        // =====================================================
        // LISTADO PARA BOOTSTRAP TABLE
        // GET ?handler=Empresas
        // =====================================================
        /*public async Task<IActionResult> OnGetEmpresasAsync(
            string? busqueda,
            string? rfc,
            string? nivel,
            string? estatus)
        {

            if (!await _permisosComplianceService
            .PuedeVisualizarAsync(User))
            {
                return Forbid();
            }

            string filtro = busqueda?.Trim() ?? string.Empty;
            string filtroRfc =
            rfc?.Trim().ToUpperInvariant() ??
            string.Empty;

            string filtroNivel =
                nivel?.Trim() ??
                string.Empty;
            string filtroEstatus = string.IsNullOrWhiteSpace(estatus)
                ? "Activas"
                : estatus.Trim();

            IQueryable<EbEmpresa> query = _context.EbEmpresas
                .AsNoTracking();

            query = filtroEstatus switch
            {
                "Inactivas" => query.Where(x => x.Deshabilitado),
                "Todas" => query,
                _ => query.Where(x => !x.Deshabilitado)
            };

            // Búsqueda general
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(x =>
                    x.RazonSocial.Contains(filtro) ||
                    x.NombreCorto.Contains(filtro) ||
                    (x.ActividadComercial != null &&
                     x.ActividadComercial.Contains(filtro)));
            }

            // Filtro independiente por RFC
            if (!string.IsNullOrWhiteSpace(filtroRfc))
            {
                query = query.Where(x =>
                    x.Rfc.Contains(filtroRfc));
            }

            // Filtro independiente por nivel
            if (!string.IsNullOrWhiteSpace(filtroNivel))
            {
                query = query.Where(x =>
                    x.Nivel != null &&
                    x.Nivel.Contains(filtroNivel));
            }

            var empresas = await query
                .OrderBy(x => x.Id)
                .Select(x => new
                {
                    id = x.Id,
                    razonSocial = x.RazonSocial,
                    nombreCorto = x.NombreCorto,
                    rfc = x.Rfc,
                    nivel = x.Nivel,
                    actividadComercial = x.ActividadComercial,
                    telefonoBancos = x.TelefonoBancos,
                    correoBancos = x.CorreoBancos,
                    fechaConstitucion = x.FechaConstitucion,
                    numeroEscritura = x.NumeroEscritura,
                    domicilioFiscal = x.DomicilioFiscal,
                    observaciones = x.Observaciones,
                    deshabilitado = x.Deshabilitado,
                    fechaCreacion = x.FechaCreacion
                })
                .ToListAsync();

            return new JsonResult(empresas);
        }*/
        public async Task<IActionResult> OnGetEmpresasAsync(
        string? busqueda,
        string? rfc,
        string? nivel,
        string? estatus)
        {
            if (!await _permisosComplianceService
                .PuedeVisualizarAsync(User))
            {
                return Forbid();
            }

            string filtro =
                busqueda?.Trim() ??
                string.Empty;

            string filtroRfc =
                rfc?.Trim().ToUpperInvariant() ??
                string.Empty;

            string filtroNivel =
                nivel?.Trim() ??
                string.Empty;

            string filtroEstatus =
                string.IsNullOrWhiteSpace(estatus)
                    ? "Activas"
                    : estatus.Trim();

            /*
             * ==========================================================
             * EMPRESAS MAESTRAS
             * ==========================================================
             *
             * A partir de este punto la información corporativa se toma
             * directamente del módulo Empresas.
             *
             * EbEmpresa solamente se conserva para recuperar el ID
             * utilizado actualmente por Compliance y no afectar
             * documentos, accionistas, bitácoras ni expedientes.
             * ==========================================================
             */

            var query =
                from empresa in _context.Set<Empresa>()
                    .AsNoTracking()

                join ebEmpresa in _context.EbEmpresas
                    .AsNoTracking()

                    on (empresa.RFC ?? string.Empty)
                        .Trim()
                        .ToUpper()
                    equals
                       (ebEmpresa.Rfc ?? string.Empty)
                        .Trim()
                        .ToUpper()
                    into complianceRelacion

                from ebEmpresa in complianceRelacion
                    .DefaultIfEmpty()

                select new
                {
                    /*
                     * ID MAESTRO DEL MÓDULO EMPRESAS
                     */
                    empresaId = empresa.Id,

                    /*
                     * ID INTERNO ACTUAL DE COMPLIANCE.
                     *
                     * Si ya existía la empresa en Compliance,
                     * mantenemos exactamente el mismo ID.
                     */
                    complianceId =
                        ebEmpresa != null
                            ? (int?)ebEmpresa.Id
                            : null,

                    /*
                     * Para las empresas que ya tienen expediente
                     * conservamos como "id" el ID histórico de Compliance.
                     *
                     * Si todavía no existe en Compliance utilizamos
                     * temporalmente el ID negativo de Empresa únicamente
                     * para mantener una llave única en la tabla.
                     */
                    id = empresa.Id,

                    tieneRegistroCompliance =
                        ebEmpresa != null,

                    /*
                     * ==================================================
                     * INFORMACIÓN PROVENIENTE DEL MÓDULO EMPRESAS
                     * ==================================================
                     */

                    razonSocial =
                        empresa.RazonSocial ??
                        string.Empty,

                    /*
                     * Empresa actualmente no tiene NombreCorto.
                     * Conservamos el valor anterior de Compliance
                     * solamente como información auxiliar.
                     */
                    nombreCorto =
                        ebEmpresa != null
                            ? ebEmpresa.NombreCorto
                            : string.Empty,

                    rfc =
                        empresa.RFC ??
                        string.Empty,

                    nivel =
                        empresa.Nivel != null
                            ? empresa.Nivel.Nombre
                            : string.Empty,

                    /*
                     * Mientras posteriormente incorporamos las
                     * actividades económicas de Empresa, conservamos
                     * este campo auxiliar para no alterar la tabla.
                     */
                    actividadComercial =
                        ebEmpresa != null
                            ? ebEmpresa.ActividadComercial
                            : string.Empty,

                    /*
                     * Empresa tiene un teléfono general.
                     * Lo mostramos aquí temporalmente en la columna
                     * utilizada actualmente por Compliance.
                     */
                    telefonoBancos =
                        empresa.Telefono ??
                        string.Empty,

                    correoBancos =
                        empresa.CorreoBancos ??
                        string.Empty,

                    fechaConstitucion =
                        empresa.FechaConstitucion,

                    /*
                     * Este dato todavía pertenece únicamente a
                     * Compliance.
                     */
                    numeroEscritura =
                        ebEmpresa != null
                            ? ebEmpresa.NumeroEscritura
                            : null,

                    domicilioFiscal =
                        empresa.DomicilioFiscal ??
                        string.Empty,

                    /*
                     * Las observaciones siguen siendo propias de
                     * Compliance.
                     */
                    observaciones =
                        ebEmpresa != null
                            ? ebEmpresa.Observaciones
                            : null,

                    deshabilitado =
                        empresa.Deshabilitado != 0,

                    /*
                     * Conservamos la fecha histórica de Compliance
                     * cuando ya existe un expediente.
                     */
                    fechaCreacion =
                        ebEmpresa != null
                            ? ebEmpresa.FechaCreacion
                            : (DateTime?)null
                };

            /*
             * ==========================================================
             * FILTRO DE ESTATUS
             * ==========================================================
             *
             * Ahora depende del módulo Empresas.
             */
            query = filtroEstatus switch
            {
                "Inactivas" =>
                    query.Where(x =>
                        x.deshabilitado),

                "Todas" =>
                    query,

                _ =>
                    query.Where(x =>
                        !x.deshabilitado)
            };

            /*
             * ==========================================================
             * BÚSQUEDA GENERAL
             * ==========================================================
             */
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(x =>
                    x.razonSocial.Contains(filtro) ||
                    x.nombreCorto.Contains(filtro) ||
                    x.actividadComercial.Contains(filtro));
            }

            /*
             * ==========================================================
             * RFC
             * ==========================================================
             */
            if (!string.IsNullOrWhiteSpace(filtroRfc))
            {
                query = query.Where(x =>
                    x.rfc.Contains(filtroRfc));
            }

            /*
             * ==========================================================
             * NIVEL
             * ==========================================================
             */
            if (!string.IsNullOrWhiteSpace(filtroNivel))
            {
                query = query.Where(x =>
                    x.nivel.Contains(filtroNivel));
            }

            /*
 * ==========================================================
 * FILTRAR EMPRESAS SEGÚN EL ALCANCE DEL USUARIO
 * ==========================================================
 */
            string usuarioActualId =
                ObtenerUsuarioId();

            AppUser? usuarioActual =
                await _userManager.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.Id == usuarioActualId
                    );

            if (usuarioActual != null)
            {
                IList<string> rolesUsuario =
                    await _userManager
                        .GetRolesAsync(
                            usuarioActual
                        );

                bool esAdministradorCompliance =
                    rolesUsuario.Contains(
                        ServicesConfiguration.RolMaster
                    ) ||
                    rolesUsuario.Contains(
                        ServicesConfiguration.RolAdministrador
                    ) ||
                    rolesUsuario.Contains(
                        ServicesConfiguration.RolAdministradorBancos
                    );

                /*
                 * Los administradores siempre ven todas
                 * las empresas.
                 */
                if (!esAdministradorCompliance)
                {
                    EbAlcanceComplianceUsuario? alcance =
                        await _context
                            .EbAlcancesComplianceUsuarios
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x =>
                                x.UsuarioId ==
                                    usuarioActualId
                            );

                    /*
                     * Solamente aplicamos filtro cuando el usuario
                     * tiene restricción activa.
                     *
                     * Si no existe configuración todavía,
                     * conserva el comportamiento anterior.
                     */
                    if (
                        alcance?.RestringirEmpresas ==
                        true
                    )
                    {
                        List<int> empresasPermitidas =
                            await _context
                                .EbPermisosComplianceEmpresasUsuario
                                .AsNoTracking()
                                .Where(x =>
                                    x.UsuarioId ==
                                        usuarioActualId
                                )
                                .Select(x =>
                                    x.EmpresaId
                                )
                                .Distinct()
                                .ToListAsync();

                        query =
                            query.Where(x =>
                                empresasPermitidas.Contains(
                                    x.empresaId
                                )
                            );
                    }
                }
            }

            var empresas =
            await query
                .OrderBy(x =>
                    x.empresaId
                )
                .ToListAsync();

                    return new JsonResult(
                        empresas
                    );
        }

        // =====================================================
        // CONSULTAR USUARIOS Y PERMISOS DE COMPLIANCE
        // GET ?handler=PermisosCompliance
        // =====================================================
        // =====================================================
        // CONSULTAR USUARIOS Y PERMISOS DE COMPLIANCE
        // GET ?handler=PermisosCompliance
        // =====================================================
        public async Task<IActionResult>
            OnGetPermisosComplianceAsync()
        {
            bool puedeAdministrar =
                await _permisosComplianceService
                    .PuedeAdministrarPermisosAsync(
                        User
                    );

            if (!puedeAdministrar)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        success = false,
                        message =
                            "No tienes autorización para administrar los permisos de Compliance."
                    }
                );
            }

            /*
             * ==========================================================
             * USUARIOS ACTIVOS
             * ==========================================================
             */
            List<AppUser> usuarios =
                await _userManager.Users
                    .AsNoTracking()
                    .Where(x =>
                        !x.IsBanned
                    )
                    .OrderBy(x =>
                        x.UserName
                    )
                    .ThenBy(x =>
                        x.Email
                    )
                    .ToListAsync();

            /*
             * ==========================================================
             * PERMISOS GENERALES
             * ==========================================================
             */
            Dictionary<string, EbPermisoComplianceUsuario>
                permisosPorUsuario =
                    await _context
                        .EbPermisosComplianceUsuarios
                        .AsNoTracking()
                        .ToDictionaryAsync(
                            x => x.UsuarioId
                        );

            /*
             * ==========================================================
             * TOTAL DE EMPRESAS MAESTRAS
             * ==========================================================
             */
            int totalEmpresas =
                await _context
                    .Set<Empresa>()
                    .AsNoTracking()
                    .CountAsync(x =>
                        x.Deshabilitado == 0
                    );

            /*
             * ==========================================================
             * CONFIGURACIÓN DE ALCANCE
             * ==========================================================
             */
            Dictionary<string, bool>
                alcancePorUsuario =
                    await _context
                        .EbAlcancesComplianceUsuarios
                        .AsNoTracking()
                        .ToDictionaryAsync(
                            x => x.UsuarioId,
                            x => x.RestringirEmpresas
                        );

            /*
             * ==========================================================
             * NÚMERO DE EMPRESAS ASIGNADAS
             * ==========================================================
             */
            Dictionary<string, int>
                empresasAsignadasPorUsuario =
                    await (
                        from permisoEmpresa
                            in _context
                                .EbPermisosComplianceEmpresasUsuario
                                .AsNoTracking()

                        join empresa
                            in _context
                                .Set<Empresa>()
                                .AsNoTracking()

                        on permisoEmpresa.EmpresaId
                        equals empresa.Id

                        where empresa.Deshabilitado == 0

                        group permisoEmpresa
                            by permisoEmpresa.UsuarioId
                            into grupo

                        select new
                        {
                            UsuarioId =
                                grupo.Key,

                            Total =
                                grupo
                                    .Select(x =>
                                        x.EmpresaId
                                    )
                                    .Distinct()
                                    .Count()
                        }
                    )
                    .ToDictionaryAsync(
                        x => x.UsuarioId,
                        x => x.Total
                    );

            List<UsuarioPermisoComplianceResponse>
                resultado =
                    new();

            foreach (
                AppUser usuario
                in usuarios)
            {
                IList<string> rolesUsuario =
                    await _userManager
                        .GetRolesAsync(
                            usuario
                        );

                bool esAdministradorCompliance =
                    rolesUsuario.Contains(
                        ServicesConfiguration.RolMaster
                    ) ||
                    rolesUsuario.Contains(
                        ServicesConfiguration
                            .RolAdministrador
                    ) ||
                    rolesUsuario.Contains(
                        ServicesConfiguration
                            .RolAdministradorBancos
                    );

                bool esUsuarioCompliance =
                    rolesUsuario.Contains(
                        ServicesConfiguration
                            .RolUsuarioBancos
                    ) ||
                    rolesUsuario.Contains(
                        ServicesConfiguration
                            .RolUsuarioOperacionesInternas
                    );

                permisosPorUsuario.TryGetValue(
                    usuario.Id,
                    out EbPermisoComplianceUsuario?
                        permiso
                );

                bool puedeVisualizar =
                    esAdministradorCompliance ||
                    (
                        permiso == null &&
                        esUsuarioCompliance
                    ) ||
                    permiso?.PuedeVisualizar ==
                        true;

                /*
                 * ======================================================
                 * NÚMERO DE EMPRESAS
                 * ======================================================
                 *
                 * Admin:
                 * todas.
                 *
                 * Usuario sin restricción:
                 * todas.
                 *
                 * Usuario restringido:
                 * solamente las seleccionadas.
                 * ======================================================
                 */
                alcancePorUsuario.TryGetValue(
                    usuario.Id,
                    out bool restringirEmpresas
                );

                int numeroEmpresas;

                if (
                    esAdministradorCompliance ||
                    !restringirEmpresas)
                {
                    numeroEmpresas =
                        totalEmpresas;
                }
                else
                {
                    empresasAsignadasPorUsuario
                        .TryGetValue(
                            usuario.Id,
                            out numeroEmpresas
                        );
                }

                resultado.Add(
                    new UsuarioPermisoComplianceResponse
                    {
                        Id =
                            usuario.Id,

                        Nombre =
                            ObtenerNombreUsuarioCompliance(
                                usuario
                            ),

                        Correo =
                            usuario.Email ??
                            usuario.UserName ??
                            string.Empty,

                        Roles =
                            rolesUsuario
                                .OrderBy(x => x)
                                .ToArray(),

                        EsAdministrador =
                            esAdministradorCompliance,

                        PuedeEditarPermisos =
                            !esAdministradorCompliance,

                        PuedeVisualizar =
                            puedeVisualizar,

                        PuedeCrearCargar =
                            esAdministradorCompliance ||
                            permiso?.PuedeCrearCargar ==
                                true,

                        PuedeModificar =
                            esAdministradorCompliance ||
                            permiso?.PuedeModificar ==
                                true,

                        PuedeEliminar =
                            esAdministradorCompliance ||
                            permiso?.PuedeEliminar ==
                                true,

                        PuedeDescargar =
                            esAdministradorCompliance ||
                            permiso?.PuedeDescargar ==
                                true,

                        NumeroEmpresas =
                            numeroEmpresas
                    }
                );
            }

            List<UsuarioPermisoComplianceResponse>
                usuariosOrdenados =
                    resultado
                        .OrderBy(x =>
                            x.Nombre
                        )
                        .ThenBy(x =>
                            x.Correo
                        )
                        .ToList();

            return new JsonResult(
                new
                {
                    success = true,
                    data =
                        usuariosOrdenados
                }
            );
        }

        // =====================================================
        // GUARDAR PERMISOS DE COMPLIANCE
        // POST ?handler=GuardarPermisosCompliance
        // =====================================================
        public async Task<IActionResult>
            OnPostGuardarPermisosComplianceAsync(
                [FromBody]
        GuardarPermisosComplianceRequest request)
        {
            bool puedeAdministrar =
                await _permisosComplianceService
                    .PuedeAdministrarPermisosAsync(
                        User
                    );

            if (!puedeAdministrar)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        success = false,
                        message =
                            "No tienes autorización para modificar los permisos de Compliance."
                    }
                );
            }

            if (
                request == null ||
                request.Permisos == null
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No se recibieron permisos para guardar."
                    }
                );
            }

            string usuarioModificacionId =
                ObtenerUsuarioId();

            List<string> idsRecibidos =
                request.Permisos
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.UsuarioId
                        ))
                    .Select(x =>
                        x.UsuarioId.Trim()
                    )
                    .Distinct()
                    .ToList();

            if (idsRecibidos.Count == 0)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Selecciona al menos un usuario."
                    }
                );
            }

            List<AppUser> usuariosExistentes =
                await _userManager.Users
                    .Where(x =>
                        idsRecibidos.Contains(
                            x.Id
                        ) &&
                        !x.IsBanned)
                    .ToListAsync();

            Dictionary<string, AppUser>
                usuariosPorId =
                    usuariosExistentes
                        .ToDictionary(
                            x => x.Id
                        );

            List<EbPermisoComplianceUsuario>
                permisosExistentes =
                    await _context
                        .EbPermisosComplianceUsuarios
                        .Where(x =>
                            idsRecibidos.Contains(
                                x.UsuarioId
                            ))
                        .ToListAsync();

            Dictionary<string, EbPermisoComplianceUsuario>
                permisosPorUsuario =
                    permisosExistentes
                        .ToDictionary(
                            x => x.UsuarioId
                        );

            int totalGuardados = 0;

            foreach (
                PermisoComplianceUsuarioRequest item
                in request.Permisos
            )
            {
                string usuarioId =
                    item.UsuarioId?.Trim() ??
                    string.Empty;

                if (
                    string.IsNullOrWhiteSpace(
                        usuarioId
                    ) ||
                    !usuariosPorId.TryGetValue(
                        usuarioId,
                        out AppUser? usuario
                    )
                )
                {
                    continue;
                }

                IList<string> rolesUsuario =
                    await _userManager
                        .GetRolesAsync(
                            usuario
                        );

                bool esAdministradorCompliance =
                    rolesUsuario.Contains(
                        ServicesConfiguration.RolMaster
                    ) ||
                    rolesUsuario.Contains(
                        ServicesConfiguration
                            .RolAdministrador
                    ) ||
                    rolesUsuario.Contains(
                        ServicesConfiguration
                            .RolAdministradorBancos
                    );

                /*
                 * Master, Administrador y Administrador Bancos
                 * siempre conservan acceso total.
                 */
                if (esAdministradorCompliance)
                {
                    continue;
                }


                if (
                    !permisosPorUsuario.TryGetValue(
                        usuarioId,
                        out EbPermisoComplianceUsuario?
                            permiso
                    )
                )
                {
                    permiso =
                        new EbPermisoComplianceUsuario
                        {
                            UsuarioId =
                                usuarioId,

                            FechaCreacion =
                                DateTime.Now
                        };

                    _context
                        .EbPermisosComplianceUsuarios
                        .Add(
                            permiso
                        );

                    permisosPorUsuario[
                        usuarioId
                    ] = permiso;
                }

                permiso.PuedeVisualizar =
                    item.PuedeVisualizar;

                permiso.PuedeCrearCargar =
                    item.PuedeCrearCargar;

                permiso.PuedeModificar =
                    item.PuedeModificar;

                permiso.PuedeEliminar =
                    item.PuedeEliminar;

                permiso.PuedeDescargar =
                    item.PuedeDescargar;

                permiso.FechaModificacion =
                    DateTime.Now;

                permiso.UsuarioModificacionId =
                    usuarioModificacionId;

                totalGuardados++;
            }

            await _context.SaveChangesAsync();

            return new JsonResult(
                new
                {
                    success = true,

                    message =
                        totalGuardados == 1
                            ? "El permiso se guardó correctamente."
                            : $"Se guardaron correctamente los permisos de {totalGuardados} usuarios.",

                    totalGuardados
                }
            );
        }

        // =====================================================
        // EMPRESAS PERMITIDAS POR USUARIO
        // GET ?handler=EmpresasPermisoUsuario&usuarioId=...
        // =====================================================
        public async Task<IActionResult>
            OnGetEmpresasPermisoUsuarioAsync(
                string usuarioId)
        {
            /*
             * ==========================================================
             * VALIDAR QUE QUIEN CONSULTA PUEDA ADMINISTRAR PERMISOS
             * ==========================================================
             */
            bool puedeAdministrar =
                await _permisosComplianceService
                    .PuedeAdministrarPermisosAsync(
                        User
                    );

            if (!puedeAdministrar)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        success = false,
                        message =
                            "No tienes autorización para administrar el alcance de empresas."
                    }
                );
            }

            usuarioId =
                usuarioId?.Trim() ??
                string.Empty;

            if (string.IsNullOrWhiteSpace(
                usuarioId))
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "El usuario seleccionado no es válido."
                    }
                );
            }

            /*
             * ==========================================================
             * VALIDAR USUARIO DESTINO
             * ==========================================================
             */
            AppUser? usuario =
                await _userManager.Users
                    .FirstOrDefaultAsync(x =>
                        x.Id == usuarioId &&
                        !x.IsBanned
                    );

            if (usuario == null)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No se encontró el usuario seleccionado."
                    }
                );
            }

            /*
             * ==========================================================
             * ROLES DEL USUARIO
             * ==========================================================
             */
            IList<string> rolesUsuario =
                await _userManager
                    .GetRolesAsync(
                        usuario
                    );

            bool esAdministradorCompliance =
                rolesUsuario.Contains(
                    ServicesConfiguration.RolMaster
                ) ||
                rolesUsuario.Contains(
                    ServicesConfiguration.RolAdministrador
                ) ||
                rolesUsuario.Contains(
                    ServicesConfiguration
                        .RolAdministradorBancos
                );

            /*
             * ==========================================================
             * CONFIGURACIÓN DE ALCANCE
             * ==========================================================
             */
            EbAlcanceComplianceUsuario? alcance =
                await _context
                    .EbAlcancesComplianceUsuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.UsuarioId == usuarioId
                    );

            bool restringirEmpresas =
                alcance?.RestringirEmpresas ??
                false;

            /*
             * Administradores siempre tienen acceso total.
             */
            if (esAdministradorCompliance)
            {
                restringirEmpresas =
                    false;
            }

            /*
             * ==========================================================
             * EMPRESAS ACTUALMENTE ASIGNADAS
             * ==========================================================
             */
            List<int> empresasAsignadas =
                await _context
                    .EbPermisosComplianceEmpresasUsuario
                    .AsNoTracking()
                    .Where(x =>
                        x.UsuarioId == usuarioId
                    )
                    .Select(x =>
                        x.EmpresaId
                    )
                    .Distinct()
                    .ToListAsync();

            HashSet<int> idsAsignados =
                empresasAsignadas
                    .ToHashSet();

            /*
             * ==========================================================
             * CATÁLOGO MAESTRO DE EMPRESAS
             * ==========================================================
             *
             * Se utilizan Empresa.Id.
             *
             * Incluimos activas e inactivas porque el alcance debe
             * funcionar también cuando se consulte el filtro "Todas"
             * o "Inactivas" dentro de Compliance.
             * ==========================================================
             */
            var empresas =
                await _context
                    .Set<Empresa>()
                    .AsNoTracking()
                    .Where(x =>
                        x.Deshabilitado == 0
                    )
                    .OrderBy(x =>
                        x.RazonSocial
                    )
                    .ThenBy(x =>
                        x.Id
                    )
                    .Select(x => new
                    {
                        id =
                            x.Id,

                        razonSocial =
                            x.RazonSocial ??
                            string.Empty,

                        rfc =
                            x.RFC ??
                            string.Empty,

                        deshabilitado =
                            false
                    })
                    .ToListAsync();

            /*
             * ==========================================================
             * RESPUESTA
             * ==========================================================
             *
             * Si NO existe restricción:
             * todas aparecen seleccionadas visualmente.
             *
             * Si existe restricción:
             * solamente aparecen seleccionadas las almacenadas
             * en la tabla puente.
             * ==========================================================
             */
            var empresasRespuesta =
                empresas
                    .Select(x => new
                    {
                        x.id,
                        x.razonSocial,
                        x.rfc,
                        x.deshabilitado,

                        seleccionada =
                            esAdministradorCompliance ||
                            !restringirEmpresas ||
                            idsAsignados.Contains(
                                x.id
                            )
                    })
                    .ToList();

            return new JsonResult(
                new
                {
                    success = true,

                    usuario = new
                    {
                        id =
                            usuario.Id,

                        nombre =
                            ObtenerNombreUsuarioCompliance(
                                usuario
                            ),

                        correo =
                            usuario.Email ??
                            string.Empty,

                        roles =
                            rolesUsuario,

                        esAdministrador =
                            esAdministradorCompliance
                    },

                    restringirEmpresas,

                    totalEmpresas =
                        empresasRespuesta.Count,

                    totalSeleccionadas =
                        empresasRespuesta.Count(x =>
                            x.seleccionada
                        ),

                    empresas =
                        empresasRespuesta
                }
            );
        }

        // =====================================================
        // GUARDAR EMPRESAS PERMITIDAS POR USUARIO
        // POST ?handler=GuardarEmpresasPermisoUsuario
        // =====================================================
        public async Task<IActionResult>
            OnPostGuardarEmpresasPermisoUsuarioAsync(
                [FromBody]
        GuardarEmpresasPermisoUsuarioRequest request)
        {
            /*
             * ==========================================================
             * VALIDAR ADMINISTRACIÓN DE PERMISOS
             * ==========================================================
             */
            bool puedeAdministrar =
                await _permisosComplianceService
                    .PuedeAdministrarPermisosAsync(
                        User
                    );

            if (!puedeAdministrar)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        success = false,
                        message =
                            "No tienes autorización para administrar el alcance de empresas."
                    }
                );
            }

            if (request == null)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No se recibió información para guardar."
                    }
                );
            }

            string usuarioId =
                request.UsuarioId?.Trim() ??
                string.Empty;

            if (string.IsNullOrWhiteSpace(
                usuarioId))
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "El usuario seleccionado no es válido."
                    }
                );
            }

            /*
             * ==========================================================
             * VALIDAR USUARIO
             * ==========================================================
             */
            AppUser? usuario =
                await _userManager.Users
                    .FirstOrDefaultAsync(x =>
                        x.Id == usuarioId &&
                        !x.IsBanned
                    );

            if (usuario == null)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No se encontró el usuario seleccionado."
                    }
                );
            }

            /*
             * ==========================================================
             * ADMINISTRADORES NO PUEDEN SER RESTRINGIDOS
             * ==========================================================
             */
            IList<string> rolesUsuario =
                await _userManager
                    .GetRolesAsync(
                        usuario
                    );

            bool esAdministradorCompliance =
                rolesUsuario.Contains(
                    ServicesConfiguration.RolMaster
                ) ||
                rolesUsuario.Contains(
                    ServicesConfiguration.RolAdministrador
                ) ||
                rolesUsuario.Contains(
                    ServicesConfiguration
                        .RolAdministradorBancos
                );

            if (esAdministradorCompliance)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Master, Administrador y Administrador Bancos siempre tienen acceso a todas las empresas."
                    }
                );
            }

            /*
             * ==========================================================
             * NORMALIZAR IDS RECIBIDOS
             * ==========================================================
             */
            List<int> idsSeleccionados =
                (
                    request.EmpresaIds ??
                    new List<int>()
                )
                .Where(x =>
                    x > 0
                )
                .Distinct()
                .ToList();

            /*
             * ==========================================================
             * VALIDAR QUE LAS EMPRESAS EXISTAN
             * ==========================================================
             */
            List<int> todasLasEmpresas =
                await _context
                    .Set<Empresa>()
                    .AsNoTracking()
                    .Where(x =>
                        x.Deshabilitado == 0
                    )
                    .Select(x =>
                        x.Id
                    )
                    .ToListAsync();

            HashSet<int> idsEmpresasExistentes =
                todasLasEmpresas
                    .ToHashSet();

            List<int> idsInvalidos =
                idsSeleccionados
                    .Where(x =>
                        !idsEmpresasExistentes.Contains(
                            x
                        )
                    )
                    .ToList();

            if (idsInvalidos.Count > 0)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Una o más empresas seleccionadas ya no existen."
                    }
                );
            }

            /*
             * ==========================================================
             * DETERMINAR SI REALMENTE EXISTE RESTRICCIÓN
             * ==========================================================
             *
             * Todas seleccionadas:
             *      acceso total
             *      RestringirEmpresas = false
             *
             * Algunas seleccionadas:
             *      acceso restringido
             *
             * Ninguna seleccionada:
             *      acceso restringido a 0 empresas
             *
             * Esto permite que el modal funcione únicamente
             * mediante checkboxes, sin agregar otra configuración
             * complicada para el administrador.
             * ==========================================================
             */
            bool restringirEmpresas =
                idsSeleccionados.Count <
                todasLasEmpresas.Count;

            string usuarioModificacionId =
                ObtenerUsuarioId();

            DateTime fechaActual =
                DateTime.Now;

            await using var transaccion =
                await _context.Database
                    .BeginTransactionAsync();

            try
            {
                /*
                 * ======================================================
                 * CONFIGURACIÓN GENERAL DEL USUARIO
                 * ======================================================
                 */
                EbAlcanceComplianceUsuario? alcance =
                    await _context
                        .EbAlcancesComplianceUsuarios
                        .FirstOrDefaultAsync(x =>
                            x.UsuarioId ==
                                usuarioId
                        );

                if (alcance == null)
                {
                    alcance =
                        new EbAlcanceComplianceUsuario
                        {
                            UsuarioId =
                                usuarioId,

                            RestringirEmpresas =
                                restringirEmpresas,

                            FechaCreacion =
                                fechaActual,

                            FechaModificacion =
                                null,

                            UsuarioModificacionId =
                                usuarioModificacionId
                        };

                    _context
                        .EbAlcancesComplianceUsuarios
                        .Add(
                            alcance
                        );
                }
                else
                {
                    alcance.RestringirEmpresas =
                        restringirEmpresas;

                    alcance.FechaModificacion =
                        fechaActual;

                    alcance.UsuarioModificacionId =
                        usuarioModificacionId;
                }

                /*
                 * ======================================================
                 * ELIMINAR ASIGNACIONES ANTERIORES
                 * ======================================================
                 *
                 * No eliminamos ninguna empresa.
                 *
                 * Solamente reemplazamos las filas de la tabla puente
                 * correspondientes al usuario.
                 * ======================================================
                 */
                List<EbPermisoComplianceEmpresaUsuario>
                    asignacionesActuales =
                        await _context
                            .EbPermisosComplianceEmpresasUsuario
                            .Where(x =>
                                x.UsuarioId ==
                                    usuarioId
                            )
                            .ToListAsync();

                if (asignacionesActuales.Count > 0)
                {
                    _context
                        .EbPermisosComplianceEmpresasUsuario
                        .RemoveRange(
                            asignacionesActuales
                        );
                }

                /*
                 * ======================================================
                 * CREAR NUEVAS ASIGNACIONES
                 * ======================================================
                 *
                 * Cuando están seleccionadas TODAS las empresas,
                 * RestringirEmpresas queda false y no necesitamos
                 * guardar una fila por cada empresa.
                 *
                 * Cuando existe restricción, sí almacenamos
                 * únicamente las seleccionadas.
                 * ======================================================
                 */
                if (restringirEmpresas)
                {
                    foreach (
                        int empresaId
                        in idsSeleccionados)
                    {
                        EbPermisoComplianceEmpresaUsuario
                            asignacion =
                                new()
                                {
                                    UsuarioId =
                                        usuarioId,

                                    EmpresaId =
                                        empresaId,

                                    FechaCreacion =
                                        fechaActual,

                                    UsuarioCreacionId =
                                        usuarioModificacionId
                                };

                        _context
                            .EbPermisosComplianceEmpresasUsuario
                            .Add(
                                asignacion
                            );
                    }
                }

                await _context
                    .SaveChangesAsync();

                await transaccion
                    .CommitAsync();

                /*
                 * ======================================================
                 * RESPUESTA
                 * ======================================================
                 */
                string mensaje;

                if (!restringirEmpresas)
                {
                    mensaje =
                        "El usuario tendrá acceso a todas las empresas.";
                }
                else if (idsSeleccionados.Count == 0)
                {
                    mensaje =
                        "El usuario quedó sin empresas asignadas.";
                }
                else if (idsSeleccionados.Count == 1)
                {
                    mensaje =
                        "Se asignó correctamente 1 empresa al usuario.";
                }
                else
                {
                    mensaje =
                        $"Se asignaron correctamente {idsSeleccionados.Count} empresas al usuario.";
                }

                return new JsonResult(
                    new
                    {
                        success = true,

                        message =
                            mensaje,

                        restringirEmpresas,

                        totalEmpresas =
                            todasLasEmpresas.Count,

                        totalAsignadas =
                            restringirEmpresas
                                ? idsSeleccionados.Count
                                : todasLasEmpresas.Count
                    }
                );
            }
            catch
            {
                await transaccion
                    .RollbackAsync();

                throw;
            }
        }

        // =====================================================
        // CONSULTAR REGISTRO
        // GET ?handler=Empresa&id=1
        // =====================================================
        /*public async Task<IActionResult> OnGetEmpresaAsync(int id)
        {
            if (!await _permisosComplianceService
                    .PuedeVisualizarAsync(User))
            {
                return Forbid();
            }

            var empresa = await _context.EbEmpresas
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (empresa == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No se encontró la empresa solicitada."
                });
            }

            RegistrarBitacoraEmpresa(
                empresa,
                EbAccionesBitacoraEmpresa.Consulta,
                exitoso: true,
                detalle:
                    $"Se consultó la información de la empresa '{empresa.RazonSocial}'."
            );

            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                success = true,
                data = new
                {
                    id = empresa.Id,
                    razonSocial = empresa.RazonSocial,
                    nombreCorto = empresa.NombreCorto,
                    rfc = empresa.Rfc,
                    nivel = empresa.Nivel,
                    actividadComercial = empresa.ActividadComercial,
                    telefonoBancos = empresa.TelefonoBancos,
                    correoBancos = empresa.CorreoBancos,
                    fechaConstitucion = empresa.FechaConstitucion?
                        .ToString("yyyy-MM-dd"),
                    numeroEscritura = empresa.NumeroEscritura,
                    domicilioFiscal = empresa.DomicilioFiscal,
                    observaciones = empresa.Observaciones,
                    deshabilitado = empresa.Deshabilitado
                }
            });
        }*/

        public async Task<IActionResult> OnGetEmpresaAsync(int id)
        {
            if (!await _permisosComplianceService
                .PuedeVisualizarAsync(User))
            {
                return Forbid();
            }

            if (id <= 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "El identificador de la empresa no es válido."
                });
            }

            Empresa? empresaMaestra =
                await _context.Set<Empresa>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.Id == id
                    );

            if (empresaMaestra == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No se encontró la empresa solicitada."
                });
            }

            string nivelNombre =
                string.Empty;

            if (empresaMaestra.NivelId.HasValue)
            {
                Nivel? nivel =
                    await _context.Set<Nivel>()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x =>
                            x.Id == empresaMaestra.NivelId.Value
                        );

                nivelNombre =
                    nivel?.Nombre ??
                    string.Empty;
            }

            string rfcEmpresa =
                empresaMaestra.RFC?
                    .Trim()
                    .ToUpperInvariant()
                ?? string.Empty;

            EbEmpresa? empresaCompliance =
                null;

            if (!string.IsNullOrWhiteSpace(rfcEmpresa))
            {
                empresaCompliance =
                    await _context.EbEmpresas
                        .IgnoreQueryFilters()
                        .FirstOrDefaultAsync(x =>
                            x.Rfc == rfcEmpresa
                        );
            }

            /*
             * Registrar la consulta solamente cuando exista
             * registro interno de Compliance.
             */
            if (empresaCompliance != null)
            {
                RegistrarBitacoraEmpresa(
                    empresaCompliance,
                    EbAccionesBitacoraEmpresa.Consulta,
                    exitoso: true,
                    detalle:
                        $"Se consultó la información de la empresa " +
                        $"'{empresaMaestra.RazonSocial}'."
                );

                await _context.SaveChangesAsync();
            }

            return new JsonResult(new
            {
                success = true,

                data = new
                {
                    id =
                        empresaMaestra.Id,

                    complianceId =
                        empresaCompliance?.Id,

                    tieneRegistroCompliance =
                        empresaCompliance != null,

                    razonSocial =
                        empresaMaestra.RazonSocial ??
                        string.Empty,

                    nombreCorto =
                        empresaCompliance?.NombreCorto ??
                        string.Empty,

                    rfc =
                        empresaMaestra.RFC ??
                        string.Empty,

                    nivel =
                        nivelNombre,

                    actividadComercial =
                        empresaCompliance?.ActividadComercial ??
                        string.Empty,

                    telefonoBancos =
                        empresaMaestra.Telefono ??
                        string.Empty,

                    correoBancos =
                        empresaMaestra.CorreoBancos ??
                        string.Empty,

                    fechaConstitucion =
                        empresaMaestra.FechaConstitucion?
                            .ToString("yyyy-MM-dd"),

                    numeroEscritura =
                        empresaCompliance?.NumeroEscritura,

                    domicilioFiscal =
                        empresaMaestra.DomicilioFiscal ??
                        string.Empty,

                    observaciones =
                        empresaCompliance?.Observaciones,

                    deshabilitado =
                        empresaMaestra.Deshabilitado != 0
                }
            });
        }

        private static void AgregarCambioEmpresa(
        ICollection<string> cambios,
        string campo,
        string? valorAnterior,
        string? valorNuevo)
        {
            string anterior =
                string.IsNullOrWhiteSpace(valorAnterior)
                    ? "Sin valor"
                    : valorAnterior.Trim();

            string nuevo =
                string.IsNullOrWhiteSpace(valorNuevo)
                    ? "Sin valor"
                    : valorNuevo.Trim();

            if (string.Equals(
                    anterior,
                    nuevo,
                    StringComparison.Ordinal))
            {
                return;
            }

            cambios.Add(
                $"{campo}: '{anterior}' → '{nuevo}'"
            );
        }

        // =====================================================
        // CREAR EMPRESA
        // POST ?handler=Crear
        // =====================================================

        /*public async Task<IActionResult> OnPostCrearAsync(
            [FromBody] EmpresaRequest request)
        {

            if (!await _permisosComplianceService
        .PuedeCrearCargarAsync(User))
            {
                return Forbid();
            }

            NormalizarRequest(request);

            Dictionary<string, string[]> errores =
                ValidarRequest(request, requiereId: false);

            if (errores.Any())
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Revisa la información capturada.",
                    errors = errores
                });
            }

            bool rfcExistente = await _context.EbEmpresas
                .IgnoreQueryFilters()
                .AnyAsync(x => x.Rfc == request.Rfc);

            if (rfcExistente)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Ya existe una empresa registrada con este RFC.",
                    errors = new Dictionary<string, string[]>
                    {
                        ["Rfc"] = new[]
                        {
                            "Ya existe una empresa registrada con este RFC."
                        }
                    }
                });
            }

            string usuarioId = ObtenerUsuarioId();

            var empresa = new EbEmpresa
            {
                RazonSocial = request.RazonSocial,
                NombreCorto = request.NombreCorto,
                Rfc = request.Rfc,
                Nivel = request.Nivel,
                ActividadComercial = request.ActividadComercial,
                TelefonoBancos = request.TelefonoBancos,
                CorreoBancos = request.CorreoBancos,
                FechaConstitucion = request.FechaConstitucion,
                NumeroEscritura = request.NumeroEscritura,
                DomicilioFiscal = request.DomicilioFiscal,
                Observaciones = request.Observaciones,
                Deshabilitado = false,
                Eliminado = false,
                FechaCreacion = DateTime.Now,
                UsuarioCreacionId = usuarioId
            };

            _context.EbEmpresas.Add(empresa);

            await _context.SaveChangesAsync();

            RegistrarBitacoraEmpresa(
                empresa,
                EbAccionesBitacoraEmpresa.Creacion,
                exitoso: true,
                detalle:
                    $"Se creó la empresa '{empresa.RazonSocial}'."
            );

            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                success = true,
                message = "La empresa se registró correctamente.",
                id = empresa.Id
            });
        }*/

        public async Task<IActionResult> OnPostCrearAsync(
    [FromBody] EmpresaRequest request)
        {
            if (!await _permisosComplianceService
                .PuedeCrearCargarAsync(User))
            {
                return Forbid();
            }

            NormalizarRequest(request);

            Dictionary<string, string[]> errores =
                ValidarRequest(
                    request,
                    requiereId: false
                );

            if (errores.Any())
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Revisa la información capturada.",
                    errors = errores
                });
            }

            /*
             * ==========================================================
             * VALIDAR RFC EN EMPRESA MAESTRA
             * ==========================================================
             */
            bool rfcEmpresaExistente =
                await _context.Set<Empresa>()
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.RFC != null &&
                        x.RFC.ToUpper() == request.Rfc
                    );

            if (rfcEmpresaExistente)
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "Ya existe una empresa registrada con este RFC.",

                    errors =
                        new Dictionary<string, string[]>
                        {
                            ["Rfc"] = new[]
                            {
                        "Ya existe una empresa registrada con este RFC."
                            }
                        }
                });
            }

            /*
             * ==========================================================
             * VALIDAR RFC EN COMPLIANCE
             * ==========================================================
             */
            bool rfcComplianceExistente =
                await _context.EbEmpresas
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.Rfc == request.Rfc
                    );

            if (rfcComplianceExistente)
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "Ya existe un registro de Compliance con este RFC.",

                    errors =
                        new Dictionary<string, string[]>
                        {
                            ["Rfc"] = new[]
                            {
                        "Ya existe un registro de Compliance con este RFC."
                            }
                        }
                });
            }

            /*
             * ==========================================================
             * NIVEL
             * ==========================================================
             */
            int? nivelId = null;

            if (!string.IsNullOrWhiteSpace(request.Nivel))
            {
                Nivel? nivel =
                    await _context.Set<Nivel>()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x =>
                            x.Nombre == request.Nivel
                        );

                if (nivel == null)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message =
                            "El nivel seleccionado no existe en el catálogo."
                    });
                }

                nivelId = nivel.Id;
            }

            string usuarioId =
                ObtenerUsuarioId();

            /*
             * ==========================================================
             * TRANSACCIÓN
             * ==========================================================
             */
            await using var transaccion =
                await _context.Database.BeginTransactionAsync();

            try
            {
                /*
                 * ======================================================
                 * CREAR EMPRESA MAESTRA
                 * ======================================================
                 */
                Empresa empresaMaestra =
                    new Empresa
                    {
                        RazonSocial =
                            request.RazonSocial,

                        RFC =
                            request.Rfc,

                        NivelId =
                            nivelId,

                        FechaConstitucion =
                            request.FechaConstitucion,

                        DomicilioFiscal =
                            request.DomicilioFiscal,

                        CorreoBancos =
                            request.CorreoBancos,

                        Telefono =
                            request.TelefonoBancos,

                        Deshabilitado =
                            0
                    };

                int empresaId =
                    await _empresaManager
                        .CreateAsync(
                            empresaMaestra
                        );

                if (empresaId <= 0)
                {
                    await transaccion.RollbackAsync();

                    return new JsonResult(new
                    {
                        success = false,
                        message =
                            "No fue posible crear la empresa en el catálogo maestro."
                    });
                }

                /*
                 * ======================================================
                 * CREAR REGISTRO INTERNO COMPLIANCE
                 * ======================================================
                 */
                EbEmpresa empresaCompliance =
                    new EbEmpresa
                    {
                        RazonSocial =
                            request.RazonSocial,

                        NombreCorto =
                            request.NombreCorto,

                        Rfc =
                            request.Rfc,

                        Nivel =
                            request.Nivel,

                        ActividadComercial =
                            request.ActividadComercial,

                        TelefonoBancos =
                            request.TelefonoBancos,

                        CorreoBancos =
                            request.CorreoBancos,

                        FechaConstitucion =
                            request.FechaConstitucion,

                        NumeroEscritura =
                            request.NumeroEscritura,

                        DomicilioFiscal =
                            request.DomicilioFiscal,

                        Observaciones =
                            request.Observaciones,

                        Deshabilitado =
                            false,

                        Eliminado =
                            false,

                        FechaCreacion =
                            DateTime.Now,

                        UsuarioCreacionId =
                            usuarioId
                    };

                _context.EbEmpresas.Add(
                    empresaCompliance
                );

                await _context.SaveChangesAsync();

                /*
                 * ======================================================
                 * BITÁCORA
                 * ======================================================
                 */
                RegistrarBitacoraEmpresa(
                    empresaCompliance,
                    EbAccionesBitacoraEmpresa.Creacion,
                    exitoso: true,
                    detalle:
                        $"Se creó la empresa " +
                        $"'{empresaMaestra.RazonSocial}' " +
                        $"desde Compliance."
                );

                await _context.SaveChangesAsync();

                /*
                 * ======================================================
                 * CONFIRMAR TRANSACCIÓN
                 * ======================================================
                 */
                await transaccion.CommitAsync();

                return new JsonResult(new
                {
                    success = true,

                    message =
                        "La empresa se registró correctamente.",

                    empresaId =
                        empresaId,

                    complianceId =
                        empresaCompliance.Id
                });
            }
            catch (Exception)
            {
                await transaccion.RollbackAsync();

                throw;
            }
        }

        /*public async Task<IActionResult> OnPostEditarAsync(
        [FromBody] EmpresaRequest request)
        {
            if (!await _permisosComplianceService
                    .PuedeModificarAsync(User))
            {
                return Forbid();
            }

            NormalizarRequest(request);

            Dictionary<string, string[]> errores =
                ValidarRequest(
                    request,
                    requiereId: true
                );

            if (errores.Any())
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "Revisa la información capturada.",
                    errors = errores
                });
            }

            var empresa =
                await _context.EbEmpresas
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id == request.Id &&
                            !x.Eliminado
                    );

            if (empresa == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "No se encontró la empresa solicitada."
                });
            }

            bool rfcExistente =
                await _context.EbEmpresas
                    .IgnoreQueryFilters()
                    .AnyAsync(
                        x =>
                            x.Id != request.Id &&
                            x.Rfc == request.Rfc
                    );

            if (rfcExistente)
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "Ya existe otra empresa registrada con este RFC.",

                    errors =
                        new Dictionary<string, string[]>
                        {
                            ["Rfc"] =
                                new[]
                                {
                            "Ya existe otra empresa registrada con este RFC."
                                }
                        }
                });
            }

            string razonSocialAnterior =
                empresa.RazonSocial;

            string nombreCortoAnterior =
                empresa.NombreCorto;

            string rfcAnterior =
                empresa.Rfc;

            string? nivelAnterior =
                empresa.Nivel;

            string? actividadComercialAnterior =
                empresa.ActividadComercial;

            string? telefonoBancosAnterior =
                empresa.TelefonoBancos;

            string? correoBancosAnterior =
                empresa.CorreoBancos;

            DateTime? fechaConstitucionAnterior =
                empresa.FechaConstitucion;

            string? numeroEscrituraAnterior =
                empresa.NumeroEscritura;

            string? domicilioFiscalAnterior =
                empresa.DomicilioFiscal;

            string? observacionesAnterior =
                empresa.Observaciones;

            empresa.RazonSocial =
                request.RazonSocial;

            empresa.NombreCorto =
                request.NombreCorto;

            empresa.Rfc =
                request.Rfc;

            empresa.Nivel =
                request.Nivel;

            empresa.ActividadComercial =
                request.ActividadComercial;

            empresa.TelefonoBancos =
                request.TelefonoBancos;

            empresa.CorreoBancos =
                request.CorreoBancos;

            empresa.FechaConstitucion =
                request.FechaConstitucion;

            empresa.NumeroEscritura =
                request.NumeroEscritura;

            empresa.DomicilioFiscal =
                request.DomicilioFiscal;

            empresa.Observaciones =
                request.Observaciones;

            
            empresa.FechaActualizacion =
                DateTime.Now;

            empresa.UsuarioActualizacionId =
                ObtenerUsuarioId();

            List<string> cambios =
                new();

            AgregarCambioEmpresa(
                cambios,
                "Razón social",
                razonSocialAnterior,
                empresa.RazonSocial
            );

            AgregarCambioEmpresa(
                cambios,
                "Nombre corto",
                nombreCortoAnterior,
                empresa.NombreCorto
            );

            AgregarCambioEmpresa(
                cambios,
                "RFC",
                rfcAnterior,
                empresa.Rfc
            );

            AgregarCambioEmpresa(
                cambios,
                "Nivel",
                nivelAnterior,
                empresa.Nivel
            );

            AgregarCambioEmpresa(
                cambios,
                "Actividad comercial",
                actividadComercialAnterior,
                empresa.ActividadComercial
            );

            AgregarCambioEmpresa(
                cambios,
                "Teléfono de bancos",
                telefonoBancosAnterior,
                empresa.TelefonoBancos
            );

            AgregarCambioEmpresa(
                cambios,
                "Correo de bancos",
                correoBancosAnterior,
                empresa.CorreoBancos
            );

            AgregarCambioEmpresa(
                cambios,
                "Fecha de constitución",
                fechaConstitucionAnterior?
                    .ToString("dd/MM/yyyy"),
                empresa.FechaConstitucion?
                    .ToString("dd/MM/yyyy")
            );

            AgregarCambioEmpresa(
                cambios,
                "Número de escritura",
                numeroEscrituraAnterior,
                empresa.NumeroEscritura
            );

            AgregarCambioEmpresa(
                cambios,
                "Domicilio fiscal",
                domicilioFiscalAnterior,
                empresa.DomicilioFiscal
            );

            AgregarCambioEmpresa(
                cambios,
                "Observaciones",
                observacionesAnterior,
                empresa.Observaciones
            );

            RegistrarBitacoraEmpresa(
                empresa,
                EbAccionesBitacoraEmpresa.Edicion,
                exitoso: true,
                detalle:
                    cambios.Count > 0
                        ? string.Join(
                            " | ",
                            cambios
                        )
                        : "Se guardó la empresa sin cambios detectables."
            );

            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                success = true,
                message =
                    "La empresa se actualizó correctamente."
            });
        }*/

        public async Task<IActionResult> OnPostEditarAsync(
            [FromBody] EmpresaRequest request)
        {
            if (!await _permisosComplianceService
                .PuedeModificarAsync(User))
            {
                return Forbid();
            }

            NormalizarRequest(request);

            Dictionary<string, string[]> errores =
                ValidarRequest(
                    request,
                    requiereId: true
                );

            if (errores.Any())
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Revisa la información capturada.",
                    errors = errores
                });
            }

            /*
             * ==========================================================
             * 1. LOCALIZAR EMPRESA MAESTRA
             * ==========================================================
             */
            Empresa? empresaMaestra =
                await _context.Set<Empresa>()
                    .Include(x => x.Nivel)
                    .FirstOrDefaultAsync(x =>
                        x.Id == request.Id
                    );

            if (empresaMaestra == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No se encontró la empresa solicitada."
                });
            }

            /*
             * Conservamos el RFC anterior porque actualmente
             * utilizamos RFC como relación Empresa <-> EbEmpresa.
             */
            string rfcAnterior =
                empresaMaestra.RFC?
                    .Trim()
                    .ToUpperInvariant()
                ?? string.Empty;

            /*
             * ==========================================================
             * 2. VALIDAR RFC CONTRA EMPRESAS MAESTRAS
             * ==========================================================
             */
            bool rfcExistente =
                await _context.Set<Empresa>()
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.Id != request.Id &&
                        x.RFC != null &&
                        x.RFC.ToUpper() == request.Rfc
                    );

            if (rfcExistente)
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "Ya existe otra empresa registrada con este RFC.",

                    errors =
                        new Dictionary<string, string[]>
                        {
                            ["Rfc"] = new[]
                            {
                        "Ya existe otra empresa registrada con este RFC."
                            }
                        }
                });
            }

            /*
             * ==========================================================
             * 3. LOCALIZAR REGISTRO INTERNO DE COMPLIANCE
             * ==========================================================
             */
            EbEmpresa? empresaCompliance =
                null;

            if (!string.IsNullOrWhiteSpace(rfcAnterior))
            {
                empresaCompliance =
                    await _context.EbEmpresas
                        .IgnoreQueryFilters()
                        .FirstOrDefaultAsync(x =>
                            x.Rfc == rfcAnterior
                        );
            }

            /*
             * ==========================================================
             * 4. NIVEL
             * ==========================================================
             */
            int? nivelId =
                empresaMaestra.NivelId;

            if (!string.IsNullOrWhiteSpace(request.Nivel))
            {
                Nivel? nivel =
                    await _context.Set<Nivel>()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x =>
                            x.Nombre == request.Nivel
                        );

                if (nivel == null)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message =
                            "El nivel seleccionado no existe en el catálogo."
                    });
                }

                nivelId =
                    nivel.Id;
            }

            /*
             * ==========================================================
             * 5. VALORES ANTERIORES DE EMPRESA MAESTRA
             * ==========================================================
             */
            string razonSocialAnterior =
                empresaMaestra.RazonSocial ??
                string.Empty;

            string domicilioAnterior =
                empresaMaestra.DomicilioFiscal ??
                string.Empty;

            string correoBancosAnterior =
                empresaMaestra.CorreoBancos ??
                string.Empty;

            string telefonoAnterior =
                empresaMaestra.Telefono ??
                string.Empty;

            DateTime? fechaConstitucionAnterior =
                empresaMaestra.FechaConstitucion;

            string nivelAnterior =
                empresaMaestra.Nivel?.Nombre ??
                string.Empty;

            /*
             * ==========================================================
             * 6. VALORES ANTERIORES DE COMPLIANCE
             * ==========================================================
             */
            string nombreCortoAnterior =
                empresaCompliance?.NombreCorto ??
                string.Empty;

            string actividadComercialAnterior =
                empresaCompliance?.ActividadComercial ??
                string.Empty;

            string numeroEscrituraAnterior =
                empresaCompliance?.NumeroEscritura ??
                string.Empty;

            string observacionesAnterior =
                empresaCompliance?.Observaciones ??
                string.Empty;

            /*
             * ==========================================================
             * 7. ACTUALIZAR EMPRESA MAESTRA
             * ==========================================================
             */
            empresaMaestra.RazonSocial =
                request.RazonSocial;

            empresaMaestra.RFC =
                request.Rfc;

            empresaMaestra.NivelId =
                nivelId;

            empresaMaestra.FechaConstitucion =
                request.FechaConstitucion;

            empresaMaestra.DomicilioFiscal =
                request.DomicilioFiscal;

            empresaMaestra.CorreoBancos =
                request.CorreoBancos;

            empresaMaestra.Telefono =
                request.TelefonoBancos;

            /*
             * ==========================================================
             * 8. CREAR REGISTRO COMPLIANCE SI NO EXISTE
             * ==========================================================
             *
             * Esto soluciona el caso de empresas creadas originalmente
             * desde el módulo Empresas y que todavía no tenían EbEmpresa.
             */
            bool seCreoRegistroCompliance =
                false;

            if (empresaCompliance == null)
            {
                empresaCompliance =
                    new EbEmpresa
                    {
                        RazonSocial =
                            request.RazonSocial,

                        NombreCorto =
                            request.NombreCorto,

                        Rfc =
                            request.Rfc,

                        Nivel =
                            request.Nivel,

                        ActividadComercial =
                            request.ActividadComercial,

                        TelefonoBancos =
                            request.TelefonoBancos,

                        CorreoBancos =
                            request.CorreoBancos,

                        FechaConstitucion =
                            request.FechaConstitucion,

                        NumeroEscritura =
                            request.NumeroEscritura,

                        DomicilioFiscal =
                            request.DomicilioFiscal,

                        Observaciones =
                            request.Observaciones,

                        Deshabilitado =
                            empresaMaestra.Deshabilitado != 0,

                        Eliminado =
                            false,

                        FechaCreacion =
                            DateTime.Now,

                        UsuarioCreacionId =
                            ObtenerUsuarioId()
                    };

                _context.EbEmpresas.Add(
                    empresaCompliance
                );

                seCreoRegistroCompliance =
                    true;
            }
            else
            {
                /*
                 * ======================================================
                 * 9. ACTUALIZAR REGISTRO COMPLIANCE EXISTENTE
                 * ======================================================
                 */
                empresaCompliance.RazonSocial =
                    request.RazonSocial;

                empresaCompliance.Rfc =
                    request.Rfc;

                empresaCompliance.Nivel =
                    request.Nivel;

                empresaCompliance.CorreoBancos =
                    request.CorreoBancos;

                empresaCompliance.FechaConstitucion =
                    request.FechaConstitucion;

                empresaCompliance.DomicilioFiscal =
                    request.DomicilioFiscal;

                empresaCompliance.NombreCorto =
                    request.NombreCorto;

                empresaCompliance.ActividadComercial =
                    request.ActividadComercial;

                empresaCompliance.TelefonoBancos =
                    request.TelefonoBancos;

                empresaCompliance.NumeroEscritura =
                    request.NumeroEscritura;

                empresaCompliance.Observaciones =
                    request.Observaciones;

                empresaCompliance.Deshabilitado =
                    empresaMaestra.Deshabilitado != 0;

                empresaCompliance.Eliminado =
                    false;

                empresaCompliance.FechaActualizacion =
                    DateTime.Now;

                empresaCompliance.UsuarioActualizacionId =
                    ObtenerUsuarioId();
            }

            /*
             * ==========================================================
             * 10. BITÁCORA
             * ==========================================================
             */
            List<string> cambios =
                new();

            AgregarCambioEmpresa(
                cambios,
                "Razón social",
                razonSocialAnterior,
                empresaMaestra.RazonSocial
            );

            AgregarCambioEmpresa(
                cambios,
                "RFC",
                rfcAnterior,
                empresaMaestra.RFC
            );

            AgregarCambioEmpresa(
                cambios,
                "Nivel",
                nivelAnterior,
                request.Nivel
            );

            AgregarCambioEmpresa(
                cambios,
                "Fecha de constitución",
                fechaConstitucionAnterior?
                    .ToString("dd/MM/yyyy"),
                empresaMaestra.FechaConstitucion?
                    .ToString("dd/MM/yyyy")
            );

            AgregarCambioEmpresa(
                cambios,
                "Domicilio fiscal",
                domicilioAnterior,
                empresaMaestra.DomicilioFiscal
            );

            AgregarCambioEmpresa(
                cambios,
                "Correo bancos",
                correoBancosAnterior,
                empresaMaestra.CorreoBancos
            );

            AgregarCambioEmpresa(
                cambios,
                "Teléfono",
                telefonoAnterior,
                empresaMaestra.Telefono
            );

            /*
             * Campos específicos de Compliance.
             */
            AgregarCambioEmpresa(
                cambios,
                "Nombre corto",
                nombreCortoAnterior,
                request.NombreCorto
            );

            AgregarCambioEmpresa(
                cambios,
                "Actividad comercial",
                actividadComercialAnterior,
                request.ActividadComercial
            );

            AgregarCambioEmpresa(
                cambios,
                "Número de escritura",
                numeroEscrituraAnterior,
                request.NumeroEscritura
            );

            AgregarCambioEmpresa(
                cambios,
                "Observaciones",
                observacionesAnterior,
                request.Observaciones
            );

            /*
             * ==========================================================
             * 11. GUARDAR EMPRESA + COMPLIANCE
             * ==========================================================
             *
             * Primero guardamos para garantizar que EbEmpresa tenga ID
             * si acaba de ser creada.
             */
            await _context.SaveChangesAsync();

            /*
             * ==========================================================
             * 12. REGISTRAR BITÁCORA COMPLIANCE
             * ==========================================================
             */
            if (seCreoRegistroCompliance)
            {
                RegistrarBitacoraEmpresa(
                    empresaCompliance,
                    EbAccionesBitacoraEmpresa.Creacion,
                    exitoso: true,
                    detalle:
                        $"Se inicializó el expediente de Compliance para " +
                        $"'{empresaMaestra.RazonSocial}' durante su edición. " +
                        (
                            cambios.Count > 0
                                ? string.Join(" | ", cambios)
                                : "Sin cambios adicionales."
                        )
                );
            }
            else
            {
                RegistrarBitacoraEmpresa(
                    empresaCompliance,
                    EbAccionesBitacoraEmpresa.Edicion,
                    exitoso: true,
                    detalle:
                        cambios.Count > 0
                            ? string.Join(
                                " | ",
                                cambios
                            )
                            : "Se guardó la empresa sin cambios detectables."
                );
            }

            await _context.SaveChangesAsync();

            /*
             * ==========================================================
             * 13. RESPUESTA
             * ==========================================================
             */
            return new JsonResult(new
            {
                success = true,

                message =
                    "La empresa se actualizó correctamente.",

                empresaId =
                    empresaMaestra.Id,

                complianceId =
                    empresaCompliance.Id
            });
        }



        // =====================================================
        // HABILITAR / DESHABILITAR
        // POST ?handler=CambiarEstatus
        // =====================================================
        /*public async Task<IActionResult> OnPostCambiarEstatusAsync(
            [FromBody] EmpresaIdRequest request)
        {

            if (!await _permisosComplianceService
        .PuedeModificarAsync(User))
            {
                return Forbid();
            }

            if (request.Id <= 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "El identificador de la empresa no es válido."
                });
            }

            var empresa = await _context.EbEmpresas
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (empresa == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No se encontró la empresa solicitada."
                });
            }

            empresa.Deshabilitado =
            !empresa.Deshabilitado;

            empresa.FechaActualizacion =
                DateTime.Now;

            empresa.UsuarioActualizacionId =
                ObtenerUsuarioId();

            RegistrarBitacoraEmpresa(
                empresa,
                EbAccionesBitacoraEmpresa.CambioEstatus,
                exitoso: true,
                detalle:
                    empresa.Deshabilitado
                        ? $"Se deshabilitó la empresa '{empresa.RazonSocial}'."
                        : $"Se habilitó la empresa '{empresa.RazonSocial}'."
            );

            await _context.SaveChangesAsync();

            string mensaje = empresa.Deshabilitado
                ? "La empresa se deshabilitó correctamente."
                : "La empresa se habilitó correctamente.";

            return new JsonResult(new
            {
                success = true,
                message = mensaje,
                deshabilitado = empresa.Deshabilitado
            });
        }*/

        // =====================================================
        // HABILITAR / DESHABILITAR EMPRESA
        // POST ?handler=CambiarEstatus
        // =====================================================
        public async Task<IActionResult> OnPostCambiarEstatusAsync(
            [FromBody] EmpresaIdRequest request)
        {
            if (!await _permisosComplianceService
                .PuedeModificarAsync(User))
            {
                return Forbid();
            }

            if (request.Id <= 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "El identificador de la empresa no es válido."
                });
            }

            /*
             * ==========================================================
             * 1. LOCALIZAR EMPRESA MAESTRA
             * ==========================================================
             *
             * request.Id corresponde ahora al Id del módulo Empresas.
             */
            Empresa? empresaMaestra =
                await _context.Set<Empresa>()
                    .FirstOrDefaultAsync(x =>
                        x.Id == request.Id
                    );

            if (empresaMaestra == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No se encontró la empresa solicitada."
                });
            }

            /*
             * RFC actual utilizado para localizar, cuando exista,
             * su registro interno histórico de Compliance.
             */
            string rfcEmpresa =
                empresaMaestra.RFC?
                    .Trim()
                    .ToUpperInvariant()
                ?? string.Empty;

            /*
             * ==========================================================
             * 2. LOCALIZAR REGISTRO INTERNO DE COMPLIANCE
             * ==========================================================
             */
            EbEmpresa? empresaCompliance = null;

            if (!string.IsNullOrWhiteSpace(rfcEmpresa))
            {
                empresaCompliance =
                    await _context.EbEmpresas
                        .IgnoreQueryFilters()
                        .FirstOrDefaultAsync(x =>
                            x.Rfc == rfcEmpresa
                        );
            }

            /*
             * ==========================================================
             * 3. CAMBIAR ESTATUS EN EMPRESA MAESTRA
             * ==========================================================
             *
             * Empresa utiliza:
             *
             * 0 = activa
             * 1 = deshabilitada
             */
            bool seDeshabilita =
                empresaMaestra.Deshabilitado == 0;

            empresaMaestra.Deshabilitado =
                seDeshabilita
                    ? 1
                    : 0;

            /*
             * ==========================================================
             * 4. SINCRONIZAR ESTATUS INTERNO DE COMPLIANCE
             * ==========================================================
             *
             * No es la fuente principal, pero lo mantenemos sincronizado
             * para proteger procesos existentes que todavía utilizan
             * EbEmpresa.
             */
            if (empresaCompliance != null)
            {
                empresaCompliance.Deshabilitado =
                    seDeshabilita;

                empresaCompliance.FechaActualizacion =
                    DateTime.Now;

                empresaCompliance.UsuarioActualizacionId =
                    ObtenerUsuarioId();

                /*
                 * Conservamos la bitácora existente de Compliance.
                 */
                RegistrarBitacoraEmpresa(
                    empresaCompliance,
                    EbAccionesBitacoraEmpresa.CambioEstatus,
                    exitoso: true,
                    detalle:
                        seDeshabilita
                            ? $"Se deshabilitó la empresa '{empresaMaestra.RazonSocial}'."
                            : $"Se habilitó la empresa '{empresaMaestra.RazonSocial}'."
                );
            }

            /*
             * ==========================================================
             * 5. GUARDAR
             * ==========================================================
             */
            await _context.SaveChangesAsync();

            string mensaje =
                seDeshabilita
                    ? "La empresa se deshabilitó correctamente."
                    : "La empresa se habilitó correctamente.";

            return new JsonResult(new
            {
                success = true,
                message = mensaje,
                deshabilitado = seDeshabilita
            });
        }

        // =====================================================
        // ELIMINACIÓN LÓGICA
        // POST ?handler=Eliminar
        // =====================================================
        /*public async Task<IActionResult> OnPostEliminarAsync(
            [FromBody] EmpresaIdRequest request)
        {

            if (!await _permisosComplianceService
        .PuedeEliminarAsync(User))
            {
                return Forbid();
            }

            if (request.Id <= 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "El identificador de la empresa no es válido."
                });
            }

            var empresa = await _context.EbEmpresas
                .Include(x => x.Accionistas)
                .Include(x => x.Documentos)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (empresa == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No se encontró la empresa solicitada."
                });
            }

            bool tieneInformacionRelacionada =
                empresa.Accionistas.Any() ||
                empresa.Documentos.Any();

            if (tieneInformacionRelacionada)
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "La empresa tiene información relacionada. " +
                        "Puedes deshabilitarla, pero no eliminarla."
                });
            }

            empresa.Eliminado = true;

            empresa.Deshabilitado =
                true;

            empresa.FechaActualizacion =
                DateTime.Now;

            empresa.UsuarioActualizacionId =
                ObtenerUsuarioId();

            RegistrarBitacoraEmpresa(
                empresa,
                EbAccionesBitacoraEmpresa.Eliminacion,
                exitoso: true,
                detalle:
                    $"Se eliminó lógicamente la empresa '{empresa.RazonSocial}'."
            );

            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                success = true,
                message = "La empresa se eliminó correctamente."
            });
        }*/

        public async Task<IActionResult> OnPostEliminarAsync(
    [FromBody] EmpresaIdRequest request)
        {
            if (!await _permisosComplianceService
                .PuedeEliminarAsync(User))
            {
                return Forbid();
            }

            if (request.Id <= 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "El identificador de la empresa no es válido."
                });
            }

            /*
             * ==========================================================
             * 1. LOCALIZAR EMPRESA MAESTRA
             * ==========================================================
             */
            Empresa? empresaMaestra =
                await _context.Set<Empresa>()
                    .FirstOrDefaultAsync(x =>
                        x.Id == request.Id
                    );

            if (empresaMaestra == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No se encontró la empresa solicitada."
                });
            }

            string rfcEmpresa =
                empresaMaestra.RFC?
                    .Trim()
                    .ToUpperInvariant()
                ?? string.Empty;

            /*
             * ==========================================================
             * 2. LOCALIZAR REGISTRO INTERNO DE COMPLIANCE
             * ==========================================================
             */
            EbEmpresa? empresaCompliance = null;

            if (!string.IsNullOrWhiteSpace(rfcEmpresa))
            {
                empresaCompliance =
                    await _context.EbEmpresas
                        .IgnoreQueryFilters()
                        .Include(x => x.Accionistas)
                        .Include(x => x.Documentos)
                        .FirstOrDefaultAsync(x =>
                            x.Rfc == rfcEmpresa
                        );
            }

            /*
             * ==========================================================
             * 3. PROTEGER INFORMACIÓN RELACIONADA
             * ==========================================================
             */
            if (empresaCompliance != null)
            {
                bool tieneInformacionRelacionada =
                    empresaCompliance.Accionistas.Any() ||
                    empresaCompliance.Documentos.Any();

                if (tieneInformacionRelacionada)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message =
                            "La empresa tiene información relacionada. " +
                            "Puedes deshabilitarla, pero no eliminarla."
                    });
                }
            }

            /*
             * ==========================================================
             * 4. DESHABILITAR EMPRESA MAESTRA
             * ==========================================================
             */
            empresaMaestra.Deshabilitado = 1;

            /*
             * ==========================================================
             * 5. ACTUALIZAR REGISTRO DE COMPLIANCE
             * ==========================================================
             */
            if (empresaCompliance != null)
            {
                empresaCompliance.Eliminado = true;
                empresaCompliance.Deshabilitado = true;

                empresaCompliance.FechaActualizacion =
                    DateTime.Now;

                empresaCompliance.UsuarioActualizacionId =
                    ObtenerUsuarioId();

                RegistrarBitacoraEmpresa(
                    empresaCompliance,
                    EbAccionesBitacoraEmpresa.Eliminacion,
                    exitoso: true,
                    detalle:
                        $"Se eliminó lógicamente la empresa " +
                        $"'{empresaMaestra.RazonSocial}' desde Compliance."
                );
            }

            /*
             * ==========================================================
             * 6. GUARDAR
             * ==========================================================
             */
            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                success = true,
                message = "La empresa se eliminó correctamente."
            });
        }

        // =====================================================
        // DOCUMENTOS DE UNA EMPRESA
        // GET ?handler=Documentos&empresaId=1
        // =====================================================
        public async Task<IActionResult> OnGetDocumentosAsync(
    int empresaId)
        {
            if (!await _permisosComplianceService
                .PuedeVisualizarAsync(User))
            {
                return Forbid();
            }

            if (empresaId <= 0)
            {
                return new JsonResult(new
                {
                    success = false,

                    message =
                        "El identificador de la empresa no es válido."
                });
            }

            /*
             * ==========================================================
             * 1. LOCALIZAR EMPRESA MAESTRA
             * ==========================================================
             *
             * A partir de ahora empresaId corresponde a Empresa.Id.
             * ==========================================================
             */
            Empresa? empresaMaestra =
                await _context
                    .Set<Empresa>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.Id == empresaId
                    );

            if (empresaMaestra == null)
            {
                return new JsonResult(new
                {
                    success = false,

                    message =
                        "No se encontró la empresa solicitada."
                });
            }

            /*
             * ==========================================================
             * 2. NORMALIZAR RFC
             * ==========================================================
             */
            string rfcEmpresa =
                empresaMaestra.RFC?
                    .Trim()
                    .ToUpperInvariant()
                ?? string.Empty;

            /*
             * ==========================================================
             * 3. LOCALIZAR EXPEDIENTE COMPLIANCE
             * ==========================================================
             */
            EbEmpresa? empresaCompliance =
                null;

            if (
                !string.IsNullOrWhiteSpace(
                    rfcEmpresa
                )
            )
            {
                empresaCompliance =
                    await _context
                        .EbEmpresas
                        .IgnoreQueryFilters()
                        .FirstOrDefaultAsync(x =>
                            x.Rfc != null &&
                            x.Rfc.Trim().ToUpper() ==
                                rfcEmpresa
                        );
            }

            /*
             * ==========================================================
             * 4. CREAR EXPEDIENTE COMPLIANCE SI NO EXISTE
             * ==========================================================
             *
             * Esto permite que CUALQUIER Empresa del catálogo maestro
             * pueda abrir el modal Documentos.
             *
             * Si no tiene documentos:
             *     mostrará el catálogo con documentos pendientes.
             *
             * Si tiene documentos en Empresas:
             *     posteriormente serán sincronizados.
             * ==========================================================
             */
            if (empresaCompliance == null)
            {
                string nivelNombre =
                    string.Empty;

                if (
                    empresaMaestra.NivelId.HasValue
                )
                {
                    Nivel? nivelEmpresa =
                        await _context
                            .Set<Nivel>()
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x =>
                                x.Id ==
                                empresaMaestra
                                    .NivelId
                                    .Value
                            );

                    nivelNombre =
                        nivelEmpresa?.Nombre ??
                        string.Empty;
                }

                empresaCompliance =
                    new EbEmpresa
                    {
                        RazonSocial =
                            empresaMaestra.RazonSocial ??
                            string.Empty,

                        NombreCorto =
                            string.Empty,

                        Rfc =
                            rfcEmpresa,

                        Nivel =
                            nivelNombre,

                        ActividadComercial =
                            null,

                        TelefonoBancos =
                            empresaMaestra.Telefono,

                        CorreoBancos =
                            empresaMaestra.CorreoBancos,

                        FechaConstitucion =
                            empresaMaestra.FechaConstitucion,

                        NumeroEscritura =
                            null,

                        DomicilioFiscal =
                            empresaMaestra.DomicilioFiscal,

                        Observaciones =
                            null,

                        Deshabilitado =
                            empresaMaestra.Deshabilitado != 0,

                        Eliminado =
                            false,

                        FechaCreacion =
                            DateTime.Now,

                        UsuarioCreacionId =
                            ObtenerUsuarioId()
                    };

                _context
                    .EbEmpresas
                    .Add(
                        empresaCompliance
                    );

                await _context
                    .SaveChangesAsync();
            }

            /*
             * ==========================================================
             * 5. ID INTERNO REAL DE COMPLIANCE
             * ==========================================================
             */
            int complianceId =
                empresaCompliance.Id;

            /*
             * ==========================================================
             * 6. SINCRONIZAR DOCUMENTOS EMPRESAS → COMPLIANCE
             * ==========================================================
             */
            try
            {
                await _documentoEmpresasComplianceService
                    .SincronizarDesdeEmpresaAsync(
                        empresaMaestra.Id,
                        complianceId,
                        ObtenerUsuarioId()
                    );
            }
            catch (Exception ex)
            {
                /*
                 * La sincronización no debe impedir abrir
                 * el expediente documental.
                 */
                Console.WriteLine(
                    "======================================"
                );

                Console.WriteLine(
                    "ERROR DE SINCRONIZACIÓN " +
                    "EMPRESAS → COMPLIANCE"
                );

                Console.WriteLine(
                    ex.ToString()
                );

                Console.WriteLine(
                    "======================================"
                );
            }

            DateTime fechaActual =
                DateTime.Today;

            DateTime fechaProximaVencimiento =
                fechaActual.AddDays(30);

            /*
             * ==========================================================
             * CATÁLOGO DE TIPOS DOCUMENTALES
             * ==========================================================
             */
            var tiposDocumento =
                await _context.EbTiposDocumento
                    .AsNoTracking()
                    .Where(x =>
                        !x.Eliminado &&
                        !x.Deshabilitado
                    )
                    .OrderBy(x =>
                        x.Orden
                    )
                    .ThenBy(x =>
                        x.Nombre
                    )
                    .Select(x => new
                    {
                        id =
                            x.Id,

                        nombre =
                            x.Nombre,

                        categoria =
                            x.Categoria,

                        descripcion =
                            x.Descripcion,

                        esObligatorio =
                            x.EsObligatorio,

                        requiereFechaVencimiento =
                            x.RequiereFechaVencimiento,

                        permiteMultiplesArchivos =
                            x.PermiteMultiplesArchivos,

                        orden =
                            x.Orden
                    })
                    .ToListAsync();

            /*
             * ==========================================================
             * DOCUMENTOS ACTUALES
             * ==========================================================
             */
            var documentosEmpresa =
                await _context
                    .EbDocumentos
                    .AsNoTracking()
                    .Where(x =>
                        x.EmpresaId ==
                            complianceId &&
                        !x.Eliminado &&
                        x.EsVersionActual
                    )
                    .OrderByDescending(x =>
                        x.FechaCarga
                    )
                    .Select(x => new
                    {
                        id = x.Id,
                        empresaId = x.EmpresaId,
                        tipoDocumentoId =
                            x.TipoDocumentoId,
                        nombreOriginal =
                            x.NombreOriginal,
                        nombreAlmacenado =
                            x.NombreAlmacenado,
                        rutaArchivo =
                            x.RutaArchivo,
                        extension =
                            x.Extension,
                        mimeType =
                            x.MimeType,
                        tamanoBytes =
                            x.TamanoBytes,
                        version =
                            x.Version,
                        fechaCarga =
                            x.FechaCarga,
                        fechaVencimiento =
                            x.FechaVencimiento,
                        estado =
                            x.Estado,
                        observaciones =
                            x.Observaciones
                    })
                    .ToListAsync();

            /*
             * ==========================================================
             * CONSTRUIR RESPUESTA DOCUMENTAL
             * ==========================================================
             */
            var documentos =
                tiposDocumento
                    .Select(tipo =>
                    {
                        var archivos =
                            documentosEmpresa
                                .Where(x =>
                                    x.tipoDocumentoId ==
                                        tipo.id
                                )
                                .OrderByDescending(x =>
                                    x.fechaCarga
                                )
                                .ToList();

                        string estatus;

                        if (archivos.Count == 0)
                        {
                            estatus =
                                "Pendiente";
                        }
                        else
                        {
                            bool tieneVencidos =
                                archivos.Any(x =>
                                    x.fechaVencimiento.HasValue &&
                                    x.fechaVencimiento.Value.Date <
                                        fechaActual
                                );

                            bool tieneProximosAVencer =
                                archivos.Any(x =>
                                    x.fechaVencimiento.HasValue &&
                                    x.fechaVencimiento.Value.Date >=
                                        fechaActual &&
                                    x.fechaVencimiento.Value.Date <=
                                        fechaProximaVencimiento
                                );

                            bool tieneVigentes =
                                archivos.Any(x =>
                                    x.fechaVencimiento.HasValue &&
                                    x.fechaVencimiento.Value.Date >
                                        fechaProximaVencimiento
                                );

                            if (tieneVencidos)
                            {
                                estatus =
                                    "Vencido";
                            }
                            else if (tieneProximosAVencer)
                            {
                                estatus =
                                    "Próximo a vencer";
                            }
                            else if (tieneVigentes)
                            {
                                estatus =
                                    "Vigente";
                            }
                            else
                            {
                                estatus =
                                    "Cargado";
                            }
                        }

                        return new
                        {
                            id =
                                tipo.id,

                            nombre =
                                tipo.nombre,

                            categoria =
                                tipo.categoria,

                            descripcion =
                                tipo.descripcion,

                            obligatorio =
                                tipo.esObligatorio,

                            requiereFechaVencimiento =
                                tipo.requiereFechaVencimiento,

                            permiteMultiples =
                                tipo.permiteMultiplesArchivos,

                            orden =
                                tipo.orden,

                            estatus,

                            totalArchivos =
                                archivos.Count,

                            archivos =
                                archivos
                                    .Select(archivo => new
                                    {
                                        id =
                                            archivo.id,

                                        tipoDocumentoId =
                                            archivo.tipoDocumentoId,

                                        nombreOriginal =
                                            archivo.nombreOriginal,

                                        extension =
                                            archivo.extension,

                                        mimeType =
                                            archivo.mimeType,

                                        tamanoBytes =
                                            archivo.tamanoBytes,

                                        version =
                                            archivo.version,

                                        fechaCarga =
                                            archivo.fechaCarga,

                                        fechaVencimiento =
                                            archivo.fechaVencimiento,

                                        estado =
                                            archivo.estado,

                                        observaciones =
                                            archivo.observaciones
                                    })
                                    .ToList()
                        };
                    })
                    .ToList();

            /*
             * ==========================================================
             * RESUMEN
             * ==========================================================
             */
            int totalRequeridos =
                documentos.Count(x =>
                    x.obligatorio
                );

            int totalCargados =
                documentos.Count(x =>
                    x.totalArchivos > 0
                );

            int totalPendientes =
                documentos.Count(x =>
                    x.obligatorio &&
                    x.totalArchivos == 0
                );

            int totalVencidos =
                documentos.Count(x =>
                    x.estatus == "Vencido"
                );

            int totalProximosAVencer =
                documentos.Count(x =>
                    x.estatus ==
                        "Próximo a vencer"
                );

            return new JsonResult(new
            {
                success = true,

                complianceId =
                    complianceId,

                data =
                    documentos,

                resumen = new
                {
                    totalDocumentos =
                    documentos.Count,

                    totalRequeridos,

                    totalCargados,

                    totalPendientes,

                    totalVencidos,

                    totalProximosAVencer
                }
            });
        }

        // =====================================================
        // HISTORIAL DE VERSIONES DE UN DOCUMENTO
        // GET ?handler=HistorialDocumento
        //     &empresaId=1
        //     &tipoDocumentoId=1
        // =====================================================
        public async Task<IActionResult> OnGetHistorialDocumentoAsync(
            int empresaId,
            int tipoDocumentoId)
        {

            if (!await _permisosComplianceService
            .PuedeVisualizarAsync(User))
            {
                return Forbid();
            }

            if (empresaId <= 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "El identificador de la empresa no es válido."
                });
            }

            if (tipoDocumentoId <= 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "El tipo de documento no es válido."
                });
            }

            bool empresaExiste = await _context.EbEmpresas
                .AsNoTracking()
                .AnyAsync(x => x.Id == empresaId);

            if (!empresaExiste)
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "No se encontró la empresa seleccionada."
                });
            }

            var tipoDocumento = await _context.EbTiposDocumento
                .IgnoreQueryFilters()
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == tipoDocumentoId);

            if (tipoDocumento == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "No se encontró el tipo documental."
                });
            }

            var versiones = await _context.EbDocumentos
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(x =>
                    x.EmpresaId == empresaId &&
                    x.TipoDocumentoId == tipoDocumentoId)
                .OrderByDescending(x => x.Version)
                .ThenByDescending(x => x.FechaCarga)
                .Select(x => new
                {
                    id = x.Id,
                    empresaId = x.EmpresaId,
                    tipoDocumentoId = x.TipoDocumentoId,

                    nombreOriginal = x.NombreOriginal,
                    extension = x.Extension,
                    mimeType = x.MimeType,
                    tamanoBytes = x.TamanoBytes,

                    version = x.Version,
                    fechaCarga = x.FechaCarga,
                    fechaVencimiento = x.FechaVencimiento,
                    estado = x.Estado,
                    observaciones = x.Observaciones,

                    esVersionActual = x.EsVersionActual,
                    eliminado = x.Eliminado,
                    fechaEliminacion = x.FechaEliminacion,

                    usuarioCargaId = x.UsuarioCargaId,
                    usuarioEliminacionId =
                        x.UsuarioEliminacionId
                })
                .ToListAsync();

            var historial = versiones
                .Select(x =>
                {
                    string situacion;

                    if (x.eliminado)
                    {
                        situacion = "Eliminada";
                    }
                    else if (x.esVersionActual)
                    {
                        situacion = "Actual";
                    }
                    else
                    {
                        situacion = "Reemplazada";
                    }

                    return new
                    {
                        x.id,
                        x.empresaId,
                        x.tipoDocumentoId,
                        x.nombreOriginal,
                        x.extension,
                        x.mimeType,
                        x.tamanoBytes,
                        x.version,
                        x.fechaCarga,
                        x.fechaVencimiento,
                        x.estado,
                        x.observaciones,
                        x.esVersionActual,
                        x.eliminado,
                        x.fechaEliminacion,
                        x.usuarioCargaId,
                        x.usuarioEliminacionId,
                        situacion
                    };
                })
                .ToList();

            return new JsonResult(new
            {
                success = true,

                tipoDocumento = new
                {
                    id = tipoDocumento.Id,
                    nombre = tipoDocumento.Nombre,
                    permiteMultiplesArchivos =
                        tipoDocumento.PermiteMultiplesArchivos
                },

                data = historial,

                resumen = new
                {
                    totalVersiones = historial.Count,

                    versionesActivas = historial.Count(x =>
                        !x.eliminado),

                    versionesEliminadas = historial.Count(x =>
                        x.eliminado),

                    versionActual = historial
                        .Where(x =>
                            x.esVersionActual &&
                            !x.eliminado)
                        .Select(x => (int?)x.version)
                        .FirstOrDefault()
                }
            });
        }

        // =====================================================
        // VISUALIZAR DOCUMENTO
        // GET ?handler=VisualizarDocumento&id=1
        // =====================================================
        public async Task<IActionResult> OnGetVisualizarDocumentoAsync(int id)
        {

            if (!await _permisosComplianceService
            .PuedeVisualizarAsync(User))
            {
                return Forbid();
            }

            if (id <= 0)
            {
                return NotFound();
            }

            var documento = await _context.EbDocumentos
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                x.Id == id &&
                !x.Eliminado);

            if (documento == null)
            {
                return NotFound();
            }

            string? rutaBaseDocumentos =
                _configuration[
                    "ExpedientesBancarios:RutaDocumentos"
                ];

            if (string.IsNullOrWhiteSpace(rutaBaseDocumentos))
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError
                );
            }

            string rutaBaseCompleta = Path.GetFullPath(
                rutaBaseDocumentos
            );

            string rutaFisica = Path.GetFullPath(
                Path.Combine(
                    rutaBaseCompleta,
                    documento.RutaArchivo
                        .Replace("/", Path.DirectorySeparatorChar.ToString())
                )
            );

            /*
             * Evita acceder a rutas fuera del directorio configurado.
             */
            string rutaBaseConSeparador =
                rutaBaseCompleta.TrimEnd(
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar
                ) + Path.DirectorySeparatorChar;

            if (!rutaFisica.StartsWith(
                    rutaBaseConSeparador,
                    StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest();
            }

            if (!System.IO.File.Exists(rutaFisica))
            {
                return NotFound();
            }

            string mimeType = string.IsNullOrWhiteSpace(
                documento.MimeType)
                    ? "application/octet-stream"
                    : documento.MimeType;

            RegistrarBitacoraDocumento(
            documento,
            EbAccionesBitacoraDocumento
            .Visualizacion,
            exitoso: true,
            detalle:
            "El documento fue visualizado.");

                await _context.SaveChangesAsync();

                return PhysicalFile(
                    rutaFisica,
                    mimeType
                );
        }

        // =====================================================
        // CARGAR DOCUMENTO
        // POST ?handler=CargarDocumento
        // =====================================================
        [RequestSizeLimit(110L * 1024L * 1024L)]
        public async Task<IActionResult> OnPostCargarDocumentoAsync(
        [FromForm] CargarDocumentoRequest request)
        {
            if (!await _permisosComplianceService
            .PuedeCrearCargarAsync(User))
            {
                return Forbid();
            }

            /*
             * El archivo puede pesar hasta 100 MB.
             * La solicitud permite 110 MB para dejar margen
             * al formulario multipart y sus encabezados.
             */
            const long tamanoMaximo =
                100L * 1024L * 1024L;

            string? rutaFisica = null;

            try
            {
                string[] extensionesPermitidas =
                {
            ".pdf",
            ".doc",
            ".docx",
            ".xls",
            ".xlsx",
            ".jpg",
            ".jpeg",
            ".png",
            ".webp",
            ".zip"
        };

                if (request.EmpresaId <= 0)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "La empresa seleccionada no es válida."
                    });
                }

                if (request.TipoDocumentoId <= 0)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "El tipo de documento no es válido."
                    });
                }

                if (request.Archivo == null ||
                    request.Archivo.Length <= 0)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "Selecciona un archivo para cargar.",
                        errors = new Dictionary<string, string[]>
                        {
                            ["Archivo"] = new[]
                            {
                        "El archivo es obligatorio."
                    }
                        }
                    });
                }

                if (request.Archivo.Length > tamanoMaximo)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message =
                            "El archivo excede el tamaño máximo permitido.",
                        errors = new Dictionary<string, string[]>
                        {
                            ["Archivo"] = new[]
                            {
                        "El archivo no puede superar los 100 MB."
                    }
                        }
                    });
                }

                string extension = Path
                    .GetExtension(request.Archivo.FileName)
                    .ToLowerInvariant();

                if (string.IsNullOrWhiteSpace(extension) ||
                    !extensionesPermitidas.Contains(extension))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message = "El tipo de archivo no está permitido.",
                        errors = new Dictionary<string, string[]>
                        {
                            ["Archivo"] = new[]
                            {
                        "Formatos permitidos: PDF, Word, Excel, " +
                        "JPG, PNG, WEBP y ZIP."
                    }
                        }
                    });
                }

                bool empresaExiste = await _context.EbEmpresas
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.Id == request.EmpresaId &&
                        !x.Deshabilitado);

                if (!empresaExiste)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message =
                            "No se encontró la empresa seleccionada."
                    });
                }

                var tipoDocumento = await _context.EbTiposDocumento
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.Id == request.TipoDocumentoId &&
                        !x.Eliminado &&
                        !x.Deshabilitado);

                if (tipoDocumento == null)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message =
                            "No se encontró el tipo de documento seleccionado."
                    });
                }

                /*
                 * Todos los documentos deben registrar
                 * obligatoriamente una fecha de vencimiento.
                 */
                if (!request.FechaVencimiento.HasValue)
                {
                    return new JsonResult(new
                    {
                        success = false,

                        message =
                            "La fecha de vencimiento es obligatoria.",

                        errors =
                            new Dictionary<string, string[]>
                            {
                                ["FechaVencimiento"] =
                                    new[]
                                    {
                        "Selecciona la fecha de vencimiento del documento."
                                    }
                            }
                    });
                }

                if (request.FechaVencimiento.HasValue &&
                    request.FechaVencimiento.Value.Date < DateTime.Today)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message =
                            "La fecha de vencimiento no puede ser anterior a hoy.",
                        errors = new Dictionary<string, string[]>
                        {
                            ["FechaVencimiento"] = new[]
                            {
                        "Selecciona una fecha válida."
                    }
                        }
                    });
                }

                string nombreOriginal = Path.GetFileName(
                    request.Archivo.FileName
                );

                string nombreAlmacenado =
                    $"{Guid.NewGuid():N}{extension}";

                string? rutaBaseDocumentos =
    _configuration[
        "ExpedientesBancarios:RutaDocumentos"
    ];

                if (string.IsNullOrWhiteSpace(rutaBaseDocumentos))
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message =
                            "No se encuentra configurada la ruta de almacenamiento documental."
                    })
                    {
                        StatusCode =
                            StatusCodes.Status500InternalServerError
                    };
                }

                rutaBaseDocumentos = Path.GetFullPath(
                    rutaBaseDocumentos
                );

                string directorioFisico = Path.Combine(
                    rutaBaseDocumentos,
                    request.EmpresaId.ToString(),
                    request.TipoDocumentoId.ToString()
                );

                Console.WriteLine(
                    $"Directorio documental: {directorioFisico}"
                );

                Directory.CreateDirectory(
                    directorioFisico
                );

                rutaFisica = Path.Combine(
                    directorioFisico,
                    nombreAlmacenado
                );

                /*
                 * Esta ruta es la referencia interna que se almacena
                 * en la base de datos. No incluye la ruta física raíz.
                 */
                string rutaRelativa = Path.Combine(
                    request.EmpresaId.ToString(),
                    request.TipoDocumentoId.ToString(),
                    nombreAlmacenado
                ).Replace("\\", "/");

                var versionesDocumento =
                    await _context.EbDocumentos
                        .IgnoreQueryFilters()
                        .Where(x =>
                            x.EmpresaId == request.EmpresaId &&
                            x.TipoDocumentoId ==
                                request.TipoDocumentoId)
                        .ToListAsync();

                int version = versionesDocumento.Any()
                    ? versionesDocumento.Max(x => x.Version) + 1
                    : 1;

                if (!tipoDocumento.PermiteMultiplesArchivos)
                {
                    foreach (
                        EbDocumento documentoAnterior
                        in versionesDocumento.Where(x =>
                            !x.Eliminado &&
                            x.EsVersionActual
                        )
                    )
                    {
                        documentoAnterior.EsVersionActual = false;
                    }
                }

                await using (
                    var stream = new FileStream(
                        rutaFisica,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.None
                    )
                )
                {
                    await request.Archivo.CopyToAsync(
                        stream
                    );
                }

                string estado = CalcularEstadoDocumento(
                    request.FechaVencimiento,
                    tipoDocumento.RequiereFechaVencimiento
                );

                var documento = new EbDocumento
                {
                    EmpresaId = request.EmpresaId,
                    TipoDocumentoId = request.TipoDocumentoId,

                    NombreOriginal = nombreOriginal,
                    NombreAlmacenado = nombreAlmacenado,
                    RutaArchivo = rutaRelativa,

                    Extension = extension,
                    MimeType = string.IsNullOrWhiteSpace(
                        request.Archivo.ContentType)
                            ? "application/octet-stream"
                            : request.Archivo.ContentType,

                    TamanoBytes = request.Archivo.Length,
                    Version = version,

                    FechaCarga = DateTime.Now,
                    FechaVencimiento =
                        request.FechaVencimiento,

                    Estado = estado,

                    Observaciones = NormalizarOpcional(
                        request.Observaciones
                    ),

                    EsVersionActual = true,
                    Eliminado = false,

                    UsuarioCargaId = ObtenerUsuarioId()
                };

                _context.EbDocumentos.Add(
                        documento
                    );

                /*
                 * Guardamos primero para obtener
                 * el Id del nuevo documento.
                 */
                await _context.SaveChangesAsync();

                string accionBitacora =
                    version > 1
                        ? EbAccionesBitacoraDocumento.NuevaVersion
                        : EbAccionesBitacoraDocumento.Carga;

                string detalleBitacora =
                    accionBitacora ==
                    EbAccionesBitacoraDocumento
                        .NuevaVersion
                        ? $"Se cargó la versión {documento.Version} del documento."
                        : "Se cargó el documento.";

                RegistrarBitacoraDocumento(
                    documento,
                    accionBitacora,
                    exitoso: true,
                    detalle: detalleBitacora
                );

                await _context.SaveChangesAsync();

                /*
                 * ==========================================================
                 * SINCRONIZACIÓN COMPLIANCE → EMPRESAS
                 * ==========================================================
                 */
                try
                {
                    await _documentoEmpresasComplianceService
                        .SincronizarDesdeComplianceAsync(
                            documento.Id
                        );
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        "======================================"
                    );

                    Console.WriteLine(
                        "ERROR SINCRONIZACIÓN COMPLIANCE → EMPRESAS"
                    );

                    Console.WriteLine(
                        ex.ToString()
                    );

                    Console.WriteLine(
                        "======================================"
                    );
                }

                return new JsonResult(new
                {
                    success = true,
                    message =
                        "El documento se cargó correctamente.",
                    id = documento.Id
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "======================================"
                );

                Console.WriteLine(
                    "ERROR AL CARGAR DOCUMENTO"
                );

                Console.WriteLine(
                    ex.ToString()
                );

                Console.WriteLine(
                    "======================================"
                );

                if (!string.IsNullOrWhiteSpace(rutaFisica) &&
                    System.IO.File.Exists(rutaFisica))
                {
                    try
                    {
                        System.IO.File.Delete(rutaFisica);
                    }
                    catch (Exception errorEliminar)
                    {
                        Console.WriteLine(
                            "No se pudo eliminar el archivo incompleto:"
                        );

                        Console.WriteLine(
                            errorEliminar.ToString()
                        );
                    }
                }

                return new JsonResult(new
                {
                    success = false,
                    message =
                        "Ocurrió un error al guardar el documento.",
                    detail = _environment.IsDevelopment()
                        ? ex.Message
                        : null
                })
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        private static string CalcularEstadoDocumento(
            DateTime? fechaVencimiento,
            bool requiereFechaVencimiento)
        {
            if (!requiereFechaVencimiento)
            {
                return "Cargado";
            }

            if (!fechaVencimiento.HasValue)
            {
                return "Cargado";
            }

            DateTime fechaActual = DateTime.Today;
            DateTime fechaLimite = fechaActual.AddDays(30);

            if (fechaVencimiento.Value.Date < fechaActual)
            {
                return "Vencido";
            }

            if (fechaVencimiento.Value.Date <= fechaLimite)
            {
                return "Próximo a vencer";
            }

            return "Vigente";
        }

        // =====================================================
        // VALIDACIONES
        // =====================================================
        private static Dictionary<string, string[]> ValidarRequest(
            EmpresaRequest request,
            bool requiereId)
        {
            var errores = new Dictionary<string, string[]>();

            if (requiereId && request.Id <= 0)
            {
                errores["Id"] = new[]
                {
                    "El identificador de la empresa no es válido."
                };
            }

            if (string.IsNullOrWhiteSpace(request.RazonSocial))
            {
                errores["RazonSocial"] = new[]
                {
                    "La razón social es obligatoria."
                };
            }
            else if (request.RazonSocial.Length > 250)
            {
                errores["RazonSocial"] = new[]
                {
                    "La razón social no puede exceder 250 caracteres."
                };
            }

            if (string.IsNullOrWhiteSpace(request.NombreCorto))
            {
                errores["NombreCorto"] = new[]
                {
                    "El nombre corto es obligatorio."
                };
            }
            else if (request.NombreCorto.Length > 150)
            {
                errores["NombreCorto"] = new[]
                {
                    "El nombre corto no puede exceder 150 caracteres."
                };
            }

            if (string.IsNullOrWhiteSpace(request.Rfc))
            {
                errores["Rfc"] = new[]
                {
                    "El RFC es obligatorio."
                };
            }
            else
            {
                var atributoRfc = new RegularExpressionAttribute(
                    @"^[A-ZÑ&]{3,4}\d{6}[A-Z0-9]{3}$");

                if (request.Rfc.Length is < 12 or > 13 ||
                    !atributoRfc.IsValid(request.Rfc))
                {
                    errores["Rfc"] = new[]
                    {
                        "El formato del RFC no es válido."
                    };
                }
            }

            if (!string.IsNullOrWhiteSpace(request.CorreoBancos))
            {
                var atributoCorreo = new EmailAddressAttribute();

                if (!atributoCorreo.IsValid(request.CorreoBancos))
                {
                    errores["CorreoBancos"] = new[]
                    {
                        "El correo electrónico no es válido."
                    };
                }
            }

            if (request.FechaConstitucion.HasValue &&
                request.FechaConstitucion.Value.Date > DateTime.Today)
            {
                errores["FechaConstitucion"] = new[]
                {
                    "La fecha de constitución no puede ser futura."
                };
            }

            if (!string.IsNullOrWhiteSpace(
                    request.NumeroEscritura) &&
                request.NumeroEscritura.Length > 200)
                        {
                            errores["NumeroEscritura"] = new[]
                            {
                    "El número de escritura y/o escrituras " +
                    "no puede exceder 200 caracteres."
                };
            }

            return errores;
        }

        private async Task SincronizarAccionistasEmpresaMaestraAsync(
            int complianceEmpresaId)
        {
            if (complianceEmpresaId <= 0)
            {
                return;
            }

            EbEmpresa? empresaCompliance =
                await _context.EbEmpresas
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.Id == complianceEmpresaId
                    );

            if (empresaCompliance == null)
            {
                return;
            }

            string rfc =
                empresaCompliance.Rfc?
                    .Trim()
                    .ToUpperInvariant()
                ?? string.Empty;

            if (string.IsNullOrWhiteSpace(rfc))
            {
                return;
            }

            Empresa? empresaMaestra =
                await _context.Set<Empresa>()
                    .FirstOrDefaultAsync(x =>
                        x.RFC != null &&
                        x.RFC.ToUpper() == rfc
                    );

            if (empresaMaestra == null)
            {
                return;
            }

            List<string> accionistas =
                await _context.EbAccionistas
                    .AsNoTracking()
                    .Where(x =>
                        x.EmpresaId == complianceEmpresaId &&
                        !x.Eliminado &&
                        !x.Deshabilitado
                    )
                    .OrderByDescending(x =>
                        x.PorcentajeParticipacion
                    )
                    .ThenBy(x =>
                        x.NombreCompleto
                    )
                    .Select(x =>
                        x.NombreCompleto
                    )
                    .ToListAsync();

            empresaMaestra.Accionista =
                accionistas.Count > 0
                    ? string.Join(", ", accionistas)
                    : string.Empty;
        }

        // =====================================================
        // LISTAR ACCIONISTAS DE UNA EMPRESA
        // GET ?handler=Accionistas&empresaId=1
        // =====================================================
        public async Task<IActionResult> OnGetAccionistasAsync(
            int empresaId)
        {
            if (!await _permisosComplianceService
                .PuedeVisualizarAsync(User))
            {
                return Forbid();
            }

            if (empresaId <= 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "El identificador de la empresa no es válido."
                });
            }

            /*
             * ==========================================================
             * 1. LOCALIZAR EMPRESA MAESTRA
             * ==========================================================
             *
             * empresaId corresponde a Empresa.Id
             */
            Empresa? empresaMaestra =
                await _context.Set<Empresa>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.Id == empresaId
                    );

            if (empresaMaestra == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "No se encontró la empresa solicitada."
                });
            }

            string rfcEmpresa =
                empresaMaestra.RFC?
                    .Trim()
                    .ToUpperInvariant()
                ?? string.Empty;

            /*
             * ==========================================================
             * 2. LOCALIZAR EbEmpresa POR RFC
             * ==========================================================
             */
            EbEmpresa? empresaCompliance =
                null;

            if (!string.IsNullOrWhiteSpace(rfcEmpresa))
            {
                empresaCompliance =
                    await _context.EbEmpresas
                        .IgnoreQueryFilters()
                        .FirstOrDefaultAsync(x =>
                            x.Rfc != null &&
                            x.Rfc.Trim().ToUpper() ==
                            rfcEmpresa
                        );
            }

            /*
             * ==========================================================
             * 3. CREAR EbEmpresa SI TODAVÍA NO EXISTE
             * ==========================================================
             */
            if (empresaCompliance == null)
            {
                string nivelNombre =
                    string.Empty;

                if (empresaMaestra.NivelId.HasValue)
                {
                    Nivel? nivel =
                        await _context.Set<Nivel>()
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x =>
                                x.Id ==
                                empresaMaestra.NivelId.Value
                            );

                    nivelNombre =
                        nivel?.Nombre ??
                        string.Empty;
                }

                empresaCompliance =
                    new EbEmpresa
                    {
                        RazonSocial =
                            empresaMaestra.RazonSocial ??
                            string.Empty,

                        NombreCorto =
                            string.Empty,

                        Rfc =
                            rfcEmpresa,

                        Nivel =
                            nivelNombre,

                        ActividadComercial =
                            null,

                        TelefonoBancos =
                            empresaMaestra.Telefono,

                        CorreoBancos =
                            empresaMaestra.CorreoBancos,

                        FechaConstitucion =
                            empresaMaestra.FechaConstitucion,

                        NumeroEscritura =
                            null,

                        DomicilioFiscal =
                            empresaMaestra.DomicilioFiscal,

                        Observaciones =
                            null,

                        Deshabilitado =
                            empresaMaestra.Deshabilitado != 0,

                        Eliminado =
                            false,

                        FechaCreacion =
                            DateTime.Now,

                        UsuarioCreacionId =
                            ObtenerUsuarioId()
                    };

                _context.EbEmpresas.Add(
                    empresaCompliance
                );

                await _context.SaveChangesAsync();
            }

            /*
             * Este es el ID que realmente utiliza EbAccionista.EmpresaId
             */
            int complianceId =
                empresaCompliance.Id;

            /*
             * ==========================================================
             * 4. IMPORTAR ACCIONISTA(S) DESDE Empresa.Accionista
             * ==========================================================
             */
            if (!string.IsNullOrWhiteSpace(
                empresaMaestra.Accionista))
            {
                string[] nombresEmpresa =
                    empresaMaestra.Accionista
                        .Split(
                            new[]
                            {
                        ',',
                        ';',
                        '\n',
                        '\r'
                            },
                            StringSplitOptions.RemoveEmptyEntries
                        )
                        .Select(x =>
                            x.Trim()
                        )
                        .Where(x =>
                            !string.IsNullOrWhiteSpace(x)
                        )
                        .Distinct(
                            StringComparer.OrdinalIgnoreCase
                        )
                        .ToArray();

                List<EbAccionista> accionistasExistentes =
                    await _context.EbAccionistas
                        .IgnoreQueryFilters()
                        .Where(x =>
                            x.EmpresaId ==
                            complianceId &&
                            !x.Eliminado
                        )
                        .ToListAsync();

                bool seAgrego =
                    false;

                string usuarioId =
                    ObtenerUsuarioId();

                foreach (string nombreEmpresa in nombresEmpresa)
                {
                    bool yaExiste =
                        accionistasExistentes
                            .Any(x =>
                                string.Equals(
                                    x.NombreCompleto?
                                        .Trim(),

                                    nombreEmpresa,

                                    StringComparison
                                        .OrdinalIgnoreCase
                                )
                            );

                    if (yaExiste)
                    {
                        continue;
                    }

                    EbAccionista nuevoAccionista =
                        new EbAccionista
                        {
                            EmpresaId =
                                complianceId,

                            NombreCompleto =
                                nombreEmpresa,

                            Rfc =
                                string.Empty,

                            PorcentajeParticipacion =
                            nombresEmpresa.Length == 1
                                ? 100m
                                : 0m,

                            Nacionalidad =
                                string.Empty,

                            EsRepresentanteLegal =
                                false,

                            Deshabilitado =
                                false,

                            Eliminado =
                                false,

                            FechaCreacion =
                                DateTime.Now,

                            UsuarioCreacionId =
                                usuarioId
                        };

                    _context.EbAccionistas.Add(
                        nuevoAccionista
                    );

                    accionistasExistentes.Add(
                        nuevoAccionista
                    );

                    seAgrego =
                        true;
                }

                if (seAgrego)
                {
                    await _context.SaveChangesAsync();
                }

                List<EbAccionista> accionistasActivos =
                    await _context.EbAccionistas
                        .Where(x =>
                            x.EmpresaId == complianceId &&
                            !x.Eliminado &&
                            !x.Deshabilitado
                        )
                        .ToListAsync();

                if (
                    nombresEmpresa.Length == 1 &&
                    accionistasActivos.Count == 1 &&
                    accionistasActivos[0].PorcentajeParticipacion == 0m &&
                    string.Equals(
                        accionistasActivos[0].NombreCompleto?.Trim(),
                        nombresEmpresa[0],
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    accionistasActivos[0].PorcentajeParticipacion =
                        100m;

                    accionistasActivos[0].FechaActualizacion =
                        DateTime.Now;

                    accionistasActivos[0].UsuarioActualizacionId =
                        ObtenerUsuarioId();

                    await _context.SaveChangesAsync();
                }
            }

            /*
             * ==========================================================
             * 5. LISTAR ACCIONISTAS
             * ==========================================================
             */
            var accionistas =
                await _context.EbAccionistas
                    .AsNoTracking()
                    .Where(x =>
                        x.EmpresaId ==
                        complianceId &&
                        !x.Eliminado
                    )
                    .OrderByDescending(x =>
                        x.PorcentajeParticipacion
                    )
                    .ThenBy(x =>
                        x.NombreCompleto
                    )
                    .Select(x => new
                    {
                        id =
                            x.Id,

                        empresaId =
                            x.EmpresaId,

                        nombreCompleto =
                            x.NombreCompleto,

                        rfc =
                            x.Rfc,

                        porcentajeParticipacion =
                            x.PorcentajeParticipacion,

                        nacionalidad =
                            x.Nacionalidad,

                        esRepresentanteLegal =
                            x.EsRepresentanteLegal,

                        deshabilitado =
                            x.Deshabilitado,

                        fechaCreacion =
                            x.FechaCreacion
                    })
                    .ToListAsync();

            /*
             * ==========================================================
             * 6. RESUMEN
             * ==========================================================
             */
            decimal porcentajeTotal =
                accionistas
                    .Where(x =>
                        !x.deshabilitado
                    )
                    .Sum(x =>
                        x.porcentajeParticipacion
                    );

            decimal porcentajeDisponible =
                100m -
                porcentajeTotal;

            if (porcentajeDisponible < 0m)
            {
                porcentajeDisponible =
                    0m;
            }

            /*
             * ==========================================================
             * 7. RESPUESTA
             * ==========================================================
             */
            return new JsonResult(new
            {
                success = true,

                /*
                 * ID maestro
                 */
                empresaId =
                    empresaMaestra.Id,

                /*
                 * ID interno Compliance.
                 * Este lo necesita el JS para Crear/Editar.
                 */
                complianceId =
                    complianceId,

                data =
                    accionistas,

                resumen = new
                {
                    totalAccionistas =
                        accionistas.Count,

                    porcentajeTotal,

                    porcentajeDisponible
                }
            });
        }

        // =====================================================
        // CONSULTAR ACCIONISTA
        // GET ?handler=Accionista&id=1
        // =====================================================
        public async Task<IActionResult> OnGetAccionistaAsync(int id)
        {

            if (!await _permisosComplianceService
            .PuedeVisualizarAsync(User))
            {
                return Forbid();
            }

            if (id <= 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "El identificador del accionista no es válido."
                });
            }

            var accionista = await _context.EbAccionistas
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (accionista == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No se encontró el accionista solicitado."
                });
            }

            return new JsonResult(new
            {
                success = true,
                data = new
                {
                    id = accionista.Id,
                    empresaId = accionista.EmpresaId,
                    nombreCompleto = accionista.NombreCompleto,
                    rfc = accionista.Rfc,
                    porcentajeParticipacion =
                        accionista.PorcentajeParticipacion,
                    nacionalidad = accionista.Nacionalidad,
                    esRepresentanteLegal =
                        accionista.EsRepresentanteLegal,
                    deshabilitado = accionista.Deshabilitado
                }
            });
        }

        // =====================================================
        // CREAR ACCIONISTA
        // POST ?handler=CrearAccionista
        // =====================================================
        public async Task<IActionResult> OnPostCrearAccionistaAsync(
            [FromBody] AccionistaRequest request)
        {

            if (!await _permisosComplianceService
            .PuedeCrearCargarAsync(User))
            {
                return Forbid();
            }

            NormalizarAccionistaRequest(request);

            Dictionary<string, string[]> errores =
                await ValidarAccionistaRequestAsync(
                    request,
                    requiereId: false);

            if (errores.Any())
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Revisa la información del accionista.",
                    errors = errores
                });
            }

            bool empresaExiste = await _context.EbEmpresas
                .AnyAsync(x => x.Id == request.EmpresaId);

            if (!empresaExiste)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No se encontró la empresa seleccionada."
                });
            }

            string usuarioId = ObtenerUsuarioId();

            var accionista = new EbAccionista
            {
                EmpresaId = request.EmpresaId,
                NombreCompleto = request.NombreCompleto,
                Rfc = request.Rfc,
                PorcentajeParticipacion =
                    request.PorcentajeParticipacion,
                Nacionalidad = request.Nacionalidad,
                EsRepresentanteLegal =
                    request.EsRepresentanteLegal,
                Deshabilitado = false,
                Eliminado = false,
                FechaCreacion = DateTime.Now,
                UsuarioCreacionId = usuarioId
            };

            _context.EbAccionistas.Add(
                accionista
            );

            await _context.SaveChangesAsync();

            /*
             * Sincronizar el resumen de accionistas
             * con el módulo principal Empresas.
             */
            await SincronizarAccionistasEmpresaMaestraAsync(
                request.EmpresaId
            );

            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                success = true,
                message = "El accionista se registró correctamente.",
                id = accionista.Id
            });
        }

        // =====================================================
        // EDITAR ACCIONISTA
        // POST ?handler=EditarAccionista
        // =====================================================
        public async Task<IActionResult> OnPostEditarAccionistaAsync(
            [FromBody] AccionistaRequest request)
        {

            if (!await _permisosComplianceService
        .PuedeModificarAsync(User))
            {
                return Forbid();
            }

            NormalizarAccionistaRequest(request);

            Dictionary<string, string[]> errores =
                await ValidarAccionistaRequestAsync(
                    request,
                    requiereId: true);

            if (errores.Any())
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Revisa la información del accionista.",
                    errors = errores
                });
            }

            var accionista = await _context.EbAccionistas
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (accionista == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No se encontró el accionista que deseas editar."
                });
            }

            if (accionista.EmpresaId != request.EmpresaId)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "El accionista no pertenece a la empresa seleccionada."
                });
            }

            accionista.NombreCompleto = request.NombreCompleto;
            accionista.Rfc = request.Rfc;
            accionista.PorcentajeParticipacion =
                request.PorcentajeParticipacion;
            accionista.Nacionalidad = request.Nacionalidad;
            accionista.EsRepresentanteLegal =
                request.EsRepresentanteLegal;
            accionista.FechaActualizacion = DateTime.Now;
            accionista.UsuarioActualizacionId =
                ObtenerUsuarioId();

            await _context.SaveChangesAsync();

            /*
             * Actualizar el resumen de accionistas
             * en Empresa.Accionista.
             */
            await SincronizarAccionistasEmpresaMaestraAsync(
                accionista.EmpresaId
            );

            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                success = true,
                message = "El accionista se actualizó correctamente."
            });
        }

        // =====================================================
        // HABILITAR / DESHABILITAR ACCIONISTA
        // POST ?handler=CambiarEstatusAccionista
        // =====================================================
        public async Task<IActionResult>
            OnPostCambiarEstatusAccionistaAsync(
                [FromBody] AccionistaIdRequest request)
        {

            if (!await _permisosComplianceService
            .PuedeModificarAsync(User))
            {
                return Forbid();
            }

            if (request.Id <= 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "El identificador del accionista no es válido."
                });
            }

            var accionista = await _context.EbAccionistas
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (accionista == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No se encontró el accionista solicitado."
                });
            }

            accionista.Deshabilitado =
            !accionista.Deshabilitado;

            accionista.FechaActualizacion =
                DateTime.Now;

            accionista.UsuarioActualizacionId =
                ObtenerUsuarioId();

            await _context.SaveChangesAsync();

            await SincronizarAccionistasEmpresaMaestraAsync(
                accionista.EmpresaId
            );

            await _context.SaveChangesAsync();
            string mensaje = accionista.Deshabilitado
                ? "El accionista se deshabilitó correctamente."
                : "El accionista se habilitó correctamente.";

            return new JsonResult(new
            {
                success = true,
                message = mensaje,
                deshabilitado = accionista.Deshabilitado
            });
        }

        // =====================================================
        // ELIMINAR ACCIONISTA LÓGICAMENTE
        // POST ?handler=EliminarAccionista
        // =====================================================
        public async Task<IActionResult> OnPostEliminarAccionistaAsync(
            [FromBody] AccionistaIdRequest request)
        {

            if (!await _permisosComplianceService
            .PuedeEliminarAsync(User))
            {
                return Forbid();
            }

            if (request.Id <= 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "El identificador del accionista no es válido."
                });
            }

            var accionista = await _context.EbAccionistas
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (accionista == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No se encontró el accionista solicitado."
                });
            }

            accionista.Eliminado = true;
            accionista.Deshabilitado = true;
            accionista.FechaActualizacion = DateTime.Now;
            accionista.UsuarioActualizacionId =
                ObtenerUsuarioId();

            await _context.SaveChangesAsync();

            await SincronizarAccionistasEmpresaMaestraAsync(
                accionista.EmpresaId
            );

            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                success = true,
                message = "El accionista se eliminó correctamente."
            });
        }

        private async Task<Dictionary<string, string[]>>
    ValidarAccionistaRequestAsync(
        AccionistaRequest request,
        bool requiereId)
        {
            var errores = new Dictionary<string, string[]>();

            if (requiereId && request.Id <= 0)
            {
                errores["Id"] = new[]
                {
            "El identificador del accionista no es válido."
        };
            }

            if (request.EmpresaId <= 0)
            {
                errores["EmpresaId"] = new[]
                {
            "La empresa es obligatoria."
        };
            }

            if (string.IsNullOrWhiteSpace(request.NombreCompleto))
            {
                errores["NombreCompleto"] = new[]
                {
            "El nombre completo es obligatorio."
        };
            }
            else if (request.NombreCompleto.Length > 250)
            {
                errores["NombreCompleto"] = new[]
                {
            "El nombre no puede exceder 250 caracteres."
        };
            }

            if (!string.IsNullOrWhiteSpace(request.Rfc))
            {
                var validadorRfc = new RegularExpressionAttribute(
                    @"^[A-ZÑ&]{3,4}\d{6}[A-Z0-9]{3}$");

                if (request.Rfc.Length is < 12 or > 13 ||
                    !validadorRfc.IsValid(request.Rfc))
                {
                    errores["Rfc"] = new[]
                    {
                "El formato del RFC no es válido."
            };
                }
            }

            if (request.PorcentajeParticipacion <= 0)
            {
                errores["PorcentajeParticipacion"] = new[]
                {
            "El porcentaje debe ser mayor que cero."
        };
            }
            else if (request.PorcentajeParticipacion > 100)
            {
                errores["PorcentajeParticipacion"] = new[]
                {
            "El porcentaje no puede ser mayor que 100."
        };
            }

            if (!string.IsNullOrWhiteSpace(request.Nacionalidad) &&
                request.Nacionalidad.Length > 100)
            {
                errores["Nacionalidad"] = new[]
                {
            "La nacionalidad no puede exceder 100 caracteres."
        };
            }

            if (request.EmpresaId > 0 &&
                request.PorcentajeParticipacion > 0 &&
                request.PorcentajeParticipacion <= 100)
            {
                decimal porcentajeRegistrado =
                    await _context.EbAccionistas
                        .AsNoTracking()
                        .Where(x =>
                            x.EmpresaId == request.EmpresaId &&
                            x.Id != request.Id &&
                            !x.Deshabilitado)
                        .SumAsync(x =>
                            (decimal?)x.PorcentajeParticipacion)
                        ?? 0m;

                decimal porcentajeFinal =
                    porcentajeRegistrado +
                    request.PorcentajeParticipacion;

                if (porcentajeFinal > 100m)
                {
                    decimal porcentajeDisponible =
                        100m - porcentajeRegistrado;

                    errores["PorcentajeParticipacion"] = new[]
                    {
                $"La participación total no puede superar el 100 %. " +
                $"Actualmente hay {porcentajeRegistrado:N4} % registrado " +
                $"y quedan {porcentajeDisponible:N4} % disponibles."
            };
                }
            }

            return errores;
        }

        // =====================================================
        // DESCARGAR DOCUMENTO
        // GET ?handler=DescargarDocumento&id=1&banco=BBVA
        // =====================================================
        public async Task<IActionResult>
            OnGetDescargarDocumentoAsync(
                int id,
                string? banco)
        {

            if (!await _permisosComplianceService
            .PuedeDescargarAsync(User))
            {
                return Forbid();
            }

            if (id <= 0)
            {
                return BadRequest(
                    "El identificador del documento no es válido."
                );
            }

            string bancoNormalizado =
                banco?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(bancoNormalizado))
            {
                return BadRequest(
                    "Debes seleccionar el banco asociado a la descarga."
                );
            }

            if (bancoNormalizado.Length > 50)
            {
                return BadRequest(
                    "El nombre del banco no puede superar los 50 caracteres."
                );
            }

            /*
             * Lista controlada para evitar valores arbitrarios
             * enviados directamente por URL.
             */
            HashSet<string> bancosPermitidos =
                new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase
                )
                {
            "BBVA",
            "Banorte",
            "Santander",
            "HSBC",
            "Scotiabank",
            "Citibanamex",
            "Banco Azteca",
            "Inbursa",
            "Banca Mifel",
            "Monex",
            "Intercam",
            "BanBajío",
            "Multiva",
            "Otro banco"
                };

            if (!bancosPermitidos.Contains(bancoNormalizado))
            {
                return BadRequest(
                    "El banco seleccionado no es válido."
                );
            }

            /*
             * Conserva el nombre exactamente como está definido
             * en el catálogo para evitar diferencias de mayúsculas.
             */
            bancoNormalizado =
                bancosPermitidos.First(
                    item => item.Equals(
                        bancoNormalizado,
                        StringComparison.OrdinalIgnoreCase
                    )
                );

            var documento =
                await _context.EbDocumentos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    !x.Eliminado);

            if (documento == null)
            {
                return NotFound(
                    "No se encontró el documento solicitado."
                );
            }

            string? rutaBaseDocumentos =
                _configuration[
                    "ExpedientesBancarios:RutaDocumentos"
                ];

            if (string.IsNullOrWhiteSpace(rutaBaseDocumentos))
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "No está configurada la ruta documental."
                );
            }

            string rutaBaseCompleta =
                Path.GetFullPath(
                    rutaBaseDocumentos
                );

            string rutaFisica =
                Path.GetFullPath(
                    Path.Combine(
                        rutaBaseCompleta,
                        documento.RutaArchivo.Replace(
                            "/",
                            Path.DirectorySeparatorChar.ToString()
                        )
                    )
                );

            string rutaBaseConSeparador =
                rutaBaseCompleta.TrimEnd(
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar
                ) +
                Path.DirectorySeparatorChar;

            if (!rutaFisica.StartsWith(
                    rutaBaseConSeparador,
                    StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(
                    "La ruta del documento no es válida."
                );
            }

            if (!System.IO.File.Exists(rutaFisica))
            {
                return NotFound(
                    "El archivo físico no fue encontrado."
                );
            }

            string mimeType =
                string.IsNullOrWhiteSpace(
                    documento.MimeType
                )
                    ? "application/octet-stream"
                    : documento.MimeType;

            RegistrarBitacoraDocumento(
                documento,
                EbAccionesBitacoraDocumento.Descarga,
                exitoso: true,
                detalle:
                    $"El documento fue descargado para uso en {bancoNormalizado}.",
                banco: bancoNormalizado
            );

            await _context.SaveChangesAsync();

            return PhysicalFile(
                rutaFisica,
                mimeType,
                documento.NombreOriginal
            );
        }

        private static void NormalizarAccionistaRequest(AccionistaRequest request)
        {
            request.NombreCompleto =
                request.NombreCompleto?.Trim() ?? string.Empty;

            request.Rfc = NormalizarOpcional(request.Rfc)?
                .ToUpperInvariant();

            request.Nacionalidad =
                NormalizarOpcional(request.Nacionalidad);
        }

        private static void NormalizarRequest(EmpresaRequest request)
        {
            request.RazonSocial =
                request.RazonSocial?.Trim() ?? string.Empty;

            request.NombreCorto =
                request.NombreCorto?.Trim() ?? string.Empty;

            request.Rfc =
                request.Rfc?.Trim().ToUpperInvariant() ?? string.Empty;

            request.Nivel = NormalizarOpcional(request.Nivel);

            request.ActividadComercial =
                NormalizarOpcional(request.ActividadComercial);

            request.TelefonoBancos =
                NormalizarOpcional(request.TelefonoBancos);

            request.CorreoBancos =
                NormalizarOpcional(request.CorreoBancos);

            request.NumeroEscritura =
                NormalizarOpcional(request.NumeroEscritura);

            request.DomicilioFiscal =
                NormalizarOpcional(request.DomicilioFiscal);

            request.Observaciones =
                NormalizarOpcional(request.Observaciones);
        }

        private static string? NormalizarOpcional(string? valor)
        {
            string? resultado = valor?.Trim();

            return string.IsNullOrWhiteSpace(resultado)
                ? null
                : resultado;
        }

        // =====================================================
        // REGISTRAR EVENTO EN BITÁCORA DE EMPRESAS
        // =====================================================
        private void RegistrarBitacoraEmpresa(
            EbEmpresa empresa,
            string accion,
            bool exitoso = true,
            string? detalle = null)
        {
            if (empresa == null)
            {
                return;
            }

            string usuarioId =
                ObtenerUsuarioId();

            string nombreUsuario =
                User.Identity?.Name ??
                User.FindFirstValue(
                    ClaimTypes.Email
                ) ??
                usuarioId;

            string? direccionIp =
                HttpContext.Connection
                    .RemoteIpAddress?
                    .ToString();

            if (direccionIp == "::1")
            {
                direccionIp = "127.0.0.1";
            }

            string? navegador =
                HttpContext.Request
                    .Headers["User-Agent"]
                    .FirstOrDefault();

            var bitacora =
                new EbBitacoraEmpresa
                {
                    EmpresaId =
                        empresa.Id,

                    Accion =
                        accion,

                    UsuarioId =
                        LimitarTexto(
                            usuarioId,
                            450
                        ) ??
                        "SYSTEM",

                    NombreUsuario =
                        LimitarTexto(
                            nombreUsuario,
                            250
                        ) ??
                        "Usuario no identificado",

                    FechaEvento =
                        DateTime.Now,

                    DireccionIp =
                        LimitarTexto(
                            direccionIp,
                            64
                        ),

                    Navegador =
                        LimitarTexto(
                            navegador,
                            1000
                        ),

                    Exitoso =
                        exitoso,

                    Detalle =
                        LimitarTexto(
                            detalle,
                            2000
                        )
                };

            _context.EbBitacoraEmpresas.Add(
                bitacora
            );
        }

        // =====================================================
        // REGISTRAR EVENTO EN BITÁCORA DOCUMENTAL
        // =====================================================
        private void RegistrarBitacoraDocumento(
            EbDocumento documento,
            string accion,
            bool exitoso = true,
            string? detalle = null,
            string? banco = null)
        {
            if (documento == null)
            {
                return;
            }

            string usuarioId = ObtenerUsuarioId();

            string nombreUsuario =
                User.Identity?.Name ??
                User.FindFirstValue(ClaimTypes.Email) ??
                usuarioId;

            string? direccionIp =
                HttpContext.Connection
                    .RemoteIpAddress?
                    .ToString();

            if (direccionIp == "::1")
            {
                direccionIp = "127.0.0.1";
            }

            string? navegador =
                HttpContext.Request
                    .Headers["User-Agent"]
                    .FirstOrDefault();

            var bitacora = new EbBitacoraDocumento
            {
                EmpresaId = documento.EmpresaId,
                DocumentoId = documento.Id,
                TipoDocumentoId =
                    documento.TipoDocumentoId,

                Accion = accion,

                UsuarioId = LimitarTexto(
                    usuarioId,
                    450
                ),

                NombreUsuario = LimitarTexto(
                    nombreUsuario,
                    250
                ),

                NombreDocumento = LimitarTexto(
                    documento.NombreOriginal,
                    250
                ),

                Banco = LimitarTexto(
                    banco,
                    50
                ),

                FechaEvento = DateTime.Now,

                DireccionIp = LimitarTexto(
                    direccionIp,
                    64
                ),

                Navegador = LimitarTexto(
                    navegador,
                    1000
                ),

                Exitoso = exitoso,

                Detalle = LimitarTexto(
                    detalle,
                    1000
                ),

                VersionDocumento =
                    documento.Version
            };

            _context.EbBitacoraDocumentos.Add(
                bitacora
            );
        }

        private static string? LimitarTexto(
            string? valor,
            int longitudMaxima)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return null;
            }

            string resultado = valor.Trim();

            return resultado.Length <= longitudMaxima
                ? resultado
                : resultado[..longitudMaxima];
        }

        private string ObtenerUsuarioId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? "SYSTEM";
        }

        // =====================================================
        // ELIMINAR DOCUMENTO LÓGICAMENTE
        // POST ?handler=EliminarDocumento
        // =====================================================
        public async Task<IActionResult> OnPostEliminarDocumentoAsync(
            [FromBody] DocumentoIdRequest request)
        {
            if (request.Id <= 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "El identificador del documento no es válido."
                });
            }

            await using var transaccion =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var documento = await _context.EbDocumentos
                    .FirstOrDefaultAsync(x =>
                        x.Id == request.Id &&
                        !x.Eliminado);

                if (documento == null)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message =
                            "No se encontró el documento seleccionado."
                    });
                }

                var tipoDocumento = await _context.EbTiposDocumento
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.Id == documento.TipoDocumentoId);

                if (tipoDocumento == null)
                {
                    return new JsonResult(new
                    {
                        success = false,
                        message =
                            "No se encontró el tipo documental relacionado."
                    });
                }

                bool eraVersionActual =
                    documento.EsVersionActual;

                documento.Eliminado = true;
                documento.EsVersionActual = false;
                documento.FechaEliminacion = DateTime.Now;
                documento.UsuarioEliminacionId =
                    ObtenerUsuarioId();

                /*
                 * En tipos que no permiten múltiples archivos,
                 * recuperamos la versión anterior más reciente.
                 */
                int? versionRestauradaId = null;

                if (eraVersionActual &&
                    !tipoDocumento.PermiteMultiplesArchivos)
                {
                    var versionAnterior =
                        await _context.EbDocumentos
                            .Where(x =>
                                x.EmpresaId == documento.EmpresaId &&
                                x.TipoDocumentoId ==
                                    documento.TipoDocumentoId &&
                                x.Id != documento.Id &&
                                !x.Eliminado)
                            .OrderByDescending(x => x.Version)
                            .ThenByDescending(x => x.FechaCarga)
                            .FirstOrDefaultAsync();

                    if (versionAnterior != null)
                    {
                        versionAnterior.EsVersionActual =
                            true;

                        versionRestauradaId =
                            versionAnterior.Id;

                        RegistrarBitacoraDocumento(
                            versionAnterior,
                            EbAccionesBitacoraDocumento
                                .Restauracion,
                            exitoso: true,
                            detalle:
                                $"Se restauró automáticamente la versión {versionAnterior.Version}."
                        );
                    }
                }

                RegistrarBitacoraDocumento(
                    documento,
                    EbAccionesBitacoraDocumento
                        .Eliminacion,
                    exitoso: true,
                    detalle:
                        $"Se eliminó lógicamente la versión {documento.Version}."
                );

                await _context.SaveChangesAsync();
                await transaccion.CommitAsync();

                return new JsonResult(new
                {
                    success = true,

                    message = versionRestauradaId.HasValue
                        ? "El archivo se eliminó y se restauró la versión anterior."
                        : "El archivo se eliminó correctamente.",

                    versionRestauradaId
                });
            }
            catch (Exception ex)
            {
                await transaccion.RollbackAsync();

                Console.WriteLine(
                    "ERROR AL ELIMINAR DOCUMENTO"
                );

                Console.WriteLine(
                    ex.ToString()
                );

                return new JsonResult(new
                {
                    success = false,
                    message =
                        "Ocurrió un error al eliminar el documento.",
                    detail = _environment.IsDevelopment()
                        ? ex.Message
                        : null
                })
                {
                    StatusCode =
                        StatusCodes.Status500InternalServerError
                };
            }
        }

        public class DocumentoIdRequest
        {
            public int Id { get; set; }
        }

        // =====================================================
        // REQUESTS AJAX
        // =====================================================
        public class EmpresaRequest
        {
            public int Id { get; set; }

            public string RazonSocial { get; set; } = string.Empty;

            public string NombreCorto { get; set; } = string.Empty;

            public string Rfc { get; set; } = string.Empty;

            public string? Nivel { get; set; }

            public string? ActividadComercial { get; set; }

            public string? TelefonoBancos { get; set; }

            public string? CorreoBancos { get; set; }

            public DateTime? FechaConstitucion { get; set; }

            public string? NumeroEscritura { get; set; }

            public string? DomicilioFiscal { get; set; }

            public string? Observaciones { get; set; }
        }

        public class EmpresaIdRequest
        {
            public int Id { get; set; }
        }

        public class AccionistaRequest
        {
            public int Id { get; set; }

            public int EmpresaId { get; set; }

            public string NombreCompleto { get; set; }
                = string.Empty;

            public string? Rfc { get; set; }

            public decimal PorcentajeParticipacion { get; set; }

            public string? Nacionalidad { get; set; }

            public bool EsRepresentanteLegal { get; set; }
        }

        public class AccionistaIdRequest
        {
            public int Id { get; set; }
        }

        public class CargarDocumentoRequest
        {
            public int EmpresaId { get; set; }

            public int TipoDocumentoId { get; set; }

            public IFormFile? Archivo { get; set; }

            public DateTime? FechaVencimiento { get; set; }

            public string? Observaciones { get; set; }
        }

        public class GuardarEmpresasPermisoUsuarioRequest
        {
            public string UsuarioId
            {
                get;
                set;
            } = string.Empty;

            public List<int> EmpresaIds
            {
                get;
                set;
            } = new();
        }

        public class GuardarPermisosComplianceRequest
        {
            public List<PermisoComplianceUsuarioRequest>
                Permisos
            {
                get;
                set;
            } = new();
        }

        public class PermisoComplianceUsuarioRequest
        {
            public string UsuarioId
            {
                get;
                set;
            } = string.Empty;

            public bool PuedeVisualizar
            {
                get;
                set;
            }

            public bool PuedeCrearCargar
            {
                get;
                set;
            }

            public bool PuedeModificar
            {
                get;
                set;
            }

            public bool PuedeEliminar
            {
                get;
                set;
            }

            public bool PuedeDescargar
            {
                get;
                set;
            }
        }

        public class UsuarioPermisoComplianceResponse
        {
            public string Id
            {
                get;
                set;
            } = string.Empty;

            public string Nombre
            {
                get;
                set;
            } = string.Empty;

            public string Correo
            {
                get;
                set;
            } = string.Empty;

            public string[] Roles
            {
                get;
                set;
            } = Array.Empty<string>();

            public bool EsAdministrador
            {
                get;
                set;
            }

            public bool PuedeEditarPermisos
            {
                get;
                set;
            }

            public bool PuedeVisualizar
            {
                get;
                set;
            }

            public bool PuedeCrearCargar
            {
                get;
                set;
            }

            public bool PuedeModificar
            {
                get;
                set;
            }

            public bool PuedeEliminar
            {
                get;
                set;
            }

            public bool PuedeDescargar
            {
                get;
                set;
            }

            public int NumeroEmpresas
            {
                get;
                set;
            }
        }

        public int NumeroEmpresas
        {
            get;
            set;
        }
    }
}