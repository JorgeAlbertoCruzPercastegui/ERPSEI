
using ERPSEI.Services.RelojChecador.Models;

namespace ERPSEI.Services.RelojChecador;

public sealed class AsistenciaMovimientosService
{
    private readonly IReadOnlyList<IProveedorMovimientosAsistencia>
        _proveedores;

    public AsistenciaMovimientosService(
        IEnumerable<IProveedorMovimientosAsistencia> proveedores)
    {
        _proveedores = proveedores.ToList();

        var duplicados = _proveedores
            .GroupBy(
                x => x.Fuente,
                StringComparer.OrdinalIgnoreCase)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToList();

        if (duplicados.Count > 0)
        {
            throw new InvalidOperationException(
                "Hay proveedores de movimientos duplicados: " +
                string.Join(", ", duplicados));
        }
    }

    public async Task<IReadOnlyList<MovimientoAsistenciaNormalizado>>
        ObtenerMovimientosAsync(
            int empleadoId,
            DateTime fechaInicioLocal,
            DateTime fechaFinLocal,
            CancellationToken cancellationToken = default)
    {
        if (empleadoId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(empleadoId));

        if (fechaFinLocal.Date < fechaInicioLocal.Date ||
            (fechaFinLocal.Date - fechaInicioLocal.Date)
                .TotalDays > 365)
        {
            throw new ArgumentException(
                "El periodo de consulta debe estar entre 1 y 366 días.");
        }

        var movimientos =
            new List<MovimientoAsistenciaNormalizado>();

        // Consultar secuencialmente para evitar
        // operaciones simultáneas en el mismo DbContext.
        foreach (var proveedor in _proveedores)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var registros = await proveedor.ObtenerMovimientosAsync(
                empleadoId,
                fechaInicioLocal,
                fechaFinLocal,
                cancellationToken);

            movimientos.AddRange(registros);
        }

        // Dos movimientos que ocurren en el mismo
        // segundo no son necesariamente duplicados.
        // Se conserva cada registro con su identificador.
        var identificadoresDuplicados = movimientos
            .GroupBy(x => x.IdMovimiento)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToList();

        if (identificadoresDuplicados.Count > 0)
        {
            throw new InvalidOperationException(
                "Se detectaron identificadores duplicados: " +
                string.Join(", ", identificadoresDuplicados));
        }

        return movimientos
            .OrderBy(x => x.FechaHoraUtc)
            .ThenBy(x => x.Fuente)
            .ThenBy(x => x.IdMovimiento)
            .ToList();
    }
}
