using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    private GameObject player;
    private Animator anim;

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
    public Canvas canvas;
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
    }

    private void Update()
    {
        if (!dead)
        {
            if (Vector2.Distance(this.transform.position, player.transform.position) <= attackRange && !attackRunning)
            {
                StopAllCoroutines();
                attacking = true;

                StartCoroutine(Attack());

            }
            else if (Vector2.Distance(this.transform.position, player.transform.position) > attackRange && attackRunning)
            {
                attacking = false;
                attackRunning = false;
                moveRightRunning = false;
                moveLeftRunning = false;
                StopCoroutine(Attack());

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
    }

    IEnumerator Attack()
    {
        attackRunning = true;
        while (attacking)
        {
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

            if (currentHealth <= 0)
            {
                currentHealth = 0;

                KillEnemy();
            }
        }
    }

    void ActivateHealthBar()
    {
        healthbar.enabled = true;
        healthbar.transform.SetParent(canvas.transform);

        CancelInvoke();
        Invoke("DeactivateHealthBar", deactivateTime);
    }

    void DeactivateHealthBar()
    {
        healthbar.enabled = false;
    }

    void KillEnemy()
    {
        dead = true;
        anim.SetTrigger("Dead");
        StopAllCoroutines();
        ApplyDeathKnockback();
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