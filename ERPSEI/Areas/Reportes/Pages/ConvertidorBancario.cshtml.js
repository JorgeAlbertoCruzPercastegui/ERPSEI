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

document.addEventListener("DOMContentLoaded", function (event) {
    table = $("#table");
    initTable();
});

function initTable() {
    table.bootstrapTable('destroy').bootstrapTable({
        height: 550,
        locale: cultureName,
        exportDataType: 'all',
        exportTypes: ['excel'],
        toolbar: '#toolbar', // Asegúrate de que este ID coincida con el elemento HTML donde quieres que aparezcan los botones
        columns: [
            {
                title: colFechaMovimientoHeader,
                field: "FechaMovimiento",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colFechaAplicacionHeader,
                field: "FechaAplicacion",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colReferenciaHeader,
                field: "Referencia",
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
                title: colCargoHeader,
                field: "Cargo",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colAbonoHeader,
                field: "Abono",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colSaldoHeader,
                field: "Saldo",
                align: "center",
                valign: "middle",
                sortable: true
            }
        ]
    });
}
async function onImportarMovimientosBancariosClick(event) {
    const file = event.target.files[0];
    if (file) {
        try {
            // Verificar que el archivo sea un PDF
            if (file.type !== 'application/pdf') {
                alert('El archivo seleccionado no es un PDF.');
                return;
            }

            // Lógica adicional para manejar el archivo PDF (lectura, procesamiento, etc.)
            console.log('Archivo PDF válido seleccionado:', file.name);

        } catch (error) {
            console.error('Error al leer el archivo:', error);
        }
    }
}

// Método para importar la información desde PDF
function onImportarMovimientosBancariosClick() {
    var fileUpload = document.getElementById('fileUpload');
    var selectedBankId = $('#selFiltroBanco').val();

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

    if (fileType === 'pdf') {
        importarMovimientosDesdePDF(fileUpload.files[0], selectedBankId);
    } else {
        alert('Por favor selecciona un archivo PDF.');
    }
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

                // Obtener el nombre del banco seleccionado desde el select
                var nombreBancoSeleccionado = $('#selFiltroBanco option:selected').text().trim();

                if (bancoDetectado.toLowerCase() === nombreBancoSeleccionado.toLowerCase()) {
                    alert(`Banco detectado correctamente: ${bancoDetectado}`);
                } else {
                    alert(`Banco detectado: ${bancoDetectado}, pero seleccionaste: ${nombreBancoSeleccionado}. \nFavor de seleccionar el correcto.`);
                }
                console.log('Texto extraído del PDF:', extractedText);
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
        "Alquimia": ["Alquimia", "ALQUIMIA", "Alquimia Digital", "alquimiapay"],
        "Bankaool": ["Bankaool", "BANKAOOL"]
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
