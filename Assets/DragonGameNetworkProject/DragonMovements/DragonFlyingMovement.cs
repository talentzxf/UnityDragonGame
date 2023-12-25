using System;
using Fusion;
using UnityEngine;

namespace DragonGameNetworkProject.DragonMovements
{
    class InputHandler
    {
        public float Horizontal;
        public float Vertical;

        public bool Jump;
        public bool JumpHold;

        public bool Fly;

        public bool LB;
        public bool RB;

        public void Update()
        {
            Horizontal = Input.GetAxis("Horizontal");
            Vertical = Input.GetAxis("Vertical");

            Jump = Input.GetButtonDown("Jump");
            JumpHold = Input.GetButton("Jump");
            Fly = JumpHold;

            RB = Input.GetButton("RightTilt");
            LB = Input.GetButton("LeftTilt");
        }
    }

    public class DragonFlyingMovement : AbstractRigidBodyMovement
    {
        [Networked] public Vector3 rigidbodyVelocity { set; get; }

        private float maxSpeed = 500.0f;
        private float rotationSpeed = 1000f;

        private int speedFWD = Animator.StringToHash("SpeedFWD");

        private InputHandler input = new InputHandler();

        [Header("Physics")] public float
            HandleReturnSpeed; //how quickly our handle on our character is returned to normal after a force is added (such as jumping

        private float ActGravAmt; //the actual gravity that is applied to our character
        public LayerMask GroundLayers; //what layers the ground can be
        private float FloorTimer; //how long we are on the floor
        private bool OnGround; //the bool for if we are on ground (this is used for the animator
        private float ActionAirTimer; //the air timer counting our current actions performed in air

        [Header("Stats")] private float MaxSpeed = 15f; //max speed for basic movement
        private float SpeedClamp = 50f; //max possible speed
        private float ActAccel; //our actual acceleration
        private float Acceleration = 4f; //how quickly we build speed
        private float MovementAcceleration = 20f; //how quickly we adjust to new speeds
        private float SlowDownAcceleration = 2f; //how quickly we slow down
        private float turnSpeed = 2f; //how quickly we turn on the ground

        private float
            FlownAdjustmentLerp = 1; //if we have flown this will be reset at 0, and effect turn speed on the ground

        private float ActSpeed; //our actual speed
        private Vector3 movepos, targetDir, DownwardDirection; //where to move to

        [Header("Flying")] private float
            FlyingDirectionSpeed =
                15f; //how much influence our direction relative to the camera will influence our flying

        private float FlyingRotationSpeed = 6f; //how fast we turn in air overall
        private float FlyingUpDownSpeed = 10.0f; //how fast we rotate up and down
        private float FlyingLeftRightSpeed = 8.0f; //how fast we rotate left and right
        private float FlyingRollSpeed = 6f; //how fast we roll

        private float FlyingAcceleration = 4f; //how much we accelerate to max speed
        private float FlyingDecelleration = 0.1f; //how quickly we slow down when flying
        private float FlyingSpeed = 20; //our max flying speed
        private float FlyingMinSpeed = 6; //our flying slow down speed

        private float FlyingAdjustmentSpeed = 2; //how quickly our velocity adjusts to the flying speed
        private float FlyingAdjustmentLerp = 0; //the lerp for our adjustment amount

        [Header("Flying Physics")] private float FlyingGravityAmt = 2f; //how much gravity will pull us down when flying
        private float GlideGravityAmt = 6f; //how much gravity affects us when just gliding
        private float FlyingGravBuildSpeed = 0.2f; //how much our gravity is lerped when stopping flying

        private float FlyingVelocityGain = 1f; //how much velocity we gain for flying downwards
        private float FlyingVelocityLoss = 0.5f; //how much velocity we lose for flying upwards
        private float FlyingLowerLimit = -6f; //how much we fly down before a boost
        private float FlyingUpperLimit = 7f; //how much we fly up before a boost;
        private float GlideTime = 10f; //how long we glide for when not flying before we start to fall

        private float FlyingTimer; //the time before the animation stops flying

        private Transform Cam;
        private Transform CamY;

        private string rootBonePath = "Armature/Bone";
        private Transform boneRoot;

