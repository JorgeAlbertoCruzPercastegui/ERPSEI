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

    let descripcionField = document.getElementById("inpDocumentacionDescripcion");

    let creadoPorIdField = document.getElementById("inpDocumentacionCreadoPor");
    let modificadoPorIdField = document.getElementById("inpDocumentacionModificadoPor");
    
    let nombreArchivoField = document.getElementById("inpDocumentacionNombreArchivo");
    
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
    
    if (idField) idField.setAttribute("disabled", true);
    
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

    resetAutorizacionesUI(action === NUEVO ? "NUEVO" : "CARGANDO");

    
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
    
    const setCodigoModoAuto = () => {
        if (!nombreArchivoField) return;
        nombreArchivoField.disabled = false;
        setReadonly(nombreArchivoField, true);
    };

    const setCodigoModoManual = () => {
        if (!nombreArchivoField) return;
        nombreArchivoField.disabled = false;
        setReadonly(nombreArchivoField, false);
    };
    
    const aplicarReglaCodigoPorDescripcion = () => {
        if (!nombreArchivoField || !descripcionField) return;

        const val = (descripcionField.value || "").trim();
        
        if (val !== "") {
            setCodigoModoManual();
            if (!nombreArchivoField.value || nombreArchivoField.value.trim() === "") {
                nombreArchivoField.value = "";
            }
        } else {
            setCodigoModoAuto();
            if (action === NUEVO) refreshCodigo();
        }
    };

    const refreshCodigo = () => {
        if (action !== NUEVO) return;
        if (!nombreArchivoField) return;
        if (!nombreArchivoField.readOnly) return;

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
    
    const resetResponsableUI = (msg) => {
        if (!responsableField) return;
        responsableField.innerHTML = "";
        responsableField.appendChild(new Option(msg || "Seleccione un área primero...", ""));
        responsableField.disabled = true;
    };
    
    const cargarResponsablesYAplicarBloqueo = (areaId, responsableActual) => {
        if (areaId > 0) cargarResponsablesPorArea(areaId, responsableActual);
        else resetResponsableUI("Seleccione un área primero...");
        
        if (esVer && responsableField) {
            setTimeout(() => {
                responsableField.disabled = true;
            }, 300);
        }
    };
    
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

        if (descripcionField && action !== VER) {
            descripcionField.value = "";
        }

        if (action === NUEVO) {
            setCodigoModoAuto();
            refreshCodigo();
        }
    };
    
    setDisabled(areaField, esVer);
    setDisabled(tipoDocumentoField, esVer);
    setDisabled(estatusField, esVer);
    setDisabled(tituloField, esVer);
    setDisabled(observacionesField, esVer);
    setDisabled(descripcionField, esVer);
    
    if (responsableField) responsableField.disabled = esVer;

    if (descripcionField) descripcionField.disabled = esVer;

    setDisabled(creadoPorIdField, esVer);
    setDisabled(modificadoPorIdField, esVer);

    setDisabled(rutaArchivoField, esVer);
    setDisabled(ubicacionField, esVer);

    
    if (action === EDITAR) setDisabled(archivoField, false);
    else setDisabled(archivoField, esVer);

    setDisabled(btnGuardar, esVer);
    
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
        
        resetResponsableUI("Seleccione un área primero...");
        
        resetDescripcionSelect("Seleccione Tipo Documento...");
        
        if (nombreArchivoField) {
            nombreArchivoField.value = "";
            setCodigoModoAuto();
        }
        
        if (preview) preview.style.display = "none";
        if (upload) upload.style.display = "block";
        
        if (areaField) {
            areaField.onchange = () => {
                const areaId = parseInt(areaField.value || "0");
                
                if (areaId > 0) cargarResponsablesPorArea(areaId, null);
                else resetResponsableUI("Seleccione un área primero...");

                refreshCodigo();
            };
        }

        if (tipoDocumentoField) {
            tipoDocumentoField.onchange = () => {
                refreshDescripcionPorTipo();
            };
        }

        if (descripcionField) {
            descripcionField.onchange = () => {
                aplicarReglaCodigoPorDescripcion();
            };
        }

        refreshCodigo();

    } else {
        
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
        
        if (action === VER) {
            if (upload) upload.style.display = "none";
        } else {
            if (upload) upload.style.display = "block";
        }
        
        const areaId = parseInt(areaField?.value || "0");
        const responsableActual = row?.responsable || "";
        cargarResponsablesYAplicarBloqueo(areaId, responsableActual);
        
        const tipoNombre = getTipoDocumentoNombreSeleccionado();
        const opciones = getOpcionesDescripcionPorTipoDocumento(tipoNombre);
        setDescripcionOptions(opciones);

        if (descripcionField) {
            descripcionField.value = row?.descripcion ?? "";
            if (action === VER) descripcionField.disabled = true;
        }
        
        if (descripcionField && (descripcionField.value || "").trim() !== "") {
            if (action === EDITAR) setCodigoModoManual();
            else setCodigoModoAuto();
        } else {
            setCodigoModoAuto();
        }

        if (action === VER) {
            setCodigoModoAuto();
            if (nombreArchivoField) nombreArchivoField.disabled = false;
        }
        
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
                    refreshDescripcionPorTipo();
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
            if (areaField) areaField.onchange = null;
            if (tipoDocumentoField) tipoDocumentoField.onchange = null;
            if (descripcionField) descripcionField.onchange = null;
            
            if (responsableField) responsableField.onchange = null;
        }
    }

    dlgModal.show();

    const documentoId =
        (action !== NUEVO && row && row.id && row.id !== "Nuevo")
            ? parseInt(row.id)
            : 0;

    cargarAutorizacionesDocumento(documentoId);

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

    const isNuevo = (idField?.value === "Nuevo" || idField?.value === "" || idField?.value === null);
    const idValue = isNuevo ? "0" : (idField.value || "0");

    fd.append("Id", idValue);
    fd.append("Titulo", tituloField?.value || "");
    fd.append("Descripcion", descripcionField?.value || "");

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
        processData: false,
        contentType: false,
        headers: token ? { "RequestVerificationToken": token } : {},
        success: function (resp) {
            if (!resp) {
                showError(dlgTitle?.innerHTML || "Error", "Respuesta vacía del servidor.");
                return;
            }

            if (resp.tieneError) {
                if (Array.isArray(resp.errores) && resp.errores.length >= 1) {
                    let summary = ``;
                    resp.errores.forEach(function (error) {
                        summary += `<li>${error}</li>`;
                    });
                    if (summaryContainer) summaryContainer.innerHTML = `<ul>${summary}</ul>`;
                }
                showError(dlgTitle?.innerHTML || "Error", resp.mensaje || "No se pudo guardar.");
                return;
            }

            // ✅ Si el backend regresó el ID, actualízalo (IMPORTANTE para autorizaciones)
            const newId = parseInt(resp.id || "0") || 0;
            if (newId > 0 && idField) idField.value = newId.toString();

            // ✅ Refrescar autorizaciones (ya existirán porque el backend las crea al guardar nuevo)
            if (newId > 0) {
                cargarAutorizacionesDocumento(newId);
            }

            if (btnClose) btnClose.click();

            if (table) table.bootstrapTable('refresh');

            showSuccess(dlgTitle?.innerHTML || "OK", resp.mensaje || "Guardado correctamente.");
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

// ==========================
// AUTORIZACIONES UI (REEMPLAZO)
// ==========================

// Helper: URL de handler Razor Pages
function _h(handler, qs) {
    const q = qs ? `&${qs}` : "";
    return `/Reportes/Documentacion?handler=${handler}${q}`;
}

function getAuthEls() {
    return {
        resumen: document.getElementById("authResumenBadge"),

        badgeDirectivos: document.getElementById("authBadgeDirectivos"),
        infoDirectivos: document.getElementById("authInfoDirectivos"),
        btnsDirectivos: document.getElementById("authBtnsDirectivos"),

        badgeReingenieria: document.getElementById("authBadgeReingenieria"),
        infoReingenieria: document.getElementById("authInfoReingenieria"),
        btnsReingenieria: document.getElementById("authBtnsReingenieria"),

        badgeGerente: document.getElementById("authBadgeGerente"),
        infoGerente: document.getElementById("authInfoGerente"),
        btnsGerente: document.getElementById("authBtnsGerente"),
    };
}

function _setBadge(el, estado) {
    if (!el) return;

    // esperado: "PENDIENTE" | "APROBADO" | "RECHAZADO" | "NA"
    el.classList.remove("text-bg-secondary", "text-bg-success", "text-bg-danger", "text-bg-warning");

    switch ((estado || "").toUpperCase()) {
        case "APROBADO":
            el.classList.add("text-bg-success");
            el.textContent = "Aprobado";
            break;
        case "RECHAZADO":
            el.classList.add("text-bg-danger");
            el.textContent = "Rechazado";
            break;
        case "PENDIENTE":
            el.classList.add("text-bg-secondary");
            el.textContent = "Pendiente";
            break;
        case "NA":
        default:
            el.classList.add("text-bg-secondary");
            el.textContent = "—";
            break;
    }
}

function _setInfo(el, texto) {
    if (!el) return;
    el.textContent = texto || "—";
}

function _showBtns(el, show) {
    if (!el) return;
    if (show) el.classList.remove("d-none");
    else el.classList.add("d-none");
}

function resetAutorizacionesUI(modo) {
    // modo: "NUEVO" | "CARGANDO" | "OK"
    const a = getAuthEls();
    if (!a.resumen && !a.badgeDirectivos && !a.badgeReingenieria && !a.badgeGerente) return;

    if (a.resumen) {
        a.resumen.className = "badge rounded-pill text-bg-secondary";
        a.resumen.textContent =
            (modo === "CARGANDO") ? "Autorización: Cargando..." :
                (modo === "NUEVO") ? "Autorización: No aplica (nuevo)" :
                    "Autorización: —";
    }

    _setBadge(a.badgeDirectivos, modo === "NUEVO" ? "NA" : "PENDIENTE");
    _setBadge(a.badgeReingenieria, modo === "NUEVO" ? "NA" : "PENDIENTE");
    _setBadge(a.badgeGerente, modo === "NUEVO" ? "NA" : "PENDIENTE");

    _setInfo(a.infoDirectivos, "—");
    _setInfo(a.infoReingenieria, "—");
    _setInfo(a.infoGerente, "—");

    _showBtns(a.btnsDirectivos, false);
    _showBtns(a.btnsReingenieria, false);
    _showBtns(a.btnsGerente, false);
}

/*function formatearInfoAuth(obj) {
    if (!obj) return "—";

    const estado = (obj.estado || "").toUpperCase();
    if (!estado) return "—";

    if (estado === "PENDIENTE") return "Pendiente de revisión";

    if (estado === "APROBADO") {
        const por = obj.por ? `Por: ${obj.por}` : "";
        const fecha = obj.fecha ? `Fecha: ${obj.fecha}` : "";
        return [por, fecha].filter(Boolean).join(" • ") || "Aprobado";
    }

    if (estado === "RECHAZADO") {
        const por = obj.por ? `Por: ${obj.por}` : "";
        const fecha = obj.fecha ? `Fecha: ${obj.fecha}` : "";
        const com = obj.comentario ? `Motivo: ${obj.comentario}` : "";
        return [por, fecha, com].filter(Boolean).join(" • ") || "Rechazado";
    }

    return "—";
}*/

function formatearInfoAuth(obj) {
    if (!obj) return "—";

    const estado = (obj.estado || "").toUpperCase();
    if (!estado) return "—";

    if (estado === "PENDIENTE") return "Pendiente de revisión";

    // ✅ Nuevo: prioriza PUESTO, fallback a "por" si existe
    const puesto = obj.puesto ? `Autorizado Por: ${obj.puesto}` : "";
    const por = obj.por ? `Por: ${obj.por}` : "";
    const quien = puesto || por;

    const fecha = obj.fecha ? `Fecha: ${obj.fecha}` : "";

    if (estado === "APROBADO") {
        return [quien, fecha].filter(Boolean).join(" • ") || "Aprobado";
    }

    if (estado === "RECHAZADO") {
        const com = obj.comentario ? `Motivo: ${obj.comentario}` : "";
        return [quien, fecha, com].filter(Boolean).join(" • ") || "Rechazado";
    }

    return "—";
}


/*function cargarAutorizacionesDocumento(documentoId) {
    if (!documentoId || documentoId <= 0) {
        resetAutorizacionesUI("NUEVO");
        return;
    }

    resetAutorizacionesUI("CARGANDO");

    $.ajax({
        url: _h("Autorizaciones", `documentoId=${encodeURIComponent(documentoId)}`),
        type: "GET",
        success: function (resp) {
            if (!resp || resp.tieneError) {
                resetAutorizacionesUI("OK");
                return;
            }

            //console.log("ROLES DEL USUARIO:", resp?.datos?.rolesUsuario);
            //console.log("RESP COMPLETA:", resp);


            const d = resp.datos || {};
            const a = getAuthEls();

            // resumen
            if (a.resumen) {
                const estado = (d.resumen || "PENDIENTE").toUpperCase();
                a.resumen.className =
                    "badge rounded-pill " + (estado === "APROBADO" ? "text-bg-success" :
                        estado === "RECHAZADO" ? "text-bg-danger" : "text-bg-secondary");

                a.resumen.textContent =
                    `Autorización: ${estado === "APROBADO" ? "Aprobado" : estado === "RECHAZADO" ? "Rechazado" : "Pendiente"}`;
            }

            // estados
            _setBadge(a.badgeDirectivos, d.directivos?.estado);
            _setInfo(a.infoDirectivos, formatearInfoAuth(d.directivos));

            _setBadge(a.badgeReingenieria, d.reingenieria?.estado);
            _setInfo(a.infoReingenieria, formatearInfoAuth(d.reingenieria));

            _setBadge(a.badgeGerente, d.gerente?.estado);
            _setInfo(a.infoGerente, formatearInfoAuth(d.gerente));

            // botones según permisos + pendiente
            const p = d.permisos || {};
            _showBtns(a.btnsDirectivos, !!p.puedeAutorizarDirectivos && (d.directivos?.estado || "").toUpperCase() === "PENDIENTE");
            _showBtns(a.btnsReingenieria, !!p.puedeAutorizarReingenieria && (d.reingenieria?.estado || "").toUpperCase() === "PENDIENTE");
            _showBtns(a.btnsGerente, !!p.puedeAutorizarGerente && (d.gerente?.estado || "").toUpperCase() === "PENDIENTE");
        },
        error: function () {
            resetAutorizacionesUI("OK");
        }
    });
}*/

function cargarAutorizacionesDocumento(documentoId) {
    if (!documentoId || documentoId <= 0) {
        resetAutorizacionesUI("NUEVO");
        return;
    }

    resetAutorizacionesUI("CARGANDO");

    $.ajax({
        url: _h("Autorizaciones", `documentoId=${encodeURIComponent(documentoId)}`),
        type: "GET",
        success: function (resp) {
            if (!resp || resp.tieneError) {
                resetAutorizacionesUI("OK");
                return;
            }

            const d = resp.datos || {};
            const a = getAuthEls();
            
            if (a.resumen) {
                const estado = (d.resumen || "PENDIENTE").toUpperCase();
                a.resumen.className =
                    "badge rounded-pill " + (estado === "APROBADO" ? "text-bg-success" :
                        estado === "RECHAZADO" ? "text-bg-danger" : "text-bg-secondary");

                a.resumen.textContent =
                    `Autorización: ${estado === "APROBADO" ? "Aprobado" : estado === "RECHAZADO" ? "Rechazado" : "Pendiente"}`;
            }
            
            _setBadge(a.badgeDirectivos, d.directivos?.estado);
            _setInfo(a.infoDirectivos, formatearInfoAuth(d.directivos));

            _setBadge(a.badgeReingenieria, d.reingenieria?.estado);
            _setInfo(a.infoReingenieria, formatearInfoAuth(d.reingenieria));

            _setBadge(a.badgeGerente, d.gerente?.estado);
            _setInfo(a.infoGerente, formatearInfoAuth(d.gerente));
            
            const p = d.permisos || {};

            const estDir = (d.directivos?.estado || "").toUpperCase();
            const estRei = (d.reingenieria?.estado || "").toUpperCase();
            const estGer = (d.gerente?.estado || "").toUpperCase();
            
            _showBtns(a.btnsReingenieria, !!p.puedeAutorizarReingenieria && estRei === "PENDIENTE");
            _showBtns(a.btnsGerente, !!p.puedeAutorizarGerente && estGer === "PENDIENTE");
            
            const habilitarDirectivos =
                !!p.puedeAutorizarDirectivos &&
                estDir === "PENDIENTE" &&
                estRei === "APROBADO" &&
                estGer === "APROBADO";

            _showBtns(a.btnsDirectivos, habilitarDirectivos);
        },
        error: function () {
            resetAutorizacionesUI("OK");
        }
    });
}


function onAutorizarClick(rol, aprobar) {
    const idField = document.getElementById("inpDocumentacionId");
    const raw = (idField?.value || "0").toString().trim();
    const documentoId = (raw.toLowerCase() === "nuevo") ? 0 : (parseInt(raw) || 0);

    if (!documentoId || documentoId <= 0) {
        showError("Autorizar", "Primero guarda el documento antes de autorizar.");
        return;
    }

    const token = $('input[name="__RequestVerificationToken"]').val();

    const oParams = {
        DocumentoId: documentoId,
        Rol: rol,               // "DIRECTIVOS" | "REINGENIERIA" | "GERENTE_AREA"
        Aprobar: !!aprobar,
        Comentario: ""   
    };

    doAjax(
        _h("Autorizar"),
        oParams,
        function (resp) {
            if (!resp || resp.tieneError) {
                showError("Autorizar", resp?.mensaje || "No se pudo autorizar.");
                return;
            }

            showSuccess("Autorizar", resp?.mensaje || "Autorización actualizada.");

            // refrescar UI del modal
            cargarAutorizacionesDocumento(documentoId);

            // refrescar tabla para ver cambios
            if (table) table.bootstrapTable("refresh");
        },
        function (error) {
            showError("Autorizar", error || "Error al autorizar.");
        },
        { headers: { "RequestVerificationToken": token } }
    );
}

