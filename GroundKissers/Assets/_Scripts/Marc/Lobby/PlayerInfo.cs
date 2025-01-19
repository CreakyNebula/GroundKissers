using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInfo : NetworkBehaviour
{

   
    public Image miBarraDeJugador;
    public Color playerColor;
    public string playerName;
    public int playersCount;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
       
        LobbyPlayerSingleUI[] lobbyPlayerSingleUI = LobbyUI.Instance.GetComponentsInChildren<LobbyPlayerSingleUI>();
        GameObject barraNombre = GameObject.Find("PlayerNameText");
        GameObject lobbyCanvas = GameObject.Find("SuperLobbyCanvas");
        if(lobbyCanvas!=null)
        {
            playersCount = lobbyPlayerSingleUI.Length;
        }
        if (barraNombre != null)
        {
             playerName = barraNombre.GetComponent<TMP_Text>().text;

            foreach (LobbyPlayerSingleUI playerUI in lobbyPlayerSingleUI)
            {
                if(playerUI!=null)
                {
                    if (playerUI.playerNameText.text == playerName)
                    {
                        playerColor = playerUI.characterImage.color;

                    }
                }
            }
        }

        
    }
}