        public override void Spawned()
        {
            base.Spawned();
            rigidBody.useGravity = false;
            rigidBody.freezeRotation = false;

            Cam = Camera.main.transform;
            CamY = Cam;

            FlyingTimer = GlideTime;
            ActGravAmt = 0.0f;
            FlownAdjustmentLerp = -1;

            boneRoot = ccTransform.Find(rootBonePath);
        }

        private void Update()
        {
            input.Update();
        }

        //handle how our speed is increased or decreased when flying
        void HandleVelocity(float d, float TargetSpeed, float Accel, float YAmt)
        {
            if (ActSpeed > FlyingSpeed) //we are over out max speed, slow down slower
                Accel = Accel * 0.8f;

            if (YAmt < FlyingLowerLimit) //we are flying down! boost speed
            {
                TargetSpeed = TargetSpeed + (FlyingVelocityGain * (YAmt * -0.5f));
            }
            else if (YAmt > FlyingUpperLimit) //we are flying up! reduce speed
            {
                TargetSpeed = TargetSpeed - (FlyingVelocityLoss * YAmt);
                ActSpeed -= (FlyingVelocityLoss * YAmt) * d;
            }

            //clamp speed
            TargetSpeed = Mathf.Clamp(TargetSpeed, -SpeedClamp, SpeedClamp);
            //lerp speed
            ActSpeed = Mathf.Lerp(ActSpeed, TargetSpeed, Accel * d);
        }

        void FlyingCtrl(float d, float Speed, float XMove, float ZMove)
        {
            //input direction 
            float InvertX = -1;
            float InvertY = -1;

            XMove = XMove * InvertX; //horizontal inputs
            ZMove = ZMove * InvertY; //vertical inputs

            //get direction to move character
            DownwardDirection = VehicleFlyingDownwardDirection(d, ZMove);
            Vector3 SideDir = VehicleFlyingSideDirection(d, XMove);
            //get our rotation and adjustment speeds
            float rotSpd = FlyingRotationSpeed;
            float FlyLerpSpd = FlyingAdjustmentSpeed * FlyingAdjustmentLerp;

            //lerp mesh slower when not on ground
            RotateSelf(DownwardDirection, d, rotSpd);
            RotateMesh(d, SideDir, rotSpd);

            if (FlyingTimer < GlideTime * 0.7f) //lerp to velocity if not flying
                RotateToVelocity(d, rotSpd * 0.05f);

            Vector3 targetVelocity = ccTransform.forward * Speed;
            //push down more when not pressing fly
            if (input.Fly)
                ActGravAmt = Mathf.Lerp(ActGravAmt, FlyingGravityAmt, FlyingGravBuildSpeed * 4f * d);
            else
                ActGravAmt = Mathf.Lerp(ActGravAmt, GlideGravityAmt, FlyingGravBuildSpeed * 0.5f * d);

            targetVelocity -= Vector3.up * ActGravAmt;
            //lerp velocity
            Vector3 dir = Vector3.Lerp(rigidBody.velocity, targetVelocity, d * FlyLerpSpd);

            Debug.Log("Calculated velocity:" + dir);
            rigidBody.velocity = dir;
            Debug.Log("Rigid body velocity:" + rigidBody.velocity);
        }

        Vector3 VehicleFlyingDownwardDirection(float d, float ZMove)
        {
            Vector3 VD = -ccTransform.up;

            //up and down input = moving up and down (this effects our downward direction
            if (ZMove > 0.1) //upward tilt
            {
                VD = Vector3.Lerp(VD, -ccTransform.forward, d * (FlyingUpDownSpeed * ZMove));
            }
            else if (ZMove < -.1) //downward tilt
            {
                VD = Vector3.Lerp(VD, ccTransform.forward, d * (FlyingUpDownSpeed * (ZMove * -1)));
            }

            //LB and RB input = roll (this effects our downward direction
            if (input.LB) //left roll
            {
                VD = Vector3.Lerp(VD, -ccTransform.right, d * FlyingRollSpeed);
            }
            else if (input.RB) //right roll
            {
                VD = Vector3.Lerp(VD, ccTransform.right, d * FlyingRollSpeed);
            }

            return VD;
        }

