
/* =====================================================
   ERPSEI - TIME INTEGRATION
   CONSULTA DE REGISTROS CYBERTRACK
   ===================================================== */

// =====================================================
// NOMBRES AMIGABLES DE COLUMNAS
// =====================================================

const nombresColumnas = {
    id: "ID",
    fecha: "Fecha",
    horas: "Horas registradas",
    actividad_id: "ID de actividad",
    usuario_id: "ID de usuario",
    empresa_id: "ID de empresa",
    departamento_id: "ID de departamento",
    periodo_id: "ID de periodo",
    clave_id: "ID de clave",
    grupo_id: "ID de grupo",
    departamento_actividad_id: "ID de actividad departamental",
    nombre: "Nombre",
    email: "Correo electrónico",
    proyecto: "Proyecto",
    clave: "Clave",
    actividad: "Actividad",
    descripcion: "Descripción",
    estatus: "Estatus",
    tipo: "Tipo",
    orden: "Orden",
    incremental: "Incremental",
    dia_inicio: "Día de inicio",
    dia_fin: "Día de término",
    mes: "Mes",
    anio: "Año",
    created_at: "Fecha de creación",
    updated_at: "Última actualización"
};

// =====================================================
// FORMATEO Y SEGURIDAD DE VALORES
// =====================================================

function escaparHtml(valor) {
    return $("<div>")
        .text(String(valor))
        .html();
}

function formatearValor(valor, campo) {

    if (valor === null || valor === undefined || valor === "") {
        return "-";
    }

    if (typeof valor === "object") {
        return escaparHtml(JSON.stringify(valor));
    }

    if (
        campo === "created_at" ||
        campo === "updated_at"
    ) {
        const texto = String(valor);

        const coincidencia = texto.match(
            /^(\d{4})-(\d{2})-(\d{2})[ T](\d{2}:\d{2}:\d{2})/
        );

        if (coincidencia) {
            return escaparHtml(
                `${coincidencia[3]}/${coincidencia[2]}/${coincidencia[1]} ${coincidencia[4]}`
            );
        }
    }

    if (
        campo === "fecha" ||
        campo === "fecha_inicio" ||
        campo === "fecha_fin"
    ) {
        const texto = String(valor);

        const coincidencia = texto.match(
            /^(\d{4})-(\d{2})-(\d{2})$/
        );

        if (coincidencia) {
            return escaparHtml(
                `${coincidencia[3]}/${coincidencia[2]}/${coincidencia[1]}`
            );
        }
    }

    return escaparHtml(valor);
}

// =====================================================
// INICIALIZACIÓN DEL MÓDULO
// =====================================================

