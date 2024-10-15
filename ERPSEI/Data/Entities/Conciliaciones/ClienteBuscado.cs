using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.Conciliaciones
{
    [NotMapped]
    public class ClienteBuscado
    {
        public int Id { get; set; }
        public string? NombreCliente { get; set; } = string.Empty;
    }
}
