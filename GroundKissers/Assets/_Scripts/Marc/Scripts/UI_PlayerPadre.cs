using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class UI_PlayerPadre : MonoBehaviour
{
    public GameObject[] playerUi;
    private PlayerConfigurationMenu playerConfigurationMenu;
    // Start is called before the first frame update
    public bool coroutineStarted;
    private Image winnerCanvas;

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
            PlayerCollisionsManager collisionsScript = playerUi[winnerId].GetComponent<Ui_PlayerIndividual>().scriptPlayer.gameObject.GetComponent<PlayerCollisionsManager>();
            collisionsScript.enabled = false;
        }
    }

    private IEnumerator SlowTimeAndShowWinner(int winnerId)
    {
        winnerCanvas = GameObject.Find("WinnerCanvas").transform.GetChild(0).GetComponent<Image>();

        Color winnerCanvasColor = winnerCanvas.color;
        TMP_Text winnerText = GameObject.Find("WinnerText").GetComponent<TMP_Text>();
        if (winnerText != null)
        {
            winnerText.text = "¡El ganador es el Jugador " + (winnerId + 1) + "!";


        }

        // Ralentizar el tiempo poco a poco
        while (Time.timeScale > 0.1f)
        {
            Time.timeScale -= 0.01f; // Reduce la escala del tiempo gradualmente
            yield return new WaitForSecondsRealtime(0.02f); // Espera en tiempo real para no depender de Time.timeScale
            winnerCanvas.color = new Vector4(winnerCanvasColor.r, winnerCanvasColor.g, winnerCanvasColor.b, 1 - Time.timeScale);
            winnerText.color = new Vector4(winnerText.color.r, winnerText.color.g, winnerText.color.b, 1 - Time.timeScale);
        }

        Time.timeScale = 0.1f; // Detener completamente el tiempo
        Destroy(playerConfigurationMenu.gameObject);

        // Mostrar el canvas del ganador
        winnerCanvas.gameObject.SetActive(true);

        
        yield return new WaitForSecondsRealtime(5f); // Espera en tiempo real para no depender de Time.timeScale
        SceneManager.LoadScene("Selector");

    }
}
