using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.Nomina12
{
	[Serializable()]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
	public partial class NominaEmisor
	{
		[XmlIgnore]
		public int Id { get; set; }

		public NominaEmisorEntidadSNCF? EntidadSNCF { get; set; }

		[XmlAttribute]
		public string? Curp { get; set; }

		[XmlAttribute]
		public string? RegistroPatronal { get; set; }

		[XmlAttribute]
		public string? RfcPatronOrigen { get; set; }
	}
}
