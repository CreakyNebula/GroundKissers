using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GameStatsManager : MonoBehaviour
{
    [System.Serializable]
    public class GameStats
    {
        public int totalMuertes; // Muertes totales de la partida
        public int totalZancadillas; // Zancadillas totales de la partida
        public int totalParrys; // Parrys totales de la partida
    }

    private GameStats stats = new GameStats(); // Estadísticas globales de la partida
    public string partidaId; // ID de la partida

    private const string saveStatsUrl = "http://localhost/playergroundkisser/register_game_id.php";

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        if (LobbyManager.Instance != null && LobbyManager.Instance.joinedLobby != null)
        {
            Debug.Log(LobbyManager.Instance.joinedLobby.Id);
        }
        // Incrementar estadísticas globales con teclas específicas
        if (Input.GetKeyDown(KeyCode.A)) { stats.totalMuertes++; Debug.Log("Total Muertes: " + stats.totalMuertes); }
        if (Input.GetKeyDown(KeyCode.C)) { stats.totalZancadillas++; Debug.Log("Total Zancadillas: " + stats.totalZancadillas); }
        if (Input.GetKeyDown(KeyCode.D)) { stats.totalParrys++; Debug.Log("Total Parrys: " + stats.totalParrys); }

        // Enviar estadísticas al servidor al presionar la tecla R
        if (Input.GetKeyDown(KeyCode.R)) SaveGameStats();
    }

    private void SaveGameStats()
    {
        StartCoroutine(SendStatsToServer(partidaId, stats));
    }

    private IEnumerator SendStatsToServer(string partidaId, GameStats stats)
    {
        WWWForm form = new WWWForm();
        form.AddField("game_id", partidaId);
        form.AddField("total_muertes", stats.totalMuertes);
        form.AddField("total_zancadillas", stats.totalZancadillas);
        form.AddField("total_parrys", stats.totalParrys);

        Debug.Log($"Enviando estadísticas: partida_id={partidaId}, muertes={stats.totalMuertes}, zancadillas={stats.totalZancadillas}, parrys={stats.totalParrys}");

        using (UnityWebRequest www = UnityWebRequest.Post(saveStatsUrl, form))
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

    public void PartidaIdName(string lobbyId)
    {
        partidaId = lobbyId;
        Debug.Log($"Partida ID generado: {partidaId}");
    }
}