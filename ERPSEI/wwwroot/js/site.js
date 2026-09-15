function manejarSesionExpiradaGlobal() {

    if (typeof window.mostrarSesionExpirada === "function") {
        window.mostrarSesionExpirada();
    }
}

$(document).ajaxError(function (event, xhr) {

    if (
        xhr.status === 401 ||
        xhr.status === 403
    ) {
        manejarSesionExpiradaGlobal();
    }
});

$(document).ajaxComplete(function (event, xhr) {

    if (
        xhr.status === 401 ||
        xhr.status === 403
    ) {
        manejarSesionExpiradaGlobal();
        return;
    }

    const responseUrl =
        xhr.responseURL ||
        xhr.getResponseHeader("X-Response-URL") ||
        "";

    if (
        responseUrl &&
        responseUrl
            .toLowerCase()
            .includes("/identity/account/login")
    ) {
        manejarSesionExpiradaGlobal();
    }
});

const fetchOriginal = window.fetch.bind(window);

window.fetch = async function (...args) {

    const response =
        await fetchOriginal(...args);

    if (
        response.status === 401 ||
        response.status === 403
    ) {
        manejarSesionExpiradaGlobal();
        return response;
    }

    if (
        response.redirected &&
        response.url &&
        response.url
            .toLowerCase()
            .includes("/identity/account/login")
    ) {
        manejarSesionExpiradaGlobal();
    }

    return response;
};