using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.Nomina12
{
	[Serializable]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
	public partial class NominaOtroPagoSubsidioAlEmpleo
	{
		[XmlIgnore]
		public int Id { get; set; }

		[XmlAttribute]
		public decimal SubsidioCausado { get; set; }
	}
}
