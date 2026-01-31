using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TorchEffects : MonoBehaviour
{
    [Header("Light Reference")]
    [SerializeField] private Light2D torchLight;
    
    [Header("Base Settings")]
    [SerializeField] private float baseIntensity = 1f;
    [SerializeField] private float baseOuterRadius = 5f;
    
    [Header("Noise Effects")]
    [SerializeField, Range(0f, 1f)] private float noiseThreshold = 0.2f;
    [SerializeField] private float minIntensityMultiplier = 0.3f;
    [SerializeField] private float intensityReduceSpeed = 2f;
    [SerializeField] private float intensityRecoverSpeed = 1f;
    
    [Header("Flicker Settings")]
    [SerializeField] private float flickerSpeed = 20f;
    [SerializeField] private float flickerIntensity = 0.3f;
    [SerializeField] private float flickerChance = 0.3f;
    
    [Header("Radius Effect")]
    [SerializeField] private float minRadiusMultiplier = 0.6f;

    private float currentIntensityMultiplier = 1f;
    private float flickerTimer;
    private float flickerOffset;
    private bool isFlickering;

    void Start()
    {
        if (torchLight == null)
        {
            torchLight = GetComponentInChildren<Light2D>();
        }
        
        if (torchLight != null)
        {
            baseIntensity = torchLight.intensity;
            baseOuterRadius = torchLight.pointLightOuterRadius;
        }
    }

    void Update()
    {
        if (torchLight == null) return;
        
        float noiseLevel = GetNoiseLevel();
        
        if (noiseLevel > noiseThreshold)
        {
            ApplyNoiseEffects(noiseLevel);
        }
        else
        {
            RecoverFromNoise();
        }
        
        ApplyLightSettings();
    }

    private float GetNoiseLevel()
    {
        if (MicrophoneInput.Instance == null) return 0f;
        return MicrophoneInput.Instance.NoiseLevel;
    }

    private void ApplyNoiseEffects(float noiseLevel)
    {
        // Reduce intensity based on noise level
        float targetMultiplier = Mathf.Lerp(1f, minIntensityMultiplier, (noiseLevel - noiseThreshold) / (1f - noiseThreshold));
        currentIntensityMultiplier = Mathf.Lerp(currentIntensityMultiplier, targetMultiplier, intensityReduceSpeed * Time.deltaTime);
        
        // Random flicker chance
        flickerTimer -= Time.deltaTime;
        if (flickerTimer <= 0f)
        {
            isFlickering = Random.value < flickerChance * noiseLevel;
            flickerTimer = Random.Range(0.05f, 0.15f);
            
            if (isFlickering)
            {
                flickerOffset = Random.Range(-flickerIntensity, flickerIntensity);
            }
        }
    }

    private void RecoverFromNoise()
    {
        // Gradually recover intensity
        currentIntensityMultiplier = Mathf.Lerp(currentIntensityMultiplier, 1f, intensityRecoverSpeed * Time.deltaTime);
        isFlickering = false;
        flickerOffset = Mathf.Lerp(flickerOffset, 0f, intensityRecoverSpeed * Time.deltaTime);
    }

    private void ApplyLightSettings()
    {
        // Apply intensity with flicker
        float finalIntensity = baseIntensity * currentIntensityMultiplier;
        
        if (isFlickering)
        {
            // Add rapid flicker effect
            float flicker = Mathf.Sin(Time.time * flickerSpeed) * flickerOffset;
            finalIntensity += flicker;
        }
        
        torchLight.intensity = Mathf.Max(0.1f, finalIntensity);
        
        // Also reduce radius slightly when intensity drops
        float radiusMultiplier = Mathf.Lerp(minRadiusMultiplier, 1f, currentIntensityMultiplier);
        torchLight.pointLightOuterRadius = baseOuterRadius * radiusMultiplier;
    }

    // Public method to manually trigger flicker (e.g., for jump scares)
    public void TriggerFlicker(float duration = 0.5f)
    {
        StartCoroutine(FlickerCoroutine(duration));
    }

    private System.Collections.IEnumerator FlickerCoroutine(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            isFlickering = true;
            flickerOffset = Random.Range(-flickerIntensity, flickerIntensity);
            elapsed += Time.deltaTime;
            yield return null;
        }
        isFlickering = false;
    }
}
