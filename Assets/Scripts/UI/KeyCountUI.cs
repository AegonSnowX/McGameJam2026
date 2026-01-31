using UnityEngine;
using TMPro;

public class KeyCountUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI keyCountText;

    [Header("Settings")]
    [SerializeField] private string format = "Keys: {0}/{1}";

    private int _lastKeysCollected = -1;
    private int _lastKeysRequired = -1;

    void Start()
    {
        RefreshText();
    }

    void Update()
    {
        if (keyCountText == null) return;
        if (GameManager.Instance == null) return;

        int collected = GameManager.Instance.KeysCollected;
        int required = GameManager.Instance.KeysRequired;

        if (collected == _lastKeysCollected && required == _lastKeysRequired)
            return;

        _lastKeysCollected = collected;
        _lastKeysRequired = required;
        RefreshText();
    }

    private void RefreshText()
    {
        if (keyCountText == null) return;
        if (GameManager.Instance == null)
        {
            keyCountText.text = string.Format(format, 0, "?");
            return;
        }
        keyCountText.text = string.Format(format, GameManager.Instance.KeysCollected, GameManager.Instance.KeysRequired);
    }
}
