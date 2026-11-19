namespace ERPSEI.Data.Entities.BioTime;

public class BioTimeEmpleado
{
    public string EmpCode { get; set; } = "";
    public string? Nombre { get; set; }
    public string? Apellidos { get; set; }
    public string? Departamento { get; set; }
    public string? Puesto { get; set; }
    public DateTime ActualizadoUtc { get; set; }
}