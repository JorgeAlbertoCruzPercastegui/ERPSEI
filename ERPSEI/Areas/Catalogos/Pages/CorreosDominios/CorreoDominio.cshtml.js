$(document).ready(function () {
    cargarCorreosDominios();

    $("#btnBuscarCorreoDominio").on("click", function () {
        cargarCorreosDominios();
    });

    $("#btnLimpiarCorreoDominio").on("click", function () {
        $("#txtFiltroEmpresa").val("");
        $("#txtFiltroDominio").val("");
        $("#txtFiltroProveedor").val("");
        $("#txtFiltroEstado").val("");
        cargarCorreosDominios();
    });

    $("#btnNuevoCorreoDominio").on("click", function () {
        limpiarModalCorreoDominio();
        abrirModal("dlgCorreoDominio");
    });

    $("#btnAbrirImportacionMasiva").on("click", function () {
        $("#inpExcelCorreosDominios").val("");
        abrirModal("dlgImportacionMasiva");
    });

    $("#btnGuardarCorreoDominio").on("click", function () {
        guardarCorreoDominio();
    });

    $("#btnImportarCorreosDominios").on("click", function () {
        importarCorreosDominios();
    });

    $("#txtBuscarTablaCorreosDominios").on("keyup", function () {
        $("#tableCorreosDominios").bootstrapTable("resetSearch", $(this).val());
    });

    inicializarModalMensajeCorreoDominio();

    // Evita el stacking context del layout, que deja el modal detrás del encabezado.
    const modalAvisos = document.getElementById("dlgConfigAvisos");
    if (modalAvisos) {
        document.body.appendChild(modalAvisos);
        modalAvisos.addEventListener("show.bs.modal", function () {
            document.body.classList.add("modal-avisos-abierto");
        });
        modalAvisos.addEventListener("hidden.bs.modal", function () {
            document.body.classList.remove("modal-avisos-abierto");
        });
    }
});

function cargarCorreosDominios() {
    $("#tableCorreosDominios").bootstrapTable("destroy");

    $("#tableCorreosDominios").bootstrapTable({
        url: window.location.pathname + "?handler=CorreosDominiosList",
        method: "get",
        pagination: true,
        search: false,
        pageSize: 10,
        queryParams: function () {
            return {
                empresa: $("#txtFiltroEmpresa").val(),
                dominio: $("#txtFiltroDominio").val(),
                proveedor: $("#txtFiltroProveedor").val(),
                estado: $("#txtFiltroEstado").val()
            };
        }
    });
}

function accionesCorreoDominioFormatter(value, row) {
    return `
        <button type="button" class="btn btn-sm btn-warning me-1" onclick="editarCorreoDominioPorId(${row.id})">
            <i class="bi bi-pencil-square"></i>
        </button>

        <button type="button" class="btn btn-sm btn-danger" onclick="eliminarCorreoDominio(${row.id})">
            <i class="bi bi-trash"></i>
        </button>
    `;
}

function editarCorreoDominioPorId(id) {
    const registros = $("#tableCorreosDominios").bootstrapTable("getData");

    const row = registros.find(x => x.id === id || x.Id === id);

    if (!row) {
        mostrarMensajeCorreoDominio(
            "error",
            "Error",
            "No se encontró la información del registro seleccionado."
        );
        return;
    }

    editarCorreoDominio(row);
}

function estadoFormatter(value, row) {
    const texto = (value || "").toString().trim().toUpperCase();

    if (texto === "VIGENTE") {
        return `<span class="badge-estado badge-estado-vigente">${value}</span>`;
    }

    if (texto === "SUSPENDIDA") {
        return `<span class="badge-estado badge-estado-suspendida">${value}</span>`;
    }

    if (texto === "CLIENTE") {
        return `<span class="badge-estado badge-estado-cliente">${value}</span>`;
    }

    return value || "";
}

function pagWebFormatter(value, row) {
    const texto = (value || "").toString().trim().toUpperCase();

    if (texto === "OK") {
        return `<span class="badge-pagweb badge-pagweb-ok">${value}</span>`;
    }

    if (texto === "N/A") {
        return `<span class="badge-pagweb badge-pagweb-na">${value}</span>`;
    }

    return value || "";
}

