using System.Xml.Serialization;
namespace ERPSEI.Data.Entities.SAT.Nomina12
{
	[Serializable]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(AnonymousType = true)]
	[XmlRoot(Namespace = "http://www.sat.gob.mx/nomina12", IsNullable = false)]
	public partial class Nomina
	{
		public NominaReceptor? Receptor { get; set; }

		public NominaPercepciones? Percepciones { get; set; }

		public NominaDeducciones? Deducciones { get; set; }

		[XmlAttribute]
		public decimal Version { get; set; }

		[XmlAttribute]
		public string? TipoNomina { get; set; }

		[XmlAttribute(DataType = "date")]
		public DateTime FechaPago { get; set; }

		[XmlAttribute(DataType = "date")]
		public DateTime FechaInicialPago { get; set; }

		[XmlAttribute(DataType = "date")]
		public DateTime FechaFinalPago { get; set; }

		[XmlAttribute]
		public decimal NumDiasPagados { get; set; }

		[XmlAttribute]
		public decimal TotalPercepciones { get; set; }

		[XmlAttribute]
		public decimal TotalDeducciones { get; set; }
	}
}
