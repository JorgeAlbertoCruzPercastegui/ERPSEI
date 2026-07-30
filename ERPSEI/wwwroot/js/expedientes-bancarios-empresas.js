document.addEventListener("DOMContentLoaded", function () {
    document.body.classList.add(
        "module-main-theme",
        "expedientes-bancarios-page"
    );

    inicializarEventosEmpresas();

    if (
        window.jQuery &&
        typeof window.jQuery.fn.bootstrapTable === "function"
    ) {
        inicializarTablaEmpresas();
    } else {
        console.error(
            "Bootstrap Table no está disponible."
        );
    }
});

let modalEmpresa;
let modalEliminarEmpresa;
let modalResultado;
let modalAccionistas;

let empresaIdEliminar = 0;
let empresasIdsEliminar = [];
let modoEliminacion = "individual";
let modoEmpresa = "crear";

let empresaIdAccionistas = 0;
let empresaNombreAccionistas = "";

let modalAccionista;
let modalEliminarAccionista;

let modoAccionista = "crear";
let accionistaIdEliminar = 0;

let modalDocumentos;
let modalCargarDocumento;

let empresaIdDocumentos = 0;
let empresaNombreDocumentos = "";

function inicializarEventosEmpresas() {

    modalCargarDocumento = obtenerInstanciaModal(
        "modalCargarDocumento"
    );

    modalEmpresa = obtenerInstanciaModal(
        "modalEmpresa"
    );

    modalEliminarEmpresa = obtenerInstanciaModal(
        "modalEliminarEmpresa"
    );

    modalResultado = obtenerInstanciaModal(
        "modalResultado"
    );

    modalAccionistas = obtenerInstanciaModal(
        "modalAccionistas"
    );

    modalAccionista = obtenerInstanciaModal(
        "modalAccionista"
    );

    modalEliminarAccionista = obtenerInstanciaModal(
        "modalEliminarAccionista"
    );

    modalDocumentos = obtenerInstanciaModal(
        "modalDocumentos"
    );

    document
        .getElementById("btnGuardarDocumento")
        ?.addEventListener(
            "click",
            guardarDocumento
        );

    document
        .getElementById("btnBuscarEmpresas")
        ?.addEventListener(
            "click",
            refrescarTablaEmpresas
        );

    document
        .getElementById("filtroEstatus")
        ?.addEventListener(
            "change",
            refrescarTablaEmpresas
        );

    document
        .getElementById("filtroBusqueda")
        ?.addEventListener(
            "keydown",
            function (event) {
                if (event.key === "Enter") {
                    event.preventDefault();
                    refrescarTablaEmpresas();
                }
            }
        );

    document
        .getElementById("btnNuevaEmpresa")
        ?.addEventListener(
            "click",
            abrirModalCrearEmpresa
        );

    document
        .getElementById("btnGuardarEmpresa")
        ?.addEventListener(
            "click",
            guardarEmpresa
        );

    document
        .getElementById(
            "btnConfirmarEliminarEmpresa"
        )
        ?.addEventListener(
            "click",
            eliminarEmpresa
        );

    document
        .getElementById(
            "btnEliminarSeleccionadas"
        )
        ?.addEventListener(
            "click",
            confirmarEliminarSeleccionadas
        );

    document
        .getElementById("ebRfc")
        ?.addEventListener(
            "input",
            function () {
                this.value = this.value
                    .toUpperCase()
                    .replace(/\s+/g, "");
            }
        );

    if (
        window.jQuery &&
        typeof window.jQuery.fn.bootstrapTable ===
        "function"
    ) {
        $("#tablaEmpresas").on(
            "check.bs.table " +
            "uncheck.bs.table " +
            "check-all.bs.table " +
            "uncheck-all.bs.table",
            actualizarBotonSeleccionados
        );
    }

    document
        .getElementById("btnNuevoAccionista")
        ?.addEventListener(
            "click",
            abrirModalCrearAccionista
        );

    document
        .getElementById("btnGuardarAccionista")
        ?.addEventListener(
            "click",
            guardarAccionista
        );

    document
        .getElementById(
            "btnConfirmarEliminarAccionista"
        )
        ?.addEventListener(
            "click",
            eliminarAccionista
        );

    document
        .getElementById("eaRfc")
        ?.addEventListener(
            "input",
            function () {
                this.value = this.value
                    .toUpperCase()
                    .replace(/\s+/g, "");
            }
        );

    document
        .getElementById(
            "btnCargarDocumentoSeleccionado"
        )
        ?.addEventListener(
            "click",
            function () {
                if (!tipoDocumentoSeleccionado) {
                    mostrarResultado(
                        "Selecciona primero un tipo de documento.",
                        "error"
                    );

                    return;
                }

                prepararCargaDocumento(
                    tipoDocumentoSeleccionado.id
                );
            }
        );

    document
        .getElementById("modalEliminarEmpresa")
        ?.addEventListener(
            "hidden.bs.modal",
            function () {
                empresaIdEliminar = 0;
                empresasIdsEliminar = [];
                modoEliminacion = "individual";
            }
        );

    document
        .getElementById(
            "modalEliminarAccionista"
        )
        ?.addEventListener(
            "hidden.bs.modal",
            function () {
                const debeRegresar =
                    accionistaIdEliminar > 0;

                accionistaIdEliminar = 0;

                if (
                    debeRegresar &&
                    modalAccionistas
                ) {
                    modalAccionistas.show();
                }
            }
        );

    document
        .getElementById("modalDocumentos")
        ?.addEventListener(
            "hidden.bs.modal",
            function () {
                /*
                 * No limpiar la empresa cuando el modal se oculta
                 * para abrir el formulario de carga.
                 */
                const modalCargaVisible =
                    document
                        .getElementById("modalCargarDocumento")
                        ?.classList.contains("show");

                if (modalCargaVisible) {
                    return;
                }
            }
        );
}

function obtenerInstanciaModal(id) {
    const elemento = document.getElementById(id);

    if (!elemento) {
        console.warn(`No se encontró el modal #${id}.`);
        return null;
    }

    if (
        typeof bootstrap === "undefined" ||
        !bootstrap.Modal
    ) {
        console.error(
            "Bootstrap Modal no está disponible."
        );

        return null;
    }

    return bootstrap.Modal.getOrCreateInstance(
        elemento
    );
}

function inicializarTablaEmpresas() {
    const tabla = $("#tablaEmpresas");

    tabla.bootstrapTable({
        url: construirUrlEmpresas(),
        method: "get",
        toolbar: "#toolbarEmpresas",

        detailView: true,
        detailFormatter: detalleEmpresaFormatter,

        search: true,
        pagination: true,
        sidePagination: "client",

        pageSize: 10,
        pageList: [10, 20, 30, 50],

        sortName: "id",
        sortOrder: "asc",

        paginationLoop: false,
        showRefresh: true,
        showColumns: true,

        uniqueId: "id",
        locale: "es-MX",
        height: undefined,

        onPostBody: function () {
            actualizarBotonSeleccionados();
        }
    });
}



function construirUrlEmpresas() {
    const busqueda = document
        .getElementById("filtroBusqueda")
        ?.value?.trim() ?? "";

    const estatus = document
        .getElementById("filtroEstatus")
        ?.value ?? "Activas";

    const params = new URLSearchParams({
        handler: "Empresas",
        busqueda: busqueda,
        estatus: estatus
    });

    return `${window.location.pathname}?${params.toString()}`;
}

function refrescarTablaEmpresas() {
    $("#tablaEmpresas").bootstrapTable("refresh", {
        url: construirUrlEmpresas()
    });
}

function actualizarBotonSeleccionados() {
    const seleccionadas = $("#tablaEmpresas")
        .bootstrapTable("getSelections");

    const boton = document.getElementById(
        "btnEliminarSeleccionadas"
    );

    if (!boton) {
        return;
    }

    const haySeleccionadas = seleccionadas.length > 0;

    boton.disabled = !haySeleccionadas;

    boton.classList.toggle(
        "btn-danger",
        haySeleccionadas
    );

    boton.classList.toggle(
        "btn-outline-secondary",
        !haySeleccionadas
    );
}

