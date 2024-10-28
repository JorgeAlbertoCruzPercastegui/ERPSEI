using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.Pagos20
{
	[Serializable]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/Pagos20")]
	public partial class PagosPago
	{
		[XmlIgnore]
		public int Id { get; set; }

		[XmlElement("DoctoRelacionado")]
		public PagosPagoDoctoRelacionado[]? DoctoRelacionado { get; set; }

		public PagosPagoImpuestosP? ImpuestosP { get; set; }

		[XmlAttribute]
		public DateTime FechaPago { get; set; }

		[XmlAttribute]
		public string? FormaDePagoP { get; set; }

		[XmlAttribute]
		public string? MonedaP { get; set; }

		[XmlAttribute]
		public decimal TipoCambioP { get; set; }

		[XmlIgnore]
		public bool TipoCambioPSpecified { get; set; }

		[XmlAttribute]
		public decimal Monto { get; set; }

		[XmlAttribute]
		public string? NumOperacion { get; set; }

		[XmlAttribute]
		public string? RfcEmisorCtaOrd { get; set; }

		[XmlAttribute]
		public string? NomBancoOrdExt { get; set; }

		[XmlAttribute]
		public string? CtaOrdenante { get; set; }

		[XmlAttribute]
		public string? RfcEmisorCtaBen { get; set; }

		[XmlAttribute]
		public string? CtaBeneficiario { get; set; }

		[XmlAttribute]
		public string? TipoCadPago { get; set; }

		[XmlIgnore]
		public bool TipoCadPagoSpecified { get; set; }

		[XmlAttribute(DataType = "base64Binary")]
		public byte[]? CertPago { get; set; }

		[XmlAttribute]
		public string? CadPago { get; set; }

		[XmlAttribute(DataType = "base64Binary")]
		public byte[]? SelloPago { get; set; }
	}
}
