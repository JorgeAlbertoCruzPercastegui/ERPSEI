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
    dlg = document.getElementById('dlgContrato');

    if (dlg) {
        dlgModal = new bootstrap.Modal(dlg, null);

        dlg.addEventListener('hidden.bs.modal', function (event) {
            onCerrarClick();
        });
    } else {
        console.error("No se encontró el modal con id #dlgGeneradorContrato");
    }

    initTable();

    //Llenado automático de datos al seleccionar una empresa
    $('#prestadorSelect').on('change', function () {
        const selectedId = $(this).val();
        const rfc = empresaRFCMap.get(selectedId) || '';
        const domicilioFiscal = empresaDomicilioFiscalMap.get(selectedId) || '';
        const representanteLegal = representanteLegalMap.get(selectedId) || '';
        const fechaConstitucion = fechaConstitucionMap.get(selectedId) || '';
        const fechaFormateada = fechaConstitucion ? fechaConstitucion.split('T')[0] : '';
        const correoElectronico = correoElectronicoMap.get(selectedId) || '';

        $('#prestadorRFC').val(rfc);
        $('#prestadorDomicilio').val(domicilioFiscal);
        $('#prestadorRepresentante').val(representanteLegal);
        $('#prestadorFecha').val(fechaFormateada);
        $('#prestadorEmail').val(correoElectronico);
    });

    //Llenado automático de datos del cliente al seleccionar un clientes
    $('#prestatarioSelect').on('change', function () {
        const selectedId = $(this).val();
        const crfc = clienteRFCMap.get(selectedId) || '';
        const cdomicilioFiscal = clienteDomicilioFiscalMap.get(selectedId) || '';
        const crepresentanteLegal = clienterepresentanteLegalMap.get(selectedId) || '';
        const cfechaConstitucion = clientefechaConstitucionMap.get(selectedId) || '';
        const cfechaFormateada = cfechaConstitucion ? cfechaConstitucion.split('T')[0] : '';
        const ccorreoElectronico = clientecorreoElectronicoMap.get(selectedId) || '';

        $('#prestatarioRFC').val(crfc);
        $('#prestatarioDomicilio').val(cdomicilioFiscal);
        $('#prestatarioRepresentante').val(crepresentanteLegal);
        $('#prestatarioFecha').val(cfechaFormateada);
        $('#prestatarioEmail').val(ccorreoElectronico);
    });


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


// ✅ Refrescar tabla automáticamente cuando se hace clic en "Generar Contrato"
$(document).on("click", "a[href*='GenerarWord']", function () {
    // Espera 1.5 segundos para que el backend actualice estatus y el historial
    setTimeout(function () {
        $('#table').bootstrapTable('refresh');
    }, 1500);
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
    let razonSocialInput = document.getElementById("inputRazonSocial").value.trim().toLowerCase();
    let razonSocialPrestatarioInput = document.getElementById("inputRazonSocialPrestatario").value.trim().toLowerCase();

    let oParams = {
        tipoContratoId: tipoContrato.value === "0" || tipoContrato.value === "" ? null : parseInt(tipoContrato.value)
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

            let resultadosFiltrados = resp.datos.filter(row => {
                let matchPrestador = razonSocialInput === "" || row.razonSocial.toLowerCase().includes(razonSocialInput);
                let matchPrestatario = razonSocialPrestatarioInput === "" || row.razonSocialPrestatario.toLowerCase().includes(razonSocialPrestatarioInput);
                return matchPrestador && matchPrestatario;
            });

            table.bootstrapTable('load', responseHandler(resultadosFiltrados));
        },
        function (error) {
            showError("Error", error);
        },
        postOptions
    );
}

$(document).ready(function () {
    autoCompletar("#inputRazonSocial");
});

$(document).ready(function () {
    autoCompletar("#inputRazonSocialPrestatario");
});


// Mapas de datos para Empresas
let empresaRFCMap = new Map();
let empresaDomicilioFiscalMap = new Map();
let representanteLegalMap = new Map();
let fechaConstitucionMap = new Map();
let correoElectronicoMap = new Map();

// Mapas de datos para Clientes
let clienteRFCMap = new Map();
let clienteDomicilioFiscalMap = new Map();
let clienterepresentanteLegalMap = new Map();
let clientefechaConstitucionMap = new Map();
let clientecorreoElectronicoMap = new Map();

