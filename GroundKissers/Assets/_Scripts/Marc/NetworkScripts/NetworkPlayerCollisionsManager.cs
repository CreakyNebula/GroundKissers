using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NWPlayerCollisionsManager : NetworkBehaviour
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
        if (!IsOwner) return;

        if (other.gameObject.tag ==("Danger"))
        {
            Debug.Log("chispas");
            RequestDestroyServerRpc();
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
    }
    [ServerRpc]
    public void RequestDestroyServerRpc()
    {
        DestroyThisObject();
    }

    private void DestroyThisObject()
    {
        NetworkObject networkObject = GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Despawn(true);
            Debug.Log("cirrosis");
        }
        else
        {
            Debug.Log("cachalote");
        }
    }
}
