using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingAnim : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.5f;
    [SerializeField] private float frequency = 1f;

    private Vector3 startPosition;
    private float timeOffset;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;

        //offset de tiempo aleatorio para variar el movimiento entre objetos
        timeOffset = Random.value * Mathf.PI * 2;
    }

    // Update is called once per frame
    void Update()
    {
        //desplazamiento vertical usando la función seno
        float yOffset = amplitude * Mathf.Sin((Time.time + timeOffset) * frequency);
        
        transform.position = new Vector3(startPosition.x, startPosition.y + yOffset, startPosition.z);
    }
}