function onAgregarClick() {
    $('#dlgContrato input').val('');
    $('#tipoContratoSelectPrestador').empty().append('<option value="">Seleccione...</option>');
    $('#tipoContratoSelectPrestatario').empty().append('<option value="">Seleccione...</option>');
    $('#prestadorSelect').empty().append('<option value="">Seleccione...</option>');
    $('#prestatarioSelect').empty().append('<option value="">Seleccione...</option>');

    // Obtener IDs siguientes
    $.get("/Reportes/GeneradorContrato?handler=ObtenerSiguientesIds", function (data) {
        $('#prestadorId').val(data.empresaId);
        $('#prestatarioId').val(data.clienteId);
    });

    // Cargar tipos de contrato
    $.get("/Reportes/GeneradorContrato?handler=TiposContrato", function (data) {
        data.forEach(function (item) {
            $('#tipoContratoSelectPrestador').append(`<option value="${item.id}">${item.nombre}</option>`);
            $('#tipoContratoSelectPrestatario').append(`<option value="${item.id}">${item.nombre}</option>`);
        });
    });

    // Cargar datos de empresas
    $.get("/Reportes/GeneradorContrato?handler=Empresas", function (data) {
        empresaRFCMap.clear();
        empresaDomicilioFiscalMap.clear();
        representanteLegalMap.clear();
        fechaConstitucionMap.clear();
        correoElectronicoMap.clear();

        data.forEach(function (item) {
            $('#prestadorSelect').append(`<option value="${item.id}">${item.nombre}</option>`);
            empresaRFCMap.set(item.id.toString(), item.rfc);
            empresaDomicilioFiscalMap.set(item.id.toString(), item.domicilioFiscal);
            representanteLegalMap.set(item.id.toString(), item.representanteLegal);
            fechaConstitucionMap.set(item.id.toString(), item.fechaConstitucion);
            correoElectronicoMap.set(item.id.toString(), item.correoElectronico);
        });
    });

    // Cargar datos de clientes
    $.get("/Reportes/GeneradorContrato?handler=Clientes", function (data) {
        clienteRFCMap.clear();
        clienteDomicilioFiscalMap.clear();
        clienterepresentanteLegalMap.clear();
        clientefechaConstitucionMap.clear();
        clientecorreoElectronicoMap.clear();

        data.forEach(function (item) {
            $('#prestatarioSelect').append(`<option value="${item.id}">${item.nombre}</option>`);
            clienteRFCMap.set(item.id.toString(), item.crfc);
            clienteDomicilioFiscalMap.set(item.id.toString(), item.cdomicilioFiscal);
            clienterepresentanteLegalMap.set(item.id.toString(), item.crepresentanteLegal);
            clientefechaConstitucionMap.set(item.id.toString(), item.cfechaConstitucion);
            clientecorreoElectronicoMap.set(item.id.toString(), item.ccorreoElectronico);
        });
    });
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

function generarContrato(clienteId) {
    window.location.href = `/Reportes/GeneradorContrato?handler=GenerarWord&clienteId=${clienteId}`;
}

function onGuardarClick() {
    const data = {
        prestadorId: parseInt($('#prestadorId').val()),
        prestadorNombre: $('#prestadorSelect option:selected').text(),
        prestadorRFC: $('#prestadorRFC').val(),
        prestadorDomicilio: $('#prestadorDomicilio').val(),
        prestadorRepresentante: $('#prestadorRepresentante').val(),
        prestadorEmail: $('#prestadorEmail').val(),
        prestadorFecha: $('#prestadorFecha').val(),
        prestadorFechaInicio: $('#prestadorFechaInicio').val(),
        prestadorFechaFin: $('#prestadorFechaFin').val(),
        tipoContratoPrestadorId: parseInt($('#tipoContratoSelectPrestador').val()),
        prestadorNoNotario: parseInt($('#prestadorNumeroNotario').val()),
        prestadorNotario: $('#prestadorNotario').val(),
        prestadorPaginaWeb: $('#prestadorWeb').val(),

        prestatarioId: parseInt($('#prestatarioId').val()),
        prestatarioNombre: $('#prestatarioSelect option:selected').text(),
        prestatarioRFC: $('#prestatarioRFC').val(),
        prestatarioDomicilio: $('#prestatarioDomicilio').val(),
        prestatarioRepresentante: $('#prestatarioRepresentante').val(),
        prestatarioEmail: $('#prestatarioEmail').val(),
        prestatarioFecha: $('#prestatarioFecha').val(),
        prestatarioFechaInicio: $('#prestatarioFechaInicio').val(),
        prestatarioFechaFin: $('#prestatarioFechaFin').val(),
        tipoContratoPrestatarioId: parseInt($('#tipoContratoSelectPrestatario').val()),
        prestatarioNoNotario: parseInt($('#prestatarioNumeroNotario').val()), 
        prestatarioNotario: $('#prestatarioNotario').val(),
        prestatarioPaginaWeb: $('#prestatarioWeb').val()
    };

    $.ajax({
        url: '/Reportes/GeneradorContrato?handler=GuardarContrato',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        headers: {
            "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (resp) {
            if (resp.tieneError) {
                showError("Error al guardar", resp.mensaje);
            } else {
                showSuccess("Éxito", resp.mensaje);

                // ✅ Cierra el modal (asegúrate de que dlgModal esté definido correctamente)
                const dlgModal = bootstrap.Modal.getInstance(document.getElementById('dlgContrato'));
                dlgModal.hide();

                // ✅ Refresca la tabla
                $('#table').bootstrapTable('refresh');
            }
        },
        error: function (xhr, status, error) {
            showError("Error", "No se pudo guardar el contrato.");
        }
    });
}

function initGeneradorContratoDialog(modo, row) {
    if (!row) return;

    if (modo === VER) {
        // Prestador
        $('#prestadorIdVer').val(row.id);
        $('#tipoContratoPrestadorVer').val(row.tipoContrato || '');
        $('#prestadorNombreVer').val(row.razonSocial || '');
        $('#prestadorRFCVer').val(row.rfc || '');
        $('#prestadorDomicilioVer').val(row.domicilioFiscal || '');
        $('#prestadorNumeroNotarioVer').val(row.noNotario || '');
        $('#prestadorNotarioVer').val(row.notario || '');
        $('#prestadorRepresentanteVer').val(row.representanteLegal || '');
        $('#prestadorFechaVer').val(row.fechaConstitucionJS || '');
        $('#prestadorFechaInicioVer').val(row.fechaInicioJS || '');
        $('#prestadorFechaFinVer').val(row.fechaFinJS || '');
        $('#prestadorEmailVer').val(row.email || '');
        $('#prestadorWebVer').val(row.paginaWeb || '');

        // Obtener datos del cliente (prestatario)
        $.get(`/Reportes/GeneradorContrato?handler=ClientesPorEmpresa&id=${row.id}`, function (clientes) {
            if (clientes.length > 0) {
                const cliente = clientes[0];
                $('#prestatarioIdVer').val(cliente.id);
                $('#tipoContratoPrestatarioVer').val(cliente.tipoContrato?.toString() || '');
                $('#prestatarioNombreVer').val(cliente.nombre || '');
                $('#prestatarioRFCVer').val(cliente.rfc || '');
                $('#prestatarioDomicilioVer').val(cliente.domicilioFiscal || '');
                $('#prestatarioNumeroNotarioVer').val(cliente.noNotario || '');
                $('#prestatarioNotarioVer').val(cliente.notario || '');
                $('#prestatarioRepresentanteVer').val(cliente.representanteLegal || '');
                $('#prestatarioFechaVer').val(cliente.fechaConstitucion || '');
                $('#prestatarioFechaInicioVer').val(cliente.fechaInicio || '');
                $('#prestatarioFechaFinVer').val(cliente.fechaFin || '');
                $('#prestatarioEmailVer').val(cliente.email || '');
                $('#prestatarioWebVer').val(cliente.paginaWeb || '');
            }

            const modalVer = new bootstrap.Modal(document.getElementById('dlgContratoVer'));
            modalVer.show();
        });

    } else if (modo === EDITAR) {
        // Cargar tipos de contrato antes de llenar campos
        $.get("/Reportes/GeneradorContrato?handler=TiposContrato", function (tipos) {
            $('#tipoContratoPrestadorEditar').empty().append('<option value="">Seleccione...</option>');
            $('#tipoContratoPrestatarioEditar').empty().append('<option value="">Seleccione...</option>');

            tipos.forEach(function (item) {
                $('#tipoContratoPrestadorEditar').append(`<option value="${item.id}">${item.nombre}</option>`);
                $('#tipoContratoPrestatarioEditar').append(`<option value="${item.id}">${item.nombre}</option>`);
            });

            // Prestador
            $('#prestadorIdEditar').val(row.id);
            $('#tipoContratoPrestadorEditar').val(row.tipoContratoId?.toString() || '');
            $('#prestadorNombreEditar').val(row.razonSocial || '');
            $('#prestadorRFCEditar').val(row.rfc || '');
            $('#prestadorDomicilioEditar').val(row.domicilioFiscal || '');
            $('#prestadorNumeroNotarioEditar').val(row.noNotario || '');
            $('#prestadorNotarioEditar').val(row.notario || '');
            $('#prestadorRepresentanteEditar').val(row.representanteLegal || '');
            $('#prestadorFechaEditar').val(row.fechaConstitucionJS || '');
            $('#prestadorFechaInicioEditar').val(row.fechaInicioJS || '');
            $('#prestadorFechaFinEditar').val(row.fechaFinJS || '');
            $('#prestadorEmailEditar').val(row.email || '');
            $('#prestadorWebEditar').val(row.paginaWeb || '');

            // Cliente
            $.get(`/Reportes/GeneradorContrato?handler=ClientesPorEmpresa&id=${row.id}`, function (clientes) {
                if (clientes.length > 0) {
                    const cliente = clientes[0];
                    $('#prestatarioIdEditar').val(cliente.id);
                    $('#tipoContratoPrestatarioEditar').val(cliente.tipoContratoId?.toString() || '');
                    $('#prestatarioNombreEditar').val(cliente.nombre || '');
                    $('#prestatarioRFCEditar').val(cliente.rfc || '');
                    $('#prestatarioDomicilioEditar').val(cliente.domicilioFiscal || '');
                    $('#prestatarioNumeroNotarioEditar').val(cliente.noNotario || '');
                    $('#prestatarioNotarioEditar').val(cliente.notario || '');
                    $('#prestatarioRepresentanteEditar').val(cliente.representanteLegal || '');
                    $('#prestatarioFechaEditar').val(cliente.fechaConstitucion || '');
                    $('#prestatarioFechaInicioEditar').val(cliente.fechaInicio || '');
                    $('#prestatarioFechaFinEditar').val(cliente.fechaFin || '');
                    $('#prestatarioEmailEditar').val(cliente.email || '');
                    $('#prestatarioWebEditar').val(cliente.paginaWeb || '');
                }

                const modalEditar = new bootstrap.Modal(document.getElementById('dlgContratoEditar'));
                modalEditar.show();
            });
        });
    }
}



function onCerrarClick() {
    $('#dlgContrato input, #dlgContrato select').prop('disabled', false);
    $('#btnGuardarContrato').show();
}

function onActualizarClick() {
    const tipoContratoPrestador = $('#tipoContratoPrestadorEditar').val();
    const tipoContratoPrestatario = $('#tipoContratoPrestatarioEditar').val();

    // Validación básica
    if (!tipoContratoPrestador || tipoContratoPrestador === "0") {
        showError("Validación", "Debes seleccionar un Tipo de Contrato para el Prestador.");
        return;
    }

    if (!tipoContratoPrestatario || tipoContratoPrestatario === "0") {
        showError("Validación", "Debes seleccionar un Tipo de Contrato para el Prestatario.");
        return;
    }

    const data = {
        empresa: {
            id: parseInt($('#prestadorIdEditar').val()),
            razonSocial: $('#prestadorNombreEditar').val(),
            rfc: $('#prestadorRFCEditar').val(),
            domicilioFiscal: $('#prestadorDomicilioEditar').val(),
            representanteLegal: $('#prestadorRepresentanteEditar').val(),
            email: $('#prestadorEmailEditar').val(),
            fechaConstitucion: $('#prestadorFechaEditar').val(),
            fechaInicio: $('#prestadorFechaInicioEditar').val(),
            fechaFin: $('#prestadorFechaFinEditar').val(),
            tipoContratoId: parseInt(tipoContratoPrestador),
            noNotario: parseInt($('#prestadorNumeroNotarioEditar').val()),
            notario: $('#prestadorNotarioEditar').val(),
            paginaWeb: $('#prestadorWebEditar').val()
        },
        cliente: {
            id: parseInt($('#prestatarioIdEditar').val()),
            razonSocial: $('#prestatarioNombreEditar').val(),
            rfc: $('#prestatarioRFCEditar').val(),
            domicilioFiscal: $('#prestatarioDomicilioEditar').val(),
            representanteLegal: $('#prestatarioRepresentanteEditar').val(),
            email: $('#prestatarioEmailEditar').val(),
            fechaConstitucion: $('#prestatarioFechaEditar').val(),
            fechaInicio: $('#prestatarioFechaInicioEditar').val(),
            fechaFin: $('#prestatarioFechaFinEditar').val(),
            tipoContratoId: parseInt(tipoContratoPrestatario),
            noNotario: parseInt($('#prestatarioNumeroNotarioEditar').val()),
            notario: $('#prestatarioNotarioEditar').val(),
            paginaWeb: $('#prestatarioWebEditar').val()
        }
    };

    $.ajax({
        url: '/Reportes/GeneradorContrato?handler=ActualizarContrato',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        headers: {
            "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (resp) {
            if (resp.tieneError) {
                showError("Error al actualizar", resp.mensaje);
            } else {
                showSuccess("Actualizado", resp.mensaje);

                const modalEditar = bootstrap.Modal.getInstance(document.getElementById('dlgContratoEditar'));
                modalEditar.hide();

                $('#table').bootstrapTable('refresh');
            }
        },
        error: function () {
            showError("Error", "No se pudo actualizar el contrato.");
        }
    });
}