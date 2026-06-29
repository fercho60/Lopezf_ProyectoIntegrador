using FrontendPublico.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace FrontendPublico.Controladores;

public class RankingController : Controller
{
    private readonly IServicioMonedas _monedas;

    public RankingController(IServicioMonedas monedas)
    {
        _monedas = monedas;
    }

    public async Task<IActionResult> Indice()
    {
        var ranking = await _monedas.ObtenerRankingAsync();
        return View(ranking);
    }
}
