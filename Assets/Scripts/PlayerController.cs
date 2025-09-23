using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // The variable that acts as the toggle. When true, the player can move.
    public bool canMove = true;

    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    [SerializeField] private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Check for player input to toggle movement on and off
        if (Input.GetKeyDown(KeyCode.T))
        {
            canMove = !canMove; // This flips the boolean's value
        }

        // Get movement input from Unity's Input Manager
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        // Only run the movement code if 'canMove' is true
        if (canMove)
        {
            Vector2 movement = moveInput.normalized * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);
        }
        else
        {
            // If movement is disabled, set velocity to zero to stop the character
            rb.linearVelocity = Vector2.zero;
        }
    }
}