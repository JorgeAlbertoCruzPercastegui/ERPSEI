
using ERPSEI.Services.RelojChecador;
using System;

int correctas = 0;
int fallidas = 0;

void Ejecutar(string nombre, Action prueba)
{
    try
    {
        prueba();
        correctas++;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("[OK] ");
        Console.ResetColor();
        Console.WriteLine(nombre);
    }
    catch (Exception ex)
    {
        fallidas++;

        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("[ERROR] ");
        Console.ResetColor();

        Console.WriteLine(nombre);
        Console.WriteLine($"        {ex.Message}");
    }
}

void Verificar(bool condicion, string mensaje)
{
    if (!condicion)
    {
        throw new InvalidOperationException(mensaje);
    }
}

void VerificarCodigo(
    RelojReglasConciliacion.ResultadoValidacion resultado,
    string codigoEsperado)
{
    Verificar(
        resultado.Codigo == codigoEsperado,
        $"Se esperaba '{codigoEsperado}', " +
        $"pero se obtuvo '{resultado.Codigo}'.");
}

const string tipo = "Entrada";
const string entradaDigital = "INTRANET:1";
const string entradaBioTime = "BIOTIME:QA-ENTRADA";

string[] candidatos =
{
    entradaDigital,
    entradaBioTime
};

string firma = RelojReglasConciliacion.CalcularFirma(
    tipo,
    candidatos);

Console.WriteLine();
Console.WriteLine("INTRANET - PRUEBAS DE CONCILIACION");
Console.WriteLine("==================================");
Console.WriteLine();

// ======================================================
// GRUPO 1: TIPOS DE MOVIMIENTO
// ======================================================

Ejecutar(
    "01. Reconoce los cuatro tipos de movimiento",
    () =>
    {
        string[] tipos =
        {
            "Entrada",
            "SalidaComer",
            "RegresoComer",
            "Salida"
        };

        foreach (string movimiento in tipos)
        {
            Verificar(
                RelojReglasConciliacion.EsTipoValido(movimiento),
                $"Tipo no reconocido: {movimiento}");
        }
    });

Ejecutar(
    "02. Rechaza un tipo desconocido",
    () =>
    {
        Verificar(
            !RelojReglasConciliacion.EsTipoValido(
                "MovimientoInvalido"),
            "Se aceptó un tipo no permitido.");
    });

// ======================================================
// GRUPO 2: FIRMA DE MOVIMIENTOS
// ======================================================

Ejecutar(
    "03. Firma estable sin importar el orden",
    () =>
    {
        string firmaInvertida =
            RelojReglasConciliacion.CalcularFirma(
                tipo,
                Enumerable.Reverse(candidatos));

        Verificar(
            firma == firmaInvertida,
            "La firma cambió al invertir el orden.");
    });

Ejecutar(
    "04. Detecta un candidato adicional",
    () =>
    {
        string firmaNueva =
            RelojReglasConciliacion.CalcularFirma(
                tipo,
                candidatos.Append("INTRANET:99"));

        Verificar(
            firma != firmaNueva,
            "La firma no cambió al agregar un candidato.");
    });

Ejecutar(
    "05. Detecta cambios en el tipo de movimiento",
    () =>
    {
        string firmaSalida =
            RelojReglasConciliacion.CalcularFirma(
                "Salida",
                candidatos);

        Verificar(
            firma != firmaSalida,
            "La firma no cambió con un tipo diferente.");
    });

// ======================================================
// GRUPO 3: VALIDACION DE SELECCIONES
// ======================================================

Ejecutar(
    "06. Acepta dos candidatos válidos",
    () =>
    {
        var resultado =
            RelojReglasConciliacion.ValidarSeleccion(
                tipo,
                entradaDigital,
                firma,
                candidatos);

        Verificar(
            resultado.Correcto,
            resultado.Mensaje);
    });

Ejecutar(
    "07. Rechaza una única marca sin conflicto",
    () =>
    {
        var resultado =
            RelojReglasConciliacion.ValidarSeleccion(
                tipo,
                entradaDigital,
                firma,
                new[] { entradaDigital });

        VerificarCodigo(
            resultado,
            "SIN_CONFLICTO");
    });

Ejecutar(
    "08. Rechaza una marca ajena al conflicto",
    () =>
    {
        var resultado =
            RelojReglasConciliacion.ValidarSeleccion(
                tipo,
                "INTRANET:999",
                firma,
                candidatos);

        VerificarCodigo(
            resultado,
            "SELECCION_INVALIDA");
    });

Ejecutar(
    "09. Rechaza una firma desactualizada",
    () =>
    {
        var resultado =
            RelojReglasConciliacion.ValidarSeleccion(
                tipo,
                entradaDigital,
                firma,
                candidatos.Append("BIOTIME:OTRA"));

        VerificarCodigo(
            resultado,
            "DATOS_CAMBIARON");
    });

