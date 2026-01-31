using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

/// <summary>
/// Attach to the root of a clue panel. Assign clueText and closeButton in the inspector.
/// If clueText is not assigned, the first TextMeshProUGUI in children will be used.
/// </summary>
public class ClueUI : MonoBehaviour
{
    [Header("Optional - set in prefab; text is auto-found in children if empty")]
    [SerializeField] private TextMeshProUGUI clueText;
    [SerializeField] private Button closeButton;

    private Action _onClose;
    private string _pendingText;
    private GameObject _sourceToDestroyOnClose;

    void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(Close);
    }

    void OnEnable()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
        ApplyPendingText();
    }

    /// <summary>
    /// Set clue text and callback when closed. sourceToDestroyOnClose is the clue object to destroy when Close is pressed (e.g. the ClueInteractable).
    /// </summary>
    public void Setup(string text, Action onClose, GameObject sourceToDestroyOnClose = null)
    {
        _onClose = onClose;
        _sourceToDestroyOnClose = sourceToDestroyOnClose;
        _pendingText = text;
        ApplyPendingText();

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(Close);
            closeButton.navigation = new Navigation { mode = Navigation.Mode.None };
        }
    }

    private void ApplyPendingText()
    {
        if (string.IsNullOrEmpty(_pendingText)) return;

        TextMeshProUGUI targetText = clueText;
        if (targetText == null)
            targetText = FindClueTextInChildren();
        if (targetText != null)
        {
            targetText.text = _pendingText;
            targetText.ForceMeshUpdate(true, true);
        }
    }

    /// <summary>
    /// Find first TextMeshProUGUI that is not inside a Button (so we get the clue body, not the button label).
    /// </summary>
    private TextMeshProUGUI FindClueTextInChildren()
    {
        var all = GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var tmp in all)
        {
            if (tmp.GetComponentInParent<Button>() != null)
                continue;
            return tmp;
        }
        return GetComponentInChildren<TextMeshProUGUI>(true);
    }

    /// <summary>
    /// Call when the close button is pressed.
    /// </summary>
    public void Close()
    {
        if (_onClose != null)
            _onClose.Invoke();
        else
            gameObject.SetActive(false);

        if (_sourceToDestroyOnClose != null)
        {
            _sourceToDestroyOnClose.SetActive(false);
            Destroy(_sourceToDestroyOnClose);
        }
    }
}
