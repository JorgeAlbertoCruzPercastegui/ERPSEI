using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.ActivosFijos
{
    public class CategoriaActivoFijo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Descripcion { get; set; } = string.Empty;

        public bool Deshabilitado { get; set; } = false;

        // Relación uno-a-muchos con ActivoFijo
        public ICollection<ActivoFijo>? ActivosFijos { get; set; }
    }
}
