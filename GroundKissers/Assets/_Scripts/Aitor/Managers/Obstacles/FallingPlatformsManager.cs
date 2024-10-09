using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FloatingAnim))]
public class FallingPlatformsManager : MonoBehaviour
{
    [Header("Explicacion")]
    [TextArea(3,10)]
    public string comment;

    [Header("Cuenta atrás")]
    [SerializeField] private float countdownTime = 3f; // Tiempo de cuenta atrás en segundos
    [SerializeField] private float currentCountdown;

    private FloatingAnim fa;
    private Rigidbody2D rb;

    private bool isPlayerOnTop = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        fa = GetComponent<FloatingAnim>();

        currentCountdown = countdownTime;
        rb.isKinematic = true;
    }

    private void Update()
    {
        if (isPlayerOnTop)
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
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnTop = true;
        }

        if (collision.gameObject.CompareTag("Floor"))
        {
            Destroy(gameObject);
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
