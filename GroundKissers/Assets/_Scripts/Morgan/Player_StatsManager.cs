using UnityEngine;

public class Player_StatsManager : MonoBehaviour
{
    // Clase interna para manejar estadísticas individuales de un jugador
    public class PlayerStats
    {
        public int playerId; // Identificador único del jugador
        public int victorias; // Número total de victorias del jugador
        public int derrotas; // Número total de derrotas del jugador
        public int empates; // Número total de empates del jugador
        public int muertes; // Muertes en la partida actual
        public int parrys; // Parrys realizados en la partida actual
        public int zancadillas; // Zancadillas realizadas en la partida actual

        // Constructor para inicializar las estadísticas
        public PlayerStats(int id)
        {
            playerId = id;
            victorias = 0;
            derrotas = 0;
            empates = 0;
            muertes = 0;
            parrys = 0;
            zancadillas = 0;
        }

        // Reinicia las estadísticas de la partida actual (pero mantiene acumulados)
        public void ResetPartida()
        {
            muertes = 0;
            parrys = 0;
            zancadillas = 0;
        }
    }

    // Instancias de estadísticas para los jugadores
    public PlayerStats jugador1;
    public PlayerStats jugador2;

    void Start()
    {
        // Inicializamos las estadísticas de los jugadores
        jugador1 = new PlayerStats(1);
        jugador2 = new PlayerStats(2);
    }

    // Método para registrar el resultado de una partida
    public void RegistrarResultado(int ganadorId)
    {
        if (ganadorId == jugador1.playerId)
        {
            jugador1.victorias++;
            jugador2.derrotas++;
        }
        else if (ganadorId == jugador2.playerId)
        {
            jugador2.victorias++;
            jugador1.derrotas++;
        }
        else
        {
            jugador1.empates++;
            jugador2.empates++;
        }
    }

    // Método para mostrar estadísticas en la consola (para pruebas)
    public void MostrarEstadisticas()
    {
        Debug.Log($"Jugador 1 - Victorias: {jugador1.victorias}, Derrotas: {jugador1.derrotas}, Empates: {jugador1.empates}");
        Debug.Log($"Jugador 2 - Victorias: {jugador2.victorias}, Derrotas: {jugador2.derrotas}, Empates: {jugador2.empates}");
    }
}
