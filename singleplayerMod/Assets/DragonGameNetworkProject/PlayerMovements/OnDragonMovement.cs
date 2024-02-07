using System;

using UnityEngine.PlayerLoop;

namespace DragonGameNetworkProject
{
    public class OnDragonMovement : OnDragonBase
    {
        public new void Start()
        {
          
        }

        private void Update()
        {
            ccTransform.position = sitPoint.position;
            ccTransform.rotation = sitPoint.rotation;
        }

        public  void FixedUpdate()
        {
            // if (HasStateAuthority)
            // {
                ccTransform.position = sitPoint.position;
                ccTransform.rotation = sitPoint.rotation;
            //}
        }
    }
}