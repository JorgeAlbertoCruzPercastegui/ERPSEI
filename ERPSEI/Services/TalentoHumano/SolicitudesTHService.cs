
using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERPSEI.Data;
using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Entities.TalentoHumano;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Hosting;

namespace ERPSEI.Services.TalentoHumano
{
    internal sealed class PruebaRollbackTHException : Exception
    {
        public PruebaRollbackTHException()
            : base("Fallo controlado después de SaveChangesAsync.")
        {
        }
    }

    public sealed class SolicitudesTHService : ISolicitudesTHService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        private static readonly TimeZoneInfo ZonaHorariaMexico =
            ObtenerZonaHorariaMexico();

        public SolicitudesTHService(
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // =========================================================
        // OPERACIÓN NORMAL - UTILIZADA POR LA INTRANET
        // =========================================================

        public Task<THSolicitud> CrearSolicitudAsync(
            TipoSolicitudTH tipo,
            ClaimsPrincipal usuario,
            CancellationToken cancellationToken = default)
        {
            return CrearSolicitudInternaAsync(
                tipo,
                usuario,
                false,
                cancellationToken);
        }

        // =========================================================
        // OPERACIÓN EXCLUSIVA PARA PROBAR ROLLBACK
        // No está expuesta en la interfaz pública del módulo.
        // =========================================================

        internal Task<THSolicitud> CrearSolicitudParaPruebaRollbackAsync(
            TipoSolicitudTH tipo,
            ClaimsPrincipal usuario,
            CancellationToken cancellationToken = default)
        {
            if (!_environment.IsDevelopment())
            {
                throw new InvalidOperationException(
                    "La prueba de rollback solo está permitida en Development.");
            }

            return CrearSolicitudInternaAsync(
                tipo,
                usuario,
                true,
                cancellationToken);
        }

        // =========================================================
        // VALIDACIONES REUTILIZABLES
        // =========================================================

        internal static void ValidarCuentaHabilitada(bool isBanned)
        {
            if (isBanned)
            {
                throw new UnauthorizedAccessException(
                    "La cuenta no se encuentra habilitada.");
            }
        }

        internal static void ValidarEmpleadoActivo(Empleado empleado)
        {
            if (empleado == null)
            {
                throw new InvalidOperationException(
                    "El empleado no existe.");
            }

            if (empleado.Deshabilitado != 0)
            {
                throw new InvalidOperationException(
                    "El empleado no se encuentra activo.");
            }
        }

        // =========================================================
        // MOTOR TRANSACCIONAL
        // =========================================================

        private async Task<THSolicitud> CrearSolicitudInternaAsync(
            TipoSolicitudTH tipo,
            ClaimsPrincipal usuario,
            bool provocarFalloDespuesDeGuardar,
            CancellationToken cancellationToken)
        {
            if (provocarFalloDespuesDeGuardar &&
                !_environment.IsDevelopment())
            {
                throw new InvalidOperationException(
                    "La prueba no está habilitada en este ambiente.");
            }

            // 1. AUTENTICACIÓN

            if (usuario?.Identity?.IsAuthenticated != true)
            {
                throw new UnauthorizedAccessException(
                    "Es necesario iniciar sesión.");
            }

            string? usuarioId = usuario.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(usuarioId))
            {
                throw new UnauthorizedAccessException(
                    "No se pudo identificar al usuario.");
            }

            // 2. TIPO DE SOLICITUD

            if (!Enum.IsDefined(typeof(TipoSolicitudTH), tipo))
            {
                throw new ArgumentException(
                    "El tipo de solicitud no es válido.",
                    nameof(tipo));
            }

            var definicion =
                CatalogoSolicitudesTH.ObtenerPorTipo(tipo);

            // 3. CUENTA DEL USUARIO

            var cuenta = await _context.Users
                .AsNoTracking()
                .Where(x => x.Id == usuarioId)
                .Select(x => new
                {
                    x.Id,
                    x.EmpleadoId,
                    x.IsBanned
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (cuenta == null)
            {
                throw new UnauthorizedAccessException(
                    "La cuenta del usuario no existe.");
            }

            ValidarCuentaHabilitada(cuenta.IsBanned);

            // 4. EMPLEADO VINCULADO

            Empleado? empleado;

            if (cuenta.EmpleadoId.HasValue)
            {
                empleado = await _context.Empleados
                    .AsNoTracking()
                    .SingleOrDefaultAsync(
                        x => x.Id == cuenta.EmpleadoId.Value,
                        cancellationToken);
            }
            else
            {
                empleado = await _context.Empleados
                    .AsNoTracking()
                    .SingleOrDefaultAsync(
                        x => x.UserId == usuarioId,
                        cancellationToken);
            }

            if (empleado == null)
            {
                throw new InvalidOperationException(
                    "El usuario no tiene un empleado asociado.");
            }

            ValidarEmpleadoActivo(empleado);

            if (!string.IsNullOrWhiteSpace(empleado.UserId)
                && empleado.UserId != usuarioId)
            {
                throw new InvalidOperationException(
                    "La vinculación del empleado no coincide con la cuenta.");
            }

            if (cuenta.EmpleadoId.HasValue
                && cuenta.EmpleadoId.Value != empleado.Id)
            {
                throw new InvalidOperationException(
                    "La vinculación del empleado es inconsistente.");
            }

            // 5. NOMBRE DEL SOLICITANTE

            string nombreCompleto = empleado.NombreCompleto;

            if (string.IsNullOrWhiteSpace(nombreCompleto))
            {
                nombreCompleto = string.Join(
                    " ",
                    new[]
                    {
                        empleado.Nombre,
                        empleado.ApellidoPaterno,
                        empleado.ApellidoMaterno
                    }.Where(x => !string.IsNullOrWhiteSpace(x)));
            }

            if (string.IsNullOrWhiteSpace(nombreCompleto))
            {
                throw new InvalidOperationException(
                    "El empleado no tiene un nombre válido.");
            }

            // 6. FECHA Y AÑO DEL FOLIO

            DateTime ahoraUtc = DateTime.UtcNow;

            DateTime ahoraMexico =
                TimeZoneInfo.ConvertTimeFromUtc(
                    ahoraUtc,
                    ZonaHorariaMexico);

            int anio = ahoraMexico.Year;

            // 7. INICIAR TRANSACCIÓN

            await using var transaccion =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.ReadCommitted,
                    cancellationToken);

            // 8. BLOQUEAR CONSECUTIVO POR TIPO Y AÑO

            await AdquirirBloqueoConsecutivoAsync(
                tipo,
                anio,
                transaccion,
                cancellationToken);

            // 9. OBTENER SIGUIENTE CONSECUTIVO

            var registro = await _context.THFolioConsecutivos
                .SingleOrDefaultAsync(
                    x => x.Tipo == tipo && x.Anio == anio,
                    cancellationToken);

            int siguiente;

            if (registro == null)
            {
                siguiente = 1;

                registro = new THFolioConsecutivo
                {
                    Tipo = tipo,
                    Anio = anio,
                    UltimoConsecutivo = siguiente,
                    FechaActualizacionUtc = ahoraUtc
                };

                _context.THFolioConsecutivos.Add(registro);
            }
            else
            {
                if (registro.UltimoConsecutivo == int.MaxValue)
                {
                    throw new InvalidOperationException(
                        "Se alcanzó el límite de consecutivos.");
                }

                siguiente = registro.UltimoConsecutivo + 1;

                registro.UltimoConsecutivo = siguiente;
                registro.FechaActualizacionUtc = ahoraUtc;
            }

            // 10. GENERAR FOLIO

            string folio =
                $"SOL-{definicion.CodigoFolio}-{anio}-{siguiente:D4}";

            if (folio.Length > 40)
            {
                throw new InvalidOperationException(
                    "El folio supera la longitud permitida.");
            }

            // 11. CREAR SOLICITUD

            var solicitud = new THSolicitud
            {
                Folio = folio,
                Tipo = tipo,
                Estatus = EstatusSolicitudTH.Enviada,

                EmpleadoId = empleado.Id,
                SolicitanteUserId = usuarioId,

                NumeroEmpleado = empleado.Id.ToString(),
                NombreSolicitante = nombreCompleto,

                AreaSolicitante = null,
                OficinaSolicitante = null,

                FechaEnvioUtc = ahoraUtc,
                FechaActualizacionUtc = ahoraUtc,

                RespondidaAutomaticamente = false
            };

            _context.THSolicitudes.Add(solicitud);

            // 12. HISTORIAL INICIAL

            var historial = new THHistorialSolicitud
            {
                Solicitud = solicitud,
                UsuarioId = usuarioId,

                RolEjecutor = "Solicitante",
                Accion = "Creación de solicitud",

                EstatusAnterior = null,
                EstatusNuevo = EstatusSolicitudTH.Enviada,

                Comentario =
                    "Solicitud registrada en el portal de Talento Humano.",

                FechaHoraUtc = ahoraUtc,
                EjecutadoPorSistema = false,
                IpOrigen = null
            };

            _context.THHistorialSolicitudes.Add(historial);

            // 13. GUARDAR LOS CAMBIOS EN LA TRANSACCIÓN

            await _context.SaveChangesAsync(cancellationToken);

            // 14. FALLO CONTROLADO: SOLO DEVELOPMENT
            //
            // SaveChangesAsync ya ejecutó los INSERT/UPDATE,
            // pero aún NO se ha ejecutado CommitAsync.
            //
            // Al lanzar la excepción, se dispone la transacción
            // sin confirmar y SQL Server revierte los cambios.

            if (provocarFalloDespuesDeGuardar)
            {
                throw new PruebaRollbackTHException();
            }

            // 15. CONFIRMAR OPERACIÓN NORMAL

            await transaccion.CommitAsync(cancellationToken);

            return solicitud;
        }

        // =========================================================
        // BLOQUEO DE CONSECUTIVOS EN SQL SERVER
        // =========================================================

        private async Task AdquirirBloqueoConsecutivoAsync(
            TipoSolicitudTH tipo,
            int anio,
            IDbContextTransaction transaccion,
            CancellationToken cancellationToken)
        {
            string recurso = $"TH-FOLIO-{(int)tipo}-{anio}";

            DbConnection conexion =
                _context.Database.GetDbConnection();

            await using DbCommand comando =
                conexion.CreateCommand();

            comando.Transaction =
                transaccion.GetDbTransaction();

            comando.CommandText = @"
                DECLARE @Resultado int;

                EXEC @Resultado = sys.sp_getapplock
                    @Resource = @Recurso,
                    @LockMode = 'Exclusive',
                    @LockOwner = 'Transaction',
                    @LockTimeout = 15000;

                SELECT @Resultado;";

            DbParameter parametro =
                comando.CreateParameter();

            parametro.ParameterName = "@Recurso";
            parametro.DbType = DbType.String;
            parametro.Size = 255;
            parametro.Value = recurso;

            comando.Parameters.Add(parametro);

            object? resultado =
                await comando.ExecuteScalarAsync(cancellationToken);

            int codigo = Convert.ToInt32(resultado);

            if (codigo < 0)
            {
                throw new InvalidOperationException(
                    $"No se pudo reservar el folio. Código: {codigo}.");
            }
        }

        private static TimeZoneInfo ObtenerZonaHorariaMexico()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(
                    "America/Mexico_City");
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.FindSystemTimeZoneById(
                    "Central Standard Time (Mexico)");
            }
        }
    }
}