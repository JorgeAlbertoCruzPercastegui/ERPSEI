namespace ERPSEI.Services.Adquisiciones
{
    public sealed class AdquisicionesConfiguracionAreaDto
    {
        public int AreaId { get; set; }

        public string NombreArea { get; set; } =
            string.Empty;

        public bool AreaExiste { get; set; }

        public bool UsuarioAutorizado { get; set; }

        public bool ConfiguracionCompleta { get; set; }

        public bool PuedeConfigurar { get; set; }

        public string Mensaje { get; set; } =
            string.Empty;

        public List<AdquisicionesNivelAprobacionDto>
            Etapas
        { get; set; } = new();

        public int TotalEtapas =>
            Etapas.Count;

        public int TotalEtapasConfiguradas =>
            Etapas.Count(x =>
                x.Activo &&
                !string.IsNullOrWhiteSpace(
                    x.UsuarioResponsableId
                )
            );

        public int TotalEtapasPendientes =>
            Math.Max(
                0,
                4 - TotalEtapasConfiguradas
            );
    }
}