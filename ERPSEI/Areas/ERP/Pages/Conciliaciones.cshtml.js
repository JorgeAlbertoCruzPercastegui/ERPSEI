var table;
var buttonRemove;
var tableActividad;
var selections = [];
var dlgConciliacion = null;
var dlgConciliacionModal = null;
var numFormatter = null;

const NUEVO = 0;
const EDITAR = 1;
const VER = 2;
const maxFileSizeInBytes = 5242880; //5mb = (5 * 1024) * 1024;
const oneMegabyteSizeInBytes = 1048576; // 1mb = (1 * 1024) * 1024
const postOptions = {
    headers: {
        "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
    },
    type: 'POST'
};
const getOptions = {
    headers: postOptions.headers,
    type: 'GET'
};
const putOptions = {
    headers: postOptions.headers,
    type: 'PUT'
};

document.addEventListener("DOMContentLoaded", function (event) {
    numFormatter = new Intl.NumberFormat(cultureName);
    table = $("#table");
    buttonRemove = $("#remove");
    dlgConciliacion = document.getElementById('dlgConciliacion');
    dlgConciliacionModal = new bootstrap.Modal(dlgConciliacion, {});
    dlgConciliacion.addEventListener('hidden.bs.modal', function (event) {
        onCerrarClick();
    });

    dlgConciliacion.addEventListener('shown.bs.modal', function (event) {
        autoCompletar("#inpConciliacionClienteId", {
            select: function (Element, item) {
                buscarComprobantesPorRFC(item.rfc);
                actualizarContadores();
            }
        });

        //initTableComprobantes();
    });

    initTable();
    initTableComprobantes();

    let btnBuscar = document.getElementById("btnBuscar");
    if (btnBuscar) { btnBuscar.click(); }

    jQuery.validator.setDefaults({
        highlight: function (element, errorClass, validClass) {
            $(element).addClass("is-invalid").removeClass("is-valid");
        },
        unhighlight: function (element, errorClass, validClass) {
            if ($(element).hasClass("is-invalid")) {
                $(element).addClass("is-valid").removeClass("is-invalid");
            }
        }
    });

    // Variable para verificar si es la primera vez que el modal se abre
    let isFirstTimeOpening = true;

    document.getElementById('consultarComprobantesModal').addEventListener('shown.bs.modal', function () {
        // Solo establecer las fechas automáticamente la primera vez que se abre el modal
        if (isFirstTimeOpening) {
            // Obtener la fecha actual
            const today = new Date();
            // Configurar la fecha de inicio como el primer día del mes
            const firstDayOfMonth = new Date(today.getFullYear(), today.getMonth(), 1);

            // Formatear fechas a formato compatible con 'datetime-local' (YYYY-MM-DDTHH:MM)
            const formatDate = (date) => {
                const offset = date.getTimezoneOffset();
                const localDate = new Date(date.getTime() - offset * 60 * 1000);
                return localDate.toISOString().slice(0, 16); // Solo incluye fecha y hora
            };

            // Asignar la fecha de inicio y la fecha de fin a los campos de entrada
            document.getElementById("inpFiltroFechaInicioModalDComprobantes").value = formatDate(firstDayOfMonth);
            document.getElementById("inpFiltroFechaFinModalDComprobantes").value = formatDate(today);

            // Cambiar el indicador para que no se vuelva a ejecutar esta lógica
            isFirstTimeOpening = false;
        }
    });
});

// Nueva función para buscar comprobantes por RFC y cargar en tableCardView
function buscarComprobantesPorRFC(rfc) {
    if (!rfc) {
        showError("Error", "Por favor, selecciona una empresa válida.");
        return;
    }

    // Obtener los valores de fecha de inicio y fecha de fin
    let fechaInicio = document.getElementById("inpFiltroFechaInicioModalDComprobantes").value;
    let fechaFin = document.getElementById("inpFiltroFechaFinModalDComprobantes").value;

    // Agregar las fechas al objeto de parámetros
    let oParams = {
        rfc: rfc,
        fechaInicio: fechaInicio,
        fechaFin: fechaFin
    };

    doAjax(
        "/ERP/Conciliaciones/ComprobantesListEmpresas",
        oParams,
        function (resp) {
            if (resp.tieneError) {
                let summary = resp.errores.map(error => `<li>${error}</li>`).join("");
                saveValidationSummary.innerHTML += `<ul>${summary}</ul>`;
                showError("Buscar Comprobantes", resp.mensaje);
                return;
            }

            $('#tableCardComprobantes').bootstrapTable('load', responseHandler(resp.datos));
        },
        function (error) {
            showError("Error", error);
        },
        postOptions
    );
}

async function onImportarMovimientosBancariosClick(event) {
    const file = event.target.files[0];
    if (file) {
        try {
            // Verificar que el archivo sea un PDF o Excel
            const validTypes = ['application/pdf', 'application/vnd.ms-excel', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'];
            if (!validTypes.includes(file.type)) {
                alert('El archivo seleccionado no es un PDF ni un archivo de Excel.');
                return;
            }

            // Lógica adicional para manejar el archivo (lectura, procesamiento, etc.)
            console.log('Archivo válido seleccionado:', file.name);

        } catch (error) {
            console.error('Error al leer el archivo:', error);
        }
    }
}

//Funcionalidad Tabla
function getIdSelections() {
    return $.map(table.bootstrapTable('getSelections'), function (row) {
        return row.id
    })
}

function responseHandler(res) {
    if (typeof res == "string" && res.length >= 1) {
        res = JSON.parse(res);
    }
    $.each(res, function (i, row) {
        row.state = $.inArray(row.id, selections) !== -1;
    });

    return res
}

//Función para dar formato a los campos booleanos
function booleanFormatter(value, row, index) {
    if ((row.puedeFacturar || "False") == "True") {
        return `<i class="bi bi-check-circle-fill text-success"></i>`;
    }
    else {
        return `<i class="bi bi-x-circle-fill text-danger"></i>`;
    }
}

//Función para dar formato a los iconos de operación de los registros
function operateFormatter(value, row, index) {
    let icons = [];

    // Icono Ver
    icons.push(`<li><a class="dropdown-item see" href="#" title="${btnVerTitle}"><i class="bi bi-search"></i> ${btnVerTitle}</a></li>`);

    // Icono Editar (habilitado solo si el Estatus es diferente de "Finalizada")
    if (row.Finalizada == "En progreso") {
        icons.push(`<li><a class="dropdown-item edit" href="#" title="${btnEditarTitle}"><i class="bi bi-pencil-fill"></i> ${btnEditarTitle}</a></li>`);
    }

    // Icono Exportar a Excel
    icons.push(`<li><a class="dropdown-item export" href="#" title="Exportar a Excel"><i class="bi bi-file-earmark-excel"></i> Exportar a Excel</a></li>`);

    return `<div class="dropdown">
              <button class="btn" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                <i class="bi bi-three-dots-vertical success"></i>
              </button>
              <ul class="dropdown-menu">${icons.join("")}</ul>
            </div>`;
}


window.operateEvents = {
    'click .see': function (e, value, row, index) {
        initConciliacionDialog(VER, row);
    },
    'click .edit': function (e, value, row, index) {
        initConciliacionDialog(EDITAR, row);

        $('#tableCardComprobantes').bootstrapTable('load', []);
        $('#tableCardMovimientos').bootstrapTable('load', []);
        $('#tableResult').bootstrapTable('load', []);
    },
    'click .export': function (e, value, row, index) {

        console.log("Contenido del objeto row:", row);
        
        const conciliacionId = parseInt(row.id, 10) || 0;
        console.log("ID de conciliación seleccionado (convertido a entero):", conciliacionId);

        if (conciliacionId === 0) {
            showError("Exportación Fallida", "ID de conciliación no válido.");
            return;
        }

        exportarAExcel(conciliacionId);
    }
}

async function exportarAExcel(conciliacionId) {
    let oParams = { id: conciliacionId };
    $.extend(postOptions, { type: 'GET' });

    doAjax(
        "/ERP/Conciliaciones/ExportarExcel",
        oParams,
        async function (resp) {
            if (resp.tieneError) {
                showError("Error, favor de revisar", resp.mensaje);
                return;
            }

            const ExcelJS = window.ExcelJS;
            const workbook = new ExcelJS.Workbook();
            const worksheet = workbook.addWorksheet('Conciliación');

            // Ajustes en el diseño inicial
            worksheet.getCell('A3').value = 'lg';
            worksheet.getCell('A3').font = { bold: true };
            worksheet.getCell('B3').value = 1;
            worksheet.getCell('B3').font = { bold: true };

            // Extraer solo el día de la fecha y colocarlo en la celda D3 en negritas
            const fechaEmisor = resp.datos.length > 0 ? resp.datos[0].fecha : 'N/A';
            const dia = fechaEmisor !== 'N/A' ? new Date(fechaEmisor).getDate() : 'N/A';
            worksheet.getCell('D3').value = dia;
            worksheet.getCell('D3').font = { bold: true };

            // Obtener el nombre del receptor, serie y folio
            const nombreReceptor = resp.datos.length > 0 ? resp.datos[0].nombreReceptor : 'N/A';
            const serie = resp.datos.length > 0 ? resp.datos[0].serie : 'N/A';
            const folio = resp.datos.length > 0 ? resp.datos[0].folio : 'N/A';

            // Concatenar el nombre del receptor con la serie y el folio
            const nombreCompleto = `${nombreReceptor} ${serie}-F-${folio}`;

            // Asignar el nombre concatenado a las celdas D4, D5, D6 y D7
            ['D4', 'D5', 'D6', 'D7'].forEach(cell => {
                worksheet.getCell(cell).value = nombreCompleto;
            });

            // Concatenar "INGRESOS" con el nombre del receptor, la serie y el folio
            const ingresosTexto = `INGRESOS ${nombreReceptor} ${serie}-F-${folio}`;
            worksheet.getCell('C3').value = ingresosTexto;
            worksheet.getCell('C3').font = { bold: true };

            // Aplicar color azul claro a las celdas A3 y B3
            const lightBlue = 'CCECFF';
            worksheet.getCell('A3').fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'FF45C9ED' }
            };
            worksheet.getCell('B3').fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'FF45C9ED' }
            };

            // Colocar texto en las celdas B5 y B6
            worksheet.getCell('B5').value = '2180-001-000';
            worksheet.getCell('B6').value = '2181-001-000';
            worksheet.getCell('B8').value = 'FIN_PARTIDAS';

            // Colocar el número 0 en las celdas C4, C5, C6 y C7, centrado
            ['C4', 'C5', 'C6', 'C7'].forEach(cell => {
                worksheet.getCell(cell).value = 0;
                worksheet.getCell(cell).alignment = { horizontal: 'center' };
            });

            // Texto "CARGO" y "ABONO" en negritas
            worksheet.getCell('F3').value = 'CARGO';
            worksheet.getCell('F3').font = { bold: true };
            worksheet.getCell('G3').value = 'ABONO';
            worksheet.getCell('G3').font = { bold: true };

            // Calcular el total de los cargos y colocarlo en la celda F4
            const datos = Array.isArray(resp.datos) ? resp.datos : [];
            const totalCargos = datos.reduce((sum, dato) => sum + (dato.cargos || 0), 0);
            worksheet.getCell('F4').value = totalCargos;
            
            // Obtener el TotalImpuestosTrasladados desde los datos del modelo
            const totalImpuestosTrasladados = resp.datos.length > 0 ? resp.datos[0].totalImpuestosTrasladados : 0;

            // Asignar el valor de TotalImpuestosTrasladados en las celdas G5 y F6
            worksheet.getCell('G5').value = totalImpuestosTrasladados;
            worksheet.getCell('F6').value = totalImpuestosTrasladados;
            
            // Colocar el cargo del movimiento en la celda G7
            worksheet.getCell('G7').value = datos.length > 0 ? datos[0].cargos : 0;

            // Validar si hay datos para exportar
            if (datos.length === 0) {
                showError("Exportación Fallida", "No se encontraron datos para exportar.");
                return;
            }

            // Definir las columnas del Excel
            worksheet.columns = [
                { header: '', key: 'cliente', width: 5 },
                { header: '', key: 'comprobanteId', width: 15 },
                { header: '', key: 'serie', width: 50 },
                { header: '', key: 'folio', width: 38 },
                { header: '', key: 'total', width: 3 },
                { header: '', key: 'movimientoId', width: 15 },
                { header: '', key: 'descripcionMovimiento', width: 15 },
                { header: '', key: 'cargos', width: 15 }
            ];

            // Agregar los datos al Excel
            /*datos.forEach((dato) => {
                worksheet.addRow({
                    cliente: dato.cliente,
                    comprobanteId: dato.comprobanteId,
                    serie: dato.serie,
                    folio: dato.folio,
                    total: dato.total,
                    movimientoId: dato.movimientoId,
                    descripcionMovimiento: dato.descripcionMovimiento,
                    cargos: dato.cargos
                });
            });*/

            // Crear el archivo Excel y descargarlo
            const buffer = await workbook.xlsx.writeBuffer();
            const blob = new Blob([buffer], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            const link = document.createElement('a');
            link.href = URL.createObjectURL(blob);
            link.download = `Conciliacion_${conciliacionId}.xlsx`;
            link.click();
            URL.revokeObjectURL(link.href);
        },
        function (error) {
            showError("Error", "No se pudo exportar a Excel.");
        },
        postOptions
    );
}


function onAgregarClick() {
    initConciliacionDialog(NUEVO, { id: "Nuevo", nombre: "" });

    $('#tableCardComprobantes').bootstrapTable('load', []);
    $('#tableCardMovimientos').bootstrapTable('load', []);
    $('#tableResult').bootstrapTable('load', []);
}