Ejecutar(
    "10. Rechaza identificadores duplicados",
    () =>
    {
        var resultado =
            RelojReglasConciliacion.ValidarSeleccion(
                tipo,
                entradaDigital,
                firma,
                new[]
                {
                    entradaDigital,
                    entradaDigital
                });

        VerificarCodigo(
            resultado,
            "CANDIDATOS_INVALIDOS");
    });

Ejecutar(
    "11. Excluye marcas pendientes y tipos distintos",
    () =>
    {
        var movimientos = new[]
        {
            new RelojReglasConciliacion.Candidato(
                entradaDigital,
                "Entrada",
                true),

            new RelojReglasConciliacion.Candidato(
                entradaBioTime,
                "Entrada",
                false),

            new RelojReglasConciliacion.Candidato(
                "INTRANET:55",
                "Salida",
                true)
        };

        var filtrados =
            RelojReglasConciliacion.ObtenerConfirmados(
                movimientos,
                "Entrada");

        Verificar(
            filtrados.Count == 1 &&
            filtrados[0].IdMovimiento == entradaDigital,
            "Se incluyeron marcas no confirmadas " +
            "o de otro tipo.");
    });

// ======================================================
// GRUPO 4: RESOLUCIONES ADMINISTRATIVAS
// ======================================================

Ejecutar(
    "12. Aplica una resolución activa",
    () =>
    {
        var participantes =
            RelojReglasConciliacion
                .ObtenerMovimientosParticipantes(
                    tipo,
                    candidatos,
                    entradaDigital,
                    firma,
                    "Activa");

        Verificar(
            participantes.Count == 1 &&
            participantes[0] == entradaDigital,
            "No se conservó únicamente el candidato elegido.");
    });

Ejecutar(
    "13. Permite seleccionar la marca BioTime",
    () =>
    {
        var participantes =
            RelojReglasConciliacion
                .ObtenerMovimientosParticipantes(
                    tipo,
                    candidatos,
                    entradaBioTime,
                    firma,
                    "Activa");

        Verificar(
            participantes.Count == 1 &&
            participantes[0] == entradaBioTime,
            "No se aplicó el cambio de selección.");
    });

Ejecutar(
    "14. No aplica resoluciones revocadas",
    () =>
    {
        bool aplicable =
            RelojReglasConciliacion.ResolucionAplicable(
                tipo,
                candidatos,
                entradaDigital,
                firma,
                "Revocada");

        Verificar(
            !aplicable,
            "Se aplicó una resolución revocada.");
    });

Ejecutar(
    "15. No aplica resoluciones desactualizadas",
    () =>
    {
        bool aplicable =
            RelojReglasConciliacion.ResolucionAplicable(
                tipo,
                candidatos.Append("INTRANET:77"),
                entradaDigital,
                firma,
                "Activa");

        Verificar(
            !aplicable,
            "Se aplicó una resolución con firma antigua.");
    });

Ejecutar(
    "16. No elige automáticamente sin resolución",
    () =>
    {
        var participantes =
            RelojReglasConciliacion
                .ObtenerMovimientosParticipantes(
                    tipo,
                    candidatos,
                    null,
                    null,
                    "SinResolver");

        Verificar(
            participantes.Count == 2,
            "Se seleccionó una marca sin autorización.");
    });


// ======================================================
// GRUPO 5: EVALUACION PRELIMINAR TH
// ======================================================

RelojReglasEvaluacionTH.DatosJornada JornadaEjemplo(
    int horaEntrada = 9,
    int minutoEntrada = 10,
    int horaSalida = 18,
    int minutoSalida = 5)
{
    return new RelojReglasEvaluacionTH.DatosJornada
    {
        EntradaProgramada = new TimeSpan(9, 0, 0),
        LimiteRetardo = new TimeSpan(9, 16, 0),
        SalidaProgramada = new TimeSpan(18, 0, 0),
        EntradaReal = new DateTime(
            2026, 11, 23, horaEntrada, minutoEntrada, 0),
        SalidaComerReal = new DateTime(
            2026, 11, 23, 14, 0, 0),
        RegresoComerReal = new DateTime(
            2026, 11, 23, 15, 0, 0),
        SalidaReal = new DateTime(
            2026, 11, 23, horaSalida, minutoSalida, 0)
    };
}

Ejecutar("17. Entrada puntual", () =>
{
    var r = RelojReglasEvaluacionTH.Evaluar(
        JornadaEjemplo());

    Verificar(
        r.Puntualidad == "Puntual",
        "No reconoció la entrada puntual.");
});

Ejecutar("18. Entrada en límite de tolerancia", () =>
{
    var r = RelojReglasEvaluacionTH.Evaluar(
        JornadaEjemplo(9, 16));

    Verificar(
        r.Puntualidad == "Puntual",
        "El límite de tolerancia debe incluirse.");
});

Ejecutar("19. Entrada posterior a tolerancia", () =>
{
    var r = RelojReglasEvaluacionTH.Evaluar(
        JornadaEjemplo(9, 17));

    Verificar(
        r.Puntualidad == "RetardoPotencial",
        "No detectó el retardo potencial.");
});

