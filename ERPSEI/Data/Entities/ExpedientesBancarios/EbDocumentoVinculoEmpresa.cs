namespace ERPSEI.Data.Entities.ExpedientesBancarios
{
    public class EbDocumentoVinculoEmpresa
    {
        public int Id { get; set; }

        /*
         * Empresa.Id del módulo maestro.
         */
        public int EmpresaMaestraId { get; set; }

        /*
         * EbEmpresa.Id de Compliance.
         */
        public int EmpresaComplianceId { get; set; }

        /*
         * TipoArchivoId utilizado por Empresas.
         */
        public int TipoArchivoEmpresaId { get; set; }

        /*
         * EbTipoDocumento.Id utilizado por Compliance.
         */
        public int TipoDocumentoComplianceId { get; set; }

        /*
         * Id actual de ArchivoEmpresa.
         *
         * Es nullable porque Empresas puede eliminar y
         * volver a crear el archivo durante una edición.
         */
        public string? ArchivoEmpresaId { get; set; }

        /*
         * EbDocumento.Id asociado a la versión de Compliance.
         */
        public int? DocumentoComplianceId { get; set; }

        /*
         * SHA-256 del contenido sincronizado.
         *
         * Permite saber si realmente cambió el archivo,
         * independientemente de su nombre o Id.
         */
        public string HashContenido { get; set; }
            = string.Empty;

        /*
         * Indica desde qué módulo se originó
         * la última sincronización.
         *
         * Valores previstos:
         * Empresas
         * Compliance
         */
        public string Origen { get; set; }
            = string.Empty;

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; }
            = DateTime.Now;

        public DateTime? FechaActualizacion { get; set; }
    }
}