using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Entities.Empresas;
using ERPSEI.Data.Entities.SAT;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers.Empleados;
using ERPSEI.Data.Managers.Empresas;
using ERPSEI.Data.Managers.SAT;
using ERPSEI.Data.Managers.Usuarios;
using ERPSEI.Utils;
using iText.Html2pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using System.Net.Mime;

namespace ERPSEI.Pages
{
    [Authorize]
    public class FileViewerModel(
			IPrefacturaManager prefacturaManager,
            IArchivoEmpleadoManager userFileManager,
            IArchivoEmpresaManager archivoEmpresaManager,
            IStringLocalizer<FileViewerModel> localizer,
			IEncriptacionAES encriptacionAES,
			AppUserManager userManager,
			AppRoleManager roleManager
	) : PageModel
    {
        public string HtmlContainer { get; set; } = string.Empty;
		public string Base64 { get; set; } = string.Empty;
		public string Extension { get; set; } = string.Empty;

        private class FileToRender
        {
            public string src { get; set; } = string.Empty;
			public string name { get; set; } = string.Empty;
            public string extension { get; set; } = string.Empty;

            public FileToRender(){}
        }

		public async Task<IActionResult> OnGet(string safeL)
		{
			try
			{
				safeL = encriptacionAES.Base64AESToPlainText(safeL);
				string[] urlParts = safeL.Split(['&', '='], StringSplitOptions.RemoveEmptyEntries);

				if (urlParts.Length >= 6) {
					string userId = urlParts[1];
					AppUser? usr = await userManager.GetUserAsync(User);

					if(usr != null && userId == usr.Id){ return await SearchFile(urlParts[1], urlParts[3], urlParts[5]); }
				}
			}
			catch (Exception)
			{
				return RedirectToPage("/404");
			}

			return RedirectToPage("/404");
		}

		private async Task<bool> UsuarioPuedeEditarEmpresas(string userId)
		{
            AppUser? usr = await userManager.FindByIdAsync(userId);
            IList<string> rolesUsuario = usr != null ? await userManager.GetRolesAsync(usr) : [];
            List<AccesoModulo> accesos = [];
            foreach (string rol in rolesUsuario)
            {
                AppRole? foundRole = await roleManager.GetByNameAsync(rol);
                accesos.AddRange(foundRole?.Accesos.Where(acceso => acceso.Modulo?.NombreNormalizado == "empresas" && (acceso.PuedeTodo == 1 || acceso.PuedeEditar == 1)) ?? []);
            }

			return accesos.Count >= 1;
        }

		private async Task<IActionResult> SearchFile(string userId, string fileId, string moduleName)
		{
			try
			{
				if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(fileId) || string.IsNullOrEmpty(moduleName)) { return RedirectToPage("/404"); }

				FileToRender? ftr = null;
				switch (moduleName)
				{
					case "gestiondetalento":
					case "perfil":
						ftr = GetArchivoEmpleadoB64(fileId);
                        break;
					case "empresas":
                        //Se busca si el usuario que está intentando ver el archivo, tiene accesos de edición al módulo de empresas.
						if(await UsuarioPuedeEditarEmpresas(userId)) { ftr = GetArchivoEmpresaB64(fileId); }
						break;
					case "prefacturas":
						int idPf = 0;
						if (!int.TryParse(fileId, out idPf)) { idPf = 0; }
						ftr = await GetPrefacturaB64(idPf);
						break;
					default:
						HtmlContainer = string.Empty;
						break;
				}

				if (ftr == null) { return RedirectToPage("/404"); }
				if (ftr.src.Length <= 0) { return RedirectToPage("/404"); }

				switch (ftr.extension)
				{
					case "pdf":
						HtmlContainer = $"<iframe id=\"fileContainer\" style=\"position:fixed; top:0; left:0; bottom:0; right:0; width:100%; height:100%; border:none; margin:0; padding:0; overflow:hidden; z-index:999999;\"></iframe>";
						Base64 = ftr.src;
						Extension = ftr.extension;
						break;
					case "png":
					case "jpg":
					case "jpeg":
						HtmlContainer = $"<img id=\"fileContainer\" style=\"height:100%\" />";
						Base64 = ftr.src;
						Extension = ftr.extension;
						break;
					default:
						//Show page file not viewable
						HtmlContainer = $"<div class=\"container\">" +
											$"<h1>{localizer["DownloadFileTitle"]}</h1>" +
											$"<p>{localizer["DownloadFileInstructions"]}</p>" +
											$"<a id=\"fileContainer\" href=\"{Url.Page("FileViewer", "DownloadFile", new { fileId, moduleName })}\" download class=\"btn btn-primary\">{localizer["DownloadFileTitle"]}</a>" +
										$"</div>";
						break;
				}

				return Page();
			}
			catch (Exception)
			{
				return RedirectToPage("/404");
			}
		}

