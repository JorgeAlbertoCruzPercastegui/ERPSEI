document.addEventListener("DOMContentLoaded", function () {
    document.body.classList.add(
        "module-main-theme",
        "expedientes-bancarios-page"
    );

    inicializarEventosEmpresas();

    if (
        window.jQuery &&
        typeof window.jQuery.fn.bootstrapTable === "function"
    ) {
        inicializarTablaEmpresas();
    } else {
        console.error(
            "Bootstrap Table no está disponible."
        );
    }
});

let modalEmpresa;
let modalEliminarEmpresa;
let modalResultado;
let modalAccionistas;

let empresaIdEliminar = 0;
let empresasIdsEliminar = [];
let modoEliminacion = "individual";
let modoEmpresa = "crear";

let empresaMaestraIdAccionistas = 0;
let empresaIdAccionistas = 0;
let empresaNombreAccionistas = "";

let modalAccionista;
let modalEliminarAccionista;

let modoAccionista = "crear";
let accionistaIdEliminar = 0;

let modalDocumentos;
let modalCargarDocumento;

let empresaIdDocumentos = 0;
let empresaNombreDocumentos = "";
let empresaMaestraIdDocumentos = 0;

let modalEliminarDocumento;
let documentoIdEliminar = 0;
let nombreDocumentoEliminar = "";

let modalHistorialDocumento;
let tipoDocumentoHistorialId = 0;
let nombreTipoDocumentoHistorial = "";

let modalSeleccionarBanco;

/*
 * Permisos Compliance.
 */
/*
 * Permisos Compliance.
 */
let modalPermisosCompliance;
let usuariosPermisosCompliance = [];

/*
 * Empresas permitidas por usuario.
 */
let modalEmpresasPermisoUsuario;
let empresasPermisoUsuario = [];
let usuarioEmpresasPermisoId = "";

/*
 * Evita limpiar el modal principal de permisos cuando
 * solamente lo ocultamos para abrir el selector de empresas.
 */
let abriendoModalEmpresasPermiso = false;

/*
 * Contiene únicamente los identificadores de usuarios
 * cuyos permisos fueron modificados.
 */
const usuariosPermisosModificados =
    new Set();

let documentoIdDescarga = 0;
let nombreDocumentoDescarga = "";
let regresarDocumentosDespuesDescarga = false;
let regresarHistorialDespuesDescarga = false;

const permisosCompliance =
    window.permisosCompliance ?? {
        puedeVisualizar: false,
        puedeCrearCargar: false,
        puedeModificar: false,
        puedeEliminar: false,
        puedeDescargar: false,
        esAdministrador: false
    };

function inicializarEventosEmpresas() {

    modalPermisosCompliance =
        obtenerInstanciaModal(
            "modalPermisosCompliance"
        );

    modalEmpresasPermisoUsuario =
        obtenerInstanciaModal(
            "modalEmpresasPermisoUsuario"
        );

    modalSeleccionarBanco =
        obtenerInstanciaModal(
            "modalSeleccionarBanco"
        );

    modalHistorialDocumento = obtenerInstanciaModal(
        "modalHistorialDocumento"
    );

    modalEliminarDocumento = obtenerInstanciaModal(
        "modalEliminarDocumento"
    );

    modalCargarDocumento = obtenerInstanciaModal(
        "modalCargarDocumento"
    );

    modalEmpresa = obtenerInstanciaModal(
        "modalEmpresa"
    );

    modalEliminarEmpresa = obtenerInstanciaModal(
        "modalEliminarEmpresa"
    );

    modalResultado = obtenerInstanciaModal(
        "modalResultado"
    );

    modalAccionistas = obtenerInstanciaModal(
        "modalAccionistas"
    );

    modalAccionista = obtenerInstanciaModal(
        "modalAccionista"
    );

    modalEliminarAccionista = obtenerInstanciaModal(
        "modalEliminarAccionista"
    );

    modalDocumentos = obtenerInstanciaModal(
        "modalDocumentos"
    );

    document
        .getElementById(
            "btnBorrarTextoPermisos"
        )
        ?.addEventListener(
            "click",
            function () {
                const buscador =
                    document.getElementById(
                        "buscarUsuarioCompliance"
                    );

                if (buscador) {
                    buscador.value = "";
                    buscador.focus();
                }

                filtrarPermisosCompliance();
            }
    );

    /*
 * Abrir administración de permisos.
 */
    document
        .getElementById(
            "btnPermisosCompliance"
        )
        ?.addEventListener(
            "click",
            abrirPermisosCompliance
        );

    /*
     * Guardar permisos.
     */
    document
        .getElementById(
            "btnGuardarPermisosCompliance"
        )
        ?.addEventListener(
            "click",
            guardarPermisosCompliance
        );

    /*
     * Búsqueda inmediata.
     */
    document
        .getElementById(
            "buscarUsuarioCompliance"
        )
        ?.addEventListener(
            "input",
            filtrarPermisosCompliance
        );

    /*
     * Limpiar búsqueda.
     */
    /*
 * Limpiar búsqueda.
 */
    document
        .getElementById(
            "btnLimpiarBusquedaPermisos"
        )
        ?.addEventListener(
            "click",
            function () {
                const buscador =
                    document.getElementById(
                        "buscarUsuarioCompliance"
                    );

                if (buscador) {
                    buscador.value = "";
                    buscador.focus();
                }

                filtrarPermisosCompliance();
            }
        );

    /*
     * =====================================================
     * EMPRESAS PERMITIDAS POR USUARIO
     * =====================================================
     */

    /*
     * Buscar empresa.
     */
    document
        .getElementById(
            "buscarEmpresaPermisoUsuario"
        )
        ?.addEventListener(
            "input",
            filtrarEmpresasPermisoUsuario
        );

    /*
     * Limpiar búsqueda de empresas.
     */
    document
        .getElementById(
            "btnLimpiarBusquedaEmpresaPermiso"
        )
        ?.addEventListener(
            "click",
            function () {
                const buscador =
                    document.getElementById(
                        "buscarEmpresaPermisoUsuario"
                    );

                if (buscador) {
                    buscador.value = "";
                    buscador.focus();
                }

                filtrarEmpresasPermisoUsuario();
            }
        );

    /*
     * Seleccionar / deseleccionar todas las empresas.
     */
    document
        .getElementById(
            "seleccionarTodasEmpresasPermiso"
        )
        ?.addEventListener(
            "change",
            function () {
                seleccionarTodasEmpresasPermiso(
                    this.checked
                );
            }
        );

    /*
     * Limpiar selección de empresas.
     */
    document
        .getElementById(
            "btnLimpiarEmpresasPermiso"
        )
        ?.addEventListener(
            "click",
            function () {
                seleccionarTodasEmpresasPermiso(
                    false
                );
            }
        );

    /*
     * Limpiar el estado al cerrar el modal.
     */
    document
        .getElementById(
            "modalPermisosCompliance"
        )
        ?.addEventListener(
            "hidden.bs.modal",
            function () {

                /*
                 * Si únicamente estamos ocultando el modal
                 * principal para abrir Empresas permitidas,
                 * NO limpiamos su contenido.
                 */
                /*
                 * Limpiar el estado al cerrar el modal.
                 */
                document
                    .getElementById(
                        "modalPermisosCompliance"
                    )
                    ?.addEventListener(
                        "hidden.bs.modal",
                        function () {

                            /*
                             * Si únicamente ocultamos el modal principal
                             * para abrir Empresas permitidas,
                             * NO limpiamos su contenido.
                             *
                             * Restablecemos aquí la bandera porque en este
                             * punto Bootstrap ya terminó realmente de ocultar
                             * el modal.
                             */
                            if (abriendoModalEmpresasPermiso) {

                                abriendoModalEmpresasPermiso =
                                    false;

                                return;
                            }

                            limpiarPermisosCompliance();
                        }
                    );
            }
        );
    document
        .getElementById(
            "btnConfirmarDescargaDocumento"
        )
        ?.addEventListener(
            "click",
            confirmarDescargaDocumento
        );

    document
        .getElementById(
            "bancoDocumentoDescarga"
        )
        ?.addEventListener(
            "change",
            function () {
                limpiarErrorBancoDescarga();
            }
        );

    document
        .getElementById(
            "modalSeleccionarBanco"
        )
        ?.addEventListener(
            "hidden.bs.modal",
            function () {
                limpiarModalSeleccionarBanco();

                if (
                    regresarHistorialDespuesDescarga &&
                    modalHistorialDocumento &&
                    empresaIdDocumentos > 0 &&
                    tipoDocumentoHistorialId > 0
                ) {
                    regresarHistorialDespuesDescarga =
                        false;

                    setTimeout(
                        function () {
                            modalHistorialDocumento.show();
                        },
                        180
                    );

                    return;
                }

                if (
                    regresarDocumentosDespuesDescarga &&
                    modalDocumentos &&
                    empresaIdDocumentos > 0
                ) {
                    regresarDocumentosDespuesDescarga =
                        false;

                    setTimeout(
                        function () {
                            modalDocumentos.show();
                        },
                        180
                    );

                    return;
                }

                regresarHistorialDespuesDescarga = false;
                regresarDocumentosDespuesDescarga = false;
            }
        );

    document
        .getElementById("modalHistorialDocumento")
        ?.addEventListener(
            "hidden.bs.modal",
            function () {
                tipoDocumentoHistorialId = 0;
                nombreTipoDocumentoHistorial = "";

                limpiarHistorialDocumento();

                if (
                    modalDocumentos &&
                    empresaIdDocumentos > 0
                ) {
                    setTimeout(function () {
                        modalDocumentos.show();
                    }, 180);
                }
            }
        );

    document
        .getElementById("modalEliminarDocumento")
        ?.addEventListener(
            "hidden.bs.modal",
            function () {
                documentoIdEliminar = 0;
                nombreDocumentoEliminar = "";
            }
    );

    document
        .getElementById("btnConfirmarEliminarDocumento")
        ?.addEventListener(
            "click",
            eliminarArchivoDocumento
        );

    document
        .getElementById("btnGuardarDocumento")
        ?.addEventListener(
            "click",
            guardarDocumento
        );

    document
        .getElementById("btnBuscarEmpresas")
        ?.addEventListener(
            "click",
            refrescarTablaEmpresas
        );

    document
        .getElementById("filtroEstatus")
        ?.addEventListener(
            "change",
            refrescarTablaEmpresas
        );

    [
        "filtroBusqueda",
        "filtroRfc",
        "filtroNivel"
    ].forEach(function (idFiltro) {
        document
            .getElementById(idFiltro)
            ?.addEventListener(
                "keydown",
                function (event) {
                    if (event.key === "Enter") {
                        event.preventDefault();
                        refrescarTablaEmpresas();
                    }
                }
            );
    });

    document
        .getElementById("filtroRfc")
        ?.addEventListener(
            "input",
            function () {
                this.value = this.value
                    .toUpperCase()
                    .replace(/\s+/g, "");
            }
        );

    document
        .getElementById("btnNuevaEmpresa")
        ?.addEventListener(
            "click",
            abrirModalCrearEmpresa
        );

    document
        .getElementById("btnGuardarEmpresa")
        ?.addEventListener(
            "click",
            guardarEmpresa
        );

    document
        .getElementById(
            "btnConfirmarEliminarEmpresa"
        )
        ?.addEventListener(
            "click",
            eliminarEmpresa
        );

    document
        .getElementById(
            "btnEliminarSeleccionadas"
        )
        ?.addEventListener(
            "click",
            confirmarEliminarSeleccionadas
        );

    document
        .getElementById("ebRfc")
        ?.addEventListener(
            "input",
            function () {
                this.value = this.value
                    .toUpperCase()
                    .replace(/\s+/g, "");
            }
        );

    if (
        window.jQuery &&
        typeof window.jQuery.fn.bootstrapTable ===
        "function"
    ) {
        $("#tablaEmpresas").on(
            "check.bs.table " +
            "uncheck.bs.table " +
            "check-all.bs.table " +
            "uncheck-all.bs.table",
            actualizarBotonSeleccionados
        );
    }

    document
        .getElementById("btnNuevoAccionista")
        ?.addEventListener(
            "click",
            abrirModalCrearAccionista
        );

    document
        .getElementById("btnGuardarAccionista")
        ?.addEventListener(
            "click",
            guardarAccionista
        );

    document
        .getElementById(
            "btnConfirmarEliminarAccionista"
        )
        ?.addEventListener(
            "click",
            eliminarAccionista
        );

    document
        .getElementById("eaRfc")
        ?.addEventListener(
            "input",
            function () {
                this.value = this.value
                    .toUpperCase()
                    .replace(/\s+/g, "");
            }
        );

    document
        .getElementById(
            "btnCargarDocumentoSeleccionado"
        )
        ?.addEventListener(
            "click",
            function () {
                if (!tipoDocumentoSeleccionado) {
                    mostrarResultado(
                        "Selecciona primero un tipo de documento.",
                        "error"
                    );

                    return;
                }

                prepararCargaDocumento(
                    tipoDocumentoSeleccionado.id
                );
            }
        );

    document
        .getElementById("modalEliminarEmpresa")
        ?.addEventListener(
            "hidden.bs.modal",
            function () {
                empresaIdEliminar = 0;
                empresasIdsEliminar = [];
                modoEliminacion = "individual";
            }
        );

    document
        .getElementById(
            "modalEliminarAccionista"
        )
        ?.addEventListener(
            "hidden.bs.modal",
            function () {
                const debeRegresar =
                    accionistaIdEliminar > 0;

                accionistaIdEliminar = 0;

                if (
                    debeRegresar &&
                    modalAccionistas
                ) {
                    modalAccionistas.show();
                }
            }
        );

    document
        .getElementById("modalDocumentos")
        ?.addEventListener(
            "hidden.bs.modal",
            function () {
                /*
                 * No limpiar la empresa cuando el modal se oculta
                 * para abrir el formulario de carga.
                 */
                const modalCargaVisible =
                    document
                        .getElementById("modalCargarDocumento")
                        ?.classList.contains("show");

                if (modalCargaVisible) {
                    return;
                }
            }
        );
}

function obtenerInstanciaModal(id) {
    const elemento = document.getElementById(id);

    if (!elemento) {
        console.warn(`No se encontró el modal #${id}.`);
        return null;
    }

    if (
        typeof bootstrap === "undefined" ||
        !bootstrap.Modal
    ) {
        console.error(
            "Bootstrap Modal no está disponible."
        );

        return null;
    }

    return bootstrap.Modal.getOrCreateInstance(
        elemento
    );
}

// =====================================================
// PERMISOS COMPLIANCE
// =====================================================

async function abrirPermisosCompliance() {

    if (!modalPermisosCompliance) {
        console.error(
            "No se encontró el modal de permisos Compliance."
        );

        mostrarResultado(
            "No fue posible abrir la administración de permisos.",
            "error"
        );

        return;
    }

    limpiarPermisosCompliance();
    mostrarCargaPermisosCompliance();

    modalPermisosCompliance.show();

    await cargarPermisosCompliance();
}

async function cargarPermisosCompliance() {

    try {
        const parametros =
            new URLSearchParams({
                handler:
                    "PermisosCompliance"
            });

        const response =
            await fetch(
                `${window.location.pathname}?${parametros.toString()}`,
                {
                    method: "GET",
                    headers: {
                        "Accept":
                            "application/json"
                    }
                }
            );

        if (!response.ok) {

            let mensaje =
                "No fue posible consultar los permisos.";

            if (response.status === 403) {
                mensaje =
                    "No tienes autorización para administrar los permisos de Compliance.";
            }

            throw new Error(mensaje);
        }

        const resultado =
            await response.json();

        if (!resultado.success) {
            mostrarErrorPermisosCompliance(
                resultado.message ??
                "No fue posible consultar los permisos."
            );

            return;
        }

        usuariosPermisosCompliance =
            Array.isArray(resultado.data)
                ? resultado.data
                : [];

        renderizarPermisosCompliance(
            usuariosPermisosCompliance
        );

    } catch (error) {

        console.error(error);

        mostrarErrorPermisosCompliance(
            error?.message ??
            "Ocurrió un error al consultar los permisos."
        );
    }
}

