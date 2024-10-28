using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.Pagos20
{
	[Serializable]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/Pagos20")]
	public partial class PagosPagoImpuestosPTrasladoP
	{
		[XmlIgnore]
		public int Id { get; set; }

		[XmlAttribute]
		public decimal BaseP { get; set; }

		[XmlAttribute]
		public string? ImpuestoP { get; set; }

		[XmlAttribute]
		public string? TipoFactorP { get; set; }

		[XmlAttribute]
		public decimal TasaOCuotaP { get; set; }

		[XmlIgnore]
		public bool TasaOCuotaPSpecified { get; set; }

		[XmlAttribute]
		public decimal ImporteP { get; set; }

		[XmlIgnore]
		public bool ImportePSpecified { get; set; }
	}
}
