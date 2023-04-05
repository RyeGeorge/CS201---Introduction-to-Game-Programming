using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    public float damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            Debug.Log("PLAYER DETECTED");

            if (playerHealth != null)
            {
                Debug.Log("OUCH!");
                playerHealth.DecreaseHealth(damage);
            }
        }
    }
}
