using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.Nomina12
{
	[Serializable]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
	public partial class NominaPercepcionesPercepcionHorasExtra
	{
		[XmlIgnore]
		public int Id { get; set; }

		[XmlAttribute]
		public int Dias { get; set; }

		[XmlAttribute]
		public string? TipoHoras { get; set; }

		[XmlAttribute]
		public int HorasExtra { get; set; }

		[XmlAttribute]
		public decimal ImportePagado { get; set; }
	}
}
