
using System;

namespace ERPSEI.Data.Entities.TalentoHumano
{
    public class THFolioConsecutivo
    {
        public int Id { get; set; }

        public TipoSolicitudTH Tipo { get; set; }

        public int Anio { get; set; }

        public int UltimoConsecutivo { get; set; }

        public DateTime FechaActualizacionUtc { get; set; }

        public byte[] RowVersion { get; set; }
            = Array.Empty<byte>();
    }
}
