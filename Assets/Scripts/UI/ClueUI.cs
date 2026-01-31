using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Attach to the root of a clue panel. Assign clueText and closeButton in the inspector or via Setup().
/// </summary>
public class ClueUI : MonoBehaviour
{
    [Header("Optional - set in prefab or via Setup")]
    [SerializeField] private TextMeshProUGUI clueText;
    [SerializeField] private Button closeButton;

    private Action _onClose;

    void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseClicked);
    }

    /// <summary>
    /// Set clue text and callback when closed. Call from ClueInteractable.
    /// </summary>
    public void Setup(string text, Action onClose)
    {
        _onClose = onClose;
        if (clueText != null)
            clueText.text = text;
    }

    private void OnCloseClicked()
    {
        _onClose?.Invoke();
    }
}
