using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginUserUnity : MonoBehaviour
{
    public TMP_InputField usernameInputField; // Campo de texto para el nombre de usuario
    public TMP_InputField passwordInputField; // Campo de texto para la contraseña
    public TMP_Text resultText; // Texto para mostrar mensajes de resultado en la interfaz de usuario

    // Método llamado desde el botón de la UI para iniciar sesión
    public void StartLogin()
    {
        string username = usernameInputField.text; // Capturar el nombre de usuario desde la UI
        string password = passwordInputField.text; // Capturar la contraseña desde la UI

        StartCoroutine(LoginUser(username, password, HandleLoginResponse));
    }

    // Corutina para enviar los datos al servidor
    public IEnumerator LoginUser(string username, string password, System.Action<string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username); // Añadir el nombre de usuario al formulario
        form.AddField("password", password); // Añadir la contraseña al formulario

        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/playergroundkisser/login.php", form))
        {
            resultText.text = "Intentando iniciar sesión...";
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Respuesta del servidor: " + www.downloadHandler.text);
                callback?.Invoke(www.downloadHandler.text); // Pasar la respuesta al callback
            }
            else
            {
                Debug.LogError("Error al iniciar sesión: " + www.error);
                callback?.Invoke(null);
            }
        }
    }

    // Método para manejar la respuesta del servidor
    void HandleLoginResponse(string response)
    {
        if (!string.IsNullOrEmpty(response) && response.Contains("success"))
        {
            Debug.Log("Inicio de sesión exitoso: " + response);
            resultText.text = "Inicio de sesión exitoso";

            int playerId = ExtractPlayerId(response); // Extraer el player_id de la respuesta
            if (playerId != -1)
            {
                PlayerPrefs.SetInt("player_id", playerId); // Guardar el player_id localmente
                Debug.Log("ID del jugador guardado: " + playerId);

                // Cambiar de escena después del inicio de sesión exitoso
                SceneManager.LoadScene("MainMenuScene");
            }
            else
            {
                Debug.LogError("No se pudo extraer el player_id.");
                resultText.text = "Error: No se pudo obtener el ID del jugador.";
            }
        }
        else
        {
            Debug.LogError("Inicio de sesión fallido: " + response);
            resultText.text = "Error: Usuario o contraseña incorrectos.";
        }
    }

    // Método para extraer el player_id de la respuesta del servidor
    private int ExtractPlayerId(string response)
    {
        try
        {
            var jsonResponse = JsonUtility.FromJson<LoginResponse>(response);
            return jsonResponse.player_id;
        }
        catch
        {
            return -1; // Si falla la extracción, devolver -1
        }
    }

    // Clase para deserializar la respuesta del servidor
    [System.Serializable]
    public class LoginResponse
    {
        public string status;
        public string message;
        public int player_id;
    }
}
