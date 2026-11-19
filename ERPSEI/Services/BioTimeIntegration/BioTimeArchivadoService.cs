using System.Security.Cryptography;
using System.Text.Json;
using ClosedXML.Excel;
using ERPSEI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ERPSEI.Services.BioTimeIntegration;

public sealed class BioTimeArchivoOptions
{
    public bool Enabled { get; set; } = false;
    public string LocalDirectory { get; set; } = "/var/backups/intranet/biotime";
    public string ExternalDirectory { get; set; } = "";
    public int RowsPerFile { get; set; } = 25_000;
}

// Respaldos del bimestre CERRADO, sin eliminación automática.
// Copia secundaria obligatoria y comprobación SHA-256 + recuento de filas.
public sealed class BioTimeArchivadoService
{
    private readonly ApplicationDbContext _db;
    private readonly BioTimeArchivoOptions _options;
    private readonly ILogger<BioTimeArchivadoService> _logger;

    public BioTimeArchivadoService(ApplicationDbContext db, IOptions<BioTimeArchivoOptions> options,
        ILogger<BioTimeArchivadoService> logger)
    { _db = db; _options = options.Value; _logger = logger; }

    public async Task ArchivadoBimestreAsync(DateTime inicio, CancellationToken ct)
    {
        if (!_options.Enabled) return;
        if (inicio.Day != 1 || inicio.Month % 2 == 0)
            throw new ArgumentException("Usa el primer día del bimestre (enero, marzo, etc.).");
        var fin = inicio.AddMonths(2);
        if (fin > DateTime.Today.AddDays(1))
            throw new InvalidOperationException("No se puede archivar un bimestre abierto.");
        if (string.IsNullOrWhiteSpace(_options.ExternalDirectory))
            throw new InvalidOperationException("Configura una ruta externa de respaldos.");
        var origen = Path.GetFullPath(_options.LocalDirectory);
        var externo = Path.GetFullPath(_options.ExternalDirectory);
        if (string.Equals(origen.TrimEnd(Path.DirectorySeparatorChar), externo.TrimEnd(Path.DirectorySeparatorChar),
            StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("El destino externo debe ser diferente al local.");
        Directory.CreateDirectory(origen);
        Directory.CreateDirectory(externo);
        var max = Math.Clamp(_options.RowsPerFile, 1000, 50000);
        var totalDb = await _db.BioTimeChecadas.AsNoTracking().CountAsync(x => x.FechaHora >= inicio && x.FechaHora < fin, ct);
        var prefijo = $"BioTime_{inicio:yyyyMM}_{fin.AddDays(-1):yyyyMM}";
        // Se regeneran las copias: evita considerar cerrado un respaldo obsoleto.
        var archivos = new List<object>();
        var totalExportadas = 0;
        var parte = 0;
        while (totalExportadas < totalDb)
        {
            ct.ThrowIfCancellationRequested();
            var filas = await (from c in _db.BioTimeChecadas.AsNoTracking()
                               join e in _db.BioTimeEmpleados.AsNoTracking() on c.EmpCode equals e.EmpCode into g
                               from e in g.DefaultIfEmpty()
                               where c.FechaHora >= inicio && c.FechaHora < fin
                               orderby c.FechaHora, c.Id
                               select new
                               {
                                   c.Id,
                                   c.EmpCode,
                                   c.FechaHora,
                                   c.PunchState,
                                   c.TerminalSn,
                                   Nombre = e == null ? "" : (e.Nombre ?? "") + " " + (e.Apellidos ?? ""),
                                   Departamento = e == null ? "" : (e.Departamento ?? "")
                               })
                             .Skip(totalExportadas).Take(max).ToListAsync(ct);
            if (filas.Count == 0) throw new IOException("La consulta cambió durante el respaldo; reintentar.");
            parte++;
            var nombre = $"{prefijo}_parte{parte:D4}.xlsx";
            var temporal = Path.Combine(origen, $".{nombre}.{Guid.NewGuid():N}.tmp");
            var local = Path.Combine(origen, nombre);
            var copia = Path.Combine(externo, nombre);
            try
            {
                using (var libro = new XLWorkbook())
                {
                    var hoja = libro.Worksheets.Add("Checadas");
                    var columnas = new[] { "ID", "Código", "Colaborador", "Departamento", "FechaHora", "Estado original", "Dispositivo" };
                    for (var k = 0; k < columnas.Length; k++) hoja.Cell(1, k + 1).Value = columnas[k];
                    for (var j = 0; j < filas.Count; j++)
                    {
                        var r = filas[j]; var n = j + 2;
                        hoja.Cell(n, 1).Value = r.Id;
                        BioTimeExcelService.Texto(hoja.Cell(n, 2), r.EmpCode);
                        BioTimeExcelService.Texto(hoja.Cell(n, 3), r.Nombre);
                        BioTimeExcelService.Texto(hoja.Cell(n, 4), r.Departamento);
                        hoja.Cell(n, 5).Value = r.FechaHora;
                        BioTimeExcelService.Texto(hoja.Cell(n, 6), r.PunchState);
                        BioTimeExcelService.Texto(hoja.Cell(n, 7), r.TerminalSn);
                    }
                    hoja.Range(1, 1, 1, 7).Style.Font.Bold = true;
                    hoja.Range(1, 1, 1, 7).Style.Fill.BackgroundColor = XLColor.FromHtml("#21166b");
                    hoja.Range(1, 1, 1, 7).Style.Font.FontColor = XLColor.White;
                    hoja.Column(5).Style.DateFormat.Format = "dd/mm/yyyy hh:mm:ss";
                    hoja.SheetView.FreezeRows(1);
                    libro.SaveAs(temporal);
                }
                using (var verificacion = new XLWorkbook(temporal))
                {
                    var cantidad = verificacion.Worksheet(1).RowsUsed().Count() - 1;
                    if (cantidad != filas.Count) throw new IOException("El archivo Excel no pasó validación de filas.");
                }
                File.Move(temporal, local, true);
                File.Copy(local, copia, true);
                var hashLocal = await HashAsync(local, ct);
                var hashExterno = await HashAsync(copia, ct);
                if (hashLocal != hashExterno) throw new IOException("La copia externa difiere del respaldo local.");
                archivos.Add(new { nombre, filas = filas.Count, sha256 = hashLocal });
                totalExportadas += filas.Count;
            }
            finally { if (File.Exists(temporal)) File.Delete(temporal); }
        }
        var comprobacion = await _db.BioTimeChecadas.AsNoTracking()
            .CountAsync(x => x.FechaHora >= inicio && x.FechaHora < fin, ct);
        if (totalExportadas != totalDb || comprobacion != totalDb)
            throw new IOException("La base cambió durante el respaldo; no se marca como completo.");
        var manifest = JsonSerializer.Serialize(new
        {
            periodoInicio = inicio,
            periodoFinExclusivo = fin,
            creadoUtc = DateTime.UtcNow,
            filasSql = totalDb,
            filasExportadas = totalExportadas,
            archivos,
            purgaAutorizada = false,
            observacion = "Excel no sustituye por sí solo una copia de seguridad recuperable de SQL Server."
        }, new JsonSerializerOptions { WriteIndented = true });
        var rutaManifest = Path.Combine(origen, prefijo + "_manifest.json");
        var rutaCopiaManifest = Path.Combine(externo, prefijo + "_manifest.json");
        await File.WriteAllTextAsync(rutaManifest, manifest, ct);
        File.Copy(rutaManifest, rutaCopiaManifest, true);
        if (await HashAsync(rutaManifest, ct) != await HashAsync(rutaCopiaManifest, ct))
            throw new IOException("No se verificó el manifiesto externo.");
        _logger.LogInformation("BioTime archivado bimestral verificado: {Registros} registros, {Archivos} partes", totalDb, parte);
        // No DELETE. La depuración exige aprobación, restauración probada y retención legal.
    }

    private static async Task<string> HashAsync(string ruta, CancellationToken ct)
    {
        await using var archivo = File.OpenRead(ruta);
        return Convert.ToHexString(await SHA256.HashDataAsync(archivo, ct));
    }
}

// Cubre el primer día de enero, marzo, mayo, julio, septiembre y noviembre.
// Procesa el bimestre anterior (ej.: enero archiva noviembre-diciembre).
public sealed class BioTimeArchivadoBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopes;
    private readonly IOptions<BioTimeArchivoOptions> _options;
    private readonly ILogger<BioTimeArchivadoBackgroundService> _logger;
    public BioTimeArchivadoBackgroundService(IServiceScopeFactory scopes,
        IOptions<BioTimeArchivoOptions> options, ILogger<BioTimeArchivadoBackgroundService> logger)
    { _scopes = scopes; _options = options; _logger = logger; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Revisión cada seis horas; primera ejecución inmediatamente al iniciar.
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_options.Value.Enabled)
                {
                    var hoy = DateTime.Today;
                    if (hoy.Day == 1 && hoy.Month % 2 == 1)
                    {
                        var inicio = new DateTime(hoy.Year, hoy.Month, 1).AddMonths(-2);
                        var prefijo = $"BioTime_{inicio:yyyyMM}_{inicio.AddMonths(2).AddDays(-1):yyyyMM}_manifest.json";
                        var manifiesto = Path.Combine(Path.GetFullPath(_options.Value.ExternalDirectory), prefijo);
                        if (!File.Exists(manifiesto))
                        {
                            await using var scope = _scopes.CreateAsyncScope();
                            await scope.ServiceProvider.GetRequiredService<BioTimeArchivadoService>()
                                .ArchivadoBimestreAsync(inicio, stoppingToken);
                        }
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception ex) { _logger.LogError(ex, "Error en archivado bimestral BioTime"); }
            await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
        }
    }
}
