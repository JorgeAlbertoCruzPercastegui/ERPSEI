using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.Nomina12
{
	[Serializable]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
	public partial class NominaOtroPago
	{
		[XmlIgnore]
		public int Id { get; set; }

		public NominaOtroPagoSubsidioAlEmpleo? SubsidioAlEmpleo { get; set; }

		public NominaOtroPagoCompensacionSaldosAFavor? CompensacionSaldosAFavor { get; set; }

		[XmlAttribute]
		public string? TipoOtroPago { get; set; }

		[XmlAttribute]
		public string? Clave { get; set; }

		[XmlAttribute]
		public string? Concepto { get; set; }

		[XmlAttribute]
		public decimal Importe { get; set; }
	}
}
