using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.Nomina12
{
	[Serializable]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
	[XmlRoot(Namespace = "http://www.sat.gob.mx/nomina12", IsNullable = false)]
	public partial class NominaPercepciones
	{

		public NominaPercepcionesPercepcion? Percepcion { get; set; }

		[XmlAttribute]
		public decimal TotalExento { get; set; }

		[XmlAttribute]
		public decimal TotalGravado { get; set; }

		[XmlAttribute]
		public decimal TotalSueldos { get; set; }
	}
}
