using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSettings : MonoBehaviour
{
    private float volume = 1;
    private bool musicEnabled = true;

    public void SetVolume(float volume)
    {
        this.volume = volume;
    }

    public void SetMusicToggle(bool toggle)
    {
        musicEnabled = toggle;
    }

    public bool GetMusicToggle() 
    {  
        return musicEnabled;
    }

    public float GetVolume()
    {
        return volume;
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        Debug.Log("Volume: " + volume);
        Debug.Log("Music Toggled: " + musicEnabled);
    }
}