function initTable() {
    table.bootstrapTable('destroy').bootstrapTable({
        height: 550,
        locale: cultureName,
        exportDataType: 'all',
        exportTypes: ['excel'],
        toolbar: '#toolbar',
        columns: [
            {
                field: "state",
                checkbox: true,
                align: "center",
                valign: "middle"
            },
            {
                title: colIdHeader,
                field: "id",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colFechaHeader,
                field: "Fecha",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colDescripcionHeader,
                field: "Descripcion",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colClienteHeader,
                field: "Cliente",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colTotalHeader,
                field: "Total",
                align: "center",
                valign: "middle",
                sortable: true,
                formatter: currencyFormatter
            },
            {
                title: colUsuarioCreadorHeader,
                field: "UsuarioCreador",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colUsuarioModificoHeader,
                field: "UsuarioModificador",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colFinalizadaHeader,
                field: "Finalizada",
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
                "/ERP/Conciliaciones/DeleteConciliaciones",
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
                    //showError(dlgDeleteTitle, error);
                },
                postOptions
            );
        });
    })
}

function onCerrarConciliacionClick() {

    // Muestra la confirmación antes de proceder
    askConfirmation(
        dlgFinishConTitle,
        dlgMessageFinishConTitle,
        function () {
            // Ejecuta la validación
            $("#theFormT").validate();
            let valid = $("#theFormT").valid();
            if (!valid) { return; }

            // Obtén el valor del campo
            let conciliacionId = document.getElementById("inpConciliacionId").value;
            let dlgTitle = document.getElementById("dlgConciliacionTitle");
            let summaryContainer = document.getElementById("saveValidationSummary");
            summaryContainer.innerHTML = "";

            // Configuración de los parámetros
            let oParams = {
                id: conciliacionId,
                Finalizada: 1
            };

            doAjax(
                "/ERP/Conciliaciones/FinalizarConciliaciones",
                oParams,
                function (resp) {
                    if (resp.tieneError) {
                        if (Array.isArray(resp.errores) && resp.errores.length >= 1) {
                            let summary = ``;
                            resp.errores.forEach(function (error) {
                                summary += `<li>${error}</li>`;
                            });
                            summaryContainer.innerHTML += `<ul>${summary}</ul>`;
                            console.log("Respuesta:", resp);
                        }
                        showError(dlgTitle.innerHTML, resp.mensaje);
                        return;
                    }

                    // Cierra el modal de conciliación
                    let conciliacionModal = document.getElementById("dlgConciliacion");
                    if (conciliacionModal) {
                        let bootstrapModal = bootstrap.Modal.getInstance(conciliacionModal);
                        if (bootstrapModal) {
                            bootstrapModal.hide();
                        }
                    }
                    
                    initTable();

                    showSuccess(dlgTitle.innerHTML, resp.mensaje);
                }, function (error) {
                    showError("Error", error);
                },
                putOptions
            );
        },
        function () {
            // Acción cancelada, puedes manejarlo si es necesario
            console.log("Acción cancelada por el usuario.");
        }
    );
}


/*function onCerrarConciliacionClick() {
    $.extend(postOptions, { type: 'POST', contentType: 'application/json' });

    const conciliacionId = document.getElementById("inpConciliacionId").value;

    if (!conciliacionId) {
        showError(dlgFinishConTitle, "No se pudo obtener el ID de la conciliación.");
        return;
    }

    // Crear un objeto con la clave `id`
    let oParams = { id: parseInt(conciliacionId) };

    console.log("Enviando oParams:", oParams);

    doAjax(
        "/ERP/Conciliaciones/finalizarConciliacion",
        oParams,
        function (resp) {
            if (resp.tieneError) {
                showError(dlgFinishConTitle, resp.mensaje);
                return;
            }

            showSuccess(dlgFinishConTitle, resp.mensaje);
            document.querySelector("[name='refresh']").click();
        },
        function (error) {
            showError(dlgFinishConTitle, error.responseText || error.statusText);
        }
    );
}*/

let cachedData = $('#tableCardComprobantes').bootstrapTable('getData');
function initTableComprobantes() {
    $("#tableCardComprobantes").bootstrapTable({
        locale: cultureName,
        toolbar: '#toolbar2',
        method: 'get',
        columns: [
            {
                title: "Id",
                field: "Id",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "Serie",
                field: "Serie",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "Folio",
                field: "Folio",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "Fecha",
                field: "Fecha",
                align: "center",
                valign: "middle",
                sortable: true,
                formatter: formatDate
            },
            {
                title: "UUID",
                field: "UUID",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "Total",
                field: "Total",
                align: "center",
                valign: "middle",
                sortable: true,
                formatter: currencyFormatter
            },
            {
                title: "",
                field: "",
                formatter: conciliacionIndidual,
                align: "center",
                valign: "middle"
            }
        ],
        responseHandler: responseHandler
    });
}

/*******Aquí empieza el codigo de comrpobante seleccionado con movimientos */
function detailFormatterC(index, row) {
    let movimientosConciliados = row.movimientosConciliados || [];

    // Verificar si hay movimientos conciliados
    if (movimientosConciliados.length === 0) {
        return `<p>No hay movimientos asociados a este comprobante.</p>`;
    }

    let totalComprobante = parseFloat(row.Total).toFixed(2);
    let tableHtml = `<div class="table-responsive">
                        <table class="table table-striped table-bordered">
                            <thead>
                                <tr>
                                    <th>Id</th>
                                    <th>Fecha</th>
                                    <th>Banco</th>
                                    <th>Descripción</th>
                                    <th>Cargos</th>
                                    <th>% Similitud</th> <!-- Agregar columna de % Similitud -->
                                </tr>
                            </thead>
                            <tbody>`;

    movimientosConciliados.forEach(mov => {
        let totalMovimiento = parseFloat(mov.Cargos).toFixed(2); // Tomar el total de los cargos del movimiento
        let porcentajeSimilitud = ((totalMovimiento * 100) / totalComprobante) || 0; // Calcular el % de similitud

        tableHtml += `<tr>
                        <td>${mov.Id}</td>
                        <td>${mov.Fecha}</td>
                        <td>${mov.Banco}</td>
                        <td>${mov.Descripcion}</td>
                        <td>${currencyFormatter(mov.Cargos)}</td>
                        <td>${porcentajeSimilitud.toFixed(2)}%</td> <!-- Mostrar % de Similitud -->
                      </tr>`;
    });

    tableHtml += `    </tbody>
                    </table>
                </div>`;

    return tableHtml;
}

// Lista para almacenar los IDs de registros sin coincidencia ya contados
let registrosSinCoincidencia = [];
let registrosSinCoincidenciaM = [];

// Función para realizar la conciliación individual
function conciliacionIndidual(value, row, index) {
    // Si el comprobante ya ha sido conciliado o está bloqueado, deshabilitar el botón
    let disabled = row.coincidencia || row.bloqueado ? 'disabled' : '';

    return `
        <button class="btn btn-primary btn-sm" onclick="consultarComp(${row.Id}, '${row.Serie}', '${row.Folio}', '${row.Fecha}', '${row.UUID}', '${row.Total}')" ${disabled}>
            <i class="bi bi-paperclip rotate-clip"></i> Conciliar
        </button>
    `;
}


let movimientosSeleccionados = []; // Lista para almacenar los movimientos seleccionados

// Función para consultar y conciliar un comprobante
function consultarComp(id, serie, folio, fechaComprobante, uuid, totalComprobante) {
    let fechaComprobanteFormateada = fechaComprobante.split('T')[0];
    let totalComprobanteFormateado = parseFloat(totalComprobante).toFixed(2);

    // Obtener los movimientos desde la tabla de movimientos
    let movimientosData = $('#tableCardMovimientos').bootstrapTable('getData');

    // Agregar ID incremental a los movimientos si no tienen uno
    movimientosData.forEach((mov, index) => {
        if (!mov.Id) {
            mov.Id = index + 1;
        }
    });

    // Variables para manejar coincidencias basadas en fechas
    let coincidenciasMovimientos = [];
    let resultadoComprobante = `<strong>Comprobante Seleccionado:</strong><br/><br/>
        <p>Id: ${id}</p>
        <p>Serie: ${serie}</p>
        <p>Folio: ${folio}</p>
        <p>Fecha: ${fechaComprobanteFormateada}</p>
        <p>UUID: ${uuid}</p>
        <p>Total: ${currencyFormatter(totalComprobanteFormateado)}</p>
        <p><strong>Cargos Seleccionados: $<span id="totalCargosSeleccionados">0.00</span> (<span id="porcentajeSimilitudSeleccionado">0%</span>)</strong></p>
        <hr /><strong>Movimientos Coincidentes</strong><br/>`;

    let fechaComprobanteDate = new Date(fechaComprobanteFormateada);
    let mesComprobante = fechaComprobanteDate.getMonth();
    let anioComprobante = fechaComprobanteDate.getFullYear();

    movimientosData.forEach((mov) => {
        let fechaMovimientoDate = new Date(mov.Fecha.split('/').reverse().join('-'));
        let mesMovimiento = fechaMovimientoDate.getMonth();
        let anioMovimiento = fechaMovimientoDate.getFullYear();
        let cargoMovimientoFormateado = parseFloat(mov.Cargos).toFixed(2);
        let porcentajeSimilitud = (cargoMovimientoFormateado / totalComprobanteFormateado) * 100 || 0.00;

        if (mesMovimiento === mesComprobante && anioMovimiento === anioComprobante && parseFloat(cargoMovimientoFormateado) <= parseFloat(totalComprobanteFormateado)) {
            coincidenciasMovimientos.push({
                id: mov.Id,
                Fecha: mov.Fecha,
                Banco: mov.Banco,
                Descripción: mov.Descripcion,
                Cargos: mov.Cargos,
                porcentajeSimilitud: porcentajeSimilitud.toFixed(2),
                idComprobante: id
            });
        }
    });

    coincidenciasMovimientos.sort((a, b) => parseFloat(b.porcentajeSimilitud) - parseFloat(a.porcentajeSimilitud));

    let modalBody = document.getElementById('modalConciliacionCompMensaje');
    if (coincidenciasMovimientos.length > 0) {
        let tableHtml = `<div style="max-height: 300px; overflow-y: auto;">
            <table class="table table-striped">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Fecha</th>
                        <th>Banco</th>
                        <th>Descripción</th>
                        <th>Cargos</th>
                        <th>Similitud</th>
                        <th>Seleccionar</th>
                    </tr>
                </thead>
                <tbody>`;

        coincidenciasMovimientos.forEach(mov => {
            tableHtml += `<tr>
                    <td>${mov.id}</td>
                    <td>${mov.Fecha}</td>
                    <td>${mov.Banco}</td>
                    <td>${mov.Descripción}</td>
                    <td>${currencyFormatter(parseFloat(mov.Cargos).toFixed(3))}</td>
                    <td>${mov.porcentajeSimilitud}%</td>
                    <td>
                        <input type="checkbox" class="form-check-input" value="${mov.Cargos}" data-id="${mov.id}" 
                            onchange="actualizarContadorSeleccionados(this, ${totalComprobanteFormateado});">
                    </td>
                </tr>`;
        });

        tableHtml += `</tbody></table></div>`;

        tableHtml += `<br/><div class="text-end">
            <button class="btn btn-primary" onclick="conciliarSeleccionadosComprobante(${id}, '${serie}', '${folio}', '${fechaComprobanteFormateada}', '${uuid}', ${totalComprobanteFormateado});">
                Conciliar Seleccionados
            </button>
        </div>`;

        modalBody.innerHTML = resultadoComprobante + tableHtml;
    } else {
        modalBody.innerHTML = resultadoComprobante + "<p>No se encontraron movimientos coincidentes con este comprobante.</p>";
    }

    var myModal = new bootstrap.Modal(document.getElementById('modalConciliacionComp'));
    myModal.show();
}

