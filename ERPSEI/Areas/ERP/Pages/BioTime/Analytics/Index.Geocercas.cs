
using ERPSEI.Data.Entities.RelojChecador;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ERPSEI.Areas.ERP.Pages.BioTime.Analytics;

public sealed partial class IndexModel
{
    public sealed class GuardarGeocercaInput
    {
        public int OficinaId { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }
        public int RadioMetros { get; set; } = 75;
        public int PrecisionMaximaMetros { get; set; } = 30;
        public bool Activa { get; set; }
        public bool ConfirmarCoordenadas { get; set; }
    }

    // GET /ERP/BioTime/Analytics?handler=Geocercas
    public async Task<IActionResult> OnGetGeocercasAsync(
        CancellationToken ct)
    {
        var oficinas = await _db.Oficinas
            .AsNoTracking()
            .OrderBy(x => x.Nombre)
            .Select(x => new
            {
                x.Id,
                x.Nombre
            })
            .ToListAsync(ct);

        var geocercas = await _db.RC_OficinasGeocercas
            .AsNoTracking()
            .Select(x => new
            {
                x.OficinaId,
                x.Latitud,
                x.Longitud,
                x.RadioMetros,
                x.PrecisionMaximaMetros,
                x.Activa,
                x.ActualizadoUtc
            })
            .ToListAsync(ct);

        var configuradas = geocercas.ToDictionary(
            x => x.OficinaId);

        var registros = oficinas.Select(o =>
        {
            configuradas.TryGetValue(o.Id, out var g);

            return new
            {
                id = o.Id,
                nombre = o.Nombre,
                configurada = g != null,
                latitud = g?.Latitud,
                longitud = g?.Longitud,
                radioMetros = g?.RadioMetros ?? 75,
                precisionMaximaMetros =
                    g?.PrecisionMaximaMetros ?? 30,
                activa = g?.Activa ?? false,
                actualizadoUtc = g?.ActualizadoUtc
            };
        }).ToList();

        return new JsonResult(new
        {
            success = true,
            total = registros.Count,
            configuradas = registros.Count(x => x.configurada),
            activas = registros.Count(x => x.activa),
            registros
        });
    }

    // POST /ERP/BioTime/Analytics?handler=GuardarGeocerca
    // Razor Pages valida el token antifalsificación.
    public async Task<IActionResult> OnPostGuardarGeocercaAsync(
        [FromForm] GuardarGeocercaInput input,
        CancellationToken ct)
    {
        var actorId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(actorId))
        {
            return new JsonResult(new
            {
                success = false,
                message = "La sesión no es válida."
            })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }

        if (input.OficinaId <= 0 ||
            !input.Latitud.HasValue ||
            !input.Longitud.HasValue)
        {
            return BadRequest(new
            {
                success = false,
                message = "Selecciona una oficina y sus coordenadas."
            });
        }

        if (input.Latitud.Value < -90 ||
            input.Latitud.Value > 90 ||
            input.Longitud.Value < -180 ||
            input.Longitud.Value > 180)
        {
            return BadRequest(new
            {
                success = false,
                message = "Las coordenadas están fuera de rango."
            });
        }

        if (input.RadioMetros < 10 ||
            input.RadioMetros > 1000 ||
            input.PrecisionMaximaMetros < 5 ||
            input.PrecisionMaximaMetros > 500)
        {
            return BadRequest(new
            {
                success = false,
                message = "El radio o la precisión están fuera de rango."
            });
        }

        if (input.Activa && !input.ConfirmarCoordenadas)
        {
            return BadRequest(new
            {
                success = false,
                message = "Confirma las coordenadas antes de activar la geocerca."
            });
        }

        var existeOficina = await _db.Oficinas
            .AsNoTracking()
            .AnyAsync(x => x.Id == input.OficinaId, ct);

        if (!existeOficina)
        {
            return NotFound(new
            {
                success = false,
                message = "La oficina seleccionada no existe."
            });
        }

        var ahora = DateTime.UtcNow;

        var geocerca = await _db.RC_OficinasGeocercas
            .SingleOrDefaultAsync(
                x => x.OficinaId == input.OficinaId,
                ct);

        var esNueva = geocerca == null;

        if (geocerca == null)
        {
            geocerca = new RC_OficinaGeocerca
            {
                OficinaId = input.OficinaId,
                CreadoUtc = ahora
            };

            _db.RC_OficinasGeocercas.Add(geocerca);
        }

        geocerca.Latitud = input.Latitud.Value;
        geocerca.Longitud = input.Longitud.Value;
        geocerca.RadioMetros = input.RadioMetros;
        geocerca.PrecisionMaximaMetros =
            input.PrecisionMaximaMetros;
        geocerca.Activa = input.Activa;
        geocerca.ActualizadoUtc = ahora;
        geocerca.ActualizadoPorUserId = actorId;

        _db.RC_Auditoria.Add(new RC_Auditoria
        {
            ActorUserId = actorId,
            AfectadoUserId = actorId,
            Accion = esNueva
                ? "GEOCERCA_CREAR"
                : "GEOCERCA_ACTUALIZAR",
            Resultado = "CORRECTO",
            CodigoDetalle =
                $"OFICINA:{input.OficinaId};ACTIVA:{input.Activa}",
            FechaUtc = ahora
        });

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            return StatusCode(409, new
            {
                success = false,
                message = "No fue posible guardar. Actualiza la lista e inténtalo nuevamente."
            });
        }

        return new JsonResult(new
        {
            success = true,
            message = input.Activa
                ? "Geocerca guardada y activada."
                : "Geocerca guardada como inactiva.",
            oficinaId = input.OficinaId,
            activa = input.Activa
        });
    }
}
