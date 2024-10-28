using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.Nomina12
{
	[Serializable]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
	public partial class NominaPercepcionesPercepcionAccionesOTitulos
	{
		[XmlIgnore]
		public int Id { get; set; }

		[XmlAttribute]
		public decimal ValorMercado { get; set; }

		[XmlAttribute]
		public decimal PrecioAlOtorgarse { get; set; }
	}
}
