using UnityEngine;

namespace DragonGameNetworkProject.DragonMovements
{
    public class DragonTakeOffMovement: AbstractRigidBodyMovement
    {
        private string rootBonePath = "Armature/Bone";
        private int takeOff = Animator.StringToHash("TakeOff");
        
        private Transform boneRoot;
        public override void Spawned()
        {
            base.Spawned();
            boneRoot = ccTransform.Find(rootBonePath);
        }

        public override void OnEnterMovement()
        {
            networkAnimator.Animator.SetBool(takeOff, true);
        }
        
        public override void FixedUpdateNetwork()
        {
            float takeOffProgress = networkAnimator.Animator.GetFloat("TakeOffProgress");
            if (takeOffProgress > 0.0)
            {
                networkAnimator.Animator.SetBool(takeOff, false); // Reset the flag.
            }

            if (takeOffProgress > 0.99)
            {
                ccTransform.position = boneRoot.position;
                
                Debug.Log("Set dragon transform to:" + ccTransform.position + " ," + networkAnimator.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);

                controller.SwitchTo<DragonFlyingMovement>();
            }
        }
    }
}