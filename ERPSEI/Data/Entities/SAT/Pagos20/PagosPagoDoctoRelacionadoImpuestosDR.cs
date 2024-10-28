using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.Pagos20
{
	[Serializable]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/Pagos20")]
	public partial class PagosPagoDoctoRelacionadoImpuestosDR
	{
		[XmlIgnore]
		public int Id { get; set; }

		[XmlArrayItem("RetencionDR", IsNullable = false)]
		public PagosPagoDoctoRelacionadoImpuestosDRRetencionDR[]? RetencionesDR { get; set; }

		[XmlArrayItem("TrasladoDR", IsNullable = false)]
		public PagosPagoDoctoRelacionadoImpuestosDRTrasladoDR[]? TrasladosDR { get; set; }
	}
}