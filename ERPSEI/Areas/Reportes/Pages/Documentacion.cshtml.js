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
                title: "FechaModificacion",
                field: "fechaModificacion",
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
            
            selections = getIdSelections()
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

function initDocumentacionDialog(action, row) {

    let idField = document.getElementById("inpDocumentacionId");
    let areaField = document.getElementById("inpDocumentacionArea");
    let tipoDocumentoField = document.getElementById("inpDocumentacionTipoDocumento");
    let estatusField = document.getElementById("inpDocumentacionEstatusDocumento");
    let tituloField = document.getElementById("inpDocumentacionTitulo");
    let descripcionField = document.getElementById("inpDocumentacionDescripcion");
    let creadoPorIdField = document.getElementById("inpDocumentacionCreadoPor");
    let modificadoPorIdField = document.getElementById("inpDocumentacionModificadoPor");
    let nombreArchivoField = document.getElementById("inpDocumentacionNombreArchivo");
    let responsableField = document.getElementById("inpDocumentacionRespoonsable");
    let rutaArchivoField = document.getElementById("inpDocumentacionRutaArchivo");
    let ubicacionField = document.getElementById("inpDocumentacionUbicacion");
    let observacionesField = document.getElementById("inpDocumentacionObservaciones");
    let archivoField = document.getElementById("inpDocumentacionArchivo");

    let fechaCreacionField = document.getElementById("inpDocumentacionFechaCreacion");
    let fechaModificacionField = document.getElementById("inpDocumentacionFechaModificacion");


    let btnGuardar = document.getElementById("dlgDocumentoBtnGuardar");
    let dlgTitle = document.getElementById("dlgDocumentacionTitle");
    let summaryContainer = document.getElementById("saveValidationSummary");
    if (summaryContainer) summaryContainer.innerHTML = "";

    if (idField) idField.setAttribute("disabled", true);

    const setDisabled = (el, disabled) => {
        if (!el) return;
        if (disabled) el.setAttribute("disabled", true);
        else el.removeAttribute("disabled");
    };

    const setTitleByAction = () => {
        if (!dlgTitle) return;
        
        if (action === NUEVO) dlgTitle.innerHTML = (typeof dlgNuevoTitle !== "undefined") ? dlgNuevoTitle : "Nuevo Documento";
        else if (action === EDITAR) dlgTitle.innerHTML = (typeof dlgEditarTitle !== "undefined") ? dlgEditarTitle : "Editar Documento";
        else dlgTitle.innerHTML = (typeof dlgVerTitle !== "undefined") ? dlgVerTitle : "Ver Documento";
    };

    setTitleByAction();
    
    const esVer = !(action === NUEVO || action === EDITAR);
    
    setDisabled(areaField, esVer);
    setDisabled(tipoDocumentoField, esVer);
    setDisabled(estatusField, esVer);
    setDisabled(tituloField, esVer);
    setDisabled(descripcionField, esVer);
    setDisabled(observacionesField, esVer);
    
    setDisabled(creadoPorIdField, esVer);
    setDisabled(modificadoPorIdField, esVer);
    setDisabled(nombreArchivoField, esVer);
    setDisabled(responsableField, esVer);
    setDisabled(rutaArchivoField, esVer);
    setDisabled(ubicacionField, esVer);
    
    if (action === EDITAR) setDisabled(archivoField, false);
    else setDisabled(archivoField, esVer);
    
    setDisabled(btnGuardar, esVer);

    if (action === NUEVO) {
        if (idField) idField.value = "Nuevo";
        if (areaField) areaField.value = "";
        if (tipoDocumentoField) tipoDocumentoField.value = "";
        if (estatusField) estatusField.value = "";
        if (tituloField) tituloField.value = "";
        if (descripcionField) descripcionField.value = "";
        if (observacionesField) observacionesField.value = "";
        
        if (creadoPorIdField) creadoPorIdField.value = "";
        if (modificadoPorIdField) modificadoPorIdField.value = "";
        if (nombreArchivoField) nombreArchivoField.value = "";
        if (responsableField) responsableField.value = "";
        if (rutaArchivoField) rutaArchivoField.value = "";
        if (ubicacionField) ubicacionField.value = "";
        
        if (archivoField) archivoField.value = "";
        
        const hoy = new Date().toISOString().split('T')[0];
        if (fechaCreacionField) fechaCreacionField.value = hoy;
        if (fechaModificacionField) fechaModificacionField.value = "";

    } else {
        
        if (idField) idField.value = row?.id ?? "";
        
        if (areaField) areaField.value = row?.areaId != null ? row.areaId.toString() : "";
        if (tipoDocumentoField) tipoDocumentoField.value = row?.tipoDocumentoId != null ? row.tipoDocumentoId.toString() : "";
        if (estatusField) estatusField.value = row?.estatusDocumentoId != null ? row.estatusDocumentoId.toString() : "";

        if (tituloField) tituloField.value = row?.titulo ?? "";
        if (descripcionField) descripcionField.value = row?.descripcion ?? "";
        if (observacionesField) observacionesField.value = row?.observaciones ?? "";

        
        if (creadoPorIdField) creadoPorIdField.value = row?.creadoPorId ?? "";
        if (modificadoPorIdField) modificadoPorIdField.value = row?.modificadoPorId ?? "";
        if (nombreArchivoField) nombreArchivoField.value = row?.nombreArchivo ?? "";
        if (responsableField) responsableField.value = row?.responsable ?? "";
        if (rutaArchivoField) rutaArchivoField.value = row?.rutaArchivo ?? "";
        if (ubicacionField) ubicacionField.value = row?.ubicacion ?? "";
       
        if (archivoField) archivoField.value = "";

        const fc = row?.fechaCreacion ? row.fechaCreacion.toString().substring(0, 10) : "";
        const fm = row?.fechaModificacion ? row.fechaModificacion.toString().substring(0, 10) : "";

        if (fechaCreacionField) fechaCreacionField.value = fc;
        if (fechaModificacionField) fechaModificacionField.value = fm;
    }
    
    dlgModal.show();
}