function renderizarPermisosCompliance(
    usuarios
) {
    const cuerpo =
        document.getElementById(
            "tablaPermisosComplianceBody"
        );

    const carga =
        document.getElementById(
            "permisosComplianceCargando"
        );

    const error =
        document.getElementById(
            "permisosComplianceError"
        );

    const vacio =
        document.getElementById(
            "permisosComplianceVacio"
        );

    const contenedor =
        document.getElementById(
            "contenedorTablaPermisosCompliance"
        );

    const botonGuardar =
        document.getElementById(
            "btnGuardarPermisosCompliance"
        );

    carga?.classList.add(
        "d-none"
    );

    error?.classList.add(
        "d-none"
    );

    /*
     * Al volver a renderizar, todavía no hay
     * modificaciones pendientes.
     */
    usuariosPermisosModificados.clear();

    if (!cuerpo) {
        return;
    }

    if (
        !Array.isArray(usuarios) ||
        usuarios.length === 0
    ) {
        cuerpo.innerHTML = "";

        contenedor?.classList.add(
            "d-none"
        );

        vacio?.classList.remove(
            "d-none"
        );

        if (botonGuardar) {
            botonGuardar.disabled = true;
        }

        actualizarTotalUsuariosPermisos(
            0
        );

        return;
    }

    vacio?.classList.add(
        "d-none"
    );

    contenedor?.classList.remove(
        "d-none"
    );

    /*
     * El botón solo se habilitará cuando cambie
     * al menos un checkbox.
     */
    if (botonGuardar) {
        botonGuardar.disabled = true;
    }

    cuerpo.innerHTML =
        usuarios
            .map(
                function (usuario) {
                    const esAdministrador =
                        Boolean(
                            usuario.esAdministrador
                        );

                    const puedeEditar =
                        Boolean(
                            usuario.puedeEditarPermisos
                        );

                    const claseFila =
                        esAdministrador
                            ? "eb-permission-admin-row"
                            : "";

                    const roles =
                        Array.isArray(usuario.roles)
                            ? usuario.roles
                            : [];

                    const htmlRoles =
                        roles.length > 0
                            ? roles
                                .map(
                                    function (rol) {
                                        const claseRol =
                                            esAdministrador
                                                ? "eb-permission-role-admin"
                                                : "";

                                        return `
                                            <span class="
                                                eb-permission-role
                                                ${claseRol}
                                            ">
                                                ${escaparHtml(
                                            rol
                                        )}
                                            </span>
                                        `;
                                    }
                                )
                                .join("")
                            : `
                                <span class="text-muted small">
                                    Sin rol
                                </span>
                            `;

                    return `
                        <tr class="${claseFila}"
                            data-permiso-usuario-id="${escaparHtml(
                        usuario.id
                    )}"
                            data-permiso-busqueda="${escaparHtml(
                        obtenerTextoBusquedaPermiso(
                            usuario
                        )
                    )}">

                            <td>
                                <div class="eb-permission-user">

                                    <strong>
                                        ${escaparHtml(
                        usuario.nombre
                    )}
                                    </strong>

                                    <span>
                                        ${escaparHtml(
                        usuario.correo
                    )}
                                    </span>

                                </div>
                            </td>

                            <td>
                                <div class="eb-permission-roles">
                                    ${htmlRoles}
                                </div>
                            </td>

                            ${crearCeldaPermisoCompliance(
                        usuario.id,
                        "puedeVisualizar",
                        usuario.puedeVisualizar,
                        puedeEditar,
                        esAdministrador
                    )}

                            ${crearCeldaPermisoCompliance(
                        usuario.id,
                        "puedeCrearCargar",
                        usuario.puedeCrearCargar,
                        puedeEditar,
                        esAdministrador
                    )}

                            ${crearCeldaPermisoCompliance(
                        usuario.id,
                        "puedeModificar",
                        usuario.puedeModificar,
                        puedeEditar,
                        esAdministrador
                    )}

                            ${crearCeldaPermisoCompliance(
                        usuario.id,
                        "puedeEliminar",
                        usuario.puedeEliminar,
                        puedeEditar,
                        esAdministrador
                    )}

                            ${crearCeldaPermisoCompliance(
                        usuario.id,
                        "puedeDescargar",
                        usuario.puedeDescargar,
                        puedeEditar,
                        esAdministrador
                    )}

                    <td class="text-center">

                        <span class="
                            badge
                            bg-light
                            text-dark
                            border
                            px-3
                            py-2
                        "
                        data-numero-empresas-usuario="${escaparHtml(
                            usuario.id
                        )}">

                            ${Number(
                            usuario.numeroEmpresas ?? 0
                        )}

                        </span>

                    </td>

                    <td class="text-center">

                        ${esAdministrador
                                                ? `
                                    <button type="button"
                                            class="btn btn-sm btn-light"
                                            disabled
                                            title="Los administradores tienen acceso a todas las empresas">

                                        <span class="eb-action-dots">
                                            ⋮
                                        </span>

                                    </button>
                                `
                                                : `
                                    <button type="button"
                                            class="btn btn-sm eb-action-button"
                                            title="Administrar empresas permitidas"
                                            onclick="abrirEmpresasPermisoUsuario(
                                                '${escaparAtributoJs(usuario.id)}'
                                            )">

                                        <span class="eb-action-dots">
                                            ⋮
                                        </span>

                                    </button>
                                `
                        }

                    </td>
                        </tr>
                    `;
                }
            )
            .join("");

    actualizarTotalUsuariosPermisos(
        usuarios.length
    );

    actualizarEstadoBotonGuardarPermisos();
}

function crearCeldaPermisoCompliance(
    usuarioId,
    permiso,
    marcado,
    puedeEditar,
    esAdministrador
) {
    const checked =
        marcado
            ? "checked"
            : "";

    const disabled =
        puedeEditar
            ? ""
            : "disabled";

    return `
        <td class="text-center">

            <div class="eb-permission-checkbox">

                <input type="checkbox"
                       class="form-check-input"
                       data-permiso-usuario="${escaparHtml(
        usuarioId
    )}"
                       data-permiso-campo="${permiso}"
                       onchange="marcarUsuarioPermisoModificado(
                           '${escaparAtributoJs(usuarioId)}'
                       )"
                       ${checked}
                       ${disabled} />

            </div>

        </td>
    `;
}

function marcarUsuarioPermisoModificado(
    usuarioId
) {
    const id =
        String(usuarioId ?? "")
            .trim();

    if (id === "") {
        return;
    }

    usuariosPermisosModificados.add(
        id
    );

    const fila =
        document.querySelector(
            `[data-permiso-usuario-id="${escaparSelectorCss(
                id
            )}"]`
        );

    fila?.classList.add(
        "eb-permission-row-modified"
    );

    actualizarEstadoBotonGuardarPermisos();
}

function actualizarEstadoBotonGuardarPermisos() {
    const boton =
        document.getElementById(
            "btnGuardarPermisosCompliance"
        );

    if (!boton) {
        return;
    }

    boton.disabled =
        usuariosPermisosModificados.size === 0;
}

function obtenerTextoBusquedaPermiso(
    usuario
) {
    const roles =
        Array.isArray(usuario.roles)
            ? usuario.roles.join(" ")
            : "";

    return normalizarTextoBusqueda(
        `${usuario.nombre ?? ""} ` +
        `${usuario.correo ?? ""} ` +
        `${roles}`
    );
}

function filtrarPermisosCompliance() {

    const busqueda =
        normalizarTextoBusqueda(
            document
                .getElementById(
                    "buscarUsuarioCompliance"
                )
                ?.value ??
            ""
        );

    const filas =
        document.querySelectorAll(
            "#tablaPermisosComplianceBody tr"
        );

    let totalVisibles = 0;

    filas.forEach(
        function (fila) {

            const texto =
                fila.dataset
                    .permisoBusqueda ??
                "";

            const visible =
                busqueda === "" ||
                texto.includes(
                    busqueda
                );

            fila.classList.toggle(
                "d-none",
                !visible
            );

            if (visible) {
                totalVisibles++;
            }
        }
    );

    actualizarTotalUsuariosPermisos(
        totalVisibles
    );

    const vacio =
        document.getElementById(
            "permisosComplianceVacio"
        );

    const contenedor =
        document.getElementById(
            "contenedorTablaPermisosCompliance"
        );

    if (
        filas.length > 0 &&
        totalVisibles === 0
    ) {
        contenedor?.classList.add(
            "d-none"
        );

        vacio?.classList.remove(
            "d-none"
        );

        const tituloVacio =
            vacio?.querySelector(
                "h3"
            );

        const textoVacio =
            vacio?.querySelector(
                "p"
            );

        if (tituloVacio) {
            tituloVacio.textContent =
                "No se encontraron coincidencias";
        }

        if (textoVacio) {
            textoVacio.textContent =
                "No hay usuarios que coincidan con la búsqueda capturada.";
        }
    } else {
        vacio?.classList.add(
            "d-none"
        );

        if (filas.length > 0) {
            contenedor?.classList.remove(
                "d-none"
            );
        }
    }
}

function normalizarTextoBusqueda(
    texto
) {
    return String(
        texto ??
        ""
    )
        .normalize("NFD")
        .replace(
            /[\u0300-\u036f]/g,
            ""
        )
        .toLowerCase()
        .trim();
}

async function guardarPermisosCompliance() {

    const boton =
        document.getElementById(
            "btnGuardarPermisosCompliance"
        );

    if (!boton) {
        return;
    }

    const permisos =
        obtenerPermisosComplianceFormulario();

    if (permisos.length === 0) {
        mostrarResultado(
            "No se detectaron cambios en los permisos.",
            "error"
        );

        return;
    }

    const htmlOriginal =
        boton.innerHTML;

    boton.disabled = true;

    boton.innerHTML =
        '<i class="fa-solid fa-spinner fa-spin me-1"></i>' +
        " Guardando...";

    try {
        const resultado =
            await enviarJson(
                "GuardarPermisosCompliance",
                {
                    permisos:
                        permisos
                }
            );

        if (!resultado.success) {
            mostrarResultado(
                resultado.message ??
                "No fue posible guardar los permisos.",
                "error"
            );

            return;
        }

        usuariosPermisosModificados.clear();

        modalPermisosCompliance?.hide();

        mostrarResultado(
            resultado.message ??
            "Los permisos se guardaron correctamente.",
            "success"
        );

    } catch (error) {

        console.error(error);

        mostrarResultado(
            "Ocurrió un error al guardar los permisos de Compliance.",
            "error"
        );

    } finally {
        boton.disabled = false;
        boton.innerHTML = htmlOriginal;
    }
}

function obtenerPermisosComplianceFormulario() {
    const usuariosEditables =
        usuariosPermisosCompliance.filter(
            function (usuario) {
                return (
                    usuario.puedeEditarPermisos &&
                    usuariosPermisosModificados.has(
                        String(usuario.id)
                    )
                );
            }
        );

    return usuariosEditables.map(
        function (usuario) {
            return {
                usuarioId:
                    usuario.id,

                puedeVisualizar:
                    obtenerValorCheckboxPermiso(
                        usuario.id,
                        "puedeVisualizar"
                    ),

                puedeCrearCargar:
                    obtenerValorCheckboxPermiso(
                        usuario.id,
                        "puedeCrearCargar"
                    ),

                puedeModificar:
                    obtenerValorCheckboxPermiso(
                        usuario.id,
                        "puedeModificar"
                    ),

                puedeEliminar:
                    obtenerValorCheckboxPermiso(
                        usuario.id,
                        "puedeEliminar"
                    ),

                puedeDescargar:
                    obtenerValorCheckboxPermiso(
                        usuario.id,
                        "puedeDescargar"
                    )
            };
        }
    );
}

function obtenerValorCheckboxPermiso(
    usuarioId,
    campo
) {
    const selector =
        `[data-permiso-usuario="${escaparSelectorCss(
            usuarioId
        )}"]` +
        `[data-permiso-campo="${campo}"]`;

    const checkbox =
        document.querySelector(
            selector
        );

    return Boolean(
        checkbox?.checked
    );
}

