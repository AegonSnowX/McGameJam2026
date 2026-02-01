using UnityEngine;

public class TextScrollUp : MonoBehaviour
{
    [Header("Text Panel")]
    [SerializeField] private RectTransform textPanel;

    [Header("Scroll Settings")]
    [SerializeField] private float scrollSpeed = 50f;
    [SerializeField] private float startY = -500f;
    [SerializeField] private float endY = 1000f;
    [SerializeField] private bool loop = false;
    [SerializeField] private bool startOnAwake = true;

    private bool isScrolling;
    private Vector2 currentPosition;

    void Awake()
    {
        if (textPanel == null)
        {
            textPanel = GetComponent<RectTransform>();
        }
    }

    void Start()
    {
        // Set starting position
        currentPosition = textPanel.anchoredPosition;
        currentPosition.y = startY;
        textPanel.anchoredPosition = currentPosition;

        if (startOnAwake)
        {
            StartScrolling();
        }
    }

    void Update()
    {
        if (!isScrolling) return;

        // Move text upward
        currentPosition = textPanel.anchoredPosition;
        currentPosition.y += scrollSpeed * Time.deltaTime;
        textPanel.anchoredPosition = currentPosition;

        // Check if reached end
        if (currentPosition.y >= endY)
        {
            if (loop)
            {
                // Reset to start position for looping
                currentPosition.y = startY;
                textPanel.anchoredPosition = currentPosition;
            }
            else
            {
                // Stop scrolling
                isScrolling = false;
                OnScrollComplete();
            }
        }
    }

    public void StartScrolling()
    {
        isScrolling = true;
    }

    public void StopScrolling()
    {
        isScrolling = false;
    }

    public void ResetAndStart()
    {
        currentPosition = textPanel.anchoredPosition;
        currentPosition.y = startY;
        textPanel.anchoredPosition = currentPosition;
        StartScrolling();
    }

    // Override this or add UnityEvent if you need callback when scroll finishes
    protected virtual void OnScrollComplete()
    {
        Debug.Log("Text scroll complete!");
    }
}
