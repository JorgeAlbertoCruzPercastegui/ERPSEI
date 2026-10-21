
using ERPSEI.Services.TimeIntegration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace ERPSEI.Areas.ERP.Pages
{
    [Authorize(Roles = "Administrador,Master")]
    public class TimeApiTestModel : PageModel
    {
        private readonly ITimeApiClient _timeApiClient;
        private readonly IWebHostEnvironment _environment;

        public TimeApiTestModel(
            ITimeApiClient timeApiClient,
            IWebHostEnvironment environment)
        {
            _timeApiClient = timeApiClient;
            _environment = environment;
        }

        public string? Resultado { get; private set; }
        public string? Error { get; private set; }

        public async Task OnGetAsync()
        {
            if (!_environment.IsDevelopment())
            {
                Response.StatusCode = 404;
                return;
            }

            try
            {
                using JsonDocument respuesta =
                    await _timeApiClient.ObtenerTablaAsync(
                        tabla: "periodos",
                        afterId: 0,
                        limit: 2,
                        cancellationToken: HttpContext.RequestAborted);

                Resultado = JsonSerializer.Serialize(
                    respuesta.RootElement,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
            }
            catch (Exception ex)
            {
                Error = ex is HttpRequestException
                    ? "Error HTTP al consultar TIME: " + ex.Message
                    : "No fue posible consultar TIME: " + ex.Message;
            }
        }
    }
}