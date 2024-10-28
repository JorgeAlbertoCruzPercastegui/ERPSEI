using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.Pagos20
{
	[Serializable]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/Pagos20")]
	public partial class PagosPagoImpuestosPRetencionP
	{
		[XmlIgnore]
		public int Id { get; set; }

		[XmlAttribute]
		public string? ImpuestoP { get; set; }

		[XmlAttribute]
		public decimal ImporteP { get; set; }
	}
}
