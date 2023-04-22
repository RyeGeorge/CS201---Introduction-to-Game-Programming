using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PickupMap : MonoBehaviour
{
    private Transform player;

    //public static int numberOfMaps;
    public int maps;
    public TextMeshProUGUI mapCounter;
    public Animator textAnim;

    private GameEndTrigger gameEndTrigger;

    public float pickupRange;
    public GameObject pickupKey;
    private bool canPickUp = false;
    private bool pickedUp = false;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        textAnim = mapCounter.GetComponent<Animator>();
        gameEndTrigger = GameObject.Find("Game End Trigger").GetComponent<GameEndTrigger>();
    }

    private void Update()
    {
        //maps = numberOfMaps;

        if (Vector2.Distance(transform.position, player.position) < pickupRange)
        {
            canPickUp = true;
        }
        else
        {
            canPickUp = false;
        }


        if (canPickUp && pickedUp == false)
        {
            pickupKey.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
                OnMapPickup();
        }
        else
        {
            pickupKey.SetActive(false);
        }
    }

    void OnMapPickup()
    {
        //numberOfMaps++;
        gameEndTrigger.numberOfMaps++;
        mapCounter.text = "Torn Maps Collected " + gameEndTrigger.numberOfMaps + "/4";
        textAnim.SetTrigger("Appear Counter");
        Destroy(this.gameObject);
        
    }
}
