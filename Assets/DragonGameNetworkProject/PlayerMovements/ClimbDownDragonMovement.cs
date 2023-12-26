using UnityEngine;

namespace DragonGameNetworkProject
{
    public class ClimbDownDragonMovement: OnDragonBase
    {
        private int _climbDown = Animator.StringToHash("climbDown");
        private int _isIdleHash = Animator.StringToHash("isIdle");
        protected override bool isEnableIK => false;

        public override void OnEnterMovement()
        {
            networkAnimator.Animator.SetBool(_isIdleHash, true);
            networkAnimator.Animator.SetBool(_climbDown, true);
            cc.enabled = true;
            Camera.main.GetComponent<FirstPersonCamera>().LerpToDistance(1.0f, 3.0f);
        }

        public override void OnLeaveMovement()
        {
            networkAnimator.Animator.SetBool(_climbDown, false);
        }

        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority)
            {
                networkAnimator.Animator.applyRootMotion = true;
            }
            else
            {
                networkAnimator.Animator.applyRootMotion = false;
            }
            
            if (!HasStateAuthority)
                return;
            
            float climbDownProgress = networkAnimator.Animator.GetFloat("ClimbDownProgress");
            
            Debug.Log("Climb Down Progress");
            
            if (climbDownProgress > 0.0f)
            {
                networkAnimator.Animator.SetBool(_climbDown, false);
            }

            if (climbDownProgress > 0.9f)
            {
                controller.SwitchTo<PlayerGroundMovement>();
            }
        }
    }
}