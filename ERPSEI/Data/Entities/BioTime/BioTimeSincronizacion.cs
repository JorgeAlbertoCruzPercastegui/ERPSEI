namespace ERPSEI.Data.Entities.BioTime;

public class BioTimeSincronizacion
{
    public long Id { get; set; }
    public DateTime InicioUtc { get; set; }
    public DateTime? FinUtc { get; set; }
    public DateTime FechaInicioConsulta { get; set; }
    public DateTime FechaFinConsulta { get; set; }
    public string Estado { get; set; } = "Iniciada";
    public int EmpleadosProcesados { get; set; }
    public int ChecadasInsertadas { get; set; }
    public string? Error { get; set; }
}
