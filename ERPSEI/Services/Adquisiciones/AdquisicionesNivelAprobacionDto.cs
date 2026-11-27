namespace ERPSEI.Services.Adquisiciones
{
    public sealed class AdquisicionesNivelAprobacionDto
    {
        public int Id { get; set; }

        public int Orden { get; set; }

        public string TipoEtapa { get; set; } =
            string.Empty;

        public string NombreEtapa { get; set; } =
            string.Empty;

        public string UsuarioResponsableId { get; set; } =
            string.Empty;

        public string? NombreResponsable { get; set; }

        public string? UsuarioAsistenteId { get; set; }

        public string? NombreAsistente { get; set; }

        public bool AsistenteRecibeCopia { get; set; }

        public int? RecibirCopiaDesdeOrden { get; set; }

        public bool Activo { get; set; } = true;
    }
}