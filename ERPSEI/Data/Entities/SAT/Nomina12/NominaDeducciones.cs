using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.Nomina12
{
	[Serializable]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
	[XmlRoot(Namespace = "http://www.sat.gob.mx/nomina12", IsNullable = false)]
	public partial class NominaDeducciones
	{
		[XmlIgnore]
		public int Id { get; set; }

		public NominaDeduccionesDeduccion? Deduccion { get; set; }

		[XmlAttribute]
		public decimal TotalImpuestosRetenidos { get; set; }

		[XmlAttribute]
		public decimal TotalOtrasDeducciones { get; set; }
	}
}