function detalleEmpresaFormatter(index, row) {
    return `
        <div class="eb-detail-panel">
            <div class="eb-detail-grid">

                <div class="eb-detail-item">
                    <strong>Fecha de constitución:</strong>
                    <span>${formatearFecha(row.fechaConstitucion)}</span>
                </div>

                <div class="eb-detail-item">
                    <strong>Teléfono:</strong>
                    <span>${valorTexto(row.telefonoBancos)}</span>
                </div>

                <div class="eb-detail-item">
                    <strong>Correo:</strong>
                    <span>${valorTexto(row.correoBancos)}</span>
                </div>

                <div class="eb-detail-item">
                    <strong>Escritura:</strong>
                    <span>${valorTexto(row.numeroEscritura)}</span>
                </div>

                <div class="eb-detail-item eb-detail-item-wide">
                    <strong>Domicilio fiscal:</strong>
                    <span>${valorTexto(row.domicilioFiscal)}</span>
                </div>

                <div class="eb-detail-item eb-detail-item-wide">
                    <strong>Actividad comercial:</strong>
                    <span>${valorTexto(row.actividadComercial)}</span>
                </div>

                <div class="eb-detail-item eb-detail-item-wide">
                    <strong>Observaciones:</strong>
                    <span>${valorTexto(row.observaciones)}</span>
                </div>

            </div>
        </div>
    `;
}

function estatusEmpresaFormatter(value, row) {
    if (row.deshabilitado) {
        return `
            <span class="eb-status eb-status-inactive">
                <i class="fa-solid fa-circle"></i>
                Inactiva
            </span>
        `;
    }

    return `
        <span class="eb-status eb-status-active">
            <i class="fa-solid fa-circle"></i>
            Activa
        </span>
    `;
}

function accionesEmpresaFormatter(value, row) {
    const accionEstatus = row.deshabilitado
        ? "Habilitar"
        : "Deshabilitar";

    const iconoEstatus = row.deshabilitado
        ? "fa-circle-check"
        : "fa-ban";

    return `
    <div class="dropdown eb-actions-dropdown">

        <button type="button"
                class="btn btn-sm eb-action-button"
                data-bs-toggle="dropdown"
                data-bs-boundary="viewport"
                data-bs-offset="8,0"
                aria-expanded="false"
                title="Acciones">

            <span class="eb-action-dots">⋮</span>
        </button>

        <ul class="dropdown-menu eb-actions-menu">

                <li>
                    <button type="button"
                            class="dropdown-item"
                            onclick="consultarEmpresa(${row.id})">
                        <i class="fa-regular fa-eye me-2"></i>
                        Consultar empresa
                    </button>
                </li>

                <li>
                    <button type="button"
                            class="dropdown-item"
                            onclick="editarEmpresa(${row.id})">
                        <i class="fa-regular fa-pen-to-square me-2"></i>
                        Editar empresa
                    </button>
                </li>

                <li>
                    <button type="button"
                            class="dropdown-item"
                            onclick="abrirAccionistasEmpresa(
                                ${row.id},
                                '${escaparAtributoJs(row.razonSocial)}'
                            )">
                        <i class="fa-solid fa-users me-2"></i>
                        Accionistas
                    </button>
                </li>

                <li>
                    <button type="button"
                            class="dropdown-item"
                            onclick="abrirDocumentosEmpresa(
                                ${row.id},
                                '${escaparAtributoJs(row.razonSocial)}'
                            )">

                        <i class="fa-regular fa-folder-open me-2"></i>
                        Documentos
                    </button>
                </li>

                <li>
                    <hr class="dropdown-divider" />
                </li>

                <li>
                    <button type="button"
                            class="dropdown-item"
                            onclick="cambiarEstatusEmpresa(${row.id})">
                        <i class="fa-solid ${iconoEstatus} me-2"></i>
                        ${accionEstatus}
                    </button>
                </li>

                <li>
                    <button type="button"
                            class="dropdown-item text-danger"
                            onclick="confirmarEliminarEmpresa(
                                ${row.id},
                                '${escaparAtributoJs(row.razonSocial)}'
                            )">
                        <i class="fa-regular fa-trash-can me-2"></i>
                        Eliminar
                    </button>
                </li>

            </ul>
        </div>
    `;
}

function abrirModalCrearEmpresa() {
    if (!modalEmpresa) {
        console.error(
            "No se encontró el modal de empresa."
        );

        return;
    }

    modoEmpresa = "crear";

    limpiarFormularioEmpresa();
    habilitarFormularioEmpresa(true);

    const titulo = document.getElementById(
        "modalEmpresaTitulo"
    );

    if (titulo) {
        titulo.textContent = "Nueva empresa";
    }

    const botonGuardar = document.getElementById(
        "btnGuardarEmpresa"
    );

    if (botonGuardar) {
        botonGuardar.classList.remove("d-none");
        botonGuardar.disabled = false;
    }

    modalEmpresa.show();

    setTimeout(function () {
        document
            .getElementById("ebRazonSocial")
            ?.focus();
    }, 250);
}

async function consultarEmpresa(id) {
    if (!modalEmpresa) {
        console.error(
            "No se encontró el modal de empresa."
        );

        return;
    }

    modoEmpresa = "consultar";

    const respuesta = await obtenerEmpresa(id);

    if (!respuesta?.success) {
        mostrarResultado(
            respuesta?.message ??
            "No fue posible consultar la empresa.",
            "error"
        );

        return;
    }

    cargarFormularioEmpresa(
        respuesta.data
    );

    habilitarFormularioEmpresa(false);

    const titulo = document.getElementById(
        "modalEmpresaTitulo"
    );

    if (titulo) {
        titulo.textContent =
            "Consultar empresa";
    }

    document
        .getElementById("btnGuardarEmpresa")
        ?.classList.add("d-none");

    modalEmpresa.show();
}

async function editarEmpresa(id) {
    if (!modalEmpresa) {
        console.error(
            "No se encontró el modal de empresa."
        );

        return;
    }

    modoEmpresa = "editar";

    const respuesta = await obtenerEmpresa(id);

    if (!respuesta?.success) {
        mostrarResultado(
            respuesta?.message ??
            "No fue posible cargar la empresa.",
            "error"
        );

        return;
    }

    cargarFormularioEmpresa(
        respuesta.data
    );

    habilitarFormularioEmpresa(true);

    const titulo = document.getElementById(
        "modalEmpresaTitulo"
    );

    if (titulo) {
        titulo.textContent =
            "Editar empresa";
    }

    document
        .getElementById("btnGuardarEmpresa")
        ?.classList.remove("d-none");

    modalEmpresa.show();
}

async function obtenerEmpresa(id) {
    try {
        const params = new URLSearchParams({
            handler: "Empresa",
            id: id
        });

        const response = await fetch(
            `${window.location.pathname}?${params.toString()}`
        );

        return await response.json();
    } catch {
        return null;
    }
}

function cargarFormularioEmpresa(data) {
    document.getElementById("ebId").value = data.id ?? 0;
    document.getElementById("ebRazonSocial").value =
        data.razonSocial ?? "";
    document.getElementById("ebNombreCorto").value =
        data.nombreCorto ?? "";
    document.getElementById("ebRfc").value =
        data.rfc ?? "";
    document.getElementById("ebNivel").value =
        data.nivel ?? "";
    document.getElementById("ebActividadComercial").value =
        data.actividadComercial ?? "";
    document.getElementById("ebTelefonoBancos").value =
        data.telefonoBancos ?? "";
    document.getElementById("ebCorreoBancos").value =
        data.correoBancos ?? "";
    document.getElementById("ebFechaConstitucion").value =
        data.fechaConstitucion ?? "";
    document.getElementById("ebNumeroEscritura").value =
        data.numeroEscritura ?? "";
    document.getElementById("ebDomicilioFiscal").value =
        data.domicilioFiscal ?? "";
    document.getElementById("ebObservaciones").value =
        data.observaciones ?? "";

    limpiarErroresEmpresa();
}

function limpiarFormularioEmpresa() {
    const formulario = document.getElementById("formEmpresa");

    if (formulario) {
        formulario.reset();
    }

    const valoresIniciales = {
        ebId: "0",
        ebRazonSocial: "",
        ebNombreCorto: "",
        ebRfc: "",
        ebNivel: "",
        ebActividadComercial: "",
        ebTelefonoBancos: "",
        ebCorreoBancos: "",
        ebFechaConstitucion: "",
        ebNumeroEscritura: "",
        ebDomicilioFiscal: "",
        ebObservaciones: ""
    };

    Object.entries(valoresIniciales).forEach(
        function ([id, valor]) {
            const elemento = document.getElementById(id);

            if (elemento) {
                elemento.value = valor;
            }
        }
    );

    limpiarErroresEmpresa();

    document
        .querySelectorAll("#formEmpresa .is-invalid")
        .forEach(function (elemento) {
            elemento.classList.remove("is-invalid");
        });
}

function habilitarFormularioEmpresa(habilitado) {
    const campos = document.querySelectorAll(
        "#formEmpresa input:not([type='hidden']), " +
        "#formEmpresa textarea, " +
        "#formEmpresa select"
    );

    campos.forEach(function (campo) {
        campo.disabled = !habilitado;
    });
}

