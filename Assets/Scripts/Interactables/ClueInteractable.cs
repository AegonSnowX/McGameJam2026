using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class ClueInteractable : MonoBehaviour, IInteractable
{
    [Header("Clue UI")]
    [SerializeField] private GameObject clueCanvas;
    [SerializeField] private string clueText = "Clue text here.";
    [SerializeField] private bool destroyAfterClose = true;

    [Header("In-range visuals (show when player can interact)")]
    [SerializeField] private GameObject outlineVisual;
    [SerializeField] private GameObject pressEPrompt;
    [SerializeField] private string pressEText = "Press E";

    private Collider2D _col;
    private bool _used;

    void Awake()
    {
        _col = GetComponent<Collider2D>();
        if (_col != null)
            _col.isTrigger = true;

        if (clueCanvas != null)
            clueCanvas.SetActive(false);

        SetInRangeVisuals(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_used) return;
        if (!IsPlayer(other)) return;

        SetInRangeVisuals(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;

        SetInRangeVisuals(false);
    }

    private bool IsPlayer(Collider2D other)
    {
        if (other.CompareTag("Player")) return true;
        return other.GetComponent<PlayerMovement>() != null || other.GetComponentInParent<PlayerMovement>() != null;
    }

    private void SetInRangeVisuals(bool inRange)
    {
        if (outlineVisual != null)
            outlineVisual.SetActive(inRange);

        if (pressEPrompt != null)
        {
            pressEPrompt.SetActive(inRange);
            var tmp = pressEPrompt.GetComponent<TMP_Text>() ?? pressEPrompt.GetComponentInChildren<TMP_Text>(true);
            if (tmp != null && !string.IsNullOrEmpty(pressEText))
                tmp.text = pressEText;
        }
    }

    public void Interact()
    {
        if (_used) return;

        _used = true;
        SetInRangeVisuals(false);

        if (clueCanvas != null)
        {
            clueCanvas.SetActive(true);
            var clueUI = clueCanvas.GetComponent<ClueUI>() ?? clueCanvas.GetComponentInChildren<ClueUI>(true);
            if (clueUI != null)
                clueUI.Setup(clueText, OnClueClosed, gameObject);
            else
                Debug.LogWarning("ClueInteractable: No ClueUI found on clueCanvas. Add ClueUI to the canvas and assign the Close button.");
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
