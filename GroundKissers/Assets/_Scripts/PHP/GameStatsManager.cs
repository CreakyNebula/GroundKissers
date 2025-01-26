using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class GameStatsManager : NetworkBehaviour
{
    [System.Serializable]
    public class GameStats
    {
        public int totalMuertes; // Muertes totales de la partida
        public int totalZancadillas; // Zancadillas totales de la partida
        public int totalParrys; // Parrys totales de la partida
    }

    private GameStats stats = new GameStats(); // Estadísticas globales de la partida
    private string partidaId; // ID de la partida

    private const string saveStatsUrl = "http://localhost/playergroundkisser/register_game_id.php";

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        if (!IsClient) return; // Solo los clientes manejan los inputs

        // Incrementar estadísticas globales mediante ServerRpc
        if (Input.GetKeyDown(KeyCode.A)) { IncrementMuertesServerRpc(); }
        if (Input.GetKeyDown(KeyCode.C)) { IncrementZancadillasServerRpc(); }
        if (Input.GetKeyDown(KeyCode.D)) { IncrementParrysServerRpc(); }

        // Enviar estadísticas al servidor al presionar la tecla R
        if (Input.GetKeyDown(KeyCode.R)) SaveGameStats();
    }

    [ServerRpc(RequireOwnership = false)] // Permite que cualquier cliente invoque este método
    private void IncrementMuertesServerRpc(ServerRpcParams rpcParams = default)
    {
        stats.totalMuertes++;
        Debug.Log($"Total Muertes actualizado en el servidor: {stats.totalMuertes}");
        UpdateStatsClientRpc(stats.totalMuertes, stats.totalZancadillas, stats.totalParrys);
    }

    [ServerRpc(RequireOwnership = false)]
    private void IncrementZancadillasServerRpc(ServerRpcParams rpcParams = default)
    {
        stats.totalZancadillas++;
        Debug.Log($"Total Zancadillas actualizado en el servidor: {stats.totalZancadillas}");
        UpdateStatsClientRpc(stats.totalMuertes, stats.totalZancadillas, stats.totalParrys);
    }

    [ServerRpc(RequireOwnership = false)]
    private void IncrementParrysServerRpc(ServerRpcParams rpcParams = default)
    {
        stats.totalParrys++;
        Debug.Log($"Total Parrys actualizado en el servidor: {stats.totalParrys}");
        UpdateStatsClientRpc(stats.totalMuertes, stats.totalZancadillas, stats.totalParrys);
    }

    [ClientRpc]
    private void UpdateStatsClientRpc(int totalMuertes, int totalZancadillas, int totalParrys)
    {
        // Actualiza las estadísticas en todos los clientes
        stats.totalMuertes = totalMuertes;
        stats.totalZancadillas = totalZancadillas;
        stats.totalParrys = totalParrys;

        Debug.Log($"Estadísticas sincronizadas en el cliente: Muertes={totalMuertes}, Zancadillas={totalZancadillas}, Parrys={totalParrys}");
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
