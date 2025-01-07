using System;
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

    public event System.Action<PlayerInput> PlayerJoinedGame;
    public event System.Action<PlayerInput> PlayerLeftGame;

    [SerializeField]private InputAction joinAction;
    [SerializeField]public InputAction leaveAction;

    public static PlayerConfigurationMenu Instance { get; private set; }

    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        if (Instance == null)
        {
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

        }
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
        playerList.Add(playerInput);
        if(PlayerJoinedGame != null)
        {
            PlayerJoinedGame(playerInput);
        }
    }
    void OnPlayerLeft(PlayerInput playerInput)
    {

    }

    private void JoinAction(InputAction.CallbackContext context)
    {
        PlayerInputManager.instance.JoinPlayerFromActionIfNotAlreadyJoined(context);
    }
    private void LeaveAction(InputAction.CallbackContext context)
    {

    }

}




