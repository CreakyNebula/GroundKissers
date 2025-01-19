using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NW_BouncingSawManager : NetworkBehaviour
{
    [Header("Configuración de lanzamiento")]
    [SerializeField] private float launchPower = 10f; // Potencia de lanzamiento
    [SerializeField] private Vector2 launchDirection ; // Dirección de lanzamiento
    [SerializeField] private int maxCollisions = 3;

    [Header("Gizmo")]
    [SerializeField] private float gizmoLength = 2f;

    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;

    private int collisionsCount;
    private Vector2 initialForce;
    private Vector3 lastVelocity;
    private Vector2 startVelocity;

    public  void Start()
    {

        if (IsServer)
        {
            launchDirection = transform.right;
            rb = GetComponent<Rigidbody2D>();
            circleCollider = GetComponent<CircleCollider2D>();

            // Normaliza la dirección y multiplica por la potencia
            initialForce = launchDirection.normalized * launchPower;
            rb.AddForce(initialForce, ForceMode2D.Impulse);
        }
    }

    private void Update()
    {
        if (!IsServer) return; // Solo el servidor controla la lógica

        lastVelocity = rb.velocity;

        if (rb.velocity.magnitude < startVelocity.magnitude)
        {
            rb.velocity = new Vector2(rb.velocity.x * 1.1f, rb.velocity.y * 1.1f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer) return; // Solo el servidor procesa las colisiones

        if (collisionsCount < maxCollisions)
        {
            if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("FallingPlatform"))
            {
                collisionsCount++;

                float speed = lastVelocity.magnitude;
                Vector3 direction = Vector3.Reflect(lastVelocity.normalized, collision.contacts[0].normal);
                rb.velocity = direction * Mathf.Max(speed, 0f);
            }

            if (collision.gameObject.CompareTag("Player"))
            {
                DestroySawServerRpc(); // Notificar a todos los clientes
            }
        }
        else
        {
            DestroySawServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroySawServerRpc()
    {
        DestroySawClientRpc();
        Destroy(gameObject); // El servidor destruye el objeto localmente
    }

    [ClientRpc]
    private void DestroySawClientRpc()
    {
        // Sincroniza la destrucción visual en los clientes
        if (gameObject != null)
        {
            // Agrega efectos visuales o animaciones antes de destruir
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return; // Solo el servidor procesa la lógica de trigger

        if (collision.gameObject.CompareTag("Wall"))
        {
            StartCoroutine(ActivateSawCollider());
        }
    }

    private IEnumerator ActivateSawCollider()
    {
        yield return new WaitForSeconds(0.3f);
        circleCollider.isTrigger = false;
        startVelocity = rb.velocity;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 direction = launchDirection.normalized;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)direction * gizmoLength);
    }
}
