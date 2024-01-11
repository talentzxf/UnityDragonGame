using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace DragonGameNetworkProject.DragonAvatarMovements
{
    public class DragonAvatarController : CharacterMovementController
    {
        private void Awake()
        {
            Collider[] colliders = GetComponentsInChildren<Collider>(true);
            foreach (var collider in colliders)
            {
                collider.gameObject.tag = "Player"; // Setup tags.
                collider.isTrigger = true;
            }
        }
    }
}