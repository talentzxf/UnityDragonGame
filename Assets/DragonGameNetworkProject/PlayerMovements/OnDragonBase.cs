using UnityEngine;

namespace DragonGameNetworkProject
{
    public class OnDragonBase : AbstractCharacterMovement
    {
        private string leftFootName = "LeftFoot";
        private string rightFootName = "RightFoot";

        private Transform _leftFootIK;
        private Transform _rightFootIK;

        protected GameObject dragonGO;
        protected Transform dragonTransform;

        public void Prepare(GameObject dragonGO)
        {
            this.dragonGO = dragonGO;
            dragonTransform = this.dragonGO.transform;
        }

        private void OnAnimatorIK(int layerIndex)
        {
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