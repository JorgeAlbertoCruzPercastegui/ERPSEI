
using ERPSEI.Data;
using ERPSEI.Data.Entities.Reportes;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Services.RelojChecador
{
    public sealed class RelojHorariosService
    {
        private readonly ApplicationDbContext _db;

        public RelojHorariosService(ApplicationDbContext db)
        {
            _db = db;
        }

        public sealed class HorarioEmpleadoResultado
        {
            public bool Configurado { get; init; }
            public bool DiaLaborable { get; init; }
            public string Codigo { get; init; } = "";
            public string Mensaje { get; init; } = "";

            public int EmpleadoId { get; init; }
            public int? HorarioId { get; init; }
            public int? HorarioDetalleId { get; init; }
            public string? Descripcion { get; init; }

            public DateTime FechaLocal { get; init; }
            public int NumeroDiaSemana { get; init; }

            public TimeSpan? Entrada { get; init; }
            public TimeSpan? ToleranciaEntrada { get; init; }
            public TimeSpan? ToleranciaFalta { get; init; }
            public TimeSpan? Salida { get; init; }
            public TimeSpan? ToleranciaSalida { get; init; }
        }

        public async Task<HorarioEmpleadoResultado>
            ObtenerHorarioEmpleadoAsync(
                int empleadoId,
                DateTime fechaLocal,
                CancellationToken cancellationToken = default)
        {
            if (empleadoId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(empleadoId));
            }

            DateTime fecha = fechaLocal.Date;

            // En HorariosDetalles:
            // Domingo=0, Lunes=1, ... Sábado=6.
            int numeroDiaSemana = (int)fecha.DayOfWeek;

            HorarioEmpleadoResultado SinHorario(
                string codigo,
                string mensaje,
                int? horarioId = null,
                string? descripcion = null,
                bool configurado = false)
            {
                return new HorarioEmpleadoResultado
                {
                    Configurado = configurado,
                    DiaLaborable = false,
                    Codigo = codigo,
                    Mensaje = mensaje,
                    EmpleadoId = empleadoId,
                    HorarioId = horarioId,
                    Descripcion = descripcion,
                    FechaLocal = fecha,
                    NumeroDiaSemana = numeroDiaSemana
                };
            }

            // ================================================
            // 1. OBTENER EMPLEADO Y SU HORARIO ASIGNADO
            // ================================================

            var empleado = await _db.Empleados
                .AsNoTracking()
                .Where(x => x.Id == empleadoId)
                .Select(x => new
                {
                    x.Id,
                    x.HorarioId,
                    x.CalcularAsistencia
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (empleado == null)
            {
                return SinHorario(
                    "EMPLEADO_NO_ENCONTRADO",
                    "El colaborador no existe.");
            }

            if (!empleado.HorarioId.HasValue ||
                empleado.HorarioId.Value <= 0)
            {
                return SinHorario(
                    "SIN_HORARIO",
                    "El colaborador no tiene horario asignado.");
            }

            // El indicador de cálculo de asistencia
            // debe respetarse como configuración de TH.
            if (empleado.CalcularAsistencia != true)
            {
                return SinHorario(
                    "ASISTENCIA_NO_HABILITADA",
                    "El colaborador no tiene habilitado el cálculo de asistencia.",
                    empleado.HorarioId);
            }

            // ================================================
            // 2. CONSULTAR EL CATÁLOGO EXISTENTE
            // ================================================

            var horario = await _db.Set<Horario>()
                .AsNoTracking()
                .Where(x => x.Id == empleado.HorarioId.Value)
                .Select(x => new
                {
                    x.Id,
                    x.Descripcion,
                    x.Deshabilitado
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (horario == null)
            {
                return SinHorario(
                    "HORARIO_NO_ENCONTRADO",
                    "El horario asignado no existe.",
                    empleado.HorarioId);
            }

            if (horario.Deshabilitado.GetValueOrDefault() != 0)
            {
                return SinHorario(
                    "HORARIO_DESHABILITADO",
                    "El horario asignado está deshabilitado.",
                    horario.Id,
                    horario.Descripcion);
            }

            // ================================================
            // 3. OBTENER EL DETALLE DEL DÍA
            // ================================================

            var detalles = await _db.Set<Horario>()
                .AsNoTracking()
                .Where(x => x.Id == horario.Id)
                .SelectMany(x => x.HorarioDetalles!
                    .Where(d =>
                        d.NumeroDiaSemana == numeroDiaSemana)
                    .Select(d => new
                    {
                        d.Id,
                        d.Activado,
                        d.Entrada,
                        d.ToleranciaEntrada,
                        d.ToleranciaFalta,
                        d.Salida,
                        d.ToleranciaSalida
                    }))
                .Take(2)
                .ToListAsync(cancellationToken);

            if (detalles.Count == 0)
            {
                return SinHorario(
                    "DIA_NO_CONFIGURADO",
                    "No existe configuración para este día de la semana.",
                    horario.Id,
                    horario.Descripcion);
            }

            if (detalles.Count > 1)
            {
                return SinHorario(
                    "DETALLES_DUPLICADOS",
                    "El horario contiene más de un detalle para el mismo día.",
                    horario.Id,
                    horario.Descripcion);
            }

            var detalle = detalles[0];

            if (!detalle.Activado)
            {
                return SinHorario(
                    "DIA_NO_LABORABLE",
                    "Este día está desactivado en el horario.",
                    horario.Id,
                    horario.Descripcion,
                    configurado: true);
            }

            // ================================================
            // 4. VALIDAR CONSISTENCIA
            // ================================================

            if (detalle.ToleranciaEntrada < detalle.Entrada ||
                detalle.ToleranciaFalta < detalle.ToleranciaEntrada ||
                detalle.Salida <= detalle.Entrada ||
                detalle.ToleranciaSalida > detalle.Salida)
            {
                return SinHorario(
                    "CONFIGURACION_INCONSISTENTE",
                    "Los límites de entrada, falta o salida son inconsistentes.",
                    horario.Id,
                    horario.Descripcion);
            }

            // ================================================
            // 5. DEVOLVER HORARIO APLICABLE
            // ================================================

            return new HorarioEmpleadoResultado
            {
                Configurado = true,
                DiaLaborable = true,
                Codigo = "OK",
                Mensaje = "Horario encontrado correctamente.",

                EmpleadoId = empleadoId,
                HorarioId = horario.Id,
                HorarioDetalleId = detalle.Id,
                Descripcion = horario.Descripcion,

                FechaLocal = fecha,
                NumeroDiaSemana = numeroDiaSemana,

                Entrada = detalle.Entrada,
                ToleranciaEntrada = detalle.ToleranciaEntrada,
                ToleranciaFalta = detalle.ToleranciaFalta,
                Salida = detalle.Salida,
                ToleranciaSalida = detalle.ToleranciaSalida
            };
        }
    }
}
