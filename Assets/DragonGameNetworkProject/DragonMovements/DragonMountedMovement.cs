using System;
using UnityEngine;

namespace DragonGameNetworkProject.DragonMovements
{
    class InputData
    {
        public bool TakeOff;
        public bool UnMount;

        public void Update()
        {
            TakeOff = Input.GetKey(KeyCode.Space);
            UnMount = Input.GetKey(KeyCode.M);

            if (TakeOff)
            {
                Debug.Log("TakeOff!!!!");
            }

            if (UnMount)
            {
                Debug.Log("UnMount!!!");
            }
        }

        public void Reset()
        {
            TakeOff = false;
            UnMount = false;
        }
    }
    
    public class DragonMountedMovement: AbstractRigidBodyMovement
    {
        private int hasLandedOnGround = Animator.StringToHash("HasLandedOnGround");
        private int speedFWD = Animator.StringToHash("SpeedFWD");
        
        private InputData inputData = new InputData();
        
        public override void OnEnterMovement()
        {
            animator.SetBool(hasLandedOnGround, true);

            rigidBody.freezeRotation = true;
            rigidBody.useGravity = true;
            
            animator.SetFloat(speedFWD, 0.0f);
        }

        public override void OnLeaveMovement()
        {
            animator.SetBool(hasLandedOnGround, false);
        }

        public void FixedUpdate() // Not sure why, but proxy won't execute FixedUpdateNetwork???
        {
            var animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (!animatorStateInfo.IsName("IdleSimple") && !animatorStateInfo.IsName("TakeOff") && !animatorStateInfo.IsName("Hover") && !animator.IsInTransition(0))
            {
                animator.Play("IdleSimple");
            }
        }

        private void Update()
        {
            inputData.Update();
        }

        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority == false)
                return;

            try
            {
                if (inputData.TakeOff)
                {
                    Debug.Log("Begin to take off!");
                    
                    rigidBody.useGravity = false;
                    rigidBody.freezeRotation = false;
                    controller.SwitchTo<DragonTakeOffMovement>();
                }

                if (inputData.UnMount)
                {
                    Debug.Log("Begin to unmount!");
                    
                    rigidBody.useGravity = true;
                    rigidBody.freezeRotation = true;
                    (controller as DragonMovementController).playerController.SwitchTo<ClimbDownDragonMovement>(); // Player climb down.
                    controller.SwitchTo<DragonIdleMovement>();
                }
            }
            finally
            {
                inputData.Reset();
            }

        }
    }
}