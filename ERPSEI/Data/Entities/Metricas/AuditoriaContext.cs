namespace ERPSEI.Data.Entities.Metricas
{
    public class AuditoriaContext
    {
        public bool Activada { get; private set; }
        public string? Modulo { get; private set; }
        public string? Accion { get; private set; }

        public void Activar(string modulo, string accion)
        {
            Activada = true;
            Modulo = modulo;
            Accion = accion;
        }

        public void Desactivar()
        {
            Activada = false;
            Modulo = null;
            Accion = null;
        }
    }
}
