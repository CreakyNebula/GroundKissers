using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class Local_Timer : MonoBehaviour
{
    [SerializeField] private int min, seg;     // Minutos y segundos iniciales
    [SerializeField] TMP_Text timer;           // Texto del temporizador en pantalla
    [SerializeField] EndGame endGame;
    [SerializeField] YouLost lostScript;

    public int Muertes;

    // Variable sincronizada
    private float remainingTime;

    private bool onGoing;



    private void Start()
    {
            // Solo el servidor inicializa el tiempo restante
            remainingTime = (min * 60) + seg;
            onGoing = true;
      
    }

    private void Update()
    {
        if ( onGoing)
        {
            // El servidor actualiza el temporizador
            remainingTime -= Time.deltaTime;

            if (remainingTime <= 0)
            {
                onGoing = false;
                remainingTime = 0;
                endGame.MostrarEndGame();
            }
        }

        // Todos los clientes actualizan la UI
        UpdateUI();
        
    }

    private void UpdateUI()
    {
        int tempMin = Mathf.FloorToInt(remainingTime / 60);
        int tempSeg = Mathf.FloorToInt(remainingTime % 60);
        timer.text = string.Format("{0:00}:{1:00}", tempMin, tempSeg);
    }
}
