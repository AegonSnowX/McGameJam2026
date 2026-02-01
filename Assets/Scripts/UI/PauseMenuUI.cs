using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Pause menu opened with Escape. Assign the pause panel and buttons: Continue, Back to Menu, Restart, Exit.
/// </summary>
public class PauseMenuUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject pausePanel;

    [Header("HUD to hide when paused")]
    [Tooltip("Assign key count, noise meter, puzzle image, etc. (or one parent that contains them). They are hidden when paused and shown when resumed.")]
    [SerializeField] private GameObject[] hudElementsToHide;

    [Header("Scene Names")]
    [SerializeField] private string menuSceneName = "MenuScene";

    [Header("Buttons")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;

    private bool _isPaused;

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (continueButton != null)
            continueButton.onClick.AddListener(Resume);
        if (backToMenuButton != null)
            backToMenuButton.onClick.AddListener(GoToMenu);
        if (restartButton != null)
            restartButton.onClick.AddListener(Restart);
        if (exitButton != null)
            exitButton.onClick.AddListener(Exit);
    }

    void Update()
    {
        if (GameManager.Instance != null && (GameManager.Instance.IsGameOver || GameManager.Instance.HasWon))
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    private void TogglePause()
    {
        _isPaused = !_isPaused;
        SetPaused(_isPaused);
    }

    private void SetPaused(bool paused)
    {
        _isPaused = paused;
        if (GameManager.Instance != null)
            GameManager.Instance.SetPaused(paused);
        if (pausePanel != null)
            pausePanel.SetActive(_isPaused);

        if (hudElementsToHide != null)
        {
            foreach (GameObject go in hudElementsToHide)
            {
                if (go != null)
                    go.SetActive(!_isPaused);
            }
        }
    }

    public void Resume()
    {
        SetPaused(false);
    }

    private void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    private void Restart()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
            GameManager.Instance.RestartGame();
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Exit()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
            GameManager.Instance.QuitGame();
        else
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
