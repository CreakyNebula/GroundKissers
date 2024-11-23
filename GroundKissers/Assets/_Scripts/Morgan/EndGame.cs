using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    [SerializeField] GameObject EndGamePanel;

    private int muerte;

    public void MostrarEndGame()
    {
        gameObject.SetActive(true);
    }

    

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void MainMenu()
    {
        //SceneManager.LoadScene("MainMenu");
      //  UnityEditor.EditorApplication.isPlaying = false; // Para detener el juego en el editor
    }

    public void Muerte()
    {
        muerte++;
    }
}
