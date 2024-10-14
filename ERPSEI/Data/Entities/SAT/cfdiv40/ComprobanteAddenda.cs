using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.cfdiv40
{
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/4")]
	public partial class ComprobanteAddenda
	{
		[XmlIgnore]
		public int Id { get; set; }

		[NotMapped]
		[System.Xml.Serialization.XmlAnyElementAttribute()]
		public System.Xml.XmlElement[]? Any { get; set; }
	}
}
