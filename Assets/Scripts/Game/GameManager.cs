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
    public int KeysCollected { get; private set; }
    public int KeysRequired => keysRequiredToWin;

    void Awake()
    {
        // Always set the instance to the new one (handles scene reloads)
        Instance = this;
        
        // Reset game state
        IsGameOver = false;
        HasWon = false;
        KeysCollected = 0;
        Time.timeScale = 1f;
        
        // Hide screens at start
        if (deathScreenUI != null) deathScreenUI.SetActive(false);
        if (winScreenUI != null) winScreenUI.SetActive(false);
    }

    /// <summary>
    /// Call when player collects a key. Wins the game when enough keys are collected.
    /// </summary>
    public void AddKey()
    {
        if (IsGameOver || HasWon) return;
        KeysCollected++;
        if (KeysCollected >= keysRequiredToWin)
        {
            OnPlayerWin();
        }
    }

    private void OnPlayerWin()
    {
        if (HasWon) return;
        HasWon = true;
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

    public void RestartGame()
    {
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
