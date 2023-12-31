using DragonGameNetworkProject.DragonMovements;
using Fusion;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public class OnDragonBase : AbstractCharacterMovement
    {
        private string leftFootName = "LeftFoot";
        private string rightFootName = "RightFoot";

        private Transform _leftFootIK;
        private Transform _rightFootIK;

        private string sitPointStr = "SitPoint";
        private Transform _sitPoint;

        protected virtual bool isEnableIK => true;

        protected Transform sitPoint
        {
            get
            {
                if (_sitPoint == null)
                {
                    _sitPoint = Utility.RecursiveFind(dragonTransform, sitPointStr);
                }

                return _sitPoint;
            }
        }

        protected NetworkObject dragonNO => (controller as PlayerMovementController).dragonNO;

        private Transform _dragonTransform;

        protected Transform dragonTransform
        {
            get
            {
                if (_dragonTransform == null)
                    _dragonTransform = dragonNO.transform;
                return _dragonTransform;
            }
        }
        
        public override void OnAnimatorIK(int layerIndex)
        {
            if (!isEnableIK)
                return;
            
            float climbUpProgress = networkAnimator.Animator.GetFloat("ClimbUpProgress");
            if (climbUpProgress > 0.5)
            {
                networkAnimator.Animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, climbUpProgress);
                networkAnimator.Animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, climbUpProgress);

                networkAnimator.Animator.SetIKPosition(AvatarIKGoal.LeftFoot, GetLeftFootIK().position);
                networkAnimator.Animator.SetIKPosition(AvatarIKGoal.RightFoot, GetRightFootIK().position);
            }
        }

        Transform GetLeftFootIK()
        {
            if (_leftFootIK == null)
            {
                _leftFootIK = Utility.RecursiveFind(dragonTransform, leftFootName);
            }

            return _leftFootIK;
        }

        Transform GetRightFootIK()
        {
            if (_rightFootIK == null)
            {
                _rightFootIK = Utility.RecursiveFind(dragonTransform, rightFootName);
            }

            return _rightFootIK;
        }
    }
}