using System.Collections;
using System.Collections.Generic;
using UnityEditor.TextCore.Text;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private DamageFlash damageFlash;
    private PlayerMovement playerMovement;

    [Header("Hit")]
    public float health;
    private float currentHealth;
    public float hitCooldown;
    private float hitTimer;
    public bool playerHit;
    public Vector2 hitKnockback = new Vector2(3, 1.5f);


    [Header("Death")]
    public Vector2 deathKnockback = new Vector2(6, 6);
    public Transform deathGroundCheck;
    public bool playerDead;


    [Header("UI")]
    public Slider healthSlider;

    private void Start()
    {
        damageFlash = GetComponent<DamageFlash>();
        playerMovement = GetComponent<PlayerMovement>();

        healthSlider.maxValue = health;
        currentHealth = health;

        hitTimer = 0;
    }

    private void Update()
    {
        healthSlider.value = currentHealth;

        if (hitTimer > 0)
            hitTimer -= Time.deltaTime;
    }

    public void DecreaseHealth(float decreaseAmount)
    {
        if (hitTimer <= 0)
        {
            currentHealth -= decreaseAmount;
            damageFlash.TriggerFlash();
            playerHit = true;
            StartCoroutine(ApplyHitKnockback());

            if (currentHealth < 0)
            {
                currentHealth = 0;
                KillPlayer();
            }

            hitTimer = hitCooldown;
        }
    }

    public void IncreaseHealth(float increaseAmount)
    {
        currentHealth += increaseAmount;

        if (currentHealth > health)
            currentHealth = health;
    }

    private void KillPlayer()
    {
        PlayerMovement pm = GetComponent<PlayerMovement>();
        Destroy(pm);

        GameObject weapon = GameObject.Find("Weapon Offset");
        Destroy(weapon);

        ApplyDeathKnockback();

        playerDead = true;
    }

    void ApplyDeathKnockback()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero;

        if (playerMovement.isFacingRight)
            rb.velocity = new Vector2(-1 * deathKnockback.x, deathKnockback.y);
        else
            rb.velocity = new Vector2(1 * deathKnockback.x, deathKnockback.y);
    }

    IEnumerator ApplyHitKnockback()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero;

        if (playerMovement.isFacingRight)
            rb.velocity = new Vector2(-1 * hitKnockback.x, hitKnockback.y);
        else
            rb.velocity = new Vector2(1 * hitKnockback.x, hitKnockback.y);

        PlayerMovement pm = GetComponent<PlayerMovement>();
        pm.enabled = false;

        yield return new WaitForSeconds(0.25f);

        pm.enabled = true;
    }

    public bool OnGroundDead()
    {
        if (deathGroundCheck.gameObject.activeSelf)
            return Physics2D.OverlapCircle(deathGroundCheck.position, 0.2f, playerMovement.groundLayer);

        return false;
    }
}
