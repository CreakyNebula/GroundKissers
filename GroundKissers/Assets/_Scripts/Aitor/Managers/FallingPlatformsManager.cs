using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatformsManager : MonoBehaviour
{
    [SerializeField] private bool isPlayerOnTop = false;

    private void OnEnable()
    {
        if (isPlayerOnTop)
        {
            print("AAAAAAAAAA");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnTop = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOnTop = false;
        }
    }
}
