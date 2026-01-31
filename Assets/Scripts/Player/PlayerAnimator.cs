using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;
    
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        
        // Auto-find PlayerMovement if not assigned
        if (playerMovement == null)
        {
            playerMovement = GetComponentInParent<PlayerMovement>();
        }
    }

    void Update()
    {
        if (playerMovement == null || animator == null) return;

        // Set animator parameters from PlayerMovement
        animator.SetBool("IsMoving", playerMovement.IsMoving);
        animator.SetFloat("MoveX", playerMovement.MoveX);
        animator.SetFloat("MoveY", playerMovement.MoveY);
        animator.SetFloat("MouseX", playerMovement.MouseX);
        animator.SetFloat("MouseY", playerMovement.MouseY);
    }
}
