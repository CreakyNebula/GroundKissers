using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class RegistrarUserUnity : MonoBehaviour
{
    // Corutina para registrar un usuario en el servidor
    public IEnumerator RegisterUser(string username, string password, System.Action<string> callback)
    {
        // Crear un formulario para enviar los datos al servidor
        WWWForm form = new WWWForm();
        form.AddField("username", username); // Añadir el nombre de usuario al formulario
        form.AddField("password", password); // Añadir la contraseña al formulario

        // Enviar solicitud POST al servidor
        using (UnityWebRequest www = UnityWebRequest.Post("http://localhost/GroundKissers/register.php", form))

        {
            yield return www.SendWebRequest(); // Esperar a que se complete la solicitud

            if (www.result == UnityWebRequest.Result.Success) // Si la solicitud fue exitosa
            {
                Debug.Log("Usuario registrado: " + www.downloadHandler.text); // Mostrar respuesta del servidor en la consola
                callback?.Invoke(www.downloadHandler.text); // Llamar al callback con la respuesta del servidor
            }
            else // Si hubo un error en la solicitud
            {
                Debug.Log("Error al registrar: " + www.error); // Mostrar el error en la consola
                callback?.Invoke(null); // Llamar al callback con null para indicar error
            }
        }
    }
}