function conciliarSeleccionadosComprobante(idComprobante, serie, folio, fechaComprobante, uuid, totalComprobante) {
    let selectedMovimientos = [];
    let movimientosData = $('#tableCardMovimientos').bootstrapTable('getData');
    let comprobantesData = $('#tableCardComprobantes').bootstrapTable('getData');

    let comprobanteSeleccionado = comprobantesData.find(comp => comp.Id == idComprobante);

    if (!comprobanteSeleccionado) {
        showModal("No se encontró el comprobante con el ID: " + idComprobante);
        return;
    }

    let checkboxesSeleccionados = document.querySelectorAll('#modalConciliacionCompMensaje input[type="checkbox"]:checked');

    if (checkboxesSeleccionados.length === 0) {
        showModal("No se ha seleccionado ningún movimiento.");
        return;
    }

    let totalCargosSeleccionados = 0;
    checkboxesSeleccionados.forEach(checkbox => {
        let cargoMovimiento = parseFloat(checkbox.value);
        totalCargosSeleccionados += cargoMovimiento;
    });

    let diferencia = (totalCargosSeleccionados - parseFloat(totalComprobante)).toFixed(2);
    let diferenciaAbsoluta = Math.abs(diferencia);

    // Permitir una diferencia máxima de hasta un peso
    if (diferenciaAbsoluta > 1) {
        let mensajeDiferencia = diferencia > 0 ? `Te has excedido por ${diferencia}` : `Te faltan ${diferenciaAbsoluta}`;
        showModal(`El total acumulado de los cargos seleccionados (${totalCargosSeleccionados.toFixed(2)}) no coincide con el total del comprobante (${parseFloat(totalComprobante).toFixed(2)}). ${mensajeDiferencia}. Por favor selecciona los movimientos correctos.`);
        return;
    } else if (diferenciaAbsoluta > 0) {
        // Mostrar advertencia sobre la diferencia de hasta un peso
        showModal(`Advertencia: existe una diferencia de $${diferenciaAbsoluta}. La conciliación se realizará de todos modos.`);
    }

    checkboxesSeleccionados.forEach(checkbox => {
        let movId = checkbox.getAttribute('data-id');
        let selectedMovimiento = movimientosData.find(mov => mov.Id == movId);

        if (selectedMovimiento) {
            selectedMovimientos.push(selectedMovimiento);
        }
    });

    let tableResultData = $('#tableResult').bootstrapTable('getData');
    let lastId = tableResultData.length > 0 ? Math.max(...tableResultData.map(row => row.id)) : 0;

    if (selectedMovimientos.length > 0) {
        $('#tableResult').bootstrapTable('append', {
            Id: comprobanteSeleccionado.Id,
            //idComprobante: comprobanteSeleccionado.Id,  // Guardar el idComprobante original
            Serie: comprobanteSeleccionado.Serie,
            Folio: comprobanteSeleccionado.Folio,
            Fecha: comprobanteSeleccionado.Fecha,
            Banco: selectedMovimientos[0].Banco,
            UUID: comprobanteSeleccionado.UUID,
            //Receptor: comprobanteSeleccionado.Receptor ?? "Receptor no especificado",
            Total: parseFloat(comprobanteSeleccionado.Total).toFixed(2),
            UUID: uuid,
            movimientosConciliados: selectedMovimientos
        });

        // Marcar el comprobante seleccionado como conciliado
        comprobanteSeleccionado.coincidencia = true;
        $('#tableCardComprobantes').bootstrapTable('updateByUniqueId', {
            id: comprobanteSeleccionado.Id,
            row: comprobanteSeleccionado
        });

        // Marcar los movimientos seleccionados como conciliados
        selectedMovimientos.forEach(mov => {
            mov.coincidencia = true;
            $('#tableCardMovimientos').bootstrapTable('updateByUniqueId', {
                id: mov.Id,
                row: mov
            });
        });

        let myModal = bootstrap.Modal.getInstance(document.getElementById('modalConciliacionComp'));
        myModal.hide();

        showModal("Los movimientos seleccionados han sido conciliados.");

        // Actualizar contadores y refrescar tablas para aplicar estilos
        actualizarContadores();

        $('#tableCardComprobantes').bootstrapTable('refresh');
        $('#tableCardMovimientos').bootstrapTable('refresh');

    } else {
        showModal("No se ha seleccionado ningún movimiento.");
    }
}

function desconciliarFormatter(value, row, index) {
    // Verificar si el registro está bloqueado para deshabilitar el botón
    let disabled = row.bloqueado ? 'disabled' : '';

    return `
        <center><button class="btn btn-danger btn-sm" onclick="desconciliar(${row.Id}, '${row.Fecha}', '${row.Total}')" ${disabled}>
            <i class=""></i> Desconciliar
        </button></center>
    `;
}


//Desconciliar comprobante con movimientos y movimiento con comprobantes(unión de desconciliarMov y desconciliarComp)
function desconciliar(Id, fechaMovimiento, totalMovimiento) {
    // Obtener los datos actuales de `tableResult`
    let resultData = $('#tableResult').bootstrapTable('getData');
    let itemEnResultado = resultData.find(row => row.Id === String(Id) || row.Id === Number(Id));

    // Obtener los datos de las tablas de movimientos y comprobantes
    let movimientosData = $('#tableCardMovimientos').bootstrapTable('getData');
    let comprobantesData = $('#tableCardComprobantes').bootstrapTable('getData');

    // Verificar si el ID es de un movimiento
    let movimiento = movimientosData.find(mov => mov.Id == Id);
    if (movimiento) {
        // Lógica de desconciliación para movimientos
        movimiento.conciliado = false;
        movimiento.coincidencia = false;
        $('#tableCardMovimientos').bootstrapTable('updateByUniqueId', {
            Id: movimiento.Id,
            row: movimiento
        });
        $(`#tableCardMovimientos tr[data-uniqueid="${movimiento.Id}"]`).removeClass("table-success");

        if (itemEnResultado) {
            $('#tableResult').bootstrapTable('remove', {
                field: 'Id',
                values: [Number(Id)]
            });
            console.log("Movimiento eliminado de tableResult.");
            $('#tableResult').bootstrapTable('refresh');
        }

        // Desconciliar comprobantes asociados al movimiento
        if (itemEnResultado && itemEnResultado.comprobantesConciliados) {
            itemEnResultado.comprobantesConciliados.forEach(comprobante => {
                let comprobanteEncontrado = comprobantesData.find(comp => comp.Id === comprobante.Id);
                if (comprobanteEncontrado) {
                    comprobanteEncontrado.conciliado = false;
                    comprobanteEncontrado.coincidencia = false;
                    $('#tableCardComprobantes').bootstrapTable('updateByUniqueId', {
                        Id: comprobanteEncontrado.Id,
                        row: comprobanteEncontrado
                    });
                    $(`#tableCardComprobantes tr[data-uniqueid="${comprobanteEncontrado.Id}"]`).removeClass("table-success");
                }
            });
        }
        showModal("La desconciliación del movimiento y sus comprobantes asociados se realizó correctamente.");
        return;
    }

    // Verificar si el ID es de un comprobante
    let comprobante = comprobantesData.find(comp => comp.Id == Id);
    if (comprobante) {
        // Lógica de desconciliación para comprobantes
        comprobante.conciliado = false;
        comprobante.coincidencia = false;
        $('#tableCardComprobantes').bootstrapTable('updateByUniqueId', {
            Id: comprobante.Id,
            row: comprobante
        });
        $(`#tableCardComprobantes tr[data-uniqueid="${comprobante.Id}"]`).removeClass("table-success");

        if (itemEnResultado) {
            $('#tableResult').bootstrapTable('remove', {
                field: 'Id',
                values: [String(Id)]
            });
            console.log("Comprobante eliminado de tableResult.");
            $('#tableResult').bootstrapTable('refresh');
        }

        // Desconciliar movimientos asociados al comprobante
        if (itemEnResultado && itemEnResultado.movimientosConciliados) {
            itemEnResultado.movimientosConciliados.forEach(movimiento => {
                let movimientoEncontrado = movimientosData.find(mov => mov.Id === movimiento.Id);
                if (movimientoEncontrado) {
                    movimientoEncontrado.conciliado = false;
                    movimientoEncontrado.coincidencia = false;
                    $('#tableCardMovimientos').bootstrapTable('updateByUniqueId', {
                        Id: movimientoEncontrado.Id,
                        row: movimientoEncontrado
                    });
                    $(`#tableCardMovimientos tr[data-uniqueid="${movimientoEncontrado.Id}"]`).removeClass("table-success");
                }
            });
        }
        showModal("La desconciliación del comprobante y sus movimientos asociados se realizó correctamente.");
    }

    // Refrescar ambas tablas para reflejar los cambios visualmente
    $('#tableCardMovimientos').bootstrapTable('refresh');
    $('#tableCardComprobantes').bootstrapTable('refresh');
    $('#tableResult').bootstrapTable('refresh');

    // Actualizar los contadores
    actualizarContadores();
}

//Desconciliar movimiento con varios comprobantes
function desconciliarMov(idMovimiento, fechaMovimiento, totalMovimiento) {

    // Obtener los datos actuales de `tableResult`
    let resultData = $('#tableResult').bootstrapTable('getData');
    let movimientoEnResultado = resultData.find(row => row.id === Number(idMovimiento));

    // Obtener los datos de las tablas de movimientos y comprobantes
    let movimientosData = $('#tableCardMovimientos').bootstrapTable('getData');
    let comprobantesData = $('#tableCardComprobantes').bootstrapTable('getData');

    // Desconciliar el movimiento en `tableCardMovimientos`
    let movimiento = movimientosData.find(mov => mov.Id == idMovimiento);
    if (movimiento) {
        movimiento.conciliado = false;
        movimiento.coincidencia = false;
        $('#tableCardMovimientos').bootstrapTable('updateByUniqueId', {
            id: movimiento.Id,
            row: movimiento
        });
        $(`#tableCardMovimientos tr[data-uniqueid="${movimiento.Id}"]`).removeClass("table-success");

        if (movimientoEnResultado) {
            // Eliminar el movimiento de `tableResult`
            $('#tableResult').bootstrapTable('remove', {
                field: 'id',
                values: [Number(idMovimiento)]
            });

            console.log("Movimiento eliminado de tableResult.");

            // Refrescar la tabla para reflejar visualmente la eliminación
            $('#tableResult').bootstrapTable('refresh');
        } else {
            //alert("El movimiento no se encontró en tableResult.");
        }
    }

    // Desconciliar los comprobantes específicos asociados al movimiento
    if (movimientoEnResultado && movimientoEnResultado.comprobantesConciliados) {
        movimientoEnResultado.comprobantesConciliados.forEach(comprobante => {
            let comprobanteEncontrado = comprobantesData.find(comp => comp.Id === comprobante.Id);
            if (comprobanteEncontrado) {
                comprobanteEncontrado.conciliado = false;
                comprobanteEncontrado.coincidencia = false;
                $('#tableCardComprobantes').bootstrapTable('updateByUniqueId', {
                    id: comprobanteEncontrado.Id,
                    row: comprobanteEncontrado
                });
                $(`#tableCardComprobantes tr[data-uniqueid="${comprobanteEncontrado.Id}"]`).removeClass("table-success");
            }
        });
    }

    // Mostrar un mensaje de desconciliación exitosa
    showModal("La desconciliación del movimiento y sus comprobantes asociados se realizó correctamente.");

    // Refrescar ambas tablas para reflejar los cambios visualmente
    $('#tableCardMovimientos').bootstrapTable('refresh');
    $('#tableCardComprobantes').bootstrapTable('refresh');
    $('#tableResult').bootstrapTable('refresh');

    // Actualizar los contadores
    actualizarContadores();
}

//Desconciliar Comprobante con varios movimientos
function desconciliarComp(idComprobante, fechaMovimiento, totalMovimiento) {

    // Obtener los datos actuales de `tableResult`
    let resultData = $('#tableResult').bootstrapTable('getData');
    let comprobanteEnResultado = resultData.find(row => row.id === String(idComprobante));

    // Obtener los datos de las tablas de comprobantes y movimientos
    let comprobantesData = $('#tableCardComprobantes').bootstrapTable('getData');
    let movimientosData = $('#tableCardMovimientos').bootstrapTable('getData');

    // Desconciliar el comprobante en `tableCardComprobantes`
    let comprobante = comprobantesData.find(comp => comp.Id == idComprobante);
    if (comprobante) {
        comprobante.conciliado = false;
        comprobante.coincidencia = false;
        $('#tableCardComprobantes').bootstrapTable('updateByUniqueId', {
            id: comprobante.Id,
            row: comprobante
        });
        $(`#tableCardComprobantes tr[data-uniqueid="${comprobante.Id}"]`).removeClass("table-success");

        if (comprobanteEnResultado) {
            // Mostrar el registro en un alert antes de eliminarlo
            //alert("Registro encontrado en tableResult:\n" + JSON.stringify(comprobanteEnResultado, null, 2));

            // Eliminar el comprobante de tableResult
            $('#tableResult').bootstrapTable('remove', {
                field: 'id',
                values: [String(idComprobante)]
            });

            console.log("Comprobante eliminado de tableResult.");

            // Refrescar la tabla para reflejar visualmente la eliminación
            $('#tableResult').bootstrapTable('refresh');
        } else {
            alert("El comprobante no se encontró en tableResult.");
        }
    }

    // Desconciliar los movimientos específicos asociados al comprobante
    if (comprobanteEnResultado && comprobanteEnResultado.movimientosConciliados) {
        comprobanteEnResultado.movimientosConciliados.forEach(movimiento => {
            let movimientoEncontrado = movimientosData.find(mov => mov.Id === movimiento.Id);
            if (movimientoEncontrado) {
                movimientoEncontrado.conciliado = false;
                movimientoEncontrado.coincidencia = false;
                $('#tableCardMovimientos').bootstrapTable('updateByUniqueId', {
                    id: movimientoEncontrado.Id,
                    row: movimientoEncontrado
                });
                $(`#tableCardMovimientos tr[data-uniqueid="${movimientoEncontrado.Id}"]`).removeClass("table-success");
            }
        });
    }

    // Mostrar un mensaje de desconciliación exitosa
    showModal("La desconciliación del comprobante y sus movimientos asociados se realizó correctamente.");

    // Refrescar ambas tablas para reflejar los cambios visualmente
    $('#tableCardMovimientos').bootstrapTable('refresh');
    $('#tableCardComprobantes').bootstrapTable('refresh');
    $('#tableResult').bootstrapTable('refresh');

    // Actualizar los contadores
    actualizarContadores();
}

// Función para actualizar la suma de los totales seleccionados y el porcentaje de similitud
function actualizarContadorSeleccionados(checkbox, totalComprobanteFormateado) {
    let totalCargosSeleccionados = parseFloat(document.getElementById('totalCargosSeleccionados').innerText);

    let cargo = parseFloat(checkbox.value);
    let idMovimiento = checkbox.getAttribute('data-id'); // Obtener el ID del movimiento desde el atributo data-id

    if (checkbox.checked) {
        // Si se selecciona, agregar el valor al total y añadir el movimiento a la lista
        totalCargosSeleccionados += cargo;
        if (!movimientosSeleccionados.includes(idMovimiento)) {
            movimientosSeleccionados.push(idMovimiento);  // Agregar el ID del movimiento a la lista de seleccionados
        }
    } else {
        // Si se deselecciona, restar el valor del total y eliminar el movimiento de la lista
        totalCargosSeleccionados -= cargo;
        movimientosSeleccionados = movimientosSeleccionados.filter(mov => mov !== idMovimiento); // Eliminar de la lista
    }

    // Calcular el porcentaje de similitud en función del total del comprobante
    let porcentajeSimilitudSeleccionado = ((totalCargosSeleccionados / totalComprobanteFormateado) * 100).toFixed(2);

    // Actualizar los valores mostrados
    document.getElementById('totalCargosSeleccionados').innerText = totalCargosSeleccionados.toFixed(3);
    document.getElementById('porcentajeSimilitudSeleccionado').innerText = `${porcentajeSimilitudSeleccionado}%`;
}

