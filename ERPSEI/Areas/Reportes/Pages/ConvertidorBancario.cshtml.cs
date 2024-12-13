using ERPSEI.Data.Entities.Conciliaciones;
using ERPSEI.Pages.Shared;
using ERPSEI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ERPSEI.Areas.Reportes.Pages
{
	[Authorize(Policy = "AccessPolicy")]
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
