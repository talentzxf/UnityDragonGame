using System;
using System.Collections;
using System.Collections.Generic;
using Fusion.Addons.FSM;
using UnityEngine;

public class ClimbDragonAction : MonoBehaviour
{
    private Transform dragonTransform;

    private void Start()
    {
        dragonTransform = transform.parent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerLocomotion playerLocomotion = other.GetComponent<PlayerLocomotion>();

            if (playerLocomotion.HasStateAuthority)
            {
                playerLocomotion.StateMachine.GetState<PlayerClimbDragon>()
                    .Setup(dragonTransform.gameObject, transform.position);
                playerLocomotion.StateMachine.TryActivateState<PlayerClimbDragon>();                
            }
        }
    }
}
