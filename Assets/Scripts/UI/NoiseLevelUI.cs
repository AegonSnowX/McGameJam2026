using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Shows the current microphone noise level on a slider so the player can see if they're making noise.
/// Assign a Slider (and optional label). The slider should be non-interactable so it's read-only.
/// Color changes based on danger level: Green (safe) → Orange (warning) → Red (ghost chasing)
/// </summary>
public class NoiseLevelUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider noiseSlider;
    [SerializeField] private Image sliderFill;
    [SerializeField] private TextMeshProUGUI noiseLabel;

    [Header("Threshold Settings")]
    [Tooltip("Below this level = Green (safe)")]
    [SerializeField, Range(0f, 1f)] private float safeThreshold = 0.2f;
    [Tooltip("Above safe but below this = Orange (warning). Above this = Red (danger)")]
    [SerializeField, Range(0f, 1f)] private float dangerThreshold = 0.5f;

    [Header("Colors")]
    [SerializeField] private Color safeColor = new Color(0.2f, 0.8f, 0.2f); // Green
    [SerializeField] private Color warningColor = new Color(1f, 0.6f, 0f);  // Orange
    [SerializeField] private Color dangerColor = new Color(0.9f, 0.2f, 0.2f); // Red

    [Header("Settings")]
    [SerializeField] private string labelFormat = "Noise: {0:P0}";
    [SerializeField] private float updateInterval = 0.05f;
    [SerializeField] private float colorLerpSpeed = 5f;

    private float _nextUpdate;
    private Color _targetColor;
    private Color _currentColor;

    void Start()
    {
        if (noiseSlider != null)
        {
            noiseSlider.minValue = 0f;
            noiseSlider.maxValue = 1f;
            noiseSlider.value = 0f;
            noiseSlider.interactable = false;

            // Auto-find fill image if not assigned
            if (sliderFill == null)
            {
                Transform fillArea = noiseSlider.transform.Find("Fill Area");
                if (fillArea != null)
                {
                    Transform fill = fillArea.Find("Fill");
                    if (fill != null)
                    {
                        sliderFill = fill.GetComponent<Image>();
                    }
                }
            }
        }

        _currentColor = safeColor;
        _targetColor = safeColor;
        
        if (sliderFill != null)
        {
            sliderFill.color = safeColor;
        }
    }

    void Update()
    {
        if (Time.unscaledTime < _nextUpdate) return;
        _nextUpdate = Time.unscaledTime + updateInterval;

        float level = 0f;
        if (MicrophoneInput.Instance != null)
            level = MicrophoneInput.Instance.NoiseLevel;

        if (noiseSlider != null)
            noiseSlider.value = level;

        if (noiseLabel != null)
            noiseLabel.text = string.Format(labelFormat, level);

        // Update color based on noise level
        UpdateSliderColor(level);
    }

    private void UpdateSliderColor(float noiseLevel)
    {
        if (sliderFill == null) return;

        // Determine target color based on threshold
        if (noiseLevel < safeThreshold)
        {
            // Safe zone - Green
            _targetColor = safeColor;
        }
        else if (noiseLevel < dangerThreshold)
        {
            // Warning zone - Lerp between Green and Orange
            float t = (noiseLevel - safeThreshold) / (dangerThreshold - safeThreshold);
            _targetColor = Color.Lerp(safeColor, warningColor, t);
        }
        else
        {
            // Danger zone - Lerp between Orange and Red
            float t = Mathf.Clamp01((noiseLevel - dangerThreshold) / (1f - dangerThreshold));
            _targetColor = Color.Lerp(warningColor, dangerColor, t);
        }

        // Smoothly transition to target color
        _currentColor = Color.Lerp(_currentColor, _targetColor, colorLerpSpeed * Time.unscaledDeltaTime);
        sliderFill.color = _currentColor;
    }
}
