using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.Nomina12
{
	[Serializable]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
	public partial class NominaDeducciones
	{
		[XmlIgnore]
		public int Id { get; set; }

		[XmlElement("Deduccion")]
		public NominaDeduccionesDeduccion[]? Deduccion { get; set; }

		[XmlAttribute]
		public decimal TotalOtrasDeducciones { get; set; }

		[XmlIgnore]
		public bool TotalOtrasDeduccionesSpecified { get; set; }

		[XmlAttribute]
		public decimal TotalImpuestosRetenidos { get; set; }

		[XmlIgnore]
		public bool TotalImpuestosRetenidosSpecified { get; set; }
	}
}
