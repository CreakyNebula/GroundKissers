using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : NetworkBehaviour
{
    public GameObject PlayerPrefab; // Prefab del jugador que se va a instanciar
    private GameObject newPlayer;
    public bool coroutineStarted;

    [SerializeField] private GameObject[] playerUi;
    private GameObject myPlayerUi;

    public Color myColor;
    private NetworkVariable<Color> playerColor = new NetworkVariable<Color>(Color.white, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);



    void Start()
    {
        if (IsClient && IsOwner && newPlayer == null)
        {
            SpawnPlayerServerRpc(OwnerClientId);
            Debug.Log("inicio");
        }

    }
    private void Update()
    {
        if (IsOwner)
        {
            myColor = GameObject.Find("LobbyStats").GetComponent<PlayerInfo>().playerColor;
            Vector4 colorAsVector4 = ColorToVector4(myColor);

            ActivateUiObjetctServerRpc(NetworkManager.Singleton.LocalClientId, true,colorAsVector4);
        }

        if (IsClient && IsOwner && newPlayer == null)
        {
            UpdateNewPlayer(OwnerClientId);
            if (!coroutineStarted)
            {
                Debug.Log("Muelte");
                StartCoroutine(RequestPlayerSpawn());
                coroutineStarted = true;
            }

        }

    }


    IEnumerator RequestPlayerSpawn()
    {
        yield return  new WaitForSeconds(3);
        if(newPlayer!=null)
        {
            coroutineStarted = false;
            yield break;
        }
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        yield return new WaitForSeconds(1);
        coroutineStarted = false;

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

    private void UpdateNewPlayer(ulong clientId)
    {
        Network_Player_Script[] allPlayers = FindObjectsOfType<Network_Player_Script>();
        foreach (var playerScript in allPlayers)
        {
            // Verifica si el objeto tiene el mismo OwnerClientId que este jugador
            if (playerScript.NetworkObject.OwnerClientId == clientId)
            {
                Debug.Log($"Objeto encontrado con el mismo propietario: {playerScript.gameObject.name}");
                newPlayer = playerScript.gameObject; // Guarda la referencia
                break; // Termina la búsqueda
            }
        }
    }

    [ServerRpc]
    private void ActivateUiObjetctServerRpc(ulong clientId, bool isActive,Vector4 color)
    {
        playerUi[clientId].SetActive(isActive);
        myPlayerUi = playerUi[clientId];
        myPlayerUi.GetComponent<Image>().color= new Color(color.x,color.y,color.z,color.w);
        ActivateUiObjetctClientRpc(clientId, isActive,color);
    }
    [ClientRpc]
    private void ActivateUiObjetctClientRpc(ulong clientId, bool isActive, Vector4 color)
    {
        playerUi[clientId].SetActive(isActive);
        myPlayerUi = playerUi[clientId];
        myPlayerUi.GetComponent<Image>().color = new Color(color.x, color.y, color.z, color.w);
    }
    [ServerRpc]
    private void SetMyColorServerRpc(Color color)
    {
        myPlayerUi.GetComponent<Image>().color = color;
    }

    [ClientRpc]
    private void SetMyColorClientRpc(Color color)
    {
        myPlayerUi.GetComponent<Image>().color = color;
    }
    private void UpdatePlayerColor(Color newColor)
    {
        playerUi[NetworkManager.Singleton.LocalClientId].GetComponent<Image>().color = newColor;
    }

    Vector4 ColorToVector4(Color color)
    {
        // Devuelve los componentes RGBA como Vector4
        return new Vector4(color.r, color.g, color.b, color.a);
    }

}