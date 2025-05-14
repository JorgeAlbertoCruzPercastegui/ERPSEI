var table;
var numFormatter = null;
var buttonRemove;
var tableActividad;
var selections = [];
var dlgEmpresa = null;
var dlgEmpresaModal = null;

var dialogMode = null;
const NUEVO = 0;
const EDITAR = 1;
const VER = 2;
const maxFileSizeInBytes = 5242880; //5mb = (5 * 1024) * 1024
const oneMegabyteSizeInBytes = 1048576; // 1mb = (1 * 1024) * 1024
const maxBanks = 15; // Máximo 15 registros de bancos.
const postOptions = { headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() } }

//Función para inicializar el módulo.
document.addEventListener("DOMContentLoaded", function (event) {
    numFormatter = new Intl.NumberFormat(cultureName);

    table = $("#table");
    buttonRemove = $("#remove");
    tableActividad = $("#tableActividades");
    dlgEmpresa = document.getElementById('dlgEmpresa');
    dlgEmpresaModal = new bootstrap.Modal(dlgEmpresa, null);
    //Función para limpiar el cuadro de diálogo cuando es cerrado
    dlgEmpresa.addEventListener('hidden.bs.modal', function (event) { onCerrarClick(); });

    //Función para ejecutar acciones posteriores al mostrado del diálogo.
    dlgEmpresa.addEventListener('shown.bs.modal', function (e) {
        //Este evento es necesario para poder mostrar el text area ajustado al tamaño del contenido, basado en el tamaño del scroll.
        calculateTextAreaHeight(document.querySelectorAll("textarea"));
    })

    initTable();

    let btnBuscar = document.getElementById("btnBuscar");
    if (btnBuscar) { btnBuscar.click(); }

    autoCompletar("#inpFiltroActividadEconomica");
    autoCompletar("#inpEmpresaActividadEconomica");

    //jQuery.validator.setDefaults({
    //    errorClass: "is-invalid",
    //    validClass: "is-valid"
    //});

});
//Función para redimensionar el campo de objeto social cada que cambie el tamaño de pantalla.
window.addEventListener('resize', function (event) {
    calculateTextAreaHeight(document.querySelectorAll("textarea"));
}, true);

