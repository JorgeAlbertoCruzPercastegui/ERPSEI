var table;
var buttonRemove;
var selections = [];
var dlg = null;
var dlgModal = null;

const NUEVO = 0;
const EDITAR = 1;
const VER = 2;
const postOptions = { headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() } }

document.addEventListener("DOMContentLoaded", function (event) {
    table = $("#table");
    buttonRemove = $("#remove");
    buttonExportAll = $("#exportAll");
    dlg = document.getElementById('dlgDocumentacion');

    if (dlg) {
        dlgModal = new bootstrap.Modal(dlg, null);

        dlg.addEventListener('hidden.bs.modal', function (event) {
            onCerrarClick();
        });
    } else {
        console.error("No se encontró el modal con id #dlgDocumentacion");
    }

    initTable();
});

//Funcionalidad Tabla
function getIdSelections() {
    return $.map(table.bootstrapTable('getSelections'), function (row) {
        return row.id
    })
}
function responseHandler(res) {
    $.each(res, function (i, row) {
        row.state = $.inArray(row.id, selections) !== -1
    })
    return res
}

//Función para dar formato a los iconos de operación de los registros
function operateFormatter(value, row, index) {
    let icons = [];

    //Icono Ver
    icons.push(`<li><a class="dropdown-item see" href="#" title="${btnVerTitle}"><i class="bi bi-search"></i> ${btnVerTitle}</a></li>`);
    //Icono Editar
    icons.push(`<li><a class="dropdown-item edit" href="#" title="${btnEditarTitle}"><i class="bi bi-pencil-fill"></i> ${btnEditarTitle}</a></li>`);

    return `<div class="dropdown">
              <button class="btn" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                <i class="bi bi-three-dots-vertical success"></i>
              </button>
              <ul class="dropdown-menu">${icons.join("")}</ul>
            </div>`;
}
window.operateEvents = {
    'click .see': function (e, value, row, index) {
        initDocumentacionDialog(VER, row);
    },
    'click .edit': function (e, value, row, index) {
        initDocumentacionDialog(EDITAR, row);
        //table.bootstrapTable('remove', {
        //    field: 'id',
        //    values: [row.id]
        //})
    }
}

function additionalButtons() {
    return {
        btnImport: {
            text: btnImportarText,
            icon: 'bi-upload',
            event: function () { },
            attributes: {
                "title": btnImportarTitle,
                "data-bs-toggle": "modal",
                "data-bs-target": "#dlgImportarExcel"
            }
        }
    }
}

function onAgregarClick() {
    initDocumentacionDialog(NUEVO, { id: "Nuevo", nombre: "" });
}


function initTable() {
    table.bootstrapTable('destroy').bootstrapTable({
        //url: '/ERP/ActivosFijos?handler=ActivosFijosList',
        height: 550,
        locale: cultureName,
        exportDataType: 'all',
        exportTypes: ['excel'],
        columns: [
            {
                field: "state",
                checkbox: true,
                align: "center",
                valign: "middle"
            },
            {
                title: "Id",
                field: "id",
                align: "center",
                valign: "middle",
                sortable: true,
                width: "80px"
            },
            {
                title: "Area",
                field: "area",
                align: "center",
                valign: "middle",
                sortable: true,
                width: "80px"
            },
            {
                title: "TipoDocumento",
                field: "tipoDocumento",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "Titulo",
                field: "titulo",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "Descripcion",
                field: "descripcion",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "CreadoPorId",
                field: "creadoPorId",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "ModificadoPorId",
                field: "modificadoPorId",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "Activo",
                field: "activo",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colAccionesHeader,
                field: "operate",
                align: 'center',
                width: "100px",
                clickToSelect: false,
                events: window.operateEvents,
                formatter: operateFormatter
            }
        ]
    })
    table.on('check.bs.table uncheck.bs.table ' +
        'check-all.bs.table uncheck-all.bs.table',
        function () {
            buttonRemove.prop('disabled', !table.bootstrapTable('getSelections').length)
            buttonExportAll.prop('disabled', !table.bootstrapTable('getSelections').length)

            // save your data, here just save the current page
            selections = getIdSelections()
            // push or splice the selections if you want to save all data selections
        })
    table.on('all.bs.table', function (e, name, args) {
        console.log(name, args)
    })
    buttonRemove.click(function () {
        askConfirmation(dlgDeleteTitle, dlgDeleteQuestion, function () {
            let oParams = { ids: selections };

            doAjax(
                "/Reportes/Documentacion/DeleteDocumentacion",
                oParams,
                function (resp) {
                    if (resp.tieneError) {
                        showError(dlgDeleteTitle, resp.mensaje);
                        return;
                    }

                    table.bootstrapTable('remove', {
                        field: 'id',
                        values: selections
                    })
                    selections = [];
                    buttonRemove.prop('disabled', true);
                    buttonExportAll.prop('disabled', true);

                    let e = document.querySelector("[name='refresh']");
                    e.click();

                    showSuccess(dlgDeleteTitle, resp.mensaje);
                }, function (error) {
                    showError(dlgDeleteTitle, error);
                },
                postOptions
            );

        });
    })
}

