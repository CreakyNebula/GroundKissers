using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCollisionsManager : MonoBehaviour
{
    //Este script comprueba las collisiones del player y modifica su estado en consecuencia
    [SerializeField] private int playerDeads = 0;
    [SerializeField] private Transform respawn;

    [SerializeField] private Local_Timer TimerScript;
    private PlayerConfigurationMenu playerConfigurationMenu;

    public int PlayerDeads { get => playerDeads; set => playerDeads = value; }

    #region DANGER COLLISION
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Danger"))
        {
            PlayerDeads++;
            Debug.Log("DAMAGE");
            //A�adir logica de manejo de nivel, reseteo, conteo de muertes, etc.
                    //Aitor: Yo activaria una booleana que active el codigo en otro script en un empty, para mantener un orden.
           //  transform.position = respawn.transform.position;
            TimerScript.Muertes++;
            PlayerInput playerInput = GetComponent<PlayerInput>();
            UI_PlayerPadre uI_PlayerPadre = GameObject.Find("HealthManager").GetComponent<UI_PlayerPadre>();
            Ui_PlayerIndividual uiMisVidas = uI_PlayerPadre.playerUi[playerInput.playerIndex].GetComponent<Ui_PlayerIndividual>();
            uiMisVidas.TakeDamage();
            if (uiMisVidas.vidas>=1)
            {
                playerConfigurationMenu.Respawn(playerInput);
              
            }

            Destroy(this.gameObject);
        }
    }
    #endregion 

    #region FALLING PLATFORMS COLLISION
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("FallingPlatform"))
        {
            //transform.parent = collision.gameObject.transform;
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
        TimerScript = GameObject.Find("PanelTimer").GetComponent<Local_Timer>();
        playerConfigurationMenu = GameObject.Find("PlayerConfigurationManager").GetComponent<PlayerConfigurationMenu>();

    }
}
