using ERPSEI.Data;
using ERPSEI.Data.Entities.Intranet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Globalization;
using OfficeOpenXml.Style;
using System.Drawing;

namespace ERPSEI.Areas.Catalogos.Pages.CorreosDominios
{
    [Authorize(Roles = "Administrador,Administrador TI")]
    public class CorreoDominioModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public CorreoDominioModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
        }

        public async Task<JsonResult> OnGetCorreosDominiosListAsync(
            string? empresa,
            string? dominio,
            string? proveedor,
            string? estado)
        {
            var query = _db.CorreosDominios
                .AsNoTracking()
                .Where(x => !x.Deshabilitado)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(empresa))
                query = query.Where(x => x.Empresa != null && x.Empresa.Contains(empresa));

            if (!string.IsNullOrWhiteSpace(dominio))
                query = query.Where(x => x.Dominio != null && x.Dominio.Contains(dominio));

            if (!string.IsNullOrWhiteSpace(proveedor))
                query = query.Where(x => x.Proveedor != null && x.Proveedor.Contains(proveedor));

            if (!string.IsNullOrWhiteSpace(estado))
                query = query.Where(x => x.Estado != null && x.Estado.Contains(estado));

            var data = await query
                .OrderByDescending(x => x.Id)
                .Select(x => new
                {
                    x.Id,
                    x.Empresa,
                    x.Dominio,
                    x.Proveedor,
                    FechaCaducacion = x.FechaCaducacion.HasValue
                        ? x.FechaCaducacion.Value.ToString("dd/MM/yyyy")
                        : "",
                    x.Costos,
                    x.CorreoOperaciones,
                    x.ContrasenaOperaciones,
                    x.CorreoFiscal,
                    x.ContrasenaFiscal,
                    x.PagWeb,
                    x.Estado,
                    x.Observaciones
                })
                .ToListAsync();

            return new JsonResult(data);
        }

        public async Task<JsonResult> OnPostSaveCorreoDominioAsync([FromBody] CorreoDominio input)
        {
            if (input == null)
                return new JsonResult(new { success = false, message = "Información inválida." });

            if (string.IsNullOrWhiteSpace(input.Empresa) &&
                string.IsNullOrWhiteSpace(input.Dominio))
                return new JsonResult(new { success = false, message = "Capture al menos la empresa o el dominio." });

            if (input.Id == 0)
            {
                input.FechaCreacion = DateTime.Now;
                input.UsuarioCreador = User.Identity?.Name;
                input.Deshabilitado = false;

                _db.CorreosDominios.Add(input);
            }
            else
            {
                var entity = await _db.CorreosDominios
                    .FirstOrDefaultAsync(x => x.Id == input.Id);

                if (entity == null)
                    return new JsonResult(new { success = false, message = "Registro no encontrado." });

                entity.Empresa = input.Empresa;
                entity.Dominio = input.Dominio;
                entity.Proveedor = input.Proveedor;

                // Si cambia la fecha de caducación, reiniciamos la notificación
                if (entity.FechaCaducacion?.Date != input.FechaCaducacion?.Date)
                {
                    entity.Notificacion7DiasEnviada = false;
                }

                entity.FechaCaducacion = input.FechaCaducacion;
                entity.Costos = input.Costos;
                entity.CorreoOperaciones = input.CorreoOperaciones;
                entity.ContrasenaOperaciones = input.ContrasenaOperaciones;
                entity.CorreoFiscal = input.CorreoFiscal;
                entity.ContrasenaFiscal = input.ContrasenaFiscal;
                entity.PagWeb = input.PagWeb;
                entity.Estado = input.Estado;
                entity.Observaciones = input.Observaciones;
                entity.FechaModificacion = DateTime.Now;
                entity.UsuarioModificador = User.Identity?.Name;
            }

            await _db.SaveChangesAsync();

            return new JsonResult(new { success = true, message = "Registro guardado correctamente." });
        }

        public async Task<JsonResult> OnPostDeleteCorreoDominioAsync([FromBody] int id)
        {
            var entity = await _db.CorreosDominios
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return new JsonResult(new { success = false, message = "Registro no encontrado." });

            entity.Deshabilitado = true;
            entity.FechaModificacion = DateTime.Now;
            entity.UsuarioModificador = User.Identity?.Name;

            await _db.SaveChangesAsync();

            return new JsonResult(new { success = true, message = "Registro eliminado correctamente." });
        }

        public async Task<JsonResult> OnPostImportarCorreosDominiosAsync([FromForm] IFormFile archivo)
        {
            try
            {
                if (archivo == null || archivo.Length == 0)
                    return new JsonResult(new { success = false, message = "Seleccione un archivo Excel." });

                ExcelPackage.License.SetNonCommercialPersonal("SEI Consulting Group");

                using var stream = new MemoryStream();
                await archivo.CopyToAsync(stream);

                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                if (worksheet == null || worksheet.Dimension == null)
                    return new JsonResult(new { success = false, message = "El archivo no contiene información." });

                int totalRows = worksheet.Dimension.Rows;
                int insertados = 0;
                int omitidos = 0;

                for (int row = 2; row <= totalRows; row++)
                {
                    string? empresa = worksheet.Cells[row, 1].Text?.Trim();
                    string? dominio = worksheet.Cells[row, 2].Text?.Trim();
                    string? proveedor = worksheet.Cells[row, 3].Text?.Trim();
                    string? fechaTexto = worksheet.Cells[row, 4].Text?.Trim();
                    string? costosTexto = worksheet.Cells[row, 5].Text?.Trim();
                    string? correoOperaciones = worksheet.Cells[row, 6].Text?.Trim();
                    string? contrasenaOperaciones = worksheet.Cells[row, 7].Text?.Trim();
                    string? correoFiscal = worksheet.Cells[row, 8].Text?.Trim();
                    string? contrasenaFiscal = worksheet.Cells[row, 9].Text?.Trim();
                    string? pagWeb = worksheet.Cells[row, 10].Text?.Trim();
                    string? estado = worksheet.Cells[row, 11].Text?.Trim();
                    string? observaciones = worksheet.Cells[row, 12].Text?.Trim();

                    bool filaVacia =
                        string.IsNullOrWhiteSpace(empresa) &&
                        string.IsNullOrWhiteSpace(dominio);

                    if (filaVacia)
                    {
                        omitidos++;
                        continue;
                    }

                    DateTime? fechaCaducacion = ObtenerFechaExcel(worksheet.Cells[row, 4].Value, fechaTexto);
                    decimal? costos = ObtenerDecimal(costosTexto);

                    var registro = new CorreoDominio
                    {
                        Empresa = empresa,
                        Dominio = dominio,
                        Proveedor = proveedor,
                        FechaCaducacion = fechaCaducacion,
                        Costos = costos,
                        CorreoOperaciones = correoOperaciones,
                        ContrasenaOperaciones = contrasenaOperaciones,
                        CorreoFiscal = correoFiscal,
                        ContrasenaFiscal = contrasenaFiscal,
                        PagWeb = pagWeb,
                        Estado = estado,
                        Observaciones = observaciones,
                        Deshabilitado = false,
                        FechaCreacion = DateTime.Now,
                        UsuarioCreador = User.Identity?.Name
                    };

                    _db.CorreosDominios.Add(registro);
                    insertados++;
                }

                await _db.SaveChangesAsync();

                return new JsonResult(new
                {
                    success = true,
                    message = $"Importación realizada correctamente. Insertados: {insertados}. Omitidos: {omitidos}."
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Error al importar: " + ex.Message + " | Inner: " + ex.InnerException?.Message
                });
            }
        }

        public async Task<IActionResult> OnGetDescargarLayoutCorreosDominiosAsync()
        {
            ExcelPackage.License.SetNonCommercialPersonal("Jorge Cruz");

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Correos_Dominios");

            string[] headers =
            {
        "EMPRESA",
        "DOMINIO",
        "PROVEEDOR",
        "FECHA QUE CADUCA",
        "COSTOS",
        "CORREO OPERACIONES",
        "CONTRASEÑA OPERACIONES",
        "CORREO FISCAL",
        "CONTRASEÑA FISCAL",
        "PAG WEB",
        "ESTADO",
        "OBSERVACIONES"
    };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
            }

            var registros = await _db.CorreosDominios
                .AsNoTracking()
                .Where(x => !x.Deshabilitado)
                .OrderBy(x => x.Empresa)
                .ThenBy(x => x.Dominio)
                .ToListAsync();

            int row = 2;

            foreach (var item in registros)
            {
                worksheet.Cells[row, 1].Value = item.Empresa;
                worksheet.Cells[row, 2].Value = item.Dominio;
                worksheet.Cells[row, 3].Value = item.Proveedor;
                worksheet.Cells[row, 4].Value = item.FechaCaducacion;
                worksheet.Cells[row, 4].Style.Numberformat.Format = "dd/mm/yyyy";
                worksheet.Cells[row, 5].Value = item.Costos;
                worksheet.Cells[row, 6].Value = item.CorreoOperaciones;
                worksheet.Cells[row, 7].Value = item.ContrasenaOperaciones;
                worksheet.Cells[row, 8].Value = item.CorreoFiscal;
                worksheet.Cells[row, 9].Value = item.ContrasenaFiscal;
                worksheet.Cells[row, 10].Value = item.PagWeb;
                AplicarColorPagWeb(worksheet.Cells[row, 10], item.PagWeb);

                worksheet.Cells[row, 11].Value = item.Estado;
                AplicarColorEstado(worksheet.Cells[row, 11], item.Estado);
                worksheet.Cells[row, 12].Value = item.Observaciones;

                row++;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var bytes = package.GetAsByteArray();

            string fileName = $"Layout_Correos_Dominios_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }

        private static DateTime? ObtenerFechaExcel(object? valorCelda, string? textoCelda)
        {
            if (valorCelda is DateTime fechaExcel)
                return fechaExcel;

            if (valorCelda is double numeroExcel)
                return DateTime.FromOADate(numeroExcel);

            if (string.IsNullOrWhiteSpace(textoCelda))
                return null;

            string[] formatos =
            {
                "dd/MM/yyyy",
                "d/M/yyyy",
                "dd-MM-yyyy",
                "d-M-yyyy",
                "yyyy-MM-dd"
            };

            if (DateTime.TryParseExact(
                    textoCelda,
                    formatos,
                    new CultureInfo("es-MX"),
                    DateTimeStyles.None,
                    out DateTime fechaExacta))
            {
                return fechaExacta;
            }

            if (DateTime.TryParse(textoCelda, new CultureInfo("es-MX"), DateTimeStyles.None, out DateTime fechaNormal))
                return fechaNormal;

            return null;
        }

        private static decimal? ObtenerDecimal(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return null;

            string limpio = texto
                .Replace("$", "")
                .Replace(",", "")
                .Replace("MXN", "", StringComparison.OrdinalIgnoreCase)
                .Replace("USD", "", StringComparison.OrdinalIgnoreCase)
                .Trim();

            if (decimal.TryParse(limpio, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor))
                return valor;

            if (decimal.TryParse(limpio, NumberStyles.Any, new CultureInfo("es-MX"), out decimal valorMx))
                return valorMx;

            return null;
        }

        private static void AplicarColorEstado(ExcelRange celda, string? estado)
        {
            string valor = (estado ?? "").Trim().ToUpper();

            celda.Style.Font.Bold = true;
            celda.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            if (valor == "VIGENTE")
            {
                celda.Style.Fill.PatternType = ExcelFillStyle.Solid;
                celda.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(215, 243, 227));
                celda.Style.Font.Color.SetColor(Color.FromArgb(7, 95, 55));
            }
            else if (valor == "SUSPENDIDA")
            {
                celda.Style.Fill.PatternType = ExcelFillStyle.Solid;
                celda.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 243, 205));
                celda.Style.Font.Color.SetColor(Color.FromArgb(133, 100, 4));
            }
            else if (valor == "CLIENTE")
            {
                celda.Style.Fill.PatternType = ExcelFillStyle.Solid;
                celda.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(220, 236, 255));
                celda.Style.Font.Color.SetColor(Color.FromArgb(8, 66, 152));
            }
        }

        private static void AplicarColorPagWeb(ExcelRange celda, string? pagWeb)
        {
            string valor = (pagWeb ?? "").Trim().ToUpper();

            celda.Style.Font.Bold = true;
            celda.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            if (valor == "OK")
            {
                celda.Style.Fill.PatternType = ExcelFillStyle.Solid;
                celda.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(215, 243, 227));
                celda.Style.Font.Color.SetColor(Color.FromArgb(7, 95, 55));
            }
            else if (valor == "N/A")
            {
                celda.Style.Fill.PatternType = ExcelFillStyle.Solid;
                celda.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(248, 215, 218));
                celda.Style.Font.Color.SetColor(Color.FromArgb(132, 32, 41));
            }
        }
    }
}