async function guardarEmpresa() {
    limpiarErroresEmpresa();

    const request = obtenerRequestEmpresa();

    const handler = modoEmpresa === "editar"
        ? "Editar"
        : "Crear";

    const boton = document.getElementById("btnGuardarEmpresa");

    boton.disabled = true;
    const htmlOriginal = boton.innerHTML;

    boton.innerHTML =
        '<i class="fa-solid fa-spinner fa-spin me-1"></i>' +
        " Guardando...";

    try {
        const respuesta = await enviarJson(handler, request);

        if (!respuesta.success) {
            mostrarErroresEmpresa(respuesta.errors);

            if (!respuesta.errors) {
                mostrarResultado(
                    respuesta.message ??
                    "No fue posible completar la operación.",
                    "error"
                );
            }

            return;
        }

        modalEmpresa.hide();
        refrescarTablaEmpresas();

        mostrarResultado(
            respuesta.message,
            "success"
        );
    } catch {
        mostrarResultado(
            "Ocurrió un error al guardar la empresa.",
            "error"
        );
    } finally {
        boton.disabled = false;
        boton.innerHTML = htmlOriginal;
    }
}

function obtenerRequestEmpresa() {
    return {
        id: parseInt(
            document.getElementById("ebId").value || "0",
            10
        ),
        razonSocial:
            document.getElementById("ebRazonSocial").value,
        nombreCorto:
            document.getElementById("ebNombreCorto").value,
        rfc:
            document.getElementById("ebRfc").value,
        nivel:
            document.getElementById("ebNivel").value,
        actividadComercial:
            document.getElementById("ebActividadComercial").value,
        telefonoBancos:
            document.getElementById("ebTelefonoBancos").value,
        correoBancos:
            document.getElementById("ebCorreoBancos").value,
        fechaConstitucion:
            document.getElementById("ebFechaConstitucion").value || null,
        numeroEscritura:
            document.getElementById("ebNumeroEscritura").value,
        domicilioFiscal:
            document.getElementById("ebDomicilioFiscal").value,
        observaciones:
            document.getElementById("ebObservaciones").value
    };
}

async function cambiarEstatusEmpresa(id) {
    try {
        const respuesta = await enviarJson(
            "CambiarEstatus",
            { id: id }
        );

        if (!respuesta.success) {
            mostrarResultado(
                respuesta.message,
                "error"
            );
            return;
        }

        refrescarTablaEmpresas();

        mostrarResultado(
            respuesta.message,
            "success"
        );
    } catch {
        mostrarResultado(
            "No fue posible cambiar el estatus.",
            "error"
        );
    }
}

function confirmarEliminarEmpresa(id, razonSocial) {
    modoEliminacion = "individual";
    empresaIdEliminar = id;
    empresasIdsEliminar = [];

    const nombreEmpresa = document.getElementById(
        "nombreEmpresaEliminar"
    );

    const mensaje = document.getElementById(
        "mensajeConfirmacionEliminar"
    );

    if (nombreEmpresa) {
        nombreEmpresa.textContent = razonSocial;
    }

    if (mensaje) {
        mensaje.innerHTML = `
            ¿Estás seguro de que deseas eliminar la empresa
            <strong>${escaparHtml(razonSocial)}</strong>?
            Esta acción dejará de mostrar el registro en el módulo.
        `;
    }

    modalEliminarEmpresa.show();
}

async function eliminarEmpresa() {
    if (
        modoEliminacion === "individual" &&
        empresaIdEliminar <= 0
    ) {
        return;
    }

    if (
        modoEliminacion === "multiple" &&
        empresasIdsEliminar.length === 0
    ) {
        return;
    }

    const boton = document.getElementById(
        "btnConfirmarEliminarEmpresa"
    );

    boton.disabled = true;

    const htmlOriginal = boton.innerHTML;

    boton.innerHTML =
        '<i class="fa-solid fa-spinner fa-spin me-1"></i>' +
        " Eliminando...";

    try {
        if (modoEliminacion === "individual") {
            const respuesta = await enviarJson(
                "Eliminar",
                { id: empresaIdEliminar }
            );

            if (!respuesta.success) {
                modalEliminarEmpresa.hide();

                mostrarResultado(
                    respuesta.message ??
                    "No fue posible eliminar la empresa.",
                    "error"
                );

                return;
            }

            modalEliminarEmpresa.hide();

            empresaIdEliminar = 0;

            refrescarTablaEmpresas();

            mostrarResultado(
                respuesta.message,
                "success"
            );

            return;
        }

        const resultados = [];

        for (const id of empresasIdsEliminar) {
            const respuesta = await enviarJson(
                "Eliminar",
                { id: id }
            );

            resultados.push(respuesta);
        }

        const exitosos = resultados.filter(
            function (resultado) {
                return resultado.success;
            }
        );

        const fallidos = resultados.filter(
            function (resultado) {
                return !resultado.success;
            }
        );

        modalEliminarEmpresa.hide();

        empresasIdsEliminar = [];

        refrescarTablaEmpresas();

        actualizarBotonSeleccionados();

        if (fallidos.length === 0) {
            mostrarResultado(
                exitosos.length === 1
                    ? "La empresa seleccionada se eliminó correctamente."
                    : "Las empresas seleccionadas se eliminaron correctamente.",
                "success"
            );
        } else {
            mostrarResultado(
                `${exitosos.length} registros fueron eliminados y ` +
                `${fallidos.length} no pudieron eliminarse porque tienen información relacionada.`,
                "error"
            );
        }
    } catch {
        modalEliminarEmpresa.hide();

        mostrarResultado(
            "No fue posible completar la eliminación.",
            "error"
        );
    } finally {
        boton.disabled = false;
        boton.innerHTML = htmlOriginal;
    }
}

async function enviarJson(handler, data) {
    const token = document.querySelector(
        '#formToken input[name="__RequestVerificationToken"]'
    )?.value;

    const response = await fetch(
        `${window.location.pathname}?handler=${handler}`,
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "RequestVerificationToken": token ?? ""
            },
            body: JSON.stringify(data)
        }
    );

    return await response.json();
}

function mostrarErroresEmpresa(errors) {
    if (!errors) {
        return;
    }

    Object.entries(errors).forEach(function ([campo, mensajes]) {
        const elemento = document.querySelector(
            `[data-error-for="${campo}"]`
        );

        if (!elemento) {
            return;
        }

        elemento.textContent = mensajes?.[0] ?? "";
    });
}

function limpiarErroresEmpresa() {
    document
        .querySelectorAll("[data-error-for]")
        .forEach(function (elemento) {
            elemento.textContent = "";
        });
}


async function abrirAccionistasEmpresa(
    empresaId,
    razonSocial
) {
    if (!modalAccionistas) {
        console.error(
            "No se encontró el modal de accionistas."
        );

        return;
    }

    empresaIdAccionistas = empresaId;
    empresaNombreAccionistas = razonSocial;

    const nombreEmpresa = document.getElementById(
        "nombreEmpresaAccionistas"
    );

    if (nombreEmpresa) {
        nombreEmpresa.textContent =
            razonSocial;
    }

    limpiarListadoAccionistas();
    mostrarCargaAccionistas();

    modalAccionistas.show();

    await cargarAccionistasEmpresa();
}

async function cargarAccionistasEmpresa() {
    if (empresaIdAccionistas <= 0) {
        mostrarErrorAccionistas(
            "El identificador de la empresa no es válido."
        );

        return;
    }

    try {
        const parametros = new URLSearchParams({
            handler: "Accionistas",
            empresaId: empresaIdAccionistas.toString()
        });

        const response = await fetch(
            `${window.location.pathname}?${parametros.toString()}`
        );

        const resultado = await response.json();

        if (!resultado.success) {
            mostrarErrorAccionistas(
                resultado.message ??
                "No fue posible consultar los accionistas."
            );

            return;
        }

        actualizarResumenAccionistas(resultado.resumen);
        renderizarAccionistas(resultado.data);
    } catch (error) {
        console.error(error);

        mostrarErrorAccionistas(
            "Ocurrió un error al consultar los accionistas."
        );
    }
}

function actualizarResumenAccionistas(resumen) {
    document.getElementById(
        "totalAccionistas"
    ).textContent = resumen?.totalAccionistas ?? 0;

    document.getElementById(
        "porcentajeRegistrado"
    ).textContent = formatearPorcentaje(
        resumen?.porcentajeTotal
    );

    document.getElementById(
        "porcentajeDisponible"
    ).textContent = formatearPorcentaje(
        resumen?.porcentajeDisponible
    );
}

