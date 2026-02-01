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
            nearest.Interact();
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
            if (item == null || (item is MonoBehaviour mb && mb == null))
            {
                _inRange.RemoveAt(i);
                continue;
            }

            Vector3 diff = ((MonoBehaviour)item).transform.position - pos;
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
            RegisterInteractable(interactable);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var interactable = other.GetComponent<IInteractable>();
        if (interactable == null)
            interactable = other.GetComponentInParent<IInteractable>();
        if (interactable != null)
            UnregisterInteractable(interactable);
    }
}
