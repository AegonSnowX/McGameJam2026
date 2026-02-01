using UnityEngine;

/// <summary>
/// Reveals image pieces when the matching key is collected. Piece 1 shows when key 1 is collected,
/// piece 2 when key 2 is collected, etc. Each key collectible has a keyIndex (1, 2, or 3) so the
/// right piece is revealed regardless of collection order.
/// </summary>
public class KeyProgressReveal : MonoBehaviour
{
    [Header("Image pieces (piece 1 = key with keyIndex 1, piece 2 = keyIndex 2, piece 3 = keyIndex 3)")]
    [SerializeField] private GameObject piece1;
    [SerializeField] private GameObject piece2;
    [SerializeField] private GameObject piece3;

    void Start()
    {
        ApplyVisibility();
    }

    void Update()
    {
        ApplyVisibility();
    }

    private void ApplyVisibility()
    {
        if (GameManager.Instance == null) return;

        if (piece1 != null) piece1.SetActive(GameManager.Instance.HasKey1);
        if (piece2 != null) piece2.SetActive(GameManager.Instance.HasKey2);
        if (piece3 != null) piece3.SetActive(GameManager.Instance.HasKey3);
    }
}
