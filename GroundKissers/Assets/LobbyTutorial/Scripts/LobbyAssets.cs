using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static LobbyManager;

public class LobbyAssets : MonoBehaviour {



    public static LobbyAssets Instance { get; private set; }


    [SerializeField] private Sprite green;
    [SerializeField] private Sprite red;
    [SerializeField] private Sprite yellow;
    [SerializeField] private Sprite blue;



    private void Awake() {
        Instance = this;
    }

    public Sprite GetSprite(LobbyManager.PlayerCharacter playerCharacter) {
        switch (playerCharacter) {
            default:
            case LobbyManager.PlayerCharacter.Green:   return green;
            case LobbyManager.PlayerCharacter.Yellow:    return yellow;
            case LobbyManager.PlayerCharacter.Red:   return red;
            case LobbyManager.PlayerCharacter.Blue: return blue;
        }
    }
    

}