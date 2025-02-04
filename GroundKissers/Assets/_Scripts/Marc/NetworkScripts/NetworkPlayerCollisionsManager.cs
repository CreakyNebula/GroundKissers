using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NWPlayerCollisionsManager : NetworkBehaviour
{
    [SerializeField] private int playerDeads = 0;
    [SerializeField] private Transform respawn;

    private PlayerConfigurationMenu playerConfigurationMenu;
    private GameObject statsManagerGO;
    private GameStatsManager gameStatsManager;
    private PlayerStatsPartida playerStatsPartida;

    public bool ready;
    public int PlayerDeads { get => playerDeads; set => playerDeads = value; }

    #region DANGER COLLISION
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsOwner) return;

        if (other.CompareTag("Danger") && ready)
        {
            Debug.Log("chispas");
            playerStatsPartida.muertes++;
            gameStatsManager.IncrementMuertesServerRpc();
            // Solicitar al servidor que destruya el objeto
            RequestDestroyServerRpc();
        }
    }
    #endregion 

    #region FALLING PLATFORMS COLLISION
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("FallingPlatform"))
        {
            // transform.parent = collision.gameObject.transform;
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

    // Solicita al servidor la destrucción del objeto
    [ServerRpc(RequireOwnership = false)]
    public void RequestDestroyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        DestroyThisObject();
    }

    // Método para destruir el objeto
    private void DestroyThisObject()
    {
        // Obtén el NetworkObject
        NetworkObject networkObject = GetComponent<NetworkObject>();

        if (networkObject != null)
        {
            // Despawn el objeto en la red y destrúyelo localmente en el servidor
            networkObject.Despawn(true);
            Destroy(gameObject);

            Debug.Log("Objeto destruido correctamente.");
        }
        else
        {
            Debug.LogWarning("No se encontró un NetworkObject en el objeto.");
        }
    }
}