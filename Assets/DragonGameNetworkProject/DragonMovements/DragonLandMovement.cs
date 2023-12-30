using UnityEngine;

namespace DragonGameNetworkProject.DragonMovements
{
    class PoseAdjuster // Can't use coroutine in Network code, have to implement by myself.
    {
        public Rigidbody rigidBody;
        public Vector3 targetPosition;
        public Quaternion targetRotation;

        private float distanceThreshold = 0.01f;
        private float angleThreshold = 0.01f;


        public float moveSpeed = 10f;
        public float rotateSpeed = 30f;

        private bool hasFinished = false;

        private CharacterMovementController controller;

        public void Prepare(Rigidbody rigidBody, Vector3 targetPoint, Quaternion targetRotation,
            CharacterMovementController controller)
        {
            this.rigidBody = rigidBody;
            this.targetPosition = targetPoint;
            this.targetRotation = targetRotation;
            this.controller = controller;

            hasFinished = false;

            rigidBody.useGravity = false;
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
        }

        public bool Step(float deltaTime) // Return, do we need further round of iteration??
        {
            if (hasFinished)
                return false;

            if (Vector3.Distance(rigidBody.transform.position, targetPosition) > distanceThreshold||
                Quaternion.Angle(rigidBody.transform.rotation, targetRotation) > angleThreshold)
            {
                float step = moveSpeed * deltaTime;
                float rotateStep = rotateSpeed * deltaTime;

                Vector3 proposedPosition = Vector3.MoveTowards(rigidBody.transform.position, targetPosition, step);

                rigidBody.transform.position = proposedPosition;
                rigidBody.transform.rotation = Quaternion.RotateTowards(rigidBody.transform.rotation, targetRotation,
                    rotateStep);
                
                return true;
            }
            else
            {
                rigidBody.transform.position = targetPosition;
                rigidBody.transform.rotation = targetRotation;

                hasFinished = true;

                controller.SwitchTo<DragonMountedMovement>();

                return false;
            }
        }
    }

    public class DragonLandMovement : AbstractRigidBodyMovement
    {
        public float beginLandDistance = 5.0f;

        private float maxDistance = 100f;

        private int lands = Animator.StringToHash("Land");

        // Deprecated flag.
        public bool onGround = false;

        public override void OnEnterMovement()
        {
            animator.SetBool(lands, true);

            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
            
            Debug.Log("Set Land true");
            
            if(HasStateAuthority)
                UIController.Instance.HideSpeedBar();
        }

        public override void OnLeaveMovement()
        {
            animator.SetBool(lands, false);

            Debug.Log("Set Land false");
        }

        private bool isAdjustingPose = false;
        private PoseAdjuster poseAdjuster = new PoseAdjuster();

        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority)
                return;

            if (isAdjustingPose)
            {
                isAdjustingPose = poseAdjuster.Step(Runner.DeltaTime);
            }
            else
            {
                float landProgress = animator.GetFloat("LandProgress");
                if (landProgress > 0.0)
                {
                    animator.SetBool(lands, false);
                }

                int layerMask = LayerMask.GetMask("Terrain");
                RaycastHit hit;
                if (Physics.Raycast(rigidBody.position, Vector3.down, out hit, maxDistance, layerMask))
                {
                    Vector3 targetPosition = hit.point + Vector3.up * beginLandDistance;
                    Quaternion targetRotation = Quaternion.LookRotation(
                        Vector3.ProjectOnPlane(rigidBody.transform.forward, Vector3.up)
                        , Vector3.up);

                    Debug.Log("TargetPosition:" + targetPosition + " targetRotation:" + targetRotation);

                    poseAdjuster.Prepare(rigidBody, targetPosition, targetRotation, controller);
                    isAdjustingPose = true;
                }
                else
                {
                    Debug.LogError("Can't detect ground here!!!");
                }
            }
        }
    }
}