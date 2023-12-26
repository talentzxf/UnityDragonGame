using System;
using Fusion;

namespace DragonGameNetworkProject.DragonMovements
{
    public class DragonMovementController : CharacterMovementController
    {
        [Networked] public NetworkObject playerNO { set; get; }

        public PlayerMovementController playerController => playerNO.GetComponent<PlayerMovementController>();
        
        public override void Spawned()
        {
            base.Spawned();
            
            if (HasStateAuthority)
            {
                SwitchTo<DragonIdleMovement>();
            }
        }
    }
}