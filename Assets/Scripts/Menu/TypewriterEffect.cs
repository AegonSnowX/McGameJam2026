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
    [SerializeField] private bool typeByCharacter = true;

    [Header("Click to advance")]
    [Tooltip("Left click reveals this many lines at once (e.g. 2). Slow reveal still runs in parallel.")]
    [SerializeField] private int linesPerClick = 2;

    [Header("Scene Transition")]
    [SerializeField] private string nextSceneName = "MainScene";
    [SerializeField] private float delayBeforeSceneChange = 2f;

    [Header("Audio (Optional)")]
    [SerializeField] private AudioSource typingSound;

    private string fullText;
    private int _revealedLength;
    private bool isAnimating;
    private bool _animationComplete;

    void Awake()
    {
        if (textComponent == null)
            textComponent = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        fullText = textComponent.text;
        textComponent.text = "";
        _revealedLength = 0;
        _animationComplete = false;
        StartTypewriter();
        StartCoroutine(ShowButtonAfterDelay());
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        if (_animationComplete)
        {
            LoadNextScene();
            return;
        }

        AdvanceByLines(linesPerClick);
    }

    IEnumerator ShowButtonAfterDelay()
    {
        yield return new WaitForSecondsRealtime(5f);
        if (button != null)
            button.SetActive(true);
    }

    public void StartTypewriter()
    {
        if (isAnimating) return;
        isAnimating = true;
        StartCoroutine(TypewriterRoutine());
    }

    private IEnumerator TypewriterRoutine()
    {
        while (_revealedLength < fullText.Length)
        {
            textComponent.text = fullText.Substring(0, _revealedLength);

            char c = fullText[_revealedLength];
            if (typingSound != null && c != ' ' && c != '\n')
                typingSound.Play();

            if (c == '\n')
                yield return new WaitForSeconds(lineDelay);
            else
                yield return new WaitForSeconds(characterDelay);

            _revealedLength++;
        }

        textComponent.text = fullText;
        isAnimating = false;
        _animationComplete = true;
        OnAnimationComplete();
    }

    /// <summary>Reveal the next N lines on left click.</summary>
    private void AdvanceByLines(int lineCount)
    {
        if (_revealedLength >= fullText.Length) return;

        int newlinesSeen = 0;
        int pos = _revealedLength;
        while (pos < fullText.Length)
        {
            if (fullText[pos] == '\n')
            {
                newlinesSeen++;
                if (newlinesSeen >= lineCount)
                {
                    pos++;
                    break;
                }
            }
            pos++;
        }

        _revealedLength = Mathf.Min(pos, fullText.Length);
        textComponent.text = fullText.Substring(0, _revealedLength);

        if (_revealedLength >= fullText.Length)
        {
            isAnimating = false;
            _animationComplete = true;
            StopAllCoroutines();
            OnAnimationComplete();
        }
    }

    private void OnAnimationComplete()
    {
        if (button != null)
        {
            button.SetActive(true);
            var btnText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
                btnText.text = "Press Here to Continue";
        }
    }

    public void LoadNextScene()
    {
        if (string.IsNullOrEmpty(nextSceneName)) return;
        SceneManager.LoadScene(nextSceneName);
    }

    public void SkipAnimation()
    {
        StopAllCoroutines();
        textComponent.text = fullText;
        _revealedLength = fullText.Length;
        isAnimating = false;
        _animationComplete = true;
        OnAnimationComplete();
        LoadNextScene();
    }
}
