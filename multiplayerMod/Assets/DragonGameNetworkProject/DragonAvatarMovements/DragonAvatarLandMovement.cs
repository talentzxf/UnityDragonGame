using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace DragonGameNetworkProject.DragonAvatarMovements
{
    public class DragonAvatarLandMovement : AbstractCharacterMovement
    {
        private int Land = Animator.StringToHash("Land");
        private Rigidbody rb;

        private float landHeight = 1f;
        
        private float maxDistance = 100f;
        
        private float distanceThreshold = 0.1f;
        private float angleThreshold = 0.1f;
        
        public float moveSpeed = 10f;
        public float rotateSpeed = 30f;
        public override void Spawned()
        {
            base.Spawned();
            if (HasStateAuthority)
            {
                rb = ccTransform.GetComponent<Rigidbody>();
            }
        }

        public override void OnEnterMovement()
        {
            if (HasStateAuthority)
            {
                rb.isKinematic = true; // Disable physics system.
                cc.enabled = true; // Re-enable character controller.
                
                UIController.Instance.HideDragonControlUI();
                StartCoroutine(DoLand(Runner.DeltaTime));
            }
        }
        
        private RaycastHit hit;
        public override bool CanEnterMovement()
        {
            if (HasStateAuthority)
            {
                int layerMask = LayerMask.GetMask("Terrain");
            
                bool result = Physics.Raycast(ccTransform.position + Vector3.up * 10.0f, Vector3.down, out hit, maxDistance, layerMask);

                if (!result)
                {
                    UIController.Instance.ShowGameMsg("Can't land here");
                }

                return result;
            }

            return true; // In Proxy, it will always follow StateAuthority.
        }

        public override void OnLeaveMovement()
        {
            animator.SetBool(Land, false);
        }

        IEnumerator DoLand(float deltaTime)
        {
            Vector3 targetPosition = hit.point + Vector3.up * landHeight;
            Vector3 xzVector = ccTransform.forward;
            xzVector.y = 0.0f;
            xzVector.Normalize();
            
            Quaternion targetRotation = quaternion.LookRotation(xzVector, Vector3.up);
            Debug.Log("TargetPosition:" + targetPosition + " targetRotation:" + targetRotation);

            while (Vector3.Distance(ccTransform.position, targetPosition) > distanceThreshold ||
                   Quaternion.Angle(ccTransform.rotation, targetRotation) > angleThreshold)
            {
                float step = moveSpeed * deltaTime;
                float rotateStep = rotateSpeed * deltaTime;

                Vector3 proposedPosition = Vector3.MoveTowards(ccTransform.position, targetPosition, step);

                ccTransform.position = proposedPosition;
                ccTransform.rotation = Quaternion.RotateTowards(ccTransform.rotation, targetRotation,
                    rotateStep);
                yield return null;
            }
            
            Debug.Log("Second phase, playing animation!");

            ccTransform.position = targetPosition;
            ccTransform.rotation = targetRotation;
            
            animator.SetBool(Land, true);

            float lastLandProgress = 0.0f;
            float landProgress = 0.0f;
            // Drop to hit point.
            while (Vector3.Distance(ccTransform.position, hit.point) > distanceThreshold || landProgress < 0.99f)
            {
                landProgress = animator.GetFloat("LandProgress");
                ccTransform.position = Vector3.Lerp(targetPosition, hit.point, landProgress);

                if (landProgress <= lastLandProgress) // If no progress, force play.
                {
                    Debug.Log("Force play landing animation!");
                    animator.Play("Landing");
                }

                lastLandProgress = landProgress;
                
                Debug.Log($"Second phase land progress:{landProgress}");
                yield return null;
            }

            ccTransform.position = hit.point;
            
            controller.SwitchTo<DragonAvatarGroundMovement>();
        }
    }
}