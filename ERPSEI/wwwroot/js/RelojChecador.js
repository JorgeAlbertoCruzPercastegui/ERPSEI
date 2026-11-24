
(() => {
    "use strict";

    const $ = id => document.getElementById(id);
    const baseUrl = window.location.pathname;

    const tipos = [
        ["Entrada", "Entrada"],
        ["SalidaComer", "Salida a comer"],
        ["RegresoComer", "Regreso de comer"],
        ["Salida", "Salida laboral"]
    ];

    let estado = null;
    let movimientos = [];
    let stream = null;
    let fotoBlob = null;
    let fotoTomadaEn = 0;
    let registrando = false;
    let claveSolicitud = null;

    function alerta(texto, tipo = "info") {
        const elemento = $("rcAlerta");
        elemento.className = `alert alert-${tipo}`;
        elemento.textContent = texto;
    }

    function badge(id, texto, valido) {
        const elemento = $(id);
        elemento.className = valido
            ? "badge bg-success"
            : "badge bg-secondary";
        elemento.textContent = texto;
    }

    function horaLocal(fechaUtc) {
        return new Date(fechaUtc).toLocaleTimeString(
            "es-MX",
            {
                timeZone: "America/Mexico_City",
                hour: "2-digit",
                minute: "2-digit",
                hour12: false
            }
        );
    }


    function siguienteCodigo() {
        // Una jornada cerrada no permite
        // registrar más movimientos.
        if (estado?.jornada?.estado === "Cerrada") {
            return null;
        }

        const existentes = new Set(
            movimientos.map(x => x.tipo)
        );

        for (const [codigo] of tipos) {
            if (!existentes.has(codigo)) {
                return codigo;
            }
        }

        return null;
    }


    function nombreMovimiento(codigo) {
        return tipos.find(x => x[0] === codigo)?.[1]
            || "Movimiento";
    }

    function actualizarBoton() {
        const boton = $("rcBtnRegistrar");
        if (!boton) return;

        const codigo = siguienteCodigo();

        boton.disabled =
            registrando ||
            !codigo ||
            !fotoBlob ||
            Date.now() - fotoTomadaEn > 60000;

        boton.textContent =
            estado?.jornada?.estado === "Cerrada"
                ? "Jornada finalizada hoy"
                : codigo
                    ? `Registrar ${nombreMovimiento(codigo).toLowerCase()}`
                    : "Jornada finalizada";
    }

    function pintarMovimientos() {
        const contenedor = $("rcMovimientos");
        contenedor.replaceChildren();

        for (const [codigo, nombre] of tipos) {
            const registro = movimientos.find(
                x => x.tipo === codigo
            );

            const fila = document.createElement("div");
            fila.className = "rc-movement";

            const icono = document.createElement("i");
            icono.className = registro
                ? "bi bi-check-circle-fill text-success"
                : "bi bi-circle text-secondary";

            const titulo = document.createElement("span");
            titulo.textContent = nombre;

            const hora = document.createElement("strong");
            hora.textContent = registro
                ? horaLocal(registro.fechaHoraUtc)
                : "—";

            fila.append(icono, titulo, hora);
            contenedor.appendChild(fila);
        }

        $("rcSiguiente").textContent = nombreMovimiento(
            siguienteCodigo() || "Finalizada"
        );

        actualizarBoton();
    }

    function formatoTiempo(totalSegundos) {
        const total = Math.max(
            0, Math.floor(totalSegundos)
        );

        const h = Math.floor(total / 3600);
        const m = Math.floor((total % 3600) / 60);
        const s = total % 60;

        return [h, m, s]
            .map(x => String(x).padStart(2, "0"))
            .join(":");
    }

    function actualizarCronometro() {
        const jornada = estado?.jornada;

        if (!jornada?.inicioUtc) {
            $("rcCronometro").textContent = "00:00:00";
            $("rcBarra").style.width = "0%";
            $("rcTiempoDescripcion").textContent =
                "Sin jornada activa";
            return;
        }


        // Si la jornada está cerrada, mostrar
        // el resultado definitivo de SQL Server.
        if (jornada.estado === "Cerrada") {
            const minutos = Math.max(
                0,
                Number(jornada.minutosEfectivos) || 0
            );

            const objetivo = Math.max(
                1,
                Number(jornada.objetivoMinutos) || 480
            );

            $("rcCronometro").textContent =
                formatoTiempo(minutos * 60);

            $("rcBarra").style.width =
                `${Math.min(100, minutos / objetivo * 100)}%`;

            $("rcMeta").textContent =
                `Meta de referencia: ${objetivo / 60} horas`;

            $("rcTiempoDescripcion").textContent =
                jornada.requiereRevision
                    ? "Jornada finalizada · Pendiente de revisión por TH"
                    : "Jornada finalizada";

            return;
        }


        const inicio = new Date(
            jornada.inicioUtc
        ).getTime();

        if (!Number.isFinite(inicio)) return;

        const pausa = movimientos.find(
            x => x.tipo === "SalidaComer"
        );

        const regreso = movimientos.find(
            x => x.tipo === "RegresoComer"
        );

        const ahora = Date.now();

        // Una salida a comer sin regreso mantiene
        // el contador de trabajo detenido.
        const finTrabajo = pausa && !regreso
            ? new Date(pausa.fechaHoraUtc).getTime()
            : ahora;

        let descanso = 0;

        if (pausa && regreso) {
            descanso =
                new Date(regreso.fechaHoraUtc).getTime() -
                new Date(pausa.fechaHoraUtc).getTime();
        }

        const milisegundos = Math.max(
            0, finTrabajo - inicio - descanso
        );

        const segundos = milisegundos / 1000;
        const objetivoMinutos =
            jornada.objetivoMinutos || 480;

        $("rcCronometro").textContent =
            formatoTiempo(segundos);

        $("rcBarra").style.width =
            `${Math.min(100,
                segundos / (objetivoMinutos * 60) * 100
            )}%`;

        $("rcMeta").textContent =
            `Meta de referencia: ${objetivoMinutos / 60} horas`;

        $("rcTiempoDescripcion").textContent =
            pausa && !regreso
                ? "Descanso en curso"
                : "Tiempo laboral acumulado estimado";
    }

    async function cargarEstado() {
        const respuesta = await fetch(
            `${baseUrl}?handler=Estado`,
            {
                credentials: "same-origin",
                cache: "no-store"
            }
        );

        const data = await respuesta.json();

        if (!respuesta.ok || !data.success) {
            throw new Error(
                data.message || "No se pudo consultar el estado."
            );
        }

        estado = data;
        movimientos = data.movimientos || [];

        $("rcNombre").textContent =
            data.colaborador?.nombre || "—";

        $("rcOficina").textContent =
            data.colaborador?.oficina || "—";

        $("rcEstadoJornada").textContent =
            data.jornada?.estado === "Cerrada"
                ? "Jornada finalizada hoy"
                : data.jornada?.estado === "Abierta"
                    ? "Jornada abierta"
                    : "Sin jornada abierta";

        pintarMovimientos();
        actualizarCronometro();
    }

    function obtenerUbicacion() {
        return new Promise((resolver, rechazar) => {
            if (!navigator.geolocation) {
                rechazar(new Error(
                    "Este navegador no admite ubicación."
                ));
                return;
            }

            navigator.geolocation.getCurrentPosition(
                resolver,
                rechazar,
                {
                    enableHighAccuracy: true,
                    timeout: 20000,
                    maximumAge: 0
                }
            );
        });
    }

    function token() {
        return document.querySelector(
            'input[name="__RequestVerificationToken"]'
        )?.value;
    }

    async function verificarGps() {
        $("rcBtnGps").disabled = true;

        try {
            const posicion = await obtenerUbicacion();

            const datos = new FormData();

            datos.append(
                "Latitud",
                String(posicion.coords.latitude)
            );

            datos.append(
                "Longitud",
                String(posicion.coords.longitude)
            );

            datos.append(
                "PrecisionMetros",
                String(posicion.coords.accuracy)
            );

            datos.append(
                "CapturadaUtc",
                new Date(posicion.timestamp).toISOString()
            );

            const respuesta = await fetch(
                `${baseUrl}?handler=ValidarUbicacion`,
                {
                    method: "POST",
                    credentials: "same-origin",
                    headers: {
                        "RequestVerificationToken": token()
                    },
                    body: datos
                }
            );

            const resultado = await respuesta.json();

            const valido =
                resultado.autorizado === true;

            badge(
                "rcGpsBadge",
                valido ? "Validada" : "No validada",
                valido
            );

            $("rcGpsDetalle").textContent =
                `${resultado.message || "Sin resultado."} ` +
                `Precisión reportada: ` +
                `${posicion.coords.accuracy.toFixed(1)} m.`;

            alerta(
                resultado.message || "Validación finalizada.",
                valido ? "success" : "warning"
            );
        } catch (error) {
            badge("rcGpsBadge", "No validada", false);

            $("rcGpsDetalle").textContent =
                error.message || "No se obtuvo ubicación.";

            alerta(
                "No fue posible verificar la ubicación.",
                "warning"
            );
        } finally {
            $("rcBtnGps").disabled = false;
        }
    }

    function detenerCamara() {
        stream?.getTracks().forEach(
            pista => pista.stop()
        );

        stream = null;

        if ($("rcVideo"))
            $("rcVideo").srcObject = null;
    }

    function limpiarFoto() {
        fotoBlob = null;
        fotoTomadaEn = 0;
        claveSolicitud = null;

        $("rcVistaFoto").hidden = true;
        $("rcVistaFoto").removeAttribute("src");

        badge("rcCamaraBadge", "Pendiente", false);
        actualizarBoton();
    }

    async function activarCamara() {
        $("rcBtnCamara").disabled = true;

        try {
            detenerCamara();
            limpiarFoto();

            stream = await navigator.mediaDevices
                .getUserMedia({
                    video: {
                        facingMode: "user",
                        width: { ideal: 640 },
                        height: { ideal: 480 }
                    },
                    audio: false
                });

            const video = $("rcVideo");
            video.srcObject = stream;

            $("rcCamaraContenedor").hidden = false;
            video.hidden = false;

            $("rcCamaraDetalle").textContent =
                "Cámara activa. Captura una fotografía.";
        } catch (error) {
            detenerCamara();

            alerta(
                "No se pudo activar la cámara. Revisa los permisos.",
                "warning"
            );
        } finally {
            $("rcBtnCamara").disabled = false;
        }
    }

    async function capturarFoto() {
        if (!stream) return;

        const video = $("rcVideo");
        const canvas = $("rcCanvas");

        if (!video.videoWidth || !video.videoHeight) {
            alerta("La cámara no está lista.", "warning");
            return;
        }

        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;

        const contexto = canvas.getContext("2d");
        if (!contexto) return;

        contexto.drawImage(
            video, 0, 0,
            canvas.width, canvas.height
        );

        const blob = await new Promise(
            resolver => canvas.toBlob(
                resolver, "image/jpeg", 0.85
            )
        );

        if (!blob) {
            alerta("No se pudo capturar la fotografía.", "danger");
            return;
        }

        fotoBlob = blob;
        fotoTomadaEn = Date.now();
        claveSolicitud = crypto.randomUUID();

        const imagen = $("rcVistaFoto");

        imagen.src = URL.createObjectURL(blob);
        imagen.onload = () =>
            URL.revokeObjectURL(imagen.src);

        imagen.hidden = false;
        video.hidden = true;

        detenerCamara();

        badge("rcCamaraBadge", "Capturada", true);

        $("rcCamaraDetalle").textContent =
            "Fotografía lista para registrar el movimiento.";

        actualizarBoton();

        alerta(
            "Fotografía capturada. Puedes registrar el siguiente movimiento.",
            "success"
        );
    }

    async function registrarMovimiento() {
        if (registrando || !fotoBlob) return;

        if (Date.now() - fotoTomadaEn > 60000) {
            alerta(
                "La fotografía expiró. Captura una nueva.",
                "warning"
            );
            limpiarFoto();
            return;
        }

        const codigo = siguienteCodigo();
        if (!codigo) return;

        registrando = true;
        actualizarBoton();

        try {
            alerta("Obteniendo ubicación actual...", "info");

            // No reutilizar la ubicación de una
            // verificación previa.
            const posicion = await obtenerUbicacion();

            const datos = new FormData();

            datos.append("TipoMovimiento", codigo);
            datos.append(
                "Latitud",
                String(posicion.coords.latitude)
            );
            datos.append(
                "Longitud",
                String(posicion.coords.longitude)
            );
            datos.append(
                "PrecisionMetros",
                String(posicion.coords.accuracy)
            );
            datos.append(
                "CapturadaUtc",
                new Date(posicion.timestamp).toISOString()
            );
            datos.append(
                "IdempotencyKey",
                claveSolicitud
            );

            datos.append(
                "Fotografia",
                fotoBlob,
                "captura.jpg"
            );

            alerta("Registrando movimiento...", "info");

            const respuesta = await fetch(
                `${baseUrl}?handler=RegistrarMovimiento`,
                {
                    method: "POST",
                    credentials: "same-origin",
                    headers: {
                        "RequestVerificationToken": token()
                    },
                    body: datos
                }
            );

            const resultado = await respuesta.json();

            if (!respuesta.ok || !resultado.success) {
                throw new Error(
                    resultado.message ||
                    "No se pudo registrar el movimiento."
                );
            }

            limpiarFoto();

            await cargarEstado();

            badge(
                "rcGpsBadge",
                "Ubicación utilizada",
                true
            );

            if (resultado.requiereRevision) {
                alerta(
                    "Salida registrada. La jornada quedó pendiente de revisión por TH.",
                    "warning"
                );
            } else {
                alerta(
                    "Movimiento registrado correctamente.",
                    "success"
                );
            }

            $("rcCamaraDetalle").textContent =
                "Captura nuevamente para el siguiente movimiento.";

        } catch (error) {
            alerta(
                error.message ||
                "No fue posible registrar la checada.",
                "danger"
            );
        } finally {
            registrando = false;
            actualizarBoton();
        }
    }

    document.addEventListener(
        "DOMContentLoaded",
        async () => {
            if (!$("rcCronometro")) return;

            $("rcBtnGps").addEventListener(
                "click", verificarGps
            );

            $("rcBtnCamara").addEventListener(
                "click", activarCamara
            );

            $("rcBtnFoto").addEventListener(
                "click", capturarFoto
            );

            $("rcBtnRegistrar").addEventListener(
                "click", registrarMovimiento
            );

            try {
                await cargarEstado();
                alerta(
                    "Información cargada. Captura una fotografía para comenzar.",
                    "success"
                );
            } catch (error) {
                alerta(error.message, "danger");
            }

            window.setInterval(() => {
                actualizarCronometro();
                actualizarBoton();
            }, 1000);

            window.addEventListener(
                "pagehide", detenerCamara
            );
        }
    );
})();
