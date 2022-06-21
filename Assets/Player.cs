using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region components
    private Rigidbody2D rb; // rb - rigid body
    private Animator anim;
    #endregion

    private bool canRun = false;
    private bool canDoubleJump = true;
    private bool isGrounded;
    private bool isRunning;
    private bool isBottomWallDetected;
    private bool isWallDetected;
    private bool isCeilingDetected;
    private bool canClimbLedge;
    private bool canRoll;

    private float speedMilestone;
    [SerializeField] private float speedMultiplier;
    [SerializeField] private float speedIncreaseMilestone;
    private float defaultSpeedIncreaseMilestone;

    [Header("Coins Info")]
    public int coins;

    [Header("Movement Info")]
    public float moveSpeed;
    public float maxMoveSpeed;
    public float animationSpeedMultiplier = 0.04f;
    private float defaultMoveSpeed;

    [Header("Jump Info")]
    public float jumpForce;
    public float doubleJumpForceMultiplier;
    public float speedForRoll;

    [Header("Knockback Info")]
    [SerializeField] private Vector2 knockbackDirection;
    [SerializeField] private float knockbackPower;

    private bool canBeKnocked = true;
    private bool isKnocked;

    [Header("SlideInfo")]
    public float slideSpeedMultiplier;
    private bool isSliding;
    private bool canSlide = true;
    [SerializeField] private float slidingTime;
    [SerializeField] private float slidingCooldown;
    private float slidingBegun;

    [Header("Ledge Info")]
    [SerializeField] private Transform ledgeCheck;
    public float ledgeClimbXoffset1 = 0f;
    public float ledgeClimbYoffset1 = 0f;
    public float ledgeClimbXoffset2 = 0f;
    public float ledgeClimbYoffset2 = 0f;
    private bool isTouchingLedge;
    private bool isLedgeDetected;

    private Vector2 ledgePosBot;
    private Vector2 ledgePos1; // Position to hold the player before animation ends.
    private Vector2 ledgePos2; // Position to move the player after animation ends.

    [Header("Collision detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform bottomWallCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform ceilingCheck;
    public float wallCheckDistance;
    public float groundCheckRadius;
    public LayerMask whatIsGround;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Record starting speed and milestone so we can reset them back to the original values.
        defaultMoveSpeed = moveSpeed;
        defaultSpeedIncreaseMilestone = speedIncreaseMilestone;

        speedMilestone = speedIncreaseMilestone;
    }

    // Update is called once per frame // if you have 60fps -  60 times per second
    void Update()
    {
        // If any key is pressed, start the game.
        if (Input.anyKey)
        {
            canRun = true;
        }

        checkForRun();
        checkForJump();
        checkForSlide();
        checkForSpeedingUp();
        checkForLedgeClimb();
        checkForCollisions();

        AnimationControllers();
    }

    private void AnimationControllers()
    {
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isRunning", isRunning);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isSliding", isSliding);
        anim.SetBool("canClimbLedge", canClimbLedge);
        anim.SetBool("canDoubleJump", canDoubleJump);
        anim.SetBool("canRoll", canRoll);
        anim.SetBool("isKnocked", isKnocked);
        anim.SetFloat("runningSpeed", rb.velocity.x * animationSpeedMultiplier);

        if (rb.velocity.y < speedForRoll)
        {
            canRoll = true;
        }
    }

    // This is called automatically by the Animation Event system when the rolling animation is finished.
    private void rollAnimationFinished()
    {
        canRoll = false;
    }

    // This is called automatically by the Animation Event system when the knockback animation is finished.
    private void knockbackAnimationFinished()
    {
        isKnocked = false;
        canBeKnocked = true;
        canRun = true;
    }

    public void knockback()
    {
        if (canBeKnocked)
        {
            isKnocked = true;
        }
    }

    private void checkForRun()
    {
        // This makes sure that we can only be knocked back once, until the animation is completed.
        if (isKnocked && canBeKnocked)
        {
            canBeKnocked = false;
            canRun = false;
            rb.velocity = knockbackDirection * knockbackPower;
        }

        if (canRun)
        {
            // If player has collided with a wall, reset the speed.
            if (isBottomWallDetected || isWallDetected  && !isSliding)
            {
                speedReset();
            }

            // If player is sliding, increase the speed by the multiplier.
            if (isSliding)
            {
                rb.velocity = new Vector2(moveSpeed * slideSpeedMultiplier, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
            }
        }

        if (rb.velocity.x > 0)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }
    }

    private void checkForJump()
    {
        if (Input.GetKeyDown("space") || Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(isGrounded)
            {
                jump(jumpForce);
                canDoubleJump = true;
            }
            else if (canDoubleJump)
            {
                jump(jumpForce * doubleJumpForceMultiplier);
                canDoubleJump = false;
            }
        }
    }

    private void checkForSlide()
    {
        float current_time = Time.time;

        if (Input.GetKeyDown(KeyCode.LeftShift) && canSlide && isGrounded && !isBottomWallDetected && !isWallDetected)
        {
            isSliding = true;
            slidingBegun = current_time;
            canSlide = false;
        }

        if (current_time > slidingBegun + slidingTime && !isCeilingDetected)
        {
            isSliding = false;
        }

        if (current_time > slidingBegun + slidingTime + slidingCooldown)
        {
            canSlide = true;
        }
    }

    public void checkForSpeedingUp()
    {
        if (transform.position.x > speedMilestone)
        {
            // Change the milestone at which speed will increase.
            speedMilestone += speedIncreaseMilestone;

            // Increase the milestone increase
            speedIncreaseMilestone *= speedMultiplier;
            moveSpeed *= speedMultiplier;

            // Check if our current speed is greater than maximum speed.
            if (moveSpeed > maxMoveSpeed)
            {
                moveSpeed = maxMoveSpeed;
            }
        }
    }

    public void checkForLedgeClimb()
    {
        if (isLedgeDetected && !canClimbLedge)
        {
            canClimbLedge = true;
            canRun = false;

            ledgePos1 = new Vector2(ledgePosBot.x + wallCheckDistance + ledgeClimbXoffset1, ledgePosBot.y + ledgeClimbYoffset1);
            ledgePos2 = new Vector2(ledgePosBot.x + wallCheckDistance + ledgeClimbXoffset2, ledgePosBot.y + ledgeClimbYoffset2);
        }

        if (canClimbLedge)
        {
            transform.position = ledgePos1;
        }
    }

    private void speedReset()
    {
        moveSpeed = defaultMoveSpeed;
        speedIncreaseMilestone = defaultSpeedIncreaseMilestone;
    }
    
    private void checkIfLedgeclimbFinished()
    {
        transform.position = ledgePos2;
        canClimbLedge = false;
        canRun = true;
        isLedgeDetected = false;
    }

    private void jump(float force)
    {
        rb.velocity = new Vector2(rb.velocity.x, force);
    }


    private void checkForCollisions()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        isBottomWallDetected = Physics2D.Raycast(bottomWallCheck.position, Vector2.right, wallCheckDistance, whatIsGround);
        isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, Vector2.right, wallCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(wallCheck.position, Vector2.right, wallCheckDistance, whatIsGround);
        isCeilingDetected = Physics2D.Raycast(ceilingCheck.position, Vector2.up, wallCheckDistance, whatIsGround);

        if (isWallDetected && !isTouchingLedge && !isLedgeDetected)
        {
            isLedgeDetected = true;
            ledgePosBot = wallCheck.position;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawLine(bottomWallCheck.position, new Vector3(bottomWallCheck.position.x + wallCheckDistance, bottomWallCheck.position.y, bottomWallCheck.position.z));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
        Gizmos.DrawLine(ledgeCheck.position, new Vector3(ledgeCheck.position.x + wallCheckDistance, ledgeCheck.position.y, ledgeCheck.position.z));
        Gizmos.DrawLine(ceilingCheck.position, new Vector3(ceilingCheck.position.x, ceilingCheck.position.y + wallCheckDistance, ceilingCheck.position.z));
    }
}
