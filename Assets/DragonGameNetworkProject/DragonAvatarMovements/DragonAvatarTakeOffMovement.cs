using UnityEngine;

namespace DragonGameNetworkProject.DragonAvatarMovements
{
    public class DragonAvatarTakeOffMovement: AbstractCharacterMovement
    {
        private int takeOff = Animator.StringToHash("TakeOff");
        public override void OnEnterMovement()
        {
            animator.SetBool(takeOff, true);
        }

        public override void OnLeaveMovement()
        {
            animator.SetBool(takeOff, false);
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority)
                return;
            
            float takeOffProgress = animator.GetFloat("TakeOffProgress");
            if (takeOffProgress > 0.0f)
            {
                animator.SetBool(takeOff, false);
            }

            if (takeOffProgress >= 0.99)
            {
                controller.SwitchTo<DragonAvatarFlyingMovement>();
            }
        }
    }
}