function costosFormatter(value, row) {

    if (!value)
        return "";

    const numero = parseFloat(value);

    return `<span class="badge-costo">
                ${numero.toLocaleString("es-MX", {
        style: "currency",
        currency: "MXN"
    })}
            </span>`;
}

/*function editarCorreoDominioDesdeJson(rowJson) {
    const row = JSON.parse(decodeURIComponent(rowJson));
    editarCorreoDominio(row);
}*/

function limpiarModalCorreoDominio() {
    $("#inpCorreoDominioId").val(0);
    $("#inpEmpresa").val("");
    $("#inpDominio").val("");
    $("#inpProveedor").val("");
    $("#inpFechaCaducacion").val("");
    $("#inpCostos").val("");
    $("#inpCorreoOperaciones").val("");
    $("#inpContrasenaOperaciones").val("");
    $("#inpCorreoFiscal").val("");
    $("#inpContrasenaFiscal").val("");
    $("#inpPagWeb").val("");
    $("#inpEstado").val("");
    $("#inpObservaciones").val("");
}

function editarCorreoDominio(row) {
    $("#inpCorreoDominioId").val(row.id);
    $("#inpEmpresa").val(row.empresa || "");
    $("#inpDominio").val(row.dominio || "");
    $("#inpProveedor").val(row.proveedor || "");
    $("#inpFechaCaducacion").val(convertirFechaInput(row.fechaCaducacion));
    $("#inpCostos").val(row.costos || "");
    $("#inpCorreoOperaciones").val(row.correoOperaciones || "");
    $("#inpContrasenaOperaciones").val(row.contrasenaOperaciones || "");
    $("#inpCorreoFiscal").val(row.correoFiscal || "");
    $("#inpContrasenaFiscal").val(row.contrasenaFiscal || "");
    $("#inpPagWeb").val(row.pagWeb || "");
    $("#inpEstado").val(row.estado || "");
    $("#inpObservaciones").val(row.observaciones || "");

    abrirModal("dlgCorreoDominio");
}