// Funcionalidad Diálogo - DOCUMENTOS
function initDocumentacionDialog(action, row) {

    let idField = document.getElementById("inpDocumentacionId");
    let areaField = document.getElementById("inpDocumentacionArea");
    let tipoDocumentoField = document.getElementById("inpDocumentacionTipoDocumento");
    let estatusField = document.getElementById("inpDocumentacionEstatusDocumento");
    let tituloField = document.getElementById("inpDocumentacionTitulo");
    let descripcionField = document.getElementById("inpDocumentacionDescripcion");
    let observacionesField = document.getElementById("inpDocumentacionObservaciones");
    let archivoField = document.getElementById("inpDocumentacionArchivo");

    let btnGuardar = document.getElementById("dlgDocumentoBtnGuardar");
    let dlgTitle = document.getElementById("dlgDocumentacionTitle");
    let summaryContainer = document.getElementById("saveValidationSummary");
    summaryContainer.innerHTML = "";

    // ID siempre deshabilitado
    idField.setAttribute("disabled", true);

    // Helpers
    const setDisabled = (el, disabled) => {
        if (!el) return;
        if (disabled) el.setAttribute("disabled", true);
        else el.removeAttribute("disabled");
    };

    const setTitleByAction = () => {
        // Usa tus variables si ya existen (dlgNuevoTitle/dlgEditarTitle/dlgVerTitle).
        // Si no existen, puedes dejar estos textos fijos.
        if (action === NUEVO) dlgTitle.innerHTML = (typeof dlgNuevoTitle !== "undefined") ? dlgNuevoTitle : "Nuevo Documento";
        else if (action === EDITAR) dlgTitle.innerHTML = (typeof dlgEditarTitle !== "undefined") ? dlgEditarTitle : "Editar Documento";
        else dlgTitle.innerHTML = (typeof dlgVerTitle !== "undefined") ? dlgVerTitle : "Ver Documento";
    };

    setTitleByAction();

    // Habilitar/Deshabilitar según acción
    switch (action) {
        case NUEVO:
            // En nuevo: habilitar todo excepto ID
            setDisabled(areaField, false);
            setDisabled(tipoDocumentoField, false);
            setDisabled(estatusField, false);
            setDisabled(tituloField, false);
            setDisabled(descripcionField, false);
            setDisabled(observacionesField, false);
            setDisabled(archivoField, false);

            setDisabled(btnGuardar, false);
            break;

        case EDITAR:
            // En editar: habilitar (tú decides si permites cambiar archivo)
            setDisabled(areaField, false);
            setDisabled(tipoDocumentoField, false);
            setDisabled(estatusField, false);
            setDisabled(tituloField, false);
            setDisabled(descripcionField, false);
            setDisabled(observacionesField, false);

            // Si quieres permitir cambiar PDF en editar, deja false. Si no, true.
            setDisabled(archivoField, false);

            setDisabled(btnGuardar, false);
            break;

        default:
            // VER: todo disabled
            setDisabled(areaField, true);
            setDisabled(tipoDocumentoField, true);
            setDisabled(estatusField, true);
            setDisabled(tituloField, true);
            setDisabled(descripcionField, true);
            setDisabled(observacionesField, true);
            setDisabled(archivoField, true);

            setDisabled(btnGuardar, true);
            break;
    }

    // =========================
    // Asignación de valores
    // =========================
    if (action === NUEVO) {
        idField.value = "Nuevo";
        areaField.value = "";
        tipoDocumentoField.value = "";
        estatusField.value = "";
        tituloField.value = "";
        descripcionField.value = "";
        observacionesField.value = "";

        // Limpia file input
        if (archivoField) archivoField.value = "";
    } else {
        // Row viene del grid
        idField.value = row?.id ?? "";

        // OJO: asegúrate que tu JSON del list traiga estos campos:
        // areaId, tipoDocumentoId, estatusDocumentoId, titulo, descripcion, observaciones
        areaField.value = row?.areaId != null ? row.areaId.toString() : "";
        tipoDocumentoField.value = row?.tipoDocumentoId != null ? row.tipoDocumentoId.toString() : "";
        estatusField.value = row?.estatusDocumentoId != null ? row.estatusDocumentoId.toString() : "";

        tituloField.value = row?.titulo ?? "";
        descripcionField.value = row?.descripcion ?? "";
        observacionesField.value = row?.observaciones ?? "";

        // Por seguridad, no se puede “precargar” un file input desde JS (browser restriction)
        if (archivoField) archivoField.value = "";
    }

    // Mostrar modal
    dlgModal.show();
}


