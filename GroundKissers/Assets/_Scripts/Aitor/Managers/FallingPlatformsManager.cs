using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatformsManager : MonoBehaviour
{
    [SerializeField] private bool isPlayerOnTop = false;
    [SerializeField] private float countdownTime = 3f; // Tiempo de cuenta atrás en segundos
    [SerializeField] private FloatingAnim fa;

    private Rigidbody2D rb;
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
