using UnityEngine;

/// <summary>
/// Put this on a child that has the trigger Collider2D. When the player enters that trigger,
/// it notifies the parent's NoiseDoor so the door still opens. Use when the trigger collider
/// must be on a child (e.g. the bookshelf visual) instead of the same object as NoiseDoor.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class NoiseDoorTriggerForwarder : MonoBehaviour
{
    private NoiseDoor _noiseDoor;

    void Awake()
    {
        _noiseDoor = GetComponentInParent<NoiseDoor>();
        if (_noiseDoor == null)
            Debug.LogWarning("[NoiseDoorTriggerForwarder] " + gameObject.name + ": No NoiseDoor found on parent.", this);
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_noiseDoor != null)
            _noiseDoor.OnPlayerEnterTrigger(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (_noiseDoor != null)
            _noiseDoor.OnPlayerExitTrigger(other);
    }
}
