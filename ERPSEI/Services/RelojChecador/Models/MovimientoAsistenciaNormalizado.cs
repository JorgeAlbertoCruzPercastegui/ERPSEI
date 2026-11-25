
using System;

namespace ERPSEI.Services.RelojChecador.Models
{
    public sealed class MovimientoAsistenciaNormalizado
    {
        // Identificador único de la checada
        // Ejemplo: INTRANET:15 o BIOTIME:2034
        public string IdMovimiento { get; init; } = string.Empty;

        // Empleado al que pertenece la checada
        public int EmpleadoId { get; init; }

        // INTRANET o BIOTIME
        public string Fuente { get; init; } = string.Empty;

        // INTRANET_WEB o dispositivo biométrico
        public string Origen { get; init; } = string.Empty;

        // Entrada, SalidaComer, RegresoComer,
        // Salida o SinClasificar
        public string TipoMovimiento { get; init; } = "SinClasificar";

        // Fecha y hora original en UTC
        public DateTime FechaHoraUtc { get; init; }

        // Fecha y hora local de Ciudad de México
        public DateTime FechaHoraMexico { get; init; }

        // Validada, Pendiente, Rechazada
        public string EstadoValidacion { get; init; } = string.Empty;

        // Referencias originales del Reloj Checador
        public long? JornadaId { get; init; }
        public long? ChecadaId { get; init; }

        // Indica si se capturaron coordenadas
        public bool TieneUbicacion { get; init; }

        // Indica si conocemos el significado
        // del movimiento
        public bool EsMovimientoClasificado =>
            TipoMovimiento == "Entrada" ||
            TipoMovimiento == "SalidaComer" ||
            TipoMovimiento == "RegresoComer" ||
            TipoMovimiento == "Salida";
    }
}
