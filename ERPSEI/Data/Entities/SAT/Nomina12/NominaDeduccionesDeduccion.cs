using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.Nomina12
{
	[Serializable]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
	[XmlRoot(Namespace = "http://www.sat.gob.mx/nomina12", IsNullable = false)]
	public partial class NominaDeduccionesDeduccion
	{
		[XmlAttribute]
		public string? Clave { get; set; }

		[XmlAttribute]
		public string? Concepto { get; set; }

		[XmlAttribute]
		public decimal Importe { get; set; }

		[XmlAttribute]
		public byte TipoDeduccion { get; set; }
	}
}
