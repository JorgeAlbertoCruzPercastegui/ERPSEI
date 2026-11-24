
using ERPSEI.Data;
using ERPSEI.Data.Entities.RH;
using ERPSEI.Data.Entities.RelojChecador;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Services.RelojChecador;

public sealed class RelojAutorizacionesAusenciasService
{
    public const int TipoHomeOfficeId = 14;

    private readonly ApplicationDbContext _db;

    public RelojAutorizacionesAusenciasService(
        ApplicationDbContext db)
    {
        _db = db;
    }

    private static bool EsSolicitudHomeOffice(Ausencia ausencia)
    {
        return ausencia.TipoAusenciaId == TipoHomeOfficeId &&
               ausencia.Categoria == "SolicitudPermiso";
    }

    // =========================================================
    // AUTORIZACIÓN AL APROBAR EL JEFE DIRECTO
    // =========================================================
    // Este método no ejecuta SaveChangesAsync.
    // El handler de Ausencias controla la transacción.
    // =========================================================

    public async Task AutorizarDesdeJefeAsync(
        Ausencia ausencia,
        string usuarioAutorizadorId,
        CancellationToken ct = default)
    {
        if (!EsSolicitudHomeOffice(ausencia))
            return;

        if (ausencia.EstadoJefeDirecto != "Aprobado")
        {
            throw new InvalidOperationException(
                "La solicitud todavía no está aprobada por el jefe directo.");
        }

        if (ausencia.EstadoTH == "Rechazado")
        {
            throw new InvalidOperationException(
                "No puede autorizarse un permiso HO rechazado por TH.");
        }

        if (!ausencia.EmpleadoId.HasValue ||
            !ausencia.FechaInicio.HasValue ||
            !ausencia.FechaFin.HasValue)
        {
            throw new InvalidOperationException(
                "La solicitud HO no tiene empleado o fechas válidas.");
        }

        if (ausencia.TipoCaptura != "Dias")
        {
            throw new InvalidOperationException(
                "El Home Office debe solicitarse por días completos.");
        }

        var inicio = ausencia.FechaInicio.Value.Date;
        var fin = ausencia.FechaFin.Value.Date;

        if (fin < inicio || (fin - inicio).TotalDays > 365)
        {
            throw new InvalidOperationException(
                "El periodo solicitado de Home Office no es válido.");
        }

        // Obtener al usuario vinculado al empleado
        // que solicitó el permiso.
        var empleado = await _db.Empleados
            .AsNoTracking()
            .Where(x => x.Id == ausencia.EmpleadoId.Value)
            .Select(x => new
            {
                x.UserId
            })
            .FirstOrDefaultAsync(ct);

        if (empleado == null ||
            string.IsNullOrWhiteSpace(empleado.UserId))
        {
            throw new InvalidOperationException(
                "El colaborador no tiene un usuario vinculado en la INTRANET.");
        }

        var userId = empleado.UserId;

        var cuentaValida = await _db.Users
            .AsNoTracking()
            .AnyAsync(x =>
                x.Id == userId &&
                !x.IsBanned &&
                x.EmpleadoId == ausencia.EmpleadoId.Value,
                ct);

        if (!cuentaValida)
        {
            throw new InvalidOperationException(
                "La cuenta del colaborador no está activa o no coincide con su empleado.");
        }

        // Evitar duplicados por solicitud.
        var existente = await _db
            .RC_AutorizacionesModalidades
            .FirstOrDefaultAsync(x =>
                x.AusenciaId == ausencia.Id,
                ct);

        if (existente != null)
        {
            if (existente.UserId != userId)
            {
                throw new InvalidOperationException(
                    "La autorización existente pertenece a otro usuario.");
            }

            if (existente.Estado == "Revocada")
            {
                throw new InvalidOperationException(
                    "La autorización ya fue revocada y requiere revisión administrativa.");
            }

            // La solicitud ya está sincronizada.
            // No generar una segunda autorización.
            return;
        }

        // Impedir periodos superpuestos con permisos
        // aprobados de otra solicitud o manuales.
        var superpuesta = await _db
            .RC_AutorizacionesModalidades
            .AsNoTracking()
            .AnyAsync(x =>
                x.UserId == userId &&
                x.Modalidad == "Remoto" &&
                x.Estado == "Aprobada" &&
                x.FechaInicio < fin.AddDays(1) &&
                x.FechaFin >= inicio,
                ct);

        if (superpuesta)
        {
            throw new InvalidOperationException(
                "El colaborador ya tiene otra autorización Home Office aprobada para esas fechas.");
        }

        var ahoraUtc = DateTime.UtcNow;

        var autorizacion = new RC_AutorizacionModalidad
        {
            AusenciaId = ausencia.Id,
            UserId = userId,
            Modalidad = "Remoto",
            OficinaId = null,
            FechaInicio = inicio,
            FechaFin = fin,
            Estado = "Aprobada",
            Motivo = $"Autorización HO desde Ausencias #{ausencia.Id}",
            AutorizadoPorUserId = usuarioAutorizadorId,
            AutorizadoUtc = ahoraUtc,
            CreadoUtc = ahoraUtc
        };

        _db.RC_AutorizacionesModalidades.Add(
            autorizacion);
    }

    // =========================================================
    // REVOCACIÓN CUANDO TH RECHAZA
    // =========================================================

    public async Task RevocarPorRechazoTHAsync(
        Ausencia ausencia,
        CancellationToken ct = default)
    {
        if (!EsSolicitudHomeOffice(ausencia))
            return;

        var autorizacion = await _db
            .RC_AutorizacionesModalidades
            .FirstOrDefaultAsync(x =>
                x.AusenciaId == ausencia.Id &&
                x.Modalidad == "Remoto",
                ct);

        if (autorizacion == null)
            return;

        if (autorizacion.Estado == "Revocada")
            return;

        autorizacion.Estado = "Revocada";

        // Se conservan las jornadas, checadas
        // y fotografías ya registradas.
    }
}
