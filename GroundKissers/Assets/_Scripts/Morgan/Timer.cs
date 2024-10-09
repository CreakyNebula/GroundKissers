using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] int min, seg;     //variable de min y segs
    [SerializeField] TMP_Text timer;   //componente de texto se encarga de mostrarlo por pantalla

    private float remaining;    //almacena el tiempo total que queda
    private bool onGoing;       

    private void Awake()       //metodo que se llama automaticamente cuando el script comienza
    {
        remaining = (min * 60) + seg;    //convertimos min en seg y sumamos los seg para el tiempo total
        onGoing= true;      //indica que el temporizador está activo
    }

    // Update is called once per frame
    void Update()
    {
        if (onGoing)      //comprueba si el temporizador está en marcha
        {
            remaining -= Time.deltaTime;      //se resta el tiempo que ha pasado en ese frae del tiempo restante

            if(remaining < 1)     //si el tiempo es menor a 1 detenemos el contador
            {
                onGoing= false;    
                //Acabar Partida
            }
            int tempMin = Mathf.FloorToInt(remaining / 60);     //calcula cuantos minutos quedan dvidiendo entre 60
            int tempSeg = Mathf.FloorToInt(remaining % 60);     //clacula segundos restantes 
            timer.text = string.Format("{0:00}:{1:00}", tempMin, tempSeg);    //convierte los mintos y segundos en texto y lo actualiza en la pantalla
        }
    }
}
