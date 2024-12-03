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
                        if (bancoDetectado == "Bankaool" || bancoDetectado == "bankaool")
                        {
                            const datos = extraerDatosEspecificos(extractedText);

                            if (datos) {
                                // Retornar los datos extraídos para el archivo procesado
                                resolve([datos]); // Los datos se convierten en un arreglo
                            } else {
                                console.log("No se pudieron extraer los datos específicos.");
                                resolve([]); // Retorna un arreglo vacío si no se encuentran datos
                            }
                        }
                        if (bancoDetectado == "Eplata" || bancoDetectado == "EPlata")
                        {
                            const datos = extraerDatosEspecificosEplata(extractedText);

                            if (datos) {
                                // Retornar los datos extraídos para el archivo procesado
                                resolve(datos); // Los datos se convierten en un arreglo
                            } else {
                                console.log("No se pudieron extraer los datos específicos.");
                                resolve([]); // Retorna un arreglo vacío si no se encuentran datos
                            }
                        }
                        if (bancoDetectado == "Banbajio" || bancoDetectado == "BANBAJIO" || bancoDetectado == "Banbajío") {
                            const datos = extraerDatosEspecificosBanbajio(extractedText);

                            if (datos) {
                                // Retornar los datos extraídos para el archivo procesado
                                resolve(datos); // Los datos se convierten en un arreglo
                            } else {
                                console.log("No se pudieron extraer los datos específicos.");
                                resolve([]); // Retorna un arreglo vacío si no se encuentran datos
                            }
                        }
                    } else {
                        const mensajeModal = `Banco detectado: ${bancoDetectado}, pero seleccionaste: ${nombreBancoSeleccionado}. \nFavor de seleccionar el correcto.`;

                        // Llamar a la función para mostrar el mensaje en el modal
                        mostrarMensajeModal(mensajeModal);
                        resolve([]); // Retorna un arreglo vacío si el banco no coincide
                    }

                    //console.log('Texto extraído del PDF:', extractedText);
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
        "Banbajio": ["Banbajio", "banbajio", "Banbajío", "CUENTA CONECTA BANBAJIO"],
        "Banregio": ["BANREGIO", "BANCO REGIONAL", "Banregio"],
        "BBVA": ["BBVA", "BANCO BBVA"],
        "Alquimia": ["Alquimia", "ALQUIMIA", "Alquimia Digital", "alquimiapay"],
        "Bankaool": ["Bankaool", "BANKAOOL"],
        "Eplata": ["Eplata", "EPlata", "EPLATA"]
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

