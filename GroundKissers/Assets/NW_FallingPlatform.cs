using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NW_FallingPlatform : NetworkBehaviour
{
    [Header("Explicacion")]
    [TextArea(3, 10)]
    public string comment;

    [Header("Cuenta atrás")]
    [SerializeField] private float countdownTime = 3f; // Tiempo de cuenta atrás en segundos
    [SerializeField] private float currentCountdown;

    private NW_FloatingAnim fa;
    private Rigidbody2D rb;

    private bool isPlayerOnTop = false;

  
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        rb = GetComponent<Rigidbody2D>();
        fa = GetComponent<NW_FloatingAnim>();
        
        currentCountdown = countdownTime;
        rb.isKinematic = true;
    }
    private void Update()
    {
        if (isPlayerOnTop &&IsServer)
        {
            //TODO: Añadir logica para efecto warning de que va a caer para feedback jugador. 
            StartCountDown();

            if (currentCountdown <= 0)
            {
                OnCountdownFinished();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnTop = true;
            // collision.gameObject.transform.parent = this.gameObject;
        }

        if (collision.gameObject.CompareTag("Floor"))
        {

            NetworkObject networkObject = GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                networkObject.Despawn(true);
                Debug.Log("cirrosis");
            }
        }
    }

    //De momento no necesitamos comprobar si deja de estar encima. Si fuera asi, esto bloquea el timer al apagar la booleana.
    //habria que arreglarlo.

    /*private void OnCollisionExit2D(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnTop = false;
        }
    }*/

    private void StartCountDown()
    {
        currentCountdown -= Time.deltaTime;

        Debug.Log("Mirame soy un efeco de warning cuidado que me caigo!!!!");
    }

    private void OnCountdownFinished()
    {
        ChildOut();

        if (rb != null)
        {
            fa.enabled = false;

            rb.isKinematic = false;
            rb.gravityScale = 1f;
        }
    }

    private void ChildOut()
    {
        if (transform.childCount > 1) //si tiene que tener otro hijo aqui seria + 1, no se como automatizarlo
        {
            GameObject child = gameObject.transform.GetChild(gameObject.transform.childCount - 1).gameObject; //obtenemos el ultimo hijo (el player)

            child.transform.parent = null;
        }
    }
}