function renderizarAccionistas(accionistas) {
    const cuerpo = document.getElementById(
        "tablaAccionistasBody"
    );

    const estadoVacio = document.getElementById(
        "accionistasEstadoVacio"
    );

    const contenedorTabla = document.getElementById(
        "contenedorTablaAccionistas"
    );

    const carga = document.getElementById(
        "accionistasCargando"
    );

    carga?.classList.add("d-none");

    if (!cuerpo) {
        return;
    }

    if (!Array.isArray(accionistas) ||
        accionistas.length === 0) {

        cuerpo.innerHTML = "";

        contenedorTabla?.classList.add("d-none");
        estadoVacio?.classList.remove("d-none");

        return;
    }

    estadoVacio?.classList.add("d-none");
    contenedorTabla?.classList.remove("d-none");

    cuerpo.innerHTML = accionistas.map(function (accionista) {
        const representante = accionista.esRepresentanteLegal
            ? `
                <span class="eb-status eb-status-representative">
                    Sí
                </span>
            `
            : `
                <span class="text-muted">
                    No
                </span>
            `;

        const estatus = accionista.deshabilitado
            ? `
                <span class="eb-status eb-status-inactive">
                    Inactivo
                </span>
            `
            : `
                <span class="eb-status eb-status-active">
                    Activo
                </span>
            `;

        const accionEstatus = accionista.deshabilitado
            ? "Habilitar"
            : "Deshabilitar";

        return `
            <tr>
                <td>
                    <strong>
                        ${escaparHtml(
            accionista.nombreCompleto
        )}
                    </strong>
                </td>

                <td>
                    ${valorTexto(accionista.rfc)}
                </td>

                <td class="text-end">
                    ${formatearPorcentaje(
            accionista.porcentajeParticipacion
        )}
                </td>

                <td>
                    ${valorTexto(accionista.nacionalidad)}
                </td>

                <td class="text-center">
                    ${representante}
                </td>

                <td class="text-center">
                    ${estatus}
                </td>

                <td class="text-center eb-accionista-actions-cell">
                    <div class="dropdown eb-accionista-actions-dropdown">

                        <button type="button"
                                class="btn btn-sm eb-action-button"
                                data-bs-toggle="dropdown"
                                data-bs-display="static"
                                aria-expanded="false"
                                title="Acciones">

                            <span class="eb-action-dots">⋮</span>
                        </button>

                        <ul class="dropdown-menu dropdown-menu-end eb-accionista-actions-menu">

                            <li>
                                <button type="button"
                                        class="dropdown-item"
                                        onclick="consultarAccionista(${accionista.id})">
                                    Consultar
                                </button>
                            </li>

                            <li>
                                <button type="button"
                                        class="dropdown-item"
                                        onclick="editarAccionista(${accionista.id})">
                                    Editar
                                </button>
                            </li>

                            <li>
                                <button type="button"
                                        class="dropdown-item"
                                        onclick="cambiarEstatusAccionista(${accionista.id})">
                                    ${accionEstatus}
                                </button>
                            </li>

                            <li>
                                <hr class="dropdown-divider" />
                            </li>

                            <li>
                                <button type="button"
                                        class="dropdown-item text-danger"
                                        onclick="confirmarEliminarAccionista(
                                            ${accionista.id},
                                            '${escaparAtributoJs(
                                                accionista.nombreCompleto
                                            )}'
                                        )">
                                    Eliminar
                                </button>
                            </li>

                        </ul>

                    </div>
                </td>
            </tr>
        `;
    }).join("");
}

function limpiarListadoAccionistas() {
    document.getElementById(
        "tablaAccionistasBody"
    ).innerHTML = "";

    document.getElementById(
        "totalAccionistas"
    ).textContent = "0";

    document.getElementById(
        "porcentajeRegistrado"
    ).textContent = "0.0000 %";

    document.getElementById(
        "porcentajeDisponible"
    ).textContent = "100.0000 %";

    document.getElementById(
        "accionistasEstadoVacio"
    )?.classList.add("d-none");

    document.getElementById(
        "contenedorTablaAccionistas"
    )?.classList.add("d-none");
}

function mostrarCargaAccionistas() {
    document.getElementById(
        "accionistasCargando"
    )?.classList.remove("d-none");

    document.getElementById(
        "accionistasError"
    )?.classList.add("d-none");
}

function mostrarErrorAccionistas(mensaje) {
    document.getElementById(
        "accionistasCargando"
    )?.classList.add("d-none");

    document.getElementById(
        "contenedorTablaAccionistas"
    )?.classList.add("d-none");

    document.getElementById(
        "accionistasEstadoVacio"
    )?.classList.add("d-none");

    const alerta = document.getElementById(
        "accionistasError"
    );

    const texto = document.getElementById(
        "accionistasErrorMensaje"
    );

    if (texto) {
        texto.textContent = mensaje;
    }

    alerta?.classList.remove("d-none");
}

function formatearPorcentaje(valor) {
    const numero = Number(valor ?? 0);

    if (Number.isNaN(numero)) {
        return "0.0000 %";
    }

    return `${numero.toFixed(4)} %`;
}

function mostrarResultado(mensaje, tipo) {
    const icono = document.getElementById("resultadoIcono");
    const titulo = document.getElementById("resultadoTitulo");
    const texto = document.getElementById("resultadoMensaje");

    texto.textContent = mensaje ?? "";

    if (tipo === "error") {
        icono.className = "eb-result-icon eb-result-error";
        icono.innerHTML =
            '<i class="fa-solid fa-circle-xmark"></i>';

        titulo.textContent = "No fue posible completar la operación";
    } else {
        icono.className = "eb-result-icon eb-result-success";
        icono.innerHTML =
            '<i class="fa-solid fa-circle-check"></i>';

        titulo.textContent = "Operación exitosa";
    }

    modalResultado.show();
}

function valorTexto(valor) {
    if (valor === null ||
        valor === undefined ||
        String(valor).trim() === "") {
        return "-";
    }

    return escaparHtml(String(valor));
}

function formatearFecha(valor) {
    if (!valor) {
        return "-";
    }

    const fecha = new Date(valor);

    if (Number.isNaN(fecha.getTime())) {
        return "-";
    }

    return fecha.toLocaleDateString("es-MX");
}

function escaparHtml(texto) {
    return String(texto ?? "")
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#039;");
}

function confirmarEliminarSeleccionadas() {
    const seleccionadas = $("#tablaEmpresas")
        .bootstrapTable("getSelections");

    if (seleccionadas.length === 0) {
        return;
    }

    modoEliminacion = "multiple";
    empresaIdEliminar = 0;

    empresasIdsEliminar = seleccionadas.map(
        function (empresa) {
            return empresa.id;
        }
    );

    const mensaje = document.getElementById(
        "mensajeConfirmacionEliminar"
    );

    if (mensaje) {
        const cantidad = empresasIdsEliminar.length;

        mensaje.innerHTML = `
            ¿Estás seguro de que deseas eliminar
            <strong>${cantidad}</strong>
            ${cantidad === 1 ? "empresa seleccionada" : "empresas seleccionadas"}?
            No se realizará ningún cambio hasta que confirmes la operación.
        `;
    }

    modalEliminarEmpresa.show();
}

function escaparAtributoJs(texto) {
    return String(texto ?? "")
        .replaceAll("\\", "\\\\")
        .replaceAll("'", "\\'")
        .replaceAll('"', "&quot;")
        .replaceAll("\r", " ")
        .replaceAll("\n", " ");
}

function abrirModalCrearAccionista() {
    modoAccionista = "crear";

    limpiarFormularioAccionista();
    habilitarFormularioAccionista(true);

    document.getElementById("eaEmpresaId").value =
        empresaIdAccionistas;

    document.getElementById(
        "nombreEmpresaFormularioAccionista"
    ).textContent = empresaNombreAccionistas;

    document.getElementById(
        "modalAccionistaTitulo"
    ).textContent = "Nuevo accionista";

    document.getElementById(
        "btnGuardarAccionista"
    ).classList.remove("d-none");

    mostrarModalAccionista();
}

async function consultarAccionista(id) {
    modoAccionista = "consultar";

    const resultado = await obtenerAccionista(id);

    if (!resultado?.success) {
        mostrarResultado(
            resultado?.message ??
            "No fue posible consultar el accionista.",
            "error"
        );

        return;
    }

    cargarFormularioAccionista(resultado.data);
    habilitarFormularioAccionista(false);

    document.getElementById(
        "modalAccionistaTitulo"
    ).textContent = "Consultar accionista";

    document.getElementById(
        "nombreEmpresaFormularioAccionista"
    ).textContent = empresaNombreAccionistas;

    document.getElementById(
        "btnGuardarAccionista"
    ).classList.add("d-none");

    mostrarModalAccionista();
}

