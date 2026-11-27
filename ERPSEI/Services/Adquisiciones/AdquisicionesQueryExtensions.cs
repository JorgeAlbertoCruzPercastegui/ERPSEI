using ERPSEI.Data.Entities.Adquisiciones;

namespace ERPSEI.Services.Adquisiciones
{
    public static class AdquisicionesQueryExtensions
    {
        public static IQueryable<AdqSolicitud>
            AplicarAlcanceOperativo(
                this IQueryable<AdqSolicitud> query,
                AdquisicionesAlcance alcance)
        {
            if (!alcance.UsuarioValido)
            {
                return query.Where(x => false);
            }

            query = query.Where(
                x => !x.Eliminado
            );

            if (alcance.TieneAlcanceGlobal)
            {
                return query;
            }

            int[] areas =
                alcance.AreasAutorizadas.ToArray();

            if (areas.Length == 0)
            {
                return query.Where(
                    x => false
                );
            }

            return query.Where(
                x => areas.Contains(x.AreaId)
            );
        }

        public static IQueryable<AdqSolicitud>
            AplicarAlcancePersonal(
                this IQueryable<AdqSolicitud> query,
                AdquisicionesAlcance alcance)
        {
            if (!alcance.UsuarioValido)
            {
                return query.Where(x => false);
            }

            string usuarioId =
                alcance.UsuarioId;

            return query.Where(
                x =>
                    !x.Eliminado &&
                    x.UsuarioSolicitanteId == usuarioId
            );
        }
    }
}