function escaparSelectorCss(
    valor
) {
    const texto =
        String(
            valor ??
            ""
        );

    if (
        window.CSS &&
        typeof window.CSS.escape ===
        "function"
    ) {
        return window.CSS.escape(
            texto
        );
    }

    return texto.replace(
        /["\\]/g,
        "\\$&"
    );
}

function mostrarCargaPermisosCompliance() {

    document
        .getElementById(
            "permisosComplianceCargando"
        )
        ?.classList.remove(
            "d-none"
        );

    document
        .getElementById(
            "permisosComplianceError"
        )
        ?.classList.add(
            "d-none"
        );

    document
        .getElementById(
            "permisosComplianceVacio"
        )
        ?.classList.add(
            "d-none"
        );

    document
        .getElementById(
            "contenedorTablaPermisosCompliance"
        )
        ?.classList.add(
            "d-none"
        );

    const boton =
        document.getElementById(
            "btnGuardarPermisosCompliance"
        );

    if (boton) {
        boton.disabled = true;
    }
}

function mostrarErrorPermisosCompliance(
    mensaje
) {
    document
        .getElementById(
            "permisosComplianceCargando"
        )
        ?.classList.add(
            "d-none"
        );

    document
        .getElementById(
            "contenedorTablaPermisosCompliance"
        )
        ?.classList.add(
            "d-none"
        );

    document
        .getElementById(
            "permisosComplianceVacio"
        )
        ?.classList.add(
            "d-none"
        );

    const alerta =
        document.getElementById(
            "permisosComplianceError"
        );

    const texto =
        document.getElementById(
            "permisosComplianceErrorMensaje"
        );

    if (texto) {
        texto.textContent =
            mensaje;
    }

    alerta?.classList.remove(
        "d-none"
    );

    const boton =
        document.getElementById(
            "btnGuardarPermisosCompliance"
        );

    if (boton) {
        boton.disabled = true;
    }
}

function limpiarPermisosCompliance() {

    usuariosPermisosModificados.clear();
    usuariosPermisosCompliance = [];

    document
        .getElementById(
            "mensajeEmpresasPermisosCompliance"
        )
        ?.classList.add(
            "d-none"
        );


    const cuerpo =
        document.getElementById(
            "tablaPermisosComplianceBody"
        );

    if (cuerpo) {
        cuerpo.innerHTML = "";
    }

    const buscador =
        document.getElementById(
            "buscarUsuarioCompliance"
        );

    if (buscador) {
        buscador.value = "";
    }

    document
        .getElementById(
            "permisosComplianceCargando"
        )
        ?.classList.add(
            "d-none"
        );

    document
        .getElementById(
            "permisosComplianceError"
        )
        ?.classList.add(
            "d-none"
        );

    document
        .getElementById(
            "permisosComplianceVacio"
        )
        ?.classList.add(
            "d-none"
        );

    document
        .getElementById(
            "contenedorTablaPermisosCompliance"
        )
        ?.classList.add(
            "d-none"
        );

    actualizarTotalUsuariosPermisos(
        0
    );
}

function actualizarTotalUsuariosPermisos(
    total
) {
    const elemento =
        document.getElementById(
            "totalUsuariosPermisosCompliance"
        );

    if (!elemento) {
        return;
    }

    elemento.textContent =
        total === 1
            ? "1 usuario"
            : `${total} usuarios`;
}

// =====================================================
// EMPRESAS PERMITIDAS POR USUARIO
// =====================================================

async function abrirEmpresasPermisoUsuario(
    usuarioId
) {
    if (!modalEmpresasPermisoUsuario) {

        mostrarResultado(
            "No fue posible abrir la selección de empresas.",
            "error"
        );

        return;
    }

    usuarioEmpresasPermisoId =
        String(
            usuarioId ?? ""
        ).trim();

    if (
        usuarioEmpresasPermisoId === ""
    ) {
        return;
    }

    empresasPermisoUsuario = [];

    const inputUsuario =
        document.getElementById(
            "empresasPermisoUsuarioId"
        );

    if (inputUsuario) {
        inputUsuario.value =
            usuarioEmpresasPermisoId;
    }

    const buscador =
        document.getElementById(
            "buscarEmpresaPermisoUsuario"
        );

    if (buscador) {
        buscador.value = "";
    }

    const seleccionarTodas =
        document.getElementById(
            "seleccionarTodasEmpresasPermiso"
        );

    if (seleccionarTodas) {
        seleccionarTodas.checked = false;
        seleccionarTodas.indeterminate = false;
    }

    establecerTexto(
        "empresasPermisoUsuarioNombre",
        "-"
    );

    establecerTexto(
        "empresasPermisoUsuarioCorreo",
        "-"
    );

    establecerTexto(
        "empresasPermisoContador",
        "0"
    );

    document
        .getElementById(
            "empresasPermisoError"
        )
        ?.classList.add(
            "d-none"
        );

    document
        .getElementById(
            "empresasPermisoVacio"
        )
        ?.classList.add(
            "d-none"
        );

    document
        .getElementById(
            "contenedorEmpresasPermisoUsuario"
        )
        ?.classList.add(
            "d-none"
        );

    document
        .getElementById(
            "empresasPermisoCargando"
        )
        ?.classList.remove(
            "d-none"
        );

    /*
     * Indicamos que el modal principal se oculta
     * únicamente para abrir el selector.
     */
    abriendoModalEmpresasPermiso = true;

    modalPermisosCompliance?.hide();

    setTimeout(
        function () {

            modalEmpresasPermisoUsuario?.show();

        },
        200
    );

    await cargarEmpresasPermisoUsuario(
        usuarioEmpresasPermisoId
    );
}


async function cargarEmpresasPermisoUsuario(
    usuarioId
) {
    try {

        const parametros =
            new URLSearchParams({
                handler:
                    "EmpresasPermisoUsuario",

                usuarioId:
                    usuarioId
            });

        const response =
            await fetch(
                `${window.location.pathname}?${parametros.toString()}`,
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

            let mensaje =
                "No fue posible consultar las empresas.";

            if (
                response.status === 403
            ) {
                mensaje =
                    "No tienes autorización para administrar el alcance de empresas.";
            }

            throw new Error(
                mensaje
            );
        }

        const resultado =
            await response.json();

        if (!resultado.success) {

            throw new Error(
                resultado.message ??
                "No fue posible consultar las empresas."
            );
        }

        empresasPermisoUsuario =
            Array.isArray(
                resultado.empresas
            )
                ? resultado.empresas
                : [];

        establecerTexto(
            "empresasPermisoUsuarioNombre",
            resultado.usuario?.nombre ??
            "-"
        );

        establecerTexto(
            "empresasPermisoUsuarioCorreo",
            resultado.usuario?.correo ??
            "-"
        );

        renderizarEmpresasPermisoUsuario();

    } catch (error) {

        console.error(
            error
        );

        document
            .getElementById(
                "empresasPermisoCargando"
            )
            ?.classList.add(
                "d-none"
            );

        document
            .getElementById(
                "contenedorEmpresasPermisoUsuario"
            )
            ?.classList.add(
                "d-none"
            );

        const alerta =
            document.getElementById(
                "empresasPermisoError"
            );

        const texto =
            document.getElementById(
                "empresasPermisoErrorTexto"
            );

        if (texto) {

            texto.textContent =
                error?.message ??
                "Ocurrió un error al consultar las empresas.";
        }

        alerta?.classList.remove(
            "d-none"
        );
    }
}


function renderizarEmpresasPermisoUsuario() {

    const lista =
        document.getElementById(
            "listaEmpresasPermisoUsuario"
        );

    document
        .getElementById(
            "empresasPermisoCargando"
        )
        ?.classList.add(
            "d-none"
        );

    if (!lista) {
        return;
    }

    if (
        !Array.isArray(
            empresasPermisoUsuario
        ) ||
        empresasPermisoUsuario.length === 0
    ) {

        lista.innerHTML = "";

        document
            .getElementById(
                "contenedorEmpresasPermisoUsuario"
            )
            ?.classList.add(
                "d-none"
            );

        document
            .getElementById(
                "empresasPermisoVacio"
            )
            ?.classList.remove(
                "d-none"
            );

        actualizarContadorEmpresasPermiso();

        return;
    }

    document
        .getElementById(
            "empresasPermisoVacio"
        )
        ?.classList.add(
            "d-none"
        );

    document
        .getElementById(
            "contenedorEmpresasPermisoUsuario"
        )
        ?.classList.remove(
            "d-none"
        );

    lista.innerHTML =
        empresasPermisoUsuario
            .map(
                function (empresa) {

                    const checked =
                        empresa.seleccionada
                            ? "checked"
                            : "";

                    const estatus =
                        empresa.deshabilitado
                            ? `
                                <span class="
                                    badge
                                    bg-secondary
                                    ms-2
                                ">
                                    Inactiva
                                </span>
                              `
                            : "";

                    const textoBusqueda =
                        normalizarTextoBusqueda(
                            `${empresa.razonSocial ?? ""} ` +
                            `${empresa.rfc ?? ""}`
                        );

                    return `
                        <label class="
                            list-group-item
                            list-group-item-action
                            d-flex
                            align-items-center
                            gap-3
                            py-3
                        "
                        data-empresa-permiso-busqueda="${escaparHtml(
                        textoBusqueda
                    )}">

                            <input type="checkbox"
                                   class="
                                       form-check-input
                                       mt-0
                                       empresa-permiso-checkbox
                                   "
                                   value="${Number(
                        empresa.id
                    )}"
                                   ${checked}
                                   onchange="
                                       actualizarContadorEmpresasPermiso()
                                   " />

                            <div class="flex-grow-1">

                                <div class="fw-semibold">

                                    ${escaparHtml(
                        empresa.razonSocial ??
                        ""
                    )}

                                    ${estatus}

                                </div>

                                <div class="
                                    small
                                    text-muted
                                    mt-1
                                ">

                                    RFC:
                                    ${escaparHtml(
                        empresa.rfc ||
                        "Sin RFC"
                    )}

                                </div>

                            </div>

                        </label>
                    `;
                }
            )
            .join("");

    actualizarContadorEmpresasPermiso();
}


function filtrarEmpresasPermisoUsuario() {

    const filtro =
        normalizarTextoBusqueda(
            document
                .getElementById(
                    "buscarEmpresaPermisoUsuario"
                )
                ?.value ??
            ""
        );

    const empresas =
        document.querySelectorAll(
            "[data-empresa-permiso-busqueda]"
        );

    let visibles = 0;

    empresas.forEach(
        function (empresa) {

            const texto =
                empresa.dataset
                    .empresaPermisoBusqueda ??
                "";

            const visible =
                filtro === "" ||
                texto.includes(
                    filtro
                );

            empresa.classList.toggle(
                "d-none",
                !visible
            );

            if (visible) {
                visibles++;
            }
        }
    );

    const vacio =
        document.getElementById(
            "empresasPermisoVacio"
        );

    if (
        empresas.length > 0 &&
        visibles === 0
    ) {

        vacio?.classList.remove(
            "d-none"
        );

    } else {

        vacio?.classList.add(
            "d-none"
        );
    }
}


function seleccionarTodasEmpresasPermiso(
    seleccionar
) {

    const checkboxes =
        document.querySelectorAll(
            ".empresa-permiso-checkbox"
        );

    checkboxes.forEach(
        function (checkbox) {

            checkbox.checked =
                Boolean(
                    seleccionar
                );
        }
    );

    actualizarContadorEmpresasPermiso();
}


function actualizarContadorEmpresasPermiso() {

    const checkboxes =
        Array.from(
            document.querySelectorAll(
                ".empresa-permiso-checkbox"
            )
        );

    const seleccionadas =
        checkboxes.filter(
            function (checkbox) {

                return checkbox.checked;
            }
        ).length;

    establecerTexto(
        "empresasPermisoContador",
        `${seleccionadas} de ${checkboxes.length}`
    );

    const seleccionarTodas =
        document.getElementById(
            "seleccionarTodasEmpresasPermiso"
        );

    if (!seleccionarTodas) {
        return;
    }

    seleccionarTodas.checked =
        checkboxes.length > 0 &&
        seleccionadas ===
        checkboxes.length;

    seleccionarTodas.indeterminate =
        seleccionadas > 0 &&
        seleccionadas <
        checkboxes.length;
}


async function guardarEmpresasPermisoUsuario() {

    const boton =
        document.getElementById(
            "btnGuardarEmpresasPermisoUsuario"
        );

    if (!boton) {
        return;
    }

    if (
        usuarioEmpresasPermisoId === ""
    ) {

        mostrarResultado(
            "No se identificó el usuario seleccionado.",
            "error"
        );

        return;
    }

    const empresaIds =
        Array.from(
            document.querySelectorAll(
                ".empresa-permiso-checkbox:checked"
            )
        )
            .map(
                function (checkbox) {

                    return parseInt(
                        checkbox.value,
                        10
                    );
                }
            )
            .filter(
                function (empresaId) {

                    return (
                        Number.isInteger(
                            empresaId
                        ) &&
                        empresaId > 0
                    );
                }
            );

    const htmlOriginal =
        boton.innerHTML;

    boton.disabled = true;

    boton.innerHTML =
        '<i class="fa-solid fa-spinner fa-spin me-1"></i>' +
        " Guardando...";

    try {

        const resultado =
            await enviarJson(
                "GuardarEmpresasPermisoUsuario",
                {
                    usuarioId:
                        usuarioEmpresasPermisoId,

                    empresaIds:
                        empresaIds
                }
            );

        if (!resultado.success) {

            mostrarResultado(
                resultado.message ??
                "No fue posible guardar las empresas.",
                "error"
            );

            return;
        }

        /*
         * Cerrar selector.
         */
        modalEmpresasPermisoUsuario?.hide();

        /*
         * Recargar permisos para actualizar inmediatamente
         * la columna No. Empresas.
         */
        await cargarPermisosCompliance();

        setTimeout(
            function () {

                modalPermisosCompliance?.show();

                mostrarMensajeEmpresasPermisos(
                    resultado.message ??
                    "Las empresas se guardaron correctamente."
                );

            },
            200
        );

        usuarioEmpresasPermisoId =
            "";

        empresasPermisoUsuario =
            [];

    } catch (error) {

        console.error(
            error
        );

        mostrarResultado(
            "Ocurrió un error al guardar las empresas permitidas.",
            "error"
        );

    } finally {

        boton.disabled =
            false;

        boton.innerHTML =
            htmlOriginal;
    }
}


function cancelarEmpresasPermisoUsuario() {

    modalEmpresasPermisoUsuario?.hide();

    usuarioEmpresasPermisoId =
        "";

    empresasPermisoUsuario =
        [];

    setTimeout(
        function () {

            modalPermisosCompliance?.show();

        },
        200
    );
}


function mostrarMensajeEmpresasPermisos(
    mensaje
) {

    const alerta =
        document.getElementById(
            "mensajeEmpresasPermisosCompliance"
        );

    const texto =
        document.getElementById(
            "mensajeEmpresasPermisosComplianceTexto"
        );

    if (texto) {

        texto.textContent =
            mensaje;
    }

    alerta?.classList.remove(
        "d-none"
    );

    window.setTimeout(
        function () {

            alerta?.classList.add(
                "d-none"
            );

        },
        5000
    );
}
function inicializarTablaEmpresas() {
    const tabla = $("#tablaEmpresas");

    tabla.bootstrapTable({
        url: construirUrlEmpresas(),
        method: "get",
        toolbar: "#toolbarEmpresas",

        detailView: true,
        detailFormatter: detalleEmpresaFormatter,

        search: true,
        pagination: true,
        sidePagination: "client",

        pageSize: 10,
        pageList: [10, 20, 30, 50],

        sortName: "id",
        sortOrder: "asc",

        paginationLoop: false,
        showRefresh: true,
        showColumns: true,

        uniqueId: "id",
        locale: "es-MX",
        height: undefined,

        onPostBody: function () {
            actualizarBotonSeleccionados();
        }
    });
}

function construirUrlEmpresas() {
    const busqueda = document
        .getElementById("filtroBusqueda")
        ?.value?.trim() ?? "";

    const rfc = document
        .getElementById("filtroRfc")
        ?.value?.trim() ?? "";

    const nivel = document
        .getElementById("filtroNivel")
        ?.value?.trim() ?? "";

    const estatus = document
        .getElementById("filtroEstatus")
        ?.value ?? "Activas";

    const params = new URLSearchParams({
        handler: "Empresas",
        busqueda: busqueda,
        rfc: rfc,
        nivel: nivel,
        estatus: estatus
    });

    return `${window.location.pathname}?${params.toString()}`;
}

function refrescarTablaEmpresas() {
    $("#tablaEmpresas").bootstrapTable("refresh", {
        url: construirUrlEmpresas()
    });
}

function actualizarBotonSeleccionados() {
    const seleccionadas = $("#tablaEmpresas")
        .bootstrapTable("getSelections");

    const boton = document.getElementById(
        "btnEliminarSeleccionadas"
    );

    if (!boton) {
        return;
    }

    const haySeleccionadas = seleccionadas.length > 0;

    boton.disabled = !haySeleccionadas;

    boton.classList.toggle(
        "btn-danger",
        haySeleccionadas
    );

    boton.classList.toggle(
        "btn-outline-secondary",
        !haySeleccionadas
    );
}

function detalleEmpresaFormatter(index, row) {
    return `
        <div class="eb-detail-panel">
            <div class="eb-detail-grid">

                <div class="eb-detail-item">
                    <strong>Fecha de constitución:</strong>
                    <span>${formatearFecha(row.fechaConstitucion)}</span>
                </div>

                <div class="eb-detail-item">
                    <strong>Teléfono:</strong>
                    <span>${valorTexto(row.telefonoBancos)}</span>
                </div>

                <div class="eb-detail-item">
                    <strong>Correo:</strong>
                    <span>${valorTexto(row.correoBancos)}</span>
                </div>

                <div class="eb-detail-item">
                    <strong>Escritura:</strong>
                    <span>${valorTexto(row.numeroEscritura)}</span>
                </div>

                <div class="eb-detail-item eb-detail-item-wide">
                    <strong>Domicilio fiscal:</strong>
                    <span>${valorTexto(row.domicilioFiscal)}</span>
                </div>

                <div class="eb-detail-item eb-detail-item-wide">
                    <strong>Actividad comercial:</strong>
                    <span>${valorTexto(row.actividadComercial)}</span>
                </div>

                <div class="eb-detail-item eb-detail-item-wide">
                    <strong>Observaciones:</strong>
                    <span>${valorTexto(row.observaciones)}</span>
                </div>

            </div>
        </div>
    `;
}

function estatusEmpresaFormatter(value, row) {
    if (row.deshabilitado) {
        return `
            <span class="eb-status eb-status-inactive">
                <i class="fa-solid fa-circle"></i>
                Inactiva
            </span>
        `;
    }

    return `
        <span class="eb-status eb-status-active">
            <i class="fa-solid fa-circle"></i>
            Activa
        </span>
    `;
}

function accionesEmpresaFormatter(value, row) {

    const accionEstatus =
        row.deshabilitado
            ? "Habilitar"
            : "Deshabilitar";

    const iconoEstatus =
        row.deshabilitado
            ? "fa-circle-check"
            : "fa-ban";

    const opcionConsultar =
        permisosCompliance.puedeVisualizar
            ? `
                <li>
                    <button type="button"
                            class="dropdown-item"
                            onclick="consultarEmpresa(${row.id})">

                        <i class="fa-regular fa-eye me-2"></i>
                        Consultar empresa

                    </button>
                </li>
            `
            : "";

    const opcionEditar =
        permisosCompliance.puedeModificar
            ? `
                <li>
                    <button type="button"
                            class="dropdown-item"
                            onclick="editarEmpresa(${row.id})">

                        <i class="fa-regular fa-pen-to-square me-2"></i>
                        Editar empresa

                    </button>
                </li>
            `
            : "";

    const opcionAccionistas =
        permisosCompliance.puedeVisualizar
            ? `
            <li>
                <button type="button"
                        class="dropdown-item"
                        onclick="abrirAccionistasEmpresa(
                            ${row.id},
                            '${escaparAtributoJs(row.razonSocial)}'
                        )">

                    <i class="fa-solid fa-users me-2"></i>
                    Accionistas

                </button>
            </li>
        `
            : "";

    const opcionDocumentos =
        permisosCompliance.puedeVisualizar
            ? `
                <li>
                    <button type="button"
                            class="dropdown-item"
                            onclick="abrirDocumentosEmpresa(
                            ${row.complianceId ?? 0},
                            ${row.id},
                            '${escaparAtributoJs(row.razonSocial)}'
                        )">

                        <i class="fa-regular fa-folder-open me-2"></i>
                        Documentos

                    </button>
                </li>
            `
            : "";

    const opcionCambiarEstatus =
        permisosCompliance.puedeModificar
            ? `
                <li>
                    <button type="button"
                            class="dropdown-item"
                            onclick="cambiarEstatusEmpresa(${row.id})">

                        <i class="fa-solid ${iconoEstatus} me-2"></i>
                        ${accionEstatus}

                    </button>
                </li>
            `
            : "";

    const opcionEliminar =
        permisosCompliance.puedeEliminar
            ? `
                <li>
                    <button type="button"
                            class="dropdown-item text-danger"
                            onclick="confirmarEliminarEmpresa(
                                ${row.id},
                                '${escaparAtributoJs(row.razonSocial)}'
                            )">

                        <i class="fa-regular fa-trash-can me-2"></i>
                        Eliminar

                    </button>
                </li>
            `
            : "";

    const mostrarSeparador =
        permisosCompliance.puedeModificar ||
        permisosCompliance.puedeEliminar;

    return `
        <div class="dropdown eb-actions-dropdown">

            <button type="button"
                    class="btn btn-sm eb-action-button"
                    data-bs-toggle="dropdown"
                    data-bs-boundary="viewport"
                    data-bs-offset="8,0"
                    aria-expanded="false"
                    title="Acciones">

                <span class="eb-action-dots">⋮</span>

            </button>

            <ul class="dropdown-menu eb-actions-menu">

                ${opcionConsultar}
                ${opcionEditar}
                ${opcionAccionistas}
                ${opcionDocumentos}

                ${mostrarSeparador
            ? `<li><hr class="dropdown-divider" /></li>`
            : ""
        }

                ${opcionCambiarEstatus}
                ${opcionEliminar}

            </ul>

        </div>
    `;
}

function abrirModalCrearEmpresa() {
    if (!modalEmpresa) {
        console.error(
            "No se encontró el modal de empresa."
        );

        return;
    }

    modoEmpresa = "crear";

    limpiarFormularioEmpresa();
    habilitarFormularioEmpresa(true);

    const titulo = document.getElementById(
        "modalEmpresaTitulo"
    );

    if (titulo) {
        titulo.textContent = "Nueva empresa";
    }

    const botonGuardar = document.getElementById(
        "btnGuardarEmpresa"
    );

    if (botonGuardar) {
        botonGuardar.classList.remove("d-none");
        botonGuardar.disabled = false;
    }

    modalEmpresa.show();

    setTimeout(function () {
        document
            .getElementById("ebRazonSocial")
            ?.focus();
    }, 250);
}

async function consultarEmpresa(id) {
    if (!modalEmpresa) {
        console.error(
            "No se encontró el modal de empresa."
        );

        return;
    }

    modoEmpresa = "consultar";

    const respuesta = await obtenerEmpresa(id);

    if (!respuesta?.success) {
        mostrarResultado(
            respuesta?.message ??
            "No fue posible consultar la empresa.",
            "error"
        );

        return;
    }

    cargarFormularioEmpresa(
        respuesta.data
    );

    habilitarFormularioEmpresa(false);

    const titulo = document.getElementById(
        "modalEmpresaTitulo"
    );

    if (titulo) {
        titulo.textContent =
            "Consultar empresa";
    }

    document
        .getElementById("btnGuardarEmpresa")
        ?.classList.add("d-none");

    modalEmpresa.show();
}

async function editarEmpresa(id) {
    if (!modalEmpresa) {
        console.error(
            "No se encontró el modal de empresa."
        );

        return;
    }

    modoEmpresa = "editar";

    const respuesta = await obtenerEmpresa(id);

    if (!respuesta?.success) {
        mostrarResultado(
            respuesta?.message ??
            "No fue posible cargar la empresa.",
            "error"
        );

        return;
    }

    cargarFormularioEmpresa(
        respuesta.data
    );

    habilitarFormularioEmpresa(true);

    const titulo = document.getElementById(
        "modalEmpresaTitulo"
    );

    if (titulo) {
        titulo.textContent =
            "Editar empresa";
    }

    document
        .getElementById("btnGuardarEmpresa")
        ?.classList.remove("d-none");

    modalEmpresa.show();
}

async function obtenerEmpresa(id) {
    try {
        const params = new URLSearchParams({
            handler: "Empresa",
            id: id
        });

        const response = await fetch(
            `${window.location.pathname}?${params.toString()}`
        );

        return await response.json();
    } catch {
        return null;
    }
}

function cargarFormularioEmpresa(data) {
    document.getElementById("ebId").value = data.id ?? 0;
    document.getElementById("ebRazonSocial").value =
        data.razonSocial ?? "";
    document.getElementById("ebNombreCorto").value =
        data.nombreCorto ?? "";
    document.getElementById("ebRfc").value =
        data.rfc ?? "";
    document.getElementById("ebNivel").value =
        data.nivel ?? "";
    document.getElementById("ebActividadComercial").value =
        data.actividadComercial ?? "";
    document.getElementById("ebTelefonoBancos").value =
        data.telefonoBancos ?? "";
    document.getElementById("ebCorreoBancos").value =
        data.correoBancos ?? "";
    document.getElementById("ebFechaConstitucion").value =
        data.fechaConstitucion ?? "";
    document.getElementById("ebNumeroEscritura").value =
        data.numeroEscritura ?? "";
    document.getElementById("ebDomicilioFiscal").value =
        data.domicilioFiscal ?? "";
    document.getElementById("ebObservaciones").value =
        data.observaciones ?? "";

    limpiarErroresEmpresa();
}

function limpiarFormularioEmpresa() {
    const formulario = document.getElementById("formEmpresa");

    if (formulario) {
        formulario.reset();
    }

    const valoresIniciales = {
        ebId: "0",
        ebRazonSocial: "",
        ebNombreCorto: "",
        ebRfc: "",
        ebNivel: "",
        ebActividadComercial: "",
        ebTelefonoBancos: "",
        ebCorreoBancos: "",
        ebFechaConstitucion: "",
        ebNumeroEscritura: "",
        ebDomicilioFiscal: "",
        ebObservaciones: ""
    };

    Object.entries(valoresIniciales).forEach(
        function ([id, valor]) {
            const elemento = document.getElementById(id);

            if (elemento) {
                elemento.value = valor;
            }
        }
    );

    limpiarErroresEmpresa();

    document
        .querySelectorAll("#formEmpresa .is-invalid")
        .forEach(function (elemento) {
            elemento.classList.remove("is-invalid");
        });
}

function habilitarFormularioEmpresa(habilitado) {
    const campos = document.querySelectorAll(
        "#formEmpresa input:not([type='hidden']), " +
        "#formEmpresa textarea, " +
        "#formEmpresa select"
    );

    campos.forEach(function (campo) {
        campo.disabled = !habilitado;
    });
}

async function guardarEmpresa() {
    limpiarErroresEmpresa();

    const request = obtenerRequestEmpresa();

    const handler = modoEmpresa === "editar"
        ? "Editar"
        : "Crear";

    const boton = document.getElementById("btnGuardarEmpresa");

    boton.disabled = true;
    const htmlOriginal = boton.innerHTML;

    boton.innerHTML =
        '<i class="fa-solid fa-spinner fa-spin me-1"></i>' +
        " Guardando...";

    try {
        const respuesta = await enviarJson(handler, request);

        if (!respuesta.success) {
            mostrarErroresEmpresa(respuesta.errors);

            if (!respuesta.errors) {
                mostrarResultado(
                    respuesta.message ??
                    "No fue posible completar la operación.",
                    "error"
                );
            }

            return;
        }

        modalEmpresa.hide();
        refrescarTablaEmpresas();

        mostrarResultado(
            respuesta.message,
            "success"
        );
    } catch {
        mostrarResultado(
            "Ocurrió un error al guardar la empresa.",
            "error"
        );
    } finally {
        boton.disabled = false;
        boton.innerHTML = htmlOriginal;
    }
}

function obtenerRequestEmpresa() {
    return {
        id: parseInt(
            document.getElementById("ebId").value || "0",
            10
        ),
        razonSocial:
            document.getElementById("ebRazonSocial").value,
        nombreCorto:
            document.getElementById("ebNombreCorto").value,
        rfc:
            document.getElementById("ebRfc").value,
        nivel:
            document.getElementById("ebNivel").value,
        actividadComercial:
            document.getElementById("ebActividadComercial").value,
        telefonoBancos:
            document.getElementById("ebTelefonoBancos").value,
        correoBancos:
            document.getElementById("ebCorreoBancos").value,
        fechaConstitucion:
            document.getElementById("ebFechaConstitucion").value || null,
        numeroEscritura:
            document.getElementById("ebNumeroEscritura").value,
        domicilioFiscal:
            document.getElementById("ebDomicilioFiscal").value,
        observaciones:
            document.getElementById("ebObservaciones").value
    };
}

async function cambiarEstatusEmpresa(id) {
    try {
        const respuesta = await enviarJson(
            "CambiarEstatus",
            { id: id }
        );

        if (!respuesta.success) {
            mostrarResultado(
                respuesta.message,
                "error"
            );
            return;
        }

        refrescarTablaEmpresas();

        mostrarResultado(
            respuesta.message,
            "success"
        );
    } catch {
        mostrarResultado(
            "No fue posible cambiar el estatus.",
            "error"
        );
    }
}

function confirmarEliminarEmpresa(id, razonSocial) {
    modoEliminacion = "individual";
    empresaIdEliminar = id;
    empresasIdsEliminar = [];

    const nombreEmpresa = document.getElementById(
        "nombreEmpresaEliminar"
    );

    const mensaje = document.getElementById(
        "mensajeConfirmacionEliminar"
    );

    if (nombreEmpresa) {
        nombreEmpresa.textContent = razonSocial;
    }

    if (mensaje) {
        mensaje.innerHTML = `
            ¿Estás seguro de que deseas eliminar la empresa
            <strong>${escaparHtml(razonSocial)}</strong>?
            Esta acción dejará de mostrar el registro en el módulo.
        `;
    }

    modalEliminarEmpresa.show();
}

async function eliminarEmpresa() {
    if (
        modoEliminacion === "individual" &&
        empresaIdEliminar <= 0
    ) {
        return;
    }

    if (
        modoEliminacion === "multiple" &&
        empresasIdsEliminar.length === 0
    ) {
        return;
    }

    const boton = document.getElementById(
        "btnConfirmarEliminarEmpresa"
    );

    boton.disabled = true;

    const htmlOriginal = boton.innerHTML;

    boton.innerHTML =
        '<i class="fa-solid fa-spinner fa-spin me-1"></i>' +
        " Eliminando...";

    try {
        if (modoEliminacion === "individual") {
            const respuesta = await enviarJson(
                "Eliminar",
                { id: empresaIdEliminar }
            );

            if (!respuesta.success) {
                modalEliminarEmpresa.hide();

                mostrarResultado(
                    respuesta.message ??
                    "No fue posible eliminar la empresa.",
                    "error"
                );

                return;
            }

            modalEliminarEmpresa.hide();

            empresaIdEliminar = 0;

            refrescarTablaEmpresas();

            mostrarResultado(
                respuesta.message,
                "success"
            );

            return;
        }

        const resultados = [];

        for (const id of empresasIdsEliminar) {
            const respuesta = await enviarJson(
                "Eliminar",
                { id: id }
            );

            resultados.push(respuesta);
        }

        const exitosos = resultados.filter(
            function (resultado) {
                return resultado.success;
            }
        );

        const fallidos = resultados.filter(
            function (resultado) {
                return !resultado.success;
            }
        );

        modalEliminarEmpresa.hide();

        empresasIdsEliminar = [];

        refrescarTablaEmpresas();

        actualizarBotonSeleccionados();

        if (fallidos.length === 0) {
            mostrarResultado(
                exitosos.length === 1
                    ? "La empresa seleccionada se eliminó correctamente."
                    : "Las empresas seleccionadas se eliminaron correctamente.",
                "success"
            );
        } else {
            mostrarResultado(
                `${exitosos.length} registros fueron eliminados y ` +
                `${fallidos.length} no pudieron eliminarse porque tienen información relacionada.`,
                "error"
            );
        }
    } catch {
        modalEliminarEmpresa.hide();

        mostrarResultado(
            "No fue posible completar la eliminación.",
            "error"
        );
    } finally {
        boton.disabled = false;
        boton.innerHTML = htmlOriginal;
    }
}

async function enviarJson(handler, data) {
    const token = document.querySelector(
        '#formToken input[name="__RequestVerificationToken"]'
    )?.value;

    const response = await fetch(
        `${window.location.pathname}?handler=${handler}`,
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "RequestVerificationToken": token ?? ""
            },
            body: JSON.stringify(data)
        }
    );

    return await response.json();
}

