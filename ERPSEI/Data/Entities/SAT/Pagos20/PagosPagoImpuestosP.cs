using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.Pagos20
{
	[Serializable]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/Pagos20")]
	public partial class PagosPagoImpuestosP
	{
		[XmlIgnore]
		public int Id { get; set; }

		[XmlArrayItem("RetencionP", IsNullable = false)]
		public PagosPagoImpuestosPRetencionP[]? RetencionesP { get; set; }

		[XmlArrayItem("TrasladoP", IsNullable = false)]
		public PagosPagoImpuestosPTrasladoP[]? TrasladosP { get; set; }
	}
}
