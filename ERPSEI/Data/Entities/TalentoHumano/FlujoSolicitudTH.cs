
using System.Collections.Generic;

namespace ERPSEI.Data.Entities.TalentoHumano
{
    public static class FlujoSolicitudTH
    {
        private static readonly IReadOnlyDictionary<
            EstatusSolicitudTH,
            HashSet<EstatusSolicitudTH>> Transiciones =
            new Dictionary<EstatusSolicitudTH,
                HashSet<EstatusSolicitudTH>>
            {
                [EstatusSolicitudTH.Enviada] = new()
                {
                    EstatusSolicitudTH.EnProceso,
                    EstatusSolicitudTH.Respondida,
                    EstatusSolicitudTH.Rechazada,
                    EstatusSolicitudTH.Cancelada
                },

                [EstatusSolicitudTH.EnProceso] = new()
                {
                    EstatusSolicitudTH.Respondida,
                    EstatusSolicitudTH.Rechazada
                },

                [EstatusSolicitudTH.Respondida] = new()
                {
                    EstatusSolicitudTH.EnProceso,
                    EstatusSolicitudTH.Cerrada
                },

                [EstatusSolicitudTH.Cerrada] = new(),
                [EstatusSolicitudTH.Rechazada] = new(),
                [EstatusSolicitudTH.Cancelada] = new()
            };

        public static bool PuedeCambiar(
            EstatusSolicitudTH actual,
            EstatusSolicitudTH nuevo)
        {
            return Transiciones.TryGetValue(
                actual, out var permitidos)
                && permitidos.Contains(nuevo);
        }

        public static bool PuedeCancelar(
            EstatusSolicitudTH actual)
        {
            return actual == EstatusSolicitudTH.Enviada;
        }

        public static bool EsEstadoFinal(
            EstatusSolicitudTH estatus)
        {
            return estatus == EstatusSolicitudTH.Cerrada
                || estatus == EstatusSolicitudTH.Rechazada
                || estatus == EstatusSolicitudTH.Cancelada;
        }

        public static bool RequiereRespuesta(
            EstatusSolicitudTH nuevo)
        {
            return nuevo == EstatusSolicitudTH.Respondida
                || nuevo == EstatusSolicitudTH.Cerrada;
        }

        public static bool RequiereMotivoRechazo(
            EstatusSolicitudTH nuevo)
        {
            return nuevo == EstatusSolicitudTH.Rechazada;
        }
    }
}
