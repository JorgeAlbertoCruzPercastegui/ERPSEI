using ERPSEI.Data.Entities.Empleados;

namespace ERPSEI.Data.Entities.TipoContratos
{
    public class TipoContrato
    {
       public int Id { get; set; }
       public string? Nombre { get; set; }
       public string? Descripcion { get; set; }
        public bool Deshabilitado { get; set; } = false;
    }
}