function extraerDatosEspecificosEplata(textoExtraido) {
    // Limitar el texto al contenido entre "DETALLE DE MOVIMIENTOS" e "Incumplir tus obligaciones"
    const inicio = textoExtraido.indexOf("DETALLE DE MOVIMIENTOS");
    const fin = textoExtraido.indexOf("Incumplir tus obligaciones");

    if (inicio === -1 || fin === -1 || inicio >= fin) {
        console.error("No se encontró el rango de texto esperado.");
        return [{
            FechaMovimiento: null,
            Concepto: null,
            Cargo: null,
            Abono: null,
            Saldo: null,
            Descripcion: null,
        }];
    }

    const textoFiltrado = textoExtraido.substring(inicio, fin).trim();
    console.log("Texto del pdf: ", textoExtraido);

    // Expresión regular para capturar el texto desde "PERIODO" hasta un año en formato de 4 dígitos
    const regexPeriodo = /PERIODO[\s\S]*?\b(\d{4})\b/i;
    const matchPeriodo = textoExtraido.match(regexPeriodo);

    if (!matchPeriodo) {
        console.error("No se encontró el período en el texto.");
        return [];
    }

    const year = matchPeriodo[1]; // Captura el año del período
    console.log("Año del período:", year);

    console.log("Texto filtrado para análisis:", textoFiltrado);

    // Extraer todas las fechas específicas "DD MMM"
    const regexFechas = /\b(\d{2})\s(ENE|FEB|MAR|ABR|MAY|JUN|JUL|AGO|SEP|OCT|NOV|DIC)\b/g;
    const fechasEncontradas = [];
    let matchFecha;

    const monthMap = {
        ENE: "01",
        FEB: "02",
        MAR: "03",
        ABR: "04",
        MAY: "05",
        JUN: "06",
        JUL: "07",
        AGO: "08",
        SEP: "09",
        OCT: "10",
        NOV: "11",
        DIC: "12"
    };

    while ((matchFecha = regexFechas.exec(textoFiltrado)) !== null) {
        const day = matchFecha[1]; // Captura el día
        const month = monthMap[matchFecha[2]]; // Mapea el mes al número correspondiente
        fechasEncontradas.push(`${day}/${month}/${year}`); // Formato DD/MM/YYYY
    }

    if (fechasEncontradas.length === 0) {
        console.warn("No se encontraron fechas específicas en el texto.");
    }

    console.log("Fechas específicas encontradas (formato DD/MM/YYYY):", fechasEncontradas);

    // Extraer registros de PAGO y cantidades (Cargos y Saldos)
    const regexMovimientos = /PAGO[\s\S]*?\|\d{18}\s+\$(\d{1,3}(?:,\d{3})*\.\d{2})\s+\$(\d{1,3}(?:,\d{3})*\.\d{2})/g;
    const registrosMovimiento = [];
    let matchMovimiento;

    while ((matchMovimiento = regexMovimientos.exec(textoFiltrado)) !== null) {
        let descripcion = matchMovimiento[0].trim();

        // Eliminar solo las cantidades que comienzan con $
        descripcion = descripcion
            .replace(/\$\d{1,3}(?:,\d{3})*\.\d{2}/g, '') // Eliminar cualquier cantidad que inicie con $
            .trim(); // Quitar espacios sobrantes

        const cantidad1 = matchMovimiento[1]; // Primer número
        const cantidad2 = matchMovimiento[2]; // Segundo número (Saldo)

        // Lógica para determinar si la cantidad1 es Cargo o Abono
        let cargo = "$0.00";
        let abono = "$0.00";
        if (descripcion.startsWith("PAGO FACTURA")) {
            cargo = cantidad1; // Si es PAGO FACTURA, se asigna a Cargo
        } else if (/^PAGO\s\d+/.test(descripcion)) {
            abono = cantidad1; // Si es PAGO seguido de un número, se asigna a Abono
        }

        registrosMovimiento.push({
            Descripcion: descripcion, // Capturar la descripción del PAGO sin las cantidades
            Cargo: cargo,
            Abono: abono,
            Saldo: cantidad2, // El segundo número siempre es el Saldo
        });
    }

    if (registrosMovimiento.length === 0) {
        console.warn("No se encontraron registros de movimientos con cargos y saldos.");
    }

    console.log("Registros de movimientos encontrados:", registrosMovimiento);

    // Alinear fechas, descripciones, cargos, abonos y saldos
    const maxLength = Math.max(fechasEncontradas.length, registrosMovimiento.length);
    const resultadoFinal = [];

    for (let i = 0; i < maxLength; i++) {
        const movimiento = registrosMovimiento[i] || { Descripcion: null, Cargo: "$0.00", Abono: "$0.00", Saldo: null };

        resultadoFinal.push({
            FechaMovimiento: fechasEncontradas[i] || null, // Si no hay más fechas, rellenar con null
            Concepto: null,
            Cargo: movimiento.Cargo,
            Abono: movimiento.Abono,
            Saldo: movimiento.Saldo,
            Descripcion: movimiento.Descripcion,
        });
    }

    console.log("Resultado final alineado:", resultadoFinal);

    return resultadoFinal;
}

