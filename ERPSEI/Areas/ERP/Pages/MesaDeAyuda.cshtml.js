document.addEventListener("DOMContentLoaded", function () {

    const categoria =
        document.getElementById("CategoryId");

    const subcategoria =
        document.getElementById("SubcategoryId");


    if (!categoria || !subcategoria) {
        return;
    }


    categoria.addEventListener(
        "change",
        async function () {

            const categoryId = this.value;


            subcategoria.innerHTML =
                '<option value="">Selecciona...</option>';


            if (!categoryId || categoryId === "0") {
                return;
            }


            try {

                const response =
                    await fetch(
                        "?handler=Subcategorias&categoryId="
                        + encodeURIComponent(categoryId)
                    );


                if (!response.ok) {
                    console.error(
                        "No fue posible obtener las subcategorías."
                    );

                    return;
                }


                const datos =
                    await response.json();


                datos.forEach(
                    function (item) {

                        const option =
                            document.createElement(
                                "option"
                            );

                        option.value =
                            item.id;

                        option.textContent =
                            item.nombre;

                        subcategoria.appendChild(
                            option
                        );

                    }
                );

            }
            catch (error) {

                console.error(
                    "Error al cargar subcategorías:",
                    error
                );

            }

        }
    );

});