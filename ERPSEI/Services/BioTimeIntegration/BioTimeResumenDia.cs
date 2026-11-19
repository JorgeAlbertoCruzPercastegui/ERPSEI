namespace ERPSEI.Services.BioTimeIntegration;

// Modelo de proyección para consultas agrupadas traducibles por EF Core.
public sealed class ResumenDiaBioTime
{
    public DateTime Fecha { get; set; }
    public string Codigo { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Departamento { get; set; } = "";
    public int TotalChecadas { get; set; }
    public DateTime PrimeraChecada { get; set; }
    public DateTime UltimaChecada { get; set; }
}
