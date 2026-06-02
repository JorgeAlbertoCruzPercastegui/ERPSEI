public class CorreoDominio
{
    public int Id { get; set; }

    public string? Empresa { get; set; }
    public string? Dominio { get; set; }
    public string? Proveedor { get; set; }
    public DateTime? FechaCaducacion { get; set; }
    public decimal? Costos { get; set; }

    public string? CorreoOperaciones { get; set; }
    public string? ContrasenaOperaciones { get; set; }

    public string? CorreoFiscal { get; set; }
    public string? ContrasenaFiscal { get; set; }

    public string? PagWeb { get; set; }
    public string? Estado { get; set; }
    public string? Observaciones { get; set; }

    public bool Deshabilitado { get; set; }

    public DateTime FechaCreacion { get; set; }
    public string? UsuarioCreador { get; set; }

    public DateTime? FechaModificacion { get; set; }
    public string? UsuarioModificador { get; set; }
}