////////////////////////////////
//Funcionalidad Tabla
////////////////////////////////
//Función para obtener los identificadores de los registros seleccionados
function getIdSelections() {
    return $.map(table.bootstrapTable('getSelections'), function (row) {
        return row.id
    })
}
//Función para calcular cuantos meses hay entre dos fechas
function monthDiff(d1, d2) {
    var months;
    months = (d2.getFullYear() - d1.getFullYear()) * 12;
    months -= d1.getMonth() + 1;
    months += d2.getMonth() + 1;
    if (d2.getDate() < d1.getDate()) { months -= 1; }
    return months <= 0 ? 0 : months;
}
//Función para establecer el estilo de los rows individualmente
function rowStyle(row, index) {
    let fechaActual = new Date();
    let fechaInvalida = new Date(1, 0, 1, 0, 0, 0, 0);

    let fcParts = (row.fechaConstitucionJS || "").split("-");
    let fechaConstitucion = new Date((fcParts[0]||1), (fcParts[1]||1) - 1, (fcParts[2]||1), 0, 0, 0, 0);
    let fifParts = (row.fechaInicioFacturacionJS || "").split("-");
    let fechaInicioFacturacion = new Date((fifParts[0] || 1), (fifParts[1] || 1) - 1, (fifParts[2] || 1), 0, 0, 0, 0);
    let fiaParts = (row.fechaInicioAsimiladosJS || "").split("-");
    let fechaInicioAsimilados = new Date((fiaParts[0] || 1), (fiaParts[1] || 1) - 1, (fiaParts[2] || 1), 0, 0, 0, 0);

    let alertaMesesConstituida = 32;
    let maxMesesConstituida = 36;
    let alertaMesesFacturando = 8;
    let maxMesesFacturando = 12;
    let alertaMesesAsimilados = 4;

    
    //Se verifica la fecha de consitución
    if (fechaInvalida.toLocaleString() != fechaConstitucion.toLocaleString()) {
        let cMonths = monthDiff(fechaConstitucion, fechaActual);
        if (cMonths >= maxMesesConstituida) {
            return {
                classes: "bd-callout bd-callout-danger border-5 border-top-0 border-bottom-0"
            };
        }
        else if (cMonths >= alertaMesesConstituida) {
            return {
                classes: "bd-callout bd-callout-warning border-5 border-top-0 border-bottom-0"
            };
        }
    }

    //Se verifica la fecha de inicio de facturación
    if (fechaInvalida.toLocaleString() != fechaInicioFacturacion.toLocaleString()) {
        let cMonths = monthDiff(fechaInicioFacturacion, fechaActual);
        if (cMonths >= maxMesesFacturando) {
            return {
                classes: "bd-callout bd-callout-danger border-5 border-top-0 border-bottom-0"
            };
        }
        else if (cMonths >= alertaMesesFacturando) {
            return {
                classes: "bd-callout bd-callout-warning border-5 border-top-0 border-bottom-0"
            };
        }
    }

    //Se verifica la fecha de inicio de asimilados
    if (fechaInvalida.toLocaleString() != fechaInicioAsimilados.toLocaleString()) {
        let cMonths = monthDiff(fechaInicioAsimilados, fechaActual);
        if (cMonths >= alertaMesesAsimilados) {
            return {
                classes: "bd-callout bd-callout-warning border-5 border-top-0 border-bottom-0"
            };
        }
    }

    return {};
}
//Función para procesar la respuesta del servidor al consultar datos
function responseHandler(res) {
    if (typeof res == "string" && res.length >= 1) {
        res = JSON.parse(res);
    }
    $.each(res, function (i, row) {
        row.state = $.inArray(row.id, selections) !== -1
    });



    return res
}
//Función para dar formato a los iconos de operación de los registros
function operateFormatter(value, row, index) {
    let icons = [];

    //Icono Ver
    if (puedeTodo || puedeConsultar || puedeEditar || puedeEliminar || puedeTodoBancos || puedeConsultarBancos || puedeEditarBancos || puedeEliminarBancos) { icons.push(`<li><a class="dropdown-item see" href="#" title="${btnVerTitle}"><i class="bi bi-search"></i> ${btnVerTitle}</a></li>`); }
    //Icono Editar
    if (puedeTodo || puedeEditar || puedeTodoBancos || puedeEditarBancos) { icons.push(`<li><a class="dropdown-item edit" href="#" title="${btnEditarTitle}"><i class="bi bi-pencil-fill"></i> ${btnEditarTitle}</a></li>`); }

    if (icons.length >= 1) {

        return `<div class="dropdown">
                  <button class="btn" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                    <i class="bi bi-three-dots-vertical success"></i>
                  </button>
                  <ul class="dropdown-menu">${icons.join("")}</ul>
                </div>`;
    }
    else {
        return '';
    }
}
//Eventos de los iconos de operación
window.operateEvents = {
    'click .see': function (e, value, row, index) {
        initEmpresaDialog(VER, row);
    },
    'click .edit': function (e, value, row, index) {
        initEmpresaDialog(EDITAR, row);
    }
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
//Función para agregar empresas
function onAgregarClick() {
    let oEmpresaNueva = {
        id: nuevoRegistro,
        razonSocial: "",
        origenId: 0,
        nivelId: 0,
        fechaConstitucion: null,
        fechaInicioOperacion: null,
        fechaInicioFacturacion: null,
        fechaInicioAsimilados: null,
        rfc: "",
        domicilioFiscal: "",
        administrador: "",
        accionista: "",
        correoGeneral: "",
        correoBancos: "",
        correoFiscal: "",
        correoFacturacion: "",
        telefono: "",
        actividadesEconomicas: [],
        objetoSocial: "",
        perfilId: 0,
        perfil: "",
        bancos: [],
        archivos: []
    };

    for (let a in arrTiposDocumentos) {
        oEmpresaNueva.archivos.push({
            nombre: "",
            tipoArchivoId: a,
            extension: "",
            imgSrc: "",
            htmlContainer: "",
            fileSize: 0,
            actualizar: 1
        });
    }

    initEmpresaDialog(NUEVO, oEmpresaNueva);
}
//Función para inicializar la tabla
function initTable() {
    table.bootstrapTable('destroy').bootstrapTable({
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
                title: colNombreHeader,
                field: "razonSocial",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colOrigenHeader,
                field: "origen",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colNivelHeader,
                field: "nivel",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colAdministradorHeader,
                field: "administrador",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colPerfilHeader,
                field: "perfil",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colAccionesHeader,
                field: "operate",
                align: "center",
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
            if (buttonRemove) { buttonRemove.prop('disabled', !table.bootstrapTable('getSelections').length) }

            // save your data, here just save the current page
            selections = getIdSelections()
            // push or splice the selections if you want to save all data selections
        });
    if (buttonRemove) { buttonRemove.click(function () { onDeleteEmpresaClick(selections); }); }
}
//Función para capturar el click de los botones para dar de baja empresas. Ejecuta una llamada ajax para dar de baja empresas.
function onDeleteEmpresaClick(ids = null) {
    askConfirmation(dlgDeleteTitle, dlgDeleteQuestion, function () {
        let oParams = {};

        if (ids != null) { oParams.ids = ids; }
        else { oParams.ids = [document.getElementById("inpEmpresaId").value]; }

        doAjax(
            "/Catalogos/Empresas/DisableEmpresas",
            oParams,
            function (resp) {
                if (resp.tieneError) {
                    showError(dlgDeleteTitle, resp.mensaje);
                    return;
                }

                table.bootstrapTable('remove', {
                    field: 'id',
                    values: oParams.ids
                });

                if (ids != null) {
                    ids = [];
                    selections = null;
                    buttonRemove.prop('disabled', true);
                }

                onBuscarClick();

                showSuccess(dlgDeleteTitle, resp.mensaje);
            }, function (error) {
                showError(dlgDeleteTitle, error);
            },
            postOptions
        );

    });
}
////////////////////////////////

////////////////////////////////
//Funcionalidad Filtrar
////////////////////////////////
//Función para filtrar los datos de la tabla.
function onBuscarClick() {
    let btnBuscar = document.getElementById("btnBuscar");
    let selOrigen = document.getElementById("selFiltroOrigen");
    let selNivel = document.getElementById("selFiltroNivel");
    let inpActividadEconomica = document.getElementById("inpFiltroActividadEconomica");
    let inpRFC = document.getElementById("inpFiltroRFC");

    let oParams = {
        origenId: selOrigen.value == 0 ? null : parseInt(selOrigen.value),
        nivelId: selNivel.value == 0 ? null : parseInt(selNivel.value),
        actividadEconomicaId: (inpActividadEconomica.getAttribute('idselected') || "0") == "0" ? null : parseInt(inpActividadEconomica.getAttribute("idselected")),
        rfc: inpRFC.value.trim().length <= 0 ? null : inpRFC.value
    };

    //Resetea el valor de los filtros.
    document.querySelectorAll("#filtros .form-control").forEach(function (e) { e.value = ""; });
    document.querySelectorAll("#filtros .form-select").forEach(function (e) { e.value = 0; });
    inpActividadEconomica.setAttribute("idselected", 0);

    doAjax(
        "/Catalogos/Empresas/FiltrarEmpresas",
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
                showError(btnBuscar.innerHTML, resp.mensaje);
                return;
            }

            table.bootstrapTable('load', responseHandler(resp.datos));
        }, function (error) {
            showError("Error", error);
        },
        postOptions
    );
}
////////////////////////////////

