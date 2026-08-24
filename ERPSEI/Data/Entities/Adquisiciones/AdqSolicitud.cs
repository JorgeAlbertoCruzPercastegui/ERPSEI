using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Entities.Usuarios;

namespace ERPSEI.Data.Entities.Adquisiciones
{
    public class AdqSolicitud
    {
        public int Id
        {
            get;
            set;
        }

        public string Folio
        {
            get;
            set;
        } = string.Empty;

        public string Titulo
        {
            get;
            set;
        } = string.Empty;

        public DateTime FechaSolicitud
        {
            get;
            set;
        }

        public string UsuarioSolicitanteId
        {
            get;
            set;
        } = string.Empty;

        public AppUser UsuarioSolicitante
        {
            get;
            set;
        } = null!;

        public int? EmpleadoSolicitanteId
        {
            get;
            set;
        }

        public Empleado? EmpleadoSolicitante
        {
            get;
            set;
        }

        public int AreaId
        {
            get;
            set;
        }

        public Area Area
        {
            get;
            set;
        } = null!;

        public string Descripcion
        {
            get;
            set;
        } = string.Empty;

        public string Justificacion
        {
            get;
            set;
        } = string.Empty;

        public int EstatusId
        {
            get;
            set;
        }

        public AdqEstatus Estatus
        {
            get;
            set;
        } = null!;

        public string? UsuarioAsignadoId
        {
            get;
            set;
        }

        public AppUser? UsuarioAsignado
        {
            get;
            set;
        }

        public DateTime FechaCreacion
        {
            get;
            set;
        }

        public DateTime? FechaModificacion
        {
            get;
            set;
        }

        public DateTime? FechaEnvio
        {
            get;
            set;
        }

        public DateTime? FechaFinalizacion
        {
            get;
            set;
        }

        public bool Eliminado
        {
            get;
            set;
        }

        public ICollection<AdqSolicitudDetalle>
            Detalles
        {
            get;
            set;
        } = new List<AdqSolicitudDetalle>();

        public ICollection<AdqAdjunto>
            Adjuntos
        {
            get;
            set;
        } = new List<AdqAdjunto>();

        public ICollection<AdqHistorial>
            Historial
        {
            get;
            set;
        } = new List<AdqHistorial>();

        public ICollection<AdqAprobacion>
            Aprobaciones
        {
            get;
            set;
        } = new List<AdqAprobacion>();

        public ICollection<AdqAsignacion>
            Asignaciones
        {
            get;
            set;
        } = new List<AdqAsignacion>();

        public ICollection<AdqComentario>
            Comentarios
        {
            get;
            set;
        } = new List<AdqComentario>();
    }
}