using System;
using Unity.VisualScripting;
using UnityEngine;

namespace DragonGameNetworkProject.DragonMovements
{
    class InputData
    {
        public bool TakeOff;
        public bool UnMount;

        public void Update()
        {
            TakeOff = Input.GetKey(KeyCode.Space);
            UnMount = Input.GetKey(KeyCode.M);

            if (TakeOff)
            {
                Debug.Log("TakeOff!!!!");
            }

            if (UnMount)
            {
                Debug.Log("UnMount!!!");
            }
        }

        public void Reset()
        {
            TakeOff = false;
            UnMount = false;
        }
    }
    
    public class DragonMountedMovement: AbstractRigidBodyMovement
    {
        private int hasLandedOnGround = UnityEngine.Animator.StringToHash("HasLandedOnGround");
        private int speedFWD = UnityEngine.Animator.StringToHash("SpeedFWD");
        
        private InputData inputData = new InputData();

        private GameObject no;

        public new void Start()
        {
            no = GetComponent<GameObject>();
        }

        public override void OnEnterMovement()
        {
            animator.SetBool(hasLandedOnGround, true);

            rigidBody.freezeRotation = true;
            rigidBody.useGravity = true;
            
            animator.SetFloat(speedFWD, 0.0f);
            
            Utility.RecursiveFind(ccTransform, "OnboardingCube").gameObject.SetActive(false);
            Utility.RecursiveFind(ccTransform, "ClimbDownStair").gameObject.SetActive(true);
            
            // What if the user just landed?????
           // UIController.Instance.ShowGameMsg("Player:" + Runner.GetPlayerUserId(no.StateAuthority) + " took the dragon!");
        }

        public override void OnLeaveMovement()
        {
            animator.SetBool(hasLandedOnGround, false);
        }

        public void FixedUpdate() // Not sure why, but proxy won't execute FixedUpdateNetwork???
        {
            var animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (!animatorStateInfo.IsName("IdleSimple") && !animatorStateInfo.IsName("TakeOff") && !animatorStateInfo.IsName("Hover") && !animator.IsInTransition(0))
            {
                animator.Play("IdleSimple");
            }

            FixedUpdateNetwork();
        }

        private void Update()
        {
            inputData.Update();
        }

        public  void FixedUpdateNetwork()
        {
            // if (HasStateAuthority == false)
            //     return;

            try
            {
                if (inputData.TakeOff)
                {
                    Debug.Log("Begin to take off!");
                    
                    rigidBody.useGravity = false;
                    rigidBody.freezeRotation = false;
                    controller.SwitchTo<DragonTakeOffMovement>();
                }

                if (inputData.UnMount)
                {
                    Debug.Log("Begin to unmount!");
                    
                    rigidBody.useGravity = true;
                    rigidBody.freezeRotation = true;
                    
                    (controller as DragonMovementController).playerController.SwitchTo<ClimbDownDragonMovement>(); // Player climb down.

                    (controller as DragonMovementController).playerNO = null;
                    controller.SwitchTo<DragonIdleMovement>();
                }
            }
            finally
            {
                inputData.Reset();
            }

        }
    }
}