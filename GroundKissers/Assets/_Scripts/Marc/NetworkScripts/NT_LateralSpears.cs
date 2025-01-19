using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NT_LateralSpears : NetworkBehaviour
{
    private Animator anim;

    [SerializeField] private float minTimeBetweenActivations = 3f;
    [SerializeField] private float maxTimeBetweenActivations = 7f;
    [SerializeField] private float actionDuration = 0.1f;
    [SerializeField] private float cooldownTime = 3f;

    private NetworkVariable<bool> isPlaying = new NetworkVariable<bool>(
        false, // Valor inicial
        NetworkVariableReadPermission.Everyone, // Todos pueden leer
        NetworkVariableWritePermission.Server // Solo el servidor puede escribir
    );

    public bool IsPlaying => isPlaying.Value;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        anim = GetComponent<Animator>();

        if (IsServer) // Solo el servidor controla la lógica
        {
            StartCoroutine(ActivateActionPeriodically());
        }

        // Sincronizar animación al estado actual cuando un cliente se conecta
        isPlaying.OnValueChanged += (oldValue, newValue) =>
        {
            anim.SetBool("Action", newValue);
        };
    }

    private IEnumerator ActivateActionPeriodically()
    {
        while (true)
        {
            // Generar un tiempo aleatorio entre activaciones
            float randomTimeBetweenActivations = Random.Range(minTimeBetweenActivations, maxTimeBetweenActivations);

            yield return new WaitForSeconds(randomTimeBetweenActivations);

            SetIsPlaying(true);

            yield return new WaitForSeconds(actionDuration);

            SetIsPlaying(false);

            yield return new WaitForSeconds(cooldownTime);
        }
    }

    private void SetIsPlaying(bool value)
    {
        isPlaying.Value = value; // Actualizar el estado en el servidor
    }
}
