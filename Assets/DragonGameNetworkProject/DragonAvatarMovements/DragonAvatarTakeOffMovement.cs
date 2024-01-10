using UnityEngine;

namespace DragonGameNetworkProject.DragonAvatarMovements
{
    public class DragonAvatarTakeOffMovement: AbstractCharacterMovement
    {
        private int takeOff = Animator.StringToHash("TakeOff");
        public override void OnEnterMovement()
        {
            animator.SetBool(takeOff, true);
            takeOffPosition = ccTransform.position;
        }

        public override void OnLeaveMovement()
        {
            animator.SetBool(takeOff, false);
        }

        private Vector3 takeOffPosition;
        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority)
                return;
            
            float takeOffProgress = animator.GetFloat("TakeOffProgress");
            if (takeOffProgress > 0.0f)
            {
                animator.SetBool(takeOff, false);
            }

            if (takeOffProgress > 0.5f)
            {
                Debug.Log("Progress:" + takeOffProgress + ";Before y:" + ccTransform.position.y);
                ccTransform.position = takeOffPosition + (takeOffProgress - 0.5f) * 10.0f * transform.up;
                Debug.Log("After y:" + ccTransform.position.y);
            }

            if (takeOffProgress >= 0.99)
            {
                controller.SwitchTo<DragonAvatarFlyingMovement>();
            }
        }
    }
}