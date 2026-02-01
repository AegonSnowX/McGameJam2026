using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(BoxCollider2D))]
public class EnemyNew : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;
    [SerializeField] private string playerTag = "Player";

    [Header("Speed Settings")]
    [SerializeField] private float roamSpeed = 2f;
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float trapRushSpeed = 8f;

    [Header("Detection")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField, Range(0f, 1f)] private float noiseThresholdToChase = 0.2f;

    [Header("Roaming")]
    [SerializeField] private float roamRadius = 10f;
    [SerializeField] private float minRoamInterval = 2f;
    [SerializeField] private float maxRoamInterval = 5f;

    [Header("Trap Response")]
    [SerializeField] private float trapArrivalRadius = 2f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private NavMeshAgent agent;
    private EnemyState currentState = EnemyState.Roaming;
    private EnemyState stateBeforeTrap;
    private Vector3 lastKnownPlayerPosition;
    private Vector3 trapPosition;
    private float roamTimer;
    private Vector2 lastMoveDirection = Vector2.down;

    public enum EnemyState
    {
        Roaming,
        Chasing,
        RushingToTrap
    }

    public EnemyState CurrentState => currentState;

    void Awake()
    {
        var collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = true;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = roamSpeed;

        // Auto-find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        // Auto-find animator
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }
        }

        // Subscribe to trap events
        if (TrapSoundManager.Instance != null)
        {
            TrapSoundManager.Instance.OnTrapActivated += OnTrapTriggered;
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from trap events
        if (TrapSoundManager.Instance != null)
        {
            TrapSoundManager.Instance.OnTrapActivated -= OnTrapTriggered;
        }
    }

    void Update()
    {
        // Check if game is paused
        if (GameManager.Instance != null && GameManager.Instance.IsPaused)
        {
            agent.isStopped = true;
            return;
        }
        agent.isStopped = false;

        float noiseLevel = GetNoiseLevel();

        switch (currentState)
        {
            case EnemyState.Roaming:
                Roam();
                CheckForPlayer(noiseLevel);
                break;

            case EnemyState.Chasing:
                ChasePlayer(noiseLevel);
                break;

            case EnemyState.RushingToTrap:
                RushToTrap();
                break;
        }

        UpdateAnimator();
    }

    private void OnTrapTriggered(Vector3 position)
    {
        // Save current state before rushing to trap
        if (currentState != EnemyState.RushingToTrap)
        {
            stateBeforeTrap = currentState;
        }

        trapPosition = position;
        currentState = EnemyState.RushingToTrap;
        agent.speed = trapRushSpeed;
        agent.SetDestination(trapPosition);

        Debug.Log("[EnemyNew] Trap triggered! Rushing to trap at " + position);
    }

    private void RushToTrap()
    {
        agent.SetDestination(trapPosition);

        // Check if we've reached the trap area
        float distanceToTrap = Vector3.Distance(transform.position, trapPosition);
        if (distanceToTrap <= trapArrivalRadius)
        {
            Debug.Log("[EnemyNew] Reached trap area. Returning to " + stateBeforeTrap);
            
            // Return to previous state
            currentState = stateBeforeTrap;
            
            if (currentState == EnemyState.Roaming)
            {
                agent.speed = roamSpeed;
                roamTimer = 0f; // Pick new roam destination
            }
            else if (currentState == EnemyState.Chasing)
            {
                agent.speed = chaseSpeed;
            }
        }
    }

    private float GetNoiseLevel()
    {
        if (MicrophoneInput.Instance == null) return 0f;
        return MicrophoneInput.Instance.NoiseLevel;
    }

    private void CheckForPlayer(float noiseLevel)
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Player is loud and within detection range - start chasing
        if (noiseLevel > noiseThresholdToChase && distanceToPlayer < detectionRadius)
        {
            StartChasing();
        }
    }

    private void StartChasing()
    {
        currentState = EnemyState.Chasing;
        agent.speed = chaseSpeed;
        Debug.Log("[EnemyNew] Player detected! Chasing...");
    }

    private void ChasePlayer(float noiseLevel)
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // If player is within detection radius and making noise, keep chasing
        if (distanceToPlayer < detectionRadius && noiseLevel > noiseThresholdToChase * 0.5f)
        {
            lastKnownPlayerPosition = player.position;
            agent.SetDestination(lastKnownPlayerPosition);
        }
        else
        {
            // Lost the player - go back to roaming
            Debug.Log("[EnemyNew] Lost player. Resuming roam.");
            currentState = EnemyState.Roaming;
            agent.speed = roamSpeed;
            roamTimer = 0f;
        }
    }

    private void Roam()
    {
        agent.speed = roamSpeed;
        roamTimer -= Time.deltaTime;

        // Time to pick a new random destination
        if (roamTimer <= 0f || (!agent.pathPending && agent.remainingDistance < 0.5f))
        {
            Vector3 newDestination = GetRandomNavMeshPosition();
            agent.SetDestination(newDestination);
            roamTimer = Random.Range(minRoamInterval, maxRoamInterval);
        }
    }

    private Vector3 GetRandomNavMeshPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return transform.position;
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        Vector2 velocity = new Vector2(agent.velocity.x, agent.velocity.y);
        bool isMoving = velocity.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            lastMoveDirection = velocity.normalized;
        }

        animator.SetBool("IsMoving", isMoving);
        animator.SetFloat("MoveX", lastMoveDirection.x);
        animator.SetFloat("MoveY", lastMoveDirection.y);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
        if (playerMovement == null)
        {
            playerMovement = other.GetComponentInParent<PlayerMovement>();
        }

        if (playerMovement != null && !playerMovement.IsDead)
        {
            playerMovement.Die();
        }
    }

    // Debug visualization
    void OnDrawGizmosSelected()
    {
        // Detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Roam radius
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, roamRadius);

        // Trap arrival radius (if rushing to trap)
        if (currentState == EnemyState.RushingToTrap)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(trapPosition, trapArrivalRadius);
            Gizmos.DrawLine(transform.position, trapPosition);
        }
    }
}