function guardarCorreoDominio() {
    const costoTexto = $("#inpCostos").val();

    const data = {
        id: parseInt($("#inpCorreoDominioId").val()) || 0,
        empresa: $("#inpEmpresa").val(),
        dominio: $("#inpDominio").val(),
        proveedor: $("#inpProveedor").val(),
        fechaCaducacion: $("#inpFechaCaducacion").val() || null,
        costos: costoTexto === "" ? null : parseFloat(costoTexto),
        correoOperaciones: $("#inpCorreoOperaciones").val(),
        contrasenaOperaciones: $("#inpContrasenaOperaciones").val(),
        correoFiscal: $("#inpCorreoFiscal").val(),
        contrasenaFiscal: $("#inpContrasenaFiscal").val(),
        pagWeb: $("#inpPagWeb").val(),
        estado: $("#inpEstado").val(),
        observaciones: $("#inpObservaciones").val()
    };

    $.ajax({
        url: window.location.pathname + "?handler=SaveCorreoDominio",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(data),
        headers: {
            "RequestVerificationToken": $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        success: function (resp) {
            if (resp.success) {
                cerrarModal("dlgCorreoDominio");
                cargarCorreosDominios();

                mostrarMensajeCorreoDominio(
                    "success",
                    "Editar Registro",
                    resp.message || "Registro guardado satisfactoriamente."
                );
            } else {
                mostrarMensajeCorreoDominio(
                    "error",
                    "Error",
                    resp.message || "No se pudo guardar el registro."
                );
            }
        },
        error: function (xhr) {
            console.error("Error guardar:", xhr.responseText);

            mostrarMensajeCorreoDominio(
                "error",
                "Error",
                "Ocurrió un error al guardar el registro."
            );
        }
    });
}

function eliminarCorreoDominio(id) {
    mostrarConfirmacionCorreoDominio(
        "Eliminar Registro",
        "¿Desea eliminar este registro?",
        function () {
            $.ajax({
                url: window.location.pathname + "?handler=DeleteCorreoDominio",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(id),
                headers: {
                    "RequestVerificationToken": $('input:hidden[name="__RequestVerificationToken"]').val()
                },
                success: function (resp) {
                    if (resp.success) {
                        cargarCorreosDominios();

                        mostrarMensajeCorreoDominio(
                            "success",
                            "Eliminar Registro",
                            resp.message || "Registro eliminado correctamente."
                        );
                    } else {
                        mostrarMensajeCorreoDominio(
                            "error",
                            "Error",
                            resp.message || "No se pudo eliminar el registro."
                        );
                    }
                },
                error: function (xhr) {
                    console.error("Error eliminar:", xhr.responseText);

                    mostrarMensajeCorreoDominio(
                        "error",
                        "Error",
                        "Ocurrió un error al eliminar el registro."
                    );
                }
            });
        }
    );
}

function importarCorreosDominios() {
    const archivo = $("#inpExcelCorreosDominios")[0].files[0];

    if (!archivo) {
        mostrarMensajeCorreoDominio(
            "warning",
            "Archivo requerido",
            "Seleccione un archivo Excel."
        );
        return;
    }

    const formData = new FormData();
    formData.append("archivo", archivo);

    $.ajax({
        url: window.location.pathname + "?handler=ImportarCorreosDominios",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        headers: {
            "RequestVerificationToken": $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        success: function (resp) {
            console.log("Respuesta importación:", resp);

            if (resp.success) {
                $("#inpExcelCorreosDominios").val("");
                cerrarModal("dlgImportacionMasiva");
                cargarCorreosDominios();

                mostrarMensajeCorreoDominio(
                    "success",
                    "Importación Masiva",
                    resp.message || "Importación realizada correctamente."
                );
            } else {
                mostrarMensajeCorreoDominio(
                    "error",
                    "Error",
                    resp.message || "No se pudo importar el registro."
                );
            }
        },
        error: function (xhr) {
            console.error("Error importación:", xhr.responseText);

            mostrarMensajeCorreoDominio(
                "error",
                "Error al importar",
                `Error del servidor: ${xhr.status} - ${xhr.statusText}`
            );
        }
    });
}

function convertirFechaInput(fecha) {
    if (!fecha) return "";

    const partes = fecha.split("/");

    if (partes.length === 3) {
        return `${partes[2]}-${partes[1]}-${partes[0]}`;
    }

    return fecha;
}

function abrirModal(id) {
    const modalElement = document.getElementById(id);
    const modal = bootstrap.Modal.getOrCreateInstance(modalElement);
    modal.show();
}

function cerrarModal(id) {
    const modalElement = document.getElementById(id);
    const modal = bootstrap.Modal.getOrCreateInstance(modalElement);

    modal.hide();

    setTimeout(function () {
        $(".modal-backdrop").remove();
        $("body").removeClass("modal-open").css("padding-right", "");
    }, 250);
}

$(document).on("click", "[data-bs-dismiss='modal']", function () {
    const modalElement = $(this).closest(".modal")[0];

    if (modalElement) {
        cerrarModal(modalElement.id);
    }
});

function inicializarModalMensajeCorreoDominio() {
    if ($("#dlgMensajeCorreoDominio").length > 0) return;

    $("body").append(`
        <div class="modal fade" id="dlgMensajeCorreoDominio" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered modal-mensaje-correo-dominio">
                <div class="modal-content mensaje-modal-content">

                    <div class="modal-header mensaje-modal-header">
                        <h5 id="dlgMensajeCorreoDominioTitulo" class="modal-title">Mensaje</h5>
                        <button type="button"
                                class="btn-close mensaje-modal-close"
                                data-bs-dismiss="modal"
                                aria-label="Close">
                        </button>
                    </div>

                    <div class="modal-body mensaje-modal-body">
                        <div id="dlgMensajeCorreoDominioBox" class="mensaje-box">
                            <div id="dlgMensajeCorreoDominioIcono" class="mensaje-icono">
                                <i class="bi bi-check-lg"></i>
                            </div>

                            <div id="dlgMensajeCorreoDominioTexto" class="mensaje-texto">
                                Registro guardado correctamente.
                            </div>
                        </div>
                    </div>

                    <div class="modal-footer mensaje-modal-footer">
                        <button type="button"
                                class="btn btn-primary btn-mensaje-ok"
                                data-bs-dismiss="modal">
                            Ok
                        </button>
                    </div>

                </div>
            </div>
        </div>
    `);
}

function mostrarMensajeCorreoDominio(tipo, titulo, texto) {
    inicializarModalMensajeCorreoDominio();

    const box = $("#dlgMensajeCorreoDominioBox");
    const icono = $("#dlgMensajeCorreoDominioIcono");

    box.removeClass("mensaje-success mensaje-error mensaje-warning");
    icono.html("");

    if (tipo === "success") {
        box.addClass("mensaje-success");
        icono.html('<i class="bi bi-check-lg"></i>');
    } else if (tipo === "warning") {
        box.addClass("mensaje-warning");
        icono.html('<i class="bi bi-exclamation-lg"></i>');
    } else {
        box.addClass("mensaje-error");
        icono.html('<i class="bi bi-x-lg"></i>');
    }

    $("#dlgMensajeCorreoDominioTitulo").text(titulo || "Mensaje");
    $("#dlgMensajeCorreoDominioTexto").text(texto || "");

    abrirModal("dlgMensajeCorreoDominio");
}

function mostrarConfirmacionCorreoDominio(titulo, texto, onConfirmar) {
    inicializarModalMensajeCorreoDominio();

    const box = $("#dlgMensajeCorreoDominioBox");
    const icono = $("#dlgMensajeCorreoDominioIcono");

    box.removeClass("mensaje-success mensaje-error mensaje-warning");
    box.addClass("mensaje-warning");

    icono.html('<i class="bi bi-exclamation-lg"></i>');

    $("#dlgMensajeCorreoDominioTitulo").text(titulo || "Confirmar");
    $("#dlgMensajeCorreoDominioTexto").text(texto || "");

    $(".mensaje-modal-footer").html(`
        <button type="button"
                class="btn btn-secondary btn-mensaje-cancelar"
                data-bs-dismiss="modal">
            Cancelar
        </button>

        <button type="button"
                id="btnConfirmarMensajeCorreoDominio"
                class="btn btn-primary btn-mensaje-ok">
            Ok
        </button>
    `);

    $("#btnConfirmarMensajeCorreoDominio").off("click").on("click", function () {
        cerrarModal("dlgMensajeCorreoDominio");

        $(".mensaje-modal-footer").html(`
            <button type="button"
                    class="btn btn-primary btn-mensaje-ok"
                    data-bs-dismiss="modal">
                Ok
            </button>
        `);

        if (typeof onConfirmar === "function") {
            onConfirmar();
        }
    });

    abrirModal("dlgMensajeCorreoDominio");
}

function fechaCaducacionFormatter(value, row) {
    if (!value) return "";

    const partes = value.split("/");
    if (partes.length !== 3) return value;

    const fecha = new Date(partes[2], partes[1] - 1, partes[0]);
    const hoy = new Date();

    hoy.setHours(0, 0, 0, 0);
    fecha.setHours(0, 0, 0, 0);

    const dias = Math.ceil((fecha - hoy) / (1000 * 60 * 60 * 24));

    if (dias <= 0) {
        return `<span class="badge-fecha-caducada">${value}</span>`;
    }

    if (dias <= 7) {
        return `<span class="badge-fecha-proxima">${value}</span>`;
    }

    return value;
}

// =====================================================
// CONFIGURACION DE AVISOS (solo Administrador en backend)
// =====================================================
$(document).on("click", "#btnConfigAvisos", async function () {
    abrirModal("dlgConfigAvisos");
    await cargarConfigAvisos();
});

function tokenAvisos() {
    return $('input:hidden[name="__RequestVerificationToken"]').val();
}
function mostrarErrorAvisos(mensaje) {
    $("#avisoConfigError").text(mensaje || "Ocurrió un error.").removeClass("d-none");
}
function escaparAvisos(texto) {
    return $("<div>").text(texto == null ? "" : String(texto)).html();
}
function mensajeErrorAjaxAvisos(xhr) {
    return xhr.responseJSON?.message || (xhr.status === 403 ? "No tienes permisos para esta operación." : "No fue posible completar la operación.");
}
async function cargarConfigAvisos() {
    $("#avisoConfigError").addClass("d-none");
    try {
        const data = await $.ajax({ url: window.location.pathname + "?handler=ConfiguracionAvisos", type: "GET" });
        const c = data.configuracion;
        $("#avisoHabilitado").prop("checked", c.habilitado);
        $("#avisoDiasAnticipacion").val(c.diasAnticipacion);
        $("#avisoDiasEscalamiento").val(c.diasEscalamiento);
        $("#avisoDiarios").prop("checked", c.recordatoriosDiarios);
        $("#avisoVencidos").prop("checked", c.notificarVencidos);
        const sel = $("#avisoUsuario").empty().append($("<option>").val("").text("-- Elegir usuario --"));
        for (const u of data.usuarios) sel.append($("<option>").val(u.id).text(u.email + (u.userName && u.userName !== u.email ? " (" + u.userName + ")" : "")));
        const mapa = new Map(data.usuarios.map(u => [u.id, u.email]));
        const tbody = $("#tablaDestinatariosAvisos").empty();
        if (!data.destinatarios.length) tbody.append($("<tr>").append($("<td colspan='5' class='text-muted text-center'>").text("Todavía no hay destinatarios configurados.")));
        for (const d of data.destinatarios) {
            const correo = d.userId ? (mapa.get(d.userId) || "Usuario sin correo/disponibilidad") : d.correoElectronico;
            const btn = $("<button type='button' class='btn btn-sm'>")
                .addClass(d.activo ? "btn-outline-danger" : "btn-outline-success")
                .text(d.activo ? "Desactivar" : "Activar")
                .on("click", () => cambiarEstadoDestinatarioAvisos(d.id, !d.activo));
            tbody.append($("<tr>")
                .append($("<td>").text(d.tipoDestinatario || "—"))
                .append($("<td>").text(d.nombre || "—"))
                .append($("<td>").text(correo || "—"))
                .append($("<td>").text(d.activo ? "Activo" : "Inactivo"))
                .append($("<td>").append(btn)));
        }
    } catch (e) { mostrarErrorAvisos(mensajeErrorAjaxAvisos(e)); }
}

$(document).on("change", "#avisoUsuario", function () {
    if ($(this).val()) $("#avisoCorreoExterno").val("");
});
$(document).on("input", "#avisoCorreoExterno", function () {
    if ($(this).val().trim()) $("#avisoUsuario").val("");
});
$(document).on("click", "#btnGuardarConfigAvisos", async function () {
    const dias = Number($("#avisoDiasAnticipacion").val());
    const escalar = Number($("#avisoDiasEscalamiento").val());
    if (!Number.isInteger(dias) || dias < 1 || dias > 365 || !Number.isInteger(escalar) || escalar < 0 || escalar > dias) {
        mostrarErrorAvisos("Verifica que los días estén completos y que escalamiento no supere anticipación."); return;
    }
    try {
        await $.ajax({
            url: window.location.pathname + "?handler=GuardarConfiguracionAvisos", type: "POST", contentType: "application/json", headers: { RequestVerificationToken: tokenAvisos() },
            data: JSON.stringify({
                habilitado: $("#avisoHabilitado").is(":checked"), diasAnticipacion: dias, diasEscalamiento: escalar,
                recordatoriosDiarios: $("#avisoDiarios").is(":checked"), notificarVencidos: $("#avisoVencidos").is(":checked")
            })
        });
        $("#avisoConfigError").addClass("d-none");
        mostrarMensajeCorreoDominio("success", "Configuración guardada", "Se actualizaron las reglas de notificación.");
    } catch (e) { mostrarErrorAvisos(mensajeErrorAjaxAvisos(e)); }
});
$(document).on("click", "#btnAgregarDestinatarioAvisos", async function () {
    const userId = $("#avisoUsuario").val() || null;
    const correoElectronico = $("#avisoCorreoExterno").val().trim() || null;
    const nombre = $("#avisoNombre").val().trim() || null;
    const tipoDestinatario = $("#avisoTipoDestinatario").val();
    if (!!userId === !!correoElectronico) { mostrarErrorAvisos("Selecciona un usuario o captura un correo externo."); return; }
    try {
        await $.ajax({
            url: window.location.pathname + "?handler=AgregarDestinatarioAvisos", type: "POST", contentType: "application/json", headers: { RequestVerificationToken: tokenAvisos() },
            data: JSON.stringify({ userId, correoElectronico, nombre, tipoDestinatario })
        });
        $("#avisoUsuario").val(""); $("#avisoCorreoExterno").val(""); $("#avisoNombre").val("");
        await cargarConfigAvisos();
    } catch (e) { mostrarErrorAvisos(mensajeErrorAjaxAvisos(e)); }
});
async function cambiarEstadoDestinatarioAvisos(id, activo) {
    try {
        await $.ajax({ url: window.location.pathname + "?handler=CambiarEstadoDestinatarioAvisos", type: "POST", contentType: "application/json", headers: { RequestVerificationToken: tokenAvisos() }, data: JSON.stringify({ id, activo }) });
        await cargarConfigAvisos();
    } catch (e) { mostrarErrorAvisos(mensajeErrorAjaxAvisos(e)); }
}


// =====================================================
// SIMULACIÓN / VISTA PREVIA: SOLO LECTURA
// =====================================================
$(document).on("click", "#btnSimularAvisos", async function () {
    const btn = $(this);
    btn.prop("disabled", true).text("Consultando...");
    $("#avisoConfigError").addClass("d-none");
    try {
        const d = await $.ajax({
            url: window.location.pathname + "?handler=SimularAvisos",
            type: "GET",
            cache: false
        });
        $("#panelSimulacionAvisos").removeClass("d-none");
        const estado = d.configuracionHabilitada ? "Habilitada" : "Deshabilitada (simulación disponible)";
        $("#simulacionResumenAvisos").empty()
            .append($("<strong>").text("Corte: " + d.fechaCorte + " | Vencidos: " + d.totalVencidos + " | Próximos: " + d.totalProximos))
            .append($("<div class='small text-muted'>").text("Configuración guardada: " + estado + ". Resumen de vencidos: " + (d.resumenVencidos ? "Sí" : "No")));
        const dest = $("#simulacionDestinatariosAvisos").empty();
        dest.append($("<div>").text("Seguimiento: " + (d.destinatariosSeguimiento.join(", ") || "Ninguno")));
        dest.append($("<div>").text("Escalamiento: " + (d.destinatariosEscalamiento.join(", ") || "Ninguno")));
        dest.append($("<div>").text("Recibirían resumen vencidos: " + (d.destinatariosResumen.join(", ") || "Ninguno")));
        $("#simulacionAsuntoAvisos").text("Asunto previsto: " + (d.resumenVencidos ? d.asuntoResumen : "No se generaría resumen con las reglas actuales"));
        const tb = $("#simulacionTablaVencidos").empty();
        if (!d.vencidos.length) tb.append($("<tr>").append($("<td colspan='5' class='text-muted text-center'>").text("Sin registros vencidos.")));
        for (const r of d.vencidos) {
            const tr = $("<tr>");
            for (const v of [r.empresa, r.dominio, r.proveedor, r.fechaCaducacion, r.diasAtraso]) tr.append($("<td>").text(v ?? "—"));
            tb.append(tr);
        }
        const tp = $("#simulacionTablaProximos").empty();
        if (!d.proximos.length) tp.append($("<tr>").append($("<td colspan='5' class='text-muted text-center'>").text("No hay registros próximos a vencer.")));
        for (const r of d.proximos) {
            const tr = $("<tr>");
            for (const v of [r.empresa, r.dominio, r.fechaCaducacion, r.diasRestantes, r.categoria]) tr.append($("<td>").text(v ?? "—"));
            tp.append(tr);
        }
        $("#simulacionNotaAvisos").text(d.nota + " Guarda las reglas antes de simular cambios.");
    } catch (err) {
        mostrarErrorAvisos(mensajeErrorAjaxAvisos(err));
    } finally {
        btn.prop("disabled", false).html('<i class="bi bi-eye"></i> Simular avisos');
    }
});
