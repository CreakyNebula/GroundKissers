using UnityEngine;
using UnityEngine.InputSystem;

public class GameInitializer : MonoBehaviour
{
    public Transform[] spawnPoints; // Puntos de spawn para cada jugador
    public GameObject[] availablePrefabs; // Lista de prefabs disponibles

    private void Start()
    {
        int playerCount = PlayerPrefs.GetInt("PlayerCount", 0);

        for (int i = 0; i < playerCount; i++)
        {
            // Recuperar el prefab y el dispositivo
            string prefabName = PlayerPrefs.GetString($"Player{i}_Prefab");
            int deviceId = PlayerPrefs.GetInt($"Player{i}_DeviceID");

            // Encontrar el prefab correspondiente
            GameObject prefab = System.Array.Find(availablePrefabs, p => p.name == prefabName);
            if (prefab != null)
            {
                // Instanciar el prefab en un punto de spawn
                Transform spawnPoint = spawnPoints[i % spawnPoints.Length];
                GameObject playerInstance = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

                // Reasignar el controlador al prefab
                PlayerInput playerInput = playerInstance.GetComponent<PlayerInput>();
                if (playerInput != null)
                {
                    InputDevice device = InputSystem.GetDeviceById(deviceId);
                    if (device != null)
                    {
                        playerInput.SwitchCurrentControlScheme(device);
                    }
                }
            }
        }
    }
}
