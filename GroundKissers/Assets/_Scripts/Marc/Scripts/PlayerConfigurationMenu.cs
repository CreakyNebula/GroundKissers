using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerConfigurationMenu : MonoBehaviour
{
    
    private bool hasLogged;
    public string sceneToLoad;
    public GameObject playerPrefab;
    private PlayerInputManager playerInputManager;

    public List<PlayerInput> playerList=new List<PlayerInput>();
    public List<InputDevice> playerDevice =new List<InputDevice>();
    public List<Color> playerColor = new List<Color>();
    public List<GameObject> ui = new List<GameObject>();

    [SerializeField]private InputAction joinAction;
    [SerializeField]public InputAction leaveAction;

    public bool spawnear;
    public static PlayerConfigurationMenu Instance { get; private set; }

    public int devicesConected;
    bool llegue;
    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        if (Instance == null)
        {
            Time.timeScale = 1f; // Detener completamente el tiempo

            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantener el GameManager entre escenas

        }
        else
        {
            Destroy(gameObject); // Eliminar el duplicado
        }

        joinAction.Enable();

        joinAction.performed += context => JoinAction(context);
        

        leaveAction.Enable();
        leaveAction.performed += context => LeaveAction(context);
    }

    

    private void Start()
    {
       // PlayerInputManager.instance.JoinPlayer(0,-1,null);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == sceneToLoad && !hasLogged)
        {
            hasLogged = true; // Marca que el mensaje ya fue mostrado
             PlayerInputManager.instance.playerPrefab = playerPrefab;
            PlayerInputManager.instance.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;

            
        }
        if (spawnear)
        {
            spawnear = false;
           

        }
        
        

    }
    

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if(SceneManager.GetActiveScene().name != sceneToLoad)
        {
            devicesConected++;
            playerList.Add(playerInput);
            playerColor.Add(Color.white);
        }
        else
        {
            for (int i = 0; i < playerList.Count; i++)
            {
                if (i == playerInput.playerIndex) // Verifica si el elemento está Missing.
                {
                    playerList[i] = playerInput; // Sustituye el elemento.

                    Debug.Log($"Jugador reemplazado en la posición {i}.");
                    return;
                }
            }
        }

        


    }
    void OnPlayerLeft(PlayerInput playerInput)
    {

    }

    private void JoinAction(InputAction.CallbackContext context)
    {
        PlayerInputManager.instance.JoinPlayer();
    }
    private void LeaveAction(InputAction.CallbackContext context)
    {

    }
    public void Respawn(PlayerInput playerInput)
    {
        StartCoroutine(RespawnCorroutine(playerInput));

    }
    IEnumerator RespawnCorroutine(PlayerInput playerInput)
    {
        InputDevice device = playerInput.devices[0];
        int idOculto = playerInput.playerIndex;

        yield return new WaitForSeconds(3);
        PlayerInputManager.instance.JoinPlayer(idOculto, -1, null, device);

    }
    
}