$(document).ready(function () {

    // =================================================
    // ESTADO DE PAGINACIÓN
    // =================================================

    let historial = [];
    let afterId = 0;
    let upperId = null;
    let nextAfterId = null;
    let hasMore = false;
    let cargando = false;

    const $tabla = $("#timeTablaDatos");

    // =================================================
    // ACTUALIZAR ESTADO DE CONTROLES
    // =================================================

    function actualizarBotones() {

        $("#btnTimeAnterior").prop(
            "disabled",
            cargando || historial.length === 0
        );

        $("#btnTimeSiguiente").prop(
            "disabled",
            cargando ||
            !hasMore ||
            nextAfterId === null
        );

        $("#btnTimeConsultar").prop(
            "disabled",
            cargando
        );

        $("#timeTabla").prop(
            "disabled",
            cargando
        );

        $("#timeLimit").prop(
            "disabled",
            cargando
        );
    }

    // =================================================
    // MENSAJES
    // =================================================

    function mostrarError(mensaje) {

        $("#timeError")
            .text(mensaje)
            .removeClass("d-none");
    }

    function ocultarError() {

        $("#timeError")
            .addClass("d-none")
            .text("");
    }

    // =================================================
    // REINICIAR CONSULTA
    // =================================================

    async function reiniciarConsulta() {

        if (cargando) {
            return;
        }

        const historialAnterior = [...historial];
        const cursorAnterior = afterId;
        const upperIdAnterior = upperId;
        const siguienteAnterior = nextAfterId;
        const hasMoreAnterior = hasMore;

        historial = [];
        afterId = 0;
        upperId = null;
        nextAfterId = null;
        hasMore = false;

        const correcto = await cargarRegistros();

        if (!correcto) {
            historial = historialAnterior;
            afterId = cursorAnterior;
            upperId = upperIdAnterior;
            nextAfterId = siguienteAnterior;
            hasMore = hasMoreAnterior;

            actualizarBotones();
        }
    }

    // =================================================
    // CONSULTAR REGISTROS
    // =================================================

    async function cargarRegistros() {

        if (cargando) {
            return false;
        }

        const tabla = $("#timeTabla").val();

        const limit = parseInt(
            $("#timeLimit").val(),
            10
        );

        cargando = true;
        actualizarBotones();

        ocultarError();

        $("#timeEstado").text(
            "Consultando TIME..."
        );

        try {

            // =========================================
            // PARÁMETROS DEL HANDLER RAZOR
            // =========================================

            const parametros = new URLSearchParams({
                handler: "Registros",
                tabla: tabla,
                afterId: String(afterId),
                limit: String(limit)
            });

            if (upperId !== null) {
                parametros.set(
                    "upperId",
                    String(upperId)
                );
            }

            const url =
                window.location.pathname +
                "?" +
                parametros.toString();

            // =========================================
            // PETICIÓN AL BACKEND ERPSEI
            // =========================================

            const response = await fetch(url, {
                method: "GET",
                credentials: "same-origin",
                headers: {
                    "Accept": "application/json"
                }
            });

            if (!response.ok) {
                throw new Error(
                    "Error HTTP " + response.status
                );
            }

            const resultado =
                await response.json();

            if (!resultado.success) {
                throw new Error(
                    "TIME no devolvió una respuesta exitosa."
                );
            }

            // =========================================
            // VALIDACIÓN DE RESPUESTA
            // =========================================

            if (
                !Array.isArray(resultado.data) ||
                !resultado.sync
            ) {
                throw new Error(
                    "La API devolvió una estructura inválida."
                );
            }

            const registros = resultado.data;
            const sync = resultado.sync;

            const nuevoUpperId =
                upperId === null
                    ? sync.upper_id
                    : upperId;

            const nuevoHasMore =
                sync.has_more === true;

            const nuevoNextAfterId =
                sync.next_after_id;

            if (
                nuevoUpperId === null ||
                nuevoUpperId === undefined ||
                !Number.isSafeInteger(nuevoUpperId) ||
                nuevoUpperId < 0
            ) {
                throw new Error(
                    "TIME devolvió un límite superior inválido."
                );
            }

            if (
                nuevoHasMore &&
                (
                    !Number.isSafeInteger(nuevoNextAfterId) ||
                    nuevoNextAfterId <= afterId
                )
            ) {
                throw new Error(
                    "TIME devolvió un cursor de paginación inválido."
                );
            }

            // =========================================
            // CONSTRUIR COLUMNAS DINÁMICAS
            // =========================================

            const columnas =
                registros.length > 0
                    ? Object.keys(registros[0]).map(
                        campo => ({
                            field: campo,

                            title:
                                nombresColumnas[campo] ||
                                campo.replaceAll("_", " "),

                            sortable: true,

                            formatter: function (valor) {
                                return formatearValor(
                                    valor,
                                    campo
                                );
                            }
                        })
                    )
                    : [];

            // =========================================
            // ACTUALIZAR TABLA
            // =========================================

            $tabla.bootstrapTable("destroy");

            if (columnas.length > 0) {

                $tabla.bootstrapTable({
                    columns: columnas,
                    data: registros,
                    pagination: false,
                    search: true,
                    sortStable: true,
                    classes:
                        "table table-striped table-bordered table-sm"
                });

            } else {

                $tabla.empty();
            }

            // =========================================
            // CONFIRMAR ESTADO SOLO TRAS ÉXITO
            // =========================================

            upperId = nuevoUpperId;
            hasMore = nuevoHasMore;
            nextAfterId = nuevoNextAfterId;

            // =========================================
            // ACTUALIZAR INDICADORES
            // =========================================

            $("#timeTitulo").text(
                "Registros de " + tabla
            );

            $("#timeEstado").text(
                registros.length +
                " registros en esta página"
            );

            $("#timePagina").text(
                "Página " +
                (historial.length + 1)
            );

            return true;

        } catch (error) {

            mostrarError(
                "No fue posible obtener los registros: " +
                error.message
            );

            $("#timeEstado").text(
                "Error de consulta"
            );

            return false;

        } finally {

            cargando = false;
            actualizarBotones();

        }
    }

    // =================================================
    // CONSULTAR
    // =================================================

    $("#btnTimeConsultar").on(
        "click",
        async function () {
            await reiniciarConsulta();
        }
    );

    // =================================================
    // SIGUIENTE PÁGINA
    // =================================================

    $("#btnTimeSiguiente").on(
        "click",
        async function () {

            if (
                cargando ||
                !hasMore ||
                nextAfterId === null
            ) {
                return;
            }

            const cursorAnterior = afterId;
            const historialAnterior = [...historial];
            const upperIdAnterior = upperId;
            const siguienteAnterior = nextAfterId;
            const hasMoreAnterior = hasMore;

            historial.push(afterId);
            afterId = nextAfterId;

            const correcto =
                await cargarRegistros();

            if (!correcto) {

                afterId = cursorAnterior;
                historial = historialAnterior;
                upperId = upperIdAnterior;
                nextAfterId = siguienteAnterior;
                hasMore = hasMoreAnterior;

                actualizarBotones();
            }
        }
    );

    // =================================================
    // PÁGINA ANTERIOR
    // =================================================

    $("#btnTimeAnterior").on(
        "click",
        async function () {

            if (
                cargando ||
                historial.length === 0
            ) {
                return;
            }

            const cursorAnterior = afterId;
            const historialAnterior = [...historial];
            const upperIdAnterior = upperId;
            const siguienteAnterior = nextAfterId;
            const hasMoreAnterior = hasMore;

            afterId = historial.pop();

            const correcto =
                await cargarRegistros();

            if (!correcto) {

                afterId = cursorAnterior;
                historial = historialAnterior;
                upperId = upperIdAnterior;
                nextAfterId = siguienteAnterior;
                hasMore = hasMoreAnterior;

                actualizarBotones();
            }
        }
    );

    // =================================================
    // CAMBIO DE TABLA
    // =================================================

    $("#timeTabla").on(
        "change",
        async function () {
            await reiniciarConsulta();
        }
    );

    // =================================================
    // CAMBIO DE CANTIDAD POR PÁGINA
    // =================================================

    $("#timeLimit").on(
        "change",
        async function () {
            await reiniciarConsulta();
        }
    );

    // =================================================
    // CARGA INICIAL
    // =================================================

    cargarRegistros();

});
