using Microsoft.AspNetCore.Mvc;

namespace FrontendPublico.Controladores;

public class ErroresController : Controller
{
    /// <summary>Página de error genérica (degradación controlada, RNF10).</summary>
    public IActionResult Error()
    {
        Response.StatusCode = StatusCodes.Status500InternalServerError;
        return View();
    }

    /// <summary>Re-ejecución de respuestas de error sin cuerpo (404, 400, 403...).</summary>
    [Route("Errores/Codigo/{codigoEstado:int}")]
    public IActionResult Codigo(int codigoEstado)
    {
        Response.StatusCode = codigoEstado;
        return codigoEstado == StatusCodes.Status404NotFound
            ? View("NoEncontrado")
            : View("Error");
    }

    public IActionResult NoEncontrado()
    {
        Response.StatusCode = StatusCodes.Status404NotFound;
        return View();
    }
}
