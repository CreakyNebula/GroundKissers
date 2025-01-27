using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerStatsPartida : NetworkBehaviour
{
    public string partidaId; // ID de la partida actual
    public int playerId;
    public int muertes = 0;
    public int parrys = 0;
    public int zancadillas = 0;
    public int ganadas = 0; // Contador de partidas ganadas
    public int perdidas = 0; // Contador de partidas perdidas
    public int partidasTotales = 0; // Contador de partidas totales

    void Start()
    {
        // Recupera el player_id guardado en las preferencias locales (PlayerPrefs)
        playerId = PlayerPrefs.GetInt("player_id", -1);
        if (playerId == -1)
        {
            Debug.LogError("Player ID no encontrado. Asegúrate de iniciar sesión.");
            return;
        }
    }

    void Update()
    {
        // Para probar, asignamos teclas para incrementar las estadísticas y enviarlas
        if (Input.GetKeyDown(KeyCode.G)) // Incrementar partidas ganadas
        {
            Win();
        }

        if (Input.GetKeyDown(KeyCode.L)) // Incrementar partidas perdidas
        {
            Lose();
        }

        if (Input.GetKeyDown(KeyCode.H)) // Guardar estadísticas manualmente
        {
            SavePlayerStatsServerRpc(playerId, muertes, parrys, zancadillas, ganadas, perdidas, partidasTotales);
        }
    }

    public void Lose()
    {
        perdidas++;
        partidasTotales++;
        SavePlayerStatsServerRpc(playerId, muertes, parrys, zancadillas, ganadas, perdidas, partidasTotales);
    }

    public void Win()
    {
        ganadas++;
        partidasTotales++;
        SavePlayerStatsServerRpc(playerId, muertes, parrys, zancadillas, ganadas, perdidas, partidasTotales);
    }

    // Este método envía estadísticas al servidor (se ejecuta en el servidor)
    [ServerRpc(RequireOwnership = false)]
    public void SavePlayerStatsServerRpc(int playerId, int muertes, int parrys, int zancadillas, int ganadas, int perdidas, int partidasTotales)
    {
        // Obtiene el ID de la partida desde otro componente (por ejemplo, GameStatsManager)
        partidaId = GameObject.Find("StatsManager").GetComponent<GameStatsManager>().partidaId;

        // Validación para asegurarse de que el código solo se ejecute en el servidor
        if (!IsServer)
        {
            Debug.LogError("SavePlayerStatsServerRpc fue llamado desde un cliente pero no se ejecutará en el servidor.");
            return;
        }

        // Llama a la rutina que envía los datos al servidor
        StartCoroutine(SendStatsToServer(partidaId, playerId, muertes, parrys, zancadillas, ganadas, perdidas, partidasTotales));
        Debug.Log("Estadísticas enviadas al servidor.");
    }

    // Esta rutina realiza una solicitud HTTP POST para guardar las estadísticas en la base de datos
    private IEnumerator SendStatsToServer(string partidaId, int playerId, int muertes, int parrys, int zancadillas, int ganadas, int perdidas, int partidasTotales)
    {
        WWWForm form = new WWWForm();
        form.AddField("partida_id", partidaId);
        form.AddField("player_id", playerId);
        form.AddField("muertes", muertes);
        form.AddField("parrys", parrys);
        form.AddField("zancadillas", zancadillas);
        form.AddField("ganadas", ganadas);
        form.AddField("perdidas", perdidas);
        form.AddField("partidas_totales", partidasTotales);

        Debug.Log($"Enviando datos: partida_id={partidaId}, player_id={playerId}, muertes={muertes}, parrys={parrys}, zancadillas={zancadillas}, ganadas={ganadas}, perdidas={perdidas}, partidas_totales={partidasTotales}");

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