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
    private float desiredSpeed;
    private bool isFacingRight = true;

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
    public Transform wallCheck;
    public bool wallSliding;
    private bool wallSlideStarted = false;
    

    [Header("Wall Jump")]
    public Vector2 wallJumpForce;
    public bool wallJumping;
    private float wallJumpDirection;
    private float wallJumpTime = 0.2f;
    private float wallJumpCounter;
    private float wallJumpDuration = 0.4f;

    [Header("Dash")]
    public float dashSpeed;
    public float dashTime;
    public float dashCooldown;  
    private bool canDash;
    private bool dashing;

    private float horizontalInput;
    

    private Rigidbody2D rb;


    #endregion


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        startGrav = rb.gravityScale;
        startSlideSpeed = slideSpeed;
    }

    #region Update / Fixed Update

    private void FixedUpdate()
    {
        if (!wallJumping)
            CalculateAcceleration();

        if (wallSliding)
            WallSlide();
    }

    private void Update()
    {
        Jump();
        WallJump();

        if (Input.GetButtonDown("Dash") && dashing == false)
        {
            StartCoroutine(Dash());
        }

        if (OnGround() && wallJumping)
            wallJumping = false;

        if (rb.velocity.y < 0 && !OnWall())
        {
            UpdateGravity();
        }
            
            
        if (rb.velocity.y >= 0 && !OnWall())
        {
            rb.gravityScale = startGrav;
        }
            

        if (!wallJumping)
            Flip();


        if (rb.velocity.y != 0 && OnWall() && !wallSliding)
            StartSlide();
            

        if (OnGround())
            StopSlide();

        if (wallSliding && !OnWall())
            StopSlide();


        // Get input
        horizontalInput = Input.GetAxis("Horizontal");

        // Set desired speed
        if (horizontalInput == 0)
            desiredSpeed = 0;
        else
            desiredSpeed = moveSpeed;

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

    public bool OnGround()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    #endregion

    #region Movement

    private void CalculateAcceleration()
    {
        // Accelerate
        if (rb.velocity.x < desiredSpeed && wallSliding == false)
        {
            Debug.Log("Accelerate");
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
        else if (rb.velocity.x > desiredSpeed && wallSliding == false)
        {
            Debug.Log("Decelerate");
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

    private void Flip()
    {
        if (isFacingRight && rb.velocity.x < 0f || !isFacingRight && rb.velocity.x > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
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

    public bool OnWall()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, groundLayer);
    }


    #endregion

    #region Wall Jump

    private void WallJump()
    {
        if (wallSliding)
        {
            wallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpCounter = wallJumpTime;

            //CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpCounter > 0f)
        {
            wallJumping = true;
            rb.velocity = new Vector2(wallJumpDirection * wallJumpForce.x, wallJumpForce.y);
            wallJumpCounter = 0;

            if (transform.localScale.x != wallJumpDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            //Invoke(nameof(StopWallJumping), wallJumpDuration);
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

        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, rb.velocity.y);

        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = startGrav;
        dashing = false;

        yield return new WaitForSeconds(dashCooldown);
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        // Ground Check
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, 0.2f);

        // Wall Check
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(wallCheck.position, 0.2f);
    }
}
