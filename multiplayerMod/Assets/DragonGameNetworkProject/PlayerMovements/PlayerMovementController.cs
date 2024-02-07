using Fusion;
using UnityEngine;

namespace DragonGameNetworkProject
{
    public class PlayerMovementController: CharacterMovementController
    {
        [Networked] public NetworkObject dragonNO { set; get; }
    }
}