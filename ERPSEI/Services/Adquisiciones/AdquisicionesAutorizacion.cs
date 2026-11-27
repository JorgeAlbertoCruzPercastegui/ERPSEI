namespace ERPSEI.Services.Adquisiciones
{
    public static class AdquisicionesAutorizacion
    {
        public static bool Autorizar(
            AdquisicionesAccesoSolicitud acceso,
            AdquisicionesOperacion operacion)
        {
            if (acceso == null ||
                !acceso.UsuarioValido ||
                !acceso.SolicitudExiste)
            {
                return false;
            }

            return operacion switch
            {
                AdquisicionesOperacion.Consultar =>
                    acceso.PuedeConsultar,

                AdquisicionesOperacion.Gestionar =>
                    acceso.PuedeGestionar,

                AdquisicionesOperacion.CrearDocumentacion =>
                    acceso.PuedeCrearDocumentacion,

                AdquisicionesOperacion.EliminarDocumentacion =>
                    acceso.PuedeEliminarDocumentacion,

                AdquisicionesOperacion.Descargar =>
                    acceso.PuedeDescargar,

                AdquisicionesOperacion.EditarSolicitudPropia =>
                    acceso.PuedeEditarSolicitudPropia,

                AdquisicionesOperacion.CancelarSolicitudPropia =>
                    acceso.PuedeCancelarSolicitudPropia,

                AdquisicionesOperacion.AprobarComoGerente =>
                    acceso.PuedeParticiparComoGerente,

                AdquisicionesOperacion.AprobarPresupuesto =>
                    acceso.PuedeParticiparComoAprobador,

                AdquisicionesOperacion.ConsultarComoParticipante =>
                    acceso.PuedeConsultar,

                AdquisicionesOperacion.Comentar =>
                    acceso.PuedeConsultar &&
                    (
                        acceso.EsSolicitante ||
                        acceso.EsGerenteAprobador ||
                        acceso.EsAprobadorPresupuestal ||
                        acceso.PuedeGestionar
                    ),

                _ => false
            };
        }
    }
}