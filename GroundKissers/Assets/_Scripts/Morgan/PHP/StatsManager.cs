using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class StatsManager : MonoBehaviour
{
    // Método para obtener estadísticas acumuladas desde el servidor
    public IEnumerator GetAccountStats(int accountId, System.Action<string> callback)
    {
        // Crear la URL con el parámetro accountId
        string url = $"http://localhost/GroundKissers/account_stats.php?account_id={accountId}";


        // Enviar solicitud GET al servidor
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest(); // Esperar la respuesta del servidor

            if (www.result == UnityWebRequest.Result.Success) // Si la solicitud fue exitosa
            {
                Debug.Log("Estadísticas recibidas: " + www.downloadHandler.text); // Mostrar la respuesta en la consola
                callback?.Invoke(www.downloadHandler.text); // Llamar al callback con la respuesta del servidor
            }
            else // Si hubo un error en la solicitud
            {
                Debug.LogError("Error al obtener estadísticas: " + www.error); // Mostrar el error en la consola
                callback?.Invoke(null); // Llamar al callback con null para indicar error
            }
        }
    }
}
