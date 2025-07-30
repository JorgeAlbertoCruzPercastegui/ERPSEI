using System;

namespace ERPSEI.Data.Entities.TipoContratos
{
    public class ClienteContrato
    {
        public int Id { get; set; }
        public DateTime? FechaConstitucion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? RazonSocial { get; set; }
        public string? DomicilioFiscal { get; set; }
        public string? RFC { get; set; }
        public int? NoNotario { get; set; }
        public string? Notario { get; set; }
        public string? RepresentanteLegal { get; set; }
        public string? Email { get; set; }
        public string? PaginaWeb { get; set; }
        public bool Deshabilitado { get; set; } = false;
        public bool Estatus { get; set; } = false;

        // 🔗 Relación con EmpresaContrato
        public int EmpresaContratoId { get; set; }
        public EmpresaContrato EmpresaContrato { get; set; } = null!;

        public int? TipoContratoId { get; set; }
        public TipoContrato? TipoContrato { get; set; }
    }
}
