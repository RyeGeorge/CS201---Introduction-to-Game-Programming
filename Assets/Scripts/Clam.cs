using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clam : MonoBehaviour
{
    Animator anim;
    PlayerHealth playerHealth;
    ClamDeathParticles deathEffect;
    ClamMeleeCheck meleeCheck;

    SFXPlayer sfxPlayer;

    [Header("Health")]
    public float health;
    private float currentHealth;
    public Slider healthbar;
    public float deactivateTime;

    [Header("Ranged Attack")]
    public float attackRange;
    public float attackCooldown;
    public Transform projectileOrigin;
    public GameObject projectile;

    [Header("Melee Attack")]
    public float meleeRange;
    public float meleeCooldown;
    private bool meleeAttacking = false;
    public float damage;

    private GameObject player;

    private bool shooting;
    private bool canShoot = true;
    private bool dead;

    private void Start()
    {
        player = GameObject.Find("Player");
        anim = GetComponent<Animator>();
        playerHealth = player.GetComponent<PlayerHealth>();
        meleeCheck = GetComponentInChildren<ClamMeleeCheck>();
        deathEffect = GetComponent<ClamDeathParticles>();

        currentHealth = health;
        healthbar.maxValue = health;
        healthbar.value = currentHealth;

        sfxPlayer = GameObject.Find("SFXPlayer").GetComponent<SFXPlayer>();
    }

    private void Update()
    {
        healthbar.value = currentHealth;

        if (meleeCheck.PlayerHit())
        {
            playerHealth.DecreaseHealth(damage);
        }

        if (Vector2.Distance(this.transform.position, player.transform.position) < attackRange && shooting == false && !playerHealth.playerDead && transform.position.x > player.transform.position.x)
        {
            if (canShoot && Vector2.Distance(this.transform.position, player.transform.position) > meleeRange)
            {
                shooting = true;
                meleeAttacking = false;
                StopCoroutine(MeleeAttack());
                StartCoroutine(ShootProjectile());
            }
        }
        else if (Vector2.Distance(this.transform.position, player.transform.position) > attackRange || transform.position.x < player.transform.position.x || playerHealth.playerDead)
        {
            shooting = false;
            StopCoroutine(ShootProjectile());
        }

        if (Vector2.Distance(this.transform.position, player.transform.position) < meleeRange && playerHealth.playerDead == false)
        {
            if (!meleeAttacking)
            {
                meleeAttacking = true;
                shooting = false;
                StartCoroutine(MeleeAttack());
                StopCoroutine(ShootProjectile());
                Debug.Log("2");
            }
        }
        else
        {
            StopCoroutine(MeleeAttack());
            meleeAttacking = false;
        }

    }

    IEnumerator MeleeAttack()
    {
        while (meleeAttacking)
        {
            anim.SetTrigger("Melee Attack");
            yield return new WaitForSeconds(meleeCooldown);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            PlayerBulletProjectile playerBulletProjectile = collision.gameObject.GetComponent<PlayerBulletProjectile>();
            if (playerBulletProjectile != null)
            {
                TakeDamage(playerBulletProjectile.damage);
            }
        }
    }

    void TakeDamage(float damage)
    {
        currentHealth -= damage;
        anim.SetTrigger("Hit");
        ActivateHealthBar();
        sfxPlayer.PlayAudioClip(sfxPlayer.enemyHurt, sfxPlayer.audioSource);

        if (currentHealth <=0) 
        {
            currentHealth = 0;
            KillClam();
        }
    }

    void KillClam()
    {
        if (!dead)
        {
            dead = true;
            canShoot = false;

            deathEffect.StartDeathEffect();
            Destroy(gameObject);
            Invoke("DeactivateHealthBar", 0.35f);
        }
    }

    void ActivateHealthBar()
    {
        healthbar.gameObject.SetActive(true);

        CancelInvoke();
        Invoke("DeactivateHealthBar", deactivateTime);
    }

    void DeactivateHealthBar()
    {
        healthbar.gameObject.SetActive(false);
    }
}
