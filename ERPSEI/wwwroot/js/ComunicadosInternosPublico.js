let comunicadoPublicModal = null;

document.addEventListener("DOMContentLoaded", function () {
    const modalElement = document.getElementById("comunicadoModal");
    if (modalElement) {
        comunicadoPublicModal = new bootstrap.Modal(modalElement);
    }

    const selMes = document.getElementById("selMes");
    if (selMes) {
        selMes.value = window.comunicadosPublicConfig.mesActual || "";

        selMes.addEventListener("change", function () {
            cargarComunicados(this.value);
        });
    }
});

function cargarComunicados(mes) {
    const grid = document.getElementById("comunicadosGrid");
    if (!grid) return;

    grid.innerHTML = `<div class="empty-state">Cargando comunicados...</div>`;

    const baseUrl = window.comunicadosPublicConfig.listaUrl;
    const separator = baseUrl.includes("?") ? "&" : "?";
    const url = mes ? `${baseUrl}${separator}mes=${mes}` : baseUrl;

    fetch(url)
        .then(resp => {
            if (!resp.ok) {
                throw new Error("No se pudieron cargar los comunicados.");
            }
            return resp.json();
        })
        .then(data => {
            if (!data || data.length === 0) {
                grid.innerHTML = `<div class="empty-state">No hay comunicados disponibles para el filtro seleccionado.</div>`;
                return;
            }

            let html = "";

            data.forEach(item => {
                html += `
                    <div class="comunicado-card moderno">
                        <div class="comunicado-cover">
                            <img src="${item.rutaPortada}" alt="${escapeHtml(item.titulo)}" />
                        </div>

                        <div class="comunicado-card-body">
                            <h3 class="comunicado-title">${escapeHtml(item.titulo)}</h3>

                            ${item.fechaPublicacion}

                            <div class="comunicado-actions">
                                <span class="comunicado-chip">
                                    ${item.esPdf ? "Manual" : "Info"}
                                </span>

                                <button type="button" class="btn-consultar" onclick="verComunicado(${item.id})">
                                    Consultar aquí
                                </button>
                            </div>
                        </div>
                    </div>
                `;
            });

            grid.innerHTML = html;
        })
        .catch(error => {
            console.error(error);
            grid.innerHTML = `<div class="empty-state">Ocurrió un error al cargar los comunicados.</div>`;
        });
}

function verComunicado(id) {
    const baseUrl = window.comunicadosPublicConfig.detalleUrl;
    const separator = baseUrl.includes("?") ? "&" : "?";

    fetch(`${baseUrl}${separator}id=${id}`)
        .then(resp => {
            if (!resp.ok) {
                throw new Error("No se pudo obtener el comunicado.");
            }
            return resp.json();
        })
        .then(resp => {
            if (resp.tieneError) {
                throw new Error(resp.mensaje || "No se encontró el comunicado.");
            }

            document.getElementById("comunicadoModalTitle").textContent = resp.titulo;
            document.getElementById("comunicadoModalFecha").textContent =
                `Publicado el ${resp.fechaPublicacion}${resp.horaPublicacion ? " · " + resp.horaPublicacion : ""}`;

            const viewer = document.getElementById("comunicadoModalViewer");

            if (resp.esPdf) {
                viewer.innerHTML = `
                    <iframe src="${resp.rutaArchivo}" class="comunicado-iframe" frameborder="0"></iframe>
                `;
            } else {
                viewer.innerHTML = `
                    <div class="comunicado-image-wrap">
                        <img src="${resp.rutaArchivo}" alt="${escapeHtml(resp.titulo)}" class="comunicado-full-image" />
                    </div>
                `;
            }

            comunicadoPublicModal.show();
        })
        .catch(error => {
            console.error(error);
        });
}

function escapeHtml(text) {
    if (!text) return "";
    return text
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#039;");
}

const openId = window.comunicadosPublicConfig.openId;
if (openId) {
    setTimeout(() => {
        verComunicado(openId);
    }, 500);
}