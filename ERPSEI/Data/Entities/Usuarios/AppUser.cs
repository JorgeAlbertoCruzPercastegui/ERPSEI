using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Entities.SAT;
using Microsoft.AspNetCore.Identity;
using ERPSEI.Data.Entities.Conciliaciones;
using ERPSEI.Data.Entities.Polizas;

namespace ERPSEI.Data.Entities.Usuarios
{
    public class AppUser : IdentityUser
    {
        public bool IsBanned { get; set; }
        public bool PasswordResetNeeded { get; set; }
        public bool IsPreregisterAuthorized { get; set; }
        public bool IsMaster { get; set; }

        public int? EmpleadoId { get; set; }
        public Empleado? Empleado { get; set; }

        public ICollection<Prefactura> PrefacturasCreadas { get; set; } = [];
        public ICollection<AutorizacionesPrefactura> AutorizacionesPrefacturas { get; set; } = [];
        public ICollection<Prefactura> PrefacturasTimbradas { get; set; } = [];
        public ICollection<Conciliacion> ConciliacionesCreadas { get; set; } = [];
        public ICollection<Conciliacion> ConciliacionesModificadas { get; set; } = [];
		public ICollection<GrupoPoliza> GruposPolizasCreados { get; set; } = [];
		public ICollection<GrupoPoliza> GruposPolizasModificados { get; set; } = [];

	}
}
