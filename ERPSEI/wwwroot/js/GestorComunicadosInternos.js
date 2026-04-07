let resultadoModal = null;

document.addEventListener("DOMContentLoaded", function () {
    limpiarFormularioComunicado();

    const modalElement = document.getElementById("resultadoModal");
    if (modalElement) {
        resultadoModal = new bootstrap.Modal(modalElement);
    }
});

function limpiarFormularioComunicado() {
    document.getElementById("formComunicado").reset();
    document.getElementById("Input_Id").value = 0;
    document.getElementById("Input_FechaPublicacion").value = obtenerFechaHoy();
    document.getElementById("Input_HoraPublicacion").value = obtenerHoraActual();
    document.getElementById("Input_Activo").value = "true";
    document.getElementById("Input_EsPermanente").checked = false;
    document.getElementById("previewPortada").src = "";
    document.getElementById("previewPortada").style.display = "none";
    document.getElementById("previewPortadaTexto").style.display = "block";

    const archivoContainer = document.getElementById("archivoActualContainer");
    const archivoLink = document.getElementById("archivoActualLink");

    archivoContainer.style.display = "none";
    archivoLink.href = "#";
}

function obtenerFechaHoy() {
    const fecha = new Date();
    const anio = fecha.getFullYear();
    const mes = String(fecha.getMonth() + 1).padStart(2, "0");
    const dia = String(fecha.getDate()).padStart(2, "0");
    return `${anio}-${mes}-${dia}`;
}

function obtenerHoraActual() {
    const fecha = new Date();
    const horas = String(fecha.getHours()).padStart(2, "0");
    const minutos = String(fecha.getMinutes()).padStart(2, "0");
    return `${horas}:${minutos}`;
}

function mostrarResultado(mensaje) {
    document.getElementById("resultadoMensaje").textContent = mensaje;
    resultadoModal.show();
}

function accionesFormatter(value, row) {
    let botones = `
        <div class="d-flex gap-2">
            <button class="btn btn-sm btn-outline-primary" onclick="editarComunicado(${row.id})">
                Editar
            </button>
            <button class="btn btn-sm ${row.activo ? 'btn-outline-warning' : 'btn-outline-success'}" onclick="toggleComunicado(${row.id})">
                ${row.activo ? 'Desactivar' : 'Activar'}
            </button>
    `;

    if (!row.publicado) {
        botones += `
            <button class="btn btn-sm btn-outline-info" onclick="publicarComunicado(${row.id})">
                Publicar
            </button>
        `;
    }

    botones += `
            <button class="btn btn-sm btn-outline-danger btn-delete-icon" onclick="eliminarComunicado(${row.id})" title="Eliminar">
                <i class="bi bi-trash"></i>
            </button>
        </div>
    `;

    return botones;
}

function estatusFormatter(value, row) {
    return row.activo
        ? `<span class="badge bg-success">Activo</span>`
        : `<span class="badge bg-secondary">Inactivo</span>`;
}

function publicacionFormatter(value, row) {
    return row.publicado
        ? `<span class="badge bg-primary">Publicado</span>`
        : `<span class="badge bg-warning text-dark">Borrador</span>`;
}

function permanenteFormatter(value, row) {
    return row.esPermanente
        ? `<span class="badge bg-info text-dark">Sí</span>`
        : `<span class="badge bg-light text-dark border">No</span>`;
}

function archivoFormatter(value, row) {
    if (!row.rutaArchivo) return "";
    return `<a href="${row.rutaArchivo}" target="_blank" class="btn btn-sm btn-outline-dark">Ver</a>`;
}

function editarComunicado(id) {
    $.get(`/Catalogos/GestorComunicadosInternos/ComunicadoById?id=${id}`, function (resp) {
        if (resp.tieneError) {
            mostrarResultado(resp.mensaje);
            return;
        }

        document.getElementById("Input_Id").value = resp.id;
        document.getElementById("Input_Titulo").value = resp.titulo ?? "";
        document.getElementById("Input_Descripcion").value = resp.descripcion ?? "";
        document.getElementById("Input_FechaPublicacion").value = resp.fechaPublicacion ?? "";
        document.getElementById("Input_HoraPublicacion").value = resp.horaPublicacion ?? "";
        document.getElementById("Input_Activo").value = resp.activo ? "true" : "false";
        document.getElementById("Input_EsPermanente").checked = resp.esPermanente === true;

        if (resp.rutaArchivo) {
            const archivoContainer = document.getElementById("archivoActualContainer");
            const archivoLink = document.getElementById("archivoActualLink");
            archivoContainer.style.display = "block";
            archivoLink.href = resp.rutaArchivo;
        }

        if (resp.rutaPortada) {
            document.getElementById("previewPortada").src = resp.rutaPortada;
            document.getElementById("previewPortada").style.display = "block";
            document.getElementById("previewPortadaTexto").style.display = "none";
        } else {
            document.getElementById("previewPortada").src = "";
            document.getElementById("previewPortada").style.display = "none";
            document.getElementById("previewPortadaTexto").style.display = "block";
        }

        window.scrollTo({ top: 0, behavior: "smooth" });
    });
}

function guardarComunicado(publicar) {
    const form = document.getElementById("formComunicado");
    const formData = new FormData(form);

    if (!document.getElementById("Input_EsPermanente").checked) {
        formData.set("Input.EsPermanente", "false");
    }

    $.ajax({
        url: `/Catalogos/GestorComunicadosInternos/SaveComunicado?publicar=${publicar}`,
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        headers: {
            "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        success: function (resp) {
            mostrarResultado(resp.mensaje);

            if (!resp.tieneError) {
                limpiarFormularioComunicado();
                $("#tableComunicados").bootstrapTable("refresh");
            }
        },
        error: function () {
            mostrarResultado("Ocurrió un error al guardar el comunicado.");
        }
    });
}

function toggleComunicado(id) {
    $.ajax({
        url: `/Catalogos/GestorComunicadosInternos/ToggleActivo?id=${id}`,
        type: "POST",
        headers: {
            "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        success: function (resp) {
            mostrarResultado(resp.mensaje);
            if (!resp.tieneError) {
                $("#tableComunicados").bootstrapTable("refresh");
            }
        }
    });
}

function publicarComunicado(id) {
    $.ajax({
        url: `/Catalogos/GestorComunicadosInternos/Publicar?id=${id}`,
        type: "POST",
        headers: {
            "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        success: function (resp) {
            mostrarResultado(resp.mensaje);
            if (!resp.tieneError) {
                $("#tableComunicados").bootstrapTable("refresh");
            }
        }
    });
}

function previewPortada(input) {
    const preview = document.getElementById("previewPortada");
    const texto = document.getElementById("previewPortadaTexto");

    if (input.files && input.files[0]) {
        const reader = new FileReader();

        reader.onload = function (e) {
            preview.src = e.target.result;
            preview.style.display = "block";
            texto.style.display = "none";
        };

        reader.readAsDataURL(input.files[0]);
    } else {
        preview.src = "";
        preview.style.display = "none";
        texto.style.display = "block";
    }
}

function eliminarComunicado(id) {
    $.ajax({
        url: `/Catalogos/GestorComunicadosInternos/DeleteComunicado?id=${id}`,
        type: "POST",
        headers: {
            "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        success: function (resp) {
            mostrarResultado(resp.mensaje);
            if (!resp.tieneError) {
                limpiarFormularioComunicado();
                $("#tableComunicados").bootstrapTable("refresh");
            }
        }
    });
}