Ejecutar("20. Jornada completa ordenada", () =>
{
    var r = RelojReglasEvaluacionTH.Evaluar(
        JornadaEjemplo());

    Verificar(
        r.EstadoJornada == "JornadaCompleta",
        "No detectó la jornada completa.");
});

Ejecutar("21. Jornada incompleta", () =>
{
    var datos = JornadaEjemplo();
    datos.SalidaReal = null;

    var r = RelojReglasEvaluacionTH.Evaluar(datos);

    Verificar(
        r.EstadoJornada == "JornadaIncompleta",
        "No detectó el movimiento faltante.");
});

Ejecutar("22. Secuencia inconsistente", () =>
{
    var datos = JornadaEjemplo();

    datos.RegresoComerReal =
        new DateTime(2026, 11, 23, 13, 0, 0);

    var r = RelojReglasEvaluacionTH.Evaluar(datos);

    Verificar(
        r.EstadoJornada == "SecuenciaInconsistente",
        "No detectó el orden incorrecto.");
});

Ejecutar("23. Salida anticipada potencial", () =>
{
    var r = RelojReglasEvaluacionTH.Evaluar(
        JornadaEjemplo(9, 10, 17, 30));

    Verificar(
        r.EstadoSalida == "SalidaAnticipadaPotencial",
        "No detectó la salida anticipada.");
});

Ejecutar("24. Conflicto impide cierre automático", () =>
{
    var datos = JornadaEjemplo();
    datos.ConflictosSinResolver = true;

    var r = RelojReglasEvaluacionTH.Evaluar(datos);

    Verificar(
        r.EstadoJornada == "RequiereConciliacion" &&
        r.RequiereRevisionTH &&
        !r.PermiteCierreAutomatico,
        "Se permitió el cierre de una jornada en conflicto.");
});


// ======================================================
// GRUPO 6: CUMPLIMIENTO Y TIEMPOS REGISTRADOS
// ======================================================

Ejecutar("25. Calcula tiempo efectivo registrado", () =>
{
    var datos = JornadaEjemplo();

    var r = RelojReglasEvaluacionTH.Evaluar(datos);

    Verificar(
        r.TiempoTotalRegistrado == TimeSpan.FromHours(8).Add(
            TimeSpan.FromMinutes(55)) &&
        r.TiempoComidaRegistrado == TimeSpan.FromHours(1) &&
        r.TiempoEfectivoRegistrado ==
            TimeSpan.FromHours(7).Add(TimeSpan.FromMinutes(55)),
        "Los tiempos registrados son incorrectos.");
});

Ejecutar("26. Distingue jornada completa de cumplimiento", () =>
{
    var r = RelojReglasEvaluacionTH.Evaluar(
        JornadaEjemplo(14, 19, 15, 40));

    Verificar(
        r.IntegridadChecadas == "SecuenciaInconsistente" &&
        r.CumplimientoHorario == "NoConcluyente",
        "No distinguió integridad y cumplimiento.");
});

Ejecutar("27. Detecta desviaciones de horario", () =>
{
    var datos = JornadaEjemplo(10, 0, 17, 0);

    var r = RelojReglasEvaluacionTH.Evaluar(datos);

    Verificar(
        r.IntegridadChecadas == "Completa" &&
        r.CumplimientoHorario == "DesviacionesPotenciales" &&
        r.MinutosRetardoPotencial == 44 &&
        r.MinutosSalidaAnticipadaPotencial == 60,
        "No calculó correctamente las desviaciones.");
});

Ejecutar("28. No calcula tiempos con jornada incompleta", () =>
{
    var datos = JornadaEjemplo();
    datos.SalidaReal = null;

    var r = RelojReglasEvaluacionTH.Evaluar(datos);

    Verificar(
        r.TiempoEfectivoRegistrado == null &&
        r.IntegridadChecadas == "Incompleta",
        "Calculó horas con movimientos incompletos.");
});

Ejecutar("29. No permite cierre automático con desviaciones", () =>
{
    var r = RelojReglasEvaluacionTH.Evaluar(
        JornadaEjemplo(10, 0, 17, 0));

    Verificar(
        r.RequiereRevisionTH &&
        !r.PermiteCierreAutomatico,
        "Permitió cierre sin validación de TH.");
});



// ======================================================
// RESUMEN
// ======================================================

Console.WriteLine();
Console.WriteLine("====================================");
Console.WriteLine("RESULTADOS DE PRUEBAS AUTOMATIZADAS");
Console.WriteLine("====================================");
Console.WriteLine($"Correctas: {correctas}");
Console.WriteLine($"Fallidas:  {fallidas}");
Console.WriteLine($"Total:     {correctas + fallidas}");
Console.WriteLine("====================================");

if (fallidas > 0)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("RESULTADO: PRUEBAS FALLIDAS");
}
else
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("RESULTADO: TODAS LAS PRUEBAS CORRECTAS");
}

Console.ResetColor();

return fallidas == 0 ? 0 : 1;