function mostrarErroresEmpresa(errors) {
    if (!errors) {
        return;
    }

    Object.entries(errors).forEach(function ([campo, mensajes]) {
        const elemento = document.querySelector(
            `[data-error-for="${campo}"]`
        );

        if (!elemento) {
            return;
        }

        elemento.textContent = mensajes?.[0] ?? "";
    });
}

function limpiarErroresEmpresa() {
    document
        .querySelectorAll("[data-error-for]")
        .forEach(function (elemento) {
            elemento.textContent = "";
        });
}


async function abrirAccionistasEmpresa(
    empresaId,
    razonSocial
) {
    if (!modalAccionistas) {
        console.error(
            "No se encontró el modal de accionistas."
        );

        return;
    }

    /*
     * ID del catálogo maestro Empresa.
     */
    empresaMaestraIdAccionistas =
        Number(empresaId) || 0;

    /*
     * Todavía no conocemos EbEmpresa.Id.
     * El backend lo devolverá.
     */
    empresaIdAccionistas =
        0;

    empresaNombreAccionistas =
        razonSocial;

    const nombreEmpresa =
        document.getElementById(
            "nombreEmpresaAccionistas"
        );

    if (nombreEmpresa) {
        nombreEmpresa.textContent =
            razonSocial;
    }

    limpiarListadoAccionistas();
    mostrarCargaAccionistas();

    modalAccionistas.show();

    await cargarAccionistasEmpresa();
}

async function cargarAccionistasEmpresa() {
    /*
     * Para entrar a Accionistas únicamente necesitamos
     * conocer Empresa.Id.
     */
    if (empresaMaestraIdAccionistas <= 0) {
        mostrarErrorAccionistas(
            "El identificador de la empresa no es válido."
        );

        return;
    }

    try {
        const parametros =
            new URLSearchParams({
                handler:
                    "Accionistas",

                /*
                 * Enviamos Empresa.Id maestro.
                 */
                empresaId:
                    empresaMaestraIdAccionistas
                        .toString()
            });

        const response =
            await fetch(
                `${window.location.pathname}?${parametros.toString()}`
            );

        const resultado =
            await response.json();

        if (!resultado.success) {
            mostrarErrorAccionistas(
                resultado.message ??
                "No fue posible consultar los accionistas."
            );

            return;
        }

        /*
         * El backend ya resolvió o creó EbEmpresa.
         * Guardamos su ID para Nuevo/Editar accionista.
         */
        empresaIdAccionistas =
            Number(
                resultado.complianceId
            ) || 0;

        if (empresaIdAccionistas <= 0) {
            mostrarErrorAccionistas(
                "No fue posible inicializar el expediente de Compliance."
            );

            return;
        }

        actualizarResumenAccionistas(
            resultado.resumen
        );

        renderizarAccionistas(
            resultado.data
        );
    }
    catch (error) {
        console.error(
            "Error al consultar accionistas:",
            error
        );

        mostrarErrorAccionistas(
            "Ocurrió un error al consultar los accionistas."
        );
    }
}

function actualizarResumenAccionistas(resumen) {
    document.getElementById(
        "totalAccionistas"
    ).textContent = resumen?.totalAccionistas ?? 0;

    document.getElementById(
        "porcentajeRegistrado"
    ).textContent = formatearPorcentaje(
        resumen?.porcentajeTotal
    );

    document.getElementById(
        "porcentajeDisponible"
    ).textContent = formatearPorcentaje(
        resumen?.porcentajeDisponible
    );
}

function renderizarAccionistas(accionistas) {
    const cuerpo = document.getElementById(
        "tablaAccionistasBody"
    );

    const estadoVacio = document.getElementById(
        "accionistasEstadoVacio"
    );

    const contenedorTabla = document.getElementById(
        "contenedorTablaAccionistas"
    );

    const carga = document.getElementById(
        "accionistasCargando"
    );

    carga?.classList.add("d-none");

    if (!cuerpo) {
        return;
    }

    if (
        !Array.isArray(accionistas) ||
        accionistas.length === 0
    ) {
        cuerpo.innerHTML = "";

        contenedorTabla?.classList.add(
            "d-none"
        );

        estadoVacio?.classList.remove(
            "d-none"
        );

        return;
    }

    estadoVacio?.classList.add(
        "d-none"
    );

    contenedorTabla?.classList.remove(
        "d-none"
    );

    cuerpo.innerHTML = accionistas.map(
        function (accionista) {
            const representante =
                accionista.esRepresentanteLegal
                    ? `
                        <span class="
                            eb-status
                            eb-status-representative
                        ">
                            Sí
                        </span>
                    `
                    : `
                        <span class="text-muted">
                            No
                        </span>
                    `;

            const estatus =
                accionista.deshabilitado
                    ? `
                        <span class="
                            eb-status
                            eb-status-inactive
                        ">
                            Inactivo
                        </span>
                    `
                    : `
                        <span class="
                            eb-status
                            eb-status-active
                        ">
                            Activo
                        </span>
                    `;

            const accionEstatus =
                accionista.deshabilitado
                    ? "Habilitar"
                    : "Deshabilitar";

            const opcionConsultar =
                permisosCompliance.puedeVisualizar
                    ? `
                        <li>
                            <button type="button"
                                    class="dropdown-item"
                                    onclick="consultarAccionista(
                                        ${accionista.id}
                                    )">

                                Consultar

                            </button>
                        </li>
                    `
                    : "";

            const opcionEditar =
                permisosCompliance.puedeModificar
                    ? `
                        <li>
                            <button type="button"
                                    class="dropdown-item"
                                    onclick="editarAccionista(
                                        ${accionista.id}
                                    )">

                                Editar

                            </button>
                        </li>
                    `
                    : "";

            const opcionEstatus =
                permisosCompliance.puedeModificar
                    ? `
                        <li>
                            <button type="button"
                                    class="dropdown-item"
                                    onclick="cambiarEstatusAccionista(
                                        ${accionista.id}
                                    )">

                                ${accionEstatus}

                            </button>
                        </li>
                    `
                    : "";

            const opcionEliminar =
                permisosCompliance.puedeEliminar
                    ? `
                        <li>
                            <hr class="dropdown-divider" />
                        </li>

                        <li>
                            <button type="button"
                                    class="dropdown-item text-danger"
                                    onclick="confirmarEliminarAccionista(
                                        ${accionista.id},
                                        '${escaparAtributoJs(
                        accionista.nombreCompleto
                    )}'
                                    )">

                                Eliminar

                            </button>
                        </li>
                    `
                    : "";

            const tieneAcciones =
                opcionConsultar !== "" ||
                opcionEditar !== "" ||
                opcionEstatus !== "" ||
                opcionEliminar !== "";

            const menuAcciones =
                tieneAcciones
                    ? `
                        <div class="
                            dropdown
                            eb-accionista-actions-dropdown
                        ">

                            <button type="button"
                                    class="btn btn-sm eb-action-button"
                                    data-bs-toggle="dropdown"
                                    data-bs-display="static"
                                    aria-expanded="false"
                                    title="Acciones">

                                <span class="eb-action-dots">
                                    ⋮
                                </span>

                            </button>

                            <ul class="
                                dropdown-menu
                                dropdown-menu-end
                                eb-accionista-actions-menu
                            ">
                                ${opcionConsultar}
                                ${opcionEditar}
                                ${opcionEstatus}
                                ${opcionEliminar}
                            </ul>

                        </div>
                    `
                    : `
                        <span class="text-muted">
                            Sin acciones
                        </span>
                    `;

            return `
                <tr>

                    <td>
                        <strong>
                            ${escaparHtml(
                accionista.nombreCompleto
            )}
                        </strong>
                    </td>

                    <td>
                        ${valorTexto(accionista.rfc)}
                    </td>

                    <td class="text-end">
                        ${formatearPorcentaje(
                accionista.porcentajeParticipacion
            )}
                    </td>

                    <td>
                        ${valorTexto(
                accionista.nacionalidad
            )}
                    </td>

                    <td class="text-center">
                        ${representante}
                    </td>

                    <td class="text-center">
                        ${estatus}
                    </td>

                    <td class="
                        text-center
                        eb-accionista-actions-cell
                    ">
                        ${menuAcciones}
                    </td>

                </tr>
            `;
        }
    ).join("");
}

