using UnityEngine;
using UnityEngine.Events;

namespace Invector.vCharacterController.vActions
{
    /// <summary>
    /// vSwimming Add-on
    /// On this Add-on we're locking the tpInput along with the tpMotor, tpAnimator & tpController methods to handle the Swimming behaviour.
    /// We can still access those scripts and methods, and call just what we need to use for example the FreeMovement, CameraInput, StaminaRecovery and UpdateHUD methods    
    /// This way the add-on become modular and plug&play easy to modify without changing the core of the controller. 
    /// </summary>

    [vClassHeader("Swimming Action")]
    public class vSwimming : vActionListener
    {
        #region Swimming Variables

        [vEditorToolbar("Settings")]
        [Tooltip("Name of the tag assign into the Water object")]
        public string waterTag = "Water";

        [Header("Speed & Extra Options")]
        [Tooltip("Uncheck if you don't want to go under water")]
        public bool swimUpAndDown = true;
        [Tooltip("Speed to swim forward")]
        public float swimForwardSpeed = 4f;
        [Tooltip("Speed to rotate the character")]
        public float swimRotationSpeed = 4f;
        [Tooltip("Smooth value for the character movement")]
        public float swimMovementSmooth = 2f;
        [Tooltip("Smooth value for the character animation transition")]
        public float swimAnimationSmooth = 1f;
        [Tooltip("Smooth value for the character movement up and down")]
        public float swimUpDownSmooth = 2f;

        [vHelpBox("! Assign a curve here, otherwise the character won't move up or down !")]
        public AnimationCurve updownSmoothCurve;

        [Tooltip("Speed to swim up")]
        public float swimUpSpeed = 3f;
        [Tooltip("Speed to swim down")]
        public float swimDownSpeed = -3f;
        [Tooltip("Increase the radius of the capsule collider to avoid enter walls")]
        public float colliderRadius = .5f;
        [Tooltip("Increase the radius of the capsule collider to avoid enter walls")]
        public float colliderHeight = .5f;
        [Tooltip("Height offset to match the character Y position")]
        public float heightOffset = .3f;

        [Header("Health/Stamina Consumption")]
        [Tooltip("Leave with 0 if you don't want to use stamina consumption")]
        public float stamina = 15f;
        [Tooltip("How much health will drain after all the stamina were consumed")]
        public int healthConsumption = 1;

        [Header("Particle Effects")]
        public GameObject impactEffect;
        [Tooltip("Check the Rigibody.Y of the character to trigger the ImpactEffect Particle")]
        public float velocityToImpact = -4f;
        public GameObject waterRingEffect;
        [Tooltip("Frequency to instantiate the WaterRing effect while standing still")]
        public float waterRingFrequencyIdle = .8f;
        [Tooltip("Frequency to instantiate the WaterRing effect while swimming")]
        public float waterRingFrequencySwim = .15f;
        [Tooltip("Instantiate a prefab when exit the water")]
        public GameObject waterDrops;
        [Tooltip("Y Offset based at the capsule collider")]
        public float waterDropsYOffset = 1.6f;

        [Header("Inputs")]
        [Tooltip("Input to make the character go up")]
        public GenericInput swimUpInput = new GenericInput("Space", "X", "X");
        [Tooltip("Input to make the character go down")]
        public GenericInput swimDownInput = new GenericInput("LeftShift", "Y", "Y");

        [vEditorToolbar("Events")]
        public UnityEvent OnEnterWater;
        public UnityEvent OnExitWater;
        public UnityEvent OnAboveWater;
        public UnityEvent OnUnderWater;

        [vEditorToolbar("Debug"), Tooltip("Debug Mode will show the current behavior at the console window")]
        public bool debugMode;
        [vReadOnly(false)]
        public float currentCharacterDepth;
        [vReadOnly(false)]
        public GameObject water;
        [vReadOnly(false)]
        [SerializeField] protected bool _isSwimming = false;
        public virtual bool isSwimming { get { return _isSwimming; } set { _isSwimming = value; } }
        [vReadOnly(false)]
        public bool inTheWater;
        [vReadOnly(false), SerializeField]
        public bool isUnderWater;

