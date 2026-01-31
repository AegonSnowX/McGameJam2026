using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("Mouse Rotation")]
    [SerializeField] private float mouseDeadzoneRadius = 0.5f;
    
    [Header("References")]
    [SerializeField] private Camera mainCamera;

    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 mouseWorldPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        
        // Auto-assign main camera if not set in inspector
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        // Get WASD input
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");
        movementInput = movementInput.normalized;

        // Face towards mouse cursor
        FaceMouseCursor();
    }

    void FixedUpdate()
    {
        // Apply movement
        rb.linearVelocity = movementInput * moveSpeed;
    }

    private void FaceMouseCursor()
    {
        // Get mouse position in world space
        mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        
        // Calculate direction from player to mouse
        Vector2 direction = mouseWorldPosition - (Vector2)transform.position;
        
        // Only rotate if mouse is outside the deadzone
        if (direction.sqrMagnitude > mouseDeadzoneRadius * mouseDeadzoneRadius)
        {
            // Calculate the angle in degrees (atan2 gives radians, convert to degrees)
            // Subtract 90 degrees because Unity's "up" is 0 degrees, but atan2 treats "right" as 0
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            
            // Apply rotation to player
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}