using System;
using UnityEngine;

namespace DragonGameNetworkProject.DragonMovements
{
    public class DragonLandMovement : AbstractRigidBodyMovement
    {
        private int lands = Animator.StringToHash("Land");
        
        public override void OnEnterMovement()
        {
            animator.SetBool(lands, true);
        }

        private void Update()
        {
            
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority)
                return;
            
            float landProgress = animator.GetFloat("LandProgress");
            if (landProgress > 0.0)
            {
                animator.SetBool(lands, false);
            }

            rigidBody.velocity = Vector3.up * -10.0f;
        }
    }
}