        Vector3 VehicleFlyingSideDirection(float d, float XMove)
        {
            Vector3 RollDir = ccTransform.forward;

            //rb lb = move left and right (this effects our target direction
            //left right input
            if (XMove > 0.1)
            {
                RollDir = Vector3.Lerp(RollDir, -ccTransform.right, d * (FlyingLeftRightSpeed * XMove));
            }
            else if (XMove < -.1)
            {
                RollDir = Vector3.Lerp(RollDir, ccTransform.right, d * (FlyingLeftRightSpeed * (XMove * -1)));
            }

            //bumper input
            if (input.LB)
            {
                RollDir = Vector3.Lerp(RollDir, -ccTransform.right, d * FlyingLeftRightSpeed * 0.2f);
            }
            else if (input.RB)
            {
                RollDir = Vector3.Lerp(RollDir, ccTransform.right, d * FlyingLeftRightSpeed * 0.2f);
            }

            return RollDir;
        }

        //rotate our upwards direction
        void RotateSelf(Vector3 Direction, float d, float GravitySpd)
        {
            Vector3 LerpDir = Vector3.Lerp(ccTransform.up, Direction, d * GravitySpd);
            ccTransform.rotation = Quaternion.FromToRotation(ccTransform.up, LerpDir) * ccTransform.rotation;
        }

        //rotate our left right direction
        void RotateMesh(float d, Vector3 LookDir, float spd)
        {
            Quaternion SlerpRot = Quaternion.LookRotation(LookDir, ccTransform.up);
            ccTransform.rotation = Quaternion.Slerp(ccTransform.rotation, SlerpRot, spd * d);
        }

        //rotate towards the velocity direction
        void RotateToVelocity(float d, float spd)
        {
            if (rigidBody.velocity.normalized.magnitude > 0)
            {
                Quaternion SlerpRot = Quaternion.LookRotation(rigidBody.velocity.normalized);
                ccTransform.rotation = Quaternion.Slerp(ccTransform.rotation, SlerpRot, spd * d);
            }
        }

        public void FixedUpdate() // Not sure why, but proxy won't execute FixedUpdateNetwork???
        {
            var animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (!animatorStateInfo.IsName("Flying FWD") &&
                !animatorStateInfo.IsName("TakeOff"))
            {
                Debug.Log("Current animation:" + animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
                Debug.Log("Switch to animation Flying FWD");
                animator.Play("Flying FWD");
            }            
        }
        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority)
            {
                var animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (!animatorStateInfo.IsName("Flying FWD"))
                {
                    boneRoot.position = ccTransform.position;
                }
                
                float delta = Runner.DeltaTime;
                float _xMov = input.Horizontal;
                float _zMov = input.Vertical;

                //get our direction of input based on camera position
                Vector3 screenMovementForward = CamY.transform.forward;
                Vector3 screenMovementRight = CamY.transform.right;
                Vector3 screenMovementUp = CamY.transform.up;

                Vector3 h = screenMovementRight * _xMov;
                Vector3 v = screenMovementForward * _zMov;

                Vector3 moveDirection = (v + h).normalized;

                if (ActionAirTimer > 0)
                    ActionAirTimer -= delta;

                if (FlyingAdjustmentLerp < 1.1)
                    FlyingAdjustmentLerp += delta * FlyingAdjustmentSpeed;

                //lerp speed
                float YAmt = rigidBody.velocity.y;
                float FlyAccel = FlyingAcceleration * FlyingAdjustmentLerp;
                float Spd = FlyingSpeed;

                if (!input.Fly) //we are not holding fly, slow down
                {
                    Spd = FlyingMinSpeed;
                    if (ActSpeed > FlyingMinSpeed)
                        FlyAccel = FlyingDecelleration * FlyingAdjustmentLerp;
                }

                HandleVelocity(delta, Spd, FlyAccel, YAmt);

                //flying controls
                FlyingCtrl(delta, ActSpeed, _xMov, _zMov);

                rigidbodyVelocity = rigidBody.velocity;
            }

            if (Runner.IsForward)
            {
                animator.SetFloat(speedFWD, rigidbodyVelocity.magnitude);
            }
        }
    }
}