using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingSawManager : MonoBehaviour
{
    [Header("Configuración de lanzamiento")]
    [SerializeField] private float launchPower = 10f; // Potencia de lanzamiento
    [SerializeField] private Vector2 launchDirection = Vector2.one; // Dirección de lanzamiento
    [SerializeField] private int maxCollisions = 3;

    [Header("Gizmo")]
    [SerializeField] private float gizmoLength = 2f;

    private int collisionsCount;
    private Rigidbody2D rb;
    private Vector2 initialForce;
    private Vector3 lastVelocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Normaliza la dirección y multiplica por la potencia
        initialForce = launchDirection.normalized * launchPower;
        rb.AddForce(initialForce, ForceMode2D.Impulse);
    }

    private void Update()
    {
        lastVelocity = rb.velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collisionsCount < maxCollisions)
        {
            if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Wall") ||collision.gameObject.CompareTag("FallingPlatform") )
            {
                collisionsCount++;

                float speed = lastVelocity.magnitude;
                Vector3 direction = Vector3.Reflect(lastVelocity.normalized, collision.contacts[0].normal);
                rb.velocity = direction * Mathf.Max(speed, 0f);
            }

            if (collision.gameObject.CompareTag("Player"))
            {
                Invoke("DestroyOnCollisionWithStatic", 0.1f);
            }
        }
        else
        {
            Invoke("DestroyOnCollisionWithStatic", 0.1f);
            //DestroyOnCollisionWithStatic();
        }
    }

    private void DestroyOnCollisionWithStatic()
    {
        //TODO: Actualizar para aadir efecto de desaparecer: se rompe, se clava, se desvanece...
        Destroy(gameObject);
    }

    // Dibuja un gizmo que muestra la dirección de lanzamiento
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 direction = launchDirection.normalized;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)direction * gizmoLength);
    }
}
