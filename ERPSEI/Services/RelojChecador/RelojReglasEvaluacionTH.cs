
using System;
using System.Collections.Generic;

namespace ERPSEI.Services.RelojChecador;

public static class RelojReglasEvaluacionTH
{
    public sealed class DatosJornada
    {
        public TimeSpan? EntradaProgramada { get; set; }
        public TimeSpan? LimiteRetardo { get; set; }
        public TimeSpan? SalidaProgramada { get; set; }

        public DateTime? EntradaReal { get; set; }
        public DateTime? SalidaComerReal { get; set; }
        public DateTime? RegresoComerReal { get; set; }
        public DateTime? SalidaReal { get; set; }

        public bool ConflictosSinResolver { get; set; }
        public bool ContextoTHRequiereRevision { get; set; }
    }

    public sealed class Resultado
    {
        // Compatibilidad con resultados existentes.
        public string Puntualidad { get; set; } = "NoEvaluable";
        public string EstadoJornada { get; set; } = "JornadaIncompleta";
        public string EstadoSalida { get; set; } = "NoEvaluable";

        // Nuevas dimensiones independientes.
        public string IntegridadChecadas { get; set; } = "Incompleta";
        public string CumplimientoHorario { get; set; } = "NoEvaluable";

        // Solo calculados con una secuencia íntegra.
        public TimeSpan? TiempoTotalRegistrado { get; set; }
        public TimeSpan? TiempoComidaRegistrado { get; set; }
        public TimeSpan? TiempoEfectivoRegistrado { get; set; }

        // Desviaciones preliminares, no sanciones.
        public int? MinutosRetardoPotencial { get; set; }
        public int? MinutosSalidaAnticipadaPotencial { get; set; }

        public bool RequiereRevisionTH { get; set; } = true;
        public bool PermiteCierreAutomatico { get; set; } = false;

        public List<string> Observaciones { get; set; } = new();
    }

