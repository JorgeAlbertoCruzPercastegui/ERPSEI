namespace ERPSEI.Data.Entities.TimeIntegration
{
    public class TimeDepartamentoActividad
    {
        public long Id { get; set; }

        public string? Nombre { get; set; }

        public long? DepartamentoId { get; set; }

        public string? Descripcion { get; set; }

        public byte? Estatus { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}