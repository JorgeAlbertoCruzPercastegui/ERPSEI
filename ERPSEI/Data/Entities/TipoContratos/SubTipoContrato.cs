namespace ERPSEI.Data.Entities.TipoContratos
{
    public class SubTipoContrato
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Deshabilitado { get; set; }

        // Relación con TipoContrato
        public int TipoContratoId { get; set; }
        public TipoContrato TipoContrato { get; set; }
    }
}
