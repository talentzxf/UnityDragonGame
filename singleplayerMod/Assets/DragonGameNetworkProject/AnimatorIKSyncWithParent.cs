using System;
using System.Collections;
using System.Collections.Generic;
using DragonGameNetworkProject;
using UnityEngine;

public class AnimatorIKSyncWithParent : MonoBehaviour
{
    private CharacterMovementController controller;
    private void Start()
    {
        controller = GetComponentInParent<CharacterMovementController>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (controller && controller.currentMovement)
        {
            controller.currentMovement.OnAnimatorIK(layerIndex);
        }
    }
}
