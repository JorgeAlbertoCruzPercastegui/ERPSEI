(() => {
    'use strict';
    const $ = id => document.getElementById(id);
    let chart = null, chartDept = null, dashboardController = null, tablaController = null, analiticaController = null, colaboradoresController = null;
    let paginaColaboradores = 1, debounceColaboradores = null;
    let syncController = null, debounce = null, debounceFechas = null;
    let paginaActual = 1, empleadosBioTime = [], versionFechas = 0;
    let bioResumenPagina = 1;
    let bioResumenController = null;
    let bioResumenDebounce = null;
    let sincronizando = false;
    let trabajoActivo = null;
    let pestanaActual = 'dashboard';
    const pestañas = ['dashboard', 'puntualidad', 'jornadas', 'incidencias', 'reportes', 'sincronizacion'];
    const tablasExtra = {
        puntualidad: { handler: 'Puntualidad', prefix: 'bioPuntualidad', cols: 6, page: 1, controller: null },
        jornadas: { handler: 'Jornadas', prefix: 'bioJornadas', cols: 7, page: 1, controller: null },
        incidencias: { handler: 'IncidenciasRevision', prefix: 'bioIncidencias', cols: 5, page: 1, controller: null },
        sincronizacion: { handler: 'AuditoriaSincronizacion', prefix: 'bioSync', cols: 7, page: 1, controller: null }
    };

    const num = n => new Intl.NumberFormat('es-MX').format(n ?? 0);
    const estado = s => {
        $('bioEstado').textContent = s;
        $('bioDot').style.background = s === 'Datos actualizados' ? '#16a34a' : '#c7c3d7';
    };
    const params = o => new URLSearchParams(Object.entries(o).filter(([, v]) => v !== '' && v !== null && v !== undefined));
    const filtros = () => ({ fechaInicio: $('bioInicio').value, fechaFin: $('bioFin').value, empleado: $('bioEmpleado').value, departamento: $('bioDept').value });
    const fechasValidas = () => {
        const { fechaInicio, fechaFin } = filtros();
        if (!/^\d{4}-\d{2}-\d{2}$/.test(fechaInicio) || !/^\d{4}-\d{2}-\d{2}$/.test(fechaFin)) return false;
        const inicio = new Date(`${fechaInicio}T12:00:00`), fin = new Date(`${fechaFin}T12:00:00`);
        const dias = Math.round((fin - inicio) / 86400000) + 1;
        return dias >= 1 && dias <= 365 && fechaFin <= fechaHoyLocal();
    };
    const fechaHoyLocal = () => {
        const d = new Date();
        return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
    };

    function limpiarResultados(mensaje = 'Selecciona fecha inicial y fecha final para consultar las checadas.') {
        dashboardController?.abort();
        tablaController?.abort();
        chart?.destroy();
        chart = null;
        analiticaController?.abort();
        chartDept?.destroy(); chartDept = null;
        $('bioDiasPersona').textContent = '—';
        $('bioPromedio').textContent = '—';
        $('bioExportarChecadas').disabled = true;
        $('bioExportarResumen').disabled = true;
        $('bioDeptEstado').textContent = mensaje;
        colaboradoresController?.abort();
        paginaColaboradores = 1;
        $('bioColabEstado').textContent = mensaje;
        $('bioColabInfoPagina').textContent = '0 registros';
        $('bioColabNumeroPagina').textContent = '1 / 1';
        $('bioColabAnterior').disabled = true;
        $('bioColabSiguiente').disabled = true;
        const rColab = document.createElement('tr'), cColab = document.createElement('td');
        cColab.colSpan = 6; cColab.className = 'text-center text-muted py-4'; cColab.textContent = mensaje;
        rColab.append(cColab); $('bioColabTabla').replaceChildren(rColab);
        const filaDept = document.createElement('tr'), celdaDept = document.createElement('td');
        celdaDept.colSpan = 5; celdaDept.className = 'text-center text-muted py-4'; celdaDept.textContent = mensaje;
        filaDept.append(celdaDept); $('bioDeptTabla').replaceChildren(filaDept);
        $('bioTotal').textContent = '—';
        $('bioPersonas').textContent = '—';
        $('bioSync').textContent = '—';
        $('bioSyncDetalle').textContent = 'Sin periodo seleccionado';
        $('bioTablaEstado').textContent = mensaje;
        const tr = document.createElement('tr');
        const td = document.createElement('td');
        td.colSpan = 6;
        td.className = 'text-center text-muted py-4';
        td.textContent = mensaje;
        tr.appendChild(td);
        $('bioTabla').replaceChildren(tr);
        $('bioInfoPagina').textContent = '0 registros';
        $('bioNumeroPagina').textContent = '1 / 1';
        $('bioAnterior').disabled = true;
        $('bioSiguiente').disabled = true;
        paginaActual = 1;
        limpiarResumenDiario(mensaje);
        resetExtras(mensaje);
    }

    const fechaHora = x => {
        const [fecha, hora] = String(x || '').split('T');
        if (!hora) return { fecha: '—', hora: '—' };
        const [h, m, seg] = hora.split(':');
        return { fecha, hora: `${String((+h % 12) || 12).padStart(2, '0')}:${m}:${(seg || '00').substring(0, 2)} ${+h >= 12 ? 'p. m.' : 'a. m.'}` };
    };

    async function cargarCatalogos() {
        const r = await fetch('?handler=Catalogos', { headers: { Accept: 'application/json' } });
        if (!r.ok) throw Error('Error cargando catálogos');
        const d = await r.json();
        if (!d.success) throw Error(d.message || 'No fue posible cargar los catálogos');
        empleadosBioTime = Array.isArray(d.empleados) ? d.empleados : [];
        $('bioDept').replaceChildren(new Option('Todos los departamentos', ''));
        for (const area of d.areas || []) $('bioDept').add(new Option(area, area));
        actualizarColaboradores();
    }

    function actualizarColaboradores() {
        const dept = $('bioDept').value.trim().toLocaleLowerCase('es-MX');
        const anterior = $('bioEmpleado').value;
        const empleados = empleadosBioTime
            .filter(e => !dept || String(e.departamento || '').trim().toLocaleLowerCase('es-MX') === dept)
            .sort((a, b) => String(a.nombre || '').localeCompare(String(b.nombre || ''), 'es-MX', { sensitivity: 'base' }));
        $('bioEmpleado').replaceChildren(new Option('Todos los colaboradores', ''));
        for (const e of empleados) $('bioEmpleado').add(new Option(`${e.nombre} (${e.codigo})`, e.codigo));
        $('bioEmpleado').value = empleados.some(e => String(e.codigo) === anterior) ? anterior : '';
    }

    async function cargarDashboard() {
        if (!fechasValidas()) return;
        dashboardController?.abort();
        const current = dashboardController = new AbortController();
        try {
            const r = await fetch(`?handler=Dashboard&${params(filtros())}`, { signal: current.signal, headers: { Accept: 'application/json' } });
            const d = await r.json();
            if (!r.ok || !d.success) throw Error(d.message || 'Error consultando registros');
            $('bioTotal').textContent = num(d.total);
            $('bioPersonas').textContent = num(d.personas);
            $('bioSync').textContent = d.ultima?.estado || 'Sin ejecuciones';
            $('bioSyncDetalle').textContent = d.ultima?.inicioUtc ? `Inicio UTC: ${d.ultima.inicioUtc.replace('T', ' ').substring(0, 19)}` : 'Sin información todavía';
            chart?.destroy();
            if (typeof Chart !== 'undefined') chart = new Chart($('bioChart'), {
                type: 'bar',
                data: { labels: (d.porDia || []).map(x => x.fecha.substring(0, 10)), datasets: [{ label: 'Checadas', data: (d.porDia || []).map(x => x.total), backgroundColor: '#21166b', borderRadius: 5 }] },
                options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { display: false } }, scales: { y: { beginAtZero: true, ticks: { precision: 0 } } } }
            });
        } catch (e) {
            if (e.name !== 'AbortError') { console.error('BioTime dashboard:', e); throw e; }
        }
    }

    async function cargarTabla() {
        if (!fechasValidas()) return;
        tablaController?.abort();
        const current = tablaController = new AbortController();
        $('bioTablaEstado').textContent = 'Consultando checadas...';
        try {
            const q = params({ ...filtros(), buscar: $('bioBuscar').value.trim(), pagina: paginaActual, tamano: $('bioTamano').value });
            const r = await fetch(`?handler=Checadas&${q}`, { signal: current.signal, headers: { Accept: 'application/json' } });
            const d = await r.json();
            if (!r.ok || !d.success) throw Error(d.message || 'Error consultando checadas');
            paginaActual = d.pagina;
            const tbody = $('bioTabla');
            tbody.replaceChildren();
            if (!d.registros.length) {
                const tr = document.createElement('tr'), td = document.createElement('td');
                td.colSpan = 6; td.className = 'text-center text-muted py-4'; td.textContent = 'Sin checadas para los filtros seleccionados'; tr.append(td); tbody.append(tr);
            }
            for (const x of d.registros) {
                const fh = fechaHora(x.fechaHora), tr = document.createElement('tr');
                for (const value of [`${x.nombre || 'Sin nombre'} (${x.codigo})`, x.departamento || '—', fh.fecha, fh.hora, x.estado ?? '—', x.dispositivo || '—']) {
                    const td = document.createElement('td'); td.textContent = value; tr.append(td);
                }
                tbody.append(tr);
            }
            const desde = d.total === 0 ? 0 : (d.pagina - 1) * d.tamano + 1;
            const hasta = Math.min(d.total, d.pagina * d.tamano);
            $('bioTablaEstado').textContent = `${num(d.total)} checadas encontradas`;
            $('bioInfoPagina').textContent = `Mostrando ${num(desde)}–${num(hasta)} de ${num(d.total)}`;
            $('bioNumeroPagina').textContent = `${d.pagina} / ${d.paginas}`;
            $('bioAnterior').disabled = d.pagina <= 1;
            $('bioSiguiente').disabled = d.pagina >= d.paginas;
        } catch (e) {
            if (e.name !== 'AbortError') {
                console.error('BioTime checadas:', e);
                $('bioTablaEstado').textContent = 'Error al consultar';
                const tr = document.createElement('tr'), td = document.createElement('td');
                td.colSpan = 6; td.className = 'text-center text-danger py-4'; td.textContent = 'No fue posible cargar las checadas.'; tr.append(td); $('bioTabla').replaceChildren(tr);
                throw e;
            }
        }
    }


    // =====================================================
    // RESUMEN DIARIO: ESTADO INICIAL Y CONSULTA PAGINADA
    // =====================================================

    function limpiarResumenDiario(mensaje = 'Selecciona ambas fechas para consultar información.') {
        bioResumenController?.abort();
        bioResumenPagina = 1;

        const tr = document.createElement('tr');
        const td = document.createElement('td');
        td.colSpan = 7;
        td.className = 'text-center text-muted py-4';
        td.textContent = mensaje;
        tr.appendChild(td);
        $('bioResumenTabla').replaceChildren(tr);

        $('bioResumenEstado').textContent = mensaje;
        $('bioResumenInfoPagina').textContent = '0 registros';
        $('bioResumenNumeroPagina').textContent = '1 / 1';
        $('bioResumenAnterior').disabled = true;
        $('bioResumenSiguiente').disabled = true;
    }

    async function cargarResumenDiario() {
        if (!fechasValidas() || sincronizando) return;

        bioResumenController?.abort();
        const current = bioResumenController = new AbortController();
        $('bioResumenEstado').textContent = 'Consultando resumen diario...';

        try {
            const q = params({
                ...filtros(),
                buscar: $('bioResumenBuscar').value.trim(),
                pagina: bioResumenPagina,
                tamano: $('bioResumenTamano').value
            });
            const r = await fetch(`?handler=ResumenDiario&${q}`, {
                signal: current.signal,
                headers: { Accept: 'application/json' }
            });
            const d = await r.json();
            if (!r.ok || !d.success) throw Error(d.message || 'Error consultando resumen diario');

            bioResumenPagina = d.pagina;
            const tbody = $('bioResumenTabla');
            tbody.replaceChildren();

            if (!d.registros.length) {
                const tr = document.createElement('tr');
                const td = document.createElement('td');
                td.colSpan = 7;
                td.className = 'text-center text-muted py-4';
                td.textContent = 'Sin registros diarios para los filtros seleccionados';
                tr.appendChild(td);
                tbody.appendChild(tr);
            }

            for (const x of d.registros) {
                const primera = fechaHora(x.primeraChecada);
                const ultima = fechaHora(x.ultimaChecada);
                const minutos = Math.max(0, Number(x.minutosEntreExtremos) || 0);
                const horas = Math.floor(minutos / 60);
                const minutosRestantes = Math.floor(minutos % 60);
                const tr = document.createElement('tr');
                const valores = [
                    String(x.fecha || '').substring(0, 10) || '—',
                    `${x.nombre || 'Sin nombre'} (${x.codigo})`,
                    x.departamento || '—',
                    primera.hora,
                    ultima.hora,
                    x.totalChecadas,
                    `${horas} h ${String(minutosRestantes).padStart(2, '0')} min`
                ];
                for (const valor of valores) {
                    const td = document.createElement('td');
                    td.textContent = valor;
                    tr.appendChild(td);
                }
                tbody.appendChild(tr);
            }

            const desde = d.total === 0 ? 0 : (d.pagina - 1) * d.tamano + 1;
            const hasta = Math.min(d.total, d.pagina * d.tamano);
            $('bioResumenEstado').textContent = `${num(d.total)} registros diarios encontrados`;
            $('bioResumenInfoPagina').textContent = `Mostrando ${num(desde)}–${num(hasta)} de ${num(d.total)}`;
            $('bioResumenNumeroPagina').textContent = `${d.pagina} / ${d.paginas}`;
            $('bioResumenAnterior').disabled = d.pagina <= 1;
            $('bioResumenSiguiente').disabled = d.pagina >= d.paginas;
        } catch (e) {
            if (e.name === 'AbortError') return;
            console.error('BioTime resumen diario:', e);
            $('bioResumenEstado').textContent = 'Error al consultar resumen diario';
            const tr = document.createElement('tr');
            const td = document.createElement('td');
            td.colSpan = 7;
            td.className = 'text-center text-danger py-4';
            td.textContent = 'No fue posible cargar el resumen diario.';
            tr.appendChild(td);
            $('bioResumenTabla').replaceChildren(tr);
            throw e;
        }
    }

    function registrarEventosResumenDiario() {
        $('bioResumenBuscar').addEventListener('input', () => {
            clearTimeout(bioResumenDebounce);
            if (!fechasValidas() || sincronizando) return;
            bioResumenDebounce = setTimeout(() => {
                bioResumenPagina = 1;
                cargarResumenDiario().catch(console.error);
            }, 300);
        });
        $('bioResumenTamano').addEventListener('change', () => {
            if (!fechasValidas() || sincronizando) return;
            bioResumenPagina = 1;
            cargarResumenDiario().catch(console.error);
        });
        $('bioResumenAnterior').addEventListener('click', () => {
            if (fechasValidas() && !sincronizando && bioResumenPagina > 1) {
                bioResumenPagina--;
                cargarResumenDiario().catch(console.error);
            }
        });
        $('bioResumenSiguiente').addEventListener('click', () => {
            if (fechasValidas() && !sincronizando && !$('bioResumenSiguiente').disabled) {
                bioResumenPagina++;
                cargarResumenDiario().catch(console.error);
            }
        });
    }

    async function cargarAnaliticaDepartamentos() {
        if (!fechasValidas() || sincronizando) return;
        analiticaController?.abort();
        const current = analiticaController = new AbortController();
        $('bioDeptEstado').textContent = 'Calculando indicadores...';
        try {
            const r = await fetch(`?handler=AnaliticaDepartamentos&${params(filtros())}`, {
                signal: current.signal, headers: { Accept: 'application/json' }
            });
            const d = await r.json();
            if (!r.ok || !d.success) throw Error(d.message || 'Error al calcular indicadores');
            $('bioDiasPersona').textContent = num(d.totalDiasEmpleado);
            $('bioPromedio').textContent = new Intl.NumberFormat('es-MX', { maximumFractionDigits: 2 }).format(d.promedioChecadasPorDiaEmpleado ?? 0);
            const tbody = $('bioDeptTabla'); tbody.replaceChildren();
            for (const x of d.departamentos || []) {
                const tr = document.createElement('tr');
                for (const value of [x.departamento, num(x.colaboradores), num(x.diasConActividad), num(x.checadas), x.promedioChecadasPorDiaEmpleado]) {
                    const td = document.createElement('td'); td.textContent = String(value); tr.append(td);
                }
                tbody.append(tr);
            }
            if (!(d.departamentos || []).length) {
                const tr = document.createElement('tr'), td = document.createElement('td');
                td.colSpan = 5; td.className = 'text-center text-muted py-4'; td.textContent = 'Sin actividad en el periodo'; tr.append(td); tbody.append(tr);
            }
            chartDept?.destroy();
            if (typeof Chart !== 'undefined') chartDept = new Chart($('bioDeptChart'), {
                type: 'bar',
                data: {
                    labels: (d.departamentos || []).map(x => x.departamento),
                    datasets: [{ label: 'Checadas', data: (d.departamentos || []).map(x => x.checadas), backgroundColor: '#21166b', borderRadius: 5 }]
                },
                options: {
                    indexAxis: 'y', responsive: true, maintainAspectRatio: false,
                    plugins: { legend: { display: false } }, scales: { x: { beginAtZero: true, ticks: { precision: 0 } } }
                }
            });
            $('bioExportarChecadas').disabled = false;
            $('bioExportarResumen').disabled = false;
            $('bioDeptEstado').textContent = `${num((d.departamentos || []).length)} departamentos con actividad`;
        } catch (e) {
            if (e.name === 'AbortError') return;
            console.error('BioTime departamentos:', e);
            $('bioDeptEstado').textContent = 'Error al calcular indicadores';
            throw e;
        }
    }

    async function cargarAnaliticaColaboradores() {
        if (!fechasValidas() || sincronizando) return;
        colaboradoresController?.abort();
        const current = colaboradoresController = new AbortController();
        $('bioColabEstado').textContent = 'Consultando colaboradores...';
        try {
            const q = params({
                ...filtros(), buscar: $('bioColabBuscar').value.trim(),
                pagina: paginaColaboradores, tamano: $('bioColabTamano').value
            });
            const r = await fetch(`?handler=AnaliticaColaboradores&${q}`, {
                signal: current.signal, headers: { Accept: 'application/json' }
            });
            const d = await r.json();
            if (!r.ok || !d.success) throw Error(d.message || 'No se pudo consultar colaboradores');
            paginaColaboradores = d.pagina;
            const tbody = $('bioColabTabla'); tbody.replaceChildren();
            for (const x of d.registros || []) {
                const tr = document.createElement('tr');
                const datos = [`${x.nombre || 'Sin nombre'} (${x.codigo})`, x.departamento || '—',
                num(x.checadas), num(x.diasConActividad),
                `${fechaHora(x.primeraChecada).fecha} ${fechaHora(x.primeraChecada).hora}`,
                `${fechaHora(x.ultimaChecada).fecha} ${fechaHora(x.ultimaChecada).hora}`];
                for (const dato of datos) { const td = document.createElement('td'); td.textContent = String(dato); tr.append(td); }
                tbody.append(tr);
            }
            if (!d.registros?.length) {
                const tr = document.createElement('tr'), td = document.createElement('td');
                td.colSpan = 6; td.className = 'text-center text-muted py-4'; td.textContent = 'Sin actividad'; tr.append(td); tbody.append(tr);
            }
            const desde = d.total === 0 ? 0 : (d.pagina - 1) * d.tamano + 1;
            $('bioColabInfoPagina').textContent = `Mostrando ${num(desde)}–${num(Math.min(d.total, d.pagina * d.tamano))} de ${num(d.total)}`;
            $('bioColabEstado').textContent = `${num(d.total)} colaboradores con actividad`;
            $('bioColabNumeroPagina').textContent = `${d.pagina} / ${d.paginas}`;
            $('bioColabAnterior').disabled = d.pagina <= 1;
            $('bioColabSiguiente').disabled = d.pagina >= d.paginas;
        } catch (e) {
            if (e.name === 'AbortError') return;
            console.error('BioTime colaboradores:', e);
            $('bioColabEstado').textContent = 'Error al consultar colaboradores';
            throw e;
        }
    }

    function registrarColaboradores() {
        $('bioColabBuscar').addEventListener('input', () => {
            clearTimeout(debounceColaboradores);
            if (!fechasValidas() || sincronizando) return;
            debounceColaboradores = setTimeout(() => { paginaColaboradores = 1; cargarAnaliticaColaboradores().catch(console.error); }, 300);
        });
        $('bioColabTamano').addEventListener('change', () => {
            if (!fechasValidas() || sincronizando) return;
            paginaColaboradores = 1; cargarAnaliticaColaboradores().catch(console.error);
        });
        $('bioColabAnterior').addEventListener('click', () => {
            if (paginaColaboradores > 1 && !sincronizando && fechasValidas()) {
                paginaColaboradores--; cargarAnaliticaColaboradores().catch(console.error);
            }
        });
        $('bioColabSiguiente').addEventListener('click', () => {
            if (!sincronizando && fechasValidas() && !$('bioColabSiguiente').disabled) {
                paginaColaboradores++; cargarAnaliticaColaboradores().catch(console.error);
            }
        });
    }

    function descargarExcel(tipo) {
        if (!fechasValidas() || sincronizando) return;
        // El servidor valida rol, fechas y máximo 50000 registros.
        window.location.assign(`?handler=ExportarExcel&${params({ ...filtros(), tipo })}`);
    }

    async function refrescarVista() {
        paginaActual = 1;
        bioResumenPagina = 1;
        paginaColaboradores = 1;
        await Promise.all([cargarDashboard(), cargarTabla(), cargarResumenDiario(), cargarAnaliticaDepartamentos(), cargarAnaliticaColaboradores()]);
        refrescarPestanaActiva();
    }

    const dormir = ms => new Promise(resolve => setTimeout(resolve, ms));

    async function consultarProgreso(trabajoId, version) {
        while (version === versionFechas) {
            // La descarga ocurre en el servidor, sin depender de que siga abierta esta solicitud.
            const r = await fetch(`?handler=EstadoSincronizacion&${params({ trabajoId })}`, {
                headers: { Accept: 'application/json' }, cache: 'no-store'
            });
            const d = await r.json();
            if (!r.ok || !d.success) throw Error(d.message || 'No se pudo obtener el progreso');
            const t = d.trabajo;
            if (version !== versionFechas) return;
            const avance = t.diasTotales ? Math.round(t.diasCompletados * 100 / t.diasTotales) : 0;
            estado(`BioTime: ${t.estado} · ${avance}% (${t.diasCompletados}/${t.diasTotales} días)`);
            $('bioTablaEstado').textContent = `${num(t.checadasInsertadas)} checadas nuevas · ${num(t.diasOmitidos)} días reutilizados`;
            if (t.estado === 'Correcta') return;
            if (t.estado === 'Error' || t.estado === 'Interrumpida')
                throw Error(`Importación ${t.estado.toLowerCase()} (${t.error || 'sin detalle'}). Revisa el historial de sincronización.`);
            await dormir(2000);
        }
    }

    async function sincronizarFechas() {
        const version = ++versionFechas;
        syncController?.abort();
        dashboardController?.abort();
        tablaController?.abort();
        analiticaController?.abort();
        colaboradoresController?.abort();
        bioResumenController?.abort();
        if (!fechasValidas()) {
            sincronizando = false;
            const faltaFecha = !$('bioInicio').value || !$('bioFin').value;
            const mensaje = faltaFecha
                ? 'Selecciona ambas fechas para consultar información.'
                : 'Selecciona un rango válido de 1 a 365 días, sin fechas futuras.';
            limpiarResultados(mensaje);
            estado(mensaje);
            return;
        }
        const current = syncController = new AbortController();
        sincronizando = true;
        estado('Preparando importación BioTime...');
        $('bioTablaEstado').textContent = 'Solicitando sincronización histórica...';
        try {
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
            if (!token) throw Error('No se encontró el token antifalsificación');
            const body = params({ fechaInicio: $('bioInicio').value, fechaFin: $('bioFin').value });
            const r = await fetch('?handler=SincronizarPeriodo', {
                method: 'POST', signal: current.signal,
                headers: { Accept: 'application/json', 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8', RequestVerificationToken: token },
                body: body.toString()
            });
            const d = await r.json();
            if (!r.ok || !d.success) throw Error(d.message || 'No se pudo iniciar la sincronización');
            trabajoActivo = d.trabajoId;
            await consultarProgreso(trabajoActivo, version);
            if (version !== versionFechas) return;
            const dept = $('bioDept').value, emp = $('bioEmpleado').value;
            await cargarCatalogos();
            if ([...$('bioDept').options].some(x => x.value === dept)) $('bioDept').value = dept;
            actualizarColaboradores();
            if ([...$('bioEmpleado').options].some(x => x.value === emp)) $('bioEmpleado').value = emp;
            if (version !== versionFechas) return;
            sincronizando = false;
            await refrescarVista();
            if (version === versionFechas) estado('Datos actualizados');
        } catch (e) {
            if (e.name === 'AbortError' || version !== versionFechas) return;
            console.error('BioTime histórico:', e);
            estado(`Error al sincronizar: ${e.message}`);
            $('bioTablaEstado').textContent = 'Se conservan los registros ya importados. Puedes volver a seleccionar el periodo para reintentar.';
        } finally {
            if (version === versionFechas) sincronizando = false;
        }
    }

    function cambioDeFecha() {
        clearTimeout(debounceFechas);
        // Invalidar el seguimiento del periodo anterior inmediatamente.
        versionFechas++;
        syncController?.abort();
        bioResumenController?.abort();
        sincronizando = false;
        const faltaFecha = !$('bioInicio').value || !$('bioFin').value;
        if (faltaFecha || !fechasValidas()) {
            const mensaje = faltaFecha
                ? 'Selecciona ambas fechas para consultar información.'
                : 'Selecciona un rango válido de 1 a 365 días, sin fechas futuras.';
            limpiarResultados(mensaje);
            estado(mensaje);
            return;
        }
        limpiarResultados('Consultando el periodo seleccionado...');
        // Espera breve para evitar crear trabajos mientras se ajustan fechas.
        debounceFechas = setTimeout(sincronizarFechas, 500);
    }

    function actualizarFiltros() {
        if (sincronizando || !fechasValidas()) return;
        refrescarVista().catch(e => { console.error('BioTime filtros:', e); estado('Error de consulta'); });
    }


    // =====================================================
    // PESTAÑAS: PUNTUALIDAD EXPLORATORIA, JORNADAS,
    // OBSERVACIONES DE INCIDENCIAS, REPORTES Y AUDITORÍA
    // =====================================================
    const textoFecha = x => x ? String(x).substring(0, 10) : '—';
    const textoFechaHora = x => x ? String(x).replace('T', ' ').substring(0, 19) : '—';
    const spanMinutos = n => `${Math.floor((n ?? 0) / 60)} h ${String((n ?? 0) % 60).padStart(2, '0')} min`;

    function pintarFilas(tbody, registros, columnas, mensaje) {
        tbody.replaceChildren();
        if (!registros.length) {
            const tr = document.createElement('tr'), td = document.createElement('td');
            td.colSpan = columnas; td.className = 'text-center text-muted py-4'; td.textContent = mensaje;
            tr.append(td); tbody.append(tr); return;
        }
        for (const fila of registros) {
            const tr = document.createElement('tr');
            for (const valor of fila) {
                const td = document.createElement('td');
                td.textContent = valor ?? '—'; tr.append(td);
            }
            tbody.append(tr);
        }
    }

    function resetExtras(mensaje = 'Selecciona ambas fechas para consultar información.') {
        for (const [clave, t] of Object.entries(tablasExtra)) {
            t.controller?.abort(); t.page = 1;
            pintarFilas($(t.prefix + 'Tabla'), [], t.cols, mensaje);
            $(t.prefix + 'Pagina').textContent = '1 / 1';
            $(t.prefix + 'Prev').disabled = true; $(t.prefix + 'Next').disabled = true;
            $(t.prefix + 'Info').textContent = 'Sin resultados';
            if (clave !== 'sincronizacion') $(t.prefix + 'Estado').textContent = mensaje;
        }
        $('bioReporteChecadas').disabled = true;
        $('bioReporteResumen').disabled = true;
        $('bioReportesEstado').textContent = mensaje;
    }

    async function cargarTablaExtra(clave) {
        const t = tablasExtra[clave];
        if (!t || sincronizando) return;
        if (clave !== 'sincronizacion' && !fechasValidas()) return;
        const referencia = $('bioHoraReferencia').value;
        if (clave === 'puntualidad' && !referencia) {
            pintarFilas($(t.prefix + 'Tabla'), [], t.cols, 'Indica una hora de referencia para analizar primeros registros.');
            $('bioPuntualidadEstado').textContent = 'Sin referencia horaria; no se calculan observaciones.';
            return;
        }
        t.controller?.abort();
        const current = t.controller = new AbortController();
        const q = params({
            ...(clave === 'sincronizacion' ? {} : filtros()),
            ...(clave === 'puntualidad' ? { horaReferencia: referencia, toleranciaMinutos: $('bioTolerancia').value } : {}),
            ...(['jornadas', 'incidencias'].includes(clave) ? { buscar: $(t.prefix + 'Buscar').value.trim() } : {}),
            pagina: t.page, tamano: clave === 'sincronizacion' ? 10 : $(t.prefix + 'Tamano').value
        });
        try {
            const r = await fetch(`?handler=${t.handler}&${q}`, { signal: current.signal, headers: { Accept: 'application/json' } });
            const d = await r.json();
            if (!r.ok || !d.success) throw Error(d.message || 'Error al consultar la sección');
            t.page = d.pagina;
            const rows = (d.registros || []).map(x => {
                if (clave === 'puntualidad') return [textoFecha(x.fecha), `${x.nombre || ''} (${x.codigo})`, x.departamento || '—',
                fechaHora(x.primeraChecada).hora, x.horaLimiteExploratoria, x.observacion];
                if (clave === 'jornadas') return [textoFecha(x.fecha), `${x.nombre || ''} (${x.codigo})`, x.departamento || '—',
                num(x.totalChecadas), fechaHora(x.primeraChecada).hora, fechaHora(x.ultimaChecada).hora, spanMinutos(x.minutosEntreExtremos)];
                if (clave === 'incidencias') return [textoFecha(x.fecha), `${x.nombre || ''} (${x.codigo})`,
                x.departamento || '—', num(x.totalChecadas), x.observacion];
                return [textoFechaHora(x.inicioUtc), `${textoFecha(x.fechaInicioConsulta)} al ${textoFecha(x.fechaFinConsulta)}`,
                x.estado, num(x.empleadosProcesados), num(x.checadasInsertadas), textoFechaHora(x.finUtc), x.error || '—'];
            });
            pintarFilas($(t.prefix + 'Tabla'), rows, t.cols, 'Sin registros para esta consulta.');
            $(t.prefix + 'Pagina').textContent = `${d.pagina} / ${d.paginas}`;
            $(t.prefix + 'Prev').disabled = d.pagina <= 1;
            $(t.prefix + 'Next').disabled = d.pagina >= d.paginas;
            $(t.prefix + 'Info').textContent = `${num(d.total)} registros`;
            if (clave !== 'sincronizacion') $(t.prefix + 'Estado').textContent = `${num(d.total)} filas analizadas`;
        } catch (e) {
            if (e.name === 'AbortError') return;
            console.error(`BioTime ${clave}:`, e);
            $(t.prefix + 'Info').textContent = `Error: ${e.message}`;
        }
    }

    function activarPestana(clave) {
        if (!pestañas.includes(clave)) return;
        pestanaActual = clave;
        for (const tab of document.querySelectorAll('[data-bio-tab]')) {
            const activa = tab.dataset.bioTab === clave;
            tab.classList.toggle('active', activa);
            tab.setAttribute('aria-pressed', String(activa));
        }
        for (const pane of document.querySelectorAll('[data-bio-pane]')) pane.hidden = pane.dataset.bioPane !== clave;
        // Reportes y auditoría no provocan importación histórica al abrirse.
        if (clave === 'sincronizacion') cargarTablaExtra(clave);
        else if (clave === 'reportes') {
            const habilitar = fechasValidas() && !sincronizando;
            $('bioReporteChecadas').disabled = !habilitar;
            $('bioReporteResumen').disabled = !habilitar;
            $('bioReportesEstado').textContent = habilitar ? 'Se exportará el periodo y filtros seleccionados.' : 'Selecciona ambas fechas y espera la sincronización.';
        }
        else if (clave !== 'dashboard' && fechasValidas() && !sincronizando) cargarTablaExtra(clave);
    }

    function registrarPestanas() {
        for (const tab of document.querySelectorAll('[data-bio-tab]')) {
            tab.addEventListener('click', () => activarPestana(tab.dataset.bioTab));
        }
        for (const [clave, t] of Object.entries(tablasExtra)) {
            $(t.prefix + 'Prev').addEventListener('click', () => {
                if (t.page > 1) { t.page--; cargarTablaExtra(clave); }
            });
            $(t.prefix + 'Next').addEventListener('click', () => {
                if (!$(t.prefix + 'Next').disabled) { t.page++; cargarTablaExtra(clave); }
            });
            if (clave === 'sincronizacion') continue;
            $(t.prefix + 'Tamano').addEventListener('change', () => { t.page = 1; cargarTablaExtra(clave); });
            if (clave === 'puntualidad') {
                ['bioHoraReferencia', 'bioTolerancia'].forEach(id => $(id).addEventListener('change', () => {
                    t.page = 1; cargarTablaExtra(clave);
                }));
            } else {
                let delayed;
                $(t.prefix + 'Buscar').addEventListener('input', () => {
                    clearTimeout(delayed);
                    delayed = setTimeout(() => { t.page = 1; cargarTablaExtra(clave); }, 300);
                });
            }
        }
        $('bioReporteChecadas').addEventListener('click', () => descargarExcel('checadas'));
        $('bioReporteResumen').addEventListener('click', () => descargarExcel('resumen'));
        resetExtras();
        activarPestana('dashboard');
    }

    function refrescarPestanaActiva() {
        if (pestanaActual === 'reportes') activarPestana('reportes');
        else if (pestanaActual !== 'dashboard') cargarTablaExtra(pestanaActual);
    }

    async function init() {
        // Sin periodo predeterminado: no se consulta ni sincroniza al entrar.
        $('bioInicio').value = '';
        $('bioFin').value = '';
        limpiarResultados();
        registrarEventosResumenDiario();
        registrarColaboradores();
        registrarPestanas();
        $('bioExportarChecadas').addEventListener('click', () => descargarExcel('checadas'));
        $('bioExportarResumen').addEventListener('click', () => descargarExcel('resumen'));
        estado('Selecciona fecha inicial y fecha final');
        ['bioInicio', 'bioFin'].forEach(id => $(id).addEventListener('change', cambioDeFecha));
        $('bioDept').addEventListener('change', () => { actualizarColaboradores(); actualizarFiltros(); });
        $('bioEmpleado').addEventListener('change', actualizarFiltros);
        $('bioTamano').addEventListener('change', () => { if (!fechasValidas() || sincronizando) return; paginaActual = 1; cargarTabla().catch(console.error); });
        $('bioBuscar').addEventListener('input', () => { clearTimeout(debounce); if (!fechasValidas() || sincronizando) return; debounce = setTimeout(() => { paginaActual = 1; cargarTabla().catch(console.error); }, 300); });
        $('bioAnterior').addEventListener('click', () => { if (fechasValidas() && !sincronizando && paginaActual > 1) { paginaActual--; cargarTabla().catch(console.error); } });
        $('bioSiguiente').addEventListener('click', () => { if (!fechasValidas() || sincronizando || $('bioSiguiente').disabled) return; paginaActual++; cargarTabla().catch(console.error); });
        try {
            await cargarCatalogos();
            // No consultar dashboard, tabla ni crear trabajos hasta seleccionar ambas fechas.
        } catch (e) { console.error('BioTime inicio:', e); estado('Error al inicializar'); }
    }
    document.addEventListener('DOMContentLoaded', init);
})();