function onGuardarClick() {
    $("#theForm").validate();
    let valid = $("#theForm").valid();
    if (!valid) { return; }

    let btnClose = document.getElementById("dlgDocumentoBtnCancelar");

    let idField = document.getElementById("inpDocumentacionId");
    let tituloField = document.getElementById("inpDocumentacionTitulo");
    let descripcionField = document.getElementById("inpDocumentacionDescripcion");
    let creadoPorIdField = document.getElementById("inpDocumentacionCreadoPor");
    let fechaCrecionField = document.getElementById("inpDocumentacionFechaCreacion");
    let modificadoPorField = document.getElementById("inpDocumentacionModificadoPor");
    let fechaModificacionField = document.getElementById("inpDocumentacionFechaModificacion");
    let nombreArchivoField = document.getElementById("inpDocumentacionNombreArchivo");
    let observacionesField = document.getElementById("inpDocumentacionObservaciones");

    let responsableField = document.getElementById("inpDocumentacionRespoonsable");
    let rutaArchivoField = document.getElementById("inpDocumentacionRutaArchivo");
    let ubicacionField = document.getElementById("inpDocumentacionUbicacion");

    let dlgTitle = document.getElementById("dlgDocumentacionTitle");
    let summaryContainer = document.getElementById("saveValidationSummary");
    summaryContainer.innerHTML = "";

    let areaIdField = document.getElementById("inpDocumentacionArea");
    let tipoDcumentoField = document.getElementById("inpDocumentacionTipoDocumento");
    let estatusDocumentoIdField = document.getElementById("inpDocumentacionEstatusDocumento");

    let oParams = {
        id: idField.value === "Nuevo" ? 0 : parseInt(idField.value),
        titulo: tituloField.value,
        descripcion: descripcionField.value,
        creadoPorId: creadoPorIdField.value,
        fechaCreacion: fechaCrecionField.value,
        modificadoPor: modificadoPorField.value,
        areaId: parseInt(areaIdField?.value || 0),
        tipoDocumentoId: parseInt(tipoDcumentoField?.value || 0),
        estatusDocumentoId: parseInt(estatusDocumentoIdField?.value || 0),
        fechaModificacion: fechaModificacionField.value,
        nombreArchivo: nombreArchivoField.value,
        observaciones: observacionesField.value,
        responsable: responsableField.value,
        rutaArchivo: rutaArchivoField.value,
        ubicacion: ubicacionField.value
    };

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

function onBuscarClick() {
    let btnBuscar = document.getElementById("btnBuscar");

    let inpTitulo = document.getElementById("inpFiltroTitulo");
    let selArea = document.getElementById("selFiltroArea");
    let selTipoDocumento = document.getElementById("selFiltroTipoDocumento");
    let selEstatusDocumento = document.getElementById("selFiltroEstatusDocumento");
    let inpPalabraClave = document.getElementById("inpFiltroPalabraClave");
    let inpFechaInicio = document.getElementById("inpFiltroFechaInicio");
    let inpFechaFin = document.getElementById("inpFiltroFechaFin");

    let oParams = {
        titulo: inpTitulo?.value?.trim() || null,
        areaId: (!selArea || selArea.value === "0" || selArea.value === "") ? null : parseInt(selArea.value),
        tipoDocumentoId: (!selTipoDocumento || selTipoDocumento.value === "0" || selTipoDocumento.value === "") ? null : parseInt(selTipoDocumento.value),
        estatusDocumentoId: (!selEstatusDocumento || selEstatusDocumento.value === "0" || selEstatusDocumento.value === "") ? null : parseInt(selEstatusDocumento.value),
        palabraClave: inpPalabraClave?.value?.trim() || null,
        fechaCreacionInicio: inpFechaInicio?.value || null,
        fechaCreacionFin: inpFechaFin?.value || null
    };

    doAjax(
        "/Reportes/Documentacion/FiltrarDocumentos",
        oParams,
        function (resp) {
            if (resp.tieneError) {
                // Si manejas errores tipo "resp.errores"
                if (Array.isArray(resp.errores) && resp.errores.length > 0) {
                    let summary = resp.errores.map(error => `<li>${error}</li>`).join("");
                    let saveValidationSummary = document.getElementById("saveValidationSummary");
                    if (saveValidationSummary) saveValidationSummary.innerHTML = `<ul>${summary}</ul>`;
                }
                showError(btnBuscar?.innerHTML ?? "Buscar", resp.mensaje);
                return;
            }

            table.bootstrapTable('load', responseHandler(resp.datos));
        },
        function (error) {
            showError("Error", error);
        },
        postOptions
    );

    // ✅ Igual que tu patrón actual: resetea inputs del form
    document.querySelectorAll("#filtros .form-control").forEach(function (e) { e.value = ""; });

    // Si quieres también resetear selects:
    // document.querySelectorAll("#filtros .form-select").forEach(function (e) { e.value = "0"; });
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
