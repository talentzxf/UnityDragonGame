using DragonGameNetworkProject;
using UnityEngine;

public class ClimbDragonAction : MonoBehaviour
{
    private Transform dragonTransform;

    private void Start()
    {
        dragonTransform = transform.parent;
    }
    
    private bool triggerClimb = false;
    private CharacterMovementController playerMovementController;

    void FixedUpdate()
    {
        if (triggerClimb)
        {
            playerMovementController.GetMovement<ClimbDragonMovement>().PrepareToClimb(dragonTransform.gameObject, transform.position);
            playerMovementController.SwitchTo<PlayerClimbDragon>(); 
            triggerClimb = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerLocomotion = other.GetComponentInParent<CharacterMovementController>();

            if (_playerLocomotion.HasStateAuthority)
            {
                triggerClimb = true; // Have to change state out of Render loop (Not sure why OnTriggerEnter is Render loop??).
            }
        }
    }
}
