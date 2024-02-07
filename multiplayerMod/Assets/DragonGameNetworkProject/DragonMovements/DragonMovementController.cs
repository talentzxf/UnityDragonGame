using System;
using Fusion;
using UnityEngine;

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
                
                // Find all colliders and set tag as Player.
                var colliders = GetComponentsInChildren<Collider>();
                foreach (var collider in colliders)
                {
                    collider.gameObject.tag = "Player";
                }
            }
        }
    }
}