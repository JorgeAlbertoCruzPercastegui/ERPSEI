
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ERPSEI.Services.RelojChecador;

public static class RelojReglasConciliacion
{
    private static readonly HashSet<string> TiposValidos =
        new(StringComparer.Ordinal)
        {
            "Entrada",
            "SalidaComer",
            "RegresoComer",
            "Salida"
        };

    public sealed record Candidato(
        string IdMovimiento,
        string TipoMovimiento,
        bool Confirmado);

    public sealed record ResultadoValidacion(
        bool Correcto,
        string Codigo,
        string Mensaje)
    {
        public static ResultadoValidacion Ok() =>
            new(
                true,
                "VALIDO",
                "La selección es válida.");

        public static ResultadoValidacion Error(
            string codigo,
            string mensaje) =>
            new(false, codigo, mensaje);
    }

    public static bool EsTipoValido(string? tipo)
    {
        return tipo != null &&
               TiposValidos.Contains(tipo);
    }

    // Firma que identifica el conjunto exacto
    // de movimientos confirmados de un tipo.
    public static string CalcularFirma(
        string tipo,
        IEnumerable<string> identificadores)
    {
        ArgumentNullException.ThrowIfNull(
            identificadores);

        if (!EsTipoValido(tipo))
        {
            throw new ArgumentException(
                "Tipo de movimiento inválido.",
                nameof(tipo));
        }

        var ids = identificadores
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToArray();

        string contenido =
            tipo + "\n" +
            string.Join("\n", ids);

        byte[] hash = SHA256.HashData(
            Encoding.UTF8.GetBytes(contenido));

        return Convert.ToHexString(hash);
    }

    public static IReadOnlyList<Candidato>
        ObtenerConfirmados(
            IEnumerable<Candidato> movimientos,
            string tipo)
    {
        ArgumentNullException.ThrowIfNull(
            movimientos);

        if (!EsTipoValido(tipo))
        {
            return Array.Empty<Candidato>();
        }

        return movimientos
            .Where(x =>
                x.Confirmado &&
                x.TipoMovimiento == tipo)
            .OrderBy(
                x => x.IdMovimiento,
                StringComparer.Ordinal)
            .ToArray();
    }

    public static ResultadoValidacion ValidarSeleccion(
        string tipo,
        string? seleccionado,
        string? firmaEsperada,
        IEnumerable<string> candidatos)
    {
        if (!EsTipoValido(tipo))
        {
            return ResultadoValidacion.Error(
                "TIPO_INVALIDO",
                "El tipo de movimiento no es válido.");
        }

        if (string.IsNullOrWhiteSpace(seleccionado))
        {
            return ResultadoValidacion.Error(
                "SELECCION_INVALIDA",
                "Selecciona un movimiento válido.");
        }

        if (string.IsNullOrWhiteSpace(firmaEsperada) ||
            firmaEsperada.Length != 64)
        {
            return ResultadoValidacion.Error(
                "FIRMA_INVALIDA",
                "La firma de candidatos no es válida.");
        }

        if (candidatos == null)
        {
            return ResultadoValidacion.Error(
                "CANDIDATOS_INVALIDOS",
                "No se recibieron movimientos.");
        }

        var ids = candidatos.ToList();

        if (ids.Any(string.IsNullOrWhiteSpace) ||
            ids.Count != ids.Distinct(
                StringComparer.Ordinal).Count())
        {
            return ResultadoValidacion.Error(
                "CANDIDATOS_INVALIDOS",
                "Existen identificadores inválidos o duplicados.");
        }

        if (ids.Count < 2)
        {
            return ResultadoValidacion.Error(
                "SIN_CONFLICTO",
                "No existen al menos dos movimientos confirmados del mismo tipo.");
        }

        if (!ids.Contains(
            seleccionado,
            StringComparer.Ordinal))
        {
            return ResultadoValidacion.Error(
                "SELECCION_INVALIDA",
                "La marca seleccionada no pertenece al conflicto.");
        }

        string firmaActual =
            CalcularFirma(tipo, ids);

        if (!string.Equals(
            firmaActual,
            firmaEsperada,
            StringComparison.Ordinal))
        {
            return ResultadoValidacion.Error(
                "DATOS_CAMBIARON",
                "El conjunto de movimientos confirmados cambió.");
        }

        return ResultadoValidacion.Ok();
    }

    // Determina qué movimiento participa después
    // de una resolución auditada.
    //
    // No modifica registros originales.
    public static bool ResolucionAplicable(
        string tipo,
        IEnumerable<string> candidatos,
        string? seleccionado,
        string? firmaGuardada,
        string? estadoResolucion)
    {
        if (estadoResolucion != "Activa")
            return false;

        if (!EsTipoValido(tipo))
            return false;

        if (string.IsNullOrWhiteSpace(
            seleccionado))
            return false;

        var resultado = ValidarSeleccion(
            tipo,
            seleccionado,
            firmaGuardada,
            candidatos);

        return resultado.Correcto;
    }

    public static IReadOnlyList<string>
        ObtenerMovimientosParticipantes(
            string tipo,
            IEnumerable<string> candidatos,
            string? seleccionado,
            string? firmaGuardada,
            string? estadoResolucion)
    {
        var ids = candidatos.ToArray();

        if (!ResolucionAplicable(
            tipo,
            ids,
            seleccionado,
            firmaGuardada,
            estadoResolucion))
        {
            // No decidir automáticamente cuando
            // no existe resolución aplicable.
            return ids;
        }

        return new[] { seleccionado! };
    }
}
