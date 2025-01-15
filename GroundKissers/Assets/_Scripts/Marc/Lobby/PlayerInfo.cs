using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{

   
    public Image miBarraDeJugador;
    public Color playerColor;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (miBarraDeJugador != null)
        {
            playerColor = miBarraDeJugador.color;
        }
    }
}
