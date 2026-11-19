namespace ERPSEI.Services.BioTimeIntegration;

public sealed class BioTimeOptions
{
    public string BaseUrl { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string Company { get; set; } = "";
    public bool Enabled { get; set; } = false;
    public bool RunOnStartup { get; set; } = false;
    public int IntervalMinutes { get; set; } = 15;
    public int LookbackDays { get; set; } = 2;
    public string TimeZoneId { get; set; } = "America/Mexico_City";
}