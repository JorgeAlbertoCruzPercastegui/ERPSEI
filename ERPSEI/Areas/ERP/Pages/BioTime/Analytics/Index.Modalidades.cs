
using ERPSEI.Data.Entities.RelojChecador;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace ERPSEI.Areas.ERP.Pages.BioTime.Analytics;

public sealed partial class IndexModel
{
    public sealed class AutorizarRemotoInput
    {
        public string UserId { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Motivo { get; set; } = string.Empty;
    }

    private bool PuedeAdministrarModalidades() =>
    User.IsInRole("Master") ||
    User.IsInRole("Administrador") ||
    User.IsInRole("Administrador TH");

    private IActionResult SinPermisoModalidades() =>
        new JsonResult(new
        {
            success = false,
            message = "No tienes autorización para administrar modalidades laborales."
        })
        {
            StatusCode = 403
        };

    private static DateTime FechaMexicoHoy()
    {
        var zona = TimeZoneInfo.FindSystemTimeZoneById(
            "America/Mexico_City");

        return TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow, zona).Date;
    }

    // Buscar usuarios existentes sin entregar todo
    // el directorio de empleados al navegador.
    public async Task<IActionResult> OnGetUsuariosRelojAsync(
        string? buscar,
        CancellationToken ct)
    {
        if (!PuedeAdministrarModalidades())
            return SinPermisoModalidades();

        buscar = buscar?.Trim();

        if (string.IsNullOrWhiteSpace(buscar) ||
            buscar.Length < 2 ||
            buscar.Length > 80)
        {
            return BadRequest(new
            {
                success = false,
                message = "Escribe entre 2 y 80 caracteres para buscar."
            });
        }

        var usuarios = await _db.Users
            .AsNoTracking()
            .Where(u =>
                (u.UserName != null &&
                 u.UserName.Contains(buscar)) ||
                (u.Email != null &&
                 u.Email.Contains(buscar)))
            .OrderBy(u => u.UserName)
            .Take(20)
            .Select(u => new
            {
                id = u.Id,
                usuario = u.UserName,
                correo = u.Email,
                empleadoId = u.EmpleadoId
            })
            .ToListAsync(ct);

        return new JsonResult(new
        {
            success = true,
            usuarios
        });
    }

    // Mostrar las autorizaciones más recientes.
    public async Task<IActionResult> OnGetModalidadesRelojAsync(
        CancellationToken ct)
    {
        if (!PuedeAdministrarModalidades())
            return SinPermisoModalidades();

        var autorizaciones = await (
            from a in _db.RC_AutorizacionesModalidades
                .AsNoTracking()
            join usuario in _db.Users.AsNoTracking()
                on a.UserId equals usuario.Id
            join aprobador in _db.Users.AsNoTracking()
                on a.AutorizadoPorUserId equals aprobador.Id
                into aprobadores
            from aprobador in aprobadores.DefaultIfEmpty()
            orderby a.Id descending
            select new
            {
                a.Id,
                usuario = usuario.UserName,
                a.Modalidad,
                a.Estado,
                a.FechaInicio,
                a.FechaFin,
                a.Motivo,
                autorizadoPor = aprobador == null
                    ? null
                    : aprobador.UserName
            })
            .Take(100)
            .ToListAsync(ct);

        return new JsonResult(new
        {
            success = true,
            autorizaciones
        });
    }

    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OnPostAutorizarRemotoAsync(
        [FromForm] AutorizarRemotoInput input,
        CancellationToken ct)
    {
        if (!PuedeAdministrarModalidades())
            return SinPermisoModalidades();

        var actor = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(actor))
            return Unauthorized();

        input.UserId = input.UserId?.Trim() ?? "";
        input.Motivo = input.Motivo?.Trim() ?? "";

        var inicio = input.FechaInicio.Date;
        var fin = input.FechaFin.Date;
        var hoy = FechaMexicoHoy();

        if (string.IsNullOrWhiteSpace(input.UserId) ||
            input.UserId.Length > 450 ||
            input.UserId == actor)
        {
            return BadRequest(new
            {
                success = false,
                message = "Selecciona otro colaborador. No se permite autorizarse a uno mismo."
            });
        }

