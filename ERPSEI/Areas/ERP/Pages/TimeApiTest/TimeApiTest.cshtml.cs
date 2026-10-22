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
        private readonly ITimeApiClient _timeApiClient;

        public TimeApiTestModel(
            ITimeTableReader timeTableReader,
            ITimeApiClient timeApiClient,
            IWebHostEnvironment environment)
        {
            _timeTableReader = timeTableReader;
            _timeApiClient = timeApiClient;
            _environment = environment;
        }

        public string? Resultado { get; private set; }
        public string? Error { get; private set; }




        public async Task OnGetAsync()
        {
            // =====================================================
            // SEGURIDAD: SOLO ENTORNO DE DESARROLLO
            // =====================================================

            if (!_environment.IsDevelopment())
            {
                Response.StatusCode = 404;
                return;
            }

            try
            {
                // =====================================================
                // TABLAS AUTORIZADAS DE TIME INTEGRATION API
                // =====================================================

                string[] tablas =
                {
            "actividades",
            "claves",
            "configuraciones",
            "departamento-actividades",
            "departamentos",
            "empresas",
            "eventos",
            "grupos",
            "horas",
            "ingresos",
            "periodos",
            "roles",
            "sueldos",
            "usuarios"
        };

                var resultados = new List<object>();

                int tablasExitosas = 0;
                int tablasConError = 0;

                // =====================================================
                // CONSULTAR ESQUEMAS DE LAS 14 TABLAS
                // =====================================================

                foreach (string tabla in tablas)
                {
                    try
                    {
                        using JsonDocument respuesta =
                            await _timeApiClient.ObtenerTablaAsync(
                                tabla: tabla,
                                afterId: 0,
                                limit: 1,
                                cancellationToken: HttpContext.RequestAborted);

                        JsonElement root = respuesta.RootElement;

                        bool success = root
                            .GetProperty("success")
                            .GetBoolean();

                        JsonElement data = root.GetProperty("data");

                        var columnas = new List<string>();

                        // =====================================================
                        // EXTRAER SOLO LOS NOMBRES DE LAS COLUMNAS
                        // =====================================================

                        if (data.ValueKind == JsonValueKind.Array &&
                            data.GetArrayLength() > 0)
                        {
                            JsonElement primerRegistro = data[0];

                            columnas = primerRegistro
                                .EnumerateObject()
                                .Select(propiedad => propiedad.Name)
                                .ToList();
                        }

                        // =====================================================
                        // GUARDAR RESULTADO DE LA TABLA
                        // =====================================================

                        resultados.Add(new
                        {
                            tabla,
                            success,
                            columnas,
                            totalColumnas = columnas.Count,
                            registrosMuestra = data.GetArrayLength()
                        });

                        if (success)
                        {
                            tablasExitosas++;
                        }
                        else
                        {
                            tablasConError++;
                        }
                    }
                    catch (OperationCanceledException)
                        when (HttpContext.RequestAborted.IsCancellationRequested)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        resultados.Add(new
                        {
                            tabla,
                            success = false,
                            error = ex.Message
                        });

                        tablasConError++;
                    }
                }

                // =====================================================
                // RESULTADO GENERAL DE LA VERIFICACIÓN
                // =====================================================

                Resultado = JsonSerializer.Serialize(
                    new
                    {
                        success = tablasConError == 0,
                        origen = "TIME Integration API",
                        totalTablas = tablas.Length,
                        tablasExitosas,
                        tablasConError,
                        resultados,
                        mensaje =
                            "Verificación de columnas de TIME finalizada."
                    },
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
            }
            catch (OperationCanceledException)
                when (HttpContext.RequestAborted.IsCancellationRequested)
            {
                // El navegador canceló la solicitud.
            }
            catch (Exception ex)
            {
                Error =
                    "No fue posible consultar TIME: " +
                    ex.Message;
            }
        }

    }
}