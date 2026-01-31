using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject deathScreenUI;
    
    [Header("Settings")]
    [SerializeField] private float deathScreenDelay = 0.5f;

    public bool IsGameOver { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // Hide death screen at start
        if (deathScreenUI != null)
        {
            deathScreenUI.SetActive(false);
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
        
        // Optional: Pause the game
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
