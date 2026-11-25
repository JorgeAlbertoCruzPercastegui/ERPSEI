
using ERPSEI.Data;
using ERPSEI.Services.RelojChecador.Models;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Services.RelojChecador;

public sealed class ProveedorMovimientosIntranet
    : IProveedorMovimientosAsistencia
{
    private readonly ApplicationDbContext _db;

    private static readonly TimeZoneInfo ZonaMexico =
        TimeZoneInfo.FindSystemTimeZoneById(
            "America/Mexico_City");

    public string Fuente => "INTRANET";

    public ProveedorMovimientosIntranet(
        ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<MovimientoAsistenciaNormalizado>>
        ObtenerMovimientosAsync(
            int empleadoId,
            DateTime fechaInicioLocal,
            DateTime fechaFinLocal,
            CancellationToken cancellationToken = default)
    {
        if (empleadoId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(empleadoId));

        var inicio = DateTime.SpecifyKind(
            fechaInicioLocal.Date,
            DateTimeKind.Unspecified);

        var finExclusivo = DateTime.SpecifyKind(
            fechaFinLocal.Date.AddDays(1),
            DateTimeKind.Unspecified);

        if (finExclusivo <= inicio)
        {
            throw new ArgumentException(
                "El rango de fechas no es válido.");
        }

        if ((finExclusivo - inicio).TotalDays > 366)
        {
            throw new ArgumentException(
                "El rango máximo de consulta es de 366 días.");
        }

        var inicioUtc = TimeZoneInfo.ConvertTimeToUtc(
            inicio,
            ZonaMexico);

        var finUtc = TimeZoneInfo.ConvertTimeToUtc(
            finExclusivo,
            ZonaMexico);

        // Se filtra por fecha de la checada, no
        // solamente por inicio de la jornada.
        //
        // Esto evita perder movimientos cercanos
        // a medianoche o de jornadas extendidas.

        var checadas = await _db.RC_Checadas
            .AsNoTracking()
            .Where(c =>
                c.Jornada.EmpleadoId == empleadoId &&
                c.FechaHoraUtc >= inicioUtc &&
                c.FechaHoraUtc < finUtc)
            .OrderBy(c => c.FechaHoraUtc)
            .ThenBy(c => c.Id)
            .Select(c => new
            {
                c.Id,
                c.JornadaId,
                c.TipoMovimiento,
                c.Origen,
                c.EstadoValidacion,
                c.FechaHoraUtc,
                c.Latitud,
                c.Longitud
            })
            .ToListAsync(cancellationToken);

        return checadas.Select(c =>
        {
            // SQL Server datetime2 puede devolver
            // DateTimeKind.Unspecified.
            var utc = DateTime.SpecifyKind(
                c.FechaHoraUtc,
                DateTimeKind.Utc);

            var fechaMexico =
                TimeZoneInfo.ConvertTimeFromUtc(
                    utc,
                    ZonaMexico);

            return new MovimientoAsistenciaNormalizado
            {
                IdMovimiento = $"INTRANET:{c.Id}",
                EmpleadoId = empleadoId,
                Fuente = Fuente,
                Origen = c.Origen,
                TipoMovimiento = c.TipoMovimiento,
                FechaHoraUtc = utc,
                FechaHoraMexico = fechaMexico,
                EstadoValidacion = c.EstadoValidacion,
                JornadaId = c.JornadaId,
                ChecadaId = c.Id,
                TieneUbicacion =
                    c.Latitud.HasValue &&
                    c.Longitud.HasValue
            };
        }).ToList();
    }
}