// Conciliación en automático
function conciliarAutomatico() {
    let comprobantesData = $('#tableCardComprobantes').bootstrapTable('getData');
    let movimientosData = $('#tableCardMovimientos').bootstrapTable('getData');
    let resultTableData = $('#tableResult').bootstrapTable('getData');

    let coincidenciasComprobantes = 0;
    let coincidenciasMovimientos = 0;
    let nuevasCoincidencias = false;

    // Eliminar registros que fueron conciliados manualmente
    let manualEntries = resultTableData.filter(row => row.manual === true);
    if (manualEntries.length > 0) {
        manualEntries.forEach(row => {
            $('#tableResult').bootstrapTable('remove', { field: 'id', values: [row.id] });
        });
    }

    function registroYaAgregado(id, serie, folio, fecha, total) {
        return resultTableData.some(row => row.id === id &&
            row.Serie === serie &&
            row.Folio === folio &&
            row.Fecha === fecha &&
            parseFloat(row.Total).toFixed(2) === parseFloat(total).toFixed(2));
    }

    comprobantesData.forEach((comp, indexComp) => {
        let fechaComprobanteFormateada = comp.Fecha.split('T')[0];
        let totalComprobanteFormateado = parseFloat(comp.Total).toFixed(2);

        movimientosData.forEach((mov, indexMov) => {
            let fechaMovimiento = mov.Fecha.split('/').reverse().join('-');
            let cargoMovimientoFormateado = parseFloat(mov.Cargos).toFixed(2);
            let porcentajeSimilitud = ((totalComprobanteFormateado * 100) / cargoMovimientoFormateado) || 0.00;

            if (fechaMovimiento === fechaComprobanteFormateada &&
                (porcentajeSimilitud === 100 || (porcentajeSimilitud >= 99.8 && porcentajeSimilitud < 100))) {

                if (!registroYaAgregado(comp.Id, comp.Serie, comp.Folio, fechaComprobanteFormateada, totalComprobanteFormateado)) {
                    // Conciliar y agregar a la tabla
                    $('#tableResult').bootstrapTable('append', {
                        id: comp.Id,
                        Serie: comp.Serie,
                        Folio: comp.Folio,
                        Fecha: mov.Fecha,
                        Banco: mov.Banco,
                        Descripción: mov.Descripcion,
                        Total: mov.Cargos,
                        coincidencia: true,
                        conciliado: true,
                        porcentajeSimilitud: porcentajeSimilitud.toFixed(2),
                        comprobantesConciliados: [comp] // Agregar los detalles del comprobante
                    });

                    // Marcar la fila del comprobante y del movimiento como coincidente
                    if (!comp.coincidencia) {
                        $('#tableCardComprobantes').bootstrapTable('updateRow', {
                            index: indexComp,
                            row: { coincidencia: true }
                        });
                        coincidenciasComprobantes++;
                    }

                    if (!mov.coincidencia) {
                        $('#tableCardMovimientos').bootstrapTable('updateRow', {
                            index: indexMov,
                            row: { coincidencia: true }
                        });
                        coincidenciasMovimientos++;
                    }

                    nuevasCoincidencias = true;
                }
            }
        });
    });

    let mensaje = `${coincidenciasComprobantes} coincidencia(s) de comprobantes y ${coincidenciasMovimientos} coincidencia(s) de movimientos agregada(s).<br/>`;
    mensaje += "<strong>Detalles de los comprobantes registrados:</strong><br/>";

    // Agregar al mensaje la lista de coincidencias con detalles
    resultTableData.forEach(row => {
        mensaje += `ID Comprobante: ${row.id}, Serie: ${row.Serie}, Folio: ${row.Folio}, Fecha: ${row.Fecha}, Total: ${row.Total}, ` +
            `Porcentaje de similitud: ${row.porcentajeSimilitud}%<br/>`;
    });

    document.getElementById("modalConciliacionMensaje").innerHTML = mensaje;

    let myModal = new bootstrap.Modal(document.getElementById('modalConciliacion'));
    myModal.show();

    // Llamar a actualizarContadores para actualizar los contadores
    actualizarContadores();
}

// Función para mostrar detalles de un registro en `tableResult`
function detailFormatterA(index, row) {
    let movimientosConciliados = row.movimientosConciliados || [];

    if (movimientosConciliados.length === 0) {
        return `<p>No hay movimientos asociados a esta conciliación automática.</p>`;
    }

    let tableHtml = `<div class="table-responsive">
                        <table class="table table-striped table-bordered">
                            <thead>
                                <tr>
                                    <th>Fecha</th>
                                    <th>Banco</th>
                                    <th>Descripción</th>
                                    <th>Cargos</th>
                                </tr>
                            </thead>
                            <tbody>`;

    movimientosConciliados.forEach(mov => {
        tableHtml += `<tr>
                        <td>${mov.Fecha}</td>
                        <td>${mov.Banco}</td>
                        <td>${mov.Descripcion}</td>
                        <td>${currencyFormatter(mov.Cargos)}</td>
                      </tr>`;
    });

    tableHtml += `    </tbody>
                    </table>
                </div>`;

    return tableHtml;
}

// Función de formateo para valores en formato de moneda
function currencyFormatter(value) {
    return `$${parseFloat(value).toFixed(2)}`;
}

// Función de formateo para los valores en formato de moneda
function currencyFormatter(value) {
    return `$${parseFloat(value).toFixed(2)}`;
}

//Tabla tableCardComprobantes
function rowStyleComprobantes(row, index) {
    if (row.coincidencia) {
        return {
            classes: 'table-success'
        };
    }
    return {};
}

//Tabla tableResult
function rowStyle(row, index) {
    if (row.coincidencia) {
        return {
            classes: 'table-successRes' // Clase Bootstrap para filas con fondo verde
        };
    }
    return {};
}

function darkGrayCellStyle(value, row, index, field) {
    return {
        classes: 'table-dark-gray-cell' // Aplica la clase CSS de gris oscuro
    };
}

function rowStyleMovimientos(row, index) {
    if (row.coincidencia) {
        return {
            classes: 'table-success'
        };
    }
    return {};
}

// Detalles del registro conciliado de comprobantes con movimientos bancarios.
function detailFormatterM(index, row) {
    let comprobantesConciliados = row.comprobantesConciliados || [];

    if (comprobantesConciliados.length === 0) {
        return `<p>No hay comprobantes asociados a este movimiento.</p>`;
    }

    let totalMovimiento = parseFloat(row.Total).toFixed(2); // Tomar el total del movimiento
    let tableHtml = `<div class="table-responsive">
                        <table class="table table-striped table-bordered">
                            <thead>
                                <tr>
                                    <th style="color: #000000; font-weight: bold;">Id</th>
                                    <th style="color: #000000; font-weight: bold;">Serie</th>
                                    <th style="color: #000000; font-weight: bold;">Folio</th>
                                    <th style="color: #000000; font-weight: bold;">Fecha</th>
                                    <th style="color: #000000; font-weight: bold;">Total</th>
                                    <th style="color: #000000; font-weight: bold;">% Similitud</th> <!-- Agregar columna de % Similitud -->
                                </tr>
                            </thead>
                            <tbody>`;

    comprobantesConciliados.forEach(comp => {
        let totalComprobante = parseFloat(comp.Total).toFixed(2); // Tomar el total del comprobante
        let porcentajeSimilitud = ((totalComprobante * 100) / totalMovimiento) || 0; // Calcular el % de similitud

        tableHtml += `<tr>
                        <td>${comp.Id}</td>
                        <td>${comp.Serie}</td>
                        <td>${comp.Folio}</td>
                        <td>${formatDate(comp.Fecha)}</td>
                        <td>${currencyFormatter(comp.Total)}</td>
                        <td>${porcentajeSimilitud.toFixed(2)}%</td> <!-- Mostrar % de Similitud -->
                      </tr>`;
    });

    tableHtml += `    </tbody>
                    </table>
                </div>`;

    return tableHtml;
}

// Función para formatear el botón de conciliación en movimientos
function conciliarFormatterMov(value, row, index) {
    // Si el movimiento ya ha sido conciliado o está bloqueado, deshabilitar el botón
    let disabled = row.coincidencia || row.bloqueado ? 'disabled' : '';

    return `
        <button class="btn btn-primary btn-sm" onclick="conciliarMovimiento(${index}, '${row.Fecha}', '${row.Cargos}')" ${disabled}>
            <i class="bi bi-paperclip rotate-clip"></i> Conciliar
        </button>
    `;
}

// Inicializar un contador global de ID incremental
let nextId = 1;

function conciliarMovimiento(index, fechaMovimiento, cargoMovimiento) {
    let movimientosData = $('#tableCardMovimientos').bootstrapTable('getData');
    let comprobantesData = $('#tableCardComprobantes').bootstrapTable('getData');

    let mov = movimientosData[index];
    mov.id = nextId++;  // Asignar ID incremental y luego incrementar para el siguiente

    let totalMovimientoFormateado = parseFloat(cargoMovimiento).toFixed(2);

    // Convertir la fecha del movimiento a un objeto Date, extrayendo el mes y el año
    let fechaMovimientoDate = new Date(fechaMovimiento.split('/').reverse().join('-'));
    let mesMovimiento = fechaMovimientoDate.getMonth();
    let anioMovimiento = fechaMovimientoDate.getFullYear();

    // Filtrar los comprobantes que coinciden en mes y año con el movimiento y cuyo total sea <= al cargo
    let coincidenciasComprobantes = comprobantesData.filter(comp => {
        let fechaComprobanteDate = new Date(comp.Fecha);
        let mesComprobante = fechaComprobanteDate.getMonth();
        let anioComprobante = fechaComprobanteDate.getFullYear();

        return mesComprobante === mesMovimiento &&
            anioComprobante === anioMovimiento &&
            parseFloat(comp.Total) <= parseFloat(cargoMovimiento);
    });

    // Calcular el porcentaje de similitud para cada comprobante y añadirlo al objeto
    coincidenciasComprobantes = coincidenciasComprobantes.map((comp, idx) => {
        let porcentajeSimilitud = ((parseFloat(comp.Total) / parseFloat(cargoMovimiento)) * 100).toFixed(2);
        return { ...comp, porcentajeSimilitud, incrementalId: idx + 1 };
    });

    // Ordenar los comprobantes por el porcentaje de similitud en orden descendente
    coincidenciasComprobantes.sort((a, b) => parseFloat(b.porcentajeSimilitud) - parseFloat(a.porcentajeSimilitud));

    // Actualizar el header del modal con los datos del movimiento seleccionado
    let modalHeader = document.getElementById('modalSimilitudHeader');
    modalHeader.innerHTML = `
        <p><strong>Movimiento Seleccionado:</strong></p>
        <p>ID: ${mov.id}</p>
        <p>Fecha: ${mov.Fecha}</p>
        <p>Descripción: ${mov.Descripcion}</p>
        <p>Cargos: <span>${currencyFormatter(mov.Cargos)}</span></p>
        <p><strong>Total Seleccionado: $<span id="totalSeleccionado">0.00</span> (<span id="porcentajeSeleccionado">0%</span>)</strong></p>
        <hr/>
    `;

    // Limpiar el contenido previo del body de la tabla (tbody)
    let modalTableBody = document.getElementById('modalSimilitudBody');
    modalTableBody.innerHTML = '';

    if (coincidenciasComprobantes.length > 0) {
        coincidenciasComprobantes.forEach(comp => {
            let fechaComprobanteDate = new Date(comp.Fecha);
            let fechaFormateada = ("0" + fechaComprobanteDate.getDate()).slice(-2) + "/" +
                ("0" + (fechaComprobanteDate.getMonth() + 1)).slice(-2) + "/" +
                fechaComprobanteDate.getFullYear();

            let totalFormateado = parseFloat(comp.Total).toFixed(2);

            modalTableBody.innerHTML += `
            <tr>
                <td>${comp.incrementalId}</td>
                <td>${comp.Serie}</td>
                <td>${comp.Folio}</td>
                <td>${fechaFormateada}</td>
                <td>${currencyFormatter(totalFormateado)}</td>
                <td><strong>${comp.porcentajeSimilitud}%</strong></td>
                <td><input type="checkbox" class="form-check-input" data-id="${comp.Id}" value="${comp.Total}" onchange="actualizarSumaSeleccionados(this, ${cargoMovimiento})"></td>
            </tr>
        `;
        });
    } else {
        modalTableBody.innerHTML = `<tr><td colspan="7"><p>No se encontraron comprobantes para el mismo mes y año.</p></td></tr>`;
    }

    let modalFooter = document.getElementById('modalSimilitudFooter');
    modalFooter.innerHTML = `
        <button class="btn btn-primary btn-sm" style="width: 30%;" onclick="conciliarSeleccionados(${index}, ${cargoMovimiento})">Conciliar Seleccionados</button>
    `;

    let myModal = new bootstrap.Modal(document.getElementById('modalSimilitud'));
    myModal.show();
}

// Variable global para el ID autoincrementable
let autoIncrementId = 1;

