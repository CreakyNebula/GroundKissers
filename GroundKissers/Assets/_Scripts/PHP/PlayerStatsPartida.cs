using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerStatsPartida : MonoBehaviour
{
    public string partidaId; // Ahora es un string
    private int playerId;
    public int muertes = 0;
    public int parrys = 0;
    public int zancadillas = 0;

    void Start()
    {
        // Recuperar el player_id usando PlayerPrefs
        playerId = PlayerPrefs.GetInt("player_id", -1);
        if (playerId == -1)
        {
            Debug.LogError("Player ID no encontrado. Asegúrate de iniciar sesión.");
            return;
        }

        // Generar partida_id alfanumérico y registrarlo
     /*   partidaId = GenerateAlphanumericId(8); // Generar ID alfanumérico de 8 caracteres
        Debug.Log($"Partida ID generado: {partidaId}");*/
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) // Aumentar muertes
        {
            muertes++;
            Debug.Log("Muertes: " + muertes);
        }

        if (Input.GetKeyDown(KeyCode.P)) // Aumentar parrys
        {
            parrys++;
            Debug.Log("Parrys: " + parrys);
        }

        if (Input.GetKeyDown(KeyCode.Z)) // Aumentar zancadillas
        {
            zancadillas++;
            Debug.Log("Zancadillas: " + zancadillas);
        }


        if (Input.GetKeyDown(KeyCode.H)) // Guardar estadísticas
        {
            StartCoroutine(SavePlayerStats());
        }
    }

    private IEnumerator RegisterGameId()
    {
        WWWForm form = new WWWForm();
        form.AddField("game_id", partidaId);

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/playergroundkisser/register_game_id.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Respuesta del servidor: " + www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error al registrar Game ID: " + www.error);
            }
        }
    }

    private IEnumerator SavePlayerStats()
    {
        WWWForm form = new WWWForm();
        form.AddField("partida_id", partidaId);
        form.AddField("player_id", playerId);
        form.AddField("muertes", muertes);
        form.AddField("parrys", parrys);
        form.AddField("zancadillas", zancadillas);

        // Agregar un log para verificar qué datos se envían
        Debug.Log($"Enviando datos: partida_id={partidaId}, player_id={playerId}, muertes={muertes}, parrys={parrys}, zancadillas={zancadillas}");

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

    // Método para generar un ID alfanumérico
    private string GenerateAlphanumericId(int length)
    {
        const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        char[] idArray = new char[length];
        System.Random random = new System.Random();

        for (int i = 0; i < length; i++)
        {
            idArray[i] = characters[random.Next(characters.Length)];
        }

        return new string(idArray);
    }

    public void PartidaIdName(string lobbyId)
    {
         partidaId = lobbyId;
        //StartCoroutine(RegisterGameId());

    }
}