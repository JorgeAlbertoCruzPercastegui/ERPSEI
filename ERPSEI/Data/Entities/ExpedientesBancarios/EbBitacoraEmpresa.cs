namespace ERPSEI.Data.Entities.ExpedientesBancarios
{
    public class EbBitacoraEmpresa
    {
        public long Id
        {
            get;
            set;
        }

        public int EmpresaId
        {
            get;
            set;
        }

        public string Accion
        {
            get;
            set;
        } = string.Empty;

        public string UsuarioId
        {
            get;
            set;
        } = string.Empty;

        public string NombreUsuario
        {
            get;
            set;
        } = string.Empty;

        public DateTime FechaEvento
        {
            get;
            set;
        } = DateTime.Now;

        public string? DireccionIp
        {
            get;
            set;
        }

        public string? Navegador
        {
            get;
            set;
        }

        public bool Exitoso
        {
            get;
            set;
        } = true;

        public string? Detalle
        {
            get;
            set;
        }

        public EbEmpresa? Empresa
        {
            get;
            set;
        }
    }
}