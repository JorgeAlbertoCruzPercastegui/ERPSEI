using ERPSEI.Data.Entities.Usuarios;
using System.ComponentModel.DataAnnotations.Schema;
namespace ERPSEI.Data.Entities.Polizas
{
	public class GrupoPoliza
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
		public ICollection<VPoliza>? Polizas { get; set; } = new List<VPoliza>();
		public bool Deshabilitado { get; set; }
	}
}