    public static Resultado Evaluar(DatosJornada datos)
    {
        ArgumentNullException.ThrowIfNull(datos);

        var resultado = new Resultado();

        // La revisión TH sigue siendo obligatoria.
        resultado.RequiereRevisionTH = true;
        resultado.PermiteCierreAutomatico = false;

        // 1. CONFLICTOS ADMINISTRATIVOS
        if (datos.ConflictosSinResolver)
        {
            resultado.EstadoJornada = "RequiereConciliacion";
            resultado.IntegridadChecadas = "RequiereConciliacion";
            resultado.CumplimientoHorario = "NoEvaluable";

            resultado.Observaciones.Add(
                "Existen movimientos duplicados sin " +
                "resolución administrativa.");

            return resultado;
        }

        bool entrada = datos.EntradaReal.HasValue;
        bool salidaComer = datos.SalidaComerReal.HasValue;
        bool regresoComer = datos.RegresoComerReal.HasValue;
        bool salida = datos.SalidaReal.HasValue;

        bool sinMovimientos =
            !entrada && !salidaComer &&
            !regresoComer && !salida;

        bool cuatroMovimientos =
            entrada && salidaComer &&
            regresoComer && salida;

        bool secuenciaCorrecta =
            cuatroMovimientos &&
            datos.EntradaReal!.Value <
                datos.SalidaComerReal!.Value &&
            datos.SalidaComerReal.Value <
                datos.RegresoComerReal!.Value &&
            datos.RegresoComerReal.Value <
                datos.SalidaReal!.Value;

        // 2. INTEGRIDAD DE CHECADAS
        if (sinMovimientos)
        {
            resultado.EstadoJornada = "SinMovimientos";
            resultado.IntegridadChecadas = "SinMovimientos";

            resultado.Observaciones.Add(
                "No existen movimientos confirmados. " +
                "Esto no representa automáticamente una falta.");
        }
        else if (!cuatroMovimientos)
        {
            resultado.EstadoJornada = "JornadaIncompleta";
            resultado.IntegridadChecadas = "Incompleta";

            resultado.Observaciones.Add(
                "Faltan movimientos confirmados " +
                "para completar la jornada.");
        }
        else if (!secuenciaCorrecta)
        {
            resultado.EstadoJornada = "SecuenciaInconsistente";
            resultado.IntegridadChecadas = "SecuenciaInconsistente";

            resultado.Observaciones.Add(
                "Los movimientos no siguen " +
                "el orden cronológico esperado.");
        }
        else
        {
            resultado.EstadoJornada = "JornadaCompleta";
            resultado.IntegridadChecadas = "Completa";

            // 3. TIEMPOS REGISTRADOS
            TimeSpan total =
                datos.SalidaReal!.Value -
                datos.EntradaReal!.Value;

            TimeSpan comida =
                datos.RegresoComerReal!.Value -
                datos.SalidaComerReal!.Value;

            resultado.TiempoTotalRegistrado = total;
            resultado.TiempoComidaRegistrado = comida;
            resultado.TiempoEfectivoRegistrado =
                total - comida;
        }

        // 4. HORARIO APLICABLE
        bool horarioDisponible =
            datos.EntradaProgramada.HasValue &&
            datos.LimiteRetardo.HasValue &&
            datos.SalidaProgramada.HasValue;

        // Los turnos nocturnos necesitan una regla
        // específica con fecha y zona horaria.
        bool horarioMismoDia =
            horarioDisponible &&
            datos.SalidaProgramada!.Value >
                datos.EntradaProgramada!.Value;

        if (horarioDisponible && !horarioMismoDia)
        {
            resultado.EstadoSalida = "RequiereRevisionHorario";
            resultado.CumplimientoHorario =
                "RequiereRevisionHorario";

            resultado.Observaciones.Add(
                "El horario requiere evaluación especial " +
                "por posible cruce de medianoche.");

            return resultado;
        }

        // 5. PUNTUALIDAD
        if (entrada && horarioMismoDia)
        {
            TimeSpan entradaReal =
                datos.EntradaReal!.Value.TimeOfDay;

            TimeSpan limite =
                datos.LimiteRetardo!.Value;

            if (entradaReal <= limite)
            {
                resultado.Puntualidad = "Puntual";
            }
            else
            {
                resultado.Puntualidad = "RetardoPotencial";

                // Redondeo hacia arriba para registrar
                // cualquier fracción de minuto excedida.
                resultado.MinutosRetardoPotencial =
                    (int)Math.Ceiling(
                        (entradaReal - limite).TotalMinutes);

                resultado.Observaciones.Add(
                    "La entrada supera el límite de " +
                    "tolerancia. Requiere revisión TH.");
            }
        }

        // 6. SALIDA ANTICIPADA
        if (salida && horarioMismoDia)
        {
            TimeSpan salidaReal =
                datos.SalidaReal!.Value.TimeOfDay;

            TimeSpan salidaEsperada =
                datos.SalidaProgramada!.Value;

            if (salidaReal < salidaEsperada)
            {
                resultado.EstadoSalida =
                    "SalidaAnticipadaPotencial";

                resultado.MinutosSalidaAnticipadaPotencial =
                    (int)Math.Ceiling(
                        (salidaEsperada - salidaReal)
                            .TotalMinutes);

                resultado.Observaciones.Add(
                    "La salida es anterior al horario " +
                    "programado. No se genera incidencia " +
                    "automáticamente.");
            }
            else
            {
                resultado.EstadoSalida = "SalidaEnHorario";
            }
        }

        // 7. CUMPLIMIENTO PRELIMINAR
        if (horarioMismoDia)
        {
            if (!secuenciaCorrecta)
            {
                resultado.CumplimientoHorario =
                    "NoConcluyente";
            }
            else if (
                resultado.Puntualidad == "RetardoPotencial" ||
                resultado.EstadoSalida ==
                    "SalidaAnticipadaPotencial")
            {
                resultado.CumplimientoHorario =
                    "DesviacionesPotenciales";
            }
            else
            {
                // No equivale a jornada pagable ni
                // a cumplimiento laboral aprobado.
                resultado.CumplimientoHorario =
                    "SinDesviacionesDetectadas";
            }
        }

        // 8. CONTEXTO DE TALENTO HUMANO
        if (datos.ContextoTHRequiereRevision)
        {
            resultado.Observaciones.Add(
                "Existen ausencias, permisos o modalidades " +
                "que requieren revisión contextual de TH.");
        }

        // Nunca generar faltas, descuentos ni
        // cierre automático desde estas reglas.
        return resultado;
    }
}
