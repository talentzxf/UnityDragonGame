using DragonGameNetworkProject.DragonMovements;
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

        protected GameObject dragonNO => (controller as PlayerMovementController).dragonNO;

        private Transform _dragonTransform;

        protected Transform dragonTransform
        {
            get
            {
                // if (_dragonTransform == null)
                //     _dragonTransform = dragonNO.transform;
                return _dragonTransform;
            }
        }
        
        public override void OnAnimatorIK(int layerIndex)
        {
            if (!isEnableIK)
                return;
            
            float climbUpProgress = mecanimAnimator.Animator.GetFloat("ClimbUpProgress");
            if (climbUpProgress > 0.5)
            {
                mecanimAnimator.Animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, climbUpProgress);
                mecanimAnimator.Animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, climbUpProgress);

                mecanimAnimator.Animator.SetIKPosition(AvatarIKGoal.LeftFoot, GetLeftFootIK().position);
                mecanimAnimator.Animator.SetIKPosition(AvatarIKGoal.RightFoot, GetRightFootIK().position);
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