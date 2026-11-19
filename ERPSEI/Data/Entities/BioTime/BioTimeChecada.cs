namespace ERPSEI.Data.Entities.BioTime;

public class BioTimeChecada
{
    public long Id { get; set; } // Id original de la transacción BioTime
    public string EmpCode { get; set; } = "";
    public DateTime FechaHora { get; set; } // Hora local tal como la devuelve BioTime, sin conversión implícita
    public string? PunchState { get; set; }
    public int? VerifyType { get; set; }
    public string? TerminalSn { get; set; }
    public DateTime SincronizadoUtc { get; set; }
}
