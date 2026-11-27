namespace ERPSEI.Services.Adquisiciones
{
    public sealed class AdquisicionesAccesoSolicitud
    {
        public int SolicitudId { get; init; }

        public bool SolicitudExiste { get; init; }

        public bool UsuarioValido { get; init; }

        public bool TieneAlcanceGlobal { get; init; }

        public bool TieneAlcanceArea { get; init; }

        public bool EsSolicitante { get; init; }

        public bool EsGerenteAprobador { get; init; }

        public bool EsAgenteAsignado { get; init; }

        public bool EsAprobadorPresupuestal { get; init; }

        public bool EsObservadorPresupuestal { get; init; }

        public bool AbacPuedeVisualizar { get; init; }

        public bool AbacPuedeCrear { get; init; }

        public bool AbacPuedeEditar { get; init; }

        public bool AbacPuedeEliminar { get; init; }

        public bool AbacPuedeDescargar { get; init; }

        public bool TieneAlcanceOperativo =>
            UsuarioValido &&
            SolicitudExiste &&
            (
                TieneAlcanceGlobal ||
                TieneAlcanceArea ||
                EsAgenteAsignado
            );

        public bool PuedeConsultar =>
            UsuarioValido &&
            SolicitudExiste &&
            (
                EsSolicitante ||
                EsGerenteAprobador ||
                EsAprobadorPresupuestal ||
                EsObservadorPresupuestal ||
                (
                    AbacPuedeVisualizar &&
                    TieneAlcanceOperativo
                )
            );

        public bool PuedeGestionar =>
            TieneAlcanceOperativo &&
            AbacPuedeEditar;

        public bool PuedeCrearDocumentacion =>
            TieneAlcanceOperativo &&
            AbacPuedeCrear;

        public bool PuedeEliminarDocumentacion =>
            TieneAlcanceOperativo &&
            AbacPuedeEliminar;

        public bool PuedeDescargar =>
            PuedeConsultar &&
            AbacPuedeDescargar;

        public bool PuedeEditarSolicitudPropia =>
            UsuarioValido &&
            SolicitudExiste &&
            EsSolicitante &&
            AbacPuedeEditar;

        public bool PuedeCancelarSolicitudPropia =>
            UsuarioValido &&
            SolicitudExiste &&
            EsSolicitante &&
            AbacPuedeEditar;

        public bool PuedeParticiparComoGerente =>
            UsuarioValido &&
            SolicitudExiste &&
            EsGerenteAprobador;

        public bool PuedeParticiparComoAprobador =>
            UsuarioValido &&
            SolicitudExiste &&
            EsAprobadorPresupuestal;
    }
}