using System.Security.Cryptography;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public class AbstractRigidBodyMovement: AbstractMovement
    {
        protected Rigidbody rigidBody;
        public override void Spawned()
        {
            base.Spawned();
            rigidBody = GetComponentInChildren<Rigidbody>();
        }
        
        public override void OnEnterMovement()
        {
        }

        public override void OnLeaveMovement()
        {
        }
    }
}