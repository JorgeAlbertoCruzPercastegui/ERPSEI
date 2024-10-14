using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.Nomina12
{
	[Serializable]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
	[XmlRoot(Namespace = "http://www.sat.gob.mx/nomina12", IsNullable = false)]
	public partial class NominaReceptor
	{
		[XmlAttribute]
		public string? ClaveEntFed { get; set; }

		[XmlAttribute]
		public string? Curp { get; set; }

		[XmlAttribute]
		public string? NumEmpleado { get; set; }

		[XmlAttribute]
		public byte PeriodicidadPago { get; set; }

		[XmlAttribute]
		public byte TipoContrato { get; set; }

		[XmlAttribute]
		public byte TipoJornada { get; set; }

		[XmlAttribute]
		public byte TipoRegimen { get; set; }
	}
}
