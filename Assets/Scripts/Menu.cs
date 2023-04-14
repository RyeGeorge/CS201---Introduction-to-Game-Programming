using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Slider volumeSlider;

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void AdjustVolume()
    {
        AudioSource[] audioSources = GameObject.FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.volume = volumeSlider.value;
        }
    }

    private void Update()
    {
        AdjustVolume();
    }

    public void ToggleMusic()
    {
        AudioSource[] musicSources = GameObject.FindObjectsOfType<AudioSource>();

        foreach (AudioSource source in musicSources)
        {
            if (source.CompareTag("Music Player"))
            {
                source.enabled = !source.enabled;
            }
        }
    }
}
