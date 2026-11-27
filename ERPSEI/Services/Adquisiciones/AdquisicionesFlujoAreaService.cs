using ERPSEI.Data;
using ERPSEI.Data.Entities.Adquisiciones;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace ERPSEI.Services.Adquisiciones
{
    public sealed class AdquisicionesFlujoAreaService :
        IAdquisicionesFlujoAreaService
    {
        private readonly ApplicationDbContext _context;

        private readonly IAdquisicionesAlcanceService
            _alcanceService;

        private static readonly int[] OrdenesRequeridos =
        {
            1, 2, 3, 4
        };

        public AdquisicionesFlujoAreaService(
            ApplicationDbContext context,
            IAdquisicionesAlcanceService alcanceService)
        {
            _context = context;
            _alcanceService = alcanceService;
        }

        // =====================================================
        // OBTENER CONFIGURACIÓN POR ÁREA
        // =====================================================

        public async Task<AdquisicionesConfiguracionAreaDto>
            ObtenerConfiguracionAsync(
                ClaimsPrincipal principal,
                int areaId,
                CancellationToken cancellationToken = default)
        {
            var resultado =
                new AdquisicionesConfiguracionAreaDto
                {
                    AreaId = areaId
                };

            var alcance =
                await _alcanceService.ObtenerAlcanceAsync(
                    principal,
                    cancellationToken
                );

            if (!alcance.UsuarioValido ||
                !alcance.TieneAlcanceArea(areaId))
            {
                resultado.Mensaje =
                    "No tienes autorización sobre esta área.";

                return resultado;
            }

            // La configuración de firmantes se reserva
            // a los administradores globales.

            if (!alcance.TieneAlcanceGlobal)
            {
                resultado.Mensaje =
                    "No tienes autorización para administrar firmantes.";

                return resultado;
            }

            resultado.UsuarioAutorizado = true;

            var area =
                await _context.Areas
                    .AsNoTracking()
                    .Where(x => x.Id == areaId)
                    .Select(x => new
                    {
                        x.Id,
                        x.Nombre
                    })
                    .FirstOrDefaultAsync(
                        cancellationToken
                    );

            if (area == null)
            {
                resultado.Mensaje =
                    "El área no existe.";

                return resultado;
            }

            resultado.AreaExiste = true;

            resultado.NombreArea =
                area.Nombre;

            resultado.PuedeConfigurar =
                alcance.TieneAlcanceGlobal;

            var configuracion =
                await ConsultarConfiguracionAsync(
                    areaId,
                    cancellationToken
                );

            var idsUsuarios =
                configuracion
                    .Select(x => x.UsuarioResponsableId)
                    .Concat(
                        configuracion.Select(
                            x => x.UsuarioAsistenteId
                        )
                    )
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x)
                    )
                    .Select(x => x!)
                    .Distinct()
                    .ToList();

            var usuarios =
                await _context.Users
                    .AsNoTracking()
                    .Where(x =>
                        idsUsuarios.Contains(x.Id) &&
                        !x.IsBanned
                    )
                    .ToDictionaryAsync(
                        x => x.Id,
                        x => x.UserName ??
                             x.Email ??
                             x.Id,
                        cancellationToken
                    );

            foreach (int orden in OrdenesRequeridos)
            {
                var etapa =
                    configuracion.FirstOrDefault(
                        x => x.Orden == orden
                    );

                if (etapa == null)
                {
                    resultado.Etapas.Add(
                        new AdquisicionesNivelAprobacionDto
                        {
                            Orden = orden,

                            TipoEtapa =
                                $"Nivel{orden}",

                            NombreEtapa =
                                $"Aprobación nivel {orden}",

                            Activo = false
                        }
                    );

                    continue;
                }

                usuarios.TryGetValue(
                    etapa.UsuarioResponsableId,
                    out string? nombreResponsable
                );

                string? nombreAsistente = null;

                if (!string.IsNullOrWhiteSpace(
                    etapa.UsuarioAsistenteId))
                {
                    usuarios.TryGetValue(
                        etapa.UsuarioAsistenteId,
                        out nombreAsistente
                    );
                }

                resultado.Etapas.Add(
                    new AdquisicionesNivelAprobacionDto
                    {
                        Id = etapa.Id,

                        Orden = etapa.Orden,

                        TipoEtapa = etapa.TipoEtapa,

                        NombreEtapa = etapa.NombreEtapa,

                        UsuarioResponsableId =
                            etapa.UsuarioResponsableId,

                        NombreResponsable =
                            nombreResponsable,

                        UsuarioAsistenteId =
                            etapa.UsuarioAsistenteId,

                        NombreAsistente =
                            nombreAsistente,

                        AsistenteRecibeCopia =
                            etapa.AsistenteRecibeCopia,

                        RecibirCopiaDesdeOrden =
                            etapa.RecibirCopiaDesdeOrden,

                        Activo = etapa.Activo
                    }
                );
            }

            resultado.ConfiguracionCompleta =
                await ValidarConfiguracionAsync(
                    configuracion,
                    cancellationToken
                );

            resultado.Mensaje =
                resultado.ConfiguracionCompleta
                    ? "Configuración completa."
                    : "El área requiere configurar sus cuatro niveles de aprobación.";

            return resultado;
        }

        // =====================================================
        // OBTENER FLUJO VIGENTE DE UNA SOLICITUD
        // =====================================================

        public async Task<
            List<AdqConfiguracionAprobacionPresupuestal>>
            ObtenerFlujoVigenteAsync(
                ClaimsPrincipal principal,
                int solicitudId,
                CancellationToken cancellationToken = default)
        {
            var acceso =
                await _alcanceService
                    .ObtenerAccesoSolicitudAsync(
                        principal,
                        solicitudId,
                        cancellationToken
                    );

            if (!acceso.PuedeGestionar)
            {
                throw new UnauthorizedAccessException(
                    "No tienes autorización para gestionar esta solicitud."
                );
            }

            int areaId =
                await _context.AdqSolicitudes
                    .AsNoTracking()
                    .Where(x =>
                        x.Id == solicitudId &&
                        !x.Eliminado
                    )
                    .Select(x => x.AreaId)
                    .FirstOrDefaultAsync(
                        cancellationToken
                    );

            if (areaId <= 0)
            {
                throw new InvalidOperationException(
                    "La solicitud no tiene un área válida."
                );
            }

            var configuracion =
                await ConsultarConfiguracionAsync(
                    areaId,
                    cancellationToken
                );

            bool configuracionValida =
                await ValidarConfiguracionAsync(
                    configuracion,
                    cancellationToken
                );

            if (!configuracionValida)
            {
                throw new InvalidOperationException(
                    "El área de la solicitud no cuenta con cuatro niveles de aprobación válidos."
                );
            }

            return configuracion;
        }

        // =====================================================
        // VALIDAR CONFIGURACIÓN COMPLETA
        // =====================================================

        public async Task<bool> ConfiguracionCompletaAsync(
            ClaimsPrincipal principal,
            int solicitudId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var configuracion =
                    await ObtenerFlujoVigenteAsync(
                        principal,
                        solicitudId,
                        cancellationToken
                    );

                return configuracion.Count == 4;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        // =====================================================
        // GUARDAR CONFIGURACIÓN POR ÁREA
        // =====================================================

        public async Task GuardarConfiguracionAsync(
            ClaimsPrincipal principal,
            int areaId,
            IReadOnlyCollection<
                AdquisicionesNivelAprobacionDto> etapas,
            CancellationToken cancellationToken = default)
        {
            var alcance =
                await _alcanceService
                    .ObtenerAlcanceAsync(
                        principal,
                        cancellationToken
                    );

            if (!alcance.UsuarioValido ||
                !alcance.TieneAlcanceGlobal)
            {
                throw new UnauthorizedAccessException(
                    "No tienes autorización para configurar los aprobadores."
                );
            }

            if (areaId <= 0 ||
                etapas == null ||
                etapas.Count != 4)
            {
                throw new InvalidOperationException(
                    "Debes configurar exactamente cuatro niveles."
                );
            }

            var ordenes =
                etapas
                    .Select(x => x.Orden)
                    .OrderBy(x => x)
                    .ToArray();

            if (!ordenes.SequenceEqual(
                OrdenesRequeridos))
            {
                throw new InvalidOperationException(
                    "Los niveles deben corresponder al orden 1, 2, 3 y 4."
                );
            }

            bool areaExiste =
                await _context.Areas.AnyAsync(
                    x => x.Id == areaId,
                    cancellationToken
                );

            if (!areaExiste)
            {
                throw new InvalidOperationException(
                    "El área seleccionada no existe."
                );
            }

            foreach (var etapa in etapas)
            {
                etapa.UsuarioResponsableId =
                    etapa.UsuarioResponsableId?.Trim()
                    ?? string.Empty;

                etapa.UsuarioAsistenteId =
                    string.IsNullOrWhiteSpace(
                        etapa.UsuarioAsistenteId
                    )
                        ? null
                        : etapa.UsuarioAsistenteId.Trim();

                if (string.IsNullOrWhiteSpace(
                    etapa.UsuarioResponsableId))
                {
                    throw new InvalidOperationException(
                        $"El nivel {etapa.Orden} requiere un responsable."
                    );
                }

                if (etapa.AsistenteRecibeCopia &&
                    string.IsNullOrWhiteSpace(
                        etapa.UsuarioAsistenteId))
                {
                    throw new InvalidOperationException(
                        $"El nivel {etapa.Orden} requiere un asistente válido."
                    );
                }

                if (etapa.AsistenteRecibeCopia &&
                    (
                        !etapa.RecibirCopiaDesdeOrden.HasValue ||
                        etapa.RecibirCopiaDesdeOrden.Value < 1 ||
                        etapa.RecibirCopiaDesdeOrden.Value > 4
                    ))
                {
                    throw new InvalidOperationException(
                        $"El orden de recepción de copia del nivel {etapa.Orden} no es válido."
                    );
                }
            }

            var responsables =
                etapas
                    .Select(x => x.UsuarioResponsableId)
                    .Distinct()
                    .ToList();

            if (responsables.Count != 4)
            {
                throw new InvalidOperationException(
                    "Cada nivel debe contar con un responsable diferente."
                );
            }

            var idsUsuarios =
                etapas
                    .Select(x => x.UsuarioResponsableId)
                    .Concat(
                        etapas.Select(
                            x => x.UsuarioAsistenteId
                        )
                    )
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x)
                    )
                    .Select(x => x!)
                    .Distinct()
                    .ToList();

            var usuariosActivos =
                await _context.Users
                    .AsNoTracking()
                    .Where(x =>
                        idsUsuarios.Contains(x.Id) &&
                        !x.IsBanned
                    )
                    .Select(x => x.Id)
                    .ToListAsync(
                        cancellationToken
                    );

            if (idsUsuarios.Except(
                usuariosActivos
            ).Any())
            {
                throw new InvalidOperationException(
                    "Uno o más responsables o asistentes no se encuentran activos."
                );
            }

            await using var transaccion =
                await _context.Database
                    .BeginTransactionAsync(
                        IsolationLevel.Serializable,
                        cancellationToken
                    );

            try
            {
                var existentes =
                    await _context
                        .AdqConfiguracionAprobacionPresupuestal
                        .Where(x =>
                            EF.Property<int?>(
                                x,
                                "AreaId"
                            ) == areaId &&
                            !x.Eliminado
                        )
                        .ToListAsync(
                            cancellationToken
                        );

                var ahora =
                    DateTime.UtcNow;

                foreach (var entrada in
                    etapas.OrderBy(x => x.Orden))
                {
                    var registro =
                        existentes.SingleOrDefault(
                            x => x.Orden == entrada.Orden
                        );

                    if (registro == null)
                    {
                        registro =
                            new AdqConfiguracionAprobacionPresupuestal
                            {
                                Orden = entrada.Orden,

                                FechaCreacion = ahora,

                                Eliminado = false
                            };

                        _context
                            .AdqConfiguracionAprobacionPresupuestal
                            .Add(registro);

                        _context
                            .Entry(registro)
                            .Property<int?>("AreaId")
                            .CurrentValue = areaId;
                    }

                    registro.TipoEtapa =
                        $"Nivel{entrada.Orden}";

                    registro.NombreEtapa =
                        $"Aprobación nivel {entrada.Orden}";

                    registro.UsuarioResponsableId =
                        entrada.UsuarioResponsableId;

                    registro.UsuarioAsistenteId =
                        entrada.UsuarioAsistenteId;

                    registro.AsistenteRecibeCopia =
                        entrada.AsistenteRecibeCopia;

                    registro.RecibirCopiaDesdeOrden =
                        entrada.AsistenteRecibeCopia
                            ? entrada.RecibirCopiaDesdeOrden
                            : null;

                    registro.Activo = true;

                    registro.FechaModificacion = ahora;

                    registro.UsuarioModificacionId =
                        alcance.UsuarioId;

                    registro.Eliminado = false;
                }

                await _context.SaveChangesAsync(
                    cancellationToken
                );

                await transaccion.CommitAsync(
                    cancellationToken
                );
            }
            catch
            {
                await transaccion.RollbackAsync(
                    CancellationToken.None
                );

                throw;
            }
        }

        // =====================================================
        // CONSULTA PRIVADA POR ÁREA
        // =====================================================

        private async Task<
            List<AdqConfiguracionAprobacionPresupuestal>>
            ConsultarConfiguracionAsync(
                int areaId,
                CancellationToken cancellationToken)
        {
            return await _context
                .AdqConfiguracionAprobacionPresupuestal
                .AsNoTracking()
                .Where(x =>
                    EF.Property<int?>(
                        x,
                        "AreaId"
                    ) == areaId &&
                    x.Activo &&
                    !x.Eliminado
                )
                .OrderBy(x => x.Orden)
                .ToListAsync(
                    cancellationToken
                );
        }

        // =====================================================
        // VALIDACIÓN INTERNA DEL FLUJO
        // =====================================================

        private async Task<bool>
            ValidarConfiguracionAsync(
                List<
                    AdqConfiguracionAprobacionPresupuestal>
                    configuracion,
                CancellationToken cancellationToken)
        {
            if (configuracion.Count != 4)
            {
                return false;
            }

            var ordenes =
                configuracion
                    .Select(x => x.Orden)
                    .OrderBy(x => x)
                    .ToArray();

            if (!ordenes.SequenceEqual(
                OrdenesRequeridos))
            {
                return false;
            }

            if (configuracion.Any(x =>
                string.IsNullOrWhiteSpace(
                    x.UsuarioResponsableId
                )
                ||
                (
                    x.AsistenteRecibeCopia &&
                    (
                        string.IsNullOrWhiteSpace(
                            x.UsuarioAsistenteId
                        ) ||
                        !x.RecibirCopiaDesdeOrden.HasValue ||
                        x.RecibirCopiaDesdeOrden.Value < 1 ||
                        x.RecibirCopiaDesdeOrden.Value > 4
                    )
                )))
            {
                return false;
            }

            if (configuracion
                .Select(x => x.UsuarioResponsableId)
                .Distinct()
                .Count() != 4)
            {
                return false;
            }

            var idsUsuarios =
                configuracion
                    .Select(x => x.UsuarioResponsableId)
                    .Concat(
                        configuracion.Select(
                            x => x.UsuarioAsistenteId
                        )
                    )
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x)
                    )
                    .Select(x => x!)
                    .Distinct()
                    .ToList();

            var totalActivos =
                await _context.Users
                    .AsNoTracking()
                    .CountAsync(
                        x =>
                            idsUsuarios.Contains(x.Id) &&
                            !x.IsBanned,
                        cancellationToken
                    );

            return totalActivos == idsUsuarios.Count;
        }
    }
}