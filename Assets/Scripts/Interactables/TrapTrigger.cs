using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrapTrigger : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] private AudioClip trapClip;
    [SerializeField] private float soundVolume = 1f;
    [SerializeField] private float soundAttractDuration = 5f;

    [Header("Animation")]
    [SerializeField] private GameObject trapAnimationObject;

    [Header("Optional")]
    [SerializeField] private bool oneShot = true;

    private Collider2D _col;
    private bool _triggered;
    private Animation _animation;

    void Awake()
    {
        _col = GetComponent<Collider2D>();
        if (_col != null)
            _col.isTrigger = true;

        // Cache the Animation component
        if (trapAnimationObject != null)
        {
            _animation = trapAnimationObject.GetComponent<Animation>();
            if (_animation == null)
            {
                Debug.LogError("[TrapTrigger] " + gameObject.name + ": No Animation component found on trap animation object!", this);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (oneShot && _triggered) return;

        _triggered = true;
        
        Debug.Log("[TrapTrigger] " + gameObject.name + ": TRIGGERED by player!", this);

        // Play trap animation
        PlayTrapAnimation();

        // Play trap sound
        PlayTrapSound();

        // Notify TrapSoundManager
        if (TrapSoundManager.Instance != null)
        {
            TrapSoundManager.Instance.ActivateSound(transform.position, soundAttractDuration);
        }
    }

    private void PlayTrapAnimation()
    {
        if (trapAnimationObject == null)
        {
            Debug.LogWarning("[TrapTrigger] " + gameObject.name + ": No trap animation object assigned.", this);
            return;
        }

        // Make sure the object is active
        if (!trapAnimationObject.activeInHierarchy)
        {
            trapAnimationObject.SetActive(true);
        }

        // Play the animation
        if (_animation != null)
        {
            _animation.Play();
            Debug.Log("[TrapTrigger] " + gameObject.name + ": Playing animation on " + trapAnimationObject.name, this);
        }
        else
        {
            // Try to get it again in case it wasn't ready
            _animation = trapAnimationObject.GetComponent<Animation>();
            if (_animation != null)
            {
                _animation.Play();
                Debug.Log("[TrapTrigger] " + gameObject.name + ": Playing animation on " + trapAnimationObject.name, this);
            }
            else
            {
                Debug.LogError("[TrapTrigger] " + gameObject.name + ": Cannot find Animation component!", this);
            }
        }
    }

    private void PlayTrapSound()
    {
        if (trapClip == null)
        {
            Debug.LogWarning("[TrapTrigger] " + gameObject.name + ": No trap clip assigned.", this);
            return;
        }

        // Create a temporary AudioSource to play the sound
        GameObject tempAudio = new GameObject("TrapSound_Temp");
        tempAudio.transform.position = transform.position;
        
        AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
        audioSource.clip = trapClip;
        audioSource.volume = soundVolume;
        audioSource.spatialBlend = 0f; // 2D sound
        audioSource.Play();
        
        // Destroy after clip finishes
        Destroy(tempAudio, trapClip.length + 0.1f);
        
        Debug.Log("[TrapTrigger] " + gameObject.name + ": Playing sound " + trapClip.name, this);
    }
}
