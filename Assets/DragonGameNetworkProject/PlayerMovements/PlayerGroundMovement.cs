using System;
using Fusion;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public class PlayerGroundMovement : AbstractCharacterMovement
    {
        private float maxSpeed = 2f;
        public float rotationSpeed = 10.0f;

        private Camera camera;

        private int _isIdleHash = Animator.StringToHash("isIdle");
        private int _speed = Animator.StringToHash("speed");

        public override void Spawned()
        {
            base.Spawned();
            camera = Camera.main;
        }

        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority)
            {
                cc.GetComponent<NetworkTransform>().enabled = true;
                networkAnimator.Animator.applyRootMotion = true;
            }
            else
            {
                networkAnimator.Animator.applyRootMotion = false;
            }

            if (HasStateAuthority == false)
                return;

            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");

            Vector3 forwardVelocity = vertical * camera.transform.forward;
            Vector3 horizontalVelocity = horizontal * camera.transform.right;

            Vector3 resultVelocityXY = forwardVelocity + horizontalVelocity;

            // Vector3 resultVelocity = resultVelocityXY;

            // if (!_cc.isGrounded)
            // {
            //     resultVelocity += Vector3.down;
            // }

            // _cc.Move(resultVelocity * Runner.DeltaTime);

            float speed = resultVelocityXY.magnitude;
            if (speed < Mathf.Epsilon)
            {
                networkAnimator.Animator.SetBool(_isIdleHash, true);
            }
            else
            {
                networkAnimator.Animator.SetBool(_isIdleHash, false);

                // Lerp rotate the character.
                Quaternion targetRotation = Quaternion.LookRotation(resultVelocityXY, Vector3.up);
                Quaternion resultRotation =
                    Quaternion.Slerp(ccTransform.rotation, targetRotation, rotationSpeed * Runner.DeltaTime);

                // Keep the character always up.
                resultRotation.x = 0;
                resultRotation.z = 0;
                ccTransform.rotation = resultRotation;
            }

            if (Runner.IsForward)
            {
                networkAnimator.Animator.SetFloat(_speed, speed);
            }
        }
    }
}