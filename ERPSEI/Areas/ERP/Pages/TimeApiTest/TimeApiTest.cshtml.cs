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
        private readonly ITimeTableReader _timeTableReader;
        private readonly IWebHostEnvironment _environment;

        public TimeApiTestModel(
            ITimeTableReader timeTableReader,
            IWebHostEnvironment environment)
        {
            _timeTableReader = timeTableReader;
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
                int totalRegistros = 0;
                int totalLotes = 0;

                JsonElement? primerRegistro = null;
                JsonElement? ultimoRegistro = null;

                await _timeTableReader.ProcesarTablaPorLotesAsync(
                    tabla: "horas",

                    procesarLote: lote =>
                    {
                        totalLotes++;

                        totalRegistros +=
                            lote.Count;

                        if (!primerRegistro.HasValue &&
                            lote.Count > 0)
                        {
                            primerRegistro =
                                lote[0].Clone();
                        }

                        if (lote.Count > 0)
                        {
                            ultimoRegistro =
                                lote[^1].Clone();
                        }

                        return Task.CompletedTask;
                    },

                    limit: 500,

                    cancellationToken:
                        HttpContext.RequestAborted);

                var resumen = new
                {
                    tabla = "horas",

                    totalRegistros,

                    totalLotes,

                    registrosPorLote = 500,

                    primerRegistro,

                    ultimoRegistro
                };

                Resultado =
                    JsonSerializer.Serialize(
                        resumen,
                        new JsonSerializerOptions
                        {
                            WriteIndented = true
                        });
            }
            catch (Exception ex)
            {
                Error =
                    ex is HttpRequestException
                        ? "Error HTTP al consultar TIME: " + ex.Message
                        : "No fue posible consultar TIME: " + ex.Message;
            }
        }
    }
}