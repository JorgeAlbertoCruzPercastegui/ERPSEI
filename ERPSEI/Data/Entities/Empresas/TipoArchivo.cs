namespace ERPSEI.Data.Entities.Empresas
{
    public enum FileTypes
    {
        CSF = 1,
        INE = 2,
        RFC = 3,
        ComprobanteDomicilio = 4,
        Otro = 5,
        CER = 6,
        KEY = 7,
        Logo = 8,
        HojaMembretada = 9,
        INE2 = 10,
        INE3 = 11,

        ActaConstitutiva = 12,
        Organigrama = 13,

        ActasAdicionales = 14,
        PoderNotarial = 15,
        INEAccionistas = 16,
        CSFAccionistas = 17,
        ComprobanteDomicilioAccionistas = 18,
        DeclaracionAnualMensual = 19,
        OpinionCumplimientoSAT = 20,
        PruebaVida = 21
    }

    public class TipoArchivoEmpresa
    {
        public int Id
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public ICollection<ArchivoEmpresa>? ArchivosEmpresa
        {
            get;
        }

        public TipoArchivoEmpresa(
            int id,
            string description)
        {
            Id = id;
            Description = description;
        }
    }
}