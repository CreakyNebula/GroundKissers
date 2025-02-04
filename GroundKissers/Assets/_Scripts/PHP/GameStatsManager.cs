using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

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
    public string partidaId; // ID de la partida

    public NetworkVariable<FixedString128Bytes> partidaIdNW = new NetworkVariable<FixedString128Bytes>(
        string.Empty, // Valor por defecto
        NetworkVariableReadPermission.Everyone, // Todos los clientes pueden leer
        NetworkVariableWritePermission.Server // Solo el servidor puede escribir
    );
    private const string saveStatsUrl = "http://localhost/playergroundkisser/register_game_id.php";

    private bool statsSaved = false; // Para evitar guardar más de una vez
    private bool gameStarted;
    private HealthManager[] allPlayers;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
    }
    void Update()
    {
        if (IsServer) StartCoroutine(WaitToStart());

        if (IsServer )
        {
               partidaIdNW.Value = partidaId;

            if (gameStarted && !statsSaved)
            {
                Debug.Log("buscando");
                CheckPlayersAlive();
            }
        }
        if (!IsClient) return; // Solo los clientes manejan los inputs


        /* // Incrementar estadísticas globales mediante ServerRpc
         if (Input.GetKeyDown(KeyCode.A)) { IncrementMuertesServerRpc(); }
         if (Input.GetKeyDown(KeyCode.C)) { IncrementZancadillasServerRpc(); }
         if (Input.GetKeyDown(KeyCode.D)) { IncrementParrysServerRpc(); }*/


        if (SceneManager.GetActiveScene().name == "MainSceneMenu")
        {
            stats.totalMuertes = 0;
            stats.totalZancadillas = 0;
            stats.totalParrys = 0;

        }
    }


    [ServerRpc(RequireOwnership = false)] // Permite que cualquier cliente invoque este método
    public void IncrementMuertesServerRpc(ServerRpcParams rpcParams = default)
    {
        stats.totalMuertes++;
        Debug.Log($"Total Muertes actualizado en el servidor: {stats.totalMuertes}");
        UpdateStatsClientRpc(stats.totalMuertes, stats.totalZancadillas, stats.totalParrys);
    }

    [ServerRpc(RequireOwnership = false)]
    public void IncrementZancadillasServerRpc(ServerRpcParams rpcParams = default)
    {
        stats.totalZancadillas++;
        Debug.Log($"Total Zancadillas actualizado en el servidor: {stats.totalZancadillas}");
        UpdateStatsClientRpc(stats.totalMuertes, stats.totalZancadillas, stats.totalParrys);
    }

    [ServerRpc(RequireOwnership = false)]
    public void IncrementParrysServerRpc(ServerRpcParams rpcParams = default)
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

    public void SaveGameStats()
    {
        if(IsServer)
        {
            StartCoroutine(SendStatsToServer(partidaIdNW.Value.ToString(), stats));
        }
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

    [ClientRpc]
    private void SyncPartidaIdClientRpc(string newPartidaId)
    {
        partidaId = newPartidaId;
        Debug.Log($"partidaId sincronizado en el cliente: {partidaId}");
    }

    // Método para que un cliente solicite el partidaId actual al servidor
    [ServerRpc(RequireOwnership = false)]
    public void RequestPartidaIdServerRpc(ServerRpcParams rpcParams = default)
    {
        // Responder al cliente que solicitó el `partidaId`
        SyncPartidaIdClientRpc(partidaId);
    }

    IEnumerator WaitToStart()
    {

        yield return new WaitForSeconds(10);
        if (gameStarted) yield break;
        allPlayers = FindObjectsOfType<HealthManager>();
        Debug.Log(allPlayers.Length);
        gameStarted = true;
    }
  

    void CheckPlayersAlive()
    {
        // Comprobar cuántos jugadores están vivos
        int aliveCount = 0;

        foreach (HealthManager player in allPlayers)
        {
            if (player.alive.Value)  // Verifica si el valor de 'alive' del jugador es true
            {
                aliveCount++;
            }
        }

        if (aliveCount <= 1)
        {
            SaveGameStats();
            statsSaved = true;
        }
    }

}
