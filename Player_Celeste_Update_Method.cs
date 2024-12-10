using System;
using UnityEngine;

public class Player : Actor
{
    // Physics and Movement
    public float speed = 120f;          // Horizontal movement speed
    public float gravity = 9.8f;        // Gravity acceleration
    public float jumpForce = 200f;      // Force applied when jumping
    public int maxDashes = 1;           // Maximum number of dashes
    private float stamina;              // Player stamina
    private int dashCount;              // Current dashes available
    private bool isGrounded;            // Is the player on the ground?
    private bool canDash;               // Is the player able to dash?
    private Vector2 previousPosition;   // Track position to calculate changes

    // State and Timers
    private float wallSlideTimer;       // Timer for wall sliding
    private bool wallSliding;           // Is the player wall-sliding?
    private float dashCooldownTimer;    // Dash cooldown timer

    // Input
    private float moveX;
    private bool jumpInput;
    private bool dashInput;

    private void Start()
    {
        stamina = ClimbMaxStamina;
        dashCount = maxDashes;
    }

    public override void Update()
    {
        base.Update();

        // Store previous position for calculations
        previousPosition = Position;

        // Handle Input
        HandleInput();

        // Apply Gravity
        ApplyGravity();

        // Handle Movement
        HandleMovement();

        // Handle Jumping
        HandleJumping();

        // Handle Dashing
        HandleDashing();

        // Wall Sliding
        HandleWallSlide();

        // Update Sprite or Animation
        UpdateSprite();

        // Adjust Camera (Optional)
        AdjustCamera();

        // Enforce Bounds
        EnforceBounds();

        // Reset grounded state if no longer on ground
        if (!CheckGroundCollision())
            isGrounded = false;
    }

    private void HandleInput()
    {
        // Horizontal input
        moveX = Input.GetAxis("Horizontal");

        // Jump input
        jumpInput = Input.GetButtonDown("Jump");

        // Dash input
        dashInput = Input.GetButtonDown("Dash");
    }

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            Speed = new Vector2(Speed.x, Speed.y + gravity * Time.deltaTime);
        }
    }

    private void HandleMovement()
    {
        // Apply horizontal movement
        if (moveX != 0)
        {
            MoveX(moveX * speed * Time.deltaTime, OnHorizontalCollide);
        }

        // Vertical movement (gravity or jumping handled separately)
        MoveY(Speed.y * Time.deltaTime, OnVerticalCollide);
    }

    private void HandleJumping()
    {
        if (jumpInput && isGrounded)
        {
            // Apply upward force for jumping
            Speed = new Vector2(Speed.x, -jumpForce);
            isGrounded = false;
        }
    }

    private void HandleDashing()
    {
        if (dashInput && dashCount > 0 && canDash)
        {
            // Dash logic (e.g., apply a large velocity in a direction)
            Vector2 dashDirection = new Vector2(moveX, jumpInput ? 1 : 0).normalized;
            Speed = dashDirection * 300f;

            // Reduce dash count and trigger cooldown
            dashCount--;
            dashCooldownTimer = 0.5f; // Example cooldown time
            canDash = false;
        }

        // Cooldown logic
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
        else
        {
            canDash = true;

            // Refill dash when grounded
            if (isGrounded && dashCount < maxDashes)
            {
                dashCount = maxDashes;
            }
        }
    }

    private void HandleWallSlide()
    {
        if (wallSliding)
        {
            Speed = new Vector2(Speed.x, Math.Max(Speed.y, -50f)); // Slow downward speed
            wallSlideTimer -= Time.deltaTime;

            if (wallSlideTimer <= 0)
            {
                wallSliding = false;
            }
        }
    }

    private void UpdateSprite()
    {
        // Handle sprite flipping based on movement direction
        if (moveX != 0)
        {
            SpriteRenderer.flipX = moveX < 0;
        }

        // Set appropriate animations or visual states
    }

    private void AdjustCamera()
    {
        // Optional: Smooth camera adjustment based on player's position
    }

    private void EnforceBounds()
    {
        // Ensure the player stays within level bounds
    }

    private bool CheckGroundCollision()
    {
        // Check if the player is standing on a solid
        return CollideAt(Position + Vector2.down);
    }

    private void OnHorizontalCollide()
    {
        // Horizontal collision logic
    }

    private void OnVerticalCollide()
    {
        // Vertical collision logic
        if (Speed.y > 0)
        {
            // Landed on the ground
            isGrounded = true;
            Speed = new Vector2(Speed.x, 0);
        }
    }
}
