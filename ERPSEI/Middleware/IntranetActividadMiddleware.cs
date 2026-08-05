using ERPSEI.Data;
using ERPSEI.Data.Entities;
using ERPSEI.Data.Entities.Metricas;
using ERPSEI.Data.Entities.Usuarios;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Middleware
{
    public class IntranetActividadMiddleware
    {
        private readonly RequestDelegate _next;

        public IntranetActividadMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            ApplicationDbContext db,
            UserManager<AppUser> userManager)
        {
            await _next(context);

            try
            {
                if (context.User.Identity?.IsAuthenticated == true &&
                    context.Request.Method == "GET" &&
                    context.Response.StatusCode == 200)
                {
                    var path = context.Request.Path.Value ?? "";
                    var query = context.Request.QueryString.Value ?? "";
                    var accept = context.Request.Headers["Accept"].ToString();
                    var isAjax = context.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

                    bool esPaginaHtml = accept.Contains("text/html", StringComparison.OrdinalIgnoreCase);

                    if (esPaginaHtml &&
                        !isAjax &&
                        !path.StartsWith("/css") &&
                        !path.StartsWith("/js") &&
                        !path.StartsWith("/lib") &&
                        !path.StartsWith("/img") &&
                        !path.StartsWith("/favicon") &&
                        !path.Contains("_framework") &&
                        !query.Contains("handler=", StringComparison.OrdinalIgnoreCase) &&

                        // EXCLUIR RUTAS INTERNAS
                        !path.Contains("logout", StringComparison.OrdinalIgnoreCase) &&
                        !path.Contains("login", StringComparison.OrdinalIgnoreCase) &&
                        !path.Contains("accessdenied", StringComparison.OrdinalIgnoreCase))
                    {
                        var user = await userManager.GetUserAsync(context.User);

                        if (user != null)
                        {
                            var modulo = ObtenerModulo(path);

                            var existeReciente = await db.IntranetActividades.AnyAsync(x =>
                                x.UserId == user.Id &&
                                x.TipoEvento == "VistaModulo" &&
                                x.Ruta == path &&
                                x.FechaHora >= DateTime.Now.AddMinutes(-2));

                            if (!existeReciente)
                            {
                                db.IntranetActividades.Add(new IntranetActividad
                                {
                                    UserId = user.Id,
                                    UserName = user.UserName,
                                    NombreEmpleado = user.Empleado != null
                                        ? $"{user.Empleado.Nombre} {user.Empleado.ApellidoPaterno}"
                                        : user.UserName,
                                    TipoEvento = "VistaModulo",
                                    Modulo = modulo,
                                    Ruta = path,
                                    FechaHora = DateTime.Now,
                                    //Ip = context.Connection.RemoteIpAddress?.ToString(),
                                    Ip = context.Connection.RemoteIpAddress?.ToString() == "::1"
                                    ? "127.0.0.1"
                                    : context.Connection.RemoteIpAddress?.ToString(),
                                    UserAgent = context.Request.Headers["User-Agent"].ToString()
                                });

                                await db.SaveChangesAsync();
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private string ObtenerModulo(string path)
        {
            path = path.ToLower();

            if (path == "/" ||
                path.Contains("/index"))
                return "Inicio";

            if (path.Contains("login"))
                return "Inicio de sesión";

            if (path.Contains("logout"))
                return "Cerrar sesión";

            if (path.Contains("manualespoliticas") ||
                path.Contains("gestormanualespoliticas"))
                return "Biblioteca Corporativa";

            if (path.Contains("vacaciones"))
                return "Vacaciones";

            if (path.Contains("ausencias"))
                return "Ausencias";

            if (path.Contains("permisos"))
                return "Permisos";

            if (path.Contains("incapacidades"))
                return "Incapacidades";

            if (path.Contains("asistencia"))
                return "Asistencia";

            if (path.Contains("time"))
                return "Time";

            if (path.Contains("kyndom"))
                return "Kyndom";

            if (path.Contains("friday"))
                return "Friday";

            if (path.Contains("expedientesbancarios") || path.Contains("compliance"))
            {
                return "Compliance";
            }

            if (path.Contains("empresas"))
                return "Empresas";

            if (path.Contains("activosfijos"))
                return "Activos Fijos";

            if (path.Contains("conciliaciones"))
                return "Conciliaciones";

            if (path.Contains("administradordecomprobantes"))
                return "Administrador de Comprobantes";

            if (path.Contains("prefacturas"))
                return "Prefacturas";

            if (path.Contains("facturacion"))
                return "Facturación";

            if (path.Contains("cuentascontables"))
                return "Cuentas Contables";

            if (path.Contains("bancos"))
                return "Bancos";

            if (path.Contains("convertidorbancario"))
                return "Convertidor Bancario";

            if (path.Contains("comunicadosinternos"))
                return "Comunicados Internos";

            if (path.Contains("gestorcomunicadosinternos"))
                return "Gestor Comunicados";

            if (path.Contains("eventos"))
                return "Eventos";

            if (path.Contains("gestoreventos"))
                return "Gestor Eventos";

            if (path.Contains("encuestas"))
                return "Encuestas";

            if (path.Contains("organigrama"))
                return "Organigrama";

            if (path.Contains("directorio"))
                return "Directorio";

            if (path.Contains("gestiondetalento"))
                return "Gestión de Talento";

            if (path.Contains("usuarios"))
                return "Usuarios";

            if (path.Contains("roles"))
                return "Roles";

            if (path.Contains("puestos"))
                return "Puestos";

            if (path.Contains("areas"))
                return "Áreas";

            if (path.Contains("subareas"))
                return "Subáreas";

            if (path.Contains("oficinas"))
                return "Oficinas";

            if (path.Contains("origenes"))
                return "Orígenes";

            if (path.Contains("niveles"))
                return "Niveles";

            if (path.Contains("perfiles"))
                return "Perfiles";

            if (path.Contains("banners"))
                return "Banners";

            if (path.Contains("headerimagenes"))
                return "Imágenes Header";

            if (path.Contains("metricas"))
                return "Métricas";

            if (path.Contains("sistemadocumentalcorporativo"))
                return "Sistema Documental";

            if (path.Contains("generadorcontrato"))
                return "Generador de Contrato";


            return "General";
        }

    }
}