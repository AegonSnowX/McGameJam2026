using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("Torch Rotation")]
    [SerializeField] private float mouseDeadzoneRadius = 0.5f;
    [SerializeField, Range(0f, 90f)] private float torchClampAngle = 45f;
    
    [Header("References")]
    [SerializeField] private Transform torch;
    [SerializeField] private Camera mainCamera;

    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 mouseWorldPosition;
    private Vector2 mouseDirection = Vector2.down; // Direction from player to mouse

    // Animation properties - use these in your Animator or PlayerAnimator script
    public bool IsMoving => movementInput.sqrMagnitude > 0.01f;
    public float MoveX => movementInput.x;
    public float MoveY => movementInput.y;
    public float MouseX => mouseDirection.x;
    public float MouseY => mouseDirection.y;
    
    // Death state
    public bool IsDead { get; private set; }

    void Awake()
    {
        Instance = this;
        
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
        if (IsDead) return;
        
        // Get WASD input
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");
        movementInput = movementInput.normalized;

        // Rotate torch towards mouse cursor and update mouse direction
        RotateTorchTowardsMouse();
    }

    void FixedUpdate()
    {
        if (IsDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        
        // Apply movement
        rb.linearVelocity = movementInput * moveSpeed;
    }
    
    public void Die()
    {
        if (IsDead) return;
        
        IsDead = true;
        movementInput = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        
        Debug.Log("Player died!");
        
        // Notify GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerDeath();
        }
    }

    private void RotateTorchTowardsMouse()
    {
        // Get mouse position in world space
        mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        
        // Calculate direction from player to mouse
        Vector2 direction = mouseWorldPosition - (Vector2)transform.position;
        
        // Only update if mouse is outside the deadzone
        if (direction.sqrMagnitude > mouseDeadzoneRadius * mouseDeadzoneRadius)
        {
            // Store normalized direction for animations
            mouseDirection = direction.normalized;
            
            // Calculate the angle in degrees (atan2 gives radians, convert to degrees)
            // Subtract 90 degrees because Unity's "up" is 0 degrees, but atan2 treats "right" as 0
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            
            // Clamp torch rotation based on movement direction
            if (IsMoving)
            {
                angle = ClampTorchAngle(angle);
            }
            
            // Apply rotation to torch only (not the player)
            if (torch != null)
            {
                torch.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }
    }

    private float ClampTorchAngle(float angle)
    {
        // Calculate base angle from movement direction
        // Up = 0°, Right = -90°, Down = 180°, Left = 90°
        float baseAngle = Mathf.Atan2(movementInput.y, movementInput.x) * Mathf.Rad2Deg - 90f;
        
        // Calculate the difference between mouse angle and movement angle
        float angleDiff = Mathf.DeltaAngle(baseAngle, angle);
        
        // Clamp the difference within the allowed range
        angleDiff = Mathf.Clamp(angleDiff, -torchClampAngle, torchClampAngle);
        
        // Return the clamped angle
        return baseAngle + angleDiff;
    }
}