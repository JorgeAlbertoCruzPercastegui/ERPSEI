
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace ERPSEI.Services.RelojChecador;

public sealed record RelojEvidenciaGuardada(
    string RutaPrivada,
    string HashSha256,
    string TipoContenido,
    long TamanoBytes,
    DateTime CapturadaUtc
);

public sealed class RelojEvidenciasService
{
    private readonly string _directorioRaiz;
    private readonly int _tamanoMaximoBytes;
    private readonly IDataProtector _protector;
    private readonly ILogger<RelojEvidenciasService> _logger;

    public RelojEvidenciasService(
        IOptions<RelojEvidenciasOptions> options,
        IDataProtectionProvider protectionProvider,
        IWebHostEnvironment environment,
        ILogger<RelojEvidenciasService> logger)
    {
        _logger = logger;

        var config = options.Value;

        if (string.IsNullOrWhiteSpace(config.RutaPrivada))
        {
            throw new InvalidOperationException(
                "Configura RelojChecador:Evidencias:RutaPrivada.");
        }

        if (config.TamanoMaximoMB < 1 ||
            config.TamanoMaximoMB > 10)
        {
            throw new InvalidOperationException(
                "TamanoMaximoMB debe estar entre 1 y 10.");
        }

        if (!Path.IsPathFullyQualified(config.RutaPrivada))
        {
            throw new InvalidOperationException(
                "La ruta de evidencias debe ser absoluta.");
        }

        _directorioRaiz = Path.GetFullPath(
            config.RutaPrivada);

        _tamanoMaximoBytes = checked(
            config.TamanoMaximoMB * 1024 * 1024);

        // Impedir almacenamiento dentro de wwwroot.
        if (!string.IsNullOrWhiteSpace(environment.WebRootPath))
        {
            var webRoot = Path.GetFullPath(
                environment.WebRootPath);

            var relativo = Path.GetRelativePath(
                webRoot,
                _directorioRaiz);

            var dentroDeWebRoot =
                relativo == "." ||
                (
                    !relativo.StartsWith("..") &&
                    !Path.IsPathRooted(relativo)
                );

            if (dentroDeWebRoot)
            {
                throw new InvalidOperationException(
                    "Las evidencias no pueden almacenarse en wwwroot.");
            }
        }

        _protector = protectionProvider.CreateProtector(
            "ERPSEI.RelojChecador.Evidencias.v1");
    }

    public async Task<RelojEvidenciaGuardada> GuardarAsync(
        IFormFile fotografia,
        CancellationToken ct = default)
    {
        if (fotografia == null ||
            fotografia.Length < 1024 ||
            fotografia.Length > _tamanoMaximoBytes)
        {
            throw new ArgumentException(
                "La fotografía tiene un tamaño inválido.");
        }

        // El tamaño indicado por el cliente es un límite,
        // no una garantía. Leeremos como máximo lo permitido.
        byte[] contenido;

        await using (var origen = fotografia.OpenReadStream())
        await using (var memoria = new MemoryStream())
        {
            var buffer = new byte[81920];
            int leidos;

            while ((leidos = await origen.ReadAsync(
                buffer.AsMemory(),
                ct)) > 0)
            {
                if (memoria.Length + leidos >
                    _tamanoMaximoBytes)
                {
                    throw new ArgumentException(
                        "La fotografía supera el tamaño permitido.");
                }

                await memoria.WriteAsync(
                    buffer.AsMemory(0, leidos),
                    ct);
            }

            contenido = memoria.ToArray();
        }

        if (contenido.Length < 1024)
        {
            throw new ArgumentException(
                "La fotografía está vacía o incompleta.");
        }

        var tipoContenido = DetectarTipo(contenido);

        if (tipoContenido == null)
        {
            throw new ArgumentException(
                "Solo se admiten imágenes JPEG o PNG.");
        }

        // El hash se calcula sobre los bytes originales.
        var hash = Convert.ToHexString(
            SHA256.HashData(contenido));

        var ahora = DateTime.UtcNow;

        // La ruta no contiene datos personales ni nombres
        // enviados desde el navegador.
        var carpetaRelativa = Path.Combine(
            ahora.ToString("yyyy"),
            ahora.ToString("MM"));

        var carpetaCompleta = Path.Combine(
            _directorioRaiz,
            carpetaRelativa);

        Directory.CreateDirectory(carpetaCompleta);

        var nombreArchivo =
            $"{Guid.NewGuid():N}.bin";

        var rutaRelativa = Path.Combine(
            carpetaRelativa,
            nombreArchivo);

        var rutaCompleta = Path.Combine(
            carpetaCompleta,
            nombreArchivo);

        // Data Protection protege la confidencialidad y
        // detecta manipulaciones del archivo.
        var cifrado = _protector.Protect(contenido);

        try
        {
            await using var destino = new FileStream(
                rutaCompleta,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81920,
                options: FileOptions.Asynchronous);

            await destino.WriteAsync(
                cifrado.AsMemory(),
                ct);

            await destino.FlushAsync(ct);
        }
        catch
        {
            try
            {
                if (File.Exists(rutaCompleta))
                    File.Delete(rutaCompleta);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "No se pudo limpiar una evidencia fallida.");
            }

            throw;
        }

        _logger.LogInformation(
            "Evidencia del reloj almacenada de forma cifrada.");

        return new RelojEvidenciaGuardada(
            RutaPrivada: rutaRelativa.Replace('\\', '/'),
            HashSha256: hash,
            TipoContenido: tipoContenido,
            TamanoBytes: contenido.LongLength,
            CapturadaUtc: ahora
        );
    }

    public Task EliminarAsync(
        string rutaRelativa,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(rutaRelativa) ||
            Path.IsPathRooted(rutaRelativa))
        {
            throw new ArgumentException(
                "Ruta de evidencia inválida.");
        }

        var rutaCompleta = Path.GetFullPath(
            Path.Combine(
                _directorioRaiz,
                rutaRelativa.Replace(
                    '/',
                    Path.DirectorySeparatorChar)));

        // Evitar recorrer directorios hacia fuera
        // de la carpeta privada.
        var relativo = Path.GetRelativePath(
            _directorioRaiz,
            rutaCompleta);

        if (relativo == "." ||
            relativo.StartsWith("..") ||
            Path.IsPathRooted(relativo))
        {
            throw new InvalidOperationException(
                "La ruta está fuera del almacenamiento autorizado.");
        }

        if (File.Exists(rutaCompleta))
            File.Delete(rutaCompleta);

        return Task.CompletedTask;
    }

    private static string? DetectarTipo(
        byte[] bytes)
    {
        // Verificación básica de firma, no análisis
        // completo ni decodificación de la imagen.
        if (bytes.Length >= 4 &&
            bytes[0] == 0xFF &&
            bytes[1] == 0xD8 &&
            bytes[^2] == 0xFF &&
            bytes[^1] == 0xD9)
        {
            return "image/jpeg";
        }

        byte[] firmaPng =
        [
            0x89, 0x50, 0x4E, 0x47,
            0x0D, 0x0A, 0x1A, 0x0A
        ];

        if (bytes.Length >= 8 &&
            bytes.AsSpan(0, 8)
                .SequenceEqual(firmaPng))
        {
            return "image/png";
        }

        return null;
    }
}
