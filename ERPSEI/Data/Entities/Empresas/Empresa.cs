using ERPSEI.Data.Entities.SAT;
using ERPSEI.Data.Entities.SAT.Catalogos;
using ERPSEI.Data.Entities.Conciliaciones;
using System.ComponentModel.DataAnnotations.Schema;
using ERPSEI.Data.Entities.Cuentas;

namespace ERPSEI.Data.Entities.Empresas
{
    public class Empresa
    {
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }

		public string? RazonSocial { get; set; }

		public int? OrigenId { get; set; }
		public Origen? Origen { get; set; }

		public int? NivelId { get; set; }
		public Nivel? Nivel { get; set; }

        public DateTime? FechaConstitucion { get; set; } = DateTime.MinValue;

		public DateTime? FechaInicioOperacion {  get; set; } = DateTime.MinValue;

        public DateTime? FechaInicioFacturacion { get; set; } = DateTime.MinValue;

        public DateTime? FechaInicioAsimilados { get; set; } = DateTime.MinValue;

        public string? RFC {  get; set; } = string.Empty;

		public string? DomicilioFiscal { get; set; } = string.Empty;

		public string? Administrador { get; set; } = string.Empty;

		public string? Accionista { get; set; } = string.Empty;

		public string? CorreoGeneral { get; set; } = string.Empty;

		public string? CorreoBancos { get; set; } = string.Empty;

		public string? CorreoFiscal { get; set; } = string.Empty;

		public string? CorreoFacturacion {  get; set; } = string.Empty;

		public string? Telefono { get; set; } = string.Empty;

		public int? PerfilId {  get; set; }
		public Perfil? Perfil {  get; set; }

		public ICollection<ActividadEconomicaEmpresa> ActividadesEconomicasEmpresa { get; } = new List<ActividadEconomicaEmpresa>();

		public string? ObjetoSocial { get; set; } = string.Empty; 

        public int Deshabilitado { get; set; } = 0;

        public ICollection<BancoEmpresa>? BancosEmpresa { get; }

        public ICollection<ArchivoEmpresa>? ArchivosEmpresa { get; }

		public RegimenFiscal? RegimenFiscal { get; set; }
		public int? RegimenFiscalId { get; set; }

		public ICollection<Prefactura>? PrefacturasEmitidas { get; set; }
		public ICollection<Prefactura>? PrefacturasRecibidas { get; set; }
        public ICollection<Conciliacion>? Conciliaciones { get; set; }
		public ICollection<CuentaContable>? CuentasContables { get; set; } = [];

		public string? PFESAT { get; set; }
	}
}
