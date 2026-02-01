using UnityEngine;

/// <summary>
/// Heartbeat sound that gets faster/louder when the player is loud (microphone).
/// Pairs with TorchEffects flicker: same noise threshold. Add to player or a persistent object.
/// </summary>
public class HeartbeatSound : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] private AudioSource heartbeatSource;
    [SerializeField] private AudioClip heartbeatClip;

    [Header("Noise (match TorchEffects threshold)")]
    [SerializeField, Range(0f, 1f)] private float noiseThreshold = 0.2f;
    [SerializeField] private float volumeLerpSpeed = 3f;
    [SerializeField] private float pitchMin = 0.8f;
    [SerializeField] private float pitchMax = 1.5f;

    private float _targetVolume;
    private float _currentVolume;

    void Awake()
    {
        if (heartbeatSource == null)
            heartbeatSource = GetComponent<AudioSource>();
        if (heartbeatSource != null)
        {
            heartbeatSource.loop = true;
            heartbeatSource.clip = heartbeatClip;
            heartbeatSource.volume = 0f;
            heartbeatSource.playOnAwake = false;
        }
    }

    void Update()
    {
        if (heartbeatSource == null || heartbeatClip == null) return;

        float noiseLevel = MicrophoneInput.Instance != null ? MicrophoneInput.Instance.NoiseLevel : 0f;

        if (noiseLevel > noiseThreshold)
        {
            float t = (noiseLevel - noiseThreshold) / (1f - noiseThreshold);
            _targetVolume = Mathf.Clamp01(t * 1.2f);
            heartbeatSource.pitch = Mathf.Lerp(pitchMin, pitchMax, t);
            if (!heartbeatSource.isPlaying)
                heartbeatSource.Play();
        }
        else
        {
            _targetVolume = 0f;
        }

        _currentVolume = Mathf.MoveTowards(_currentVolume, _targetVolume, volumeLerpSpeed * Time.deltaTime);
        heartbeatSource.volume = _currentVolume;

        if (_currentVolume <= 0.01f && heartbeatSource.isPlaying)
            heartbeatSource.Stop();
    }
}
