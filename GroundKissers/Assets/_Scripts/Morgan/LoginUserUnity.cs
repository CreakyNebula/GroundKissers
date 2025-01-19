using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro; // Para usar TextMeshPro

public class LoginUserUnity: MonoBehaviour
{
    // Referencias a los campos de texto y el mensaje de resultado en la UI
    public TMP_InputField usernameInputField; // Campo de texto para el nombre de usuario
    public TMP_InputField passwordInputField; // Campo de texto para la contraseña
    public TMP_Text resultText; // Texto para mostrar mensajes en la UI

    // Método llamado desde el botón de la UI para iniciar sesión
    public void StartLogin()
    {
        // Capturar el nombre de usuario y contraseña desde la UI
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        // Iniciar la corutina para enviar los datos al servidor
        StartCoroutine(LoginUser(username, password, HandleLoginResponse));
    }

    // Corutina para enviar los datos de inicio de sesión al servidor
    public IEnumerator LoginUser(string username, string password, System.Action<string> callback)
    {
        // Crear un formulario para enviar los datos al servidor
        WWWForm form = new WWWForm();
        form.AddField("username", username); // Añadir el nombre de usuario al formulario
        form.AddField("password", password); // Añadir la contraseña al formulario

        // Enviar la solicitud POST al servidor
        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/playergroundkisser/login.php", form))
        {
            // Mostrar mensaje en la UI mientras se realiza la solicitud
            resultText.text = "Intentando iniciar sesión...";
            yield return www.SendWebRequest(); // Esperar la respuesta del servidor

            if (www.result == UnityWebRequest.Result.Success) // Si la solicitud fue exitosa
            {
                Debug.Log("Respuesta del servidor: " + www.downloadHandler.text); // Mostrar la respuesta en la consola
                callback?.Invoke(www.downloadHandler.text); // Llamar al callback con la respuesta del servidor
            }
            else // Si hubo un error en la solicitud
            {
                Debug.LogError("Error al iniciar sesión: " + www.error); // Mostrar el error en la consola
                callback?.Invoke(null); // Llamar al callback con null para indicar error
            }
        }
    }

    // Método para manejar la respuesta del servidor después del intento de inicio de sesión
    void HandleLoginResponse(string response)
    {
        if (!string.IsNullOrEmpty(response) && response.Contains("success")) // Si la respuesta contiene "success"
        {
            Debug.Log("Inicio de sesión exitoso");
            resultText.text = "Inicio de sesión exitoso"; // Mostrar mensaje de éxito en la UI
            // Aquí puedes redirigir al jugador a otra escena, por ejemplo:
            // SceneManager.LoadScene("NombreDeLaEscena");
        }
        else // Si el inicio de sesión falló
        {
            Debug.LogError("Inicio de sesión fallido");
            resultText.text = "Inicio de sesión fallido"; // Mostrar mensaje de error en la UI
        }
    }
}
