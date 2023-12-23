using Fusion;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public class PlayerMovementController: CharacterMovementController
    {
        public NetworkObject dragonNO;
        public Transform avatarTransform;
    }
}