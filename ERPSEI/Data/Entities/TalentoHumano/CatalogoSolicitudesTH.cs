
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERPSEI.Data.Entities.TalentoHumano
{
    public sealed record DefinicionSolicitudTH(
        TipoSolicitudTH Tipo,
        string Nombre,
        string CodigoFolio
    );

    public static class CatalogoSolicitudesTH
    {
        private static readonly IReadOnlyList<DefinicionSolicitudTH>
            Tipos = Array.AsReadOnly(new[]
            {
                new DefinicionSolicitudTH(
                    TipoSolicitudTH.ConstanciaLaboral,
                    "Constancias laborales",
                    "CONST"),

                new DefinicionSolicitudTH(
                    TipoSolicitudTH.DesgloseNomina,
                    "Desglose de nómina",
                    "NOM"),

                new DefinicionSolicitudTH(
                    TipoSolicitudTH.ComprobanteNominaCFDI,
                    "Comprobantes de nómina timbrados",
                    "CFDI"),

                new DefinicionSolicitudTH(
                    TipoSolicitudTH.VacacionesSaldo,
                    "Vacaciones (saldo)",
                    "VAC"),

                new DefinicionSolicitudTH(
                    TipoSolicitudTH.Prestamo,
                    "Préstamos",
                    "PRES"),

                new DefinicionSolicitudTH(
                    TipoSolicitudTH.AnticipoFondoAhorro,
                    "Anticipos de Fondo de Ahorro",
                    "FA"),

                new DefinicionSolicitudTH(
                    TipoSolicitudTH.CreditoInfonavit,
                    "Créditos Infonavit",
                    "INF")
            });

        public static IReadOnlyList<DefinicionSolicitudTH>
            ObtenerTodos()
        {
            return Tipos;
        }

        public static DefinicionSolicitudTH ObtenerPorTipo(
            TipoSolicitudTH tipo)
        {
            return Tipos.FirstOrDefault(x => x.Tipo == tipo)
                ?? throw new ArgumentOutOfRangeException(
                    nameof(tipo),
                    "El tipo de solicitud no existe."
                );
        }

        public static string ObtenerPrefijoFolio(
            TipoSolicitudTH tipo,
            int anio)
        {
            if (anio < 2000 || anio > 9999)
                throw new ArgumentOutOfRangeException(
                    nameof(anio));

            var definicion = ObtenerPorTipo(tipo);

            return $"SOL-{definicion.CodigoFolio}-{anio}-";
        }
    }
}
