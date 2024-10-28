using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.Nomina12
{
	[Serializable]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
	public partial class NominaIncapacidad
	{
		[XmlIgnore]
		public int Id { get; set; }

		[XmlAttribute]
		public int DiasIncapacidad { get; set; }

		[XmlAttribute]
		public string? TipoIncapacidad { get; set; }

		[XmlAttribute]
		public decimal ImporteMonetario { get; set; }

		[XmlIgnore]
		public bool ImporteMonetarioSpecified { get; set; }
	}
}
