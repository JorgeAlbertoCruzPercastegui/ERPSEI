using ERPSEI.Data;
using ERPSEI.Data.Entities.Metricas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace ERPSEI.Areas.Catalogos.Pages.Auditoria
{
    [Authorize(Roles = "Administrador,Master")]
    public class AuditoriaModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public AuditoriaModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public int TotalAltas { get; set; }
        public int TotalEdiciones { get; set; }
        public int TotalBajas { get; set; }
        public int CambiosHoy { get; set; }

        public List<IntranetAuditoria> RegistrosAuditoria { get; set; } = new();

        public async Task OnGetAsync()
        {
            var hoy = DateTime.Today;

            TotalAltas = await _db.IntranetAuditorias
                .AsNoTracking()
                .CountAsync(x => x.Accion == "Alta");

            TotalEdiciones = await _db.IntranetAuditorias
                .AsNoTracking()
                .CountAsync(x => x.Accion == "EdiciÃ³n");

            TotalBajas = await _db.IntranetAuditorias
                .AsNoTracking()
                .CountAsync(x => x.Accion == "Baja" || x.Accion == "EliminaciÃ³n");

            CambiosHoy = await _db.IntranetAuditorias
                .AsNoTracking()
                .CountAsync(x => x.FechaHora.Date == hoy);

            RegistrosAuditoria = await _db.IntranetAuditorias
                .AsNoTracking()
                .OrderByDescending(x => x.FechaHora)
                .Take(500)
                .ToListAsync();
        }

        public async Task<IActionResult> OnGetExportarAsync()
        {
            var registros = await _db.IntranetAuditorias
                .AsNoTracking()
                .OrderByDescending(x => x.FechaHora)
                .ToListAsync();

            var csv = new StringBuilder();

            // BOM UTF-8 para que Excel respete acentos y ñ.
            csv.Append('\uFEFF');

            csv.AppendLine(
                "\"Fecha\",\"Ejecutor\",\"Módulo\",\"Acción\",\"Registro\",\"Campo\",\"Valor anterior\",\"Valor nuevo\",\"IP\",\"Latitud\",\"Longitud\""
            );

            foreach (var item in registros)
            {
                csv.AppendLine(
                    string.Join(",",
                        EscaparCsv(
                            item.FechaHora.ToString("dd/MM/yyyy HH:mm:ss")
                        ),
                        EscaparCsv(item.UsuarioEjecutor),
                        EscaparCsv(item.Modulo),
                        EscaparCsv(item.Accion),
                        EscaparCsv(item.RegistroNombre),
                        EscaparCsv(item.CampoModificado),
                        EscaparCsv(item.ValorAnterior),
                        EscaparCsv(item.ValorNuevo),
                        EscaparCsv(item.Ip),
                        EscaparCsv(
                            item.Latitud?.ToString(
                                System.Globalization.CultureInfo.InvariantCulture
                            )
                        ),
                        EscaparCsv(
                            item.Longitud?.ToString(
                                System.Globalization.CultureInfo.InvariantCulture
                            )
                        )
                    )
                );
            }

            var bytes =
                Encoding.UTF8.GetBytes(
                    csv.ToString()
                );

            var nombreArchivo =
                $"Auditoria_Intranet_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            return File(
                bytes,
                "text/csv; charset=utf-8",
                nombreArchivo
            );
        }

        private static string EscaparCsv(object? valor)
        {
            string texto =
                valor?.ToString() ?? "";

            texto =
                texto.Replace(
                    "\"",
                    "\"\""
                );

            return $"\"{texto}\"";
        }
    }
}