function onGuardarClick() {
    $("#theForm").validate();
    let valid = $("#theForm").valid();
    if (!valid) { return; }

    let btnClose = document.getElementById("dlgDocumentoBtnCancelar");

    let idField = document.getElementById("inpDocumentacionId");
    let areaField = document.getElementById("inpDocumentacionArea");
    let tipoDocumentoField = document.getElementById("inpDocumentacionTipoDocumento");
    let tituloField = document.getElementById("inpDocumentacionTitulo");
    let descripcionField = document.getElementById("inpDocumentacionDescripcion");
    let estatusField = document.getElementById("inpDocumentacionEstatusDocumento");
    let archivoField = document.getElementById("inpDocumentacionArchivo");
    let observacionesField = document.getElementById("inpDocumentacionObservaciones");

    let dlgTitle = document.getElementById("dlgDocumentacionTitle");

    let summaryContainer = document.getElementById("saveValidationSummary");
    summaryContainer.innerHTML = "";

    let oParams = new FormData();

    // ✅ Anti-forgery token
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
    if (token) oParams.append("__RequestVerificationToken", token);

    // ✅ Prefijo input. para que ligue con: OnPostSaveDocumento(DcoumentacionTableModel input)
    oParams.append("input.Id", (idField.value === "Nuevo" || idField.value === "") ? 0 : parseInt(idField.value));

    oParams.append("input.AreaId", areaField.value || "");
    oParams.append("input.TipoDocumentoId", tipoDocumentoField.value || "");
    oParams.append("input.EstatusDocumentoId", estatusField.value || "");

    oParams.append("input.Titulo", tituloField.value || "");
    oParams.append("input.Descripcion", descripcionField.value || "");
    oParams.append("input.Observaciones", observacionesField.value || "");

    // Archivo
    if (archivoField.files && archivoField.files.length > 0) {
        oParams.append("input.Archivo", archivoField.files[0]);
    }

    // multipart
    let postOptions = { processData: false, contentType: false };

    doAjax(
        "/Reportes/Documentacion/SaveDocumento",
        oParams,
        function (resp) {
            if (resp.tieneError) {
                if (Array.isArray(resp.errores) && resp.errores.length >= 1) {
                    let summary = ``;
                    resp.errores.forEach(function (error) {
                        summary += `<li>${error}</li>`;
                    });
                    summaryContainer.innerHTML += `<ul>${summary}</ul>`;
                }
                showError(dlgTitle.innerHTML, resp.mensaje);
                return;
            }

            btnClose.click();
            document.querySelector("[name='refresh']").click();
            showSuccess(dlgTitle.innerHTML, resp.mensaje);
        },
        function (error) {
            showError("Error", error);
        },
        postOptions
    );
}


function onCerrarClick() {
    //Removes validation from input-fields
    $('.input-validation-error').addClass('input-validation-valid');
    $('.input-validation-error').removeClass('input-validation-error');
    //Removes validation message after input-fields
    $('.field-validation-error').addClass('field-validation-valid');
    $('.field-validation-error').removeClass('field-validation-error');
    //Removes validation summary 
    $('.validation-summary-errors').addClass('validation-summary-valid');
    $('.validation-summary-errors').removeClass('validation-summary-errors');
    //Removes danger text from fields
    $(".text-danger").children().remove()
}
