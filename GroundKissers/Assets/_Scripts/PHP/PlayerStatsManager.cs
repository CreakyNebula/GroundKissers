using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerStatsManager : MonoBehaviour
{
    // Clase para almacenar estadísticas del jugador
    [System.Serializable]
    public class PlayerStats
    {
        public int muertes;      // Número de muertes
        public int parrys;       // Número de parrys realizados
        public int zancadillas;  // Número de zancadillas hechas
        public int ganadas;      // Número de partidas ganadas
        public int perdidas;     // Número de partidas perdidas
    }

    // Variables para almacenar estadísticas y el ID del jugador
    public PlayerStats stats = new PlayerStats(); // Objeto para estadísticas locales
    private int playerId; // ID del jugador actual

    void Start()
    {
        // Recuperar el player_id desde PlayerIdSave
        playerId = PlayerIdSave.GetPlayerId();
        if (playerId == -1)
        {
            Debug.LogError("No se encontró el Player ID. Redirigiendo al login...");
            return;
        }

        Debug.Log("Player ID cargado: " + playerId);

        // Cargar estadísticas desde el servidor al inicio
        StartCoroutine(LoadPlayerStats(playerId));
    }

    void Update()
    {
        // Ejemplo de cómo modificar estadísticas localmente con teclas
        if (Input.GetKeyDown(KeyCode.M)) // Incrementar muertes
        {
            stats.muertes++;
            Debug.Log("Muertes incrementadas: " + stats.muertes);
        }

        if (Input.GetKeyDown(KeyCode.S)) // Guardar estadísticas al presionar S
        {
            SaveStats();
        }
    }

    // Método para enviar estadísticas actualizadas al servidor
    public void SaveStats()
    {
        StartCoroutine(SavePlayerStats(playerId, stats));
    }

    // Corutina para cargar estadísticas desde el servidor
    private IEnumerator LoadPlayerStats(int playerId)
    {
        WWWForm form = new WWWForm();
        form.AddField("player_id", playerId); // Enviar el player_id al servidor

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/playergroundkisser/get_stats.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success) // Si la solicitud fue exitosa
            {
                string jsonResponse = www.downloadHandler.text;

                // Imprime la respuesta del servidor en la consola de Unity
                Debug.Log("Respuesta del servidor: " + jsonResponse);

                if (string.IsNullOrEmpty(jsonResponse))
                {
                    Debug.LogError("La respuesta del servidor está vacía.");
                    yield break; // Salir de la corutina si la respuesta está vacía
                }

                try
                {
                    // Intentar convertir el JSON en un objeto PlayerStats
                    stats = JsonUtility.FromJson<PlayerStats>(jsonResponse);
                    Debug.Log("Estadísticas cargadas correctamente.");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Error al analizar el JSON: " + ex.Message);
                }
            }
            else
            {
                // Imprime el error si la solicitud falló
                Debug.LogError("Error al cargar estadísticas: " + www.error);
            }
        }
    }

    // Corutina para guardar estadísticas en el servidor
    private IEnumerator SavePlayerStats(int playerId, PlayerStats updatedStats)
    {
        WWWForm form = new WWWForm();
        form.AddField("player_id", playerId); // Enviar el player_id
        form.AddField("muertes", updatedStats.muertes); // Enviar estadísticas actualizadas
        form.AddField("parrys", updatedStats.parrys);
        form.AddField("zancadillas", updatedStats.zancadillas);
        form.AddField("ganadas", updatedStats.ganadas);
        form.AddField("perdidas", updatedStats.perdidas);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/playergroundkisser/save_stats.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success) // Si la solicitud fue exitosa
            {
                Debug.Log("Estadísticas guardadas correctamente: " + www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error al guardar estadísticas: " + www.error);
            }
        }
    }
}