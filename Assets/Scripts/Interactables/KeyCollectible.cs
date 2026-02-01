using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class KeyCollectible : MonoBehaviour
{
    [Header("Optional")]
    [SerializeField] private AudioSource collectSound;
    [SerializeField] private GameObject visualToHideOnCollect;

    private Collider2D _col;
    private bool _collected;

    void Awake()
    {
        _col = GetComponent<Collider2D>();
        if (_col != null)
            _col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HandleTrigger(other);
    }

    /// <summary>
    /// Call this from a child's KeyTriggerForwarder if the trigger collider is on a child object.
    /// </summary>
    public void HandleTrigger(Collider2D other)
    {
        if (_collected) return;

        // Detect player by component so it works even without "Player" tag
        var player = other.GetComponent<PlayerMovement>() ?? other.GetComponentInParent<PlayerMovement>();
        if (player == null) return;

        _collected = true;

        if (collectSound != null)
            collectSound.Play();

        if (visualToHideOnCollect != null)
            visualToHideOnCollect.SetActive(false);

        if (GameManager.Instance != null)
            GameManager.Instance.AddKey();
        else
            Debug.LogWarning("KeyCollectible: No GameManager in scene. Key collected but win condition will not update.");

        float destroyDelay = 0.1f;
        if (collectSound != null && collectSound.clip != null)
            destroyDelay = collectSound.clip.length + 0.1f;
        Destroy(gameObject, destroyDelay);
    }
}
