using Unity.VisualScripting;
using UnityEngine;

namespace DragonGameNetworkProject.DragonAvatarMovements
{
    public class DragonAvatarStopMovement : AbstractCharacterMovement
    {
        public override void OnEnterMovement()
        {
            base.OnEnterMovement();

            if (HasStateAuthority)
            {
                cc.enabled = false;

                animator.enabled = false;

                var rb = ccTransform.GetComponent<Rigidbody>();
                if(rb != null)
                    rb.isKinematic = true;
                
                var fpsCamera = Camera.main.GetComponent<FirstPersonCamera>();
                if(fpsCamera != null)
                    fpsCamera.enabled = false;
            }

        }
    }
}