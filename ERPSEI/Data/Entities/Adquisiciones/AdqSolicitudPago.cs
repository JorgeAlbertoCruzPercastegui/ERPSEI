using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.Adquisiciones
{
    [Table("ADQ_SolicitudesPago")]
    public class AdqSolicitudPago
    {
        [Key]
        public int Id
        {
            get;
            set;
        }


        public int SolicitudId
        {
            get;
            set;
        }

        public int? AprobacionPresupuestalId
        {
            get;
            set;
        }

        public int CotizacionId
        {
            get;
            set;
        }


        [Required]
        [StringLength(250)]
        public string Compania
        {
            get;
            set;
        } = string.Empty;


        [Required]
        [StringLength(250)]
        public string AreaSolicitante
        {
            get;
            set;
        } = string.Empty;


        [Required]
        [StringLength(30)]
        public string Moneda
        {
            get;
            set;
        } = "Pesos";


        [Required]
        [StringLength(30)]
        public string FormaPago
        {
            get;
            set;
        } = "Transferencia";


        [Required]
        [StringLength(5000)]
        public string ConceptoPago
        {
            get;
            set;
        } = string.Empty;


        [Required]
        [StringLength(250)]
        public string NombreProveedor
        {
            get;
            set;
        } = string.Empty;


        [StringLength(250)]
        public string? Banco
        {
            get;
            set;
        }


        [StringLength(100)]
        public string? Cuenta
        {
            get;
            set;
        }


        [StringLength(100)]
        public string? ClabeInterbancaria
        {
            get;
            set;
        }


        public bool ComprobanteAdjunto
        {
            get;
            set;
        }


        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal
        {
            get;
            set;
        }


        [Column(TypeName = "decimal(18,2)")]
        public decimal Iva
        {
            get;
            set;
        }


        [Column(TypeName = "decimal(18,2)")]
        public decimal RetencionIva
        {
            get;
            set;
        }


        [Column(TypeName = "decimal(18,2)")]
        public decimal RetencionIsr
        {
            get;
            set;
        }


        [Column(TypeName = "decimal(18,2)")]
        public decimal OtrosImpuestos
        {
            get;
            set;
        }


        [Column(TypeName = "decimal(18,2)")]
        public decimal OtrosServicios
        {
            get;
            set;
        }


        [Column(TypeName = "decimal(18,2)")]
        public decimal Total
        {
            get;
            set;
        }


        [Required]
        [StringLength(30)]
        public string TipoDocumentoSolicitud
        {
            get;
            set;
        } = "Cotizaciones";


        public DateTime FechaSolicitud
        {
            get;
            set;
        }


        public DateTime FechaGeneracion
        {
            get;
            set;
        }


        [StringLength(450)]
        public string? UsuarioGeneracionId
        {
            get;
            set;
        }


        [StringLength(260)]
        public string? NombreArchivo
        {
            get;
            set;
        }


        [StringLength(1000)]
        public string? RutaArchivo
        {
            get;
            set;
        }


        [StringLength(128)]
        public string? HashArchivo
        {
            get;
            set;
        }


        public bool PdfGenerado
        {
            get;
            set;
        }


        public bool Eliminado
        {
            get;
            set;
        }


        public AdqSolicitud? Solicitud
        {
            get;
            set;
        }


        public AdqAprobacionPresupuestal? AprobacionPresupuestal
        {
            get;
            set;
        }


        public AdqCotizacion? Cotizacion
        {
            get;
            set;
        }
    }
}