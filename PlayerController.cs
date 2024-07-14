using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    public float acceleration = 5f;
    public float deceleration = 5f;

    [Header("Jump Settings")]
    public float jumpForce = 7f;
    public float lowJumpMultiplier = 2f;
    public float fallMultiplier = 2.5f;
    public float coyoteTime = 0.2f; // Time allowed to jump after falling off a platform
    public float jumpBufferTime = 0.1f; // Time allowed to register a jump input before landing

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private float moveInput;
    private bool isJumping;
    private bool isHoldingJump;

    private float currentSpeed;
    private float targetSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Coyote time and jump buffer handling
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Handle movement input
        moveInput = Input.GetAxisRaw("Horizontal");

        // Handle jumping
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpBufferCounter = 0f;
            isJumping = true;
            isHoldingJump = true;
        }

        if (Input.GetButtonUp("Jump"))
        {
            isHoldingJump = false;
        }

        // Jump height control
        if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        // Smooth movement using Mathf.SmoothDamp
        targetSpeed = moveInput * moveSpeed;
        float speedDifference = targetSpeed - rb.velocity.x;
        float smoothSpeed = Mathf.SmoothDamp(rb.velocity.x, targetSpeed, ref currentSpeed, acceleration * Time.fixedDeltaTime);
        rb.velocity = new Vector2(smoothSpeed, rb.velocity.y);

        // Deceleration when no input
        if (moveInput == 0)
        {
            float decelRate = Mathf.Min(Mathf.Abs(rb.velocity.x), deceleration * Time.fixedDeltaTime);
            rb.velocity = new Vector2(Mathf.MoveTowards(rb.velocity.x, 0, decelRate), rb.velocity.y);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a sphere at the ground check position for debugging
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
