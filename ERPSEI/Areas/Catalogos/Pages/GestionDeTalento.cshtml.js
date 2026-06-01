var table;
var buttonRemove;
var selections = [];
var dlgEmpleado = null;
var dlgEmpleadoModal = null;

const NUEVO = 0;
const EDITAR = 1;
const VER = 2;
const maxFileSizeInBytes = 5242880; //5mb = (5 * 1024) * 1024;
const oneMegabyteSizeInBytes = 1048576; // 1mb = (1 * 1024) * 1024
const postOptions = { headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() } }

//Función para inicializar el módulo.
document.addEventListener("DOMContentLoaded", function (event) {
    table = $("#table");
    buttonRemove = $("#remove");
    dlgEmpleado = document.getElementById('dlgEmpleado');
    dlgEmpleadoModal = new bootstrap.Modal(dlgEmpleado, null);
    //Función para limpiar el cuadro de diálogo cuando es cerrado
    dlgEmpleado.addEventListener('hidden.bs.modal', function (event) {
        onCerrarClick();
    });
    //Función para ejecutar acciones posteriores al mostrado del diálogo.
    dlgEmpleado.addEventListener('shown.bs.modal', function (e) {
        //Este evento es necesario para poder mostrar el text area ajustado al tamaño del contenido, basado en el tamaño del scroll.
        calculateTextAreaHeight(document.querySelectorAll("textarea"));
    })

    initTable();

    let btnBuscar = document.getElementById("btnBuscar");
    btnBuscar.click();

    autoCompletar("#inpEmpleadoJefeId");
});
//Función para redimensionar los textareas cada que cambie el tamaño de pantalla.
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
//Función para procesar la respuesta del servidor al consultar datos
function responseHandler(res) {

    if (typeof res == "string" && res.length >= 1) {

        try {
            res = JSON.parse(res);
        }
        catch (e) {

            console.error("JSON inválido");
            console.error(e);

            console.log("Posición:", e.message);

            let posMatch = e.message.match(/position (\d+)/);

            if (posMatch) {

                let pos = parseInt(posMatch[1]);

                console.log("ANTES:");
                console.log(res.substring(pos - 200, pos));

                console.log("DESPUÉS:");
                console.log(res.substring(pos, pos + 200));
            }

            throw e;
        }
    }

    $.each(res, function (i, row) {
        row.state = $.inArray(row.id, selections) !== -1;
    });

    return res;
}

