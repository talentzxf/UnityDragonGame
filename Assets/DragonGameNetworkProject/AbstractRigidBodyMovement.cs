using System.Security.Cryptography;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public class AbstractRigidBodyMovement: AbstractMovement
    {
        private Rigidbody rigidBody;
        public override void Spawned()
        {
            base.Spawned();
            rigidBody = GetComponentInChildren<Rigidbody>();
        }
    }
}