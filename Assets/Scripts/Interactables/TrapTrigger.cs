using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrapTrigger : MonoBehaviour
{
    [Header("Sound")]
    [Tooltip("Use AudioSource (its clip) or assign Trap Clip so sound keeps playing even if trap is destroyed.")]
    [SerializeField] private AudioSource trapSound;
    [SerializeField] private AudioClip trapClip;
    [SerializeField] private float soundAttractDuration = 5f;

    [Header("Optional")]
    [SerializeField] private bool oneShot = true;

    private Collider2D _col;
    private bool _triggered;

    void Awake()
    {
        _col = GetComponent<Collider2D>();
        if (_col != null)
            _col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (oneShot && _triggered) return;

        _triggered = true;

        // Play trap sound via PlayClipAtPoint so it keeps playing even if trap is destroyed
        AudioClip clipToPlay = (trapSound != null && trapSound.clip != null) ? trapSound.clip : trapClip;
        if (clipToPlay != null)
        {
            AudioSource.PlayClipAtPoint(clipToPlay, transform.position);
            Debug.Log("[TrapTrigger] " + gameObject.name + ": Playing trap sound at " + transform.position + ".", this);
        }
        else
            Debug.LogWarning("[TrapTrigger] " + gameObject.name + ": No trap sound clip assigned (trapSound=" + (trapSound != null ? "assigned" : "null") + ", trapClip=" + (trapClip != null ? "assigned" : "null") + ").", this);

        if (TrapSoundManager.Instance != null)
            TrapSoundManager.Instance.ActivateSound(transform.position, soundAttractDuration);
        else
            Debug.LogWarning("[TrapTrigger] " + gameObject.name + ": TrapSoundManager.Instance is null.", this);
    }
}
