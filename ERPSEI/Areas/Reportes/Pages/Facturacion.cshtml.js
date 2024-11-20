var numFormatter = null;
var firstChart = null;
var secondChart = null;

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
        change: function (element, item) {
            clearGraphics();
            if (!item) { $('#inpFiltroEmpresaRFC').data('rfc', null); }
        }
    });

    //CreateFirstChart();

    //CreateSecondChart();

    //CreateThirdChart();

    //CreateFourthChart();
});

////////////////////////////////
//Funcionalidad Filtrar
////////////////////////////////
//Función para limpiar la tabla de resultados
function clearGraphics() {
    if (firstChart) { firstChart.destroy(); }
    if (secondChart) { secondChart.destroy(); }
}
//Función para convertir una cadena JSON a un objeto JSON
function responseHandler(res) {
    if (typeof res == "string" && res.length >= 1) {
        res = JSON.parse(res);
    }

    return res
}

function CreateFirstChart(labelValues, pueValues, ppdValues, prefacturadoValues, disponibleValues) {
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
        type: 'bar',
        data: data,
        options: options
    }

    firstChart = new Chart("chartPerfiles", config);
}

function CreateSecondChart(labelValues, pueValues, ppdValues, prefacturadoValues, disponibleValues) {
    let barColors = [
        'rgba(255, 99, 132, 1)', //0 - RED
        'rgba(255, 159, 64, 1)', //1 -ORANGE
        'rgba(255, 205, 86, 1)', //2 -YELLOW
        'rgba(75, 192, 192, 1)', //3 -GREEN
        'rgba(54, 162, 235, 1)', //4 -BLUE
        'rgba(153, 102, 255, 1)', //5 -PURPLE
        'rgba(120, 230, 11, 1)', //6 -LIGHT GREEN
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

function CreateThirdChart() {
    let yValues = [
        "Publicidad y Mercadotecnia", //2
        "Servicios de Consultoría", //4
        "Sistemas y Tecnología", //2
        "Comercializadora", //1
        "Renta de Mobiliario de Oficina y Cómputo",//1
        "Logística y Transporte",//2
        "Renta de Computadoras y Regalías",//1
        "Servicios Médicos",//1
        "Constructoras"//2
    ];
    let y2Values = [
        "Atlantic",
        "Newgen",
        "Finance",
        "Ducuart",
        "NRD",
        "Week",
        "Cyber",
        "J&R",
        "Montena",
        "Creativity",
        "DOR",
        "Ocean",
        "HV",
        "Pharmex",
        "Reciza",
        "Newgen Construcciones",
    ]
    let barColors = [
        'rgba(255, 99, 132, 1)', //0 - RED
        'rgba(255, 159, 64, 1)', //1 -ORANGE
        'rgba(255, 205, 86, 1)', //2 -YELLOW
        'rgba(75, 192, 192, 1)', //3 -GREEN
        'rgba(54, 162, 235, 1)', //4 -BLUE
        'rgba(153, 102, 255, 1)', //5 -PURPLE
        'rgba(120, 230, 11, 1)', //6 -LIGHT GREEN
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

    let LIMITE_FACTURACION = 10;

    let pueValues = [
        3,
        1,
        4,
        9,
        2,
        5,
        8,
        6
    ];
    let ppdValues = [
        2,
        6,
        3,
        1,
        6,
        4,
        1,
        2
    ];
    let prefacturadoValues = [
        1,
        1,
        1,
        0.1,
        0.5,
        0.01,
        0.05,
        0.2
    ];
    let facturadoValues = [
        pueValues[0] + ppdValues[0] + prefacturadoValues[0],
        pueValues[1] + ppdValues[1] + prefacturadoValues[1],
        pueValues[2] + ppdValues[2] + prefacturadoValues[2],
        pueValues[3] + ppdValues[3] + prefacturadoValues[3],
        pueValues[4] + ppdValues[4] + prefacturadoValues[4],
        pueValues[5] + ppdValues[5] + prefacturadoValues[5],
        pueValues[6] + ppdValues[6] + prefacturadoValues[6],
        pueValues[7] + ppdValues[7] + prefacturadoValues[7]
    ];
    let disponibleValues = [
        (facturadoValues[0] < LIMITE_FACTURACION ? LIMITE_FACTURACION - facturadoValues[0] : 0),
        (facturadoValues[1] < LIMITE_FACTURACION ? LIMITE_FACTURACION - facturadoValues[1] : 0),
        (facturadoValues[2] < LIMITE_FACTURACION ? LIMITE_FACTURACION - facturadoValues[2] : 0),
        (facturadoValues[3] < LIMITE_FACTURACION ? LIMITE_FACTURACION - facturadoValues[3] : 0),
        (facturadoValues[4] < LIMITE_FACTURACION ? LIMITE_FACTURACION - facturadoValues[4] : 0),
        (facturadoValues[5] < LIMITE_FACTURACION ? LIMITE_FACTURACION - facturadoValues[5] : 0),
        (facturadoValues[6] < LIMITE_FACTURACION ? LIMITE_FACTURACION - facturadoValues[6] : 0),
        (facturadoValues[7] < LIMITE_FACTURACION ? LIMITE_FACTURACION - facturadoValues[7] : 0)
    ];
    //let excesoValues = [
    //    (facturadoValues[0] >= LIMITE_FACTURACION ? facturadoValues[0] - LIMITE_FACTURACION : 0),
    //    (facturadoValues[1] >= LIMITE_FACTURACION ? facturadoValues[1] - LIMITE_FACTURACION : 0),
    //    (facturadoValues[2] >= LIMITE_FACTURACION ? facturadoValues[2] - LIMITE_FACTURACION : 0),
    //    (facturadoValues[3] >= LIMITE_FACTURACION ? facturadoValues[3] - LIMITE_FACTURACION : 0),
    //    (facturadoValues[4] >= LIMITE_FACTURACION ? facturadoValues[4] - LIMITE_FACTURACION : 0),
    //    (facturadoValues[5] >= LIMITE_FACTURACION ? facturadoValues[5] - LIMITE_FACTURACION : 0),
    //    (facturadoValues[6] >= LIMITE_FACTURACION ? facturadoValues[6] - LIMITE_FACTURACION : 0),
    //    (facturadoValues[7] >= LIMITE_FACTURACION ? facturadoValues[7] - LIMITE_FACTURACION : 0)
    //];

    let catPer = 0.6
    let datasetsAgrupacionA = [
        //{
        //    label: 'Full',
        //    data: [12],
        //    backgroundColor: barColors[7],
        //    borderColor: barBorderColors[7],
        //    borderWidth: 1,
        //    grouped: false,
        //    stack: 1,
        //    categoryPercentage: 1,
        //    order: 1
        //},
        {
            label: 'PUE',
            data: [1],
            backgroundColor: barColors[4],
            borderColor: barBorderColors[4],
            borderWidth: 1,
            stack: 2,
            order: 0,
            skipNull: true
        },
        {
            label: 'PPD',
            data: [1],
            backgroundColor: barColors[5],
            borderColor: barBorderColors[5],
            borderWidth: 1,
            stack: 2,
            order: 0,
            skipNull: true
        },
        {
            label: 'Prefacturado',
            data: [1],
            backgroundColor: barColors[2],
            borderColor: barBorderColors[2],
            borderWidth: 1,
            stack: 2,
            order: 0,
            skipNull: true
        },
        {
            label: 'Disponible',
            data: [1],
            backgroundColor: barColors[3],
            borderColor: barBorderColors[3],
            borderWidth: 1,
            stack: 2,
            order: 0,
            skipNull: true
        },
        {
            label: 'PUE 2',
            data: [2],
            backgroundColor: barColors[4],
            borderColor: barBorderColors[4],
            borderWidth: 1,
            stack: 3,
            order: 0,
            skipNull: true
        },
        {
            label: 'PPD 2',
            data: [2],
            backgroundColor: barColors[5],
            borderColor: barBorderColors[5],
            borderWidth: 1,
            stack: 3,
            order: 0,
            skipNull: true
        },
        {
            label: 'Prefacturado 2',
            data: [2],
            backgroundColor: barColors[2],
            borderColor: barBorderColors[2],
            borderWidth: 1,
            stack: 3,
            order: 0,
            skipNull: true
        },
        {
            label: 'Disponible 2',
            data: [2],
            backgroundColor: barColors[3],
            borderColor: barBorderColors[3],
            borderWidth: 1,
            stack: 3,
            order: 0,
            skipNull: true
        }
    ];
    let datasetsB = [
        {
            label: 'PUE 3',
            data: [null, 3],
            backgroundColor: barColors[4],
            borderColor: barBorderColors[4],
            borderWidth: 1,
            stack: 3,
            order: 0,
            skipNull: true
        },
        {
            label: 'PPD 3',
            data: [null, 3],
            backgroundColor: barColors[5],
            borderColor: barBorderColors[5],
            borderWidth: 1,
            stack: 3,
            order: 0,
            skipNull: true
        },
        {
            label: 'Prefacturado 3',
            data: [null, 3],
            backgroundColor: barColors[2],
            borderColor: barBorderColors[2],
            borderWidth: 1,
            stack: 3,
            order: 0,
            skipNull: true
        },
        {
            label: 'Disponible 3',
            data: [null, 3],
            backgroundColor: barColors[3],
            borderColor: barBorderColors[3],
            borderWidth: 1,
            stack: 3,
            order: 0,
            skipNull: true
        },
        {
            label: 'PUE 4',
            data: [null, 11],
            backgroundColor: barColors[4],
            borderColor: barBorderColors[4],
            borderWidth: 1,
            stack: 4,
            order: 0,
            skipNull: true
        },
        {
            label: 'PPD 4',
            data: [null, 11],
            backgroundColor: barColors[5],
            borderColor: barBorderColors[5],
            borderWidth: 1,
            stack: 4,
            order: 0,
            skipNull: true
        },
        {
            label: 'Prefacturado 4',
            data: [null, 11],
            backgroundColor: barColors[2],
            borderColor: barBorderColors[2],
            borderWidth: 1,
            stack: 4,
            order: 0,
            skipNull: true
        },
        {
            label: 'Disponible 4',
            data: [null, 11],
            backgroundColor: barColors[3],
            borderColor: barBorderColors[3],
            borderWidth: 1,
            stack: 4,
            order: 0,
            skipNull: true
        },
        {
            label: 'PUE 5',
            data: [null, 6],
            backgroundColor: barColors[4],
            borderColor: barBorderColors[4],
            borderWidth: 1,
            stack: 5,
            order: 0,
            skipNull: true
        },
        {
            label: 'PPD 5',
            data: [null, 6],
            backgroundColor: barColors[5],
            borderColor: barBorderColors[5],
            borderWidth: 1,
            stack: 5,
            order: 0,
            skipNull: true
        },
        {
            label: 'Prefacturado 5',
            data: [null, 6],
            backgroundColor: barColors[2],
            borderColor: barBorderColors[2],
            borderWidth: 1,
            stack: 5,
            order: 0,
            skipNull: true
        },
        {
            label: 'Disponible 5',
            data: [null, 6],
            backgroundColor: barColors[3],
            borderColor: barBorderColors[3],
            borderWidth: 1,
            stack: 5,
            order: 0,
            skipNull: true
        },
        {
            label: 'PUE 6',
            data: [null, 3],
            backgroundColor: barColors[4],
            borderColor: barBorderColors[4],
            borderWidth: 1,
            stack: 6,
            order: 0,
            skipNull: true
        },
        {
            label: 'PPD 6',
            data: [null, 3],
            backgroundColor: barColors[5],
            borderColor: barBorderColors[5],
            borderWidth: 1,
            stack: 6,
            order: 0,
            skipNull: true
        },
        {
            label: 'Prefacturado 6',
            data: [null, 3],
            backgroundColor: barColors[2],
            borderColor: barBorderColors[2],
            borderWidth: 1,
            stack: 6,
            order: 0,
            skipNull: true
        },
        {
            label: 'Disponible 6 ',
            data: [null, 3],
            backgroundColor: barColors[3],
            borderColor: barBorderColors[3],
            borderWidth: 1,
            stack: 6,
            order: 0,
            skipNull: true
        }
    ];

    let dsA = [
        {
            label: 'PUE 3',
            data: pueValues,
            backgroundColor: barColors[4],
            borderColor: barBorderColors[4],
            borderWidth: 1,
            stack: 0,
            order: 0,
            skipNull: true,
            yAxisID: 'y2'
        },
        {
            label: 'PPD 3',
            data: ppdValues,
            backgroundColor: barColors[5],
            borderColor: barBorderColors[5],
            borderWidth: 1,
            stack: 0,
            order: 0,
            skipNull: true,
            yAxisID: 'y2'
        },
        {
            label: 'Prefacturado 3',
            data: prefacturadoValues,
            backgroundColor: barColors[2],
            borderColor: barBorderColors[2],
            borderWidth: 1,
            stack: 0,
            order: 0,
            skipNull: true,
            yAxisID: 'y2'
        },
        {
            label: 'Disponible 3',
            data: disponibleValues,
            backgroundColor: barColors[3],
            borderColor: barBorderColors[3],
            borderWidth: 1,
            stack: 0,
            order: 0,
            skipNull: true,
            yAxisID: 'y2'
        }
    ];

    let data = {
        labels: yValues,
        datasets: dsA
    };

    let options = {
        indexAxis: 'y',
        plugins: {
            title: {
                display: true,
                text: 'Facturación por Empresa'
            }
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
                //        return '$' + Chart.Ticks.formatters.numeric.apply(this, [value, index, ticks]);
                //    }
                //}
            },
            y: {
                stacked: true,
                grid: {
                    drawOnChartArea: false
                }
            },
            y2: {
                labels: y2Values,
            }
        }
    };

    let config = {
        type: 'bar',
        data: data,
        options: options
    }

    new Chart("myChartC", config);
}

function CreateFourthChart() {
    let yValues = [
        "Publicidad y Mercadotecnia", //2
        "Servicios de Consultoría", //4
        "Sistemas y Tecnología", //2
        "Comercializadora", //1
        "Renta de Mobiliario de Oficina y Cómputo",//1
        "Logística y Transporte",//2
        "Renta de Computadoras y Regalías",//1
        "Servicios Médicos",//1
        "Constructoras"//2
    ];
    let y2Values = [
        "Atlantic",
        "Newgen",
        "Finance",
        "Ducuart",
        "NRD",
        "Week",
        "Cyber",
        "J&R",
        "Montena",
        "Creativity",
        "DOR",
        "Ocean",
        "HV",
        "Pharmex",
        "Reciza",
        "Newgen Construcciones",
    ]
    let barColors = [
        'rgba(255, 99, 132, 1)', //0 - RED
        'rgba(255, 159, 64, 1)', //1 -ORANGE
        'rgba(255, 205, 86, 1)', //2 -YELLOW
        'rgba(75, 192, 192, 1)', //3 -GREEN
        'rgba(54, 162, 235, 1)', //4 -BLUE
        'rgba(153, 102, 255, 1)', //5 -PURPLE
        'rgba(120, 230, 11, 1)', //6 -LIGHT GREEN
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

    let LIMITE_FACTURACION = 10;

    let pueValues = [
        3,
        1,
        4,
        9,
        2,
        5,
        8,
        6
    ];
    let ppdValues = [
        2,
        6,
        3,
        1,
        6,
        4,
        1,
        2
    ];
    let prefacturadoValues = [
        1,
        1,
        1,
        0.1,
        0.5,
        0.01,
        0.05,
        0.2
    ];
    let facturadoValues = [
        pueValues[0] + ppdValues[0] + prefacturadoValues[0],
        pueValues[1] + ppdValues[1] + prefacturadoValues[1],
        pueValues[2] + ppdValues[2] + prefacturadoValues[2],
        pueValues[3] + ppdValues[3] + prefacturadoValues[3],
        pueValues[4] + ppdValues[4] + prefacturadoValues[4],
        pueValues[5] + ppdValues[5] + prefacturadoValues[5],
        pueValues[6] + ppdValues[6] + prefacturadoValues[6],
        pueValues[7] + ppdValues[7] + prefacturadoValues[7]
    ];
    let disponibleValues = [
        (facturadoValues[0] < LIMITE_FACTURACION ? LIMITE_FACTURACION - facturadoValues[0] : 0),
        (facturadoValues[1] < LIMITE_FACTURACION ? LIMITE_FACTURACION - facturadoValues[1] : 0),
        (facturadoValues[2] < LIMITE_FACTURACION ? LIMITE_FACTURACION - facturadoValues[2] : 0),
        (facturadoValues[3] < LIMITE_FACTURACION ? LIMITE_FACTURACION - facturadoValues[3] : 0),
        (facturadoValues[4] < LIMITE_FACTURACION ? LIMITE_FACTURACION - facturadoValues[4] : 0),
        (facturadoValues[5] < LIMITE_FACTURACION ? LIMITE_FACTURACION - facturadoValues[5] : 0),
        (facturadoValues[6] < LIMITE_FACTURACION ? LIMITE_FACTURACION - facturadoValues[6] : 0),
        (facturadoValues[7] < LIMITE_FACTURACION ? LIMITE_FACTURACION - facturadoValues[7] : 0)
    ];
    //let excesoValues = [
    //    (facturadoValues[0] >= LIMITE_FACTURACION ? facturadoValues[0] - LIMITE_FACTURACION : 0),
    //    (facturadoValues[1] >= LIMITE_FACTURACION ? facturadoValues[1] - LIMITE_FACTURACION : 0),
    //    (facturadoValues[2] >= LIMITE_FACTURACION ? facturadoValues[2] - LIMITE_FACTURACION : 0),
    //    (facturadoValues[3] >= LIMITE_FACTURACION ? facturadoValues[3] - LIMITE_FACTURACION : 0),
    //    (facturadoValues[4] >= LIMITE_FACTURACION ? facturadoValues[4] - LIMITE_FACTURACION : 0),
    //    (facturadoValues[5] >= LIMITE_FACTURACION ? facturadoValues[5] - LIMITE_FACTURACION : 0),
    //    (facturadoValues[6] >= LIMITE_FACTURACION ? facturadoValues[6] - LIMITE_FACTURACION : 0),
    //    (facturadoValues[7] >= LIMITE_FACTURACION ? facturadoValues[7] - LIMITE_FACTURACION : 0)
    //];

    let catPer = 0.6
    let datasetsAgrupacionA = [
        //{
        //    label: 'Full',
        //    data: [12],
        //    backgroundColor: barColors[7],
        //    borderColor: barBorderColors[7],
        //    borderWidth: 1,
        //    grouped: false,
        //    stack: 1,
        //    categoryPercentage: 1,
        //    order: 1
        //},
        {
            label: 'PUE',
            data: [1],
            backgroundColor: barColors[4],
            borderColor: barBorderColors[4],
            borderWidth: 1,
            stack: 2,
            order: 0,
            skipNull: true
        },
        {
            label: 'PPD',
            data: [1],
            backgroundColor: barColors[5],
            borderColor: barBorderColors[5],
            borderWidth: 1,
            stack: 2,
            order: 0,
            skipNull: true
        },
        {
            label: 'Prefacturado',
            data: [1],
            backgroundColor: barColors[2],
            borderColor: barBorderColors[2],
            borderWidth: 1,
            stack: 2,
            order: 0,
            skipNull: true
        },
        {
            label: 'Disponible',
            data: [1],
            backgroundColor: barColors[3],
            borderColor: barBorderColors[3],
            borderWidth: 1,
            stack: 2,
            order: 0,
            skipNull: true
        },
        {
            label: 'PUE 2',
            data: [2],
            backgroundColor: barColors[4],
            borderColor: barBorderColors[4],
            borderWidth: 1,
            stack: 3,
            order: 0,
            skipNull: true
        },
        {
            label: 'PPD 2',
            data: [2],
            backgroundColor: barColors[5],
            borderColor: barBorderColors[5],
            borderWidth: 1,
            stack: 3,
            order: 0,
            skipNull: true
        },
        {
            label: 'Prefacturado 2',
            data: [2],
            backgroundColor: barColors[2],
            borderColor: barBorderColors[2],
            borderWidth: 1,
            stack: 3,
            order: 0,
            skipNull: true
        },
        {
            label: 'Disponible 2',
            data: [2],
            backgroundColor: barColors[3],
            borderColor: barBorderColors[3],
            borderWidth: 1,
            stack: 3,
            order: 0,
            skipNull: true
        }
    ];
    let datasetsB = [
        {
            label: 'PUE 3',
            data: [null, 3],
            backgroundColor: barColors[4],
            borderColor: barBorderColors[4],
            borderWidth: 1,
            stack: 3,
            order: 0,
            skipNull: true
        },
        {
            label: 'PPD 3',
            data: [null, 3],
            backgroundColor: barColors[5],
            borderColor: barBorderColors[5],
            borderWidth: 1,
            stack: 3,
            order: 0,
            skipNull: true
        },
        {
            label: 'Prefacturado 3',
            data: [null, 3],
            backgroundColor: barColors[2],
            borderColor: barBorderColors[2],
            borderWidth: 1,
            stack: 3,
            order: 0,
            skipNull: true
        },
        {
            label: 'Disponible 3',
            data: [null, 3],
            backgroundColor: barColors[3],
            borderColor: barBorderColors[3],
            borderWidth: 1,
            stack: 3,
            order: 0,
            skipNull: true
        },
        {
            label: 'PUE 4',
            data: [null, 11],
            backgroundColor: barColors[4],
            borderColor: barBorderColors[4],
            borderWidth: 1,
            stack: 4,
            order: 0,
            skipNull: true
        },
        {
            label: 'PPD 4',
            data: [null, 11],
            backgroundColor: barColors[5],
            borderColor: barBorderColors[5],
            borderWidth: 1,
            stack: 4,
            order: 0,
            skipNull: true
        },
        {
            label: 'Prefacturado 4',
            data: [null, 11],
            backgroundColor: barColors[2],
            borderColor: barBorderColors[2],
            borderWidth: 1,
            stack: 4,
            order: 0,
            skipNull: true
        },
        {
            label: 'Disponible 4',
            data: [null, 11],
            backgroundColor: barColors[3],
            borderColor: barBorderColors[3],
            borderWidth: 1,
            stack: 4,
            order: 0,
            skipNull: true
        },
        {
            label: 'PUE 5',
            data: [null, 6],
            backgroundColor: barColors[4],
            borderColor: barBorderColors[4],
            borderWidth: 1,
            stack: 5,
            order: 0,
            skipNull: true
        },
        {
            label: 'PPD 5',
            data: [null, 6],
            backgroundColor: barColors[5],
            borderColor: barBorderColors[5],
            borderWidth: 1,
            stack: 5,
            order: 0,
            skipNull: true
        },
        {
            label: 'Prefacturado 5',
            data: [null, 6],
            backgroundColor: barColors[2],
            borderColor: barBorderColors[2],
            borderWidth: 1,
            stack: 5,
            order: 0,
            skipNull: true
        },
        {
            label: 'Disponible 5',
            data: [null, 6],
            backgroundColor: barColors[3],
            borderColor: barBorderColors[3],
            borderWidth: 1,
            stack: 5,
            order: 0,
            skipNull: true
        },
        {
            label: 'PUE 6',
            data: [null, 3],
            backgroundColor: barColors[4],
            borderColor: barBorderColors[4],
            borderWidth: 1,
            stack: 6,
            order: 0,
            skipNull: true
        },
        {
            label: 'PPD 6',
            data: [null, 3],
            backgroundColor: barColors[5],
            borderColor: barBorderColors[5],
            borderWidth: 1,
            stack: 6,
            order: 0,
            skipNull: true
        },
        {
            label: 'Prefacturado 6',
            data: [null, 3],
            backgroundColor: barColors[2],
            borderColor: barBorderColors[2],
            borderWidth: 1,
            stack: 6,
            order: 0,
            skipNull: true
        },
        {
            label: 'Disponible 6 ',
            data: [null, 3],
            backgroundColor: barColors[3],
            borderColor: barBorderColors[3],
            borderWidth: 1,
            stack: 6,
            order: 0,
            skipNull: true
        }
    ];

    let dsA = [
        {
            label: 'PUE 3',
            data: pueValues,
            backgroundColor: barColors[4],
            borderColor: barBorderColors[4],
            borderWidth: 1,
            stack: 0,
            order: 0,
            skipNull: true
        },
        {
            label: 'PPD 3',
            data: ppdValues,
            backgroundColor: barColors[5],
            borderColor: barBorderColors[5],
            borderWidth: 1,
            stack: 0,
            order: 0,
            skipNull: true
        },
        {
            label: 'Prefacturado 3',
            data: prefacturadoValues,
            backgroundColor: barColors[2],
            borderColor: barBorderColors[2],
            borderWidth: 1,
            stack: 0,
            order: 0,
            skipNull: true
        },
        {
            label: 'Disponible 3',
            data: disponibleValues,
            backgroundColor: barColors[3],
            borderColor: barBorderColors[3],
            borderWidth: 1,
            stack: 0,
            order: 0,
            skipNull: true
        }
    ];

    let data = {
        labels: yValues,
        datasets: dsA
    };

    let options = {
        indexAxis: 'y',
        plugins: {
            title: {
                display: true,
                text: 'Facturación por Empresa'
            }
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
                //        return '$' + Chart.Ticks.formatters.numeric.apply(this, [value, index, ticks]);
                //    }
                //}
            },
            y: {
                stacked: true,
                grid: {
                    drawOnChartArea: false
                }
            },
            y2: {
                labels: y2Values,
            }
        }
    };

    let config = {
        type: 'bar',
        data: data,
        options: options
    }

    new Chart("myChartD", config);
}

////////////////////////////////
//Funcionalidad Filtrar
////////////////////////////////
//Función para filtrar los datos de la tabla.
function onBuscarClick() {
    clearGraphics();

    let oParams = {
        PerfilId: $("#selFiltroPerfil").val() == 0 ? null : $("#selFiltroPerfil").val(),
        EmpresaRFC: ($("#inpFiltroEmpresaRFC").data("rfc") || "") == "" ? null : $("#inpFiltroEmpresaRFC").data("rfc"),
        Anio: $("#selFiltroAnio").val(),
        Mes: $("#selFiltroMes").val() == 0 ? null : $("#selFiltroMes").val()
    };

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

            CreateFirstChart(
                resp.datos.Perfiles.LabelValues,
                resp.datos.Perfiles.PUEValues,
                resp.datos.Perfiles.PPDValues,
                resp.datos.Perfiles.PrefacturadoValues,
                resp.datos.Perfiles.DisponibleValues
            );

            CreateSecondChart(
                resp.datos.Empresas.LabelValues,
                resp.datos.Empresas.PUEValues,
                resp.datos.Empresas.PPDValues,
                resp.datos.Empresas.PrefacturadoValues,
                resp.datos.Empresas.DisponibleValues
            );

        }, function (error) {
            showError("Error", error);
        },
        postOptions
    );
}
////////////////////////////////