using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.Nomina12
{
	[Serializable]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
	public partial class NominaPercepciones
	{
		[XmlIgnore]
		public int Id { get; set; }

		[XmlElement("Percepcion")]
		public NominaPercepcionesPercepcion[]? Percepcion { get; set; }

		public NominaPercepcionesJubilacionPensionRetiro? JubilacionPensionRetiro { get; set; }

		public NominaPercepcionesSeparacionIndemnizacion? SeparacionIndemnizacion { get; set; }

		[XmlAttribute]
		public decimal TotalSueldos { get; set; }

		[XmlIgnore]
		public bool TotalSueldosSpecified { get; set; }

		[XmlAttribute]
		public decimal TotalSeparacionIndemnizacion { get; set; }

		[XmlIgnore]
		public bool TotalSeparacionIndemnizacionSpecified { get; set; }

		[XmlAttribute]
		public decimal TotalJubilacionPensionRetiro { get; set; }

		[XmlIgnore]
		public bool TotalJubilacionPensionRetiroSpecified { get; set; }

		[XmlAttribute]
		public decimal TotalExento { get; set; }

		[XmlAttribute]
		public decimal TotalGravado { get; set; }
	}
}
