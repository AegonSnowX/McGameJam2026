using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Attach to the player. Tracks interactables in range and calls Interact() when the player presses E.
/// </summary>
public class PlayerInteractor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private readonly List<IInteractable> _inRange = new List<IInteractable>();

    void Update()
    {
        if (PlayerMovement.Instance != null && PlayerMovement.Instance.IsDead)
            return;

        if (!Input.GetKeyDown(interactKey))
            return;

        IInteractable nearest = GetNearestInteractable();
        if (nearest != null)
        {
            var mb = nearest as MonoBehaviour;
            string name = (mb != null) ? mb.gameObject.name : "?";
            Debug.Log("[PlayerInteractor] " + interactKey + " pressed, " + _inRange.Count + " in range, interacting with '" + name + "'.", this);
            nearest.Interact();
        }
        else
            Debug.Log("[PlayerInteractor] " + interactKey + " pressed, no interactable in range (count=" + _inRange.Count + ").", this);
    }

    private IInteractable GetNearestInteractable()
    {
        if (_inRange.Count == 0) return null;
        if (_inRange.Count == 1) return _inRange[0];

        IInteractable nearest = null;
        float nearestSq = float.MaxValue;
        Vector3 pos = transform.position;

        for (int i = _inRange.Count - 1; i >= 0; i--)
        {
            var item = _inRange[i];
            var itemMb = item as MonoBehaviour;
            if (item == null || itemMb == null)
            {
                _inRange.RemoveAt(i);
                continue;
            }

            Vector3 diff = itemMb.transform.position - pos;
            float sqMag = diff.sqrMagnitude;
            if (sqMag < nearestSq)
            {
                nearestSq = sqMag;
                nearest = item;
            }
        }

        return nearest;
    }

    public void RegisterInteractable(IInteractable interactable)
    {
        if (interactable != null && !_inRange.Contains(interactable))
            _inRange.Add(interactable);
    }

    public void UnregisterInteractable(IInteractable interactable)
    {
        _inRange.Remove(interactable);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var interactable = other.GetComponent<IInteractable>();
        if (interactable == null)
            interactable = other.GetComponentInParent<IInteractable>();
        if (interactable != null)
        {
            var mb = interactable as MonoBehaviour;
            string name = (mb != null) ? mb.gameObject.name : "?";
            Debug.Log("[PlayerInteractor] Entered trigger of '" + other.gameObject.name + "', IInteractable found on " + name + ", registered. InRange count=" + (_inRange.Count + 1) + ".", this);
            RegisterInteractable(interactable);
        }
        else
            Debug.Log("[PlayerInteractor] Entered trigger of '" + other.gameObject.name + "' (tag=" + other.tag + "), no IInteractable on this object or parent.", this);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var interactable = other.GetComponent<IInteractable>();
        if (interactable == null)
            interactable = other.GetComponentInParent<IInteractable>();
        if (interactable != null)
        {
            var mb = interactable as MonoBehaviour;
            string name = (mb != null) ? mb.gameObject.name : "?";
            Debug.Log("[PlayerInteractor] Exited trigger of '" + other.gameObject.name + "', unregistered IInteractable from " + name + ".", this);
            UnregisterInteractable(interactable);
        }
    }
}
