using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private GameObject button;
    [Header("Text Reference")]
    [SerializeField] private TextMeshProUGUI textComponent;
    
    [Header("Animation Settings")]
    [SerializeField] private float characterDelay = 0.05f;
    [SerializeField] private float lineDelay = 0.5f;
    [SerializeField] private bool typeByCharacter = true; // true = character by character, false = line by line
    
    [Header("Scene Transition")]
    [SerializeField] private string nextSceneName = "MainScene";
    [SerializeField] private float delayBeforeSceneChange = 2f;
    
    [Header("Audio (Optional)")]
    [SerializeField] private AudioSource typingSound;

    private string fullText;
    private bool isAnimating;

    void Awake()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<TextMeshProUGUI>();
        }
    }

    void Start()
    {
        // Store the full text and clear it
        fullText = textComponent.text;
        textComponent.text = "";
        
        // Start the animation automatically
        StartTypewriter();

         StartCoroutine(ShowButtonAfterDelay());
    }
      IEnumerator ShowButtonAfterDelay()
    {
        yield return new WaitForSecondsRealtime(5f);

        if (button != null)
        {
            button.SetActive(true);
        }
    }

    public void StartTypewriter()
    {
        if (isAnimating) return;
        
        if (typeByCharacter)
        {
            StartCoroutine(TypeCharacterByCharacter());
        }
        else
        {
            StartCoroutine(TypeLineByLine());
        }
    }

    private IEnumerator TypeCharacterByCharacter()
    {
        isAnimating = true;
        textComponent.text = "";
        
        foreach (char c in fullText)
        {
            textComponent.text += c;
            
            // Play typing sound for non-space characters
            if (typingSound != null && c != ' ' && c != '\n')
            {
                typingSound.Play();
            }
            
            // Small pause for newlines
            if (c == '\n')
            {
                yield return new WaitForSeconds(lineDelay);
            }
            else
            {
                yield return new WaitForSeconds(characterDelay);
            }
        }
        
        isAnimating = false;
        OnAnimationComplete();
    }

    private IEnumerator TypeLineByLine()
    {
        isAnimating = true;
        textComponent.text = "";
        
        string[] lines = fullText.Split('\n');
        
        for (int i = 0; i < lines.Length; i++)
        {
            // Add the line
            if (i > 0)
            {
                textComponent.text += "\n";
            }
            textComponent.text += lines[i];
            
            // Play sound
            if (typingSound != null)
            {
                typingSound.Play();
            }
            
            // Wait before next line
            yield return new WaitForSeconds(lineDelay);
        }
        
        isAnimating = false;
        OnAnimationComplete();
    }

    private void OnAnimationComplete()
    {
        Debug.Log("Typewriter animation complete!");
     
      button.GetComponentInChildren<TextMeshProUGUI>().text = "Press Here to Continue";
    }

    private IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(delayBeforeSceneChange);
        
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    // Skip animation and show full text
    public void SkipAnimation()
    {
        StopAllCoroutines();
        textComponent.text = fullText;
        isAnimating = false;
       loadNextScene();
          
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    public void loadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
