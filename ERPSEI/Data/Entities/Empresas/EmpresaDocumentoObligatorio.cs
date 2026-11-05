namespace ERPSEI.Data.Entities.Empresas
{
    /// <summary>
    /// Documento requerido para el expediente de una empresa concreta.
    /// Cada par (EmpresaId, TipoArchivoId) es único.
    /// La ausencia de filas no debe interpretarse como expediente completo:
    /// significa que no se han configurado documentos obligatorios.
    /// </summary>
    public class EmpresaDocumentoObligatorio
    {
        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; } = null!;

        public int TipoArchivoId { get; set; }
        public TipoArchivoEmpresa TipoArchivo { get; set; } = null!;
    }
}
