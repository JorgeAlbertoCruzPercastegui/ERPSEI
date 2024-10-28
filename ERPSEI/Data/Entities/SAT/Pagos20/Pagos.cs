using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.Pagos20
{
	[Serializable]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/Pagos20")]
	[XmlRoot(Namespace = "http://www.sat.gob.mx/Pagos20", IsNullable = false)]
	public partial class Pagos
	{
		[XmlIgnore]
		public int Id { get; set; }

		public PagosTotales? Totales { get; set; }

		[XmlElement("Pago")]
		public PagosPago[]? Pago { get; set; }

		[XmlAttribute]
		public string Version { get; set; } = "2.0";
	}
}
