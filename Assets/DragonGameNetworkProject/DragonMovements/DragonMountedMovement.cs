using System;
using UnityEngine;

namespace DragonGameNetworkProject.DragonMovements
{
    public class DragonMountedMovement: AbstractRigidBodyMovement
    {
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