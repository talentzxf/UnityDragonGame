using Fusion;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public abstract class AbstractMovement: NetworkBehaviour
    {
        protected NetworkMecanimAnimator networkAnimator;
        protected Transform ccTransform;
        protected CharacterMovementController controller;
        
        public override void Spawned()
        {
            controller = GetComponent<CharacterMovementController>();
            networkAnimator = controller.avatarGO.GetComponent<NetworkMecanimAnimator>();
            ccTransform = networkAnimator.transform;
        }

        public virtual void OnAnimatorIK(int layerIndex)
        {
        }

        public abstract void OnEnterMovement();
        public abstract void OnLeaveMovement();
    }
}