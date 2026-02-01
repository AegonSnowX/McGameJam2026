using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WinTrigger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string playerTag = "Player";

    private Collider2D _col;
    private bool _triggered;

    void Awake()
    {
        _col = GetComponent<Collider2D>();
        if (_col != null)
        {
            _col.isTrigger = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_triggered) return;
        if (!other.CompareTag(playerTag)) return;

        _triggered = true;
        
        Debug.Log("[WinTrigger] Player reached the goal! Triggering win.", this);

        // Trigger win through GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerWin();
        }
        else
        {
            Debug.LogError("[WinTrigger] GameManager.Instance is null!", this);
        }
    }
}
