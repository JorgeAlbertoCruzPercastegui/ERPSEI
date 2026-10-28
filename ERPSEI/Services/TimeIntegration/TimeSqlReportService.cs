
using ERPSEI.Data;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Services.TimeIntegration
{
    public sealed class TimeSqlReportService
        : ITimeSqlReportService
    {
        private readonly ApplicationDbContext _db;

        public TimeSqlReportService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<TimeReporteHorasResultado>
            ConsultarHorasAsync(
                TimeReporteHorasFiltro filtro,
                CancellationToken cancellationToken = default)
        {
            if (filtro.Pagina < 1 ||
                filtro.TamanoPagina < 1 ||
                filtro.TamanoPagina > 100)
            {
                throw new ArgumentException(
                    "La paginación solicitada es inválida.");
            }

            if (filtro.FechaInicio.HasValue &&
                filtro.FechaFin.HasValue &&
                filtro.FechaInicio.Value.Date >
                filtro.FechaFin.Value.Date)
            {
                throw new ArgumentException(
                    "La fecha inicial no puede superar la fecha final.");
            }

            // Relaciones entre tablas sincronizadas de TIME.
            var consulta =
                from h in _db.TimeHoras.AsNoTracking()

                join a in _db.TimeActividades.AsNoTracking()
                    on h.ActividadId equals (long?)a.Id into ga
                from a in ga.DefaultIfEmpty()

                join u in _db.TimeUsuarios.AsNoTracking()
                    on a.UsuarioId equals (long?)u.Id into gu
                from u in gu.DefaultIfEmpty()

                join c in _db.TimeClaves.AsNoTracking()
                    on a.ClaveId equals (long?)c.Id into gc
                from c in gc.DefaultIfEmpty()

                join d in _db.TimeDepartamentos.AsNoTracking()
                    on u.DepartamentoId equals (long?)d.Id into gd
                from d in gd.DefaultIfEmpty()

                join e in _db.TimeEmpresas.AsNoTracking()
                    on c.EmpresaId equals (long?)e.Id into ge
                from e in ge.DefaultIfEmpty()

                join g in _db.TimeGrupos.AsNoTracking()
                    on e.GrupoId equals (long?)g.Id into gg
                from g in gg.DefaultIfEmpty()

                select new
                {
                    Hora = h,
                    Actividad = a,
                    Usuario = u,
                    Clave = c,
                    Departamento = d,
                    Empresa = e,
                    Grupo = g
                };

            // Los filtros se aplican antes de la paginación,
            // sobre todos los registros sincronizados.

            if (filtro.FechaInicio.HasValue)
            {
                DateTime inicio = filtro.FechaInicio.Value.Date;

                consulta = consulta.Where(x =>
                    x.Hora.Fecha >= inicio);
            }

            if (filtro.FechaFin.HasValue)
            {
                DateTime finExclusivo =
                    filtro.FechaFin.Value.Date.AddDays(1);

                consulta = consulta.Where(x =>
                    x.Hora.Fecha < finExclusivo);
            }

            if (filtro.UsuarioId.HasValue)
            {
                long id = filtro.UsuarioId.Value;

                consulta = consulta.Where(x =>
                    x.Actividad != null &&
                    x.Actividad.UsuarioId == id);
            }

            if (filtro.ProyectoId.HasValue)
            {
                long id = filtro.ProyectoId.Value;

                consulta = consulta.Where(x =>
                    x.Actividad != null &&
                    x.Actividad.ClaveId == id);
            }

            if (filtro.DepartamentoId.HasValue)
            {
                long id = filtro.DepartamentoId.Value;

                consulta = consulta.Where(x =>
                    x.Usuario != null &&
                    x.Usuario.DepartamentoId == id);
            }

            long total = await consulta.LongCountAsync(
                cancellationToken);

            long totalPaginasLong =
                total / filtro.TamanoPagina +
                (total % filtro.TamanoPagina == 0 ? 0 : 1);

            // Por la firma actual, el número de páginas
            // se representa como int.
            if (totalPaginasLong > int.MaxValue)
            {
                throw new InvalidOperationException(
                    "El reporte excede la paginación admitida.");
            }

            int totalPaginas = (int)totalPaginasLong;

            if (filtro.Pagina > Math.Max(totalPaginas, 1))
            {
                throw new ArgumentException(
                    "La página solicitada no existe.");
            }

            int omitir = checked(
                (filtro.Pagina - 1) * filtro.TamanoPagina);

            var registros = await consulta
                .OrderByDescending(x => x.Hora.Fecha)
                .ThenByDescending(x => x.Hora.Id)
                .Skip(omitir)
                .Take(filtro.TamanoPagina)
                .Select(x => new TimeReporteHorasFila
                {
                    Id = x.Hora.Id,

                    Nombre = x.Usuario != null
        ? x.Usuario.Nombre
        : null,

                    Area = x.Departamento != null
        ? x.Departamento.Nombre
        : null,

                    // Aún no tenemos fuentes confirmadas
                    // para estos cuatro campos.
                    Puesto = null,
                    NivelCliente = null,
                    GrupoEmpresa = x.Empresa != null && x.Grupo != null
                    ? (x.Grupo.Nombre ?? "") + " / " + (x.Empresa.Nombre ?? "")
                    : x.Empresa != null
                        ? x.Empresa.Nombre
                        : null,
                    InternaExterna = null,

                    // Claves.Tipo existe, pero necesitamos
                    // conocer sus equivalencias de negocio.
                    Tipo = x.Clave != null && x.Clave.Tipo.HasValue
        ? x.Clave.Tipo.Value.ToString()
        : null,

                    Fecha = x.Hora.Fecha,

                    // En el Excel la columna Actividad
                    // contiene el proyecto o clave de trabajo.
                    Actividad = x.Clave != null
        ? x.Clave.Proyecto
        : null,

                    // La descripción corresponde a la
                    // actividad capturada en TIME.
                    Descripcion = x.Actividad != null
        ? x.Actividad.Actividad
        : null,

                    Horas = x.Hora.Horas,

                    // Compatibilidad con el DTO anterior.
                    Usuario = x.Usuario != null
        ? x.Usuario.Nombre
        : null,

                    Proyecto = x.Clave != null
        ? x.Clave.Proyecto
        : null,

                    Departamento = x.Departamento != null
        ? x.Departamento.Nombre
        : null
                })
                .ToListAsync(cancellationToken);

            return new TimeReporteHorasResultado
            {
                TotalRegistros = total,
                Pagina = filtro.Pagina,
                TamanoPagina = filtro.TamanoPagina,
                TotalPaginas = totalPaginas,
                Registros = registros
            };
        }
    }
}
