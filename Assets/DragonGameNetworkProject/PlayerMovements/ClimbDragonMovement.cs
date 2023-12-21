using System.Collections;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public class ClimbDragonMovement : AbstractCharacterMovement
    {
        private string sitPoint = "SitPoint";
        private string dragonNeckName = "Bone.008";
        private int _climb = Animator.StringToHash("climb");

        private GameObject dragonGO;
        private Transform dragonTransform;

        private Vector3 climbStartForward;

        void PrepareToClimb(GameObject dragonGO, Vector3 startPosition)
        {
            if (!HasStateAuthority)
            {
                return;
            }
            
            this.dragonGO = dragonGO;
            dragonTransform = this.dragonGO.transform;
            Vector3 forwardDir = dragonTransform.right;

            climbStartForward = forwardDir;
            cc.enabled = false;
            
            networkAnimator.Animator.SetBool(_climb, true);
            
            Camera.main.GetComponent<CameraController>().LerpToDistance(3.0f, 3.0f);
        }

        IEnumerator FixPlayerPosition(float durationSeconds)
        {
            Vector3 startPosition = ccTransform.position;
            Vector3 endPosition = Utility.RecursiveFind(dragonGO.transform, sitPoint).position;
            
            float progress = 0.0f;
            while (progress < durationSeconds)
            {
                ccTransform.position = Vector3.Slerp(startPosition, endPosition, progress/durationSeconds);
                progress += Time.deltaTime;
                yield return null;
            }
        }
        
        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority)
                return;
            
            float climbUpProgress = networkAnimator.Animator.GetFloat("ClimbUpProgress");
            Vector3 currentForward = Vector3.Lerp(climbStartForward, dragonTransform.forward, climbUpProgress);
            ccTransform.forward = currentForward;

            if (climbUpProgress > 0.0f)
            {
                networkAnimator.Animator.SetBool(_climb, false);
            }

            if (climbUpProgress > 0.99f)
            {
                StartCoroutine(FixPlayerPosition(0.5f));
                Transform bone08 = Utility.RecursiveFind(dragonTransform, dragonNeckName);
                ccTransform.SetParent(bone08);
                
                // _dragonGO.GetComponent<CharacterLocomotion>().SetProcessor<DragonMounted>();
                //
                // _loco.SetProcessor<OnDragonProcessor>(_dragonGO);
            }
        }

    }
}