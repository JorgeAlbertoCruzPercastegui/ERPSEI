namespace ERPSEI.Data.Entities.TipoContratos
{
    public class HistorialContratoGenerado
    {
        public int Id { get; set; }
        public string UsuarioGenerador { get; set; } = string.Empty;
        public DateTime FechaGeneracion { get; set; } = DateTime.Now;
        public int EmpresaContratoId { get; set; }
        public EmpresaContrato EmpresaContrato { get; set; } = null!;
        public int ClienteContratoId { get; set; }
        public ClienteContrato ClienteContrato { get; set; } = null!;
        public string? NumeroContrato { get; set; }
        public string? ArchivoGenerado { get; set; }
        public bool Activo { get; set; } = true;
    }
}