function conciliarSeleccionados(indexMovimiento, cargoMovimiento) {
    let totalSeleccionado = parseFloat(document.getElementById('totalSeleccionado').innerText);
    let diferencia = (totalSeleccionado - parseFloat(cargoMovimiento)).toFixed(2);
    let diferenciaAbsoluta = Math.abs(diferencia);

    if (diferenciaAbsoluta > 1) { // Permitir una diferencia máxima de hasta un peso
        if (totalSeleccionado > cargoMovimiento) {
            showModal(`El total seleccionado es mayor por $${diferencia}. Selecciona correctamente los registros.`);
        } else {
            showModal(`El total seleccionado es menor por $${diferenciaAbsoluta}. Selecciona correctamente los registros.`);
        }
        return;
    } else if (diferenciaAbsoluta > 0) {
        // Mostrar advertencia sobre la diferencia de hasta un peso
        showModal(`Advertencia: existe una diferencia de $${diferenciaAbsoluta}. La conciliación se realizará de todos modos.`);
    }

    let selectedRows = [];
    let movimientosData = $('#tableCardMovimientos').bootstrapTable('getData');
    let comprobantesData = $('#tableCardComprobantes').bootstrapTable('getData');
    let mov = movimientosData[indexMovimiento];

    // Obtener los comprobantes seleccionados dentro del modal
    document.querySelectorAll('#modalSimilitudBody input[type="checkbox"]:checked').forEach(checkbox => {
        let compId = checkbox.getAttribute('data-id');
        let selectedRow = comprobantesData.find(comp => comp.Id == compId);
        if (selectedRow) {
            selectedRows.push(selectedRow);
        }
    });

    if (selectedRows.length > 0) {
        let primerComprobante = selectedRows[0];

        // Añadir registro a `tableResult` con ID autoincrementable
        $('#tableResult').bootstrapTable('append', {
            id: mov.Id, // Asignar el ID autoincrementable y luego incrementar
            Serie: primerComprobante.Serie,
            Folio: primerComprobante.Folio,
            Fecha: mov.Fecha,
            Banco: mov.Banco,
            Descripción: mov.Descripcion,
            Total: mov.Cargos,
            coincidencia: true,
            comprobantesConciliados: selectedRows
        });

        // Marcar comprobantes seleccionados como conciliados
        selectedRows.forEach(comp => {
            comp.coincidencia = true;  // Marcar como coincidencia para aplicar el estilo
            $('#tableCardComprobantes').bootstrapTable('updateByUniqueId', {
                id: comp.Id,
                row: comp
            });
        });

        // Marcar el movimiento seleccionado como conciliado
        mov.coincidencia = true;  // Marcar como coincidencia para aplicar el estilo
        $('#tableCardMovimientos').bootstrapTable('updateByUniqueId', {
            id: mov.Id,
            row: mov
        });

        // Cerrar el modal
        let myModal = bootstrap.Modal.getInstance(document.getElementById('modalSimilitud'));
        myModal.hide();

        showModal("Los registros seleccionados han sido conciliados.");

        // Refrescar ambas tablas para aplicar el estilo de coincidencia
        $('#tableCardMovimientos').bootstrapTable('refresh');
        //$('#tableCardComprobantes').bootstrapTable('refresh');

        // Actualizar contadores si es necesario
        actualizarContadores();
    } else {
        showModal("No se ha seleccionado ningún registro.");
    }
}

//Desconcilia un comprobante que ha sido asociado con uno o varios movimientos.
function desconciliarRegistro(id) {
    console.log("ID recibido del Movimiento:", id);
    // Eliminar el registro de la tabla tableResult
    $('#tableResult').bootstrapTable('remove', { field: 'id', values: [id] });

    let comprobantesData = $('#tableCardComprobantes').bootstrapTable('getData');
    let movimientosData = $('#tableCardMovimientos').bootstrapTable('getData');

    // Encontrar el índice del comprobante en la tabla de comprobantes y quitar el color verde
    let indexComp = comprobantesData.findIndex(comp => comp.Id === id);
    if (indexComp !== -1) {
        $('#tableCardComprobantes').bootstrapTable('updateRow', {
            index: indexComp,
            row: { coincidencia: false }
        });
    }

    // Buscar en la tabla de movimientos, para quitar el color verde en base al movimiento asociado al comprobante
    let rowToRemove = $('#tableResult').bootstrapTable('getRowByUniqueId', id);

    movimientosData.forEach((mov, indexMov) => {
        if (mov.Total === rowToRemove.Total && mov.Fecha === rowToRemove.Fecha) {
            $('#tableCardMovimientos').bootstrapTable('updateRow', {
                index: indexMov,
                row: { coincidencia: false }
            });
        }
    });

    // Actualizar los contadores
    let totalConciliadosC = parseInt(document.getElementById("TotalConciliadosC").innerText) - 1;
    let totalConciliadosM = parseInt(document.getElementById("TotalConciliadosM").innerText) - 1;
    document.getElementById("TotalConciliadosC").innerText = totalConciliadosC;
    document.getElementById("TotalConciliadosM").innerText = totalConciliadosM;

    // Recargar tablas y actualizar contadores
    recargarTablas(false);
    actualizarContadores();

    showModal("El registro ha sido desconciliado y los datos se han actualizado.");
}

// Función para actualizar la suma de los totales seleccionados y el porcentaje de similitud
function actualizarSumaSeleccionados(checkbox, cargoMovimiento) {
    let totalSeleccionado = parseFloat(document.getElementById('totalSeleccionado').innerText);

    if (checkbox.checked) {
        // Si se selecciona, agregar el valor al total
        totalSeleccionado += parseFloat(checkbox.value);
    } else {
        // Si se deselecciona, restar el valor del total
        totalSeleccionado -= parseFloat(checkbox.value);
    }

    // Calcular el porcentaje de similitud en base al cargo del movimiento
    let porcentajeSeleccionado = ((totalSeleccionado / cargoMovimiento) * 100).toFixed(2);

    // Actualizar el texto en el contador y porcentaje
    document.getElementById('totalSeleccionado').innerText = totalSeleccionado.toFixed(2);
    document.getElementById('porcentajeSeleccionado').innerText = `${porcentajeSeleccionado}%`;
}

// Función para recargar las tablas de movimientos y comprobantes, asegurando que solo los no conciliados se muestren
function recargarTablas(mostrarTodos = false) {
    // Obtener los datos completos de las tablas
    let comprobantesData = $('#tableCardComprobantes').bootstrapTable('getData');
    let movimientosData = $('#tableCardMovimientos').bootstrapTable('getData');

    // Mostrar todos o solo los no conciliados dependiendo de `mostrarTodos`
    let comprobantesFiltrados = mostrarTodos ? comprobantesData : comprobantesData.filter(comp => !comp.conciliado);
    let movimientosFiltrados = mostrarTodos ? movimientosData : movimientosData.filter(mov => !mov.conciliado);

    // Recargar las tablas con los datos filtrados
    $('#tableCardComprobantes').bootstrapTable('load', comprobantesFiltrados);
    $('#tableCardMovimientos').bootstrapTable('load', movimientosFiltrados);
}

// Función para actualizar los contadores de conciliados y no conciliados
function actualizarContadores() {
    // Obtener los datos de comprobantes y movimientos desde el DOM en lugar de `bootstrapTable`
    let comprobantesRows = document.querySelectorAll('#tableCardComprobantes tbody tr');
    let movimientosRows = document.querySelectorAll('#tableCardMovimientos tbody tr');

    // Función para verificar si una fila está en verde
    const isGreenRow = (row) => {
        const backgroundColor = getComputedStyle(row).backgroundColor;
        return backgroundColor === 'rgb(212, 237, 218)'; // Valor de color verde claro (puede variar)
    };

    // Contar comprobantes conciliados (en verde) y sin conciliar
    let totalConciliadosC = Array.from(comprobantesRows).filter(row => isGreenRow(row)).length;
    let totalSinConciliarC = comprobantesRows.length - totalConciliadosC;

    // Contar movimientos conciliados (en verde) y sin conciliar
    let totalConciliadosM = Array.from(movimientosRows).filter(row => isGreenRow(row)).length;
    let totalSinConciliarM = movimientosRows.length - totalConciliadosM;

    // Actualizar los contadores en el DOM
    document.getElementById("TotalConciliadosC").innerText = totalConciliadosC;
    document.getElementById("TotalConciliadosM").innerText = totalConciliadosM;
    document.getElementById("TotalSinConciliarC").innerText = totalSinConciliarC;
    document.getElementById("TotalSinConciliarM").innerText = totalSinConciliarM;
}

function conciliarDesdeModal(idComprobante, fechaMovimiento, cargoMovimiento, indexMovimiento) {
    let comprobantesData = $('#tableCardComprobantes').bootstrapTable('getData');
    let movimientosData = $('#tableCardMovimientos').bootstrapTable('getData');
    let resultTableData = $('#tableResult').bootstrapTable('getData');

    let comp = comprobantesData.find(comp => comp.Id === idComprobante);
    let mov = movimientosData[indexMovimiento];
    let totalMovimientoFormateado = parseFloat(cargoMovimiento).toFixed(2);

    // Verificar si ya está conciliado
    let registroYaAgregado = resultTableData.some(row => row.Fecha === mov.Fecha && parseFloat(row.Total).toFixed(2) === totalMovimientoFormateado);

    if (!registroYaAgregado) {
        // Agregar la conciliación del movimiento y comprobante en la tabla `tableResult`
        $('#tableResult').bootstrapTable('append', {
            id: comp.Id,
            Serie: comp.Serie,
            Folio: comp.Folio,
            Fecha: mov.Fecha,
            Banco: mov.Banco,
            Descripción: mov.Descripcion,
            Total: mov.Cargos,
            coincidencia: true
        });

        // Marcar el comprobante y movimiento como conciliados
        $('#tableCardComprobantes').bootstrapTable('updateRow', {
            index: $('#tableCardComprobantes').bootstrapTable('getData').findIndex(c => c.Id === comp.Id),
            row: { coincidencia: true }
        });

        $('#tableCardMovimientos').bootstrapTable('updateRow', {
            index: indexMovimiento,
            row: { coincidencia: true }
        });

        // Actualizar el contador de conciliados
        let totalConciliadosM = parseInt(document.getElementById("TotalConciliadosM").innerText);
        totalConciliadosM++;
        document.getElementById("TotalConciliadosM").innerText = totalConciliadosM;

        // Cerrar el modal
        let myModal = bootstrap.Modal.getInstance(document.getElementById('modalSimilitud'));
        myModal.hide();
    } else {
        //alert("El movimiento ya ha sido conciliado.");
        showModal("El movimiento ya ha sido conciliado.");
    }
}

function mostrarMovimientosConciliacion() {
    let movimientosData = $('#tableCardMovimientos').bootstrapTable('getData', { includeHiddenRows: true });
    let listaMovimientos = document.getElementById("listaMovimientos");

    // Limpiar cualquier contenido previo en la lista
    listaMovimientos.innerHTML = '';

    // Asignar ID incremental y crear los elementos de la lista con los datos de movimientos
    movimientosData.forEach((mov, index) => {
        mov.Id = index + 1; // Asignar ID incremental empezando desde 1

        // Crear el elemento de lista para cada movimiento
        let listItem = document.createElement("li");
        listItem.className = "list-group-item";

        // Texto con los detalles del movimiento, incluyendo ID y estado de conciliación
        listItem.innerHTML = `
            <div>
                <strong>ID:</strong> ${mov.Id} |
                <strong>Fecha:</strong> ${mov.Fecha} |
                <strong>Banco:</strong> ${mov.Banco} |
                <strong>Descripción:</strong> ${mov.Descripcion} |
                <strong>Total:</strong> ${mov.Cargos}
            </div>
            <div class="mt-2">
                <span class="badge ${mov.conciliado ? 'bg-success' : 'bg-danger'}">
                    ${mov.conciliado ? 'Conciliado' : 'No conciliado'}
                </span>
            </div>
        `;

        // Si el movimiento está conciliado, agregar los comprobantes relacionados
        if (mov.conciliado && mov.comprobantesConciliados) {
            let comprobantesList = document.createElement("ul");
            comprobantesList.className = "list-group mt-2";

            mov.comprobantesConciliados.forEach(comp => {
                let comprobanteItem = document.createElement("li");
                comprobanteItem.className = "list-group-item";

                comprobanteItem.innerHTML = `
                    <div>
                        <strong>Comprobante ID:</strong> ${comp.Id} |
                        <strong>Serie:</strong> ${comp.Serie} |
                        <strong>Folio:</strong> ${comp.Folio} |
                        <strong>Fecha:</strong> ${comp.Fecha} |
                        <strong>Total:</strong> ${comp.Total}
                    </div>
                `;

                comprobantesList.appendChild(comprobanteItem);
            });

            // Añadir la lista de comprobantes al elemento de movimiento
            listItem.appendChild(comprobantesList);
        }

        // Añadir el elemento a la lista
        listaMovimientos.appendChild(listItem);
    });

    // Mostrar el modal
    let myModal = new bootstrap.Modal(document.getElementById('modalConciliacionMasiva'), {
        keyboard: false
    });
    myModal.show();
}

//Función para mostrar detalles de los movimientos o comprobantes seleccionados
function detailFormatter(index, row) {
    if (row.tipoConciliacion === 'automatica') {
        return detailFormatterA(index, row);
    } else if (row.comprobantesConciliados) {
        return detailFormatterM(index, row);
    } else {
        return detailFormatterC(index, row);
    }
}

//Función para dar formato de moneda a los campos numéricos.
function currencyFormatter(value, row, index) {
    return `$ ${numFormatter.format(value)}`;
}

//Función para dar formato de número a los campos numéricos.
function numericFormatter(value, row, index) {
    return numFormatter.format(value);
}

//Función para dar formato a fechas
function formatDate(dateString) {
    if (!dateString) return "";
    const date = new Date(dateString);
    if (isNaN(date)) return ""; // Verifica si la fecha es válida

    // Formatear como dd/MM/yyyy
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0'); // Meses son 0-indexados
    const year = date.getFullYear();

    return `${day}/${month}/${year}`;
}