function extraerDatosEspecificosBanbajio(textoExtraido) {
    // Limitar el texto al contenido de "DETALLE DE LA CUENTA"
    const inicio = textoExtraido.indexOf("DETALLE DE LA CUENTA");
    const fin = textoExtraido.lastIndexOf("CONTINUA EN LA SIGUIENTE PAGINA");

    if (inicio === -1 || fin === -1 || inicio >= fin) {
        console.error("No se encontró el rango de texto esperado.");
        return [];
    }

    // Recortar el texto inicial
    let textoFiltrado = textoExtraido.substring(inicio, fin).trim();

    console.log("Texto original filtrado:", textoFiltrado);

    // Expresión regular para capturar las referencias numéricas de 7 dígitos
    const regexReferencias = /\b\d{7}\b/g;
    const referencias = [];

    let match;
    while ((match = regexReferencias.exec(textoFiltrado)) !== null) {
        referencias.push(match[0]); // Agregar la referencia encontrada
    }

    console.log("Referencias encontradas:", referencias);

    // Expresión regular para capturar fechas
    const regexFechas = /\b\d{1,2}\s\w{3}\b/g;
    const fechasEncontradas = [];

    let matchFecha;
    while ((matchFecha = regexFechas.exec(textoFiltrado)) !== null) {
        fechasEncontradas.push({ FechaMovimiento: matchFecha[0] });
    }

    if (fechasEncontradas.length === 0) {
        console.warn("No se encontraron fechas en el texto.");
    }

    // Expresión regular para identificar registros completos por encabezado válido
    const regexRegistros = /(DEPÓSITO SPEI:|ENVÍO SPEI:|DEVOLUCIÓN DE SPEI:|TEF RECIBIDO:|TRASPASO DE RECURSOS).*?(?=(DEPÓSITO SPEI:|ENVÍO SPEI:|DEVOLUCIÓN DE SPEI:|TEF RECIBIDO:|TRASPASO DE RECURSOS|$))/gs;
    const registrosEncontrados = [];

    let matchRegistro;
    while ((matchRegistro = regexRegistros.exec(textoFiltrado)) !== null) {
        let detalle = matchRegistro[0].replace(/\s+/g, ' ').trim();

        // Si el registro contiene "BENEFICIARIO:" o "ORDENANTE:", eliminar todo desde "ESTADO DE CUENTA" hacia adelante
        if (detalle.includes("BENEFICIARIO:") || detalle.includes("ORDENANTE:")) {
            const indexEstadoCuenta = detalle.indexOf("ESTADO DE CUENTA");
            const indexSaldoTotal = detalle.indexOf("SALDO TOTAL*");
            if (indexEstadoCuenta !== -1) {
                detalle = detalle.substring(0, indexEstadoCuenta).trim();
            }
            if (indexSaldoTotal !== -1) {
                detalle = detalle.substring(0, indexSaldoTotal).trim();
            }
        }

        // Limpiar el texto no deseado de la descripción
        const textosNoDeseados = [
            "LA REPRODUCCION NO AUTORIZADA DE ESTE COMPROBANTE CONSTITUYE UN DELITO EN LOS TERMINOS DE LAS DISPOSICIONES FISCALES.",
            "CONTRIBUYENTE AUTORIZADO PARA IMPRIMIR SUS PROPIOS COMPROBANTES FISCALES.",
            "BANCO DEL BAJÍO, S.A., INSTITUCIÓN DE BANCA MÚLTIPLE RECIBE LAS CONSULTAS, RECLAMACIONES O ACLARACIONES EN SU UNIDAD ESPECIALIZADA DE ATENCIÓN A USUARIOS UBICADA EN BLVD. MANUEL J. CLOUTHIER 402, COL. JARDINES DEL CAMPESTRE, LEÓN, GTO. CP 37128 Y POR CORREO ELECTRÓNICO Y TELÉFONO 477 740 7875, ASÍ COMO CUALQUIERA DE SUS UNE@BB.COM.MX SUCURSALES U OFICINAS.",
            "EN EL CASO DE NO OBTENER UNA RESPUESTA SATISFACTORIA, PODRÁ ACUDIR A LA COMISIÓN NACIONAL PARA LA PROTECCIÓN Y DEFENSA DE LOS USUARIOS DE SERVICIOS FINANCIEROS EN SU PÁGINA DE INTERNET WWW.CONDUSEF.GOB.MX O EN SU CENTRO DE ATENCIÓN TELEFÓNICA EN EL D.F. Y ÁREA METROPOLITANA AL 5340-0999 Y EL RESTO DE LA REPÚBLICA MEXICANA AL 800 999 80 80.",
            "SI AL REVISAR SU"
        ];

        textosNoDeseados.forEach((texto) => {
            detalle = detalle.replace(texto, "").trim();
        });

        // Extraer montos con la expresión regular
        const regexMontos = /\$\s*[\d,]+\.\d{2}/g;
        const montos = detalle.match(regexMontos);

        // Determinar valores para Cargo, Abono, y Saldo
        let cargo = "$0.00";
        let abono = "$0.00";
        let saldo = "-";

        if (montos && montos.length >= 2) {
            saldo = montos[1].trim(); // El segundo monto es el Saldo
            if (detalle.startsWith("DEPÓSITO SPEI:")) {
                cargo = montos[0].trim(); // El primer monto es el Cargo
            } else if (detalle.startsWith("ENVÍO SPEI:")) {
                abono = montos[0].trim(); // El primer monto es el Abono
            } else if (detalle.startsWith("TEF RECIBIDO:")) {
                cargo = montos[0].trim();
            } else if (detalle.includes("BENEFICIARIO:")) {
                abono = montos[0].trim();
            } else if (detalle.includes("ORDENANTE:")) {
                cargo = montos[0].trim();
            }
            // Eliminar los montos del detalle del registro
            detalle = detalle.replace(montos[0], "").replace(montos[1], "").trim();
        }

        // Limpiar la descripción para que termine únicamente con "CLAVE DE RASTREO:"
        const regexClaveRastreo = /(CLAVE DE RASTREO:[A-Z0-9]{1,24}).*/;
        const matchClave = detalle.match(regexClaveRastreo);
        if (matchClave) {
            detalle = detalle.substring(0, detalle.indexOf(matchClave[1]) + matchClave[1].length);
        }

        registrosEncontrados.push({
            Descripcion: detalle,
            Cargo: cargo,
            Abono: abono,
            Saldo: saldo
        });
    }

    if (registrosEncontrados.length === 0) {
        console.warn("No se encontraron registros.");
    }

    console.log("Registros encontrados:", registrosEncontrados);

    // Combinar fechas, descripciones, montos, y referencias en un solo array
    const datosCombinados = fechasEncontradas.map((fecha, index) => {
        const registro = registrosEncontrados[index] || {};
        return {
            FechaMovimiento: fecha.FechaMovimiento,
            NumeroReferencia: referencias[index] || "Sin referencia", // Agregar la referencia correspondiente
            Descripcion: registro.Descripcion || "Sin descripción",
            Saldo: registro.Saldo || "-", // Segundo monto como Saldo
            Cargo: registro.Cargo || "$0.00", // Cargo asignado según la lógica
            Abono: registro.Abono || "$0.00"  // Abono asignado según la lógica
        };
    });

    // Llamar a la función para ajustar referencias si contienen (BI- )
    return procesarRegistrosConBI(datosCombinados);
}

function procesarRegistrosConBI(datosCombinados) {
    // Iterar sobre los registros para ajustar referencias si contienen (BI- )
    for (let i = 0; i < datosCombinados.length; i++) {
        // Verificar si la descripción contiene (BI- )
        if (datosCombinados[i].Descripcion.includes("(BI- )")) {
            // Si no es la última fila, mover la referencia a la siguiente fila
            if (i + 1 < datosCombinados.length) {
                datosCombinados[i + 1].NumeroReferencia = datosCombinados[i].NumeroReferencia;
            }
            // Vaciar la referencia de la fila actual
            datosCombinados[i].NumeroReferencia = "";
        }
    }
    return datosCombinados;
}
