using System.ComponentModel.DataAnnotations.Schema;
using System.Xml;
using System.Xml.Serialization;
using ERPSEI.Data.Entities.SAT.Nomina12;
using ERPSEI.Data.Entities.SAT.Pagos20;
using ERPSEI.Data.Entities.SAT.TimbreFiscalDigital11;

namespace ERPSEI.Data.Entities.SAT.cfdiv40
{
	[Serializable]
	[System.Diagnostics.DebuggerStepThrough()]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/4")]
	public partial class ComprobanteComplemento
	{
		[XmlIgnore]
		public int Id { get; set; }

		[NotMapped]
		[XmlAnyElement]
		public virtual XmlElement[]? Elements { get; set; }

		[XmlIgnore]
		public virtual Nomina? Nomina { get; set; }

		[XmlIgnore]
		public virtual TimbreFiscalDigital? TimbreFiscalDigital { get; set; }

		[XmlIgnore]
		public virtual Pagos? Pago { get; set; }
	}
}
