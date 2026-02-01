using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Door that opens when the player stays in range and keeps talking: average microphone noise
/// must be at least the threshold (e.g. 80%) over the required duration (e.g. 10 seconds).
/// Plays the door open animation and enables the secret room when opened.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class NoiseDoor : MonoBehaviour
{
    [Header("Noise requirement")]
    [Tooltip("Average noise over the duration must be at least this (0–1). e.g. 0.8 = 80%.")]
    [SerializeField, Range(0f, 1f)] private float requiredMeanNoise = 0.8f;
    [Tooltip("Seconds of sustained talking to consider. e.g. 10 = mean over last 10 seconds.")]
    [SerializeField] private float requiredDuration = 10f;
    [Tooltip("How often we sample the microphone (seconds).")]
    [SerializeField] private float sampleInterval = 0.1f;

    [Header("Door")]
    [SerializeField] private Animator doorAnimator;
    [Tooltip("Animator trigger name to open the door. Leave empty if using Animation component.")]
    [SerializeField] private string openTriggerName = "Open";
    [SerializeField] private Animation doorAnimation;

    [Header("Secret room")]
    [SerializeField] private GameObject secretRoom;
    [Tooltip("Optional: set inactive when noise condition is met and secret level opens (e.g. hint, blocker).")]
    [SerializeField] private GameObject setInactiveWhenOpened;

    [Header("Optional")]
    [SerializeField] private string playerTag = "Player";

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    private Collider2D _col;
    private bool _playerInRange;
    private bool _opened;
    private float _nextSampleTime;
    private readonly List<float> _noiseSamples = new List<float>();
    private int _sampleCountForDuration;

    void Awake()
    {
        _col = GetComponent<Collider2D>();
        if (_col != null)
            _col.isTrigger = true;
        else if (debugLogs)
            Debug.LogWarning("[NoiseDoor] " + gameObject.name + ": No Collider2D on this object. Add a Collider2D and set Is Trigger. Trigger must be on the SAME GameObject as NoiseDoor.", this);
        _sampleCountForDuration = Mathf.Max(1, Mathf.RoundToInt(requiredDuration / sampleInterval));
        if (debugLogs)
            Debug.Log("[NoiseDoor] " + gameObject.name + ": Need " + _sampleCountForDuration + " samples (every " + sampleInterval + "s) = " + requiredDuration + "s. Mean must be >= " + requiredMeanNoise + ".", this);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_opened) return;
        if (!IsPlayer(other))
        {
            if (debugLogs)
                Debug.Log("[NoiseDoor] " + gameObject.name + ": Something entered trigger: " + other.gameObject.name + " (tag=" + other.tag + ") - not player.", this);
            return;
        }
        _playerInRange = true;
        if (debugLogs)
            Debug.Log("[NoiseDoor] " + gameObject.name + ": Player entered trigger. Start talking for " + requiredDuration + "s (mean >= " + requiredMeanNoise + ").", this);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;
        OnPlayerExitTrigger(other);
    }

    /// <summary>Call from NoiseDoorTriggerForwarder when trigger is on a child.</summary>
    public void OnPlayerEnterTrigger(Collider2D other)
    {
        if (_opened) return;
        if (!IsPlayer(other)) return;
        _playerInRange = true;
        if (debugLogs)
            Debug.Log("[NoiseDoor] " + gameObject.name + ": Player entered trigger (via forwarder). Start talking for " + requiredDuration + "s.", this);
    }

    /// <summary>Call from NoiseDoorTriggerForwarder when trigger is on a child.</summary>
    public void OnPlayerExitTrigger(Collider2D other)
    {
        if (!IsPlayer(other)) return;
        _playerInRange = false;
        _noiseSamples.Clear();
        _nextSampleTime = 0f;
        if (debugLogs)
            Debug.Log("[NoiseDoor] " + gameObject.name + ": Player left trigger. Samples cleared.", this);
    }

    private bool IsPlayer(Collider2D other)
    {
        if (other.CompareTag(playerTag)) return true;
        return other.GetComponent<PlayerMovement>() != null || other.GetComponentInParent<PlayerMovement>() != null;
    }

    private float _lastLogTime;

    void Update()
    {
        if (_opened) return;
        if (!_playerInRange) return;
        if (MicrophoneInput.Instance == null)
        {
            if (debugLogs && Time.time - _lastLogTime > 2f)
            {
                _lastLogTime = Time.time;
                Debug.LogWarning("[NoiseDoor] " + gameObject.name + ": MicrophoneInput.Instance is null. Is MicrophoneInput in the scene?", this);
            }
            return;
        }

        float now = Time.time;
        if (now < _nextSampleTime) return;
        _nextSampleTime = now + sampleInterval;

        float noise = MicrophoneInput.Instance.NoiseLevel;
        _noiseSamples.Add(noise);
        while (_noiseSamples.Count > _sampleCountForDuration)
            _noiseSamples.RemoveAt(0);

        if (debugLogs && _noiseSamples.Count > 0 && Time.time - _lastLogTime > 1f)
        {
            _lastLogTime = Time.time;
            float sum = 0f;
            for (int i = 0; i < _noiseSamples.Count; i++)
                sum += _noiseSamples[i];
            float currentMean = sum / _noiseSamples.Count;
            Debug.Log("[NoiseDoor] " + gameObject.name + ": In range. Samples=" + _noiseSamples.Count + "/" + _sampleCountForDuration + ", current mean=" + currentMean.ToString("F2") + ", need >= " + requiredMeanNoise + ", latest noise=" + noise.ToString("F2") + ".", this);
        }

        if (_noiseSamples.Count < _sampleCountForDuration) return;

        float sumTotal = 0f;
        for (int i = 0; i < _noiseSamples.Count; i++)
            sumTotal += _noiseSamples[i];
        float mean = sumTotal / _noiseSamples.Count;

        if (mean >= requiredMeanNoise)
            OpenDoor();
    }

    private void OpenDoor()
    {
        if (_opened) return;
        _opened = true;

        if (debugLogs)
            Debug.Log("[NoiseDoor] " + gameObject.name + ": Opening! Mean reached threshold.", this);

        if (doorAnimator != null && !string.IsNullOrEmpty(openTriggerName))
        {
            doorAnimator.SetTrigger(openTriggerName);
            if (debugLogs)
                Debug.Log("[NoiseDoor] " + gameObject.name + ": Set animator trigger '" + openTriggerName + "'.", this);
        }
        else if (doorAnimator == null && (doorAnimation == null || !doorAnimation.isPlaying))
        {
            if (debugLogs)
                Debug.LogWarning("[NoiseDoor] " + gameObject.name + ": No Door Animator (with trigger) or Door Animation assigned. Assign one so the bookshelf/door can move.", this);
        }
        if (doorAnimation != null)
        {
            doorAnimation.Play();
            if (debugLogs)
                Debug.Log("[NoiseDoor] " + gameObject.name + ": Playing door Animation.", this);
        }

        if (secretRoom != null)
        {
            secretRoom.SetActive(true);
            if (debugLogs)
                Debug.Log("[NoiseDoor] " + gameObject.name + ": Secret room enabled.", this);
        }
        else if (debugLogs)
            Debug.LogWarning("[NoiseDoor] " + gameObject.name + ": No Secret Room assigned. Assign the secret room GameObject.", this);

        if (setInactiveWhenOpened != null)
        {
            setInactiveWhenOpened.SetActive(false);
            if (debugLogs)
                Debug.Log("[NoiseDoor] " + gameObject.name + ": Set inactive when opened: " + setInactiveWhenOpened.name + ".", this);
        }
    }
}
