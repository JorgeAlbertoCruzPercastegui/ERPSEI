using ERPSEI.Data.Entities.Conciliaciones;
using ERPSEI.Data.Entities.SAT.Catalogos;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERPSEI.Data.Entities.Clientes
{
    public class Cliente
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int RegimenFiscalId { get; set; }
        public RegimenFiscal? RegimenFiscal { get; set; }
        public int UsoCFDIId { get; set; }
        public UsoCFDI? UsoCFDI { get; set; }
        public string RazonSocial { get; set; } = string.Empty;
        public string RFC { get; set; } = string.Empty;
        public string? DomicilioFiscal { get; set; }
        public string? ResidenciaFiscal { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public bool Deshabilitado { get; set; }
        public ICollection<Conciliacion> Conciliaciones { get; set; } = [];
        public string? CuentaContable { get; set; }
	}
}
