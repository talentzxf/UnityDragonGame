using System.Security.Cryptography;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public class AbstractRigidBodyMovement: AbstractMovement
    {
        protected Rigidbody rigidBody;
        public void Start()
        {
           
            rigidBody = controller.avatarGO.GetComponent<Rigidbody>();
        }
        
        public override void OnEnterMovement()
        {
        }

        public override void OnLeaveMovement()
        {
        }
    }
}