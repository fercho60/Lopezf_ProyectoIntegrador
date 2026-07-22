package ec.edu.utng.guacales.service;

import jakarta.enterprise.context.ApplicationScoped;
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.time.Duration;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 * Cliente HTTP hacia UTNGolCoin (Persona 2).
 * No altera el arranque de WildFly: si el servicio no está disponible, se registra el error y se continúa.
 */
@ApplicationScoped
public class UtnGolCoinClient {

    private static final Logger LOG = Logger.getLogger(UtnGolCoinClient.class.getName());

    /** Sobreescribible con -Dutngolcoin.baseUrl=... o variable de entorno UTNGOLCOIN_BASE_URL */
    private String baseUrl() {
        String env = System.getenv("UTNGOLCOIN_BASE_URL");
        if (env != null && !env.isBlank()) {
            return env.endsWith("/") ? env : env + "/";
        }
        return System.getProperty("utngolcoin.baseUrl", "http://localhost:5000/api/");
    }

    private final HttpClient http = HttpClient.newBuilder()
            .connectTimeout(Duration.ofSeconds(5))
            .build();

    public void crearBilletera(Long usuarioId) {
        String cuerpo = "{\"usuarioId\":" + usuarioId + "}";
        post("billeteras", cuerpo, "crear billetera usuario " + usuarioId);
    }

    public void liquidarPartido(Long partidoId, int golesLocal, int golesVisitante) {
        String cuerpo = "{\"golesLocal\":" + golesLocal + ",\"golesVisitante\":" + golesVisitante + "}";
        post("liquidaciones/" + partidoId, cuerpo, "liquidar partido " + partidoId);
    }

    private void post(String ruta, String json, String contexto) {
        try {
            HttpRequest request = HttpRequest.newBuilder()
                    .uri(URI.create(baseUrl() + ruta))
                    .timeout(Duration.ofSeconds(10))
                    .header("Content-Type", "application/json")
                    .POST(HttpRequest.BodyPublishers.ofString(json))
                    .build();
            HttpResponse<String> response = http.send(request, HttpResponse.BodyHandlers.ofString());
            if (response.statusCode() >= 200 && response.statusCode() < 300) {
                LOG.info(() -> "UTNGolCoin OK (" + contexto + "): HTTP " + response.statusCode());
            } else {
                LOG.warning("UTNGolCoin error (" + contexto + "): HTTP " + response.statusCode()
                        + " body=" + response.body());
            }
        } catch (Exception e) {
            LOG.log(Level.WARNING, "UTNGolCoin no disponible (" + contexto + "): " + e.getMessage(), e);
        }
    }
}
