
namespace ERPSEI.Data.Entities.TimeIntegration
{
    public class TimeEmpresa
    {
        public long Id { get; set; }
        public string? Nombre { get; set; }
        public byte? Tipo { get; set; }
        public byte? Estatus { get; set; }
        public int? Incremental { get; set; }
        public long? GrupoId { get; set; }
        public string? Clave { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class TimeGrupo
    {
        public long Id { get; set; }
        public string? Nombre { get; set; }
        public byte? Estatus { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
