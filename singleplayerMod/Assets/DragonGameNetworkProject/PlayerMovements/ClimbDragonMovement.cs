using System.Collections;
using DragonGameNetworkProject.DragonMovements;
using Unity.VisualScripting;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public class ClimbDragonMovement : OnDragonBase
    {
        private int _climb = UnityEngine.Animator.StringToHash("climb");

        private Vector3 climbStartForward;

        private GameObject tobeSetPlayerNO;
        private string rootBoneName = "Bone.004";
        
        public void PrepareToClimb(GameObject dragonGO, Vector3 startPosition, Vector3 startForward)
        {
            // if (!HasStateAuthority)
            // {
               // return;
            //}

            ccTransform.position = startPosition;
            ccTransform.forward = startForward;

            (controller as PlayerMovementController).dragonNO = dragonGO.GetComponentInParent<GameObject>();
            // dragonNO.RequestStateAuthority(); // The dragon is mine now. Claim the authority.

            tobeSetPlayerNO = GetComponent<GameObject>();
            Vector3 forwardDir = dragonTransform.right;

            climbStartForward = forwardDir;
            cc.enabled = false;
            
            mecanimAnimator.Animator.SetBool(_climb, true);

            Transform boneRoot = Utility.RecursiveFind(dragonTransform, rootBoneName);
            Camera.main.GetComponent<FirstPersonCamera>().LerpToDistance(3.0f, boneRoot, 3.0f);
        }

        IEnumerator FixPlayerPosition(float durationSeconds)
        {
            Vector3 startPosition = ccTransform.position;
            float progress = 0.0f;
            while (progress < durationSeconds)
            {
                ccTransform.position = Vector3.Slerp(startPosition, sitPoint.position, progress / durationSeconds);
                progress += Time.deltaTime;
                yield return null;
            }

            controller.SwitchTo<OnDragonMovement>();
            
            dragonNO.GetComponent<DragonMovementController>().SwitchTo<DragonMountedMovement>();
        }

        public  void Update()
        {
            if ( tobeSetPlayerNO != null)
            {
                dragonNO.GetComponent<DragonMovementController>().playerNO = tobeSetPlayerNO;
                tobeSetPlayerNO = null;
            }

            // if (HasStateAuthority)
            // {
                mecanimAnimator.Animator.applyRootMotion = true;
            //}
            // else
            // {
            //     networkAnimator.Animator.applyRootMotion = false;
            // }

            // if (!HasStateAuthority)
            //     return;

            float climbUpProgress = mecanimAnimator.Animator.GetFloat("ClimbUpProgress");
            Vector3 currentForward = Vector3.Lerp(climbStartForward, dragonTransform.forward, climbUpProgress);
            ccTransform.forward = currentForward;

            if (climbUpProgress > 0.0f)
            {
                mecanimAnimator.Animator.SetBool(_climb, false);
            }

            if (climbUpProgress > 0.99f)
            {
                StartCoroutine(FixPlayerPosition(0.5f));
            }
        }
    }
}