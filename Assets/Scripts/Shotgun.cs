using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    private Transform player;
    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private Vector3 shotgunDir;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        playerMovement = player.GetComponent<PlayerMovement>();
        rb = player.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (playerMovement.isFacingRight)
            shotgunDir = transform.rotation * Vector3.right;
        else
            shotgunDir = transform.rotation * -Vector3.right;


        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }


    private void Shoot()
    {
        rb.AddForce(-shotgunDir * 10, ForceMode2D.Impulse);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, shotgunDir);
    }
}
