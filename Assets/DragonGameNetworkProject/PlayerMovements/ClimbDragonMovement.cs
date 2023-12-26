using System.Collections;
using DragonGameNetworkProject.DragonMovements;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public class ClimbDragonMovement : OnDragonBase
    {
        private int _climb = Animator.StringToHash("climb");

        private Vector3 climbStartForward;
        
        protected override bool isEnableIK => false;

        public void PrepareToClimb(GameObject dragonGO, Vector3 startPosition)
        {
            if (!HasStateAuthority)
            {
                return;
            }

            ccTransform.position = startPosition;

            (controller as PlayerMovementController).dragonNO = dragonGO.GetComponentInParent<NetworkObject>();
            dragonNO.RequestStateAuthority(); // The dragon is mine now. Claim the authority.

            dragonNO.GetComponent<DragonMovementController>().playerNO = GetComponent<NetworkObject>();
            
            Vector3 forwardDir = dragonTransform.right;

            climbStartForward = forwardDir;
            cc.enabled = false;
            
            networkAnimator.Animator.SetBool(_climb, true);

            Camera.main.GetComponent<FirstPersonCamera>().LerpToDistance(3.0f, 3.0f);
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
            }
        }
    }
}