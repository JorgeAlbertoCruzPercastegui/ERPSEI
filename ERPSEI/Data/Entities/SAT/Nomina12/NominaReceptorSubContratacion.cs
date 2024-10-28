using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.Nomina12
{
	[Serializable]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
	public partial class NominaReceptorSubContratacion
	{
		[XmlIgnore]
		public int Id { get; set; }

		[XmlAttribute]
		public string? RfcLabora { get; set; }

		[XmlAttribute]
		public decimal PorcentajeTiempo { get; set; }
	}
}
