using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PickupMap : MonoBehaviour
{
    private Transform player;

    public static int numberOfMaps;
    public TextMeshProUGUI mapCounter;
    private Animator textAnim;

    public float pickupRange;
    public GameObject pickupKey;
    private bool canPickUp = false;
    private bool pickedUp = false;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        textAnim = mapCounter.GetComponent<Animator>();
    }

    private void Update()
    {
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
        numberOfMaps++;     
        mapCounter.text = "Torn Maps Collected " + numberOfMaps + "/4";
        textAnim.SetTrigger("Appear Counter");
        Destroy(this.gameObject);
        
    }
}
