using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

// Clase principal para gestionar la consulta de estadísticas de la partida
public class MatchStatsManager : MonoBehaviour
{
    // Método para obtener estadísticas de la partida desde el servidor
    public IEnumerator GetMatchStats(int gameId)
    {
        // Crear una solicitud HTTP tipo GET para obtener los datos de la partida
        using (UnityWebRequest www = UnityWebRequest.Get($"http://tuservidor.com/get_match_stats.php?game_id={gameId}"))
        {
            yield return www.SendWebRequest(); // Esperar la respuesta del servidor

            if (www.result == UnityWebRequest.Result.Success) // Si la solicitud fue exitosa
            {
                Debug.Log("Datos de la partida recibidos: " + www.downloadHandler.text); // Mostrar respuesta para depuración

                // Deserializar la respuesta JSON en un objeto MatchStatsResponse
                MatchStatsResponse response = JsonUtility.FromJson<MatchStatsResponse>(www.downloadHandler.text);

                if (response != null) // Verificar que la respuesta no sea nula
                {
                    // Mostrar estadísticas globales de la partida
                    Debug.Log("Muertes totales: " + response.stats_total.total_muertes);
                    Debug.Log("Zancadillas totales: " + response.stats_total.total_zancadillas);
                    Debug.Log("Parrys totales: " + response.stats_total.total_parry);
                    Debug.Log("Duración: " + response.stats_total.duracion + " segundos");

                    // Mostrar estadísticas individuales de cada jugador
                    foreach (var playerStats in response.stats_players)
                    {
                        Debug.Log($"Jugador {playerStats.player_id}: Muertes = {playerStats.muertes}, Parrys = {playerStats.parrys}, Zancadillas = {playerStats.zancadillas}");
                    }
                }
            }
            else // Si hubo un error en la solicitud
            {
                Debug.LogError("Error al obtener estadísticas de la partida: " + www.error); // Mostrar el error en la consola
            }
        }
    }
}

// Clases para deserializar la respuesta JSON del servidor

[System.Serializable]
public class MatchStatsResponse
{
    public StatsTotal stats_total; // Contiene las estadísticas globales de la partida
    public PlayerMatchStats[] stats_players; // Contiene las estadísticas individuales de los jugadores
}

[System.Serializable]
public class StatsTotal
{
    public int total_muertes; // Total de muertes en la partida
    public int total_zancadillas; // Total de zancadillas realizadas en la partida
    public int total_parry; // Total de parrys realizados en la partida
    public int duracion; // Duración total de la partida en segundos
}

[System.Serializable]
public class PlayerMatchStats
{
    public int player_id; // Identificador único del jugador
    public int muertes; // Número de muertes del jugador en la partida
    public int parrys; // Número de parrys realizados por el jugador
    public int zancadillas; // Número de zancadillas realizadas por el jugador
}
