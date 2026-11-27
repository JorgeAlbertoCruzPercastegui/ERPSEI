using ERPSEI.Data;
using ERPSEI.Services.Adquisiciones;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace ERPSEI.Middleware
{
    // Mantiene las URL históricas; los archivos físicos ya no están en wwwroot.
    public sealed class AdquisicionesDocumentosMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly PathString Prefijo = new("/uploads/adquisiciones");

        public AdquisicionesDocumentosMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext http, ApplicationDbContext db,
            IAdquisicionesAlcanceService alcance, IWebHostEnvironment environment)
        {
            if (!http.Request.Path.StartsWithSegments(Prefijo, StringComparison.OrdinalIgnoreCase, out var resto))
            {
                await _next(http);
                return;
            }

            http.Response.Headers["Cache-Control"] = "private, no-store";
            http.Response.Headers["X-Content-Type-Options"] = "nosniff";
            if (!HttpMethods.IsGet(http.Request.Method) && !HttpMethods.IsHead(http.Request.Method))
            {
                http.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                return;
            }

            var auth = await http.AuthenticateAsync(IdentityConstants.ApplicationScheme);
            if (!auth.Succeeded || auth.Principal == null)
            {
                http.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var segmentos = (resto.Value ?? "").Split('/');
            // Se admiten únicamente las rutas generadas por el módulo.
            if (segmentos.Length < 3 || segmentos[0] != "" ||
                !int.TryParse(segmentos[1], out int solicitudId) || solicitudId <= 0 ||
                segmentos[1] != solicitudId.ToString(System.Globalization.CultureInfo.InvariantCulture) ||
                segmentos.Skip(1).Any(x => string.IsNullOrWhiteSpace(x) || x == "." || x == ".." ||
                    x.Any(c => !(char.IsAsciiLetterOrDigit(c) || c == '.' || c == '-' || c == '_'))))
            {
                http.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            var ct = http.RequestAborted;
            var acceso = await alcance.ObtenerAccesoSolicitudAsync(auth.Principal, solicitudId, ct);
            if (!acceso.PuedeDescargar)
            {
                http.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }

            string ruta = "/uploads/adquisiciones" + resto.Value;
            // La ruta debe pertenecer a un adjunto activo del expediente indicado,
            // no solamente a un directorio con el mismo identificador.
            var documentos =
                db.AdqAdjuntos.AsNoTracking()
                    .Where(x => !x.Eliminado && x.SolicitudId == solicitudId && x.RutaArchivo == ruta)
                    .Select(x => new { x.RutaArchivo, x.NombreOriginal })
                .Concat(
                    from adj in db.AdqCotizacionAdjuntos.AsNoTracking()
                    join cot in db.AdqCotizaciones.AsNoTracking() on adj.CotizacionId equals cot.Id
                    where !adj.Eliminado && !cot.Eliminado && cot.SolicitudId == solicitudId && adj.RutaArchivo == ruta
                    select new { adj.RutaArchivo, adj.NombreOriginal })
                .Concat(
                    from adj in db.AdqComentariosAdjuntos.AsNoTracking()
                    join com in db.AdqComentarios.AsNoTracking() on adj.ComentarioId equals com.Id
                    where !adj.Eliminado && !com.Eliminado && com.SolicitudId == solicitudId && adj.RutaArchivo == ruta
                    select new { adj.RutaArchivo, adj.NombreOriginal });

            var candidatos = await documentos.Take(2).ToListAsync(ct);
            if (candidatos.Count != 1 || !string.Equals(candidatos[0].RutaArchivo, ruta, StringComparison.Ordinal))
            {
                http.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            string raiz = Path.GetFullPath(Path.Combine(environment.ContentRootPath, "App_Data", "AdquisicionesDocumentos"));
            string archivo = Path.GetFullPath(Path.Combine(raiz, ruta.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)));
            var comparacion = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            if (!archivo.StartsWith(raiz + Path.DirectorySeparatorChar, comparacion) || !File.Exists(archivo))
            {
                http.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            // No seguir enlaces simbólicos ni puntos de reanálisis dentro del almacén.
            for (string? actual = archivo; actual != null; actual = Path.GetDirectoryName(actual))
            {
                if ((File.GetAttributes(actual) & FileAttributes.ReparsePoint) != 0)
                {
                    http.Response.StatusCode = StatusCodes.Status404NotFound;
                    return;
                }
                if (string.Equals(actual, raiz, comparacion)) break;
            }

            string nombre = Path.GetFileName(candidatos[0].NombreOriginal ?? "documento");
            if (string.IsNullOrWhiteSpace(nombre) || nombre.Any(char.IsControl)) nombre = "documento";
            var disposition = new ContentDispositionHeaderValue("attachment");
            disposition.SetHttpFileName(nombre);
            http.Response.Headers["Content-Disposition"] = disposition.ToString();
            http.Response.ContentType = "application/octet-stream";
            http.Response.ContentLength = new FileInfo(archivo).Length;
            if (!HttpMethods.IsHead(http.Request.Method))
                await http.Response.SendFileAsync(archivo, ct);
        }
    }
}