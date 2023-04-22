using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Slider volumeSlider;
    public Toggle musicToggle;
    private MenuSettings menuSettings;

    private void Start()
    {
        menuSettings = GameObject.Find("Menu Settings").GetComponent<MenuSettings>();

        AudioSource[] musicSources = GameObject.FindObjectsOfType<AudioSource>();

        foreach (AudioSource source in musicSources)
        {
            if (source.CompareTag("Music Player"))
            {
                source.enabled = menuSettings.GetMusicToggle();
            }
        }

        AudioSource[] audioSources = GameObject.FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.volume = menuSettings.GetVolume();
            volumeSlider.value = audioSource.volume;
        }
    }

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
            menuSettings.SetVolume(audioSource.volume);
        }
    }

    private void Update()
    {
        AdjustVolume();

        if (menuSettings.GetMusicToggle() && musicToggle.isOn == false)
        {
            musicToggle.isOn = true;
        }
        else if (menuSettings.GetMusicToggle() == false && musicToggle.isOn == true)
        {
            musicToggle.isOn = false;
        }
    }

    public void ToggleMusic()
    {
        AudioSource[] musicSources = GameObject.FindObjectsOfType<AudioSource>();

        foreach (AudioSource source in musicSources)
        {
            if (source.CompareTag("Music Player"))
            {
                source.enabled = !source.enabled;
                menuSettings.SetMusicToggle(source.enabled);
            }
        }
    }

    
}
