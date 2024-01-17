using System;
using DragonGameNetworkProject.DragonMovements;
using UnityEngine;

namespace DragonGameNetworkProject.DragonAvatarMovements
{
    class GroundInput
    {
        public float Vertical;
        public float Horizontal;
        public bool SpacePressed;

        public void Update()
        {
            Vertical = Input.GetAxis("Vertical");
            Horizontal = Input.GetAxis("Horizontal");

            SpacePressed = Input.GetKeyDown(KeyCode.Space);
            if (SpacePressed)
            {
                Debug.Log("Pressed space, begin to take off!");
            }
        }

        public void Reset()
        {
            Vertical = 0;
            Horizontal = 0;
            SpacePressed = false;
        }
    }
    
    public class DragonAvatarGroundMovement : AbstractCharacterMovement
    {
        private bool _controllable; // Player can only control after count down.
        
        public float rotationSpeed = 10.0f;

        public float walkSpeed = 5f;

        private Camera camera;

        private int _isWalkingHash = Animator.StringToHash("IsWalking");

        private GroundInput _groundInput = new GroundInput();

        public override void Spawned()
        {
            base.Spawned();
            camera = Camera.main;

            if (HasStateAuthority)
            {
                GameTimer.Instance.onGameStart.AddListener(() =>
                {
                    _controllable = true;
                    UIController.Instance.ShowPrompt("Press space to take off");
                });                
            }
        }

        public override void OnEnterMovement()
        {
            base.OnEnterMovement();
            if (HasStateAuthority)
            {
                camera.GetComponent<FirstPersonCamera>().clampEnabled = true;
                cc.enabled = true;
            }
        }

        public override void OnLeaveMovement()
        {
            base.OnLeaveMovement();

            if (HasStateAuthority)
            {
                animator.SetBool(_isWalkingHash, false);
            }
        }

        private void Update()
        {
            if (HasStateAuthority)
            {
                _groundInput.Update();

                if (_groundInput.Horizontal == 0.0f && _groundInput.Vertical == 0.0f)
                {
                    networkAnimator.Animator.SetBool(_isWalkingHash, false);
                }
                else
                {
                    networkAnimator.Animator.SetBool(_isWalkingHash, true);
                }
                    
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority == false)
                return;

            try
            {
                // Add Gravity.
                float delta = Runner.DeltaTime;
                if (!cc.isGrounded)
                {
                    cc.Move(Physics.gravity * delta);
                }
                
                if (!_controllable)
                    return;
                
                if (_groundInput.SpacePressed)
                {
                    controller.SwitchTo<DragonAvatarTakeOffMovement>();
                    return;
                }

                // Take inputs and react.

                if ((_groundInput.Vertical == 0 && _groundInput.Horizontal == 0))
                {
                    return;
                }
                
                Transform camTransform = camera.transform;

                Vector3 forwardVelocity = _groundInput.Vertical * camTransform.forward;
                Vector3 horizontalVelocity = _groundInput.Horizontal * camTransform.right;

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

                cc.Move(walkSpeed * delta * ccTransform.forward);
            }
            finally
            {
                _groundInput.Reset();
            }
        }
    }
}