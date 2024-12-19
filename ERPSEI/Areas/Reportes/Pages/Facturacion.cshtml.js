var numFormatter = null;
var chartPerfilesBarras = null;
var chartPerfilesPie = null;
var secondChart = null;
var chartsEmpresas = [];

const CHART_TYPE_BAR = "bar";
const CHART_TYPE_PIE = "pie";

let barColors = [
    'rgba(255, 99, 132, 0.5)', //0 - RED
    'rgba(255, 159, 64, 0.5)', //1 -ORANGE
    'rgba(255, 205, 86, 0.5)', //2 -YELLOW
    'rgba(75, 192, 192, 0.5)', //3 -GREEN
    'rgba(54, 162, 235, 0.5)', //4 -BLUE
    'rgba(153, 102, 255, 0.5)', //5 -PURPLE
    'rgba(120, 230, 11, 0.5)', //6 -LIGHT GREEN
    'rgba(201, 203, 207, 0.5)' //7 -GRAY
];
let barBorderColors = [
    'rgb(255, 99, 132)',//0 - RED
    'rgb(255, 159, 64)',//1 -ORANGE
    'rgb(255, 205, 86)',//2 -YELLOW
    'rgb(75, 192, 192)',//3 -GREEN
    'rgb(54, 162, 235)',//4 -BLUE
    'rgb(153, 102, 255)',//5 -PURPLE
    'rgb(120, 230, 11)',//6 -LIGHT GREEN
    'rgb(201, 203, 207)'//7 -GRAY
];

const postOptions = { headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() } }

$(window).on("load", function () {
    
});

document.addEventListener('DOMContentLoaded', function () {
    numFormatter = new Intl.NumberFormat(cultureName);

    jQuery.validator.setDefaults({
        highlight: function (element, errorClass, validClass) {
            $(element).addClass("is-invalid").removeClass("is-valid");
        },
        unhighlight: function (element, errorClass, validClass) {
            if ($(element).hasClass("is-invalid")) {
                $(element).addClass("is-valid").removeClass("is-invalid");
            }
        }
    });

    autoCompletar("#inpFiltroEmpresaRFC", {
        select: function (element, item) {
            if (item) {
                clearGraphics();
                if (item.perfil) {
                    let perfilId = $(`#selFiltroPerfil option:contains(${item.perfil})`).val() || "0";
                    $("#selFiltroPerfil").val(perfilId);
                }

                if (item.nivel) {
                    let nivelId = $(`#selFiltroNivel option:contains(${item.nivel})`).val() || "0";
                    $("#selFiltroNivel").val(nivelId);
                }
            }
        },
        change: function (element, item) {
            if (!item) {
                clearGraphics();
                $('#inpFiltroEmpresaRFC').data({ rfc: null, Perfil: null, Nivel: null });
            }
        }
    });
});

