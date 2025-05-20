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
    dlg = document.getElementById('dlgActivoFijo');
    dlgModal = new bootstrap.Modal(dlg, null);
    //Función para limpiar el cuadro de diálogo cuando es cerrado
    dlg.addEventListener('hidden.bs.modal', function (event) {
        onCerrarClick();
    });

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
        initActivoFijoDialog(VER, row);
    },
    'click .edit': function (e, value, row, index) {
        initActivoFijoDialog(EDITAR, row);
        //table.bootstrapTable('remove', {
        //    field: 'id',
        //    values: [row.id]
        //})
    }
}

function onAgregarClick() {
    initActivoFijoDialog(NUEVO, { id: "Nuevo", nombre: "" });
}
function initTable() {
    table.bootstrapTable('destroy').bootstrapTable({
        url: '/ERP/ActivosFijos?handler=ActivosFijosList',
        height: 550,
        locale: cultureName,
        exportDataType: 'all',
        exportTypes: ['excel'],
        columns: [
            {
                title: "Id",
                field: "id",
                align: "center",
                valign: "middle",
                sortable: true,
                width: "80px"
            },
            {
                title: "Folio",
                field: "folio",
                align: "center",
                valign: "middle",
                sortable: true,
                width: "80px"
            },
            {
                title: "Descripcion",
                field: "descripcion",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "Responsable",
                field: "responsable",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "Categoria",
                field: "categoria",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "Tipo",
                field: "tipo",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "FechaCompra",
                field: "fechaCompra",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "Precio",
                field: "precio",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "LinkFacturaCompra",
                field: "linkFacturaCompra",
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
                "/ERP/ActivosFijos/DeleteActivosFijos",
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


/////////////////////

//Funcionalidad Diálogo
function initActivoFijoDialog(action, row) {
    let idField = document.getElementById("inpActivoFijoId");
    let folioField = document.getElementById("inpActivoFijoFolio");
    let descripcionField = document.getElementById("inpActivoFijoDescripcion");
    let responsableField = document.getElementById("inpActivoFijoResponsable");
    let categoriaField = document.getElementById("inpActivoFijoCategoria");
    let tipoField = document.getElementById("inpActivoFijoTipo");
    let fechacompraField = document.getElementById("inpActivoFijoFechaCompra");
    let precioField = document.getElementById("inpActivoFijoPrecio");
    let linkfacturaField = document.getElementById("inpActivoFijoLinkFacturaCompra");
    let btnGuardar = document.getElementById("dlgActivoFijoBtnGuardar");
    let dlgTitle = document.getElementById("dlgActivoFijoTitle");
    let summaryContainer = document.getElementById("saveValidationSummary");
    summaryContainer.innerHTML = "";

    idField.setAttribute("disabled", true);

    switch (action) {
        case NUEVO:
            dlgTitle.innerHTML = dlgNuevoTitle;

            idField.removeAttribute("disabled");
            folioField.removeAttribute("disabled");
            descripcionField.removeAttribute("disabled");
            responsableField.removeAttribute("disabled");
            categoriaField.removeAttribute("disabled");
            tipoField.removeAttribute("disabled");
            fechacompraField.removeAttribute("disabled");
            precioField.removeAttribute("disabled");
            linkfacturaField.removeAttribute("disabled");
            btnGuardar.removeAttribute("disabled");
            break;
        case EDITAR:
            dlgTitle.innerHTML = dlgEditarTitle;

            idField.removeAttribute("disabled");
            folioField.removeAttribute("disabled");
            descripcionField.removeAttribute("disabled");
            responsableField.removeAttribute("disabled");
            categoriaField.removeAttribute("disabled");
            tipoField.removeAttribute("disabled");
            fechacompraField.removeAttribute("disabled");
            precioField.removeAttribute("disabled");
            linkfacturaField.removeAttribute("disabled");
            btnGuardar.removeAttribute("disabled");
            break;
        default:
            dlgTitle.innerHTML = dlgVerTitle;

            idField.setAttribute("disabled", true);
            folioField.setAttribute("disabled", true);
            descripcionField.setAttribute("disabled", true);
            responsableField.setAttribute("disabled", true);
            categoriaField.setAttribute("disabled", true);
            tipoField.setAttribute("disabled", true);
            fechacompraField.setAttribute("disabled", true);
            precioField.setAttribute("disabled", true);
            linkfacturaField.setAttribute("disabled", true);
            btnGuardar.setAttribute("disabled", true);
            break;
    }

    idField.value = row.id ?? "";
    folioField.value = row.folio ?? "";
    descripcionField.value = row.descripcion ?? "";
    responsableField.value = row.responsable ?? "";
    categoriaField.value = row.categoriaId ?? row.categoria ?? "";
    tipoField.value = row.tipoId ?? row.tipo ?? "";
    if (row.fechaCompra) {
        try {
            // Detectar si viene como dd/MM/yyyy
            if (row.fechaCompra.includes("/")) {
                const [dia, mes, anio] = row.fechaCompra.split("/");
                fechacompraField.value = `${anio}-${mes.padStart(2, "0")}-${dia.padStart(2, "0")}`;
            } else {
                // Si ya viene en formato ISO
                const fecha = new Date(row.fechaCompra);
                fechacompraField.value = fecha.toISOString().split("T")[0];
            }
        } catch (e) {
            fechacompraField.value = "";
        }
    } else {
        fechacompraField.value = "";
    }

    precioField.value = row.precio ?? "";
    linkfacturaField.value = row.linkFacturaCompra || '';


    dlgModal.toggle();
}

function onAgregarClick() {
    initActivoFijoDialog(NUEVO, {
        id: "Nuevo",
        folio: "",
        descripcion: "",
        responsable: "",
        categoria: "",
        tipo: "",
        fechaCompra: "",
        precio: "",
        linkFacturaCompra: ""
    });
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

function onGuardarClick() {
    //Ejecuta la validación
    $("#theForm").validate();
    //Determina los errores
    let valid = $("#theForm").valid();
    //Si la forma no es válida, entonces finaliza.
    if (!valid) { return; }

    let btnClose = document.getElementById("dlgActivoFijoBtnCancelar");
    let idField = document.getElementById("inpActivoFijoId");
    let folioField = document.getElementById("inpActivoFijoFolio");
    let descripcionField = document.getElementById("inpActivoFijoDescripcion");
    let responsableField = document.getElementById("inpActivoFijoResponsable");
    let categoriaField = document.getElementById("inpActivoFijoCategoria");
    let tipoField = document.getElementById("inpActivoFijoTipo");
    let fechacompraField = document.getElementById("inpActivoFijoFechaCompra");
    let precioField = document.getElementById("inpActivoFijoPrecio");
    let linkfacturaField = document.getElementById("inpActivoFijoLinkFacturaCompra");
    let dlgTitle = document.getElementById("dlgOficinaTitle");
    let summaryContainer = document.getElementById("saveValidationSummary");
    summaryContainer.innerHTML = "";

    let oParams = {
        id: idField.value == "Nuevo" ? 0 : idField.value,
        folio: folioField.value,
        descripcion: descripcionField.value,
        responsable: responsableField.value,
        categoria: categoriaField.value,
        tipo: tipoField.value,
        fechacompra: fechacompraField.value,
        precio: precioField.value,
        linkfacturacompra: linkfacturaField.value
    };

    doAjax(
        "/ERP/ActivosFijos/SaveActivoFijo",
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

            let e = document.querySelector("[name='refresh']");
            e.click();

            showSuccess(dlgTitle.innerHTML, resp.mensaje);
        }, function (error) {
            showError("Error", error);
        },
        postOptions
    );
}