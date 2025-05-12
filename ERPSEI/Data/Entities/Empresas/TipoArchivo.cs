namespace ERPSEI.Data.Entities.Empresas
{
    public enum FileTypes
    {
        CSF = 1,
        INE,
		RFC,
        ComprobanteDomicilio,
        Otro,
        CER,
        KEY, 
        Logo,
        HojaMembretada,
		INE2,
		INE3,
		ActaConstitutiva,
        Organigrama
    }
    public class TipoArchivoEmpresa
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public ICollection<ArchivoEmpresa>? ArchivosEmpresa { get; }

        public TipoArchivoEmpresa(int id, string description)
        {
            Id = id;
            Description = description;
        }

    }
}