async function editarAccionista(id) {
    modoAccionista = "editar";

    const resultado = await obtenerAccionista(id);

    if (!resultado?.success) {
        mostrarResultado(
            resultado?.message ??
            "No fue posible cargar el accionista.",
            "error"
        );

        return;
    }

    cargarFormularioAccionista(resultado.data);
    habilitarFormularioAccionista(true);

    document.getElementById(
        "modalAccionistaTitulo"
    ).textContent = "Editar accionista";

    document.getElementById(
        "nombreEmpresaFormularioAccionista"
    ).textContent = empresaNombreAccionistas;

    document.getElementById(
        "btnGuardarAccionista"
    ).classList.remove("d-none");

    mostrarModalAccionista();
}

async function obtenerAccionista(id) {
    try {
        const parametros = new URLSearchParams({
            handler: "Accionista",
            id: id.toString()
        });

        const response = await fetch(
            `${window.location.pathname}?${parametros.toString()}`
        );

        return await response.json();
    } catch {
        return null;
    }
}

function cargarFormularioAccionista(data) {
    document.getElementById("eaId").value = data.id ?? 0;
    document.getElementById("eaEmpresaId").value =
        data.empresaId ?? empresaIdAccionistas;

    document.getElementById("eaNombreCompleto").value =
        data.nombreCompleto ?? "";

    document.getElementById("eaRfc").value =
        data.rfc ?? "";

    document.getElementById("eaNacionalidad").value =
        data.nacionalidad ?? "";

    document.getElementById("eaPorcentajeParticipacion").value =
        data.porcentajeParticipacion ?? "";

    document.getElementById("eaEsRepresentanteLegal").checked =
        Boolean(data.esRepresentanteLegal);

    limpiarErroresAccionista();
}

function limpiarFormularioAccionista() {
    document.getElementById("formAccionista")?.reset();

    document.getElementById("eaId").value = "0";
    document.getElementById("eaEmpresaId").value =
        empresaIdAccionistas.toString();

    document.getElementById("eaNombreCompleto").value = "";
    document.getElementById("eaRfc").value = "";
    document.getElementById("eaNacionalidad").value = "";
    document.getElementById("eaPorcentajeParticipacion").value = "";
    document.getElementById("eaEsRepresentanteLegal").checked = false;

    limpiarErroresAccionista();
}

function habilitarFormularioAccionista(habilitado) {
    document
        .querySelectorAll(
            "#formAccionista input:not([type='hidden'])"
        )
        .forEach(function (campo) {
            campo.disabled = !habilitado;
        });
}

function obtenerRequestAccionista() {
    return {
        id: parseInt(
            document.getElementById("eaId").value || "0",
            10
        ),
        empresaId: parseInt(
            document.getElementById("eaEmpresaId").value || "0",
            10
        ),
        nombreCompleto:
            document.getElementById("eaNombreCompleto").value,
        rfc:
            document.getElementById("eaRfc").value,
        nacionalidad:
            document.getElementById("eaNacionalidad").value,
        porcentajeParticipacion: parseFloat(
            document.getElementById(
                "eaPorcentajeParticipacion"
            ).value || "0"
        ),
        esRepresentanteLegal:
            document.getElementById(
                "eaEsRepresentanteLegal"
            ).checked
    };
}

async function guardarAccionista() {
    limpiarErroresAccionista();

    const handler = modoAccionista === "editar"
        ? "EditarAccionista"
        : "CrearAccionista";

    const boton = document.getElementById(
        "btnGuardarAccionista"
    );

    const htmlOriginal = boton.innerHTML;
    boton.disabled = true;
    boton.innerHTML = "Guardando...";

    try {
        const resultado = await enviarJson(
            handler,
            obtenerRequestAccionista()
        );

        if (!resultado.success) {
            mostrarErroresAccionista(resultado.errors);

            if (!resultado.errors) {
                mostrarResultado(
                    resultado.message ??
                    "No fue posible guardar el accionista.",
                    "error"
                );
            }

            return;
        }

        modalAccionista.hide();

        await cargarAccionistasEmpresa();

        mostrarResultado(
            resultado.message,
            "success"
        );
    } catch {
        mostrarResultado(
            "Ocurrió un error al guardar el accionista.",
            "error"
        );
    } finally {
        boton.disabled = false;
        boton.innerHTML = htmlOriginal;
    }
}

async function cambiarEstatusAccionista(id) {
    const resultado = await enviarJson(
        "CambiarEstatusAccionista",
        { id }
    );

    if (!resultado.success) {
        mostrarResultado(resultado.message, "error");
        return;
    }

    await cargarAccionistasEmpresa();
    mostrarResultado(resultado.message, "success");
}

function confirmarEliminarAccionista(id, nombre) {
    accionistaIdEliminar = id;

    document.getElementById(
        "nombreAccionistaEliminar"
    ).textContent = nombre;

    modalAccionistas.hide();

    setTimeout(function () {
        modalEliminarAccionista.show();
    }, 200);
}

async function eliminarAccionista() {
    if (accionistaIdEliminar <= 0) {
        return;
    }

    const boton = document.getElementById(
        "btnConfirmarEliminarAccionista"
    );

    const htmlOriginal = boton.innerHTML;
    boton.disabled = true;
    boton.innerHTML = "Eliminando...";

    try {
        const resultado = await enviarJson(
            "EliminarAccionista",
            { id: accionistaIdEliminar }
        );

        if (!resultado.success) {
            modalEliminarAccionista.hide();
            mostrarResultado(resultado.message, "error");
            return;
        }

        accionistaIdEliminar = 0;

        modalEliminarAccionista.hide();

        await cargarAccionistasEmpresa();

        setTimeout(function () {
            modalAccionistas.show();
        }, 200);

        mostrarResultado(
            resultado.message,
            "success"
        );
    } catch {
        mostrarResultado(
            "No fue posible eliminar el accionista.",
            "error"
        );
    } finally {
        boton.disabled = false;
        boton.innerHTML = htmlOriginal;
    }
}

function mostrarErroresAccionista(errors) {
    if (!errors) {
        return;
    }

    Object.entries(errors).forEach(function ([campo, mensajes]) {
        const elemento = document.querySelector(
            `[data-accionista-error-for="${campo}"]`
        );

        if (elemento) {
            elemento.textContent = mensajes?.[0] ?? "";
        }
    });
}

function limpiarErroresAccionista() {
    document
        .querySelectorAll("[data-accionista-error-for]")
        .forEach(function (elemento) {
            elemento.textContent = "";
        });
}

function mostrarModalAccionista() {
    if (!modalAccionista) {
        console.error(
            "No se encontró el formulario de accionista."
        );

        return;
    }

    modalAccionistas?.hide();

    const modalElement =
        document.getElementById(
            "modalAccionista"
        );

    if (!modalElement) {
        return;
    }

    const alCerrar = function () {
        modalElement.removeEventListener(
            "hidden.bs.modal",
            alCerrar
        );

        if (
            modalAccionistas &&
            empresaIdAccionistas > 0
        ) {
            modalAccionistas.show();
        }
    };

    modalElement.addEventListener(
        "hidden.bs.modal",
        alCerrar
    );

    setTimeout(function () {
        modalAccionista.show();
    }, 180);
}

async function abrirDocumentosEmpresa(
    empresaId,
    razonSocial
) {
    if (!modalDocumentos) {
        console.error(
            "No se encontró el modal de documentos."
        );

        mostrarResultado(
            "No fue posible abrir el expediente documental.",
            "error"
        );

        return;
    }

    empresaIdDocumentos = empresaId;
    empresaNombreDocumentos = razonSocial;
    tipoDocumentoSeleccionado = null;

    const nombreEmpresa = document.getElementById(
        "nombreEmpresaDocumentos"
    );

    if (nombreEmpresa) {
        nombreEmpresa.textContent =
            razonSocial;
    }

    limpiarListadoDocumentos();
    mostrarCargaDocumentos();

    modalDocumentos.show();

    await Promise.all([
        cargarInformacionCorporativaDocumentos(
            empresaId
        ),
        cargarDocumentosEmpresa()
    ]);
}

