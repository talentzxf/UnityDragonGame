using System;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public class PlayerGroundMovement : AbstractCharacterMovement
    {
        private float maxSpeed = 2f;
        public float rotationSpeed = 10.0f;

        private Camera camera;

        private int _isIdleHash = UnityEngine.Animator.StringToHash("isIdle");
        private int _speed = UnityEngine.Animator.StringToHash("speed");
        

        public new void Start()
        {
            
            camera = Camera.main;
        }

        public override void OnEnterMovement()
        {
            base.OnEnterMovement();

            // if (HasStateAuthority)
            // {
                camera.GetComponent<FirstPersonCamera>().clampEnabled = true;
           // }
        }

        public  void FixedUpdate()
        {
            // if (HasStateAuthority)
            // {
                mecanimAnimator.Animator.applyRootMotion = true;
            // }
            // else
            // {
            //     networkAnimator.Animator.applyRootMotion = false;
            // }

            // if (HasStateAuthority == false)
            //     return;

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
                mecanimAnimator.Animator.SetBool(_isIdleHash, true);
            }
            else
            {
                mecanimAnimator.Animator.SetBool(_isIdleHash, false);

                // Lerp rotate the character.
                Quaternion targetRotation = Quaternion.LookRotation(resultVelocityXY, Vector3.up);
                Quaternion resultRotation =
                    Quaternion.Slerp(ccTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // Keep the character always up.
                resultRotation.x = 0;
                resultRotation.z = 0;
                ccTransform.rotation = resultRotation;
            }

            // if (Runner.IsForward)
            // {
                mecanimAnimator.Animator.SetFloat(_speed, speed);
           // }
        }
    }
}