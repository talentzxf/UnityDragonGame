using UnityEngine;

namespace DragonGameNetworkProject.DragonMovements
{
    public class DragonTakeOffMovement : AbstractRigidBodyMovement
    {
        private string rootBonePath = "Armature/Bone";
        private int takeOff = UnityEngine.Animator.StringToHash("TakeOff");

        private Transform animationRoot;
        private Transform boneRoot;

        public new void Start()
        {
            boneRoot = ccTransform.Find(rootBonePath);
        }

        public override void OnEnterMovement()
        {
            animator.SetBool(takeOff, true);
        }

        public override void OnLeaveMovement()
        {
            animator.SetBool(takeOff, false);
        }

        public  void FixedUpdate()
        {
            //if (!HasStateAuthority)
            //    return;

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