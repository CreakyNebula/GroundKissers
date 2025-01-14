using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : NetworkBehaviour
{
    public GameObject PlayerPrefab; // Prefab del jugador que se va a instanciar
    private GameObject newPlayer;

    [SerializeField] private Transform PlayerSpawns;

    public Image playerImage;
    public Vector2 playerImageRectTransform;
    public Color myColor;
    public GameObject playerImageInScene;

    // Variable para sincronizar el objeto del jugador
    private NetworkVariable<NetworkObjectReference> playerReference = new NetworkVariable<NetworkObjectReference>();

    private NetworkVariable<Color> playerColor = new NetworkVariable<Color>(Color.white, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<Vector2> rectTransformPosition = new NetworkVariable<Vector2>(Vector2.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private bool coroutineStarted;

    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayerServerRpc(ulong clientId)
    {
        if (!IsServer) return; // Solo el servidor puede spawnear objetos

        if (playerReference.Value.TryGet(out _))
        {
            Debug.Log($"[Server] Player for client {clientId} already spawned.");
            return; // Ya existe un jugador para este cliente
        }

        // Calcula la posición inicial del jugador
        Vector3 position = PlayerSpawns != null ? PlayerSpawns.position : Vector3.zero;

        // Instancia el jugador en el servidor
        newPlayer = Instantiate(PlayerPrefab, position, Quaternion.identity);

        // Agrega el objeto a la red y asigna el ownership al cliente que lo solicitó
        NetworkObject networkObject = newPlayer.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.SpawnWithOwnership(clientId);
            playerReference.Value = networkObject; // Actualiza la referencia sincronizada

            // Notifica al cliente que el jugador ha sido spawneado
            NotifyClientPlayerSpawnedClientRpc(clientId, networkObject);
        }

        Debug.Log($"[Server] Player spawned for client {clientId}");
    }

    [ClientRpc]
    private void NotifyClientPlayerSpawnedClientRpc(ulong clientId, NetworkObjectReference networkObject)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            if (networkObject.TryGet(out NetworkObject obj))
            {
                newPlayer = obj.gameObject; // Actualiza la referencia local del cliente
                Debug.Log($"[Client] Player spawned and reference set for client {clientId}");
            }
        }
    }

    IEnumerator RequestPlayerSpawn()
    {
        if (IsClient)
        {
            Debug.Log("[Client] Requesting player spawn...");
            coroutineStarted = true;

            // Enviar solicitud al servidor
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);

            // Esperar a que el servidor sincronice la referencia
            while (!playerReference.Value.TryGet(out _))
            {
                yield return new WaitForSeconds(0.1f);
            }

            Debug.Log("[Client] Player reference synchronized.");
            coroutineStarted = false;
        }
    }

    void Start()
    {
        if (IsClient && IsOwner)
        {
            Debug.Log("[Client] Starting as owner.");
            ChangeColor();

            rectTransformPosition.OnValueChanged += OnRectTransformPositionChanged;
            playerReference.OnValueChanged += OnPlayerReferenceChanged; // Suscribir a cambios en la referencia del jugador

            if (!playerReference.Value.TryGet(out _))
            {
                StartCoroutine(RequestPlayerSpawn());
            }
        }

        if (IsServer)
        {
            Debug.Log("[Server] Spawning player for host.");
            if (IsOwner)
            {
                SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
            }
        }
    }

    private void Update()
    {
        if (IsClient && IsOwner && !coroutineStarted)
        {
            // Verificar si ya existe un jugador sincronizado
            NetworkObject dummy;
            if (!playerReference.Value.TryGet(out dummy))
            {
                StartCoroutine(RequestPlayerSpawn());
            }
        }
    }

    void ChangeColor()
    {
        if (IsOwner)
        {
            myColor = PlayerInfo.Instance.playerColor;
            playerColor.Value = myColor;

            Debug.Log($"[PlayerDebug] ClientId asignado a este jugador: {OwnerClientId}");
        }

        playerColor.OnValueChanged += (oldValue, newValue) =>
        {
            UpdatePlayerColor(newValue);
        };

        UpdatePlayerColor(playerColor.Value);
    }

    private void UpdatePlayerColor(Color newColor)
    {
        playerImage.color = newColor;
    }

    [ServerRpc]
    private void UpdateRectTransformServerRpc(Vector2 position)
    {
        rectTransformPosition.Value = position;
    }

    private void OnRectTransformPositionChanged(Vector2 oldPosition, Vector2 newPosition)
    {
        UpdatePlayerImagePosition(newPosition);
    }

    private void UpdatePlayerImagePosition(Vector2 position)
    {
        if (playerImageInScene != null)
        {
            playerImageInScene.transform.position = position;
        }
    }

    private void OnPlayerReferenceChanged(NetworkObjectReference oldReference, NetworkObjectReference newReference)
    {
        if (newReference.TryGet(out NetworkObject networkObject))
        {
            newPlayer = networkObject.gameObject; // Actualiza la referencia local al nuevo jugador
            Debug.Log("[Client] Player reference updated.");
        }
    }
}