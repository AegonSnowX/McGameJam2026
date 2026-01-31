using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TrapTrigger : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] private AudioSource trapSound;
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

        if (trapSound != null)
            trapSound.Play();

        if (TrapSoundManager.Instance != null)
            TrapSoundManager.Instance.ActivateSound(transform.position, soundAttractDuration);
    }
}
