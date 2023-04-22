using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    private GameObject player;
    private Animator anim;

    private SFXPlayer sfxPlayer;
    private AudioSource audioSource;

    [Header("Movement")]
    public float speed;
    public float moveRange;
    private float startPos;
    private bool movingRight = true;
    private bool facingRight = false;

    [Header("Attack")]
    public float damage;
    public float attackRange;
    public float attackDelay;
    private bool attacking;

    [Header("Health")]
    public float health;
    private float currentHealth; 
    public Vector2 deathKnockback;
    private bool dead = false;


    [Header("Healthbar")]
    public Vector2 healthbarOffset;
    public Slider healthbar;
    public float deactivateTime;
    private bool healthBarActive;

    private bool attackRunning, moveRightRunning, moveLeftRunning;

    private void Start()
    {
        player = GameObject.Find("Player");
        anim = GetComponentInChildren<Animator>();

        startPos = transform.position.x;
        StartCoroutine(MoveRight());

        currentHealth = health;
        healthbar.gameObject.SetActive(false);
        healthbar.maxValue = health;
        healthbar.value = health;

        sfxPlayer = GameObject.Find("SFXPlayer").GetComponent<SFXPlayer>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Update()
    {
        healthbar.value = currentHealth;

        if (!dead)
        {
           
            if (Vector2.Distance(this.transform.position, player.transform.position) <= attackRange && !attackRunning)
            {
                StopAllCoroutines();
                attackRunning = false;
                moveRightRunning = false;
                moveLeftRunning = false;
                attacking = true;

                StartCoroutine(Attack());

            }
            else if (Vector2.Distance(this.transform.position, player.transform.position) > attackRange && attackRunning)
            {
                StopCoroutine(Attack());
                attacking = false;

                if (movingRight && moveRightRunning == false)
                    StartCoroutine(MoveRight());
                else if (!movingRight && moveLeftRunning == false)
                    StartCoroutine(MoveLeft());
            }


            if (movingRight && !facingRight)
                Flip();

            else if (!movingRight && facingRight)
                Flip();

            if (attacking)
            {
                if (facingRight && transform.position.x > player.transform.position.x)
                    Flip();

                else if (!facingRight && transform.position.x < player.transform.position.x)
                    Flip();
            }

            if (moveRightRunning || moveLeftRunning)
                anim.SetBool("Moving", true);
            else
                anim.SetBool("Moving", false);
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;

        Vector3 healthbarLocalScale = healthbar.transform.localScale;
        healthbarLocalScale.x *= -1;
        healthbar.transform.localScale = healthbarLocalScale;
    }

    IEnumerator Attack()
    {
        attackRunning = true;
        while (attacking)
        {
            yield return new WaitForEndOfFrame();

            anim.SetBool("Moving", false);
            anim.SetTrigger("Attack");
            yield return new WaitForSeconds(attackDelay);
        }
        attackRunning = false;
    }

    IEnumerator MoveRight()
    {
        moveRightRunning = true;
        if (movingRight)
        {
            while (movingRight && transform.position.x < startPos + moveRange)
            {
                transform.Translate(Vector2.right * speed * Time.deltaTime);
                yield return null;
            }
            movingRight = false;
        }
        moveRightRunning = false;
        StartCoroutine(MoveLeft());
    }

    IEnumerator MoveLeft()
    {
        moveLeftRunning = true;
        if (!movingRight)
        {
            while (!movingRight && transform.position.x > startPos + -moveRange)
            {
                transform.Translate(Vector2.left * speed * Time.deltaTime);
                yield return null;
            }
            movingRight = true;
        }

        moveLeftRunning = false;
        StartCoroutine(MoveRight());
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

        if (collision.gameObject.CompareTag("Player"))
        {
            DamagePlayer(collision.gameObject.GetComponentInParent<PlayerHealth>());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (dead && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            anim.SetTrigger("Dead Landed");
            Destroy(GetComponent<Collider2D>());
            Destroy(GetComponent<Rigidbody2D>());
        }

    }

    void DamagePlayer(PlayerHealth playerHealth)
    {
        playerHealth.DecreaseHealth(damage);
    }

    void TakeDamage(float damage)
    {
        if (!dead)
        {
            if (!healthBarActive)
            {
                ActivateHealthBar();
            }

            currentHealth -= damage;
            anim.SetTrigger("Hit");
            sfxPlayer.PlayAudioClip(sfxPlayer.enemyHurt, audioSource);

            if (currentHealth <= 0)
            {
                currentHealth = 0;

                KillEnemy();
            }
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



    void KillEnemy()
    {
        dead = true;
        anim.SetTrigger("Dead");
        StopAllCoroutines();
        ApplyDeathKnockback();

        Invoke("DeactivateHealthBar", 0.35f);
    }

    void ApplyDeathKnockback()
    {
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero;

        if (facingRight)
            rb.velocity = new Vector2(-1 * deathKnockback.x, deathKnockback.y);
        else
            rb.velocity = new Vector2(1 * deathKnockback.x, deathKnockback.y);
    }
}
