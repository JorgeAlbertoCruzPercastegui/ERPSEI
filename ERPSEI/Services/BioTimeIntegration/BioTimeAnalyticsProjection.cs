namespace ERPSEI.Services.BioTimeIntegration;

public sealed class ChecadaAnalitica
{
    public long Id { get; set; }
    public string Codigo { get; set; } = "";
    public DateTime FechaHora { get; set; }
    public string Nombre { get; set; } = "";
    public string Departamento { get; set; } = "";
    public string? Estado { get; set; }
    public string? Dispositivo { get; set; }
}
