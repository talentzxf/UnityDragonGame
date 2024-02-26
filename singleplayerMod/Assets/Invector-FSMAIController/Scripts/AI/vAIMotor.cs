using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Invector.vCharacterController.AI
{
    public enum vAIMovementSpeed
    {
        Idle, Walking, Running, Sprinting
    }

    [vClassHeader("AI Controller")]
    public abstract class vAIMotor : vCharacter, vEventSystems.vIAnimatorStateInfoController
    {
        #region AI VARIABLES

        #region Inspector Variables
        [vEditorToolbar("Start")]
        public bool disableControllerOnStart;

        [vEditorToolbar("Movement", order = 1)]
        [SerializeField] protected vAIMovementSpeed currentSpeed;
        [Tooltip("Change the velocity of all animations")]
        public float animatorSpeed = 1;
        [Tooltip("Smooth the  InputMagniture Animator parameter Update")]
        public float inputMagnitudeSmooth = 0.2f;
        [vHelpBox("When checked, make sure to reset the speed values to 1 to use the root motion original speed, increase or decrease this value to have extraSpeed", vHelpBoxAttribute.MessageType.Info)]
        [Tooltip("Turn off if you have 'in place' animations and use this values above to move the character, or use with root motion as extra speed")]
        public bool useRootMotion = false;
        public vMovementSpeed freeSpeed, strafeSpeed;


        [vHelpBox("Check this options only if the Agent needs to walk on complex meshes.", vHelpBoxAttribute.MessageType.Info)]
        [vEditorToolbar("Step Offset", order = 2)]
        [SerializeField]
        protected bool _useStepOffSet = true;
        [vHideInInspector("useStepOffSet")]
        [Tooltip("ADJUST IN PLAY MODE - Offset height limit for sters - GREY Raycast in front of the legs")]
        [SerializeField]
        protected float stepOffsetEnd = 0.45f;
        [vHideInInspector("useStepOffSet")]
        [Tooltip("ADJUST IN PLAY MODE - Offset height origin for sters, make sure to keep slight above the floor - GREY Raycast in front of the legs")]
        [SerializeField]
        protected float stepOffsetStart = 0.05f;
        [vHideInInspector("useStepOffSet")]
        [Tooltip("Higher value will result jittering on ramps, lower values will have difficulty on steps")]
        [SerializeField]
        protected float stepSmooth = 4f;


        [vEditorToolbar("Ground & Jump", order = 3)]
        [Tooltip("Make sure to bake the navmesh with the jump distance value higher then 0")]
        [SerializeField]
        protected float jumpSpeedPerMeter = 0.15f;
        [SerializeField] protected float jumpHeight = 0.75f;
        public float checkGroundDistance = 0.3f;
        [vHelpBox("Make sure to bake the navmesh and use the correct layer on your ground mesh.", vHelpBoxAttribute.MessageType.Info)]
        public LayerMask groundLayer = 1 << 0;


        [vEditorToolbar("Auto Crouch", order = 4)]
        public bool useAutoCrouch;
        [SerializeField]
        [Range(0, 1)]
        protected float headDetectStart = 0.4f;
        [SerializeField]
        protected float headDetectHeight = 0.4f;
        [SerializeField]
        protected float headDetectMargin = 0.02f;
        [vHideInInspector("useAutoCrouch")]
        public LayerMask autoCrouchLayer = 1 << 0;
        [SerializeField]
        protected bool debugAutoCrouch;


        [vEditorToolbar("Events")]
        public UnityEngine.Events.UnityEvent onEnableController;
        public UnityEngine.Events.UnityEvent onDisableController;
        #endregion

        #region Hide Inspector Variables

        [HideInInspector]
        public bool isStrafing { get; protected set; }
        public bool isGrounded { get; protected set; }

        [HideInInspector]
        public Rigidbody _rigidbody;
        [HideInInspector]
        public PhysicMaterial frictionPhysics, maxFrictionPhysics, slippyPhysics;
        [HideInInspector]
        public CapsuleCollider _capsuleCollider;
        [HideInInspector]
        public Vector3 targetDirection;
        [HideInInspector]
        public Vector3 input;
        [HideInInspector]
        public bool lockMovement, lockRotation, stopMove, isJumping, doingCustomAction;

        public vEventSystems.vAnimatorStateInfos animatorStateInfos { get; protected set; }

        public override bool isCrouching
        {
            get
            {
                return _isCrouching || _isCrouchingFromCast;
            }
            set
            {
                _isCrouching = value;
            }
        }

        public bool isRolling { get; protected set; }
        public vAIMovementSpeed movementSpeed { get { return currentSpeed; } protected set { currentSpeed = value; } }
        public bool useCustomRotationSpeed { get; set; }
        public float customRotationSpeed { get; set; }
        protected UnityEvent onUpdateAI = new UnityEvent();
        public UnityEvent OnUpdateAI { get { return onUpdateAI; } }
        protected bool _isStrafingRef;
        protected bool _isGroundedRef;
        protected float _verticalVelocityRef;
        protected float _groundDistanceRef;
        protected bool _isCrouchingRef;
        protected bool _isCrouchingFromCast;

        public virtual bool actions
        {
            get
            {
                return customAction || isRolling || isJumping;
            }
        }
        [HideInInspector]
        public bool customAction;
        #endregion

        #region Protected Variables
        protected float direction;
        protected float speed;
        protected float velocity;
        protected float strafeMagnitude;
        protected float rotationMagnitude;
        protected float colliderHeight;
        protected float verticalVelocity;
        protected RaycastHit groundHit;
        protected Quaternion freeRotation;
        protected Vector3 colliderCenter;
        protected Vector3 lastCharacterAngle;
        protected Vector3 temporaryDirection;
        protected float temporaryDirectionTime;
        #endregion

        #region Animator Variables   
        public AnimatorStateInfo baseLayerInfo, rightArmInfo, leftArmInfo, fullBodyInfo, upperBodyInfo, underBodyInfo;
        public virtual int baseLayer { get { return animator.GetLayerIndex("Base Layer"); } }
        public virtual int underBodyLayer { get { return animator.GetLayerIndex("UnderBody"); } }
        public virtual int rightArmLayer { get { return animator.GetLayerIndex("RightArm"); } }
        public virtual int leftArmLayer { get { return animator.GetLayerIndex("LeftArm"); } }
        public virtual int upperBodyLayer { get { return animator.GetLayerIndex("UpperBody"); } }
        public virtual int fullbodyLayer { get { return animator.GetLayerIndex("FullBody"); } }
        //[HideInInspector]
        //public bool triggerDieBehaviour;

        #endregion

        #endregion

        #region PROTECTED VIRTUAL METHODS.UNITY

        protected virtual void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                _capsuleCollider = GetComponent<CapsuleCollider>();
                colliderHeight = _capsuleCollider.height;
            }
            if (!debugAutoCrouch)
            {
                return;
            }

            var color = Color.green;
            // radius of SphereCast
            float radius = _capsuleCollider.radius + headDetectMargin;
            // Position of SphereCast origin stating in base of capsule
            Vector3 pos = transform.position + Vector3.up * colliderHeight * headDetectStart;
            // ray for SphereCast
            Ray ray2 = new Ray(pos, Vector3.up);

            if (!Application.isPlaying && Physics.SphereCast(ray2, radius, (headDetectHeight + (_capsuleCollider.radius)), autoCrouchLayer) || isCrouching)
            {
                color = Color.red;
            }
            else
            {
                color = Color.green;
            }

            color.a = 0.4f;
            Gizmos.color = color;
            Gizmos.DrawWireSphere(pos + Vector3.up * ((headDetectHeight + (_capsuleCollider.radius))), radius);
        }

        protected override void Start()
        {
            base.Start();
            animator = GetComponent<Animator>();

            if (animator)
            {
                animatorStateInfos = new vEventSystems.vAnimatorStateInfos(animator);
                animatorStateInfos.RegisterListener();

                hitDirectionHash = new vAnimatorParameter(animator, "HitDirection");
                reactionIDHash = new vAnimatorParameter(animator, "ReactionID");
                triggerReactionHash = new vAnimatorParameter(animator, "TriggerReaction");
                triggerResetStateHash = new vAnimatorParameter(animator, "ResetState");
                recoilIDHash = new vAnimatorParameter(animator, "RecoilID");
                triggerRecoilHash = new vAnimatorParameter(animator, "TriggerRecoil");
            }
            // slides the character through walls and edges
            frictionPhysics = new PhysicMaterial();
            frictionPhysics.name = "frictionPhysics";
            frictionPhysics.staticFriction = .25f;
            frictionPhysics.dynamicFriction = .25f;
            frictionPhysics.frictionCombine = PhysicMaterialCombine.Multiply;

            // prevents the collider from slipping on ramps
            maxFrictionPhysics = new PhysicMaterial();
            maxFrictionPhysics.name = "maxFrictionPhysics";
            maxFrictionPhysics.staticFriction = 1f;
            maxFrictionPhysics.dynamicFriction = 1f;
            maxFrictionPhysics.frictionCombine = PhysicMaterialCombine.Maximum;

            // air physics 
            slippyPhysics = new PhysicMaterial();
            slippyPhysics.name = "slippyPhysics";
            slippyPhysics.staticFriction = 0f;
            slippyPhysics.dynamicFriction = 0f;
            slippyPhysics.frictionCombine = PhysicMaterialCombine.Minimum;

            targetDirection = transform.forward;
            _rigidbody = GetComponent<Rigidbody>();
            _capsuleCollider = GetComponent<CapsuleCollider>();
            colliderCenter = _capsuleCollider.center;
            colliderHeight = _capsuleCollider.height;
            currentHealth = maxHealth;
            Collider[] AllColliders = this.GetComponentsInChildren<Collider>();
            for (int i = 0; i < AllColliders.Length; i++)
            {
                Physics.IgnoreCollision(_capsuleCollider, AllColliders[i]);
            }
            IsGroundedAnim = isGrounded = true;
            targetDirection = transform.forward;
            if (disableControllerOnStart)
            {
                DisableAIController();
            }

        }


        protected virtual void OnEnable()
        {
            if (animatorStateInfos != null && animator)
            {
                animatorStateInfos.RegisterListener();
            }
        }

        protected virtual void OnDisable()
        {
            if (animatorStateInfos != null && animator)
            {
                animatorStateInfos.RemoveListener();
            }
        }

        protected void Update()
        {
            UpdateAI();
        }

        #endregion

        #region PROTECTED VIRTUAL METHODS.Update AI

        protected virtual void UpdateAI()
        {
            UpdateLocomotion();
            UpdateAnimator();
            onUpdateAI.Invoke();
        }

        protected virtual void SetMovementInput(Vector3 input)
        {
            targetDirection = transform.TransformDirection(input).normalized;
            this.input = input;
        }

        protected virtual void SetMovementInput(Vector3 input, float smooth)
        {

            targetDirection = transform.TransformDirection(input).normalized;
            this.input = Vector3.Lerp(this.input, input, smooth * Time.deltaTime);
        }

        protected virtual void SetMovementInput(Vector3 input, Vector3 targetDirection, float smooth)
        {
            this.targetDirection = targetDirection.normalized;
            this.input = Vector3.Lerp(this.input, input, smooth * Time.deltaTime);
        }

        protected virtual void UpdateLocomotion()
        {
            StepOffset();
            ControlLocomotion();
            PhysicsBehaviour();
            CheckGroundDistance();
            CheckAutoCrouch();
        }

        protected virtual void ControlLocomotion()
        {
            if (isDead || isJumping || !isGrounded)
            {
                return;
            }

            CalculateRotationMagnitude();

            if (isStrafing)
            {
                StrafeMovement();
            }
            else
            {
                FreeMovement();
            }
            if (temporaryDirectionTime > 0) temporaryDirectionTime -= Time.deltaTime;

        }

        protected virtual void StrafeMovement()
        {
            StrafeLimitSpeed(maxSpeed);
            if (stopMove)
            {
                strafeMagnitude = 0f;
            }

            var rotDir = targetDirection.normalized;

            rotDir.y = 0;
            if (rotDir.magnitude > 0.1f && input.magnitude > 0.1f && !isRolling && !ragdolled)
            {
                if (!lockRotation)
                {
                    Rotate(rotDir);
                }
            }
            else
            {
                if (temporaryDirectionTime > 0)
                {
                    Rotate(temporaryDirection);
                }
            }

            animator.SetFloat("InputMagnitude", isJumping ? 0 : strafeMagnitude, inputMagnitudeSmooth, Time.deltaTime);
        }

        protected virtual void CalculateRotationMagnitude()
        {
            var eulerDifference = this.transform.eulerAngles - lastCharacterAngle;
            var magnitude = (eulerDifference.NormalizeAngle().y / (isStrafing ? strafeSpeed.rotationSpeed : freeSpeed.rotationSpeed));
            rotationMagnitude = magnitude;
            lastCharacterAngle = transform.eulerAngles;
        }

        protected virtual void StrafeLimitSpeed(float value)
        {
            var _speed = Mathf.Clamp(input.z, -maxSpeed, maxSpeed);
            var _direction = Mathf.Clamp(input.x, -maxSpeed, maxSpeed);
            speed = _speed;
            direction = _direction;
            var newInput = new Vector2(speed, direction);
            strafeMagnitude = Mathf.Clamp(newInput.magnitude, 0, maxSpeed);
        }

        protected virtual float rotationSpeed
        {
            get
            {
                if (lockRotation)
                {
                    return 0f;
                }
                else
                {
                    return useCustomRotationSpeed ? customRotationSpeed : isStrafing ? strafeSpeed.rotationSpeed : freeSpeed.rotationSpeed;
                }
            }
        }

        protected virtual void FreeMovement()
        {
            if (!animator)
            {
                return;
            }

            // set speed to both vertical and horizontal inputs
            speed = Mathf.Abs(input.x) + Mathf.Abs(input.z);
            //Limit speed by movementSpeedType
            speed = Mathf.Clamp(speed, 0, maxSpeed);
            if (stopMove)
            {
                speed = 0f;
            }

            animator.SetFloat("InputMagnitude", isJumping ? 0 : speed, inputMagnitudeSmooth, Time.deltaTime);

            var conditions = (!actions);

            if (input.magnitude > 0.1f && targetDirection.magnitude > 0.2f && conditions)
            {
                Vector3 lookDirection = targetDirection.normalized;
                freeRotation = Quaternion.LookRotation(lookDirection, transform.up);
                var eulerY = freeRotation.eulerAngles.y;
                var euler = new Vector3(transform.eulerAngles.x, eulerY, transform.eulerAngles.z);
                if (!lockRotation && !isRolling && speed > 0.1f && !ragdolled)
                {
                    Rotate(Quaternion.Euler(euler));
                }
            }
            else
            {
                if (temporaryDirectionTime > 0)
                {
                    Rotate(temporaryDirection);
                }

            }
        }

        protected virtual void Rotate(Vector3 targetDirection)
        {

            targetDirection.y = 0f;
            if (targetDirection.magnitude > 0.1f)
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetDirection, Vector3.up), rotationSpeed * Time.deltaTime);
        }

        protected virtual void Rotate(Quaternion targetRotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        protected virtual bool isSprinting
        {
            get
            {
                return movementSpeed == vAIMovementSpeed.Sprinting;
            }
        }

        protected virtual float maxSpeed
        {
            get
            {
                switch (movementSpeed)
                {
                    case vAIMovementSpeed.Idle:
                        return 0;
                    case vAIMovementSpeed.Walking:
                        return 0.5f;
                    case vAIMovementSpeed.Running:
                        return 1;
                    case vAIMovementSpeed.Sprinting:
                        return 1.5f;
                    default:
                        return 0;
                }
            }
        }

        protected virtual void StepOffset()
        {
            if (input.sqrMagnitude < 0.1 || !isGrounded || !_useStepOffSet || isJumping)
            {
                return;
            }

            var _hit = new RaycastHit();
            var _movementDirection = isStrafing && input.magnitude > 0 ? (transform.right * input.x + transform.forward * input.z).normalized : transform.forward;
            Ray rayStep = new Ray((transform.position + new Vector3(0, stepOffsetEnd, 0) + _movementDirection * ((_capsuleCollider).radius + 0.05f)), Vector3.down);

            if (Physics.Raycast(rayStep, out _hit, stepOffsetEnd - stepOffsetStart, groundLayer))
            {
                if (_hit.point.y >= (transform.position.y) && _hit.point.y <= (transform.position.y + stepOffsetEnd))
                {
                    var _speed = isStrafing ? Mathf.Clamp(input.magnitude, 0, 1f) : speed;
                    var velocityDirection = (_hit.point - transform.position);
                    var vel = _rigidbody.velocity;
                    vel.y = (velocityDirection * stepSmooth * (_speed * (velocity > 1 ? velocity : 1))).y;

                    _rigidbody.velocity = vel;
                }
            }
        }

        protected virtual void CheckGroundDistance()
        {
            if (_capsuleCollider != null && (_rigidbody.velocity.y > 0.1f || _rigidbody.velocity.y < -0.1f) || isJumping)
            {
                var dist = 10f;

                if (Physics.Raycast(transform.position + transform.up * _capsuleCollider.height * 0.5f, Vector3.down, out groundHit, _capsuleCollider.height, groundLayer))
                {
                    dist = transform.position.y - groundHit.point.y;
                }
                else if (Physics.SphereCast(transform.position + transform.up * _capsuleCollider.radius, _capsuleCollider.radius * 0.5f, Vector3.down, out groundHit, checkGroundDistance, groundLayer))
                {
                    dist = transform.position.y - groundHit.point.y;
                }

                GroundDistanceAnim = dist;

                if (dist >= checkGroundDistance)
                {
                    isGrounded = false;
                    verticalVelocity = _rigidbody.velocity.y;
                }
                if ((!actions || isJumping) && !isRolling && dist < checkGroundDistance * 0.9f)
                {
                    isGrounded = true;
                }

            }
            else if (!isJumping)
            {
                GroundDistanceAnim = 0f;
                isGrounded = true;
            }
        }

        protected virtual void PhysicsBehaviour()
        {
            if (isGrounded && input == Vector3.zero)
            {
                _capsuleCollider.material = maxFrictionPhysics;
            }
            else if (isGrounded && input != Vector3.zero)
            {
                _capsuleCollider.material = frictionPhysics;
            }
            else
            {
                _capsuleCollider.material = slippyPhysics;
            }
        }

        protected virtual void CheckAutoCrouch()
        {
            if (!useAutoCrouch)
            {
                return;
            }
            // radius of SphereCast
            float radius = _capsuleCollider.radius + headDetectMargin;
            // Position of SphereCast origin stating in base of capsule
            Vector3 pos = transform.position + Vector3.up * colliderHeight * headDetectStart;
            // ray for SphereCast
            Ray ray2 = new Ray(pos, Vector3.up);
            RaycastHit groundHit;
            // sphere cast around the base of capsule for check ground distance
            if (Physics.SphereCast(ray2, radius, out groundHit, (headDetectHeight + (_capsuleCollider.radius)), autoCrouchLayer))
            {
                if (!_isCrouchingFromCast)
                {
                    _isCrouchingFromCast = true;
                    _capsuleCollider.center = colliderCenter / 1.8f;
                    _capsuleCollider.height = colliderHeight / 1.8f;
                }
            }
            else if (_isCrouchingFromCast)
            {
                _isCrouchingFromCast = false;
                // back to the original values
                _capsuleCollider.center = colliderCenter;
                _capsuleCollider.height = colliderHeight;
            }
        }
        public override bool isDead
        {
            get => base.isDead;
            set
            {
                base.isDead = value;
                if (value)
                {
                    if (isGrounded)
                    {
                        if (_rigidbody) _rigidbody.isKinematic = true;
                        if (_capsuleCollider) _capsuleCollider.enabled = false;
                    }
                }
                else if (!ragdolled)
                {
                    if (_rigidbody) _rigidbody.isKinematic = false;
                    if (_capsuleCollider) _capsuleCollider.enabled = true;
                }
                if (animator) animator.SetBool("isDead", value);
            }
        }

        #endregion

        #region PROTECTED VIRTUAL METHODS.Update Animator

        protected virtual void UpdateAnimator()
        {
            if (animator == null || !animator.isActiveAndEnabled)
            {
                return;
            }

            animator.speed = animatorSpeed;
            AnimatorLayerControl();
            AnimatorLocomotion();
            AnimatorDeath();
            ActionsControl();
        }

        protected virtual bool IsStrafingAnim
        {
            get { return _isStrafingRef; }
            set
            {
                if (_isStrafingRef != value || animator.GetBool("IsStrafing") != value)
                {
                    _isStrafingRef = value;
                    animator.SetBool("IsStrafing", value);
                }
            }
        }

        protected virtual bool IsGroundedAnim
        {
            get { return _isGroundedRef; }
            set
            {
                if (_isGroundedRef != value)
                {
                    _isGroundedRef = value;
                    animator.SetBool("IsGrounded", value);
                }
            }
        }

        protected virtual bool IsCrouchingAnim
        {
            get { return _isCrouchingRef; }
            set
            {
                if (_isCrouchingRef != value)
                {
                    _isCrouchingRef = value;
                    animator.SetBool("IsCrouching", value);
                }
            }
        }

        protected virtual float GroundDistanceAnim
        {
            get { return _groundDistanceRef; }
            set
            {
                if (_groundDistanceRef != value)
                {
                    _groundDistanceRef = value;
                    animator.SetFloat("GroundDistance", value);
                }
            }
        }

        protected virtual float VerticalVelocityAnim
        {
            get { return _verticalVelocityRef; }
            set
            {
                if (_verticalVelocityRef != value)
                {
                    _verticalVelocityRef = value;
                    animator.SetFloat("VerticalVelocity", value);
                }
            }
        }

        protected virtual void AnimatorLocomotion()
        {
            var canMove = !stopMove && !lockMovement && !animatorStateInfos.HasTag("LockMovement");
            animator.SetFloat("InputHorizontal", canMove && isStrafing && !isSprinting ? direction : 0f, .2f, Time.deltaTime);
            animator.SetFloat("InputVertical", canMove ? speed : 0f, .2f, Time.deltaTime);

            IsStrafingAnim = isStrafing;
            IsGroundedAnim = isGrounded;
            IsCrouchingAnim = isCrouching;
            VerticalVelocityAnim = verticalVelocity;
        }

        protected virtual void AnimatorLayerControl()
        {
            if (baseLayer != -1)
            {
                baseLayerInfo = animator.GetCurrentAnimatorStateInfo(baseLayer);
            }

            if (underBodyLayer != -1)
            {
                underBodyInfo = animator.GetCurrentAnimatorStateInfo(underBodyLayer);
            }

            if (rightArmLayer != -1)
            {
                rightArmInfo = animator.GetCurrentAnimatorStateInfo(rightArmLayer);
            }

            if (leftArmLayer != -1)
            {
                leftArmInfo = animator.GetCurrentAnimatorStateInfo(leftArmLayer);
            }

            if (upperBodyLayer != -1)
            {
                upperBodyInfo = animator.GetCurrentAnimatorStateInfo(upperBodyLayer);
            }

            if (fullbodyLayer != -1)
            {
                fullBodyInfo = animator.GetCurrentAnimatorStateInfo(fullbodyLayer);
            }
        }

        protected virtual void AnimatorDeath()
        {
            if (!isDead) return;

            // death by animation
            if (deathBy == DeathBy.Animation)
            {
                int deadLayer = 0;
                var info = animatorStateInfos.GetStateInfoUsingTag("Dead");
                if (info != null)
                {
                    if (!animator.IsInTransition(deadLayer) && info.normalizedTime >= 0.99f && GroundDistanceAnim <= 0.15f)
                    {
                        RemoveComponents();
                    }
                }
            }
            // death by animation & ragdoll after a time
            else if (deathBy == DeathBy.AnimationWithRagdoll)
            {
                int deadLayer = 0;
                var info = animatorStateInfos.GetStateInfoUsingTag("Dead");
                if (info != null)
                {
                    if (!animator.IsInTransition(deadLayer) && info.normalizedTime >= 0.8f && GroundDistanceAnim <= 0.15f)
                    {
                        onActiveRagdoll.Invoke(null);
                    }
                }
            }
            // death by ragdoll
            else if (deathBy == DeathBy.Ragdoll)
            {
                onActiveRagdoll.Invoke(null);
            }
        }
        public virtual void RemoveComponents()
        {
            if (!removeComponentsAfterDie)
            {
                return;
            }

            if (_capsuleCollider != null)
            {
                Destroy(_capsuleCollider);
            }

            if (_rigidbody != null)
            {
                Destroy(_rigidbody);
            }

            if (animator != null)
            {
                Destroy(animator);
            }

            var comps = GetComponents<MonoBehaviour>();
            for (int i = 0; i < comps.Length; i++)
            {
                Destroy(comps[i]);
            }
        }

        protected virtual void ControlSpeed(float velocity)
        {
            if (Time.deltaTime == 0 || isJumping || _rigidbody.isKinematic)
            {
                return;
            }

            var canMove = !stopMove && !lockMovement && !animatorStateInfos.HasTag("LockMovement");
            if (!canMove)
            {
                velocity = 0;
            }

            if (useRootMotion && !actions && !customAction && canMove)
            {
                this.velocity = velocity;
                var deltaPosition = new Vector3(animator.deltaPosition.x, transform.position.y, animator.deltaPosition.z);
                Vector3 v = (deltaPosition * (velocity > 0 ? velocity : 1f)) / Time.deltaTime;
                v.y = _rigidbody.velocity.y;
                _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, v, 20f * Time.deltaTime);
            }
            else if (actions || isDead || !canMove || customAction)
            {
                this.velocity = velocity;
                Vector3 v = Vector3.zero;
                v.y = _rigidbody.velocity.y;
                _rigidbody.velocity = v;
                transform.position = animator.rootPosition;
            }
            else
            {
                if (isStrafing)
                {
                    Vector3 v = (transform.TransformDirection(new Vector3(input.x, 0, input.z)) * (velocity > 0 ? velocity : 1f));
                    v.y = _rigidbody.velocity.y;
                    _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, v, 20f * Time.deltaTime);
                }
                else
                {
                    var _targetVelocity = transform.forward * velocity * speed;
                    _targetVelocity.y = _rigidbody.velocity.y;
                    _rigidbody.velocity = _targetVelocity;
                }
            }
        }

        protected virtual void ActionsControl()
        {
            // to have better control of your actions, you can filter the animations state using bools 
            // this way you can know exactly what animation state the character is playing           

            isRolling = baseLayerInfo.IsName("Roll");

            // locks player movement while a animation with tag 'LockMovement' is playing
            UpdateLockMovement();
            UpdateLockRotation();
            // ! -- you can add the Tag "CustomAction" into a AnimatonState and the character will not perform any Melee action -- !            
            UpdateCustomAction();
        }

        protected virtual void UpdateLockMovement()
        {
            lockMovement = IsAnimatorTag("LockMovement");
        }

        protected virtual void UpdateLockRotation()
        {
            lockRotation = IsAnimatorTag("LockRotation");
        }

        public virtual void UpdateCustomAction()
        {
            customAction = IsAnimatorTag("CustomAction");
        }

        protected virtual void OnAnimatorMove()
        {
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (animator && isGrounded && !ragdolled)
            {
                // use root rotation for custom actions or 
                if (customAction || input.magnitude < 0.1f)
                {
                    //_rigidbody.velocity = Vector3.zero;
                    _rigidbody.position = animator.rootPosition;
                    transform.rotation = animator.rootRotation;

                    return;
                }
                if (lockMovement)
                {
                    //_rigidbody.velocity = Vector3.zero;
                    _rigidbody.position = animator.rootPosition;
                    if (lockRotation)
                    {
                        transform.rotation = animator.rootRotation;
                    }

                    return;
                }
                if (lockRotation)
                {
                    _rigidbody.rotation = animator.rootRotation;
                }

                var a_strafeSpeed = Mathf.Abs(strafeMagnitude);

                // strafe extra speed
                if (isStrafing)
                {
                    if (a_strafeSpeed <= 0.5f)
                    {
                        ControlSpeed(strafeSpeed.walkSpeed);
                    }
                    else if (a_strafeSpeed > 0.5f && a_strafeSpeed <= 1f)
                    {
                        ControlSpeed(strafeSpeed.runningSpeed);
                    }
                    else
                    {
                        ControlSpeed(strafeSpeed.sprintSpeed);
                    }

                    if (isCrouching)
                    {
                        ControlSpeed(strafeSpeed.crouchSpeed);
                    }
                }
                else if (!isStrafing)
                {
                    // free extra speed                
                    if (speed <= 0.5f)
                    {
                        ControlSpeed(freeSpeed.walkSpeed);
                    }
                    else if (speed > 0.5 && speed <= 1f)
                    {
                        ControlSpeed(freeSpeed.runningSpeed);
                    }
                    else
                    {
                        ControlSpeed(freeSpeed.sprintSpeed);
                    }

                    if (isCrouching)
                    {
                        ControlSpeed(freeSpeed.crouchSpeed);
                    }
                }
            }
        }

        #endregion

        #region PUBLIC VIRTUAL METHODS. AI

        /// <summary>
        /// Check  Current Animator State tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public virtual bool IsAnimatorTag(string tag)
        {
            if (animator == null)
            {
                return false;
            }

            if (animatorStateInfos.HasTag(tag))
            {
                return true;
            }

            if (baseLayerInfo.IsTag(tag))
            {
                return true;
            }

            if (underBodyInfo.IsTag(tag))
            {
                return true;
            }

            if (rightArmInfo.IsTag(tag))
            {
                return true;
            }

            if (leftArmInfo.IsTag(tag))
            {
                return true;
            }

            if (upperBodyInfo.IsTag(tag))
            {
                return true;
            }

            if (fullBodyInfo.IsTag(tag))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Set Movement speed to 0 (zero)
        /// </summary>
        public virtual void Stop()
        {
            //targetDirection = transform.forward;
            if (input != Vector3.zero)
            {
                // _turnOnSpotDirection = transform.forward;
                input = Vector3.zero;
            }

            movementSpeed = vAIMovementSpeed.Idle;
        }

        /// <summary>
        /// Set Movement speed to WalkSpeed
        /// </summary>
        public virtual void Walk()
        {
            movementSpeed = vAIMovementSpeed.Walking;
        }

        /// <summary>
        /// Set Movement speed to RunSpeed
        /// </summary>
        public virtual void Run()
        {
            movementSpeed = vAIMovementSpeed.Running;
        }

        /// <summary>
        /// Set Movement speed to SprintSpeed
        /// </summary>
        public virtual void Sprint()
        {
            movementSpeed = vAIMovementSpeed.Sprinting;
        }

        /// <summary>
        /// Jump To target point
        /// </summary>
        /// <param name="jumpTarget">target point</param>
        public virtual void JumpTo(Vector3 jumpTarget)
        {
            //  if (animator.IsInTransition(0)) return;
            if (isGrounded && !lockMovement && !actions && !isJumping)
            {
                animator.CrossFadeInFixedTime("JumpMove", .1f);
                StartCoroutine(JumpParabole(jumpTarget, jumpHeight, jumpSpeedPerMeter));
                //// isStrafing = false;
                //targetDirection = jumpTarget - transform.position;
                //_rigidbody.velocity = Vector3.zero;
                //var jumpDistance = Vector3.Distance(transform.position, jumpTarget) + (_capsuleCollider.radius * 2f);
                //var dir = targetDirection;
                //dir.y = 0;
                //transform.rotation = Quaternion.LookRotation(dir);
                //var force = GetJumpForce(jumpTarget + targetDirection.normalized * _capsuleCollider.radius, jumpDistance * jumpSpeedPerMeter);
                //_rigidbody.AddForce(force, ForceMode.VelocityChange);
                //canExitJump = false;
                //isJumping = true;
                //StartCoroutine(ResetJump());
            }
        }

        protected virtual IEnumerator JumpParabole(Vector3 targetPos, float height, float duration)
        {
            animator.CrossFadeInFixedTime("JumpMove", .1f);
            isJumping = true;
            Vector3 startPos = transform.position;
            Vector3 endPos = targetPos;
            float normalizedTime = 0.0f;
            Vector3 jumpDir = targetPos - transform.position;
            jumpDir.y = 0;
            while (normalizedTime < 1.0f)
            {
                float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
                transform.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
                normalizedTime += Time.deltaTime / duration;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(jumpDir), freeSpeed.rotationSpeed * Time.deltaTime);
                yield return null;
            }
            isJumping = false;
        }

        /// <summary>
        /// Roll to target point
        /// </summary>
        /// <param name="direction">target point</param>
        public virtual void RollTo(Vector3 direction)
        {
            if (!isRolling && !isJumping && isGrounded && !lockMovement && !customAction)
            {
                targetDirection = direction.normalized;
                targetDirection.y = 0;
                animator.SetTrigger("ResetState");
                animator.CrossFadeInFixedTime("Roll", 0.01f);
                transform.rotation = Quaternion.LookRotation(targetDirection);
            }
        }

        /// <summary>
        /// Set Strafe Locomotion type
        /// </summary>
        public virtual void SetStrafeLocomotion()
        {
            isStrafing = true;
        }

        /// <summary>
        /// Set Free Locomotion type
        /// </summary>
        public virtual void SetFreeLocomotion()
        {
            isStrafing = false;
        }

        public virtual void EnableAIController()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            _rigidbody.isKinematic = false;
            _capsuleCollider.isTrigger = false;
            enabled = true;
            onEnableController.Invoke();
        }

        public virtual void DisableAIController()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            targetDirection = transform.forward;
            input = Vector3.zero;
            movementSpeed = vAIMovementSpeed.Idle;
            if (animator.isActiveAndEnabled)
            {
                animator.SetFloat("InputHorizontal", 0f);
                animator.SetFloat("InputVertical", 0f);
                animator.SetFloat("InputMagnitude", 0f);
            }
            //_rigidbody.velocity = Vector3.zero;
            _rigidbody.isKinematic = true;
            //_capsuleCollider.isTrigger = true;
            enabled = false;
            onDisableController.Invoke();
        }

        #endregion

        #region OVERRIDE METHODS. HealthController/ICharacter interface

        public override void TakeDamage(vDamage damage)
        {
            base.TakeDamage(damage);
            if (damage.damageValue > 0)
            {
                TriggerDamageRection(damage);
            }
        }

        protected virtual void TriggerDamageRection(vDamage damage)
        {
            if (!isRolling)
            {
                if (animator != null && animator.enabled && !damage.activeRagdoll && currentHealth > 0)
                {
                    if (hitDirectionHash.isValid)
                    {
                        animator.SetInteger(hitDirectionHash, (int)transform.HitAngle(damage.sender.position));
                    }
                    // trigger hitReaction animation
                    if (damage.hitReaction)
                    {
                        // set the ID of the reaction based on the attack animation state of the attacker - Check the MeleeAttackBehaviour script
                        if (reactionIDHash.isValid)
                        {
                            animator.SetInteger(reactionIDHash, damage.reaction_id);
                        }

                        if (triggerReactionHash.isValid)
                        {
                            animator.SetTrigger(triggerReactionHash);
                        }

                        if (triggerResetStateHash.isValid)
                        {
                            animator.SetTrigger(triggerResetStateHash);
                        }
                    }
                    else
                    {
                        if (recoilIDHash.isValid)
                        {
                            animator.SetInteger(recoilIDHash, damage.recoil_id);
                        }

                        if (triggerRecoilHash.isValid)
                        {
                            animator.SetTrigger(triggerRecoilHash);
                        }

                        if (triggerResetStateHash.isValid)
                        {
                            animator.SetTrigger(triggerResetStateHash);
                        }
                    }
                }
                if (damage.activeRagdoll)
                {
                    onActiveRagdoll.Invoke(damage);
                }
            }
        }

        public override void ResetRagdoll()
        {
            lockMovement = false;
            verticalVelocity = 0f;
            ragdolled = false;
            _rigidbody.WakeUp();
            _rigidbody.useGravity = true;
            _rigidbody.isKinematic = false;
            _capsuleCollider.enabled = true;
        }

        public override void EnableRagdoll()
        {
            animator.SetFloat("InputHorizontal", 0f);
            animator.SetFloat("InputVertical", 0f);
            animator.SetFloat("VerticalVelocity", 0f);
            ragdolled = true;
            _capsuleCollider.enabled = false;
            _rigidbody.useGravity = false;
            _rigidbody.isKinematic = true;
            lockMovement = true;
        }

        #endregion

        [System.Serializable]
        public class vMovementSpeed
        {
            [Tooltip("Rotation speed of the character")]
            public float rotationSpeed = 10f;
            [Tooltip("Speed to Walk using rigibody force or extra speed if you're using RootMotion")]
            public float walkSpeed = 2f;
            [Tooltip("Speed to Run using rigibody force or extra speed if you're using RootMotion")]
            public float runningSpeed = 3f;
            [Tooltip("Speed to Sprint using rigibody force or extra speed if you're using RootMotion")]
            public float sprintSpeed = 4f;
            [Tooltip("Speed to Crouch using rigibody force or extra speed if you're using RootMotion")]
            public float crouchSpeed = 2f;
        }
    }
}
