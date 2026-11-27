using ERPSEI.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ERPSEI.Services.Adquisiciones
{
    public sealed class AdquisicionesControlCentralService :
        IAdquisicionesControlCentralService
    {
        private readonly ApplicationDbContext _context;

        private readonly IAdquisicionesAlcanceService
            _alcanceService;

        public AdquisicionesControlCentralService(
            ApplicationDbContext context,
            IAdquisicionesAlcanceService alcanceService)
        {
            _context = context;
            _alcanceService = alcanceService;
        }

        // =====================================================
        // CENTRO DE CONTROL DE SOLICITUDES DE PAGO
        // =====================================================

        public async Task<
            List<AdquisicionesSolicitudPagoCentralDto>>
            ObtenerSolicitudesPagoAsync(
                ClaimsPrincipal principal,
                int? areaId = null,
                bool soloAutorizadas = false,
                CancellationToken cancellationToken = default)
        {
            var alcance =
                await _alcanceService.ObtenerAlcanceAsync(
                    principal,
                    cancellationToken
                );

            if (!alcance.UsuarioValido ||
                !alcance.TieneAlcanceGlobal)
            {
                throw new UnauthorizedAccessException(
                    "No tienes autorización para consultar el centro de control."
                );
            }

            var permiso =
                await _context.AdqPermisosUsuarios
                    .AsNoTracking()
                    .Where(x =>
                        x.UsuarioId == alcance.UsuarioId
                    )
                    .Select(x => new
                    {
                        x.PuedeVisualizar,
                        x.PuedeDescargar,
                        x.PuedeTodo
                    })
                    .FirstOrDefaultAsync(
                        cancellationToken
                    );

            bool puedeVisualizar =
                alcance.EsAdministradorSistema ||
                permiso?.PuedeTodo == true ||
                permiso?.PuedeVisualizar == true;

            bool puedeDescargar =
                alcance.EsAdministradorSistema ||
                permiso?.PuedeTodo == true ||
                permiso?.PuedeDescargar == true;

            if (!puedeVisualizar)
            {
                throw new UnauthorizedAccessException(
                    "No tienes permiso de visualización."
                );
            }

            var consulta =
                from pago in
                    _context.AdqSolicitudesPago
                        .AsNoTracking()

                join solicitud in
                    _context.AdqSolicitudes
                        .AsNoTracking()

                    on pago.SolicitudId
                    equals solicitud.Id

                join area in
                    _context.Areas.AsNoTracking()

                    on solicitud.AreaId
                    equals area.Id

                where
                    !pago.Eliminado &&
                    !solicitud.Eliminado

                select new
                {
                    Pago = pago,

                    Solicitud = solicitud,

                    Area = area
                };

            if (areaId.HasValue)
            {
                if (areaId.Value <= 0)
                {
                    throw new ArgumentException(
                        "El identificador del área no es válido."
                    );
                }

                consulta =
                    consulta.Where(x =>
                        x.Solicitud.AreaId ==
                            areaId.Value
                    );
            }

            var registros =
                await consulta
                    .OrderByDescending(
                        x => x.Pago.FechaGeneracion
                    )
                    .Select(x =>
                        new AdquisicionesSolicitudPagoCentralDto
                        {
                            SolicitudId =
                                x.Solicitud.Id,

                            SolicitudPagoId =
                                x.Pago.Id,

                            AreaId =
                                x.Area.Id,

                            Area =
                                x.Area.Nombre,

                            Folio =
                                x.Solicitud.Folio,

                            Titulo =
                                x.Solicitud.Titulo,

                            NombreProveedor =
                                x.Pago.NombreProveedor,

                            Moneda =
                                x.Pago.Moneda,

                            FormaPago =
                                x.Pago.FormaPago,

                            Total =
                                x.Pago.Total,

                            EstatusId =
                                x.Solicitud.EstatusId,

                            Estatus =
                                _context.AdqEstatus
                                    .Where(e =>
                                        e.Id ==
                                            x.Solicitud.EstatusId
                                    )
                                    .Select(e =>
                                        e.Nombre
                                    )
                                    .FirstOrDefault()
                                ?? string.Empty,

                            FechaSolicitud =
                                x.Pago.FechaSolicitud,

                            FechaGeneracion =
                                x.Pago.FechaGeneracion,

                            PresupuestoAprobado =
                                x.Pago.AprobacionPresupuestalId != null
                                &&
                                _context
                                    .AdqAprobacionesPresupuestales
                                    .Any(a =>
                                        a.Id ==
                                            x.Pago.AprobacionPresupuestalId
                                        &&
                                        a.SolicitudId ==
                                            x.Solicitud.Id
                                        &&
                                        !a.Eliminado
                                        &&
                                        a.Estatus ==
                                            "Aprobado"
                                    ),

                            PdfGenerado =
                                x.Pago.PdfGenerado,

                            NombreArchivo =
                                x.Pago.NombreArchivo,

                            UsuarioPuedeDescargar =
                                puedeDescargar,

                            TotalFirmas =
                                0,

                            TotalParcialidades =
                                _context
                                    .AdqSolicitudesPagoParcialidades
                                    .Count(p =>
                                        p.SolicitudPagoId ==
                                            x.Pago.Id &&
                                        !p.Eliminado
                                    ),

                            ParcialidadesPagadas =
                                _context
                                    .AdqSolicitudesPagoParcialidades
                                    .Count(p =>
                                        p.SolicitudPagoId ==
                                            x.Pago.Id &&
                                        !p.Eliminado &&
                                        p.Estatus ==
                                            "Pagada"
                                    ),

                            ImportePagado =
                                _context
                                    .AdqSolicitudesPagoParcialidades
                                    .Where(p =>
                                        p.SolicitudPagoId ==
                                            x.Pago.Id &&
                                        !p.Eliminado &&
                                        p.Estatus ==
                                            "Pagada"
                                    )
                                    .Sum(p =>
                                        (decimal?)p.Importe
                                    ) ?? 0m
                        }
                    )
                    .ToListAsync(
                        cancellationToken
                    );

            // La cantidad de firmas se mantendrá en cero
            // hasta integrar la comprobación de los
            // registros reales de firma presupuestal.
            //
            // No se considerará una aprobación como firma
            // solamente por tener estatus aprobado.

            if (soloAutorizadas)
            {
                registros =
                    registros
                        .Where(x =>
                            x.ListaParaProcesar
                        )
                        .ToList();
            }

            return registros;
        }

        // =====================================================
        // AUTORIZACIÓN PARA PROCESAR PAGO
        // =====================================================

        public async Task<bool> PuedeProcesarPagoAsync(
            ClaimsPrincipal principal,
            int solicitudId,
            CancellationToken cancellationToken = default)
        {
            if (solicitudId <= 0)
            {
                return false;
            }

            var alcance =
                await _alcanceService
                    .ObtenerAlcanceAsync(
                        principal,
                        cancellationToken
                    );

            if (!alcance.UsuarioValido ||
                !alcance.TieneAlcanceGlobal)
            {
                return false;
            }

            var acceso =
                await _alcanceService
                    .ObtenerAccesoSolicitudAsync(
                        principal,
                        solicitudId,
                        cancellationToken
                    );

            if (!acceso.PuedeGestionar)
            {
                return false;
            }

            // Denegación preventiva hasta integrar
            // la validación de las cuatro firmas reales,
            // integridad del PDF y estatus de pago.

            return false;
        }
    }
}