function limpiarListadoDocumentos() {

    documentosEmpresaActuales = [];
    tipoDocumentoSeleccionado = null;

    const matriz = document.getElementById(
        "matrizDocumentos"
    );

    if (matriz) {
        matriz.innerHTML = "";
    }

    establecerTexto(
        "totalDocumentosRequeridos",
        "0"
    );

    establecerTexto(
        "totalDocumentosCargados",
        "0"
    );

    establecerTexto(
        "totalDocumentosPendientes",
        "0"
    );

    establecerTexto(
        "totalDocumentosVencidos",
        "0"
    );

    establecerTexto(
        "documentoEmpresaNombre",
        "-"
    );

    establecerTexto(
        "documentoEmpresaRfc",
        "-"
    );

    establecerTexto(
        "documentoEmpresaNivel",
        "-"
    );

    establecerTexto(
        "documentoEmpresaActividad",
        "-"
    );

    establecerTexto(
        "documentoEmpresaTelefono",
        "-"
    );

    establecerTexto(
        "documentoEmpresaCorreo",
        "-"
    );

    establecerTexto(
        "documentoEmpresaFechaConstitucion",
        "-"
    );

    establecerTexto(
        "documentoEmpresaEscritura",
        "-"
    );

    establecerTexto(
        "documentoEmpresaAccionistas",
        "-"
    );

    document.getElementById(
        "panelArchivosDocumento"
    )?.classList.add("d-none");

    document.getElementById(
        "documentosError"
    )?.classList.add("d-none");
}

function mostrarCargaDocumentos() {
    document.getElementById(
        "documentosCargando"
    )?.classList.remove("d-none");

    document.getElementById(
        "documentosError"
    )?.classList.add("d-none");

    document.getElementById(
        "documentosEstadoVacio"
    )?.classList.add("d-none");

    document.getElementById(
        "contenedorTablaDocumentos"
    )?.classList.add("d-none");
}

async function cargarDocumentosEmpresa() {
    if (empresaIdDocumentos <= 0) {
        mostrarErrorDocumentos(
            "El identificador de la empresa no es válido."
        );

        return;
    }

    try {
        const parametros = new URLSearchParams({
            handler: "Documentos",
            empresaId: empresaIdDocumentos.toString()
        });

        const response = await fetch(
            `${window.location.pathname}?${parametros.toString()}`
        );

        if (!response.ok) {
            throw new Error(
                `Error HTTP ${response.status}`
            );
        }

        const resultado = await response.json();

        if (!resultado.success) {
            mostrarErrorDocumentos(
                resultado.message ??
                "No fue posible consultar los documentos."
            );

            return;
        }

        documentosEmpresaActuales =
            Array.isArray(resultado.data)
                ? resultado.data
                : [];

        actualizarResumenDocumentosBackend(
            resultado.resumen
        );

        renderizarMatrizDocumental(
            documentosEmpresaActuales
        );

        document
            .getElementById("documentosCargando")
            ?.classList.add("d-none");

        if (documentosEmpresaActuales.length === 0) {
            const matriz = document.getElementById(
                "matrizDocumentos"
            );

            if (matriz) {
                matriz.innerHTML = `
                    <div class="eb-documents-empty">
                        <div class="eb-documents-empty-icon">
                            <i class="fa-regular fa-folder-open"></i>
                        </div>

                        <h6>
                            No hay tipos de documento configurados
                        </h6>

                        <p>
                            El catálogo documental no contiene
                            registros activos.
                        </p>
                    </div>
                `;
            }
        }
    } catch (error) {
        console.error(error);

        mostrarErrorDocumentos(
            "Ocurrió un error al consultar la documentación."
        );
    }
}

function actualizarResumenDocumentosBackend(
    resumen) {
    establecerTexto(
        "totalDocumentosRequeridos",
        resumen?.totalRequeridos ?? 0
    );

    establecerTexto(
        "totalDocumentosCargados",
        resumen?.totalCargados ?? 0
    );

    establecerTexto(
        "totalDocumentosPendientes",
        resumen?.totalPendientes ?? 0
    );

    establecerTexto(
        "totalDocumentosVencidos",
        resumen?.totalVencidos ?? 0
    );
}

let tipoDocumentoSeleccionado = null;
let documentosEmpresaActuales = [];

async function cargarInformacionCorporativaDocumentos(empresaId) {
    const respuestaEmpresa = await obtenerEmpresa(empresaId);

    if (respuestaEmpresa?.success) {
        llenarInformacionCorporativaDocumento(
            respuestaEmpresa.data
        );
    }

    try {
        const parametros = new URLSearchParams({
            handler: "Accionistas",
            empresaId: empresaId.toString()
        });

        const response = await fetch(
            `${window.location.pathname}?${parametros.toString()}`
        );

        const resultado = await response.json();

        if (!resultado.success) {
            establecerTexto(
                "documentoEmpresaAccionistas",
                "-"
            );

            return;
        }

        const accionistasActivos = (resultado.data ?? []).filter(
            function (accionista) {
                return !accionista.deshabilitado;
            }
        );

        if (accionistasActivos.length === 0) {
            establecerTexto(
                "documentoEmpresaAccionistas",
                "-"
            );

            return;
        }

        const textoAccionistas = accionistasActivos.map(
            function (accionista) {
                return `${accionista.nombreCompleto} ` +
                    `(${formatearPorcentaje(
                        accionista.porcentajeParticipacion
                    )})`;
            }
        ).join(", ");

        establecerTexto(
            "documentoEmpresaAccionistas",
            textoAccionistas
        );
    } catch {
        establecerTexto(
            "documentoEmpresaAccionistas",
            "-"
        );
    }
}

function llenarInformacionCorporativaDocumento(empresa) {
    establecerTexto(
        "documentoEmpresaNombre",
        empresa.razonSocial
    );

    establecerTexto(
        "documentoEmpresaRfc",
        empresa.rfc
    );

    establecerTexto(
        "documentoEmpresaNivel",
        empresa.nivel
    );

    establecerTexto(
        "documentoEmpresaActividad",
        empresa.actividadComercial
    );

    establecerTexto(
        "documentoEmpresaTelefono",
        empresa.telefonoBancos
    );

    establecerTexto(
        "documentoEmpresaCorreo",
        empresa.correoBancos
    );

    establecerTexto(
        "documentoEmpresaFechaConstitucion",
        formatearFecha(empresa.fechaConstitucion)
    );

    establecerTexto(
        "documentoEmpresaEscritura",
        empresa.numeroEscritura
    );
}

function establecerTexto(id, valor) {
    const elemento = document.getElementById(id);

    if (!elemento) {
        return;
    }

    const texto = valor === null ||
        valor === undefined ||
        String(valor).trim() === ""
        ? "-"
        : String(valor);

    elemento.textContent = texto;
}

function renderizarMatrizDocumental(documentos) {
    const matriz = document.getElementById(
        "matrizDocumentos"
    );

    if (!matriz) {
        return;
    }

    if (
        !Array.isArray(documentos) ||
        documentos.length === 0
    ) {
        matriz.innerHTML = `
            <div class="eb-documents-empty">

                <div class="eb-documents-empty-icon">
                    <i class="fa-regular fa-folder-open"></i>
                </div>

                <h6>
                    No hay tipos de documento configurados
                </h6>

                <p>
                    El catálogo documental no contiene registros activos.
                </p>

            </div>
        `;

        return;
    }

    matriz.innerHTML = documentos.map(
        function (documento) {
            const totalArchivos =
                Number(documento.totalArchivos ?? 0);

            const tieneArchivos =
                totalArchivos > 0;

            const estatusClase =
                obtenerClaseEstatusDocumento(
                    documento.estatus
                );

            const textoPrincipal = tieneArchivos
                ? totalArchivos === 1
                    ? "1 archivo cargado"
                    : `${totalArchivos} archivos cargados`
                : "Selecciona un archivo";

            const textoSecundario = tieneArchivos
                ? "Haz clic para consultar los archivos"
                : "Haz clic para cargar el documento";

            const accionTarjeta = tieneArchivos
                ? `seleccionarTipoDocumento(${documento.id})`
                : `prepararCargaDocumento(${documento.id})`;

            return `
                <article class="
                    eb-document-upload-card
                    ${tieneArchivos
                    ? "eb-document-upload-card-loaded"
                    : "eb-document-upload-card-empty"}
                ">

                    <div class="eb-document-upload-title-row">

                        <div class="eb-document-upload-title">

                            <strong>
                                ${escaparHtml(documento.nombre)}
                            </strong>

                            ${documento.obligatorio
                    ? `
                                    <span class="eb-document-required-mark"
                                          title="Documento obligatorio">
                                        *
                                    </span>
                                `
                    : ""
                }

                        </div>

                    </div>

                    <div class="eb-document-upload-area-wrapper">

                        <button type="button"
                                class="eb-document-upload-area"
                                onclick="${accionTarjeta}">

                            <span class="eb-document-upload-file-text">

                                <strong>
                                    ${textoPrincipal}
                                </strong>

                                <small>
                                    ${textoSecundario}
                                </small>

                            </span>

                            <span class="
                                eb-document-status
                                ${estatusClase}
                            ">
                                ${escaparHtml(documento.estatus)}
                            </span>

                        </button>

                        <div class="
                            dropdown
                            eb-document-upload-actions
                        ">

                            <button type="button"
                                    class="btn eb-document-upload-menu"
                                    data-bs-toggle="dropdown"
                                    data-bs-display="static"
                                    data-bs-boundary="viewport"
                                    aria-expanded="false"
                                    aria-label="Acciones de ${escaparHtml(
                    documento.nombre
                )}">

                                <span>⋮</span>
                            </button>

                            <ul class="
                                dropdown-menu
                                dropdown-menu-end
                                eb-document-upload-dropdown
                            ">

                                <li>
                                    <button type="button"
                                            class="dropdown-item"
                                            onclick="seleccionarTipoDocumento(
                                                ${documento.id}
                                            )">

                                        <i class="fa-regular fa-eye me-2"></i>
                                        Visualizar Archivo
                                    </button>
                                </li>

                                <li>
                                    <button type="button"
                                            class="dropdown-item"
                                            onclick="prepararCargaDocumento(
                                                ${documento.id}
                                            )">

                                        <i class="fa-solid fa-cloud-arrow-up me-2"></i>

                                        ${tieneArchivos
                    ? "Cargar nueva versión"
                    : "Cargar archivo"}
                                    </button>
                                </li>

                                ${tieneArchivos
                    ? `
                                        <li>
                                            <button type="button"
                                                    class="dropdown-item"
                                                    onclick="descargarDocumento(
                                                        ${documento.id}
                                                    )">

                                                <i class="fa-solid fa-download me-2"></i>
                                                Descargar
                                            </button>
                                        </li>
                                    `
                    : ""
                }

                            </ul>

                        </div>

                    </div>

                </article>
            `;
        }
    ).join("");
}

