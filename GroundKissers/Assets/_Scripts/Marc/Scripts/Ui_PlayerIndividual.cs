using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ui_PlayerIndividual : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private Slider[] sliders; // Para inspecci�n opcional
    [SerializeField] public Local_Player_Script scriptPlayer;
    public int myId;
    void Start()
    {
        // Busca todos los sliders hijos de este objeto y los guarda en el array
        sliders = GetComponentsInChildren<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(scriptPlayer == null)
        {

        }

        int index = scriptPlayer.utilityCount; // �ndice del slider a controlar

        // Aseg�rate de que el �ndice est� dentro del rango de sliders
        if (index >= 0 && index < sliders.Length)
        {
            Slider currentSlider = sliders[index];
            currentSlider.maxValue = scriptPlayer.maxWalkCounter; // Asignar el valor m�ximo
            currentSlider.value = scriptPlayer.walkCounter; // Asignar el valor actual
        }

        // Reinicia todos los sliders con posici�n mayor a utilityCount
        for (int i = 0; i < sliders.Length; i++)
        {
            if (i > index)
            {
                sliders[i].value = 0; // Reiniciar el slider
            }
        }
    }
}
