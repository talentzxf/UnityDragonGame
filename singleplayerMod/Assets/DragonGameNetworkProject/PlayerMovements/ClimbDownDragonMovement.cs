using UnityEngine;

namespace DragonGameNetworkProject
{
    public class ClimbDownDragonMovement: OnDragonBase
    {
        private int _climbDown = UnityEngine.Animator.StringToHash("climbDown");
        private int _isIdleHash = UnityEngine.Animator.StringToHash("isIdle");
        protected override bool isEnableIK => false;
        
        public override void OnEnterMovement()
        {
            // if (HasStateAuthority)
            // {
                mecanimAnimator.Animator.SetBool(_isIdleHash, true);
                mecanimAnimator.Animator.SetBool(_climbDown, true);
                cc.enabled = true;
                Camera.main.GetComponent<FirstPersonCamera>().LerpToDistance(1.0f, ccTransform, 3.0f);
            //}
        }

        public override void OnLeaveMovement()
        {
            // if (HasStateAuthority)
            // {
                mecanimAnimator.Animator.SetBool(_climbDown, false);
           // }
        }

        public void FixedUpdate()
        {
            // if (HasStateAuthority)
            // {
                mecanimAnimator.Animator.applyRootMotion = true;
            // }
            // else
            // {
            //     networkAnimator.Animator.applyRootMotion = false;
            // }
            
            // if (!HasStateAuthority)
            //     return;
            
            float climbDownProgress = mecanimAnimator.Animator.GetFloat("ClimbDownProgress");
            
            Debug.Log("Climb Down Progress");
            
            if (climbDownProgress > 0.0f)
            {
                mecanimAnimator.Animator.SetBool(_climbDown, false);
            }

            if (climbDownProgress > 0.9f)
            {
                controller.SwitchTo<PlayerGroundMovement>();
            }
        }
    }
}