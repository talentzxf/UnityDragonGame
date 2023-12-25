using UnityEngine;

namespace DragonGameNetworkProject.DragonMovements
{
    public class DragonTakeOffMovement : AbstractRigidBodyMovement
    {
        private string rootBonePath = "Armature/Bone";
        private int takeOff = Animator.StringToHash("TakeOff");

        private Transform animationRoot;
        private Transform boneRoot;

        public override void Spawned()
        {
            base.Spawned();
            boneRoot = ccTransform.Find(rootBonePath);
        }

        public override void OnEnterMovement()
        {
            animator.SetBool(takeOff, true);
        }

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority)
                return;

            float takeOffProgress = animator.GetFloat("TakeOffProgress");
            if (takeOffProgress > 0.0)
            {
                animator.SetBool(takeOff, false); // Reset the flag.
            }

            // Debug.Log("Dragon bone position:" + boneRoot.position + " ," +
            //           animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);

            if (takeOffProgress >= 1.0)
            {
                ccTransform.position = boneRoot.position;
                boneRoot.position = ccTransform.position;

                Debug.Log("Take off CCTransform:" + ccTransform.position);

                controller.SwitchTo<DragonFlyingMovement>();
            }
        }
    }
}