////////////////////////////////
//Funcionalidad Filtrar
////////////////////////////////
//Función para detectar el cambio de valor de los campos perfil y nivel
function onPerfilOrNivelChanged() {
    clearGraphics();
    $("#inpFiltroEmpresaRFC").attr("idselected", "").data('rfc', null).val('');
}
//Función para limpiar la tabla de resultados
function clearGraphics() {
    $("#divCharts").html(`
        <div class="col-12 opacity-25">
		    <img class="indeximage" src="${emptyGraphicsURL}" />
            <div class="col-12 h2">
			    <span>${emptyGrapicsInstructions}</span>
		    </div>
	    </div>`
    );

    if (chartPerfilesBarras) { chartPerfilesBarras.destroy(); }
    if (chartPerfilesPie) { chartPerfilesPie.destroy(); }
    if (secondChart) { secondChart.destroy(); }
}
//Función para convertir una cadena JSON a un objeto JSON
function responseHandler(res) {
    if (typeof res == "string" && res.length >= 1) {
        res = JSON.parse(res);
    }

    return res
}
//Función para crear una gráfica de perfiles tipo Pie
function CreatePerfilesPieChart(labelValues, pueValues, ppdValues, prefacturadoValues, disponibleValues) {
    $("#chartPerfilesPie").parent().show();
    $("#accordionEmpresas").parent().show();

    let data = {
        labels: labelValues,
        datasets: [
            {
                data: [
                    pueValues[0],
                    ppdValues[0],
                    prefacturadoValues[0],
                    disponibleValues[0]
                ],
                backgroundColor: [
                    barColors[4],
                    barColors[5],
                    barColors[2],
                    barColors[3]
                ],
                borderColor: [
                    barBorderColors[4],
                    barBorderColors[5],
                    barBorderColors[2],
                    barBorderColors[3]
                ]
            }
        ]
    };

    let options = {
        plugins: {
            title: {
                display: true,
                text: `Facturación del Perfil ${$("#selFiltroPerfil option:selected").text()}`
            },
            tooltip: {
                position: "nearest",
                callbacks: {
                    label: function (context) {
                        return `$${numFormatter.format(context.raw)}`;
                    }
                }
            }
        }
    };

    let config = {
        type: 'pie',
        data: data,
        options: options
    }

    chartPerfilesPie = new Chart("chartPerfilesPie", config);
}
//Función para crear una gráfica de perfiles tipo barras
function CreatePerfilesBarChart(labelValues, pueValues, ppdValues, prefacturadoValues, disponibleValues, chartType) {
    $("#chartPerfilesBarras").parent().show();

    let datasets = [
        {
            label: 'PUE',
            data: pueValues,
            backgroundColor: barColors[4],
            borderColor: barBorderColors[4],
            borderWidth: 1,
            stack: 'Stack 0'
        },
        {
            label: 'PPD',
            data: ppdValues,
            backgroundColor: barColors[5],
            borderColor: barBorderColors[5],
            borderWidth: 1,
            stack: 'Stack 0'
        },
        {
            label: 'Prefacturado',
            data: prefacturadoValues,
            backgroundColor: barColors[2],
            borderColor: barBorderColors[2],
            borderWidth: 1,
            stack: 'Stack 0'
        },
        {
            label: 'Disponible',
            data: disponibleValues,
            backgroundColor: barColors[3],
            borderColor: barBorderColors[3],
            borderWidth: 1,
            stack: 'Stack 0'
        }
        //{
        //    label: 'Exceso',
        //    data: excesoValues,
        //    backgroundColor: barColors[0],
        //    borderColor: barBorderColors[0],
        //    borderWidth: 1,
        //    stack: 'Stack 0'
        //}
    ];

    let data = {
        labels: labelValues,
        datasets: datasets
    };

    let options = {
        indexAxis: 'y',
        plugins: {
            title: {
                display: true,
                text: 'Facturación por Perfil'
            },
        },
        responsive: true,
        interaction: {
            intersect: false,
        },
        scales: {
            x: {
                stacked: true
                //ticks: {
                //    callback: (value, index, values) => {
                //        return `$ ${numFormatter.format(value)}`;
                //    }
                //}
            },
            y: {
                stacked: true
            }
        }
    };

    let config = {
        type: chartType,
        data: data,
        options: options
    }

    chartPerfilesBarras = new Chart("chartPerfilesBarras", config);
}
//Función para crear una gráfica de empresas tipo barras
function CreateSecondChart(labelValues, pueValues, ppdValues, prefacturadoValues, disponibleValues) {
    $("#chartEmpresas").parent().show();

    let datasets = [
        {
            label: 'PUE',
            data: pueValues,
            backgroundColor: barColors[4],
            borderColor: barBorderColors[4],
            borderWidth: 1
        },
        {
            label: 'PPD',
            data: ppdValues,
            backgroundColor: barColors[5],
            borderColor: barBorderColors[5],
            borderWidth: 1
        },
        {
            label: 'Prefacturado',
            data: prefacturadoValues,
            backgroundColor: barColors[2],
            borderColor: barBorderColors[2],
            borderWidth: 1
        },
        {
            label: 'Disponible',
            data: disponibleValues,
            backgroundColor: barColors[3],
            borderColor: barBorderColors[3],
            borderWidth: 1
        }
    ];

    let data = {
        labels: labelValues,
        datasets: datasets
    };

    let options = {
        indexAxis: 'y',
        plugins: {
            title: {
                display: true,
                text: 'Facturación por Empresa'
            },
        },
        responsive: true,
        scales: {
            x: {
                stacked: true
                //ticks: {
                //    callback: (value, index, values) => {
                //        return `$ ${numFormatter.format(value)}`;
                //    }
                //}
            },
            y: {
                stacked: true
            }
        }
    };

    let config = {
        type: 'bar',
        data: data,
        options: options
    }

    secondChart = new Chart("chartEmpresas", config);
}
//Función para filtrar los datos de la tabla.
function onBuscarClick() {
    clearGraphics();

    let oParams = {
        PerfilId: $("#selFiltroPerfil").val() == 0 ? null : $("#selFiltroPerfil").val(),
        EmpresaRFC: ($("#inpFiltroEmpresaRFC").data("rfc") || "") == "" ? null : $("#inpFiltroEmpresaRFC").data("rfc"),
        NivelId: $("#selFiltroNivel").val() == 0 ? null : $("#selFiltroNivel").val(),
        Anio: $("#selFiltroAnio").val() == 0 ? null : $("#selFiltroAnio").val(),
        Mes: $("#selFiltroMes").val() == 0 ? null : $("#selFiltroMes").val()
    };

    let perfilSelected = parseInt(oParams.PerfilId || "0") >= 1 || parseInt(oParams.PerfilId || "0") == -1;
    let empresaSelected = (oParams.EmpresaRFC || "").length >= 1;

    doAjax(
        "/Reportes/Facturacion/Filtrar",
        oParams,
        function (resp) {
            if (resp.tieneError) {
                let summary = ``;
                if (Array.isArray(resp.errores) && resp.errores.length >= 1) {
                    resp.errores.forEach(function (error) {
                        summary += `<li>${error}</li>`;
                    });
                }
                showError($("#btnBuscar").text(), resp.mensaje + " " + summary);
                return;
            }

            //Se convierte la cadena JSON a objeto JSON
            resp.datos = responseHandler(resp.datos);

            if (resp.datos == null && resp.datos.Perfiles == null) {
                showInfo($("#btnBuscar").text(), noInfoMessage);
                return;
            }

            if (
                (resp.datos.Perfiles.PUEValues || []).length <= 0 &&
                (resp.datos.Perfiles.PPDValues || []).length <= 0 &&
                (resp.datos.Perfiles.PrefacturadoValues || []).length <= 0 &&
                (resp.datos.Perfiles.DisponibleValues || []).length <= 0)
            {
                showInfo($("#btnBuscar").text(), noInfoMessage);
                return;
            }

            if (
                (resp.datos.Empresas.PUEValues || []).length <= 0 &&
                (resp.datos.Empresas.PPDValues || []).length <= 0 &&
                (resp.datos.Empresas.PrefacturadoValues || []).length <= 0 &&
                (resp.datos.Empresas.DisponibleValues || []).length <= 0) {
                showInfo($("#btnBuscar").text(), noInfoMessage);
                return;
            }

            $("#divCharts").html(`
                <div class="container-fluid">
	                <div class="row">
		                <div class="col-12" style="display: none;">
			                <canvas id="chartPerfilesBarras"></canvas>
		                </div>
		                <div class="col-12 col-lg-6" style="display: none;">
			                <canvas id="chartPerfilesPie"></canvas>
		                </div>
		                <div class="col-12 col-lg-6" style="display: none;">
			                <canvas id="chartEmpresas"></canvas>
		                </div>
                        <div class="col-12 col-lg-6" style="display: none;">
			                <div class="accordion" id="accordionEmpresas">
                            </div>
		                </div>
	                </div>
                </div>
            `);

            let chartType = CHART_TYPE_BAR;
            if (perfilSelected || empresaSelected) {
                chartType = CHART_TYPE_PIE;

                CreatePerfilesPieChart(
                    ["PUE", "PPD", "Prefacturado", "Disponible"],
                    resp.datos.Perfiles.PUEValues,
                    resp.datos.Perfiles.PPDValues,
                    resp.datos.Perfiles.PrefacturadoValues,
                    resp.datos.Perfiles.DisponibleValues
                );

                resp.datos.Empresas.LabelValues.forEach(function (e, i) {
                    let showClass = 'show';
                    let ariaExpanded = 'true';
                    let collapsedClass = '';
                    if (i >= 1) { showClass = ''; ariaExpanded = 'false'; collapsedClass = 'collapsed'; }

                    $("#accordionEmpresas").append(`
                        <div class="accordion-item">
                            <h2 class="accordion-header">
                                <button class="accordion-button ${collapsedClass}" type="button" data-bs-toggle="collapse" data-bs-target="#collapse${i}" aria-expanded="${ariaExpanded}" aria-controls="collapse${i}">
                                    ${resp.datos.Empresas.PorcentajeDisponible[i]}% - ${e}
                                </button>
                            </h2>
                            <div id="collapse${i}" class="accordion-collapse collapse ${showClass}">
                                <div class="accordion-body">
                                    <canvas id="chartEmpresa${i}"></canvas>
                                </div>
                            </div>
                        </div>
                    `);

                    let data = {
                        labels: ["PUE", "PPD", "Prefacturado", "Disponible"],
                        datasets: [
                            {
                                label: `PUE: $${numFormatter.format(resp.datos.Empresas.PUEValues[i])}`,
                                data: [resp.datos.Empresas.PUEValues[i], 0, 0, 0],
                                backgroundColor: [barColors[4]],
                                borderColor: [barBorderColors[4]],
                                skipNull: true,
                                stack: "0"
                            },
                            {
                                label: `PPD: $${numFormatter.format(resp.datos.Empresas.PPDValues[i])}`,
                                data: [0, resp.datos.Empresas.PPDValues[i], 0, 0],
                                backgroundColor: [barColors[5]],
                                borderColor: [barBorderColors[5]],
                                skipNull: true,
                                stack: "0"
                            },
                            {
                                label: `Prefacturado: $${numFormatter.format(resp.datos.Empresas.PrefacturadoValues[i])}`,
                                data: [ 0, 0, resp.datos.Empresas.PrefacturadoValues[i], 0],
                                backgroundColor: [barColors[2]],
                                borderColor: [barBorderColors[2]],
                                skipNull: true,
                                stack: "0"
                            },
                            {
                                label: `Disponible: $${numFormatter.format(resp.datos.Empresas.DisponibleValues[i])}`,
                                data: [0, 0, 0, resp.datos.Empresas.DisponibleValues[i]],
                                backgroundColor: [barColors[3]],
                                borderColor: [barBorderColors[3]],
                                skipNull: true,
                                stack: "0"
                            }
                        ]
                    };

                    let titles = [];
                    let perfilSelected = ($("#selFiltroPerfil").val() || 0) >= 1 ? $("#selFiltroPerfil option:selected").text() || sinPerfilSelectItemText : "";
                    let nivelSelected = resp.datos.Empresas.NivelesValues[i] || sinNivelSelectItemText;
                    if ((perfilSelected).length >= 1) { titles.push(`Perfil: ${perfilSelected}`); }
                    if ((nivelSelected).length >= 1) { titles.push(`Nivel: ${nivelSelected}`); }
                    titles.push(`PUE + PPD: $${numFormatter.format(parseFloat(resp.datos.Empresas.PUEValues[i] || 0) + parseFloat(resp.datos.Empresas.PPDValues[i]||0))}`);

                    let config = {
                        type: 'bar',
                        data: data,
                        options: {
                            plugins: {
                                tooltip: {
                                    position: "nearest",
                                    callbacks: {
                                        label: function (context) {
                                            return `$${numFormatter.format(context.raw)}`;
                                        }
                                    }
                                },
                                title: {
                                    display: true,
                                    text: titles.join("    ")
                                }
                            }
                        },
                        scales: {
                            x: {
                                stacked: true,
                            },
                            y: {
                                stacked: true
                            }
                        }
                    }

                    chartsEmpresas[i] = new Chart(`chartEmpresa${i}`, config);

                });
            }
            else {
                CreatePerfilesBarChart(
                    resp.datos.Perfiles.LabelValues,
                    resp.datos.Perfiles.PUEValues,
                    resp.datos.Perfiles.PPDValues,
                    resp.datos.Perfiles.PrefacturadoValues,
                    resp.datos.Perfiles.DisponibleValues,
                    chartType
                );
            }

        }, function (error) {
            showError("Error", error);
        },
        postOptions
    );
}
////////////////////////////////