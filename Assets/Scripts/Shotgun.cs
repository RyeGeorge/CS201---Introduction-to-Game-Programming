using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    private Transform player;
    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private Vector3 shotgunDir;

    [Header("Shooting")]
    public float shootCooldown;
    private float shotgunTimer;
    public GameObject projectile;
    public Transform projectileOrigin;

    [Header("Shotgun Launch")]
    public float launchForce;
    public float airLaunchForce;
    public bool shotgunLaunchJumping;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        playerMovement = player.GetComponent<PlayerMovement>();
        rb = player.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (playerMovement.isFacingRight)
            shotgunDir = transform.right;
        else
            shotgunDir = -transform.right;

        if (shotgunTimer >= shootCooldown)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
                shotgunTimer = 0;

                if (!playerMovement.OnGround())
                {
                    ShotGunLaunch();
                }
            }
        }
        else
            shotgunTimer += Time.deltaTime;
        
    }


    private void Shoot()
    {
        Instantiate(projectile, projectileOrigin);
    }

    private void ShotGunLaunch()
    {
        if (shotgunLaunchJumping)
        {
            rb.velocity = new Vector2(-shotgunDir.x * airLaunchForce, -shotgunDir.y * airLaunchForce);
        }
        else
        {
            shotgunLaunchJumping = true;
            rb.velocity = new Vector2(-shotgunDir.x * launchForce, -shotgunDir.y * launchForce);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, shotgunDir);
    }
}
