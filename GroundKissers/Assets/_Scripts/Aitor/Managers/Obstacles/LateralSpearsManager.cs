using System.Collections;
using UnityEngine;

public class LateralSpearsManager : MonoBehaviour
{
    private Animator anim;
    
    [SerializeField] private float minTimeBetweenActivations = 3f;
    [SerializeField] private float maxTimeBetweenActivations = 7f;
    [SerializeField] private float actionDuration = 0.1f;
    [SerializeField] private float cooldownTime = 3f;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        StartCoroutine(ActivateActionPeriodically());
    }

    private IEnumerator ActivateActionPeriodically()
    {
        while (true)
        {
            // Generar un tiempo aleatorio entre activaciones
            float randomTimeBetweenActivations = Random.Range(minTimeBetweenActivations, maxTimeBetweenActivations);
            
            yield return new WaitForSeconds(randomTimeBetweenActivations);

            anim.SetBool("Action", true);

            yield return new WaitForSeconds(actionDuration);

            anim.SetBool("Action", false);

            yield return new WaitForSeconds(cooldownTime);
        }
    }
}
