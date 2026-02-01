using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class ClueInteractable : MonoBehaviour, IInteractable
{
    [Header("Clue UI")]
    [SerializeField] private GameObject clueCanvas;
    [SerializeField] private string clueText = "Clue text here.";
    [SerializeField] private bool destroyAfterClose = true;

    [Header("Sound")]
    [SerializeField] private AudioSource openSound;
    [SerializeField] private AudioSource closeSound;

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
        else
            Debug.LogWarning("[ClueInteractable] " + gameObject.name + ": No Collider2D found.", this);

        if (clueCanvas != null)
            clueCanvas.SetActive(false);
        else
            Debug.LogWarning("[ClueInteractable] " + gameObject.name + ": clueCanvas is not assigned.", this);

        SetInRangeVisuals(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_used)
        {
            Debug.Log("[ClueInteractable] " + gameObject.name + ": Player entered trigger but clue already used, ignoring.", this);
            return;
        }
        if (!IsPlayer(other))
        {
            Debug.Log("[ClueInteractable] " + gameObject.name + ": OnTriggerEnter2D from non-player '" + other.gameObject.name + "' (tag=" + other.tag + "), ignoring.", this);
            return;
        }
        Debug.Log("[ClueInteractable] " + gameObject.name + ": Player entered trigger, in range, press E to interact.", this);
        SetInRangeVisuals(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;
        Debug.Log("[ClueInteractable] " + gameObject.name + ": Player left trigger, out of range.", this);
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
        if (_used)
        {
            Debug.Log("[ClueInteractable] " + gameObject.name + ": Interact() called but already used, ignoring.", this);
            return;
        }

        Debug.Log("[ClueInteractable] " + gameObject.name + ": Interact() called, opening clue.", this);
        _used = true;
        SetInRangeVisuals(false);

        if (openSound != null)
            openSound.Play();

        if (clueCanvas != null)
        {
            clueCanvas.SetActive(true);
            var clueUI = clueCanvas.GetComponent<ClueUI>() ?? clueCanvas.GetComponentInChildren<ClueUI>(true);
            if (clueUI != null)
            {
                Debug.Log("[ClueInteractable] " + gameObject.name + ": ClueUI found on '" + clueUI.gameObject.name + "', calling Setup.", this);
                clueUI.Setup(clueText, OnClueClosed, gameObject);
            }
            else
                Debug.LogWarning("[ClueInteractable] " + gameObject.name + ": No ClueUI on clueCanvas '" + clueCanvas.name + "'. Add ClueUI component and assign Close button.", this);
        }
        else
        {
            Debug.LogWarning("[ClueInteractable] " + gameObject.name + ": clueCanvas is null, cannot show clue.", this);
            OnClueClosed();
        }
    }

    private void OnClueClosed()
    {
        Debug.Log("[ClueInteractable] " + gameObject.name + ": OnClueClosed called.", this);
        if (closeSound != null)
            closeSound.Play();

        if (clueCanvas != null)
            clueCanvas.SetActive(false);

        if (destroyAfterClose)
            Destroy(gameObject);
    }
}
