using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;



public class UI_PlayerPadre : MonoBehaviour
{
    public GameObject[] playerUi;
    private PlayerConfigurationMenu playerConfigurationMenu;
    // Start is called before the first frame update
    void Start()
    {
        playerConfigurationMenu = GameObject.Find("PlayerConfigurationManager").GetComponent<PlayerConfigurationMenu>();
        for(int i = 0; i < playerConfigurationMenu.playerList.Count; i++)
        {
            playerUi[i].SetActive(true);
            Image image =playerUi[i].GetComponent<Image>();
            image.color = playerConfigurationMenu.playerColor[i];
            Ui_PlayerIndividual ui_PlayerIndividual = playerUi[i].GetComponent<Ui_PlayerIndividual>();
            ui_PlayerIndividual.myId = i;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
