using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoin_Manager : MonoBehaviour
{

    private PlayerInput playerInput;
    private Local_Player_Script playerScript;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
      /*  playerInput = GetComponent<PlayerInput>();
        var index = playerInput.playerIndex;
        var Players = FindObjectsOfType<Local_Player_Script>();
        playerScript = Players.FirstOrDefault(m => m.GetPlayerIndex() == index);*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddPlayer()
    {

    }
}
