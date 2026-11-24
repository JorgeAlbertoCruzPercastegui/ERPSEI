
namespace ERPSEI.Services.RelojChecador;

public sealed class RelojEvidenciasOptions
{
    public const string SectionName = "RelojChecador:Evidencias";

    public string RutaPrivada { get; set; } = string.Empty;

    public int TamanoMaximoMB { get; set; } = 2;
}