////////////////////////////////
//Funcionalidad Diálogo Empresa
////////////////////////////////
//Función para inicializar el cuadro de diálogo
function initEmpresaDialog(action, row) {
    $("#theForm .accordion-button").removeClass('collapsed');
    $("#theForm .accordion-button").attr('aria-expanded', true);
    $("#theForm .accordion-collapse").addClass('show');

    let summaryContainer = document.getElementById("saveValidationSummary");
    let idField = document.getElementById("inpEmpresaId");
    let razonSocialField = document.getElementById("inpEmpresaRazonSocial");
    let origenField = document.getElementById("selEmpresaOrigen");
    let nivelField = document.getElementById("selEmpresaNivel");
    let fechaConstitucionField = document.getElementById("inpEmpresaFechaConstitucion");
    let fechaInicioOperacionField = document.getElementById("inpEmpresaFechaInicioOperacion");
    let fechaInicioFacturacionField = document.getElementById("inpEmpresaFechaInicioFacturacion");
    let fechaInicioAsimiladosField = document.getElementById("inpEmpresaFechaInicioAsimilados");
    let rfcField = document.getElementById("inpEmpresaRFC");
    let domicilioFiscalField = document.getElementById("txtEmpresaDomicilioFiscal");
    let administradorField = document.getElementById("inpEmpresaAdministrador");
    let accionistaField = document.getElementById("inpEmpresaAccionista");
    let correoGeneralField = document.getElementById("inpEmpresaCorreoGeneral");
    let correoBancosField = document.getElementById("inpEmpresaCorreoBancos");
    let correoFiscalField = document.getElementById("inpEmpresaCorreoFiscal");
    let correoFacturacionField = document.getElementById("inpEmpresaCorreoFacturacion");
    let telefonoField = document.getElementById("inpEmpresaTelefono");
    let objetoSocialField = document.getElementById("txtEmpresaObjetoSocial");
    let perfilField = document.getElementById("selEmpresaPerfil");
    let oldPasswordField = document.getElementById("inpEmpresaArchivosSATOldPassword");
    let newPasswordField = document.getElementById("inpEmpresaArchivosSATNewPassword");
    let confirmNewPasswordField = document.getElementById("inpEmpresaArchivosSATConfirmNewPassword");

    summaryContainer.innerHTML = "";
    dialogMode = action;
    idField.setAttribute("disabled", true);
    idField.value = row.id;

    if (tieneAccesoEmpresas) {
        razonSocialField.value = row.razonSocial;
        origenField.value = row.origenId;
        nivelField.value = row.nivelId;
        fechaConstitucionField.value = row.fechaConstitucionJS;
        fechaInicioOperacionField.value = row.fechaInicioOperacionJS;
        fechaInicioFacturacionField.value = row.fechaInicioFacturacionJS;
        fechaInicioAsimiladosField.value = row.fechaInicioAsimiladosJS;
        rfcField.value = row.rfc;
        domicilioFiscalField.value = row.domicilioFiscal;
        administradorField.value = row.administrador;
        accionistaField.value = row.accionista;
        correoGeneralField.value = row.correoGeneral;
        correoBancosField.value = row.correoBancos;
        correoFiscalField.value = row.correoFiscal;
        correoFacturacionField.value = row.correoFacturacion;
        telefonoField.value = row.telefono;
        objetoSocialField.value = row.objetoSocial;
        perfilField.value = row.perfilId;
        oldPasswordField.value = "";
        newPasswordField.value = "";
        confirmNewPasswordField.value = "";
        if (row.hasPasswordSAT == "True") {
            $("#divChangePasswordSAT").show();
            $("#divPasswordSAT").hide();

            $("#inpEmpresaArchivosSATOldPassword").parent().parent().show();
        }
        else {
            $("#divChangePasswordSAT").hide();
            $("#divPasswordSAT").show();

            $("#inpEmpresaArchivosSATOldPassword").parent().parent().hide();
        }
    }

    if (action == NUEVO || (row.hasDatosAdicionales || false)) {
        establecerDatosAdicionales(row, action);
        initializeDisableableButtons(false);
        prepareForm(action);
        dlgEmpresaModal.toggle();
        return;
    }

    doAjax(
        "/Catalogos/Empresas/DatosAdicionalesEmpresa",
        {idEmpresa: row.id},
        function (resp) {
            if (resp.tieneError) {
                if (Array.isArray(resp.errores) && resp.errores.length >= 1) {
                    let summary = ``;
                    resp.errores.forEach(function (error) {
                        summary += `<li>${error}</li>`;
                    });
                    summaryContainer.innerHTML += `<ul>${summary}</ul>`;
                }
                showError(btnBuscar.innerHTML, resp.mensaje);
                return;
            }

            //Se establece el row con los datos adicionales.
            if (typeof resp.datos == "string" && resp.datos.length >= 1) { resp.datos = JSON.parse(resp.datos); };
            row.bancos = resp.datos.bancos || [];
            row.archivos = resp.datos.archivos || [];
            row.actividadesEconomicas = resp.datos.actividadesEconomicas || [];
            row.hasDatosAdicionales = true;

            //Actualiza el row para no tener que volver a obtener los datos la próxima vez.
            table.bootstrapTable('updateByUniqueId', { id: row.id, row: row });

            establecerDatosAdicionales(row, action);
            initializeDisableableButtons(action == VER);
            prepareForm(action);
            dlgEmpresaModal.toggle();

        }, function (error) {
            showError("Error", error);
        },
        postOptions
    );
}
//Función para controlar el clic del botón changePasswordSAT para cambiar el password de la firma electrónica
function onChangePasswordSATClick() {
    $("#divChangePasswordSAT").hide();
    $("#divPasswordSAT").show();

    $("#inpEmpresaArchivosSATOldPassword").show();
}
//Función para hablitar / deshabilitar el formulario de empresa
function prepareForm(action) {
    let btnDesactivar = document.getElementById("dlgEmpresaBtnDesactivar");
    let dlgTitle = document.getElementById("dlgEmpresaTitle");
    
    switch (action) {
        case NUEVO:
        case EDITAR:
            if (action == NUEVO) {
                if (btnDesactivar) { btnDesactivar.hidden = true; }
                dlgTitle.innerHTML = dlgNuevoTitle;
            }
            else {
                if (btnDesactivar) { btnDesactivar.hidden = false; }
                dlgTitle.innerHTML = dlgEditarTitle;
            }

            document.querySelectorAll(".formButton").forEach(function (btn) { btn.classList.remove("disabled"); });
            document.querySelectorAll(".formInput, .formSelect").forEach(function (e) { e.removeAttribute("disabled"); });


            break;
        default:
            dlgTitle.innerHTML = dlgVerTitle;

            document.querySelectorAll(".formButton").forEach(function (btn) { btn.classList.add("disabled"); });
            document.querySelectorAll(".formInput, .formSelect").forEach(function (e) { e.setAttribute("disabled", true); });
            break;
    }
}
//Función para establecer los datos adicionales de la empresa
function establecerDatosAdicionales(row, action) {
    if (tieneAccesoEmpresas) {
        // Actividades económicas
        let data = [];
        row.actividadesEconomicas = row.actividadesEconomicas || [];
        row.actividadesEconomicas.forEach(function (p) { data.push(p); });
        initTableActividad(data);

        // Archivos
        let ID_TIPO_ARCHIVO_CER = 6;
        let ID_TIPO_ARCHIVO_KEY = 7;
        $("#bodyArchivos, #bodyArchivosSAT").html("");

        row.archivos = (row.archivos || []).filter(a => parseInt(a.fileSize) >= 1);

        // Ordenar por ID descendente (más reciente primero)
        row.archivos.sort((a, b) => b.id.localeCompare(a.id));

        // Filtrar uno por tipo (más reciente)
        let archivosUnicosPorTipo = [];
        let tiposAgregados = new Set();
        row.archivos.forEach(function (a) {
            if (!tiposAgregados.has(a.tipoArchivoId)) {
                archivosUnicosPorTipo.push(a);
                tiposAgregados.add(a.tipoArchivoId);
            }
        });

        // Renderizar tarjetas por tipo
        Object.keys(arrTiposDocumentos).forEach(function (tipoArchivoIdStr) {
            let tipoArchivoId = parseInt(tipoArchivoIdStr);
            let archivo = archivosUnicosPorTipo.find(x => x.tipoArchivoId === tipoArchivoId);

            let containerClass = "document-container-empty";
            let iconClass = "opacity-25";
            let nameClass = "opacity-25";
            let nameHTML = `<div class="overflowed-text">${emptySelectItemText}</div>`;
            let editDisabled = (puedeTodo || puedeConsultar || puedeEditar || puedeEliminar) ? (action == VER ? "disabled" : "") : ("disabled");
            let itemVerHTML = "";
            let itemEditarHTML = "";
            let itemEliminarHTML = "";
            let menuHTML = "";
            let actualizar = 1;
            let containerName;
            let mimeTypes;
            let allowedExtensions;

            if (tipoArchivoId === ID_TIPO_ARCHIVO_CER) {
                mimeTypes = "application/x-x509-ca-cert";
                containerName = "#bodyArchivosSAT";
                allowedExtensions = ".cer";
            } else if (tipoArchivoId === ID_TIPO_ARCHIVO_KEY) {
                mimeTypes = ".key";
                containerName = "#bodyArchivosSAT";
                allowedExtensions = ".key";
            } else {
                mimeTypes = "image/png, image/jpeg, application/pdf, application/vnd.openxmlformats-officedocument.wordprocessingml.document, application/msword";
                containerName = "#bodyArchivos";
                allowedExtensions = ".png, .jpg, .jpeg, .pdf, .doc, .docx";
            }

            if (archivo) {
                containerClass = "document-container-filled";
                iconClass = "document-icon-filled";
                nameClass = "document-name-filled";
                nameHTML = `<div class="overflowed-text">${archivo.nombre}</div>.<div>${archivo.extension}</div>`;
                actualizar = 0;
            }

            // Mostrar menú incluso sin archivo
            if (puedeTodo || puedeEditar) {
                itemVerHTML = `<li><a class='dropdown-item see ${archivo ? "" : "disabled"}' onclick='onVerDocumentClick(this);' inputName="selector${tipoArchivoId}"><i class='bi bi-search'></i> ${btnVerTitle}</a></li>`;
            }
            if (puedeTodo || puedeEditar || puedeEliminar) {
                itemEditarHTML = `<li><a class='dropdown-item edit ${editDisabled}' onclick='onEditDocumentClick(this);' inputName="selector${tipoArchivoId}"><i class='bi bi-pencil-fill'></i> ${btnEditarTitle}</a></li>`;
            }
            if (puedeTodo || puedeEliminar) {
                itemEliminarHTML = `<li><a class="dropdown-item disableable ${archivo ? "" : "disabled"}" inputName="selector${tipoArchivoId}" onclick="onDeleteClick(this);" sourceId="selector${tipoArchivoId}" sourceLength="${archivo?.fileSize || 0}"><i class="bi bi-x-lg"></i> ${btnEliminarTitle}</a></li>`;
            }

            // Generar menú aunque no haya archivo
            menuHTML = `
            <div class="dropdown">
                <button class="btn p-0 p-lg-2 p-xl-2" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                    <i class="bi bi-three-dots-vertical success"></i>
                </button>
                <ul class="dropdown-menu">
                    ${itemVerHTML}
                    ${itemEditarHTML}
                    ${itemEliminarHTML}
                </ul>
            </div>`;


            $(containerName).append(
                `<div class="col-12 col-xl-6">
                    <div><b>${arrTiposDocumentos[tipoArchivoId]}</b></div>
                    <div id="container${tipoArchivoId}" class="alert mb-2 mt-2 ${containerClass} row me-0">
                        <div id="fileIcon${tipoArchivoId}" class="align-self-center col-1 ${iconClass} p-0 p-lg-2 p-xl-2"><i class='bi bi-file-image' style='font-size:25px'></i></div>
                        <div id="fileName${tipoArchivoId}" class="align-self-center col-10 ${nameClass} p-2" style="display:flex; color:dimgray">${nameHTML}</div>
                        <div class="align-self-center col-1">
                            <input type="file"
                                   actualizar="${actualizar}"
                                   id="selector${tipoArchivoId}"
                                   ${archivo ? `sourceId="${archivo.id}" safeL="${archivo.safeL}" sourceName="${archivo.nombre}.${archivo.extension}" sourceLength="${archivo.fileSize}"` : ""}
                                   tipoArchivoId="${tipoArchivoId}"
                                   containerName="container${tipoArchivoId}"
                                   fileIconName="fileIcon${tipoArchivoId}"
                                   fileNameName="fileName${tipoArchivoId}"
                                   onchange="onDocumentSelectorChanged(this, '${allowedExtensions}');"
                                   accept="${mimeTypes}" hidden />
                            ${menuHTML}
                        </div>
                    </div>
                </div>`
            );
        });
    }

    if (tieneAccesoBancos) {
        $("#bodyBancos").html("");
        row.bancos = row.bancos || [];
        row.bancos.forEach(function (b) { onAgregarBancoClick(b); });
        onLimiteBancoChanged();
    }
}

