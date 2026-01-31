using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }
    
    [SerializeField] private Transform target;

    // Dead zone size, switch later according to game design and world to find the sweet spot
    [SerializeField] private Vector2 deadZoneSize = new Vector2(1.5f, 1.0f);
    
    [Header("Noise Screen Shake")]
    [SerializeField, Range(0f, 1f)] private float noiseThreshold = 0.2f;
    [SerializeField] private float maxShakeIntensity = 0.1f;
    [SerializeField] private float shakeSpeed = 25f;
    [SerializeField] private float shakeRecoverSpeed = 2f;

    private Vector3 offset;
    private float currentShakeIntensity;
    private Vector3 shakeOffset;

    void Awake()
    {
        Instance = this;
        
        if (target == null) return;

        Vector3 pos = target.position;
        pos.z = transform.position.z; // preserve camera depth
        transform.position = pos;
    }

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("CameraFollow: Target not assigned.");
            enabled = false;
            return;
        }
 
        offset = transform.position - target.position;
    }

    void LateUpdate()
    {
        // Handle noise-based screen shake
        UpdateScreenShake();
        
        Vector3 targetPosition = target.position + offset;
        Vector3 cameraPosition = transform.position - shakeOffset; // Remove previous shake before calculating

        float deltaX = targetPosition.x - cameraPosition.x;
        float deltaY = targetPosition.y - cameraPosition.y;

        if (Mathf.Abs(deltaX) > deadZoneSize.x)
        {
            cameraPosition.x += deltaX - Mathf.Sign(deltaX) * deadZoneSize.x;
        }

        if (Mathf.Abs(deltaY) > deadZoneSize.y)
        {
            cameraPosition.y += deltaY - Mathf.Sign(deltaY) * deadZoneSize.y;
        }

        // Apply shake offset
        transform.position = cameraPosition + shakeOffset;
    }

    private void UpdateScreenShake()
    {
        float noiseLevel = GetNoiseLevel();
        
        if (noiseLevel > noiseThreshold)
        {
            // Scale shake intensity with noise level
            float targetShake = Mathf.Lerp(0f, maxShakeIntensity, (noiseLevel - noiseThreshold) / (1f - noiseThreshold));
            currentShakeIntensity = Mathf.Lerp(currentShakeIntensity, targetShake, Time.deltaTime * 5f);
        }
        else
        {
            // Recover from shake
            currentShakeIntensity = Mathf.Lerp(currentShakeIntensity, 0f, shakeRecoverSpeed * Time.deltaTime);
        }
        
        // Calculate shake offset using Perlin noise for smooth random movement
        if (currentShakeIntensity > 0.001f)
        {
            float time = Time.time * shakeSpeed;
            shakeOffset = new Vector3(
                (Mathf.PerlinNoise(time, 0f) - 0.5f) * 2f * currentShakeIntensity,
                (Mathf.PerlinNoise(0f, time) - 0.5f) * 2f * currentShakeIntensity,
                0f
            );
        }
        else
        {
            shakeOffset = Vector3.zero;
        }
    }

    private float GetNoiseLevel()
    {
        if (MicrophoneInput.Instance == null) return 0f;
        return MicrophoneInput.Instance.NoiseLevel;
    }

    // Public method to trigger manual screen shake (e.g., for impacts)
    public void TriggerShake(float intensity, float duration)
    {
        StartCoroutine(ShakeCoroutine(intensity, duration));
    }

    private System.Collections.IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        float originalIntensity = currentShakeIntensity;
        currentShakeIntensity = intensity;
        
        yield return new WaitForSeconds(duration);
        
        // Don't immediately reset if noise is still high
        if (GetNoiseLevel() <= noiseThreshold)
        {
            currentShakeIntensity = originalIntensity;
        }
    }
}
