using System;
using Fusion;
using UnityEngine.PlayerLoop;

namespace DragonGameNetworkProject
{
    public class OnDragonMovement : OnDragonBase
    {
        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority)
            {
                ccTransform.position = sitPoint.position;
                ccTransform.rotation = sitPoint.rotation;                
            }
        }
    }
}