$(document).ready(function () {
    cargarCorreosDominios();

    $("#btnBuscarCorreoDominio").on("click", function () {
        cargarCorreosDominios();
    });

    $("#btnLimpiarCorreoDominio").on("click", function () {
        $("#txtFiltroCorreo").val("");
        $("#txtFiltroDominio").val("");
        $("#txtFiltroResponsable").val("");

        cargarCorreosDominios();
    });

    $("#btnNuevoCorreoDominio").on("click", function () {
        limpiarModalCorreoDominio();
        $("#dlgCorreoDominio").modal("show");
    });

    $("#btnGuardarCorreoDominio").on("click", function () {
        guardarCorreoDominio();
    });

    $("#btnImportarCorreosDominios").on("click", function () {
        importarCorreosDominios();
    });
});

function cargarCorreosDominios() {
    $("#tableCorreosDominios").bootstrapTable("destroy");

    $("#tableCorreosDominios").bootstrapTable({
        url: "?handler=CorreosDominiosList",
        method: "get",
        pagination: true,
        search: true,
        pageSize: 10,
        queryParams: function () {
            return {
                correo: $("#txtFiltroCorreo").val(),
                dominio: $("#txtFiltroDominio").val(),
                responsable: $("#txtFiltroResponsable").val()
            };
        }
    });
}

function accionesCorreoDominioFormatter(value, row) {
    return `
        <button type="button" class="btn btn-sm btn-warning me-1" onclick='editarCorreoDominio(${JSON.stringify(row)})'>
            <i class="bi bi-pencil-square"></i>
        </button>

        <button type="button" class="btn btn-sm btn-danger" onclick="eliminarCorreoDominio(${row.id})">
            <i class="bi bi-trash"></i>
        </button>
    `;
}

function limpiarModalCorreoDominio() {
    $("#inpCorreoDominioId").val(0);
    $("#inpCorreo").val("");
    $("#inpDominio").val("");
    $("#inpDescripcion").val("");
    $("#inpResponsable").val("");
    $("#inpObservaciones").val("");
}

function editarCorreoDominio(row) {
    $("#inpCorreoDominioId").val(row.id);
    $("#inpCorreo").val(row.correo || "");
    $("#inpDominio").val(row.dominio || "");
    $("#inpDescripcion").val(row.descripcion || "");
    $("#inpResponsable").val(row.responsable || "");
    $("#inpObservaciones").val(row.observaciones || "");

    $("#dlgCorreoDominio").modal("show");
}

function guardarCorreoDominio() {
    const data = {
        id: parseInt($("#inpCorreoDominioId").val()) || 0,
        correo: $("#inpCorreo").val(),
        dominio: $("#inpDominio").val(),
        descripcion: $("#inpDescripcion").val(),
        responsable: $("#inpResponsable").val(),
        observaciones: $("#inpObservaciones").val()
    };

    $.ajax({
        url: "?handler=SaveCorreoDominio",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(data),
        headers: {
            "RequestVerificationToken": $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        success: function (resp) {
            if (resp.success) {
                $("#dlgCorreoDominio").modal("hide");
                cargarCorreosDominios();
                alert(resp.message || "Registro guardado correctamente.");
            } else {
                alert(resp.message || "No se pudo guardar el registro.");
            }
        },
        error: function () {
            alert("Ocurrió un error al guardar el registro.");
        }
    });
}

function eliminarCorreoDominio(id) {
    if (!confirm("¿Desea eliminar este registro?")) return;

    $.ajax({
        url: "?handler=DeleteCorreoDominio",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(id),
        headers: {
            "RequestVerificationToken": $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        success: function (resp) {
            if (resp.success) {
                cargarCorreosDominios();
                alert(resp.message || "Registro eliminado correctamente.");
            } else {
                alert(resp.message || "No se pudo eliminar el registro.");
            }
        },
        error: function () {
            alert("Ocurrió un error al eliminar el registro.");
        }
    });
}

function importarCorreosDominios() {
    const archivo = $("#inpExcelCorreosDominios")[0].files[0];

    if (!archivo) {
        alert("Seleccione un archivo Excel.");
        return;
    }

    const formData = new FormData();
    formData.append("archivo", archivo);

    $.ajax({
        url: "?handler=ImportarCorreosDominios",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        headers: {
            "RequestVerificationToken": $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        success: function (resp) {
            if (resp.success) {
                $("#inpExcelCorreosDominios").val("");
                cargarCorreosDominios();
                alert(resp.message || "Importación realizada correctamente.");
            } else {
                alert(resp.message || "No se pudo importar el archivo.");
            }
        },
        error: function () {
            alert("Ocurrió un error al importar el archivo.");
        }
    });
}