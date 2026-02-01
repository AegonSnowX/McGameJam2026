using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class KeyCollectible : MonoBehaviour
{
    [Header("Puzzle piece (which piece this key reveals)")]
    [Tooltip("1 = first piece, 2 = second, 3 = third. Must match the piece you assign in KeyProgressReveal.")]
    [SerializeField] private int keyIndex = 1;

    [Header("Optional")]
    [Tooltip("Use either: assign an AudioSource (its clip will play) or assign Collect Clip directly.")]
    [SerializeField] private AudioSource collectSound;
    [SerializeField] private AudioClip collectClip;
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

        // Play collect sound via PlayClipAtPoint so it keeps playing after the key is destroyed
        AudioClip clipToPlay = (collectSound != null && collectSound.clip != null) ? collectSound.clip : collectClip;
        if (clipToPlay != null)
            AudioSource.PlayClipAtPoint(clipToPlay, transform.position);

        if (visualToHideOnCollect != null)
            visualToHideOnCollect.SetActive(false);

        if (GameManager.Instance != null)
            GameManager.Instance.AddKey(Mathf.Clamp(keyIndex, 1, 3));
        else
            Debug.LogWarning("KeyCollectible: No GameManager in scene. Key collected but win condition will not update.");

        Destroy(gameObject, 0.05f);
    }
}
