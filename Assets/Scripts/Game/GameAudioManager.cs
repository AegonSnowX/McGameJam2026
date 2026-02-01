using UnityEngine;

/// <summary>
/// Central place for non-spatial / UI and game-event sounds (button click, death, win).
/// Add to a persistent GameObject (e.g. in a bootstrap scene or main game scene).
/// Assign clips and one AudioSource for UI one-shots.
/// </summary>
public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager Instance { get; private set; }

    [Header("UI / 2D sounds (one-shots)")]
    [SerializeField] private AudioSource uiSource;
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField] private AudioClip deathStingClip;
    [SerializeField] private AudioClip winStingClip;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    /// <summary>Call from any button or from PlaySoundOnButtonClick.</summary>
    public void PlayButtonClick()
    {
        if (uiSource != null && buttonClickClip != null)
            uiSource.PlayOneShot(buttonClickClip);
    }

    /// <summary>Call when player dies (e.g. from GameManager).</summary>
    public void PlayDeathSound()
    {
        if (uiSource != null && deathStingClip != null)
            uiSource.PlayOneShot(deathStingClip);
    }

    /// <summary>Call when player wins (e.g. from GameManager).</summary>
    public void PlayWinSound()
    {
        if (uiSource != null && winStingClip != null)
            uiSource.PlayOneShot(winStingClip);
    }
}
