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