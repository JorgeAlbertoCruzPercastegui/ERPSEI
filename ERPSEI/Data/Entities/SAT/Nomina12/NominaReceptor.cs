using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.Nomina12
{
	[Serializable]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/nomina12")]
	public partial class NominaReceptor
	{
		[XmlIgnore]
		public int Id { get; set; }

		[XmlElement("SubContratacion")]
		public NominaReceptorSubContratacion[]? SubContratacion { get; set; }

		
		[XmlAttribute]
		public string? Curp { get; set; }

		
		[XmlAttribute]
		public string? NumSeguridadSocial { get; set; }

		
		[XmlAttribute(DataType = "date")]
		public DateTime? FechaInicioRelLaboral { get; set; }

		
		[XmlIgnore]
		public bool FechaInicioRelLaboralSpecified { get; set; }

		
		[XmlAttribute]
		public string? Antigüedad { get; set; }

		
		[XmlAttribute]
		public string? TipoContrato { get; set; }

		
		[XmlAttribute]
		public string? Sindicalizado { get; set; }

		
		[XmlIgnore]
		public bool SindicalizadoSpecified { get; set; }

		
		[XmlAttribute]
		public string? TipoJornada { get; set; }

		
		[XmlIgnore]
		public bool TipoJornadaSpecified { get; set; }

		
		[XmlAttribute]
		public string? TipoRegimen { get; set; }

		
		[XmlAttribute]
		public string? NumEmpleado { get; set; }

		
		[XmlAttribute]
		public string? Departamento { get; set; }

		
		[XmlAttribute]
		public string? Puesto { get; set; }

		
		[XmlAttribute]
		public string? RiesgoPuesto { get; set; }

		
		[XmlIgnore]
		public bool RiesgoPuestoSpecified { get; set; }

		
		[XmlAttribute]
		public string? PeriodicidadPago { get; set; }

		
		[XmlAttribute]
		public string? Banco { get; set; }

		
		[XmlIgnore]
		public bool BancoSpecified { get; set; }

		
		[XmlAttribute(DataType = "integer")]
		public string? CuentaBancaria { get; set; }

		
		[XmlAttribute]
		public decimal SalarioBaseCotApor { get; set; }

		
		[XmlIgnore]
		public bool SalarioBaseCotAporSpecified { get; set; }

		
		[XmlAttribute]
		public decimal SalarioDiarioIntegrado { get; set; }

		
		[XmlIgnore]
		public bool SalarioDiarioIntegradoSpecified { get; set; }

		
		[XmlAttribute]
		public string? ClaveEntFed { get; set; }
	}
}