        if (inicio < hoy ||
            fin < inicio ||
            (fin - inicio).Days > 365)
        {
            return BadRequest(new
            {
                success = false,
                message = "Selecciona fechas desde hoy y una vigencia máxima de 366 días naturales."
            });
        }

        if (input.Motivo.Length < 5 ||
            input.Motivo.Length > 500)
        {
            return BadRequest(new
            {
                success = false,
                message = "El motivo debe contener de 5 a 500 caracteres."
            });
        }

        await using var tx =
            await _db.Database.BeginTransactionAsync(
                IsolationLevel.Serializable, ct);

        var usuario = await _db.Users
            .AsNoTracking()
            .Where(u => u.Id == input.UserId)
            .Select(u => new
            {
                u.Id,
                u.EmpleadoId,
                u.IsBanned
            })
            .FirstOrDefaultAsync(ct);

        if (usuario == null ||
            !usuario.EmpleadoId.HasValue ||
            usuario.IsBanned)
        {
            return BadRequest(new
            {
                success = false,
                message = "El usuario no tiene un empleado habilitado y vinculado."
            });
        }

        // Evitar autorizaciones remotas superpuestas.
        var solapada = await _db
            .RC_AutorizacionesModalidades
            .AnyAsync(a =>
                a.UserId == input.UserId &&
                a.Modalidad == "Remoto" &&
                a.Estado == "Aprobada" &&
                a.FechaInicio < fin.AddDays(1) &&
                a.FechaFin >= inicio,
                ct);

        if (solapada)
        {
            return new JsonResult(new
            {
                success = false,
                message = "Ya existe una autorización remota aprobada en ese periodo."
            })
            {
                StatusCode = 409
            };
        }

        var ahora = DateTime.UtcNow;

        var registro = new RC_AutorizacionModalidad
        {
            UserId = input.UserId,
            Modalidad = "Remoto",
            OficinaId = null,
            FechaInicio = inicio,
            FechaFin = fin,
            Estado = "Aprobada",
            Motivo = input.Motivo,
            AutorizadoPorUserId = actor,
            AutorizadoUtc = ahora,
            CreadoUtc = ahora
        };

        _db.RC_AutorizacionesModalidades.Add(registro);

        _db.RC_Auditoria.Add(new RC_Auditoria
        {
            ActorUserId = actor,
            AfectadoUserId = input.UserId,
            Accion = "MODALIDAD_REMOTA_AUTORIZAR",
            Resultado = "CORRECTO",
            CodigoDetalle = $"INICIO:{inicio:yyyy-MM-dd};FIN:{fin:yyyy-MM-dd}",
            FechaUtc = ahora
        });

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return new JsonResult(new
        {
            success = true,
            message = "Home Office autorizado correctamente.",
            autorizacionId = registro.Id
        });
    }

    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OnPostRevocarRemotoAsync(
        [FromForm] long autorizacionId,
        CancellationToken ct)
    {
        if (!PuedeAdministrarModalidades())
            return SinPermisoModalidades();

        var actor = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(actor))
            return Unauthorized();

        if (autorizacionId <= 0)
            return BadRequest(new
            {
                success = false,
                message = "Autorización inválida."
            });

        await using var tx =
            await _db.Database.BeginTransactionAsync(
                IsolationLevel.Serializable, ct);

        var registro = await _db
            .RC_AutorizacionesModalidades
            .FirstOrDefaultAsync(a =>
                a.Id == autorizacionId &&
                a.Modalidad == "Remoto",
                ct);

        if (registro == null)
            return NotFound(new
            {
                success = false,
                message = "Autorización no encontrada."
            });

        if (registro.Estado != "Aprobada")
            return BadRequest(new
            {
                success = false,
                message = "La autorización ya no está aprobada."
            });

        if (registro.UserId == actor)
            return BadRequest(new
            {
                success = false,
                message = "No puedes revocar tu propia autorización."
            });

        registro.Estado = "Revocada";

        _db.RC_Auditoria.Add(new RC_Auditoria
        {
            ActorUserId = actor,
            AfectadoUserId = registro.UserId,
            Accion = "MODALIDAD_REMOTA_REVOCAR",
            Resultado = "CORRECTO",
            CodigoDetalle = $"AUTORIZACION:{registro.Id}",
            FechaUtc = DateTime.UtcNow
        });

        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return new JsonResult(new
        {
            success = true,
            message = "Autorización revocada correctamente."
        });
    }
}
