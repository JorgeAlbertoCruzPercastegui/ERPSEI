document.addEventListener("DOMContentLoaded", function () {
    document.body.classList.add(
        "module-main-theme",
        "expedientes-bancarios-page"
    );

    inicializarTablaEmpresas();
    inicializarEventosEmpresas();
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

function inicializarEventosEmpresas() {
    modalEmpresa = bootstrap.Modal.getOrCreateInstance(
        document.getElementById("modalEmpresa")
    );

    modalEliminarEmpresa = bootstrap.Modal.getOrCreateInstance(
        document.getElementById("modalEliminarEmpresa")
    );

    modalResultado = bootstrap.Modal.getOrCreateInstance(
        document.getElementById("modalResultado")
    );

    modalAccionistas = bootstrap.Modal.getOrCreateInstance(
        document.getElementById("modalAccionistas")
    );

    document
        .getElementById("btnNuevoAccionista")
        ?.addEventListener("click", function () {
            mostrarResultado(
                "En el siguiente bloque habilitaremos el formulario para registrar accionistas.",
                "success"
            );
        });

    document
        .getElementById("btnBuscarEmpresas")
        ?.addEventListener("click", refrescarTablaEmpresas);

    document
        .getElementById("filtroEstatus")
        ?.addEventListener("change", refrescarTablaEmpresas);

    document
        .getElementById("filtroBusqueda")
        ?.addEventListener("keydown", function (event) {
            if (event.key === "Enter") {
                event.preventDefault();
                refrescarTablaEmpresas();
            }
        });

    document
        .getElementById("btnNuevaEmpresa")
        ?.addEventListener("click", function () {
            abrirModalCrearEmpresa();
        });

    document
        .getElementById("btnGuardarEmpresa")
        ?.addEventListener("click", guardarEmpresa);

    document
        .getElementById("btnConfirmarEliminarEmpresa")
        ?.addEventListener("click", eliminarEmpresa);

    document
        .getElementById("ebRfc")
        ?.addEventListener("input", function () {
            this.value = this.value
                .toUpperCase()
                .replace(/\s+/g, "");
        });

    $("#tablaEmpresas").on(
        "check.bs.table uncheck.bs.table check-all.bs.table uncheck-all.bs.table",
        actualizarBotonSeleccionados
    );

    document
        .getElementById("btnEliminarSeleccionadas")
        ?.addEventListener("click", confirmarEliminarSeleccionadas);

    document
        .getElementById("modalEliminarEmpresa")
        ?.addEventListener("hidden.bs.modal", function () {
            empresaIdEliminar = 0;
            empresasIdsEliminar = [];
            modoEliminacion = "individual";
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
    modoEmpresa = "crear";

    limpiarFormularioEmpresa();
    habilitarFormularioEmpresa(true);

    document.getElementById("modalEmpresaTitulo").textContent =
        "Nueva empresa";

    const botonGuardar = document.getElementById(
        "btnGuardarEmpresa"
    );

    botonGuardar.classList.remove("d-none");
    botonGuardar.disabled = false;

    modalEmpresa.show();

    setTimeout(function () {
        document
            .getElementById("ebRazonSocial")
            ?.focus();
    }, 250);
}

async function consultarEmpresa(id) {
    modoEmpresa = "consultar";

    const respuesta = await obtenerEmpresa(id);

    if (!respuesta?.success) {
        mostrarResultado(
            "No fue posible consultar la empresa.",
            "error"
        );
        return;
    }

    cargarFormularioEmpresa(respuesta.data);
    habilitarFormularioEmpresa(false);

    document.getElementById("modalEmpresaTitulo").textContent =
        "Consultar empresa";

    document.getElementById("btnGuardarEmpresa").classList.add(
        "d-none"
    );

    modalEmpresa.show();
}

async function editarEmpresa(id) {
    modoEmpresa = "editar";

    const respuesta = await obtenerEmpresa(id);

    if (!respuesta?.success) {
        mostrarResultado(
            "No fue posible cargar la empresa.",
            "error"
        );
        return;
    }

    cargarFormularioEmpresa(respuesta.data);
    habilitarFormularioEmpresa(true);

    document.getElementById("modalEmpresaTitulo").textContent =
        "Editar empresa";

    document.getElementById("btnGuardarEmpresa").classList.remove(
        "d-none"
    );

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

async function deshabilitarSeleccionadas() {
    const seleccionadas = $("#tablaEmpresas")
        .bootstrapTable("getSelections");

    if (seleccionadas.length === 0) {
        return;
    }

    for (const empresa of seleccionadas) {
        if (!empresa.deshabilitado) {
            await enviarJson(
                "CambiarEstatus",
                { id: empresa.id }
            );
        }
    }

    refrescarTablaEmpresas();

    mostrarResultado(
        "Las empresas seleccionadas se deshabilitaron correctamente.",
        "success"
    );
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


async function abrirAccionistasEmpresa(empresaId, razonSocial) {
    empresaIdAccionistas = empresaId;
    empresaNombreAccionistas = razonSocial;

    document.getElementById(
        "nombreEmpresaAccionistas"
    ).textContent = razonSocial;

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
                    <i class="fa-solid fa-scale-balanced"></i>
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

        return `
            <tr>
                <td>
                    <strong>
                        ${escaparHtml(accionista.nombreCompleto)}
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

                <td class="text-center">
                    <button type="button"
                            class="btn btn-sm btn-outline-primary"
                            onclick="consultarAccionista(
                                ${accionista.id}
                            )"
                            title="Consultar accionista">

                        <i class="fa-regular fa-eye"></i>
                    </button>
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

async function consultarAccionista(id) {
    try {
        const parametros = new URLSearchParams({
            handler: "Accionista",
            id: id.toString()
        });

        const response = await fetch(
            `${window.location.pathname}?${parametros.toString()}`
        );

        const resultado = await response.json();

        if (!resultado.success) {
            mostrarResultado(
                resultado.message ??
                "No fue posible consultar el accionista.",
                "error"
            );

            return;
        }

        const accionista = resultado.data;

        const mensaje = [
            `Nombre: ${accionista.nombreCompleto}`,
            `RFC: ${accionista.rfc ?? "-"}`,
            `Participación: ${formatearPorcentaje(
                accionista.porcentajeParticipacion
            )}`,
            `Nacionalidad: ${accionista.nacionalidad ?? "-"}`,
            `Representante legal: ${accionista.esRepresentanteLegal ? "Sí" : "No"
            }`
        ].join(" | ");

        mostrarResultado(mensaje, "success");
    } catch {
        mostrarResultado(
            "No fue posible consultar el accionista.",
            "error"
        );
    }
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