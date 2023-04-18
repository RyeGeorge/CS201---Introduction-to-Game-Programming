using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables

    [Header("Movement")]
    public float moveSpeed;
    public float airSpeed;
    private float desiredSpeed;
    public bool isFacingRight = true;

    //Movement lerp
    private float elapsedTime;
    public float accelTime;
    public float decelTime;
    public AnimationCurve accelRate;
    private bool accelStartSet = false;
    private bool decelStartSet = false;
    private float startSpeed;

    [Header("Jump")]
    public float jumpForce;
    private float startGrav;
    public float maxGravScale;
    public float gravIncreaseSpeed;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Wall Slide")]
    public float slideSpeed;
    public float maxSlideSpeed;
    public float slideAccelRate;
    private float startSlideSpeed;
    public Transform frontWallCheck;
    public Transform backWallCheck;
    public bool wallSliding;
    private bool wallSlideStarted = false;
    

    [Header("Wall Jump")]
    public Vector2 wallJumpForce;
    private Vector2 startWallJumpForce;
    public bool wallJumping;
    private float wallJumpDirection;
    private float wallJumpTime = 0.2f;
    private float wallJumpCounter;
    private float wallJumpDuration = 0.4f;

    [Header("Dash")]
    public float dashSpeed;
    public float dashTime;
    public float dashCooldown;  
    private float dashCooldownTime;
    private bool canDash;
    public bool dashing;
    public TrailRenderer trailRenderer;

    private float horizontalInput;

    [Header("Shotgun Launch")]
    public Shotgun shotgun;


    private Rigidbody2D rb;


    #endregion


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        startGrav = rb.gravityScale;
        startSlideSpeed = slideSpeed;
        startWallJumpForce = wallJumpForce;
    }

    #region Update / Fixed Update

    private void FixedUpdate()
    {
        if (!wallJumping && !shotgun.shotgunLaunchJumping && !dashing)
            CalculateAcceleration();

        if (wallSliding)
            WallSlide();
    }

    private void Update()
    {
        if (dashing)
            return;

        Jump();
        WallJump();

        if (Input.GetButtonDown("Dash") && dashing == false && dashCooldownTime <= 0)
        {
            StartCoroutine(Dash());
            dashCooldownTime = dashCooldown;
        }
        else if (dashCooldownTime >= 0)
        {
            dashCooldownTime -= Time.deltaTime;
        }

        if (OnGround() && wallJumping)
            wallJumping = false;

        if (rb.velocity.y < 0 && !OnWall() && !dashing)
        {
            UpdateGravity();
        }
            
            
        if (rb.velocity.y >= 0 && !OnWall() && !dashing)
        {
            rb.gravityScale = startGrav;
        }
            

        if (!wallJumping)
            //Flip();


        if (rb.velocity.y < 0 && OnWall() && !wallSliding)
            StartSlide();
            

        if (OnGround())
            StopSlide();

        if (wallSliding && !OnWall())
            StopSlide();


        // Get input
        horizontalInput = Input.GetAxis("Horizontal");

       if (shotgun.shotgunLaunchJumping)
        {
            Vector3 pos = transform.position;
            pos.x += horizontalInput * airSpeed * Time.deltaTime;
            transform.position = pos;
        }
        else
        {
            // Set desired speed
            if (horizontalInput == 0 && rb.velocity.y == 0)
                desiredSpeed = 0;
            else
                desiredSpeed = moveSpeed;
        }

        

        if (shotgun.shotgunLaunchJumping && rb.velocity.y < 0 && rb.velocity.x == 0)
            shotgun.shotgunLaunchJumping = false;
        

    }

    #endregion

    #region Jump

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && OnGround())
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        else if (Input.GetButtonUp("Jump") && OnGround())
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
    }


    void UpdateGravity()
    {
        float newGrav = Mathf.MoveTowards(rb.gravityScale, maxGravScale, gravIncreaseSpeed * Time.deltaTime);
        rb.gravityScale = newGrav;
    }

    #endregion

    #region Movement

    private void CalculateAcceleration()
    {
        // Accelerate
        if (((rb.velocity.x < desiredSpeed && rb.velocity.x >= 0) || (rb.velocity.x > -desiredSpeed && rb.velocity.x <= 0)) && wallSliding == false)
        {
            if (!accelStartSet)
            {
                startSpeed = rb.velocity.x;
                accelStartSet = true;
                decelStartSet = false;
                elapsedTime = 0;
            }


            float targetSpeed = rb.velocity.x;

            elapsedTime += Time.deltaTime;
            float lerp = elapsedTime / accelTime;

            targetSpeed = Mathf.Lerp(startSpeed, desiredSpeed, accelRate.Evaluate(lerp));
            rb.velocity = new Vector2(horizontalInput * targetSpeed, rb.velocity.y);
        }
        // Decelerate
        else if (((rb.velocity.x > desiredSpeed && rb.velocity.x > 0) || (rb.velocity.x < -desiredSpeed && rb.velocity.x < 0)) && wallSliding == false)
        {
            if (!decelStartSet)
            {
                startSpeed = rb.velocity.x;
                accelStartSet = false;
                decelStartSet = true;
                elapsedTime = 0;
            }


            float targetSpeed = rb.velocity.x;

            elapsedTime += Time.deltaTime;
            float lerp = elapsedTime / decelTime;

            targetSpeed = Mathf.Lerp(startSpeed, desiredSpeed, accelRate.Evaluate(lerp));

            rb.velocity = new Vector2(horizontalInput * targetSpeed, rb.velocity.y);
        }
        else
        {
            elapsedTime = 0;
            accelStartSet = false;
            decelStartSet = false;
        }
    }

    #endregion

    #region Wall Slide

    private void StartSlide()
    {
        wallSliding = true;
        rb.gravityScale = 0f;
    }

    private void StopSlide()
    {
        wallSliding = false;
        rb.gravityScale = startGrav;
        slideSpeed = startSlideSpeed;
    }

    private void WallSlide()
    {
        if (slideSpeed < maxSlideSpeed)
        {
            slideSpeed = Mathf.MoveTowards(slideSpeed, maxSlideSpeed, slideAccelRate);
        }

        rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);    
    }

    #endregion

    #region Wall Jump

    private void WallJump()
    {
        if (wallSliding)
        {
            wallJumpForce = startWallJumpForce;
            wallJumping = false;
            if (Physics2D.OverlapCircle(frontWallCheck.position, 0.2f, groundLayer))
            {
                wallJumpDirection = -transform.localScale.x;
            }
            else if (Physics2D.OverlapCircle(backWallCheck.position, 0.2f, groundLayer))
            {
                wallJumpDirection = transform.localScale.x;
            }
            
            if (wallJumpDirection < 0) 
            {
                wallJumpForce.y *= -1;
            }

            wallJumpCounter = wallJumpTime;

        }
        else
        {
            wallJumpCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpCounter > 0f)
        {
            wallSliding = false;
            wallJumping = true;
            rb.velocity = wallJumpDirection * wallJumpForce;
            wallJumpCounter = 0;

        }
    }

    private void StopWallJumping()
    {
        wallJumping = false;
    }

    #endregion

    #region Dash

    private IEnumerator Dash()
    {
        canDash = false;
        dashing = true;
        rb.gravityScale = 0;

        trailRenderer.emitting = true;
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, rb.velocity.y);
        Debug.Log(transform.localScale.x);

        yield return new WaitForSeconds(dashTime);

        trailRenderer.emitting = false;
        rb.gravityScale = startGrav;
        dashing = false;
    }

    #endregion

    #region Collision

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") && shotgun.shotgunLaunchJumping)
            shotgun.shotgunLaunchJumping = false;
    }

    public bool OnGround()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    public bool OnWall()
    {
        return Physics2D.OverlapCircle(frontWallCheck.position, 0.2f, groundLayer) || Physics2D.OverlapCircle(backWallCheck.position, 0.2f, groundLayer);
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        // Ground Check
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, 0.2f);

        // Wall Check
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(frontWallCheck.position, 0.2f);
    }
}