/*function establecerDatosAdicionales(row, action) {
    
    if (tieneAccesoEmpresas) {
        //Se establecen las actividades económicas
        let data = [];
        row.actividadesEconomicas = row.actividadesEconomicas || [];
        row.actividadesEconomicas.forEach(function (p) { data.push(p); });
        initTableActividad(data);

        //Se establecen los archivos.
        let ID_TIPO_ARCHIVO_CER = 6;
        let ID_TIPO_ARCHIVO_KEY = 7;
        $("#bodyArchivos, #bodyArchivosSAT").html("");
        row.archivos = row.archivos || [];
        let i = 1;
        row.archivos.forEach(function (a) {
            let containerClass = "document-container-empty";
            let iconClass = "opacity-25";
            let nameClass = "opacity-25";
            let nameHTML = `<div class="overflowed-text">${emptySelectItemText}</div>`;
            let editDisabled = (puedeTodo || puedeConsultar || puedeEditar || puedeEliminar) ? (action == VER ? "disabled" : "") : ("disabled");
            let itemVerHTML = "";
            let itemEditarHTML = "";
            let itemEliminarHTML = "";
            let menuHTML = "";
            let actualizar = a.actualizar || 0;
            let containerName;
            let mimeTypes;
            let allowedExtensions;

            if (puedeTodo || puedeEditar || puedeEliminar) {
                itemEditarHTML = `<li><a class='dropdown-item edit ${editDisabled}' onclick='onEditDocumentClick(this);' inputName="selector${a.tipoArchivoId}"><i class='bi bi-pencil-fill'></i> ${btnEditarTitle}</a></li>`;
            }
            if (puedeTodo || puedeEliminar) {
                itemEliminarHTML = `<li><a class="dropdown-item disableable" inputName="selector${a.tipoArchivoId}" onclick="onDeleteClick(this);" sourceId="selector${a.tipoArchivoId}" sourceLength="${a.fileSize}"><i class="bi bi-x-lg"></i> ${btnEliminarTitle}</a></li>`;
            }

            if (parseInt(a.fileSize) >= 1) {
                //Si el tamaño del archivo es mayor o igual a 1 byte, agrega un archivo al DOM con la información.
                containerClass = "document-container-filled";
                iconClass = "document-icon-filled";
                nameClass = "document-name-filled";
                nameHTML = `<div class="overflowed-text">${a.nombre}</div>.<div>${a.extension}</div>`;
                if (puedeTodo || puedeEditar) {
                    itemVerHTML = `<li><a class='dropdown-item see' onclick='onVerDocumentClick(this);' inputName="selector${a.tipoArchivoId}"><i class='bi bi-search'></i> ${btnVerTitle}</a></li>`;
                }
            }

            if (itemVerHTML.length >= 1 || itemEditarHTML.length >= 1 || itemEliminarHTML.length >= 1) {
                menuHTML = `<div class="dropdown">
                            <button class="btn p-0 p-lg-2 p-xl-2" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                <i class="bi bi-three-dots-vertical success"></i>
                            </button>
                            <ul class="dropdown-menu">
                                ${itemVerHTML}
                                ${itemEditarHTML}
                                ${itemEliminarHTML}
                            </ul>
                        </div>`;
            }

            if (a.tipoArchivoId == ID_TIPO_ARCHIVO_CER) {
                mimeTypes = "application/x-x509-ca-cert";
                containerName = "#bodyArchivosSAT";
                allowedExtensions = ".cer";
            }
            else if (a.tipoArchivoId == ID_TIPO_ARCHIVO_KEY) {
                mimeTypes = ".key";
                containerName = "#bodyArchivosSAT";
                allowedExtensions = ".key";
            }
            else {
                mimeTypes = "image/png, image/jpeg, application/pdf, application/vnd.openxmlformats-officedocument.wordprocessingml.document, application/msword"
                containerName = "#bodyArchivos";
                allowedExtensions = ".png, .jpg, .jpeg, .pdf, .doc, .docx";
            }

            $(containerName).append(
                `<div class="col-12 col-xl-6">
                <div><b>${arrTiposDocumentos[a.tipoArchivoId]}</b></div>
                <div id="container${a.tipoArchivoId}" class="alert mb-2 mt-2 ${containerClass} row me-0">
                    <div id="fileIcon${a.tipoArchivoId}" class="align-self-center col-1 ${iconClass} p-0 p-lg-2 p-xl-2"><i class='bi bi-file-image' style='font-size:25px'></i></div>
                    <div id="fileName${a.tipoArchivoId}" class="align-self-center col-10 ${nameClass} p-2" style="display:flex; color:dimgray">${nameHTML}</div>
                    <div class="align-self-center col-1">
                        <input type="file" actualizar="${actualizar}" id="selector${a.tipoArchivoId}" sourceId="${a.id}" safeL="${a.safeL}" sourceName="${a.nombre}.${a.extension}" sourceLength="${a.fileSize}" tipoArchivoId="${a.tipoArchivoId}" containerName="container${a.tipoArchivoId}" fileIconName="fileIcon${a.tipoArchivoId}" fileNameName="fileName${a.tipoArchivoId}" onchange="onDocumentSelectorChanged(this, '${allowedExtensions}');" accept="${mimeTypes}" hidden />
                        ${menuHTML}
                    </div>
                </div>
            </div>`
            );

            i++;
        });
    }

    if (tieneAccesoBancos) {
        //Se establecen los bancos
        $("#bodyBancos").html("");
        row.bancos = row.bancos || [];
        row.bancos.forEach(function (b) { onAgregarBancoClick(b) });
        onLimiteBancoChanged();
    }
}
*/

