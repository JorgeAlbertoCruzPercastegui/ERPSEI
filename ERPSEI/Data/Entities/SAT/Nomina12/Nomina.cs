using System.Xml.Serialization;
namespace ERPSEI.Data.Entities.SAT.Nomina12
{
	[Serializable]
	[XmlType(AnonymousType = true)]
	[XmlRoot(Namespace = "http://www.sat.gob.mx/nomina12", IsNullable = false)]
	public partial class Nomina
	{
		[XmlIgnore]
		public int Id { get; set; }

		public NominaEmisor? Emisor { get; set; }

		public NominaReceptor? Receptor { get; set; }

		public NominaPercepciones? Percepciones { get; set; }

		public NominaDeducciones? Deducciones { get; set; }

		[XmlArray("OtrosPagos", IsNullable = false)]
		[XmlArrayItem(typeof(NominaOtroPago), ElementName = "OtroPago", IsNullable = false)]
		public List<NominaOtroPago>? OtrosPagos { get; set; }

		[XmlArray("Incapacidades", IsNullable = false)]
		[XmlArrayItem(typeof(NominaIncapacidad), ElementName = "Incapacidad", IsNullable = false)]
		public List<NominaIncapacidad>? Incapacidades { get; set; }

		[XmlAttribute]
		public decimal Version { get; set; } = 1.2m;

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

		[XmlAttribute]
		public decimal TotalOtrosPagos { get; set; }

	}
}
