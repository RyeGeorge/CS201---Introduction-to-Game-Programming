using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;

    public bool inCutscene;
    private bool dead, deadLanded;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponentInParent<Rigidbody2D>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        playerHealth = GetComponentInParent<PlayerHealth>();

        inCutscene = false;
    }

    private void Update()
    {
       
        if (inCutscene)
        {
            anim.SetBool("InCutscene", true);
        }
        else
        {
            anim.SetBool("InCutscene", false);
        }

        if (!inCutscene)
        {
            if (playerHealth.playerDead == false)
            {
                if (rb.velocity.x < 0.01 && rb.velocity.x > -0.01)
                    anim.SetBool("Running", false);
                else
                    anim.SetBool("Running", true);

                if (rb.velocity.y < -0.1)
                    anim.SetBool("Falling", true);
                else
                    anim.SetBool("Falling", false);

                if (rb.velocity.y < 0.15 && playerMovement.OnGround())
                    anim.SetTrigger("Landed");

                if (Input.GetButtonDown("Jump") && playerMovement.OnGround())
                    anim.SetTrigger("Jumped");

                anim.SetBool("Wall Sliding", playerMovement.wallSliding);

                if (playerHealth.playerHit)
                {
                    anim.SetTrigger("Hit");
                    playerHealth.playerHit = false;
                }


            }
            else if (playerHealth.playerDead == true)
            {
                if (!dead)
                {
                    anim.SetBool("Falling", false);

                    anim.SetTrigger("Player Dead");
                    dead = true;
                }


                if (playerHealth.playerDead && playerHealth.OnGroundDead() && deadLanded == false)
                {
                    anim.SetTrigger("Dead Landed");
                    deadLanded = true;
                }
            }
        }

    }
}
