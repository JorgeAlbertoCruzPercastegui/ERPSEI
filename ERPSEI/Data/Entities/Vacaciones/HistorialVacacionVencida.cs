using ERPSEI.Data.Entities.Empleados;

public class HistorialVacacionVencida
{
    public int Id { get; set; }

    public int EmpleadoId { get; set; }
    public Empleado? Empleado { get; set; }

    public DateTime FechaGeneracion { get; set; }
    public DateTime FechaVencimiento { get; set; }

    public decimal DiasVencidos { get; set; }

    public string Periodo { get; set; } = string.Empty;
    public string Causa { get; set; } = string.Empty;
}