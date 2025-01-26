using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro; // Para usar TextMeshPro


public class RegistrarUserUnity : MonoBehaviour
{
    // Referencias a los campos de texto y el mensaje de resultado en la UI
    public TMP_InputField usernameInputField; // Campo de texto para el nombre de usuario
    public TMP_InputField passwordInputField; // Campo de texto para la contraseña
    public TMP_Text resultText; // Texto para mostrar mensajes en la UI

    // Método llamado desde el botón de la UI para registrar usuario
    public void StartRegister()
    {
        // Capturar el nombre de usuario y contraseña desde la UI
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        // Iniciar la corutina para enviar los datos al servidor
        StartCoroutine(RegisterUser(username, password, HandleRegisterResponse));
    }

    // Corutina para enviar los datos de registro al servidor
    public IEnumerator RegisterUser(string username, string password, System.Action<string> callback)
    {
        // Crear un formulario para enviar los datos al servidor
        WWWForm form = new WWWForm();
        form.AddField("username", username); // Añadir el nombre de usuario al formulario
        form.AddField("password", password); // Añadir la contraseña al formulario

        // Enviar la solicitud POST al servidor
        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/playergroundkisser/register.php", form))
        {
            // Mostrar mensaje en la UI mientras se realiza la solicitud
            resultText.text = "Intentando registrar usuario...";
            yield return www.SendWebRequest(); // Esperar la respuesta del servidor

            if (www.result == UnityWebRequest.Result.Success) // Si la solicitud fue exitosa
            {
                Debug.Log("Respuesta del servidor: " + www.downloadHandler.text); // Mostrar la respuesta en la consola
                callback?.Invoke(www.downloadHandler.text); // Llamar al callback con la respuesta del servidor
            }
            else // Si hubo un error en la solicitud
            {
                Debug.LogError("Error al registrar: " + www.error); // Mostrar el error en la consola
                callback?.Invoke(null); // Llamar al callback con null para indicar error
            }
        }
    }

    // Método para manejar la respuesta del servidor después del intento de registro
    void HandleRegisterResponse(string response)
    {
        if (!string.IsNullOrEmpty(response) && response.Contains("success")) // Si la respuesta contiene "success"
        {
            Debug.Log("Registro exitoso");
            resultText.text = "¡Usuario registrado exitosamente!"; // Mostrar mensaje de éxito en la UI

            
        }
        else if (!string.IsNullOrEmpty(response) && response.Contains("duplicate")) // Si la respuesta contiene "duplicate"
        {
            Debug.LogError("Registro fallido: Usuario duplicado");
            resultText.text = "Error: Nombre de usuario ya registrado."; // Mostrar mensaje de duplicado en la UI
        }
        else // Si el registro falló por otra razón
        {
            Debug.LogError("Registro fallido");
            resultText.text = "Error: No se pudo registrar el usuario."; // Mostrar mensaje de error en la UI
        }
    }
}
