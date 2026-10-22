
namespace ERPSEI.Data.Entities.TimeIntegration
{
    public class TimeUsuario
    {
        public long Id { get; set; }
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public long? DepartamentoId { get; set; }
        public byte? Estatus { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class TimeDepartamento
    {
        public long Id { get; set; }
        public string? Nombre { get; set; }
        public string? Clave { get; set; }
        public byte? Estatus { get; set; }
        public int? Incremental { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class TimeClave
    {
        public long Id { get; set; }
        public long? EmpresaId { get; set; }
        public long? DepartamentoId { get; set; }
        public string? Proyecto { get; set; }
        public string? Clave { get; set; }
        public int? Incremental { get; set; }
        public byte? Tipo { get; set; }
        public byte? Estatus { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class TimeActividad
    {
        public long Id { get; set; }
        public long? PeriodoId { get; set; }
        public long? UsuarioId { get; set; }
        public long? ClaveId { get; set; }
        public string? Actividad { get; set; }
        public long? DepartamentoActividadId { get; set; }
        public int? Orden { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class TimeHora
    {
        public long Id { get; set; }
        public DateTime? Fecha { get; set; }
        public double? Horas { get; set; }
        public long? ActividadId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class TimeEstadoSincronizacion
    {
        public string Tabla { get; set; } = string.Empty;
        public long UltimoId { get; set; }
        public long? UltimoUpperId { get; set; }
        public DateTime? UltimaEjecucion { get; set; }
        public string? Estado { get; set; }
        public long RegistrosProcesados { get; set; }
        public string? Error { get; set; }
    }
}
