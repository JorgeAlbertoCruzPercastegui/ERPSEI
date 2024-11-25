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
    $('#table').bootstrapTable('destroy').bootstrapTable({
        height: 550,
        locale: cultureName,
        exportDataType: 'all',
        exportTypes: ['excel'],
        toolbar: '#toolbar',
        showColumns: true,
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
                field: "NumeroReferencia",
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

function cargarDatosExtraidosPDF(datos) {
    // Inicializa o actualiza la tabla con los datos extraídos
    $('#table').bootstrapTable('load', datos);
}

function mostrarMensajeModal(mensaje) {
    // Insertar el mensaje en el cuerpo del modal
    document.getElementById('modalMensajeBody').innerText = mensaje;

    // Mostrar el modal
    $('#mensajeModal').modal('show');
}

function cerrarModal() {
    // Usar jQuery para ocultar el modal
    $('#mensajeModal').modal('hide');
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
        const mensajeModal = `Por favor selecciona al menos un archivo.`;
        mostrarMensajeModal(mensajeModal);
        return;
    }

    if (selectedBankId === '0') {
        const mensajeModal = `Por favor selecciona un banco.`;
        mostrarMensajeModal(mensajeModal);
        return;
    }

    // Almacena el ID del banco seleccionado en el campo oculto para enviarlo con el formulario
    $('#BancoSeleccionado').val(selectedBankId);

    // Arreglo para acumular los datos de todos los PDFs
    let registros = [];

    // Iterar sobre todos los archivos seleccionados
    for (let i = 0; i < fileUpload.files.length; i++) {
        const file = fileUpload.files[i];
        const fileType = file.name.split('.').pop().toLowerCase();

        if (fileType === 'pdf') {
            try {
                // Procesar cada archivo PDF y agregar los registros a la tabla
                importarMovimientosDesdePDF(file, selectedBankId).then(data => {
                    registros = registros.concat(data); // Acumular los datos extraídos
                    if (i === fileUpload.files.length - 1) {
                        // Una vez procesados todos los archivos, cargar los datos en la tabla
                        cargarDatosExtraidosPDF(registros);
                    }
                });
            } catch (error) {
                console.error(`Error al procesar el archivo ${file.name}:`, error);
            }
        } else {
            const mensajeModal = `El archivo "${file.name}" no es un archivo PDF válido.`;
            mostrarMensajeModal(mensajeModal);
        }
    }
}

function importarMovimientosDesdePDF(file, selectedBank) {
    var reader = new FileReader();

    return new Promise((resolve, reject) => {
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
                        // Crear el mensaje del modal
                        const mensajeModal = `Banco detectado correctamente: ${bancoDetectado}`;

                        // Llamar a la función para mostrar el mensaje en el modal
                        mostrarMensajeModal(mensajeModal);

                        // Llamar a la función para extraer los datos específicos
                        const datos = extraerDatosEspecificos(extractedText);

                        if (datos) {
                            // Retornar los datos extraídos para el archivo procesado
                            resolve([datos]); // Los datos se convierten en un arreglo
                        } else {
                            console.log("No se pudieron extraer los datos específicos.");
                            resolve([]); // Retorna un arreglo vacío si no se encuentran datos
                        }
                    } else {
                        const mensajeModal = `Banco detectado: ${bancoDetectado}, pero seleccionaste: ${nombreBancoSeleccionado}. \nFavor de seleccionar el correcto.`;

                        // Llamar a la función para mostrar el mensaje en el modal
                        mostrarMensajeModal(mensajeModal);
                        resolve([]); // Retorna un arreglo vacío si el banco no coincide
                    }

                    console.log('Texto extraído del PDF:', extractedText);
                }).catch(error => {
                    console.error("Error al procesar las páginas del PDF:", error);
                    reject(error);
                });
            }).catch(error => {
                console.error("Error al cargar el documento PDF:", error);
                reject(error);
            });
        };

        reader.onerror = function (e) {
            reject(e);
        };

        reader.readAsArrayBuffer(file);
    });
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

    return "Banco no identificado";
}

function extraerDatosEspecificos(textoExtraido) {
    // Expresión regular para capturar las fechas, el número de 7 dígitos, el texto y los valores con $
    const regex = /(\d{2}\/\d{2}\/\d{4})\s+(\d{2}\/\d{2}\/\d{4})\s+(\d{7})\s+([\s\S]*?)\s+(\$\s?\d{1,3}(?:,\d{3})*\.\d{2})\s+(\$\s?\d{1,3}(?:,\d{3})*\.\d{2})/;
    const match = regex.exec(textoExtraido);

    if (!match) {
        console.error("No se encontraron los datos específicos.");
        return null;
    }

    // Extraer los datos capturados por la expresión regular
    const fechaMovimiento = match[1];
    const fechaAplicacion = match[2];
    const numeroReferencia = match[3];
    const descripcion = match[4].replace(/\s{2,}/g, " ").trim(); // Reemplaza espacios múltiples por uno solo
    let cargo = match[5];
    let abono = match[6];
    let saldo = match[6]; // Por defecto, el saldo será igual al último valor extraído

    // Validar si la descripción contiene "Abono" o "ABONO"
    if (/abono/i.test(descripcion)) {
        cargo = "$ 0.00"; // Asignar $ 0.00 en lugar de dejar vacío
        abono = match[5]; // Mover el valor de cargo a abono
        saldo = match[6]; // Mantener el saldo igual al último valor capturado
    } else {
        cargo = match[5]; // Mantener el valor del cargo original
        abono = "$ 0.00"; // Dejar el abono en $ 0.00
        saldo = match[6]; // Mantener el saldo igual al segundo valor
    }

    // Retornar los datos extraídos en un objeto
    return {
        FechaMovimiento: fechaMovimiento,
        FechaAplicacion: fechaAplicacion,
        NumeroReferencia: numeroReferencia,
        Descripcion: descripcion,
        Cargo: cargo || "$ 0.00", // Si Cargo está vacío, asignar $ 0.00
        Abono: abono,
        Saldo: saldo,
    };
}