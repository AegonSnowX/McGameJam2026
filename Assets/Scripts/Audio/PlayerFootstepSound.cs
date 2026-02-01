using UnityEngine;

/// <summary>
/// Plays footstep sounds while the player is moving. Add to the player GameObject.
/// Assign an AudioSource (or it will use one on this object) and a footstep clip.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class PlayerFootstepSound : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioClip footstepClip;

    [Header("Timing")]
    [Tooltip("Time between footsteps when moving.")]
    [SerializeField] private float stepInterval = 0.4f;
    [SerializeField] private float pitchMin = 0.9f;
    [SerializeField] private float pitchMax = 1.1f;

    private float _stepTimer;

    void Awake()
    {
        if (footstepSource == null)
            footstepSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (footstepSource == null || footstepClip == null) return;
        if (PlayerMovement.Instance == null || PlayerMovement.Instance.IsDead) return;

        if (!PlayerMovement.Instance.IsMoving)
        {
            _stepTimer = 0f;
            return;
        }

        _stepTimer -= Time.deltaTime;
        if (_stepTimer <= 0f)
        {
            _stepTimer = stepInterval;
            footstepSource.pitch = Random.Range(pitchMin, pitchMax);
            footstepSource.PlayOneShot(footstepClip);
        }
    }
}
