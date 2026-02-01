using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// When Start is chosen: hide menu, teleport camera to the monologue shot, show the monologue
/// stage (character in darkness with light), play monologue, then load the game scene.
/// Place a "monologue anchor" empty at the position/rotation where the camera should be for the shot.
/// </summary>
public class StartSequenceController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    [Tooltip("Empty GameObject placed where the camera should be to capture the monologue shot.")]
    [SerializeField] private Transform monologueCameraAnchor;

    [Header("Monologue stage")]
    [Tooltip("The setup with character + black background + light. Inactive at start; enabled when sequence runs.")]
    [SerializeField] private GameObject monologueStage;

    [Header("Menu to hide")]
    [SerializeField] private GameObject menuPanel;

    [Header("Monologue")]
    [SerializeField] private AudioClip monologueClip;
    [Tooltip("Optional. If set, monologue plays from this source (2D). Otherwise uses PlayClipAtPoint.")]
    [SerializeField] private AudioSource monologueSource;
    [Tooltip("If no clip, we wait this many seconds before loading the game.")]
    [SerializeField] private float monologueDuration = 5f;

    [Header("Scene")]
    [SerializeField] private string gameSceneName = "Proto";

    public void PlayStartSequence()
    {
        StartCoroutine(StartSequenceRoutine());
    }

    private IEnumerator StartSequenceRoutine()
    {
        if (menuPanel != null)
            menuPanel.SetActive(false);

        if (mainCamera == null)
            mainCamera = Camera.main;
        if (mainCamera != null && monologueCameraAnchor != null)
        {
            mainCamera.transform.position = monologueCameraAnchor.position;
            mainCamera.transform.rotation = monologueCameraAnchor.rotation;
        }

        if (monologueStage != null)
            monologueStage.SetActive(true);

        float waitTime = (monologueClip != null) ? monologueClip.length : monologueDuration;
        if (monologueClip != null)
        {
            if (monologueSource != null)
            {
                monologueSource.clip = monologueClip;
                monologueSource.Play();
            }
            else
                AudioSource.PlayClipAtPoint(monologueClip, mainCamera != null ? mainCamera.transform.position : Vector3.zero);
        }

        yield return new WaitForSeconds(waitTime);

        if (!string.IsNullOrEmpty(gameSceneName))
            SceneManager.LoadScene(gameSceneName);
    }
}