function limpiarListadoAccionistas() {
    document.getElementById(
        "tablaAccionistasBody"
    ).innerHTML = "";

    document.getElementById(
        "totalAccionistas"
    ).textContent = "0";

    document.getElementById(
        "porcentajeRegistrado"
    ).textContent = "0.0000 %";

    document.getElementById(
        "porcentajeDisponible"
    ).textContent = "100.0000 %";

    document.getElementById(
        "accionistasEstadoVacio"
    )?.classList.add("d-none");

    document.getElementById(
        "contenedorTablaAccionistas"
    )?.classList.add("d-none");
}

function mostrarCargaAccionistas() {
    document.getElementById(
        "accionistasCargando"
    )?.classList.remove("d-none");

    document.getElementById(
        "accionistasError"
    )?.classList.add("d-none");
}

function mostrarErrorAccionistas(mensaje) {
    document.getElementById(
        "accionistasCargando"
    )?.classList.add("d-none");

    document.getElementById(
        "contenedorTablaAccionistas"
    )?.classList.add("d-none");

    document.getElementById(
        "accionistasEstadoVacio"
    )?.classList.add("d-none");

    const alerta = document.getElementById(
        "accionistasError"
    );

    const texto = document.getElementById(
        "accionistasErrorMensaje"
    );

    if (texto) {
        texto.textContent = mensaje;
    }

    alerta?.classList.remove("d-none");
}

function formatearPorcentaje(valor) {
    const numero = Number(valor ?? 0);

    if (Number.isNaN(numero)) {
        return "0.0000 %";
    }

    return `${numero.toFixed(4)} %`;
}

function mostrarResultado(mensaje, tipo) {
    const icono = document.getElementById("resultadoIcono");
    const titulo = document.getElementById("resultadoTitulo");
    const texto = document.getElementById("resultadoMensaje");

    texto.textContent = mensaje ?? "";

    if (tipo === "error") {
        icono.className = "eb-result-icon eb-result-error";
        icono.innerHTML =
            '<i class="fa-solid fa-circle-xmark"></i>';

        titulo.textContent = "No fue posible completar la operación";
    } else {
        icono.className = "eb-result-icon eb-result-success";
        icono.innerHTML =
            '<i class="fa-solid fa-circle-check"></i>';

        titulo.textContent = "Operación exitosa";
    }

    modalResultado.show();
}

function valorTexto(valor) {
    if (valor === null ||
        valor === undefined ||
        String(valor).trim() === "") {
        return "-";
    }

    return escaparHtml(String(valor));
}

function formatearFecha(valor) {
    if (!valor) {
        return "-";
    }

    const fecha = new Date(valor);

    if (Number.isNaN(fecha.getTime())) {
        return "-";
    }

    return fecha.toLocaleDateString("es-MX");
}

function formatearFechaHora(valor) {
    if (!valor) {
        return "-";
    }

    const fecha = new Date(valor);

    if (Number.isNaN(fecha.getTime())) {
        return "-";
    }

    return fecha.toLocaleString(
        "es-MX",
        {
            day: "2-digit",
            month: "2-digit",
            year: "numeric",
            hour: "2-digit",
            minute: "2-digit",
            hour12: true
        }
    );
}

function escaparHtml(texto) {
    return String(texto ?? "")
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#039;");
}

function confirmarEliminarSeleccionadas() {
    const seleccionadas = $("#tablaEmpresas")
        .bootstrapTable("getSelections");

    if (seleccionadas.length === 0) {
        return;
    }

    modoEliminacion = "multiple";
    empresaIdEliminar = 0;

    empresasIdsEliminar = seleccionadas.map(
        function (empresa) {
            return empresa.id;
        }
    );

    const mensaje = document.getElementById(
        "mensajeConfirmacionEliminar"
    );

    if (mensaje) {
        const cantidad = empresasIdsEliminar.length;

        mensaje.innerHTML = `
            ¿Estás seguro de que deseas eliminar
            <strong>${cantidad}</strong>
            ${cantidad === 1 ? "empresa seleccionada" : "empresas seleccionadas"}?
            No se realizará ningún cambio hasta que confirmes la operación.
        `;
    }

    modalEliminarEmpresa.show();
}

function escaparAtributoJs(texto) {
    return String(texto ?? "")
        .replaceAll("\\", "\\\\")
        .replaceAll("'", "\\'")
        .replaceAll('"', "&quot;")
        .replaceAll("\r", " ")
        .replaceAll("\n", " ");
}

function abrirModalCrearAccionista() {
    modoAccionista = "crear";

    limpiarFormularioAccionista();
    habilitarFormularioAccionista(true);

    document.getElementById("eaEmpresaId").value =
        empresaIdAccionistas;

    document.getElementById(
        "nombreEmpresaFormularioAccionista"
    ).textContent = empresaNombreAccionistas;

    document.getElementById(
        "modalAccionistaTitulo"
    ).textContent = "Nuevo accionista";

    document.getElementById(
        "btnGuardarAccionista"
    ).classList.remove("d-none");

    mostrarModalAccionista();
}

async function consultarAccionista(id) {
    modoAccionista = "consultar";

    const resultado = await obtenerAccionista(id);

    if (!resultado?.success) {
        mostrarResultado(
            resultado?.message ??
            "No fue posible consultar el accionista.",
            "error"
        );

        return;
    }

    cargarFormularioAccionista(resultado.data);
    habilitarFormularioAccionista(false);

    document.getElementById(
        "modalAccionistaTitulo"
    ).textContent = "Consultar accionista";

    document.getElementById(
        "nombreEmpresaFormularioAccionista"
    ).textContent = empresaNombreAccionistas;

    document.getElementById(
        "btnGuardarAccionista"
    ).classList.add("d-none");

    mostrarModalAccionista();
}

async function editarAccionista(id) {
    modoAccionista = "editar";

    const resultado = await obtenerAccionista(id);

    if (!resultado?.success) {
        mostrarResultado(
            resultado?.message ??
            "No fue posible cargar el accionista.",
            "error"
        );

        return;
    }

    cargarFormularioAccionista(resultado.data);
    habilitarFormularioAccionista(true);

    document.getElementById(
        "modalAccionistaTitulo"
    ).textContent = "Editar accionista";

    document.getElementById(
        "nombreEmpresaFormularioAccionista"
    ).textContent = empresaNombreAccionistas;

    document.getElementById(
        "btnGuardarAccionista"
    ).classList.remove("d-none");

    mostrarModalAccionista();
}

async function obtenerAccionista(id) {
    try {
        const parametros = new URLSearchParams({
            handler: "Accionista",
            id: id.toString()
        });

        const response = await fetch(
            `${window.location.pathname}?${parametros.toString()}`
        );

        return await response.json();
    } catch {
        return null;
    }
}

function cargarFormularioAccionista(data) {
    document.getElementById("eaId").value = data.id ?? 0;
    document.getElementById("eaEmpresaId").value =
        data.empresaId ?? empresaIdAccionistas;

    document.getElementById("eaNombreCompleto").value =
        data.nombreCompleto ?? "";

    document.getElementById("eaRfc").value =
        data.rfc ?? "";

    document.getElementById("eaNacionalidad").value =
        data.nacionalidad ?? "";

    document.getElementById("eaPorcentajeParticipacion").value =
        data.porcentajeParticipacion ?? "";

    document.getElementById("eaEsRepresentanteLegal").checked =
        Boolean(data.esRepresentanteLegal);

    limpiarErroresAccionista();
}

function limpiarFormularioAccionista() {
    document.getElementById("formAccionista")?.reset();

    document.getElementById("eaId").value = "0";
    document.getElementById("eaEmpresaId").value =
        empresaIdAccionistas.toString();

    document.getElementById("eaNombreCompleto").value = "";
    document.getElementById("eaRfc").value = "";
    document.getElementById("eaNacionalidad").value = "";
    document.getElementById("eaPorcentajeParticipacion").value = "";
    document.getElementById("eaEsRepresentanteLegal").checked = false;

    limpiarErroresAccionista();
}

function habilitarFormularioAccionista(habilitado) {
    document
        .querySelectorAll(
            "#formAccionista input:not([type='hidden'])"
        )
        .forEach(function (campo) {
            campo.disabled = !habilitado;
        });
}

function obtenerRequestAccionista() {
    return {
        id: parseInt(
            document.getElementById("eaId").value || "0",
            10
        ),
        empresaId: parseInt(
            document.getElementById("eaEmpresaId").value || "0",
            10
        ),
        nombreCompleto:
            document.getElementById("eaNombreCompleto").value,
        rfc:
            document.getElementById("eaRfc").value,
        nacionalidad:
            document.getElementById("eaNacionalidad").value,
        porcentajeParticipacion: parseFloat(
            document.getElementById(
                "eaPorcentajeParticipacion"
            ).value || "0"
        ),
        esRepresentanteLegal:
            document.getElementById(
                "eaEsRepresentanteLegal"
            ).checked
    };
}

async function guardarAccionista() {
    limpiarErroresAccionista();

    const handler = modoAccionista === "editar"
        ? "EditarAccionista"
        : "CrearAccionista";

    const boton = document.getElementById(
        "btnGuardarAccionista"
    );

    const htmlOriginal = boton.innerHTML;
    boton.disabled = true;
    boton.innerHTML = "Guardando...";

    try {
        const resultado = await enviarJson(
            handler,
            obtenerRequestAccionista()
        );

        if (!resultado.success) {
            mostrarErroresAccionista(resultado.errors);

            if (!resultado.errors) {
                mostrarResultado(
                    resultado.message ??
                    "No fue posible guardar el accionista.",
                    "error"
                );
            }

            return;
        }

        modalAccionista.hide();

        await cargarAccionistasEmpresa();

        mostrarResultado(
            resultado.message,
            "success"
        );
    } catch {
        mostrarResultado(
            "Ocurrió un error al guardar el accionista.",
            "error"
        );
    } finally {
        boton.disabled = false;
        boton.innerHTML = htmlOriginal;
    }
}

async function cambiarEstatusAccionista(id) {
    const resultado = await enviarJson(
        "CambiarEstatusAccionista",
        { id }
    );

    if (!resultado.success) {
        mostrarResultado(resultado.message, "error");
        return;
    }

    await cargarAccionistasEmpresa();
    mostrarResultado(resultado.message, "success");
}

function confirmarEliminarAccionista(id, nombre) {
    accionistaIdEliminar = id;

    document.getElementById(
        "nombreAccionistaEliminar"
    ).textContent = nombre;

    modalAccionistas.hide();

    setTimeout(function () {
        modalEliminarAccionista.show();
    }, 200);
}

async function eliminarAccionista() {
    if (accionistaIdEliminar <= 0) {
        return;
    }

    const boton = document.getElementById(
        "btnConfirmarEliminarAccionista"
    );

    const htmlOriginal = boton.innerHTML;
    boton.disabled = true;
    boton.innerHTML = "Eliminando...";

    try {
        const resultado = await enviarJson(
            "EliminarAccionista",
            { id: accionistaIdEliminar }
        );

        if (!resultado.success) {
            modalEliminarAccionista.hide();
            mostrarResultado(resultado.message, "error");
            return;
        }

        accionistaIdEliminar = 0;

        modalEliminarAccionista.hide();

        await cargarAccionistasEmpresa();

        setTimeout(function () {
            modalAccionistas.show();
        }, 200);

        mostrarResultado(
            resultado.message,
            "success"
        );
    } catch {
        mostrarResultado(
            "No fue posible eliminar el accionista.",
            "error"
        );
    } finally {
        boton.disabled = false;
        boton.innerHTML = htmlOriginal;
    }
}

function mostrarErroresAccionista(errors) {
    if (!errors) {
        return;
    }

    Object.entries(errors).forEach(function ([campo, mensajes]) {
        const elemento = document.querySelector(
            `[data-accionista-error-for="${campo}"]`
        );

        if (elemento) {
            elemento.textContent = mensajes?.[0] ?? "";
        }
    });
}

function limpiarErroresAccionista() {
    document
        .querySelectorAll("[data-accionista-error-for]")
        .forEach(function (elemento) {
            elemento.textContent = "";
        });
}

function mostrarModalAccionista() {
    if (!modalAccionista) {
        console.error(
            "No se encontró el formulario de accionista."
        );

        return;
    }

    modalAccionistas?.hide();

    const modalElement =
        document.getElementById(
            "modalAccionista"
        );

    if (!modalElement) {
        return;
    }

    const alCerrar = function () {
        modalElement.removeEventListener(
            "hidden.bs.modal",
            alCerrar
        );

        if (
            modalAccionistas &&
            empresaIdAccionistas > 0
        ) {
            modalAccionistas.show();
        }
    };

    modalElement.addEventListener(
        "hidden.bs.modal",
        alCerrar
    );

    setTimeout(function () {
        modalAccionista.show();
    }, 180);
}

/*async function abrirDocumentosEmpresa(
    empresaId,
    razonSocial
) {
    if (!modalDocumentos) {
        console.error(
            "No se encontró el modal de documentos."
        );

        mostrarResultado(
            "No fue posible abrir el expediente documental.",
            "error"
        );

        return;
    }

    empresaIdDocumentos = empresaId;
    empresaNombreDocumentos = razonSocial;
    tipoDocumentoSeleccionado = null;

    const nombreEmpresa = document.getElementById(
        "nombreEmpresaDocumentos"
    );

    if (nombreEmpresa) {
        nombreEmpresa.textContent =
            razonSocial;
    }

    limpiarListadoDocumentos();
    mostrarCargaDocumentos();

    modalDocumentos.show();

    await Promise.all([
        cargarInformacionCorporativaDocumentos(
            empresaId
        ),
        cargarDocumentosEmpresa()
    ]);
}*/

async function abrirDocumentosEmpresa(
    complianceId,
    empresaId,
    razonSocial
) {
    if (!modalDocumentos) {
        console.error(
            "No se encontró el modal de documentos."
        );

        mostrarResultado(
            "No fue posible abrir el expediente documental.",
            "error"
        );

        return;
    }

    /*
     * ==========================================================
     * EMPRESA MAESTRA
     * ==========================================================
     *
     * Para ABRIR Documentos utilizaremos siempre Empresa.Id.
     *
     * Ya NO bloquearemos la apertura si todavía no existe
     * EbEmpresa.Id.
     *
     * El backend será responsable de localizar o crear
     * automáticamente el expediente interno de Compliance.
     * ==========================================================
     */
    empresaMaestraIdDocumentos =
        Number(
            empresaId ?? 0
        );

    /*
     * Conservamos el ComplianceId recibido si ya existe,
     * pero puede venir en 0.
     *
     * OnGetDocumentosAsync devolverá posteriormente el
     * ComplianceId definitivo.
     */
    empresaIdDocumentos =
        Number(
            complianceId ?? 0
        );

    empresaNombreDocumentos =
        razonSocial;

    tipoDocumentoSeleccionado =
        null;

    /*
     * Únicamente Empresa.Id maestro es obligatorio.
     */
    if (
        empresaMaestraIdDocumentos <= 0
    ) {
        mostrarResultado(
            "No se identificó correctamente la empresa.",
            "error"
        );

        return;
    }

    const nombreEmpresa =
        document.getElementById(
            "nombreEmpresaDocumentos"
        );

    if (nombreEmpresa) {
        nombreEmpresa.textContent =
            razonSocial;
    }

    limpiarListadoDocumentos();

    mostrarCargaDocumentos();

    modalDocumentos.show();

    /*
     * Información corporativa:
     * Empresa.Id
     *
     * Documentos:
     * el backend resolverá EbEmpresa.Id.
     */
    await Promise.all([
        cargarInformacionCorporativaDocumentos(
            empresaMaestraIdDocumentos
        ),

        cargarDocumentosEmpresa()
    ]);
}

