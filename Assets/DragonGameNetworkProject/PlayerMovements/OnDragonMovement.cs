using System;
using UnityEngine.PlayerLoop;

namespace DragonGameNetworkProject
{
    public class OnDragonMovement : OnDragonBase
    {
        private void Update() // When on dragon, the player is not synced, only dragon transform need to be synced.
        {
            ccTransform.position = sitPoint.position;
            ccTransform.rotation = sitPoint.rotation;
        }
    }
}