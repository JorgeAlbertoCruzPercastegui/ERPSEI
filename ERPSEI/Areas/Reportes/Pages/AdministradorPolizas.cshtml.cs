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
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using ERPSEI.Data.Managers.Cuentas;

namespace ERPSEI.Areas.Reportes.Pages
{
	[Authorize]
	public class AdministradorPolizasModel : ERPPageModel
	{
		private readonly IGruposPolizasManager _gruposPolizasManager;
		private readonly IPolizasManager _polizasManager;
		private readonly IPolizasDetalles _polizasDetallesManager;
		private readonly IPolizasTipos _polizasTiposManager;
        private readonly ICuentaContableManager _cuentaContableManager;
        private readonly IEmpresaManager _empresaManager;
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
            ICuentaContableManager cuentaContableManager,
            IEmpresaManager empresaManager,
			IStringLocalizer<AdministradorPolizasModel> _stringLocalizer,
			ILogger<AdministradorPolizasModel> _logger,
			Data.ApplicationDbContext _db
		)
		{
			_gruposPolizasManager = polizasTiposManager;
			_polizasManager = polizasManager;
			_polizasDetallesManager = polizasDetalles;
			_polizasTiposManager = polizasTipos;
            _cuentaContableManager = cuentaContableManager;
            _empresaManager = empresaManager;
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

        public async Task<IActionResult> OnGetPolizasConsolidado(int grupoId)
        {
            ServerResponse resp = new(true, stringLocalizer["PolizaObtenidaUnsuccessfully"]);

            try
            {
                var grupo = await _gruposPolizasManager.GetByIdAsync(grupoId);
                if (grupo == null)
                {
                    resp.Mensaje = stringLocalizer["GrupoNoEncontrado"];
                    return new JsonResult(resp);
                }

                var polizas = await _polizasManager.GetByGrupoIdAsync(grupoId);
                if (polizas == null || polizas.Count == 0)
                {
                    resp.Mensaje = stringLocalizer["PolizasNoEncontradas"];
                    return new JsonResult(resp);
                }

                var detalles = await _polizasDetallesManager.GetAllAsync();
                var detallesFiltrados = detalles?.Where(d => polizas.Select(p => p.Id).Contains(d.PolizaId)).ToList();
                if (detallesFiltrados == null || detallesFiltrados.Count == 0)
                {
                    resp.Mensaje = stringLocalizer["DetallesNoEncontrados"];
                    return new JsonResult(resp);
                }

                var cuentasContables = await _cuentaContableManager.GetAllAsync();
                var cuentasFiltradas = cuentasContables.Where(c => (c.TipoId == 1 || c.TipoId == 2 || c.TipoId == 3) && (c.SubtipoId == 1 || c.SubtipoId == 4 || c.SubtipoId == 19)).ToList();
                var cuentasDic = cuentasContables.ToDictionary(c => c.Id, c => c);

                var empresas = await _cuentaContableManager.GetAllAsync();
                var empresaDic = empresas.ToDictionary(c => c.Id, c => c);

                // Extraer contenido entre comillas dobles de polizas[0].Concepto
                string concepto = polizas[0].Concepto;
                string contenidoDeseado;
                if (concepto.Contains("\""))
                {
                    contenidoDeseado = System.Text.RegularExpressions.Regex.Replace(concepto, @"INGRESOS\s*""([^""]*)""\s*SEI-F-\d+", "$1");
                    contenidoDeseado = $"\"{contenidoDeseado}\"";
                }
                else
                {
                    contenidoDeseado = System.Text.RegularExpressions.Regex.Replace(concepto, @"INGRESOS\s*([^""]*)\s*SEI-F-\d+", "$1");
                }

                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Polizas Consolidado");

                // Encabezados en la fila 3
                var headers = new[] { "lg", "1", $"{polizas[0].Concepto}", "28", "CARGO", "ABONO" };
                IRow headerRow = sheet.CreateRow(2); // Índice 2 para la fila 3
                for (int i = 0; i < headers.Length; i++)
                {
                    ICell cell = headerRow.CreateCell(i < 4 ? i : i + 1); // Mover CARGO y ABONO
                    cell.SetCellValue(headers[i]);
                    ICellStyle style = workbook.CreateCellStyle();
                    IFont font = workbook.CreateFont();
                    font.IsBold = true;
                    style.SetFont(font);
                    cell.CellStyle = style;
                }

                // Filas y columnas específicas
                int[] filas = { 3, 4, 5, 6 };
                foreach (int fila in filas)
                {
                    IRow row = sheet.CreateRow(fila);

                    // Columna C (columna 2 en índice)
                    ICell cellC = row.CreateCell(2);
                    cellC.SetCellValue(0.ToString());
                    ICellStyle styleC = workbook.CreateCellStyle();
                    styleC.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center; // Ajuste para alineación horizontal
                    cellC.CellStyle = styleC;
                }

                // Columna D (columna 3 en índice) con el concepto en las filas 4, 5, 6, 7
                for (int i = 3; i <= 6; i++)
                {
                    ICell cellD = sheet.GetRow(i)?.CreateCell(3);
                    if (cellD != null)
                    {
                        cellD.SetCellValue($"{polizas[0].Concepto}");
                    }
                }

                // Colocar valor de Debe en la fila 4, columna F
                IRow fila4 = sheet.GetRow(3);
                if (fila4 != null)
                {
                    fila4.CreateCell(5).SetCellValue(FormatDecimal(detallesFiltrados[0].Debe));

                    // Colocar valor de Haber en la fila 7, columna G
                    IRow fila7 = sheet.GetRow(6);
                    if (fila7 != null)
                    {
                        fila7.CreateCell(6).SetCellValue(FormatDecimal(detallesFiltrados[0].Haber));

                        // Colocar valor de cuenta en la fila 7, columna B
                        fila7.CreateCell(1).SetCellValue(cuentasDic[detallesFiltrados[0].CuentaId].Cuenta);
                    }

                    // Colocar valores específicos en las filas 5 y 6, columna B
                    IRow fila5 = sheet.GetRow(4);
                    if (fila5 != null)
                    {
                        fila5.CreateCell(1).SetCellValue("2180-001-000");
                    }

                    IRow fila6 = sheet.GetRow(5);
                    if (fila6 != null)
                    {
                        fila6.CreateCell(1).SetCellValue("2181-001-000");
                    }

                    // Colocar FIN_PARTIDAS en la fila 8, columna B
                    IRow fila8 = sheet.GetRow(7) ?? sheet.CreateRow(7);
                    fila8.CreateCell(1).SetCellValue("FIN_PARTIDAS");

                    // Datos a partir de la fila 9
                    int rowNumber = 8; // Comienza desde la fila 9
                    foreach (var poliza in polizas)
                    {
                        var detallesPoliza = detallesFiltrados.Where(d => d.PolizaId == poliza.Id).ToList();
                        foreach (var detalle in detallesPoliza)
                        {
                            IRow row = sheet.CreateRow(rowNumber);
                            //row.CreateCell(0).SetCellValue(contenidoDeseado); // Colocar contenido entre comillas

                            // Comparar con el campo Nombre de las cuentas bancarias y extraer la cuenta correspondiente
                            var cuentaBancaria = cuentasContables.FirstOrDefault(c => c.Nombre == contenidoDeseado);
                            if (cuentaBancaria != null)
                            {
                                //row.CreateCell(1).SetCellValue(cuentaBancaria.Cuenta);

                                // Colocar la cuenta en la fila 4, columna B
                                fila4.CreateCell(1).SetCellValue(cuentaBancaria.Cuenta);
                            }
                            else 
                            {
                                fila4.CreateCell(1).SetCellValue("");
                            }

                            row.CreateCell(2).SetCellValue("");
                            row.CreateCell(3).SetCellValue("");
                            row.CreateCell(4).SetCellValue(""); // Columna E vacía
                            row.CreateCell(5).SetCellValue(""); // Columna F vacía
                            row.CreateCell(6).SetCellValue(""); // Columna G vacía
                            rowNumber++;
                        }
                    }

                    // Estilizar las celdas de cabecera A3 y B3
                    ICellStyle headerStyle = workbook.CreateCellStyle();
                    headerStyle.FillForegroundColor = IndexedColors.LightBlue.Index;
                    headerStyle.FillPattern = FillPattern.SolidForeground;

                    headerRow.GetCell(0).CellStyle = headerStyle;
                    headerRow.GetCell(1).CellStyle = headerStyle;

                    // Ajustar ancho de las columnas
                    for (int i = 0; i < headers.Length; i++)
                    {
                        sheet.AutoSizeColumn(i);
                    }
                    // Ajustar el ancho de la columna E
                    sheet.SetColumnWidth(4, 20 * 256); // Columna E con ancho ajustado

                    // Convertir a un array de bytes
                    using (var stream = new MemoryStream())
                    {
                        workbook.Write(stream);
                        var content = stream.ToArray();
                        var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        var fileName = "PolizasConsolidado.xlsx";
                        return File(content, contentType, fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error");
            }

            return new JsonResult(resp);
        }

        private static string FormatDecimal(decimal value)
        {
            var formattedValue = Math.Truncate(value * 100) / 100; // Capturar los dos primeros dígitos después del punto decimal
            return formattedValue.ToString("0.##"); // Formatear como string, mostrar dos decimales si es necesario
        }









    }
}