function seleccionarTipoDocumento(
    tipoDocumentoId
) {
    const documento =
        documentosEmpresaActuales.find(
            function (item) {
                return item.id === tipoDocumentoId;
            }
        );

    if (!documento) {
        mostrarResultado(
            "No se encontró el documento seleccionado.",
            "error"
        );

        return;
    }

    const archivos = Array.isArray(
        documento.archivos
    )
        ? documento.archivos
        : [];

    if (archivos.length === 0) {
        prepararCargaDocumento(
            tipoDocumentoId
        );

        return;
    }

    /*
     * El backend entrega los archivos ordenados
     * del más reciente al más antiguo.
     */
    const archivoActual = archivos[0];

    visualizarArchivoDocumento(
        archivoActual.id
    );
}

function renderizarArchivosDocumento(archivos) {
    const estadoVacio = document.getElementById(
        "archivosDocumentoEstadoVacio"
    );

    const lista = document.getElementById(
        "listaArchivosDocumento"
    );

    if (!lista) {
        return;
    }

    if (
        !Array.isArray(archivos) ||
        archivos.length === 0
    ) {
        lista.innerHTML = "";
        lista.classList.add("d-none");

        estadoVacio?.classList.remove("d-none");

        return;
    }

    estadoVacio?.classList.add("d-none");
    lista.classList.remove("d-none");

    lista.innerHTML = archivos.map(
        function (archivo) {
            const vencimiento =
                archivo.fechaVencimiento
                    ? formatearFecha(
                        archivo.fechaVencimiento
                    )
                    : "No aplica";

            return `
                <article class="eb-document-file-item">

                    <div class="eb-document-file-icon">
                        <i class="${obtenerIconoArchivo(
                archivo.extension
            )}"></i>
                    </div>

                    <div class="eb-document-file-information">

                        <strong>
                            ${escaparHtml(
                archivo.nombreOriginal
            )}
                        </strong>

                        <span>
                            Versión ${archivo.version}
                            ·
                            ${formatearTamanoArchivo(
                archivo.tamanoBytes
            )}
                        </span>

                        <small>
                            Cargado:
                            ${formatearFecha(
                archivo.fechaCarga
            )}
                            ·
                            Vencimiento:
                            ${vencimiento}
                        </small>

                    </div>

                    <div class="dropdown">

                        <button type="button"
                                class="btn eb-document-menu-button"
                                data-bs-toggle="dropdown"
                                data-bs-display="static"
                                aria-expanded="false"
                                title="Acciones">

                            <span>⋮</span>
                        </button>

                        <ul class="dropdown-menu dropdown-menu-end">

                            <li>
                                <button type="button"
                                        class="dropdown-item"
                                        onclick="visualizarArchivoDocumento(
                                            ${archivo.id}
                                        )">

                                    <i class="fa-regular fa-eye me-2"></i>
                                    Visualizar
                                </button>
                            </li>

                            <li>
                                <button type="button"
                                        class="dropdown-item"
                                        onclick="descargarArchivoDocumento(
                                            ${archivo.id}
                                        )">

                                    <i class="fa-solid fa-download me-2"></i>
                                    Descargar
                                </button>
                            </li>

                            <li>
                                <button type="button"
                                        class="dropdown-item text-danger"
                                        onclick="confirmarEliminarArchivoDocumento(
                                            ${archivo.id},
                                            '${escaparAtributoJs(
                archivo.nombreOriginal
            )}'
                                        )">

                                    <i class="fa-regular fa-trash-can me-2"></i>
                                    Eliminar
                                </button>
                            </li>

                        </ul>

                    </div>

                </article>
            `;
        }
    ).join("");
}

function obtenerIconoArchivo(extension) {
    const extensionNormalizada =
        String(extension ?? "")
            .toLowerCase()
            .replace(".", "");

    switch (extensionNormalizada) {
        case "pdf":
            return "fa-regular fa-file-pdf";

        case "doc":
        case "docx":
            return "fa-regular fa-file-word";

        case "xls":
        case "xlsx":
            return "fa-regular fa-file-excel";

        case "jpg":
        case "jpeg":
        case "png":
        case "webp":
            return "fa-regular fa-file-image";

        case "zip":
        case "rar":
        case "7z":
            return "fa-regular fa-file-zipper";

        default:
            return "fa-regular fa-file-lines";
    }
}

function formatearTamanoArchivo(bytes) {
    const tamano = Number(bytes ?? 0);

    if (
        Number.isNaN(tamano) ||
        tamano <= 0
    ) {
        return "0 KB";
    }

    if (tamano < 1024) {
        return `${tamano} bytes`;
    }

    if (tamano < 1024 * 1024) {
        return `${(
            tamano / 1024
        ).toFixed(2)} KB`;
    }

    if (tamano < 1024 * 1024 * 1024) {
        return `${(
            tamano / 1024 / 1024
        ).toFixed(2)} MB`;
    }

    return `${(
        tamano / 1024 / 1024 / 1024
    ).toFixed(2)} GB`;
}

function visualizarArchivoDocumento(id) {
    const documentoId = Number(id);

    if (
        !Number.isInteger(documentoId) ||
        documentoId <= 0
    ) {
        mostrarResultado(
            "El archivo seleccionado no es válido.",
            "error"
        );

        return;
    }

    const parametros = new URLSearchParams({
        handler: "VisualizarDocumento",
        id: documentoId.toString()
    });

    const url =
        `${window.location.pathname}` +
        `?${parametros.toString()}`;

    const nuevaPestana = window.open(
        url,
        "_blank",
        "noopener,noreferrer"
    );

    if (!nuevaPestana) {
        mostrarResultado(
            "El navegador bloqueó la nueva pestaña. " +
            "Permite las ventanas emergentes para este sitio.",
            "error"
        );
    }
}

function descargarArchivoDocumento(id) {
    mostrarResultado(
        `La descarga del archivo ${id} se conectará en el siguiente paso.`,
        "success"
    );
}

function confirmarEliminarArchivoDocumento(
    id,
    nombre
) {
    mostrarResultado(
        `La eliminación de "${nombre}" se conectará en el siguiente paso.`,
        "success"
    );
}

