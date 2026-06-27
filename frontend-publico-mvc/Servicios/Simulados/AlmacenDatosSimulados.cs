using System.Security.Cryptography;
using System.Text;
using FrontendPublico.Modelos;

namespace FrontendPublico.Servicios.Simulados;

/// <summary>Usuario con credenciales, solo para la simulación en memoria.</summary>
public class UsuarioConCredenciales : Usuario
{
    public string HashContrasena { get; set; } = string.Empty;
}

/// <summary>
/// Almacén en memoria que sustituye a los dos backends durante la fase UI-primero.
/// La semilla cubre un subconjunto del torneo (4 grupos, 16 selecciones y 14 partidos);
/// el seed oficial de 48 selecciones, 12 grupos y 104 partidos lo provee el docente (RF28)
/// y lo cargará el Servicio de Estadísticas real.
/// </summary>
public class AlmacenDatosSimulados
{
    public object Candado { get; } = new();

    public List<Seleccion> Selecciones { get; } = new();
    public List<Grupo> Grupos { get; } = new();
    public List<Partido> Partidos { get; } = new();
    public List<UsuarioConCredenciales> Usuarios { get; } = new();
    public Dictionary<long, decimal> Saldos { get; } = new();
    public Dictionary<long, List<Transaccion>> Transacciones { get; } = new();
    public List<Prediccion> Predicciones { get; } = new();
    public Dictionary<long, DateOnly> UltimoBonoDiario { get; } = new();

    private long _siguienteUsuarioId = 100;
    private long _siguienteTransaccionId = 1000;
    private long _siguientePrediccionId = 500;

    public long SiguienteUsuarioId() => _siguienteUsuarioId++;
    public long SiguienteTransaccionId() => _siguienteTransaccionId++;
    public long SiguientePrediccionId() => _siguientePrediccionId++;

