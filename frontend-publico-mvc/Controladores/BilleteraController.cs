using FrontendPublico.Extensiones;
using FrontendPublico.Modelos.Vistas;
using FrontendPublico.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrontendPublico.Controladores;

[Authorize]
public class BilleteraController : Controller
{
    private readonly IServicioMonedas _monedas;

    public BilleteraController(IServicioMonedas monedas)
    {
        _monedas = monedas;
    }

    public async Task<IActionResult> Indice()
    {
        var usuarioId = User.ObtenerUsuarioId();
        return View(new ModeloBilletera
        {
            Saldo = await _monedas.ObtenerSaldoAsync(usuarioId),
            Transacciones = await _monedas.ObtenerTransaccionesAsync(usuarioId)
        });
    }
}