//Función para dar formato al detalle de empleado
function detailFormatter(index, row) {
    return `<div class="container-fluid alert alert-primary mb-0">
                <div class="row">
                    <div class="col-sm-12 col-md-6 col-lg-3 mb-2">
						<i class="bi bi-cake2-fill"></i> <span><b>${colFechaNacimientoHeader}: </b>${row.fechaNacimiento}</span>
					</div>
					<div class="col-sm-12 col-md-6 col-lg-3 mb-2">
						<i class="bi bi-telephone-fill"> </i><span><b>${colTelefonoHeader}: </b>${row.telefono}</span>
					</div>
					<div class="col-sm-12 col-md-6 col-lg-3 mb-2">
						<i class="bi bi-building-fill"></i> <span><b>${colOficinaHeader}: </b>${row.oficina}</span>
					</div>
					<div class="col-sm-12 col-md-6 col-lg-3 mb-2">
						<i class="bi bi-yin-yang"></i> <span><b>${colEstadoCivilHeader}: </b>${row.estadoCivil}</span>
					</div>
					<div class="col-12 mb-2">
						<i class="bi bi-house-door-fill"> </i><span><b>${colDireccionHeader}: </b>${row.direccion}</span>
					</div>
                </div>
            </div>`;
}
//Función para dar formato a los iconos de operación de los registros
function operateFormatter(value, row, index) {
    let icons = [];
    //¿El empleado tiene id de usuario?
    let hasUserId = (row.usuarioId || "").length >= 1;
    //El empleado si tiene usuario, pero ¿ese usuario es válido?
    let hasUsuarioValido = parseInt((row.usuarioValido || "0")) >= 1;
    //Si el empleado no tiene id de usuario o tiene un usuario inválido, entonces se habilita la opción invitar.
    let canInvite = !hasUserId || !hasUsuarioValido;

    //Icono Ver
    icons.push(`<li><a class="dropdown-item see" href="#" title="${btnVerTitle}"><i class="bi bi-search"></i> ${btnVerTitle}</a></li>`);
    //Icono Editar
    icons.push(`<li><a class="dropdown-item edit" href="#" title="${btnEditarTitle}"><i class="bi bi-pencil-fill"></i> ${btnEditarTitle}</a></li>`);

    if (canInvite) {
        //Icono Invitar
        icons.push(`<li><a class="dropdown-item invite" href="#" title="${btnInvitarTitle}"><i class="bi bi-person-fill-add"></i> ${btnInvitarTitle}</a></li>`);
    }

    return `<div class="dropdown">
              <button class="btn" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                <i class="bi bi-three-dots-vertical success"></i>
              </button>
              <ul class="dropdown-menu">${icons.join("")}</ul>
            </div>`;
}
//Eventos de los iconos de operación
window.operateEvents = {
    'click .see': function (e, value, row, index) {
        initEmpleadoDialog(VER, row);
    },
    'click .edit': function (e, value, row, index) {
        initEmpleadoDialog(EDITAR, row);
    },
    'click .invite': function (e, value, row, index) {
        invitarEmpleado(row.id);
    }
}
//Función para invitar a los empleados al sistema mediante un correo electrónico.
function invitarEmpleado(rowId) {
    let oParams = {id: rowId};

    doAjax(
        "/Catalogos/GestionDeTalento/InvitarEmpleado",
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
                showError(btnInvitarTitle, resp.mensaje);
                return;
            }

            showSuccess(btnInvitarTitle, resp.mensaje);
        }, function (error) {
            showError("Error", error);
        },
        postOptions
    );
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
//Función para agregar empleados
function onAgregarClick() {
    let oEmpleadoNuevo = {
        id: nuevoRegistro,
        nombre: "",
        nombrePreferido: "",
        segundoNombre: "",
        apellidoPaterno: "",
        apellidoMaterno: "",
        fechaNacimiento: null,
        fechaIngreso: null,
        direccion: "",
        telefono: "",
        email: "",
        generoId: 0,
        estadoCivilId: 0,
        puestoId: 0,
        areaId: 0,
        subareaId: 0,
        oficinaId: 0,
        jefeId: 0,
        jefe: "",
        curp: "",
        rfc: "",
        nss: "",
        horarioId: 0,
        nombreContacto1: "",
        telefonoContacto1: "",
        nombreContacto2: "",
        telefonoContacto2: "",
        contactosEmergencia: [],
        archivos: []
    };

    for (let a in arrTiposDocumentos) {
        oEmpleadoNuevo.archivos.push({
            nombre: "",
            tipoArchivoId: a,
            extension: "",
            imgSrc: "",
            htmlContainer: "",
            fileSize: 0,
            actualizar: 1
        });
    }

    initEmpleadoDialog(NUEVO, oEmpleadoNuevo);
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
                field: "nombreCompleto",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colFechaIngresoHeader,
                field: "fechaIngreso",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colPuestoHeader,
                field: "puesto",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colAreaHeader,
                field: "area",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colTelefonoHeader,
                field: "telefono",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colCorreoHeader,
                field: "email",
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
            buttonRemove.prop('disabled', !table.bootstrapTable('getSelections').length)

            // save your data, here just save the current page
            selections = getIdSelections()
            // push or splice the selections if you want to save all data selections
        })
    table.on('all.bs.table', function (e, name, args) {
        console.log(name, args)
    })
    buttonRemove.click(function () { onDeleteEmpleadoClick(selections); });
}
//Función para capturar el click de los botones para dar de baja empleados. Ejecuta una llamada ajax para dar de baja empleados.
function onDeleteEmpleadoClick(ids = null) {
    askConfirmation(dlgDeleteTitle, dlgDeleteQuestion, function () {
        let oParams = {};

        if (ids != null) { oParams.ids = ids; }
        else { oParams.ids = [document.getElementById("inpEmpleadoId").value]; }

        doAjax(
            "/Catalogos/GestionDeTalento/DisableEmpleados",
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

//Funcionalidad Filtrar

//Función para capturar el cambio de área. Muestra solo las subareas que corresponden al área seleccionada.
function onAreaChanged(areaField) {
    let id = areaField.getAttribute('id');
    let strSubareaField = "";
    let subareaField = null;

    if (id == "selFiltroArea") {
        strSubareaField = "selFiltroSubarea";
    }
    else {
        strSubareaField = "selEmpleadoSubareaId";
    }

    subareaField = document.getElementById(strSubareaField);

    //Establece la selección por default.
    subareaField.value = 0;

    //Oculta todas las opciones, excepto la opción "Seleccione..."
    let subareaOptions = document.querySelectorAll(`#${strSubareaField} option`);
    subareaOptions.forEach(function (o) { if (o.value >= 1) { o.style.display = 'none'; } });

    //Deshabilita el campo de selección
    subareaField.setAttribute('disabled', true);

    //Muestra solo las opciones que correspondan al área seleccionada.
    subareaOptions = document.querySelectorAll(`#${strSubareaField} option[areaid='${areaField.value}']`);
    //Si hay subareas...
    if (subareaOptions.length >= 1) {
        //Muestra las subareas
        subareaOptions.forEach(function (o) { o.style.display = 'block'; });

        //Habilita el campo de selección
        subareaField.removeAttribute('disabled');
    }
    else {
        //Habilita el campo de selección solo para el filtro y solo cuando no se tenga seleccionada ningún área.
        if (id == "selFiltroArea" && areaField.value == "0") {
            subareaField.removeAttribute('disabled');
            subareaOptions = document.querySelectorAll(`#${strSubareaField} option`);
            subareaOptions.forEach(function (o) { if (o.value >= 1) { o.style.display = 'block'; } });
        }
    }
}
//Función para filtrar los datos de la tabla.
function onBuscarClick() {
    let btnBuscar = document.getElementById("btnBuscar");
    let inpFechaIngresoIni = document.getElementById("inpFiltroFechaIngresoInicio");
    let inpFechaIngresoFin = document.getElementById("inpFiltroFechaIngresoFin");
    let inpFechaNacimientoIni = document.getElementById("inpFiltroFechaNacimientoInicio");
    let inpFechaNacimientoFin = document.getElementById("inpFiltroFechaNacimientoFin");
    let selPuesto = document.getElementById("selFiltroPuesto");
    let selArea = document.getElementById("selFiltroArea");
    let selSubarea = document.getElementById("selFiltroSubarea");
    let selOficina = document.getElementById("selFiltroOficina");

    let oParams = {
        FechaIngresoInicio: inpFechaIngresoIni.value,
        FechaIngresoFin: inpFechaIngresoFin.value,
        FechaNacimientoInicio: inpFechaNacimientoIni.value,
        FechaNacimientoFin: inpFechaNacimientoFin.value,
        PuestoId: selPuesto.value == 0 ? null : parseInt(selPuesto.value),
        AreaId: selArea.value == 0 ? null : parseInt(selArea.value),
        SubareaId: selSubarea.value == 0 ? null : parseInt(selSubarea.value),
        OficinaId: selOficina.value == 0 ? null : parseInt(selOficina.value)
    };

    //Resetea el valor de los filtros.
    document.querySelectorAll("#filtros .form-control").forEach(function (e) { e.value = ""; });
    document.querySelectorAll("#filtros .form-select").forEach(function (e) { e.value = 0; });

    doAjax(
        "/Catalogos/GestionDeTalento/FiltrarEmpleados",
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
//Funcionalidad Diálogo Empleado
////////////////////////////////
//Función para inicializar el cuadro de diálogo
function initEmpleadoDialog(action, row) {
    let idField = document.getElementById("inpEmpleadoId");
    let primerNombreField = document.getElementById("inpEmpleadoPrimerNombre");
    let nombrePreferidoField = document.getElementById("inpEmpleadoNombrePreferido");
    let apellidoPaternoField = document.getElementById("inpEmpleadoApellidoPaterno");
    let apellidoMaternoField = document.getElementById("inpEmpleadoApellidoMaterno");
    let fechaNacimientoField = document.getElementById("inpEmpleadoFechaNacimiento");
    let telefonoField = document.getElementById("inpEmpleadoTelefono");
    let generoField = document.getElementById("selEmpleadoGeneroId");
    let estadoCivilIdField = document.getElementById("selEmpleadoEstadoCivilId");
    let direccionField = document.getElementById("txtEmpleadoDireccion");
    let puestoField = document.getElementById("selEmpleadoPuestoId");
    let areaField = document.getElementById("selEmpleadoAreaId");
    let subareaField = document.getElementById("selEmpleadoSubareaId");
    let oficinaField = document.getElementById("selEmpleadoOficinaId");
    let jefeField = document.getElementById("inpEmpleadoJefeId");
    let fechaIngresoField = document.getElementById("inpEmpleadoFechaIngreso");
    let emailField = document.getElementById("inpEmpleadoEmail");
    let curpField = document.getElementById("inpEmpleadoCURP");
    let rfcField = document.getElementById("inpEmpleadoRFC");
    let nssField = document.getElementById("inpEmpleadoNSS");
    let horarioField = document.getElementById("selEmpleadoHorarioId");

    let btnDesactivar = document.getElementById("dlgEmpleadoBtnDesactivar");
    let dlgTitle = document.getElementById("dlgEmpleadoTitle");
    let summaryContainer = document.getElementById("saveValidationSummary");
    summaryContainer.innerHTML = "";

    idField.setAttribute("disabled", true);
    switch (action) {
        case NUEVO:
        case EDITAR:
            if (action == NUEVO) {
                btnDesactivar.hidden = true;
                dlgTitle.innerHTML = dlgNuevoTitle;
            }
            else {
                btnDesactivar.hidden = false;
                dlgTitle.innerHTML = dlgEditarTitle;
            }

            document.querySelectorAll(".formButton").forEach(function (btn) { btn.classList.remove("disabled"); });
            document.querySelectorAll(".formInput, .formSelect").forEach(function (e) { e.removeAttribute("disabled"); });

            if (action == NUEVO) { subareaField.setAttribute("disabled", true); }

            break;
        default:
            dlgTitle.innerHTML = dlgVerTitle;

            document.querySelectorAll(".formButton").forEach(function (btn) { btn.classList.add("disabled"); });
            document.querySelectorAll(".formInput, .formSelect").forEach(function (e) { e.setAttribute("disabled", true); });
            break;
    }

    idField.value = row.id;
    primerNombreField.value = row.nombre;
    nombrePreferidoField.value = row.nombrePreferido;
    apellidoPaternoField.value = row.apellidoPaterno;
    apellidoMaternoField.value = row.apellidoMaterno;
    fechaNacimientoField.value = row.fechaNacimientoJS;
    telefonoField.value = row.telefono;
    generoField.value = row.generoId;
    estadoCivilIdField.value = row.estadoCivilId;
    direccionField.value = row.direccion;
    puestoField.value = row.puestoId;
    areaField.value = row.areaId;
    if (action == EDITAR) { onAreaChanged(areaField); }
    subareaField.value = row.subareaId;
    oficinaField.value = row.oficinaId;
    jefeField.setAttribute("idselected", row.jefeId);
    fechaIngresoField.value = row.fechaIngresoJS;
    emailField.value = row.email;
    curpField.value = row.curp;
    rfcField.value = row.rfc;
    nssField.value = row.nss;
    horarioField.value = row.horarioId;

    if (action == NUEVO || (row.hasDatosAdicionales||false)) {
        establecerDatosAdicionales(row, action);
        initializeDisableableButtons(false);
        dlgEmpleadoModal.toggle();
        return;
    }

    doAjax(
        "/Catalogos/GestionDeTalento/DatosAdicionalesEmpleado",
        {idEmpleado: row.id},
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
            row.jefeId = resp.datos.jefeId || 0;
            row.jefe = resp.datos.jefe || "";
            row.contactosEmergencia = resp.datos.contactosEmergencia || [];
            row.archivos = resp.datos.archivos || [];
            row.hasDatosAdicionales = true;

            //Actualiza el row para no tener que volver a obtener los datos la próxima vez.
            table.bootstrapTable('updateByUniqueId', { id: row.id, row: row });

            establecerDatosAdicionales(row, action);
            initializeDisableableButtons(action == VER);
            dlgEmpleadoModal.toggle();

        }, function (error) {
            showError("Error", error);
        },
        postOptions
    );
}
//Función para establecer los datos adicionales del empleado
function establecerDatosAdicionales(row, action) {
    let picField = document.getElementById("profilePicContainer");
    let picSelector = document.getElementById("profilePicSelector");
    let nombreContacto1Field = document.getElementById("inpEmpleadoNombreContacto1");
    let telefonoContacto1Field = document.getElementById("inpEmpleadoTelefonoContacto1");
    let nombreContacto2Field = document.getElementById("inpEmpleadoNombreContacto2");
    let telefonoContacto2Field = document.getElementById("inpEmpleadoTelefonoContacto2");
    let jefeField = document.getElementById("inpEmpleadoJefeId");

    //Se establece la información del Jefe
    jefeField.setAttribute("idselected", row.jefeId);
    jefeField.value = row.jefe;

    //Se establecen los contactos de emergencia
    row.contactosEmergencia = row.contactosEmergencia || [];
    nombreContacto1Field.value = "";
    telefonoContacto1Field.value = "";
    nombreContacto2Field.value = "";
    telefonoContacto2Field.value = "";
    if (row.contactosEmergencia.length >= 1) {
        nombreContacto1Field.value = row.contactosEmergencia[0].nombre || "";
        telefonoContacto1Field.value = row.contactosEmergencia[0].telefono || "";
    }
    if (row.contactosEmergencia.length >= 2) {
        nombreContacto2Field.value = row.contactosEmergencia[1].nombre || "";
        telefonoContacto2Field.value = row.contactosEmergencia[1].telefono || "";
    }

    //Se establecen los archivos.
    $("#bodyArchivos").html("");
    row.archivos = row.archivos || [];
    let i = 1;
    row.archivos.forEach(function (a) {

        if (a.tipoArchivoId == 1) {
            let srcElements = (a.imgSrc || "").split(",");
            let b64 = srcElements.length >= 1 ? srcElements[1] || "" : "";
            //Si el tipo de archivo es la foto de perfil, se establece en el contenedor directamente.
            if ((a.fileSize||0) <= 0) { a.imgSrc = "/img/default_profile_pic.png"; }
            picField.setAttribute('src', a.imgSrc);
            picSelector.setAttribute('actualizar', a.actualizar);
            picSelector.setAttribute('sourceLength', a.fileSize);
            picSelector.setAttribute('sourceName', `${a.nombre}.${a.extension}`);
            picSelector.setAttribute('sourceId', a.id);
            picSelector.setAttribute('b64', b64);
        }
        else {
            let containerClass = "document-container-empty";
            let iconClass = "opacity-25";
            let nameClass = "opacity-25";
            let nameHTML = `<div class="overflowed-text">${emptySelectItemText}</div>`;
            let editDisabled = action == VER ? "disabled" : "";
            let itemVerHTML = "";
            let actualizar = a.actualizar || 0;

            if (parseInt(a.fileSize) >= 1) {
                //Si el tamaño del archivo es mayor o igual a 1 byte, agrega un archivo al DOM con la información.
                containerClass = "document-container-filled";
                iconClass = "document-icon-filled";
                nameClass = "document-name-filled";
                nameHTML = `<div class="overflowed-text">${a.nombre}</div>.<div>${a.extension}</div>`;
                itemVerHTML = `<li><a class='dropdown-item see' onclick='onVerDocumentClick(this);' inputName="selector${a.tipoArchivoId}"><i class='bi bi-search'></i> ${btnVerTitle}</a></li>`;
            }

            $("#bodyArchivos").append(
                `<div class="col-12 col-xl-6">
                    <div><b>${arrTiposDocumentos[a.tipoArchivoId]}</b></div>
                    <div id="container${a.tipoArchivoId}" class="alert mb-2 mt-2 ${containerClass} row me-0">
                        <div id="fileIcon${a.tipoArchivoId}" class="align-self-center col-1 ${iconClass} p-0 p-lg-2 p-xl-2"><i class='bi bi-file-image' style='font-size:25px'></i></div>
                        <div id="fileName${a.tipoArchivoId}" class="align-self-center col-10 ${nameClass} p-2" style="display:flex; color:dimgray">${nameHTML}</div>
                        <div class="align-self-center col-1">
                            <input type="file" actualizar="${actualizar}" id="selector${a.tipoArchivoId}" sourceId="${a.id}" safeL="${a.safeL}" sourceName="${a.nombre}.${a.extension}" sourceLength="${a.fileSize}" tipoArchivoId="${a.tipoArchivoId}" containerName="container${a.tipoArchivoId}" fileIconName="fileIcon${a.tipoArchivoId}" fileNameName="fileName${a.tipoArchivoId}" onchange="onDocumentSelectorChanged(this);" accept="image/png, image/jpeg, application/pdf" hidden />
                            <div class="dropdown">
                                <button class="btn p-0 p-lg-2 p-xl-2" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                    <i class="bi bi-three-dots-vertical success"></i>
                                </button>
                                <ul class="dropdown-menu">
                                    ${itemVerHTML}
                                    <li><a class='dropdown-item edit ${editDisabled}' onclick='onEditDocumentClick(this);' inputName="selector${a.tipoArchivoId}"><i class='bi bi-pencil-fill'></i> ${btnEditarTitle}</a></li>
                                    <li><a class="dropdown-item disableable" inputName="selector${a.tipoArchivoId}" onclick="onDeleteClick(this);" sourceId="selector${a.tipoArchivoId}" sourceLength="${a.fileSize}"><i class="bi bi-x-lg"></i> ${btnEliminarTitle}</a></li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>`
            );
        }
        i++;
    });
}
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
    let oParams = { safeL: input.getAttribute("safeL") }

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
        fileInput.setAttribute("actualizar", 1)

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
//Función para mostrar la foto de perfil seleccionada.
function onProfilePicSelectorChanged(input) {
    if (input.files && (input.files.length || 0) >= 1) {
        if (input.files[0].size >= maxFileSizeInBytes) {
            input.value = null;
            showAlert(maxFileSizeTitle, `${maxFileSizeMessage} ${maxFileSizeInBytes / oneMegabyteSizeInBytes}Mb`);
            return;
        }
        let imgType = input.files[0].type;
        if (imgType == "image/png" || imgType == "image/jpg" || imgType == "image/jpeg") {
            document.getElementById("profilePicContainer").src = window.URL.createObjectURL(input.files[0]);
            var reader = new FileReader();
            reader.onload = function () {

                var arrayBuffer = this.result,
                    binary = '',
                    bytes = new Uint8Array(arrayBuffer),
                    len = bytes.byteLength;

                for (var i = 0; i < len; i++) {
                    binary += String.fromCharCode(bytes[i]);
                }
                document.getElementById("profilePicSelector").setAttribute("b64", window.btoa(binary));
                document.getElementById("profilePicSelector").setAttribute("sourceLength", "0");
                document.getElementById("profilePicSelector").setAttribute("actualizar", "1");
            }
            reader.readAsArrayBuffer(input.files[0]);
        }
        else {
            showAlert(fileFormatTitle, fileFormatMessage);
        }
    }
}
//Función para mostrar cualquiera de los documentos seleccionados.
function onDocumentSelectorChanged(input) {
    if (input.files && (input.files.length || 0) >= 1) {
        if (input.files[0].size >= maxFileSizeInBytes) {
            input.value = null;
            showAlert(maxFileSizeTitle, `${maxFileSizeMessage} ${maxFileSizeInBytes / oneMegabyteSizeInBytes}Mb`);
            return;
        }
        let docType = input.files[0].type;
        let docParts = input.files[0].name.split(".");
        let fName = docParts.length >= 1 ? docParts.slice(0, -1).join(".") || "" : "";
        let fExt = docParts.length >= 2 ? docParts[docParts.length - 1] || "" : "";
        /*let fileSize = (input.files[0].size / 1000000).toFixed(2);*/
        let isImg = docType == "image/png" || docType == "image/jpg" || docType == "image/jpeg";
        let isPDF = docType == "application/pdf";
        let containerName = input.getAttribute("containerName");
        let fileIconName = input.getAttribute("fileIconName");
        let fileNameName = input.getAttribute("fileNameName");
        /*let showerName = containerName + "_children";*/
        let container = document.getElementById(containerName);
        let fileIcon = document.getElementById(fileIconName);
        let fileName = document.getElementById(fileNameName);

        if (isImg || isPDF) {
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
            showAlert(fileFormatTitle, fileFormatMessage);

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
//Función para el guardado de información del empleado
function onGuardarClick() {
    //Ejecuta la validación
    $("#theForm").validate();
    //Determina los errores
    let valid = $("#theForm").valid();
    //Si la forma no es válida, entonces finaliza.
    if (!valid) { return; }

    let btnClose = document.getElementById("dlgEmpleadoBtnCancelar");

    let idField = document.getElementById("inpEmpleadoId");
    let primerNombreField = document.getElementById("inpEmpleadoPrimerNombre");
    let nombrePreferidoField = document.getElementById("inpEmpleadoNombrePreferido");
    let apellidoPaternoField = document.getElementById("inpEmpleadoApellidoPaterno");
    let apellidoMaternoField = document.getElementById("inpEmpleadoApellidoMaterno");
    let fechaNacimientoField = document.getElementById("inpEmpleadoFechaNacimiento");
    let telefonoField = document.getElementById("inpEmpleadoTelefono");
    let generoField = document.getElementById("selEmpleadoGeneroId");
    let estadoCivilIdField = document.getElementById("selEmpleadoEstadoCivilId");
    let direccionField = document.getElementById("txtEmpleadoDireccion");
    let puestoField = document.getElementById("selEmpleadoPuestoId");
    let areaField = document.getElementById("selEmpleadoAreaId");
    let subareaField = document.getElementById("selEmpleadoSubareaId");
    let oficinaField = document.getElementById("selEmpleadoOficinaId");
    let jefeField = document.getElementById("inpEmpleadoJefeId");
    let fechaIngresoField = document.getElementById("inpEmpleadoFechaIngreso");
    let emailField = document.getElementById("inpEmpleadoEmail");
    let curpField = document.getElementById("inpEmpleadoCURP");
    let rfcField = document.getElementById("inpEmpleadoRFC");
    let nssField = document.getElementById("inpEmpleadoNSS");
    let horarioField = document.getElementById("selEmpleadoHorarioId");
    let nombreContacto1Field = document.getElementById("inpEmpleadoNombreContacto1");
    let telefonoContacto1Field = document.getElementById("inpEmpleadoTelefonoContacto1");
    let nombreContacto2Field = document.getElementById("inpEmpleadoNombreContacto2");
    let telefonoContacto2Field = document.getElementById("inpEmpleadoTelefonoContacto2");

    let dlgTitle = document.getElementById("dlgEmpleadoTitle");
    let summaryContainer = document.getElementById("saveValidationSummary");
    summaryContainer.innerHTML = "";

    let files = [];
    let profilePic = getFile("profilePicSelector");
    if (profilePic) { files.push(profilePic); }
    $("#bodyArchivos input").each(function (i, a) {
        let id = a.getAttribute("id");
        let file = getFile(id);
        if (file) { files.push(file); }
    });

    let oParams = {
        id: idField.value == nuevoRegistro ? 0 : idField.value,
        nombre: primerNombreField.value.trim(),
        nombrePreferido: nombrePreferidoField.value.trim(),
        apellidoPaterno: apellidoPaternoField.value.trim(),
        apellidoMaterno: apellidoMaternoField.value.trim(),
        fechaNacimiento: fechaNacimientoField.value,
        telefono: telefonoField.value.trim(),
        generoId: generoField.value == 0 ? null : parseInt(generoField.value),
        estadoCivilId: estadoCivilIdField.value == 0 ? null : parseInt(estadoCivilIdField.value),
        direccion: direccionField.value.trim(),
        puestoId: puestoField.value == 0 ? null : parseInt(puestoField.value),
        areaId: areaField.value == 0 ? null : parseInt(areaField.value),
        subareaId: subareaField.value == 0 ? null : parseInt(subareaField.value),
        oficinaId: oficinaField.value == 0 ? null : parseInt(oficinaField.value),
        jefeId: (jefeField.getAttribute('idselected') || "0") == "0" ? null : parseInt(jefeField.getAttribute("idselected")),
        fechaIngreso: fechaIngresoField.value,
        email: emailField.value.trim(),
        curp: curpField.value.trim(),
        rfc: rfcField.value.trim(),
        nss: nssField.value.trim(),
        horarioId: parseInt(horarioField.value),
        nombreContacto1: nombreContacto1Field.value.trim(),
        telefonoContacto1: telefonoContacto1Field.value.trim(),
        nombreContacto2: nombreContacto2Field.value.trim(),
        telefonoContacto2: telefonoContacto2Field.value.trim(),
        archivos: files
    };

    doAjax(
        "/Catalogos/GestionDeTalento/SaveEmpleado",
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
        oFile.nombre = fileParts.length >= 1 ? fileParts.slice(0, -1).join(".") : "";
        oFile.extension = fileParts.length >= 2 ? fileParts[fileParts.length - 1] : "";
    }

    if (actualizar) { return oFile; }

    return null;
}
////////////////////////////////

////////////////////////////////
//Funcionalidad Diálogo Importar
////////////////////////////////
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
        "/Catalogos/GestionDeTalento/ImportarEmpleados",
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