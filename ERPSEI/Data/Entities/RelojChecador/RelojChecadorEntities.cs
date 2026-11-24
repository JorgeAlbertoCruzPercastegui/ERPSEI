
using System;

namespace ERPSEI.Data.Entities.RelojChecador;

// Coordenadas autorizadas para una oficina existente.
public sealed class RC_OficinaGeocerca
{
    public int Id { get; set; }
    public int OficinaId { get; set; }

    public decimal Latitud { get; set; }
    public decimal Longitud { get; set; }

    public int RadioMetros { get; set; } = 75;
    public int PrecisionMaximaMetros { get; set; } = 30;

    public bool Activa { get; set; } = false;

    public DateTime CreadoUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ActualizadoUtc { get; set; }

    public string? ActualizadoPorUserId { get; set; }
}

// Modalidad autorizada: presencial, remoto,
// híbrido o comisión, con fechas de vigencia.
public sealed class RC_AutorizacionModalidad
{
    public long Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public string Modalidad { get; set; } = "Presencial";

    public int? OficinaId { get; set; }

    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }

    public string Estado { get; set; } = "Pendiente";
    public string? Motivo { get; set; }

    public string? AutorizadoPorUserId { get; set; }
    public DateTime? AutorizadoUtc { get; set; }

    public DateTime CreadoUtc { get; set; } = DateTime.UtcNow;
    public int? AusenciaId { get; set; }
}

// Una jornada por ciclo laboral.
public sealed class RC_Jornada
{
    public long Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public int EmpleadoId { get; set; }

    public string Estado { get; set; } = "Abierta";
    public string Modalidad { get; set; } = "Presencial";

    public DateTime InicioUtc { get; set; }
    public DateTime? FinUtc { get; set; }

    public int MinutosObjetivo { get; set; } = 480;
    public int? MinutosEfectivos { get; set; }

    public bool RequiereRevision { get; set; }
    public string? MotivoRevision { get; set; }

    public DateTime CreadoUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ActualizadoUtc { get; set; }
}

// Movimiento individual del empleado.
public sealed class RC_Checada
{
    public long Id { get; set; }

    public long JornadaId { get; set; }
    public string UserId { get; set; } = string.Empty;

    public string TipoMovimiento { get; set; } = string.Empty;
    public string Origen { get; set; } = string.Empty;

    // Siempre calculada por el servidor.
    public DateTime FechaHoraUtc { get; set; }

    // Ubicación reportada por el cliente.
    public decimal? Latitud { get; set; }
    public decimal? Longitud { get; set; }
    public decimal? PrecisionMetros { get; set; }

    public int? OficinaId { get; set; }
    public decimal? DistanciaOficinaMetros { get; set; }

    public string ResultadoUbicacion { get; set; } = "Pendiente";
    public string EstadoValidacion { get; set; } = "Pendiente";

    // Identificador único para reintentos seguros.
    public Guid IdempotencyKey { get; set; }

    public string? ObservacionCodigo { get; set; }

    public DateTime CreadoUtc { get; set; } = DateTime.UtcNow;

    public RC_Jornada Jornada { get; set; } = null!;
}

// Metadatos de fotografía; el archivo real
// se almacenará cifrado fuera de wwwroot.
public sealed class RC_Evidencia
{
    public long Id { get; set; }

    public long ChecadaId { get; set; }

    public string RutaPrivada { get; set; } = string.Empty;
    public string HashSha256 { get; set; } = string.Empty;
    public string TipoContenido { get; set; } = "image/jpeg";

    public long TamanoBytes { get; set; }

    public DateTime CapturadaUtc { get; set; }
    public DateTime CreadoUtc { get; set; } = DateTime.UtcNow;

    public DateTime? EliminarDespuesUtc { get; set; }

    public RC_Checada Checada { get; set; } = null!;
}

// Auditoría de eventos relevantes.
public sealed class RC_Auditoria
{
    public long Id { get; set; }

    public string? ActorUserId { get; set; }
    public string? AfectadoUserId { get; set; }

    public string Accion { get; set; } = string.Empty;
    public string Resultado { get; set; } = string.Empty;
    public string? CodigoDetalle { get; set; }

    public long? JornadaId { get; set; }
    public long? ChecadaId { get; set; }

    public DateTime FechaUtc { get; set; } = DateTime.UtcNow;
}
