document.addEventListener(
    "DOMContentLoaded",
    function () {

        // =================================================
        // REFERENCIAS
        // =================================================

        const txtBusqueda =
            document.getElementById(
                "filtroBusqueda"
            );

        const filtroEstado =
            document.getElementById(
                "filtroEstado"
            );

        const filtroCategoria =
            document.getElementById(
                "filtroCategoria"
            );

        const btnLimpiar =
            document.getElementById(
                "btnLimpiarFiltros"
            );

        const categoria =
            document.getElementById(
                "CategoryId"
            );

        const subcategoria =
            document.getElementById(
                "SubcategoryId"
            );

        const modalDetalleElemento =
            document.getElementById(
                "modalDetalleTicket"
            );

        window.mesaDeAyudaRequiereRecarga =
            false;


        modalDetalleElemento
            ?.addEventListener(
                "hidden.bs.modal",
                function () {

                    if (
                        window
                            .mesaDeAyudaRequiereRecarga ===
                        true
                    ) {

                        window.location.reload();
                    }
                }
            );

        // ENVIAR COMENTARIO

        const btnEnviarComentarioTicket =
            document.getElementById(
                "btnEnviarComentarioTicket"
            );

        const archivoTicket =
            document.getElementById(
                "archivoTicket"
            );

        const btnSeleccionarArchivoTicket =
            document.getElementById(
                "btnSeleccionarArchivoTicket"
            );

        const btnSubirArchivoTicket =
            document.getElementById(
                "btnSubirArchivoTicket"
            );


        btnSeleccionarArchivoTicket
            ?.addEventListener(
                "click",
                function () {

                    archivoTicket?.click();

                }
            );


        archivoTicket
            ?.addEventListener(
                "change",
                function () {

                    mostrarArchivoSeleccionado();

                }
            );


        btnSubirArchivoTicket
            ?.addEventListener(
                "click",
                async function () {

                    await subirAdjuntoTicket();

                }
            );

        btnEnviarComentarioTicket
            ?.addEventListener(
                "click",
                async function () {

                    await enviarComentarioTicket();

                }
            );

        let modalDetalleTicket = null;

        if (
            modalDetalleElemento &&
            typeof bootstrap !== "undefined"
        ) {
            modalDetalleTicket =
                bootstrap.Modal.getOrCreateInstance(
                    modalDetalleElemento
                );
        }


        // =================================================
        // FILTROS
        // =================================================

        function aplicarFiltros() {

            const busqueda =
                (
                    txtBusqueda?.value ||
                    ""
                )
                    .toLowerCase()
                    .trim();

            const estado =
                (
                    filtroEstado?.value ||
                    ""
                )
                    .toLowerCase();

            const categoriaFiltro =
                (
                    filtroCategoria?.value ||
                    ""
                )
                    .toLowerCase();

            const filas =
                document.querySelectorAll(
                    "#tablaMesaDeAyuda tbody tr[data-folio]"
                );

            filas.forEach(
                function (fila) {

                    const folio =
                        (
                            fila.dataset.folio ||
                            ""
                        )
                            .toLowerCase();

                    const titulo =
                        (
                            fila.dataset.titulo ||
                            ""
                        )
                            .toLowerCase();

                    const estadoFila =
                        (
                            fila.dataset.estado ||
                            ""
                        )
                            .toLowerCase();

                    const categoriaFila =
                        (
                            fila.dataset.categoria ||
                            ""
                        )
                            .toLowerCase();

                    const coincideBusqueda =
                        !busqueda ||
                        folio.includes(
                            busqueda
                        ) ||
                        titulo.includes(
                            busqueda
                        );

                    const coincideEstado =
                        !estado ||
                        estadoFila ===
                        estado;

                    const coincideCategoria =
                        !categoriaFiltro ||
                        categoriaFila ===
                        categoriaFiltro;

                    fila.style.display =
                        coincideBusqueda &&
                            coincideEstado &&
                            coincideCategoria
                            ? ""
                            : "none";
                }
            );
        }


        txtBusqueda?.addEventListener(
            "input",
            aplicarFiltros
        );

        filtroEstado?.addEventListener(
            "change",
            aplicarFiltros
        );

        filtroCategoria?.addEventListener(
            "change",
            aplicarFiltros
        );

        btnLimpiar?.addEventListener(
            "click",
            function () {

                if (txtBusqueda) {
                    txtBusqueda.value = "";
                }

                if (filtroEstado) {
                    filtroEstado.value = "";
                }

                if (filtroCategoria) {
                    filtroCategoria.value = "";
                }

                aplicarFiltros();
            }
        );


        // =================================================
        // SUBCATEGORÍAS
        // =================================================

        if (
            categoria &&
            subcategoria
        ) {

            categoria.addEventListener(
                "change",
                async function () {

                    const categoryId =
                        this.value;

                    subcategoria.innerHTML =
                        '<option value="">Selecciona...</option>';

                    if (
                        !categoryId ||
                        categoryId === "0"
                    ) {
                        return;
                    }

                    try {

                        const response =
                            await fetch(
                                "?handler=Subcategorias&categoryId=" +
                                encodeURIComponent(
                                    categoryId
                                )
                            );

                        if (!response.ok) {

                            console.error(
                                "No fue posible cargar las subcategorías."
                            );

                            return;
                        }

                        const datos =
                            await response.json();

                        datos.forEach(
                            function (item) {

                                const option =
                                    document.createElement(
                                        "option"
                                    );

                                option.value =
                                    item.id;

                                option.textContent =
                                    item.nombre;

                                subcategoria.appendChild(
                                    option
                                );
                            }
                        );
                    }
                    catch (error) {

                        console.error(
                            "Error al cargar subcategorías:",
                            error
                        );
                    }
                }
            );
        }

        // =================================================
        // GUARDAR CAMBIOS DEL TICKET
        // =================================================

        const btnGuardarCambiosTicket =
            document.getElementById(
                "btnGuardarCambiosTicket"
            );

        const detalleEstadoSelect =
            document.getElementById(
                "detalleEstadoSelect"
            );

        detalleEstadoSelect
            ?.addEventListener(
                "change",
                function () {

                    configurarCampoResolucion();

                }
            );

        btnGuardarCambiosTicket?.addEventListener(
            "click",
            async function () {

                await guardarCambiosTicket();
            }
        );

        // =================================================
        // BOTONES VER TICKET
        // =================================================

        document.addEventListener(
            "click",
            function (event) {

                const boton =
                    event.target.closest(
                        ".sd-action[data-ticket-id]"
                    );

                if (!boton) {
                    return;
                }

                const ticketId =
                    parseInt(
                        boton.dataset.ticketId ||
                        "0",
                        10
                    );

                if (
                    Number.isNaN(ticketId) ||
                    ticketId <= 0
                ) {
                    return;
                }

                abrirDetalleTicket(
                    ticketId
                );
            }
        );


        // =================================================
        // ABRIR DETALLE
        // =================================================

        async function abrirDetalleTicket(
            ticketId
        ) {

            prepararModalDetalle();

            if (
                modalDetalleTicket
            ) {
                modalDetalleTicket.show();
            }

            try {

                const response =
                    await fetch(
                        "?handler=Ticket&id=" +
                        encodeURIComponent(
                            ticketId
                        ),
                        {
                            method: "GET",
                            headers: {
                                "Accept":
                                    "application/json"
                            }
                        }
                    );

                if (
                    response.status === 403
                ) {
                    throw new Error(
                        "No tienes permisos para consultar este ticket."
                    );
                }

                if (!response.ok) {
                    throw new Error(
                        "No fue posible consultar el ticket."
                    );
                }

                const respuesta =
                    await response.json();

                if (
                    !respuesta.success
                ) {
                    throw new Error(
                        respuesta.message ||
                        "No fue posible obtener el ticket."
                    );
                }

                cargarDetalleTicket(
                    respuesta
                );
            }
            catch (error) {

                mostrarErrorDetalle(
                    error.message ||
                    "Ocurrió un error al consultar el ticket."
                );
            }
        }


        // =================================================
        // PREPARAR MODAL
        // =================================================

        function prepararModalDetalle() {

            const loading =
                document.getElementById(
                    "detalleTicketLoading"
                );

            const contenido =
                document.getElementById(
                    "detalleTicketContenido"
                );

            const error =
                document.getElementById(
                    "detalleTicketError"
                );

            const mensajeAdmin =
                document.getElementById(
                    "detalleAdminMensaje"
                );

            const mensajeComentario =
                document.getElementById(
                    "mensajeComentarioTicket"
                );

            const textarea =
                document.getElementById(
                    "nuevoComentarioTicket"
                );

            const checkbox =
                document.getElementById(
                    "comentarioEsNotaInterna"
                );

            const mensajeAdjunto =
                document.getElementById(
                    "mensajeAdjuntoTicket"
                );

            const archivoTicketInput =
                document.getElementById(
                    "archivoTicket"
                );

            const archivoSeleccionado =
                document.getElementById(
                    "archivoTicketSeleccionado"
                );

            const contadorAdjuntos =
                document.getElementById(
                    "detalleAdjuntosCantidad"
                );

            const detalleAdjuntos =
                document.getElementById(
                    "detalleAdjuntos"
                );

            loading?.classList.remove(
                "d-none"
            );

            contenido?.classList.add(
                "d-none"
            );

            error?.classList.add(
                "d-none"
            );

            mensajeAdmin?.classList.add(
                "d-none"
            );

            mensajeComentario?.classList.add(
                "d-none"
            );

            mensajeAdjunto?.classList.add(
                "d-none"
            );

            archivoSeleccionado?.classList.add(
                "d-none"
            );

            if (archivoTicketInput) {
                archivoTicketInput.value = "";
            }

            if (contadorAdjuntos) {
                contadorAdjuntos.textContent = "0";
            }

            if (detalleAdjuntos) {
                detalleAdjuntos.innerHTML = `
                            <div class="sd-empty-detail">
                                <i class="bi bi-paperclip"></i>
                                <strong>
                                    Sin archivos adjuntos
                                </strong>
                                <span>
                                    Todavía no existen evidencias registradas.
                                </span>
                            </div>
                        `;
            }

            if (error) {
                error.textContent = "";
            }

            if (mensajeAdmin) {
                mensajeAdmin.textContent = "";
            }

            if (mensajeComentario) {
                mensajeComentario.textContent = "";
            }

            if (mensajeAdjunto) {
                mensajeAdjunto.textContent = "";
            }

            if (textarea) {
                textarea.value = "";
            }

            if (checkbox) {
                checkbox.checked = false;
            }
        }


        // =================================================
        // MOSTRAR ERROR
        // =================================================

        function mostrarErrorDetalle(
            mensaje
        ) {

            const loading =
                document.getElementById(
                    "detalleTicketLoading"
                );

            const contenido =
                document.getElementById(
                    "detalleTicketContenido"
                );

            const error =
                document.getElementById(
                    "detalleTicketError"
                );

            loading?.classList.add(
                "d-none"
            );

            contenido?.classList.add(
                "d-none"
            );

            if (error) {

                error.textContent =
                    mensaje;

                error.classList.remove(
                    "d-none"
                );
            }
        }


        // =================================================
        // CARGAR INFORMACIÓN
        // =================================================

        function cargarDetalleTicket(
            respuesta
        ) {

            const ticket =
                respuesta.ticket;

            establecerTexto(
                "detalleTicketId",
                ticket.id
            );

            establecerTexto(
                "detalleFolio",
                ticket.folio
            );

            establecerTexto(
                "detalleSolicitante",
                ticket.solicitante
            );

            establecerTexto(
                "detalleSolicitanteCorreo",
                ticket.solicitanteCorreo
            );

            establecerTexto(
                "detalleFechaCreacion",
                ticket.fechaCreacion
            );

            establecerTexto(
                "detalleTecnico",
                ticket.usuarioAsignado ||
                "Sin asignar"
            );

            establecerTexto(
                "detalleEquipo",
                ticket.equipoSoporte ||
                "Sin asignar"
            );

            establecerTexto(
                "detalleTipo",
                ticket.tipo
            );

            establecerTexto(
                "detalleCategoria",
                ticket.categoria
            );

            establecerTexto(
                "detalleSubcategoria",
                ticket.subcategoria
            );

            establecerTexto(
                "detallePrioridad",
                ticket.prioridad
            );

            establecerTexto(
                "detalleTitulo",
                ticket.titulo
            );

            establecerTexto(
                "detalleDescripcion",
                ticket.descripcion
            );

            establecerTexto(
                "detalleSlaRespuesta",
                ticket.fechaLimiteRespuestaSla ||
                "Sin definir"
            );

            establecerTexto(
                "detalleSlaResolucion",
                ticket.fechaLimiteResolucionSla ||
                "Sin definir"
            );


            configurarEstadoBadge(
                ticket.estadoId,
                ticket.estado
            );


            configurarSlaBadge(
                "detalleSlaRespuestaBadge",
                ticket.slaRespuestaVencido
            );

            configurarSlaBadge(
                "detalleSlaResolucionBadge",
                ticket.slaResolucionVencido
            );


            cargarComentarios(
                respuesta.comentarios ||
                []
            );

            cargarHistorial(
                respuesta.historial ||
                []
            );


            configurarResolucion(
                ticket
            );


            configurarPanelAdmin(
                respuesta
            );

            configurarComentarioAdmin(
                respuesta.esAdmin
            );

            cargarAdjuntosTicket(
                respuesta.ticket.id
            );

            const loading =
                document.getElementById(
                    "detalleTicketLoading"
                );

            const contenido =
                document.getElementById(
                    "detalleTicketContenido"
                );

            const error =
                document.getElementById(
                    "detalleTicketError"
                );

            loading?.classList.add(
                "d-none"
            );

            error?.classList.add(
                "d-none"
            );

            contenido?.classList.remove(
                "d-none"
            );
        }


        // =================================================
        // PANEL ADMIN
        // =================================================

        function configurarPanelAdmin(
            respuesta
        ) {

            const panel =
                document.getElementById(
                    "detallePanelAdmin"
                );

            if (
                !respuesta.esAdmin
            ) {

                panel?.classList.add(
                    "d-none"
                );

                return;
            }

            panel?.classList.remove(
                "d-none"
            );


            const ticket =
                respuesta.ticket;


            // =============================================
            // TÉCNICOS
            // =============================================

            const tecnicoSelect =
                document.getElementById(
                    "detalleTecnicoSelect"
                );

            if (tecnicoSelect) {

                tecnicoSelect.innerHTML =
                    '<option value="">Sin asignar</option>';

                (
                    respuesta.tecnicos ||
                    []
                )
                    .forEach(
                        function (tecnico) {

                            const option =
                                document.createElement(
                                    "option"
                                );

                            option.value =
                                tecnico.id;

                            option.textContent =
                                tecnico.nombre;

                            tecnicoSelect.appendChild(
                                option
                            );
                        }
                    );

                tecnicoSelect.value =
                    ticket.usuarioAsignadoId ||
                    "";
            }


            // =============================================
            // PRIORIDADES
            // =============================================

            const prioridadSelect =
                document.getElementById(
                    "detallePrioridadSelect"
                );

            if (prioridadSelect) {

                prioridadSelect.innerHTML =
                    "";

                (
                    respuesta.prioridades ||
                    []
                )
                    .forEach(
                        function (prioridad) {

                            const option =
                                document.createElement(
                                    "option"
                                );

                            option.value =
                                prioridad.id;

                            option.textContent =
                                prioridad.nombre;

                            prioridadSelect.appendChild(
                                option
                            );
                        }
                    );

                prioridadSelect.value =
                    String(
                        ticket.prioridadId
                    );
            }


            // =============================================
            // ESTADOS
            // =============================================

            const estadoSelect =
                document.getElementById(
                    "detalleEstadoSelect"
                );

            if (estadoSelect) {

                estadoSelect.innerHTML =
                    "";

                (
                    respuesta.estados ||
                    []
                )
                    .forEach(
                        function (estado) {

                            const option =
                                document.createElement(
                                    "option"
                                );

                            option.value =
                                estado.id;

                            option.textContent =
                                estado.nombre;

                            estadoSelect.appendChild(
                                option
                            );
                        }
                    );

                estadoSelect.value =
                    String(
                        ticket.estadoId
                    );

                estadoSelect.dataset.estadoActual =
                    String(
                        ticket.estadoId
                    );

                const resolucionTextarea =
                    document.getElementById(
                        "detalleResolucionTextarea"
                    );

                if (resolucionTextarea) {

                    resolucionTextarea.value =
                        ticket.resolucion ||
                        "";
                }

                configurarCampoResolucion();
            }
        }


        // =================================================
        // COMENTARIOS
        // =================================================

        function cargarComentarios(
            comentarios
        ) {

            const contenedor =
                document.getElementById(
                    "detalleComentarios"
                );

            if (!contenedor) {
                return;
            }

            contenedor.innerHTML =
                "";

            if (
                comentarios.length === 0
            ) {

                contenedor.innerHTML = `
                            <div class="sd-empty-detail">
                                <i class="bi bi-chat-left-dots"></i>
                                <strong>
                                    Sin seguimiento registrado
                                </strong>
                                <span>
                                    Todavía no existen comentarios en este ticket.
                                </span>
                            </div>
                        `;

                return;
            }

            comentarios.forEach(
                function (item) {

                    const bloque =
                        document.createElement(
                            "div"
                        );

                    bloque.className =
                        item.esNotaInterna
                            ? "sd-comment sd-comment-internal"
                            : "sd-comment";

                    bloque.innerHTML = `
                                <div class="sd-comment-header">

                                    <div>

                                        <strong>
                                            ${escaparHtml(item.usuario)}
                                        </strong>

                                        ${item.esNotaInterna
                            ? `
                                                <span class="sd-internal-badge">
                                                    <i class="bi bi-lock-fill"></i>
                                                    Nota interna
                                                </span>
                                                `
                            : ""
                        }

                                    </div>

                                    <span>
                                        ${escaparHtml(item.fecha)}
                                    </span>

                                </div>

                                <div class="sd-comment-body">
                                    ${convertirSaltosLinea(
                            item.comentario
                        )}
                                </div>
                            `;

                    contenedor.appendChild(
                        bloque
                    );
                }
            );
        }


        // =================================================
        // HISTORIAL
        // =================================================

        function cargarHistorial(
            historial
        ) {

            const contenedor =
                document.getElementById(
                    "detalleHistorial"
                );

            const contador =
                document.getElementById(
                    "detalleHistorialCantidad"
                );


            if (!contenedor) {
                return;
            }


            const movimientos =
                historial ||
                [];


            if (contador) {

                contador.textContent =
                    String(
                        movimientos.length
                    );
            }


            contenedor.innerHTML =
                "";


            if (
                movimientos.length === 0
            ) {

                contenedor.innerHTML = `
                    <div class="sd-empty-detail">

                        <i class="bi bi-clock-history"></i>

                        <strong>
                            Sin movimientos
                        </strong>

                        <span>
                            No existen movimientos registrados.
                        </span>

                    </div>
                `;

                return;
            }


            const timeline =
                document.createElement(
                    "div"
                );

            timeline.className =
                "sd-history-timeline";


            movimientos.forEach(
                function (item) {

                    const configuracion =
                        obtenerConfiguracionHistorial(
                            item
                        );


                    const bloque =
                        document.createElement(
                            "div"
                        );


                    bloque.className =
                        `sd-history-item ${configuracion.clase}`;


                    const detalle =
                        item.detalle ||
                        generarDetalleHistorial(
                            item
                        );


                    const tieneCambio =
                        item.valorAnterior ||
                        item.valorNuevo;


                    bloque.innerHTML = `

                        <div class="sd-history-dot">

                            <i class="bi ${configuracion.icono}">
                            </i>

                        </div>


                        <div class="sd-history-content">

                            <div class="sd-history-top">

                                <div class="sd-history-action">

                                    <strong>
                                        ${escaparHtml(
                        item.accion ||
                        "Movimiento"
                    )}
                                    </strong>

                                    ${item.campo
                            ? `
                                                <span class="sd-history-field">
                                                    ${escaparHtml(
                                obtenerNombreCampoHistorial(
                                    item.campo
                                )
                            )}
                                                </span>
                                            `
                            : ""
                        }

                                </div>


                                <span class="sd-history-date">

                                    <i class="bi bi-calendar3"></i>

                                    ${escaparHtml(
                            item.fecha ||
                            ""
                        )}

                                </span>

                            </div>


                            ${tieneCambio
                            ? `
                                        <div class="sd-history-values">

                                            ${item.valorAnterior
                                ? `
                                                        <div class="sd-history-value sd-history-value-old">

                                                            <span>
                                                                Anterior
                                                            </span>

                                                            <strong>
                                                                ${escaparHtml(
                                    item.valorAnterior
                                )}
                                                            </strong>

                                                        </div>
                                                    `
                                : ""
                            }


                                            ${item.valorAnterior &&
                                item.valorNuevo
                                ? `
                                                        <div class="sd-history-arrow">

                                                            <i class="bi bi-arrow-right"></i>

                                                        </div>
                                                    `
                                : ""
                            }


                                            ${item.valorNuevo
                                ? `
                                                        <div class="sd-history-value sd-history-value-new">

                                                            <span>
                                                                Nuevo
                                                            </span>

                                                            <strong>
                                                                ${escaparHtml(
                                    item.valorNuevo
                                )}
                                                            </strong>

                                                        </div>
                                                    `
                                : ""
                            }

                                        </div>
                                    `
                            : ""
                        }


                            ${detalle
                            ? `
                                        <div class="sd-history-detail">
                                            ${convertirSaltosLinea(
                                detalle
                            )}
                                        </div>
                                    `
                            : ""
                        }


                            <div class="sd-history-footer">

                                <div class="sd-history-user">

                                    <i class="bi bi-person-circle"></i>

                                    ${escaparHtml(
                            item.usuario ||
                            "Sistema"
                        )}

                                </div>


                                ${item.direccionIp
                            ? `
                                            <div class="sd-history-ip">

                                                <i class="bi bi-hdd-network"></i>

                                                ${escaparHtml(
                                item.direccionIp
                            )}

                                            </div>
                                        `
                            : ""
                        }

                            </div>

                        </div>
                    `;


                    timeline.appendChild(
                        bloque
                    );
                }
            );


            contenedor.appendChild(
                timeline
            );
        }

        function obtenerConfiguracionHistorial(
            item
        ) {

            const accion =
                (
                    item?.accion ||
                    ""
                )
                    .trim()
                    .toLowerCase();


            switch (accion) {

                case "creación":

                    return {
                        icono:
                            "bi-plus-circle-fill",

                        clase:
                            "sd-history-created"
                    };


                case "asignación":

                    return {
                        icono:
                            "bi-person-check-fill",

                        clase:
                            "sd-history-assignment"
                    };


                case "cambio de prioridad":

                    return {
                        icono:
                            "bi-flag-fill",

                        clase:
                            "sd-history-priority"
                    };


                case "cambio de estado":

                    return {
                        icono:
                            "bi-arrow-left-right",

                        clase:
                            "sd-history-status"
                    };


                case "resolución":

                    return {
                        icono:
                            "bi-check-circle-fill",

                        clase:
                            "sd-history-resolution"
                    };


                case "actualización de resolución":

                    return {
                        icono:
                            "bi-pencil-square",

                        clase:
                            "sd-history-resolution"
                    };


                case "cierre":

                    return {
                        icono:
                            "bi-lock-fill",

                        clase:
                            "sd-history-closed"
                    };


                case "reapertura":

                    return {
                        icono:
                            "bi-arrow-counterclockwise",

                        clase:
                            "sd-history-reopened"
                    };


                case "comentario":

                    return {
                        icono:
                            "bi-chat-left-text-fill",

                        clase:
                            "sd-history-comment"
                    };


                case "nota interna":

                    return {
                        icono:
                            "bi-shield-lock-fill",

                        clase:
                            "sd-history-internal"
                    };


                case "adjunto":

                    return {
                        icono:
                            "bi-paperclip",

                        clase:
                            "sd-history-attachment"
                    };


                default:

                    return {
                        icono:
                            "bi-clock-history",

                        clase:
                            "sd-history-default"
                    };
            }
        }

        function obtenerNombreCampoHistorial(
            campo
        ) {

            const nombres = {

                Ticket:
                    "Ticket",

                UsuarioAsignadoId:
                    "Técnico",

                PriorityId:
                    "Prioridad",

                StatusId:
                    "Estado",

                Resolucion:
                    "Resolución",

                FechaCierre:
                    "Cierre",

                Comentario:
                    "Comentario",

                NotaInterna:
                    "Nota interna",

                Archivo:
                    "Archivo"
            };


            return (
                nombres[campo] ||
                campo ||
                "Movimiento"
            );
        }


        // =================================================
        // RESOLUCIÓN
        // =================================================

        function configurarResolucion(
            ticket) {

            const contenedor =
                document.getElementById(
                    "detalleResolucionContenedor"
                );

            const contenido =
                document.getElementById(
                    "detalleResolucion"
                );

            const fechaResolucion =
                document.getElementById(
                    "detalleFechaResolucionVista"
                );

            const fechaCierre =
                document.getElementById(
                    "detalleFechaCierreVista"
                );

            const usuarioCierre =
                document.getElementById(
                    "detalleUsuarioCierreVista"
                );

            const cierreMeta =
                document.getElementById(
                    "detalleCierreMeta"
                );

            const usuarioCierreMeta =
                document.getElementById(
                    "detalleUsuarioCierreMeta"
                );


            if (
                !ticket ||
                !ticket.resolucion ||
                !ticket.resolucion.trim()
            ) {

                contenedor?.classList.add(
                    "d-none"
                );

                return;
            }


            if (contenido) {

                contenido.textContent =
                    ticket.resolucion;
            }


            if (fechaResolucion) {

                fechaResolucion.textContent =
                    ticket.fechaResolucion ||
                    "Sin registrar";
            }


            if (
                ticket.fechaCierre
            ) {

                if (fechaCierre) {

                    fechaCierre.textContent =
                        ticket.fechaCierre;
                }

                cierreMeta?.classList.remove(
                    "d-none"
                );
            }
            else {

                cierreMeta?.classList.add(
                    "d-none"
                );
            }


            if (
                ticket.usuarioCierre
            ) {

                if (usuarioCierre) {

                    usuarioCierre.textContent =
                        ticket.usuarioCierre;
                }

                usuarioCierreMeta
                    ?.classList
                    .remove(
                        "d-none"
                    );
            }
            else {

                usuarioCierreMeta
                    ?.classList
                    .add(
                        "d-none"
                    );
            }


            contenedor?.classList.remove(
                "d-none"
            );
        }


        // =================================================
        // BADGE ESTADO
        // =================================================

        function configurarEstadoBadge(
            statusId,
            nombre
        ) {

            const badge =
                document.getElementById(
                    "detalleEstadoBadge"
                );

            if (!badge) {
                return;
            }

            badge.className =
                "sd-badge";

            let clase =
                "sd-status-new";

            let icono =
                "bi-circle-fill";

            switch (
            Number(statusId)
            ) {

                case 1:

                    clase =
                        "sd-status-new";

                    icono =
                        "bi-circle-fill";

                    break;

                case 2:

                    clase =
                        "sd-status-assigned";

                    icono =
                        "bi-person-check";

                    break;

                case 3:

                    clase =
                        "sd-status-progress";

                    icono =
                        "bi-gear";

                    break;

                case 4:

                    clase =
                        "sd-status-waiting";

                    icono =
                        "bi-hourglass-split";

                    break;

                case 5:

                    clase =
                        "sd-status-resolved";

                    icono =
                        "bi-check-circle";

                    break;

                case 6:

                    clase =
                        "sd-status-closed";

                    icono =
                        "bi-lock";

                    break;

                case 7:

                    clase =
                        "sd-status-progress";

                    icono =
                        "bi-arrow-counterclockwise";

                    break;

                case 8:

                    clase =
                        "sd-status-cancelled";

                    icono =
                        "bi-x-circle";

                    break;
            }

            badge.classList.add(
                clase
            );

            badge.innerHTML = `
                        <i class="bi ${icono}"></i>
                        ${escaparHtml(nombre || "-")}
                    `;
        }


        // =================================================
        // SLA BADGE
        // =================================================

        function configurarSlaBadge(
            id,
            vencido
        ) {

            const badge =
                document.getElementById(
                    id
                );

            if (!badge) {
                return;
            }

            badge.className =
                "sd-sla-status";

            if (vencido) {

                badge.classList.add(
                    "sd-sla-expired"
                );

                badge.innerHTML =
                    '<i class="bi bi-exclamation-triangle-fill"></i> Vencido';

                return;
            }

            badge.classList.add(
                "sd-sla-active"
            );

            badge.innerHTML =
                '<i class="bi bi-check-circle-fill"></i> Vigente';
        }


        // =================================================
        // DETALLE HISTORIAL
        // =================================================

        function generarDetalleHistorial(
            item
        ) {

            const campo =
                obtenerNombreCampoHistorial(
                    item.campo
                );


            if (
                item.valorAnterior &&
                item.valorNuevo
            ) {

                return (
                    `${campo} cambió de ` +
                    `"${item.valorAnterior}" a ` +
                    `"${item.valorNuevo}".`
                );
            }


            if (
                item.valorNuevo
            ) {

                return (
                    `${campo}: ` +
                    `${item.valorNuevo}`
                );
            }


            if (
                item.valorAnterior
            ) {

                return (
                    `Valor anterior de ${campo}: ` +
                    `${item.valorAnterior}`
                );
            }


            return (
                item.accion ||
                "Movimiento registrado."
            );
        }

        // =================================================
        // MOSTRAR / OCULTAR CAMPO DE RESOLUCIÓN
        // =================================================

        function configurarCampoResolucion() {

            const estadoSelect =
                document.getElementById(
                    "detalleEstadoSelect"
                );

            const contenedor =
                document.getElementById(
                    "detalleResolucionAdmin"
                );

            if (
                !estadoSelect ||
                !contenedor
            ) {
                return;
            }

            const statusId =
                parseInt(
                    estadoSelect.value ||
                    "0",
                    10
                );

            if (statusId === 5) {

                contenedor.classList.remove(
                    "d-none"
                );

            }
            else {

                contenedor.classList.add(
                    "d-none"
                );
            }
        }

        // GUARDAR CAMBIOS ADMINISTRATIVOS
        // =================================================

        async function guardarCambiosTicket() {

            const ticketIdElemento =
                document.getElementById(
                    "detalleTicketId"
                );

            const tecnicoSelect =
                document.getElementById(
                    "detalleTecnicoSelect"
                );

            const prioridadSelect =
                document.getElementById(
                    "detallePrioridadSelect"
                );

            const estadoSelect =
                document.getElementById(
                    "detalleEstadoSelect"
                );

            const resolucionTextarea =
                document.getElementById(
                    "detalleResolucionTextarea"
                );

            const boton =
                document.getElementById(
                    "btnGuardarCambiosTicket"
                );

            const contenidoBoton =
                document.getElementById(
                    "btnGuardarCambiosTicketContenido"
                );

            const cargandoBoton =
                document.getElementById(
                    "btnGuardarCambiosTicketCargando"
                );

            const mensaje =
                document.getElementById(
                    "detalleAdminMensaje"
                );

            const ticketId =
                parseInt(
                    ticketIdElemento?.value ||
                    "0",
                    10
                );

            const priorityId =
                parseInt(
                    prioridadSelect?.value ||
                    "0",
                    10
                );

            const statusId =
                parseInt(
                    estadoSelect?.value ||
                    "0",
                    10
                );

            const usuarioAsignadoId =
                tecnicoSelect?.value ||
                null;

            const resolucion =
                resolucionTextarea?.value
                    ?.trim() ||
                null;


            // =============================================
            // VALIDACIONES
            // =============================================

            if (
                Number.isNaN(ticketId) ||
                ticketId <= 0
            ) {

                mostrarMensajeAdmin(
                    "danger",
                    "No fue posible identificar el ticket."
                );

                return;
            }


            if (
                Number.isNaN(priorityId) ||
                priorityId <= 0
            ) {

                mostrarMensajeAdmin(
                    "warning",
                    "Debes seleccionar una prioridad."
                );

                return;
            }


            if (
                Number.isNaN(statusId) ||
                statusId <= 0
            ) {

                mostrarMensajeAdmin(
                    "warning",
                    "Debes seleccionar un estado."
                );

                return;
            }

            // VALIDAR RESOLUCIÓN
            // =============================================

            if (
                statusId === 5 &&
                !resolucion
            ) {

                mostrarMensajeAdmin(
                    "warning",
                    "Debes registrar la resolución antes de marcar el ticket como Resuelto."
                );

                resolucionTextarea?.focus();

                return;
            }

            // =============================================
            // ESTADO ORIGINAL DEL TICKET
            // =============================================

            const estadoActualId =
                parseInt(
                    estadoSelect
                        ?.dataset
                        ?.estadoActual ||
                    "0",
                    10
                );


            // =============================================
            // VALIDAR CIERRE
            // Resuelto -> Cerrado
            // =============================================

            if (
                statusId === 6 &&
                estadoActualId !== 5 &&
                estadoActualId !== 6
            ) {

                mostrarMensajeAdmin(
                    "warning",
                    "El ticket debe estar Resuelto antes de poder cerrarlo."
                );

                estadoSelect?.focus();

                return;
            }


            // =============================================
            // VALIDAR REAPERTURA
            // Resuelto/Cerrado -> Reabierto
            // =============================================

            if (
                statusId === 7 &&
                estadoActualId !== 5 &&
                estadoActualId !== 6 &&
                estadoActualId !== 7
            ) {

                mostrarMensajeAdmin(
                    "warning",
                    "Solamente un ticket Resuelto o Cerrado puede ser reabierto."
                );

                estadoSelect?.focus();

                return;
            }

            // =============================================
            // TOKEN ANTIFORGERY
            // =============================================

            const token =
                document.querySelector(
                    'input[name="__RequestVerificationToken"]'
                )
                    ?.value;


            if (!token) {

                mostrarMensajeAdmin(
                    "danger",
                    "No fue posible validar la seguridad de la solicitud."
                );

                return;
            }


            // =============================================
            // ESTADO DEL BOTÓN
            // =============================================

            if (boton) {
                boton.disabled = true;
            }

            contenidoBoton?.classList.add(
                "d-none"
            );

            cargandoBoton?.classList.remove(
                "d-none"
            );

            mensaje?.classList.add(
                "d-none"
            );


            try {

                const response =
                    await fetch(
                        "?handler=ActualizarTicket",
                        {
                            method: "POST",

                            headers: {
                                "Content-Type":
                                    "application/json",

                                "Accept":
                                    "application/json",

                                "RequestVerificationToken":
                                    token
                            },

                            body:
                                JSON.stringify(
                                    {
                                        ticketId:
                                            ticketId,

                                        usuarioAsignadoId:
                                            usuarioAsignadoId,

                                        priorityId:
                                            priorityId,

                                        statusId:
                                            statusId,

                                        resolucion:
                                            resolucion
                                    }
                                )
                        }
                    );


                let respuesta =
                    null;


                try {

                    respuesta =
                        await response.json();

                }
                catch {

                    throw new Error(
                        "El servidor respondió con un formato no válido."
                    );
                }


                if (
                    response.status === 403
                ) {

                    throw new Error(
                        respuesta?.message ||
                        "No tienes permisos para modificar este ticket."
                    );
                }


                if (!response.ok) {

                    throw new Error(
                        respuesta?.message ||
                        "No fue posible actualizar el ticket."
                    );
                }


                if (
                    !respuesta.success
                ) {

                    throw new Error(
                        respuesta.message ||
                        "No fue posible actualizar el ticket."
                    );
                }


                // =========================================
                // TODO CORRECTO
                // =========================================

                mostrarMensajeAdmin(
                    "success",
                    respuesta.message ||
                    "Cambios guardados correctamente."
                );


                if (respuesta.changed === false) {

                    return;
                }


                // =========================================
                // RECARGAR DETALLE SIN CERRAR MODAL
                // =========================================

                await recargarDetalleTicket(
                    ticketId
                );


                // =========================================
                // MENSAJE DE CONFIRMACIÓN
                // =========================================

                mostrarMensajeAdmin(
                    "success",
                    respuesta.message ||
                    "Cambios guardados correctamente."
                );


                // =========================================
                // MARCAR QUE LA TABLA REQUIERE ACTUALIZARSE
                // =========================================

                window.mesaDeAyudaRequiereRecarga =
                    true;

            }
            catch (error) {

                console.error(
                    "Error al actualizar ticket:",
                    error
                );


                mostrarMensajeAdmin(
                    "danger",
                    error.message ||
                    "Ocurrió un error al actualizar el ticket."
                );

            }
            finally {

                if (boton) {
                    boton.disabled = false;
                }

                contenidoBoton?.classList.remove(
                    "d-none"
                );

                cargandoBoton?.classList.add(
                    "d-none"
                );
            }
        }

        // =================================================
        // MENSAJE DEL PANEL ADMINISTRATIVO
        // =================================================

        function mostrarMensajeAdmin(
            tipo,
            texto
        ) {

            const mensaje =
                document.getElementById(
                    "detalleAdminMensaje"
                );

            if (!mensaje) {
                return;
            }

            mensaje.className =
                `alert alert-${tipo} mt-3 mb-0`;

            mensaje.textContent =
                texto;

            mensaje.classList.remove(
                "d-none"
            );
        }

        // =================================================
        // CONFIGURAR OPCIONES DE COMENTARIO
        // =================================================

        function configurarComentarioAdmin(
            esAdmin
        ) {

            const contenedorNotaInterna =
                document.getElementById(
                    "contenedorNotaInterna"
                );

            const checkbox =
                document.getElementById(
                    "comentarioEsNotaInterna"
                );

            if (!contenedorNotaInterna) {
                return;
            }

            if (esAdmin) {

                contenedorNotaInterna
                    .classList
                    .remove(
                        "d-none"
                    );

            }
            else {

                contenedorNotaInterna
                    .classList
                    .add(
                        "d-none"
                    );

                if (checkbox) {
                    checkbox.checked =
                        false;
                }
            }
        }
        // =================================================
        // ENVIAR COMENTARIO
        // =================================================

        async function enviarComentarioTicket() {

            const ticketIdElemento =
                document.getElementById(
                    "detalleTicketId"
                );

            const textarea =
                document.getElementById(
                    "nuevoComentarioTicket"
                );

            const checkbox =
                document.getElementById(
                    "comentarioEsNotaInterna"
                );

            const boton =
                document.getElementById(
                    "btnEnviarComentarioTicket"
                );

            const contenidoBoton =
                document.getElementById(
                    "btnEnviarComentarioContenido"
                );

            const cargandoBoton =
                document.getElementById(
                    "btnEnviarComentarioCargando"
                );

            const ticketId =
                parseInt(
                    ticketIdElemento?.value ||
                    "0",
                    10
                );

            const comentario =
                textarea?.value
                    ?.trim()
                ?? "";

            const esNotaInterna =
                checkbox?.checked === true;


            // =============================================
            // VALIDACIONES
            // =============================================

            if (
                Number.isNaN(ticketId) ||
                ticketId <= 0
            ) {

                mostrarMensajeComentario(
                    "danger",
                    "No fue posible identificar el ticket."
                );

                return;
            }


            if (!comentario) {

                mostrarMensajeComentario(
                    "warning",
                    "Escribe un comentario antes de enviarlo."
                );

                textarea?.focus();

                return;
            }


            if (
                comentario.length >
                5000
            ) {

                mostrarMensajeComentario(
                    "warning",
                    "El comentario no puede superar los 5000 caracteres."
                );

                return;
            }


            // =============================================
            // TOKEN ANTIFORGERY
            // =============================================

            const token =
                document.querySelector(
                    'input[name="__RequestVerificationToken"]'
                )
                    ?.value;


            if (!token) {

                mostrarMensajeComentario(
                    "danger",
                    "No fue posible validar la seguridad de la solicitud."
                );

                return;
            }


            // =============================================
            // ESTADO DEL BOTÓN
            // =============================================

            if (boton) {
                boton.disabled = true;
            }

            contenidoBoton?.classList.add(
                "d-none"
            );

            cargandoBoton?.classList.remove(
                "d-none"
            );


            try {

                const response =
                    await fetch(
                        "?handler=AgregarComentario",
                        {
                            method:
                                "POST",

                            headers: {

                                "Content-Type":
                                    "application/json",

                                "Accept":
                                    "application/json",

                                "RequestVerificationToken":
                                    token
                            },

                            body:
                                JSON.stringify(
                                    {
                                        ticketId:
                                            ticketId,

                                        comentario:
                                            comentario,

                                        esNotaInterna:
                                            esNotaInterna
                                    }
                                )
                        }
                    );


                let respuesta =
                    null;


                try {

                    respuesta =
                        await response.json();

                }
                catch {

                    throw new Error(
                        "El servidor respondió con un formato no válido."
                    );
                }


                if (
                    response.status === 401
                ) {

                    throw new Error(
                        respuesta?.message ||
                        "Tu sesión ya no es válida. Inicia sesión nuevamente."
                    );
                }


                if (
                    response.status === 403
                ) {

                    throw new Error(
                        respuesta?.message ||
                        "No tienes permisos para agregar este comentario."
                    );
                }


                if (!response.ok) {

                    throw new Error(
                        respuesta?.message ||
                        "No fue posible guardar el comentario."
                    );
                }


                if (
                    !respuesta.success
                ) {

                    throw new Error(
                        respuesta.message ||
                        "No fue posible guardar el comentario."
                    );
                }


                // =========================================
                // LIMPIAR FORMULARIO
                // =========================================

                if (textarea) {
                    textarea.value = "";
                }

                if (checkbox) {
                    checkbox.checked = false;
                }


                // =========================================
                // RECARGAR DETALLE SIN CERRAR MODAL
                // =========================================

                await recargarDetalleTicket(
                    ticketId
                );


                // =========================================
                // MOSTRAR MENSAJE DESPUÉS DE RECARGAR
                // =========================================

                mostrarMensajeComentario(
                    "success",
                    respuesta.message ||
                    "Comentario agregado correctamente."
                );

            }
            catch (error) {

                console.error(
                    "Error al guardar comentario:",
                    error
                );

                mostrarMensajeComentario(
                    "danger",
                    error.message ||
                    "Ocurrió un error al guardar el comentario."
                );

            }
            finally {

                if (boton) {
                    boton.disabled = false;
                }

                contenidoBoton?.classList.remove(
                    "d-none"
                );

                cargandoBoton?.classList.add(
                    "d-none"
                );
            }
        }


        // =================================================
        // RECARGAR DETALLE DEL TICKET
        // =================================================

        async function recargarDetalleTicket(
            ticketId
        ) {

            try {

                const response =
                    await fetch(
                        "?handler=Ticket&id=" +
                        encodeURIComponent(
                            ticketId
                        ),
                        {
                            method:
                                "GET",

                            headers: {
                                "Accept":
                                    "application/json"
                            }
                        }
                    );


                if (
                    response.status === 401
                ) {

                    throw new Error(
                        "Tu sesión ya no es válida."
                    );
                }


                if (
                    response.status === 403
                ) {

                    throw new Error(
                        "No tienes permisos para consultar este ticket."
                    );
                }


                if (!response.ok) {

                    throw new Error(
                        "No fue posible actualizar el seguimiento."
                    );
                }


                const respuesta =
                    await response.json();


                if (
                    !respuesta.success
                ) {

                    throw new Error(
                        respuesta.message ||
                        "No fue posible actualizar el detalle."
                    );
                }


                cargarDetalleTicket(
                    respuesta
                );

            }
            catch (error) {

                console.error(
                    "Error al recargar detalle:",
                    error
                );

                mostrarMensajeComentario(
                    "danger",
                    error.message ||
                    "No fue posible actualizar el detalle del ticket."
                );
            }
        }


        // =================================================
        // MENSAJE DEL COMENTARIO
        // =================================================

        function mostrarMensajeComentario(
            tipo,
            texto
        ) {

            const mensaje =
                document.getElementById(
                    "mensajeComentarioTicket"
                );

            if (!mensaje) {
                return;
            }

            mensaje.className =
                `alert alert-${tipo} mt-3 mb-0`;

            mensaje.textContent =
                texto;

            mensaje.classList.remove(
                "d-none"
            );
        }

        // =================================================
        // MOSTRAR ARCHIVO SELECCIONADO
        // =================================================

        function mostrarArchivoSeleccionado() {

            const input =
                document.getElementById(
                    "archivoTicket"
                );

            const contenedor =
                document.getElementById(
                    "archivoTicketSeleccionado"
                );

            const nombre =
                document.getElementById(
                    "archivoTicketNombre"
                );

            const tamano =
                document.getElementById(
                    "archivoTicketTamano"
                );

            const archivo =
                input?.files?.[0];


            if (!archivo) {

                contenedor?.classList.add(
                    "d-none"
                );

                return;
            }


            if (nombre) {
                nombre.textContent =
                    archivo.name;
            }


            if (tamano) {
                tamano.textContent =
                    formatearTamanoArchivoJs(
                        archivo.size
                    );
            }


            contenedor?.classList.remove(
                "d-none"
            );
        }


        // =================================================
        // SUBIR ADJUNTO
        // =================================================

        async function subirAdjuntoTicket() {

            const ticketIdElemento =
                document.getElementById(
                    "detalleTicketId"
                );

            const input =
                document.getElementById(
                    "archivoTicket"
                );

            const boton =
                document.getElementById(
                    "btnSubirArchivoTicket"
                );

            const contenido =
                document.getElementById(
                    "btnSubirArchivoContenido"
                );

            const cargando =
                document.getElementById(
                    "btnSubirArchivoCargando"
                );


            const ticketId =
                parseInt(
                    ticketIdElemento?.value ||
                    "0",
                    10
                );


            const archivo =
                input?.files?.[0];


            if (
                Number.isNaN(ticketId) ||
                ticketId <= 0
            ) {

                mostrarMensajeAdjunto(
                    "danger",
                    "No fue posible identificar el ticket."
                );

                return;
            }


            if (!archivo) {

                mostrarMensajeAdjunto(
                    "warning",
                    "Selecciona un archivo."
                );

                return;
            }


            if (
                archivo.size >
                10 * 1024 * 1024
            ) {

                mostrarMensajeAdjunto(
                    "warning",
                    "El archivo no puede superar los 10 MB."
                );

                return;
            }


            const token =
                document.querySelector(
                    'input[name="__RequestVerificationToken"]'
                )
                    ?.value;


            if (!token) {

                mostrarMensajeAdjunto(
                    "danger",
                    "No fue posible validar la seguridad de la solicitud."
                );

                return;
            }


            const formData =
                new FormData();

            formData.append(
                "ticketId",
                ticketId
            );

            formData.append(
                "archivo",
                archivo
            );


            if (boton) {
                boton.disabled = true;
            }

            contenido?.classList.add(
                "d-none"
            );

            cargando?.classList.remove(
                "d-none"
            );


            try {

                const response =
                    await fetch(
                        "?handler=SubirAdjunto",
                        {
                            method:
                                "POST",

                            headers: {
                                "RequestVerificationToken":
                                    token
                            },

                            body:
                                formData
                        }
                    );


                const respuesta =
                    await response.json();


                if (!response.ok) {

                    throw new Error(
                        respuesta?.message ||
                        "No fue posible subir el archivo."
                    );
                }


                if (!respuesta.success) {

                    throw new Error(
                        respuesta.message ||
                        "No fue posible subir el archivo."
                    );
                }


                if (input) {
                    input.value = "";
                }


                document
                    .getElementById(
                        "archivoTicketSeleccionado"
                    )
                    ?.classList
                    .add(
                        "d-none"
                    );


                mostrarMensajeAdjunto(
                    "success",
                    respuesta.message
                );


                await cargarAdjuntosTicket(
                    ticketId
                );

            }
            catch (error) {

                console.error(
                    "Error al subir adjunto:",
                    error
                );


                mostrarMensajeAdjunto(
                    "danger",
                    error.message ||
                    "Ocurrió un error al subir el archivo."
                );

            }
            finally {

                if (boton) {
                    boton.disabled = false;
                }

                contenido?.classList.remove(
                    "d-none"
                );

                cargando?.classList.add(
                    "d-none"
                );
            }
        }


        // =================================================
        // CARGAR ADJUNTOS
        // =================================================

        async function cargarAdjuntosTicket(
            ticketId
        ) {

            const contenedor =
                document.getElementById(
                    "detalleAdjuntos"
                );

            const contador =
                document.getElementById(
                    "detalleAdjuntosCantidad"
                );


            if (!contenedor) {
                return;
            }


            try {

                const response =
                    await fetch(
                        "?handler=Adjuntos&ticketId=" +
                        encodeURIComponent(
                            ticketId
                        ),
                        {
                            method:
                                "GET",

                            headers: {
                                "Accept":
                                    "application/json"
                            }
                        }
                    );


                if (!response.ok) {

                    throw new Error(
                        "No fue posible cargar los adjuntos."
                    );
                }


                const respuesta =
                    await response.json();


                if (!respuesta.success) {

                    throw new Error(
                        respuesta.message ||
                        "No fue posible cargar los adjuntos."
                    );
                }


                const adjuntos =
                    respuesta.adjuntos ||
                    [];


                if (contador) {
                    contador.textContent =
                        String(
                            adjuntos.length
                        );
                }


                contenedor.innerHTML =
                    "";


                if (
                    adjuntos.length === 0
                ) {

                    contenedor.innerHTML = `
                        <div class="sd-empty-detail">

                            <i class="bi bi-paperclip"></i>

                            <strong>
                                Sin archivos adjuntos
                            </strong>

                            <span>
                                Todavía no existen evidencias registradas.
                            </span>

                        </div>
                    `;

                    return;
                }


                adjuntos.forEach(
                    function (archivo) {

                        const elemento =
                            document.createElement(
                                "div"
                            );

                        elemento.className =
                            "sd-attachment-item";


                        elemento.innerHTML = `
                            <div class="sd-attachment-icon">

                                <i class="bi ${obtenerIconoArchivo(
                            archivo.extension
                        )}"></i>

                            </div>

                            <div class="sd-attachment-info">

                                <strong>
                                    ${escaparHtml(
                            archivo.nombre
                        )}
                                </strong>

                                <span>
                                    ${escaparHtml(
                            archivo.tamano
                        )}
                                    ·
                                    ${escaparHtml(
                            archivo.usuario
                        )}
                                    ·
                                    ${escaparHtml(
                            archivo.fecha
                        )}
                                </span>

                            </div>

                            <a href="${archivo.urlDescarga}"
                               class="sd-attachment-download"
                               title="Descargar archivo">

                                <i class="bi bi-download"></i>

                            </a>
                        `;


                        contenedor.appendChild(
                            elemento
                        );
                    }
                );

            }
            catch (error) {

                console.error(
                    "Error al cargar adjuntos:",
                    error
                );


                contenedor.innerHTML = `
                    <div class="alert alert-danger mb-0">
                        No fue posible cargar los archivos adjuntos.
                    </div>
                `;
            }
        }


        // =================================================
        // MENSAJE ADJUNTO
        // =================================================

        function mostrarMensajeAdjunto(
            tipo,
            texto
        ) {

            const mensaje =
                document.getElementById(
                    "mensajeAdjuntoTicket"
                );


            if (!mensaje) {
                return;
            }


            mensaje.className =
                `alert alert-${tipo} mt-3 mb-0`;


            mensaje.textContent =
                texto;


            mensaje.classList.remove(
                "d-none"
            );
        }


        // =================================================
        // ICONO ARCHIVO
        // =================================================

        function obtenerIconoArchivo(
            extension
        ) {

            switch (
            (
                extension ||
                ""
            ).toLowerCase()
            ) {

                case ".png":
                case ".jpg":
                case ".jpeg":

                    return "bi-file-earmark-image";


                case ".pdf":

                    return "bi-file-earmark-pdf";


                case ".doc":
                case ".docx":

                    return "bi-file-earmark-word";


                case ".xls":
                case ".xlsx":

                    return "bi-file-earmark-excel";


                default:

                    return "bi-file-earmark";
            }
        }


        // =================================================
        // FORMATEAR TAMAÑO
        // =================================================

        function formatearTamanoArchivoJs(
            bytes
        ) {

            if (bytes < 1024) {

                return `${bytes} B`;
            }


            const kb =
                bytes / 1024;


            if (kb < 1024) {

                return `${kb.toFixed(1)} KB`;
            }


            const mb =
                kb / 1024;


            return `${mb.toFixed(1)} MB`;
        }

        // =================================================
        // UTILIDADES
        // =================================================

        function establecerTexto(
            id,
            valor
        ) {

            const elemento =
                document.getElementById(
                    id
                );

            if (!elemento) {
                return;
            }

            if (
                elemento instanceof HTMLInputElement
            ) {

                elemento.value =
                    valor ?? "";

                return;
            }

            elemento.textContent =
                valor ??
                "-";
        }


        function escaparHtml(
            valor
        ) {

            const div =
                document.createElement(
                    "div"
                );

            div.textContent =
                valor ??
                "";

            return div.innerHTML;
        }


        function convertirSaltosLinea(
            valor
        ) {

            return escaparHtml(
                valor ||
                ""
            )
                .replace(
                    /\n/g,
                    "<br>"
                );
        }

    }
);