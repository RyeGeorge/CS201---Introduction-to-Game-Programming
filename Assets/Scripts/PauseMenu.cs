using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject menu;
    public GameObject settingsMenu;
    private bool menuOpen;

    private GameObject player;
    private PlayerAnimation playerAnimation;
    private PlayerMovement playerMovement;
    private RotateShotgun rotateShotgun;
    private Shotgun shotgun;

    private void Start()
    {
        player = GameObject.Find("Player");
        playerAnimation = player.GetComponentInChildren<PlayerAnimation>();
        playerMovement = player.GetComponent<PlayerMovement>();
        rotateShotgun = player.GetComponentInChildren<RotateShotgun>();
        shotgun = player.GetComponentInChildren<Shotgun>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !menuOpen)
        {
            OpenMenu();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && menuOpen)
        {
            CloseMenu();
        }
    }

    void OpenMenu()
    {
        Debug.Log("OPEN");
        menuOpen = !menuOpen;
        menu.SetActive(true);
        Time.timeScale = 0;

        ToggleComponents();
    }

    public void CloseMenu()
    {
        Debug.Log("CLOSE");
        menuOpen = !menuOpen;
        menu.SetActive(false);
        Time.timeScale = 1;

        if (settingsMenu.activeInHierarchy)
        {
            settingsMenu.SetActive(false);
        }

        ToggleComponents();
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene(0);
    }

    void ToggleComponents()
    {
        playerAnimation.enabled = !playerAnimation.enabled;
        playerMovement.enabled = !playerMovement.enabled;
        rotateShotgun.enabled = !rotateShotgun.enabled;
        shotgun.enabled = !shotgun.enabled;
    }
}
