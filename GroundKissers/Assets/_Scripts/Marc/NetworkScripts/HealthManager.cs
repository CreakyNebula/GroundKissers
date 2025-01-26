using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    private NetworkVariable<int> vidas = new NetworkVariable<int>(5,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner ); private GameObject[] childrenWithHeart;
    [SerializeField] private Sprite deadHeartSprite;
    public bool firstTime = true;

    public NetworkVariable<bool> alive = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<FixedString32Bytes> playerName = new NetworkVariable<FixedString32Bytes>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    [SerializeField] private Image winnerCanvas; // Canvas que se activa al finalizar
    [SerializeField] private Text winnerText; // Texto dentro del canvas para mostrar el ganador


    void Start()
    {
        




    }
    private void Update()
    {

        //if(StartGame() == true)
        {
            if (IsOwner) // Asegúrate de que esta lógica se ejecute en el servidor
            {
                CheckForWinner();
            }

            if (IsOwner)
            {
                myColor = GameObject.Find("LobbyStats").GetComponent<PlayerInfo>().playerColor;
                playerName.Value = GameObject.Find("LobbyStats").GetComponent<PlayerInfo>().playerName;

                Vector4 colorAsVector4 = ColorToVector4(myColor);

                ActivateUiObjetctServerRpc(NetworkManager.Singleton.LocalClientId, true, colorAsVector4);


            }



            if (IsClient && IsOwner)
            {
                UpdateNewPlayer(NetworkManager.Singleton.LocalClientId);

                if (newPlayer == null)
                {
                    if (!coroutineStarted && alive.Value == true)
                    {
                        if (!firstTime)
                        {
                            Debug.Log("Muelte");
                            TakeDamage();
                        }


                        firstTime = false;
                        StartCoroutine(RequestPlayerSpawn());
                        coroutineStarted = true;
                    }
                }



            }
        }
       

    }


    IEnumerator RequestPlayerSpawn()
    {
        yield return  new WaitForSeconds(3);
        if(newPlayer!=null || alive.Value==false)
        {
            coroutineStarted = false;
            yield break;
        }
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        yield return new WaitForSeconds(1);
        coroutineStarted = false;
        firstTime = false;


    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ulong clientId)
    {
        if(!IsServer) return;
        Vector3 position = new Vector3(0, 0, 0);
        GameObject newPlayerLocal = Instantiate(PlayerPrefab, position, transform.rotation);
        // Instancia el jugador en el servidor
        // Agrega el objeto a la red y asigna el ownership al cliente que lo solicitó
        NetworkObject networkObject = newPlayerLocal.GetComponent<NetworkObject>();
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
        childrenWithHeart = FindChildrenWithHeart();

        ActivateUiObjetctClientRpc(clientId, isActive,color);
    }
    [ClientRpc]
    private void ActivateUiObjetctClientRpc(ulong clientId, bool isActive, Vector4 color)
    {

        playerUi[clientId].SetActive(isActive);
        myPlayerUi = playerUi[clientId];
        childrenWithHeart = FindChildrenWithHeart();

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
  

    Vector4 ColorToVector4(Color color)
    {
        // Devuelve los componentes RGBA como Vector4
        return new Vector4(color.r, color.g, color.b, color.a);
    }

    private void TakeDamage()
    {
        vidas.Value--;
        Debug.Log(vidas.Value);

        if (vidas.Value <= 0)
        {
            alive.Value = false;
            Debug.Log("fin");
        }
       
        TakeDamageServerRpc(vidas.Value);
        
    }
    [ServerRpc]
    private void TakeDamageServerRpc(int vidaNum)
    {
        Image image = childrenWithHeart[vidaNum].gameObject.GetComponent<Image>();
        image.sprite = deadHeartSprite;
        TakeDamageClientRpc(vidaNum);
    }
    [ClientRpc]
    private void TakeDamageClientRpc(int vidaNum)
    {
        Image image = childrenWithHeart[vidaNum].gameObject.GetComponent<Image>();
        image.sprite = deadHeartSprite;

    }
    GameObject[] FindChildrenWithHeart()
    {
        // Lista temporal para almacenar los objetos encontrados
        List<GameObject> foundChildren = new List<GameObject>();

        // Recorre todos los hijos directos e indirectos del objeto
        Transform[] childTransforms = myPlayerUi.GetComponentsInChildren<Transform>();

        foreach (Transform child in childTransforms)
        {
            // Si el nombre del objeto contiene "Heart", lo añade a la lista
            if (child.name.Contains("Heart"))
            {
                foundChildren.Add(child.gameObject);
            }
        }

        // Convierte la lista a un array y lo devuelve
        return foundChildren.ToArray();
    }
    private void CheckForWinner()
    {
        
        // Obtén todos los jugadores en la escena
        HealthManager[] allPlayers = FindObjectsOfType<HealthManager>();

        // Filtra a los jugadores que aún están vivos
        List<HealthManager> alivePlayers = new List<HealthManager>();
        foreach (var player in allPlayers)
        {
            if (player.alive.Value)
            {
                alivePlayers.Add(player);
            }
        }

        // Si solo queda un jugador vivo, se activa el canvas de ganador
        if (alivePlayers.Count == 0)
        {/*
            HealthManager winner = alivePlayers[0];
            StartCoroutine(SlowTimeAndShowWinner(winner.playerName.Value.ToString()));*/

        }
    }

    private IEnumerator SlowTimeAndShowWinner(string winnerName)
    {
        // Encuentra y configura el canvas y el texto
        winnerCanvas = GameObject.Find("WinnerCanvas").transform.GetChild(0).GetComponent<Image>();
        TMP_Text winnerText = GameObject.Find("WinnerText").GetComponent<TMP_Text>();
        winnerText.text = $"¡El ganador es {winnerName}!";

        Debug.Log("socorro");
        Color winnerCanvasColor = winnerCanvas.color;

        if (winnerText != null)
        {
            winnerText.text = "¡El ganador es el Jugador " + (winnerName ) + "!";
        }

        // Ralentizar el tiempo poco a poco y aumentar la transparencia del canvas y el texto
        while (Time.timeScale > 0.1f)
        {
            Time.timeScale -= 0.01f; // Reduce la escala del tiempo gradualmente
            yield return new WaitForSecondsRealtime(0.02f); // Espera en tiempo real
            winnerCanvas.color = new Vector4(winnerCanvasColor.r, winnerCanvasColor.g, winnerCanvasColor.b, 1 - Time.timeScale);
            winnerText.color = new Vector4(winnerText.color.r, winnerText.color.g, winnerText.color.b, 1 - Time.timeScale);
        }

        Time.timeScale = 0.1f; // Fija la escala del tiempo en 0.1 para casi detenerlo
        winnerCanvas.gameObject.SetActive(true); // Activa el canvas de ganador

        yield return new WaitForSecondsRealtime(5f); // Mantén la pantalla por 5 segundos en tiempo real

        // Vuelve a la escena principal o menú de selección
        //SceneManager.LoadScene(this.scene);
        if (IsServer) // Solo el servidor debe cargar la escena para todos
        {
            ChangeSceneClientRpc("MainMenuScene");

        }
        // Carga nuevamente la escena actual

    }

    // ClientRpc para cambiar de escena en los clientes
    [ClientRpc]
    private void ChangeSceneClientRpc(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    private bool StartGame()
    {

       int jugadoresMax = GameObject.Find("LobbyStats").GetComponent<PlayerInfo>().playersCount;
        HealthManager[] allPlayers = FindObjectsOfType<HealthManager>();

        if(jugadoresMax == allPlayers.Length)
        {
            return true;
        }
        else
        {
            return false;
        }


    }
}