        public ActionResult OnGetDownloadFile(string fileId, string moduleName)
        {
			FileToRender? ftr = null;
            switch (moduleName)
			{
				case "empresas":
					ftr = GetArchivoEmpresaB64(fileId);
					break;
				default:
					break;
			}

			if(ftr != null){ return File(Convert.FromBase64String(ftr.src), MediaTypeNames.Application.Octet, $"{ftr.name}.{ftr.extension}"); }

			return new EmptyResult();
        }

        private FileToRender? GetArchivoEmpleadoB64(string fileId) 
        {
            FileToRender ftr = new();
			ArchivoEmpleado? f1 = userFileManager.GetFileById(fileId);
			if (f1 == null) { return null; }
			ftr.src = Convert.ToBase64String(f1.Archivo);
            ftr.name = f1.Nombre;
            ftr.extension = f1.Extension;

            return ftr;
		}

		private FileToRender? GetArchivoEmpresaB64(string fileId)
		{
			FileToRender ftr = new();
			ArchivoEmpresa? f2 = archivoEmpresaManager.GetFileById(fileId);
			if (f2 == null) { return null; }
			ftr.src = Convert.ToBase64String(f2.Archivo);
			ftr.name = f2.Nombre;
			ftr.extension = f2.Extension;

            return ftr;
		}

