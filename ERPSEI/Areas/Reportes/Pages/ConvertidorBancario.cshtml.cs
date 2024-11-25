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
	public class ConvertidorBancarioModel : ERPPageModel
	{
		[BindProperty]

		public Banco BancoList { get; set; }
		public InputFiltroModelAgregar InputFiltroModalAgregar { get; set; }
		public class InputFiltroModelAgregar
		{
			[Display(Name = "IdField")]
			//[StringLength(10, ErrorMessage = "FieldLength", MinimumLength = 1)]
			[RegularExpression(RegularExpressions.NumericNoRestriction, ErrorMessage = "PersonName")]
			public int? Id { get; set; }
			public int BancoId { get; set; }

		}
	}
}