        protected float swimUpInterpolate;
        protected float swimDownInterpolate;
        protected float waterHeightLevel;
        protected vThirdPersonInput tpInput;
        protected float timer;
        protected float originalMoveSpeed;
        protected float originalRotationSpeed;
        protected float originalMovementSmooth;
        protected float originalAnimationSmooth;
        protected float waterRingSpawnFrequency;
        // bools to trigger a method once on a update
        protected bool triggerSwimState;
        protected bool triggerUnderWater;
        protected bool triggerAboveWater;

        #endregion

        protected override void Start()
        {
            base.Start();
            tpInput = GetComponentInParent<vThirdPersonInput>();
            if (tpInput)
            {
                tpInput.onUpdate -= UpdateSwimmingBehavior;
                tpInput.onUpdate += UpdateSwimmingBehavior;
            }
        }

        protected override void SetUpListener()
        {
            ///Set what kind of action need to use;
            actionEnter = true;///Use Trigger Enter
            actionExit = true; ///Use Trigger Exit
            actionStay = false;///Ignore Trigger Stay
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (!water)
            {
                return;
            }

            var matrix = Gizmos.matrix;
            var position = new Vector3(transform.position.x, waterHeightLevel, transform.position.z);
            Gizmos.color = Color.blue * 0.8f;
            Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.identity, new Vector3(1, 0.001f, 1));
            Gizmos.DrawWireSphere(Vector3.zero, .5f);
            Gizmos.matrix = matrix;
            Gizmos.color = Color.green * 0.8f;
            Gizmos.DrawLine(position, characterCenter);
            Gizmos.matrix = Matrix4x4.TRS(characterCenter, Quaternion.identity, new Vector3(1, 0.001f, 1));
            Gizmos.DrawWireSphere(Vector3.zero, .25f);
        }

        protected virtual void UpdateSwimmingBehavior()
        {
            if (!inTheWater)
            {
                return;
            }

            UnderWaterBehaviour();
            SwimmingBehaviour();
        }

        protected virtual void SwimmingBehaviour()
        {
            if (water)
            {
                waterHeightLevel = water.transform.position.y;
                currentCharacterDepth = -(characterCenter.y - waterHeightLevel);
                isUnderWater = currentCharacterDepth > 0.5f;
                isSwimming = isSwimming ? currentCharacterDepth >= -0.2f : currentCharacterDepth >= 0;
            }

            // trigger swim behaviour only if the water level matches the player height + offset
            if (isSwimming)
            {
                if (tpInput.cc.currentHealth > 0)
                {
                    if (!triggerSwimState)
                    {
                        EnterSwimState();                                   // call once the swim behaviour
                    }

                    SwimUpOrDownInput();                                    // input to swim up or down
                    tpInput.SetStrafeLocomotion(false);                     // limit the player to not go on strafe mode
                    tpInput.MoveInput();                                    // update the input
                    tpInput.cc.SetAnimatorMoveSpeed(tpInput.cc.freeSpeed);  // update the animator input magnitude
                }
                //else
                //{
                //    ExitSwimState();                                        // use the trigger around the edges to exit by playing an animation                                     
                //}
            }
            else
            {
                ExitSwimState();
            }
        }

        protected virtual void UnderWaterBehaviour()
        {
            if (isUnderWater)
            {
                StaminaConsumption();

                if (!triggerUnderWater)
                {
                    tpInput.cc.colliderRadius = colliderRadius;
                    tpInput.cc.colliderHeight = colliderHeight;
                    triggerUnderWater = true;
                    triggerAboveWater = false;
                    OnUnderWater.Invoke();
                }
            }
            else
            {
                WaterRingEffect();
                if (!triggerAboveWater && triggerSwimState)
                {
                    tpInput.cc.ResetCapsule();
                    triggerUnderWater = false;
                    triggerAboveWater = true;
                    OnAboveWater.Invoke();
                }
            }
        }

        protected virtual void StaminaConsumption()
        {
            if (tpInput.cc.currentStamina <= 0)
            {
                tpInput.cc.ChangeHealth(-healthConsumption);
            }
            else
            {
                tpInput.cc.ReduceStamina(stamina, true);            // call the ReduceStamina method from the player
                tpInput.cc.currentStaminaRecoveryDelay = 0.25f;     // delay to start recovery stamina           
            }
        }

