namespace ERPSEI.Data.Entities.TipoContratos
{
    public class TipoRepresentacion
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;

        // Relaciones
        public ICollection<EmpresaContrato> Empresas { get; set; } = new List<EmpresaContrato>();
        public ICollection<ClienteContrato> Clientes { get; set; } = new List<ClienteContrato>();
    }
}