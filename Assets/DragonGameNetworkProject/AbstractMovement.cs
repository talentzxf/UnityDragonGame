using Fusion;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public abstract class AbstractMovement: NetworkBehaviour
    {
        protected NetworkMecanimAnimator networkAnimator;
        protected Transform ccTransform;
        protected CharacterMovementController controller;
        protected Animator animator;
        
        public override void Spawned()
        {
            controller = GetComponent<CharacterMovementController>();
            networkAnimator = controller.avatarGO.GetComponent<NetworkMecanimAnimator>();
            ccTransform = controller.avatarGO.transform;
            animator = controller.avatarGO.GetComponent<Animator>();
        }

        public virtual void OnAnimatorIK(int layerIndex)
        {
        }

        public abstract void OnEnterMovement();
        public abstract void OnLeaveMovement();

        public virtual bool CanLeaveMovement()
        {
            return true;
        }

        public virtual bool CanEnterMovement()
        {
            return true;
        }
    }
}