		private async Task<FileToRender?> GetPrefacturaB64(int idPf) 
        {
			FileToRender ftr = new();

			Prefactura? pf = await prefacturaManager.GetByIdAsync(idPf);
			if (pf == null) { return null; }

			string filepath = $"wwwroot/templates/{Guid.NewGuid()}.pdf";
			HtmlConverter.ConvertToPdf(PrefacturaToHTML(pf), new FileStream(filepath, FileMode.Create));

			ftr.src = await FileToB64(filepath);
			ftr.extension = "pdf";

            return ftr;
		}
        private static string PrefacturaToHTML(Prefactura pf)
        {
			decimal subtotalTotal = 0,
					descuentosTotal = 0,
					trasladosTotal = 0,
					retencionesTotal = 0,
					totalTotal = 0;

			List<string> htmlConceptos = [];
            foreach (Concepto c in pf.Conceptos)
            {
				decimal subtotal = c.Cantidad * c.PrecioUnitario,
						total = subtotal - c.Descuento + c.Traslado - c.Retencion;

				subtotalTotal += subtotal;
				descuentosTotal += c.Descuento;
				trasladosTotal += c.Traslado;
				retencionesTotal += c.Retencion;
				totalTotal += total;

				htmlConceptos.Add("<tr style='text-align: center;'>" +
									  $"<td>{c.Cantidad}</td>" +
									  $"<td>{c.UnidadMedida?.Clave}</td>" +
									  $"<td>{c.ProductoServicio?.Clave}</td>" +
									  $"<td>{c.Descripcion}</td>" +
									  $"<td>$ {c.Descuento}</td>" +
									  $"<td>$ {c.PrecioUnitario}</td>" +
									  $"<td>$ {total}</td>" +
								  "</tr>");
            }

            string html = "<html style='font-size: 12px;'>" +
                               "<body>" +
                                   $"<center><h1>{pf.Emisor?.RazonSocial}</h1></center>" +
                                   "<table border='0'>" +
                                        "<tr>" +
                                            "<td width='15%' align='right'><b>RFC: </b></td>" +
											$"<td width='45%'>{pf.Emisor?.RFC}</td>" +
											"<td width='15%' align='right'><b>Comprobante fiscal digital: </b></td>" +
											$"<td width='25%'>({pf.TipoComprobante?.Clave}){pf.TipoComprobante?.Descripcion}</td>" +
										"</tr>" +
										"<tr>" +
											"<td align='right'><b>Regimen fiscal: </b></td>" +
											$"<td>({pf.Emisor?.RegimenFiscal?.Clave}){pf.Emisor?.RegimenFiscal?.Descripcion}</td>" +
											"<td align='right'><b>Serie: </b></td>" +
											$"<td>{pf.Serie}</td>" +
										"</tr>" +
										"<tr>" +
											"<td align='right'><b>Domicilio fiscal: </b></td>" +
											$"<td>{pf.Emisor?.DomicilioFiscal}</td>" +
											"<td align='right'><b>Folio: </b></td>" +
											$"<td>{pf.Folio}</td>" +
										"</tr>" +
										"<tr>" +
											"<td align='right'><b>Expedido en: </b></td>" +
											$"<td> --- </td>" +
											"<td align='right'><b>Fecha: </b></td>" +
											$"<td>{pf.Fecha:yyyy-MM-ddThh:mm:ss}</td>" +
										"</tr>" +
										"<tr>" +
											"<td align='right'><b>Lugar de expedición: </b></td>" +
											$"<td> --- </td>" +
											"<td align='right'><b>Forma de pago: </b></td>" +
											$"<td>({pf.FormaPago?.Clave}){pf.FormaPago?.Descripcion}</td>" +
										"</tr>" +
										"<tr>" +
											"<td align='right'><b>Facturado a RFC: </b></td>" +
											$"<td>{pf.Receptor?.RFC} - {pf.Receptor?.RazonSocial}</td>" +
											"<td align='right'><b>Método de pago: </b></td>" +
											$"<td>({pf.MetodoPago?.Clave}){pf.MetodoPago?.Descripcion}</td>" +
										"</tr>" +
										"<tr>" +
											"<td align='right'><b>Régimen fiscal: </b></td>" +
											$"<td>({pf.Receptor?.RegimenFiscal?.Clave}){pf.Receptor?.RegimenFiscal?.Descripcion}</td>" +
											"<td align='right'><b>Uso de CFDI: </b></td>" +
											$"<td>({pf.UsoCFDI?.Clave}){pf.UsoCFDI?.Descripcion}</td>" +
										"</tr>" +
								   "</table>" +
								   "<br />" +
								   "<br />" +
								   "<table border='1' style='border-spacing: 0px;'>" +
										"<tr>" +
											"<th width='14.28%'>Cantidad</th>" +
											"<th width='14.28%'>Unidad</th>" +
											"<th width='14.28%'>Clave</th>" +
											"<th width='14.28%'>Descripcion</th>" +
											"<th width='14.28%'>Desc</th>" +
											"<th width='14.28%'>P/U</th>" +
											"<th width='14.28%'>Importe</th>" +
										"</tr>" +
										$"{string.Join("", htmlConceptos.ToArray())}" +
										"<tr>" +
											"<th colspan='5'></th>" +
											"<th>Subtotal</th>" +
											$"<th>$ {subtotalTotal}</th>" +
										"</tr>" +
										"<tr>" +
											"<th colspan='5'></th>" +
											"<th>Descuento</th>" +
											$"<th>$ {descuentosTotal}</th>" +
										"</tr>" +
										"<tr>" +
											"<th colspan='5'></th>" +
											"<th>Traslados</th>" +
											$"<th>$ {trasladosTotal}</th>" +
										"</tr>" +
										"<tr>" +
											"<th colspan='5'></th>" +
											"<th>Retenciones</th>" +
											$"<th>$ {retencionesTotal}</th>" +
										"</tr>" +
										"<tr>" +
											"<th colspan='5'></th>" +
											"<th>Total</th>" +
											$"<th>$ {totalTotal}</th>" +
										"</tr>" +
								   "</table>" +
							   "</body>" +
                          "</html>";

            return html;
		}
        private static async Task<string> FileToB64(string filepath)
        {
			byte[] buffer;
			using (FileStream pdf = System.IO.File.OpenRead(filepath))
			{
				buffer = new byte[pdf.Length];
				await pdf.ReadAsync(buffer);
				pdf.Close();
			}
			System.IO.File.Delete(filepath);

            return Convert.ToBase64String(buffer);
		}
	}
}
