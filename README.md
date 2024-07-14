# 🎮 › A simple, basic 2D Unity Platformer Controller
Making games in unity you will most likely come across Platformers. They are often very easy to make, but hard to perfect. One key element of a good platformer is the movement. It has to be very smooth, but no too smooth. The stats have to be perfectly balanced in order to make the movement perfect.

```c#
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    public float acceleration = 5f;
    public float deceleration = 5f;
```
This section defines the basic movement settings, including speed, acceleration, and deceleration.
```c#
    [Header("Jump Settings")]
    public float jumpForce = 7f;
    public float lowJumpMultiplier = 2f;
    public float fallMultiplier = 2.5f;
    public float coyoteTime = 0.2f; // Time allowed to jump after falling off a platform
    public float jumpBufferTime = 0.1f; // Time allowed to register a jump input before landing
```
Here, jump-related settings are defined. These include the jump force, multipliers for jump height control, and buffers to make jumping feel more responsive.
```c#
    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
```
This section sets up the parameters for checking if the player is on the ground, using a small radius circle and a specific layer mask.
```c#
    private Rigidbody2D rb;
    private bool isGrounded;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private float moveInput;
    private bool isJumping;
    private bool isHoldingJump;

    private float currentSpeed;
    private float targetSpeed;

```
Private variables are declared here to handle internal states, such as the Rigidbody2D component, jump timing, and movement input.
```c#
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
```
The Start method initializes the Rigidbody2D component.
```c#
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
```
In the Update method, ground checking is performed using a circle overlap test. Coyote time and jump buffer counters are managed to allow forgiving jump input timing.
```c#
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
```
This part handles player input for movement and jumping. It also adjusts the player's vertical velocity to control jump height and falling speed.
```c#
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

```
The FixedUpdate method smooths out horizontal movement and handles deceleration when there is no input.
```c#
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
```
