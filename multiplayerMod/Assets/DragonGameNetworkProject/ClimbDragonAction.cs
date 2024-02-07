using DragonGameNetworkProject;
using UnityEngine;

public class ClimbDragonAction : MonoBehaviour
{
    private Transform dragonTransform;

    private void Start()
    {
        dragonTransform = transform.parent;
    }
    
    private CharacterMovementController playerMovementController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerMovementController = other.GetComponentInParent<CharacterMovementController>();

            if (playerMovementController.HasStateAuthority)
            {
                playerMovementController.GetMovement<ClimbDragonMovement>().PrepareToClimb(dragonTransform.gameObject, 
                    transform.position, transform.forward);
                playerMovementController.SwitchTo<ClimbDragonMovement>();
            }
        }
    }
}