function prepararCargaDocumento(
    tipoDocumentoId
) {
    const documento =
        documentosEmpresaActuales.find(
            function (item) {
                return item.id === tipoDocumentoId;
            }
        );

    if (!documento) {
        mostrarResultado(
            "No se encontró el tipo de documento.",
            "error"
        );

        return;
    }

    if (!modalCargarDocumento) {
        mostrarResultado(
            "No se encontró el formulario de carga.",
            "error"
        );

        return;
    }

    if (empresaIdDocumentos <= 0) {
        mostrarResultado(
            "No se identificó la empresa del expediente.",
            "error"
        );

        return;
    }

    tipoDocumentoSeleccionado = documento;

    const formulario = document.getElementById(
        "formCargarDocumento"
    );

    formulario?.reset();

    const empresaInput = document.getElementById(
        "documentoCargaEmpresaId"
    );

    const tipoInput = document.getElementById(
        "documentoCargaTipoId"
    );

    if (empresaInput) {
        empresaInput.value =
            empresaIdDocumentos.toString();
    }

    if (tipoInput) {
        tipoInput.value =
            documento.id.toString();
    }

    establecerTexto(
        "nombreTipoDocumentoCarga",
        documento.nombre
    );

    const contenedorVencimiento =
        document.getElementById(
            "contenedorFechaVencimientoDocumento"
        );

    const fechaVencimiento =
        document.getElementById(
            "documentoCargaFechaVencimiento"
        );

    if (documento.requiereFechaVencimiento) {
        contenedorVencimiento?.classList.remove(
            "d-none"
        );

        fechaVencimiento?.setAttribute(
            "required",
            "required"
        );

        if (fechaVencimiento) {
            fechaVencimiento.min =
                new Date()
                    .toISOString()
                    .split("T")[0];
        }
    } else {
        contenedorVencimiento?.classList.add(
            "d-none"
        );

        fechaVencimiento?.removeAttribute(
            "required"
        );

        if (fechaVencimiento) {
            fechaVencimiento.value = "";
            fechaVencimiento.removeAttribute("min");
        }
    }

    limpiarErroresDocumento();

    modalDocumentos?.hide();

    setTimeout(function () {
        modalCargarDocumento?.show();
    }, 250);
}

async function guardarDocumento() {
    limpiarErroresDocumento();

    const archivoInput = document.getElementById(
        "documentoCargaArchivo"
    );

    const empresaInput = document.getElementById(
        "documentoCargaEmpresaId"
    );

    const tipoInput = document.getElementById(
        "documentoCargaTipoId"
    );

    const fechaInput = document.getElementById(
        "documentoCargaFechaVencimiento"
    );

    const observacionesInput =
        document.getElementById(
            "documentoCargaObservaciones"
        );

    const archivo = archivoInput?.files?.[0];

    const empresaId = parseInt(
        empresaInput?.value || "0",
        10
    );

    const tipoDocumentoId = parseInt(
        tipoInput?.value || "0",
        10
    );

    if (empresaId <= 0) {
        mostrarResultado(
            "No se identificó la empresa.",
            "error"
        );

        return;
    }

    if (tipoDocumentoId <= 0) {
        mostrarResultado(
            "No se identificó el tipo de documento.",
            "error"
        );

        return;
    }

    if (!archivo) {
        mostrarErrorCampoDocumento(
            "Archivo",
            "Selecciona un archivo."
        );

        return;
    }

    const tamanoMaximo =
        25 * 1024 * 1024;

    if (archivo.size > tamanoMaximo) {
        mostrarErrorCampoDocumento(
            "Archivo",
            "El archivo no puede superar los 25 MB."
        );

        return;
    }

    if (
        tipoDocumentoSeleccionado
            ?.requiereFechaVencimiento &&
        !fechaInput?.value
    ) {
        mostrarErrorCampoDocumento(
            "FechaVencimiento",
            "La fecha de vencimiento es obligatoria."
        );

        return;
    }

    const formData = new FormData();

    formData.append(
        "EmpresaId",
        empresaId.toString()
    );

    formData.append(
        "TipoDocumentoId",
        tipoDocumentoId.toString()
    );

    formData.append(
        "Archivo",
        archivo,
        archivo.name
    );

    if (fechaInput?.value) {
        formData.append(
            "FechaVencimiento",
            fechaInput.value
        );
    }

    formData.append(
        "Observaciones",
        observacionesInput?.value?.trim() ?? ""
    );

    const token = document.querySelector(
        '#formToken input[name="__RequestVerificationToken"]'
    )?.value;

    const boton = document.getElementById(
        "btnGuardarDocumento"
    );

    if (!boton) {
        return;
    }

    const htmlOriginal = boton.innerHTML;

    boton.disabled = true;

    boton.innerHTML =
        '<i class="fa-solid fa-spinner fa-spin me-1"></i>' +
        " Cargando...";

    try {
        const url =
            `${window.location.pathname}` +
            `?handler=CargarDocumento`;

        console.log(
            "Enviando documento:",
            {
                url,
                empresaId,
                tipoDocumentoId,
                archivo: archivo.name,
                tamano: archivo.size
            }
        );

        const response = await fetch(
            url,
            {
                method: "POST",
                headers: {
                    "RequestVerificationToken":
                        token ?? ""
                },
                body: formData,
                credentials: "same-origin"
            }
        );

        const contenidoRespuesta =
            await response.text();

        console.log(
            "Estatus del servidor:",
            response.status
        );

        console.log(
            "Respuesta del servidor:",
            contenidoRespuesta
        );

        let resultado = null;

        if (contenidoRespuesta) {
            try {
                resultado = JSON.parse(
                    contenidoRespuesta
                );
            } catch {
                throw new Error(
                    "El servidor devolvió una respuesta no válida."
                );
            }
        }

        if (!response.ok) {
            throw new Error(
                resultado?.message ??
                `Error HTTP ${response.status}.`
            );
        }

        if (!resultado?.success) {
            mostrarErroresDocumento(
                resultado?.errors
            );

            if (!resultado?.errors) {
                mostrarResultado(
                    resultado?.message ??
                    "No fue posible cargar el documento.",
                    "error"
                );
            }

            return;
        }

        /*
         * Restablecemos el ID porque el evento de cierre
         * del modal anterior pudo haberlo modificado.
         */
        empresaIdDocumentos = empresaId;

        modalCargarDocumento?.hide();

        await cargarDocumentosEmpresa();

        setTimeout(function () {
            modalDocumentos?.show();
        }, 250);

        mostrarResultado(
            resultado.message ??
            "El documento se cargó correctamente.",
            "success"
        );
    } catch (error) {
        console.error(
            "Error completo al cargar documento:",
            error
        );

        const mensaje =
            error instanceof TypeError &&
                error.message === "Failed to fetch"
                ? "No fue posible conectarse con el servidor. " +
                "Verifica que la aplicación siga ejecutándose."
                : error?.message ??
                "Ocurrió un error al cargar el documento.";

        mostrarResultado(
            mensaje,
            "error"
        );
    } finally {
        boton.disabled = false;
        boton.innerHTML = htmlOriginal;
    }
}

function mostrarErroresDocumento(errors) {
    if (!errors) {
        return;
    }

    Object.entries(errors).forEach(
        function ([campo, mensajes]) {
            mostrarErrorCampoDocumento(
                campo,
                mensajes?.[0] ?? ""
            );
        }
    );
}

function mostrarErrorCampoDocumento(
    campo,
    mensaje
) {
    const elemento = document.querySelector(
        `[data-documento-error-for="${campo}"]`
    );

    if (elemento) {
        elemento.textContent = mensaje;
    }
}

function limpiarErroresDocumento() {
    document
        .querySelectorAll(
            "[data-documento-error-for]"
        )
        .forEach(function (elemento) {
            elemento.textContent = "";
        });
}

function descargarDocumento(tipoDocumentoId) {
    mostrarResultado(
        "Este documento todavía no contiene archivos disponibles.",
        "error"
    );
}

function actualizarResumenDocumentos(documentos) {
    const total = documentos.length;

    const cargados = documentos.filter(
        function (documento) {
            return documento.totalArchivos > 0;
        }
    ).length;

    const pendientes = documentos.filter(
        function (documento) {
            return documento.totalArchivos === 0;
        }
    ).length;

    const vencidos = documentos.filter(
        function (documento) {
            return documento.estatus === "Vencido";
        }
    ).length;

    establecerTexto(
        "totalDocumentosRequeridos",
        total
    );

    establecerTexto(
        "totalDocumentosCargados",
        cargados
    );

    establecerTexto(
        "totalDocumentosPendientes",
        pendientes
    );

    establecerTexto(
        "totalDocumentosVencidos",
        vencidos
    );
}

function obtenerClaseEstatusDocumento(estatus) {
    switch (estatus) {
        case "Vigente":
            return "eb-document-status-valid";

        case "Próximo a vencer":
            return "eb-document-status-warning";

        case "Vencido":
            return "eb-document-status-expired";

        case "Cargado":
            return "eb-document-status-loaded";

        default:
            return "eb-document-status-pending";
    }
}

function mostrarErrorDocumentos(mensaje) {
    document.getElementById(
        "documentosCargando"
    )?.classList.add("d-none");

    const alerta = document.getElementById(
        "documentosError"
    );

    const texto = document.getElementById(
        "documentosErrorMensaje"
    );

    if (texto) {
        texto.textContent = mensaje;
    }

    alerta?.classList.remove("d-none");
}