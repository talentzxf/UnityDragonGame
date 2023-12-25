using System;
using UnityEngine;

namespace DragonGameNetworkProject.DragonMovements
{
    public class DragonMountedMovement: AbstractRigidBodyMovement
    {
        private int hasLandedOnGround = Animator.StringToHash("HasLandedOnGround");
        private int speedFWD = Animator.StringToHash("SpeedFWD");
        
        public override void OnEnterMovement()
        {
            animator.SetBool(hasLandedOnGround, true);

            rigidBody.freezeRotation = true;
            rigidBody.useGravity = true;
            
            animator.SetFloat(speedFWD, 0.0f);
        }

        public override void OnLeaveMovement()
        {
            rigidBody.useGravity = false;
            rigidBody.freezeRotation = false;
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
        
        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority == false)
                return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                controller.SwitchTo<DragonTakeOffMovement>();
            }
        }
    }
}