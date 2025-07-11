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
    dlg = document.getElementById('dlgGeneradorContrato');

    if (dlg) {
        dlgModal = new bootstrap.Modal(dlg, null);

        dlg.addEventListener('hidden.bs.modal', function (event) {
            onCerrarClick();
        });
    } else {
        console.error("No se encontró el modal con id #dlgGeneradorContrato");
    }

    initTable();

    table.on('expand-row.bs.table', function (e, index, row, $detail) {
        let containerId = `#detalle-clientes-${row.id}`;
        $.get(`/Reportes/GeneradorContrato?handler=ClientesPorEmpresa&id=${row.id}`, function (clientes) {
            if (clientes.length === 0) {
                $(containerId).html('<tr><td colspan="6"><em>No hay clientes relacionados.</em></td></tr>');
                return;
            }

            let html = '';
            clientes.forEach(cliente => {
                html += `<tr>
                        <td>${cliente.nombre}</td>
                        <td>${cliente.rfc}</td>
                        <td>${cliente.domicilioFiscal}</td>
                        <td>${cliente.representanteLegal}</td>
                        <td>${cliente.noNotario}</td>
                        <td>${cliente.notario}</td>
                        <td>
                            <a href="/Reportes/GeneradorContrato?handler=GenerarWord&clienteId=${cliente.id}&empresaId=${row.id}" 
                               class="btn btn-sm btn-primary" target="_blank">
                               Generar Contrato
                            </a>
                        </td>
                    </tr>`;
            });
            $(containerId).html(html);
        });
    });
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
        initGeneradorContratoDialog(VER, row);
    },
    'click .edit': function (e, value, row, index) {
        initGeneradorContratoDialog(EDITAR, row);
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
    initGeneradorContratoDialog(NUEVO, { id: "Nuevo", nombre: "" });
}

function initTable() {
    table.bootstrapTable('destroy').bootstrapTable({
        detailView: true,
        detailFormatter: detailFormatter,
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
                title: "RazonSocial",
                field: "razonSocial",
                align: "center",
                valign: "middle",
                sortable: true,
                width: "80px"
            },
            {
                title: "DomicilioFiscal",
                field: "domicilioFiscal",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "NoNotario",
                field: "noNotario",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "Notario",
                field: "notario",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "RepresentanteLegal",
                field: "representanteLegal",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "RFC",
                field: "rfc",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "TipoContrato",
                field: "tipoContrato",
                align: "center",
                valign: "middle",
                sortable: true
            }/*,
            {
                title: colAccionesHeader,
                field: "operate",
                align: 'center',
                width: "100px",
                clickToSelect: false,
                events: window.operateEvents,
                formatter: operateFormatter
            }*/
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
                "/Reportes/GeneradorContrato/DeleteEmpresaContratos",
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

function onBuscarClick() {
    let btnBuscar = document.getElementById("btnBuscar");
    let tipoContrato = document.getElementById("selFiltroTipoContrato");
    let prestador = document.getElementById("selFiltroPrestador");
    let prestatario = document.getElementById("selFiltroPrestatario");

    let oParams = {
        tipoContratoId: tipoContrato.value === "0" || tipoContrato.value === "" ? null : parseInt(tipoContrato.value),
        prestadorId: prestador.value === "0" || prestador.value === "" ? null : parseInt(prestador.value),
        prestatarioId: prestatario.value === "0" || prestatario.value === "" ? null : parseInt(prestatario.value)
    };

    doAjax(
        "/Reportes/GeneradorContrato/FiltrarEmpresasContratos",
        oParams,
        function (resp) {
            if (resp.tieneError) {
                if (Array.isArray(resp.errores) && resp.errores.length > 0) {
                    let summary = resp.errores.map(error => `<li>${error}</li>`).join("");
                    saveValidationSummary.innerHTML = `<ul>${summary}</ul>`;
                }
                showError(btnBuscar.innerHTML, resp.mensaje);
                return;
            }

            table.bootstrapTable('load', responseHandler(resp.datos));
        },
        function (error) {
            showError("Error", error);
        },
        postOptions
    );
}


function detailFormatter(index, row) {
    return `<div id="clientes-${row.id}">
        <div class="table-responsive mt-2">
            <table class="table table-bordered table-sm mb-0">
                <thead class="table-light text-center"> <!-- 👈 centrado aquí -->
                    <tr>
                        <th>Razón Social</th>
                        <th>RFC</th>
                        <th>Domicilio Fiscal</th>
                        <th>Representante Legal</th>
                        <th>No. Notario</th>
                        <th>Notario</th>
                        <th>Acciones</th>
                    </tr>
                </thead>
                <tbody id="detalle-clientes-${row.id}">
                    <tr><td colspan="7" class="text-center">Cargando clientes...</td></tr> <!-- 👈 centrado también aquí -->
                </tbody>
            </table>
        </div>
    </div>`;
}


/*function generarContrato(clienteId) {
    alert("Generar contrato para Cliente ID: " + clienteId);
    // Aquí puedes luego hacer una llamada AJAX para generar el contrato real.
}*/

function generarContrato(clienteId) {
    window.location.href = `/Reportes/GeneradorContrato?handler=GenerarWord&clienteId=${clienteId}`;
}