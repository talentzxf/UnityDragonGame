using System;
using UnityEngine;

namespace DragonGameNetworkProject.DragonMovements
{
    public class DragonIdleMovement : AbstractRigidBodyMovement
    {
        public override void OnEnterMovement()
        {
            base.OnEnterMovement();
            
            Utility.RecursiveFind(ccTransform, "OnboardingCube").gameObject.SetActive(true);
            Utility.RecursiveFind(ccTransform, "ClimbDownStair").gameObject.SetActive(false);
        }
    }
}