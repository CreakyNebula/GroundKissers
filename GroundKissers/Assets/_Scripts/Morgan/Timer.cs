using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] int min, seg;
    [SerializeField] TMP_Text timer;

    private float remaining;
    private bool onGoing;

    private void Awake()
    {
        remaining = (min * 60) + seg;
        onGoing= true;
    }

    // Update is called once per frame
    void Update()
    {
        if (onGoing)
        {
            remaining -= Time.deltaTime;
            if(remaining < 1)
            {
                onGoing= false;
                //Acabar Partida
            }
            int tempMin = Mathf.FloorToInt(remaining / 60);
            int tempSeg = Mathf.FloorToInt(remaining % 60);
            timer.text = string.Format("{0:00}:{1:00}", tempMin, tempSeg);
        }
    }
}
