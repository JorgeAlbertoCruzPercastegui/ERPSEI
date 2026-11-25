using ERPSEI.Services.RelojChecador.Models;

namespace ERPSEI.Services.RelojChecador;

public interface IProveedorMovimientosAsistencia
{
    string Fuente { get; }

    Task<IReadOnlyList<MovimientoAsistenciaNormalizado>>
        ObtenerMovimientosAsync(
            int empleadoId,
            DateTime fechaInicioLocal,
            DateTime fechaFinLocal,
            CancellationToken cancellationToken = default);
}
