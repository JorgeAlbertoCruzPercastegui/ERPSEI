using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.Nomina12
{
	[Serializable]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
	public partial class NominaPercepcionesSeparacionIndemnizacion
	{
		[XmlIgnore]
		public int Id { get; set; }

		[XmlAttribute]
		public decimal TotalPagado { get; set; }

		[XmlAttribute]
		public int NumAñosServicio { get; set; }

		[XmlAttribute]
		public decimal UltimoSueldoMensOrd { get; set; }

		[XmlAttribute]
		public decimal IngresoAcumulable { get; set; }

		[XmlAttribute]
		public decimal IngresoNoAcumulable { get; set; }
	}
}
