using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PearlProjectile : MonoBehaviour
{
    public float speed;
    public float damage;

    private bool canTravel = true;

    private PlayerHealth playerHealth;
    private Animator anim;

    private void Start()
    {
        playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (canTravel)
            transform.Translate(-Vector3.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerHealth.DecreaseHealth(damage);
        }

        canTravel = false;
        anim.SetTrigger("Break");
    }

    public void DestroyProjectile()
    {
        Destroy(gameObject);
    }

}
