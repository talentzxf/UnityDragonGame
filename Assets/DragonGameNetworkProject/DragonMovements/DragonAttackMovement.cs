using System;
using UnityEngine;

namespace DragonGameNetworkProject.DragonMovements
{
    public class DragonAttackMovement: AbstractRigidBodyMovement
    {
        private int attack = Animator.StringToHash("Attack");
        
        public override void OnEnterMovement()
        {
            animator.SetBool(attack, true);
        }

        public override void OnLeaveMovement()
        {
            animator.SetBool(attack, false);
        }

        private void FixedUpdate()
        {
            float attackProgress = animator.GetFloat("AttackProgress");
            
            Debug.Log("Attacking:" + attackProgress);

            if (attackProgress > 0.9)
            {
                controller.SwitchTo<DragonFlyingMovement>();
            }
        }
    }
}