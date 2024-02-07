using UnityEngine;

namespace DragonGameNetworkProject
{
    public class AbstractCharacterMovement : AbstractMovement
    {
        protected CharacterController cc;

        // Proxy might also need to do some local rendering related things, like IK. So fetch these components for both Proxy and StateAuthority. 
        public new void Start()
        {
            cc = controller.characterController;
        }

        public override void OnEnterMovement()
        {
        }

        public override void OnLeaveMovement()
        {
        }
    }
}