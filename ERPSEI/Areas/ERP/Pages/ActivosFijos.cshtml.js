var table;
var buttonRemove;
var selections = [];
var dlg = null;
var dlgModal = null;

const NUEVO = 0;
const EDITAR = 1;
const VER = 2;
const postOptions = { headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() } }

/*document.addEventListener("DOMContentLoaded", function (event) {
    table = $("#table");
    buttonRemove = $("#remove");
    dlg = document.getElementById('dlgActivoFijo');
    dlgModal = new bootstrap.Modal(dlg, null);

    dlg.addEventListener('hidden.bs.modal', function (event) {
        onCerrarClick();
    });

    initTable();
});*/
document.addEventListener("DOMContentLoaded", function (event) {
    table = $("#table");
    buttonRemove = $("#remove");
    buttonExportAll = $("#exportAll");
    dlg = document.getElementById('dlgActivoFijo');

    if (dlg) {
        dlgModal = new bootstrap.Modal(dlg, null);

        dlg.addEventListener('hidden.bs.modal', function (event) {
            onCerrarClick();
        });
    } else {
        console.error("No se encontró el modal con id #dlgActivoFijo");
    }

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

    icons.push(`
    <li>
        <a class="dropdown-item qr-activo"
           href="#"
           title="Generar etiqueta QR">
            <i class="bi bi-qr-code"></i>
            Etiqueta QR
        </a>
    </li>
`);

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
    },
    'click .qr-activo': function (e, value, row, index) {
        mostrarEtiquetaQr(row);
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

function mostrarEtiquetaQr(row) {

    if (!row || !row.id) {
        return;
    }

    const qr =
        document.getElementById("etiquetaQrImagen");

    const folio =
        document.getElementById("etiquetaFolio");

    const descripcion =
        document.getElementById("etiquetaDescripcion");

    qr.src =
        `/ERP/ActivosFijos?handler=QrActivo&id=${row.id}`;

    folio.textContent =
        row.folio || "";

    descripcion.textContent =
        row.descripcion || "";

    const modalElement =
        document.getElementById("dlgEtiquetaQr");

    const modal =
        bootstrap.Modal.getOrCreateInstance(
            modalElement
        );

    modal.show();
}

function onAgregarClick() {
    initActivoFijoDialog(NUEVO, { id: "Nuevo", nombre: "" });
}

function obtenerExtensionArchivo(rutaArchivo) {
    if (!rutaArchivo) {
        return "";
    }

    const rutaLimpia = rutaArchivo
        .split("?")[0]
        .split("#")[0];

    const partes = rutaLimpia.split(".");

    return partes.length > 1
        ? partes.pop().toLowerCase()
        : "";
}

function limpiarVistaPreviaFactura() {
    const previewVacio =
        document.getElementById("facturaPreviewVacio");

    const previewImagen =
        document.getElementById("facturaPreviewImagen");

    const previewPdf =
        document.getElementById("facturaPreviewPdf");

    if (previewVacio) {
        previewVacio.style.display = "flex";
    }

    if (previewImagen) {
        previewImagen.style.display = "none";
        previewImagen.removeAttribute("src");
    }

    if (previewPdf) {
        previewPdf.style.display = "none";
        previewPdf.removeAttribute("src");
    }
}

function cargarVistaPreviaFactura(rutaArchivo) {
    const previewVacio =
        document.getElementById("facturaPreviewVacio");

    const previewImagen =
        document.getElementById("facturaPreviewImagen");

    const previewPdf =
        document.getElementById("facturaPreviewPdf");

    limpiarVistaPreviaFactura();

    if (!rutaArchivo) {
        return;
    }

    const extension =
        obtenerExtensionArchivo(rutaArchivo);

    if (previewVacio) {
        previewVacio.style.display = "none";
    }

    if (["jpg", "jpeg", "png"].includes(extension)) {
        previewImagen.src = rutaArchivo;
        previewImagen.style.display = "block";
        return;
    }

    if (extension === "pdf") {
        previewPdf.src =
            `${rutaArchivo}#page=1&toolbar=0&navpanes=0&scrollbar=0`;

        previewPdf.style.display = "block";
        return;
    }

    limpiarVistaPreviaFactura();
}

function mostrarVistaPreviaFactura(input) {
    const nombreArchivo =
        document.getElementById("facturaNombreArchivo");

    if (!input.files || input.files.length === 0) {
        nombreArchivo.textContent =
            "Ningún archivo seleccionado";

        limpiarVistaPreviaFactura();
        return;
    }

    const archivo = input.files[0];

    const extensionesPermitidas = [
        "application/pdf",
        "image/jpeg",
        "image/png"
    ];

    if (!extensionesPermitidas.includes(archivo.type)) {
        input.value = "";

        nombreArchivo.textContent =
            "Ningún archivo seleccionado";

        limpiarVistaPreviaFactura();

        showError(
            "Formato no permitido",
            "Solo se permiten archivos PDF, JPG, JPEG o PNG."
        );

        return;
    }

    const tamanioMaximo = 10 * 1024 * 1024;

    if (archivo.size > tamanioMaximo) {
        input.value = "";

        nombreArchivo.textContent =
            "Ningún archivo seleccionado";

        limpiarVistaPreviaFactura();

        showError(
            "Archivo demasiado grande",
            "La factura no puede superar los 10 MB."
        );

        return;
    }

    nombreArchivo.textContent = archivo.name;

    const rutaTemporal =
        URL.createObjectURL(archivo);

    cargarVistaPreviaFactura(rutaTemporal);
}

function limpiarVistaPreviaImagenActivo() {

    const previewVacio =
        document.getElementById(
            "imagenActivoPreviewVacio"
        );

    const previewImagen =
        document.getElementById(
            "imagenActivoPreview"
        );

    if (previewVacio) {
        previewVacio.style.display =
            "flex";
    }

    if (previewImagen) {
        previewImagen.style.display =
            "none";

        previewImagen.removeAttribute(
            "src"
        );
    }
}


function cargarVistaPreviaImagenActivo(rutaImagen) {

    const previewVacio =
        document.getElementById(
            "imagenActivoPreviewVacio"
        );

    const previewImagen =
        document.getElementById(
            "imagenActivoPreview"
        );

    limpiarVistaPreviaImagenActivo();

    if (!rutaImagen) {
        return;
    }

    if (previewVacio) {
        previewVacio.style.display =
            "none";
    }

    if (previewImagen) {
        previewImagen.src =
            rutaImagen;

        previewImagen.style.display =
            "block";
    }
}


function mostrarVistaPreviaImagenActivo(input) {

    const nombreArchivo =
        document.getElementById(
            "imagenActivoNombreArchivo"
        );

    if (!input.files ||
        input.files.length === 0) {

        nombreArchivo.textContent =
            "Ninguna imagen seleccionada";

        limpiarVistaPreviaImagenActivo();

        return;
    }

    const imagen =
        input.files[0];

    const extensionesPermitidas =
        [
            "image/jpeg",
            "image/png"
        ];

    if (!extensionesPermitidas.includes(
        imagen.type)) {
        input.value = "";

        nombreArchivo.textContent =
            "Ninguna imagen seleccionada";

        limpiarVistaPreviaImagenActivo();

        showError(
            "Formato no permitido",
            "Solo se permiten imágenes JPG, JPEG o PNG."
        );

        return;
    }

    const tamanioMaximo =
        10 * 1024 * 1024;

    if (imagen.size >
        tamanioMaximo) {
        input.value = "";

        nombreArchivo.textContent =
            "Ninguna imagen seleccionada";

        limpiarVistaPreviaImagenActivo();

        showError(
            "Imagen demasiado grande",
            "La imagen del activo no puede superar los 10 MB."
        );

        return;
    }

    nombreArchivo.textContent =
        imagen.name;

    const rutaTemporal =
        URL.createObjectURL(imagen);

    cargarVistaPreviaImagenActivo(
        rutaTemporal
    );
}

function facturaActivoFormatter(value, row, index) {
    const rutaArchivo = row.archivoAdjunto;

    if (!rutaArchivo) {
        return `
            <span class="text-muted small">
                <i class="bi bi-file-earmark-x"></i>
                Sin factura
            </span>
        `;
    }

    const extension = obtenerExtensionArchivo(rutaArchivo);

    if (["jpg", "jpeg", "png"].includes(extension)) {
        return `
            <a href="${rutaArchivo}"
               target="_blank"
               rel="noopener noreferrer"
               title="Ver factura completa">

                <img src="${rutaArchivo}"
                     alt="Factura del activo"
                     style="
                        width: 100px;
                        height: 70px;
                        object-fit: cover;
                        border: 1px solid #ddd;
                        border-radius: 5px;
                     ">
            </a>
        `;
    }

    if (extension === "pdf") {
        return `
            <a href="${rutaArchivo}"
               target="_blank"
               rel="noopener noreferrer"
               title="Abrir factura PDF"
               style="text-decoration:none;">

                <div style="
                    position:relative;
                    width:100px;
                    height:70px;
                    overflow:hidden;
                    border:1px solid #ddd;
                    border-radius:5px;
                    background:#fff;
                    margin:auto;
                ">
                    <iframe
                        src="${rutaArchivo}#page=1&toolbar=0&navpanes=0&scrollbar=0"
                        style="
                            width:200px;
                            height:140px;
                            border:0;
                            pointer-events:none;
                            transform:scale(.5);
                            transform-origin:top left;
                        ">
                    </iframe>

                    <span style="
                        position:absolute;
                        right:3px;
                        bottom:3px;
                        padding:1px 4px;
                        border-radius:3px;
                        background:#dc3545;
                        color:#fff;
                        font-size:10px;
                    ">
                        PDF
                    </span>
                </div>
            </a>
        `;
    }

    return `
        <a href="${rutaArchivo}"
           target="_blank"
           rel="noopener noreferrer"
           class="btn btn-sm btn-outline-primary">

            <i class="bi bi-paperclip"></i>
            Ver
        </a>
    `;
}
function initTable() {
    table.bootstrapTable('destroy').bootstrapTable({
        //url: '/ERP/ActivosFijos?handler=ActivosFijosList',
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
            /*{
                title: "Responsable",
                field: "responsable",
                align: "center",
                valign: "middle",
                sortable: true
            },*/
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
                title: "Oficina",
                field: "oficina",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "Cantidad",
                field: "cantidades",
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
                title: "Factura del Activo",
                field: "archivoAdjunto",
                align: "center",
                valign: "middle",
                sortable: false,
                width: "140px",
                clickToSelect: false,
                formatter: facturaActivoFormatter
            },
            {
                title: "Comentarios",
                field: "comentarios",
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


/////////////////////

//Funcionalidad Diálogo
function initActivoFijoDialog(action, row) {

    let idField =
        document.getElementById(
            "inpActivoFijoId"
        );

    let folioField =
        document.getElementById(
            "inpActivoFijoFolio"
        );

    let descripcionField =
        document.getElementById(
            "inpActivoFijoDescripcion"
        );

    let responsableField =
        document.getElementById(
            "inpActivoFijoResponsable"
        );

    let empleadoIdField =
        document.getElementById(
            "inpEmpleadoId"
        );

    let categoriaField =
        document.getElementById(
            "inpActivoFijoCategoria"
        );

    let tipoField =
        document.getElementById(
            "inpActivoFijoTipo"
        );

    let fechacompraField =
        document.getElementById(
            "inpActivoFijoFechaCompra"
        );

    let precioField =
        document.getElementById(
            "inpActivoFijoPrecio"
        );

    let marcaField =
        document.getElementById(
            "inpActivoFijoMarca"
        );

    let numeroSerieField =
        document.getElementById(
            "inpActivoFijoNumeroSerie"
        );

    let comentariosField =
        document.getElementById(
            "inpActivoFijoComentarios"
        );

    let archivoField =
        document.getElementById(
            "inpActivoFijoArchivo"
        );

    let imagenField =
        document.getElementById(
            "inpActivoFijoImagen"
        );

    let fechaRenovacionField =
        document.getElementById(
            "inpActivoFijoFechaRenovacion"
        );

    let cantidadesField =
        document.getElementById(
            "inpActivoFijoCantidad"
        );

    let oficinaField =
        document.getElementById(
            "inpActivoFijoOficina"
        );

    let btnGuardar =
        document.getElementById(
            "dlgActivoFijoBtnGuardar"
        );

    let dlgTitle =
        document.getElementById(
            "dlgActivoFijoTitle"
        );

    let summaryContainer =
        document.getElementById(
            "saveValidationSummary"
        );


    summaryContainer.innerHTML = "";


    // =====================================================
    // BORRADOR AUTOMÁTICO
    // =====================================================

    const formActivo =
        document.getElementById(
            "theForm"
        );


    const conservarBorrador =
        action === NUEVO &&
        formActivo &&
        formActivo.dataset.draftRestored ===
        "true";


    // =====================================================
    // VALIDAR OFICINA
    // =====================================================

    if (!oficinaField) {

        console.warn(
            "inpActivoFijoOficina NO se encontró en el HTML."
        );
    }


    idField.setAttribute(
        "disabled",
        true
    );


    // =====================================================
    // CONFIGURAR MODAL
    // =====================================================

    switch (action) {

        case NUEVO:

            dlgTitle.innerHTML =
                dlgNuevoTitle;

            idField.setAttribute(
                "disabled",
                true
            );

            folioField.setAttribute(
                "disabled",
                true
            );

            descripcionField.removeAttribute(
                "disabled"
            );

            responsableField.removeAttribute(
                "disabled"
            );

            categoriaField.removeAttribute(
                "disabled"
            );

            tipoField.removeAttribute(
                "disabled"
            );

            fechacompraField.removeAttribute(
                "disabled"
            );

            precioField.removeAttribute(
                "disabled"
            );

            marcaField.removeAttribute(
                "disabled"
            );

            numeroSerieField.removeAttribute(
                "disabled"
            );

            comentariosField.removeAttribute(
                "disabled"
            );

            archivoField.removeAttribute(
                "disabled"
            );

            imagenField.removeAttribute(
                "disabled"
            );

            fechaRenovacionField.removeAttribute(
                "disabled"
            );

            cantidadesField.removeAttribute(
                "disabled"
            );

            oficinaField.removeAttribute(
                "disabled"
            );

            btnGuardar.removeAttribute(
                "disabled"
            );

            break;


        case EDITAR:

            dlgTitle.innerHTML =
                dlgEditarTitle;

            idField.setAttribute(
                "disabled",
                true
            );

            folioField.setAttribute(
                "disabled",
                true
            );

            descripcionField.removeAttribute(
                "disabled"
            );

            responsableField.removeAttribute(
                "disabled"
            );

            categoriaField.removeAttribute(
                "disabled"
            );

            tipoField.removeAttribute(
                "disabled"
            );

            fechacompraField.removeAttribute(
                "disabled"
            );

            precioField.removeAttribute(
                "disabled"
            );

            marcaField.removeAttribute(
                "disabled"
            );

            numeroSerieField.removeAttribute(
                "disabled"
            );

            comentariosField.removeAttribute(
                "disabled"
            );

            archivoField.removeAttribute(
                "disabled"
            );

            imagenField.removeAttribute(
                "disabled"
            );

            fechaRenovacionField.removeAttribute(
                "disabled"
            );

            cantidadesField.removeAttribute(
                "disabled"
            );

            oficinaField.removeAttribute(
                "disabled"
            );

            btnGuardar.removeAttribute(
                "disabled"
            );

            break;


        default:

            dlgTitle.innerHTML =
                dlgVerTitle;

            idField.setAttribute(
                "disabled",
                true
            );

            folioField.setAttribute(
                "disabled",
                true
            );

            descripcionField.setAttribute(
                "disabled",
                true
            );

            responsableField.setAttribute(
                "disabled",
                true
            );

            categoriaField.setAttribute(
                "disabled",
                true
            );

            tipoField.setAttribute(
                "disabled",
                true
            );

            fechacompraField.setAttribute(
                "disabled",
                true
            );

            precioField.setAttribute(
                "disabled",
                true
            );

            marcaField.setAttribute(
                "disabled",
                true
            );

            numeroSerieField.setAttribute(
                "disabled",
                true
            );

            comentariosField.setAttribute(
                "disabled",
                true
            );

            archivoField.setAttribute(
                "disabled",
                true
            );

            imagenField.setAttribute(
                "disabled",
                true
            );

            fechaRenovacionField.setAttribute(
                "disabled",
                true
            );

            cantidadesField.setAttribute(
                "disabled",
                true
            );

            oficinaField.setAttribute(
                "disabled",
                true
            );

            btnGuardar.setAttribute(
                "disabled",
                true
            );

            break;
    }


    // =====================================================
    // CARGAR DATOS
    // =====================================================
    //
    // Si se recuperó un borrador de un ACTIVO NUEVO,
    // no sobrescribimos sus valores.
    // =====================================================

    if (!conservarBorrador) {

        idField.value =
            row.id ?? "";

        folioField.value =
            row.folio ?? "";

        descripcionField.value =
            row.descripcion ?? "";


        if (row.responsableId) {

            responsableField.value =
                row.responsableId.toString();

            empleadoIdField.value =
                row.responsableId;

        } else {

            responsableField.value =
                "";

            empleadoIdField.value =
                "0";
        }


        categoriaField.value =
            row.categoriaId ??
            row.categoria ??
            "";


        tipoField.value =
            row.tipoId ??
            row.tipo ??
            "";


        if (
            oficinaField &&
            row.oficinaId
        ) {

            oficinaField.value =
                row.oficinaId.toString();

        } else if (oficinaField) {

            oficinaField.value =
                "";
        }


        // =====================================================
        // FECHA COMPRA
        // =====================================================

        if (row.fechaCompra) {

            try {

                if (
                    row.fechaCompra
                        .includes("/")
                ) {

                    const [
                        dia,
                        mes,
                        anio
                    ] =
                        row.fechaCompra
                            .split("/");


                    fechacompraField.value =
                        `${anio}-${mes.padStart(2, "0")}-${dia.padStart(2, "0")}`;

                } else {

                    const fecha =
                        new Date(
                            row.fechaCompra
                        );


                    fechacompraField.value =
                        fecha
                            .toISOString()
                            .split("T")[0];
                }

            } catch (e) {

                fechacompraField.value =
                    "";
            }

        } else {

            fechacompraField.value =
                "";
        }


        precioField.value =
            row.precio ?? "";

        marcaField.value =
            row.marca ?? "";

        numeroSerieField.value =
            row.numeroSerie ?? "";

        comentariosField.value =
            row.comentarios ?? "";

        cantidadesField.value =
            row.cantidades ?? "";


        // =====================================================
        // FECHA RENOVACIÓN
        // =====================================================

        if (row.fechaRenovacion) {

            try {

                if (
                    row.fechaRenovacion
                        .includes("/")
                ) {

                    const [
                        dia,
                        mes,
                        anio
                    ] =
                        row.fechaRenovacion
                            .split("/");


                    fechaRenovacionField.value =
                        `${anio}-${mes.padStart(2, "0")}-${dia.padStart(2, "0")}`;

                } else {

                    const fecha =
                        new Date(
                            row.fechaRenovacion
                        );


                    fechaRenovacionField.value =
                        fecha
                            .toISOString()
                            .split("T")[0];
                }

            } catch (e) {

                fechaRenovacionField.value =
                    "";
            }

        } else {

            fechaRenovacionField.value =
                "";
        }

    }


    // =====================================================
    // FACTURA
    // =====================================================

    let archivoContainer =
        document.getElementById(
            "archivoActualContainer"
        );

    let archivoLink =
        document.getElementById(
            "archivoActualLink"
        );

    let nombreArchivo =
        document.getElementById(
            "facturaNombreArchivo"
        );


    if (archivoField) {

        archivoField.value =
            "";
    }


    if (
        !conservarBorrador &&
        row.archivoAdjunto &&
        row.archivoAdjunto.trim() !==
        ""
    ) {

        archivoContainer.style.display =
            "flex";

        archivoLink.href =
            row.archivoAdjunto;

        nombreArchivo.textContent =
            row.archivoAdjunto
                .split("/")
                .pop();


        cargarVistaPreviaFactura(
            row.archivoAdjunto
        );

    } else {

        archivoContainer.style.display =
            "none";

        archivoLink.href =
            "#";

        nombreArchivo.textContent =
            "Ningún archivo seleccionado";

        limpiarVistaPreviaFactura();
    }


    // =====================================================
    // IMAGEN DEL ACTIVO
    // =====================================================

    let imagenActualContainer =
        document.getElementById(
            "imagenActivoActualContainer"
        );

    let imagenActualLink =
        document.getElementById(
            "imagenActivoActualLink"
        );

    let imagenNombreArchivo =
        document.getElementById(
            "imagenActivoNombreArchivo"
        );


    if (imagenField) {

        imagenField.value =
            "";
    }


    if (
        !conservarBorrador &&
        row.imagenActivo &&
        row.imagenActivo.trim() !==
        ""
    ) {

        imagenActualContainer.style.display =
            "flex";

        imagenActualLink.href =
            row.imagenActivo;

        imagenNombreArchivo.textContent =
            row.imagenActivo
                .split("/")
                .pop();


        cargarVistaPreviaImagenActivo(
            row.imagenActivo
        );

    } else {

        imagenActualContainer.style.display =
            "none";

        imagenActualLink.href =
            "#";

        imagenNombreArchivo.textContent =
            "Ninguna imagen seleccionada";

        limpiarVistaPreviaImagenActivo();
    }


    // =====================================================
    // RESPONSABLE
    // =====================================================

    responsableField.onchange =
        function () {

            empleadoIdField.value =
                this.value || "0";
        };


    // =====================================================
    // ABRIR MODAL
    // =====================================================

    dlgModal.show();
}


async function obtenerFolioDesdeServidor() {
    const response = await fetch('/ERP/ActivosFijos?handler=ObtenerSiguienteFolio');
    const data = await response.json();
    return data.folio;
}

async function onAgregarClick() {
    const siguienteFolio = await obtenerFolioDesdeServidor();

    initActivoFijoDialog(NUEVO, {
        id: "Nuevo",
        folio: siguienteFolio,
        descripcion: "",
        responsable: "",
        categoria: null,
        tipo: null,
        fechaCompra: "",
        precio: "",
        linkFacturaCompra: "",
        marca: "",
        numeroSerie: "",
        //ubicacion: "",
        oficina: null,
        comentarios: "",
        fechaRenovacion: "",
        cantidades: ""
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
    $("#theForm").validate();
    let valid = $("#theForm").valid();
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

    let marcaField = document.getElementById("inpActivoFijoMarca");
    let numeroSerieField = document.getElementById("inpActivoFijoNumeroSerie");
    let comentariosField = document.getElementById("inpActivoFijoComentarios");
    let fechaRenovacionField = document.getElementById("inpActivoFijoFechaRenovacion");
    let cantidadesField = document.getElementById("inpActivoFijoCantidad");

    let empleadoIdField = document.getElementById("inpEmpleadoId");
    let oficinaField = document.getElementById("inpActivoFijoOficina");
    let archivoField = document.getElementById("inpActivoFijoArchivo");
    let imagenField =
        document.getElementById(
            "inpActivoFijoImagen"
        );

    let dlgTitle = document.getElementById("dlgActivoFijoTitle");
    let summaryContainer = document.getElementById("saveValidationSummary");
    summaryContainer.innerHTML = "";

    $("#inpEmpleadoId").val(responsableField.value);

    let formData = new FormData();

    formData.append("id", idField.value === "Nuevo" ? 0 : parseInt(idField.value));
    formData.append("folio", folioField.value);
    formData.append("descripcion", descripcionField.value);
    formData.append("responsable", responsableField.value);
    formData.append("empleadoId", parseInt(empleadoIdField?.value || 0));
    formData.append("oficina", oficinaField.value);
    formData.append("categoria", categoriaField.value);
    formData.append("tipo", tipoField.value);
    formData.append("fechaCompra", fechacompraField.value);
    formData.append("precio", parseFloat(precioField.value) || 0);
    formData.append("marca", marcaField.value);
    formData.append("numeroSerie", numeroSerieField.value);
    formData.append("comentarios", comentariosField.value);
    formData.append("fechaRenovacion", fechaRenovacionField.value);
    formData.append("cantidades", parseInt(cantidadesField?.value || 0));

    if (archivoField && archivoField.files.length > 0) {
        formData.append("archivo", archivoField.files[0]);
    }

    if (
        imagenField &&
        imagenField.files.length > 0
    ) {
        formData.append(
            "imagen",
            imagenField.files[0]
        );
    }

    $.ajax({
        url: "/ERP/ActivosFijos/SaveActivoFijo",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        headers: {
            "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (resp) {
            if (resp.tieneError) {
                if (Array.isArray(resp.errores) && resp.errores.length >= 1) {
                    let summary = ``;
                    resp.errores.forEach(function (error) {
                        summary += `<li>${error}</li>`;
                    });
                    summaryContainer.innerHTML = `<ul>${summary}</ul>`;
                }

                showError(dlgTitle.innerHTML, resp.mensaje);
                return;
            }

            // =====================================================
            // BORRAR BORRADOR DESPUÉS DEL GUARDADO EXITOSO
            // =====================================================

            if (
                window.IntranetDrafts &&
                typeof window.IntranetDrafts.clearById ===
                "function"
            ) {
                window.IntranetDrafts.clearById(
                    "theForm"
                );
            }

            btnClose.click();
            document.querySelector("[name='refresh']").click();
            showSuccess(dlgTitle.innerHTML, resp.mensaje);
        },
        error: function (xhr) {
            console.error("Error al guardar:", {
                status: xhr.status,
                statusText: xhr.statusText,
                responseText: xhr.responseText,
                responseJSON: xhr.responseJSON
            });

            let mensaje =
                xhr.responseJSON?.mensaje ||
                xhr.responseJSON?.title ||
                xhr.responseText ||
                "Ocurrió un error al guardar el activo fijo.";

            showError("Error", mensaje);
        }
    });
}

function onBuscarClick() {
    let btnBuscar = document.getElementById("btnBuscar");
    let inpFolio = document.getElementById("inpFiltroFolio");
    let inpResponsable = document.getElementById("inpFiltroResponsable");
    let selCategoria = document.getElementById("selFiltroCategoria");
    let selTipo = document.getElementById("selFiltroTipo");
    let selOficina = document.getElementById("selFiltroOficina");
    let inpFechaInicio = document.getElementById("inpFiltroFechaCompraInicio");
    let inpFechaFin = document.getElementById("inpFiltroFechaCompraFin");
    let inpEstatus = document.getElementById("inpFiltroEstatus");

    let oParams = {
        folio: inpFolio.value.trim() || null,
        responsable: inpResponsable.value.trim() || null,
        categoriaId: selCategoria.value === "0" || selCategoria.value === "" ? null : parseInt(selCategoria.value),
        tipoId: selTipo.value === "0" || selTipo.value === "" ? null : parseInt(selTipo.value),
        oficinaId: selOficina.value === "0" || selOficina.value === ""
            ? null
            : parseInt(selOficina.value),
        fechaCompraInicio: inpFechaInicio.value || null,
        fechaCompraFin: inpFechaFin.value || null,
        estatus: inpEstatus.value.trim() || null
    };

    doAjax(
        "/ERP/ActivosFijos/FiltrarActivosFijos",
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
            //$("#table").bootstrapTable("load", responseHandler(JSON.parse(resp.datos)));
        },
        function (error) {
            showError("Error", error);
        },
        postOptions
    );

    // Resetea el valor de los filtros después de la solicitud.
    document.querySelectorAll("#filtros .form-control").forEach(function (e) { e.value = ""; });
    //document.querySelectorAll("#filtros .form-select").forEach(function (e) { e.value = 0; });

}
$(document).ready(function () {
    autoCompletar("#inpFiltroResponsable");
});

//Método para exportar todos los campos y registros en excel del backend
function exportarActivosFijos() {
    window.location.href = "/ERP/ActivosFijos?handler=ExportarActivosFijos";
}

//Función para el importado del archivo con información de empleados
function onImportarClick() {
    //Ejecuta la validación
    $("#importForm").validate();
    //Determina los errores
    let valid = $("#importForm").valid();
    //Si la forma no es válida, entonces finaliza.
    if (!valid) { return; }

    let form = new FormData();
    let btnClose = document.getElementById("dlgExcelBtnCancelar");
    let dlgTitle = document.getElementById("dlgExcelTitle");
    let fileField = document.getElementById("excelFile");
    fileField = fileField != null ? fileField.files : null;

    if (fileField) { fileField = fileField.length > 0 ? fileField[0] : null; }

    if (fileField) { form.append("plantilla", fileField); }

    let extendedOptions = {
        headers: postOptions.headers,
        data: form,
        contentType: false,
        processData: false
    }

    doAjax(
        "/ERP/ActivosFijos/ImportarActivosFijos",
        {},
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

            onBuscarClick();

            showSuccess(dlgTitle.innerHTML, resp.mensaje);
        },
        function (error) {
            showError("Error", error);
        },
        extendedOptions
    );
}

//Función para el cierre del cuadro de diálogo
function onCerrarImportarClick() {
    let fileField = document.getElementById("excelFile");
    fileField.value = null;
    onCerrarClick();
}

//Función para procesar el cambio de archivo a exportar
function onExcelSelectorChanged(input) {
    //Validación para seleccionar archivos excel solamente.
    if (input.files && (input.files.length || 0) >= 1) {
        let docType = input.files[0].type;
        let isExcel = docType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" || docType == "application/vnd.ms-excel";

        if (!isExcel) {
            input.value = null;
            showAlert(invalidFormatTitle, invalidFormatMsg);
        }
    }
}

function imprimirEtiquetaActivo() {

    const etiqueta =
        document.getElementById("etiquetaActivo");

    if (!etiqueta) {
        showError(
            "Imprimir etiqueta",
            "No se encontró la etiqueta del activo."
        );

        return;
    }

    const qrImagen =
        document.getElementById("etiquetaQrImagen");

    if (!qrImagen ||
        !qrImagen.src) {

        showError(
            "Imprimir etiqueta",
            "El código QR aún no está disponible."
        );

        return;
    }

    const contenidoEtiqueta =
        etiqueta.outerHTML;

    const ventanaImpresion =
        window.open(
            "",
            "_blank",
            "width=700,height=700"
        );

    if (!ventanaImpresion) {

        showError(
            "Imprimir etiqueta",
            "El navegador bloqueó la ventana de impresión."
        );

        return;
    }

    ventanaImpresion.document.write(`
        <!DOCTYPE html>
        <html lang="es">
        <head>

            <meta charset="UTF-8">

            <title>
                Etiqueta Activo Fijo
            </title>

            <style>

                @page {
                    size: 60mm 80mm;
                    margin: 0;
                }

                html,
                body {
                    margin: 0;
                    padding: 0;

                    width: 60mm;
                    height: 80mm;

                    font-family:
                        Arial,
                        Helvetica,
                        sans-serif;

                    background: #ffffff;
                }

                body {
                    display: flex;
                    justify-content: center;
                    align-items: center;
                }

                .etiqueta-activo {
                    width: 54mm;

                    box-sizing: border-box;

                    padding: 4mm;

                    background: #ffffff;

                    border: 0.6mm solid #21166f;

                    border-radius: 3mm;

                    text-align: center;
                }

                .etiqueta-titulo {
                    color: #21166f;

                    font-size: 10pt;
                    font-weight: 900;

                    margin-bottom: 1mm;
                }

                .etiqueta-subtitulo {
                    color: #555555;

                    font-size: 7pt;
                    font-weight: 700;

                    margin-bottom: 3mm;
                }

                .etiqueta-qr {
                    display: block;

                    width: 32mm;
                    height: 32mm;

                    object-fit: contain;

                    margin:
                        0 auto
                        3mm auto;
                }

                .etiqueta-folio {
                    color: #21166f;

                    font-size: 16pt;
                    font-weight: 900;

                    margin-bottom: 2mm;
                }

                .etiqueta-descripcion {
                    color: #111111;

                    font-size: 8pt;
                    font-weight: 600;

                    line-height: 1.3;

                    word-break: break-word;
                }

            </style>

        </head>

        <body>

            ${contenidoEtiqueta}

        </body>

        </html>
    `);

    ventanaImpresion.document.close();

    ventanaImpresion.focus();

    // Esperar a que el QR termine de cargar
    ventanaImpresion.onload =
        function () {

            setTimeout(
                function () {

                    ventanaImpresion.print();

                    ventanaImpresion.close();

                },
                300
            );
        };
}

// =========================================================
// BORRADOR AUTOMÁTICO - ACTIVOS FIJOS
// =========================================================

document.addEventListener(
    "erpsei:draft-restored",

    function (event) {

        if (
            event.detail?.form?.id !==
            "theForm"
        ) {
            return;
        }


        showSuccess(
            "Borrador recuperado",
            "Se restauró la información del activo fijo que estabas capturando antes de que finalizara tu sesión."
        );
    }
);