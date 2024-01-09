using UnityEngine;

namespace DragonGameNetworkProject.DragonAvatarMovements
{
    public class DragonAvatarGroundMovement : AbstractCharacterMovement
    {
        private float maxSpeed = 2f;

        public float rotationSpeed = 10.0f;

        public float walkSpeed = 10f;

        private Camera camera;

        private int _isWalkingHash = Animator.StringToHash("IsWalking");

        public override void Spawned()
        {
            base.Spawned();
            camera = Camera.main;
        }

        public override void OnEnterMovement()
        {
            base.OnEnterMovement();
            if (HasStateAuthority)
            {
                camera.GetComponent<FirstPersonCamera>().clampEnabled = true;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority == false)
                return;

            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");

            if (vertical == 0 && horizontal == 0)
            {
                networkAnimator.Animator.SetBool(_isWalkingHash, false);
                return;
            }
            
            networkAnimator.Animator.SetBool(_isWalkingHash, true);
            
            Transform camTransform = camera.transform;

            Vector3 forwardVelocity = vertical * camTransform.forward;
            Vector3 horizontalVelocity = horizontal * camTransform.right;

            Vector3 addedVelocity = forwardVelocity + horizontalVelocity;
            addedVelocity.y = 0.0f; // No need to care about y axis.
            Vector3 resultVelocityDir = addedVelocity.normalized;
            
            // Lerp rotate the character.
            Quaternion targetRotation = Quaternion.LookRotation(resultVelocityDir, Vector3.up);
            Quaternion resultRotation =
                Quaternion.Slerp(ccTransform.rotation, targetRotation, rotationSpeed * Runner.DeltaTime);
            // Keep the character always up.
            resultRotation.x = 0;
            resultRotation.z = 0;

            ccTransform.rotation = resultRotation;
            cc.SimpleMove(resultVelocityDir * walkSpeed);
        }
    }
}