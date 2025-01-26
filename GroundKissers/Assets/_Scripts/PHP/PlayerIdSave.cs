using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerIdSave : MonoBehaviour
{
    // Guardar el player_id
    public static void SavePlayerId(int playerId)
    {
        PlayerPrefs.SetInt("player_id", playerId);
        PlayerPrefs.Save();
        Debug.Log("Player ID guardado: " + playerId);
    }

    // Recuperar el player_id
    public static int GetPlayerId()
    {
        int playerId = PlayerPrefs.GetInt("player_id", -1);
        if (playerId != -1)
        {
            Debug.Log("Player ID encontrado: " + playerId);
            return playerId;
        }
        else
        {
            Debug.LogError("No se encontró el Player ID. Redirigiendo al login...");
            SceneManager.LoadScene("LoginScene");
            return -1;
        }
    }

    // Borrar el player_id
    public static void ClearPlayerId()
    {
        PlayerPrefs.DeleteKey("player_id");
        PlayerPrefs.Save();
        Debug.Log("Player ID eliminado. El usuario ha cerrado sesión.");
    }
}
