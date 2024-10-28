using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.Pagos20
{
	[Serializable]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/Pagos20")]
	public partial class PagosPagoDoctoRelacionadoImpuestosDRTrasladoDR
	{
		[XmlIgnore]
		public int Id { get; set; }

		[XmlAttribute]
		public decimal BaseDR { get; set; }

		[XmlAttribute]
		public string? ImpuestoDR { get; set; }

		[XmlAttribute]
		public string? TipoFactorDR { get; set; }

		[XmlAttribute]
		public decimal TasaOCuotaDR { get; set; }

		[XmlIgnore]
		public bool TasaOCuotaDRSpecified { get; set; }

		[XmlAttribute]
		public decimal ImporteDR { get; set; }

		[XmlIgnore]
		public bool ImporteDRSpecified { get; set; }
	}
}
