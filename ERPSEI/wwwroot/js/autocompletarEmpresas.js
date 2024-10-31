// Función para el llenado de campos a autocompletar
function autoCompletarEmp(selector, oExtend) {
    const classIcon = typeof oExtend != 'undefined' && typeof oExtend.icon === 'boolean' && oExtend.icon === true ? 'ui-icon ui-autocomplete-icon' : '';
    selector = selector || 'input[area][module][source]';
    oExtend = $.extend({ select: null, change: null }, oExtend);

    // Muestra la lista de sugerencias
    return $(selector).autocomplete({
        position: { collision: "flip" },
        minLength: 3,
        search: function (event, ui) {
            if ($(event.target).val().trim().length < $(this).autocomplete('option', 'minLength')) {
                $(event.target).autocomplete('close');
                return false;
            }
            $(event.target).addClass('autocompleteLoading');
        },
        source: function (request, response) {
            let itemDOM = $(this.element);
            itemDOM.attr({ idselected: '' });

            // url y parametros
            let area = itemDOM.attr("area");
            let module = itemDOM.attr("module");
            let source = itemDOM.attr("source");
            let url = `/${area}/${module}/${source}`;

            // Arma objeto de datos para solicitud AJAX
            let oDatos = { texto: request.term };

            let _filtro = itemDOM.attr('filtro');
            // Verifica si requiere datos adicionales para el filtro
            if (typeof _filtro != 'undefined' && _filtro != null) {
                _filtro = _filtro.split(',');
                if (_filtro.length >= 1) // Agrega valores adicionales de los atributos data del elemento
                    _filtro.forEach(dataName => oDatos[dataName] = itemDOM.data(dataName));
            }

            // Deshabilita componentes de la UI para evitar interacción del usuario durante la petición
            toDisable('.ui-dialog-titlebar-close, .ui-disabled-on-suggest');
            let objDefaults = {
                url: url,
                data: oDatos,
                headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
                type: 'POST',
                dataType: "json",
                success: function (resp) {
                    toEnable('.ui-dialog-titlebar-close, .ui-disabled-on-suggest');
                    $(itemDOM).removeClass('autocompleteLoading');
                    try {
                        if (resp.tieneError) {
                            showError('', resp.mensaje);
                            return;
                        }
                        if (typeof resp.datos == "string" && resp.datos.length >= 1) { resp.datos = JSON.parse(resp.datos); };
                        if (resp.datos.length == 0) resp.datos.push({ id: -1, value: 'Sin Coincidencias...', label: 'Sin Coincidencias...' });

                        response(resp.datos);
                    } catch (error) {
                        console.warn(error);
                    }
                },
                error: function (xhr, estado, errdata) {
                    console.warn(estado);
                    console.warn(errdata);
                    $(itemDOM).removeClass('autocompleteLoading');
                    showError([JSON.stringify(estado), JSON.stringify(errdata)].join());
                    return false;
                }
            };
            $.post(objDefaults);
        },
        select: function (event, ui) {
            let itemDOM = $(this);
            // En caso de no existir coincidencias
            if (ui.item.id == -1) {
                itemDOM.val('').attr({ idselected: '' });
                ui.item.value = '';
                return false;
            }

            // Recupera el ID del renglón seleccionado
            itemDOM.attr({ idselected: ui.item.id });

            // Asigna atributos 'data' al elemento con atributos del item seleccionado
            itemDOM.data(ui.item);

            // Llama a la función para consultar comprobantes por RFC
            onConsultarComprobantesClick(ui.item.value);  // Pasar el RFC como parámetro

            // Invoca la función personalizada para procesar el elemento seleccionado
            if (typeof oExtend.select == 'function' || typeof eval(itemDOM.attr('onselect')) == 'function') {
                let exec = typeof oExtend.select == 'function' ? oExtend.select : eval(itemDOM.attr('onselect'));
                let respuesta = exec(itemDOM, ui.item);
                if (respuesta === false)
                    return false;
            }
        },
        change: function (event, ui) {
            let itemDOM = $(this);
            if (itemDOM.val().trim() == '' || (itemDOM.attr('idselected') || '') == '' || ui.item == null)
                itemDOM.val('').attr({ idselected: '' });

            if (typeof oExtend.change == 'function') oExtend.change(itemDOM, ui.item);
        }
    }).addClass(classIcon);
}

// Función para buscar comprobantes por RFC
function onConsultarComprobantesClick(rfc) {
    if (!rfc) {
        showError("Error", "Por favor, selecciona una empresa válida.");
        return;
    }

    let oParams = { rfc: rfc };

    doAjax(
        "/ERP/Conciliaciones/ComprobantesListEmpresa",
        oParams,
        function (resp) {
            if (resp.tieneError) {
                let summary = resp.errores.map(error => `<li>${error}</li>`).join("");
                summaryContainer.innerHTML += `<ul>${summary}</ul>`;
                showError("Buscar Comprobantes", resp.mensaje);
                return;
            }

            $('#tableCardComprobantes').bootstrapTable('load', responseHandler(resp.datos));
        },
        function (error) {
            showError("Error", error);
        },
        postOptions
    );
}
