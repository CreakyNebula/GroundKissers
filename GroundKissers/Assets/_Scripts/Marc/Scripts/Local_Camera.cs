using System.Collections.Generic;
using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
    public string playerName = "LocalPlayer (1)(Clone)"; // Nombre de los jugadores a buscar
    public float smoothSpeed = 0.125f; // Velocidad de suavizado para la posici�n de la c�mara
    public float minZoom = 5f; // Zoom m�nimo de la c�mara
    public float maxZoom = 10f; // Zoom m�ximo de la c�mara
    public float zoomLimiter = 5f; // Factor para limitar el zoom

    public Vector2 minLimits; // L�mite inferior izquierdo del mapa
    public Vector2 maxLimits; // L�mite superior derecho del mapa

    private Camera cam;
    private List<Transform> players;

    void Start()
    {
        cam = GetComponent<Camera>();
        FindPlayers(); // Encontrar jugadores al inicio
    }

    void LateUpdate()
    {
        // Actualizar la lista de jugadores en tiempo real
        FindPlayers();

        if (players.Count == 0) return;

        // Ajustar la posici�n de la c�mara
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint;
        newPosition.z = -10f; // Mantener la c�mara en 2D

        // Aplicar l�mites a la posici�n de la c�mara
        newPosition.x = Mathf.Clamp(newPosition.x, minLimits.x, maxLimits.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minLimits.y, maxLimits.y);

        transform.position = Vector3.Lerp(transform.position, newPosition, smoothSpeed);

        // Ajustar el zoom de la c�mara
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, smoothSpeed);

        // Ajustar el zoom para no salir de los l�mites
        AdjustZoomToBounds();
    }

    // Encontrar todos los jugadores en la escena
    void FindPlayers()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");

        players = new List<Transform>();
        foreach (GameObject obj in playerObjects)
        {
            if (obj.name.Contains(playerName))
            {
                players.Add(obj.transform);
            }
        }
    }

    // Obtener el punto medio entre todos los jugadores
    Vector3 GetCenterPoint()
    {
        if (players.Count == 1) return players[0].position;

        var bounds = new Bounds(players[0].position, Vector3.zero);
        foreach (Transform player in players)
        {
            bounds.Encapsulate(player.position);
        }
        return bounds.center;
    }

    // Calcular la mayor distancia entre los jugadores
    float GetGreatestDistance()
    {
        if (players.Count == 1) return 0f;

        var bounds = new Bounds(players[0].position, Vector3.zero);
        foreach (Transform player in players)
        {
            bounds.Encapsulate(player.position);
        }
        return bounds.size.x > bounds.size.y ? bounds.size.x : bounds.size.y;
    }

    // Ajustar el zoom para mantenerse dentro de los l�mites
    void AdjustZoomToBounds()
    {
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minLimits.x + halfWidth, maxLimits.x - halfWidth);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minLimits.y + halfHeight, maxLimits.y - halfHeight);

        transform.position = clampedPosition;
    }
}
