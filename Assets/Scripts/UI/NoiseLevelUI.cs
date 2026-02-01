using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Shows the current microphone noise level on a slider so the player can see if they're making noise.
/// Assign a Slider (and optional label). The slider should be non-interactable so it's read-only.
/// </summary>
public class NoiseLevelUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider noiseSlider;
    [SerializeField] private TextMeshProUGUI noiseLabel;

    [Header("Settings")]
    [SerializeField] private string labelFormat = "Noise: {0:P0}";
    [SerializeField] private float updateInterval = 0.05f;

    private float _nextUpdate;

    void Start()
    {
        if (noiseSlider != null)
        {
            noiseSlider.minValue = 0f;
            noiseSlider.maxValue = 1f;
            noiseSlider.value = 0f;
            noiseSlider.interactable = false;
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
    }
}
