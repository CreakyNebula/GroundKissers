using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyAssets : MonoBehaviour {



    public static LobbyAssets Instance { get; private set; }


    [SerializeField] private Image redSprite;
    [SerializeField] private Image blueSprite;
    [SerializeField] private Image greenSprite;
    [SerializeField] private Image yellowSprite;


    private void Awake() {
        Instance = this;
    }

    public Image GetSprite(LobbyManager.PlayerCharacter playerCharacter) {
        switch (playerCharacter) {
            default:
            case LobbyManager.PlayerCharacter.Red:   return redSprite;
            case LobbyManager.PlayerCharacter.Blue:    return blueSprite;
            case LobbyManager.PlayerCharacter.Green:   return greenSprite;
            case LobbyManager.PlayerCharacter.Yellow: return yellowSprite;

        }
    }

}