using UnityEngine;

/// <summary>
/// Simple 2D wander for menu background: NPC moves to random points in a radius.
/// No NavMesh, no player detection. Use on the same GameObject as your enemy visual (or a child).
/// Optional: assign an Animator with IsMoving, MoveX, MoveY for walk animation.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class MenuNpcWanderer : MonoBehaviour
{
    [Header("Wander Area")]
    [SerializeField] private float wanderRadius = 6f;
    [SerializeField] private Vector2 center = Vector2.zero;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [Tooltip("Distance to consider 'arrived' at target.")]
    [SerializeField] private float arriveThreshold = 0.3f;
    [Tooltip("Min/max seconds before picking a new random target.")]
    [SerializeField] private Vector2 waitBetweenTargets = new Vector2(1f, 4f);

    [Header("Animation (optional)")]
    [SerializeField] private Animator animator;

    private Rigidbody2D _rb;
    private Vector2 _currentTarget;
    private float _nextTargetTime;
    private Vector2 _lastMoveDirection;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;
    }

    void Start()
    {
        PickNewTarget();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (Time.time >= _nextTargetTime || HasArrived())
            PickNewTarget();
    }

    void FixedUpdate()
    {
        Vector2 toTarget = _currentTarget - (Vector2)transform.position;
        float distance = toTarget.magnitude;

        if (distance < arriveThreshold)
        {
            _rb.linearVelocity = Vector2.zero;
            UpdateAnimator(Vector2.zero, false);
            return;
        }

        Vector2 direction = toTarget.normalized;
        _rb.linearVelocity = direction * moveSpeed;
        _lastMoveDirection = direction;
        UpdateAnimator(direction, true);
    }

    private bool HasArrived()
    {
        float dist = Vector2.Distance(transform.position, _currentTarget);
        return dist < arriveThreshold;
    }

    private void PickNewTarget()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        _currentTarget = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * wanderRadius;
        _nextTargetTime = Time.time + Random.Range(waitBetweenTargets.x, waitBetweenTargets.y);
    }

    private void UpdateAnimator(Vector2 direction, bool isMoving)
    {
        if (animator == null) return;
        animator.SetBool("IsMoving", isMoving);
        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(center, wanderRadius);
    }
}
