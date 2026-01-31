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
        if (_collected) return;
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance == null) return;

        _collected = true;

        if (collectSound != null)
            collectSound.Play();

        if (visualToHideOnCollect != null)
            visualToHideOnCollect.SetActive(false);

        GameManager.Instance.AddKey();

        // Destroy after a short delay so sound can play
        float destroyDelay = 0.1f;
        if (collectSound != null && collectSound.clip != null)
            destroyDelay = collectSound.clip.length + 0.1f;
        Destroy(gameObject, destroyDelay);
    }
}
