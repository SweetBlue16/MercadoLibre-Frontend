document.addEventListener("DOMContentLoaded", () => {
    const imagen = document.querySelector(".portada");
    const archivo = document.getElementById("ArchivoId");

    if (!imagen || !archivo) {
        return;
    }

    const urlWebApi = (imagen.dataset.url || "").replace(/\/$/, "").replace(/\/api$/i, "");

    function cargarImagen() {
        const archivoId = archivo.value;

        if (!archivoId) {
            imagen.src = "/images/temp.png";
            return;
        }

        imagen.src = `${urlWebApi}/api/archivos/${archivoId}`;
    }

    archivo.addEventListener("change", cargarImagen);
    cargarImagen();
});
