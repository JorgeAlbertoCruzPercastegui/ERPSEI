namespace ERPSEI.Services.Adquisiciones
{
    public sealed class AdquisicionesAlcance
    {
        public string UsuarioId
        {
            get;
            init;
        } = string.Empty;

        public bool UsuarioValido
        {
            get;
            init;
        }

        public bool EsAdministradorSistema
        {
            get;
            init;
        }

        public bool EsAdministradorAdquisiciones
        {
            get;
            init;
        }

        public bool EsUsuarioAdquisiciones
        {
            get;
            init;
        }

        public bool EsAprobadorAdquisiciones
        {
            get;
            init;
        }

        public IReadOnlyCollection<int> AreasAutorizadas
        {
            get;
            init;
        } = Array.Empty<int>();

        public bool TieneAlcanceGlobal =>
            UsuarioValido &&
            (
                EsAdministradorSistema ||
                EsAdministradorAdquisiciones
            );

        public bool EsGestorArea =>
            UsuarioValido &&
            AreasAutorizadas.Count > 0;

        public bool TieneAccesoModulo =>
            UsuarioValido &&
            (
                TieneAlcanceGlobal ||
                EsGestorArea ||
                EsUsuarioAdquisiciones ||
                EsAprobadorAdquisiciones
            );

        public bool TieneAlcanceArea(int areaId)
        {
            if (!UsuarioValido || areaId <= 0)
            {
                return false;
            }

            return TieneAlcanceGlobal ||
                   AreasAutorizadas.Contains(areaId);
        }
    }
}