        public override void OnActionEnter(Collider other)
        {
            if (other.gameObject.CompareTag(waterTag) && !tpInput.cc.customAction)
            {
                if (debugMode)
                {
                    Debug.Log("Player enter the Water");
                }

                inTheWater = true;
                water = other.gameObject;
                waterHeightLevel = other.transform.position.y;
                originalMoveSpeed = tpInput.cc.moveSpeed;
                originalRotationSpeed = tpInput.cc.freeSpeed.rotationSpeed;
                originalAnimationSmooth = tpInput.cc.freeSpeed.animationSmooth;
                originalMovementSmooth = tpInput.cc.freeSpeed.movementSmooth;

                if (tpInput.cc.verticalVelocity <= velocityToImpact && impactEffect)
                {
                    var newPos = new Vector3(transform.position.x, other.transform.position.y, transform.position.z);
                    Instantiate(impactEffect, newPos, tpInput.transform.rotation).transform.SetParent(vObjectContainer.root, true); ;
                }
            }
        }

        public override void OnActionExit(Collider other)
        {
            if (other.gameObject.CompareTag(waterTag))
            {
                if (debugMode)
                {
                    Debug.Log("Player left the Water");
                }

                if (other.gameObject == water)
                {
                    water = null;
                    inTheWater = false;
                    isSwimming = false;
                    ExitSwimState();
                    if (waterDrops)
                    {
                        var newPos = new Vector3(transform.position.x, transform.position.y + waterDropsYOffset, transform.position.z);
                        GameObject myWaterDrops = Instantiate(waterDrops, newPos, tpInput.transform.rotation);
                        myWaterDrops.transform.parent = transform;
                    }
                }

            }
        }

        protected virtual void EnterSwimState()
        {
            if (debugMode)
            {
                Debug.Log("Player is Swimming");
            }
            triggerUnderWater = false;
            triggerAboveWater = false;
            triggerSwimState = true;
            OnEnterWater.Invoke();
            tpInput.SetLockAllInput(true);
            tpInput.cc.disableCheckGround = true;
            tpInput.cc.disableSnapToGround = true;
            tpInput.cc.lockSetMoveSpeed = true;
            tpInput.cc.moveSpeed = swimForwardSpeed;
            tpInput.cc.freeSpeed.rotationSpeed = swimRotationSpeed;
            tpInput.cc.freeSpeed.animationSmooth = swimAnimationSmooth;
            tpInput.cc.freeSpeed.movementSmooth = swimMovementSmooth;
            ResetPlayerValues();
            tpInput.cc.animator.CrossFadeInFixedTime("Swimming", 0.25f);
            tpInput.cc._rigidbody.useGravity = false;
            tpInput.cc._rigidbody.drag = 10f;
            tpInput.cc._capsuleCollider.isTrigger = false;
        }

        protected virtual void ExitSwimState()
        {
            if (!triggerSwimState)
            {
                return;
            }

            if (debugMode)
            {
                Debug.Log("Player Stop Swimming");
            }
            isUnderWater = false;
            triggerSwimState = false;
            OnExitWater.Invoke();
            tpInput.SetLockAllInput(false);
            tpInput.cc.disableCheckGround = false;
            tpInput.cc.lockSetMoveSpeed = false;
            tpInput.cc.disableSnapToGround = false;
            tpInput.cc.moveSpeed = originalMoveSpeed;
            tpInput.cc.freeSpeed.rotationSpeed = originalRotationSpeed;
            tpInput.cc.freeSpeed.animationSmooth = originalAnimationSmooth;
            tpInput.cc.freeSpeed.movementSmooth = originalMovementSmooth;
            tpInput.cc.animator.SetInteger(vAnimatorParameters.ActionState, 0);
            tpInput.cc.ResetCapsule();
            tpInput.cc._rigidbody.useGravity = true;
            tpInput.cc._rigidbody.drag = 0f;
        }

