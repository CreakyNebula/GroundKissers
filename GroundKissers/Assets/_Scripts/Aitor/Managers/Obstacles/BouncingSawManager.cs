using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingSawManager : MonoBehaviour
{
    [Header("Velocidades")]
    [SerializeField] private float forceX;
    [SerializeField] private float forceY;

    [Header("Gizmo")]
    [SerializeField] private float gizmoLength = 2f; // Longitud del gizmo

    private int collisionsCount;

    private Rigidbody2D rb;
    private Vector2 initialForce;
    private Vector3 lastVelocity;

    private float gravity = 9.8f;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialForce = new Vector2(gravity * forceX, gravity * forceY);
        rb.AddForce(initialForce);
    }

    private void Update()
    {
        lastVelocity = rb.velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collisionsCount < 3)
        {
            if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Wall"))
            {
                collisionsCount++;

                float speed = lastVelocity.magnitude;
                Vector3 direction = Vector3.Reflect(lastVelocity.normalized, collision.contacts[0].normal);
                rb.velocity = direction * Mathf.Max(speed, 0f);
            }
        }
        else
        {
            DestroyOnCollisionWithStatic();
        }
    }

    private void DestroyOnCollisionWithStatic()
    {
        //TODO: Actualizar para aadir efecto de desaparecer: se rompe, se clava, se desvanece...
        Destroy(gameObject);
    }

    // Dibuja un gizmo que muestra la dirección inicial de la fuerza
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 direction = new Vector3(forceX, forceY, 0).normalized;
        Gizmos.DrawLine(transform.position, transform.position + direction * gizmoLength);
    }
}
