var table;
var buttonRemove;
var tableActividad;
var selections = [];
var dlgConciliacion = null;
var dlgConciliacionModal = null;

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
document.addEventListener("DOMContentLoaded", function (event) {
    table = $("#table");
    buttonRemove = $("#remove");
    dlgConciliacion = document.getElementById('dlgConciliacion');
    dlgConciliacionModal = new bootstrap.Modal(dlgConciliacion, {});
    dlgConciliacion.addEventListener('hidden.bs.modal', function (event) {
        onCerrarClick();
    });
    initTable();

    autoCompletar("#inpConciliacionClienteId");

    /*jQuery.validator.setDefaults({
        highlight: function (element, errorClass, validClass) {
            $(element).addClass("is-invalid").removeClass("is-valid");
        },
        unhighlight: function (element, errorClass, validClass) {
            if ($(element).hasClass("is-invalid")) {
                $(element).addClass("is-valid").removeClass("is-invalid");
            }
        }
    });*/
});

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

/*function responseHandler(res) {
    if (typeof res == "string" && res.length >= 1) {
        res = JSON.parse(res);
    }
    $.each(res, function (i, row) {
        row.state = $.inArray(row.id, selections) !== -1;
    });

    return res
}*/

function responseHandler(res) {
    if (typeof res == "string" && res.length >= 1) {
        res = JSON.parse(res);
    }

    // Verifica si res es un array o si es un objeto con un array dentro
    if (!Array.isArray(res) && res.length === undefined && res[0] !== undefined) {
        res = res[0]; // O el nombre de la propiedad si el array está anidado
    }

    $.each(res, function (i, row) {
        // Verifica que 'row' y 'row.Id' no sean null o undefined
        if (row && row.id != null) {
            row.state = $.inArray(row.id, selections) !== -1;
        } else {
            console.warn("Elemento sin Id encontrado:", row);
        }
    });

    return res;
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
        initConciliacionDialog(VER, row);
    },
    'click .edit': function (e, value, row, index) {
        initConciliacionDialog(EDITAR, row);
    }
}
function onAgregarClick() {
    initConciliacionDialog(NUEVO, { id: "Nuevo", nombre: "" });
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
                title: colUsuarioModificoHeader,
                field: "UsuarioModificador",
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

//Funcionalidad Diálogo Conciliación
function initConciliacionDialog(action, row) {
    // Obtener los campos del formulario
    let idField = document.getElementById("inpConciliacionId");
    let fechaField = document.getElementById("inpConciliacionFecha");
    let clienteIdField = document.getElementById("inpConciliacionClienteId");
    let descripcionField = document.getElementById("inpConciliacionDescripcion");
    let dlgTitle = document.getElementById("dlgConciliacionTitle");

    //Botones
    let btnGuardar = document.getElementById("dlgConciliacionBtnGuardar");
    //let btnCancelar = document.getElementById("dlgConciliacionBtnCancelar");
    //let btnComprobantesFechas = document.getElementById("dlgConciliacionBtnFechas");
    //let btnBtnMovimientosImportar = document.getElementById("dlgConciliacionBtnMovimientos");
    //let btnComprobantes = document.getElementById("dlgConciliacionBtnGuardar");
    let summaryContainer = document.getElementById("saveValidationSummary");
    summaryContainer.innerHTML = "";

    idField.setAttribute("disabled", true);

    switch (action) {
        case NUEVO:
            dlgTitle.innerHTML = dlgNuevoTitle;

            idField.setAttribute("disabled", true);
            fechaField.removeAttribute("disabled");
            clienteIdField.removeAttribute("disabled");
            descripcionField.removeAttribute("disabled");
            btnGuardar.removeAttribute("disabled");
            break;
        case EDITAR:
            dlgTitle.innerHTML = dlgEditarTitle;

            fechaField.removeAttribute("disabled");
            clienteIdField.removeAttribute("disabled");
            descripcionField.removeAttribute("disabled");
            btnGuardar.removeAttribute("disabled");
            break;
        default:
            dlgTitle.innerHTML = dlgVerTitle;

            fechaField.setAttribute("disabled", true);
            clienteIdField.setAttribute("disabled", true);
            descripcionField.setAttribute("disabled", true);
            btnGuardar.setAttribute("disabled", true);
            break;
    }

    // Extraer la parte de la fecha (YYYY-MM-DD) y verificar si el campo de fecha es de tipo "date"
    let fechaFormateada = "";
    if (row.Fecha) {
        let fecha = row.Fecha.split(' ')[0]; // Tomar solo la fecha (YYYY-MM-DD)
        fechaFormateada = fecha;  // Mantener el formato YYYY-MM-DD para campos de tipo date
    }

    // Asignar valores a los campos del diálogo usando los valores de la entidad Conciliacion
    idField.value = row.id || "";
    fechaField.value = fechaFormateada || "";
    clienteIdField.value = row.Cliente || "";
    descripcionField.value = row.Descripcion || "";

    // Mostrar el modal
    dlgConciliacionModal.show();
}
function onGuardarConciliacionClick() {
    // Ejecuta la validación del formulario con el id "theForm"
    $("#theForm").validate();
    // Determina si la validación fue exitosa
    let valid = $("#theForm").valid();
    // Si el formulario no es válido, termina la ejecución
    if (!valid) { return; }

    let btnClose = document.getElementById("dlgConciliacionBtnCancelar");
    let fechaField = document.getElementById("inpConciliacionFecha");
    let clienteIdField = document.getElementById("inpConciliacionClienteId");
    let descripcionField = document.getElementById("inpConciliacionDescripcion");
    let idField = document.getElementById("inpConciliacionId"); // Añadido: Obtener el idField
    let dlgTitle = document.getElementById("dlgConciliacionTitle");
    let summaryContainer = document.getElementById("saveValidationSummaryConciliacion");
    summaryContainer.innerHTML = "";

    // Parámetros que se enviarán en la solicitud
    let oParams = {
        id: idField.value == "Nuevo" ? 0 : idField.value,
        descripcion: descripcionField.value,
        fecha: fechaField.value,
        clienteId: clienteIdField.value
    };

    // Llamada AJAX para guardar los datos
    doAjax(
        "/ERP/Conciliaciones/SaveConciliacion",
        oParams,
        function (resp) {
            if (resp.tieneError) {
                // Si hay errores, los muestra en el resumen de validación
                if (Array.isArray(resp.errores) && resp.errores.length >= 1) {
                    let summary = ``;
                    resp.errores.forEach(function (error) {
                        summary += `<li>${error}</li>`;
                    });
                    summaryContainer.innerHTML += `<ul>${summary}</ul>`;
                }
                // Muestra el error general
                showError(dlgTitle.innerHTML, resp.mensaje);
                return;
            }

            // Cierra el diálogo al terminar
            btnClose.click();

            // Actualiza la tabla para reflejar los cambios
            let e = document.querySelector("[name='refresh']");
            e.click();

            // Muestra mensaje de éxito
            showSuccess(dlgTitle.innerHTML, resp.mensaje);
        }, function (error) {
            // Maneja errores de la solicitud AJAX
            showError("Error", error);
        },
        postOptions
    );
}

//Funciones para el cardview del lado izquierdo del principal modal
function actionFormatter(value, row, index) {
    return `
        <button class="btn btn-primary btn-sm" onclick="eliminarRegistro(${row.Id})">
            <i class="bi bi-paperclip rotate-clip"></i> Conciliar/Reconciliar
        </button>
    `;
}
function eliminarRegistro(Id) {
    if (confirm('¿Estás seguro de eliminar el registro con ID: ' + Id + '?')) {
        alert('Registro eliminado con éxito.');
        // Lógica para eliminar el registro
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
                    summaryContainer.innerHTML = `<ul>${summary}</ul>`;
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



//Método para importar la información del excel y pdf
function onImportarMovimientosBancariosClick() {
    var fileUpload = document.getElementById('fileUpload');
    var selectedBank = $('#selFiltroBanco option:selected').text();

    if (fileUpload.files.length === 0) {
        alert('Por favor selecciona un archivo.');
        return;
    }

    if (selectedBank === 'Seleccione...') {
        alert('Por favor selecciona un banco.');
        return;
    }

    var fileType = fileUpload.files[0].name.split('.').pop().toLowerCase();

    if (fileType === 'xlsx' || fileType === 'xls') {
        importarMovimientosDesdeExcel(fileUpload.files[0], selectedBank);
    } else if (fileType === 'pdf') {
        importarMovimientosDesdePDF(fileUpload.files[0], selectedBank);
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

        // Iterar sobre las filas del Excel, empezando desde la segunda fila
        for (var i = 1; i < excelRows.length; i++) {
            var row = excelRows[i]; // Obtener la fila actual

            // Verificar si el valor de la fecha es un número y convertirlo a fecha
            var fecha = row[0];
            if (!isNaN(fecha)) {
                fecha = excelDateToJSDate(fecha); // Convertir si es un número serial
            }

            // Agregar la fila al array de filas
            rows.push({
                Fecha: fecha || '',
                Banco: selectedBank, // Usar el valor seleccionado del banco
                Descripción: row[1] || '',
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

        // Utilizar pdfjs-dist para leer el archivo PDF
        pdfjsLib.getDocument(typedArray).promise.then(function (pdf) {
            var numPages = pdf.numPages;
            var extractedText = '';

            // Leer todas las páginas del PDF
            var promises = [];
            for (var i = 1; i <= numPages; i++) {
                promises.push(pdf.getPage(i).then(function (page) {
                    return page.getTextContent().then(function (textContent) {
                        textContent.items.forEach(function (item) {
                            extractedText += item.str + ' '; // Concatenar el texto extraído
                        });
                    });
                }));
            }

            // Esperar a que todas las páginas se hayan procesado
            Promise.all(promises).then(function () {
                // Detectar el banco en el texto extraído
                var bancoDetectado = detectarBanco(extractedText);

                // Comparar el banco detectado con el banco seleccionado
                if (bancoDetectado.toLowerCase() === selectedBank.toLowerCase()) {
                    var confirmation = confirm(`Banco detectado y seleccionado: ${bancoDetectado}. ¿Desea continuar?`);

                    if (confirmation) {
                        // Si el usuario hace clic en "Aceptar", vamos a exportar los datos del pdf al Cardview
                        // Aquí puedes mostrar el texto extraído en un alert
                        //alert("Texto extraído del PDF:\n" + extractedText); // Mostrar el texto extraído
                        exportToTxt(extractedText); // Llama a la función pasando el texto extraído
                    } else {
                        // Si el usuario hace clic en "Cancelar"
                        console.log('El usuario ha cancelado la operación.');
                        // Aquí puedes detener o deshacer alguna operación si es necesario.
                    }
                }
                else {
                    alert(`Banco detectado: ${bancoDetectado}, pero seleccionaste: ${selectedBank}. \nFavor de seleccionar el correcto.`);
                }

                // Aquí puedes continuar con el procesamiento del PDF si se detecta el banco.
                console.log('Log: ' + extractedText); // Mostrar el texto extraído para depuración
            });
        });
    };

    reader.readAsArrayBuffer(file); // Leer el archivo PDF como ArrayBuffer
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
// Función para exportar texto extraído a un archivo .txt
function exportToTxt(extractedText) {
    if (!extractedText || extractedText.trim() === '') {
        alert("No hay texto extraído para exportar.");
        return;
    }

    // Crear un array para almacenar las líneas procesadas
    const processedData = [];

    // Dividir el texto extraído en líneas
    const lines = extractedText.split(/\r\n|\r|\n/);

    let currentTraSection = '';
    let currentDocSection = '';
    let currentIntSection = '';
    let insideTraSection = false; // Para saber si estamos en una sección TRA
    let insideDocSection = false; // Para saber si estamos en una sección DOC
    let insideIntSection = false; // Para saber si estamos en una sección INT
    let ignoreSection = false; // Para saber si debemos ignorar líneas (después de "Régimen Fiscal del Emisor:")

    lines.forEach((line, index) => {
        // Ignorar lo que hay antes de la palabra "CONCEPTO"
        const conceptoIndex = line.indexOf("CONCEPTO");
        if (conceptoIndex !== -1) {
            // Si se encuentra "CONCEPTO", eliminar lo que hay antes
            line = line.slice(conceptoIndex);
        }

        // Comprobar si estamos en la sección a ignorar
        if (ignoreSection) {
            // Si encontramos "TRA", "DOC" o "INT", salir de la sección a ignorar
            if (/TRA|DOC|INT/.test(line)) {
                ignoreSection = false; // Dejar de ignorar
            } else {
                return; // Ignorar la línea actual
            }
        }

        // Verificar si encontramos "Régimen Fiscal del Emisor:"
        if (line.includes("Régimen Fiscal del Emisor:")) {
            ignoreSection = true; // Iniciar el estado de ignorar
            return; // Ignorar la línea actual
        }

        // Buscar todas las ocurrencias de "TRA", "DOC" e "INT" en la línea
        let parts = line.split(/TRA|DOC|INT/);

        parts.forEach((part, i) => {
            if (i > 0) {
                if (line.includes("TRA")) {
                    if (insideTraSection) {
                        // Si estamos en una sección TRA, agregar lo acumulado
                        processedData.push(currentTraSection.trim());
                    }
                    // Agregar "TRA" antes de continuar
                    currentTraSection = 'TRA' + part; // Iniciar la nueva sección con "TRA"
                    insideTraSection = true; // Comenzamos una nueva sección TRA
                    insideDocSection = false; // Asegurarnos de que no estamos en DOC
                    insideIntSection = false; // Asegurarnos de que no estamos en INT
                } else if (line.includes("DOC")) {
                    if (insideDocSection) {
                        // Si estamos en una sección DOC, agregar lo acumulado
                        processedData.push(currentDocSection.trim());
                    }
                    // Agregar "DOC" antes de continuar
                    currentDocSection = 'DOC' + part; // Iniciar la nueva sección con "DOC"
                    insideDocSection = true; // Comenzamos una nueva sección DOC
                    insideTraSection = false; // Asegurarnos de que no estamos en TRA
                    insideIntSection = false; // Asegurarnos de que no estamos en INT
                } else if (line.includes("INT")) {
                    if (insideIntSection) {
                        // Si estamos en una sección INT, agregar lo acumulado
                        processedData.push(currentIntSection.trim());
                    }
                    // Agregar "INT" antes de continuar
                    currentIntSection = 'INT' + part; // Iniciar la nueva sección con "INT"
                    insideIntSection = true; // Comenzamos una nueva sección INT
                    insideTraSection = false; // Asegurarnos de que no estamos en TRA
                    insideDocSection = false; // Asegurarnos de que no estamos en DOC
                }
            } else {
                // Si estamos en una sección TRA, seguir concatenando
                if (insideTraSection) {
                    currentTraSection += ' ' + part;
                }
                // Si estamos en una sección DOC, seguir concatenando
                if (insideDocSection) {
                    currentDocSection += ' ' + part;
                }
                // Si estamos en una sección INT, seguir concatenando
                if (insideIntSection) {
                    currentIntSection += ' ' + part;
                }
            }
        });

        // Si es la última línea, agregar las últimas secciones
        if (index === lines.length - 1) {
            if (currentTraSection) {
                processedData.push(currentTraSection.trim());
            }
            if (currentDocSection) {
                processedData.push(currentDocSection.trim());
            }
            if (currentIntSection) {
                processedData.push(currentIntSection.trim());
            }
        }
    });

    // Unir todas las líneas procesadas en una sola cadena con doble salto de línea
    const outputText = processedData
        .map((line, idx) => `${idx + 1}.- ${line}`) // Añadir número secuencial
        .join('\n');

    // Crear un Blob para el archivo de texto
    const blob = new Blob([outputText], { type: 'text/plain' });

    // Crear un URL para el Blob
    const url = URL.createObjectURL(blob);

    // Crear un elemento <a> para descargar el archivo automáticamente
    const a = document.createElement('a');
    a.href = url;
    a.download = 'Banregio.txt';

    // Simular el clic en el enlace para iniciar la descarga
    a.click();

    // Revocar el objeto URL para liberar memoria
    URL.revokeObjectURL(url);
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


