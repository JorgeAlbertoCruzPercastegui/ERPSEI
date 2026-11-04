using ERPSEI.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

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

            // =====================================================
            // RELACIONES ENTRE TABLAS SINCRONIZADAS DE TIME
            // =====================================================

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

                join ec in _db.TimeEmpresaClasificaciones.AsNoTracking()
                    on e.Nombre equals ec.NombreEmpresa into gec
                from ec in gec.DefaultIfEmpty()

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
                    EmpresaClasificacion = ec,
                    Grupo = g
                };

            // =====================================================
            // FILTROS
            // =====================================================

            if (filtro.FechaInicio.HasValue)
            {
                DateTime inicio =
                    filtro.FechaInicio.Value.Date;

                consulta =
                    consulta.Where(x =>
                        x.Hora.Fecha >= inicio);
            }

            if (filtro.FechaFin.HasValue)
            {
                DateTime finExclusivo =
                    filtro.FechaFin.Value.Date.AddDays(1);

                consulta =
                    consulta.Where(x =>
                        x.Hora.Fecha < finExclusivo);
            }

            if (filtro.UsuarioId.HasValue)
            {
                long id =
                    filtro.UsuarioId.Value;

                consulta =
                    consulta.Where(x =>
                        x.Actividad != null &&
                        x.Actividad.UsuarioId == id);
            }

            if (filtro.ProyectoId.HasValue)
            {
                long id =
                    filtro.ProyectoId.Value;

                consulta =
                    consulta.Where(x =>
                        x.Actividad != null &&
                        x.Actividad.ClaveId == id);
            }

            if (filtro.DepartamentoId.HasValue)
            {
                long id =
                    filtro.DepartamentoId.Value;

                consulta =
                    consulta.Where(x =>
                        x.Usuario != null &&
                        x.Usuario.DepartamentoId == id);
            }

            // =====================================================
            // TOTAL Y PAGINACIÓN
            // =====================================================

            long total =
                await consulta.LongCountAsync(
                    cancellationToken);

            long totalPaginasLong =
                total / filtro.TamanoPagina +
                (
                    total % filtro.TamanoPagina == 0
                        ? 0
                        : 1
                );

            if (totalPaginasLong > int.MaxValue)
            {
                throw new InvalidOperationException(
                    "El reporte excede la paginación admitida.");
            }

            int totalPaginas =
                (int)totalPaginasLong;

            if (filtro.Pagina >
                Math.Max(totalPaginas, 1))
            {
                throw new ArgumentException(
                    "La página solicitada no existe.");
            }

            int omitir =
                checked(
                    (filtro.Pagina - 1) *
                    filtro.TamanoPagina
                );

            // =====================================================
            // OBTENER REGISTROS
            // =====================================================

            var registros =
                await consulta
                    .OrderByDescending(x =>
                        x.Hora.Fecha)
                    .ThenByDescending(x =>
                        x.Hora.Id)
                    .Skip(omitir)
                    .Take(
                        filtro.TamanoPagina)
                    .Select(x =>
                        new TimeReporteHorasFila
                        {
                            Id =
                                x.Hora.Id,

                            Nombre =
                                x.Usuario != null
                                    ? x.Usuario.Nombre
                                    : null,

                            Area =
                                x.Departamento != null
                                    ? x.Departamento.Nombre
                                    : null,

                            // Se llena posteriormente
                            // utilizando la Intranet.
                            Puesto =
                                null,

                            // Pendiente de fuente
                            // confirmada.
                            NivelCliente =
                                null,

                            GrupoEmpresa =
                                x.Empresa != null &&
                                x.Grupo != null
                                    ? (x.Grupo.Nombre ?? "") +
                                      " / " +
                                      (x.Empresa.Nombre ?? "")
                                    : x.Empresa != null
                                        ? x.Empresa.Nombre
                                        : null,

                            InternaExterna =
                                x.EmpresaClasificacion != null
                                    ? x.EmpresaClasificacion.Tipo ==
                                      "Interna"
                                        ? "Interno"
                                        : x.EmpresaClasificacion.Tipo ==
                                          "Externa"
                                            ? "Externo"
                                            : null
                                    : null,

                            // Se llenará posteriormente
                            // desde TimeTipoClasificaciones.
                            Tipo =
                                null,

                            Fecha =
                                x.Hora.Fecha,

                            // En el reporte Actividad
                            // corresponde al proyecto o clave.
                            Actividad =
                                x.Clave != null
                                    ? x.Clave.Proyecto
                                    : null,

                            // Descripción corresponde a la
                            // actividad capturada en TIME.
                            Descripcion =
                                x.Actividad != null
                                    ? x.Actividad.Actividad
                                    : null,

                            Horas =
                                x.Hora.Horas,

                            // Compatibilidad con DTO anterior.
                            Usuario =
                                x.Usuario != null
                                    ? x.Usuario.Nombre
                                    : null,

                            Proyecto =
                                x.Clave != null
                                    ? x.Clave.Proyecto
                                    : null,

                            Departamento =
                                x.Departamento != null
                                    ? x.Departamento.Nombre
                                    : null
                        })
                    .ToListAsync(
                        cancellationToken);

            // =====================================================
            // ENRIQUECER PUESTO DESDE LA INTRANET
            //
            // TIME Nombre
            // -> Empleados.NombreCompleto
            // -> Puestos.Nombre
            // =====================================================

            var empleadosConPuesto =
                await (
                    from empleado in
                        _db.Empleados.AsNoTracking()

                    join puesto in
                        _db.Puestos.AsNoTracking()

                        on empleado.PuestoId
                        equals puesto.Id
                        into gp

                    from puesto in
                        gp.DefaultIfEmpty()

                    where
                        empleado.NombreCompleto != null &&
                        empleado.NombreCompleto != ""

                    select new
                    {
                        empleado.NombreCompleto,

                        Puesto =
                            puesto != null
                                ? puesto.Nombre
                                : null
                    })
                .ToListAsync(
                    cancellationToken);

            var puestosPorNombre =
                empleadosConPuesto
                    .Select(x =>
                        new
                        {
                            NombreNormalizado =
                                NormalizarNombreParaComparacion(
                                    x.NombreCompleto),

                            x.Puesto
                        })
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.NombreNormalizado))
                    .GroupBy(x =>
                        x.NombreNormalizado)
                    .Where(g =>
                        g.Count() == 1)
                    .ToDictionary(
                        g => g.Key,
                        g => g.First().Puesto ??
                             string.Empty,
                        StringComparer.OrdinalIgnoreCase);

            foreach (var registro
                     in registros)
            {
                if (string.IsNullOrWhiteSpace(
                    registro.Nombre))
                {
                    continue;
                }

                string nombreNormalizado =
                    NormalizarNombreParaComparacion(
                        registro.Nombre);

                if (puestosPorNombre
                    .TryGetValue(
                        nombreNormalizado,
                        out string? puesto))
                {
                    registro.Puesto =
                        puesto;
                }
            }

            // =====================================================
            // ENRIQUECER TIPO DESDE CATÁLOGO TIME
            //
            // Prioridad:
            //
            // 1. Usuario + Empresa + Actividad + Descripción
            // 2. Usuario + Empresa + Actividad
            // 3. Usuario + Actividad + Descripción
            // 4. Usuario + Actividad
            //
            // Solamente se utilizan combinaciones que
            // correspondan a UN SOLO Tipo.
            // =====================================================

            var clasificacionesTipo =
                await _db
                    .TimeTipoClasificaciones
                    .AsNoTracking()
                    .ToListAsync(
                        cancellationToken);

            // =====================================================
            // NIVEL 1
            // Usuario + Empresa + Actividad + Descripción
            // =====================================================

            var tiposExactos =
                clasificacionesTipo
                    .GroupBy(x =>
                        CrearClaveTipo(
                            NormalizarNombreParaComparacion(
                                x.NombreUsuario),

                            NormalizarTextoTipo(
                                x.NombreEmpresa),

                            NormalizarActividadTipo(
                                x.Actividad),

                            NormalizarTextoTipo(
                                x.Descripcion)
                        ))
                    .Where(g =>
                        g.Select(x => x.Tipo)
                            .Distinct(
                                StringComparer.OrdinalIgnoreCase)
                            .Count() == 1)
                    .ToDictionary(
                        g => g.Key,
                        g => g.First().Tipo,
                        StringComparer.OrdinalIgnoreCase);

            // =====================================================
            // NIVEL 2
            // Usuario + Empresa + Actividad
            //
            // Se utiliza cuando TIME no tiene descripción
            // o ésta no coincide con el catálogo.
            // =====================================================

            var tiposUsuarioEmpresaActividad =
                clasificacionesTipo
                    .GroupBy(x =>
                        CrearClaveTipo(
                            NormalizarNombreParaComparacion(
                                x.NombreUsuario),

                            NormalizarTextoTipo(
                                x.NombreEmpresa),

                            NormalizarActividadTipo(
                                x.Actividad)
                        ))
                    .Where(g =>
                        g.Select(x => x.Tipo)
                            .Distinct(
                                StringComparer.OrdinalIgnoreCase)
                            .Count() == 1)
                    .ToDictionary(
                        g => g.Key,
                        g => g.First().Tipo,
                        StringComparer.OrdinalIgnoreCase);

            // =====================================================
            // NIVEL 3
            // Usuario + Actividad + Descripción
            //
            // Se utiliza principalmente cuando TIME no
            // tiene empresa relacionada.
            // =====================================================

            var tiposUsuarioActividadDescripcion =
                clasificacionesTipo
                    .GroupBy(x =>
                        CrearClaveTipo(
                            NormalizarNombreParaComparacion(
                                x.NombreUsuario),

                            NormalizarActividadTipo(
                                x.Actividad),

                            NormalizarTextoTipo(
                                x.Descripcion)
                        ))
                    .Where(g =>
                        g.Select(x => x.Tipo)
                            .Distinct(
                                StringComparer.OrdinalIgnoreCase)
                            .Count() == 1)
                    .ToDictionary(
                        g => g.Key,
                        g => g.First().Tipo,
                        StringComparer.OrdinalIgnoreCase);

            // =====================================================
            // NIVEL 4
            // Usuario + Actividad
            //
            // Último nivel.
            //
            // Únicamente existe en el diccionario cuando
            // esa combinación tiene siempre el mismo Tipo.
            // =====================================================

            var tiposUsuarioActividad =
                clasificacionesTipo
                    .GroupBy(x =>
                        CrearClaveTipo(
                            NormalizarNombreParaComparacion(
                                x.NombreUsuario),

                            NormalizarActividadTipo(
                                x.Actividad)
                        ))
                    .Where(g =>
                        g.Select(x => x.Tipo)
                            .Distinct(
                                StringComparer.OrdinalIgnoreCase)
                            .Count() == 1)
                    .ToDictionary(
                        g => g.Key,
                        g => g.First().Tipo,
                        StringComparer.OrdinalIgnoreCase);

            // =====================================================
            // ASIGNAR TIPO A CADA REGISTRO
            // =====================================================

            foreach (var registro
                     in registros)
            {
                string usuario =
                    NormalizarNombreParaComparacion(
                        registro.Nombre);

                string empresa =
                    NormalizarEmpresaReporte(
                        registro.GrupoEmpresa);

                string actividad =
                    NormalizarActividadTipo(
                        registro.Actividad);

                string descripcion =
                    NormalizarTextoTipo(
                        registro.Descripcion);

                registro.Tipo =
                    null;

                // =============================================
                // NIVEL 1
                // Usuario + Empresa + Actividad + Descripción
                // =============================================

                string claveExacta =
                    CrearClaveTipo(
                        usuario,
                        empresa,
                        actividad,
                        descripcion);

                if (tiposExactos
                    .TryGetValue(
                        claveExacta,
                        out string? tipoExacto))
                {
                    registro.Tipo =
                        tipoExacto;

                    continue;
                }

                // =============================================
                // NIVEL 2
                // Usuario + Empresa + Actividad
                // =============================================

                string claveEmpresaActividad =
                    CrearClaveTipo(
                        usuario,
                        empresa,
                        actividad);

                if (tiposUsuarioEmpresaActividad
                    .TryGetValue(
                        claveEmpresaActividad,
                        out string?
                            tipoEmpresaActividad))
                {
                    registro.Tipo =
                        tipoEmpresaActividad;

                    continue;
                }

                // =============================================
                // NIVEL 3
                // Usuario + Actividad + Descripción
                // =============================================

                string claveActividadDescripcion =
                    CrearClaveTipo(
                        usuario,
                        actividad,
                        descripcion);

                if (tiposUsuarioActividadDescripcion
                    .TryGetValue(
                        claveActividadDescripcion,
                        out string?
                            tipoActividadDescripcion))
                {
                    registro.Tipo =
                        tipoActividadDescripcion;

                    continue;
                }

                // =============================================
                // NIVEL 4
                // Usuario + Actividad
                // =============================================

                string claveActividad =
                    CrearClaveTipo(
                        usuario,
                        actividad);

                if (tiposUsuarioActividad
                    .TryGetValue(
                        claveActividad,
                        out string?
                            tipoActividad))
                {
                    registro.Tipo =
                        tipoActividad;
                }
            }

            // =====================================================
            // RESULTADO
            // =====================================================

            return new TimeReporteHorasResultado
            {
                TotalRegistros =
                    total,

                Pagina =
                    filtro.Pagina,

                TamanoPagina =
                    filtro.TamanoPagina,

                TotalPaginas =
                    totalPaginas,

                Registros =
                    registros
            };
        }

        // =========================================================
        // CREAR CLAVE PARA LOS DICCIONARIOS DE TIPO
        //
        // Utilizamos un separador interno poco probable
        // para evitar problemas con tuplas y tipos genéricos.
        // =========================================================

        private static string CrearClaveTipo(
            params string[] valores)
        {
            return string.Join(
                "\u001F",
                valores);
        }

        // =========================================================
        // NORMALIZAR TEXTO PARA TIPO
        //
        // - Trim
        // - Mayúsculas
        // - Elimina acentos
        // - Normaliza espacios
        // =========================================================

        private static string NormalizarTextoTipo(
            string? valor)
        {
            if (string.IsNullOrWhiteSpace(
                valor))
            {
                return string.Empty;
            }

            string texto =
                valor
                    .Trim()
                    .ToUpperInvariant()
                    .Normalize(
                        NormalizationForm.FormD);

            var sb =
                new StringBuilder();

            foreach (char caracter
                     in texto)
            {
                UnicodeCategory categoria =
                    CharUnicodeInfo
                        .GetUnicodeCategory(
                            caracter);

                if (categoria !=
                    UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(
                        caracter);
                }
            }

            string sinAcentos =
                sb
                    .ToString()
                    .Normalize(
                        NormalizationForm.FormC);

            string[] palabras =
                sinAcentos
                    .Split(
                        ' ',
                        StringSplitOptions.RemoveEmptyEntries |
                        StringSplitOptions.TrimEntries);

            return string.Join(
                " ",
                palabras);
        }

        // =========================================================
        // NORMALIZAR ACTIVIDAD
        //
        // El catálogo histórico contiene "KIYNDOM"
        // y TIME puede contener "KYNDOM".
        //
        // Ambos se consideran la misma actividad.
        // =========================================================

        private static string NormalizarActividadTipo(
            string? actividad)
        {
            string valor =
                NormalizarTextoTipo(
                    actividad);

            if (valor == "KIYNDOM")
            {
                return "KYNDOM";
            }

            return valor;
        }

        // =========================================================
        // NORMALIZAR EMPRESA DEL REPORTE
        //
        // GrupoEmpresa puede traer:
        //
        // EMPRESA
        //
        // o eventualmente:
        //
        // GRUPO / EMPRESA
        //
        // Para la clasificación se necesita Empresa.
        // =========================================================

        private static string NormalizarEmpresaReporte(
            string? grupoEmpresa)
        {
            string valor =
                NormalizarTextoTipo(
                    grupoEmpresa);

            if (string.IsNullOrWhiteSpace(
                valor))
            {
                return string.Empty;
            }

            const string separador =
                " / ";

            int posicion =
                valor.LastIndexOf(
                    separador,
                    StringComparison.Ordinal);

            if (posicion >= 0)
            {
                int inicioEmpresa =
                    posicion +
                    separador.Length;

                if (inicioEmpresa <
                    valor.Length)
                {
                    return valor
                        .Substring(
                            inicioEmpresa)
                        .Trim();
                }
            }

            return valor;
        }

        // =========================================================
        // NORMALIZAR NOMBRE PARA COMPARACIÓN
        //
        // Permite comparar:
        //
        // VILLAVERDE BARCENAS XIMENA ABIGAIL
        //
        // contra otra variante del mismo nombre aunque
        // el orden de nombres/apellidos sea distinto.
        // =========================================================

        private static string NormalizarNombreParaComparacion(
            string? nombre)
        {
            if (string.IsNullOrWhiteSpace(
                nombre))
            {
                return string.Empty;
            }

            string texto =
                nombre
                    .Trim()
                    .ToUpperInvariant()
                    .Normalize(
                        NormalizationForm.FormD);

            var sb =
                new StringBuilder();

            foreach (char caracter
                     in texto)
            {
                UnicodeCategory categoria =
                    CharUnicodeInfo
                        .GetUnicodeCategory(
                            caracter);

                if (categoria !=
                    UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(
                        caracter);
                }
            }

            string sinAcentos =
                sb
                    .ToString()
                    .Normalize(
                        NormalizationForm.FormC);

            string[] palabras =
                sinAcentos
                    .Split(
                        ' ',
                        StringSplitOptions.RemoveEmptyEntries |
                        StringSplitOptions.TrimEntries);

            Array.Sort(
                palabras,
                StringComparer.OrdinalIgnoreCase);

            return string.Join(
                " ",
                palabras);
        }
    }
}