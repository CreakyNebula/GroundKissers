using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Utility_UI : MonoBehaviour
{
    private Slider[] sliders;
    private Local_Player_Script playerScript;
    private Quaternion initialRotation;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = transform.parent.GetComponent<Local_Player_Script>();
        sliders = GetComponentsInChildren<Slider>();
        initialRotation = transform.rotation;

    }

    // Update is called once per frame
    void Update()
    {
        int index = playerScript.utilityCount; // Índice del slider a controlar

        // Asegúrate de que el índice esté dentro del rango de sliders
        if (index >= 0 && index < sliders.Length)
        {
            Slider currentSlider = sliders[index];
            currentSlider.maxValue = playerScript.maxWalkCounter; // Asignar el valor máximo
            currentSlider.value = playerScript.walkCounter; // Asignar el valor actual
        }

        // Reinicia todos los sliders con posición mayor a utilityCount
        for (int i = 0; i < sliders.Length; i++)
        {
            if (i > index)
            {
                sliders[i].value = 0; // Reiniciar el slider
            }
        }
        transform.rotation = initialRotation;

    }
}
