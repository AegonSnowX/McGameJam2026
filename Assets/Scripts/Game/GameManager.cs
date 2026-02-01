using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject deathScreenUI;
    [SerializeField] private GameObject winScreenUI;
    
    [Header("Settings")]
    [SerializeField] private float deathScreenDelay = 0.5f;
    [SerializeField] private int keysRequiredToWin = 3;

    public bool IsGameOver { get; private set; }
    public bool HasWon { get; private set; }
    public bool IsPaused { get; private set; }
    public int KeysCollected { get; private set; }
    public int KeysRequired => keysRequiredToWin;

    /// <summary>True when key 1 (first puzzle piece) has been collected.</summary>
    public bool HasKey1 { get; private set; }
    /// <summary>True when key 2 (second puzzle piece) has been collected.</summary>
    public bool HasKey2 { get; private set; }
    /// <summary>True when key 3 (third puzzle piece) has been collected.</summary>
    public bool HasKey3 { get; private set; }

    void Awake()
    {
        // Always set the instance to the new one (handles scene reloads)
        Instance = this;
        
        // Reset game state
        IsGameOver = false;
        HasWon = false;
        IsPaused = false;
        KeysCollected = 0;
        HasKey1 = false;
        HasKey2 = false;
        HasKey3 = false;
        Time.timeScale = 1f;
        
        // Hide screens at start
        if (deathScreenUI != null) deathScreenUI.SetActive(false);
        if (winScreenUI != null) winScreenUI.SetActive(false);
    }

    /// <summary>
    /// Call when player collects a key. keyIndex is 1, 2, or 3 and matches the puzzle piece that key reveals.
    /// Wins the game when all three keys are collected.
    /// </summary>
    public void AddKey(int keyIndex)
    {
        if (IsGameOver || HasWon) return;
        if (keyIndex == 1 && !HasKey1) { HasKey1 = true; KeysCollected++; }
        else if (keyIndex == 2 && !HasKey2) { HasKey2 = true; KeysCollected++; }
        else if (keyIndex == 3 && !HasKey3) { HasKey3 = true; KeysCollected++; }
        // Win is now triggered by going through the win door, not by collecting all keys.
    }

    /// <summary>Call when the player exits through the win door (after collecting all keys).</summary>
    public void TriggerWin()
    {
        OnPlayerWin();
    }

    private void OnPlayerWin()
    {
        if (HasWon) return;
        HasWon = true;
        if (GameAudioManager.Instance != null)
            GameAudioManager.Instance.PlayWinSound();
        Time.timeScale = 0f;
        if (winScreenUI != null) winScreenUI.SetActive(true);
    }

    void OnDestroy()
    {
        // Clear instance when destroyed
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void OnPlayerDeath()
    {
        if (IsGameOver) return;
        
        IsGameOver = true;
        if (GameAudioManager.Instance != null)
            GameAudioManager.Instance.PlayDeathSound();

        // Show death screen after short delay
        StartCoroutine(ShowDeathScreenDelayed());
    }

    private System.Collections.IEnumerator ShowDeathScreenDelayed()
    {
        yield return new WaitForSeconds(deathScreenDelay);
        
        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(true);
        }
        
        // Pause the game
        Time.timeScale = 0f;
    }

    /// <summary>Pause or resume the game (used by pause menu).</summary>
    public void SetPaused(bool paused)
    {
        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
    }

    public void RestartGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
