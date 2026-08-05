using ERPSEI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Areas.Catalogos.Pages.Metricas
{
    [Authorize(Roles = "Administrador,Master")]
    public class MetricaModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public MetricaModel(
            ApplicationDbContext db)
        {
            _db = db;
        }

        public class UsuarioActivoDto
        {
            public string Usuario { get; set; } =
                string.Empty;

            public int Total { get; set; }
        }

        public class ModuloVisitadoDto
        {
            public string Modulo { get; set; } =
                string.Empty;

            public int Total { get; set; }
        }

        public class ActividadGeneralDto
        {
            public string UserId { get; set; } =
                string.Empty;

            public string UserName { get; set; } =
                string.Empty;

            public string NombreEmpleado { get; set; } =
                string.Empty;

            public string TipoEvento { get; set; } =
                string.Empty;

            public string Modulo { get; set; } =
                string.Empty;

            public string Ruta { get; set; } =
                string.Empty;

            public DateTime FechaHora { get; set; }

            public string Ip { get; set; } =
                string.Empty;

            public decimal? Latitud { get; set; }

            public decimal? Longitud { get; set; }
        }

        public int LoginsHoy { get; set; }

        public int UsuariosActivosHoy { get; set; }

        public int VisitasMes { get; set; }

        public string ModuloMasUsado { get; set; } =
            "Sin datos";

        public List<ActividadGeneralDto>
            UltimosAccesos
        {
            get;
            set;
        } = new();

        public object MetricasJson { get; set; } =
            new();

        public List<UsuarioActivoDto>
            UsuariosMasActivos
        {
            get;
            set;
        } = new();

        public List<ModuloVisitadoDto>
            ModulosMasVisitados
        {
            get;
            set;
        } = new();

        public async Task OnGetAsync()
        {
            DateTime hoy =
                DateTime.Today;

            DateTime manana =
                hoy.AddDays(1);

            DateTime inicioMes =
                new DateTime(
                    hoy.Year,
                    hoy.Month,
                    1
                );

            // =====================================================
            // KPI: LOGINS DE HOY
            // =====================================================
            LoginsHoy =
                await _db
                    .IntranetActividades
                    .AsNoTracking()
                    .CountAsync(x =>
                        x.TipoEvento == "Login" &&
                        x.FechaHora >= hoy &&
                        x.FechaHora < manana
                    );

            // =====================================================
            // KPI: USUARIOS ACTIVOS HOY
            // =====================================================
            UsuariosActivosHoy =
                await _db
                    .IntranetActividades
                    .AsNoTracking()
                    .Where(x =>
                        x.FechaHora >= hoy &&
                        x.FechaHora < manana &&
                        !string.IsNullOrWhiteSpace(
                            x.UserId
                        )
                    )
                    .Select(x =>
                        x.UserId
                    )
                    .Distinct()
                    .CountAsync();

            // =====================================================
            // KPI: VISITAS DEL MES
            // =====================================================
            VisitasMes =
                await _db
                    .IntranetActividades
                    .AsNoTracking()
                    .CountAsync(x =>
                        x.FechaHora >= inicioMes
                    );

            // =====================================================
            // KPI: MÓDULO MÁS USADO
            // =====================================================
            var moduloMasUsado =
                await _db
                    .IntranetActividades
                    .AsNoTracking()
                    .Where(x =>
                        x.TipoEvento ==
                        "VistaModulo"
                    )
                    .GroupBy(x =>
                        x.Modulo
                    )
                    .Select(grupo =>
                        new
                        {
                            Modulo =
                                grupo.Key,

                            Total =
                                grupo.Count()
                        }
                    )
                    .OrderByDescending(x =>
                        x.Total
                    )
                    .FirstOrDefaultAsync();

            ModuloMasUsado =
                moduloMasUsado?.Modulo ??
                "Sin datos";

            // =====================================================
            // ACTIVIDAD GENERAL DE LA INTRANET
            // =====================================================
            var actividadesIntranet =
                await _db
                    .IntranetActividades
                    .AsNoTracking()
                    .OrderByDescending(x =>
                        x.FechaHora
                    )
                    .Take(100)
                    .Select(x =>
                        new ActividadGeneralDto
                        {
                            UserId =
                                x.UserId ??
                                string.Empty,

                            UserName =
                                x.UserName ??
                                "Usuario desconocido",

                            NombreEmpleado =
                                x.NombreEmpleado ??
                                x.UserName ??
                                "Usuario desconocido",

                            TipoEvento =
                                x.TipoEvento,

                            Modulo =
                                string.IsNullOrWhiteSpace(
                                    x.Modulo
                                )
                                    ? "General"
                                    : x.Modulo,

                            Ruta =
                                x.Ruta,

                            FechaHora =
                                x.FechaHora,

                            Ip =
                                x.Ip ??
                                "-",

                            Latitud =
                                x.Latitud,

                            Longitud =
                                x.Longitud
                        }
                    )
                    .ToListAsync();

            // =====================================================
            // ACTIVIDAD DE EMPRESAS DE COMPLIANCE
            // =====================================================
            var actividadEmpresasCompliance =
                await _db
                    .EbBitacoraEmpresas
                    .AsNoTracking()
                    .Where(x =>
                        x.Exitoso
                    )
                    .OrderByDescending(x =>
                        x.FechaEvento
                    )
                    .Take(100)
                    .Select(x =>
                        new ActividadGeneralDto
                        {
                            UserId =
                                x.UsuarioId,

                            UserName =
                                string.IsNullOrWhiteSpace(
                                    x.NombreUsuario
                                )
                                    ? x.UsuarioId
                                    : x.NombreUsuario,

                            NombreEmpleado =
                                string.IsNullOrWhiteSpace(
                                    x.NombreUsuario
                                )
                                    ? x.UsuarioId
                                    : x.NombreUsuario,

                            TipoEvento =
                                x.Accion,

                            Modulo =
                                "Compliance / Empresas",

                            Ruta =
                                "/ExpedientesBancarios/Empresas",

                            FechaHora =
                                x.FechaEvento,

                            Ip =
                                x.DireccionIp ??
                                "-",

                            Latitud =
                                null,

                            Longitud =
                                null
                        }
                    )
                    .ToListAsync();

            // =====================================================
            // ACTIVIDAD DOCUMENTAL DE COMPLIANCE
            // =====================================================
            var actividadDocumentosCompliance =
                await _db
                    .EbBitacoraDocumentos
                    .AsNoTracking()
                    .Where(x =>
                        x.Exitoso
                    )
                    .OrderByDescending(x =>
                        x.FechaEvento
                    )
                    .Take(100)
                    .Select(x =>
                        new ActividadGeneralDto
                        {
                            UserId =
                                x.UsuarioId ??
                                string.Empty,

                            UserName =
                                !string.IsNullOrWhiteSpace(
                                    x.NombreUsuario
                                )
                                    ? x.NombreUsuario
                                    : x.UsuarioId ??
                                      "Usuario desconocido",

                            NombreEmpleado =
                                !string.IsNullOrWhiteSpace(
                                    x.NombreUsuario
                                )
                                    ? x.NombreUsuario
                                    : x.UsuarioId ??
                                      "Usuario desconocido",

                            TipoEvento =
                                x.Accion,

                            Modulo =
                                "Compliance / Documentos",

                            Ruta =
                                "/ExpedientesBancarios/Empresas",

                            FechaHora =
                                x.FechaEvento,

                            Ip =
                                x.DireccionIp ??
                                "-",

                            Latitud =
                                null,

                            Longitud =
                                null
                        }
                    )
                    .ToListAsync();

            // =====================================================
            // UNIFICAR LA ACTIVIDAD MÁS RECIENTE
            // =====================================================
            UltimosAccesos =
                actividadesIntranet
                    .Concat(
                        actividadEmpresasCompliance
                    )
                    .Concat(
                        actividadDocumentosCompliance
                    )
                    .OrderByDescending(x =>
                        x.FechaHora
                    )
                    .Take(100)
                    .ToList();

            // =====================================================
            // USUARIOS CON MÁS INICIOS DE SESIÓN
            // =====================================================
            UsuariosMasActivos =
                await _db
                    .IntranetActividades
                    .AsNoTracking()
                    .Where(x =>
                        x.TipoEvento ==
                            "Login" &&
                        !string.IsNullOrWhiteSpace(
                            x.UserName
                        )
                    )
                    .GroupBy(x =>
                        x.UserName
                    )
                    .Select(grupo =>
                        new UsuarioActivoDto
                        {
                            Usuario =
                                grupo.Key!,

                            Total =
                                grupo.Count()
                        }
                    )
                    .OrderByDescending(x =>
                        x.Total
                    )
                    .Take(10)
                    .ToListAsync();

            // =====================================================
            // MÓDULOS MÁS VISITADOS
            // =====================================================
            ModulosMasVisitados =
                await _db
                    .IntranetActividades
                    .AsNoTracking()
                    .Where(x =>
                        x.TipoEvento ==
                        "VistaModulo"
                    )
                    .GroupBy(x =>
                        x.Modulo
                    )
                    .Select(grupo =>
                        new ModuloVisitadoDto
                        {
                            Modulo =
                                string.IsNullOrWhiteSpace(
                                    grupo.Key
                                )
                                    ? "General"
                                    : grupo.Key,

                            Total =
                                grupo.Count()
                        }
                    )
                    .OrderByDescending(x =>
                        x.Total
                    )
                    .Take(5)
                    .ToListAsync();
        }
    }
}