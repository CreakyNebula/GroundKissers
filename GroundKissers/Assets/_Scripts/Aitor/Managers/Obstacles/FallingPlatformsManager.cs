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

    private FloatingAnim fa;
    private Rigidbody2D rb;

    private bool isPlayerOnTop = false;
    private float currentCountdown;

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
            currentCountdown -= Time.deltaTime;
           //Debug.Log($"Cuenta atrás: {currentCountdown:F2} segundos");

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

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnTop = false;
        }
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
        GameObject child = gameObject.transform.GetChild(gameObject.transform.childCount - 1).gameObject;

        child.transform.parent = null;
    }
}
