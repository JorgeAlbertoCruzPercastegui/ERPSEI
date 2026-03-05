document.addEventListener("DOMContentLoaded", () => {
    const dropZone = document.getElementById("dropZone");
    const fileInput = document.getElementById("fileBanner");
    const previewImg = document.getElementById("previewImg");
    const dropHint = document.getElementById("dropHint");
    const lblFileInfo = document.getElementById("lblFileInfo");

    const btnLimpiar = document.getElementById("btnLimpiar");

    function setPreview(file) {
        if (!file) return;

        lblFileInfo.textContent = `${file.name} · ${(file.size / 1024).toFixed(1)} KB`;

        const reader = new FileReader();
        reader.onload = (e) => {
            previewImg.src = e.target.result;
            previewImg.style.display = "block";
            dropHint.style.display = "none";
        };
        reader.readAsDataURL(file);
    }

    function clearAll() {
        fileInput.value = "";
        previewImg.src = "";
        previewImg.style.display = "none";
        dropHint.style.display = "block";
        lblFileInfo.textContent = "Ningún archivo seleccionado";

        document.getElementById("inpTitulo").value = "";
        document.getElementById("inpDescripcion").value = "";
        document.getElementById("inpInicio").value = "";
        document.getElementById("inpFin").value = "";
        document.getElementById("chkFijo").checked = false;
    }

    // Click abre selector
    dropZone.addEventListener("click", () => fileInput.click());

    // Cambio de archivo
    fileInput.addEventListener("change", () => {
        const file = fileInput.files && fileInput.files[0];
        if (file) setPreview(file);
    });

    // Drag & Drop
    ["dragenter", "dragover"].forEach(evt => {
        dropZone.addEventListener(evt, (e) => {
            e.preventDefault();
            e.stopPropagation();
            dropZone.classList.add("border-primary");
        });
    });

    ["dragleave", "drop"].forEach(evt => {
        dropZone.addEventListener(evt, (e) => {
            e.preventDefault();
            e.stopPropagation();
            dropZone.classList.remove("border-primary");
        });
    });

    dropZone.addEventListener("drop", (e) => {
        const file = e.dataTransfer.files && e.dataTransfer.files[0];
        if (!file) return;

        // solo imágenes
        if (!file.type.startsWith("image/")) {
            alert("Solo se permiten imágenes.");
            return;
        }

        // asigna al input para que luego se pueda enviar
        const dt = new DataTransfer();
        dt.items.add(file);
        fileInput.files = dt.files;

        setPreview(file);
    });

    btnLimpiar.addEventListener("click", clearAll);
});