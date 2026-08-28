console.log("ADQUISICIONES JS CARGADO");

document.addEventListener(
    "DOMContentLoaded",
    function () {

        // =========================================================
        // DECISIÓN DEL GERENTE
        // =========================================================

        const modalDecisionGerenteElement =
            document.getElementById(
                "modalDecisionGerenteAdq"
            );

        const inputSolicitudDecision =
            document.getElementById(
                "SolicitudDecisionId"
            );

        const inputComentarioDecision =
            document.getElementById(
                "ComentarioDecision"
            );

        const tituloDecisionGerente =
            document.getElementById(
                "tituloDecisionGerenteAdq"
            );

        const folioDecisionGerente =
            document.getElementById(
                "folioDecisionGerenteAdq"
            );

        const mensajeDecisionGerente =
            document.getElementById(
                "mensajeDecisionGerenteAdq"
            );

        const labelComentarioDecision =
            document.getElementById(
                "labelComentarioDecisionAdq"
            );

        const ayudaComentarioDecision =
            document.getElementById(
                "ayudaComentarioDecisionAdq"
            );

        const btnConfirmarAprobacion =
            document.getElementById(
                "btnConfirmarAprobacionAdq"
            );

        const btnConfirmarRechazo =
            document.getElementById(
                "btnConfirmarRechazoAdq"
            );

        const btnEditarDesdeDetalle =
            document.getElementById(
                "btnEditarDesdeDetalleAdq"
            );


        const btnCancelarDesdeDetalle =
            document.getElementById(
                "btnCancelarDesdeDetalleAdq"
            );


        const btnEnviarDesdeDetalle =
            document.getElementById(
                "btnEnviarDesdeDetalleAdq"
            );

        let solicitudDetalleActualAdq =
            null;

        const badgeMensajesPendientesAdq =
            document.getElementById(
                "badgeMensajesPendientesAdq"
            );

        let intervaloSeguimientoAdq =
            null;

        // =========================================================
        // CHAT / HISTORIAL
        // =========================================================

        const itemTabSeguimientoAdq =
            document.getElementById(
                "itemTabSeguimientoAdq"
            );


        const itemTabHistorialAdq =
            document.getElementById(
                "itemTabHistorialAdq"
            );


        const listaComentariosAdq =
            document.getElementById(
                "listaComentariosAdq"
            );


        const listaHistorialAdq =
            document.getElementById(
                "listaHistorialAdq"
            );


        const inputSolicitudComentarioAdq =
            document.getElementById(
                "SolicitudComentarioId"
            );


        const inputNuevoComentarioAdq =
            document.getElementById(
                "NuevoComentarioAdq"
            );


        const btnEnviarComentarioAdq =
            document.getElementById(
                "btnEnviarComentarioAdq"
            );


        const seguimientoEstatusAdq =
            document.getElementById(
                "seguimientoEstatusAdq"
            );


        const formComentarioAdq =
            document.getElementById(
                "formComentarioAdq"
            );

        // =========================================================
        // ARCHIVOS DEL CHAT
        // =========================================================

        const btnSeleccionarAdjuntoComentarioAdq =
            document.getElementById(
                "btnSeleccionarAdjuntoComentarioAdq"
            );


        const inputArchivoComentarioAdq =
            document.getElementById(
                "archivoComentarioAdq"
            );


        const listaAdjuntosComentarioAdq =
            document.getElementById(
                "listaAdjuntosComentarioAdq"
            );

        let archivosComentarioSeleccionadosAdq =
            [];

        // =========================================================
        // COTIZACIONES
        // =========================================================

        const modalCotizacionElement =
            document.getElementById(
                "modalCotizacionAdq"
            );


        const formCotizacionAdq =
            document.getElementById(
                "formCotizacionAdq"
            );


        const cotizacionSolicitudIdAdq =
            document.getElementById(
                "cotizacionSolicitudIdAdq"
            );


        const folioCotizacionAdq =
            document.getElementById(
                "folioCotizacionAdq"
            );


        const contenedorDetallesCotizacionAdq =
            document.getElementById(
                "contenedorDetallesCotizacionAdq"
            );


        const cotizacionAplicaIvaAdq =
            document.getElementById(
                "cotizacionAplicaIvaAdq"
            );


        const cotizacionPorcentajeIvaAdq =
            document.getElementById(
                "cotizacionPorcentajeIvaAdq"
            );


        const cotizacionSubtotalAdq =
            document.getElementById(
                "cotizacionSubtotalAdq"
            );


        const cotizacionIvaAdq =
            document.getElementById(
                "cotizacionIvaAdq"
            );


        const cotizacionTotalAdq =
            document.getElementById(
                "cotizacionTotalAdq"
            );


        const cotizacionIvaLabelAdq =
            document.getElementById(
                "cotizacionIvaLabelAdq"
            );

        // =========================================================
        // FORMATEAR MONEDA
        // =========================================================

        function formatearMonedaCotizacionAdq(
            valor
        ) {

            const numero =
                Number(
                    valor ?? 0
                );


            return numero.toLocaleString(
                "es-MX",
                {
                    style:
                        "currency",

                    currency:
                        "MXN"
                }
            );
        }


        // =========================================================
        // CALCULAR COTIZACIÓN
        // =========================================================

        function calcularCotizacionAdq() {

            let subtotal =
                0;


            document
                .querySelectorAll(
                    ".cotizacion-precio-unitario-adq"
                )
                .forEach(
                    function (input) {

                        const cantidad =
                            Number(
                                input.dataset.cantidad
                                ??
                                0
                            );


                        const precio =
                            Number(
                                input.value
                                ??
                                0
                            );


                        const importe =
                            cantidad *
                            precio;


                        subtotal +=
                            importe;


                        const fila =
                            input.closest(
                                "tr"
                            );


                        const importeElement =
                            fila?.querySelector(
                                ".cotizacion-importe-adq"
                            );


                        if (
                            importeElement
                        ) {

                            importeElement.textContent =
                                formatearMonedaCotizacionAdq(
                                    importe
                                );
                        }
                    }
                );


            const aplicaIva =
                cotizacionAplicaIvaAdq?.checked
                ??
                false;


            const porcentajeIva =
                aplicaIva
                    ? Number(
                        cotizacionPorcentajeIvaAdq?.value
                        ??
                        0
                    )
                    : 0;


            const importeIva =
                subtotal *
                (
                    porcentajeIva /
                    100
                );


            const total =
                subtotal +
                importeIva;


            if (
                cotizacionSubtotalAdq
            ) {

                cotizacionSubtotalAdq.textContent =
                    formatearMonedaCotizacionAdq(
                        subtotal
                    );
            }


            if (
                cotizacionIvaAdq
            ) {

                cotizacionIvaAdq.textContent =
                    formatearMonedaCotizacionAdq(
                        importeIva
                    );
            }


            if (
                cotizacionTotalAdq
            ) {

                cotizacionTotalAdq.textContent =
                    formatearMonedaCotizacionAdq(
                        total
                    );
            }


            if (
                cotizacionIvaLabelAdq
            ) {

                cotizacionIvaLabelAdq.textContent =
                    aplicaIva
                        ? `IVA ${porcentajeIva}%`
                        : "IVA";
            }


            if (
                cotizacionPorcentajeIvaAdq
            ) {

                cotizacionPorcentajeIvaAdq.disabled =
                    !aplicaIva;
            }
        }

        // =========================================================
        // RENDERIZAR PRODUCTOS DE COTIZACIÓN
        // =========================================================

        function renderizarDetallesCotizacionAdq(
            detalles
        ) {

            if (
                !contenedorDetallesCotizacionAdq
            ) {
                return;
            }


            contenedorDetallesCotizacionAdq.innerHTML =
                "";


            if (
                !Array.isArray(
                    detalles
                )
                ||
                detalles.length ===
                0
            ) {

                contenedorDetallesCotizacionAdq.innerHTML = `
            <tr>

                <td colspan="5"
                    class="text-center text-muted py-4">

                    La solicitud no contiene productos.

                </td>

            </tr>
        `;

                return;
            }


            detalles.forEach(
                function (
                    detalle,
                    index
                ) {

                    const fila =
                        document.createElement(
                            "tr"
                        );


                    const cantidad =
                        Number(
                            detalle.cantidad
                            ??
                            0
                        );


                    fila.innerHTML = `
                <td>

                    <strong>
                        ${escapeHtmlAdq(
                        detalle.productoServicio
                        ??
                        ""
                    )}
                    </strong>

                    ${detalle.descripcion
                            ? `
                                <small class="d-block text-muted mt-1">

                                    ${escapeHtmlAdq(
                                detalle.descripcion
                            )}

                                </small>
                            `
                            : ""
                        }

                    <input type="hidden"
                           name="InputCotizacion.Detalles[${index}].SolicitudDetalleId"
                           value="${Number(
                            detalle.id
                            ??
                            0
                        )}" />

                </td>


                <td>

                    ${cantidad}

                </td>


                <td>

                    ${escapeHtmlAdq(
                            detalle.unidad
                            ??
                            ""
                        )}

                </td>


                <td>

                    <div class="input-group input-group-sm">

                        <span class="input-group-text">
                            $
                        </span>

                        <input type="number"
                               class="form-control cotizacion-precio-unitario-adq"
                               name="InputCotizacion.Detalles[${index}].PrecioUnitario"
                               min="0.01"
                               step="0.01"
                               data-cantidad="${cantidad}"
                               required />

                    </div>

                </td>


                <td class="text-end">

                    <strong class="cotizacion-importe-adq">
                        $0.00
                    </strong>

                </td>
            `;


                    contenedorDetallesCotizacionAdq
                        .appendChild(
                            fila
                        );
                }
            );


            calcularCotizacionAdq();
        }

        // =========================================================
        // ABRIR COTIZACIÓN
        // =========================================================

        document.addEventListener(
            "click",
            async function (
                event
            ) {

                const boton =
                    event.target.closest(
                        ".btnGenerarCotizacionAdq"
                    );


                if (!boton) {
                    return;
                }


                const solicitudId =
                    Number(
                        boton.dataset.id
                        ??
                        0
                    );


                if (
                    solicitudId <=
                    0
                ) {
                    return;
                }


                try {

                    boton.disabled =
                        true;


                    const solicitud =
                        await obtenerSolicitudAdq(
                            solicitudId
                        );


                    if (
                        !solicitud
                    ) {
                        throw new Error(
                            "No fue posible obtener la solicitud."
                        );
                    }


                    if (
                        cotizacionSolicitudIdAdq
                    ) {

                        cotizacionSolicitudIdAdq.value =
                            String(
                                solicitud.id
                            );
                    }


                    if (
                        folioCotizacionAdq
                    ) {

                        folioCotizacionAdq.textContent =
                            `${solicitud.folio ?? ""} · ${solicitud.titulo ?? ""}`;
                    }


                    if (
                        formCotizacionAdq
                    ) {

                        formCotizacionAdq.reset();
                    }


                    /*
                     * reset() restaura el hidden,
                     * así que volvemos a colocar el ID.
                     */
                    if (
                        cotizacionSolicitudIdAdq
                    ) {

                        cotizacionSolicitudIdAdq.value =
                            String(
                                solicitud.id
                            );
                    }


                    if (
                        cotizacionAplicaIvaAdq
                    ) {

                        cotizacionAplicaIvaAdq.checked =
                            true;
                    }


                    if (
                        cotizacionPorcentajeIvaAdq
                    ) {

                        cotizacionPorcentajeIvaAdq.value =
                            "16";
                    }


                    renderizarDetallesCotizacionAdq(
                        solicitud.detalles
                    );


                    bootstrap.Modal
                        .getOrCreateInstance(
                            modalCotizacionElement
                        )
                        .show();

                }
                catch (
                error
                ) {

                    console.error(
                        "Error al preparar cotización:",
                        error
                    );


                    alert(
                        error.message
                        ??
                        "No fue posible preparar la cotización."
                    );

                }
                finally {

                    boton.disabled =
                        false;
                }
            }
        );

        // =========================================================
        // EVENTOS DE CÁLCULO
        // =========================================================

        document.addEventListener(
            "input",
            function (
                event
            ) {

                if (
                    event.target.matches(
                        ".cotizacion-precio-unitario-adq"
                    )
                ) {

                    calcularCotizacionAdq();
                }
            }
        );


        cotizacionPorcentajeIvaAdq
            ?.addEventListener(
                "input",
                calcularCotizacionAdq
            );


        cotizacionAplicaIvaAdq
            ?.addEventListener(
                "change",
                calcularCotizacionAdq
            );

        // =========================================================
        // MIS ÓRDENES ASIGNADAS - ABRIR CHAT
        // =========================================================

        document.addEventListener(
            "click",
            function (event) {

                const botonChat =
                    event.target.closest(
                        ".btnChatOrdenAdq"
                    );

                if (!botonChat) {
                    return;
                }

                const solicitudId =
                    Number(
                        botonChat.dataset.id
                        ??
                        0
                    );

                if (solicitudId <= 0) {
                    return;
                }

                const botonVer =
                    document.querySelector(
                        `.btnVerSolicitudAdq[data-id="${solicitudId}"]`
                    );

                if (!botonVer) {
                    return;
                }

                botonVer.click();


                setTimeout(
                    function () {

                        const tabChat =
                            document.querySelector(
                                "#tabSeguimientoAdq"
                            );

                        if (tabChat) {
                            tabChat.click();
                        }

                    },
                    350
                );
            }
        );

        // =========================================================
        // OBTENER CHAT / HISTORIAL
        // =========================================================

        async function obtenerSeguimientoAdq(
            solicitudId
        ) {

            const response =
                await fetch(
                    `?handler=SeguimientoSolicitud&id=${encodeURIComponent(
                        solicitudId
                    )}`,
                    {
                        method:
                            "GET",

                        headers: {
                            "X-Requested-With":
                                "XMLHttpRequest"
                        }
                    }
                );


            const resultado =
                await response.json();


            if (
                !response.ok
                ||
                !resultado.success
            ) {

                throw new Error(
                    resultado.message
                    ??
                    "No fue posible consultar el seguimiento."
                );
            }


            return resultado.seguimiento;
        }

        // =========================================================
        // RENDERIZAR CHAT
        // =========================================================

        function renderizarComentariosAdq(
            comentarios
        ) {

            if (
                !listaComentariosAdq
            ) {
                return;
            }


            listaComentariosAdq.innerHTML =
                "";


            if (
                !Array.isArray(
                    comentarios
                )
                ||
                comentarios.length ===
                0
            ) {

                listaComentariosAdq.innerHTML = `
            <div class="adq-chat-empty">

                <i class="bi bi-chat-square-dots"></i>

                <strong>
                    Aún no hay mensajes
                </strong>

                <span>
                    Inicia la conversación relacionada
                    con esta solicitud.
                </span>

            </div>
        `;

                return;
            }


            comentarios.forEach(
                function (
                    comentario
                ) {

                    const elemento =
                        document.createElement(
                            "div"
                        );


                    elemento.className =
                        comentario.esUsuarioActual
                            ? "adq-chat-message adq-chat-message-own"
                            : "adq-chat-message";


                    const fecha =
                        new Date(
                            comentario.fechaCreacion
                        );


                    const adjuntos =
                        Array.isArray(
                            comentario.adjuntos
                        )
                            ? comentario.adjuntos
                            : [];


                    let htmlComentario =
                        "";


                    /*
                     * Un mensaje puede contener solamente
                     * archivos, por eso el texto es opcional.
                     */
                    if (
                        comentario.comentario
                        &&
                        comentario.comentario.trim()
                    ) {
                        htmlComentario = `
                    <div class="adq-chat-bubble">
                        ${escapeHtmlAdq(
                            comentario.comentario
                        )}
                    </div>
                `;
                    }


                    let htmlAdjuntos =
                        "";


                    if (
                        adjuntos.length >
                        0
                    ) {

                        htmlAdjuntos = `
                    <div class="adq-chat-attachments">

                        ${adjuntos
                                .map(
                                    function (
                                        archivo
                                    ) {

                                        return `
                                            <a href="${escapeAttributeAdq(
                                                                                    archivo.rutaArchivo
                                                                                )}"
                                               target="_blank"
                                               rel="noopener noreferrer"
                                               class="adq-chat-attachment"
                                               title="Abrir ${escapeAttributeAdq(
                                                                                    archivo.nombreOriginal
                                                                                )}">

                                                <div class="adq-chat-file-icon">

                                                    <i class="bi bi-file-earmark"></i>

                                                </div>


                                                <div class="adq-chat-file-name">

                                                    ${escapeHtmlAdq(
                                                                                    archivo.nombreOriginal
                                                                                )}

                                                </div>


                                                <div class="adq-chat-file-size">

                                                    ${formatearTamanoAdq(
                                                                                    Number(
                                                                                        archivo.tamanoBytes
                                                                                        ??
                                                                                        0
                                                                                    )
                                                                                )}

                                                </div>


                                                <div class="adq-chat-file-action">

                                                    <i class="bi bi-box-arrow-up-right"></i>

                                                </div>

                                            </a>
                                        `;
                                    }
                                )
                                .join(
                                    ""
                                )
                            }

                    </div>
                `;
                    }


                    elemento.innerHTML = `
                <div class="adq-chat-message-header">

                    <strong>

                        ${comentario.esUsuarioActual
                            ? "Tú"
                            : escapeHtmlAdq(
                                comentario.usuario
                            )
                        }

                    </strong>

                    <span>

                        ${fecha.toLocaleString(
                            "es-MX"
                        )}

                    </span>

                </div>


                ${htmlComentario}

                ${htmlAdjuntos}
            `;


                    listaComentariosAdq
                        .appendChild(
                            elemento
                        );
                }
            );


            listaComentariosAdq.scrollTop =
                listaComentariosAdq.scrollHeight;
        }

        // =========================================================
        // RENDERIZAR HISTORIAL
        // =========================================================

        function renderizarHistorialAdq(
            historial
        ) {

            if (
                !listaHistorialAdq
            ) {
                return;
            }


            listaHistorialAdq.innerHTML =
                "";


            if (
                !Array.isArray(
                    historial
                )
                ||
                historial.length ===
                0
            ) {

                listaHistorialAdq.innerHTML = `
                    <div class="text-muted small">
                        No existen movimientos registrados.
                    </div>
                `;

                return;
            }


            historial.forEach(
                function (
                    evento
                ) {

                    const fecha =
                        new Date(
                            evento.fechaEvento
                        );


                    const item =
                        document.createElement(
                            "div"
                        );


                    item.className =
                        "adq-history-item";


                    item.innerHTML = `
                        <div class="adq-history-marker">

                            <i class="bi bi-circle-fill"></i>

                        </div>


                        <div class="adq-history-content">

                            <strong>
                                ${escapeHtmlAdq(
                        evento.descripcion
                    )}
                            </strong>

                            <span>
                                ${escapeHtmlAdq(
                        evento.usuario
                        ||
                        "Sistema"
                    )
                        }
                                ·
                                ${fecha.toLocaleString(
                            "es-MX"
                        )}
                            </span>

                        </div>
                    `;


                    listaHistorialAdq
                        .appendChild(
                            item
                        );
                }
            );
        }

        // =========================================================
        // CARGAR SEGUIMIENTO
        // =========================================================

        async function cargarSeguimientoAdq(
            solicitudId
        ) {

            const seguimiento =
                await obtenerSeguimientoAdq(
                    solicitudId
                );

            if (
                badgeMensajesPendientesAdq
            ) {

                const pendientes =
                    Number(
                        seguimiento.mensajesPendientes ??
                        0
                    );


                if (
                    pendientes > 0
                ) {

                    badgeMensajesPendientesAdq
                        .classList
                        .remove(
                            "d-none"
                        );


                    badgeMensajesPendientesAdq.textContent =
                        String(
                            pendientes
                        );

                }
                else {

                    badgeMensajesPendientesAdq
                        .classList
                        .add(
                            "d-none"
                        );


                    badgeMensajesPendientesAdq.textContent =
                        "";
                }
            }


            if (
                inputSolicitudComentarioAdq
            ) {

                inputSolicitudComentarioAdq.value =
                    String(
                        solicitudId
                    );
            }


            if (
                seguimientoEstatusAdq
            ) {

                seguimientoEstatusAdq.textContent =
                    seguimiento.estatus
                    ??
                    "";
            }


            /*
             * La escritura del chat la determina el backend.
             * Puede incluir solicitante, gerente/aprobador,
             * Adquisiciones y agente asignado.
             */
            if (
                formComentarioAdq
            ) {

                formComentarioAdq.classList.toggle(
                    "d-none",
                    seguimiento.puedeEscribir !==
                    true
                );
            }


            renderizarComentariosAdq(
                seguimiento.comentarios
            );


            renderizarHistorialAdq(
                seguimiento.historial
            );
        }


        document.addEventListener(
            "click",
            function (event) {

                const boton =
                    event.target.closest(
                        ".btnDecisionAprobacionAdq"
                    );


                if (!boton) {
                    return;
                }


                const id =
                    boton.dataset.id;

                const folio =
                    boton.dataset.folio ??
                    "";

                const accion =
                    boton.dataset.accion;


                inputSolicitudDecision.value =
                    id;

                inputComentarioDecision.value =
                    "";


                folioDecisionGerente.textContent =
                    folio;


                if (
                    accion ===
                    "aprobar"
                ) {
                    tituloDecisionGerente.textContent =
                        "Aprobar solicitud";


                    mensajeDecisionGerente.className =
                        "alert alert-success mb-3";


                    mensajeDecisionGerente.innerHTML = `
                        <i class="bi bi-check-circle me-1"></i>

                        La solicitud será aprobada y enviada
                        al área de Adquisiciones.
                    `;


                    labelComentarioDecision.textContent =
                        "Comentario (opcional)";


                    ayudaComentarioDecision.textContent =
                        "Puedes agregar una observación para el solicitante.";


                    inputComentarioDecision.required =
                        false;


                    btnConfirmarAprobacion
                        .classList.remove(
                            "d-none"
                        );


                    btnConfirmarRechazo
                        .classList.add(
                            "d-none"
                        );
                }
                else {
                    tituloDecisionGerente.textContent =
                        "Rechazar solicitud";


                    mensajeDecisionGerente.className =
                        "alert alert-danger mb-3";


                    mensajeDecisionGerente.innerHTML = `
                        <i class="bi bi-exclamation-triangle me-1"></i>

                        La solicitud quedará marcada
                        como rechazada.
                    `;


                    labelComentarioDecision.textContent =
                        "Motivo del rechazo";


                    ayudaComentarioDecision.textContent =
                        "El motivo es obligatorio y quedará registrado en el historial.";


                    inputComentarioDecision.required =
                        true;


                    btnConfirmarRechazo
                        .classList.remove(
                            "d-none"
                        );


                    btnConfirmarAprobacion
                        .classList.add(
                            "d-none"
                        );
                }


                bootstrap.Modal
                    .getOrCreateInstance(
                        modalDecisionGerenteElement
                    )
                    .show();

            }
        );

        // =========================================================
        // REFERENCIAS GENERALES
        // =========================================================

        const formulario =
            document.getElementById(
                "formNuevaSolicitudAdq"
            );

        const modalNuevaSolicitudElement =
            document.getElementById(
                "modalNuevaSolicitud"
            );

        const modalVerSolicitudElement =
            document.getElementById(
                "modalVerSolicitudAdq"
            );

        const btnNuevaSolicitud =
            document.getElementById(
                "btnNuevaSolicitudAdq"
            );

        const btnEnviar =
            document.getElementById(
                "btnEnviarSolicitudAdq"
            );

        const btnGuardarBorrador =
            document.getElementById(
                "btnGuardarBorradorAdq"
            );

        const btnGuardarCambios =
            document.getElementById(
                "btnGuardarCambiosAdq"
            );

        const btnEnviarBorrador =
            document.getElementById(
                "btnEnviarBorradorAdq"
            );

        const solicitudEditarId =
            document.getElementById(
                "SolicitudEditarId"
            );

        const tituloModal =
            document.getElementById(
                "modalNuevaSolicitudLabel"
            );


        // =========================================================
        // PRODUCTOS
        // =========================================================

        const contenedorDetalles =
            document.getElementById(
                "contenedorDetallesAdq"
            );

        const btnAgregarProducto =
            document.getElementById(
                "btnAgregarDetalleAdq"
            );

        const inputProducto =
            document.getElementById(
                "adqProductoServicio"
            );

        const inputCantidad =
            document.getElementById(
                "adqCantidad"
            );

        const inputUnidad =
            document.getElementById(
                "adqUnidad"
            );

        const inputDescripcionProducto =
            document.getElementById(
                "adqDescripcionProducto"
            );

        const contadorProductos =
            document.getElementById(
                "adqContadorProductos"
            );


        // =========================================================
        // ARCHIVOS
        // =========================================================

        const inputArchivos =
            document.getElementById(
                "ArchivosSolicitud"
            );

        const btnAgregarArchivos =
            document.getElementById(
                "btnAgregarArchivosAdq"
            );

        const listaArchivos =
            document.getElementById(
                "listaArchivosAdq"
            );

        const contadorArchivos =
            document.getElementById(
                "contadorArchivosAdq"
            );


        /*
         * Aquí conservaremos todos los archivos
         * aunque el usuario abra varias veces
         * el selector.
         */
        let archivosSeleccionadosAdq =
            [];

        /*
         * Archivos que ya existen en la BD
         * cuando estamos editando una solicitud.
         */
        let archivosExistentesAdq =
            [];


        /*
         * IDs de archivos existentes que el
         * usuario decidió eliminar.
         */
        let adjuntosEliminarIdsAdq =
            [];


        const extensionesPermitidasAdq =
            [
                ".pdf",
                ".doc",
                ".docx",
                ".xls",
                ".xlsx",
                ".png",
                ".jpg",
                ".jpeg"
            ];

        const tamanoMaximoArchivoAdq =
            15 * 1024 * 1024;


        // =========================================================
        // FILTROS
        // =========================================================

        const filtroBusqueda =
            document.getElementById(
                "adqFiltroBusqueda"
            );

        const filtroEstatus =
            document.getElementById(
                "adqFiltroEstatus"
            );

        const btnLimpiarFiltros =
            document.getElementById(
                "btnLimpiarFiltrosAdq"
            );

        // =========================================================
        // PAGINACIÓN - REFERENCIAS
        // =========================================================

        const paginacionLista =
            document.getElementById(
                "adqPaginacionLista"
            );


        const paginacionContenedor =
            document.getElementById(
                "paginacionSolicitudesAdq"
            );


        const paginaInicio =
            document.getElementById(
                "adqPaginaInicio"
            );


        const paginaFin =
            document.getElementById(
                "adqPaginaFin"
            );


        const paginaTotal =
            document.getElementById(
                "adqPaginaTotal"
            );


        const registrosPorPaginaAdq =
            10;


        let paginaActualAdq =
            1;


        // =========================================================
        // HISTORIAL DE APROBACIONES - FILTROS + PAGINACIÓN
        // =========================================================

        const filtroHistorialAprobacionesAdq =
            document.getElementById(
                "adqFiltroHistorialAprobaciones"
            );

        const filtroDecisionHistorialAdq =
            document.getElementById(
                "adqFiltroDecisionHistorial"
            );

        const btnLimpiarHistorialAprobacionesAdq =
            document.getElementById(
                "btnLimpiarHistorialAprobaciones"
            );

        const paginacionHistorialAdq =
            document.getElementById(
                "paginacionHistorialAprobacionesAdq"
            );

        const listaPaginacionHistorialAdq =
            document.getElementById(
                "adqHistorialPaginacionLista"
            );

        const historialPaginaInicioAdq =
            document.getElementById(
                "adqHistorialPaginaInicio"
            );

        const historialPaginaFinAdq =
            document.getElementById(
                "adqHistorialPaginaFin"
            );

        const historialPaginaTotalAdq =
            document.getElementById(
                "adqHistorialPaginaTotal"
            );

        const registrosPorPaginaHistorialAdq =
            10;

        let paginaActualHistorialAdq =
            1;

        // =========================================================
        // UTILIDADES
        // =========================================================

        function escapeHtmlAdq(
            texto
        ) {

            const elemento =
                document.createElement(
                    "div"
                );

            elemento.textContent =
                texto ?? "";

            return elemento.innerHTML;
        }


        function escapeAttributeAdq(
            texto
        ) {

            return String(
                texto ?? ""
            )
                .replaceAll(
                    "&",
                    "&amp;"
                )
                .replaceAll(
                    "\"",
                    "&quot;"
                )
                .replaceAll(
                    "<",
                    "&lt;"
                )
                .replaceAll(
                    ">",
                    "&gt;"
                );
        }


        function obtenerExtensionAdq(
            nombreArchivo
        ) {

            const indice =
                nombreArchivo.lastIndexOf(
                    "."
                );

            if (indice < 0) {
                return "";
            }

            return nombreArchivo
                .substring(
                    indice
                )
                .toLowerCase();
        }


        function formatearTamanoAdq(
            bytes
        ) {

            if (bytes < 1024) {

                return `${bytes} B`;

            }

            if (
                bytes <
                1024 * 1024
            ) {

                return (
                    bytes /
                    1024
                )
                    .toFixed(2) +
                    " KB";
            }

            return (
                bytes /
                1024 /
                1024
            )
                .toFixed(2) +
                " MB";
        }


        function mostrarAdvertenciaAdq(
            titulo,
            mensaje
        ) {

            if (window.Swal) {

                Swal.fire({
                    icon: "warning",
                    title: titulo,
                    text: mensaje
                });

                return;
            }

            alert(
                `${titulo}\n${mensaje}`
            );
        }

        // =========================================================
        // CHAT - RENDERIZAR ARCHIVOS SELECCIONADOS
        // =========================================================

        function renderizarAdjuntosComentarioSeleccionadosAdq() {

            if (
                !listaAdjuntosComentarioAdq
            ) {
                return;
            }


            listaAdjuntosComentarioAdq.innerHTML =
                "";


            if (
                archivosComentarioSeleccionadosAdq.length ===
                0
            ) {
                return;
            }


            archivosComentarioSeleccionadosAdq
                .forEach(
                    function (
                        archivo,
                        indice
                    ) {

                        const item =
                            document.createElement(
                                "div"
                            );


                        item.className =
                            "adq-chat-selected-file";


                        item.innerHTML = `
                            <div class="adq-chat-file-icon">

                                <i class="bi bi-file-earmark"></i>

                            </div>


                            <div class="adq-chat-file-name"
                                 title="${escapeAttributeAdq(
                                                    archivo.name
                                                )}">

                                ${escapeHtmlAdq(
                                                    archivo.name
                                                )}

                            </div>


                            <div class="adq-chat-file-size">

                                ${formatearTamanoAdq(
                                                    archivo.size
                                                )}

                            </div>


                            <button type="button"
                                    class="adq-chat-file-action adq-chat-file-remove btnEliminarAdjuntoComentarioAdq"
                                    data-index="${indice}"
                                    title="Quitar archivo"
                                    aria-label="Quitar archivo">

                                <i class="bi bi-x-lg"></i>

                            </button>
                        `;


                        listaAdjuntosComentarioAdq
                            .appendChild(
                                item
                            );
                    }
                );
        }


        // =========================================================
        // CHAT - AGREGAR ARCHIVOS
        // =========================================================

        function agregarAdjuntosComentarioAdq(
            archivosNuevos
        ) {

            const clavesExistentes =
                new Set(
                    archivosComentarioSeleccionadosAdq
                        .map(
                            obtenerClaveArchivoAdq
                        )
                );


            archivosNuevos.forEach(
                function (
                    archivo
                ) {

                    const clave =
                        obtenerClaveArchivoAdq(
                            archivo
                        );


                    /*
                     * Evitamos agregar el mismo archivo
                     * varias veces.
                     */
                    if (
                        clavesExistentes.has(
                            clave
                        )
                    ) {
                        return;
                    }


                    const validacion =
                        archivoValidoAdq(
                            archivo
                        );


                    if (
                        !validacion.formatoValido
                    ) {
                        mostrarAdvertenciaAdq(
                            "Formato no permitido",
                            `El archivo ${archivo.name} no tiene un formato permitido.`
                        );

                        return;
                    }


                    if (
                        !validacion.tamanoValido
                    ) {
                        mostrarAdvertenciaAdq(
                            "Archivo demasiado grande",
                            `El archivo ${archivo.name} supera el límite de 15 MB.`
                        );

                        return;
                    }


                    archivosComentarioSeleccionadosAdq
                        .push(
                            archivo
                        );


                    clavesExistentes.add(
                        clave
                    );
                }
            );


            renderizarAdjuntosComentarioSeleccionadosAdq();
        }


        // =========================================================
        // CHAT - ELIMINAR ARCHIVO SELECCIONADO
        // =========================================================

        function eliminarAdjuntoComentarioAdq(
            indice
        ) {

            if (
                indice < 0
                ||
                indice >=
                archivosComentarioSeleccionadosAdq.length
            ) {
                return;
            }


            archivosComentarioSeleccionadosAdq
                .splice(
                    indice,
                    1
                );


            renderizarAdjuntosComentarioSeleccionadosAdq();
        }


        // =========================================================
        // CHAT - LIMPIAR ARCHIVOS
        // =========================================================

        function limpiarAdjuntosComentarioAdq() {

            archivosComentarioSeleccionadosAdq =
                [];


            if (
                inputArchivoComentarioAdq
            ) {
                inputArchivoComentarioAdq.value =
                    "";
            }


            renderizarAdjuntosComentarioSeleccionadosAdq();
        }

        // =========================================================
        // CHAT - EVENTOS DE ARCHIVOS
        // =========================================================

        btnSeleccionarAdjuntoComentarioAdq
            ?.addEventListener(
                "click",
                function () {

                    inputArchivoComentarioAdq
                        ?.click();
                }
            );


        inputArchivoComentarioAdq
            ?.addEventListener(
                "change",
                function () {

                    const archivos =
                        Array.from(
                            inputArchivoComentarioAdq.files
                            ??
                            []
                        );


                    agregarAdjuntosComentarioAdq(
                        archivos
                    );


                    /*
                     * Limpiamos el input real para permitir
                     * volver a seleccionar archivos.
                     *
                     * Los File quedan almacenados en nuestro
                     * arreglo independiente.
                     */
                    inputArchivoComentarioAdq.value =
                        "";
                }
            );


        listaAdjuntosComentarioAdq
            ?.addEventListener(
                "click",
                function (
                    event
                ) {

                    const boton =
                        event.target.closest(
                            ".btnEliminarAdjuntoComentarioAdq"
                        );


                    if (
                        !boton
                    ) {
                        return;
                    }


                    const indice =
                        Number(
                            boton.dataset.index
                        );


                    if (
                        Number.isNaN(
                            indice
                        )
                    ) {
                        return;
                    }


                    eliminarAdjuntoComentarioAdq(
                        indice
                    );
                }
            );


        // =========================================================
        // PRODUCTOS - CREAR ELEMENTO
        // =========================================================

        function crearElementoProductoAdq(
            producto,
            cantidad,
            unidad,
            descripcion
        ) {

            const fila =
                document.createElement(
                    "div"
                );

            fila.className =
                "adq-product-item";

            fila.innerHTML = `
                <div class="adq-product-main">

                    <div class="adq-product-icon">

                        <i class="bi bi-box-seam"></i>

                    </div>

                    <div class="adq-product-information">

                        <strong class="adq-product-name">
                            ${escapeHtmlAdq(producto)}
                        </strong>

                        <span class="adq-product-description">
                            ${descripcion
                    ? escapeHtmlAdq(
                        descripcion
                    )
                    : "Sin descripción adicional"
                }
                        </span>

                    </div>

                </div>

                <div class="adq-product-data">

                    <div>

                        <span>
                            Cantidad
                        </span>

                        <strong>
                            ${escapeHtmlAdq(cantidad)}
                        </strong>

                    </div>

                    <div>

                        <span>
                            Unidad
                        </span>

                        <strong>
                            ${escapeHtmlAdq(unidad)}
                        </strong>

                    </div>

                </div>

                <button type="button"
                        class="btn btn-outline-danger btnEliminarDetalleAdq"
                        title="Eliminar producto">

                    <i class="bi bi-trash"></i>

                </button>

                <input type="hidden"
                       data-field="ProductoServicio"
                       value="${escapeAttributeAdq(producto)}" />

                <input type="hidden"
                       data-field="Cantidad"
                       value="${escapeAttributeAdq(cantidad)}" />

                <input type="hidden"
                       data-field="Unidad"
                       value="${escapeAttributeAdq(unidad)}" />

                <input type="hidden"
                       data-field="Descripcion"
                       value="${escapeAttributeAdq(descripcion)}" />
            `;

            return fila;
        }


        // =========================================================
        // PRODUCTOS - AGREGAR
        // =========================================================

        function agregarProductoAdq() {

            if (
                !contenedorDetalles ||
                !inputProducto ||
                !inputCantidad ||
                !inputUnidad
            ) {
                return;
            }

            const producto =
                inputProducto.value
                    .trim();

            const cantidadTexto =
                inputCantidad.value
                    .trim();

            const cantidad =
                Number(
                    cantidadTexto
                );

            const unidad =
                inputUnidad.value
                    .trim();

            const descripcion =
                inputDescripcionProducto
                    ?.value
                    .trim() ??
                "";


            if (!producto) {

                mostrarAdvertenciaAdq(
                    "Producto incompleto",
                    "Captura el producto o servicio."
                );

                inputProducto.focus();

                return;
            }


            if (
                !cantidadTexto ||
                Number.isNaN(
                    cantidad
                ) ||
                cantidad <= 0
            ) {

                mostrarAdvertenciaAdq(
                    "Cantidad incorrecta",
                    "La cantidad debe ser mayor a cero."
                );

                inputCantidad.focus();

                return;
            }


            if (!unidad) {

                mostrarAdvertenciaAdq(
                    "Unidad requerida",
                    "Selecciona una unidad."
                );

                inputUnidad.focus();

                return;
            }


            document
                .getElementById(
                    "adqProductosVacio"
                )
                ?.remove();


            const elemento =
                crearElementoProductoAdq(
                    producto,
                    cantidadTexto,
                    unidad,
                    descripcion
                );


            contenedorDetalles
                .appendChild(
                    elemento
                );


            renumerarProductosAdq();

            actualizarContadorProductosAdq();

            limpiarCapturaProductoAdq();

            actualizarEstadoEnviar();
        }


        // =========================================================
        // PRODUCTOS - CARGAR EXISTENTE
        // =========================================================

        function agregarProductoExistenteAdq(
            detalle
        ) {

            if (!contenedorDetalles) {
                return;
            }


            document
                .getElementById(
                    "adqProductosVacio"
                )
                ?.remove();


            const elemento =
                crearElementoProductoAdq(
                    detalle.productoServicio,
                    detalle.cantidad,
                    detalle.unidad,
                    detalle.descripcion ?? ""
                );


            contenedorDetalles
                .appendChild(
                    elemento
                );


            renumerarProductosAdq();

            actualizarContadorProductosAdq();
        }


        // =========================================================
        // PRODUCTOS - ELIMINAR
        // =========================================================

        function eliminarProductoAdq(
            boton
        ) {

            const item =
                boton.closest(
                    ".adq-product-item"
                );

            if (!item) {
                return;
            }


            item.remove();


            renumerarProductosAdq();

            actualizarContadorProductosAdq();

            verificarListaProductosVaciaAdq();

            actualizarEstadoEnviar();
        }


        // =========================================================
        // PRODUCTOS - RENUMERAR
        // =========================================================

        function renumerarProductosAdq() {

            if (!contenedorDetalles) {
                return;
            }


            const productos =
                contenedorDetalles
                    .querySelectorAll(
                        ".adq-product-item"
                    );


            productos.forEach(
                function (
                    producto,
                    indice
                ) {

                    const campos =
                        producto
                            .querySelectorAll(
                                "[data-field]"
                            );


                    campos.forEach(
                        function (
                            campo
                        ) {

                            const nombre =
                                campo.dataset.field;


                            campo.name =
                                `Input.Detalles[${indice}].${nombre}`;
                        }
                    );

                }
            );
        }


        // =========================================================
        // PRODUCTOS - LIMPIAR CAPTURA
        // =========================================================

        function limpiarCapturaProductoAdq() {

            if (inputProducto) {
                inputProducto.value =
                    "";
            }

            if (inputCantidad) {
                inputCantidad.value =
                    "1";
            }

            if (inputUnidad) {
                inputUnidad.value =
                    "";
            }

            if (
                inputDescripcionProducto
            ) {
                inputDescripcionProducto.value =
                    "";
            }


            inputProducto?.focus();
        }


        // =========================================================
        // PRODUCTOS - CONTADOR
        // =========================================================

        function actualizarContadorProductosAdq() {

            if (
                !contenedorDetalles ||
                !contadorProductos
            ) {
                return;
            }


            const cantidad =
                contenedorDetalles
                    .querySelectorAll(
                        ".adq-product-item"
                    )
                    .length;


            contadorProductos.textContent =
                cantidad === 1
                    ? "1 producto"
                    : `${cantidad} productos`;
        }


        // =========================================================
        // PRODUCTOS - VACÍO
        // =========================================================

        function verificarListaProductosVaciaAdq() {

            if (!contenedorDetalles) {
                return;
            }


            const cantidad =
                contenedorDetalles
                    .querySelectorAll(
                        ".adq-product-item"
                    )
                    .length;


            if (cantidad > 0) {
                return;
            }


            if (
                document.getElementById(
                    "adqProductosVacio"
                )
            ) {
                return;
            }


            const vacio =
                document.createElement(
                    "div"
                );


            vacio.id =
                "adqProductosVacio";

            vacio.className =
                "adq-products-empty";


            vacio.innerHTML = `
                <i class="bi bi-box"></i>

                <strong>
                    Aún no has agregado productos o servicios
                </strong>

                <span>
                    Captura la información superior
                    y presiona el botón +.
                </span>
            `;


            contenedorDetalles
                .appendChild(
                    vacio
                );
        }


        function limpiarProductosAdq() {

            if (!contenedorDetalles) {
                return;
            }


            contenedorDetalles.innerHTML =
                "";


            verificarListaProductosVaciaAdq();

            actualizarContadorProductosAdq();
        }


        // =========================================================
        // EVENTOS DE PRODUCTOS
        // =========================================================

        btnAgregarProducto
            ?.addEventListener(
                "click",
                agregarProductoAdq
            );


        contenedorDetalles
            ?.addEventListener(
                "click",
                function (
                    event
                ) {

                    const boton =
                        event.target.closest(
                            ".btnEliminarDetalleAdq"
                        );


                    if (!boton) {
                        return;
                    }


                    eliminarProductoAdq(
                        boton
                    );
                }
            );


        inputProducto
            ?.addEventListener(
                "keydown",
                function (
                    event
                ) {

                    if (
                        event.key !==
                        "Enter"
                    ) {
                        return;
                    }


                    event.preventDefault();

                    agregarProductoAdq();
                }
            );


        inputDescripcionProducto
            ?.addEventListener(
                "keydown",
                function (
                    event
                ) {

                    if (
                        event.key !==
                        "Enter"
                    ) {
                        return;
                    }


                    event.preventDefault();

                    agregarProductoAdq();
                }
            );


        // =========================================================
        // ARCHIVOS - VALIDACIÓN
        // =========================================================

        function archivoValidoAdq(
            archivo
        ) {

            const extension =
                obtenerExtensionAdq(
                    archivo.name
                );


            const formatoValido =
                extensionesPermitidasAdq
                    .includes(
                        extension
                    );


            const tamanoValido =
                archivo.size <=
                tamanoMaximoArchivoAdq;


            return {
                valido:
                    formatoValido &&
                    tamanoValido,

                formatoValido:
                    formatoValido,

                tamanoValido:
                    tamanoValido
            };
        }


        // =========================================================
        // ARCHIVOS - IDENTIFICADOR
        // =========================================================

        function obtenerClaveArchivoAdq(
            archivo
        ) {

            return [
                archivo.name,
                archivo.size,
                archivo.lastModified
            ]
                .join(
                    "|"
                );
        }


        // =========================================================
        // ARCHIVOS - SINCRONIZAR INPUT
        // =========================================================

        function sincronizarInputArchivosAdq() {

            if (!inputArchivos) {
                return;
            }


            const transferencia =
                new DataTransfer();


            archivosSeleccionadosAdq
                .forEach(
                    function (
                        archivo
                    ) {

                        transferencia.items.add(
                            archivo
                        );

                    }
                );


            inputArchivos.files =
                transferencia.files;
        }

        // =========================================================
        // ARCHIVOS EXISTENTES - SINCRONIZAR ELIMINADOS
        // =========================================================

        function sincronizarAdjuntosEliminarAdq() {

            const contenedor =
                document.getElementById(
                    "contenedorAdjuntosEliminarAdq"
                );


            if (!contenedor) {
                return;
            }


            contenedor.innerHTML =
                "";


            adjuntosEliminarIdsAdq
                .forEach(
                    function (
                        id
                    ) {

                        const input =
                            document.createElement(
                                "input"
                            );


                        input.type =
                            "hidden";


                        input.name =
                            "AdjuntosEliminarIds";


                        input.value =
                            String(
                                id
                            );


                        contenedor.appendChild(
                            input
                        );
                    }
                );
        }

        // =========================================================
        // ARCHIVOS - RENDERIZAR
        // =========================================================

        function renderizarArchivosSeleccionadosAdq() {

            if (!listaArchivos) {
                return;
            }


            listaArchivos.innerHTML =
                "";


            const existentesActivos =
                archivosExistentesAdq
                    .filter(
                        function (
                            archivo
                        ) {

                            return !adjuntosEliminarIdsAdq
                                .includes(
                                    Number(
                                        archivo.id
                                    )
                                );
                        }
                    );


            const cantidadTotal =
                existentesActivos.length +
                archivosSeleccionadosAdq.length;


            if (contadorArchivos) {

                contadorArchivos.textContent =
                    cantidadTotal === 1
                        ? "1 archivo agregado"
                        : `${cantidadTotal} archivos agregados`;
            }


            if (cantidadTotal === 0) {

                const vacio =
                    document.createElement(
                        "div"
                    );


                vacio.className =
                    "adq-files-empty";


                vacio.innerHTML = `
                    <i class="bi bi-folder2-open"></i>

                    <span>
                        Aún no has agregado archivos.
                    </span>
                `;


                listaArchivos.appendChild(
                    vacio
                );


                return;
            }


            // =====================================================
            // ARCHIVOS EXISTENTES
            // =====================================================

            existentesActivos
                .forEach(
                    function (
                        archivo
                    ) {

                        const item =
                            document.createElement(
                                "div"
                            );


                        item.className =
                            "adq-file-item";


                        item.innerHTML = `
                            <div class="adq-file-item-main">

                                <div class="adq-file-item-icon">

                                    <i class="bi bi-file-earmark-check"></i>

                                </div>


                                <div class="adq-file-item-info">

                                    <strong>
                                        ${escapeHtmlAdq(
                            archivo.nombreOriginal
                        )}
                                    </strong>

                                    <span>
                                        ${formatearTamanoAdq(
                            archivo.tamanoBytes ?? 0
                        )}
                                        · Archivo existente
                                    </span>

                                </div>

                            </div>


                            <div class="d-flex gap-2">

                                <a href="${escapeAttributeAdq(
                            archivo.rutaArchivo
                        )}"
                                   target="_blank"
                                   class="btn btn-sm btn-outline-primary"
                                   title="Abrir archivo">

                                    <i class="bi bi-eye"></i>

                                </a>


                                <button type="button"
                                        class="btn btn-sm btn-outline-danger btnEliminarArchivoExistenteAdq"
                                        data-id="${archivo.id}"
                                        title="Eliminar archivo">

                                    <i class="bi bi-trash"></i>

                                </button>

                            </div>
                        `;


                        listaArchivos.appendChild(
                            item
                        );
                    }
                );


            // =====================================================
            // ARCHIVOS NUEVOS
            // =====================================================

            archivosSeleccionadosAdq
                .forEach(
                    function (
                        archivo,
                        indice
                    ) {

                        const validacion =
                            archivoValidoAdq(
                                archivo
                            );


                        const item =
                            document.createElement(
                                "div"
                            );


                        item.className =
                            "adq-file-item";


                        if (!validacion.valido) {

                            item.classList.add(
                                "adq-file-item-error"
                            );
                        }


                        let estado =
                            "Archivo nuevo";


                        if (!validacion.formatoValido) {

                            estado =
                                "Formato no permitido";
                        }
                        else if (!validacion.tamanoValido) {

                            estado =
                                "Supera 15 MB";
                        }


                        item.innerHTML = `
                            <div class="adq-file-item-main">

                                <div class="adq-file-item-icon">

                                    <i class="bi ${validacion.valido
                                ? "bi-file-earmark-plus"
                                : "bi-file-earmark-x"
                            }"></i>

                                </div>


                                <div class="adq-file-item-info">

                                    <strong>
                                        ${escapeHtmlAdq(
                                archivo.name
                            )}
                                    </strong>

                                    <span>
                                        ${formatearTamanoAdq(
                                archivo.size
                            )}
                                        ·
                                        ${escapeHtmlAdq(
                                estado
                            )}
                                    </span>

                                </div>

                            </div>


                            <button type="button"
                                    class="btn btn-sm btn-outline-danger btnEliminarArchivoAdq"
                                    data-index="${indice}"
                                    title="Eliminar archivo">

                                <i class="bi bi-trash"></i>

                            </button>
                        `;


                        listaArchivos.appendChild(
                            item
                        );
                    }
                );
        }


        // =========================================================
        // ARCHIVOS - AGREGAR
        // =========================================================

        function agregarArchivosAdq(
            archivosNuevos
        ) {

            const clavesExistentes =
                new Set(
                    archivosSeleccionadosAdq
                        .map(
                            obtenerClaveArchivoAdq
                        )
                );


            let archivoInvalido =
                false;


            archivosNuevos
                .forEach(
                    function (
                        archivo
                    ) {

                        const clave =
                            obtenerClaveArchivoAdq(
                                archivo
                            );


                        if (
                            clavesExistentes.has(
                                clave
                            )
                        ) {

                            return;
                        }


                        const validacion =
                            archivoValidoAdq(
                                archivo
                            );


                        if (
                            !validacion.valido
                        ) {

                            archivoInvalido =
                                true;

                        }


                        archivosSeleccionadosAdq
                            .push(
                                archivo
                            );


                        clavesExistentes.add(
                            clave
                        );

                    }
                );


            sincronizarInputArchivosAdq();

            renderizarArchivosSeleccionadosAdq();

            actualizarEstadoEnviar();


            if (archivoInvalido) {

                mostrarAdvertenciaAdq(
                    "Revisa los archivos",
                    "Uno o más archivos tienen un formato no permitido o superan el límite de 15 MB."
                );

            }
        }


        // =========================================================
        // ARCHIVOS - ELIMINAR
        // =========================================================

        function eliminarArchivoAdq(
            indice
        ) {

            archivosSeleccionadosAdq
                .splice(
                    indice,
                    1
                );


            sincronizarInputArchivosAdq();

            renderizarArchivosSeleccionadosAdq();

            actualizarEstadoEnviar();
        }


        // =========================================================
        // EVENTOS DE ARCHIVOS
        // =========================================================

        btnAgregarArchivos
            ?.addEventListener(
                "click",
                function () {

                    inputArchivos
                        ?.click();

                }
            );


        inputArchivos
            ?.addEventListener(
                "change",
                function () {

                    const archivos =
                        Array.from(
                            inputArchivos.files ??
                            []
                        );


                    agregarArchivosAdq(
                        archivos
                    );

                }
            );


        listaArchivos
            ?.addEventListener(
                "click",
                function (
                    event
                ) {

                    const boton =
                        event.target.closest(
                            ".btnEliminarArchivoAdq"
                        );


                    if (!boton) {
                        return;
                    }


                    const indice =
                        Number(
                            boton.dataset.index
                        );


                    if (
                        Number.isNaN(
                            indice
                        )
                    ) {
                        return;
                    }


                    eliminarArchivoAdq(
                        indice
                    );
                }
            );

        // =========================================================
        // ARCHIVOS EXISTENTES - ELIMINAR
        // =========================================================

        listaArchivos
            ?.addEventListener(
                "click",
                function (
                    event
                ) {

                    const boton =
                        event.target.closest(
                            ".btnEliminarArchivoExistenteAdq"
                        );


                    if (!boton) {
                        return;
                    }


                    const id =
                        Number(
                            boton.dataset.id
                        );


                    if (
                        !id ||
                        Number.isNaN(
                            id
                        )
                    ) {
                        return;
                    }


                    if (
                        !adjuntosEliminarIdsAdq
                            .includes(
                                id
                            )
                    ) {

                        adjuntosEliminarIdsAdq
                            .push(
                                id
                            );
                    }


                    sincronizarAdjuntosEliminarAdq();

                    renderizarArchivosSeleccionadosAdq();

                    actualizarEstadoEnviar();
                }
            );


        // =========================================================
        // ARCHIVOS - VALIDAR TODOS
        // =========================================================

        function archivosValidosAdq() {

            return archivosSeleccionadosAdq
                .every(
                    function (
                        archivo
                    ) {

                        return archivoValidoAdq(
                            archivo
                        ).valido;

                    }
                );
        }


        // =========================================================
        // FORMULARIO COMPLETO
        // =========================================================

        function formularioCompleto() {

            if (!formulario) {
                return false;
            }


            const titulo =
                formulario.querySelector(
                    '[name="Input.Titulo"]'
                );

            const area =
                formulario.querySelector(
                    '[name="Input.AreaId"]'
                );

            const descripcion =
                formulario.querySelector(
                    '[name="Input.Descripcion"]'
                );

            const justificacion =
                formulario.querySelector(
                    '[name="Input.Justificacion"]'
                );


            if (
                !titulo?.value.trim() ||
                !area?.value ||
                area.value === "0" ||
                !descripcion?.value.trim() ||
                !justificacion?.value.trim()
            ) {
                return false;
            }


            const productos =
                formulario.querySelectorAll(
                    ".adq-product-item"
                );


            if (
                productos.length ===
                0
            ) {
                return false;
            }


            if (
                !archivosValidosAdq()
            ) {
                return false;
            }


            return true;
        }


        function actualizarEstadoEnviar() {

            if (!btnEnviar) {
                return;
            }


            const editando =
                Boolean(
                    solicitudEditarId?.value
                );


            if (editando) {

                btnEnviar.disabled =
                    false;

                return;
            }


            btnEnviar.disabled =
                !formularioCompleto();
        }


        formulario
            ?.addEventListener(
                "input",
                actualizarEstadoEnviar
            );


        formulario
            ?.addEventListener(
                "change",
                actualizarEstadoEnviar
            );


        // =========================================================
        // RESET DE ARCHIVOS
        // =========================================================

        function limpiarArchivosAdq() {

            archivosSeleccionadosAdq =
                [];


            archivosExistentesAdq =
                [];


            adjuntosEliminarIdsAdq =
                [];


            sincronizarInputArchivosAdq();

            sincronizarAdjuntosEliminarAdq();

            renderizarArchivosSeleccionadosAdq();
        }


        // =========================================================
        // NUEVA SOLICITUD
        // =========================================================

        function prepararNuevaSolicitudAdq() {

            if (!formulario) {
                return;
            }


            formulario.reset();


            if (
                solicitudEditarId
            ) {

                solicitudEditarId.value =
                    "";

            }


            limpiarProductosAdq();

            limpiarArchivosAdq();

            limpiarCapturaProductoAdq();


            if (tituloModal) {

                tituloModal.innerHTML = `
                    <i class="bi bi-cart-plus me-2"></i>
                    Nueva solicitud de compra
                `;

            }


            btnGuardarCambios
                ?.classList.add(
                    "d-none"
                );

            btnEnviarBorrador
                ?.classList.add(
                    "d-none"
                );


            btnGuardarBorrador
                ?.classList.remove(
                    "d-none"
                );


            btnEnviar
                ?.classList.remove(
                    "d-none"
                );


            actualizarEstadoEnviar();
        }


        btnNuevaSolicitud
            ?.addEventListener(
                "click",
                prepararNuevaSolicitudAdq
            );


        // =========================================================
        // CONSULTAR SOLICITUD
        // =========================================================

        async function obtenerSolicitudAdq(
            id
        ) {

            const response =
                await fetch(
                    `?handler=DetalleSolicitud&id=${encodeURIComponent(id)}`,
                    {
                        method: "GET",
                        headers: {
                            "X-Requested-With":
                                "XMLHttpRequest"
                        }
                    }
                );


            if (!response.ok) {

                throw new Error(
                    "No fue posible consultar la solicitud."
                );

            }


            const resultado =
                await response.json();


            if (
                !resultado.success ||
                !resultado.solicitud
            ) {

                throw new Error(
                    resultado.message ??
                    "No fue posible consultar la solicitud."
                );

            }


            return resultado.solicitud;
        }


        // =========================================================
        // VER SOLICITUD
        // =========================================================

        document.addEventListener(
            "click",
            async function (
                event
            ) {

                const boton =
                    event.target.closest(
                        ".btnVerSolicitudAdq"
                    );


                if (!boton) {
                    return;
                }


                try {
                    boton.disabled =
                        true;


                    const solicitud =
                        await obtenerSolicitudAdq(
                            boton.dataset.id
                        );

                    limpiarAdjuntosComentarioAdq();


                    if (
                        inputNuevoComentarioAdq
                    ) {
                        inputNuevoComentarioAdq.value =
                            "";
                    }

                    solicitudDetalleActualAdq =
                        solicitud;


                    /*
                     * Siempre reiniciamos el detalle
                     * en la pestaña Información.
                     */
                    const tabInformacion =
                        document.getElementById(
                            "tabInformacionAdq"
                        );


                    if (
                        tabInformacion
                    ) {

                        bootstrap.Tab
                            .getOrCreateInstance(
                                tabInformacion
                            )
                            .show();
                    }


                    /*
                     * El chat comienza cuando el usuario
                     * ya envió la solicitud.
                     */
                    if (
                        solicitud.estatusId !==
                        1
                    ) {

                        itemTabSeguimientoAdq
                            ?.classList.remove(
                                "d-none"
                            );


                        itemTabHistorialAdq
                            ?.classList.remove(
                                "d-none"
                            );


                        try {

                            await cargarSeguimientoAdq(
                                solicitud.id
                            );

                            if (
                                intervaloSeguimientoAdq
                            ) {

                                clearInterval(
                                    intervaloSeguimientoAdq
                                );
                            }

                            intervaloSeguimientoAdq =
                                setInterval(
                                    async function () {

                                        if (
                                            !solicitudDetalleActualAdq?.id
                                        ) {
                                            return;
                                        }

                                        try {

                                            await cargarSeguimientoAdq(
                                                solicitudDetalleActualAdq.id
                                            );

                                        }
                                        catch (
                                        error
                                        ) {

                                            console.error(
                                                "Error al actualizar seguimiento:",
                                                error
                                            );
                                        }

                                    },
                                    20000
                                );

                        }
                        catch (
                        error
                        ) {

                            console.error(
                                "Error al cargar seguimiento:",
                                error
                            );
                        }

                    }
                    else {

                        itemTabSeguimientoAdq
                            ?.classList.add(
                                "d-none"
                            );


                        itemTabHistorialAdq
                            ?.classList.add(
                                "d-none"
                            );


                        if (
                            inputNuevoComentarioAdq
                        ) {

                            inputNuevoComentarioAdq.value =
                                "";
                        }
                    }

                    document.getElementById(
                        "verSolicitudFolio"
                    ).textContent =
                        solicitud.folio;


                    document.getElementById(
                        "verSolicitudTitulo"
                    ).textContent =
                        solicitud.titulo;

                    document.getElementById(
                        "verSolicitudSolicitante"
                    ).textContent =
                        solicitud.solicitante ??
                        "No disponible";


                    document.getElementById(
                        "verSolicitudArea"
                    ).textContent =
                        solicitud.area;


                    document.getElementById(
                        "verSolicitudEstatus"
                    ).textContent =
                        solicitud.estatus;


                    document.getElementById(
                        "verSolicitudDescripcion"
                    ).textContent =
                        solicitud.descripcion;


                    document.getElementById(
                        "verSolicitudJustificacion"
                    ).textContent =
                        solicitud.justificacion;


                    const fecha =
                        new Date(
                            solicitud.fechaSolicitud
                        );


                    document.getElementById(
                        "verSolicitudFecha"
                    ).textContent =
                        fecha.toLocaleDateString(
                            "es-MX"
                        );


                    const productos =
                        document.getElementById(
                            "verSolicitudProductos"
                        );


                    productos.innerHTML =
                        "";


                    solicitud.detalles
                        .forEach(
                            function (
                                detalle
                            ) {

                                const item =
                                    document.createElement(
                                        "div"
                                    );


                                item.className =
                                    "adq-detail-product-item";


                                item.innerHTML = `
                                    <div class="adq-detail-product-main">

                                        <div class="adq-detail-product-icon">

                                            <i class="bi bi-box-seam"></i>

                                        </div>


                                        <div class="adq-detail-product-info">

                                            <span class="adq-detail-product-title">
                                                ${escapeHtmlAdq(
                                    detalle.productoServicio
                                )}
                                            </span>

                                            <span class="adq-detail-product-description">

                                                ${escapeHtmlAdq(
                                    detalle.descripcion ??
                                    "Sin descripción adicional"
                                )}

                                            </span>

                                        </div>

                                    </div>


                                    <div class="adq-detail-product-meta">

                                        <div>

                                            <span class="adq-detail-meta-label">
                                                Cantidad
                                            </span>

                                            <span class="adq-detail-meta-value">
                                                ${escapeHtmlAdq(
                                    detalle.cantidad
                                )}
                                            </span>

                                        </div>


                                        <div>

                                            <span class="adq-detail-meta-label">
                                                Unidad
                                            </span>

                                            <span class="adq-detail-meta-value">
                                                ${escapeHtmlAdq(
                                    detalle.unidad
                                )}
                                            </span>

                                        </div>

                                    </div>
                                `;


                                productos.appendChild(
                                    item
                                );

                            }
                        );


                    const documentos =
                        document.getElementById(
                            "verSolicitudDocumentos"
                        );


                    documentos.innerHTML =
                        "";


                    if (
                        !solicitud.adjuntos ||
                        solicitud.adjuntos.length ===
                        0
                    ) {

                        documentos.innerHTML = `
                            <div class="text-muted small">
                                No existen documentos adjuntos.
                            </div>
                        `;

                    }
                    else {

                        solicitud.adjuntos
                            .forEach(
                                function (
                                    archivo
                                ) {

                                    const enlace =
                                        document.createElement(
                                            "a"
                                        );


                                    enlace.className =
                                        "adq-file-item text-decoration-none";


                                    enlace.href =
                                        archivo.rutaArchivo;


                                    enlace.target =
                                        "_blank";


                                    enlace.rel =
                                        "noopener noreferrer";


                                    enlace.innerHTML = `
                                        <div class="adq-file-item-main">

                                            <div class="adq-file-item-icon">

                                                <i class="bi bi-file-earmark"></i>

                                            </div>

                                            <div class="adq-file-item-info">

                                                <strong>
                                                    ${escapeHtmlAdq(
                                        archivo.nombreOriginal
                                    )}
                                                </strong>

                                                <span>
                                                    Abrir documento
                                                </span>

                                            </div>

                                        </div>
                                    `;


                                    documentos.appendChild(
                                        enlace
                                    );

                                }
                            );

                    }

                    // =========================================================
                    // ACCIONES DISPONIBLES DESDE EL DETALLE
                    // =========================================================

                    btnEditarDesdeDetalle
                        ?.classList.add(
                            "d-none"
                        );


                    btnCancelarDesdeDetalle
                        ?.classList.add(
                            "d-none"
                        );


                    btnEnviarDesdeDetalle
                        ?.classList.add(
                            "d-none"
                        );


                    /*
                     * Borrador:
                     * editar + cancelar + enviar.
                     */
                    if (
                        solicitud.estatusId ===
                        1
                    ) {
                        btnEditarDesdeDetalle
                            ?.classList.remove(
                                "d-none"
                            );

                        btnCancelarDesdeDetalle
                            ?.classList.remove(
                                "d-none"
                            );

                        btnEnviarDesdeDetalle
                            ?.classList.remove(
                                "d-none"
                            );
                    }


                    /*
                     * Pendiente del gerente:
                     * editar + cancelar.
                     */
                    else if (
                        solicitud.estatusId ===
                        2
                    ) {
                        btnEditarDesdeDetalle
                            ?.classList.remove(
                                "d-none"
                            );

                        btnCancelarDesdeDetalle
                            ?.classList.remove(
                                "d-none"
                            );
                    }


                    /*
                     * Gerente ya aprobó y llegó a Adquisiciones:
                     * ya no puede editar, solamente cancelar.
                     */
                    else if (
                        solicitud.estatusId ===
                        3
                    ) {
                        btnCancelarDesdeDetalle
                            ?.classList.remove(
                                "d-none"
                            );
                    }

                    bootstrap.Modal
                        .getOrCreateInstance(
                            modalVerSolicitudElement
                        )
                        .show();

                }
                catch (
                error
                ) {
                    console.error(
                        error
                    );


                    mostrarAdvertenciaAdq(
                        "No fue posible consultar",
                        error.message
                    );

                }
                finally {
                    boton.disabled =
                        false;
                }
            }
        );

        // =========================================================
        // DETENER ACTUALIZACIÓN DEL CHAT AL CERRAR DETALLE
        // =========================================================

        modalVerSolicitudElement
            ?.addEventListener(
                "hidden.bs.modal",
                function () {

                    if (
                        intervaloSeguimientoAdq
                    ) {

                        clearInterval(
                            intervaloSeguimientoAdq
                        );

                        intervaloSeguimientoAdq =
                            null;

                        limpiarAdjuntosComentarioAdq();


                        if (
                            inputNuevoComentarioAdq
                        ) {
                            inputNuevoComentarioAdq.value =
                                "";
                        }
                    }

                    solicitudDetalleActualAdq =
                        null;

                    if (
                        badgeMensajesPendientesAdq
                    ) {

                        badgeMensajesPendientesAdq
                            .classList
                            .add(
                                "d-none"
                            );

                        badgeMensajesPendientesAdq.textContent =
                            "";
                    }
                }
            );


        // =========================================================
        // EDITAR DESDE DETALLE
        // =========================================================

        btnEditarDesdeDetalle
            ?.addEventListener(
                "click",
                function () {

                    if (
                        !solicitudDetalleActualAdq
                    ) {
                        return;
                    }


                    const id =
                        solicitudDetalleActualAdq.id;


                    bootstrap.Modal
                        .getInstance(
                            modalVerSolicitudElement
                        )
                        ?.hide();


                    const botonEditar =
                        document.querySelector(
                            `.btnEditarSolicitudAdq[data-id="${id}"]`
                        );


                    botonEditar?.click();
                }
            );

        // =========================================================
        // CANCELAR DESDE DETALLE
        // =========================================================

        btnCancelarDesdeDetalle
            ?.addEventListener(
                "click",
                function () {

                    if (
                        !solicitudDetalleActualAdq
                    ) {
                        return;
                    }


                    abrirCancelacionSolicitudUsuarioAdq(
                        solicitudDetalleActualAdq.id,
                        solicitudDetalleActualAdq.folio
                    );


                    bootstrap.Modal
                        .getInstance(
                            modalVerSolicitudElement
                        )
                        ?.hide();
                }
            );

        // =========================================================
        // ABRIR CANCELACIÓN DEL SOLICITANTE
        // =========================================================

        function abrirCancelacionSolicitudUsuarioAdq(
            id,
            folio
        ) {

            const modalElement =
                document.getElementById(
                    "modalCancelarSolicitudUsuarioAdq"
                );


            const inputId =
                document.getElementById(
                    "SolicitudCancelarUsuarioId"
                );


            const folioElemento =
                document.getElementById(
                    "folioCancelarSolicitudUsuarioAdq"
                );


            const motivo =
                document.getElementById(
                    "MotivoCancelacionUsuario"
                );


            if (
                !modalElement ||
                !inputId
            ) {
                return;
            }


            inputId.value =
                id;


            if (folioElemento) {
                folioElemento.textContent =
                    folio ?? "";
            }


            if (motivo) {
                motivo.value =
                    "";
            }


            bootstrap.Modal
                .getOrCreateInstance(
                    modalElement
                )
                .show();
        }

        // =========================================================
        // CANCELAR DESDE MIS SOLICITUDES
        // =========================================================

        document.addEventListener(
            "click",
            function (
                event
            ) {

                const boton =
                    event.target.closest(
                        ".btnCancelarSolicitudUsuarioAdq"
                    );


                if (!boton) {
                    return;
                }


                abrirCancelacionSolicitudUsuarioAdq(
                    boton.dataset.id,
                    boton.dataset.folio
                );
            }
        );

        // =========================================================
        // ENVIAR BORRADOR DESDE DETALLE
        // =========================================================

        btnEnviarDesdeDetalle
            ?.addEventListener(
                "click",
                function () {

                    if (
                        !solicitudDetalleActualAdq ||
                        solicitudDetalleActualAdq.estatusId !== 1
                    ) {
                        return;
                    }


                    const id =
                        solicitudDetalleActualAdq.id;


                    bootstrap.Modal
                        .getInstance(
                            modalVerSolicitudElement
                        )
                        ?.hide();


                    const botonEditar =
                        document.querySelector(
                            `.btnEditarSolicitudAdq[data-id="${id}"]`
                        );


                    botonEditar?.click();
                }
            );

        // =========================================================
        // EDITAR SOLICITUD
        // =========================================================

        document.addEventListener(
            "click",
            async function (
                event
            ) {

                const boton =
                    event.target.closest(
                        ".btnEditarSolicitudAdq"
                    );


                if (!boton) {
                    return;
                }


                try {
                    boton.disabled =
                        true;


                    const solicitud =
                        await obtenerSolicitudAdq(
                            boton.dataset.id
                        );


                    if (
                        solicitud.estatusId !== 1 &&
                        solicitud.estatusId !== 2
                    ) {

                        mostrarAdvertenciaAdq(
                            "Edición no disponible",
                            "La solicitud ya no puede modificarse porque ya fue aprobada por el gerente."
                        );

                        return;
                    }


                    prepararNuevaSolicitudAdq();


                    solicitudEditarId.value =
                        solicitud.id;


                    document.querySelector(
                        '[name="Input.Titulo"]'
                    ).value =
                        solicitud.titulo;


                    document.querySelector(
                        '[name="Input.AreaId"]'
                    ).value =
                        String(
                            solicitud.areaId
                        );


                    document.querySelector(
                        '[name="Input.Descripcion"]'
                    ).value =
                        solicitud.descripcion;


                    document.querySelector(
                        '[name="Input.Justificacion"]'
                    ).value =
                        solicitud.justificacion;


                    limpiarProductosAdq();


                    solicitud.detalles
                        .forEach(
                            function (
                                detalle
                            ) {

                                agregarProductoExistenteAdq(
                                    detalle
                                );

                            }
                        );

                    // =========================================================
                    // CARGAR ARCHIVOS EXISTENTES
                    // =========================================================

                    archivosExistentesAdq =
                        Array.isArray(
                            solicitud.adjuntos
                        )
                            ? solicitud.adjuntos
                                .map(
                                    function (
                                        archivo
                                    ) {

                                        return {
                                            id:
                                                Number(
                                                    archivo.id
                                                ),

                                            nombreOriginal:
                                                archivo.nombreOriginal,

                                            rutaArchivo:
                                                archivo.rutaArchivo,

                                            extension:
                                                archivo.extension,

                                            mimeType:
                                                archivo.mimeType,

                                            tamanoBytes:
                                                Number(
                                                    archivo.tamanoBytes ??
                                                    0
                                                )
                                        };
                                    }
                                )
                            : [];


                    adjuntosEliminarIdsAdq =
                        [];


                    archivosSeleccionadosAdq =
                        [];


                    sincronizarInputArchivosAdq();

                    sincronizarAdjuntosEliminarAdq();

                    renderizarArchivosSeleccionadosAdq();


                    if (tituloModal) {

                        let textoEstado =
                            "";

                        if (solicitud.estatusId === 1) {
                            textoEstado =
                                "Borrador";
                        }
                        else if (solicitud.estatusId === 2) {
                            textoEstado =
                                "Pendiente aprobación gerente";
                        }
                        else if (solicitud.estatusId === 3) {
                            textoEstado =
                                "Solicitud enviada";
                        }


                        tituloModal.innerHTML = `
                            <i class="bi bi-pencil-square me-2"></i>
                            Editar solicitud
                            <small class="d-block mt-1 text-muted">
                                ${escapeHtmlAdq(textoEstado)}
                            </small>
                        `;
                    }


                    btnGuardarCambios
                        ?.classList.remove(
                            "d-none"
                        );

                    if (
                        solicitud.estatusId ===
                        1
                    ) {

                        btnEnviarBorrador
                            ?.classList.remove(
                                "d-none"
                            );
                    }
                    else {

                        btnEnviarBorrador
                            ?.classList.add(
                                "d-none"
                            );
                    }


                    btnGuardarBorrador
                        ?.classList.add(
                            "d-none"
                        );


                    btnEnviar
                        ?.classList.add(
                            "d-none"
                        );


                    actualizarContadorProductosAdq();


                    bootstrap.Modal
                        .getOrCreateInstance(
                            modalNuevaSolicitudElement
                        )
                        .show();

                }
                catch (
                error
                ) {
                    console.error(
                        error
                    );


                    mostrarAdvertenciaAdq(
                        "No fue posible editar",
                        error.message
                    );

                }
                finally {
                    boton.disabled =
                        false;
                }
            }
        );


        // =========================================================
        // FILTROS + PAGINACIÓN - MIS SOLICITUDES
        // =========================================================

        function aplicarFiltros(
            reiniciarPagina = true
        ) {

            const texto =
                filtroBusqueda
                    ?.value
                    .trim()
                    .toLowerCase() ??
                "";


            const estatus =
                filtroEstatus
                    ?.value ??
                "";


            const filas =
                Array.from(
                    document.querySelectorAll(
                        "#tablaSolicitudesAdq .adq-solicitud-row"
                    )
                );


            const filasFiltradas =
                filas.filter(
                    function (
                        fila
                    ) {

                        const folio =
                            fila.dataset.folio ??
                            "";

                        const titulo =
                            fila.dataset.titulo ??
                            "";

                        const area =
                            fila.dataset.area ??
                            "";

                        const filaEstatus =
                            fila.dataset.estatus ??
                            "";


                        const coincideTexto =
                            !texto ||
                            folio.includes(
                                texto
                            ) ||
                            titulo.includes(
                                texto
                            ) ||
                            area.includes(
                                texto
                            );


                        const coincideEstatus =
                            !estatus ||
                            filaEstatus ===
                            estatus;


                        return (
                            coincideTexto &&
                            coincideEstatus
                        );
                    }
                );


            if (
                reiniciarPagina
            ) {
                paginaActualAdq =
                    1;
            }


            renderizarPaginaSolicitudesAdq(
                filas,
                filasFiltradas
            );
        }

        // =========================================================
        // RENDERIZAR PÁGINA DE SOLICITUDES
        // =========================================================

        function renderizarPaginaSolicitudesAdq(
            todasLasFilas,
            filasFiltradas
        ) {

            /*
             * Primero ocultamos absolutamente
             * todas las filas.
             */
            todasLasFilas
                .forEach(
                    function (
                        fila
                    ) {

                        fila.classList.add(
                            "d-none"
                        );
                    }
                );


            const totalRegistros =
                filasFiltradas.length;


            const totalPaginas =
                Math.max(
                    1,
                    Math.ceil(
                        totalRegistros /
                        registrosPorPaginaAdq
                    )
                );


            /*
             * Evitamos quedar en una página
             * que ya no exista después de filtrar.
             */
            if (
                paginaActualAdq >
                totalPaginas
            ) {

                paginaActualAdq =
                    totalPaginas;
            }


            const inicio =
                (
                    paginaActualAdq -
                    1
                ) *
                registrosPorPaginaAdq;


            const fin =
                Math.min(
                    inicio +
                    registrosPorPaginaAdq,
                    totalRegistros
                );


            /*
             * Mostramos únicamente los registros
             * correspondientes a la página.
             */
            filasFiltradas
                .slice(
                    inicio,
                    fin
                )
                .forEach(
                    function (
                        fila
                    ) {

                        fila.classList.remove(
                            "d-none"
                        );
                    }
                );


            actualizarInformacionPaginacionAdq(
                inicio,
                fin,
                totalRegistros
            );


            renderizarControlesPaginacionAdq(
                totalPaginas,
                totalRegistros
            );
        }

        // =========================================================
        // INFORMACIÓN DE PAGINACIÓN
        // =========================================================

        function actualizarInformacionPaginacionAdq(
            inicio,
            fin,
            total
        ) {

            if (
                paginaInicio
            ) {

                paginaInicio.textContent =
                    total === 0
                        ? "0"
                        : String(
                            inicio + 1
                        );
            }


            if (
                paginaFin
            ) {

                paginaFin.textContent =
                    String(
                        fin
                    );
            }


            if (
                paginaTotal
            ) {

                paginaTotal.textContent =
                    String(
                        total
                    );
            }
        }

        // =========================================================
        // CONTROLES DE PAGINACIÓN
        // =========================================================

        function renderizarControlesPaginacionAdq(
            totalPaginas,
            totalRegistros
        ) {

            if (
                !paginacionLista ||
                !paginacionContenedor
            ) {
                return;
            }


            paginacionLista.innerHTML =
                "";


            /*
             * Si no existen registros,
             * ocultamos completamente el paginador.
             */
            if (
                totalRegistros === 0
            ) {

                paginacionContenedor
                    .classList
                    .add(
                        "d-none"
                    );

                return;
            }


            paginacionContenedor
                .classList
                .remove(
                    "d-none"
                );


            // =====================================================
            // ANTERIOR
            // =====================================================

            crearBotonPaginacionAdq(
                "Anterior",
                paginaActualAdq - 1,
                paginaActualAdq === 1,
                false
            );


            // =====================================================
            // NÚMEROS
            // =====================================================

            for (
                let pagina = 1;
                pagina <= totalPaginas;
                pagina++
            ) {

                crearBotonPaginacionAdq(
                    String(
                        pagina
                    ),
                    pagina,
                    false,
                    pagina === paginaActualAdq
                );
            }


            // =====================================================
            // SIGUIENTE
            // =====================================================

            crearBotonPaginacionAdq(
                "Siguiente",
                paginaActualAdq + 1,
                paginaActualAdq ===
                totalPaginas,
                false
            );
        }

        // =========================================================
        // CREAR BOTÓN DE PAGINACIÓN
        // =========================================================

        function crearBotonPaginacionAdq(
            texto,
            pagina,
            deshabilitado,
            activo
        ) {

            if (
                !paginacionLista
            ) {
                return;
            }


            const item =
                document.createElement(
                    "li"
                );


            item.className =
                "page-item";


            if (
                deshabilitado
            ) {

                item.classList.add(
                    "disabled"
                );
            }


            if (
                activo
            ) {

                item.classList.add(
                    "active"
                );
            }


            const boton =
                document.createElement(
                    "button"
                );


            boton.type =
                "button";


            boton.className =
                "page-link";


            boton.textContent =
                texto;


            boton.disabled =
                deshabilitado;


            boton.addEventListener(
                "click",
                function () {

                    if (
                        deshabilitado
                    ) {
                        return;
                    }


                    paginaActualAdq =
                        pagina;


                    aplicarFiltros(
                        false
                    );


                    /*
                     * Regresamos suavemente
                     * al inicio de la tabla.
                     */
                    document
                        .getElementById(
                            "tablaSolicitudesAdq"
                        )
                        ?.scrollIntoView({
                            behavior:
                                "smooth",

                            block:
                                "start"
                        });
                }
            );


            item.appendChild(
                boton
            );


            paginacionLista.appendChild(
                item
            );
        }


        filtroBusqueda
            ?.addEventListener(
                "input",
                function () {

                    aplicarFiltros(
                        true
                    );
                }
            );


        filtroEstatus
            ?.addEventListener(
                "change",
                function () {

                    aplicarFiltros(
                        true
                    );
                }
            );


        btnLimpiarFiltros
            ?.addEventListener(
                "click",
                function () {

                    if (
                        filtroBusqueda
                    ) {

                        filtroBusqueda.value =
                            "";

                    }


                    if (
                        filtroEstatus
                    ) {

                        filtroEstatus.value =
                            "";

                    }

                    paginaActualAdq =
                        1;


                    aplicarFiltros(
                        true
                    );
                }
            );


        // =========================================================
        // EVITAR DOBLE SUBMIT
        // =========================================================

        formulario
            ?.addEventListener(
                "submit",
                function () {

                    sincronizarInputArchivosAdq();

                    renumerarProductosAdq();


                    setTimeout(
                        function () {

                            const botones =
                                formulario.querySelectorAll(
                                    'button[type="submit"]'
                                );


                            botones.forEach(
                                function (
                                    boton
                                ) {

                                    boton.disabled =
                                        true;

                                }
                            );

                        },
                        0
                    );
                }
            );

        // =========================================================
        // ACCIONES DE ADQUISICIONES
        // =========================================================

        document.addEventListener(
            "click",
            function (
                event
            ) {

                const boton =
                    event.target.closest(
                        ".btnAccionAdquisiciones"
                    );


                if (!boton) {
                    return;
                }


                const id =
                    boton.dataset.id;

                const folio =
                    boton.dataset.folio ??
                    "";

                const accion =
                    boton.dataset.accion;


                const inputId =
                    document.getElementById(
                        "SolicitudAdquisicionesId"
                    );


                const titulo =
                    document.getElementById(
                        "tituloAccionAdquisiciones"
                    );


                const folioElemento =
                    document.getElementById(
                        "folioAccionAdquisiciones"
                    );


                const mensaje =
                    document.getElementById(
                        "mensajeAccionAdquisiciones"
                    );


                const comentario =
                    document.getElementById(
                        "ComentarioAdquisiciones"
                    );


                const labelComentario =
                    document.getElementById(
                        "labelComentarioAdquisiciones"
                    );


                const ayudaComentario =
                    document.getElementById(
                        "ayudaComentarioAdquisiciones"
                    );


                const btnAprobar =
                    document.getElementById(
                        "btnAprobarAdquisiciones"
                    );


                const btnCancelar =
                    document.getElementById(
                        "btnCancelarAdquisiciones"
                    );


                inputId.value =
                    id;


                folioElemento.textContent =
                    folio;


                comentario.value =
                    "";


                if (
                    accion ===
                    "aprobar"
                ) {
                    titulo.textContent =
                        "Aprobar solicitud";


                    mensaje.className =
                        "alert alert-success mb-3";


                    mensaje.innerHTML = `
                        <i class="bi bi-check-circle me-1"></i>

                        La solicitud será aprobada por el área
                        de Adquisiciones.
                    `;


                    labelComentario.textContent =
                        "Comentario (opcional)";


                    ayudaComentario.textContent =
                        "Puedes registrar una observación de la revisión.";


                    comentario.required =
                        false;


                    btnAprobar.classList.remove(
                        "d-none"
                    );


                    btnCancelar.classList.add(
                        "d-none"
                    );
                }
                else {
                    titulo.textContent =
                        "Cancelar solicitud";


                    mensaje.className =
                        "alert alert-danger mb-3";


                    mensaje.innerHTML = `
                        <i class="bi bi-exclamation-triangle me-1"></i>

                        La solicitud será cancelada
                        y no continuará con el proceso de compra.
                    `;


                    labelComentario.textContent =
                        "Motivo de cancelación";


                    ayudaComentario.textContent =
                        "Este campo es obligatorio.";


                    comentario.required =
                        true;


                    btnCancelar.classList.remove(
                        "d-none"
                    );


                    btnAprobar.classList.add(
                        "d-none"
                    );
                }


                bootstrap.Modal
                    .getOrCreateInstance(
                        document.getElementById(
                            "modalAccionAdquisiciones"
                        )
                    )
                    .show();

            }
        );


        // =========================================================
        // ASIGNAR AGENTE
        // =========================================================

        document.addEventListener(
            "click",
            function (
                event
            ) {

                const boton =
                    event.target.closest(
                        ".btnAsignarAdquisiciones"
                    );


                if (!boton) {
                    return;
                }


                document.getElementById(
                    "SolicitudAsignarAdqId"
                ).value =
                    boton.dataset.id;


                document.getElementById(
                    "folioAsignarAdq"
                ).textContent =
                    boton.dataset.folio ??
                    "";


                bootstrap.Modal
                    .getOrCreateInstance(
                        document.getElementById(
                            "modalAsignarAdquisiciones"
                        )
                    )
                    .show();

            }
        );

        // =========================================================
        // ENVIAR MENSAJE DEL CHAT
        // =========================================================

        // =========================================================
        // ENVIAR MENSAJE DEL CHAT
        // =========================================================

        btnEnviarComentarioAdq
            ?.addEventListener(
                "click",
                async function () {

                    const solicitudId =
                        Number(
                            inputSolicitudComentarioAdq
                                ?.value
                            ??
                            0
                        );


                    const comentario =
                        inputNuevoComentarioAdq
                            ?.value
                            .trim()
                        ??
                        "";


                    if (
                        !solicitudId
                    ) {
                        return;
                    }


                    /*
                     * Permitimos:
                     *
                     * mensaje
                     * mensaje + archivo
                     * solo archivo
                     */
                    if (
                        !comentario
                        &&
                        archivosComentarioSeleccionadosAdq.length ===
                        0
                    ) {

                        mostrarAdvertenciaAdq(
                            "Mensaje requerido",
                            "Escribe un mensaje o adjunta al menos un archivo."
                        );


                        inputNuevoComentarioAdq
                            ?.focus();


                        return;
                    }


                    const token =
                        document.querySelector(
                            'input[name="__RequestVerificationToken"]'
                        )
                            ?.value;


                    const datos =
                        new FormData();


                    datos.append(
                        "SolicitudComentarioId",
                        String(
                            solicitudId
                        )
                    );


                    datos.append(
                        "NuevoComentarioAdq",
                        comentario
                    );


                    /*
                     * ASP.NET Core enlazará cada elemento
                     * con:
                     *
                     * List<IFormFile> ArchivosComentarioAdq
                     */
                    archivosComentarioSeleccionadosAdq
                        .forEach(
                            function (
                                archivo
                            ) {

                                datos.append(
                                    "ArchivosComentarioAdq",
                                    archivo,
                                    archivo.name
                                );
                            }
                        );


                    try {

                        btnEnviarComentarioAdq.disabled =
                            true;


                        const contenidoOriginal =
                            btnEnviarComentarioAdq.innerHTML;


                        btnEnviarComentarioAdq.innerHTML = `
                    <span class="spinner-border spinner-border-sm me-1"></span>
                    Enviando...
                `;


                        try {

                            const response =
                                await fetch(
                                    "?handler=AgregarComentarioAdq",
                                    {
                                        method:
                                            "POST",

                                        headers: {
                                            "RequestVerificationToken":
                                                token
                                                ??
                                                ""
                                        },

                                        body:
                                            datos
                                    }
                                );


                            const resultado =
                                await response.json();


                            if (
                                !response.ok
                                ||
                                !resultado.success
                            ) {

                                throw new Error(
                                    resultado.message
                                    ??
                                    "No fue posible enviar el mensaje."
                                );
                            }


                            // ==========================================
                            // LIMPIAR MENSAJE
                            // ==========================================

                            if (
                                inputNuevoComentarioAdq
                            ) {

                                inputNuevoComentarioAdq.value =
                                    "";
                            }


                            // ==========================================
                            // LIMPIAR ARCHIVOS
                            // ==========================================

                            limpiarAdjuntosComentarioAdq();


                            // ==========================================
                            // RECARGAR CHAT
                            // ==========================================

                            await cargarSeguimientoAdq(
                                solicitudId
                            );

                        }
                        finally {

                            btnEnviarComentarioAdq.disabled =
                                false;


                            btnEnviarComentarioAdq.innerHTML =
                                contenidoOriginal;
                        }

                    }
                    catch (
                    error
                    ) {

                        mostrarAdvertenciaAdq(
                            "No fue posible enviar",
                            error.message
                            ??
                            "Ocurrió un error al enviar el mensaje."
                        );
                    }
                }
            );

        // =========================================================
        // HISTORIAL DE APROBACIONES - FILTROS
        // =========================================================

        function aplicarFiltrosHistorialAdq(
            reiniciarPagina = true
        ) {

            const texto =
                filtroHistorialAprobacionesAdq
                    ?.value
                    .trim()
                    .toLowerCase()
                ??
                "";

            const decision =
                filtroDecisionHistorialAdq
                    ?.value
                    .trim()
                    .toLowerCase()
                ??
                "";

            const filas =
                Array.from(
                    document.querySelectorAll(
                        "#tablaHistorialAprobacionesAdq .adq-historial-aprobacion-row"
                    )
                );

            const filasFiltradas =
                filas.filter(
                    function (
                        fila
                    ) {

                        const folio =
                            fila.dataset.folio ??
                            "";

                        const titulo =
                            fila.dataset.titulo ??
                            "";

                        const solicitante =
                            fila.dataset.solicitante ??
                            "";

                        const area =
                            fila.dataset.area ??
                            "";

                        const decisionFila =
                            fila.dataset.decision ??
                            "";

                        const coincideTexto =
                            !texto
                            ||
                            folio.includes(
                                texto
                            )
                            ||
                            titulo.includes(
                                texto
                            )
                            ||
                            solicitante.includes(
                                texto
                            )
                            ||
                            area.includes(
                                texto
                            );

                        const coincideDecision =
                            !decision
                            ||
                            decisionFila ===
                            decision;

                        return (
                            coincideTexto
                            &&
                            coincideDecision
                        );
                    }
                );

            if (
                reiniciarPagina
            ) {

                paginaActualHistorialAdq =
                    1;
            }

            renderizarPaginaHistorialAdq(
                filas,
                filasFiltradas
            );
        }


        // =========================================================
        // HISTORIAL DE APROBACIONES - RENDERIZAR PÁGINA
        // =========================================================

        function renderizarPaginaHistorialAdq(
            todasLasFilas,
            filasFiltradas
        ) {

            todasLasFilas.forEach(
                function (
                    fila
                ) {

                    fila.classList.add(
                        "d-none"
                    );
                }
            );

            const totalRegistros =
                filasFiltradas.length;

            const totalPaginas =
                Math.max(
                    1,
                    Math.ceil(
                        totalRegistros /
                        registrosPorPaginaHistorialAdq
                    )
                );

            if (
                paginaActualHistorialAdq >
                totalPaginas
            ) {

                paginaActualHistorialAdq =
                    totalPaginas;
            }

            const inicio =
                (
                    paginaActualHistorialAdq -
                    1
                )
                *
                registrosPorPaginaHistorialAdq;

            const fin =
                Math.min(
                    inicio +
                    registrosPorPaginaHistorialAdq,
                    totalRegistros
                );

            filasFiltradas
                .slice(
                    inicio,
                    fin
                )
                .forEach(
                    function (
                        fila
                    ) {

                        fila.classList.remove(
                            "d-none"
                        );
                    }
                );

            actualizarInformacionHistorialAdq(
                inicio,
                fin,
                totalRegistros
            );

            renderizarPaginacionHistorialAdq(
                totalPaginas,
                totalRegistros
            );
        }


        // =========================================================
        // HISTORIAL DE APROBACIONES - INFORMACIÓN
        // =========================================================

        function actualizarInformacionHistorialAdq(
            inicio,
            fin,
            total
        ) {

            if (
                historialPaginaInicioAdq
            ) {

                historialPaginaInicioAdq.textContent =
                    total === 0
                        ? "0"
                        : String(
                            inicio + 1
                        );
            }

            if (
                historialPaginaFinAdq
            ) {

                historialPaginaFinAdq.textContent =
                    String(
                        fin
                    );
            }

            if (
                historialPaginaTotalAdq
            ) {

                historialPaginaTotalAdq.textContent =
                    String(
                        total
                    );
            }
        }


        // =========================================================
        // HISTORIAL DE APROBACIONES - PAGINACIÓN
        // =========================================================

        function renderizarPaginacionHistorialAdq(
            totalPaginas,
            totalRegistros
        ) {

            if (
                !listaPaginacionHistorialAdq
                ||
                !paginacionHistorialAdq
            ) {
                return;
            }

            listaPaginacionHistorialAdq.innerHTML =
                "";

            if (
                totalRegistros === 0
            ) {

                paginacionHistorialAdq
                    .classList
                    .add(
                        "d-none"
                    );

                return;
            }

            paginacionHistorialAdq
                .classList
                .remove(
                    "d-none"
                );

            crearBotonPaginacionHistorialAdq(
                "Anterior",
                paginaActualHistorialAdq - 1,
                paginaActualHistorialAdq === 1,
                false
            );

            for (
                let pagina = 1;
                pagina <= totalPaginas;
                pagina++
            ) {

                crearBotonPaginacionHistorialAdq(
                    String(
                        pagina
                    ),
                    pagina,
                    false,
                    pagina ===
                    paginaActualHistorialAdq
                );
            }

            crearBotonPaginacionHistorialAdq(
                "Siguiente",
                paginaActualHistorialAdq + 1,
                paginaActualHistorialAdq ===
                totalPaginas,
                false
            );
        }


        function crearBotonPaginacionHistorialAdq(
            texto,
            pagina,
            deshabilitado = false,
            activo = false
        ) {

            if (
                !listaPaginacionHistorialAdq
            ) {
                return;
            }

            const item =
                document.createElement(
                    "li"
                );

            item.className =
                "page-item";

            if (
                activo
            ) {

                item.classList.add(
                    "active"
                );
            }

            if (
                deshabilitado
            ) {

                item.classList.add(
                    "disabled"
                );
            }

            const boton =
                document.createElement(
                    "button"
                );

            boton.type =
                "button";

            boton.className =
                "page-link";

            boton.textContent =
                texto;

            boton.disabled =
                deshabilitado;

            boton.addEventListener(
                "click",
                function () {

                    if (
                        deshabilitado
                    ) {
                        return;
                    }

                    paginaActualHistorialAdq =
                        pagina;

                    aplicarFiltrosHistorialAdq(
                        false
                    );


                    document
                        .getElementById(
                            "tablaHistorialAprobacionesAdq"
                        )
                        ?.scrollIntoView({
                            behavior:
                                "smooth",

                            block:
                                "start"
                        });
                }
            );

            item.appendChild(
                boton
            );

            listaPaginacionHistorialAdq
                .appendChild(
                    item
                );
        }


        // =========================================================
        // HISTORIAL DE APROBACIONES - EVENTOS
        // =========================================================

        filtroHistorialAprobacionesAdq
            ?.addEventListener(
                "input",
                function () {

                    aplicarFiltrosHistorialAdq(
                        true
                    );
                }
            );

        filtroDecisionHistorialAdq
            ?.addEventListener(
                "change",
                function () {

                    aplicarFiltrosHistorialAdq(
                        true
                    );
                }
            );

        btnLimpiarHistorialAprobacionesAdq
            ?.addEventListener(
                "click",
                function () {

                    if (
                        filtroHistorialAprobacionesAdq
                    ) {

                        filtroHistorialAprobacionesAdq.value =
                            "";
                    }

                    if (
                        filtroDecisionHistorialAdq
                    ) {

                        filtroDecisionHistorialAdq.value =
                            "";
                    }

                    aplicarFiltrosHistorialAdq(
                        true
                    );
                }
            );


        // =========================================================
        // INICIALIZACIÓN
        // =========================================================

        renumerarProductosAdq();

        actualizarContadorProductosAdq();

        verificarListaProductosVaciaAdq();

        renderizarArchivosSeleccionadosAdq();

        actualizarEstadoEnviar();

        aplicarFiltros(
            true
        );

        aplicarFiltrosHistorialAdq(
            true
        );

        // =========================================================
        // ABRIR SOLICITUD DESDE NOTIFICACIÓN
        // =========================================================

        const parametrosUrlAdq =
            new URLSearchParams(
                window.location.search
            );


        const solicitudAbrirIdAdq =
            Number(
                parametrosUrlAdq.get(
                    "openId"
                )
                ??
                0
            );


        if (
            solicitudAbrirIdAdq > 0
        ) {

            const botonSolicitudAdq =
                document.querySelector(
                    `.btnVerSolicitudAdq[data-id="${solicitudAbrirIdAdq}"]`
                );


            if (
                botonSolicitudAdq
            ) {

                /*
                 * Reutilizamos exactamente el mismo flujo
                 * que ya utiliza el botón del ojo.
                 */
                setTimeout(
                    function () {

                        botonSolicitudAdq.click();

                    },
                    250
                );
            }
        }

    }
);