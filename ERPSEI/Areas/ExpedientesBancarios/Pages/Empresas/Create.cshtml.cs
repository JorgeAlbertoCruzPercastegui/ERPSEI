using ERPSEI.Data;
using ERPSEI.Data.Entities.ExpedientesBancarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace ERPSEI.Areas.ExpedientesBancarios.Pages.Empresas
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EmpresaInputModel Input { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            NormalizarDatos();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            bool rfcExistente = await _context.EbEmpresas
                .IgnoreQueryFilters()
                .AnyAsync(x => x.Rfc == Input.Rfc);

            if (rfcExistente)
            {
                ModelState.AddModelError(
                    "Input.Rfc",
                    "Ya existe una empresa registrada con este RFC.");

                return Page();
            }

            string usuarioId =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? "SYSTEM";

            var empresa = new EbEmpresa
            {
                RazonSocial = Input.RazonSocial,
                NombreCorto = Input.NombreCorto,
                Rfc = Input.Rfc,
                Nivel = Input.Nivel,
                ActividadComercial = Input.ActividadComercial,
                TelefonoBancos = Input.TelefonoBancos,
                CorreoBancos = Input.CorreoBancos,
                FechaConstitucion = Input.FechaConstitucion,
                NumeroEscritura = Input.NumeroEscritura,
                DomicilioFiscal = Input.DomicilioFiscal,
                Observaciones = Input.Observaciones,
                Deshabilitado = false,
                Eliminado = false,
                FechaCreacion = DateTime.Now,
                UsuarioCreacionId = usuarioId
            };

            _context.EbEmpresas.Add(empresa);
            await _context.SaveChangesAsync();

            TempData["MensajeExito"] =
                $"La empresa {empresa.RazonSocial} se registró correctamente.";

            return RedirectToPage("/Empresas/Index",
                new
                {
                    area = "ExpedientesBancarios"
                });
        }

        private void NormalizarDatos()
        {
            Input.RazonSocial = Input.RazonSocial?.Trim() ?? string.Empty;
            Input.NombreCorto = Input.NombreCorto?.Trim() ?? string.Empty;
            Input.Rfc = Input.Rfc?.Trim().ToUpperInvariant() ?? string.Empty;
            Input.Nivel = Input.Nivel?.Trim();
            Input.ActividadComercial = Input.ActividadComercial?.Trim();
            Input.TelefonoBancos = Input.TelefonoBancos?.Trim();
            Input.CorreoBancos = Input.CorreoBancos?.Trim();
            Input.NumeroEscritura = Input.NumeroEscritura?.Trim();
            Input.DomicilioFiscal = Input.DomicilioFiscal?.Trim();
            Input.Observaciones = Input.Observaciones?.Trim();
        }

        public class EmpresaInputModel
        {
            [Required(ErrorMessage = "La razón social es obligatoria.")]
            [StringLength(250)]
            [Display(Name = "Razón social")]
            public string RazonSocial { get; set; } = string.Empty;

            [Required(ErrorMessage = "El nombre corto es obligatorio.")]
            [StringLength(150)]
            [Display(Name = "Nombre corto")]
            public string NombreCorto { get; set; } = string.Empty;

            [Required(ErrorMessage = "El RFC es obligatorio.")]
            [StringLength(
                13,
                MinimumLength = 12,
                ErrorMessage = "El RFC debe contener entre 12 y 13 caracteres.")]
            [RegularExpression(
                @"^[A-ZÑ&]{3,4}\d{6}[A-Z0-9]{3}$",
                ErrorMessage = "El formato del RFC no es válido.")]
            [Display(Name = "RFC")]
            public string Rfc { get; set; } = string.Empty;

            [StringLength(100)]
            [Display(Name = "Nivel")]
            public string? Nivel { get; set; }

            [StringLength(500)]
            [Display(Name = "Actividad comercial")]
            public string? ActividadComercial { get; set; }

            [StringLength(30)]
            [Phone(ErrorMessage = "El número telefónico no es válido.")]
            [Display(Name = "Teléfono de bancos")]
            public string? TelefonoBancos { get; set; }

            [StringLength(200)]
            [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
            [Display(Name = "Correo de bancos")]
            public string? CorreoBancos { get; set; }

            [DataType(DataType.Date)]
            [Display(Name = "Fecha de constitución")]
            public DateTime? FechaConstitucion { get; set; }

            [StringLength(200)]
            [Display(Name = "Número de escritura")]
            public string? NumeroEscritura { get; set; }

            [StringLength(500)]
            [Display(Name = "Domicilio fiscal")]
            public string? DomicilioFiscal { get; set; }

            [StringLength(1000)]
            [Display(Name = "Observaciones")]
            public string? Observaciones { get; set; }
        }
    }
}