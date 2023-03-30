using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
    private bool startSet = false;
    private float startSpeed;

    [Header("Jump")]
    public float jumpForce;
    private float startGrav;
    public float maxGravScale;
    public float gravIncreaseSpeed;
    public Transform groundCheck;
    public LayerMask groundLayer;

    // Input
    [Header("Input")]
    private float horizontalInput;
    public KeyCode jumpInput;

    private Rigidbody2D rb;

    #endregion


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        startGrav = rb.gravityScale;
    }

    #region Update / Fixed Update

    private void FixedUpdate()
    {
        CalculateAcceleration();
    }

    private void Update()
    {
        Jump();

        if (rb.velocity.y < 0)
        {
            UpdateGravity();
        }
            
            
        if (rb.velocity.y >= 0)
        {
            rb.gravityScale = startGrav;
        }
            

        Flip();

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
        //Accelerate
        if (rb.velocity.x != desiredSpeed)
        {
            if (!startSet)
            {
                startSpeed = rb.velocity.x;
                startSet = true;
            }


            float targetSpeed = rb.velocity.x;

            elapsedTime += Time.deltaTime;
            float lerp = elapsedTime / accelTime;

            targetSpeed = Mathf.Lerp(targetSpeed, desiredSpeed, accelRate.Evaluate(lerp));
            rb.velocity = new Vector2(horizontalInput * targetSpeed, rb.velocity.y);
        }
        else
        {
            elapsedTime = 0;
            startSet = false;
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
 
}
