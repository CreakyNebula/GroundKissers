using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    

    private void Awake()
    {
        PlayerInputManager.instance.JoinPlayer(0);

    }

}

