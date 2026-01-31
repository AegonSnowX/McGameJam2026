using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Main menu HUD: Start, Credit, Exit. Assign buttons in the Inspector.
/// Add MenuScene and your game scene (e.g. Proto) to Build Settings.
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string gameSceneName = "Proto";
    [SerializeField] private string creditSceneName = "Credit";

    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button creditButton;
    [SerializeField] private Button exitButton;

    [Header("Optional Credit Panel")]
    [Tooltip("If you use an in-scene credit panel instead of a Credit scene, assign it here.")]
    [SerializeField] private GameObject creditPanel;

    void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(OnStartClicked);
        if (creditButton != null)
            creditButton.onClick.AddListener(OnCreditClicked);
        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitClicked);
    }

    private void OnStartClicked()
    {
        if (string.IsNullOrEmpty(gameSceneName))
        {
            Debug.LogWarning("MainMenuUI: gameSceneName is empty. Add your game scene to Build Settings and set gameSceneName (e.g. Proto).");
            return;
        }
        SceneManager.LoadScene(gameSceneName);
    }

    private void OnCreditClicked()
    {
        if (creditPanel != null)
        {
            creditPanel.SetActive(!creditPanel.activeSelf);
            return;
        }
        if (!string.IsNullOrEmpty(creditSceneName))
            SceneManager.LoadScene(creditSceneName);
        else
            Debug.Log("Credit: Add a Credit scene to Build Settings and set creditSceneName, or assign a creditPanel.");
    }

    private void OnExitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
