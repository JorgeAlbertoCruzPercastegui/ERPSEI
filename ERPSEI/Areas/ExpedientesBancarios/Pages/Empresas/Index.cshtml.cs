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
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
        }

        // =====================================================
        // LISTADO PARA BOOTSTRAP TABLE
        // GET ?handler=Empresas
        // =====================================================
        public async Task<IActionResult> OnGetEmpresasAsync(
            string? busqueda,
            string? estatus)
        {
            string filtro = busqueda?.Trim() ?? string.Empty;
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

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(x =>
                    x.RazonSocial.Contains(filtro) ||
                    x.NombreCorto.Contains(filtro) ||
                    x.Rfc.Contains(filtro) ||
                    (x.Nivel != null && x.Nivel.Contains(filtro)) ||
                    (x.ActividadComercial != null &&
                     x.ActividadComercial.Contains(filtro)));
            }

            var empresas = await query
                .OrderBy(x => x.RazonSocial)
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
        }

        // =====================================================
        // CONSULTAR REGISTRO
        // GET ?handler=Empresa&id=1
        // =====================================================
        public async Task<IActionResult> OnGetEmpresaAsync(int id)
        {
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
        }

        // =====================================================
        // CREAR EMPRESA
        // POST ?handler=Crear
        // =====================================================
        public async Task<IActionResult> OnPostCrearAsync(
            [FromBody] EmpresaRequest request)
        {
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

            return new JsonResult(new
            {
                success = true,
                message = "La empresa se registró correctamente.",
                id = empresa.Id
            });
        }

        // =====================================================
        // EDITAR EMPRESA
        // POST ?handler=Editar
        // =====================================================
        public async Task<IActionResult> OnPostEditarAsync(
            [FromBody] EmpresaRequest request)
        {
            NormalizarRequest(request);

            Dictionary<string, string[]> errores =
                ValidarRequest(request, requiereId: true);

            if (errores.Any())
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "Revisa la información capturada.",
                    errors = errores
                });
            }

            var empresa = await _context.EbEmpresas
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (empresa == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No se encontró la empresa que deseas editar."
                });
            }

            bool rfcExistente = await _context.EbEmpresas
                .IgnoreQueryFilters()
                .AnyAsync(x =>
                    x.Rfc == request.Rfc &&
                    x.Id != request.Id);

            if (rfcExistente)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "El RFC ya está asignado a otra empresa.",
                    errors = new Dictionary<string, string[]>
                    {
                        ["Rfc"] = new[]
                        {
                            "El RFC ya está asignado a otra empresa."
                        }
                    }
                });
            }

            empresa.RazonSocial = request.RazonSocial;
            empresa.NombreCorto = request.NombreCorto;
            empresa.Rfc = request.Rfc;
            empresa.Nivel = request.Nivel;
            empresa.ActividadComercial = request.ActividadComercial;
            empresa.TelefonoBancos = request.TelefonoBancos;
            empresa.CorreoBancos = request.CorreoBancos;
            empresa.FechaConstitucion = request.FechaConstitucion;
            empresa.NumeroEscritura = request.NumeroEscritura;
            empresa.DomicilioFiscal = request.DomicilioFiscal;
            empresa.Observaciones = request.Observaciones;
            empresa.FechaActualizacion = DateTime.Now;
            empresa.UsuarioActualizacionId = ObtenerUsuarioId();

            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                success = true,
                message = "La empresa se actualizó correctamente."
            });
        }

        // =====================================================
        // HABILITAR / DESHABILITAR
        // POST ?handler=CambiarEstatus
        // =====================================================
        public async Task<IActionResult> OnPostCambiarEstatusAsync(
            [FromBody] EmpresaIdRequest request)
        {
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

            empresa.Deshabilitado = !empresa.Deshabilitado;
            empresa.FechaActualizacion = DateTime.Now;
            empresa.UsuarioActualizacionId = ObtenerUsuarioId();

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
        }

        // =====================================================
        // ELIMINACIÓN LÓGICA
        // POST ?handler=Eliminar
        // =====================================================
        public async Task<IActionResult> OnPostEliminarAsync(
            [FromBody] EmpresaIdRequest request)
        {
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
            empresa.Deshabilitado = true;
            empresa.FechaActualizacion = DateTime.Now;
            empresa.UsuarioActualizacionId = ObtenerUsuarioId();

            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                success = true,
                message = "La empresa se eliminó correctamente."
            });
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

            return errores;
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

        private string ObtenerUsuarioId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? "SYSTEM";
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
    }
}