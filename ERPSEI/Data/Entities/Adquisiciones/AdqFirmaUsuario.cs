using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.Adquisiciones
{
    [Table("ADQ_FirmasUsuario")]
    public class AdqFirmaUsuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get;
            set;
        }


        [Required]
        [StringLength(450)]
        public string UsuarioId
        {
            get;
            set;
        } = string.Empty;


        [Required]
        [StringLength(150)]
        public string NombreFirma
        {
            get;
            set;
        } = string.Empty;


        [Required]
        [StringLength(30)]
        public string TipoFirma
        {
            get;
            set;
        } = "Dibujada";


        [Required]
        [StringLength(1000)]
        public string RutaArchivo
        {
            get;
            set;
        } = string.Empty;


        [Required]
        [StringLength(128)]
        public string HashArchivo
        {
            get;
            set;
        } = string.Empty;


        public bool EsPredeterminada
        {
            get;
            set;
        }


        public int TotalUsos
        {
            get;
            set;
        }


        public DateTime? FechaUltimoUso
        {
            get;
            set;
        }


        public DateTime FechaCreacion
        {
            get;
            set;
        } = DateTime.Now;


        public DateTime? FechaModificacion
        {
            get;
            set;
        }


        public bool Activa
        {
            get;
            set;
        } = true;


        public bool Eliminado
        {
            get;
            set;
        }


        public ICollection<AdqFirmaAprobacionPresupuestal>
            FirmasAprobaciones
        {
            get;
            set;
        } = new List<AdqFirmaAprobacionPresupuestal>();
    }
}