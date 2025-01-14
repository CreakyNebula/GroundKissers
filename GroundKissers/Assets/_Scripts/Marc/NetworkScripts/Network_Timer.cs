using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class Network_Timer : NetworkBehaviour
{
    [SerializeField] private int min, seg;     // Minutos y segundos iniciales
    [SerializeField] TMP_Text timer;           // Texto del temporizador en pantalla
    [SerializeField] EndGame endGame;
    [SerializeField] YouLost lostScript;

    public int Muertes;

    // Variable sincronizada
    private NetworkVariable<float> remainingTime = new NetworkVariable<float>();

    private bool onGoing;



    private void Start()
    {
        
           
            // Solo el servidor inicializa el tiempo restante
            remainingTime.Value = (min * 60) + seg;
            onGoing = true;
      
    }

    private void Update()
    {
        if (IsServer && onGoing)
        {
            // El servidor actualiza el temporizador
            remainingTime.Value -= Time.deltaTime;

            if (remainingTime.Value <= 0)
            {
                onGoing = false;
                remainingTime.Value = 0;
                endGame.MostrarEndGame();
            }
        }

        // Todos los clientes actualizan la UI
        UpdateUI();

        if (Muertes >= 3)
        {
            lostScript.MostrarEndGame();
        }
    }

    private void UpdateUI()
    {
        int tempMin = Mathf.FloorToInt(remainingTime.Value / 60);
        int tempSeg = Mathf.FloorToInt(remainingTime.Value % 60);
        timer.text = string.Format("{0:00}:{1:00}", tempMin, tempSeg);
    }
}
