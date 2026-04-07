let resultadoEventoModal = null;

document.addEventListener("DOMContentLoaded", function () {
    limpiarFormularioEvento();

    const modalElement = document.getElementById("resultadoEventoModal");
    if (modalElement) {
        resultadoEventoModal = new bootstrap.Modal(modalElement);
    }
});

function mostrarResultadoEvento(mensaje) {
    const el = document.getElementById("resultadoEventoMensaje");
    if (el && resultadoEventoModal) {
        el.textContent = mensaje;
        resultadoEventoModal.show();
    }
}

function limpiarFormularioEvento() {
    const form = document.getElementById("formEvento");
    if (form) form.reset();

    const id = document.getElementById("Input_Id");
    if (id) id.value = 0;

    const fecha = document.getElementById("Input_FechaEvento");
    if (fecha) fecha.value = obtenerFechaHoy();

    const preview = document.getElementById("previewPortadaEvento");
    const texto = document.getElementById("previewPortadaEventoTexto");

    if (preview) {
        preview.src = "";
        preview.style.display = "none";
    }

    if (texto) {
        texto.style.display = "block";
    }
}

function obtenerFechaHoy() {
    const fecha = new Date();
    const anio = fecha.getFullYear();
    const mes = String(fecha.getMonth() + 1).padStart(2, "0");
    const dia = String(fecha.getDate()).padStart(2, "0");
    return `${anio}-${mes}-${dia}`;
}

function previewPortadaEvento(input) {
    const preview = document.getElementById("previewPortadaEvento");
    const texto = document.getElementById("previewPortadaEventoTexto");

    if (!preview || !texto) return;

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

function guardarEvento(publicar) {
    const form = document.getElementById("formEvento");
    const formData = new FormData(form);

    if (!document.getElementById("Input_RequiereGeolocalizacion").checked) {
        formData.set("Input.RequiereGeolocalizacion", "false");
    }

    $.ajax({
        url: `/Catalogos/GestorEventos/SaveEvento?publicar=${publicar}`,
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        headers: {
            "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        success: function (resp) {
            mostrarResultadoEvento(resp.mensaje);
            if (!resp.tieneError) {
                limpiarFormularioEvento();
                $("#tableEventos").bootstrapTable("refresh");
            }
        },
        error: function () {
            mostrarResultadoEvento("Ocurrió un error al guardar el evento.");
        }
    });
}

function editarEvento(id) {
    $.get(`/Catalogos/GestorEventos/EventoById?id=${id}`, function (resp) {
        if (resp.tieneError) {
            mostrarResultadoEvento(resp.mensaje);
            return;
        }

        document.getElementById("Input_Id").value = resp.id;
        document.getElementById("Input_Titulo").value = resp.titulo ?? "";
        document.getElementById("Input_Descripcion").value = resp.descripcion ?? "";
        document.getElementById("Input_TipoEvento").value = resp.tipoEvento ?? "";
        document.getElementById("Input_FechaEvento").value = resp.fechaEvento ?? "";
        document.getElementById("Input_HoraEvento").value = resp.horaEvento ?? "";
        document.getElementById("Input_FechaPublicacionProgramada").value = resp.fechaPublicacionProgramada ?? "";
        document.getElementById("Input_RequiereGeolocalizacion").checked = resp.requiereGeolocalizacion === true;
        document.getElementById("Input_Region").value = resp.region ?? "";
        document.getElementById("Input_UrlFormulario").value = resp.urlFormulario ?? "";
        //document.getElementById("Input_TextoBoton").value = resp.textoBoton ?? "Consulta aquí";
        document.getElementById("Input_Activo").value = resp.activo ? "true" : "false";

        if (resp.rutaPortada) {
            document.getElementById("previewPortadaEvento").src = resp.rutaPortada;
            document.getElementById("previewPortadaEvento").style.display = "block";
            document.getElementById("previewPortadaEventoTexto").style.display = "none";
        } else {
            document.getElementById("previewPortadaEvento").src = "";
            document.getElementById("previewPortadaEvento").style.display = "none";
            document.getElementById("previewPortadaEventoTexto").style.display = "block";
        }

        window.scrollTo({ top: 0, behavior: "smooth" });
    });
}

function publicarEvento(id) {
    $.ajax({
        url: `/Catalogos/GestorEventos/Publicar?id=${id}`,
        type: "POST",
        headers: {
            "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        success: function (resp) {
            mostrarResultadoEvento(resp.mensaje);
            if (!resp.tieneError) {
                $("#tableEventos").bootstrapTable("refresh");
            }
        }
    });
}

function toggleEvento(id) {
    $.ajax({
        url: `/Catalogos/GestorEventos/ToggleActivo?id=${id}`,
        type: "POST",
        headers: {
            "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        success: function (resp) {
            mostrarResultadoEvento(resp.mensaje);
            if (!resp.tieneError) {
                $("#tableEventos").bootstrapTable("refresh");
            }
        }
    });
}

function eliminarEvento(id) {
    $.ajax({
        url: `/Catalogos/GestorEventos/DeleteEvento?id=${id}`,
        type: "POST",
        headers: {
            "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        success: function (resp) {
            mostrarResultadoEvento(resp.mensaje);
            if (!resp.tieneError) {
                limpiarFormularioEvento();
                $("#tableEventos").bootstrapTable("refresh");
            }
        }
    });
}

function accionesEventoFormatter(value, row) {
    let botones = `
        <div class="d-flex gap-2">
            <button class="btn btn-sm btn-outline-primary" onclick="editarEvento(${row.id})">Editar</button>
            <button class="btn btn-sm ${row.activo ? 'btn-outline-warning' : 'btn-outline-success'}" onclick="toggleEvento(${row.id})">
                ${row.activo ? 'Desactivar' : 'Activar'}
            </button>
    `;

    if (!row.publicado) {
        botones += `<button class="btn btn-sm btn-outline-info" onclick="publicarEvento(${row.id})">Publicar</button>`;
    }

    botones += `
            <button class="btn btn-sm btn-outline-danger btn-delete-icon" onclick="eliminarEvento(${row.id})" title="Eliminar">
                <i class="bi bi-trash"></i>
            </button>
        </div>
    `;
    return botones;
}

function estatusEventoFormatter(value, row) {
    return row.activo
        ? `<span class="badge bg-success">Activo</span>`
        : `<span class="badge bg-secondary">Inactivo</span>`;
}

function publicacionEventoFormatter(value, row) {
    return row.publicado
        ? `<span class="badge bg-primary">Publicado</span>`
        : `<span class="badge bg-warning text-dark">Pendiente</span>`;
}

function vistaEventoFormatter(value, row) {
    if (!row.rutaPortada) return "";
    return `<a href="${row.rutaPortada}" target="_blank" class="btn btn-sm btn-outline-dark">Ver</a>`;
}