using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Add this to any Button. On click it will play the GameAudioManager button-click sound.
/// No need to assign anything if GameAudioManager exists in the scene.
/// </summary>
[RequireComponent(typeof(Button))]
public class PlaySoundOnButtonClick : MonoBehaviour
{
    private Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
        if (_button != null)
            _button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (GameAudioManager.Instance != null)
            GameAudioManager.Instance.PlayButtonClick();
    }
}
