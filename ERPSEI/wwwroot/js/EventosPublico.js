let eventoModalInstance = null;

document.addEventListener("DOMContentLoaded", function () {
    const modalElement = document.getElementById("eventoModal");
    if (modalElement) {
        eventoModalInstance = new bootstrap.Modal(modalElement);
    }
});

function verEvento(id) {
    const baseUrl = window.eventosConfig.detalleUrl;
    const separator = baseUrl.includes("?") ? "&" : "?";
    const url = `${baseUrl}${separator}id=${id}`;

    fetch(url)
        .then(resp => {
            if (!resp.ok) {
                throw new Error(`No se pudo obtener el evento. Status: ${resp.status}`);
            }
            return resp.json();
        })
        .then(resp => {
            if (resp.tieneError) {
                throw new Error(resp.mensaje || "No se encontró el evento.");
            }

            document.getElementById("eventoModalTitle").textContent = resp.titulo ?? "";
            document.getElementById("eventoModalDescripcion").textContent = resp.descripcion ?? "";
            document.getElementById("eventoModalImg").src = resp.rutaPortada ?? "";
            
            eventoModalInstance.show();
        })
        .catch(error => {
            console.error("Error al abrir el evento:", error);
        });
}

const openId = window.eventosConfig.openId;
if (openId) {
    setTimeout(() => {
        verEvento(openId);
    }, 500);
}
