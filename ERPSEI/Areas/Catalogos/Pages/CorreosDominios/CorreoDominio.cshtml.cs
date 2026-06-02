using ERPSEI.Data;
using ERPSEI.Data.Entities.Intranet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

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
            string? correo,
            string? dominio,
            string? responsable)
        {
            var query = _db.CorreosDominios
                .AsNoTracking()
                .Where(x => !x.Deshabilitado)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(correo))
                query = query.Where(x => x.Correo != null && x.Correo.Contains(correo));

            if (!string.IsNullOrWhiteSpace(dominio))
                query = query.Where(x => x.Dominio != null && x.Dominio.Contains(dominio));

            if (!string.IsNullOrWhiteSpace(responsable))
                query = query.Where(x => x.Responsable != null && x.Responsable.Contains(responsable));

            var data = await query
                .OrderByDescending(x => x.Id)
                .Select(x => new
                {
                    x.Id,
                    x.Correo,
                    x.Dominio,
                    x.Descripcion,
                    x.Responsable,
                    x.Observaciones
                })
                .ToListAsync();

            return new JsonResult(data);
        }

        public async Task<JsonResult> OnPostSaveCorreoDominioAsync([FromBody] CorreoDominio input)
        {
            if (input == null)
                return new JsonResult(new { success = false, message = "Información inválida." });

            if (string.IsNullOrWhiteSpace(input.Correo) && string.IsNullOrWhiteSpace(input.Dominio))
                return new JsonResult(new { success = false, message = "Capture al menos el correo o el dominio." });

            if (input.Id == 0)
            {
                input.FechaCreacion = DateTime.Now;
                input.UsuarioCreador = User.Identity?.Name;
                input.Deshabilitado = false;

                _db.CorreosDominios.Add(input);
            }
            else
            {
                var entity = await _db.CorreosDominios.FirstOrDefaultAsync(x => x.Id == input.Id);

                if (entity == null)
                    return new JsonResult(new { success = false, message = "Registro no encontrado." });

                entity.Correo = input.Correo;
                entity.Dominio = input.Dominio;
                entity.Descripcion = input.Descripcion;
                entity.Responsable = input.Responsable;
                entity.Observaciones = input.Observaciones;
                entity.FechaModificacion = DateTime.Now;
                entity.UsuarioModificador = User.Identity?.Name;
            }

            await _db.SaveChangesAsync();

            return new JsonResult(new { success = true, message = "Registro guardado correctamente." });
        }

        public async Task<JsonResult> OnPostDeleteCorreoDominioAsync([FromBody] int id)
        {
            var entity = await _db.CorreosDominios.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return new JsonResult(new { success = false, message = "Registro no encontrado." });

            entity.Deshabilitado = true;
            entity.FechaModificacion = DateTime.Now;
            entity.UsuarioModificador = User.Identity?.Name;

            await _db.SaveChangesAsync();

            return new JsonResult(new { success = true, message = "Registro eliminado correctamente." });
        }

        public async Task<JsonResult> OnPostImportarCorreosDominiosAsync(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return new JsonResult(new { success = false, message = "Seleccione un archivo Excel." });

            using var stream = new MemoryStream();
            await archivo.CopyToAsync(stream);

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();

            if (worksheet == null || worksheet.Dimension == null)
                return new JsonResult(new { success = false, message = "El archivo no contiene información." });

            int totalRows = worksheet.Dimension.Rows;
            int insertados = 0;

            for (int row = 2; row <= totalRows; row++)
            {
                string? correo = worksheet.Cells[row, 1].Text?.Trim();
                string? dominio = worksheet.Cells[row, 2].Text?.Trim();
                string? descripcion = worksheet.Cells[row, 3].Text?.Trim();
                string? responsable = worksheet.Cells[row, 4].Text?.Trim();
                string? observaciones = worksheet.Cells[row, 5].Text?.Trim();

                if (string.IsNullOrWhiteSpace(correo) && string.IsNullOrWhiteSpace(dominio))
                    continue;

                var registro = new CorreoDominio
                {
                    Correo = correo,
                    Dominio = dominio,
                    Descripcion = descripcion,
                    Responsable = responsable,
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
                message = $"Importación realizada correctamente. Registros insertados: {insertados}"
            });
        }
    }
}