function limpiarListadoDocumentos() {

    documentosEmpresaActuales = [];
    tipoDocumentoSeleccionado = null;

    const matriz = document.getElementById(
        "matrizDocumentos"
    );

    if (matriz) {
        matriz.innerHTML = "";
    }

    establecerTexto(
        "totalDocumentosRequeridos",
        "0"
    );

    establecerTexto(
        "totalDocumentosCargados",
        "0"
    );

    establecerTexto(
        "totalDocumentosPendientes",
        "0"
    );

    establecerTexto(
        "totalDocumentosVencidos",
        "0"
    );

    establecerTexto(
        "documentoEmpresaNombre",
        "-"
    );

    establecerTexto(
        "documentoEmpresaRfc",
        "-"
    );

    establecerTexto(
        "documentoEmpresaNivel",
        "-"
    );

    establecerTexto(
        "documentoEmpresaActividad",
        "-"
    );

    establecerTexto(
        "documentoEmpresaTelefono",
        "-"
    );

    establecerTexto(
        "documentoEmpresaCorreo",
        "-"
    );

    establecerTexto(
        "documentoEmpresaFechaConstitucion",
        "-"
    );

    establecerTexto(
        "documentoEmpresaEscritura",
        "-"
    );

    establecerTexto(
        "documentoEmpresaAccionistas",
        "-"
    );

    document.getElementById(
        "panelArchivosDocumento"
    )?.classList.add("d-none");

    document.getElementById(
        "documentosError"
    )?.classList.add("d-none");
}

function mostrarCargaDocumentos() {
    document.getElementById(
        "documentosCargando"
    )?.classList.remove("d-none");

    document.getElementById(
        "documentosError"
    )?.classList.add("d-none");

    document.getElementById(
        "documentosEstadoVacio"
    )?.classList.add("d-none");

    document.getElementById(
        "contenedorTablaDocumentos"
    )?.classList.add("d-none");
}

async function cargarDocumentosEmpresa() {

    /*
     * ==========================================================
     * PARA CONSULTAR DOCUMENTOS ENTRAMOS CON Empresa.Id
     * ==========================================================
     */
    if (
        empresaMaestraIdDocumentos <= 0
    ) {
        mostrarErrorDocumentos(
            "El identificador de la empresa no es válido."
        );

        return;
    }

    try {

        const parametros =
            new URLSearchParams({
                handler:
                    "Documentos",

                /*
                 * Empresa.Id maestro.
                 */
                empresaId:
                    empresaMaestraIdDocumentos
                        .toString()
            });

        const response =
            await fetch(
                `${window.location.pathname}?${parametros.toString()}`
            );

        if (!response.ok) {
            throw new Error(
                `Error HTTP ${response.status}`
            );
        }

        const resultado =
            await response.json();

        if (!resultado.success) {
            mostrarErrorDocumentos(
                resultado.message ??
                "No fue posible consultar los documentos."
            );

            return;
        }

        /*
         * ======================================================
         * GUARDAR EbEmpresa.Id RESUELTO POR EL BACKEND
         * ======================================================
         *
         * A partir de aquí:
         *
         * empresaIdDocumentos = EbEmpresa.Id
         *
         * y podrá seguir utilizándose normalmente para:
         * - cargar
         * - historial
         * - eliminar
         * - descargar
         * - versiones
         * ======================================================
         */
        empresaIdDocumentos =
            Number(
                resultado.complianceId ?? 0
            );

        if (
            empresaIdDocumentos <= 0
        ) {
            mostrarErrorDocumentos(
                "No fue posible inicializar el expediente de Compliance."
            );

            return;
        }

        documentosEmpresaActuales =
            Array.isArray(
                resultado.data
            )
                ? resultado.data
                : [];

        actualizarResumenDocumentosBackend(
            resultado.resumen
        );

        renderizarMatrizDocumental(
            documentosEmpresaActuales
        );

        document
            .getElementById(
                "documentosCargando"
            )
            ?.classList.add(
                "d-none"
            );

        if (
            documentosEmpresaActuales.length ===
            0
        ) {
            const matriz =
                document.getElementById(
                    "matrizDocumentos"
                );

            if (matriz) {
                matriz.innerHTML = `
                    <div class="eb-documents-empty">

                        <div class="eb-documents-empty-icon">
                            <i class="fa-regular fa-folder-open"></i>
                        </div>

                        <h6>
                            No hay tipos de documento configurados
                        </h6>

                        <p>
                            El catálogo documental no contiene
                            registros activos.
                        </p>

                    </div>
                `;
            }
        }

    } catch (error) {

        console.error(
            error
        );

        mostrarErrorDocumentos(
            "Ocurrió un error al consultar la documentación."
        );
    }
}

function actualizarResumenDocumentosBackend(
    resumen) {
    establecerTexto(
        "totalDocumentosRequeridos",
        resumen?.totalRequeridos ?? 0
    );

    establecerTexto(
        "totalDocumentosCargados",
        resumen?.totalCargados ?? 0
    );

    establecerTexto(
        "totalDocumentosPendientes",
        resumen?.totalPendientes ?? 0
    );

    establecerTexto(
        "totalDocumentosVencidos",
        resumen?.totalVencidos ?? 0
    );
}

let tipoDocumentoSeleccionado = null;
let documentosEmpresaActuales = [];

/*async function cargarInformacionCorporativaDocumentos(empresaId) {
    const respuestaEmpresa = await obtenerEmpresa(empresaId);

    if (respuestaEmpresa?.success) {
        llenarInformacionCorporativaDocumento(
            respuestaEmpresa.data
        );
    }

    try {
        const parametros = new URLSearchParams({
            handler: "Accionistas",
            empresaId: empresaId.toString()
        });

        const response = await fetch(
            `${window.location.pathname}?${parametros.toString()}`
        );

        const resultado = await response.json();

        if (!resultado.success) {
            establecerTexto(
                "documentoEmpresaAccionistas",
                "-"
            );

            return;
        }

        const accionistasActivos = (resultado.data ?? []).filter(
            function (accionista) {
                return !accionista.deshabilitado;
            }
        );

        if (accionistasActivos.length === 0) {
            establecerTexto(
                "documentoEmpresaAccionistas",
                "-"
            );

            return;
        }

        const textoAccionistas = accionistasActivos.map(
            function (accionista) {
                return `${accionista.nombreCompleto} ` +
                    `(${formatearPorcentaje(
                        accionista.porcentajeParticipacion
                    )})`;
            }
        ).join(", ");

        establecerTexto(
            "documentoEmpresaAccionistas",
            textoAccionistas
        );
    } catch {
        establecerTexto(
            "documentoEmpresaAccionistas",
            "-"
        );
    }
}*/

async function cargarInformacionCorporativaDocumentos(
    empresaId
) {
    /*
     * ==========================================================
     * INFORMACIÓN CORPORATIVA
     * ==========================================================
     *
     * empresaId = Empresa.Id
     */
    const respuestaEmpresa =
        await obtenerEmpresa(
            empresaId
        );

    if (respuestaEmpresa?.success) {
        llenarInformacionCorporativaDocumento(
            respuestaEmpresa.data
        );
    }

    /*
     * ==========================================================
     * ACCIONISTAS
     * ==========================================================
     *
     * Los accionistas todavía dependen de EbEmpresa.Id.
     */
    if (empresaIdDocumentos <= 0) {
        establecerTexto(
            "documentoEmpresaAccionistas",
            "-"
        );

        return;
    }

    try {
        const parametros =
            new URLSearchParams({
                handler: "Accionistas",
                empresaId:
                    empresaIdDocumentos.toString()
            });

        const response =
            await fetch(
                `${window.location.pathname}?${parametros.toString()}`
            );

        const resultado =
            await response.json();

        if (!resultado.success) {
            establecerTexto(
                "documentoEmpresaAccionistas",
                "-"
            );

            return;
        }

        const accionistasActivos =
            (resultado.data ?? [])
                .filter(
                    function (accionista) {
                        return !accionista.deshabilitado;
                    }
                );

        if (accionistasActivos.length === 0) {
            establecerTexto(
                "documentoEmpresaAccionistas",
                "-"
            );

            return;
        }

        const textoAccionistas =
            accionistasActivos
                .map(
                    function (accionista) {
                        return (
                            `${accionista.nombreCompleto} ` +
                            `(${formatearPorcentaje(
                                accionista.porcentajeParticipacion
                            )})`
                        );
                    }
                )
                .join(", ");

        establecerTexto(
            "documentoEmpresaAccionistas",
            textoAccionistas
        );

    } catch {
        establecerTexto(
            "documentoEmpresaAccionistas",
            "-"
        );
    }
}

function llenarInformacionCorporativaDocumento(empresa) {
    establecerTexto(
        "documentoEmpresaNombre",
        empresa.razonSocial
    );

    establecerTexto(
        "documentoEmpresaRfc",
        empresa.rfc
    );

    establecerTexto(
        "documentoEmpresaNivel",
        empresa.nivel
    );

    establecerTexto(
        "documentoEmpresaActividad",
        empresa.actividadComercial
    );

    establecerTexto(
        "documentoEmpresaTelefono",
        empresa.telefonoBancos
    );

    establecerTexto(
        "documentoEmpresaCorreo",
        empresa.correoBancos
    );

    establecerTexto(
        "documentoEmpresaFechaConstitucion",
        formatearFecha(empresa.fechaConstitucion)
    );

    establecerTexto(
        "documentoEmpresaEscritura",
        empresa.numeroEscritura
    );
}

function establecerTexto(id, valor) {
    const elemento = document.getElementById(id);

    if (!elemento) {
        return;
    }

    const texto = valor === null ||
        valor === undefined ||
        String(valor).trim() === ""
        ? "-"
        : String(valor);

    elemento.textContent = texto;
}

function renderizarMatrizDocumental(documentos) {
    const matriz = document.getElementById(
        "matrizDocumentos"
    );

    if (!matriz) {
        return;
    }

    if (
        !Array.isArray(documentos) ||
        documentos.length === 0
    ) {
        matriz.innerHTML = `
            <div class="eb-documents-empty">

                <div class="eb-documents-empty-icon">
                    <i class="fa-regular fa-folder-open"></i>
                </div>

                <h6>
                    No hay tipos de documento configurados
                </h6>

                <p>
                    El catálogo documental no contiene registros activos.
                </p>

            </div>
        `;

        return;
    }

    matriz.innerHTML = documentos.map(
        function (documento) {
            const totalArchivos =
                Number(
                    documento.totalArchivos ?? 0
                );

            const tieneArchivos =
                totalArchivos > 0;

            const estatusClase =
                obtenerClaseEstatusDocumento(
                    documento.estatus
                );

            const textoPrincipal =
                tieneArchivos
                    ? totalArchivos === 1
                        ? "1 archivo cargado"
                        : `${totalArchivos} archivos cargados`
                    : "Selecciona un archivo";

            const textoSecundario =
                tieneArchivos
                    ? "Haz clic para consultar los archivos"
                    : permisosCompliance.puedeCrearCargar
                        ? "Haz clic para cargar el documento"
                        : "No tienes permiso para cargar documentos";

            /*
             * La tarjeta puede abrirse:
             *
             * - Cuando hay archivos y puede visualizar.
             * - Cuando está pendiente y puede cargar.
             */
            const puedeAbrirDocumento =
                tieneArchivos
                    ? permisosCompliance.puedeVisualizar
                    : permisosCompliance.puedeCrearCargar;

            const accionTarjeta =
                puedeAbrirDocumento
                    ? tieneArchivos
                        ? `seleccionarTipoDocumento(${documento.id})`
                        : `prepararCargaDocumento(${documento.id})`
                    : "";

            const atributoClick =
                accionTarjeta
                    ? `onclick="${accionTarjeta}"`
                    : "";

            const atributoDisabled =
                puedeAbrirDocumento
                    ? ""
                    : "disabled";

            const claseSinPermiso =
                puedeAbrirDocumento
                    ? ""
                    : "eb-document-card-disabled";

            /*
             * Visualizar documento.
             */
            const opcionVisualizarArchivo =
                tieneArchivos &&
                    permisosCompliance.puedeVisualizar
                    ? `
                        <li>
                            <button type="button"
                                    class="dropdown-item"
                                    onclick="event.stopPropagation();
                                             seleccionarTipoDocumento(
                                                 ${documento.id}
                                             )">

                                <i class="fa-regular fa-eye me-2"></i>
                                Visualizar archivo

                            </button>
                        </li>
                    `
                    : "";

            /*
             * Cargar documento o nueva versión.
             */
            const opcionCargarArchivo =
                permisosCompliance.puedeCrearCargar
                    ? `
                        <li>
                            <button type="button"
                                    class="dropdown-item"
                                    onclick="event.stopPropagation();
                                             prepararCargaDocumento(
                                                 ${documento.id}
                                             )">

                                <i class="fa-solid fa-cloud-arrow-up me-2"></i>

                                ${tieneArchivos
                        ? "Cargar nueva versión"
                        : "Cargar archivo"
                    }

                            </button>
                        </li>
                    `
                    : "";

            /*
             * Descargar documento.
             */
            const opcionDescargarArchivo =
                tieneArchivos &&
                    permisosCompliance.puedeDescargar
                    ? `
                        <li>
                            <button type="button"
                                    class="dropdown-item"
                                    onclick="event.stopPropagation();
                                             descargarDocumento(
                                                 ${documento.id}
                                             )">

                                <i class="fa-solid fa-download me-2"></i>
                                Descargar

                            </button>
                        </li>
                    `
                    : "";

            /*
             * Historial de versiones.
             */
            const opcionHistorialArchivo =
                tieneArchivos &&
                    permisosCompliance.puedeVisualizar
                    ? `
                        <li>
                            <button type="button"
                                    class="dropdown-item"
                                    onclick="event.stopPropagation();
                                             abrirHistorialDocumento(
                                                 ${documento.id}
                                             )">

                                <i class="fa-solid fa-clock-rotate-left me-2"></i>
                                Historial de versiones

                            </button>
                        </li>
                    `
                    : "";

            /*
             * Eliminar documento.
             */
            const opcionEliminarArchivo =
                tieneArchivos &&
                    permisosCompliance.puedeEliminar
                    ? `
                        <li>
                            <hr class="dropdown-divider" />
                        </li>

                        <li>
                            <button type="button"
                                    class="dropdown-item text-danger"
                                    onclick="event.stopPropagation();
                                             confirmarEliminarArchivoDocumento(
                                                 ${documento.id}
                                             )">

                                <i class="fa-regular fa-trash-can me-2"></i>
                                Eliminar archivo

                            </button>
                        </li>
                    `
                    : "";

            const tieneAccionesDocumento =
                opcionVisualizarArchivo !== "" ||
                opcionCargarArchivo !== "" ||
                opcionDescargarArchivo !== "" ||
                opcionHistorialArchivo !== "" ||
                opcionEliminarArchivo !== "";

            const menuAccionesDocumento =
                tieneAccionesDocumento
                    ? `
                        <div class="
                            dropdown
                            eb-document-upload-actions
                        ">

                            <button type="button"
                                    class="btn eb-document-upload-menu"
                                    data-bs-toggle="dropdown"
                                    data-bs-display="static"
                                    data-bs-boundary="viewport"
                                    aria-expanded="false"
                                    aria-label="Acciones de ${escaparHtml(
                        documento.nombre
                    )}">

                                <span>⋮</span>

                            </button>

                            <ul class="
                                dropdown-menu
                                dropdown-menu-end
                                eb-document-upload-dropdown
                            ">
                                ${opcionVisualizarArchivo}
                                ${opcionCargarArchivo}
                                ${opcionDescargarArchivo}
                                ${opcionHistorialArchivo}
                                ${opcionEliminarArchivo}
                            </ul>

                        </div>
                    `
                    : "";

            return `
                <article class="
                    eb-document-upload-card
                    ${tieneArchivos
                    ? "eb-document-upload-card-loaded"
                    : "eb-document-upload-card-empty"
                }
                    ${claseSinPermiso}
                ">

                    <div class="eb-document-upload-title-row">

                        <div class="eb-document-upload-title">

                            <strong>
                                ${escaparHtml(documento.nombre)}
                            </strong>

                            ${documento.obligatorio
                    ? `
                                        <span class="
                                            eb-document-required-mark
                                        "
                                              title="Documento obligatorio">
                                            *
                                        </span>
                                    `
                    : ""
                }

                        </div>

                    </div>

                    <div class="eb-document-upload-area-wrapper">

                        <button type="button"
                                class="eb-document-upload-area"
                                ${atributoClick}
                                ${atributoDisabled}>

                            <span class="eb-document-upload-file-text">

                                <strong>
                                    ${textoPrincipal}
                                </strong>

                                <small>
                                    ${textoSecundario}
                                </small>

                            </span>

                            <span class="
                                eb-document-status
                                ${estatusClase}
                            ">
                                ${escaparHtml(documento.estatus)}
                            </span>

                        </button>

                        ${menuAccionesDocumento}

                    </div>

                </article>
            `;
        }
    ).join("");
}

