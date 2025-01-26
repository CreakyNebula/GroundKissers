using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;

public class NWPlayerCollisionsManager : NetworkBehaviour
{
    //Este script comprueba las collisiones del player y modifica su estado en consecuencia
    [SerializeField] private int playerDeads = 0;
    [SerializeField] private Transform respawn;

    [SerializeField] private Local_Timer TimerScript;
    private PlayerConfigurationMenu playerConfigurationMenu;
    private GameObject statsManagerGO;
    private GameStatsManager gameStatsManager;
    private PlayerStatsPartida playerStatsPartida;

    public int PlayerDeads { get => playerDeads; set => playerDeads = value; }

    #region DANGER COLLISION
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsOwner) return;

        if (other.gameObject.tag ==("Danger")&& IsOwner)
        {
            Debug.Log(statsManagerGO.name);
           
            playerStatsPartida.muertes++;
            Debug.Log("chispas");
            RequestDestroyServerRpc();
        }
    }
    #endregion 

    private void Update()
    {/*
        if(!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.A)) { gameStatsManager.stats.totalMuertes++; Debug.Log("Total Muertes: " + gameStatsManager.stats.totalMuertes); }
        if (Input.GetKeyDown(KeyCode.C)) { gameStatsManager.stats.totalZancadillas++; Debug.Log("Total Zancadillas: " + gameStatsManager.stats.totalZancadillas); }
        if (Input.GetKeyDown(KeyCode.D)) { gameStatsManager.stats.totalParrys++; Debug.Log("Total Parrys: " + gameStatsManager.stats.totalParrys); }

        // Enviar estadísticas al servidor al presionar la tecla R
        if (Input.GetKeyDown(KeyCode.R)) gameStatsManager.SaveGameStats();*/
    }
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
        if (IsOwner)
        {
            statsManagerGO = GameObject.Find("StatsManager");
            gameStatsManager = statsManagerGO.GetComponent<GameStatsManager>();
            playerStatsPartida = statsManagerGO.GetComponent<PlayerStatsPartida>();

        }
    }
    [ServerRpc]
    public void RequestDestroyServerRpc()
    {
        DestroyThisObject();
    }

    private void DestroyThisObject()
    {
       
       

        Debug.Log(playerStatsPartida.muertes);

        NetworkObject networkObject = GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Despawn(true);
            Destroy(this.gameObject);
            Debug.Log("cirrosis");
        }
        else
        {
            Debug.Log("cachalote");
        }
    }
}
