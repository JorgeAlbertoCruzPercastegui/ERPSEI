namespace ERPSEI.Data.Entities.RH
{
    public class AusenciaDocumento
    {
        public int Id { get; set; }

        public int AusenciaId { get; set; }
        public Ausencia? Ausencia { get; set; }

        public string NombreOriginal { get; set; } = string.Empty;
        public string NombreGuardado { get; set; } = string.Empty;
        public string RutaArchivo { get; set; } = string.Empty;
        public string Extension { get; set; } = ".pdf";
        public long TamanioBytes { get; set; }

        public string UsuarioCreadorId { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}