    public static string CalcularHash(string contrasena) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(contrasena)));

    public AlmacenDatosSimulados()
    {
        SembrarSeleccionesYGrupos();
        SembrarPartidos();
        SembrarUsuarios();
    }

    private void SembrarSeleccionesYGrupos()
    {
        long id = 1;
        Seleccion Crear(string nombre, string codigo, string bandera, string grupo) =>
            AgregarSeleccion(new Seleccion { Id = id++, Nombre = nombre, CodigoPais = codigo, Bandera = bandera, Grupo = grupo });

        var mexico = Crear("México", "MX", "🇲🇽", "Grupo A");
        var ecuador = Crear("Ecuador", "EC", "🇪🇨", "Grupo A");
        var senegal = Crear("Senegal", "SN", "🇸🇳", "Grupo A");
        var paisesBajos = Crear("Países Bajos", "NL", "🇳🇱", "Grupo A");

        var estadosUnidos = Crear("Estados Unidos", "US", "🇺🇸", "Grupo B");
        var inglaterra = Crear("Inglaterra", "GB", "🏴", "Grupo B");
        var iran = Crear("Irán", "IR", "🇮🇷", "Grupo B");
        var gales = Crear("Gales", "GB", "🏴", "Grupo B");

        var argentina = Crear("Argentina", "AR", "🇦🇷", "Grupo C");
        var polonia = Crear("Polonia", "PL", "🇵🇱", "Grupo C");
        var brasil = Crear("Brasil", "BR", "🇧🇷", "Grupo C");
        var arabiaSaudita = Crear("Arabia Saudita", "SA", "🇸🇦", "Grupo C");

        var canada = Crear("Canadá", "CA", "🇨🇦", "Grupo D");
        var francia = Crear("Francia", "FR", "🇫🇷", "Grupo D");
        var alemania = Crear("Alemania", "DE", "🇩🇪", "Grupo D");
        var espana = Crear("España", "ES", "🇪🇸", "Grupo D");

        Grupos.Add(new Grupo
        {
            Id = 1,
            Nombre = "Grupo A",
            Posiciones = new List<PosicionEquipo>
            {
                Posicion(ecuador, 3, 2, 1, 0, 4, 2),
                Posicion(paisesBajos, 3, 2, 0, 1, 5, 1),
                Posicion(senegal, 3, 2, 0, 1, 5, 4),
                Posicion(mexico, 3, 0, 1, 2, 1, 7),
            }
        });
        Grupos.Add(new Grupo
        {
            Id = 2,
            Nombre = "Grupo B",
            Posiciones = new List<PosicionEquipo>
            {
                Posicion(inglaterra, 3, 2, 1, 0, 9, 2),
                Posicion(estadosUnidos, 3, 1, 2, 0, 2, 1),
                Posicion(iran, 3, 1, 0, 2, 4, 7),
                Posicion(gales, 3, 0, 1, 2, 1, 6),
            }
        });
        Grupos.Add(new Grupo
        {
            Id = 3,
            Nombre = "Grupo C",
            Posiciones = new List<PosicionEquipo>
            {
                Posicion(argentina, 3, 2, 0, 1, 5, 2),
                Posicion(brasil, 3, 2, 0, 1, 6, 3),
                Posicion(polonia, 3, 1, 1, 1, 2, 2),
                Posicion(arabiaSaudita, 3, 1, 0, 2, 3, 5),
            }
        });
        Grupos.Add(new Grupo
        {
            Id = 4,
            Nombre = "Grupo D",
            Posiciones = new List<PosicionEquipo>
            {
                Posicion(francia, 3, 2, 0, 1, 6, 3),
                Posicion(espana, 3, 2, 0, 1, 8, 3),
                Posicion(alemania, 3, 1, 1, 1, 6, 5),
                Posicion(canada, 3, 0, 1, 2, 2, 7),
            }
        });
    }

    private Seleccion AgregarSeleccion(Seleccion seleccion)
    {
        Selecciones.Add(seleccion);
        return seleccion;
    }

    private static PosicionEquipo Posicion(Seleccion s, int j, int g, int e, int p, int gf, int gc) => new()
    {
        Seleccion = s,
        Jugados = j,
        Ganados = g,
        Empatados = e,
        Perdidos = p,
        GolesFavor = gf,
        GolesContra = gc,
        Puntos = g * 3 + e
    };

    private void SembrarPartidos()
    {
        var hoy = DateTime.Today;
        long id = 1;

        Seleccion Buscar(string nombre) => Selecciones.First(s => s.Nombre == nombre);

        void Agregar(string local, string visitante, DateTime fechaHora, string sede, string? grupo,
            EstadoPartido estado, int? golesLocal, int? golesVisitante, decimal cLocal, decimal cEmpate, decimal cVisitante,
            FaseTorneo fase = FaseTorneo.FaseDeGrupos)
        {
            Partidos.Add(new Partido
            {
                Id = id++,
                SeleccionLocal = Buscar(local),
                SeleccionVisitante = Buscar(visitante),
                FechaHora = fechaHora,
                Sede = sede,
                Grupo = grupo,
                Fase = fase,
                Estado = estado,
                GolesLocal = golesLocal,
                GolesVisitante = golesVisitante,
                Cuotas = new CuotasPartido { Local = cLocal, Empate = cEmpate, Visitante = cVisitante }
            });
        }

        // Finalizados (fechas pasadas)
        Agregar("México", "Ecuador", hoy.AddDays(-3).AddHours(18), "Estadio Azteca, Ciudad de México", "Grupo A", EstadoPartido.Finalizado, 0, 2, 2.1m, 3.0m, 3.4m);
        Agregar("Senegal", "Países Bajos", hoy.AddDays(-3).AddHours(21), "MetLife Stadium, Nueva Jersey", "Grupo A", EstadoPartido.Finalizado, 0, 2, 3.1m, 3.2m, 2.0m);
        Agregar("Inglaterra", "Irán", hoy.AddDays(-2).AddHours(16), "SoFi Stadium, Los Ángeles", "Grupo B", EstadoPartido.Finalizado, 6, 2, 1.5m, 4.0m, 6.5m);
        Agregar("Estados Unidos", "Gales", hoy.AddDays(-2).AddHours(22), "AT&T Stadium, Texas", "Grupo B", EstadoPartido.Finalizado, 1, 1, 1.9m, 3.3m, 4.2m);
        Agregar("Argentina", "Arabia Saudita", hoy.AddDays(-1).AddHours(13), "Estadio Akron, Guadalajara", "Grupo C", EstadoPartido.Finalizado, 1, 2, 1.3m, 4.8m, 9.0m);
        Agregar("Francia", "Canadá", hoy.AddDays(-1).AddHours(19), "BC Place, Vancouver", "Grupo D", EstadoPartido.Finalizado, 4, 1, 1.6m, 3.8m, 5.5m);

        // En juego (iniciaron hace poco)
        Agregar("Argentina", "Brasil", DateTime.Now.AddMinutes(-35), "MetLife Stadium, Nueva Jersey", "Grupo C", EstadoPartido.EnJuego, 1, 0, 2.4m, 3.1m, 2.7m);
        Agregar("Francia", "Alemania", DateTime.Now.AddMinutes(-20), "AT&T Stadium, Texas", "Grupo D", EstadoPartido.EnJuego, 2, 2, 2.5m, 3.2m, 2.6m);

        // Programados (fechas futuras: admiten predicciones)
        Agregar("España", "Inglaterra", hoy.AddDays(1).AddHours(18), "Rose Bowl, Los Ángeles", "Grupo D", EstadoPartido.Programado, null, null, 2.2m, 3.0m, 2.9m);
        Agregar("Países Bajos", "Senegal", hoy.AddDays(2).AddHours(15), "BC Place, Vancouver", "Grupo A", EstadoPartido.Programado, null, null, 1.8m, 3.2m, 4.0m);
        Agregar("Estados Unidos", "México", hoy.AddDays(2).AddHours(20), "MetLife Stadium, Nueva Jersey", "Grupo B", EstadoPartido.Programado, null, null, 2.0m, 3.1m, 3.3m);
        Agregar("Ecuador", "Argentina", hoy.AddDays(3).AddHours(17), "Estadio BBVA, Monterrey", "Grupo C", EstadoPartido.Programado, null, null, 3.5m, 3.1m, 1.9m);
        Agregar("Alemania", "Canadá", hoy.AddDays(4).AddHours(16), "BMO Field, Toronto", "Grupo D", EstadoPartido.Programado, null, null, 1.6m, 3.6m, 5.0m);
        Agregar("Irán", "Gales", hoy.AddDays(5).AddHours(14), "Estadio Azteca, Ciudad de México", "Grupo B", EstadoPartido.Programado, null, null, 2.8m, 3.0m, 2.4m);
    }

    private void SembrarUsuarios()
    {
        void Sembrar(string nombre, string correo, decimal saldo, int aciertos, int prediccionesTotales)
        {
            var usuario = new UsuarioConCredenciales
            {
                Id = SiguienteUsuarioId(),
                Nombre = nombre,
                Correo = correo,
                Rol = "usuario",
                HashContrasena = CalcularHash("Clave123!")
            };
            Usuarios.Add(usuario);
            Saldos[usuario.Id] = saldo;
            Transacciones[usuario.Id] = new List<Transaccion>
            {
                new()
                {
                    Id = SiguienteTransaccionId(),
                    Tipo = TipoTransaccion.BonoBienvenida,
                    Concepto = "Bono de bienvenida a UTN GolMundial 2026",
                    Monto = 10,
                    FechaHora = DateTime.Now.AddDays(-10)
                }
            };

            // Historial de predicciones liquidadas para que el ranking tenga aciertos coherentes
            var partidosFinalizados = Partidos.Where(p => p.Estado == EstadoPartido.Finalizado).Take(prediccionesTotales).ToList();
            for (var i = 0; i < partidosFinalizados.Count; i++)
            {
                var partido = partidosFinalizados[i];
                var gano = i < aciertos;
                Predicciones.Add(new Prediccion
                {
                    Id = SiguientePrediccionId(),
                    UsuarioId = usuario.Id,
                    PartidoId = partido.Id,
                    DescripcionPartido = $"{partido.SeleccionLocal.Nombre} vs {partido.SeleccionVisitante.Nombre}",
                    Pronostico = Pronostico.Local,
                    Monto = 2,
                    CuotaAplicada = partido.Cuotas.Local,
                    Estado = gano ? EstadoPrediccion.Ganada : EstadoPrediccion.Perdida,
                    PremioPagado = gano ? Math.Round(2 * partido.Cuotas.Local, 2) : null,
                    FechaHora = partido.FechaHora.AddHours(-5)
                });
            }
        }

        Sembrar("Diego Rodríguez", "diego@utn.edu.ec", 47.5m, 4, 6);
        Sembrar("María García", "maria@utn.edu.ec", 32.0m, 3, 6);
        Sembrar("Carlos López", "carlos@utn.edu.ec", 21.5m, 2, 5);
        Sembrar("Ana Martínez", "ana@utn.edu.ec", 15.0m, 2, 4);
        Sembrar("Laura Sánchez", "laura@utn.edu.ec", 8.5m, 1, 4);
        Sembrar("Pedro Fernández", "pedro@utn.edu.ec", 0m, 0, 3);
    }
}
