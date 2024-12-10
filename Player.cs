using UnityEngine;

public class Player : Actor
{
    public float Speed = 100f;       // Horizontal movement speed in pixels per second
    public float Gravity = 9.8f;    // Gravity applied each frame
    public float JumpForce = 200f;  // Upward force applied when jumping
    private float velocityY;        // Vertical velocity
    private bool isGrounded;        // Is the player on the ground?
    private bool canMove = true;    // Can the player move?

    void Update()
    {
        if (!canMove) return;

        // Horizontal movement
        float horizontalInput = Input.GetAxis("Horizontal");
        MoveX(horizontalInput * Speed * Time.deltaTime, OnHorizontalCollide);

        // Vertical movement
        ApplyGravity();
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        MoveY(velocityY * Time.deltaTime, OnVerticalCollide);
    }

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            // Increase downward velocity over time
            velocityY += Gravity * Time.deltaTime;
        }
        else
        {
            // Reset vertical velocity when grounded
            velocityY = 0;
        }
    }

    private void Jump()
    {
        // Apply upward force
        velocityY = -JumpForce; // Negative because Unity's Y-axis goes down
        isGrounded = false;    // Player is no longer on the ground
    }

    private void OnHorizontalCollide()
    {
        // Handle horizontal collisions
        Debug.Log("Collided horizontally!");
    }

    private void OnVerticalCollide()
    {
        if (velocityY > 0) // Falling down
        {
            isGrounded = true;  // Player is grounded
            velocityY = 0;      // Stop downward movement
        }
        else if (velocityY < 0) // Jumping up and hitting a ceiling
        {
            velocityY = 0;      // Stop upward movement
            Debug.Log("Hit a ceiling!");
        }
    }
}
