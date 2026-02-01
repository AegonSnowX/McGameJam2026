using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class EnemyTeleportTrap : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private GameObject enemyToTeleport;
    
    [Header("Teleport Destination")]
    [SerializeField] private Transform teleportDestination;
    [SerializeField] private Vector3 teleportCoordinates;
    [SerializeField] private bool useTransformDestination = true;
    
    [Header("Freeze Settings")]
    [SerializeField] private float freezeDuration = 1f;
    
    [Header("Sound")]
    [SerializeField] private AudioClip trapSound;
    [SerializeField] private float soundVolume = 1f;
    
    [Header("Optional")]
    [SerializeField] private bool oneShot = true;
    [SerializeField] private float soundAttractDuration = 5f;

    private Collider2D _col;
    private bool _triggered;

    void Awake()
    {
        _col = GetComponent<Collider2D>();
        if (_col != null)
            _col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (oneShot && _triggered) return;

        _triggered = true;
        
        Debug.Log("[EnemyTeleportTrap] " + gameObject.name + ": TRIGGERED!", this);

        // Play trap sound
        PlayTrapSound();

        // Teleport the enemy
        TeleportEnemy();

        // Notify TrapSoundManager if exists
        if (TrapSoundManager.Instance != null)
        {
            TrapSoundManager.Instance.ActivateSound(transform.position, soundAttractDuration);
        }
    }

    private void TeleportEnemy()
    {
        if (enemyToTeleport == null)
        {
            Debug.LogWarning("[EnemyTeleportTrap] " + gameObject.name + ": No enemy assigned to teleport.", this);
            return;
        }

        // Get destination position
        Vector3 destination;
        if (useTransformDestination && teleportDestination != null)
        {
            destination = teleportDestination.position;
        }
        else
        {
            destination = teleportCoordinates;
        }

        // Get the NavMeshAgent component
        NavMeshAgent agent = enemyToTeleport.GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            // Use Warp to properly teleport NavMeshAgent without breaking it
            agent.Warp(destination);
            Debug.Log("[EnemyTeleportTrap] " + gameObject.name + ": Warped enemy to " + destination, this);
        }
        else
        {
            // No NavMeshAgent, just move transform directly
            enemyToTeleport.transform.position = destination;
            Debug.Log("[EnemyTeleportTrap] " + gameObject.name + ": Teleported enemy to " + destination, this);
        }

        // Trigger teleport animation and freeze enemy
        StartCoroutine(FreezeAndAnimateEnemy(agent));
    }

    private IEnumerator FreezeAndAnimateEnemy(NavMeshAgent agent)
    {
        // Get the Enemy script component to disable it during freeze
        Enemy enemyScript = enemyToTeleport.GetComponent<Enemy>();
        
        // Get the Animator component
        Animator animator = enemyToTeleport.GetComponent<Animator>();
        if (animator == null)
        {
            animator = enemyToTeleport.GetComponentInChildren<Animator>();
        }

        // Trigger teleport animation
        if (animator != null)
        {
            animator.SetTrigger("Teleported");
            Debug.Log("[EnemyTeleportTrap] " + gameObject.name + ": Triggered 'Teleported' animation.", this);
        }
        else
        {
            Debug.LogWarning("[EnemyTeleportTrap] " + gameObject.name + ": No Animator found on enemy.", this);
        }

        // Disable enemy script to prevent it from overriding our freeze
        if (enemyScript != null)
        {
            enemyScript.enabled = false;
            Debug.Log("[EnemyTeleportTrap] " + gameObject.name + ": Disabled Enemy script.", this);
        }

        // Freeze enemy movement
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            Debug.Log("[EnemyTeleportTrap] " + gameObject.name + ": Enemy frozen for " + freezeDuration + " seconds.", this);
        }

        // Wait for freeze duration
        yield return new WaitForSeconds(freezeDuration);

        // Re-enable enemy script
        if (enemyScript != null)
        {
            enemyScript.enabled = true;
            Debug.Log("[EnemyTeleportTrap] " + gameObject.name + ": Re-enabled Enemy script.", this);
        }

        // Resume enemy movement
        if (agent != null)
        {
            agent.isStopped = false;
            Debug.Log("[EnemyTeleportTrap] " + gameObject.name + ": Enemy resumed movement.", this);
        }
    }

    private void PlayTrapSound()
    {
        if (trapSound == null)
        {
            Debug.LogWarning("[EnemyTeleportTrap] " + gameObject.name + ": No trap sound assigned.", this);
            return;
        }

        // Create a temporary AudioSource to play the sound
        GameObject tempAudio = new GameObject("TeleportTrapSound_Temp");
        tempAudio.transform.position = transform.position;
        
        AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
        audioSource.clip = trapSound;
        audioSource.volume = soundVolume;
        audioSource.spatialBlend = 0f; // 2D sound
        audioSource.Play();
        
        // Destroy after clip finishes
        Destroy(tempAudio, trapSound.length + 0.1f);
        
        Debug.Log("[EnemyTeleportTrap] " + gameObject.name + ": Playing sound " + trapSound.name, this);
    }

    // Draw gizmo to show teleport destination in editor
    void OnDrawGizmosSelected()
    {
        Vector3 destination;
        if (useTransformDestination && teleportDestination != null)
        {
            destination = teleportDestination.position;
        }
        else
        {
            destination = teleportCoordinates;
        }

        // Draw line from trap to destination
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, destination);
        
        // Draw sphere at destination
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(destination, 0.5f);
    }
}
