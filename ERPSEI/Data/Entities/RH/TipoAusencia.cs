namespace ERPSEI.Data.Entities.RH
{
    public class TipoAusencia
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
        public bool ManejaHoras { get; set; } = false;
        public bool ManejaDias { get; set; } = true;
        public int Orden { get; set; }
    }
}