//Función que servira para el modal de conciliados y no conciliados
function showModal(message) {
    document.getElementById("alertModalBody").innerText = message;
    var alertModal = new bootstrap.Modal(document.getElementById("alertModal"));
    alertModal.show();
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

function bloquearRegistrosAlVer() {
    // Bloquear registros de comprobantes
    $("#tableCardComprobantes .comprobante-row").each(function () {
        let rowData = $(this).data("row");
        if (rowData.coincidencia) {
            rowData.bloqueado = true;
        }
    });

    // Bloquear registros de movimientos
    $("#tableCardMovimientos .movimiento-row").each(function () {
        let rowData = $(this).data("row");
        if (rowData.coincidencia) {
            rowData.bloqueado = true;
        }
    });

    // Bloquear registros de tableResult
    $("#tableResult .table-result-row").each(function () {
        let rowData = $(this).data("row");
        rowData.bloqueado = true; // Marcar como bloqueado
    });

    // Refrescar las tablas para aplicar los cambios visuales
    $('#tableCardComprobantes').bootstrapTable('refresh');
    $('#tableCardMovimientos').bootstrapTable('refresh');
    $('#tableResult').bootstrapTable('refresh');
}

//Funcionalidad Diálogo Conciliación
function initConciliacionDialog(action, row) {
    // Obtener los campos del formulario
    let idField = document.getElementById("inpConciliacionId");
    let fechaField = document.getElementById("inpConciliacionFecha");
    let clienteIdField = document.getElementById("inpConciliacionClienteId");
    let descripcionField = document.getElementById("inpConciliacionDescripcion");
    let dlgTitle = document.getElementById("dlgConciliacionTitle");

    // Botones
    let btnGuardar = document.getElementById("dlgConciliacionBtnGuardar");
    let botonConsultarComprobantes = document.getElementById("dlgConciliacionBtnFechas");
    let botonConsultarMovimientos = document.getElementById("dlgConciliacionBtnMovimientos");
    let botonConciliacionAsistida = document.getElementById("dlgConciliacionAsistidaBtn");
    let botonFinalizarConciliacion = document.getElementById("dlgConciliacionBtnCerrar");
    let saveValidationSummary = document.getElementById("saveValidationSummary");
    saveValidationSummary.innerHTML = "";

    idField.setAttribute("disabled", true);

    switch (action) {
        case NUEVO:
            dlgTitle.innerHTML = dlgNuevoTitle;

            idField.setAttribute("disabled", true);
            fechaField.removeAttribute("disabled");
            clienteIdField.removeAttribute("disabled");
            descripcionField.removeAttribute("disabled");
            btnGuardar.removeAttribute("disabled");
            botonConsultarMovimientos.removeAttribute("disabled");
            botonConciliacionAsistida.removeAttribute("disabled");

            // Deshabilitar el botón `botonConsultarComprobantes` inicialmente
            botonConsultarComprobantes.setAttribute("disabled", true);

            // Habilitar el botón `botonConsultarComprobantes` solo cuando `clienteIdField` tenga un valor completo
            clienteIdField.addEventListener("change", function () {
                if (clienteIdField.value.trim() !== "") {
                    botonConsultarComprobantes.removeAttribute("disabled");
                } else {
                    botonConsultarComprobantes.setAttribute("disabled", true);
                }
            });

            break;
        case EDITAR:
            dlgTitle.innerHTML = dlgEditarTitle;

            fechaField.removeAttribute("disabled");
            clienteIdField.removeAttribute("disabled");
            descripcionField.removeAttribute("disabled");
            btnGuardar.removeAttribute("disabled");
            botonConsultarComprobantes.removeAttribute("disabled");
            botonConsultarMovimientos.removeAttribute("disabled");
            botonConciliacionAsistida.removeAttribute("disabled");
            botonFinalizarConciliacion.removeAttribute("disabled");

            // Cargar comprobantes y movimientos solo en modo EDITAR
            obtenerRegistrosConciliadosEdit(row.id);
            break;
        default:
            dlgTitle.innerHTML = dlgVerTitle;

            fechaField.setAttribute("disabled", true);
            clienteIdField.setAttribute("disabled", true);
            descripcionField.setAttribute("disabled", true);
            btnGuardar.setAttribute("disabled", true);
            botonConsultarComprobantes.setAttribute("disabled", true);
            botonConsultarMovimientos.setAttribute("disabled", true);
            botonConciliacionAsistida.setAttribute("disabled", true);
            botonFinalizarConciliacion.setAttribute("disabled", true);

            // Verificar si la tabla ya tiene datos antes de recargar
            obtenerRegistrosConciliados(row.id);
            bloquearRegistrosAlVer();

            break;
    }

    // Asignar valores a los campos del diálogo usando los valores de la entidad Conciliacion
    idField.value = row.id || "";
    fechaField.value = row.FechaJS || "";
    clienteIdField.value = row.Cliente || "";
    descripcionField.value = row.Descripcion || "";

    // Mostrar el modal
    dlgConciliacionModal.show();
}

// Función para obtener comprobantes y movimientos conciliados
/*function obtenerRegistrosConciliados(conciliacionId) {
    let oParams = { id: conciliacionId };
    $.extend(postOptions, { type: 'GET' });
    
    doAjax(
        "/ERP/Conciliaciones/ComprobantesMovimientosList",
        oParams,
        function (resp) {
            if (resp.tieneError) {
                let summary = ``;
                if (Array.isArray(resp.errores) && resp.errores.length >= 1) {
                    resp.errores.forEach(function (error) {
                        summary += `<li>${error}</li>`;
                    });
                }
                showError("Error, favor de revisar", resp.mensaje);
                return;
            }

            $('#tableCardComprobantes').bootstrapTable('load', responseHandler(resp.datos));

        }, function (error) {
            showError("Error", error);
        },
        postOptions
    );
}*/
function obtenerRegistrosConciliados(conciliacionId) {

    // Limpiar las tablas antes de cargar nuevos datos
    $('#tableCardComprobantes').bootstrapTable('load', []);
    $('#tableCardMovimientos').bootstrapTable('load', []);
    $('#tableResult').bootstrapTable('load', []);

    let oParams = { id: conciliacionId };
    $.extend(postOptions, { type: 'GET' });

    doAjax(
        "/ERP/Conciliaciones/ProcessedConciliacionList",
        oParams,
        function (resp) {
            if (resp.tieneError) {
                let summary = ``;
                if (Array.isArray(resp.errores) && resp.errores.length >= 1) {
                    resp.errores.forEach(function (error) {
                        summary += `<li>${error}</li>`;
                    });
                }
                showError("Error, favor de revisar", resp.mensaje);
                return;
            }

            // Procesar los datos y extraer los arreglos de comprobantes, movimientos y conciliados
            //const { comprobantes, movimientos } = procesarDatosConciliados(resp.datos);
            //const { comprobantes, movimientos, conciliaciones } = procesarDatosConciliados(resp.datos);
            const { comprobantes, movimientos, conciliaciones } = JSON.parse(resp.datos);

            // Cargar los datos en las tablas correspondientes
            $('#tableCardComprobantes').bootstrapTable('load', comprobantes);
            $('#tableCardMovimientos').bootstrapTable('load', movimientos);
            $('#tableResult').bootstrapTable('load', conciliaciones);

        }, function (error) {
            showError("Error", error);
        },
        postOptions
    );
}
function obtenerRegistrosConciliadosEdit(conciliacionId) {

    // Limpiar las tablas antes de cargar nuevos datos
    $('#tableCardComprobantes').bootstrapTable('load', []);
    $('#tableCardMovimientos').bootstrapTable('load', []);
    $('#tableResult').bootstrapTable('load', []);

    let oParams = { id: conciliacionId };
    $.extend(postOptions, { type: 'GET' });

    doAjax(
        "/ERP/Conciliaciones/ProcessedConciliacionEditList",
        oParams,
        function (resp) {
            if (resp.tieneError) {
                let summary = ``;
                if (Array.isArray(resp.errores) && resp.errores.length >= 1) {
                    resp.errores.forEach(function (error) {
                        summary += `<li>${error}</li>`;
                    });
                }
                showError("Error, favor de revisar", resp.mensaje);
                return;
            }

            // Procesar los datos y extraer los arreglos de comprobantes, movimientos y conciliados
            //const { comprobantes, movimientos } = procesarDatosConciliados(resp.datos);
            //const { comprobantes, movimientos, conciliaciones } = procesarDatosConciliados(resp.datos);
            const { comprobantes, movimientos, conciliaciones } = JSON.parse(resp.datos);

            // Cargar los datos en las tablas correspondientes
            $('#tableCardComprobantes').bootstrapTable('load', comprobantes);
            $('#tableCardMovimientos').bootstrapTable('load', movimientos);
            $('#tableResult').bootstrapTable('load', conciliaciones);

        }, function (error) {
            showError("Error", error);
        },
        postOptions
    );
}

function procesarDatosConciliadosEdit(datos) {
    let parsedData;
    try {
        parsedData = JSON.parse(datos.value.datos);
    } catch (error) {
        console.error("Error al parsear los datos:", error);
        return { comprobantes: [], movimientos: [], resultados: [], conciliaciones: [] };
    }

    let detalles = parsedData.detalles;

    if (!Array.isArray(detalles)) {
        console.error("La propiedad 'detalles' no existe o no es un arreglo.");
        return { comprobantes: [], movimientos: [], resultados: [], conciliaciones: [] };
    }

    let comprobantes = [];
    let movimientos = [];
    let conciliaciones = [];
    let idsConciliados = new Set();
    let mapaMovimientos = {};

    // Procesar detallesMovimientos y llenar el mapa de movimientos
    detalles.forEach(detalle => {
        if (Array.isArray(detalle.detallesMovimientos)) {
            detalle.detallesMovimientos.forEach(movimiento => {
                mapaMovimientos[movimiento.Id] = movimiento;

                movimientos.push({
                    Id: movimiento.Id,
                    Fecha: movimiento.Fecha,
                    Descripcion: movimiento.Descripción || "Sin descripción",
                    Cargos: parseFloat(movimiento.Cargos) || 0,
                    Abonos: parseFloat(movimiento.Abonos) || 0,
                    Banco: movimiento.BancoId || "-",
                    bloqueado: true
                });
            });
        }
    });

    // Procesar detallesComprobantes y asociar movimientos conciliados
    detalles.forEach(detalle => {
        if (Array.isArray(detalle.detallesComprobantes)) {
            detalle.detallesComprobantes.forEach(comprobante => {
                if (idsConciliados.has(comprobante.Id)) return;
                idsConciliados.add(comprobante.Id);

                // Buscar movimientos asociados a este comprobante
                let movimientosConciliados = [];
                if (Array.isArray(detalle.detallesMovimientos)) {
                    detalle.detallesMovimientos.forEach(mov => {
                        if (mov.Id) {
                            movimientosConciliados.push({
                                Id: mov.Id,
                                Fecha: mov.Fecha,
                                Banco: mov.BancoId || "-",
                                Descripcion: mov.Descripción || "Sin descripción",
                                Cargos: parseFloat(mov.Cargos) || 0,
                                Abonos: parseFloat(mov.Abonos) || 0,
                                bloqueado: true
                            });
                        }
                    });
                }

                // Agregar comprobante con movimientos conciliados
                comprobantes.push({
                    Id: comprobante.Id,
                    Serie: comprobante.Serie,
                    Folio: comprobante.Folio,
                    Fecha: comprobante.Fecha,
                    UUID: comprobante.UUID,
                    Total: parseFloat(comprobante.Total) || 0,
                    movimientosConciliados: movimientosConciliados,
                    bloqueado: true
                });

                // Añadir a conciliaciones para tableResult
                conciliaciones.push({
                    id: comprobante.Id,
                    Serie: comprobante.Serie,
                    Folio: comprobante.Folio,
                    Fecha: comprobante.Fecha,
                    Total: parseFloat(comprobante.Total) || 0,
                    movimientosConciliados: movimientosConciliados
                });
            });
        }
    });

    return { comprobantes, movimientos, conciliaciones };
}


function onGuardarClick() {
    $("#theFormT").validate();
    let valid = $("#theFormT").valid();
    if (!valid) { return; }

    let idField = document.getElementById("inpConciliacionId");
    let fechaField = document.getElementById("inpConciliacionFecha");
    let clienteIdField = document.getElementById("inpConciliacionClienteId");
    let descripcionField = document.getElementById("inpConciliacionDescripcion");
    let bancoIdField = document.getElementById("selFiltroBanco");

    let dlgTitle = document.getElementById("dlgConciliacionTitle");
    let saveValidationSummary = document.getElementById("saveValidationSummary");
    let btnClose = document.getElementById("dlgConciliacionBtnCancelar");
    saveValidationSummary.innerHTML = "";

    let oParams = {
        Id: idField.value === "Nuevo" ? 0 : idField.value,
        FechaElaboracionInicio: fechaField.value,
        Cliente: clienteIdField.value,
        Descripcion: descripcionField.value,
        BancoId: bancoIdField.value,
        Movimientos: [],
        Comprobantes: []
    };

    let allMovRows = $('#tableCardMovimientos').bootstrapTable('getData');
    allMovRows.forEach(function (row) {
        if (row.coincidencia === true) {
            let movimiento = {
                Fecha: row.Fecha,
                Descripcion: row.Descripcion,
                Importe: row.Cargos
            };
            oParams.Movimientos.push(movimiento);
        }
    });

    let allCompRows = $('#tableCardComprobantes').bootstrapTable('getData');
    allCompRows.forEach(function (row) {
        if (row.coincidencia === true) {
            let comprobante = {
                Id: row.Id,
                Serie: row.Serie,
                Folio: row.Folio,
                Fecha: row.Fecha,
                UUID: row.UUID,
                Total: row.Total
            };
            oParams.Comprobantes.push(comprobante);
        }
    });

    console.log("Comprobantes seleccionados:", oParams.Comprobantes); // Para verificar

    doAjax(
        "/ERP/Conciliaciones/SaveConciliacion",
        oParams,
        function (resp) {
            if (resp.tieneError) {
                if (Array.isArray(resp.errores) && resp.errores.length >= 1) {
                    let summary = "";
                    resp.errores.forEach(function (error) {
                        summary += `<li>${error}</li>`;
                    });
                    saveValidationSummary.innerHTML += `<ul>${summary}</ul>`;
                }
                showError(dlgTitle.innerHTML, resp.mensaje);
                return;
            }

            btnClose.click();
            onBuscarClick();
            showSuccess(dlgTitle.innerHTML, resp.mensaje);
        },
        function (error) {
            showError("Error", error);
        },
        postOptions
    );
}

function eliminarRegistro(Id) {
    if (confirm('¿Estás seguro de eliminar el registro con ID: ' + Id + '?')) {
        alert('Registro eliminado con éxito.');

        // Recargar tablas y actualizar contadores
        recargarTablas(false);
        actualizarContadores();
    }
}

//Función para filtrar los datos de la tabla.
function onBuscarClick() {
    let btnBuscar = document.getElementById("btnBuscar");
    let inpId = document.getElementById("inpFiltroId");
    let inpCliente = document.getElementById("inpFiltroCliente");
    let inpUsuarioCreador = document.getElementById("inpFiltroUsuarioCreador");
    let inpUsuarioModificador = document.getElementById("inpFiltroUsuarioModificador");
    let inpFechaElaboracionInicio = document.getElementById("inpFiltroFechaElaboracionInicio");
    let inpFechaElaboracionFin = document.getElementById("inpFiltroFechaElaboracionFin");

    let oParams = {
        id: inpId.value ? parseInt(inpId.value) || null : null,
        cliente: inpCliente.value || null,
        usuarioCreador: inpUsuarioCreador.value || null,
        usuarioModificador: inpUsuarioModificador.value || null,
        fechaElaboracionInicio: inpFechaElaboracionInicio.value || null,
        fechaElaboracionFin: inpFechaElaboracionFin.value || null
    };

    doAjax(
        "/ERP/Conciliaciones/FiltrarConciliaciones",
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

    // Resetea el valor de los filtros después de la solicitud.
    document.querySelectorAll("#filtros .form-control").forEach(function (e) { e.value = ""; });
    document.querySelectorAll("#filtros .form-select").forEach(function (e) { e.value = 0; });
}

function onConsultarComprobantesClick() {
    let btnBuscar = document.getElementById("dlgConsultarBtnGuardar");
    let inpFechaInicial = document.getElementById("inpFiltroFechaInicioModalDComprobantes");
    let inpFechaFinal = document.getElementById("inpFiltroFechaFinModalDComprobantes");
    //let clienteIdField = document.getElementById("inpConciliacionClienteId"); // Obtener el campo del cliente seleccionado

    let oParams = {
        FechaInicioModalDComprobantes: inpFechaInicial.value,
        FechaFinModalDComprobantes: inpFechaFinal.value,
        //Cliente: clienteIdField.value // Incluir el cliente seleccionado
    };

    // Resetea el valor de los filtros
    document.querySelectorAll("#filtros .form-control").forEach(function (e) { e.value = ""; });
    document.querySelectorAll("#filtros .form-select").forEach(function (e) { e.value = 0; });

    doAjax(
        "/ERP/Conciliaciones/FiltrarComprobantesFechas",
        oParams,
        function (resp) {
            if (resp.tieneError) {
                if (Array.isArray(resp.errores) && resp.errores.length >= 1) {
                    let summary = ``;
                    resp.errores.forEach(function (error) {
                        summary += `<li>${error}</li>`;
                    });
                    saveValidationSummary.innerHTML += `<ul>${summary}</ul>`;
                }
                showError(btnBuscar.innerHTML, resp.mensaje);
                return;
            }

            $('#tableCardComprobantes').bootstrapTable('load', responseHandler(resp.datos));

            // Actualizar contadores después de cargar los datos
            actualizarContadores();

            // Cerrar el modal de fechas
            let modal = bootstrap.Modal.getInstance(document.getElementById('consultarComprobantesModal'));
            if (modal) {
                modal.hide();
            }
        }, function (error) {
            showError("Error", error);
        },
        postOptions
    );
}

function onGuardarMovimientos() {
    // Validación para asegurar que haya datos en la tabla
    $("#theFormM").validate();
    let valid = $("#theFormM").valid();
    if (!valid) { return; }

    // Contenedor de resumen de validación
    let saveValidationSummary = document.getElementById("saveValidationSummaryM");
    saveValidationSummary.innerHTML = "";

    // Obtén todos los datos de la tabla `tableCardMovimientos`
    var datosMovimientos = $('#tableCardMovimientos').bootstrapTable('getData');

    if (datosMovimientos.length === 0) {
        saveValidationSummary.innerHTML = `<ul><li>No hay datos para subir a la base de datos.</li></ul>`;
        return;
    }

    // Transformar los datos para que coincidan con la entidad MovimientoBancario
    let movimientosParaEnviar = datosMovimientos.map(mov => {
        let importe = mov.Cargos ? parseFloat(mov.Cargos) : parseFloat(mov.Abonos);

        return {
            Fecha: mov.Fecha,
            Descripcion: mov.Descripcion,
            Importe: importe,
            Conciliado: false // Se inicializa como no conciliado
        };
    });

    console.table(movimientosParaEnviar);

    // Parámetros que se enviarán en la solicitud
    let oParams = { movimientos: movimientosParaEnviar };

    // Llamada AJAX utilizando `doAjax`
    doAjax(
        "/ERP/Conciliaciones/GuardarMovimientosImportados",
        oParams,
        function (resp) {
            if (resp.tieneError) {
                if (Array.isArray(resp.errores) && resp.errores.length >= 1) {
                    let summary = ``;
                    resp.errores.forEach(function (error) {
                        summary += `<li>${error}</li>`;
                    });
                    saveValidationSummary.innerHTML += `<ul>${summary}</ul>`;
                }
                showError("Error al guardar movimientos", resp.mensaje);
                return;
            }

            // Acción al cerrar
            $('#ImportarMovimientosModal').modal('hide');

            // Refrescar la vista
            let e = document.querySelector("[name='refresh']");
            e.click();

            showSuccess("Movimientos guardados", resp.mensaje);
        },
        function (error) {
            showError("Error", error);
        },
        postOptions
    );
}

//Método para importar la información del excel y pdf
function onImportarMovimientosBancariosClick() {
    var fileUpload = document.getElementById('fileUpload');
    var selectedBankId = $('#selFiltroBanco').val(); // Obtener el ID del banco seleccionado

    if (fileUpload.files.length === 0) {
        alert('Por favor selecciona un archivo.');
        return;
    }

    if (selectedBankId === '0') {
        alert('Por favor selecciona un banco.');
        return;
    }

    // Almacena el ID del banco seleccionado en el campo oculto para enviarlo con el formulario
    $('#BancoSeleccionado').val(selectedBankId);

    var fileType = fileUpload.files[0].name.split('.').pop().toLowerCase();

    if (fileType === 'xlsx' || fileType === 'xls') {
        importarMovimientosDesdeExcel(fileUpload.files[0], selectedBankId);
        actualizarContadores();
        recargarTablas(false);
    } else if (fileType === 'pdf') {
        importarMovimientosDesdePDF(fileUpload.files[0], selectedBankId);
    } else {
        alert('Por favor selecciona un archivo Excel o PDF.');
    }
}

function importarMovimientosDesdeExcel(file, selectedBank) {
    var reader = new FileReader();
    reader.onload = function (e) {
        var data = new Uint8Array(e.target.result);
        var workbook = XLSX.read(data, { type: 'array' });

        // Leer la primera hoja del archivo Excel
        var firstSheet = workbook.Sheets[workbook.SheetNames[0]];
        var excelRows = XLSX.utils.sheet_to_json(firstSheet, { header: 1 });

        // Limpiar la tabla antes de insertar nuevos datos
        $('#tableCardMovimientos').bootstrapTable('removeAll');

        // Función para convertir los seriales de Excel a fechas en formato DD/MM/YYYY
        function excelDateToJSDate(serial) {
            var utc_days = Math.floor(serial - 25569); // Fecha base 1900
            var utc_value = utc_days * 86400; // Convertir días a segundos
            var date_info = new Date(utc_value * 1000); // Crear la fecha

            // Obtener día, mes y año
            var day = date_info.getUTCDate().toString().padStart(2, '0');
            var month = (date_info.getUTCMonth() + 1).toString().padStart(2, '0'); // Meses empiezan desde 0
            var year = date_info.getUTCFullYear();

            return `${day}/${month}/${year}`; // Formato DD/MM/YYYY
        }

        // Crear un array para almacenar las filas a insertar
        var rows = [];

        // Inicializar un contador de ID para autoincrementar
        var idCounter = 1;

        // Iterar sobre las filas del Excel, empezando desde la segunda fila
        for (var i = 1; i < excelRows.length; i++) {
            var row = excelRows[i]; // Obtener la fila actual

            // Verificar si el valor de la fecha es un número y convertirlo a fecha
            var fecha = row[0];
            if (!isNaN(fecha)) {
                fecha = excelDateToJSDate(fecha); // Convertir si es un número serial
            }

            // Agregar la fila al array de filas con ID autoincrementable
            rows.push({
                Id: idCounter++, // Asignar ID y luego incrementarlo
                Fecha: fecha || '',
                Banco: selectedBank, // Usar el valor seleccionado del banco
                Descripcion: row[1] || '',
                Cargos: row[2] || '',
                Abonos: row[3] || ''
            });
        }

        // Agregar los datos a la tabla usando Bootstrap Table
        $('#tableCardMovimientos').bootstrapTable('append', rows);

        // Cerrar el modal después de que los datos se hayan agregado
        $('#ImportarMovimientosModal').modal('hide');
    };

    // Leer el archivo Excel
    reader.readAsArrayBuffer(file);
}


function importarMovimientosDesdePDF(file, selectedBank) {
    var reader = new FileReader();

    reader.onload = function (e) {
        var typedArray = new Uint8Array(e.target.result);

        pdfjsLib.getDocument(typedArray).promise.then(function (pdf) {
            var numPages = pdf.numPages;
            var extractedText = '';
            var promises = [];

            for (var i = 1; i <= numPages; i++) {
                promises.push(pdf.getPage(i).then(function (page) {
                    return page.getTextContent().then(function (textContent) {
                        textContent.items.forEach(function (item) {
                            extractedText += item.str + ' ';
                        });
                    });
                }));
            }

            Promise.all(promises).then(function () {
                var bancoDetectado = detectarBanco(extractedText);
                if (bancoDetectado.toLowerCase() === selectedBank.toLowerCase()) {
                    var confirmation = confirm(`Banco detectado y seleccionado: ${bancoDetectado}. ¿Desea continuar?`);

                    if (confirmation) {
                        const processedData = [];
                        const lines = extractedText.split(/\r\n|\r|\n/);

                        let currentTraSection = '';
                        let currentDocSection = '';
                        let currentIntSection = '';
                        let insideTraSection = false;
                        let insideDocSection = false;
                        let insideIntSection = false;
                        let ignoreSection = false;

                        lines.forEach((line, index) => {
                            // Eliminar el saldo final del registro, si existe
                            line = line.replace(/\s\d{1,3}(?:,\d{3})*(\.\d{2})?\s*$/, '');

                            const conceptoIndex = line.indexOf("CONCEPTO");
                            if (conceptoIndex !== -1) {
                                line = line.slice(conceptoIndex);
                            }

                            if (ignoreSection) {
                                if (/TRA|DOC|INT/.test(line)) {
                                    ignoreSection = false;
                                } else {
                                    return;
                                }
                            }

                            if (line.includes("Régimen Fiscal del Emisor:") && line.includes(" Cliente :")) {
                                ignoreSection = false;
                                return;
                            }

                            let parts = line.split(/TRA|DOC|INT/);

                            parts.forEach((part, i) => {
                                if (i > 0) {
                                    if (line.includes("TRA")) {
                                        if (insideTraSection) processedData.push([currentTraSection.trim()]);
                                        currentTraSection = 'TRA' + part;
                                        insideTraSection = true;
                                        insideDocSection = false;
                                        insideIntSection = false;
                                    } else if (line.includes("DOC")) {
                                        if (insideDocSection) processedData.push([currentDocSection.trim()]);
                                        currentDocSection = 'DOC' + part;
                                        insideDocSection = true;
                                        insideTraSection = false;
                                        insideIntSection = false;
                                    } else if (line.includes("INT")) {
                                        if (insideIntSection) processedData.push([currentIntSection.trim()]);
                                        currentIntSection = 'INT' + part;
                                        insideIntSection = true;
                                        insideTraSection = false;
                                        insideDocSection = false;
                                    }
                                } else {
                                    if (insideTraSection) currentTraSection += ' ' + part;
                                    if (insideDocSection) currentDocSection += ' ' + part;
                                    if (insideIntSection) currentIntSection += ' ' + part;
                                }
                            });

                            const extractDayAndCleanLine = (section) => {
                                const match = section.match(/(\d{2})\s+\d+\.\d{2}/);
                                if (match) {
                                    const day = match[1];
                                    const cleanedSection = section.replace(/^\d{2}\s+/, '');
                                    return [day, cleanedSection];
                                }
                                return [null, section];
                            };

                            if (index === lines.length - 1) {
                                if (currentTraSection) {
                                    const [day, cleanedSection] = extractDayAndCleanLine(currentTraSection);
                                    if (day) processedData.push([day, cleanedSection.trim()]);
                                }
                                if (currentDocSection) {
                                    const [day, cleanedSection] = extractDayAndCleanLine(currentDocSection);
                                    if (day) processedData.push([day, cleanedSection.trim()]);
                                }
                                if (currentIntSection) {
                                    const [day, cleanedSection] = extractDayAndCleanLine(currentIntSection);
                                    if (day) processedData.push([day, cleanedSection.trim()]);
                                }
                            }
                        });

                        const worksheet = XLSX.utils.aoa_to_sheet(processedData);
                        const workbook = XLSX.utils.book_new();
                        XLSX.utils.book_append_sheet(workbook, worksheet, 'Data');
                        XLSX.writeFile(workbook, 'Banregio.xlsx');

                    } else {
                        console.log('El usuario ha cancelado la operación.');
                    }
                } else {
                    alert(`Banco detectado: ${bancoDetectado}, pero seleccionaste: ${selectedBank}. \nFavor de seleccionar el correcto.`);
                }
                console.log('Log: ' + extractedText);
            });
        });
    };
    reader.readAsArrayBuffer(file);
}

function detectarBanco(extractedText) {
    // Diccionario de bancos y sus palabras clave
    var bancoKeywords = {
        "Banregio": ["BANREGIO", "BANCO REGIONAL", "Banregio"],
        "BBVA": ["BBVA", "BANCO BBVA"],
        "Alquimia": ["Alquimia", "ALQUIMIA", "Alquimia Digital", "alquimiapay"]
    };

    // Recorrer cada banco y sus palabras clave
    for (var banco in bancoKeywords) {
        var keywords = bancoKeywords[banco];
        // Comprobar si alguna de las palabras clave está en el texto extraído
        for (var i = 0; i < keywords.length; i++) {
            if (extractedText.toLowerCase().includes(keywords[i].toLowerCase())) {
                return banco; // Retorna el banco detectado
            }
        }
    }

    return "Banco no identificado"; // Retorna esto si no se detecta ningún banco
}

function dividirConceptoPorPalabrasClave(concepto) {
    // Palabras clave a detectar
    const keywords = ["TRA", "INT", "DOC"];

    // Buscar cualquier palabra clave que esté en el concepto
    keywords.forEach(keyword => {
        const keywordIndex = concepto.indexOf(keyword);
        if (keywordIndex > 0) {
            // Insertar un salto de línea antes de la palabra clave
            concepto = concepto.slice(0, keywordIndex) + '\n' + concepto.slice(keywordIndex);
        }
    });

    return concepto;
}
function exportarToExcel(extractedText) {

    if (!extractedText || extractedText.trim() === '') {
        alert("No hay texto extraído para exportar.");
        return;
    }

    // Crear un objeto de libro de trabajo de Excel
    const workbook = XLSX.utils.book_new();
    const worksheetData = [];

    // Encabezados
    worksheetData.push(["COMERCIO"]);
    worksheetData.push(["Banregio"]);
    worksheetData.push([]);

    // Encabezados de cálculo
    worksheetData.push(["FECHA", "DESCRIPCIÓN", "CARGOS", "ABONOS"]);

    // Dividir el texto extraído en líneas
    const lines = extractedText.split(/\r\n|\r|\n/);
    const pattern = /\d{1,3}(?:,\d{3})*\.\d{2}/;

    let rowIndex = 6; // Empezar desde la segunda fila (la primera tiene los encabezados)
    let totalCargos = 0; // Equivalente a decimal en JS es número
    let totalAbonos = 0;
    let skipLines = false; // Bandera para saltar líneas tras encontrar "Page"
    let afterLastValidRecord = false; // Bandera para indicar si hemos encontrado el último registro válido
    let linesToSkip = 0; // Contador para líneas a ignorar

    for (let i = 0; i < lines.length; i++) {
        let currentLine = lines[i].trim();

        // Si ya encontramos el último registro válido y seguimos viendo contenido no relevante, ignorarlo
        if (afterLastValidRecord) {
            continue; // Saltar todo lo que está después del último registro válido
        }

        // Si encontramos una línea que contiene "Page", activamos la bandera para saltar las siguientes 5 líneas
        if (currentLine.includes("Page")) {
            skipLines = true;
            linesToSkip = 5; // Restablecemos las líneas a ignorar
            continue; // Saltar esta línea
        }

        // Si estamos saltando líneas, restamos del contador y continuamos hasta que lleguemos a 0
        if (skipLines && linesToSkip > 0) {
            linesToSkip--;
            continue; // Ignorar las líneas mientras linesToSkip sea mayor que 0
        }

        // Desactivar la bandera de saltar líneas una vez que hemos ignorado las primeras 5
        if (linesToSkip === 0) {
            skipLines = false;
        }

        // Evitar concatenar cualquier información que esté relacionada con pie de página
        if (currentLine.includes("Banco") ||
            currentLine.includes("ESTADO DE CUENTA") ||
            currentLine.includes("R.F.C") ||
            currentLine.includes("Centro de Atención") ||
            currentLine.includes("Cliente")) {
            continue; // Saltar estas líneas directamente
        }

        // Si la línea siguiente parece ser una continuación (no contiene cargos/abonos), unirla con la actual
        while (i + 1 < lines.length && !new RegExp(pattern).test(lines[i + 1])) {
            // Evitar concatenar si la línea contiene "Page" o alguna información irrelevante
            if (lines[i + 1].includes("Page") ||
                lines[i + 1].includes("Banco") ||
                lines[i + 1].includes("ESTADO DE CUENTA") ||
                lines[i + 1].includes("AV. PEDRO RAMIREZ VAZQUEZ 200 - 12 PARQUE CORPORATIVO UCALY") ||
                lines[i + 1].includes("R.F.C. BRM940216EQ6") ||
                lines[i + 1].includes("SAN PEDRO GARZA GARCIA N.L., C.P. 66278") ||
                /Cliente\s*:\s*\d{3}-\d{5}/.test(lines[i + 1]) ||
                lines[i + 1].includes("Centro de Atención 81-BANREGIO (22673446) Desde cualquier ciudad de la República Mexicana.") ||
                lines[i + 1].includes("COMERCIO LOGCAL FORTUNA S.A. DE C.V.") ||
                lines[i + 1].includes("DIA CONCEPTO CARGOS ABONOS SALDO") ||
                /del\s*\d{2}\s*al\s*\d{2}\s*de\s*\w+\s*\d{4}/.test(lines[i + 1]) ||
                /Corte\s*al\s*Día\s*\d{2}\s*-\s*\d{2}\s*Días/.test(lines[i + 1])
            ) {
                i++; // Saltar la línea no válida
                continue;
            }

            currentLine += " " + lines[i + 1].trim(); // Concatenar la línea siguiente
            i++; // Saltar a la siguiente línea
        }

        let matches = currentLine.match(new RegExp(pattern, 'g'));

        if (matches && matches.length >= 1) {
            // Asumimos que las últimas tres coincidencias son CARGOS, ABONOS y SALDO (puede haber algunas líneas sin abonos)
            let abonos = matches.length >= 2 ? matches[matches.length - 2] : "";
            let cargos = matches.length >= 3 ? matches[matches.length - 3] : "";

            // El concepto será todo lo que esté antes de los cargos
            let cargoIndex = currentLine.indexOf(cargos);
            let concepto = (cargoIndex > 0) ? currentLine.substring(0, cargoIndex).trim() : currentLine.trim();

            // Eliminar números con decimales y comas del concepto (números como 120.00, 1,000.00, etc.)
            concepto = concepto.replace(new RegExp(pattern, 'g'), "").trim();

            // Validar si el concepto contiene "TRA", "INT" o "DOC" y dividirlo si es necesario
            concepto = DividirConceptoPorPalabrasClave(concepto);

            // Declarar variables para el mes y año detectados en el PDF
            let mes = 0;  // Aquí deberías asignar el número del mes de acuerdo a lo detectado
            let year = 2024;  // Aquí deberías asignar el año detectado

            let pattern0 = /del\s*(\d{2})\s*al\s*(\d{2})\s*de\s*(\w+)\s*(\d{4})/i;

            // Simular la detección del mes y año desde el PDF utilizando Regex
            let match = extractedText.match(pattern0);

            if (match) {
                // Extraer el mes y año
                let mesString = match[3]; // El mes está en el grupo 3
                year = parseInt(match[4], 10); // El año está en el grupo 4

                if (!isNaN(year)) {
                    // Asignar el mes según el nombre extraído
                    switch (mesString.toUpperCase()) {
                        case "ENERO": mes = 1; break;
                        case "FEBRERO": mes = 2; break;
                        case "MARZO": mes = 3; break;
                        case "ABRIL": mes = 4; break;
                        case "MAYO": mes = 5; break;
                        case "JUNIO": mes = 6; break;
                        case "JULIO": mes = 7; break;
                        case "AGOSTO": mes = 8; break;
                        case "SEPTIEMBRE": mes = 9; break;
                        case "OCTUBRE": mes = 10; break;
                        case "NOVIEMBRE": mes = 11; break;
                        case "DICIEMBRE": mes = 12; break;
                    }
                }
            }

            // Si el concepto contiene el día al principio, extraer el día
            let dia = concepto.split(' ')[0]; // Primer valor como el día
            concepto = concepto.substring(dia.length).trim(); // Remover el día del concepto

            // Verificar si el valor de 'dia' es un número de dos dígitos y que no sea 'Total'
            if (/^\d{2}$/.test(dia)) {
                // Construir la fecha completa en formato DD/MM/YYYY
                let fechaCompleta = `${dia.padStart(2, '0')}/${mes.toString().padStart(2, '0')}/${year}`;

                // Colocar los valores en las celdas correspondientes
                worksheet.getRow(rowIndex).getCell(1).value = fechaCompleta; // DIA
                worksheet.getRow(rowIndex).getCell(2).value = concepto; // CONCEPTO

                if (concepto.includes("TRANSFER") && concepto.startsWith("TRA") && concepto.endsWith("TRASPASO ENTRE CUENTAS")) {
                    // Verificar y convertir CARGOS
                    let totalCargos = !isNaN(parseFloat(cargos)) ? parseFloat(cargos) : 0;

                    // Verificar y convertir ABONOS
                    let totalAbonos = !isNaN(parseFloat(abonos)) ? parseFloat(abonos) : 0;

                    worksheet.getRow(rowIndex).getCell(3).numFmt = "#,##0.00";
                    // Asignar el valor de CARGOS en la celda
                    worksheet.getRow(rowIndex).getCell(3).value = totalCargos;

                    // Sumar ABONOS a CARGOS
                    worksheet.getRow(rowIndex).getCell(3).value = totalAbonos; // Sumar directamente
                } else if (
                    concepto.endsWith("DISP DE REC") ||
                    concepto.endsWith("PRESTAMO") ||
                    concepto.endsWith("TRASPASO ENTRE CUENTAS") ||
                    concepto.endsWith("Deposito en efectivo") ||
                    concepto.endsWith("Devolucion") ||
                    concepto.endsWith("PAGO FACT") ||
                    concepto.endsWith("TRASPASO") ||
                    concepto.endsWith("PAGO FACTURA") ||
                    concepto.endsWith("SPEI: INVU143") ||
                    concepto.startsWith("INT SPEI-Devolucion") ||
                    concepto.startsWith("INT COM. SPEI-Devolucion") ||
                    concepto.startsWith("INT IVA SPEI-Devolucion") ||
                    concepto.endsWith("Interes pagado por SPEI") ||
                    concepto.endsWith("pago factura") ||
                    concepto.endsWith("TRASPASO MIG TER ABASTO Y COMERCIO S.A. DE C.V.") ||
                    concepto.endsWith("PAGO FACTURA MIG TER ABASTO Y COMERCIO S.A. DE C.V.")
                ) {
                    // Verificar y convertir CARGOS
                    let totalCargos = !isNaN(parseFloat(cargos)) ? parseFloat(cargos) : 0;

                    // Verificar y convertir ABONOS
                    let totalAbonos = !isNaN(parseFloat(abonos)) ? parseFloat(abonos) : 0;

                    worksheet.getRow(rowIndex).getCell(4).numFmt = "#,##0.00";
                    // Asignar CARGOS vacío
                    worksheet.getRow(rowIndex).getCell(3).value = "";
                    // Asignar ABONOS en la celda correspondiente
                    worksheet.getRow(rowIndex).getCell(4).value = totalAbonos;
                } else {
                    // Verificar y convertir CARGOS
                    let totalCargos = !isNaN(parseFloat(cargos)) ? parseFloat(cargos) : 0;

                    // Verificar y convertir ABONOS
                    let totalAbonos = !isNaN(parseFloat(abonos)) ? parseFloat(abonos) : 0;

                    worksheet.getRow(rowIndex).getCell(3).numFmt = "#,##0.00";
                    // Asignar el valor de CARGOS en la celda
                    worksheet.getRow(rowIndex).getCell(3).value = totalCargos;

                    // Sumar ABONOS a CARGOS
                    worksheet.getRow(rowIndex).getCell(3).value = totalAbonos;
                }

                rowIndex++; // Incrementar el índice de la fila solo si se inserta algo
            }
        }
    }

    // Agregar la hoja de trabajo al libro
    const worksheet = XLSX.utils.aoa_to_sheet(worksheetData);
    XLSX.utils.book_append_sheet(workbook, worksheet, "Estado de Cuenta");

    // Exportar el libro de trabajo a un archivo
    XLSX.writeFile(workbook, "Estado_Cuenta.xlsx");
}