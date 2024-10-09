using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionsManager : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Danger"))
        {
            Debug.Log("DAMAGE");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("FallingPlatform"))
        {
            transform.parent = collision.gameObject.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("FallingPlatform"))
        {
            transform.parent = null;
        }
    }
}
