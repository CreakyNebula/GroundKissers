using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NT_Spikes : NetworkBehaviour
{
    [SerializeField] private Animator anim;

    [Header("Animación")]
    [SerializeField] private float minTimeBetweenActivations = 3f;
    [SerializeField] private float maxTimeBetweenActivations = 7f;
    [SerializeField] private float actionDuration = 0.1f;
    [SerializeField] private float cooldownTime = 3f;

    [Header("Valores de aparición")]
    [SerializeField] private float elevationDistance = 0.5f; // Distancia de elevación
    [SerializeField] private float elevationDuration = 0.5f; // Duración de la elevación
    [SerializeField] private float pauseBeforeAction = 1f;

    private NetworkVariable<Vector3> initialPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer) // Solo el servidor controla la lógica
        {

            initialPosition.Value = transform.position; // Asegurarse de registrar la posición inicial al iniciar
            StartCoroutine(ActivateActionPeriodicallyServer());
        }
    }
    private IEnumerator ActivateActionPeriodicallyServer()
    {
        while (true)
        {
            // Generar un tiempo aleatorio entre activaciones
            float randomTimeBetweenActivations = Random.Range(minTimeBetweenActivations, maxTimeBetweenActivations);

            yield return new WaitForSeconds(randomTimeBetweenActivations);

            // Iniciar elevación en el servidor y notificar a los clientes
            ElevateObjectClientRpc();

            yield return new WaitForSeconds(pauseBeforeAction);

            // Iniciar animación en el servidor y notificar a los clientes
            TriggerActionClientRpc(true);

            yield return new WaitForSeconds(actionDuration);

            // Finalizar animación en el servidor y notificar a los clientes
            TriggerActionClientRpc(false);

            // Descender el objeto en el servidor y notificar a los clientes
            DescendObjectClientRpc();

            // Esperar el tiempo de enfriamiento antes de reiniciar el ciclo
            yield return new WaitForSeconds(cooldownTime);
        }
    }

    [ClientRpc]
    private void ElevateObjectClientRpc()
    {
        StartCoroutine(ElevarObjeto());
    }

    [ClientRpc]
    private void DescendObjectClientRpc()
    {
        StartCoroutine(DescenderObjeto());
    }

    [ClientRpc]
    private void TriggerActionClientRpc(bool isActive)
    {
        anim.SetBool("Action", isActive);
    }

    private IEnumerator ElevarObjeto()
    {
        Vector3 targetPosition = initialPosition.Value + Vector3.up * elevationDistance;
        float elapsedTime = 0f;

        while (elapsedTime < elevationDuration)
        {
            transform.position = Vector3.Lerp(initialPosition.Value, targetPosition, elapsedTime / elevationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }

    private IEnumerator DescenderObjeto()
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < elevationDuration)
        {
            transform.position = Vector3.Lerp(startPosition, initialPosition.Value, elapsedTime / elevationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = initialPosition.Value;
    }
}
