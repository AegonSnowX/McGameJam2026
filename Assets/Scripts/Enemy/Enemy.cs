using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BoxCollider2D))]
public class Enemy : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private string playerTag = "Player";
    
    [Header("Speed Settings")]
    [SerializeField] private float baseSpeed = 2f;
    [SerializeField] private float maxChaseSpeed = 8f;
    
    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private float noiseMultiplier = 2f;
    [SerializeField, Range(0f, 1f)] private float noiseThresholdToChase = 0.2f;
    [SerializeField, Range(0f, 1f)] private float noiseThresholdToLose = 0.05f;
    
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolWaitTime = 2f;
    [SerializeField] private float patrolSpeed = 2f;
    
    [Header("Random Wander (when no patrol points)")]
    [SerializeField] private float wanderRadius = 5f;
    [SerializeField] private float minWanderInterval = 2f;
    [SerializeField] private float maxWanderInterval = 5f;
    
    [Header("Memory")]
    [SerializeField] private float memoryDuration = 3f;

    private NavMeshAgent agent;
    private EnemyState currentState = EnemyState.Patrolling;
    private Vector3 lastKnownPlayerPosition;
    private float memoryTimer;
    private int currentPatrolIndex;
    private float patrolWaitTimer;
    private bool isWaitingAtPatrol;
    private float wanderTimer;
    private Vector3 wanderTarget;

    public enum EnemyState
    {
        Patrolling,
        Chasing,
        Searching
    }

    public EnemyState CurrentState => currentState;

    void Awake()
    {
        // Ensure collider is set as trigger for kill zone
        var collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = true;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed = patrolSpeed;
    }

    void Update()
    {
        float noiseLevel = GetNoiseLevel();
        
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
                CheckForPlayer(noiseLevel);
                break;
                
            case EnemyState.Chasing:
                ChasePlayer(noiseLevel);
                break;
                
            case EnemyState.Searching:
                SearchForPlayer(noiseLevel);
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player == null)
            {
                player = other.GetComponentInParent<PlayerMovement>();
            }
            
            if (player != null && !player.IsDead)
            {
                player.Die();
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
        if (target == null) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        
        // Player is loud and within detection range
        if (noiseLevel > noiseThresholdToChase && distanceToPlayer < detectionRadius)
        {
            StartChasing();
        }
    }

    private void StartChasing()
    {
        currentState = EnemyState.Chasing;
        isWaitingAtPatrol = false;
        Debug.Log("Enemy: Player detected! Chasing...");
    }

    private void ChasePlayer(float noiseLevel)
    {
        if (target == null) return;

        // Update last known position while player is loud
        if (noiseLevel > noiseThresholdToLose)
        {
            lastKnownPlayerPosition = target.position;
            memoryTimer = memoryDuration;
            
            // Speed scales with noise level
            float speedBoost = noiseLevel * noiseMultiplier;
            agent.speed = Mathf.Lerp(baseSpeed, maxChaseSpeed, speedBoost);
        }
        else
        {
            // Player is quiet, start losing them
            memoryTimer -= Time.deltaTime;
            
            if (memoryTimer <= 0f)
            {
                // Lost the player, go search last known location
                StartSearching();
                return;
            }
        }
        
        agent.SetDestination(lastKnownPlayerPosition);
    }

    private void StartSearching()
    {
        currentState = EnemyState.Searching;
        agent.speed = patrolSpeed;
        Debug.Log("Enemy: Lost player, searching...");
    }

    private void SearchForPlayer(float noiseLevel)
    {
        // If we hear the player again, resume chase
        if (noiseLevel > noiseThresholdToChase)
        {
            StartChasing();
            return;
        }
        
        // Check if we've reached the last known position
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // Didn't find player, go back to patrolling
            currentState = EnemyState.Patrolling;
            Debug.Log("Enemy: Player not found, resuming patrol...");
        }
        else
        {
            agent.SetDestination(lastKnownPlayerPosition);
        }
    }

    private void Patrol()
    {
        agent.speed = patrolSpeed;

        // If no patrol points, use random wandering
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            RandomWander();
            return;
        }

        if (isWaitingAtPatrol)
        {
            patrolWaitTimer -= Time.deltaTime;
            if (patrolWaitTimer <= 0f)
            {
                isWaitingAtPatrol = false;
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            }
            return;
        }

        Transform patrolTarget = patrolPoints[currentPatrolIndex];
        agent.SetDestination(patrolTarget.position);

        // Check if we've reached the patrol point
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            isWaitingAtPatrol = true;
            patrolWaitTimer = patrolWaitTime;
        }
    }

    private void RandomWander()
    {
        wanderTimer -= Time.deltaTime;

        // Time to pick a new random destination
        if (wanderTimer <= 0f || (!agent.pathPending && agent.remainingDistance < 0.5f))
        {
            wanderTarget = GetRandomNavMeshPosition();
            agent.SetDestination(wanderTarget);
            wanderTimer = Random.Range(minWanderInterval, maxWanderInterval);
        }
    }

    private Vector3 GetRandomNavMeshPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        
        return transform.position;
    }

    // Debug visualization
    void OnDrawGizmosSelected()
    {
        // Detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        // Last known position
        if (currentState == EnemyState.Chasing || currentState == EnemyState.Searching)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(lastKnownPlayerPosition, 0.3f);
        }
        
        // Patrol points
        if (patrolPoints != null)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    Gizmos.DrawSphere(patrolPoints[i].position, 0.2f);
                    if (i < patrolPoints.Length - 1 && patrolPoints[i + 1] != null)
                    {
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                    }
                }
            }
        }
    }
}