async function abrirHistorialDocumento(
    tipoDocumentoId
) {
    const documento =
        obtenerDocumentoPorTipo(
            tipoDocumentoId
        );

    if (!documento) {
        mostrarResultado(
            "No se encontró el documento seleccionado.",
            "error"
        );

        return;
    }

    if (!modalHistorialDocumento) {
        mostrarResultado(
            "No se encontró el historial documental.",
            "error"
        );

        return;
    }

    if (empresaIdDocumentos <= 0) {
        mostrarResultado(
            "No se identificó la empresa del expediente.",
            "error"
        );

        return;
    }

    tipoDocumentoHistorialId =
        Number(documento.id);

    nombreTipoDocumentoHistorial =
        documento.nombre ?? "Documento";

    establecerTexto(
        "nombreDocumentoHistorial",
        nombreTipoDocumentoHistorial
    );

    limpiarHistorialDocumento();
    mostrarCargaHistorialDocumento();

    modalDocumentos?.hide();

    setTimeout(function () {
        modalHistorialDocumento?.show();
    }, 180);

    await cargarHistorialDocumento();
}

function limpiarHistorialDocumento() {
    establecerTexto(
        "historialTotalVersiones",
        "0"
    );

    establecerTexto(
        "historialVersionesActivas",
        "0"
    );

    establecerTexto(
        "historialVersionesEliminadas",
        "0"
    );

    establecerTexto(
        "historialVersionActual",
        "-"
    );

    document
        .getElementById("historialDocumentoCargando")
        ?.classList.add("d-none");

    document
        .getElementById("historialDocumentoError")
        ?.classList.add("d-none");

    document
        .getElementById("historialDocumentoVacio")
        ?.classList.add("d-none");

    document
        .getElementById("listaHistorialDocumento")
        ?.classList.add("d-none");

    const lista = document.getElementById(
        "listaHistorialDocumento"
    );

    if (lista) {
        lista.innerHTML = "";
    }
}

function mostrarCargaHistorialDocumento() {
    document
        .getElementById("historialDocumentoCargando")
        ?.classList.remove("d-none");

    document
        .getElementById("historialDocumentoError")
        ?.classList.add("d-none");

    document
        .getElementById("historialDocumentoVacio")
        ?.classList.add("d-none");

    document
        .getElementById("listaHistorialDocumento")
        ?.classList.add("d-none");
}

function mostrarErrorHistorialDocumento(
    mensaje
) {
    document
        .getElementById("historialDocumentoCargando")
        ?.classList.add("d-none");

    document
        .getElementById("historialDocumentoVacio")
        ?.classList.add("d-none");

    document
        .getElementById("listaHistorialDocumento")
        ?.classList.add("d-none");

    establecerTexto(
        "historialDocumentoErrorMensaje",
        mensaje
    );

    document
        .getElementById("historialDocumentoError")
        ?.classList.remove("d-none");
}

function actualizarResumenHistorialDocumento(
    resumen
) {
    establecerTexto(
        "historialTotalVersiones",
        resumen?.totalVersiones ?? 0
    );

    establecerTexto(
        "historialVersionesActivas",
        resumen?.versionesActivas ?? 0
    );

    establecerTexto(
        "historialVersionesEliminadas",
        resumen?.versionesEliminadas ?? 0
    );

    establecerTexto(
        "historialVersionActual",
        resumen?.versionActual
            ? `V${resumen.versionActual}`
            : "-"
    );
}

async function cargarHistorialDocumento() {
    if (
        empresaIdDocumentos <= 0 ||
        tipoDocumentoHistorialId <= 0
    ) {
        mostrarErrorHistorialDocumento(
            "No fue posible identificar el documento."
        );

        return;
    }

    try {
        const parametros = new URLSearchParams({
            handler: "HistorialDocumento",
            empresaId:
                empresaIdDocumentos.toString(),
            tipoDocumentoId:
                tipoDocumentoHistorialId.toString()
        });

        const response = await fetch(
            `${window.location.pathname}?${parametros.toString()}`,
            {
                credentials: "same-origin"
            }
        );

        const contenido =
            await response.text();

        let resultado = null;

        if (contenido) {
            try {
                resultado = JSON.parse(
                    contenido
                );
            } catch {
                throw new Error(
                    "El servidor devolvió una respuesta no válida."
                );
            }
        }

        if (!response.ok) {
            throw new Error(
                resultado?.message ??
                `Error HTTP ${response.status}.`
            );
        }

        if (!resultado?.success) {
            mostrarErrorHistorialDocumento(
                resultado?.message ??
                "No fue posible consultar el historial."
            );

            return;
        }

        actualizarResumenHistorialDocumento(
            resultado.resumen
        );

        renderizarHistorialDocumento(
            resultado.data
        );
    } catch (error) {
        console.error(
            "Error al consultar historial:",
            error
        );

        mostrarErrorHistorialDocumento(
            error?.message ??
            "Ocurrió un error al consultar el historial."
        );
    }
}

function renderizarHistorialDocumento(
    versiones
) {
    document
        .getElementById("historialDocumentoCargando")
        ?.classList.add("d-none");

    document
        .getElementById("historialDocumentoError")
        ?.classList.add("d-none");

    const lista = document.getElementById(
        "listaHistorialDocumento"
    );

    const estadoVacio = document.getElementById(
        "historialDocumentoVacio"
    );

    if (!lista) {
        return;
    }

    if (
        !Array.isArray(versiones) ||
        versiones.length === 0
    ) {
        lista.innerHTML = "";
        lista.classList.add("d-none");

        estadoVacio?.classList.remove("d-none");

        return;
    }

    estadoVacio?.classList.add("d-none");
    lista.classList.remove("d-none");

    lista.innerHTML = versiones.map(
        function (version) {
            const claseSituacion =
                obtenerClaseSituacionHistorial(
                    version.situacion
                );

            const fechaCarga = version.fechaCarga
                ? formatearFechaHora(
                    version.fechaCarga
                )
                : "-";

            const fechaVencimiento =
                version.fechaVencimiento
                    ? formatearFecha(
                        version.fechaVencimiento
                    )
                    : "No aplica";

            const fechaEliminacion =
                version.fechaEliminacion
                    ? formatearFecha(
                        version.fechaEliminacion
                    )
                    : null;

            const botonVisualizarHistorial =
                !version.eliminado &&
                    permisosCompliance.puedeVisualizar
                    ? `
            <button type="button"
                    class="btn btn-outline-primary"
                    onclick="visualizarArchivoDocumento(
                        ${version.id}
                    )">

                <i class="fa-regular fa-eye"></i>

                <span>
                    Visualizar
                </span>

            </button>
        `
                    : "";

            const botonDescargarHistorial =
                !version.eliminado &&
                    permisosCompliance.puedeDescargar
                    ? `
            <button type="button"
                    class="btn btn-outline-primary"
                    onclick="descargarArchivoDocumento(
                        ${version.id},
                        '${escaparAtributoJs(
                        version.nombreOriginal ??
                        "Documento"
                    )}'
                    )">

                <i class="fa-solid fa-download"></i>

                <span>
                    Descargar
                </span>

            </button>
        `
                    : "";

            const tieneAccionesHistorial =
                botonVisualizarHistorial !== "" ||
                botonDescargarHistorial !== "";

            const accionesHistorial =
                version.eliminado
                    ? `
            <span class="eb-history-unavailable">
                Archivo no disponible
            </span>
        `
                    : tieneAccionesHistorial
                        ? `
                ${botonVisualizarHistorial}
                ${botonDescargarHistorial}
            `
                        : "";

            return `
                <article class="
                    eb-history-item
                    ${version.eliminado
                    ? "eb-history-item-deleted"
                    : ""}
                ">

                    <div class="eb-history-version">

                        <span>
                            Versión
                        </span>

                        <strong>
                            ${version.version}
                        </strong>

                    </div>

                    <div class="eb-history-information">

                        <div class="eb-history-title-row">

                            <strong>
                                ${escaparHtml(
                        version.nombreOriginal
                    )}
                            </strong>

                            <span class="
                                eb-history-status
                                ${claseSituacion}
                            ">
                                ${escaparHtml(
                        version.situacion
                    )}
                            </span>

                        </div>

                        <div class="eb-history-metadata">

                            <span>
                                <i class="fa-regular fa-calendar me-1"></i>
                                Cargado:
                                ${fechaCarga}
                            </span>

                            <span>
                                <i class="fa-regular fa-clock me-1"></i>
                                Vencimiento:
                                ${fechaVencimiento}
                            </span>

                            <span>
                                <i class="fa-solid fa-hard-drive me-1"></i>
                                ${formatearTamanoArchivo(
                        version.tamanoBytes
                    )}
                            </span>

                        </div>

                        ${version.observaciones
                    ? `
                                <p class="eb-history-observations">
                                    ${escaparHtml(
                        version.observaciones
                    )}
                                </p>
                            `
                    : ""
                }

                        ${fechaEliminacion
                    ? `
                                <small class="eb-history-deletion">
                                    Eliminado:
                                    ${fechaEliminacion}
                                </small>
                            `
                    : ""
                }

                    </div>

                    <div class="eb-history-actions">
                        ${accionesHistorial}
                    </div>

                </article>
            `;
        }
    ).join("");
}

function obtenerClaseSituacionHistorial(
    situacion
) {
    const valor = String(
        situacion ?? ""
    ).toLowerCase();

    switch (valor) {
        case "actual":
            return "eb-history-status-current";

        case "eliminada":
            return "eb-history-status-deleted";

        default:
            return "eb-history-status-replaced";
    }
}

function obtenerDocumentoPorTipo(
    tipoDocumentoId
) {
    const id = Number(tipoDocumentoId);

    if (
        !Number.isInteger(id) ||
        id <= 0
    ) {
        return null;
    }

    return documentosEmpresaActuales.find(
        function (documento) {
            return Number(documento.id) === id;
        }
    ) ?? null;
}

function obtenerArchivoActualDocumento(
    documento
) {
    const archivos = Array.isArray(
        documento?.archivos
    )
        ? documento.archivos
        : [];

    if (archivos.length === 0) {
        return null;
    }

    /*
     * El backend devuelve primero
     * el archivo más reciente.
     */
    return archivos[0];
}

function seleccionarTipoDocumento(
    tipoDocumentoId
) {
    const documento =
        obtenerDocumentoPorTipo(
            tipoDocumentoId
        );

    if (!documento) {
        mostrarResultado(
            "No se encontró el documento seleccionado.",
            "error"
        );

        return;
    }

    const archivoActual =
        obtenerArchivoActualDocumento(
            documento
        );

    if (!archivoActual) {
        prepararCargaDocumento(
            tipoDocumentoId
        );

        return;
    }

    visualizarArchivoDocumento(
        archivoActual.id
    );
}

function renderizarArchivosDocumento(archivos) {
    const estadoVacio = document.getElementById(
        "archivosDocumentoEstadoVacio"
    );

    const lista = document.getElementById(
        "listaArchivosDocumento"
    );

    if (!lista) {
        return;
    }

    if (
        !Array.isArray(archivos) ||
        archivos.length === 0
    ) {
        lista.innerHTML = "";
        lista.classList.add("d-none");

        estadoVacio?.classList.remove("d-none");

        return;
    }

    estadoVacio?.classList.add("d-none");
    lista.classList.remove("d-none");

    lista.innerHTML = archivos.map(
        function (archivo) {
            const vencimiento =
                archivo.fechaVencimiento
                    ? formatearFecha(
                        archivo.fechaVencimiento
                    )
                    : "No aplica";

            return `
                <article class="eb-document-file-item">

                    <div class="eb-document-file-icon">
                        <i class="${obtenerIconoArchivo(
                archivo.extension
            )}"></i>
                    </div>

                    <div class="eb-document-file-information">

                        <strong>
                            ${escaparHtml(
                archivo.nombreOriginal
            )}
                        </strong>

                        <span>
                            Versión ${archivo.version}
                            ·
                            ${formatearTamanoArchivo(
                archivo.tamanoBytes
            )}
                        </span>

                        <small>
                            Cargado:
                            ${formatearFecha(
                archivo.fechaCarga
            )}
                            ·
                            Vencimiento:
                            ${vencimiento}
                        </small>

                    </div>

                    <div class="dropdown">

                        <button type="button"
                                class="btn eb-document-upload-menu"
                                data-bs-toggle="dropdown"
                                data-bs-display="static"
                                data-bs-boundary="viewport"
                                onclick="event.stopPropagation()"
                                aria-expanded="false">

                            <span>⋮</span>
                        </button>

                        <ul class="dropdown-menu dropdown-menu-end">

                            <li>
                                <button type="button"
                                        class="dropdown-item"
                                        onclick="visualizarArchivoDocumento(
                                            ${archivo.id}
                                        )">

                                    <i class="fa-regular fa-eye me-2"></i>
                                    Visualizar
                                </button>
                            </li>

                            <li>
                                <button type="button"
                                        class="dropdown-item"
                                        onclick="descargarArchivoDocumento(
                                            ${archivo.id}
                                        )">

                                    <i class="fa-solid fa-download me-2"></i>
                                    Descargar
                                </button>
                            </li>

                            <li>
                                <button type="button"
                                        class="dropdown-item text-danger"
                                        onclick="confirmarEliminarArchivoDocumento(
                                            ${archivo.id},
                                            '${escaparAtributoJs(
                archivo.nombreOriginal
            )}'
                                        )">

                                    <i class="fa-regular fa-trash-can me-2"></i>
                                    Eliminar
                                </button>
                            </li>

                        </ul>

                    </div>

                </article>
            `;
        }
    ).join("");
}

function obtenerIconoArchivo(extension) {
    const extensionNormalizada =
        String(extension ?? "")
            .toLowerCase()
            .replace(".", "");

    switch (extensionNormalizada) {
        case "pdf":
            return "fa-regular fa-file-pdf";

        case "doc":
        case "docx":
            return "fa-regular fa-file-word";

        case "xls":
        case "xlsx":
            return "fa-regular fa-file-excel";

        case "jpg":
        case "jpeg":
        case "png":
        case "webp":
            return "fa-regular fa-file-image";

        case "zip":
        case "rar":
        case "7z":
            return "fa-regular fa-file-zipper";

        default:
            return "fa-regular fa-file-lines";
    }
}

function formatearTamanoArchivo(bytes) {
    const tamano = Number(bytes ?? 0);

    if (
        Number.isNaN(tamano) ||
        tamano <= 0
    ) {
        return "0 KB";
    }

    if (tamano < 1024) {
        return `${tamano} bytes`;
    }

    if (tamano < 1024 * 1024) {
        return `${(
            tamano / 1024
        ).toFixed(2)} KB`;
    }

    if (tamano < 1024 * 1024 * 1024) {
        return `${(
            tamano / 1024 / 1024
        ).toFixed(2)} MB`;
    }

    return `${(
        tamano / 1024 / 1024 / 1024
    ).toFixed(2)} GB`;
}

function visualizarArchivoDocumento(id) {
    const archivoId = Number(id);

    if (
        !Number.isInteger(archivoId) ||
        archivoId <= 0
    ) {
        mostrarResultado(
            "El archivo seleccionado no es válido.",
            "error"
        );

        return;
    }

    cerrarResultadoAnterior();

    const parametros = new URLSearchParams({
        handler: "VisualizarDocumento",
        id: archivoId.toString()
    });

    const url =
        `${window.location.pathname}` +
        `?${parametros.toString()}`;

    window.open(
        url,
        "_blank"
    );
}

