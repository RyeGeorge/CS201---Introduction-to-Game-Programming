using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    public float attackRange;
    public float attackDelay;
    private bool attacking;

    [Header("Health")]
    public float health;
    private float currentHealth;


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
    }

    void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            KillEnemy();
        }
    }

    void KillEnemy()
    {
        Debug.Log("Dead");
    }
}
