using System;
using UnityEngine;

namespace DragonGameNetworkProject.DragonAvatarMovements
{
    public class DragonAvatarFlyingMovement: AbstractCharacterMovement
    {
        private int isFlying = Animator.StringToHash("IsFlying");
        public override void OnEnterMovement()
        {
            cc.enabled = false;
            
            animator.SetBool(isFlying, true);
        }


        public override void OnLeaveMovement()
        {
            animator.SetBool(isFlying, false);
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
        }
    }
}