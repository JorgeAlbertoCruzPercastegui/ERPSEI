namespace ERPSEI.Data.Entities.Empleados
{
    public enum FileTypes
    {
        ImagenPerfil = 1,
        ActaNacimiento = 2,
        CURP = 3,
        CLABE = 4,
        ComprobanteDomicilio = 5,
        CSF = 6,
        INE = 7,
        RFC = 8,
        ComprobanteEstudios = 9,
        NSS = 10,
        Otro = 11
    }

    public class TipoArchivo
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

        public ICollection<ArchivoEmpleado>? ArchivosEmpleado
        {
            get;
        }

        public TipoArchivo(
            int id,
            string description)
        {
            Id = id;
            Description = description;
        }
    }
}