        protected virtual void SwimUpOrDownInput()
        {
            if (tpInput.cc.customAction)
            {
                return;
            }
            ///Check if Can move UP
            var upConditions = (currentCharacterDepth > 0.2f);
            var input = tpInput.cc.input;
            if ((swimUpInput.GetButton() || swimUpAndDown == false) && upConditions)  //Conditions to Move UP         
            {
                if (debugMode)
                {
                    Debug.Log("Player Swimming UP");
                }

                ///Set Velocity do Move UP
                swimDownInterpolate = 0f;
                swimUpInterpolate += Time.deltaTime * swimUpDownSmooth;
                swimUpInterpolate = Mathf.Clamp(swimUpInterpolate, 0f, 1f);

                var vel = tpInput.cc._rigidbody.velocity;
                vel.y = Mathf.Lerp(vel.y, swimUpSpeed, updownSmoothCurve.Evaluate(swimUpInterpolate));
                tpInput.cc._rigidbody.velocity = vel;

                ///Set input Y to force character movement;

                input.y = 1f;

                ///Change Action State to Swim Down
                tpInput.cc.animator.SetInteger(vAnimatorParameters.ActionState, 4);
            }
            else if (swimDownInput.GetButton() && swimUpAndDown)//Conditions to Move Down        
            {
                if (debugMode)
                {
                    Debug.Log("Player Swimming Down");
                }
                swimUpInterpolate = 0f;
                swimDownInterpolate += Time.deltaTime * swimUpDownSmooth;
                swimDownInterpolate = Mathf.Clamp(swimDownInterpolate, 0f, 1f);

                var vel = tpInput.cc._rigidbody.velocity;
                vel.y = Mathf.Lerp(vel.y, swimDownSpeed, updownSmoothCurve.Evaluate(swimDownInterpolate));
                tpInput.cc._rigidbody.velocity = vel;


                ///Set input Y to force charter movement;
                input.y = -1f;
                ///Change Action State to Swim Down
                tpInput.cc.animator.SetInteger(vAnimatorParameters.ActionState, 3);
            }
            else
            {
                ///Reset Input Y and Character Vel Y;
                input.y = 0f;
                swimDownInterpolate = 0f;
                swimUpInterpolate = 0f;
                var vel = tpInput.cc._rigidbody.velocity;
                vel.y = Mathf.Lerp(vel.y, 0f, swimUpDownSmooth * Time.deltaTime);
                tpInput.cc._rigidbody.velocity = vel;

                if (isUnderWater)
                {
                    ///Change Action State to Under Water         
                    tpInput.cc.animator.SetInteger(vAnimatorParameters.ActionState, 2);
                }
                else
                {
                    ///Keep Character in WaterHeightLevel               
                    var positionInWaterSurface = transform.position;
                    positionInWaterSurface.y = waterHeightLevel - (tpInput.cc.colliderHeightDefault * 0.5f + heightOffset);
                    transform.position = Vector3.Lerp(transform.position, positionInWaterSurface, 0.5f * Time.deltaTime);
                    ///Change Action State to Above Water
                    tpInput.cc.animator.SetInteger(vAnimatorParameters.ActionState, 1);
                }
            }
            tpInput.cc.input = input;
        }

        protected virtual void WaterRingEffect()
        {
            if (!waterRingEffect)
            {
                return;
            }
            // switch between waterRingFrequency for idle and swimming
            if (tpInput.cc.input != Vector3.zero)
            {
                waterRingSpawnFrequency = waterRingFrequencySwim;
            }
            else
            {
                waterRingSpawnFrequency = waterRingFrequencyIdle;
            }

            // counter to instantiate the waterRingEffects using the current frequency
            timer += Time.deltaTime;
            if (timer >= waterRingSpawnFrequency)
            {
                var newPos = new Vector3(transform.position.x, waterHeightLevel, transform.position.z);
                Instantiate(waterRingEffect, newPos, tpInput.transform.rotation).transform.SetParent(vObjectContainer.root, true);
                timer = 0f;
            }
        }

        protected virtual void ResetPlayerValues()
        {
            tpInput.cc.isJumping = false;
            tpInput.cc.isSprinting = false;
            tpInput.cc.isCrouching = false;
            tpInput.cc.animator.SetFloat(vAnimatorParameters.InputHorizontal, 0);
            tpInput.cc.animator.SetFloat(vAnimatorParameters.InputVertical, 0);
            tpInput.cc.animator.SetInteger(vAnimatorParameters.ActionState, 1);
            tpInput.cc.isGrounded = true;                                         // ground the character so that we can run the root motion without any issues
            tpInput.cc.animator.SetBool(vAnimatorParameters.IsGrounded, true);    // also ground the character on the animator so that he won't float after finishes the climb animation
            tpInput.cc.verticalVelocity = 0f;
        }

        public virtual Vector3 characterCenter
        {
            get
            {
                return transform.position + Vector3.up * (tpInput.cc.colliderHeightDefault * 0.5f + heightOffset);
            }
        }

    }
}