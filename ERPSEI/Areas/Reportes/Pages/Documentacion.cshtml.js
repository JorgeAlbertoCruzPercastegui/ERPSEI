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
    dlg = document.getElementById('dlgDocumentacion');

    if (dlg) {
        dlgModal = new bootstrap.Modal(dlg, null);

        dlg.addEventListener('hidden.bs.modal', function (event) {
            onCerrarClick();
        });
    } else {
        console.error("No se encontró el modal con id #dlgDocumentacion");
    }

    initTable();

    bindCodigoAuto();
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
        initDocumentacionDialog(VER, row);
    },
    'click .edit': function (e, value, row, index) {
        initDocumentacionDialog(EDITAR, row);
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
    initDocumentacionDialog(NUEVO, { id: "Nuevo", nombre: "" });
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
                title: "Area",
                field: "area",
                align: "center",
                valign: "middle",
                sortable: true,
                width: "80px"
            },
            {
                title: "TipoDocumento",
                field: "tipoDocumento",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "Titulo",
                field: "titulo",
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
            
            selections = getIdSelections()
        })
    table.on('all.bs.table', function (e, name, args) {
        console.log(name, args)
    })
    buttonRemove.click(function () {
        askConfirmation(dlgDeleteTitle, dlgDeleteQuestion, function () {
            let oParams = { ids: selections };

            doAjax(
                "/Reportes/Documentacion/DeleteDocumentacion",
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

function initDocumentacionDialog(action, row) {

    let idField = document.getElementById("inpDocumentacionId");
    let areaField = document.getElementById("inpDocumentacionArea");
    let tipoDocumentoField = document.getElementById("inpDocumentacionTipoDocumento");
    let estatusField = document.getElementById("inpDocumentacionEstatusDocumento");
    let tituloField = document.getElementById("inpDocumentacionTitulo");

    // ✅ SELECT (Descripción)
    let descripcionField = document.getElementById("inpDocumentacionDescripcion");

    let creadoPorIdField = document.getElementById("inpDocumentacionCreadoPor");
    let modificadoPorIdField = document.getElementById("inpDocumentacionModificadoPor");

    // ✅ Código
    let nombreArchivoField = document.getElementById("inpDocumentacionNombreArchivo");

    // ✅ Responsable (select)
    let responsableField = document.getElementById("inpDocumentacionRespoonsable");

    let rutaArchivoField = document.getElementById("inpDocumentacionRutaArchivo");
    let ubicacionField = document.getElementById("inpDocumentacionUbicacion");
    let observacionesField = document.getElementById("inpDocumentacionObservaciones");
    let archivoField = document.getElementById("inpDocumentacionArchivo");

    let fechaCreacionField = document.getElementById("inpDocumentacionFechaCreacion");
    let fechaModificacionField = document.getElementById("inpDocumentacionFechaModificacion");

    let btnGuardar = document.getElementById("dlgDocumentoBtnGuardar");
    let dlgTitle = document.getElementById("dlgDocumentacionTitle");
    let summaryContainer = document.getElementById("saveValidationSummary");
    if (summaryContainer) summaryContainer.innerHTML = "";

    // ✅ ID siempre disabled
    if (idField) idField.setAttribute("disabled", true);

    // ========= Helpers base =========
    const setDisabled = (el, disabled) => {
        if (!el) return;
        if (disabled) el.setAttribute("disabled", true);
        else el.removeAttribute("disabled");
    };

    const setReadonly = (el, readonly) => {
        if (!el) return;
        el.readOnly = !!readonly;
    };

    const setTitleByAction = () => {
        if (!dlgTitle) return;
        if (action === NUEVO) dlgTitle.innerHTML = (typeof dlgNuevoTitle !== "undefined") ? dlgNuevoTitle : "Nuevo Documento";
        else if (action === EDITAR) dlgTitle.innerHTML = (typeof dlgEditarTitle !== "undefined") ? dlgEditarTitle : "Editar Documento";
        else dlgTitle.innerHTML = (typeof dlgVerTitle !== "undefined") ? dlgVerTitle : "Ver Documento";
    };

    setTitleByAction();

    const esVer = (action === VER);

    // ==========================
    // UI archivo (preview vs upload)
    // ==========================
    const preview = document.getElementById("docArchivoPreview");
    const link = document.getElementById("docArchivoLink");
    const info = document.getElementById("docArchivoInfo");
    const upload = document.getElementById("docArchivoUpload");

    const resetArchivoUI = () => {
        if (preview) preview.style.display = "none";
        if (upload) upload.style.display = "block";
        if (link) link.href = "#";
        if (info) info.textContent = "";
    };

    resetArchivoUI();

    // ==========================
    // Helpers: Código
    // ==========================
    const setCodigoModoAuto = () => {
        // Automático: readonly true (no editable), NO disabled (se postea)
        if (!nombreArchivoField) return;
        nombreArchivoField.disabled = false;
        setReadonly(nombreArchivoField, true);
    };

    const setCodigoModoManual = () => {
        // Manual: editable, NO disabled (se postea)
        if (!nombreArchivoField) return;
        nombreArchivoField.disabled = false;
        setReadonly(nombreArchivoField, false);
    };

    // ✅ Si el usuario selecciona una opción válida de Descripción => Código manual
    const aplicarReglaCodigoPorDescripcion = () => {
        if (!nombreArchivoField || !descripcionField) return;

        const val = (descripcionField.value || "").trim();

        // Si seleccionó algo (no vacío) => manual
        if (val !== "") {
            setCodigoModoManual();
            // opcional: si estaba vacío, pon "-" como placeholder
            if (!nombreArchivoField.value || nombreArchivoField.value.trim() === "") {
                nombreArchivoField.value = ""; // que el usuario lo capture
            }
        } else {
            // Si no hay descripción seleccionada => auto (solo en NUEVO)
            if (action === NUEVO) setCodigoModoAuto();
            else {
                // en EDITAR, normalmente se deja como venía (pero editable no conviene)
                setCodigoModoAuto();
            }
        }
    };

    const refreshCodigo = () => {
        // Solo aplica para NUEVO y cuando está en modo AUTO
        if (action !== NUEVO) return;
        if (!nombreArchivoField) return;
        if (!nombreArchivoField.readOnly) return; // si está manual, NO recalcular

        const areaId = parseInt(areaField?.value || "0");
        const tipoId = parseInt(tipoDocumentoField?.value || "0");

        if (areaId <= 0 || tipoId <= 0) {
            nombreArchivoField.value = "";
            return;
        }

        $.ajax({
            url: `/Reportes/Documentacion/SiguienteCodigoDocumento?areaId=${areaId}&tipoDocumentoId=${tipoId}`,
            type: "GET",
            success: function (resp) {
                if (resp && resp.tieneError === false) {
                    nombreArchivoField.value = resp.datos || "";
                } else {
                    nombreArchivoField.value = "";
                }
            },
            error: function () {
                nombreArchivoField.value = "";
            }
        });
    };

    // ==========================
    // Helpers: Responsable (select)
    // ==========================
    const resetResponsableUI = (msg) => {
        if (!responsableField) return;
        responsableField.innerHTML = "";
        responsableField.appendChild(new Option(msg || "Seleccione un área primero...", ""));
        responsableField.disabled = true;
    };

    // ==========================
    // Helpers: Descripción (select dependiente de TipoDocumento)
    // ==========================
    const getTipoDocumentoNombreSeleccionado = () => {
        if (!tipoDocumentoField) return "";
        const opt = tipoDocumentoField.options[tipoDocumentoField.selectedIndex];
        return (opt?.text || "").trim();
    };

    const resetDescripcionSelect = (msg) => {
        if (!descripcionField) return;
        descripcionField.innerHTML = "";
        descripcionField.appendChild(new Option(msg || "Seleccione Tipo Documento...", ""));
        descripcionField.disabled = true;
        // al resetear descripción, vuelve a modo auto en NUEVO
        if (action === NUEVO) {
            setCodigoModoAuto();
            refreshCodigo();
        }
    };

    const setDescripcionOptions = (options) => {
        if (!descripcionField) return;

        descripcionField.innerHTML = "";

        if (!options || options.length === 0) {
            resetDescripcionSelect("No hay opciones disponibles");
            return;
        }

        // habilita en NUEVO/EDITAR, pero si es VER se deja deshabilitado
        descripcionField.disabled = (action === VER);

        descripcionField.appendChild(new Option("Seleccione...", ""));
        options.forEach(x => descripcionField.appendChild(new Option(x, x)));
    };

    const getOpcionesDescripcionPorTipoDocumento = (tipoDocNombre) => {
        if (tipoDocNombre === "Referencias Normativas") {
            return ["Manuales", "Políticas", "Reglamentos"];
        }
        if (tipoDocNombre === "Manuales de Capacitación") {
            return ["Manuales", "Procedimientos"];
        }
        return [];
    };

    const refreshDescripcionPorTipo = () => {
        const tipoNombre = getTipoDocumentoNombreSeleccionado();
        const opciones = getOpcionesDescripcionPorTipoDocumento(tipoNombre);
        setDescripcionOptions(opciones);

        // limpia selección anterior si no es VER
        if (descripcionField && action !== VER) {
            descripcionField.value = "";
        }

        // ✅ al cambiar tipo doc, por defecto código vuelve a auto (hasta que seleccionen descripción)
        if (action === NUEVO) {
            setCodigoModoAuto();
            refreshCodigo();
        }
    };

    // ==========================
    // Habilitar / deshabilitar (base)
    // ==========================
    setDisabled(areaField, esVer);
    setDisabled(tipoDocumentoField, esVer);
    setDisabled(estatusField, esVer);
    setDisabled(tituloField, esVer);
    setDisabled(observacionesField, esVer);

    // descripción: en VER siempre disabled; en NUEVO/EDITAR depende del tipo doc
    if (descripcionField) descripcionField.disabled = esVer;

    setDisabled(creadoPorIdField, esVer);
    setDisabled(modificadoPorIdField, esVer);

    // responsable: en VER disabled
    if (responsableField) responsableField.disabled = esVer;

    setDisabled(rutaArchivoField, esVer);
    setDisabled(ubicacionField, esVer);

    // file input
    if (action === EDITAR) setDisabled(archivoField, false);
    else setDisabled(archivoField, esVer);

    setDisabled(btnGuardar, esVer);

    // ==========================
    // Asignación de valores
    // ==========================
    if (action === NUEVO) {

        if (idField) idField.value = "Nuevo";
        if (areaField) areaField.value = "";
        if (tipoDocumentoField) tipoDocumentoField.value = "";
        if (estatusField) estatusField.value = "";
        if (tituloField) tituloField.value = "";
        if (observacionesField) observacionesField.value = "";

        if (creadoPorIdField) creadoPorIdField.value = "";
        if (modificadoPorIdField) modificadoPorIdField.value = "";

        if (rutaArchivoField) rutaArchivoField.value = "";
        if (ubicacionField) ubicacionField.value = "";

        if (archivoField) archivoField.value = "";

        const hoy = new Date().toISOString().split('T')[0];
        if (fechaCreacionField) fechaCreacionField.value = hoy;
        if (fechaModificacionField) fechaModificacionField.value = "";

        // ✅ Responsable inicia bloqueado
        resetResponsableUI("Seleccione un área primero...");

        // ✅ Descripción inicia bloqueado
        resetDescripcionSelect("Seleccione Tipo Documento...");

        // ✅ Código inicia en modo AUTO (readonly)
        if (nombreArchivoField) {
            nombreArchivoField.value = "";
            setCodigoModoAuto();
        }

        // NUEVO: solo upload
        if (preview) preview.style.display = "none";
        if (upload) upload.style.display = "block";

        // ========= listeners (NUEVO) =========
        if (areaField) {
            areaField.onchange = () => {
                const areaId = parseInt(areaField.value || "0");

                // responsables por área
                if (areaId > 0) cargarResponsablesPorArea(areaId, null);
                else resetResponsableUI("Seleccione un área primero...");

                // si código está en auto, refresca
                refreshCodigo();
            };
        }

        if (tipoDocumentoField) {
            tipoDocumentoField.onchange = () => {
                refreshDescripcionPorTipo();
                // refreshCodigo ya está dentro de refreshDescripcionPorTipo para NUEVO
            };
        }

        if (descripcionField) {
            descripcionField.onchange = () => {
                // ✅ si seleccionan descripción => código manual
                aplicarReglaCodigoPorDescripcion();
            };
        }

        // opcional: si ya hubiera defaults
        refreshCodigo();

    } else {

        // EDITAR o VER
        if (idField) idField.value = row?.id ?? "";

        if (areaField) areaField.value = row?.areaId != null ? row.areaId.toString() : "";
        if (tipoDocumentoField) tipoDocumentoField.value = row?.tipoDocumentoId != null ? row.tipoDocumentoId.toString() : "";
        if (estatusField) estatusField.value = row?.estatusDocumentoId != null ? row.estatusDocumentoId.toString() : "";

        if (tituloField) tituloField.value = row?.titulo ?? "";
        if (observacionesField) observacionesField.value = row?.observaciones ?? "";

        if (creadoPorIdField) creadoPorIdField.value = row?.creadoPorId ?? "";
        if (modificadoPorIdField) modificadoPorIdField.value = row?.modificadoPorId ?? "";

        if (nombreArchivoField) nombreArchivoField.value = row?.nombreArchivo ?? "";

        if (rutaArchivoField) rutaArchivoField.value = row?.rutaArchivo ?? "";
        if (ubicacionField) ubicacionField.value = row?.ubicacion ?? "";

        if (archivoField) archivoField.value = "";

        const fc = row?.fechaCreacion ? row.fechaCreacion.toString().substring(0, 10) : "";
        const fm = row?.fechaModificacion ? row.fechaModificacion.toString().substring(0, 10) : "";

        if (fechaCreacionField) fechaCreacionField.value = fc;
        if (fechaModificacionField) fechaModificacionField.value = fm;

        // Preview si hay ruta
        const ruta = (row?.versionRutaArchivo || row?.rutaArchivo || "").trim();
        const nombre = (row?.versionNombreArchivo || row?.nombreArchivo || "").trim();
        const tieneArchivo = ruta !== "";

        if (tieneArchivo) {
            if (preview) preview.style.display = "block";
            if (link) link.href = ruta;
            if (info) info.textContent = nombre || "";
        } else {
            if (preview) preview.style.display = "none";
        }

        // VER: ocultar upload
        if (action === VER) {
            if (upload) upload.style.display = "none";
        } else {
            if (upload) upload.style.display = "block";
        }

        // Responsable: cargar lista por área y seleccionar guardado
        const areaId = parseInt(areaField?.value || "0");
        const responsableActual = row?.responsable || "";

        if (areaId > 0) cargarResponsablesPorArea(areaId, responsableActual);
        else resetResponsableUI("Seleccione un área primero...");

        // Descripción: cargar opciones por tipo doc y seleccionar guardado
        const tipoNombre = getTipoDocumentoNombreSeleccionado();
        const opciones = getOpcionesDescripcionPorTipoDocumento(tipoNombre);
        setDescripcionOptions(opciones);

        if (descripcionField) {
            descripcionField.value = row?.descripcion ?? "";
            if (action === VER) descripcionField.disabled = true;
        }

        // ✅ regla código en EDITAR/VER:
        // - si hay descripción seleccionada => que el código sea MANUAL (editable) SOLO en EDITAR, en VER no.
        if (descripcionField && (descripcionField.value || "").trim() !== "") {
            if (action === EDITAR) setCodigoModoManual();
            else setCodigoModoAuto(); // VER no editable
        } else {
            setCodigoModoAuto();
        }

        if (action === VER) {
            // VER: código bloqueado siempre
            setCodigoModoAuto();
            if (nombreArchivoField) nombreArchivoField.disabled = false; // solo por si, pero readonly
        }

        // ========= listeners (EDITAR) =========
        if (action === EDITAR) {

            if (areaField) {
                areaField.onchange = () => {
                    const newAreaId = parseInt(areaField.value || "0");
                    if (newAreaId > 0) cargarResponsablesPorArea(newAreaId, null);
                    else resetResponsableUI("Seleccione un área primero...");
                };
            }

            if (tipoDocumentoField) {
                tipoDocumentoField.onchange = () => {
                    refreshDescripcionPorTipo(); // recarga lista de descripción
                    // en EDITAR no recalculamos código automático; solo ajustamos modo
                    setCodigoModoAuto();
                };
            }

            if (descripcionField) {
                descripcionField.onchange = () => {
                    aplicarReglaCodigoPorDescripcion();
                };
            }
        }
        else {
            // VER: sin listeners
            if (areaField) areaField.onchange = null;
            if (tipoDocumentoField) tipoDocumentoField.onchange = null;
            if (descripcionField) descripcionField.onchange = null;
        }
    }

    dlgModal.show();
}








/*function onGuardarClick() {
    $("#theForm").validate();
    let valid = $("#theForm").valid();
    if (!valid) { return; }

    let btnClose = document.getElementById("dlgDocumentoBtnCancelar");

    let idField = document.getElementById("inpDocumentacionId");
    let tituloField = document.getElementById("inpDocumentacionTitulo");
    let descripcionField = document.getElementById("inpDocumentacionDescripcion");
    let creadoPorIdField = document.getElementById("inpDocumentacionCreadoPor");
    let fechaCrecionField = document.getElementById("inpDocumentacionFechaCreacion");
    let modificadoPorField = document.getElementById("inpDocumentacionModificadoPor");
    let fechaModificacionField = document.getElementById("inpDocumentacionFechaModificacion");
    let nombreArchivoField = document.getElementById("inpDocumentacionNombreArchivo");
    let observacionesField = document.getElementById("inpDocumentacionObservaciones");

    let responsableField = document.getElementById("inpDocumentacionRespoonsable");
    let rutaArchivoField = document.getElementById("inpDocumentacionRutaArchivo");
    let ubicacionField = document.getElementById("inpDocumentacionUbicacion");

    let dlgTitle = document.getElementById("dlgDocumentacionTitle");
    let summaryContainer = document.getElementById("saveValidationSummary");
    summaryContainer.innerHTML = "";

    let areaIdField = document.getElementById("inpDocumentacionArea");
    let tipoDcumentoField = document.getElementById("inpDocumentacionTipoDocumento");
    let estatusDocumentoIdField = document.getElementById("inpDocumentacionEstatusDocumento");

    let oParams = {
        id: idField.value === "Nuevo" ? 0 : parseInt(idField.value),
        titulo: tituloField.value,
        descripcion: descripcionField.value,
        creadoPorId: creadoPorIdField.value,
        fechaCreacion: fechaCrecionField.value,
        modificadoPor: modificadoPorField.value,
        areaId: parseInt(areaIdField?.value || 0),
        tipoDocumentoId: parseInt(tipoDcumentoField?.value || 0),
        estatusDocumentoId: parseInt(estatusDocumentoIdField?.value || 0),
        fechaModificacion: fechaModificacionField.value,
        nombreArchivo: nombreArchivoField.value,
        observaciones: observacionesField.value,
        responsable: responsableField.value,
        rutaArchivo: rutaArchivoField.value,
        ubicacion: ubicacionField.value
    };

    doAjax(
        "/Reportes/Documentacion/SaveDocumento",
        oParams,
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

            document.querySelector("[name='refresh']").click();
            showSuccess(dlgTitle.innerHTML, resp.mensaje);
        },
        function (error) {
            showError("Error", error);
        },
        postOptions
    );
}*/

function bindCodigoAuto() {
    const area = document.getElementById("inpDocumentacionArea");
    const tipo = document.getElementById("inpDocumentacionTipoDocumento");
    const codigo = document.getElementById("inpDocumentacionNombreArchivo");

    async function refreshCodigo() {
        const areaId = parseInt(area?.value || "0");
        const tipoId = parseInt(tipo?.value || "0");

        if (!codigo) return;
        
        if (areaId <= 0 || tipoId <= 0) {
            codigo.value = "";
            return;
        }

        $.ajax({
            url: `/Reportes/Documentacion/SiguienteCodigoDocumento?areaId=${areaId}&tipoDocumentoId=${tipoId}`,
            type: "GET",
            success: function (resp) {
                if (resp && resp.tieneError === false) {
                    codigo.value = resp.datos || "";
                } else {
                    codigo.value = "";
                }
            },
            error: function () {
                codigo.value = "";
            }
        });
    }

    if (area) area.addEventListener("change", refreshCodigo);
    if (tipo) tipo.addEventListener("change", refreshCodigo);
}


function onGuardarClick() {
    $("#theForm").validate();
    let valid = $("#theForm").valid();
    if (!valid) { return; }

    let btnClose = document.getElementById("dlgDocumentoBtnCancelar");

    let idField = document.getElementById("inpDocumentacionId");
    let tituloField = document.getElementById("inpDocumentacionTitulo");
    let descripcionField = document.getElementById("inpDocumentacionDescripcion");

    let fechaCrecionField = document.getElementById("inpDocumentacionFechaCreacion");
    let fechaModificacionField = document.getElementById("inpDocumentacionFechaModificacion");

    let nombreArchivoField = document.getElementById("inpDocumentacionNombreArchivo");
    let observacionesField = document.getElementById("inpDocumentacionObservaciones");

    let responsableField = document.getElementById("inpDocumentacionRespoonsable");
    let rutaArchivoField = document.getElementById("inpDocumentacionRutaArchivo");
    let ubicacionField = document.getElementById("inpDocumentacionUbicacion");

    let areaIdField = document.getElementById("inpDocumentacionArea");
    let tipoDcumentoField = document.getElementById("inpDocumentacionTipoDocumento");
    let estatusDocumentoIdField = document.getElementById("inpDocumentacionEstatusDocumento");

    let archivoField = document.getElementById("inpDocumentacionArchivo");

    let dlgTitle = document.getElementById("dlgDocumentacionTitle");
    let summaryContainer = document.getElementById("saveValidationSummary");
    if (summaryContainer) summaryContainer.innerHTML = "";
    
    const fd = new FormData();
    
    fd.append("Id", (idField.value === "Nuevo" ? "0" : idField.value));
    fd.append("Titulo", tituloField.value || "");
    fd.append("Descripcion", descripcionField.value || "");

    fd.append("AreaId", (areaIdField?.value || "0"));
    fd.append("TipoDocumentoId", (tipoDcumentoField?.value || "0"));
    fd.append("EstatusDocumentoId", (estatusDocumentoIdField?.value || "0"));

    fd.append("FechaCreacion", fechaCrecionField?.value || "");
    fd.append("FechaModificacion", fechaModificacionField?.value || "");

    fd.append("NombreArchivo", nombreArchivoField?.value || "");
    fd.append("RutaArchivo", rutaArchivoField?.value || "");
    fd.append("Ubicacion", ubicacionField?.value || "");
    fd.append("Observaciones", observacionesField?.value || "");
    fd.append("Responsable", responsableField?.value || "");

    // ✅ Archivo (solo si seleccionaron)
    if (archivoField && archivoField.files && archivoField.files.length > 0) {
        fd.append("Archivo", archivoField.files[0]);
    }

    // ✅ Anti-forgery token (Razor Pages)
    const token = $('input[name="__RequestVerificationToken"]').val();
    if (token) fd.append("__RequestVerificationToken", token);

    $.ajax({
        url: "/Reportes/Documentacion/SaveDocumento",
        type: "POST",
        data: fd,
        processData: false,   // ✅ clave para FormData
        contentType: false,   // ✅ clave para FormData
        headers: { "RequestVerificationToken": token }, // (si tu servidor lo lee por header)
        success: function (resp) {
            if (resp.tieneError) {
                if (Array.isArray(resp.errores) && resp.errores.length >= 1) {
                    let summary = ``;
                    resp.errores.forEach(function (error) {
                        summary += `<li>${error}</li>`;
                    });
                    if (summaryContainer) summaryContainer.innerHTML = `<ul>${summary}</ul>`;
                }
                showError(dlgTitle?.innerHTML || "Error", resp.mensaje);
                return;
            }
        
            if (btnClose) btnClose.click();

            if (table) table.bootstrapTable('refresh');

            showSuccess(dlgTitle?.innerHTML || "OK", resp.mensaje);
        },
        error: function (xhr) {
            showError("Error", xhr?.responseText || "Error al guardar.");
        }
    });
}

async function cargarResponsablesPorArea(areaId, valorSeleccionado) {
    const sel = document.getElementById("inpDocumentacionRespoonsable");
    if (!sel) return;

    // Reset UI
    sel.innerHTML = "";
    sel.disabled = true;

    if (!areaId || areaId <= 0) {
        sel.innerHTML = `<option value="">Seleccione un área primero...</option>`;
        return;
    }

    sel.innerHTML = `<option value="">Cargando...</option>`;

    $.ajax({
        url: `/Reportes/Documentacion/ResponsablesByArea?areaId=${areaId}`,
        type: "GET",
        success: function (resp) {
            sel.innerHTML = "";

            if (!resp || resp.tieneError) {
                sel.innerHTML = `<option value="">No se pudieron cargar responsables</option>`;
                sel.disabled = true;
                return;
            }

            const data = resp.datos || [];
            if (data.length === 0) {
                sel.innerHTML = `<option value="">No hay responsables para esta área</option>`;
                sel.disabled = true;
                return;
            }

            // Opción default
            sel.innerHTML = `<option value="">Seleccione...</option>`;

            // ✅ Guardaremos NOMBRE en value (lo que quieres persistir en Documento.Responsable)
            data.forEach(x => {
                const opt = document.createElement("option");
                opt.value = x.nombre;         // <-- aquí va el NOMBRE
                opt.textContent = x.nombre;   // <-- texto visible
                sel.appendChild(opt);
            });

            sel.disabled = false;

            // Seleccionar valor si viene (para EDITAR/VER)
            if (valorSeleccionado) {
                sel.value = valorSeleccionado;
            }
        },
        error: function () {
            sel.innerHTML = `<option value="">Error cargando responsables</option>`;
            sel.disabled = true;
        }
    });
}

function getTipoDocumentoNombreSeleccionado(tipoSelect) {
    if (!tipoSelect) return "";
    const opt = tipoSelect.options[tipoSelect.selectedIndex];
    return (opt?.text || "").trim();
}

function setDescripcionOptions(options) {
    const sel = document.getElementById("inpDocumentacionDescripcion");
    if (!sel) return;

    sel.innerHTML = "";

    if (!options || options.length === 0) {
        sel.disabled = true;
        sel.innerHTML = `<option value="">No hay opciones disponibles</option>`;
        return;
    }

    sel.disabled = false;
    sel.innerHTML = `<option value="">Seleccione...</option>` +
        options.map(x => `<option value="${x}">${x}</option>`).join("");
}

function resetDescripcionSelect(msg) {
    const sel = document.getElementById("inpDocumentacionDescripcion");
    if (!sel) return;
    sel.disabled = true;
    sel.innerHTML = `<option value="">${msg || "Seleccione Tipo Documento..."}</option>`;
}

function getOpcionesDescripcionPorTipoDocumento(tipoDocNombre) {

    // Ejemplos EXACTOS como pediste:
    if (tipoDocNombre === "Referencias normativas") {
        return ["Manuales", "Políticas", "Reglamentos"];
    }

    if (tipoDocNombre === "Manuales de Capacitación") {
        return ["Manuales", "Procedimientos"];
    }

    // Para cualquier otro tipo:
    return [];
}




function onBuscarClick() {
    let btnBuscar = document.getElementById("btnBuscar");

    let inpTitulo = document.getElementById("inpFiltroTitulo");
    let selArea = document.getElementById("selFiltroArea");
    let selTipoDocumento = document.getElementById("selFiltroTipoDocumento");
    let selEstatusDocumento = document.getElementById("selFiltroEstatusDocumento");
    let inpPalabraClave = document.getElementById("inpFiltroPalabraClave");

    let inpFechaInicio = document.getElementById("inpFiltroFechaInicio");
    let inpFechaFin = document.getElementById("inpFiltroFechaFin");
    
    const fi = (inpFechaInicio?.value || "").trim();
    const ff = (inpFechaFin?.value || "").trim();
    
    if (fi && ff) {
        const d1 = new Date(fi + "T00:00:00");
        const d2 = new Date(ff + "T00:00:00");
        if (d2 < d1) {
            showError("Fechas inválidas", "La fecha fin no puede ser menor que la fecha inicio. Selecciona un rango correcto.");
            return;
        }
    }

    let fechaCreacionInicio = null;
    let fechaCreacionFin = null;

    if (fi && !ff) {
        fechaCreacionInicio = fi;
        fechaCreacionFin = null;
    } else if (fi && ff) {
        fechaCreacionInicio = fi;
        fechaCreacionFin = ff;
    } else if (!fi && ff) {

        fechaCreacionInicio = ff;
        fechaCreacionFin = null;
    }

    let oParams = {
        titulo: inpTitulo?.value?.trim() || null,
        areaId: (!selArea || selArea.value === "0" || selArea.value === "") ? null : parseInt(selArea.value),
        tipoDocumentoId: (!selTipoDocumento || selTipoDocumento.value === "0" || selTipoDocumento.value === "") ? null : parseInt(selTipoDocumento.value),
        estatusDocumentoId: (!selEstatusDocumento || selEstatusDocumento.value === "0" || selEstatusDocumento.value === "") ? null : parseInt(selEstatusDocumento.value),
        palabraClave: inpPalabraClave?.value?.trim() || null,
        
        fechaCreacionInicio: fechaCreacionInicio,
        fechaCreacionFin: fechaCreacionFin
    };

    doAjax(
        "/Reportes/Documentacion/FiltrarDocumentos",
        oParams,
        function (resp) {
            if (resp.tieneError) {
                if (Array.isArray(resp.errores) && resp.errores.length > 0) {
                    let summary = resp.errores.map(error => `<li>${error}</li>`).join("");
                    let saveValidationSummary = document.getElementById("saveValidationSummary");
                    if (saveValidationSummary) saveValidationSummary.innerHTML = `<ul>${summary}</ul>`;
                }
                showError(btnBuscar?.innerHTML ?? "Buscar", resp.mensaje);
                return;
            }

            table.bootstrapTable('load', responseHandler(resp.datos));
        },
        function (error) {
            showError("Error", error);
        },
        postOptions
    );

    // ⚠️ Si no quieres borrar las fechas después de buscar, quita este reset o excluye los inputs date
    document.querySelectorAll("#filtros .form-control").forEach(function (e) { e.value = ""; });
}


/*function onBuscarClick() {
    let btnBuscar = document.getElementById("btnBuscar");

    let inpTitulo = document.getElementById("inpFiltroTitulo");
    let selArea = document.getElementById("selFiltroArea");
    let selTipoDocumento = document.getElementById("selFiltroTipoDocumento");
    let selEstatusDocumento = document.getElementById("selFiltroEstatusDocumento");
    let inpPalabraClave = document.getElementById("inpFiltroPalabraClave");
    let inpFechaInicio = document.getElementById("inpFiltroFechaInicio");
    let inpFechaFin = document.getElementById("inpFiltroFechaFin");

    let oParams = {
        titulo: inpTitulo?.value?.trim() || null,
        areaId: (!selArea || selArea.value === "0" || selArea.value === "") ? null : parseInt(selArea.value),
        tipoDocumentoId: (!selTipoDocumento || selTipoDocumento.value === "0" || selTipoDocumento.value === "") ? null : parseInt(selTipoDocumento.value),
        estatusDocumentoId: (!selEstatusDocumento || selEstatusDocumento.value === "0" || selEstatusDocumento.value === "") ? null : parseInt(selEstatusDocumento.value),
        palabraClave: inpPalabraClave?.value?.trim() || null,
        fechaCreacionInicio: inpFechaInicio?.value || null,
        fechaCreacionFin: inpFechaFin?.value || null
    };

    doAjax(
        "/Reportes/Documentacion/FiltrarDocumentos",
        oParams,
        function (resp) {
            if (resp.tieneError) {
                // Si manejas errores tipo "resp.errores"
                if (Array.isArray(resp.errores) && resp.errores.length > 0) {
                    let summary = resp.errores.map(error => `<li>${error}</li>`).join("");
                    let saveValidationSummary = document.getElementById("saveValidationSummary");
                    if (saveValidationSummary) saveValidationSummary.innerHTML = `<ul>${summary}</ul>`;
                }
                showError(btnBuscar?.innerHTML ?? "Buscar", resp.mensaje);
                return;
            }

            table.bootstrapTable('load', responseHandler(resp.datos));
        },
        function (error) {
            showError("Error", error);
        },
        postOptions
    );

    // ✅ Igual que tu patrón actual: resetea inputs del form
    document.querySelectorAll("#filtros .form-control").forEach(function (e) { e.value = ""; });

    // Si quieres también resetear selects:
    // document.querySelectorAll("#filtros .form-select").forEach(function (e) { e.value = "0"; });
}*/



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
