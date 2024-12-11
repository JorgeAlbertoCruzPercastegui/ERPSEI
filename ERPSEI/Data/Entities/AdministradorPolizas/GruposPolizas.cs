using ERPSEI.Data.Entities.Usuarios;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERPSEI.Data.Entities.Polizas
{
	public class GruposPolizas
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }
		public string? UsuarioCreadorId { get; set; }
		public AppUser? UsuarioCreador { get; set; }
		public string? UsuarioModificadorId { get; set; }
		public AppUser? UsuarioModificador { get; set; }
		public DateTime? FechaHoraCreacion { get; set; }
		public DateTime? FechaHoraModificacion { get; set; }
		public int NumeroImpresion { get; set; }
		public ICollection<VPolizas>? Polizas { get; set; } = new List<VPolizas>();
		public bool Deshabilitado { get; set; }
	}
}
