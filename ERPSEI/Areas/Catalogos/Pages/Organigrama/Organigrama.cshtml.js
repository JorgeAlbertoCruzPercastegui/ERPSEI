document.addEventListener("DOMContentLoaded", function () {
    const dataElement = document.getElementById("organigramaData");
    const container = document.getElementById("organigramaContainer");
    const inputBuscar = document.getElementById("txtBuscarOrganigrama");
    const btnExpandir = document.getElementById("btnExpandirTodo");
    const btnContraer = document.getElementById("btnContraerTodo");
    const btnSinJefe = document.getElementById("btnSinJefe");
    const panelSinJefe = document.getElementById("panelSinJefe");
    const btnCerrarSinJefe = document.getElementById("btnCerrarSinJefe");

    if (!dataElement || !container) return;

    let data = [];

    try {
        data = JSON.parse(dataElement.textContent || "[]");
    } catch {
        data = [];
    }

    if (!data || data.length === 0) {
        container.innerHTML = `
            <div class="org-empty">
                No hay jerarquía configurada. Revisa la asignación de jefe directo en empleados.
            </div>
        `;
    } else {
        renderizar(data);
        contraerTodo();
    }

    function normalizarTexto(texto) {
        return (texto || "")
            .toString()
            .toLowerCase()
            .normalize("NFD")
            .replace(/[\u0300-\u036f]/g, "");
    }

    function obtenerTodosLosEmpleados(lista, resultado = []) {
        lista.forEach(emp => {
            resultado.push(emp);

            if (emp.Subordinados && emp.Subordinados.length > 0) {
                obtenerTodosLosEmpleados(emp.Subordinados, resultado);
            }
        });

        return resultado;
    }

    function crearNodo(empleado) {
        const tieneHijos = empleado.Subordinados && empleado.Subordinados.length > 0;

        const nodo = document.createElement("div");
        nodo.className = "org-node";

        nodo.innerHTML = `
            <div class="org-card"
                 data-id="${empleado.Id}"
                 data-search="${normalizarTexto(empleado.NombreCompleto)} ${normalizarTexto(empleado.Puesto)} ${normalizarTexto(empleado.Area)}">
                <div class="org-avatar">
                    <i class="bi bi-person-circle"></i>
                </div>

                <div class="org-info">
                    <div class="org-name">${empleado.NombreCompleto}</div>
                    <div class="org-position">${empleado.Puesto}</div>
                    <div class="org-area">${empleado.Area}</div>
                </div>

                ${tieneHijos
                ? `<button type="button" class="org-toggle" title="Expandir/contraer">
                                <i class="bi bi-chevron-down"></i>
                           </button>`
                : ""
            }
            </div>
        `;

        if (tieneHijos) {
            const hijosContainer = document.createElement("div");
            hijosContainer.className = "org-children";

            empleado.Subordinados.forEach(hijo => {
                hijosContainer.appendChild(crearNodo(hijo));
            });

            nodo.appendChild(hijosContainer);

            const toggle = nodo.querySelector(".org-toggle");

            toggle.addEventListener("click", function (e) {
                e.preventDefault();
                e.stopPropagation();

                hijosContainer.classList.toggle("org-children-collapsed");
                toggle.classList.toggle("collapsed");
            });
        }

        return nodo;
    }

    function renderizar(lista) {
        container.innerHTML = "";

        if (!lista || lista.length === 0) {
            container.innerHTML = `
                <div class="org-empty">
                    No se encontraron empleados con ese criterio de búsqueda.
                </div>
            `;
            return;
        }

        lista.forEach(empleado => {
            container.appendChild(crearNodo(empleado));
        });
    }

    function expandirTodo() {
        document.querySelectorAll(".org-children").forEach(x => {
            x.classList.remove("org-children-collapsed");
        });

        document.querySelectorAll(".org-toggle").forEach(x => {
            x.classList.remove("collapsed");
        });
    }

    function contraerTodo() {
        document.querySelectorAll(".org-children").forEach(x => {
            x.classList.add("org-children-collapsed");
        });

        document.querySelectorAll(".org-toggle").forEach(x => {
            x.classList.add("collapsed");
        });
    }

    function buscar(texto) {
        const filtro = normalizarTexto(texto);

        if (filtro.length < 3) {
            renderizar(data);
            contraerTodo();
            return;
        }

        const todos = obtenerTodosLosEmpleados(data, []);

        const encontrados = todos.filter(emp => {
            const contenido = `${normalizarTexto(emp.NombreCompleto)} ${normalizarTexto(emp.Puesto)} ${normalizarTexto(emp.Area)}`;
            return contenido.includes(filtro);
        });

        const resultadosPlanos = encontrados.map(emp => ({
            ...emp,
            Subordinados: []
        }));

        renderizar(resultadosPlanos);
    }

    inputBuscar?.addEventListener("input", function () {
        buscar(this.value);
    });

    btnExpandir?.addEventListener("click", function () {
        inputBuscar.value = "";
        renderizar(data);
        expandirTodo();
    });

    btnContraer?.addEventListener("click", function () {
        inputBuscar.value = "";
        renderizar(data);
        contraerTodo();
    });

    btnSinJefe?.addEventListener("click", function () {
        panelSinJefe?.classList.toggle("d-none");
    });

    btnCerrarSinJefe?.addEventListener("click", function () {
        panelSinJefe?.classList.add("d-none");
    });
});