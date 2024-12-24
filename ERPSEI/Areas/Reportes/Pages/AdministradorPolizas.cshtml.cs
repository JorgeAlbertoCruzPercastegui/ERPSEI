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
using System.Text.RegularExpressions;

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
			ServerResponse resp = new(true, stringLocalizer["PolizasFiltradosUnsuccessfully"]);

			try
			{
				resp.Datos = await GetAdminPolizasList(InputFiltro);
				resp.TieneError = false;
				resp.Mensaje = stringLocalizer["PolizasFiltradosSuccessfully"];
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
			}

			return new JsonResult(resp);
		}

		private async Task<string> GetAdminPolizasList(FiltroModel? filtro = null)
		{
			List<object> jsonPolizas = new List<object>();
			List<GrupoPoliza> gruposPolizas;

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

			// Construir el JSON con objetos
			foreach (GrupoPoliza grupo in gruposPolizas)
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
		public async Task<JsonResult> OnGetPolizas()
		{
			ServerResponse resp = new(true, stringLocalizer["PolizasObtenidasUnsuccessfully"]);

			try
			{
				resp.Datos = await GetgrupoPolizasList();
				resp.TieneError = false;
				resp.Mensaje = stringLocalizer["PolizasObtenidasSuccessfully"];
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
			}

			return new JsonResult(resp);
		}

		public async Task<JsonResult> GetgrupoPolizasList()
		{
			List<string> jsonGruposPolizas = new List<string>();
			List<GrupoPoliza> gruposPolizas = await _gruposPolizasManager.GetAllAsync();

			foreach (GrupoPoliza grupo in gruposPolizas)
			{
				string usuarioCreador = grupo.UsuarioCreador?.Empleado?.NombreCompleto ?? grupo.UsuarioCreador?.UserName ?? "-";
				string usuarioModificador = grupo.UsuarioModificador?.Empleado?.NombreCompleto ?? grupo.UsuarioModificador?.UserName ?? "-";

				jsonGruposPolizas.Add("{" +
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

			string jsonResponse = $"[{string.Join(",", jsonGruposPolizas)}]";
			return new JsonResult(jsonResponse);
		}

        public async Task<JsonResult> OnGetVPolizas()
        {
            ServerResponse resp = new(true, stringLocalizer["PolizasObtenidasUnsuccessfully"]);

            try
            {
                resp.Datos = await GetVPolizasList();
                resp.TieneError = false;
                resp.Mensaje = stringLocalizer["PolizasObtenidasSuccessfully"];
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }

            return new JsonResult(resp);
        }

        public async Task<JsonResult> GetVPolizasList()
        {
            List<string> jsonVPolizas = new List<string>();
            List<VPoliza> vPolizas = await _polizasManager.GetAllAsync();

            foreach (VPoliza vpoliza in vPolizas)
            {

                jsonVPolizas.Add("{" +
                $"\"Id\": \"{vpoliza.Id}\", " +
                $"\"GrupoId\": \"{vpoliza.GrupoId}\", " +
                $"\"TipoId\": \"{vpoliza.TipoId}\", " +
                $"\"FechaHora\": \"{vpoliza.FechaHora}\", " +
                $"\"Concepto\": \"{vpoliza.Concepto}\"" +
                "}");
            }

            string jsonResponse = $"[{string.Join(",", jsonVPolizas)}]";
            return new JsonResult(jsonResponse);
        }

        public async Task<JsonResult> OnGetPolizasDetalles()
        {
            ServerResponse resp = new(true, stringLocalizer["PolizasObtenidasUnsuccessfully"]);

            try
            {
                resp.Datos = await GetPolizasDetallesList();
                resp.TieneError = false;
                resp.Mensaje = stringLocalizer["PolizasObtenidasSuccessfully"];
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }

            return new JsonResult(resp);
        }

        public async Task<JsonResult> GetPolizasDetallesList()
        {
            List<string> jsonPolizasDetalles = new List<string>();
            List<PolizaDetalle> polizasDetalles = await _polizasDetallesManager.GetAllAsync();

            foreach (PolizaDetalle polizadetalle in polizasDetalles)
            {

                jsonPolizasDetalles.Add("{" +
                $"\"Id\": \"{polizadetalle.Id}\", " +
                $"\"PolizaId\": \"{polizadetalle.PolizaId}\", " +
                $"\"CuentaId\": \"{polizadetalle.CuentaId}\", " +
                $"\"Concepto\": \"{polizadetalle.Concepto}\", " +
                $"\"Debe\": \"{polizadetalle.Debe}\", " +
                $"\"Haber\": \"{polizadetalle.Haber}\"" +
                "}");
            }

            string jsonResponse = $"[{string.Join(",", jsonPolizasDetalles)}]";
            return new JsonResult(jsonResponse);
        }

        public async Task<JsonResult> OnGetPolizasTipos()
        {
            ServerResponse resp = new(true, stringLocalizer["PolizasObtenidasUnsuccessfully"]);

            try
            {
                resp.Datos = await GetPolizasTiposList();
                resp.TieneError = false;
                resp.Mensaje = stringLocalizer["PolizasObtenidasSuccessfully"];
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }

            return new JsonResult(resp);
        }

        public async Task<JsonResult> GetPolizasTiposList()
        {
            List<string> jsonPolizasTipos = new List<string>();
            List<PolizaTipo> polizasTipo = await _polizasTiposManager.GetAllAsync();

            foreach (PolizaTipo polizatipo in polizasTipo)
            {

                jsonPolizasTipos.Add("{" +
                $"\"Id\": \"{polizatipo.Id}\", " +
                $"\"PolizaId\": \"{polizatipo.Descripcion}\", " +
                $"\"Haber\": \"{polizatipo.Deshabilitado}\"" +
                "}");
            }

            string jsonResponse = $"[{string.Join(",", jsonPolizasTipos)}]";
            return new JsonResult(jsonResponse);
        }

        public async Task<JsonResult> OnGetPolizasConsolidado()
        {
            ServerResponse resp = new(true, stringLocalizer["PolizasObtenidasUnsuccessfully"]);

            try
            {
                var result = new
                {
                    GrupoPolizas = await GetJsonList(
                        _gruposPolizasManager.GetAllAsync(),
                        grupo => new
                        {
                            grupo.Id,
                            FechaHoraCreacion = grupo.FechaHoraCreacion,
                            FechaHoraCreacionJS = grupo.FechaHoraCreacion,
                            FechaHoraModificacion = grupo.FechaHoraModificacion,
                            FechaHoraModificacionJS = grupo.FechaHoraModificacion,
                            grupo.NumeroImpresion,
                            grupo.UsuarioCreadorId,
                            UsuarioCreador = grupo.UsuarioCreador?.Empleado?.NombreCompleto ?? grupo.UsuarioCreador?.UserName ?? "-",
                            grupo.UsuarioModificadorId,
                            UsuarioModificador = grupo.UsuarioModificador?.Empleado?.NombreCompleto ?? grupo.UsuarioModificador?.UserName ?? "-",
                            grupo.Deshabilitado
                        }),

                    VPolizas = await GetJsonList(
                        _polizasManager.GetAllAsync(),
                        vpoliza => new
                        {
                            vpoliza.Id,
                            vpoliza.GrupoId,
                            vpoliza.TipoId,
                            vpoliza.FechaHora,
                            vpoliza.Concepto
                        }),

                    PolizasDetalles = await GetJsonList(
                        _polizasDetallesManager.GetAllAsync(),
                        detalle => new
                        {
                            detalle.Id,
                            detalle.PolizaId,
                            detalle.CuentaId,
                            detalle.Concepto,
                            detalle.Debe,
                            detalle.Haber
                        }),

                    PolizasTipos = await GetJsonList(
                        _polizasTiposManager.GetAllAsync(),
                        tipo => new
                        {
                            tipo.Id,
                            tipo.Descripcion,
                            tipo.Deshabilitado
                        })
                };

                resp.Datos = result;
                resp.TieneError = false;
                resp.Mensaje = stringLocalizer["PolizasObtenidasSuccessfully"];
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }

            return new JsonResult(resp);
        }

        private async Task<List<object>> GetJsonList<T>(Task<List<T>> fetchDataTask, Func<T, object> transform)
        {
            List<T> data = await fetchDataTask;
            return data.Select(transform).ToList();
        }

    }
}

