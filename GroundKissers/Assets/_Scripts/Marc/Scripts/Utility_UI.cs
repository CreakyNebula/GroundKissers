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
        int index = playerScript.utilityCount; // �ndice del slider a controlar

        // Aseg�rate de que el �ndice est� dentro del rango de sliders
        if (index >= 0 && index < sliders.Length)
        {
            Slider currentSlider = sliders[index];
            currentSlider.maxValue = playerScript.maxWalkCounter; // Asignar el valor m�ximo
            currentSlider.value = playerScript.walkCounter; // Asignar el valor actual
        }

        // Reinicia todos los sliders con posici�n mayor a utilityCount
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
