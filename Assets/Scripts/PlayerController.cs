using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;      // Speed for horizontal movement
    public float jumpForce = 10f;     // Force applied when jumping
    public LayerMask groundLayer;     // To detect ground layer for jumping

    private Rigidbody2D rb;           // Reference to the Rigidbody2D component
    private bool isGrounded = false;  // Check if the player is on the ground

    // This will allow us to detect where the player is standing
    public Transform groundCheck;     
    public float groundCheckRadius = 0.2f;  // Radius to check for ground

    void Start()
    {
        // Get the Rigidbody2D component attached to the player
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Horizontal movement (Left/Right)
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Check if the player is grounded before allowing jump
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Jumping input, only jump if the player is grounded
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
}