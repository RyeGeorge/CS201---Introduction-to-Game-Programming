using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClamMeleeCheck : MonoBehaviour
{
    private bool hit;

    public bool PlayerHit()
    {
        return hit;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            hit = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            hit = false;
        }
    }
}
