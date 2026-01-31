using UnityEngine;

/// <summary>
/// Singleton that tracks trap sound positions. When a trap triggers, enemies are attracted to that position.
/// </summary>
public class TrapSoundManager : MonoBehaviour
{
    public static TrapSoundManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private float defaultSoundDuration = 5f;

    private Vector3 _soundPosition;
    private float _soundEndTime;
    private bool _hasActiveSound;

    public bool HasActiveSound => _hasActiveSound && Time.time < _soundEndTime;
    public Vector3 SoundPosition => _soundPosition;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    /// <summary>
    /// Notify that a sound was triggered at this position. Enemies in range will be attracted to it.
    /// </summary>
    public void ActivateSound(Vector3 position, float duration = -1f)
    {
        _soundPosition = position;
        _soundEndTime = Time.time + (duration > 0f ? duration : defaultSoundDuration);
        _hasActiveSound = true;
    }

    /// <summary>
    /// Clear the current sound (e.g. when trap is disabled).
    /// </summary>
    public void ClearSound()
    {
        _hasActiveSound = false;
    }
}
