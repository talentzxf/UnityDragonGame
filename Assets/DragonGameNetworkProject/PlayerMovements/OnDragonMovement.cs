using System;

namespace DragonGameNetworkProject
{
    public class OnDragonMovement : OnDragonBase
    {
        private void LateUpdate()
        {
            if (HasStateAuthority)
            {
                ccTransform.position = sitPoint.position;
            }
        }
    }
}