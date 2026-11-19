(() => {
    'use strict';
    const $ = id => document.getElementById(id);
    let chart = null, dashboardController = null, tablaController = null;
    let syncController = null, debounce = null, debounceFechas = null;
    let paginaActual = 1, empleadosBioTime = [], versionFechas = 0;
    let sincronizando = false;
    let trabajoActivo = null;

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

    async function refrescarVista() {
        paginaActual = 1;
        await Promise.all([cargarDashboard(), cargarTabla()]);
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

    async function init() {
        // Sin periodo predeterminado: no se consulta ni sincroniza al entrar.
        $('bioInicio').value = '';
        $('bioFin').value = '';
        limpiarResultados();
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
