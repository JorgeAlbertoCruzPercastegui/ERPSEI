namespace ERPSEI.Data.Entities.Intranet
{
    public class CorreoDominio
    {
        public int Id { get; set; }

        public string? Correo { get; set; }
        public string? Dominio { get; set; }
        public string? Descripcion { get; set; }
        public string? Responsable { get; set; }
        public string? Observaciones { get; set; }

        public bool Deshabilitado { get; set; }

        public DateTime FechaCreacion { get; set; }
        public string? UsuarioCreador { get; set; }

        public DateTime? FechaModificacion { get; set; }
        public string? UsuarioModificador { get; set; }
    }
}