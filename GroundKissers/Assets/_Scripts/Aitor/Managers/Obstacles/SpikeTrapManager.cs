using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrapManager : MonoBehaviour
{
    private Animator anim;
    
    [Header("Animacion")]
    [SerializeField] private float minTimeBetweenActivations = 3f;
    [SerializeField] private float maxTimeBetweenActivations = 7f;
    [SerializeField] private float actionDuration = 0.1f;
    [SerializeField] private float cooldownTime = 3f;

    [Header("Valores de aparicion")]
    [SerializeField] private float elevationDistance = 0.5f; // Distancia de elevación
    [SerializeField] private float elevationDuration = 0.5f; // Duración de la elevación
    [SerializeField] private float pauseBeforeAction = 1f; 

    private Vector3 initialPosition;

    void Start()
    {
        anim = GetComponent<Animator>();
        initialPosition = transform.position;

        StartCoroutine(ActivateActionPeriodically());
    }

    private IEnumerator ActivateActionPeriodically()
    {
        while (true)
        {
            // Generar un tiempo aleatorio entre activaciones
            float randomTimeBetweenActivations = Random.Range(minTimeBetweenActivations, maxTimeBetweenActivations);
            
            yield return new WaitForSeconds(randomTimeBetweenActivations);

            yield return StartCoroutine(ElevarObjeto());

            yield return new WaitForSeconds(pauseBeforeAction);

            anim.SetBool("Action", true);

            yield return new WaitForSeconds(actionDuration);

            anim.SetBool("Action", false);

            yield return StartCoroutine(DescenderObjeto());

            // Esperar el tiempo de enfriamiento antes de volver a iniciar el ciclo
            yield return new WaitForSeconds(cooldownTime);
        }
    }

    private IEnumerator ElevarObjeto()
    {
        Vector3 targetPosition = initialPosition + Vector3.up * elevationDistance;
        float elapsedTime = 0f;

        while (elapsedTime < elevationDuration)
        {
            transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / elevationDuration);
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
            transform.position = Vector3.Lerp(startPosition, initialPosition, elapsedTime / elevationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = initialPosition;
    }
}
