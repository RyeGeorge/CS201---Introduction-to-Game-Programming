using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerMovement playerMovement;

    private bool jumping;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponentInParent<Rigidbody2D>();
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    private void Update()
    {
        if (rb.velocity.x != 0)
            anim.SetBool("Running", true);
        else if (rb.velocity.x == 0)
            anim.SetBool("Running", false);

        if (rb.velocity.y < 0)
            anim.SetBool("Falling", true);
        else
            anim.SetBool("Falling", false);

        if (rb.velocity.y < 0 && playerMovement.OnGround())
            anim.SetTrigger("Landed");

        if (Input.GetButtonDown("Jump") && playerMovement.OnGround())
            anim.SetTrigger("Jumped");



    }
}
