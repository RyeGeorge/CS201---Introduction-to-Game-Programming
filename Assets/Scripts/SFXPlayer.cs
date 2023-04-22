using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public AudioClip jump;
    public AudioClip dash;
    public AudioClip enemyHurt;
    public AudioClip playerHurt;
    public AudioClip healthPotionPickup;
    public AudioClip shootGun;

    public void PlayAudioClip(AudioClip clip, AudioSource source)
    {
        source.PlayOneShot(clip);
    }
}
