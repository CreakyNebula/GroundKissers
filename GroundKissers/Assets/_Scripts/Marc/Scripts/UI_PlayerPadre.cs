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
    public bool coroutineStarted;
    void Start()
    {
        playerConfigurationMenu = GameObject.Find("PlayerConfigurationManager").GetComponent<PlayerConfigurationMenu>();
        for (int i = 0; i < playerConfigurationMenu.playerList.Count; i++)
        {
            playerUi[i].SetActive(true);
            Image image = playerUi[i].GetComponent<Image>();
            image.color = playerConfigurationMenu.playerColor[i];
            Ui_PlayerIndividual ui_PlayerIndividual = playerUi[i].GetComponent<Ui_PlayerIndividual>();
            ui_PlayerIndividual.myId = i;

        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckWinnerAndShowCanvas();
    }
    public void CheckWinnerAndShowCanvas()
    {
        int alivePlayerCount = 0;
        int winnerId = -1;

        // Recorre todos los objetos de playerUi para verificar las vidas
        foreach (GameObject ui in playerUi)
        {
            if (ui.activeSelf) // Asegúrate de verificar solo los jugadores activos
            {
                Ui_PlayerIndividual uiPlayer = ui.GetComponent<Ui_PlayerIndividual>();
                if (uiPlayer != null && uiPlayer.vidas > 0) // Comprueba si el jugador tiene vidas
                {
                    alivePlayerCount++;
                    winnerId = uiPlayer.myId; // Guarda el ID del jugador que sigue vivo
                }
            }
        }

        if (alivePlayerCount == 1 && !coroutineStarted) // Si solo queda un jugador vivo
        {
            coroutineStarted = true;
            StartCoroutine(SlowTimeAndShowWinner(winnerId));
        }
    }

    private IEnumerator SlowTimeAndShowWinner(int winnerId)
    {
        // Ralentizar el tiempo poco a poco
        while (Time.timeScale > 0.1f)
        {
            Time.timeScale -= 0.01f; // Reduce la escala del tiempo gradualmente
            yield return new WaitForSecondsRealtime(0.05f); // Espera en tiempo real para no depender de Time.timeScale
        }

        Time.timeScale = 0.1f; // Detener completamente el tiempo

        // Mostrar el canvas del ganador
        Canvas winnerCanvas = GameObject.Find("WinnerCanvas").GetComponent<Canvas>();
        winnerCanvas.gameObject.SetActive(true);

        Text winnerText = winnerCanvas.transform.GetChild(0).GetComponentInChildren<Text>();
        if (winnerText != null)
        {
            winnerText.text = "¡El ganador es el Jugador " + (winnerId + 1) + "!";
        }
    }
}
