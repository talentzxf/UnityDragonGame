using Fusion;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public class AbstractCharacterMovement : NetworkBehaviour
    {
        protected CharacterController cc;
        protected NetworkMecanimAnimator networkAnimator;
        protected Transform ccTransform;

        protected CharacterMovementController controller;

        public virtual void OnAnimatorIK(int layerIndex)
        {
        }

        // Proxy might also need to do some local rendering related things, like IK. So fetch these components for both Proxy and StateAuthority. 
        public override void Spawned()
        {
            cc = GetComponentInChildren<CharacterController>();
            networkAnimator = GetComponentInChildren<NetworkMecanimAnimator>();
            ccTransform = cc.transform;
            controller = GetComponent<CharacterMovementController>();
        }
    }
}