//Función para habilitar/deshabilitar los botones de visualización en base a si existe contenido o no para visualizar.
function initializeDisableableButtons(isConsulta = false) {
    //Botones de acción de editar y eliminar
    let buttons = document.getElementsByClassName("disableable");

    //Se inicializan los botones de edición
    if (buttons.length >= 1) {
        for (let i = 0; i < buttons.length; i++) {
            let button = buttons[i];
            let inputName = button.getAttribute("inputName");
            let input = document.getElementById(inputName);
            let sourceLength = (input.getAttribute("b64") || "").length;
            if (sourceLength <= 0) { sourceLength = parseInt(input.getAttribute("sourceLength") || "0"); }
            let hasFile = sourceLength >= 1;
            if (hasFile && !isConsulta) {
                button.classList.remove("disabled");
            }
            else {
                button.classList.add("disabled");
            }
        }
    }
}
//Función para capturar el clic en el botón de ver, que dispara la visualización del archivo.
function onVerDocumentClick(button) {
    let inputName = button.getAttribute("inputName");
    let input = document.getElementById(inputName);
    let oParams = {safeL: input.getAttribute("safeL")}

    if (oParams.safeL.length >= 1) { window.open(`/FileViewer?safeL=${encodeURIComponent(oParams.safeL)}`, "_blank"); }
}
//Función para capturar el clic en el botón de edición, que dispara la apertura del selector de archivo.
function onEditDocumentClick(button) {
    let inputName = button.getAttribute("inputName");
    document.getElementById(inputName).click();
}
//Función para capturar el clic en el botón quitar, que elimina el archivo seleccionado.
function onDeleteClick(button) {
    let inputName = button.getAttribute("inputName") || "";

    if (inputName.length >= 1) {
        let fileInput = document.getElementById(inputName);
        let containerName = fileInput.getAttribute("containerName") || "";
        let fileIconName = fileInput.getAttribute("fileIconName");
        let fileNameName = fileInput.getAttribute("fileNameName");
        let container = document.getElementById(containerName);
        let fileIcon = document.getElementById(fileIconName);
        let fileName = document.getElementById(fileNameName);

        //Se elimina menú para ver el documento.
        let menuVer = document.querySelector(`a.dropdown-item.see[inputName='${inputName}']`);
        if (menuVer != null) { menuVer.parentElement.remove(); }

        fileInput.value = null;
        fileInput.setAttribute("b64", "");
        fileInput.setAttribute("sourceLength", "");
        fileInput.setAttribute("actualizar", 1);

        container.classList.remove("document-container-filled");
        container.classList.add("document-container-empty");

        fileIcon.classList.remove("document-icon-filled");
        fileIcon.classList.add("opacity-25");

        fileName.classList.remove("document-name-filled");
        fileName.classList.add("opacity-25");
        fileName.innerHTML = `<div class="overflowed-text">${emptySelectItemText}</div>`;
    }

    initializeDisableableButtons();
}
//Función para capturar el clic en el botón agregar banco, que añade un item de banco para capturar datos.
function onAgregarBancoClick(row = {banco: "", responsable: "", firmante: "", limite: ""}) {
    let btnAgregarBanco = document.getElementById("dlgEmpresaBtnAgregarBanco");
    let currentRows = document.querySelectorAll(".rowBancos").length;

    if (currentRows >= maxBanks) {
        showAlert(btnAgregarBanco.innerHTML, dlgMaxBancosAllowedMessage);
        return;
    }

    currentRows += 1;

    let iconoEliminarItemHTML = ``;
    if (puedeTodoBancos || puedeEliminarBancos) {
        iconoEliminarItemHTML = `<button type="button" class="btn-close formButton" onclick="onEliminarBancoClick(this);"></button>`;
    }
    
    $("#bodyBancos").append(`
        <div class="card mb-3 shadow">    
            <span class="text-end mt-2" data-effect="fadeOut">
                ${iconoEliminarItemHTML}
            </span>
            <div rownumber="${currentRows}" class="col-sm-12 col-md-12 col-lg-12 rowBancos">
				<div class="row">
					<h6 class="col-12"><i>${empresaBancoTitle} ${currentRows}</i></h6>
                    <div class="col-xs-12 col-sm-12 col-md-12 col-lg-3">
						<div class="form-floating mb-3">
							<input id="inpEmpresaBancoNombre${currentRows}" type="text" class="form-control formInput" placeholder="${bancoNombrePlaceholder}" value="${row.banco}" maxlength="40" />
							<label for="inpEmpresaBancoNombre${currentRows}" class="form-label">${bancoNombrePlaceholder}</label>
						</div>
					</div>
                    <div class="col-xs-12 col-sm-12 col-md-12 col-lg-3">
						<div class="form-floating mb-3">
							<input id="inpEmpresaBancoLimite${currentRows}" type="number" class="form-control formInput limiteTransaccionalField" placeholder="${limitePlaceholder}" value="${row.limite}" min="0" max="999999999999" step="0.01" onchange="onLimiteBancoChanged();" />
							<label for="inpEmpresaBancoLimite${currentRows}" class="form-label">${limitePlaceholder}</label>
						</div>
					</div>
                    <div class="col-xs-12 col-sm-12 col-md-12 col-lg-6">
						<div class="form-floating mb-3">
							<input id="inpEmpresaBancoResponsable${currentRows}" type="text" class="form-control formInput" placeholder="${responsablePlaceholder}" value="${row.responsable}" maxlength="40" />
							<label for="inpEmpresaBancoResponsable${currentRows}" class="form-label">${responsablePlaceholder}</label>
						</div>
					</div>
					<div class="col-xs-12 col-sm-12 col-md-12 col-lg-6">
						<div class="form-floating mb-3">
							<input id="inpEmpresaBancoFirmante${currentRows}" type="text" class="form-control formInput" placeholder="${firmantePlaceholder}" value="${row.firmante}" maxlength="40" />
							<label for="inpEmpresaBancoFirmante${currentRows}" class="form-label">${firmantePlaceholder}</label>
						</div>
					</div>
				</div>
			</div>
        </div>
    `);
}
//Función para capturar el cambio de valor en uno de los campos de límite transaccional bancario.
function onLimiteBancoChanged() {
    let total = 0;
    $(".limiteTransaccionalField").each(function (i, e) { total += parseFloat(e.value||"0"); });
    $("#spanTotalTransaccional").text(`$${numFormatter.format(total.toFixed(2))}`);
}
//Función para capturar el clic en el botón eliminar banco, que elimina un item de banco del listado.
function onEliminarBancoClick(button) {
    $(button).closest('.card').remove();
    onLimiteBancoChanged();
}
//Función para mostrar cualquiera de los documentos seleccionados.
function onDocumentSelectorChanged(input, allowedExtensions) {
    if (input.files && (input.files.length || 0) >= 1) {
        let docType = input.files[0].type;
        let docParts = input.files[0].name.split(".");
        let fName = docParts.length >= 1 ? docParts.slice(0, -1).join('.') || "" : "";
        let fExt = docParts.length >= 2 ? docParts[docParts.length - 1] || "" : "";
        let isImg = docType == "image/png" || docType == "image/jpg" || docType == "image/jpeg";
        let isPDF = docType == "application/pdf";
        let isDOCX = docType == "application/msword" || docType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
        let isCER = docType == "application/x-x509-ca-cert";
        let isKEY = fExt == "key";
        let containerName = input.getAttribute("containerName");
        let fileIconName = input.getAttribute("fileIconName");
        let fileNameName = input.getAttribute("fileNameName");
        let container = document.getElementById(containerName);
        let fileIcon = document.getElementById(fileIconName);
        let fileName = document.getElementById(fileNameName);

        if (isImg || isPDF || isDOCX || isCER || isKEY) {
            if ((isImg || isPDF || isDOCX) && input.files[0].size >= maxFileSizeInBytes) {
                input.value = null;
                showAlert(maxFileSizeTitle, `${maxFileSizeMessage} ${maxFileSizeInBytes / oneMegabyteSizeInBytes}Mb`);
                return;
            }
            var reader = new FileReader();
            reader.onload = function () {

                var arrayBuffer = this.result,
                    binary = '',
                    bytes = new Uint8Array(arrayBuffer),
                    len = bytes.byteLength;

                for (var i = 0; i < len; i++) {
                    binary += String.fromCharCode(bytes[i]);
                }
                input.setAttribute("b64", window.btoa(binary));
                input.setAttribute("sourceLength", "0");
                input.setAttribute("actualizar", "1");

                //Se elimina menú para ver el documento, en caso de haber uno anterior.
                let inputName = input.getAttribute("id");
                let menuVer = document.querySelector(`a.dropdown-item.see[inputName='${inputName}']`);
                if (menuVer != null) { menuVer.parentElement.remove(); }

                initializeDisableableButtons();
            }
            reader.readAsArrayBuffer(input.files[0]);

            container.classList.remove("document-container-empty");
            container.classList.add("document-container-filled");

            fileIcon.classList.remove("opacity-25");
            fileIcon.classList.add("document-icon-filled");

            fileName.classList.remove("opacity-25");
            fileName.classList.add("document-name-filled");
            fileName.innerHTML = `<div class="overflowed-text">${fName}</div>.<div>${fExt}</div>`;
        }
        else {
            input.value = null;
            showAlert(fileFormatTitle, `${fileFormatMessage} ${allowedExtensions}` );

            return;
        }
    }
}
//Función para el cierre del cuadro de diálogo
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
//Función para el guardado de información de la empresa
function onGuardarClick() {
    let oParams = {};
    let postURL = "";

    let idField = document.getElementById("inpEmpresaId");
    let btnClose = document.getElementById("dlgEmpresaBtnCancelar");
    let dlgTitle = document.getElementById("dlgEmpresaTitle");
    let summaryContainer = document.getElementById("saveValidationSummary");
    summaryContainer.innerHTML = "";

    if ((puedeEditar && puedeEditarBancos) || (puedeEditar && !puedeEditarBancos)) {
        //Ejecuta la validación
        $("#theForm").validate();
        //Determina los errores
        let valid = $("#theForm").valid();
        //Si la forma no es válida, entonces finaliza.
        if (!valid) { return; }

        let razonSocialField = document.getElementById("inpEmpresaRazonSocial");
        let origenField = document.getElementById("selEmpresaOrigen");
        let nivelField = document.getElementById("selEmpresaNivel");
        let fechaConstitucionField = document.getElementById("inpEmpresaFechaConstitucion");
        let fechaInicioOperacionField = document.getElementById("inpEmpresaFechaInicioOperacion");
        let fechaInicioFacturacionField = document.getElementById("inpEmpresaFechaInicioFacturacion");
        let fechaInicioAsimiladosField = document.getElementById("inpEmpresaFechaInicioAsimilados");
        let rfcField = document.getElementById("inpEmpresaRFC");
        let domicilioFiscalField = document.getElementById("txtEmpresaDomicilioFiscal");
        let administradorField = document.getElementById("inpEmpresaAdministrador");
        let accionistaField = document.getElementById("inpEmpresaAccionista");
        let correoGeneralField = document.getElementById("inpEmpresaCorreoGeneral");
        let correoBancosField = document.getElementById("inpEmpresaCorreoBancos");
        let correoFiscalField = document.getElementById("inpEmpresaCorreoFiscal");
        let correoFacturacionField = document.getElementById("inpEmpresaCorreoFacturacion");
        let telefonoField = document.getElementById("inpEmpresaTelefono");
        let objetoSocialField = document.getElementById("txtEmpresaObjetoSocial");
        let perfilField = document.getElementById("selEmpresaPerfil");
        let oldPasswordField = document.getElementById("inpEmpresaArchivosSATOldPassword");
        let newPasswordField = document.getElementById("inpEmpresaArchivosSATNewPassword");
        let confirmNewPasswordField = document.getElementById("inpEmpresaArchivosSATConfirmNewPassword");

        let activities = [];
        let data = tableActividad.bootstrapTable('getData');
        data.forEach(function (e) { activities.push(e.id); });

        let banks = [];
        $("#bodyBancos .rowBancos").each(function (i, b) {
            let row = b.getAttribute("rownumber");
            let bancoNombreField = document.getElementById(`inpEmpresaBancoNombre${row}`);
            let bancoResponsableField = document.getElementById(`inpEmpresaBancoResponsable${row}`);
            let bancoFirmanteField = document.getElementById(`inpEmpresaBancoFirmante${row}`);
            let bancoLimiteField = document.getElementById(`inpEmpresaBancoLimite${row}`);

            banks.push({
                banco: bancoNombreField.value.trim(),
                responsable: bancoResponsableField.value.trim(),
                firmante: bancoFirmanteField.value.trim(),
                limite: bancoLimiteField.value.trim()
            });
        });

        let files = [];
        $("#bodyArchivos input").each(function (i, a) {
            let id = a.getAttribute("id");
            let file = getFile(id);
            if (file) { files.push(file); }
        });
        $("#bodyArchivosSAT input").each(function (i, a) {
            let id = a.getAttribute("id");
            let file = getFile(id);
            if (file) { files.push(file); }
        });

        oParams = {
            id: idField.value == nuevoRegistro ? 0 : idField.value,
            razonSocial: razonSocialField.value.trim(),
            origenId: origenField.value == 0 ? null : parseInt(origenField.value),
            nivelId: nivelField.value == 0 ? null : parseInt(nivelField.value),
            fechaConstitucion: fechaConstitucionField.value,
            fechaInicioOperacion: fechaInicioOperacionField.value,
            fechaInicioFacturacion: fechaInicioFacturacionField.value,
            fechaInicioAsimilados: fechaInicioAsimiladosField.value,
            rfc: rfcField.value.trim(),
            domicilioFiscal: domicilioFiscalField.value.trim(),
            administrador: administradorField.value.trim(),
            accionista: accionistaField.value.trim(),
            correoGeneral: correoGeneralField.value.trim(),
            correoBancos: correoBancosField.value.trim(),
            correoFiscal: correoFiscalField.value.trim(),
            correoFacturacion: correoFacturacionField.value.trim(),
            telefono: telefonoField.value.trim(),
            actividadesEconomicas: activities,
            objetoSocial: objetoSocialField.value.trim(),
            perfilId: perfilField.value == 0 ? null : parseInt(perfilField.value),
            bancos: banks,
            archivosSATOldPassword: oldPasswordField.value,
            archivosSATNewPassword: newPasswordField.value,
            archivosSATConfirmNewPassword: confirmNewPasswordField.value,
            archivos: files
        };

        postURL = "/Catalogos/Empresas/SaveEmpresa";
    }
    else if (!puedeEditar && puedeEditarBancos) {
        let banks = [];
        $("#bodyBancos .rowBancos").each(function (i, b) {
            let row = b.getAttribute("rownumber");
            let bancoNombreField = document.getElementById(`inpEmpresaBancoNombre${row}`);
            let bancoResponsableField = document.getElementById(`inpEmpresaBancoResponsable${row}`);
            let bancoFirmanteField = document.getElementById(`inpEmpresaBancoFirmante${row}`);
            let bancoLimiteField = document.getElementById(`inpEmpresaBancoLimite${row}`);

            banks.push({
                banco: bancoNombreField.value.trim(),
                responsable: bancoResponsableField.value.trim(),
                firmante: bancoFirmanteField.value.trim(),
                limite: bancoLimiteField.value.trim()
            });
        });

        oParams = {
            id: idField.value,
            bancos: banks
        };

        postURL = "/Catalogos/Empresas/SaveBancosEmpresa";
    }

    doAjax(
        postURL,
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

            onBuscarClick();

            showSuccess(dlgTitle.innerHTML, resp.mensaje);
        }, function (error) {
            showError("Error", error);
        },
        postOptions
    );
}
//Función para dar formato a la descripción de los registros de actividades económicas
function actividadDescFormatter(value, row, index) {
    return `<div>
                <div class="fw-bold">${row.clave}</div>
                ${row.nombre}
            </div>`;
}
//Función para dar formato a los iconos de operación de los registros de actividades económicas
function operateFormatterActividad(value, row, index) {
    if (puedeTodo || puedeEliminar) {
        switch (dialogMode) {
            case NUEVO:
            case EDITAR:
                return `<a class="delete" href="#" title="${btnEliminarTitle}"><i class="bi bi-x btn-close formButton"></i></a>`;
                break;
            default:
                return `<a class="delete" href="#" title="${btnEliminarTitle}"><i class="bi bi-x btn-close formButton disabled"></i></a>`;
                break;
        }
    }
    else {
        return `<a class="delete" href="#" title="${btnEliminarTitle}"><i class="bi bi-x btn-close formButton disabled"></i></a>`;
    }
}
//Eventos de los iconos de operación
window.operateActividadEvents = {
    'click .delete': function (e, value, row, index) {
        onEliminarActividadClick(row.id);
    }
}
//Función para inicializar la tabla de actividades
function initTableActividad(data = null) {
    tableActividad.bootstrapTable('destroy').bootstrapTable({
        locale: cultureName,
        data: data,
        columns: [
            {
                field: "state",
                checkbox: true,
                align: "center",
                valign: "middle"
            },
            {
                title: colActividadEconomicaHeader,
                field: "nombre",
                align: "left",
                valign: "middle",
                sortable: true,
                formatter: actividadDescFormatter
            },
            {
                title: colAccionesHeader,
                field: "operate",
                align: "center",
                width: "100px",
                clickToSelect: false,
                events: window.operateActividadEvents,
                formatter: operateFormatterActividad
            }
        ]
    });
    tableActividad.on('check.bs.table uncheck.bs.table check-all.bs.table uncheck-all.bs.table', function () {
        if ($("#removeActividad")) { $("#removeActividad").prop('disabled', !tableActividad.bootstrapTable('getSelections').length) }
    });
    if ($("#removeActividad")) {
        $("#removeActividad").click(function () {
            askConfirmation(btnEliminarTitle, dlgDeleteElementQuestion, function () {
                let elements = tableActividad.bootstrapTable('getSelections');
                let data = tableActividad.bootstrapTable('getData');
                let newData = [];
                data.forEach(function (d) {
                    let foundElement = elements.find(f => f.id == d.id);
                    if (!foundElement) { newData.push(d); }
                });
                initTableActividad(newData);
                $("#removeActividad").prop('disabled', true);
            });
        });
    }
}
//Función para obtener el archivo de un input
function getFile(inputId) {
    let fileField = document.getElementById(inputId),
        file = null,
        tipoArchivo = fileField.getAttribute("tipoArchivoId"),
        b64 = fileField.getAttribute("b64")||"",
        sourceId = fileField.getAttribute("sourceId") || "", 
        actualizar = parseInt(fileField.getAttribute("actualizar") || "0")

    file = fileField != null ? fileField.files : null;
    if (file) { file = file.length > 0 ? file[0] : null; }

    let oFile = {
        id: sourceId,
        tipoArchivoId: tipoArchivo,
        imgSrc: b64,
        nombre: "",
        extension: ""
    }
    
    if (file) {
        //Si se estableció archivo en pantalla, crea el json con el archivo.
        let fileParts = (file.name || "").split(".");
        oFile.nombre = fileParts.length >= 1 ? fileParts.slice(0, -1).join('.') : "";
        oFile.extension = fileParts.length >= 2 ? fileParts[fileParts.length - 1] : "";
    }

    if (actualizar) { return oFile; }

    return null;
}
//Función para agregar una actividad económica al listado
function onAgregarActividadClick() {
    let actividadField = $(document.getElementById("inpEmpresaActividadEconomica"));
    //Si el campo actividad económica no tiene elemento seleccionado, muestra error.
    if (parseInt(actividadField.attr("idselected") || "0") <= 0) {
        showAlert(msgAgregarActividad, sinActividad);
        return;
    }

    let data = tableActividad.bootstrapTable('getData');
    let listItem = data.find(function (e) { return e.clave == actividadField.data("clave") });
    //Si el elemento ya existe, muestra error.
    if (listItem) {
        showAlert(msgAgregarActividad, actividadRepetida);
        return;
    }

    agregarActividad(actividadField.attr("idselected"), actividadField.data("clave"), actividadField.data("value"));

    actividadField.val("");
    actividadField.attr("idselected", 0);
}
//Función para añadir un elemento al listado de actividades
function agregarActividad(id, clave, descripcion) {
    tableActividad.bootstrapTable('prepend', [{
        id: id,
        clave: clave,
        nombre: descripcion
    }]);
}
//Función para eliminar una actividad económica del listado
function onEliminarActividadClick(id) {
    askConfirmation(btnEliminarTitle, dlgDeleteElementQuestion, function () {
        tableActividad.bootstrapTable('removeByUniqueId', id);
    });
}
/////////////////////

//Funcionalidad Diálogo Actividades Económicas
function initDialogActividad() {
    let actividadField = document.getElementById("inpEmpresaActividadEconomica");

    actividadField.value = "";
    actividadField.setAttribute('idselected', "");
}
////////////////////////////////

////////////////////////////////
//Funcionalidad Diálogo Importar
////////////////////////////////
//Función para el importado del archivo con información de empresas
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
        "/Catalogos/Empresas/ImportarEmpresas",
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

        if(!isExcel){
            input.value = null;
            showAlert(invalidFormatTitle, invalidFormatMsg);
        }
    }
}
////////////////////////////////