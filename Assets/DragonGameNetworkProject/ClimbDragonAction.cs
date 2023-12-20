using Fusion;
using Fusion.Addons.FSM;
using Unity.VisualScripting;
using UnityEngine;

public class ClimbDragonAction : MonoBehaviour
{
    private Transform dragonTransform;

    private void Start()
    {
        dragonTransform = transform.parent;
    }
    
    private bool triggerClimb = false;
    private PlayerLocomotion _playerLocomotion;

    void FixedUpdate()
    {
        if (triggerClimb)
        {
            _playerLocomotion.StateMachine.GetState<PlayerClimbDragon>()
                .Setup(dragonTransform.gameObject, transform.position);
            _playerLocomotion.SetNextState<PlayerClimbDragon>(); 
            triggerClimb = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerLocomotion = other.GetComponentInParent<PlayerLocomotion>();

            if (_playerLocomotion.HasStateAuthority)
            {
                triggerClimb = true; // Have to change state out of Render loop (Not sure why OnTriggerEnter is Render loop??).
            }
        }
    }
}
