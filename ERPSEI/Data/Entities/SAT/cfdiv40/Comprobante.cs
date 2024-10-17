using ERPSEI.Data.Entities.Conciliaciones;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ERPSEI.Data.Entities.SAT.cfdiv40
{
	[Serializable()]
	[System.Diagnostics.DebuggerStepThrough()]
	[System.ComponentModel.DesignerCategory("code")]
	[XmlType(AnonymousType = true, Namespace = "http://www.sat.gob.mx/cfd/4")]
	[XmlRoot(Namespace = "http://www.sat.gob.mx/cfd/4", IsNullable = false)]
	public partial class Comprobante
	{
		[XmlIgnore]
		public int Id { get; set; }

		public ComprobanteInformacionGlobal? InformacionGlobal{ get; set; }

		[XmlArray("CfdiRelacionados")]
		[XmlArrayItem(typeof(ComprobanteCfdiRelacionados), ElementName = "CfdiRelacionados", IsNullable = false)]
		public ComprobanteCfdiRelacionados[]? CfdiRelacionados { get; set; }

		
		public ComprobanteEmisor? Emisor { get; set; }

		
		public ComprobanteReceptor? Receptor { get; set; }


		[XmlArray("Conceptos")]
		[XmlArrayItem(typeof(ComprobanteConcepto), ElementName = "Concepto", IsNullable = false)]
		public ComprobanteConcepto[]? Conceptos { get; set; }

		
		public ComprobanteImpuestos? Impuestos { get; set; }


		public ComprobanteComplemento? Complemento { get; set; }

		[XmlAttribute()]
		public string Version { get; set; } = "4.0";

		
		[XmlAttribute()]
		public string? Serie { get; set; }

		
		[XmlAttribute()]
		public string? Folio { get; set; }


		[XmlAttribute()]
		public string? Fecha { get; set; }

		
		[XmlAttribute()]
		public string? Sello { get; set; }

		
		[XmlAttribute()]
		public string? FormaPago { get; set; }

		
		[XmlIgnore()]
		public bool FormaPagoSpecified { get; set; }

		
		[XmlAttribute()]
		public string? NoCertificado { get; set; }

		
		[XmlAttribute()]
		public string? Certificado {  get; set; }

		
		[XmlAttribute()]
		public string? CondicionesDePago { get; set; }

		
		[XmlAttribute()]
		public decimal SubTotal { get; set; }

		
		[XmlAttribute()]
		public decimal Descuento { get; set; }

		
		[XmlIgnore()]
		public bool DescuentoSpecified { get; set; }

		
		[XmlAttribute()]
		public string? Moneda { get; set; }

		
		[XmlAttribute()]
		public decimal TipoCambio { get; set; }

		
		[XmlIgnore()]
		public bool TipoCambioSpecified { get; set; }

		
		[XmlAttribute()]
		public decimal Total { get; set; }

		
		[XmlAttribute()]
		public string? TipoDeComprobante { get; set; }

		
		[XmlAttribute()]
		public string? Exportacion { get; set; }

		
		[XmlAttribute()]
		public string? MetodoPago { get; set; }

		
		[XmlIgnore()]
		public bool MetodoPagoSpecified { get; set; }

		
		[XmlAttribute()]
		public string? LugarExpedicion { get; set; }

		[NotMapped]
		[XmlAttribute("schemaLocation", Namespace = XmlSchema.InstanceNamespace)]
		public string xsiSchemaLocation = "http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd";
		
		[XmlAttribute()]
		public string? Confirmacion { get; set; }

		[XmlIgnore()]
		public ConciliacionDetalleComprobante? ConciliacionDetalleComprobante { get; set; }
    
		[XmlIgnore]
		public bool? Conciliado { get; set; }

		[XmlIgnore]
		public bool? Cancelado { get; set; }
	}
}
