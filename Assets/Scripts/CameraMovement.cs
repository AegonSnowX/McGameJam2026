using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    // Dead zone size , switch later according to game design and world to find the sweet spot
    [SerializeField] private Vector2 deadZoneSize = new Vector2(1.5f, 1.0f);

    private Vector3 offset;
   void Awake()
{
    if (target == null) return;

    Vector3 pos = target.position;
    pos.z = transform.position.z; // preserve camera depth
    transform.position = pos;
}


    void Start()
    {
        if (target == null)
        {
            Debug.LogError("CameraFollow: Target not assigned.");
            enabled = false;
            return;
        }
 
        offset = transform.position - target.position;
      
    }

    void LateUpdate()
    {
        Vector3 targetPosition = target.position + offset;
        Vector3 cameraPosition = transform.position;

        float deltaX = targetPosition.x - cameraPosition.x;
        float deltaY = targetPosition.y - cameraPosition.y;

        if (Mathf.Abs(deltaX) > deadZoneSize.x)
        {
            cameraPosition.x += deltaX - Mathf.Sign(deltaX) * deadZoneSize.x;
        }

        if (Mathf.Abs(deltaY) > deadZoneSize.y)
        {
            cameraPosition.y += deltaY - Mathf.Sign(deltaY) * deadZoneSize.y;
        }

        transform.position = cameraPosition;
    }
}
