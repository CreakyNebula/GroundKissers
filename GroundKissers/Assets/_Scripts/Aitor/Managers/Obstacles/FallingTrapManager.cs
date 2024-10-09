using UnityEngine;
using System.Collections;

public class FallingTrapManager : MonoBehaviour
{
    [SerializeField] private float initialDelay = 0.5f;
    [SerializeField] private float tensionDropDistance = 0.2f;
    [SerializeField] private float tensionDuration = 0.3f;
    [SerializeField] private float reboundDuration = 0.2f;
    [SerializeField] private float finalDropDistance = 0.1f; // Nueva variable para la posición final después del rebote
    [SerializeField] private float delayBetweenAnimations = 1f; // Nuevo delay entre animaciones
    [SerializeField] private float swingAngle = 15f;
    [SerializeField] private float swingDuration = 0.5f;
    [SerializeField] private float fallDuration = 1f;
    [SerializeField] private float destroyDelay = 1f;

    private void Start()
    {
        // Inicia la secuencia de caída cuando el objeto se crea
        StartCoroutine(FallSequence());
    }

    private IEnumerator FallSequence()
    {
        Vector3 initialPosition = transform.position;
        Vector3 finalPosition = initialPosition - new Vector3(0, finalDropDistance, 0);

        // Espera inicial
        yield return new WaitForSeconds(initialDelay);

        // Animación de tensión (caída ligera con rebote)
        LeanTween.moveY(gameObject, initialPosition.y - tensionDropDistance, tensionDuration)
            .setEaseInQuad()
            .setOnComplete(() => {
                // Rebote a la posición final
                LeanTween.moveY(gameObject, finalPosition.y, reboundDuration)
                    .setEaseOutQuad();
            });

        // Espera a que termine la animación de tensión
        yield return new WaitForSeconds(tensionDuration + reboundDuration);

        // Nuevo delay entre animaciones
        yield return new WaitForSeconds(delayBetweenAnimations);

        // Animación de balanceo antes de caer
        LeanTween.rotateZ(gameObject, swingAngle, swingDuration)
            .setEasePunch()
            .setOnComplete(() => {
                // Inicia la caída después del balanceo
                LeanTween.moveY(gameObject, transform.position.y - 10f, fallDuration)
                    .setEaseInQuad();
            });

        // Espera a que termine la animación antes de destruir el objeto
        yield return new WaitForSeconds(swingDuration + fallDuration + destroyDelay);

        // Destruye el objeto después de la animación
        Destroy(gameObject);
    }
}
