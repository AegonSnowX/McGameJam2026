using UnityEngine;

/// <summary>
/// Prison Architect–style menu background: top-down camera locked on a moving NPC.
/// Attach to the Main Camera in the menu scene. Assign the wandering NPC as target.
/// </summary>
public class MenuCameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [Tooltip("Lower = snappier (more locked on target). 0 = instant follow.")]
    [SerializeField] private float smoothTime = 0.04f;
    [Tooltip("Camera Z (depth). Keep same as your game (e.g. -10 for 2D).")]
    [SerializeField] private float cameraZ = -10f;

    private Vector3 _velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = target.position;
        desired.z = cameraZ;

        if (smoothTime <= 0f)
        {
            transform.position = desired;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref _velocity, smoothTime);
        }
    }

    /// <summary>Set target at runtime (e.g. after spawning the menu NPC).</summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
