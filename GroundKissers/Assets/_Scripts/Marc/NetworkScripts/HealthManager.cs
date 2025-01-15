using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : NetworkBehaviour
{
    public GameObject PlayerPrefab; // Prefab del jugador que se va a instanciar
    private GameObject newPlayer;
    public bool playerSpawned;

    [SerializeField] private GameObject[] playerUi;
    private GameObject myPlayerUi;

    public Color myColor;
    private NetworkVariable<Color> playerColor = new NetworkVariable<Color>(Color.white, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);



    void Start()
    {

        if (IsOwner)
        {
            ActivateUiObjetctServerRpc(NetworkManager.Singleton.LocalClientId, true);

            myColor = GameObject.Find("LobbyStats").GetComponent<PlayerInfo>().playerColor;
            playerColor.Value = myColor;


        }

        // Suscribirse a cambios en la NetworkVariable
        playerColor.OnValueChanged += (oldValue, newValue) =>
        {
            UpdatePlayerColor(newValue);
        };

        // Configurar el color inicial en base al valor de la NetworkVariable
        UpdatePlayerColor(playerColor.Value);

    }
    private void Update()
    {
        if (IsClient && IsOwner && !playerSpawned)
        {
            playerSpawned = true;
            StartCoroutine(RequestPlayerSpawn());

        }
        
        
    }

    
    IEnumerator RequestPlayerSpawn()
    {
        yield return new WaitForSeconds(3);
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ulong clientId)
    {
        if(!IsServer) return;
        Vector3 position = new Vector3(0, 0, 0);
        newPlayer = Instantiate(PlayerPrefab, position, transform.rotation);
        // Instancia el jugador en el servidor
        // Agrega el objeto a la red y asigna el ownership al cliente que lo solicitó
        NetworkObject networkObject = newPlayer.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.SpawnWithOwnership(clientId);
            Debug.Log(networkObject + " payasada");
        }
        else
        {
            Debug.Log("Lo intenté y fallé");
        }
    }

    [ServerRpc]
    private void ActivateUiObjetctServerRpc(ulong clientId, bool isActive)
    {
        playerUi[clientId].SetActive(isActive);
        myPlayerUi = playerUi[clientId];
        ActivateUiObjetctClientRpc(clientId, isActive);
    }
    [ClientRpc]
    private void ActivateUiObjetctClientRpc(ulong clientId, bool isActive)
    {
        playerUi[clientId].SetActive(isActive);
        myPlayerUi = playerUi[clientId];

    }

    [ClientRpc]
    private void SetMyColorClientRpc(Color color)
    {
        myPlayerUi.GetComponent<Image>().color = color;
    }

    private void UpdatePlayerColor(Color newColor)
    {
        myPlayerUi.GetComponent<Image>().color = newColor;
    }



}