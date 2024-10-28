using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.Pagos20
{
	[Serializable]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/Pagos20")]
	public partial class PagosTotales
	{
		[XmlIgnore]
		public int Id { get; set; }

		[XmlAttribute]
		public decimal TotalRetencionesIVA { get; set; }

		[XmlIgnore]
		public bool TotalRetencionesIVASpecified { get; set; }

		[XmlAttribute]
		public decimal TotalRetencionesISR { get; set; }

		[XmlIgnore]
		public bool TotalRetencionesISRSpecified { get; set; }

		[XmlAttribute]
		public decimal TotalRetencionesIEPS { get; set; }

		[XmlIgnore]
		public bool TotalRetencionesIEPSSpecified { get; set; }

		[XmlAttribute]
		public decimal TotalTrasladosBaseIVA16 { get; set; }

		[XmlIgnore]
		public bool TotalTrasladosBaseIVA16Specified { get; set; }

		[XmlAttribute]
		public decimal TotalTrasladosImpuestoIVA16 { get; set; }

		[XmlIgnore]
		public bool TotalTrasladosImpuestoIVA16Specified { get; set; }

		[XmlAttribute]
		public decimal TotalTrasladosBaseIVA8 { get; set; }

		[XmlIgnore]
		public bool TotalTrasladosBaseIVA8Specified { get; set; }

		[XmlAttribute]
		public decimal TotalTrasladosImpuestoIVA8 { get; set; }

		[XmlIgnore]
		public bool TotalTrasladosImpuestoIVA8Specified { get; set; }

		[XmlAttribute]
		public decimal TotalTrasladosBaseIVA0 { get; set; }

		[XmlIgnore]
		public bool TotalTrasladosBaseIVA0Specified { get; set; }

		[XmlAttribute]
		public decimal TotalTrasladosImpuestoIVA0 { get; set; }

		[XmlIgnore]
		public bool TotalTrasladosImpuestoIVA0Specified { get; set; }

		[XmlAttribute]
		public decimal TotalTrasladosBaseIVAExento { get; set; }

		[XmlIgnore]
		public bool TotalTrasladosBaseIVAExentoSpecified { get; set; }

		[XmlAttribute]
		public decimal MontoTotalPagos { get; set; }
	}
}
