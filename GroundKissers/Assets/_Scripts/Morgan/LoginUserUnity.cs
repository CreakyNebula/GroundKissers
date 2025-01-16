using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LoginUserUnity : MonoBehaviour
{
    // Método para validar credenciales de usuario
    public IEnumerator LoginUser(string username, string password, System.Action<string> callback)
    {
        // Crear un formulario para enviar los datos al servidor
        WWWForm form = new WWWForm();
        form.AddField("username", username); // Añadir el nombre de usuario al formulario
        form.AddField("password", password); // Añadir la contraseña al formulario

        // Enviar solicitud POST al servidor
        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/GroundKissers/login.php", form))
        {
            yield return www.SendWebRequest(); // Esperar a que se complete la solicitud

            if (www.result == UnityWebRequest.Result.Success) // Si la solicitud fue exitosa
            {
                Debug.Log("Respuesta del servidor: " + www.downloadHandler.text); // Mostrar la respuesta del servidor
                callback?.Invoke(www.downloadHandler.text); // Llamar al callback con la respuesta del servidor
            }
            else // Si hubo un error en la solicitud
            {
                Debug.LogError("Error al iniciar sesión: " + www.error); // Mostrar el error en la consola
                callback?.Invoke(null); // Llamar al callback con null para indicar error
            }
        }
    }
}
