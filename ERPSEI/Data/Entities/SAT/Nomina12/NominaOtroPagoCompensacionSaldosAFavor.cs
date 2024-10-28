using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.Nomina12
{
	[Serializable]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
	public partial class NominaOtroPagoCompensacionSaldosAFavor
	{
		[XmlIgnore]
		public int Id { get; set; }

		[XmlAttribute]
		public decimal SaldoAFavor { get; set; }

		[XmlAttribute]
		public short Año { get; set; }

		[XmlAttribute]
		public decimal RemanenteSalFav { get; set; }
	}
}
