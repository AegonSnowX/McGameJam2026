using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ClueInteractable : MonoBehaviour, IInteractable
{
    [Header("Clue UI")]
    [SerializeField] private GameObject clueCanvas;
    [SerializeField] private string clueText = "Clue text here.";
    [SerializeField] private bool destroyAfterClose = true;

    private Collider2D _col;
    private bool _used;

    void Awake()
    {
        _col = GetComponent<Collider2D>();
        if (_col != null)
            _col.isTrigger = true;

        if (clueCanvas != null)
            clueCanvas.SetActive(false);
    }

    public void Interact()
    {
        if (_used) return;

        _used = true;

        if (clueCanvas != null)
        {
            clueCanvas.SetActive(true);
            var clueUI = clueCanvas.GetComponent<ClueUI>() ?? clueCanvas.GetComponentInChildren<ClueUI>();
            if (clueUI != null)
                clueUI.Setup(clueText, OnClueClosed);
        }
        else
        {
            OnClueClosed();
        }
    }

    private void OnClueClosed()
    {
        if (clueCanvas != null)
            clueCanvas.SetActive(false);

        if (destroyAfterClose)
            Destroy(gameObject);
    }
}
