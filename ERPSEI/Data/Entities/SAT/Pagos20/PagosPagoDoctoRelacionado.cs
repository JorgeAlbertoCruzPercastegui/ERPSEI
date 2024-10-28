using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.Pagos20
{
	[Serializable]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/Pagos20")]
	public partial class PagosPagoDoctoRelacionado
	{
		[XmlIgnore]
		public int Id { get; set; }

		public PagosPagoDoctoRelacionadoImpuestosDR? ImpuestosDR { get; set; }

		[XmlAttribute]
		public string? IdDocumento { get; set; }

		[XmlAttribute]
		public string? Serie { get; set; }

		[XmlAttribute]
		public string? Folio { get; set; }

		[XmlAttribute]
		public string? MonedaDR { get; set; }

		[XmlAttribute]
		public decimal EquivalenciaDR { get; set; }

		[XmlIgnore]
		public bool EquivalenciaDRSpecified { get; set; }

		[XmlAttribute(DataType = "integer")]
		public string? NumParcialidad { get; set; }

		[XmlAttribute]
		public decimal ImpSaldoAnt { get; set; }

		[XmlAttribute]
		public decimal ImpPagado { get; set; }

		[XmlAttribute]
		public decimal ImpSaldoInsoluto { get; set; }

		[XmlAttribute]
		public string? ObjetoImpDR { get; set; }
	}
}
