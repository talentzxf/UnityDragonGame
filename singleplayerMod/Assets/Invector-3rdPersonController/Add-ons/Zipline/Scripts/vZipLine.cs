using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Invector.vCharacterController.vActions
{
    /// <summary>
    /// vZipline Add-on
    /// On this Add-on we're locking the tpInput and disabling it, so we can manipulate some variables of the controller and create the zipline behaviour.
    /// We can still access those scripts and methods, and call just what we need to use for example the CameraInput method
    /// This way the add-on become modular and plug&play easy to modify without changing the core of the controller.
    /// </summary>

    [vClassHeader("Zipline Action")]
    public class vZipLine : vActionListener
    {
        #region Zipline Variables
        [vEditorToolbar("Settings")]
        [Tooltip("Name of the tag assign into the Zipline object")]
        public string ziplineTag = "Zipline";

        public ZiplineSettings defaultSettings;
        public List<ZiplineSettings> customSettings;
        internal ZiplineSettings currentSettings;

        public virtual float maxSpeed => currentSettings != null ? currentSettings.maxSpeed : defaultSettings.maxSpeed;
        public virtual string animationClip => currentSettings != null ? currentSettings.animationClip : defaultSettings.animationClip;
        public virtual GameObject ziplineHandler => currentSettings != null ? currentSettings.ziplineHandler : defaultSettings.ziplineHandler;
        public virtual bool lockCharacterRotation => currentSettings != null ? currentSettings.lockCharacterRotation : defaultSettings.lockCharacterRotation;

#if INVECTOR_MELEE
        public virtual bool lockMeleeInput => currentSettings != null ? currentSettings.lockMeleeInput : defaultSettings.lockMeleeInput;
#endif
#if INVECTOR_SHOOTER
        public virtual bool lockShooterInput => currentSettings != null ? currentSettings.lockShooterInput : defaultSettings.lockShooterInput;
#endif

        [vEditorToolbar("Input")]
        public GenericInput enterZipline = new GenericInput("E", false, "A", false, "A", false);
        public GenericInput exitZipline = new GenericInput("Space", false, "X", false, "X", false);
        [vEditorToolbar("Events")]
        public UnityEvent onZiplineEnter;
        public UnityEvent onZiplineUsing;
        public UnityEvent onZiplineExit;

        [vEditorToolbar("Debug")]
        [Tooltip("Debug Mode will show the current behavior at the console window")]
        public bool debugMode;
        [vReadOnly, SerializeField]
        protected bool _isUsingZipline;
        [vReadOnly, SerializeField]
        protected bool _inExitZipline;
        [vReadOnly, SerializeField]
        protected float _currentSpeed;

        protected virtual vZiplineAnchorPoints anchorPoint { get; set; }
        protected virtual Transform handlerParent { get; set; }
        protected virtual Vector3 handlerParentLocalPosition { get; set; }
        protected virtual Quaternion handlerParentLocalRotation { get; set; }
        protected virtual RigidbodyConstraints originalConstrains { get; set; }
        /// <summary>
        /// Access to <seealso cref="vThirdPersonInput"/>
        /// </summary>
        public virtual vThirdPersonInput inputController { get; protected set; }
        /// <summary>
        /// current velocity of the character in a zipline
        /// </summary>
        public virtual float currentSpeed { get => _currentSpeed; protected set => _currentSpeed = value; }
        /// <summary>
        /// Is using a zipline
        /// </summary>
        public virtual bool isUsingZipline { get => _isUsingZipline; protected set => _isUsingZipline = value; }
        /// <summary>
        /// Is in the exit zipline process
        /// </summary>
        public virtual bool inExitZipline { get => _inExitZipline; protected set => _inExitZipline = value; }
        /// <summary>
        /// The zipline collider
        /// </summary>
        public virtual Collider ziplineCollider { get; protected set; }
        #endregion

        [System.Serializable]
        public class ZiplineSettings
        {
            public string name = "Name of Zipline Object";
            [Tooltip("Max speed to use the Zipline")]
            public float maxSpeed = 10f;
            [Tooltip("Name of the animation clip that will play when you use the Zipline")]
            public string animationClip = "Zipline";
            [Tooltip("Make sure to enable when you're using and disable when you exit using Events")]
            public GameObject ziplineHandler;
            [Tooltip("The rotation of the character will be aligned to the zipline direction")]
            public bool lockCharacterRotation;
#if INVECTOR_MELEE
            public bool lockMeleeInput = true;
#endif
#if INVECTOR_SHOOTER
            public bool lockShooterInput = false;
#endif
        }

        protected virtual ZiplineSettings GetSettings(string name)
        {
            ZiplineSettings settings = null;
            settings = customSettings.Find(s => s.name == name);

            if (settings == null) settings = defaultSettings;
            return settings;
        }

        /// <summary>
        /// Override setup listener of <seealso cref="vActionListener"/>
        /// </summary>
        protected override void SetUpListener()
        {

            inputController = GetComponentInParent<vThirdPersonInput>();
            if (inputController)
            {
                if (TryGetComponent(out vHealthController healthController))
                    healthController.onDead.AddListener(ExitOnDead);
            }
            inputController.onFixedUpdate += UsingZipline;
            inputController.onUpdate += UpdateInput;
            actionEnter = true;
            actionStay = false;
            actionExit = true;
        }

        /// <summary>
        /// Event used to exit zipline when character dies
        /// </summary>
        /// <param name="character"></param>
        protected virtual void ExitOnDead(GameObject character)
        {
            if (isUsingZipline && !inExitZipline)
            {
                ExitZipline();
            }
        }

        protected override void Start()
        {
            base.Start();

            originalConstrains = GetComponentInParent<Rigidbody>().constraints;
            if (ziplineHandler)
            {
                handlerParent = ziplineHandler.transform.parent;
                handlerParentLocalPosition = handlerParent.localPosition;
                handlerParentLocalRotation = handlerParent.localRotation;
            }
        }

        /// <summary>
        /// Trigger enter event
        /// </summary>
        /// <param name="other"></param>
        public override void OnActionEnter(Collider other)
        {
            if (other.gameObject.CompareTag(ziplineTag) && !isUsingZipline && !inExitZipline)
            {
                var ap = other.gameObject.GetComponent<vZiplineAnchorPoints>();
                if (ap)
                {
                    if (debugMode)
                    {
                        UnityEngine.Debug.Log("<b><color=green>Enter trigger Zipline<color></b>");
                    }
                    anchorPoint = ap;
                    ziplineCollider = other;
                    // if you want to automatically enter the zipline, disable the input enterZipline in the inspector
                    if (!enterZipline.useInput)
                    {
                        InitiateZipline();
                    }
                }
            }
        }

        /// <summary>
        /// Trigger exit event 
        /// </summary>
        /// <param name="other"></param>
        public override void OnActionExit(Collider other)
        {
            if (other.gameObject.CompareTag(ziplineTag) && this.ziplineCollider != null && other == this.ziplineCollider)
            {
                if (debugMode)
                {
                    UnityEngine.Debug.Log("<b><color=red>Exit trigger Zipline</color></b>");
                }
                ziplineCollider = null;
                anchorPoint = null;
                if (isUsingZipline)
                {
                    ExitZipline();
                }
            }
        }

        /// <summary>
        /// Prepare the Controller to use the Zipline
        /// </summary>      
        protected virtual void InitiateZipline()
        {
            if (inputController && anchorPoint)
            {
                currentSettings = GetSettings(anchorPoint.gameObject.name);

                if (debugMode)
                {
                    UnityEngine.Debug.Log("<b><color=green>Enter Zipline Mode</color></b>");
                }

                isUsingZipline = true;
                inputController.SetLockBasicInput(true);
#if INVECTOR_MELEE
                if (lockMeleeInput && inputController is vMeleeCombatInput)
                {
                    (inputController as vMeleeCombatInput).SetLockMeleeInput(true);
                }
#endif
#if INVECTOR_SHOOTER
                if (lockShooterInput && inputController is vShooterMeleeInput)
                {
                    (inputController as vShooterMeleeInput).SetLockShooterInput(true);
                }
#endif
                if (lockCharacterRotation) inputController.cc.lockRotation = true;
                if (ziplineHandler) ziplineHandler.SetActive(true);
                inputController.cc.animator.CrossFadeInFixedTime(animationClip, 0.2f);
                inputController.cc._rigidbody.useGravity = false;
                originalConstrains = inputController.cc._rigidbody.constraints;
                inputController.cc._rigidbody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                inputController.cc.disableCheckGround = true;
                inputController.cc.animator.SetBool(vAnimatorParameters.IsGrounded, true);
                UpdatePosition(true);
                onZiplineEnter.Invoke();

            }
        }

        /// <summary>
        /// Update the zipline inputs
        /// </summary>
        protected virtual void UpdateInput()
        {
            if (ziplineCollider)
            {
                // enter the zipline only if you press the enterZipline input
                if (enterZipline.GetButton() && !isUsingZipline && !inExitZipline)
                {
                    if (debugMode)
                    {
                        UnityEngine.Debug.Log("<b><color=green>Enter Input Zipline</color></b>");
                    }
                    InitiateZipline();
                }
                // exit the zipline by pressing the exitZipline input
                if (exitZipline.GetButtonDown() && isUsingZipline && !inExitZipline)
                {
                    if (debugMode)
                    {
                        UnityEngine.Debug.Log("<b><color=red>Exit Input Zipline</color></b>");
                    }
                    ExitZipline();
                }

            }
        }

        /// <summary>
        /// Behaviour while using the Zipline
        /// </summary>
        /// <param name="other"></param>
        protected virtual void UsingZipline()
        {
            if (!isUsingZipline)
            {
                return;
            }

            if (inputController && anchorPoint)
            {
                if (debugMode)
                {
                    UnityEngine.Debug.Log("<b><color=yellow>Using Zipline</color></b>");
                }

                UpdatePosition(false);
                inputController.cc.heightReached = inputController.transform.position.y;
                inputController.CameraInput();
                onZiplineUsing.Invoke();
            }
        }
        /// <summary>
        /// Update the position
        /// </summary>
        /// <param name="entrance">First update when enter in zipline</param>
        protected virtual void UpdatePosition(bool entrance = true)
        {
            if (!anchorPoint) return;
            inputController.cc._rigidbody.velocity = Vector3.zero;
            Vector3 position = inputController.transform.position;
            var direction = anchorPoint.movementDirection;
            ziplineHandler.transform.rotation = Quaternion.LookRotation(direction);
            direction.y = 0;
            var euler = Quaternion.LookRotation(direction).eulerAngles;
            if (!lockCharacterRotation) euler.y = transform.eulerAngles.y;
            inputController.transform.eulerAngles = euler;
            var localPos = anchorPoint.transform.InverseTransformPoint(inputController.transform.position);
            localPos.x = 0f;
            localPos.y = 0f;
            var targetPosition = anchorPoint.transform.TransformPoint(localPos);

            var _localPos = anchorPoint.transform.InverseTransformPoint(targetPosition - inputController.transform.rotation * ziplineHandler.transform.localPosition);

            _localPos.z = localPos.z;
            position = anchorPoint.transform.TransformPoint(_localPos);
            if (!entrance)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, .5f * Time.deltaTime);
                position += anchorPoint.movementDirection * currentSpeed * Time.fixedDeltaTime;
            }
            inputController.transform.position = position;
        }

        /// <summary>
        /// Behaviour to Exit the Zipline
        /// </summary>
        /// <param name="other"></param>
        public virtual void ExitZipline()
        {
            if (!isUsingZipline)
            {
                return;
            }

            if (inputController)
            {
                inExitZipline = true;
                ziplineCollider = null;
                anchorPoint = null;
                inputController.cc.isGrounded = false;
                inputController.cc.animator.SetBool(vAnimatorParameters.IsGrounded, false);
                currentSpeed = 0f;
                inputController.cc._rigidbody.useGravity = true;
                inputController.cc._rigidbody.constraints = originalConstrains;
                inputController.cc.animator.CrossFadeInFixedTime("Falling", .2f);
                inputController.cc.disableCheckGround = false;
                if (lockCharacterRotation) inputController.cc.lockRotation = false;
                if (ziplineHandler) ziplineHandler.SetActive(false);

                inputController.SetLockBasicInput(false);
#if INVECTOR_MELEE
                if (lockMeleeInput && inputController is vMeleeCombatInput)
                {
                    (inputController as vMeleeCombatInput).SetLockMeleeInput(false);
                }
#endif
#if INVECTOR_SHOOTER
                if (lockShooterInput && inputController is vShooterMeleeInput)
                {
                    (inputController as vShooterMeleeInput).SetLockShooterInput(false);
                }
#endif

                isUsingZipline = false;
                onZiplineExit.Invoke();

                if (debugMode)
                {
                    UnityEngine.Debug.Log("<b><color=red>Finish Zipline mode</color></b>");
                }
                Invoke("ResetExitZipline", 1);
            }
        }
        /// <summary>
        /// Reset the  <see cref="inExitZipline"/> property
        /// </summary>
        protected virtual void ResetExitZipline()
        {
            inExitZipline = false;
        }

    }
}