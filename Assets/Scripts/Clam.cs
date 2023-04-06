using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clam : MonoBehaviour
{
    Animator anim;
    PlayerHealth playerHealth;

    public float attackRange;
    public float attackCooldown;
    public Transform projectileOrigin;
    public GameObject projectile;

    private GameObject player;

    private bool shooting;

    private void Start()
    {
        player = GameObject.Find("Player");
        anim = GetComponent<Animator>();
        playerHealth = player.GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (Vector2.Distance(this.transform.position, player.transform.position) < attackRange && shooting == false && !playerHealth.playerDead && transform.position.x > player.transform.position.x)
        {
            shooting = true;
            StartCoroutine(ShootProjectile());
        }
        else if (Vector2.Distance(this.transform.position, player.transform.position) > attackRange || transform.position.x < player.transform.position.x || playerHealth.playerDead)
        {
            shooting = false;
            StopCoroutine(ShootProjectile());
        }
            
    }

    IEnumerator ShootProjectile()
    {
        while (shooting)
        {
            anim.SetTrigger("Shoot");
            yield return new WaitForSeconds(attackCooldown);
        }
    }

    public void SpawnProjectile()
    {
        Instantiate(projectile, projectileOrigin);
    }
}
