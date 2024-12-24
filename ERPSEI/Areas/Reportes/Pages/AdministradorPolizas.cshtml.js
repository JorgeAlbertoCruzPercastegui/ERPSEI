var table;
var buttonRemove;
var selections = [];
var dlg = null;
var dlgModal = null;

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

document.addEventListener("DOMContentLoaded", function () {
    table = $("#table");
    dlg = document.getElementById('dlgPoliza');
    dlgModal = new bootstrap.Modal(dlg, null);
    //Función para limpiar el cuadro de diálogo cuando es cerrado
    dlg.addEventListener('hidden.bs.modal', function (event) {
        onCerrarClick();
    });

    initTable();
    onObtenerRegistrosClick();
});

//Función para procesar la respuesta del servidor al consultar datos
function responseHandler(res) {
    if (typeof res === "string" && res.length >= 1) {
        res = JSON.parse(res);
    }

    $.each(res, function (i, row) {
        row.state = $.inArray(row.Id, selections) !== -1;
    });

    return res;
}


//Función para añadir botones a la cinta de botones de la tabla
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

function operateFormatter(value, row, index) {
    let icons = [];

    // Icono Ver
    icons.push(`<li><a class="dropdown-item see" href="#" title="${btnVerTitle}"><i class="bi bi-search"></i> ${btnVerTitle}</a></li>`);

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
        initPolizaDialog(VER, row); // Lógica para "Ver"
    },
    'click .export': function (e, value, row, index) {
        exportarAExcel(row.Id); // Llamar al método exportarAExcel
    }
};

       
function initTable() {
    table.bootstrapTable('destroy').bootstrapTable({
        height: 550,
        locale: cultureName,
        exportDataType: 'all',
        exportTypes: ['excel'],
        toolbar: '#toolbar',
        showColumns: true,
        columns: [
            {
                title: colIdHeader,
                field: "Id",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colFechaCreacionHeader,
                field: "FechaHoraCreacion",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colFechaModificacionHeader,
                field: "FechaHoraModificacion",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colUsuarioCreadorHeader,
                field: "UsuarioCreador",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colUsuarioModificadorHeader,
                field: "UsuarioModificador",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colPrintNumberHeader,
                field: "NumeroImpresion",
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
    });
}


//Función para filtrar los datos de la tabla.
function onBuscarClick() {
    let btnBuscar = document.getElementById("btnBuscar");
    let inpId = document.getElementById("inpFiltroId");
    let inpUsuarioCreador = document.getElementById("inpFiltroUsuarioCreador");
    let inpUsuarioModificador = document.getElementById("inpFiltroUsuarioModificador");
    let inpFechaInicio = document.getElementById("inpFiltroFechaInicio");
    let inpFechaFin = document.getElementById("inpFiltroFechaFin");

    let oParams = {
        Id: inpId.value ? parseInt(inpId.value) || null : null,
        FechaCreacion: inpFechaInicio.value || null,
        FechaModificacion: inpFechaFin.value || null,
        UsuarioCreador: inpUsuarioCreador.value || null,
        UsuarioModificador: inpUsuarioModificador.value || null
    };

    doAjax(
        "/Reportes/AdministradorPolizas/FiltrarPolizas",
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

function onObtenerRegistrosClick() {
    doAjax(
        "/Reportes/AdministradorPolizas/Polizas",
        null,
        function (resp) {
            console.log("Respuesta recibida:", resp);

            if (resp.tieneError) {
                showError("Error", resp.mensaje);
                return;
            }

            // Extraer el valor de resp.datos
            let datos = resp.datos && resp.datos.value ? resp.datos.value : [];

            if (typeof datos === "string") {
                datos = JSON.parse(datos);
            }

            // Asegúrate de que la tabla está inicializada antes de cargar los datos
            if (!table.data('bootstrap.table')) {
                table.bootstrapTable();
            }

            table.bootstrapTable('load', responseHandler(datos));
        },
        function (error) {
            showError("Error", error);
        },
        getOptions
    );
}

//Funcionalidad Diálogo
function initPolizaDialog(action, row) {
    let polizaIdField = document.getElementById("inpPolizaId");
    let polizaUsuarioCreadorField = document.getElementById("inpPolizaUsuarioCreador");
    let polizaUsuarioModificadorField = document.getElementById("inpPolizaUsuarioModificador");
    let polizaFechaCreacionField = document.getElementById("inpPolizaFechaCreacion");
    let polizaFechaModificacionField = document.getElementById("inpPolizaFechaModificacion");
    let btnGuardar = document.getElementById("dlgPolizaBtnGuardar");
    let dlgTitle = document.getElementById("dlgPolizaTitle");
    let summaryContainer = document.getElementById("saveValidationSummary");
    summaryContainer.innerHTML = "";

    polizaIdField.setAttribute("disabled", true);
    polizaUsuarioCreadorField.setAttribute("disabled", true);
    polizaUsuarioModificadorField.setAttribute("disabled", true);
    polizaFechaCreacionField.setAttribute("disabled", true);
    polizaFechaModificacionField.setAttribute("disabled", true);

    switch (action) {
        case NUEVO:
            dlgTitle.innerHTML = dlgNuevoTitle;

            polizaUsuarioCreadorField.removeAttribute("disabled");
            polizaUsuarioModificadorField.removeAttribute("disabled");
            polizaFechaCreacionField.removeAttribute("disabled");
            polizaFechaModificacionField.removeAttribute("disabled");
            btnGuardar.removeAttribute("disabled");
            break;
        case EDITAR:
            dlgTitle.innerHTML = dlgEditarTitle;

            polizaUsuarioCreadorField.removeAttribute("disabled");
            polizaUsuarioModificadorField.removeAttribute("disabled");
            polizaFechaCreacionField.removeAttribute("disabled");
            polizaFechaModificacionField.removeAttribute("disabled");
            btnGuardar.removeAttribute("disabled");
            break;
        default:
            dlgTitle.innerHTML = dlgVerTitle;

            polizaUsuarioCreadorField.removeAttribute("disabled");
            polizaUsuarioModificadorField.removeAttribute("disabled");
            polizaFechaCreacionField.removeAttribute("disabled");
            polizaFechaModificacionField.removeAttribute("disabled");

            polizaUsuarioCreadorField.setAttribute("disabled", true);
            polizaUsuarioModificadorField.setAttribute("disabled", true);
            polizaFechaCreacionField.setAttribute("disabled", true);
            polizaFechaModificacionField.setAttribute("disabled", true);
            btnGuardar.setAttribute("disabled", true);
            break;
    }

    polizaIdField.value = row.Id;
    polizaUsuarioCreadorField.value = row.UsuarioCreador;
    polizaUsuarioModificadorField.value = row.UsuarioModificador;
    polizaFechaCreacionField.value = row.FechaHoraCreacion;
    polizaFechaModificacionField.value = row.FechaHoraModificacion;

    dlgModal.toggle();
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

async function exportarAExcel(conciliacionId) {
    let oParams = {
        id: conciliacionId
    };
    $.extend(postOptions, { type: 'GET' });

    let dlgTitle = "Resultado de exportación de Excel";
    let saveValidationSummary = document.getElementById("saveValidationSummary");
    saveValidationSummary.innerHTML = "";

    // Realizar la llamada AJAX para exportar el Excel
    doAjax(
        "/Reportes/AdministradorPolizas/PolizasConsolidado",
        oParams,
        async function (resp) {
            if (resp.tieneError) {
                showError("Error", "No se pudo exportar a Excel.");
                return;
            }
            // Llamar a la función para generar y descargar el Excel
            await generarExcel(resp.datos, conciliacionId, dlgTitle, resp.mensaje);
        },
        function (error) {
            showError("Error", "No se pudo exportar a Excel.");
        },
        postOptions
    );
}

async function generarExcel(datos, conciliacionId, dlgTitle, mensaje) {
    const ExcelJS = window.ExcelJS;
    const workbook = new ExcelJS.Workbook();
    const worksheet = workbook.addWorksheet('Conciliación');

    // Ajustes en el diseño inicial
    worksheet.getCell('A3').value = 'lg';
    worksheet.getCell('A3').font = { bold: true };
    worksheet.getCell('B3').value = 1;
    worksheet.getCell('B3').font = { bold: true };

    // Recopila las cuentas contables ingresadas
    const cuentasContables = obtenerCuentasContables();
    console.log("Cuentas a exportar:", cuentasContables);

    // Genera el Excel usando las cuentas contables
    cuentasContables.forEach((cuenta, index) => {
        const row = 7 + index; // Empieza en la fila 7
        worksheet.getCell(`B${row}`).value = cuenta.substring(0, 12);
    });

    // Extraer solo el día de la fecha y colocarlo en la celda D3 en negritas
    const fechaEmisor = datos.length > 0 ? datos[0].fecha : 'N/A';
    const dia = fechaEmisor !== 'N/A' ? new Date(fechaEmisor).getDate() : 'N/A';
    worksheet.getCell('D3').value = dia;
    worksheet.getCell('D3').font = { bold: true };

    // Obtener el nombre del receptor, serie y folio
    const nombreReceptor = datos.length > 0 ? datos[0].nombreReceptor : 'N/A';
    const serie = datos.length > 0 ? datos[0].serie : 'N/A';
    const folio = datos.length > 0 ? datos[0].folio : 'N/A';

    // Concatenar el nombre del receptor con la serie y el folio
    const nombreCompleto = `${nombreReceptor} ${serie}-F-${folio}`;
    ['D4', 'D5', 'D6', 'D7'].forEach(cell => {
        worksheet.getCell(cell).value = nombreCompleto;
    });

    // Concatenar "INGRESOS" con el nombre del receptor, la serie y el folio
    const ingresosTexto = `INGRESOS ${nombreReceptor} ${serie}-F-${folio}`;
    worksheet.getCell('C3').value = ingresosTexto;
    worksheet.getCell('C3').font = { bold: true };

    // Aplicar color azul claro a las celdas A3 y B3
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

    // Colocar el valor de CuentaContable en la celda B4
    const cuentaContable = datos.length > 0 ? datos[0].cuentaContable : 'N/A';
    worksheet.getCell('B4').value = cuentaContable;

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
    const totalCargos = datos.reduce((sum, dato) => sum + (dato.cargos || 0), 0);
    worksheet.getCell('F4').value = totalCargos;

    // Obtener el TotalImpuestosTrasladados desde los datos
    const totalImpuestosTrasladados = datos.length > 0 ? datos[0].totalImpuestosTrasladados : 0;
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

    // Crear el archivo Excel y descargarlo
    const buffer = await workbook.xlsx.writeBuffer();
    const blob = new Blob([buffer], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = `Conciliacion_${conciliacionId}.xlsx`;
    link.click();
    URL.revokeObjectURL(link.href);

    // Mostrar mensaje de éxito
    showSuccess(dlgTitle, mensaje);
}

function obtenerCuentasContables() {
    const cuentas = [];
    $('#modalAsignacionCuentasBody').find('tr').each(function () {
        // Busca la celda de "Cuenta Bancaria"
        const cuenta = $(this).find('td:nth-child(3)').text().trim(); // Toma el texto de la tercera columna
        cuentas.push(cuenta || '0000-000-000'); // Agrega el valor o 'N/A' si está vacío
    });
    return cuentas;
}

