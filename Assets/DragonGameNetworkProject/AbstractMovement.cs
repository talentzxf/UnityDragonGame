using Fusion;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public class AbstractMovement: NetworkBehaviour
    {
        protected NetworkMecanimAnimator networkAnimator;
        protected Transform ccTransform;
        protected CharacterMovementController controller;
        
        public override void Spawned()
        {
            networkAnimator = GetComponentInChildren<NetworkMecanimAnimator>();
            ccTransform = networkAnimator.transform;
            controller = GetComponent<CharacterMovementController>();
        }

        public virtual void OnAnimatorIK(int layerIndex)
        {
        }
    }
}