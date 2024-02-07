using System;
using UnityEngine;

namespace DragonGameNetworkProject.DragonMovements
{
    public class DragonMovementController : CharacterMovementController
    {
        public GameObject playerNO { set; get; }

        public PlayerMovementController playerController => playerNO.GetComponent<PlayerMovementController>();
        
        public new void Start()
        {
            //if (HasStateAuthority)
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