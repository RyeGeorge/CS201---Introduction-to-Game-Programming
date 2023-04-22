using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    public float regenAmount;
    private GameObject player;
    private PlayerHealth playerHealth;

    private SFXPlayer sfxPlayer;

    private void Start()
    {
        player = GameObject.Find("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        sfxPlayer = GameObject.Find("SFXPlayer").GetComponent<SFXPlayer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (playerHealth.currentHealth < playerHealth.health)
            {
                sfxPlayer.PlayAudioClip(sfxPlayer.healthPotionPickup, sfxPlayer.audioSource);
                playerHealth.IncreaseHealth(regenAmount);
                Destroy(gameObject);
            }
        }
    }
}
