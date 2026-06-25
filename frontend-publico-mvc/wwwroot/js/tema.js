// Conmutador del tema adaptativo (claro/oscuro) con persistencia en localStorage
(function () {
    var boton = document.getElementById("botonTema");
    if (!boton) return;

    function actualizarIcono() {
        var tema = document.documentElement.getAttribute("data-bs-theme");
        boton.innerHTML = tema === "dark"
            ? '<i class="bi bi-sun"></i>'
            : '<i class="bi bi-moon-stars"></i>';
    }

    boton.addEventListener("click", function () {
        var actual = document.documentElement.getAttribute("data-bs-theme");
        var nuevo = actual === "dark" ? "light" : "dark";
        document.documentElement.setAttribute("data-bs-theme", nuevo);
        localStorage.setItem("tema", nuevo);
        actualizarIcono();
    });

    actualizarIcono();
})();
