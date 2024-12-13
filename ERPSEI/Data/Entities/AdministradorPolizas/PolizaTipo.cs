using System.ComponentModel.DataAnnotations.Schema;
namespace ERPSEI.Data.Entities.Polizas
{
	public class PolizaTipo
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }
		public string Descripcion { get; set; } = string.Empty;
		public bool Deshabilitado { get; set; }

		public ICollection<VPoliza> Polizas { get; set; } = [];
	}
}
