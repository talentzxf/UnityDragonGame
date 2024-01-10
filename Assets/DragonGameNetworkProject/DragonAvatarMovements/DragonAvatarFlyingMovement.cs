using System;
using UnityEngine;

namespace DragonGameNetworkProject.DragonAvatarMovements
{
    public class DragonAvatarFlyingMovement: AbstractCharacterMovement
    {
        private int isFlying = Animator.StringToHash("IsFlying");
        public override void OnEnterMovement()
        {
            animator.SetBool(isFlying, true);
        }
    }
}