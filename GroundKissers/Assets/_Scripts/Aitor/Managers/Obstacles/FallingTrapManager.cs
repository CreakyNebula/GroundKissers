using UnityEngine;
using System.Collections;

public class FallingTrapManager : MonoBehaviour
{
    [SerializeField] private float fallDelay = 0.5f;
    [SerializeField] private float fallDuration = 1f;
    [SerializeField] private float destroyDelay = 1f;
    [SerializeField] private float swingAngle = 15f;
    [SerializeField] private float swingDuration = 0.5f;

    private void Start()
    {
        // Inicia la secuencia de caída cuando el objeto se crea
        StartCoroutine(FallSequence());
    }

    private IEnumerator FallSequence()
    {
        // Espera un momento antes de iniciar la animación
        yield return new WaitForSeconds(fallDelay);

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
