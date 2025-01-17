using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class MatchStatsHandler : MonoBehaviour
{
    // Método para enviar estadísticas de la partida al servidor
    public IEnumerator RegisterMatch(string jsonData, System.Action<string> callback)
    {
        // Crear una solicitud HTTP tipo POST para enviar los datos en formato JSON
        using (UnityWebRequest www = UnityWebRequest.Put("http://localhost/GroundKissers/register_match.php", jsonData))
        {
            www.SetRequestHeader("Content-Type", "application/json"); // Indicar que los datos enviados son JSON
            yield return www.SendWebRequest(); // Enviar la solicitud y esperar respuesta del servidor

            if (www.result == UnityWebRequest.Result.Success) // Si la solicitud fue exitosa
            {
                Debug.Log("Estadísticas registradas exitosamente: " + www.downloadHandler.text); // Mostrar respuesta en la consola
                callback?.Invoke(www.downloadHandler.text); // Llamar al callback con la respuesta del servidor
            }
            else // Si hubo un error en la solicitud
            {
                Debug.LogError("Error al registrar estadísticas: " + www.error); // Mostrar el error en la consola
                callback?.Invoke(null); // Llamar al callback con un valor nulo para indicar error
            }
        }
    }
}
