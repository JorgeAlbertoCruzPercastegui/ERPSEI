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
using ERPSEI.Data.Managers.Conciliaciones;
using ERPSEI.Data.Entities.Polizas;
using ERPSEI.Data.Managers.Polizas;
using ERPSEI.Data.Managers.Empresas;
using ERPSEI.Data.Managers.AdministradorPolizas;

namespace ERPSEI.Areas.Reportes.Pages
{
	[Authorize]
	public class AdministradorPolizasModel : ERPPageModel
	{
		private readonly IGruposPolizasManager _gruposPolizasManager;
		private readonly IPolizasManager _polizasManager;
		private readonly IPolizasDetalles _polizasDetallesManager;
		private readonly IPolizasTipos _polizasTiposManager;
		private readonly IStringLocalizer<AdministradorPolizasModel> stringLocalizer;
		private readonly ILogger<AdministradorPolizasModel> logger;
		private readonly Data.ApplicationDbContext db;


		[BindProperty]
		public FiltroModel InputFiltro { get; set; } = new FiltroModel();

		public class FiltroModel
		{
			public int? Id { get; set; }

			[DataType(DataType.DateTime)]
			public DateTime? FechaInicio { get; set; }

			[DataType(DataType.DateTime)]
			public DateTime? FechaFin { get; set; }

			[DataType(DataType.Text)]
			public string? UsuarioCreador { get; set; }

			[DataType(DataType.Text)]
			public string? UsuarioModificador { get; set; }

			[Display(Name = "PolizaNumImpresionField")]
			public int? NumeroImpresion { get; set; }

			[Display(Name = "DeshabilitadoField")]
			public bool Deshabilitado { get; set; } = false;
		}

		public AdministradorPolizasModel(
			IGruposPolizasManager polizasTiposManager,
			IPolizasManager polizasManager,
			IPolizasDetalles polizasDetalles,
			IPolizasTipos polizasTipos,
			IStringLocalizer<AdministradorPolizasModel> _stringLocalizer,
			ILogger<AdministradorPolizasModel> _logger,
			Data.ApplicationDbContext _db
		)
		{
			_gruposPolizasManager = polizasTiposManager;
			_polizasManager = polizasManager;
			_polizasDetallesManager = polizasDetalles;
			_polizasTiposManager = polizasTipos;
			stringLocalizer = _stringLocalizer;
			logger = _logger;
			db = _db;

			InputFiltro = new FiltroModel();
		}

		public async Task<JsonResult> OnPostFiltrarPolizas()
		{
			// Inicializar la respuesta con mensaje de error por defecto
			ServerResponse resp = new(true, stringLocalizer["PolizasFiltradosUnsuccessfully"]);

			try
			{
				resp.Datos = await GetAdminPolizasList(InputFiltro);
				resp.TieneError = false;
				resp.Mensaje = stringLocalizer["PolizasFiltradosSuccessfully"];
			}
			catch (Exception ex)
			{
				// Registrar el error en el log
				logger.LogError(ex.Message);
			}

			return new JsonResult(resp);
		}

		private async Task<string> GetAdminPolizasList(FiltroModel? filtro = null)
		{
			List<object> jsonPolizas = new List<object>();
			List<GruposPolizas> gruposPolizas;

			// Aplicar los filtros a la llamada a GetAllAsync
			if (filtro != null)
			{
				gruposPolizas = await _gruposPolizasManager.GetAllAsync(
					filtro.Id,
					filtro.UsuarioCreador,
					filtro.UsuarioModificador,
					filtro.FechaInicio,
					filtro.FechaFin,
					filtro.NumeroImpresion,
					filtro.Deshabilitado = false
				);
			}
			else
			{
				// Si no hay filtros, obtener todos los registros
				gruposPolizas = await _gruposPolizasManager.GetAllAsync();
			}

			// Construir el JSON con objetos anónimos
			foreach (GruposPolizas grupo in gruposPolizas)
			{
				string usuarioCreador = grupo.UsuarioCreador?.Empleado?.NombreCompleto ?? grupo.UsuarioCreador?.UserName ?? "-";
				string usuarioModificador = grupo.UsuarioModificador?.Empleado?.NombreCompleto ?? grupo.UsuarioModificador?.UserName ?? "-";

				jsonPolizas.Add("{" +
				$"\"Id\": \"{grupo.Id}\", " +
				$"\"FechaHoraCreacion\": \"{grupo.FechaHoraCreacion:dd/MM/yyyy HH:mm:ss}\", " +
				$"\"FechaHoraCreacionJS\": \"{grupo.FechaHoraCreacion:yyyy-MM-dd HH:mm:ss}\", " +
				$"\"FechaHoraModificacion\": \"{grupo.FechaHoraModificacion:dd/MM/yyyy HH:mm:ss}\", " +
				$"\"FechaHoraModificacionJS\": \"{grupo.FechaHoraModificacion:yyyy-MM-dd HH:mm:ss}\", " +
				$"\"NumeroImpresion\": \"{grupo.NumeroImpresion}\", " +
				$"\"UsuarioCreadorId\": \"{grupo.UsuarioCreadorId}\", " +
				$"\"UsuarioCreador\": \"{usuarioCreador}\", " +
				$"\"UsuarioModificadorId\": \"{grupo.UsuarioModificadorId}\", " +
				$"\"UsuarioModificador\": \"{usuarioModificador}\", " +
				$"\"Deshabilitado\": \"{grupo.Deshabilitado}\"" +
				"}");
			}

			string jsonResponse = $"[{string.Join(",", jsonPolizas)}]";
			return jsonResponse;
		}


	}
}