function descargarArchivoDocumento(
    id,
    nombreArchivo = ""
) {
    const archivoId = Number(id);

    if (
        !Number.isInteger(archivoId) ||
        archivoId <= 0
    ) {
        mostrarResultado(
            "El archivo seleccionado no es válido.",
            "error"
        );

        return;
    }

    if (!modalSeleccionarBanco) {
        mostrarResultado(
            "No se encontró el formulario de selección de banco.",
            "error"
        );

        return;
    }

    cerrarResultadoAnterior();

    documentoIdDescarga =
        archivoId;

    nombreDocumentoDescarga =
        nombreArchivo?.trim() ||
        "Documento seleccionado";

    regresarDocumentosDespuesDescarga =
        Boolean(
            modalDocumentos &&
            document
                .getElementById(
                    "modalDocumentos"
                )
                ?.classList.contains("show")
        );

    regresarHistorialDespuesDescarga =
        Boolean(
            modalHistorialDocumento &&
            document
                .getElementById(
                    "modalHistorialDocumento"
                )
                ?.classList.contains("show")
        );

    document
        .getElementById(
            "documentoIdDescarga"
        )
        ?.setAttribute(
            "value",
            archivoId.toString()
        );

    establecerTexto(
        "nombreDocumentoDescarga",
        nombreDocumentoDescarga
    );

    const selectorBanco =
        document.getElementById(
            "bancoDocumentoDescarga"
        );

    if (selectorBanco) {
        selectorBanco.value = "";
    }

    limpiarErrorBancoDescarga();

    if (
        regresarHistorialDespuesDescarga &&
        modalHistorialDocumento
    ) {
        modalHistorialDocumento.hide();

        setTimeout(
            function () {
                modalSeleccionarBanco.show();

                document
                    .getElementById(
                        "bancoDocumentoDescarga"
                    )
                    ?.focus();
            },
            180
        );

        return;
    }

    if (
        regresarDocumentosDespuesDescarga &&
        modalDocumentos
    ) {
        modalDocumentos.hide();

        setTimeout(
            function () {
                modalSeleccionarBanco.show();

                document
                    .getElementById(
                        "bancoDocumentoDescarga"
                    )
                    ?.focus();
            },
            180
        );

        return;
    }

    modalSeleccionarBanco.show();

    setTimeout(
        function () {
            document
                .getElementById(
                    "bancoDocumentoDescarga"
                )
                ?.focus();
        },
        250
    );
}


function confirmarDescargaDocumento() {
    const archivoId =
        Number(documentoIdDescarga);

    const banco =
        document
            .getElementById(
                "bancoDocumentoDescarga"
            )
            ?.value
            ?.trim() ?? "";

    limpiarErrorBancoDescarga();

    if (
        !Number.isInteger(archivoId) ||
        archivoId <= 0
    ) {
        mostrarErrorBancoDescarga(
            "El documento seleccionado no es válido."
        );

        return;
    }

    if (!banco) {
        mostrarErrorBancoDescarga(
            "Selecciona el banco asociado a la descarga."
        );

        document
            .getElementById(
                "bancoDocumentoDescarga"
            )
            ?.focus();

        return;
    }

    const boton =
        document.getElementById(
            "btnConfirmarDescargaDocumento"
        );

    const htmlOriginal =
        boton?.innerHTML ?? "";

    if (boton) {
        boton.disabled = true;

        boton.innerHTML =
            '<i class="fa-solid fa-spinner ' +
            'fa-spin me-1"></i>' +
            " Preparando...";
    }

    try {
        const parametros =
            new URLSearchParams({
                handler:
                    "DescargarDocumento",

                id:
                    archivoId.toString(),

                banco:
                    banco
            });

        const url =
            `${window.location.pathname}` +
            `?${parametros.toString()}`;

        const enlace =
            document.createElement("a");

        enlace.href = url;
        enlace.style.display = "none";

        document.body.appendChild(
            enlace
        );

        enlace.click();
        enlace.remove();

        modalSeleccionarBanco?.hide();
    } catch (error) {
        console.error(
            "Error al preparar la descarga:",
            error
        );

        mostrarErrorBancoDescarga(
            "No fue posible preparar la descarga."
        );
    } finally {
        if (boton) {
            setTimeout(
                function () {
                    boton.disabled = false;
                    boton.innerHTML =
                        htmlOriginal;
                },
                500
            );
        }
    }
}

function mostrarErrorBancoDescarga(
    mensaje
) {
    const elemento =
        document.getElementById(
            "bancoDocumentoDescargaError"
        );

    if (elemento) {
        elemento.textContent =
            mensaje ?? "";
    }

    document
        .getElementById(
            "bancoDocumentoDescarga"
        )
        ?.classList.add(
            "is-invalid"
        );
}

function limpiarErrorBancoDescarga() {
    const elemento =
        document.getElementById(
            "bancoDocumentoDescargaError"
        );

    if (elemento) {
        elemento.textContent = "";
    }

    document
        .getElementById(
            "bancoDocumentoDescarga"
        )
        ?.classList.remove(
            "is-invalid"
        );
}

function limpiarModalSeleccionarBanco() {
    documentoIdDescarga = 0;
    nombreDocumentoDescarga = "";

    const inputDocumento =
        document.getElementById(
            "documentoIdDescarga"
        );

    if (inputDocumento) {
        inputDocumento.value = "0";
    }

    const selectorBanco =
        document.getElementById(
            "bancoDocumentoDescarga"
        );

    if (selectorBanco) {
        selectorBanco.value = "";
    }

    establecerTexto(
        "nombreDocumentoDescarga",
        "Documento"
    );

    limpiarErrorBancoDescarga();
}


function cerrarResultadoAnterior() {
    const modalResultadoElemento =
        document.getElementById(
            "modalResultado"
        );

    if (
        modalResultadoElemento &&
        modalResultadoElemento.classList.contains(
            "show"
        )
    ) {
        modalResultado?.hide();
    }
}

function confirmarEliminarArchivoDocumento(
    tipoDocumentoId
) {
    const documento =
        obtenerDocumentoPorTipo(
            tipoDocumentoId
        );

    if (!documento) {
        mostrarResultado(
            "No se encontró el documento seleccionado.",
            "error"
        );

        return;
    }

    const archivoActual =
        obtenerArchivoActualDocumento(
            documento
        );

    if (!archivoActual) {
        mostrarResultado(
            "El documento no contiene un archivo disponible.",
            "error"
        );

        return;
    }

    if (!modalEliminarDocumento) {
        mostrarResultado(
            "No se encontró el formulario de confirmación.",
            "error"
        );

        return;
    }

    documentoIdEliminar =
        Number(archivoActual.id);

    nombreDocumentoEliminar =
        archivoActual.nombreOriginal ??
        documento.nombre ??
        "archivo seleccionado";

    establecerTexto(
        "nombreDocumentoEliminar",
        nombreDocumentoEliminar
    );

    modalEliminarDocumento.show();
}

async function eliminarArchivoDocumento() {
    if (
        !Number.isInteger(documentoIdEliminar) ||
        documentoIdEliminar <= 0
    ) {
        return;
    }

    const boton = document.getElementById(
        "btnConfirmarEliminarDocumento"
    );

    if (!boton) {
        return;
    }

    const htmlOriginal = boton.innerHTML;

    boton.disabled = true;

    boton.innerHTML =
        '<i class="fa-solid fa-spinner fa-spin me-1"></i>' +
        " Eliminando...";

    try {
        const resultado = await enviarJson(
            "EliminarDocumento",
            {
                id: documentoIdEliminar
            }
        );

        if (!resultado?.success) {
            modalEliminarDocumento?.hide();

            mostrarResultado(
                resultado?.message ??
                "No fue posible eliminar el archivo.",
                "error"
            );

            return;
        }

        modalEliminarDocumento?.hide();

        documentoIdEliminar = 0;
        nombreDocumentoEliminar = "";

        /*
         * Recargamos matriz, contadores y estados.
         */
        await cargarDocumentosEmpresa();

        mostrarResultado(
            resultado.message ??
            "El archivo se eliminó correctamente.",
            "success"
        );
    } catch (error) {
        console.error(
            "Error al eliminar el documento:",
            error
        );

        modalEliminarDocumento?.hide();

        mostrarResultado(
            "Ocurrió un error al eliminar el archivo.",
            "error"
        );
    } finally {
        boton.disabled = false;
        boton.innerHTML = htmlOriginal;
    }
}

function prepararCargaDocumento(
    tipoDocumentoId
) {
    const documento =
        documentosEmpresaActuales.find(
            function (item) {
                return item.id === tipoDocumentoId;
            }
        );

    if (!documento) {
        mostrarResultado(
            "No se encontró el tipo de documento.",
            "error"
        );

        return;
    }

    if (!modalCargarDocumento) {
        mostrarResultado(
            "No se encontró el formulario de carga.",
            "error"
        );

        return;
    }

    if (empresaIdDocumentos <= 0) {
        mostrarResultado(
            "No se identificó la empresa del expediente.",
            "error"
        );

        return;
    }

    tipoDocumentoSeleccionado = documento;

    const formulario = document.getElementById(
        "formCargarDocumento"
    );

    formulario?.reset();

    const empresaInput = document.getElementById(
        "documentoCargaEmpresaId"
    );

    const tipoInput = document.getElementById(
        "documentoCargaTipoId"
    );

    if (empresaInput) {
        empresaInput.value =
            empresaIdDocumentos.toString();
    }

    if (tipoInput) {
        tipoInput.value =
            documento.id.toString();
    }

    establecerTexto(
        "nombreTipoDocumentoCarga",
        documento.nombre
    );

    const contenedorVencimiento =
        document.getElementById(
            "contenedorFechaVencimientoDocumento"
        );

    const fechaVencimiento =
        document.getElementById(
            "documentoCargaFechaVencimiento"
        );

    /*
     * La fecha de vencimiento debe mostrarse
     * y ser obligatoria para todos los documentos.
     */
    contenedorVencimiento?.classList.remove(
        "d-none"
    );

    fechaVencimiento?.setAttribute(
        "required",
        "required"
    );

    if (fechaVencimiento) {
        fechaVencimiento.value = "";

        const hoy = new Date();

        const anio = hoy.getFullYear();
        const mes = String(
            hoy.getMonth() + 1
        ).padStart(2, "0");

        const dia = String(
            hoy.getDate()
        ).padStart(2, "0");

        fechaVencimiento.min =
            `${anio}-${mes}-${dia}`;
    }

    limpiarErroresDocumento();

    modalDocumentos?.hide();

    setTimeout(function () {
        modalCargarDocumento?.show();
    }, 250);
}

async function guardarDocumento() {
    limpiarErroresDocumento();

    const archivoInput = document.getElementById(
        "documentoCargaArchivo"
    );

    const empresaInput = document.getElementById(
        "documentoCargaEmpresaId"
    );

    const tipoInput = document.getElementById(
        "documentoCargaTipoId"
    );

    const fechaInput = document.getElementById(
        "documentoCargaFechaVencimiento"
    );

    const observacionesInput =
        document.getElementById(
            "documentoCargaObservaciones"
        );

    const archivo = archivoInput?.files?.[0];

    const empresaId = parseInt(
        empresaInput?.value || "0",
        10
    );

    const tipoDocumentoId = parseInt(
        tipoInput?.value || "0",
        10
    );

    if (empresaId <= 0) {
        mostrarResultado(
            "No se identificó la empresa.",
            "error"
        );

        return;
    }

    if (tipoDocumentoId <= 0) {
        mostrarResultado(
            "No se identificó el tipo de documento.",
            "error"
        );

        return;
    }

    if (!archivo) {
        mostrarErrorCampoDocumento(
            "Archivo",
            "Selecciona un archivo."
        );

        return;
    }

    const tamanoMaximo =
        100 * 1024 * 1024;

    if (archivo.size > tamanoMaximo) {
        mostrarErrorCampoDocumento(
            "Archivo",
            "El archivo no puede superar los 100 MB."
        );

        return;
    }

    if (!fechaInput?.value) {
        mostrarErrorCampoDocumento(
            "FechaVencimiento",
            "La fecha de vencimiento es obligatoria."
        );

        fechaInput?.focus();

        return;
    }

    const formData = new FormData();

    formData.append(
        "EmpresaId",
        empresaId.toString()
    );

    formData.append(
        "TipoDocumentoId",
        tipoDocumentoId.toString()
    );

    formData.append(
        "Archivo",
        archivo,
        archivo.name
    );

    if (fechaInput?.value) {
        formData.append(
            "FechaVencimiento",
            fechaInput.value
        );
    }

    formData.append(
        "Observaciones",
        observacionesInput?.value?.trim() ?? ""
    );

    const token = document.querySelector(
        '#formToken input[name="__RequestVerificationToken"]'
    )?.value;

    const boton = document.getElementById(
        "btnGuardarDocumento"
    );

    if (!boton) {
        return;
    }

    const htmlOriginal = boton.innerHTML;

    boton.disabled = true;

    boton.innerHTML =
        '<i class="fa-solid fa-spinner fa-spin me-1"></i>' +
        " Cargando...";

    try {
        const url =
            `${window.location.pathname}` +
            `?handler=CargarDocumento`;

        console.log(
            "Enviando documento:",
            {
                url,
                empresaId,
                tipoDocumentoId,
                archivo: archivo.name,
                tamano: archivo.size
            }
        );

        const response = await fetch(
            url,
            {
                method: "POST",
                headers: {
                    "RequestVerificationToken":
                        token ?? ""
                },
                body: formData,
                credentials: "same-origin"
            }
        );

        const contenidoRespuesta =
            await response.text();

        console.log(
            "Estatus del servidor:",
            response.status
        );

        console.log(
            "Respuesta del servidor:",
            contenidoRespuesta
        );

        let resultado = null;

        if (contenidoRespuesta) {
            try {
                resultado = JSON.parse(
                    contenidoRespuesta
                );
            } catch {
                throw new Error(
                    "El servidor devolvió una respuesta no válida."
                );
            }
        }

        if (!response.ok) {
            throw new Error(
                resultado?.message ??
                `Error HTTP ${response.status}.`
            );
        }

        if (!resultado?.success) {
            mostrarErroresDocumento(
                resultado?.errors
            );

            if (!resultado?.errors) {
                mostrarResultado(
                    resultado?.message ??
                    "No fue posible cargar el documento.",
                    "error"
                );
            }

            return;
        }

        /*
         * Restablecemos el ID porque el evento de cierre
         * del modal anterior pudo haberlo modificado.
         */
        empresaIdDocumentos = empresaId;

        modalCargarDocumento?.hide();

        await cargarDocumentosEmpresa();

        setTimeout(function () {
            modalDocumentos?.show();
        }, 250);

        mostrarResultado(
            resultado.message ??
            "El documento se cargó correctamente.",
            "success"
        );
    } catch (error) {
        console.error(
            "Error completo al cargar documento:",
            error
        );

        const mensaje =
            error instanceof TypeError &&
                error.message === "Failed to fetch"
                ? "No fue posible conectarse con el servidor. " +
                "Verifica que la aplicación siga ejecutándose."
                : error?.message ??
                "Ocurrió un error al cargar el documento.";

        mostrarResultado(
            mensaje,
            "error"
        );
    } finally {
        boton.disabled = false;
        boton.innerHTML = htmlOriginal;
    }
}

function mostrarErroresDocumento(errors) {
    if (!errors) {
        return;
    }

    Object.entries(errors).forEach(
        function ([campo, mensajes]) {
            mostrarErrorCampoDocumento(
                campo,
                mensajes?.[0] ?? ""
            );
        }
    );
}

function mostrarErrorCampoDocumento(
    campo,
    mensaje
) {
    const elemento = document.querySelector(
        `[data-documento-error-for="${campo}"]`
    );

    if (elemento) {
        elemento.textContent = mensaje;
    }
}

function limpiarErroresDocumento() {
    document
        .querySelectorAll(
            "[data-documento-error-for]"
        )
        .forEach(function (elemento) {
            elemento.textContent = "";
        });
}

function descargarDocumento(
    tipoDocumentoId
) {
    const documento =
        obtenerDocumentoPorTipo(
            tipoDocumentoId
        );

    if (!documento) {
        mostrarResultado(
            "No se encontró el documento seleccionado.",
            "error"
        );

        return;
    }

    const archivoActual =
        obtenerArchivoActualDocumento(
            documento
        );

    if (!archivoActual) {
        mostrarResultado(
            "El documento todavía no contiene archivos.",
            "error"
        );

        return;
    }

    descargarArchivoDocumento(
        archivoActual.id,
        archivoActual.nombreOriginal ??
        documento.nombre ??
        "Documento"
    );
}

function actualizarResumenDocumentos(documentos) {
    const total = documentos.length;

    const cargados = documentos.filter(
        function (documento) {
            return documento.totalArchivos > 0;
        }
    ).length;

    const pendientes = documentos.filter(
        function (documento) {
            return documento.totalArchivos === 0;
        }
    ).length;

    const vencidos = documentos.filter(
        function (documento) {
            return documento.estatus === "Vencido";
        }
    ).length;

    establecerTexto(
        "totalDocumentosRequeridos",
        total
    );

    establecerTexto(
        "totalDocumentosCargados",
        cargados
    );

    establecerTexto(
        "totalDocumentosPendientes",
        pendientes
    );

    establecerTexto(
        "totalDocumentosVencidos",
        vencidos
    );
}

function obtenerClaseEstatusDocumento(estatus) {
    switch (estatus) {
        case "Vigente":
            return "eb-document-status-valid";

        case "Próximo a vencer":
            return "eb-document-status-warning";

        case "Vencido":
            return "eb-document-status-expired";

        case "Cargado":
            return "eb-document-status-loaded";

        default:
            return "eb-document-status-pending";
    }
}

function mostrarErrorDocumentos(mensaje) {
    document.getElementById(
        "documentosCargando"
    )?.classList.add("d-none");

    const alerta = document.getElementById(
        "documentosError"
    );

    const texto = document.getElementById(
        "documentosErrorMensaje"
    );

    if (texto) {
        texto.textContent = mensaje;
    }

    alerta?.classList.remove("d-none");
}