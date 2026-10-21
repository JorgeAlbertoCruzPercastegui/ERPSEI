namespace ERPSEI.Services.TimeIntegration
{
    public sealed class TimeApiOptions
    {
        public string BaseUrl { get; set; } =
            "https://api-time.cybertradeinternational.mx/";

        public string ApiKey { get; set; } = string.Empty;

        public string? DevelopmentProxy { get; set; }
    }
}