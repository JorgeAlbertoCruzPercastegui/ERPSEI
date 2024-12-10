using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Entities.Reportes;
using ERPSEI.Data.Managers.Empleados;
using ERPSEI.Data.Managers.Reportes;
using ERPSEI.Pages.Shared;
using ERPSEI.Requests;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Net.Mime;
using System.Globalization;
using ERPSEI.Data.Entities.Conciliaciones;
using static ERPSEI.Areas.ERP.Pages.ConciliacionesModel;
using ERPSEI.Data.Entities.SAT.cfdiv40;
using ERPSEI.Resources;

namespace ERPSEI.Areas.Reportes.Pages
{
	[Authorize]
	public class AdministradorPolizasModel : ERPPageModel
	{
		private readonly IStringLocalizer<AdministradorPolizasModel> stringLocalizer;
		private readonly ILogger<AdministradorPolizasModel> logger;
		private readonly Data.ApplicationDbContext db;


		[BindProperty]
		public FiltroModel InputFiltro { get; set; } = new FiltroModel();

		public class FiltroModel
		{
			[DataType(DataType.Text)]
			[Display(Name = "PolizaNameField")]
			public string? NombrePoliza { get; set; }

			[DataType(DataType.DateTime)]
			[Display(Name = "CreateDateField")]
			public DateTime? FechaCreacion { get; set; }

			[DataType(DataType.Text)]
			[Display(Name = "CreateUserField")]
			public string? UsuarioCreador { get; set; }

			[DataType(DataType.Text)]
			[Display(Name = "ModifyUserField")]
			public string? UsuarioModificador { get; set; }
		}

		public AdministradorPolizasModel(
			IStringLocalizer<AdministradorPolizasModel> _stringLocalizer,
			ILogger<AdministradorPolizasModel> _logger,
			Data.ApplicationDbContext _db
		)
		{
			stringLocalizer = _stringLocalizer;
			logger = _logger;
			db = _db;

			InputFiltro = new FiltroModel();
		}
	}
}
