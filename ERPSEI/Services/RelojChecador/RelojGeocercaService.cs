
using ERPSEI.Data;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Services.RelojChecador;

public sealed record ResultadoGeocerca(
    bool Autorizado,
    string Codigo,
    string Mensaje,
    double? DistanciaMetros,
    double? PrecisionMetros,
    int? RadioPermitidoMetros
);

public sealed class RelojGeocercaService
{
    private readonly ApplicationDbContext _db;

    public RelojGeocercaService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ResultadoGeocerca> ValidarAsync(
        int oficinaId,
        double latitud,
        double longitud,
        double precisionMetros,
        CancellationToken ct = default)
    {
        // Validar coordenadas recibidas.
        if (!double.IsFinite(latitud) ||
            !double.IsFinite(longitud) ||
            latitud < -90 || latitud > 90 ||
            longitud < -180 || longitud > 180)
        {
            return Rechazar(
                "COORDENADAS_INVALIDAS",
                "La ubicación recibida no es válida.");
        }

        if (!double.IsFinite(precisionMetros) ||
            precisionMetros <= 0)
        {
            return Rechazar(
                "PRECISION_INVALIDA",
                "No fue posible determinar la precisión GPS.");
        }

        // Obtener únicamente la geocerca autorizada.
        var oficina = await _db.RC_OficinasGeocercas
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.OficinaId == oficinaId && x.Activa,
                ct);

        if (oficina == null)
        {
            return Rechazar(
                "GEOCERCA_NO_CONFIGURADA",
                "La oficina no tiene una geocerca activa.");
        }

        if (precisionMetros > oficina.PrecisionMaximaMetros)
        {
            return new ResultadoGeocerca(
                false,
                "PRECISION_INSUFICIENTE",
                "La ubicación es imprecisa. Acércate a una zona con mejor señal GPS e inténtalo nuevamente.",
                null,
                precisionMetros,
                oficina.RadioMetros);
        }

        double distancia = CalcularDistanciaMetros(
            latitud,
            longitud,
            (double)oficina.Latitud,
            (double)oficina.Longitud);

        // Criterio conservador:
        // distancia + incertidumbre debe quedar
        // dentro del radio permitido.
        //
        // La precisión del navegador es una estimación,
        // no una garantía de ubicación auténtica.
        bool dentro =
            distancia + precisionMetros <= oficina.RadioMetros;

        return new ResultadoGeocerca(
            dentro,
            dentro ? "UBICACION_VALIDA" : "UBICACION_NO_CONFIRMADA",
            dentro
                ? "Ubicación dentro del perímetro autorizado."
                : "No se pudo confirmar que estés dentro del perímetro de la oficina.",
            Math.Round(distancia, 2),
            precisionMetros,
            oficina.RadioMetros);
    }

    private static ResultadoGeocerca Rechazar(
        string codigo,
        string mensaje)
    {
        return new ResultadoGeocerca(
            false,
            codigo,
            mensaje,
            null,
            null,
            null);
    }

    private static double CalcularDistanciaMetros(
        double lat1,
        double lon1,
        double lat2,
        double lon2)
    {
        const double radioTierra = 6371000;

        double radLat1 = lat1 * Math.PI / 180;
        double radLat2 = lat2 * Math.PI / 180;

        double diferenciaLatitud =
            (lat2 - lat1) * Math.PI / 180;

        double diferenciaLongitud =
            (lon2 - lon1) * Math.PI / 180;

        double a =
            Math.Pow(Math.Sin(diferenciaLatitud / 2), 2) +
            Math.Cos(radLat1) *
            Math.Cos(radLat2) *
            Math.Pow(Math.Sin(diferenciaLongitud / 2), 2);

        double c = 2 * Math.Atan2(
            Math.Sqrt(a),
            Math.Sqrt(1 - a));

        return radioTierra * c;
    }
}
