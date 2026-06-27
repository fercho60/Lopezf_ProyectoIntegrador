using System.ComponentModel.DataAnnotations;

namespace FrontendPublico.Modelos.Vistas;

public class ModeloCrearPrediccion
{
    [Required]
    public long PartidoId { get; set; }

    [Display(Name = "Pronóstico")]
    [Required(ErrorMessage = "Debe elegir un pronóstico: local, empate o visitante.")]
    public Pronostico? Pronostico { get; set; }

    [Display(Name = "Monto en UTNGolCoin")]
    [Required(ErrorMessage = "Debe indicar el monto a apostar.")]
    [Range(1, 1000, ErrorMessage = "El monto debe ser de al menos 1 UTNGolCoin.")]
    public decimal Monto { get; set; } = 1;

    /// <summary>Partido asociado, cargado por el controlador para renderizar la vista.</summary>
    public Partido? Partido { get; set; }

    /// <summary>Saldo actual del usuario, para mostrar el disponible en el formulario.</summary>
    public decimal SaldoDisponible { get; set; }
}
