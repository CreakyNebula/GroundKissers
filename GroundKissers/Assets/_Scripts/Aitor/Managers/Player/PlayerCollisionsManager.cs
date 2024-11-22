using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionsManager : MonoBehaviour
{
    //Este script comprueba las collisiones del player y modifica su estado en consecuencia
    [SerializeField] private int playerDeads = 0;
    [SerializeField] private Transform respawn;

    private Timer TimerScript;

    public int PlayerDeads { get => playerDeads; set => playerDeads = value; }

    #region DANGER COLLISION
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Danger"))
        {
            PlayerDeads++;
            Debug.Log("DAMAGE");
            //Añadir logica de manejo de nivel, reseteo, conteo de muertes, etc.
                    //Aitor: Yo activaria una booleana que active el codigo en otro script en un empty, para mantener un orden.
             transform.position = respawn.transform.position;
            TimerScript.Muertes++;
        }
    }
    #endregion 

    #region FALLING PLATFORMS COLLISION
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("FallingPlatform"))
        {
            transform.parent = collision.gameObject.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("FallingPlatform"))
        {
            transform.parent = null;
        }
    }
    #endregion

    private void Start()
    {
        TimerScript = GameObject.Find("PanelTimer").GetComponent<Timer>();
    }
}
