using UnityEngine;
using TMPro;

/// <summary>
/// Win door: when the player has collected all required keys (GameManager.KeysRequired),
/// the door plays its open animation. When the player enters the door trigger (goes out),
/// the game wins. Shows a "not enough keys" prompt while the player is in the trigger without enough keys.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class WinDoor : MonoBehaviour
{
    [Header("Door animation")]
    [SerializeField] private Animator doorAnimator;
    [Tooltip("Animator trigger to open the door. Leave empty if using Animation component.")]
    [SerializeField] private string openTriggerName = "Open";
    [SerializeField] private Animation doorAnimation;

    [Header("Not enough keys prompt")]
    [Tooltip("Shown while the player is in the trigger and doesn't have enough keys. Hidden when they leave or have enough keys.")]
    [SerializeField] private GameObject notEnoughKeysPrompt;
    [Tooltip("Optional. If set, text is updated to show current/required keys (e.g. 'Keys: 2/3'). Use {0}=current, {1}=required.")]
    [SerializeField] private TextMeshProUGUI notEnoughKeysText;
    [SerializeField] private string notEnoughKeysFormat = "Keys: {0}/{1}";

    [Header("Optional")]
    [SerializeField] private string playerTag = "Player";

    private Collider2D _col;
    private bool _doorOpened;
    private bool _playerInTrigger;

    void Awake()
    {
        _col = GetComponent<Collider2D>();
        if (_col != null)
            _col.isTrigger = true;
        if (notEnoughKeysPrompt != null)
            notEnoughKeysPrompt.SetActive(false);
    }

    void Update()
    {
        if (_doorOpened) return;
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.KeysCollected < GameManager.Instance.KeysRequired) return;

        _doorOpened = true;

        if (doorAnimator != null && !string.IsNullOrEmpty(openTriggerName))
            doorAnimator.SetTrigger(openTriggerName);
        if (doorAnimation != null)
            doorAnimation.Play();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;
        if (GameManager.Instance == null) return;

        _playerInTrigger = true;

        if (GameManager.Instance.HasWon || GameManager.Instance.IsGameOver) return;
        if (GameManager.Instance.KeysCollected >= GameManager.Instance.KeysRequired)
        {
            HideNotEnoughKeysPrompt();
            GameManager.Instance.TriggerWin();
            return;
        }

        ShowNotEnoughKeysPrompt();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;
        _playerInTrigger = false;
        HideNotEnoughKeysPrompt();
    }

    private void ShowNotEnoughKeysPrompt()
    {
        if (notEnoughKeysText != null && GameManager.Instance != null && !string.IsNullOrEmpty(notEnoughKeysFormat))
            notEnoughKeysText.text = string.Format(notEnoughKeysFormat, GameManager.Instance.KeysCollected, GameManager.Instance.KeysRequired);
        if (notEnoughKeysPrompt != null)
            notEnoughKeysPrompt.SetActive(true);
    }

    private void HideNotEnoughKeysPrompt()
    {
        if (notEnoughKeysPrompt != null)
            notEnoughKeysPrompt.SetActive(false);
    }

    private bool IsPlayer(Collider2D other)
    {
        if (other.CompareTag(playerTag)) return true;
        return other.GetComponent<PlayerMovement>() != null || other.GetComponentInParent<PlayerMovement>() != null;
    }
}
