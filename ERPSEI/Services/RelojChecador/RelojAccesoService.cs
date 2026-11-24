
using ERPSEI.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ERPSEI.Services.RelojChecador;

public sealed class RelojAccesoService
{
    private readonly ApplicationDbContext _db;

    public RelojAccesoService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<bool> TieneAccesoAsync(
        ClaimsPrincipal usuario,
        CancellationToken ct = default)
    {
        if (usuario.Identity?.IsAuthenticated != true)
            return false;

        var userId = usuario.FindFirstValue(
            ClaimTypes.NameIdentifier);

        return await TieneAccesoAsync(userId, ct);
    }

    public async Task<bool> TieneAccesoAsync(
        string? userId,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return false;

        var usuarioValido = await _db.Users
            .AsNoTracking()
            .AnyAsync(x =>
                x.Id == userId &&
                !x.IsBanned &&
                x.EmpleadoId != null,
                ct);

        if (!usuarioValido)
            return false;

        var zona = TimeZoneInfo.FindSystemTimeZoneById(
            "America/Mexico_City");

        var fechaHoy = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            zona).Date;

        var fechaManana = fechaHoy.AddDays(1);

        return await _db.RC_AutorizacionesModalidades
            .AsNoTracking()
            .AnyAsync(x =>
                x.UserId == userId &&
                x.Estado == "Aprobada" &&
                x.Modalidad == "Remoto" &&
                x.FechaInicio < fechaManana &&
                x.FechaFin >= fechaHoy,
                ct);
    }
}
