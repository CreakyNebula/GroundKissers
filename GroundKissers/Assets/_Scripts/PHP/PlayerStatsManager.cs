using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerStatsManager : MonoBehaviour
{
    [System.Serializable]
    public class PlayerStats
    {
        public int muertes;
        public int parrys;
        public int zancadillas;
        public int ganadas;
        public int perdidas;
        public int partidasTotales;
    }

    private PlayerStats stats = new PlayerStats();
    private int playerId;

    void Start()
    {
        // Obtener el player_id desde PlayerPrefs
        playerId = PlayerPrefs.GetInt("player_id", -1);
        if (playerId == -1)
        {
            Debug.LogError("No se encontró el Player ID. Redirigiendo al login...");
            return;
        }

        Debug.Log("Player ID cargado: " + playerId);

        // Cargar estadísticas desde el servidor
        StartCoroutine(LoadPlayerStats(playerId));
    }

    public void SaveStats()
    {
        StartCoroutine(SavePlayerStats(playerId, stats));
    }

    private IEnumerator LoadPlayerStats(int playerId)
    {
        WWWForm form = new WWWForm();
        form.AddField("player_id", playerId);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/playergroundkisser/get_stats.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                Debug.Log("Respuesta del servidor: " + jsonResponse);

                if (!string.IsNullOrEmpty(jsonResponse))
                {
                    try
                    {
                        stats = JsonUtility.FromJson<PlayerStats>(jsonResponse);
                        Debug.Log("Estadísticas cargadas correctamente.");
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError("Error al analizar el JSON: " + ex.Message);
                    }
                }
            }
            else
            {
                Debug.LogError("Error al cargar estadísticas: " + www.error);
            }
        }
    }

    private IEnumerator SavePlayerStats(int playerId, PlayerStats updatedStats)
    {
        WWWForm form = new WWWForm();
        form.AddField("player_id", playerId);
        form.AddField("muertes", updatedStats.muertes);
        form.AddField("parrys", updatedStats.parrys);
        form.AddField("zancadillas", updatedStats.zancadillas);
        form.AddField("ganadas", updatedStats.ganadas);
        form.AddField("perdidas", updatedStats.perdidas);
        form.AddField("partidasTotales", updatedStats.partidasTotales);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/playergroundkisser/save_player_stats.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
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