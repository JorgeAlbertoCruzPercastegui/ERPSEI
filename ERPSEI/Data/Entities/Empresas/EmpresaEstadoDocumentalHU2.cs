using System.ComponentModel.DataAnnotations;

namespace ERPSEI.Data.Entities.Empresas
{
    // Estado del expediente respecto de la regla documental global vigente.
    public class EmpresaEstadoDocumentalHU2
    {
        [Key]
        public int EmpresaId { get; set; }
        [Required, StringLength(64)]
        public string FirmaRegla { get; set; } = string.Empty;
        public bool Completo { get; set; }
        public DateTime FechaEvaluacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaUltimaNotificacion { get; set; }
        public Empresa Empresa { get; set; } = null!;
    }
}
