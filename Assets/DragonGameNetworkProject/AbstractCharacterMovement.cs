using System;
using Fusion;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public class AbstractCharacterMovement : NetworkBehaviour
    {
        protected CharacterController cc;
        protected NetworkMecanimAnimator networkAnimator;
        protected Transform ccTransform;

        public bool enabledByDefault = false;

        protected CharacterMovementController controller;

        public virtual void OnAnimatorIK(int layerIndex)
        {
            
        }

        public override void Spawned()
        {
            if (HasStateAuthority)
            {
                cc = GetComponentInChildren<CharacterController>();

                networkAnimator = GetComponentInChildren<NetworkMecanimAnimator>();
                ccTransform = cc.transform;

                enabled = enabledByDefault;

                controller = GetComponent<CharacterMovementController>();
            }
        }
    }
}