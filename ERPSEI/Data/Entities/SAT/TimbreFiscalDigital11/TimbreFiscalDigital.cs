using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.TimbreFiscalDigital11
{
	[Serializable]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/TimbreFiscalDigital")]
	[XmlRoot(Namespace = "http://www.sat.gob.mx/TimbreFiscalDigital", IsNullable = false)]
	public partial class TimbreFiscalDigital
	{

		[XmlAttribute()]
		public DateTime FechaTimbrado { get; set; }

		[XmlAttribute()]
		public ulong NoCertificadoSAT { get; set; }

		[XmlAttribute()]
		public string? RfcProvCertif { get; set; }

		[XmlAttribute()]
		public string? SelloCFD { get; set; }

		[XmlAttribute()]
		public string? SelloSAT { get; set; }

		[XmlAttribute()]
		public string? UUID { get; set; }

		[XmlAttribute()]
		public decimal Version { get; set; }
	}


}
