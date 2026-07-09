package ec.edu.utng.guacales.dto;

public class EstadisticaSeleccionDTO {
    private SeleccionDTO seleccion;
    private int jugados;
    private int ganados;
    private int empatados;
    private int perdidos;
    private int golesFavor;
    private int golesContra;

    public EstadisticaSeleccionDTO() {}

    public SeleccionDTO getSeleccion() { return seleccion; }
    public void setSeleccion(SeleccionDTO seleccion) { this.seleccion = seleccion; }
    public int getJugados() { return jugados; }
    public void setJugados(int jugados) { this.jugados = jugados; }
    public int getGanados() { return ganados; }
    public void setGanados(int ganados) { this.ganados = ganados; }
    public int getEmpatados() { return empatados; }
    public void setEmpatados(int empatados) { this.empatados = empatados; }
    public int getPerdidos() { return perdidos; }
    public void setPerdidos(int perdidos) { this.perdidos = perdidos; }
    public int getGolesFavor() { return golesFavor; }
    public void setGolesFavor(int golesFavor) { this.golesFavor = golesFavor; }
    public int getGolesContra() { return golesContra; }
    public void setGolesContra(int golesContra) { this.golesContra = golesContra; }
}
