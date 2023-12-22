using Fusion;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public class AbstractCharacterMovement : AbstractMovement
    {
        protected CharacterController cc;

        // Proxy might also need to do some local rendering related things, like IK. So fetch these components for both Proxy and StateAuthority. 
        public override void Spawned()
        {
            base.Spawned();
            cc = GetComponentInChildren<CharacterController>();
        }
    }
}