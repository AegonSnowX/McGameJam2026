using UnityEngine;

public class MicrophoneInput : MonoBehaviour
{
    public static MicrophoneInput Instance { get; private set; }

    [Header("Microphone Settings")]
    [SerializeField] private int sampleRate = 44100;
    [SerializeField] private int sampleWindow = 128;
    
    [Header("Noise Detection")]
    [SerializeField] private float sensitivity = 100f;
    [SerializeField] private float noiseThreshold = 0.01f;
    [SerializeField, Range(0f, 1f)] private float smoothing = 0.5f;

    private AudioClip microphoneClip;
    private string microphoneName;
    private float[] sampleData;
    private float currentNoiseLevel;
    private float smoothedNoiseLevel;
    private bool isMicrophoneActive;

    /// <summary>
    /// Current raw noise level (0-1)
    /// </summary>
    public float NoiseLevel => smoothedNoiseLevel;
    
    /// <summary>
    /// Returns true if noise is above threshold
    /// </summary>
    public bool IsLoud => smoothedNoiseLevel > noiseThreshold;
    
    /// <summary>
    /// Returns true if microphone is working
    /// </summary>
    public bool IsMicrophoneActive => isMicrophoneActive;

    void Awake()
    {
        // Always set instance to the new one (handles scene reloads)
        Instance = this;
        sampleData = new float[sampleWindow];
    }

    void OnDestroy()
    {
          StopMicrophone();
        // Clear instance when destroyed
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void Start()
    {
        StartMicrophone();
    }

    void Update()
    {
        if (isMicrophoneActive)
        {
            currentNoiseLevel = GetMicrophoneLoudness();
            // Smooth the noise level to avoid jittery values
            smoothedNoiseLevel = Mathf.Lerp(smoothedNoiseLevel, currentNoiseLevel, 1f - smoothing);
        }
    }


    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && !isMicrophoneActive)
        {
            StartMicrophone();
        }
    }

    public void StartMicrophone()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogWarning("MicrophoneInput: No microphone detected!");
            return;
        }

        microphoneName = Microphone.devices[0];
        microphoneClip = Microphone.Start(microphoneName, true, 1, sampleRate);
        isMicrophoneActive = true;
        
        Debug.Log($"MicrophoneInput: Started listening on '{microphoneName}'");
    }

    public void StopMicrophone()
    {
        if (isMicrophoneActive && !string.IsNullOrEmpty(microphoneName))
        {
            Microphone.End(microphoneName);
            isMicrophoneActive = false;
            Debug.Log("MicrophoneInput: Stopped listening");
        }
    }

    private float GetMicrophoneLoudness()
    {
        if (microphoneClip == null) return 0f;

        // Get the current position in the microphone buffer
        int micPosition = Microphone.GetPosition(microphoneName);
        if (micPosition < sampleWindow) return 0f;

        // Read samples from the microphone clip
        microphoneClip.GetData(sampleData, micPosition - sampleWindow);

        // Calculate RMS (Root Mean Square) for loudness
        float sum = 0f;
        for (int i = 0; i < sampleWindow; i++)
        {
            sum += sampleData[i] * sampleData[i];
        }
        float rms = Mathf.Sqrt(sum / sampleWindow);

        // Apply sensitivity and clamp
        return Mathf.Clamp01(rms * sensitivity);
    }
}
