using ERPSEI.Pages.Shared;
using ERPSEI.Requests;
using ERPSEI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using static ERPSEI.Areas.ERP.Pages.PrefacturasModel;

namespace ERPSEI.Areas.Catalogos.Pages
{
	[Authorize(Policy = "AccessPolicy")]
	public class CuentasContablesModel(
			IStringLocalizer<CuentasContablesModel> localizer,
			ILogger<CuentasContablesModel> logger
		) : ERPPageModel
	{
		[BindProperty]
		public FiltroModel InputFiltro { get; set; } = new FiltroModel();

		public class FiltroModel
		{
			[Display(Name = "EmpresaField")]
			public string? EmpresaRFC { get; set; }

			[Display(Name = "ClienteField")]
			public string? ClienteRFC { get; set; }

			[Display(Name = "ProveedorField")]
			public string? ProveedorRFC { get; set; }

			[Display(Name = "TipoField")]
			public int? TipoId { get; set; }

			[Display(Name = "SubtipoField")]
			public int? SubtipoId { get; set; }
		}

		[BindProperty]
		public CuentaContableModel InputCuenta { get; set; } = new CuentaContableModel();

		public class CuentaContableModel
		{
			public int Id { get; set; }

			[Required(ErrorMessage = "Required")]
			[Display(Name = "CuentaField")]
			public string Cuenta { get; set; } = string.Empty;

			[Required(ErrorMessage = "Required")]
			[Display(Name = "FullNameField")]
			public string Nombre { get; set; } = string.Empty;

			[Display(Name = "RFCField")]
			public string? RFC { get; set; }

			[Required(ErrorMessage = "Required")]
			[Display(Name = "EmpresaField")]
			public int EmpresaId { get; set; }

			[Required(ErrorMessage = "Required")]
			[Display(Name = "TipoField")]
			public int TipoId { get; set; }

			[Required(ErrorMessage = "Required")]
			[Display(Name = "SubtipoField")]
			public int SubtipoId { get; set; }
		}

		public IActionResult OnGet()
		{
			return Page();
		}

		public async Task<JsonResult> OnPostFiltrar()
		{
			ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);
			try
			{
				if (PuedeTodo || PuedeConsultar || PuedeEditar || PuedeEliminar)
				{
					//resp.Datos = await GetComprobantesList(InputFiltro);
					resp.TieneError = false;
					resp.Mensaje = localizer["ConsultadoSuccessfully"];
				}
				else
				{
					resp.Mensaje = localizer["AccesoDenegado"];
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				logger.LogError("{message}", message);
			}

			return new JsonResult(resp);
		}

		private static string JsonEscape(string str)
		{
			return str.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t").Replace("\"", "\\\"");
		}
	}
}
