using UnityEngine;

/// <summary>
/// Attach this to a CHILD object that has the trigger Collider2D.
/// When the player touches that child, the parent's KeyCollectible will still receive the collect.
/// Use this when your key visual/collider is on a child and the KeyCollectible script is on the parent.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class KeyTriggerForwarder : MonoBehaviour
{
    void Awake()
    {
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var key = GetComponentInParent<KeyCollectible>();
        if (key != null)
            key.HandleTrigger(other);
    }
}
