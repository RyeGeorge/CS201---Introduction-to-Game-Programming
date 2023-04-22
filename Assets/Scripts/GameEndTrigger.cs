using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class GameEndTrigger : MonoBehaviour
{
    private PlayableDirector endCutscene;

    public int numberOfMaps;
    private GameObject player;
    public Transform targetPlayerPositon;
    public float speed;

    private void Start()
    {
        endCutscene = GetComponent<PlayableDirector>();
        player = GameObject.Find("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
           
            if (numberOfMaps == 4)
            {
                GameOver();
            }
        }
    }

    void GameOver()
    {
        DisableComponents();
        StartCoroutine(SetPlayerPosition());
    }


    IEnumerator SetPlayerPosition()
    {
        while ((player.GetComponent<Rigidbody2D>().velocity.y != 0))
        {
            yield return null;
        }

        while (player.transform.position.x < targetPlayerPositon.position.x)
        {
            player.transform.Translate(Vector2.right * speed * Time.deltaTime);
            yield return null;
        }

        TriggerCutscene();
    }

    void TriggerCutscene()
    {
        endCutscene.Play();
    }

    void DisableComponents()
    {
        player.GetComponent<PlayerMovement>().enabled = false;
        player.GetComponentInChildren<PlayerAnimation>().inCutscene = true;
        player.GetComponentInChildren<PlayerAnimation>().enabled = false;
        player.GetComponentInChildren<Shotgun>().enabled = false;
        player.GetComponentInChildren<RotateShotgun>().enabled = false;
        player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, player.GetComponent<Rigidbody2D>().velocity.y);
    }

    public void LoadEndLevel()
    {
        SceneManager.LoadScene(0);
    }

}



