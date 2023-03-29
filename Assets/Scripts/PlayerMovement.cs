using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    private float desiredSpeed;
    private bool isFacingRight = true;

    //Movement lerp
    private float elapsedTime;
    public float accelTime;
    public float longAccelTime;
    public AnimationCurve accelRate;
    private bool startSet = false;
    private float startSpeed;

    [Header("Jump")]
    public float jumpForce;
    private float startGrav;
    public float maxGravScale;
    public float gravIncreaseSpeed;

    // Input
    [Header("Input")]
    private float horizontalInput;
    public KeyCode jumpInput;

    private Rigidbody2D rb;
   

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

        Debug.Log(OnGround());

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
        if (Input.GetKeyDown(jumpInput))
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        else if (Input.GetKeyUp(jumpInput))
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
    }


    void UpdateGravity()
    {
        float newGrav = Mathf.MoveTowards(rb.gravityScale, maxGravScale, gravIncreaseSpeed * Time.deltaTime);
        rb.gravityScale = newGrav;
    }

    bool OnGround()
    {
        return Physics2D.Raycast(transform.position, -Vector2.up, 1.5f);
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
            rb.velocity = new Vector2(targetSpeed, rb.velocity.y) * horizontalInput;
        }
        else
        {
            elapsedTime = 0;
            startSet = false;
        }

        

    }

    private void Flip()
    {
        if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    #endregion

    #region Gizmos

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 dir = Vector2.up;
        Gizmos.DrawRay(transform.position, dir);
    }

    #endregion
}
