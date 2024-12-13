var table;

const NUEVO = 0;
const EDITAR = 1;
const VER = 2;
const maxFileSizeInBytes = 5242880; //5mb = (5 * 1024) * 1024;
const oneMegabyteSizeInBytes = 1048576; // 1mb = (1 * 1024) * 1024
const postOptions = {
    headers: {
        "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
    }
};

document.addEventListener("DOMContentLoaded", function () {
    table = $("#table");
    initTable();

    let btnBuscar = document.getElementById("btnBuscar");
    if (btnBuscar) {
        btnBuscar.addEventListener("click", onBuscarClick);
        btnBuscar.click();
    } else {
        console.warn("El botón con id 'btnBuscar' no se encontró en el DOM.");
    }
});


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

//Función para dar formato a los iconos de operación de los registros
function operateFormatter(value, row, index) {
    let icons = [];

    //Icono Ver
    //icons.push(`<li><a class="dropdown-item see" href="#" title="${btnVerTitle}"><i class="bi bi-search"></i> ${btnVerTitle}</a></li>`);
    //Icono Editar
    icons.push(`<li><a class="dropdown-item edit" href="#" title="${btnEditarTitle}"><i class="bi bi-pencil-fill"></i> ${btnEditarTitle}</a></li>`);

    return `<div class="dropdown">
              <button class="btn" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                <i class="bi bi-three-dots-vertical success"></i>
              </button>
              <ul class="dropdown-menu">${icons.join("")}</ul>
            </div>`;
}
       

function initTable() {
    $('#table').bootstrapTable('destroy').bootstrapTable({
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
                field: "FechaCreacion",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colFechaModificacionHeader,
                field: "FechaModificacion",
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
        FechaCreacion: inpUsuarioCreador.value || null,
        FechaModificacion: inpUsuarioModificador.value || null,
        UsuarioCreador: inpFechaInicio.value || null,
        UsuarioModificador: inpFechaFin.value || null
    };

    doAjax(
        "/ERP/AdministradorPolizas/FiltrarPolizas",
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

