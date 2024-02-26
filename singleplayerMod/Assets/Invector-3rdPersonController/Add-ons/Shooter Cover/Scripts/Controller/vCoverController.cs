using Invector.vCharacterController;
using Invector.vCharacterController.vActions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Invector.vCover
{

    [vClassHeader("Cover Controller")]
    public class vCoverController : vMonoBehaviour
    {
        #region Variables

        #region -------------Settings----------------

        [vEditorToolbar("Cover Settings")]
        [vSeparator("Enter/Exit Cover Input")]
        public GenericInput enterExitInput = new GenericInput("Space", "A", "A");
        [Tooltip("Check this option to automatically enter cover mode when close to a cover point without input and within the EnterInputDirectionAngle limit.")]
        public bool autoEnterCover;
        [vHideInInspector("autoEnterCover")]
        [Tooltip("Angle of Input direction to enter cover")]
        public float enterInputDirectionAngle = 60;
        [vHideInInspector("autoEnterCover")]
        [Tooltip("Time to auto enter when enterDirectionAngle is reached")]
        public float enterDirectionTimer = 0.25f;
        [Tooltip("Auto Exit Cover based on a Input Direction")]
        public bool autoExitCover;
        [vHideInInspector("autoExitCover")]
        [Tooltip("Angle of Input direction to exit cover")]
        public float exitInputDirectionAngle = 135;
        [vHideInInspector("autoExitCover")]
        [Tooltip("Timer to exit when Exit direction angle is reached")]
        public float exitDirectionTimer = 0.25f;

        [vSeparator("Get Cover Settings")]
        [Range(1, 25), Tooltip("Distance to get cover point to enter in cover\nOnly work with Auto Enter disabled")]
        public float maxEnterCoverDistance = 4f;
        [Tooltip("The minimum distance to cancel moving to a CoverPoint if you release the input")]
        public float minDistanceToCancelTravel = 2.5f;
        [Tooltip("Enable to Detect distante CoverPoints and Travel to it *NAVMESH REQUIRED")]
        public bool autoTravelToNextCover = true;
        [vMinMax(1, 30), Tooltip("Min Max Distance to get a new cover point when it's in cover\n *The Min Value is ignored if is in cover and next Cover is not connected to current cover line\nWorks with autoTravelToNextCover enabled")]
        public Vector2 nextCoverDistance = new Vector2(4, 10);
        [Tooltip("Creates more Raycasts within a open angle, Turn on DEBUG to see it")]
        [Range(0, 4)]
        public int rayCastAnglePass = 3;
        [Range(1, 10)]
        [Tooltip("Multiplies the raycastAnglePass into several casts, Turn on DEBUG to see it")]
        public float rayCastAngleMultiplier = 2;
        [Tooltip("Capsule Collider Radius while in Cover")]
        public float capsuleRadiusInCover = 0.2f;
        [Tooltip("The minimum angle accepted to be a valid corner")]
        [Range(15, 90)]
        public float minAngleOfValidCover = 80;
        [Tooltip("Distance OffsetZ of the Controller from the Cover")]
        public float positionOffsetZ;

        [vSeparator("Slide Settings")]

        [Tooltip("Check to use a Slide animation when switching between covers using the AutoTravel")]
        public bool useSlideToCover = true;
        [Tooltip("Min and Max distance to Slide")]
        [vMinMax(2, 5)]
        public Vector2 minPathLenghToSlide = new Vector2(2, 5);


        [vSeparator("Side Settings")]
        [vToggleOption("Change Side Method", "Only Input Direction", "Change By Angle and Input")]
        public bool changeSide = true;
        public Vector2 angleToChangeSide = new Vector3(65, 180);
        [vToggleOption("Change Side Method Aiming", "Only Input Direction", "Change By Angle and Input")]
        public bool changeSideAiming = true;
        [vMinMax(minLimit = 45, maxLimit = 180)]
        public Vector2 angleToChangeSideOnAiming = new Vector3(45, 135);
        [vMinMax(minLimit = -180, maxLimit = 180), Tooltip("Angle to keep in corner state pose when aiming")]
        public Vector2 cornerAimingAngle = new Vector2(-15, 90);

        [vSeparator("Crouch Settings")]
        [Tooltip("Standup when exiting a Cover Crouched or Keep it crouched")]
        public bool alwaysStandUpOnExit = true;
        [Range(0.01f, 0.2f), Tooltip("Radius to verify if the character will be on crouch, Turn on DEBUG to see it")]
        public float crouchRayRadius = 0.05f;
        [Tooltip("Distance to verify if the character will be on crouch, Turn on DEBUG to see it")]
        public float crouchRayDistance = 0.5f;
        [Tooltip("Height to verify if the character will be on crouch, Turn on DEBUG to see it")]
        public float crouchHeight = 1.5f;
        [Tooltip("Height to verify if the character will be on crouch and aiming, Turn on DEBUG to see it")]
        public float crouchHeightAiming = 1f;
        public float crouchRayOffsetInCorner = 0.25f;
        public float timeToStandUp = 0.5f;

        [vSeparator("Corner Settings")]
        [Tooltip("Check to use the GetCoverInput to move on corners, Keep it False to just press the input direction to do corners,  this will also change the Corner UI")]
        public bool enterUsingGetCoverInput;
        [Tooltip("Angle to Enter a Corner")]
        public float angleToEnterCorner = -10;
        [Tooltip("Minimum angle to start moving to a corner")]
        public float minAngleToStartMoveToCorner = 10f;
        [Tooltip("Delay to do Corner")]
        public float timeToEnterCorner = 0.5f;
        [Tooltip("Controller Offset Position when Aiming on a Corner")]
        public float cornerLeftAiming_OffsetPosition = 0.6f;
        [Tooltip("Controller Offset Position when Aiming on a Corner")]
        public float cornerRightAiming_OffsetPosition = 0.4f;
        [Tooltip("Smooth when performing a Corner")]
        public float cornerWeightSmooth = 20;

        [vSeparator("Shot Over Barrier *Experimental*")]
        public bool useHipFireOverBarrier;
        public float hipFireTimer;
        public float checkAimOverBarrierOffsetStartY = 0.5f;
        public float checkAimOverBarrierOffsetStartX = 0;
        public float checkAimOverBarrierOffsetEndY = 0.5f;
        public float checkAimOverBarrierOffsetEndX = 0;
        [vSeparator("Generic Actions Settings")]
        public List<string> possibleActions = new List<string>() { "Action" };
        public Vector3 checkActionMargin;
        public Vector3 checkActionOffset;
        public bool showActionGizmos;

        public float rotationSpeed => Mathf.Lerp(rotationSpeedIdle, rotationSpeedMoving, Mathf.Clamp(shooterInput.cc.input.magnitude, 0, 1f));

        [vEditorToolbar("Movement Speed")]
        [vSeparator("Movement Speed")]
        public float crouchedMoveSpeed = 2;
        public float standingMoveSpeed = 2;
        public float goToCoverPointRunningMoveSpeed = 2;
        public float goToCoverPointSprintingMoveSpeed = 2;
        public float goToCornerCrouchedMoveSpeed = 2;
        public float goToCornerStandingMoveSpeed = 2;
        [Tooltip("Rotation Speed of the Controller while in Cover")]
        public float rotationSpeedMoving = 5f;
        public float rotationSpeedIdle = 10f;

        [vSeparator("Animation Speed")]
        public float standAnimatorSpeed = 1;
        public float crouchAnimatorSpeed = 1;
        public float goToCoverPointRunningAnimatorSpeed = 1;
        public float goToCoverPointSprintingAnimatorSpeed = 1;
        public float goToCornerPointAnimatorSpeed = 1;
        LayerMask oldStopMoveMask;

        #endregion

        #region-----------Custom IK Poses-----------
        [vEditorToolbar("IK Pose")]
        public string ikLeftStanding;
        public string ikLeftStandingAiming;
        public string ikLeftStandingCornerAiming;
        public string ikLeftCrouching;
        public string ikLeftCrouchingAiming;
        public string ikLeftCrouchingBarrierAiming;
        public string ikLeftCrouchingCornerAiming;
        public string ikRightStanding;
        public string ikRightStandingAiming;
        public string ikRightStandingCornerAiming;
        public string ikRightCrouching;
        public string ikRightCrouchingAiming;
        public string ikRightCrouchingBarrierAiming;
        public string ikRightCrouchingCornerAiming;
        #endregion

        #region ---------Layers and Tags ------------
        [vEditorToolbar("Layers and Tags")]

        public LayerMask obstaclesLayer;
        [Tooltip("Tags to be ignored when checking if need to crouch or standup")]
        public vTagMask checkHeightIgnoreTags = new vTagMask("Glass");
        [Tooltip("GameObject Names to be ignored when checking if need to crouch or standup")]
        public List<string> checkHeightIgnoreNames = new List<string>();
        public LayerMask coverLayer;
        public vTagMask coverTag;

        #endregion

        #region -------------Cover UI----------------
        [vEditorToolbar("Cover UI")]
        public Color routeLineColor = Color.grey;
        [Range(0, 2)]
        public float drawRouteDelayTime;
        public LineRenderer routeLine;
        public int routeLineSmoothPass = 2;
        public float routeLineSmoothFactor = 0.2f;
        [Range(0.1f, 10f)]
        public float coverIndicatorDistance = 5f;
        public float coverIndicaterHeight = 1f;
        public GameObject coverUI;
        public GameObject cornerUICenter, cornerLeftUI, cornerRightUI;
        public GameObject cornerInputUI;
        public UnityEngine.UI.Slider.SliderEvent onUpdateCornerInput;
        #endregion

        #region ----------Animation States-----------
        [vEditorToolbar("Animation States")]
        [vSeparator("Default Locomotion")]
        public string defaultFreeStanding = "Free Movement";
        public string defaultFreeCrouched = "Free Crouch";
        public string defaultStrafeStanding = "Strafing Movement";
        public string defaultStrafeCrouched = "Strafing Crouch";

        [vSeparator("Cover Locomotion")]
        public string goToCoverPoint = "Go To Cover Point";
        public string startSlideToCoverPoint = "Start Slide To Cover";
        public string finishSlideToCoverPoint = "Finish Slide To Cover";
        public string coverStandingLeft = "Cover Stand Left";
        public string coverStandingRight = "Cover Stand Right";
        public string coverCrouchedLeft = "Cover Crouch Left";
        public string coverCrouchedRight = "Cover Crouch Right";

        [vSeparator("Cross Fade Transition")]
        [Range(0.1f, .5f)]
        public float enterCoverAnimCrossfade = 0.25f;
        [Range(0.1f, .5f)]
        public float exitCoverAnimCrossfade = 0.25f;
        [Range(0.1f, .5f)]
        public float goToCoverAnimCrossfade = 0.25f;
        [Range(0.1f, .5f)]
        public float startSlideToCoverAnimCrossfade = 0.25f;
        [Range(0.1f, .5f)]
        public float finishSlideToCoverAnimCrossfade = 0.25f;
        [Range(0.1f, 1)]
        public float finishSlideToCoverWaitTime = 0.5f;
        #endregion

        #region-----------Camera States-------------

        [vEditorToolbar("Camera Settings")]
        [vSeparator("Camera Offset Right")]
        public float cornerLeft_OffsetCameraRight = 1;
        public float cornerRight_OffsetCameraRight = 1;
        public float cornerLeftAiming_OffsetCameraRight = 1;
        public float cornerRightAiming_OffsetCameraRight = 1;

        [vSeparator("Moving to Next Cover CameraState")]
        public string goToCoverCameraState = "GoToCover";

        [vSeparator("Stand Cover CameraState")]
        public string standCameraState = "CoverStand";
        public string standAimingCameraState = "CoverStandAiming";

        [vSeparator("Crouch Cover CameraState")]
        public string crouchCameraState = "CoverCrouch";
        public string crouchAimingCameraState = "CoverCrouchAiming";

        [vSeparator("HipFire Cover CameraState")]
        public string aimingOverBarrierCameraState;

        [vEditorToolbar("Events")]
        public UnityEngine.Events.UnityEvent onEnterCover;
        public UnityEngine.Events.UnityEvent onExitCover;
        public UnityEngine.Events.UnityEvent onStartGoToCoverPoint;
        public UnityEngine.Events.UnityEvent onFinishGoToCoverPoint;
        #endregion

        #region--------------Debug------------------
        [vEditorToolbar("Debug")]
        public bool debug;
        [vReadOnly] public bool inCover;
        [vReadOnly] public bool wasInCover;
        [vReadOnly] public bool goingToCoverPoint;
        [vReadOnly] public bool goingToCornerPoint;
        [vReadOnly, SerializeField] public float currentTimeToEnterCorner;
        [vReadOnly] public vCoverPoint currentCoverPoint;
        [vReadOnly] public vCoverPoint possibleCoverPoint;
        [vReadOnly] public vCoverPoint possibleCornerPoint;
        [vReadOnly, SerializeField] protected float drawRouteTimer;
        [vReadOnly, SerializeField] protected Side side = Side.right;
        #endregion

        #region ---------Protected Variables---------
        protected int animatorCoverCornerLayer;
        protected int animatorCoverOverBarrierLayer;
        protected float remainingDistanceToNextCover;
        protected float movementAngle;
        protected float cameraAngle;
        [SerializeField, vReadOnly] protected float currentCornerWeight;
        protected float cameraSwtichRight;
        protected float cameraSwtichLeft;
        protected float overBarrierWeight;
        [SerializeField, vReadOnly] protected float currentTimeToGetUp;
        protected float timeToEnterByDirection;
        protected float timeToExitByDirection;
        protected float lastHipFireTimer;
        protected float timeToExit;
        protected bool lastUseHipFire;
        protected bool enterCoverFromLeftSide;

        protected bool isInCornerInputAction;
        protected bool inCornerAdjustPosition;
        protected bool holdingCornerInput;
        protected bool originalWalkByDefault;
        protected bool lastMoveToDirectionInFree;
        [SerializeField, vReadOnly] protected bool isInLeftCornerPositionRange;
        [SerializeField, vReadOnly] protected bool isInRightCornerPositionRange;
        [SerializeField, vReadOnly] protected bool isLeftCorner;
        [SerializeField, vReadOnly] protected bool isRightCorner;
        [SerializeField, vReadOnly] protected bool isInLeftCorner;
        [SerializeField, vReadOnly] protected bool isInRightCorner;
        [SerializeField, vReadOnly] protected bool inCornerAngle;
        [SerializeField, vReadOnly] protected bool enableHipFireOverBarrier;
        [SerializeField, vReadOnly] protected bool aimingOverBarrier;
        [SerializeField, vReadOnly] protected bool wasAiming;
        [SerializeField, vReadOnly] protected bool isAimingInCorner;
        [SerializeField, vReadOnly] protected bool canMove;
        [SerializeField, vReadOnly] protected string currentIKPose;
        protected string routeLineColorName = "_TintColor";

        protected Vector3 coverIndicatorPosition;
        protected Vector3 coverMoveDirection;
        protected Vector3 coverNormal;
        protected Vector3 positionHelper;
        protected Vector3 inputDirection;
        protected Vector3 cameraForwardDir;
        protected Vector3 cameraRightDir;
        [SerializeField, vReadOnly] protected Vector3 localCharPosition;
        protected RaycastHit checkHeightHit;
        protected Transform helper;
        protected List<Vector3> wayPath = new List<Vector3>();
        protected List<Vector3> wayPathToDraw = new List<Vector3>();
        protected List<Vector3> pathDrawPoints = new List<Vector3>();

        public List<vCoverPoint> coverPoints = new List<vCoverPoint>();
        protected NavMeshPath path;
        protected vShooterMeleeInput shooterInput;
        protected vGenericAction genericAction;
        protected Coroutine currentCoverRoutine;
        protected vThirdPersonMotor.vMovementSpeed moveSpeed;
        protected enum Side
        {
            left = -1, right = 1
        }
        #endregion

        #endregion

        #region Methods

        #region Unity Methods
        protected virtual void OnDrawGizmos()
        {
            #region Display Helper Point and CoverPoint box
            if (wayPath.Count > 0)
            {
                Gizmos.DrawWireSphere(wayPath[wayPath.Count - 1], shooterInput.cc._capsuleCollider.radius);
            }
            if (inCover)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(helper.position, 0.1f);
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(helper.position, helper.forward * 0.5f);
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(helper.position, helper.right * 0.5f);
                Gizmos.color = Color.red;
                Gizmos.DrawRay(helper.position, -helper.right * 0.5f);

                if (currentCoverPoint)
                {
                    var c = currentCoverPoint.boxCollider;
                    Gizmos.color = Color.yellow * 0.2f;
                    var pos = c.transform.TransformPoint(c.center);
                    var size = c.size;
                    var matrix = Gizmos.matrix;
                    size.Scale(c.transform.lossyScale);
                    Gizmos.matrix = Matrix4x4.TRS(pos, c.transform.rotation, size);
                    Gizmos.DrawCube(Vector3.zero, Vector3.one);
                    Gizmos.color = Color.yellow * 0.5f;
                    Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                    Gizmos.matrix = matrix;
                }
            }
            #endregion

            #region Display TriggerAction box
            if (inCover || goingToCoverPoint)
            {
                if (showActionGizmos && genericAction.triggerAction && genericAction.triggerAction._collider is BoxCollider)
                {
                    var c = genericAction.triggerAction._collider as BoxCollider;
                    if (c)
                    {
                        bool isClose = isClose = c.IsClosed(transform.position, checkActionMargin, checkActionOffset);
                        var matrix = Gizmos.matrix;
                        var pos = c.transform.TransformPoint(c.center);
                        var size = c.BoxSize();
                        size = new Vector3(size.x + checkActionMargin.x, size.y + checkActionMargin.y, size.z + checkActionMargin.z);
                        pos = c.transform.TransformPoint(c.center + checkActionOffset);
                        Gizmos.matrix = Matrix4x4.TRS(pos, c.transform.rotation, size);
                        Gizmos.color = isClose ? Color.green * 0.2f : Color.yellow * 0.2f;
                        Gizmos.DrawCube(Vector3.zero, Vector3.one);
                        Gizmos.color = isClose ? Color.green * 0.8f : Color.yellow * 0.8f;
                        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                        Gizmos.matrix = matrix;
                        Gizmos.color = Color.yellow * 0.2f;
                        pos = c.transform.TransformPoint(c.center);
                        size = c.BoxSize();
                        Gizmos.matrix = Matrix4x4.TRS(pos, c.transform.rotation, size);
                        Gizmos.DrawCube(Vector3.zero, Vector3.one);
                        Gizmos.color = Color.yellow * 0.5f;
                        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                        Gizmos.matrix = matrix;
                    }
                }
            }
            #endregion
        }

        protected IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            path = new NavMeshPath();
            shooterInput = GetComponent<vShooterMeleeInput>();

            originalWalkByDefault = shooterInput.cc.freeSpeed.walkByDefault;
            shooterInput.onFixedUpdate += UpdateCover;
            shooterInput.onUpdate += UpdateInput;
            shooterInput.onUpdateCheckAimPointsEvent += UpdateCheckAimPoints;
            genericAction = GetComponent<vGenericAction>();
            helper = new GameObject("Helper").transform;
            helper.gameObject.hideFlags = HideFlags.HideInHierarchy;
            moveSpeed = new vThirdPersonMotor.vMovementSpeed
            {
                walkByDefault = true
            };
            lastMoveToDirectionInFree = shooterInput.cc.moveToDirectionInFree;
            animatorCoverCornerLayer = shooterInput.animator.GetLayerIndex("CoverCorner");
            animatorCoverOverBarrierLayer = shooterInput.animator.GetLayerIndex("AimingOverBarrier");
            onEnterCover.AddListener(() => shooterInput.cc.colliderRadius = capsuleRadiusInCover);
            onExitCover.AddListener(() => shooterInput.cc.colliderRadius = shooterInput.cc.colliderRadiusDefault);
            lastHipFireTimer = shooterInput.shooterManager.HipfireAimTime;
            lastUseHipFire = shooterInput.shooterManager.hipfireShot;
        }

        #endregion

        #region Update Methods
        protected virtual void UpdateCover()
        {
            CheckForHelperTransform();
            HandlePossibleCoverPoint();
            HandlePossibleCornerPoint();
            HandleCoverRouteLine();
            HandleCoverIndicator();
            HandleCoverMovement();
        }

        protected virtual void CheckForHelperTransform()
        {
            if (helper == null)
            {
                helper = new GameObject("Helper").transform;
                helper.gameObject.hideFlags = HideFlags.HideInHierarchy;
            }
        }

        protected virtual bool isRolling
        {
            get
            {
                return shooterInput.cc.isRolling;
            }
        }

        protected virtual void UpdateInput()
        {
            HandleEnterExitCover();
        }
        #endregion

        #region Enter Cover / Exit Cover Methods

        #region Cover Action

        /// <summary>
        /// Control When Enter our Exit cover
        /// </summary>
        protected virtual void HandleEnterExitCover()
        {
            if (isInCornerInputAction || goingToCornerPoint)
            {
                if (inCover && shooterInput.cc.IsAnimatorTag("ExitCover"))
                {
                    ExitCover(true);
                }

                return;
            }

            bool up = false;
            float inputTimer = 0;
            bool inTimer = enterExitInput.GetButtonTimer(ref inputTimer, ref up, .2f);

            ///Enter Cover or Go To Cover Point
            if (!shooterInput.animator.IsInTransition(0) && possibleCoverPoint && !goingToCoverPoint && !shooterInput.cc.customAction && !isRolling && !shooterInput.cc.isJumping && !shooterInput.cc.ragdolled && !shooterInput.cc.isDead)
            {
                if (((enterExitInput.GetButtonDown() || inAutoEnterTimer) && !inCover) || (inTimer && inCover && wasInCover))
                {
                    timeToExit = Time.time + .5f;
                    currentCoverRoutine = (autoEnterCover && !inCover) || !autoTravelToNextCover ? StartCoroutine(EnterCoverPointRoutine(possibleCoverPoint)) : StartCoroutine(GoToCoverPointRoutine(possibleCoverPoint, wayPath.vCopy()));
                    possibleCoverPoint = null;
                }
            }

            ///ExitCover
            bool exitCoverA = inCover && wasInCover && up && autoTravelToNextCover && !goingToCornerPoint && inputTimer > 0 && (timeToExit < Time.time);
            bool exitCoverB = !inCover && (!autoEnterCover || wasInCover) && inputTimer == 0 && autoTravelToNextCover && !enterExitInput.GetButton() && goingToCoverPoint;
            bool exitCoverC = inCover && wasInCover && !goingToCornerPoint && !shooterInput.animator.IsInTransition(0) && (shooterInput.cc.customAction || isRolling || shooterInput.cc.isJumping || shooterInput.cc.ragdolled || shooterInput.cc.isDead);
            bool exitCoverD = inCover && !autoTravelToNextCover && !goingToCornerPoint && !shooterInput.animator.IsInTransition(0) && inputTimer > 0 && up;
            bool forceExit = shooterInput.cc.IsAnimatorTag("ExitCover");
            bool exitCover = exitCoverA || exitCoverB || exitCoverC || exitCoverD || forceExit;

            if (exitCover && (inCover || (goingToCoverPoint && (remainingDistanceToNextCover > minDistanceToCancelTravel || forceExit))))
            {
                if (debug)
                {
                    Debug.Log($"EXIT Condition   A :{exitCoverA.ToStringColor()}    B:{exitCoverB.ToStringColor()}  / C:{exitCoverC.ToStringColor()}  / D:{exitCoverD.ToStringColor()} / E:{remainingDistanceToNextCover}");
                }

                if (goingToCoverPoint)
                {
                    onFinishGoToCoverPoint.Invoke();
                }
                if (inCover)
                {
                    onExitCover.Invoke();
                }

                ExitCover(forceExit);

            }

            if ((inCover || wasInCover) && ExitByActions())
            {
                ExitCover();

            }
        }

        protected virtual bool inAutoEnterTimer
        {
            get
            {
                if (!autoEnterCover)
                {
                    return false;
                }

                if (timeToEnterByDirection >= enterDirectionTimer)
                {

                    return true;

                }
                else if (possibleCoverPoint)
                {
                    timeToEnterByDirection += Time.deltaTime;
                }
                else
                {
                    timeToEnterByDirection = 0;
                }

                return false;
            }

        }
        /// <summary>
        /// Control the possible cover point to enter or to travel when is in cover 
        /// </summary>
        protected virtual void HandlePossibleCoverPoint()
        {
            if (goingToCoverPoint)
            {
                return;
            }
            if (autoEnterCover && !inCover && !wasInCover)
            {
                vCoverPoint validCoverPoint = null;
                if (shooterInput.cc.inputSmooth.magnitude > 0.5f && EnterCoverConditions())
                {

                    var _possibleNearCoverPoint = GetNearCoverPoint();

                    if (_possibleNearCoverPoint)
                    {
                        ControlInputDirectionInCover();
                        var _inputDirection = inputDirection;
                        _inputDirection.y = 0;
                        if (_inputDirection.magnitude > 0.5)
                        {
                            var angle = Mathf.Abs((-_possibleNearCoverPoint.transform.forward).AngleFormOtherDirection(_inputDirection.normalized).y);

                            if (angle < enterInputDirectionAngle)
                            {

                                validCoverPoint = _possibleNearCoverPoint;


                            }
                        }
                    }

                }
                possibleCoverPoint = validCoverPoint;
                return;
            }
            if (inCover && !autoTravelToNextCover)
            {
                return;
            }

            if (!inCover && autoEnterCover)
            {
                return;
            }

            if (!inCover && !autoTravelToNextCover)
            {
                var _possibleNearCoverPoint = GetNearCoverPoint();
                if (_possibleNearCoverPoint)
                {
                    coverIndicatorPosition = _possibleNearCoverPoint.posePosition;


                }
                if (_possibleNearCoverPoint != possibleCoverPoint)
                {
                    if (debug && _possibleNearCoverPoint)
                    {
                        Debug.Log("Select possible cover point ", _possibleNearCoverPoint);
                    }

                    possibleCoverPoint = _possibleNearCoverPoint;
                }
                return;
            }
            var _possibleCoverPoint = GetCoverPoint();
            if (_possibleCoverPoint != null && _possibleCoverPoint != possibleCornerPoint && CalculatePath(_possibleCoverPoint.posePosition))
            {

                possibleCoverPoint = _possibleCoverPoint;

                wayPath = path.corners.vToList();
                var _transform = possibleCoverPoint.transform;
                Vector3 localPosition = _transform.InverseTransformPoint(wayPath[wayPath.Count - 1]);
                localPosition.z = Mathf.Max(shooterInput.cc._capsuleCollider.radius + positionOffsetZ, shooterInput.cc._capsuleCollider.radius);
                wayPath[wayPath.Count - 1] = _transform.TransformPoint(localPosition);

            }
            else
            {
                drawRouteTimer = 0;
                possibleCoverPoint = null;
                if (wayPath.Count > 0)
                {
                    wayPath.Clear();
                }
            }
            wayPathToDraw = wayPath;
        }

        /// <summary>
        /// Get possible cover point
        /// </summary>
        /// <returns></returns>
        protected virtual vCoverPoint GetCoverPoint()
        {
            vCoverPoint coverPoint = null;
            Vector3 cameraDir = shooterInput.cameraMain.transform.forward;
            Ray rayCamera = new Ray(shooterInput.cameraMain.transform.position, shooterInput.cameraMain.transform.forward);
            cameraDir.y = 0;
            Ray rayCharacter = new Ray(transform.position + Vector3.up * shooterInput.cc._capsuleCollider.radius + shooterInput.cameraMain.transform.right * shooterInput.tpCamera.lerpState.right - cameraDir * shooterInput.cc._capsuleCollider.radius, cameraDir.normalized);
            Vector3 hitPoint = transform.position;
            if (inCover)
            {
                FindCoverPoint(rayCamera, ref coverPoint);
            }
            else
            {

                FindCoverPoint(rayCharacter, ref coverPoint);
                if (!coverPoint)
                {
                    coverPoint = GetNearCoverPoint();
                }
            }

            return coverPoint;
        }

        /// <summary>
        /// Ray Cast cover point to find one
        /// </summary>
        /// <param name="ray">Ray to cast</param>
        /// <param name="coverPoint">reference of the Cover point to asign</param>
        protected virtual bool RayCastCoverPoint(Ray ray, ref vCoverPoint coverPoint)
        {

            var hits = Physics.RaycastAll(ray, inCover ? nextCoverDistance.y : maxEnterCoverDistance, coverLayer);

            for (int i = 0; i < hits.Length; i++)
            {
                if (debug)
                {
                    Debug.DrawLine(ray.origin, hits[i].point, Color.yellow);
                }
                var _cp = hits[i].collider.gameObject.GetComponent<vCoverPoint>();
                if (_cp == null)
                {
                    continue;
                }

                float angle = Mathf.Abs(ray.direction.AngleFormOtherDirection(-_cp.transform.forward).y);
                if (angle > minAngleOfValidCover) continue;
                if (!Physics.Linecast(_cp.boxCollider.bounds.center, ray.origin, obstaclesLayer) && (!inCover || hits[i].distance >= nextCoverDistance.x || !CoverIsConnectedTo(currentCoverPoint, _cp)))
                {
                    coverPoint = _cp;
                    coverIndicatorPosition = hits[i].point;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check if target cover point is connected to current cover point
        /// </summary>
        /// <param name="currentCoverPoint">Current cover point</param>
        /// <param name="coverPointToCheck">Target cover point</param>
        /// <returns></returns>
        protected virtual bool CoverIsConnectedTo(vCoverPoint currentCoverPoint, vCoverPoint coverPointToCheck)
        {
            if (currentCoverPoint == null)
            {
                return false;
            }

            bool isConnected = false;
            if (currentCoverPoint.Equals(coverPointToCheck))
            {
                return true;
            }

            vCoverPoint left = currentCoverPoint.left;
            vCoverPoint right = currentCoverPoint.right;
            int count = (int)(nextCoverDistance.x / currentCoverPoint.boxCollider.size.x) + 1;
            for (int i = 0; i < count; i++)
            {
                if (coverPointToCheck.Equals(left) || coverPointToCheck.Equals(right))
                {
                    isConnected = true;
                    break;
                }
                if (left == null && right == null)
                {
                    isConnected = false;
                    break;
                }
                else
                {
                    if (left)
                    {
                        left = left.left;
                    }

                    if (right)
                    {
                        right = right.right;
                    }
                }
            }
            return isConnected;
        }

        /// <summary>
        /// Find a cover point using multiple rays
        /// </summary>
        /// <param name="ray">Default ray to cast</param>
        /// <param name="coverPoint">reference of the Cover point to asing</param>
        /// <returns></returns>
        protected virtual bool FindCoverPoint(Ray ray, ref vCoverPoint coverPoint)
        {
            bool success = false;

            Vector3 rayDirection = ray.direction;
            Vector3 axisX = Quaternion.AngleAxis(90, Vector3.up) * rayDirection;
            axisX.y = 0;
            axisX.Normalize();

            float angleToInvert = Mathf.Max(2 * rayCastAnglePass, 1) * 0.5f;
            bool invertX = false;
            bool invertY = false;

            for (int angleX = 0; angleX < Mathf.Max(2 * rayCastAnglePass, 1); angleX++)
            {
                if (angleX >= angleToInvert)
                {
                    invertX = true;
                }

                for (int angleY = 0; angleY < Mathf.Max(2 * rayCastAnglePass, 1); angleY++)
                {
                    if (angleY >= angleToInvert)
                    {
                        invertY = true;
                    }

                    var x = invertX ? -(angleToInvert - (angleX - angleToInvert)) : angleToInvert - angleX;
                    var y = invertY ? -(angleToInvert - (angleY - angleToInvert)) : angleToInvert - angleY;

                    var angleDirection = Quaternion.AngleAxis(y * rayCastAngleMultiplier, Vector3.up) * Quaternion.AngleAxis(x * rayCastAngleMultiplier, axisX) * rayDirection;

                    ray.direction = angleDirection;
                    if (debug)
                    {
                        Debug.DrawLine(ray.origin, ray.GetPoint(inCover ? nextCoverDistance.y : maxEnterCoverDistance), Color.white * 0.5f);
                    }
                    if (RayCastCoverPoint(ray, ref coverPoint))
                    {
                        if (coverPoint)
                        {
                            if (debug)
                            {
                                Debug.DrawLine(ray.origin, coverIndicatorPosition, Color.green);
                            }

                            success = true;
                            break;
                        }
                    }
                }
                invertY = false;
                if (success)
                {
                    break;
                }
            }

            return success;
        }

        /// <summary>
        /// Calculate Path using NavMesh
        /// </summary>
        /// <param name="position">Target Position</param>
        /// <returns></returns>
        protected virtual bool CalculatePath(Vector3 position)
        {

            bool valid = NavMesh.CalculatePath(transform.position, position, NavMesh.AllAreas, path) && path.GetLenght() < (inCover ? nextCoverDistance.y : maxEnterCoverDistance) && path.status == NavMeshPathStatus.PathComplete;
            return valid;
        }

        /// <summary>
        /// Control the Cover UI Indictor
        /// </summary>
        protected virtual void HandleCoverIndicator()
        {
            if (coverUI)
            {

                var active = possibleCoverPoint != null && !goingToCornerPoint && !isInCornerInputAction;
                if (active)
                {
                    Vector3 localPosition = possibleCoverPoint.transform.InverseTransformPoint(coverIndicatorPosition);
                    if (IsCoverCorner(possibleCoverPoint))
                    {
                        localPosition.x = 0;
                    }

                    localPosition.z = 0;
                    localPosition.y = coverIndicaterHeight;
                    Vector3 position = possibleCoverPoint.transform.TransformPoint(localPosition);
                    float distance = Vector3.Distance(position, shooterInput.cameraMain.transform.position);
                    coverUI.transform.position = position;
                    coverUI.transform.forward = possibleCoverPoint.transform.forward;
                    coverUI.transform.localScale = Vector3.one * Mathf.Max(1, distance / coverIndicatorDistance);

                }
                if (coverUI.activeSelf != active)
                {
                    coverUI.SetActive(active);
                }
            }
        }

        /// <summary>
        /// Control the Cover route Line indicator
        /// </summary>
        protected virtual void HandleCoverRouteLine()
        {

            if (!routeLine || goingToCoverPoint)
            {
                return;
            }

            UpdateDrawRouteDelayTimer();

            if (!goingToCornerPoint && !isInCornerInputAction && drawRouteTimer >= drawRouteDelayTime)
            {
                pathDrawPoints.Clear();
                wayPath = wayPathToDraw;
                var pLast = transform.position + Vector3.up * .5f;
                pathDrawPoints.Add(pLast);
                RaycastHit hitCoverPoint;
                for (int i = 1; i < wayPath.Count; i++)
                {
                    var pNew = wayPath[i] + Vector3.up * (i == wayPath.Count - 1 ? 0.2f : .5f);

                    if (pNew != pLast)
                    {
                        var dir = pNew - pLast;

                        dir.y = 0;
                        dir.Normalize();
                        bool getPoint = false;

                        if (Physics.Linecast(pLast, pNew, out hitCoverPoint, obstaclesLayer))
                        {
                            if (debug)
                            {
                                Debug.DrawLine(pNew, pLast, Color.cyan, 1);
                            }

                            var p = hitCoverPoint.point - dir * 0.05f;
                            for (int a = 0; a < 20; a++)
                            {
                                float value = 0.1f * a;
                                p += Vector3.up * value;
                                if (Physics.Raycast(p, dir, out hitCoverPoint, .1f, obstaclesLayer))
                                {
                                    ///
                                }
                                else
                                {
                                    if (a > 0)
                                    {
                                        pathDrawPoints.Add(p);
                                    }

                                    getPoint = true;
                                    break;
                                }
                            }

                        }

                        if (Physics.Linecast(pNew, pLast, out hitCoverPoint, obstaclesLayer))
                        {
                            var p = hitCoverPoint.point + dir * 0.05f;
                            for (int a = 0; a < 20; a++)
                            {
                                float value = 0.1f * a;
                                p += Vector3.up * value;
                                if (Physics.Raycast(p, -dir, out hitCoverPoint, .1f, obstaclesLayer))
                                {
                                }
                                else
                                {
                                    if (a > 0)
                                    {
                                        pathDrawPoints.Add(p);
                                    }

                                    getPoint = true;
                                    break;
                                }
                            }
                        }

                        if (!getPoint)
                        {
                            pathDrawPoints.Add(pNew);
                            pLast = pNew;
                        }
                        else
                        {
                            i--;
                            pLast = pathDrawPoints[pathDrawPoints.Count - 1];
                        }
                    }
                }
                Vector3[] pathSmooth = pathDrawPoints.MakeSmoothCurveArray(routeLineSmoothFactor);
                for (int i = 0; i < routeLineSmoothPass; i++)
                {
                    pathSmooth = pathSmooth.MakeSmoothCurve(routeLineSmoothFactor);
                }
                routeLine.positionCount = pathSmooth.Length;
                routeLine.SetPositions(pathSmooth);
            }
            else if (routeLine.positionCount > 0)
            {
                routeLine.positionCount = 0;
            }
        }

        /// <summary>
        /// Update the time to show or hide the Cover route Line
        /// </summary>
        protected virtual void UpdateDrawRouteDelayTimer()
        {
            if (!inCover)
            {
                drawRouteTimer = 0;
            }

            if (wayPathToDraw.Count > 1)
            {
                if (drawRouteTimer < drawRouteDelayTime + 1f)
                {
                    drawRouteTimer += Time.deltaTime;
                }
            }
            else if (drawRouteTimer > 0)
            {

                drawRouteTimer = 0;
            }

            routeLine.material.SetColor(routeLineColorName, routeLineColor * ((drawRouteTimer > drawRouteDelayTime) ? Mathf.Clamp(drawRouteTimer - drawRouteDelayTime, 0, 1f) : 0));

        }

        /// <summary>
        /// Routine to enter in near cover point
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        protected virtual IEnumerator EnterCoverPointRoutine(vCoverPoint target)
        {
            onStartGoToCoverPoint.Invoke();

            inCover = false;
            goingToCoverPoint = true;
            bool needToCrouch = CheckCoverHeight(target, false);
            enterCoverFromLeftSide = EnterCoverFromLeftSide(target);
            var coverDirection = enterCoverFromLeftSide ? target.transform.right : -target.transform.right;
            side = enterCoverFromLeftSide ? Side.left : Side.right;

            currentTimeToGetUp = timeToStandUp - 0.01f;

            if (shooterInput.cc.isCrouching != needToCrouch)
            {
                shooterInput.cc.isCrouching = needToCrouch;
            }
            EnterCoverLocomotionState();
            float t = 0;
            var p1 = transform.position;
            var p2 = target.transform.InverseTransformPoint(p1);
            p2.z = PositionOffsetZ;
            p2 = target.transform.TransformPoint(p2);
            var r1 = transform.rotation;
            var r2 = Quaternion.LookRotation(coverDirection);
            shooterInput.cc.lockRotation = true;
            shooterInput.cc.lockMovement = true;
            shooterInput.cc.inputMagnitude = 0;
            shooterInput.cc.input = Vector3.zero;
            shooterInput.cc.inputSmooth = Vector3.zero;
            shooterInput.cc.moveDirection = Vector3.zero;
            shooterInput.cc._rigidbody.velocity = Vector3.zero;
            shooterInput.cc.ResetInputAnimatorParameters();
            shooterInput.cc.moveToDirectionInFree = true;
            float diference = Mathf.Abs(transform.forward.AngleFormOtherDirection(coverDirection).y);

            target.EnterCover(gameObject);
            //Adjust position and Rotation
            while (diference > 5f)
            {
                t += Time.deltaTime * 5f;
                p2.y = transform.position.y;
                p1.y = transform.position.y;
                shooterInput.cc.transform.position = Vector3.Lerp(p1, p2, Mathf.Clamp(t, 0, 1f));
                shooterInput.cc.transform.rotation = Quaternion.Lerp(r1, r2, Mathf.Clamp(t, 0, 1f));
                diference = Mathf.Abs(transform.forward.AngleFormOtherDirection(coverDirection).y);
                yield return null;
            }
            shooterInput.cc.lockMovement = false;
            currentCoverPoint = target;
            goingToCoverPoint = false;
            inCover = true;
            onFinishGoToCoverPoint.Invoke();
            onEnterCover.Invoke();
            yield return new WaitForEndOfFrame();
            wasInCover = true;
            yield return null;
        }

        /// <summary>
        /// Routine to move character to target cover point
        /// </summary>
        /// <param name="target">Target cover point</param>
        /// <param name="movePath">Way path to move character</param>
        /// <returns></returns>
        protected virtual IEnumerator GoToCoverPointRoutine(vCoverPoint target, List<Vector3> movePath)
        {
            shooterInput.cc.lockInStrafe = true;
            float pathLenght = PathLenght(movePath);
            remainingDistanceToNextCover = pathLenght;
            if (inCover)
            {
                onExitCover.Invoke();
            }

            onStartGoToCoverPoint.Invoke();

            inCover = false;
            goingToCoverPoint = true;

            Vector3 coverDirection = enterCoverFromLeftSide ? target.transform.right : -target.transform.right;
            bool needToCrouch = CheckCoverHeight(target, false);
            float stoppingDistance = .5f;
            Vector3 targetDirection = transform.forward;
            Vector3 targetPosition = transform.position;
            bool slideToCover = false;

            if (movePath.Count > 0)
            {
                SetCameraState(goToCoverCameraState);
                if (movePath != null && movePath.Count > 1)
                {
                    movePath.RemoveAt(0);
                }

                ///Check Action thats needs to travel
                enterCoverFromLeftSide = EnterCoverFromLeftSide(target);
                coverDirection = enterCoverFromLeftSide ? target.transform.right : -target.transform.right;
                side = enterCoverFromLeftSide ? Side.left : Side.right;

                Vector3 p = transform.position;

                slideToCover = useSlideToCover && pathLenght > minPathLenghToSlide.y;
                bool isSliding = false;
                currentCornerWeight = 0;

                PrepareToMoveOnPath(pathLenght);

                targetDirection = transform.forward;
                targetPosition = movePath.Count > 0 ? movePath[movePath.Count - 1] : transform.position;
                Vector3 targetLocalPosition = target.transform.InverseTransformPoint(targetPosition);
                targetLocalPosition.z = PositionOffsetZ;

                targetPosition = target.transform.TransformPoint(targetLocalPosition);
                Debug.DrawLine(targetPosition, targetPosition + Vector3.up, Color.red);
                if (movePath.Count > 0)
                {
                    movePath[movePath.Count - 1] = targetPosition;
                }
                else
                {
                    movePath.Add(targetPosition);
                }

                // Play Default Locomotion State
                PlayGoToCoverPointState();

                // Move Character to Cover Point            
                while (movePath.Count > 1 || (targetPosition - transform.position).magnitude > 0.5f)
                {

                    yield return new WaitForFixedUpdate();

                    remainingDistanceToNextCover = PathLenght(movePath);

                    Vector3 nextPosition = movePath[0];
                    nextPosition.y = transform.position.y;
                    targetDirection = nextPosition - transform.position;
                    targetDirection.y = 0;
                    shooterInput.cc.moveDirection = targetDirection.normalized;
                    shooterInput.cc.MoveToPosition(nextPosition);
                    if (!wasInCover && !shooterInput.cc.isSprinting && shooterInput.cc.isStrafing)
                    {
                        shooterInput.cc.lockRotation = true;
                        shooterInput.cc.isStrafing = true;
                        shooterInput.cc.RotateToDirection(enterCoverFromLeftSide ? target.transform.right : -target.transform.right, 5f);
                    }

                    shooterInput.cc.inputSmooth = shooterInput.cc.input;
                    if (Vector3.Distance(transform.position, nextPosition) < 0.2f)
                    {
                        if (movePath.Count > 1)
                        {
                            movePath.RemoveAt(0);
                        }
                        continue;
                    }
                    // Check Action thats needs to travel               
                    if (genericAction.triggerAction)
                    {
                        if (CheckActionsOnWay(nextPosition) && !shooterInput.cc.customAction)
                        {
                            if (possibleActions.Contains(genericAction.triggerAction.actionName))
                            {
                                genericAction.TriggerActionEvents();
                                genericAction.TriggerAnimation();
                                if (movePath.Count > 1)
                                {
                                    movePath.RemoveAt(0);
                                }

                                continue;
                            }
                            else
                            {
                                ExitCover();
                                onFinishGoToCoverPoint.Invoke();
                                StopCoroutine(currentCoverRoutine);
                            }
                        }

                    }
                    if (!shooterInput.cc.IsAnimatorTag("SlideToCover") && !shooterInput.cc.IsAnimatorTag("CoverLocomotion") && !shooterInput.cc.customAction)
                    {
                        PlayGoToCoverPointState();
                    }
                    if (remainingDistanceToNextCover < (isSliding ? stoppingDistance * 2f : stoppingDistance))
                    {
                        if (isSliding)
                        {
                            shooterInput.cc.input = Vector3.zero;
                            shooterInput.animator.CrossFadeInFixedTime(finishSlideToCoverPoint, finishSlideToCoverAnimCrossfade);
                            yield return new WaitForSeconds(finishSlideToCoverWaitTime);
                        }
                        break;
                    }
                    if (slideToCover && movePath.Count == 1 && !shooterInput.cc.IsAnimatorTag("SlideToCover") && remainingDistanceToNextCover < Mathf.Max(minPathLenghToSlide.x + 0.5f, minPathLenghToSlide.y - 1) && remainingDistanceToNextCover > minPathLenghToSlide.x && !shooterInput.cc.customAction)
                    {
                        shooterInput.animator.CrossFadeInFixedTime(startSlideToCoverPoint, startSlideToCoverAnimCrossfade);
                        isSliding = true;
                    }
                }

                while (shooterInput.cc.customAction || shooterInput.animator.IsInTransition(0))
                {
                    yield return null;
                }
            }
            ///Check Distance to crouch or standup if needs to change
            currentTimeToGetUp = timeToStandUp - 0.01f;
            goingToCoverPoint = false;
            if (shooterInput.cc.isCrouching != needToCrouch)
            {
                shooterInput.cc.isCrouching = needToCrouch;
            }
            EnterCoverLocomotionState(true);
            shooterInput.cc.lockInStrafe = false;

            ///Play Cover Locomotion 
            float t = 0;
            var p1 = transform.position;
            var p2 = targetPosition;
            var r1 = transform.rotation;
            var r2 = Quaternion.LookRotation(coverDirection);
            shooterInput.cc.lockRotation = true;
            shooterInput.cc.lockMovement = true;
            shooterInput.cc.lockSetMoveSpeed = true;
            shooterInput.cc.inputMagnitude = 0;
            shooterInput.cc.input = Vector3.zero;
            shooterInput.cc.inputSmooth = Vector3.zero;
            shooterInput.cc.moveDirection = Vector3.zero;
            shooterInput.cc._rigidbody.velocity = Vector3.zero;
            shooterInput.cc.moveToDirectionInFree = true;
            shooterInput.cc.ResetInputAnimatorParameters();
            float diference = Mathf.Abs(transform.forward.AngleFormOtherDirection(coverDirection).y);

            target.EnterCover(gameObject);
            ///Adjust position and Rotation
            while (diference > 5f)
            {
                t += Time.deltaTime * 5f;
                p2.y = transform.position.y;
                p1.y = transform.position.y;
                shooterInput.cc.transform.position = Vector3.Lerp(p1, p2, Mathf.Clamp(t, 0, 1f));
                shooterInput.cc.transform.rotation = Quaternion.Lerp(r1, r2, Mathf.Clamp(t, 0, 1f));
                diference = Mathf.Abs(transform.forward.AngleFormOtherDirection(coverDirection).y);
                yield return null;
            }
            shooterInput.cc.lockMovement = false;
            currentCoverPoint = target;
            inCover = true;

            onFinishGoToCoverPoint.Invoke();
            onEnterCover.Invoke();
            yield return new WaitForEndOfFrame();
            wasInCover = true;

        }

        /// <summary>
        /// Check if has actions in Character route when is moving along way path
        /// </summary>
        /// <param name="nextWayPoint">Next way path point</param>
        /// <returns></returns>
        protected virtual bool CheckActionsOnWay(Vector3 nextWayPoint, bool checkAngle = true)
        {
            if (!genericAction.triggerAction)
            {
                return false;
            }

            var positionRelativeToTrigger = genericAction.triggerAction._collider.ClosestPoint(nextWayPoint);
            var distance = Vector3.Distance(positionRelativeToTrigger, nextWayPoint);
            positionRelativeToTrigger = genericAction.triggerAction._collider.ClosestPoint(transform.position);
            positionRelativeToTrigger.y = transform.position.y;
            var playerDistance = Vector3.Distance(positionRelativeToTrigger, transform.position);

            bool isClose = true;
            if (genericAction.triggerAction._collider is BoxCollider)
            {
                BoxCollider c = genericAction.triggerAction._collider as BoxCollider;
                isClose = c.IsClosed(transform.position, checkActionMargin, checkActionOffset);
            }
            if (isClose && distance > .2f && playerDistance < shooterInput.cc._capsuleCollider.radius * 0.5f && (!checkAngle || CheckAngleOfDirections(genericAction.triggerAction.transform.forward, shooterInput.cc.moveDirection.normalized, 45)) &&
                !shooterInput.cc.customAction)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Enter Cover Locomoition state
        /// </summary>
        /// <param name="target">Target Cover Point</param>
        protected virtual void EnterCoverLocomotionState(bool playCoverLocomotion = true)
        {
            if (debug)
            {
                Debug.Log("Enter Cover");
            }

            shooterInput.SetWalkByDefault(originalWalkByDefault);
            shooterInput.lockCameraInput = false;
            shooterInput.cc.lockRotation = true;
            shooterInput.cc.isStrafing = false;
            shooterInput.cc.lockInStrafe = false;

            shooterInput.cc.isSprinting = false;
            shooterInput.crouchInput.useInput = false;

            shooterInput.SetLockAllInput(false);
            shooterInput.lockMoveInput = false;
            shooterInput.SetLockUpdateMoveDirection(true);
            if (playCoverLocomotion)
            {
                ControlSideInCover();
                PlayCoverLocomotionState(enterCoverFromLeftSide);

            }

        }

        /// <summary>
        /// Play Go To Cover Point Animation state 
        /// </summary>
        protected virtual void PlayGoToCoverPointState()
        {
            string state = goToCoverPoint;
            shooterInput.animator.CrossFadeInFixedTime(state, goToCoverAnimCrossfade);
        }

        /// <summary>
        /// Play Cover Locomotion Animator State
        /// </summary>
        /// <param name="enterCoverFromLeftSide">Needs to enter using left side Animation state</param>
        protected virtual void PlayCoverLocomotionState(bool enterCoverFromLeftSide)
        {
            if (debug)
            {
                Debug.Log("Play Cover Anim");
            }

            side = enterCoverFromLeftSide ? Side.left : Side.right;
            shooterInput.cc.animator.SetFloat("CoverSide", (int)side, 0.2f, Time.deltaTime);
            string state = enterCoverFromLeftSide ? (shooterInput.cc.isCrouching ? coverCrouchedLeft : coverStandingLeft) :
                                                    (shooterInput.cc.isCrouching ? coverCrouchedRight : coverStandingRight);
            shooterInput.animator.CrossFadeInFixedTime(state, enterCoverAnimCrossfade);
        }

        /// <summary>
        /// Check if Needs to enter cover using the left side
        /// </summary>
        /// <param name="coverPoint">Target Cover point</param>
        /// <returns></returns>
        protected virtual bool EnterCoverFromLeftSide(vCoverPoint coverPoint)
        {
            if (coverPoint == null)
            {
                return false;
            }

            isLeftCorner = IsLeftCoverCorner(coverPoint);
            isRightCorner = IsRightCoverCorner(coverPoint);

            if (isRightCorner)
            {
                return false;
            }

            if (isLeftCorner)
            {
                return true;
            }

            Vector3 sightDirection = coverPoint.transform.position - shooterInput.cameraMain.transform.position;
            sightDirection.y = 0;
            float angleOfSight = (Quaternion.LookRotation(sightDirection.normalized).eulerAngles - Quaternion.LookRotation(-coverPoint.transform.forward).eulerAngles).NormalizeAngle().y;
            return angleOfSight < -15;
        }

        /// <summary>
        /// Play Default Locomotion State
        /// </summary>
        protected virtual void PlayDefaultLocomotionState()
        {
            string state = shooterInput.cc.isStrafing ? shooterInput.cc.isCrouching ? defaultStrafeCrouched : defaultStrafeStanding :
                                                        shooterInput.cc.isCrouching ? defaultFreeCrouched : defaultFreeStanding;
            shooterInput.animator.CrossFadeInFixedTime(state, exitCoverAnimCrossfade);
        }

        /// <summary>
        /// Exit cover and Reset the Controller
        /// </summary>
        protected virtual void ExitCover(bool forceExit = false)
        {
            if (debug)
            {
                Debug.Log("Exit Cover");
            }
            StopAllCoroutines();
            ResetCoverCornerUI();
            currentCornerWeight = 0;
            shooterInput.animator.SetLayerWeight(animatorCoverCornerLayer, 0);
            shooterInput.animator.SetLayerWeight(animatorCoverOverBarrierLayer, 0);
            shooterInput.tpCamera.switchRight = 1;
            shooterInput.ResetCameraState();
            shooterInput.SetLockUpdateMoveDirection(false);
            shooterInput.SetLockAllInput(false);
            shooterInput.lockCameraInput = false;
            shooterInput.lockUpdateMoveDirection = false;
            shooterInput.lockMoveInput = false;
            shooterInput.crouchInput.useInput = true;
            shooterInput.cc.ignoreAnimatorMovement = false;
            shooterInput.cc.lockRotation = false;
            shooterInput.cc.isSprinting = false;
            shooterInput.cc.lockSetMoveSpeed = false;
            shooterInput.cc.ResetCapsule();
            shooterInput.cc.lockInStrafe = false;
            shooterInput.shooterManager.hipfireAimTime = lastHipFireTimer;
            shooterInput.shooterManager.hipfireShot = lastUseHipFire;
            shooterInput.cc.moveToDirectionInFree = lastMoveToDirectionInFree;
            if (alwaysStandUpOnExit)
            {
                shooterInput.cc.isCrouching = false;
            }
            currentCoverPoint = null;
            possibleCoverPoint = null;
            possibleCornerPoint = null;
            goingToCoverPoint = false;
            goingToCornerPoint = false;
            isAimingInCorner = false;
            isRightCorner = false;
            isLeftCorner = false;
            isInRightCornerPositionRange = false;
            isInLeftCornerPositionRange = false;
            inCornerAngle = false;
            canMove = false;

            // reset camera state to default
            shooterInput.ResetCameraState();
            // reset custom ik adjust
            shooterInput.ResetCustomIKAdjustState();
            // always reset the camera side to right
            shooterInput.tpCamera.switchRight = 1;
            if (shooterInput.cc.IsAnimatorTag("CoverLocomotion") && ((!shooterInput.cc.customAction && !isRolling && !shooterInput.cc.isDead) || forceExit))
            {

                PlayDefaultLocomotionState();
            }

            wasInCover = false;
            inCover = false;
            goingToCoverPoint = false;
            shooterInput.animator.speed = 1;
        }

        #endregion

        #region Corner Action

        /// <summary>
        /// Control the possible corner point to travel when in cover
        /// </summary>
        protected virtual void HandlePossibleCornerPoint()
        {
            if (!inCover || goingToCoverPoint || !currentCoverPoint || goingToCornerPoint || shooterInput.IsAiming)
            {
                ResetCoverCornerUI();
                return;
            }

            holdingCornerInput = enterUsingGetCoverInput ? enterExitInput.GetButton() : false;
            bool inputToCorner = IsLookingToCorner();
            possibleCornerPoint = side == Side.left ? currentCoverPoint.leftCorner : currentCoverPoint.rightCorner;

            isInCornerInputAction = inputToCorner && possibleCornerPoint && enterUsingGetCoverInput;
            if (cornerInputUI)
            {
                var activeCornerInputUI = isInCornerInputAction;
                if (cornerInputUI.activeSelf != activeCornerInputUI)
                {
                    cornerInputUI.SetActive(activeCornerInputUI);
                }

                if (possibleCornerPoint)
                {
                    cornerInputUI.transform.SetPositionAndRotation(possibleCornerPoint.transform.position + possibleCornerPoint.transform.forward * 0.1f + Vector3.up * CornerUIHeight * 0.25f, Quaternion.LookRotation(possibleCornerPoint.transform.forward));
                }

            }
            if (cornerUICenter)
            {
                cornerUICenter.transform.localPosition = Vector3.up * CornerUIHeight;
            }
            if (inputToCorner && possibleCornerPoint && (!enterUsingGetCoverInput || holdingCornerInput))
            {
                currentTimeToEnterCorner += Time.deltaTime;
                if (cornerRightUI)
                {
                    if (cornerRightUI.gameObject.activeSelf != (side == Side.right && !enterUsingGetCoverInput))
                    {
                        cornerRightUI.gameObject.SetActive(side == Side.right && !enterUsingGetCoverInput);
                    }
                }
                if (cornerLeftUI)
                {
                    if (cornerLeftUI.gameObject.activeSelf != (side == Side.left && !enterUsingGetCoverInput))
                    {
                        cornerLeftUI.gameObject.SetActive(side == Side.left && !enterUsingGetCoverInput);
                    }
                }

                onUpdateCornerInput.Invoke(currentTimeToEnterCorner / timeToEnterCorner);
                if (currentTimeToEnterCorner > timeToEnterCorner)
                {
                    if (CalculatePath(possibleCornerPoint.posePosition) && path.status == UnityEngine.AI.NavMeshPathStatus.PathComplete)
                    {
                        StartGoToCornerPoint(possibleCornerPoint);
                    }
                }
            }
            else if (currentTimeToEnterCorner > 0)
            {
                onUpdateCornerInput.Invoke(0);
                currentTimeToEnterCorner = 0;
                if (cornerRightUI && cornerRightUI.gameObject.activeSelf)
                {
                    cornerRightUI.gameObject.SetActive(false);
                }

                if (cornerLeftUI && cornerLeftUI.gameObject.activeSelf)
                {
                    cornerLeftUI.gameObject.SetActive(false);
                }
            }

        }

        /// <summary>
        /// Start Go To Corner Action
        /// </summary>
        /// <param name="newCp"></param>
        protected virtual void StartGoToCornerPoint(vCoverPoint newCp)
        {
            goingToCornerPoint = true;
            shooterInput.cc.lockSetMoveSpeed = true;
            if (cornerInputUI && cornerInputUI.gameObject.activeSelf)
            {
                cornerInputUI.SetActive(false);
            }

            currentTimeToEnterCorner = 0;
            onUpdateCornerInput.Invoke(0);
            var p1 = side == Side.left ? currentCoverPoint.leftCornerP : currentCoverPoint.rightCornerP;
            p1 = currentCoverPoint.transform.InverseTransformPoint(p1);
            p1.z = localCharPosition.z;
            p1 = currentCoverPoint.transform.TransformPoint(p1);
            p1 = newCp.transform.InverseTransformPoint(p1);
            p1.z = localCharPosition.z;
            p1 = newCp.transform.TransformPoint(p1);
            var p2 = newCp.transform.InverseTransformPoint(newCp.posePosition);
            p2.z = localCharPosition.z;
            p2 = newCp.transform.TransformPoint(p2);
            Vector3[] cornerPath = (new Vector3[] { transform.position, p1, p2 }).MakeSmoothCurve(.25f);
            cornerPath = cornerPath.MakeSmoothCurve(.25f);
            var wP = cornerPath[0];


            if (cornerRightUI && cornerRightUI.gameObject.activeSelf)
            {
                cornerRightUI.gameObject.SetActive(false);
            }

            if (cornerLeftUI && cornerLeftUI.gameObject.activeSelf)
            {
                cornerLeftUI.gameObject.SetActive(false);
            }
            currentCoverPoint = newCp;
            currentCoverRoutine = StartCoroutine(GoToCornerPointRoutine(cornerPath));
        }

        /// <summary>
        /// Go To Corner Action Routine
        /// </summary>
        /// <param name="cornerPath"></param>
        /// <returns></returns>
        protected virtual IEnumerator GoToCornerPointRoutine(Vector3[] cornerPath)
        {
            int pathIndex = 0;

            PrepareToMoveOnPath(PathLenght(cornerPath.vToList()), shooterInput.cc.isCrouching);

            Vector3 targetPosition = targetPosition = cornerPath[pathIndex];
            targetPosition.y = transform.position.y;
            Vector3 comparePosition = transform.position;

            Vector3 targetDirection = targetPosition - comparePosition;
            targetDirection.y = 0;
            while (pathIndex < cornerPath.Length)
            {

                yield return null;
                ControlCoverHelperTransform();
                comparePosition = transform.position;
                targetPosition = cornerPath[pathIndex];
                targetDirection = targetPosition - comparePosition;
                targetPosition.y = transform.position.y;
                if (targetDirection.magnitude > 0.25f)
                {
                    targetDirection.y = 0;
                    shooterInput.cc.MoveToPosition(transform.position + targetDirection.normalized);
                    shooterInput.cc.inputMagnitude = 1;
                    shooterInput.cc.inputSmooth = shooterInput.cc.input;
                }
                else
                {
                    if (pathIndex + 1 > cornerPath.Length - 1)
                    {
                        break;
                    }
                    else
                    {
                        pathIndex++;
                    }
                }
            }

            shooterInput.lockCameraInput = false;
            shooterInput.cc.lockRotation = true;
            shooterInput.cc.isStrafing = false;
            shooterInput.cc.lockInStrafe = false;
            shooterInput.crouchInput.useInput = false;
            shooterInput.SetLockAllInput(false);
            shooterInput.lockMoveInput = false;
            goingToCornerPoint = false;
            isInCornerInputAction = false;
        }
        #endregion

        #endregion

        #region Cover Movement

        /// <summary>
        /// Handles all methods related to movement
        /// </summary>
        protected virtual void HandleCoverMovement()
        {
            if (inCover || goingToCornerPoint || goingToCoverPoint)
            {
                ControlSpeedInCover();
            }

            if (!inCover)
            {
                return;
            }

            ControlNearCoverPoint();
            if (!currentCoverPoint)
            {
                return;
            }

            ControlCoverHelperTransform();
            ControlCrouchStateInCover();
            ControlInputDirectionInCover();
            ControlMovementDirectionInCover();
            ControlCoverAngles();
            ControlSideInCover();
            ControlCoverLocomotionState();

            ControlRotationInCover();
            ControlMovementInCover();
            CheckOverBarrier();
            ControlCameraState();
            ControlIKState();
            ControlExitCoverByAngle();
        }

        /// <summary>
        /// Control the <see cref="helper"/> transform position and rotation , and <see cref="coverNormal"/>
        /// </summary>
        protected virtual void ControlCoverHelperTransform()
        {
            if (!currentCoverPoint)
            {
                return;
            }

            var _coverNormal = currentCoverPoint.transform.forward;
            var _positionHelper = currentCoverPoint.posePosition;


            int count = 1;
            for (int i = 0; i < coverPoints.Count; i++)
            {
                if (coverPoints[i] != currentCoverPoint && currentCoverPoint.leftCorner != coverPoints[i] && currentCoverPoint.rightCorner != coverPoints[i])
                {
                    count++;

                    if (currentCoverPoint.corner == 0)
                    {
                        _positionHelper += coverPoints[i].posePosition;
                    }
                }
            }

            if (count > 1)
            {
                if (currentCoverPoint.corner == 0)
                {
                    _positionHelper /= coverPoints.Count;

                }
            }
            vCoverPoint right = currentCoverPoint.right;
            vCoverPoint left = currentCoverPoint.left;

            var localHelperPosition = currentCoverPoint.transform.InverseTransformPoint(_positionHelper);
            ///Clamp helper in corners;
            if (isLeftCorner || isRightCorner)
            {
                if (isLeftCorner)
                {
                    if (localHelperPosition.x >= 0)
                    {
                        localHelperPosition.x = 0;

                    }
                    _positionHelper = currentCoverPoint.transform.TransformPoint(localHelperPosition);
                }
                else if (isRightCorner)
                {
                    if (localHelperPosition.x <= 0)
                    {
                        localHelperPosition.x = 0;
                    }
                    _positionHelper = currentCoverPoint.transform.TransformPoint(localHelperPosition);
                }
                _coverNormal.y = 0;
                _coverNormal.Normalize();
                _positionHelper -= _coverNormal * currentCoverPoint.posePositionZ;
                if (_coverNormal.magnitude > 0)
                {
                    helper.transform.position = _positionHelper;
                    helper.forward = _coverNormal;
                    coverNormal = _coverNormal;
                    positionHelper = _positionHelper;
                }
            }
            else
            {
                if ((int)side == 1 && right)
                {
                    var p1 = currentCoverPoint.posePosition;
                    var p2 = right.posePosition;

                    var dir = right.transform.forward;
                    dir.y = 0;
                    float distance = Vector3.Distance(p1, p2);
                    float currentDistante = Mathf.Clamp(Vector3.Distance(transform.position, p2) - 0.2f, 0, distance);
                    float evaluate = 1f - Mathf.Clamp(currentDistante / distance, 0, 1f);

                    _coverNormal = Vector3.Lerp(_coverNormal, dir.normalized, evaluate);
                    _positionHelper = Vector3.Lerp(p1, p2, evaluate);
                }
                if ((int)side == -1 && left)
                {
                    var p1 = currentCoverPoint.posePosition;
                    var p2 = left.posePosition;
                    var dir = left.transform.forward;
                    dir.y = 0;
                    float distance = Vector3.Distance(p1, p2);
                    float currentDistante = Mathf.Clamp(Vector3.Distance(transform.position, p2) - 0.2f, 0, distance);
                    float evaluate = 1f - Mathf.Clamp(currentDistante / distance, 0, 1f);

                    _coverNormal = Vector3.Lerp(_coverNormal, dir.normalized, evaluate);
                    _positionHelper = Vector3.Lerp(p1, p2, evaluate);
                }
                _positionHelper -= _coverNormal * currentCoverPoint.posePositionZ;

                _coverNormal.y = 0;
                _coverNormal.Normalize();
                if (_coverNormal.magnitude > 0)
                {
                    RaycastHit hit;
                    Debug.DrawLine(_positionHelper + _coverNormal * PositionOffsetZ + Vector3.up * 0.2f, _positionHelper + Vector3.up * 0.2f, Color.red);
                    if (Physics.Linecast(_positionHelper + _coverNormal * PositionOffsetZ + Vector3.up * 0.2f, _positionHelper + Vector3.up * 0.2f, out hit, obstaclesLayer))
                        _positionHelper = hit.point - Vector3.up * 0.2f;
                    helper.transform.position = _positionHelper;
                    helper.forward = _coverNormal;
                    coverNormal = _coverNormal;
                    positionHelper = _positionHelper;
                }
            }

        }

        /// <summary>
        /// Control if needs to Crouch when in cover
        /// </summary>
        protected virtual void ControlCrouchStateInCover()
        {
            shooterInput.cc.isCrouching = CheckCoverHeight(currentCoverPoint, shooterInput.isAimingByInput);
        }

        /// <summary>
        /// Control the direction of the input when in cover
        /// </summary>
        protected virtual void ControlInputDirectionInCover()
        {
            cameraRightDir = shooterInput.cameraMain.transform.right;
            cameraRightDir.y = 0;
            cameraRightDir.Normalize();
            //get the forward direction relative to referenceTransform Right
            cameraForwardDir = Quaternion.AngleAxis(-90, Vector3.up) * cameraRightDir;
            inputDirection = (shooterInput.cc.inputSmooth.x * cameraRightDir) + (shooterInput.cc.inputSmooth.z * cameraForwardDir);
        }

        /// <summary>
        /// Control the movement direction based to <see cref="ControlInputDirectionInCover"/> result
        /// </summary>
        protected virtual void ControlMovementDirectionInCover()
        {
            if (!inCover)
            {
                return;
            }

            coverMoveDirection = helper.InverseTransformDirection(inputDirection);
        }

        /// <summary>
        /// Control all necessary angles  o handle cover controller movement and side
        /// </summary>
        protected virtual void ControlCoverAngles()
        {
            var coverNormalEuler = Quaternion.LookRotation(-helper.forward).eulerAngles;
            var cameraEuler = Quaternion.LookRotation(cameraForwardDir).eulerAngles;
            cameraAngle = (int)(cameraEuler - coverNormalEuler).NormalizeAngle().y;
            if (inputDirection == Vector3.zero)
            {
                movementAngle = 0;
                return;
            }
            var moveEuler = Quaternion.LookRotation(inputDirection.normalized).eulerAngles;
            movementAngle = (int)(moveEuler - coverNormalEuler).NormalizeAngle().y;
        }

        /// <summary>
        /// Control side relative to <see cref="currentCoverPoint"/>
        /// </summary>
        protected virtual void ControlSideInCover()
        {
            if (goingToCornerPoint)
            {
                return;
            }

            Vector3 localMovementDirection = coverMoveDirection;
            float localMovementMagnitude = localMovementDirection.magnitude;
            if (Mathf.Abs(localMovementDirection.y) > 0)
            {
                localMovementDirection.y = 0;
            }
            float localX = (float)System.Math.Round(localCharPosition.x, 1);
            ///Determine Side (left or right) 
            ///Not Aiming
            if (!shooterInput.IsAiming)
            {
                float angleOfInput = inputDirection.magnitude > 0.1 && coverNormal.magnitude > 0.1 ? Mathf.Abs(inputDirection.AngleFormOtherDirection(coverNormal).y) : 0;
                if (localMovementDirection.magnitude > 0.1f && angleOfInput.IsInSideRange(20, 160))
                {
                    if (localMovementDirection.x > .1)
                    {
                        side = Side.left;
                    }
                    else if (localMovementDirection.x < -0.1)
                    {
                        side = Side.right;
                    }
                }
                else if (localMovementDirection.magnitude < 0.1f)
                {
                    if (changeSide)
                    {
                        if (Mathf.Abs(cameraAngle) > angleToChangeSide.x)
                        {
                            if (side == Side.right)
                            {
                                if (cameraAngle < -angleToChangeSide.x && cameraAngle > -angleToChangeSide.y)
                                {
                                    side = Side.left;
                                }
                            }
                            else if (side == Side.left)
                            {
                                if (cameraAngle > angleToChangeSide.x && cameraAngle < angleToChangeSide.y)
                                {
                                    side = Side.right;
                                }
                            }
                        }
                    }
                }
            }
            else ///When Aiming
            {
                if (changeSideAiming)
                {
                    if (cameraAngle.IsInSideRange(-angleToChangeSideOnAiming.x, angleToChangeSideOnAiming.x))
                    {

                        if (isLeftCorner && side == Side.right && localX >= 0 && localMovementDirection.x > 0.9f)
                        {
                            side = Side.left;
                        }
                        else if (isRightCorner && side == Side.left && localX <= 0 && localMovementDirection.x < -0.9f)
                        {
                            side = Side.right;
                        }
                    }
                    else if (side == Side.left)
                    {
                        if (cameraAngle.IsInSideRange(angleToChangeSideOnAiming.x, angleToChangeSideOnAiming.y))
                        {
                            side = Side.right;
                        }

                    }
                    else if (side == Side.right)
                    {
                        if (cameraAngle.IsInSideRange(-angleToChangeSideOnAiming.y, -angleToChangeSideOnAiming.x))
                        {
                            side = Side.left;
                        }

                    }
                }
            }

            ///Store the last side update
            enterCoverFromLeftSide = side == Side.left;
        }

        /// <summary>
        /// Control state of Locomotion based on Animator Tag and Aiming state
        /// </summary>
        protected virtual void ControlCoverLocomotionState()
        {
            shooterInput.cc.animator.SetFloat("CoverSide", (int)side, 0.2f, Time.deltaTime);
            shooterInput.animator.SetLayerWeight(animatorCoverCornerLayer, currentCornerWeight);
            if (!shooterInput.IsAiming)
            {

                if (!shooterInput.cc.IsAnimatorTag("CoverLocomotion") && EnterCoverConditions())
                {
                    shooterInput.cc.isCrouching = CheckCoverHeight(currentCoverPoint, false);
                    onEnterCover.Invoke();
                    PlayCoverLocomotionState(enterCoverFromLeftSide);
                }
            }
            else if (shooterInput.IsAiming)
            {

                if (shooterInput.cc.IsAnimatorTag("CoverLocomotion") && !shooterInput.animator.IsInTransition(0))
                {
                    onExitCover.Invoke();
                    string stateToAiming = shooterInput.cc.isCrouching ? "Strafing Crouch" : "Strafing Movement";
                    shooterInput.animator.CrossFadeInFixedTime(stateToAiming, exitCoverAnimCrossfade);
                }
            }
        }

        /// <summary>
        /// Control movement speed when in cover
        /// </summary>
        protected virtual void ControlSpeedInCover()
        {
            shooterInput.cc.lockSetMoveSpeed = !shooterInput.IsAiming;
            if (!shooterInput.IsAiming)
            {
                float speed = 1;
                if (goingToCoverPoint)
                {
                    moveSpeed.walkByDefault = false;
                    speed = shooterInput.cc.isSprinting ? goToCoverPointSprintingAnimatorSpeed : goToCoverPointRunningAnimatorSpeed;
                }
                else if (goingToCornerPoint)
                {
                    moveSpeed.walkByDefault = true;
                    speed = goToCornerPointAnimatorSpeed;
                }
                else if (shooterInput.cc.isCrouching)
                {
                    moveSpeed.walkByDefault = true;
                    speed = crouchAnimatorSpeed;
                }
                else
                {
                    moveSpeed.walkByDefault = true;
                    speed = standAnimatorSpeed;
                }
                shooterInput.animator.speed = Mathf.Lerp(1, speed, Mathf.Min(shooterInput.cc.inputSmooth.magnitude, 1f));
                moveSpeed.crouchSpeed = goingToCornerPoint ? goToCornerCrouchedMoveSpeed : crouchedMoveSpeed;
                moveSpeed.walkSpeed = goingToCornerPoint ? goToCornerStandingMoveSpeed : standingMoveSpeed;
                moveSpeed.runningSpeed = goToCoverPointRunningMoveSpeed;
                moveSpeed.sprintSpeed = goToCoverPointSprintingMoveSpeed;
                shooterInput.cc.SetControllerMoveSpeed(moveSpeed);
                shooterInput.cc.SetAnimatorMoveSpeed(moveSpeed);
            }
            else
            {
                shooterInput.animator.speed = 1;
            }
        }

        /// <summary>
        /// Control the rotation when in cover, based to <see cref="ControlSideInCover"/> result
        /// </summary>
        protected virtual void ControlRotationInCover()
        {

            if (goingToCornerPoint || !inCover)
            {
                return;
            }
            shooterInput.cc.lockRotation = !shooterInput.IsAiming || (shooterInput.isAimingByInput && currentCornerWeight > 0.1f);
            if (shooterInput.IsAiming)
            {
                if (shooterInput.cc.lockRotation && currentCornerWeight > 0.1f)
                {
                    var cornerAimDir = -currentCoverPoint.transform.forward;
                    cornerAimDir.y = 0;
                    transform.rotation = Quaternion.LerpUnclamped(transform.rotation, Quaternion.LookRotation(cornerAimDir), currentCornerWeight);
                }
                return;
            }
            else if (shooterInput.cc.lockRotation)
            {
                var sideDir = Quaternion.AngleAxis((-90) * (int)side, Vector3.up) * coverNormal.normalized;
                var dir = Quaternion.AngleAxis((-70) * (int)side, Vector3.up) * coverNormal.normalized;
                var angleToReachRotation = Vector3.Angle(transform.forward, dir);
                var eulerY = angleToReachRotation > 25 ? Quaternion.LookRotation(dir).eulerAngles.y : Quaternion.LookRotation(sideDir).eulerAngles.y;
                var euler = transform.rotation.eulerAngles;
                euler.y = Mathf.LerpAngle(euler.y, eulerY, rotationSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(euler);
            }
        }

        /// <summary>
        /// Control movement when in cover
        /// </summary>
        protected virtual void ControlMovementInCover()
        {
            if (goingToCornerPoint)
            {
                return;
            }

            Vector3 localMovementDirection = coverMoveDirection;
            float localMovementMagnitude = localMovementDirection.magnitude;
            if (Mathf.Abs(localMovementDirection.y) >= 0)
            {
                localMovementDirection.y = 0;
            }

            if (Mathf.Abs(localMovementDirection.x) <= 0.1f)
            {
                localMovementDirection.x = 0;
            }

            shooterInput.cc.ignoreAnimatorMovement = shooterInput.cc.inputSmooth.magnitude < .1f;

            isLeftCorner = IsLeftCoverCorner(currentCoverPoint);
            isRightCorner = IsRightCoverCorner(currentCoverPoint);


            localCharPosition = helper.InverseTransformPoint(transform.position);
            localCharPosition.z = PositionOffsetZ;
            inCornerAngle = !shooterInput.isAimingByInput || (Mathf.Abs(cameraAngle) < 60f);
            float localMovementX = localMovementDirection.x;

            float clampX = XLocalClampedInCorner;

            if (clampX < 0 && isLeftCorner && isRightCorner)
            {
                clampX = 0;
            }

            float cornerWeight = 0;

            ///Clamp Character in Corners;             
            float localCharPositionX = (float)System.Math.Round(localCharPosition.x, 2);
            if (isLeftCorner)
            {
                if (localCharPositionX >= clampX)
                {
                    localCharPosition.x = clampX;
                    if (localMovementX > -0.1f)
                    {
                        localMovementDirection = Vector3.zero;
                    }
                }
                isInLeftCornerPositionRange = side == Side.left && localCharPosition.x >= clampX - (wasAiming ? 0.1f : shooterInput.cc.colliderRadiusDefault + 0.1f);
            }
            else
            {
                isInLeftCornerPositionRange = false;
            }

            if (isRightCorner)
            {

                if (localCharPositionX <= -clampX)
                {
                    localCharPosition.x = -clampX;

                    if (localMovementX < 0.1f)
                    {
                        localMovementDirection = Vector3.zero;
                    }
                }
                isInRightCornerPositionRange = side == Side.right && localCharPosition.x <= -(clampX - (wasAiming ? 0.1f : shooterInput.cc.colliderRadiusDefault + 0.1f));
            }
            else
            {
                isInRightCornerPositionRange = false;
            }

            if (shooterInput.aimInput.GetButtonDown())
            {
                if (side == Side.right)
                {

                    isInRightCorner = isInRightCornerPositionRange && inCornerAngle;
                    isInLeftCorner = false;
                }
                else if (side == Side.left)
                {

                    isInLeftCorner = isInLeftCornerPositionRange && inCornerAngle;
                    isInRightCorner = false;
                }
                isAimingInCorner = isInRightCorner || isInLeftCorner;

            }
            else if (shooterInput.isAimingByInput && inCornerAngle)
            {
                if (isInRightCorner && localMovementX > 0.1)
                {

                    isAimingInCorner = false;
                    isInRightCorner = false;
                }
                else if (!isInRightCorner && isInRightCornerPositionRange && (localMovementX < -0.1f || isAimingInCorner))
                {

                    isAimingInCorner = true;
                    isInRightCorner = true;
                }

                if (isInLeftCorner && localMovementX < -0.1)
                {

                    isAimingInCorner = false;
                    isInLeftCorner = false;
                }
                else if (!isInLeftCorner && isInLeftCornerPositionRange && (localMovementX > 0.1f || isAimingInCorner))
                {
                    isAimingInCorner = true;
                    isInLeftCorner = true;
                }
            }
            else
            {
                if (!shooterInput.isAimingByInput || (!inCornerAngle && isInLeftCornerPositionRange && !isInRightCornerPositionRange))
                {
                    isAimingInCorner = false;
                }
                isInRightCorner = false;
                isInLeftCorner = false;
            }
            if ((isInRightCorner || isInLeftCorner) && shooterInput.isAimingByInput)
            {
                cornerWeight = 1;
            }
            else
            {
                cornerWeight = 0;
            }

            currentCornerWeight = Mathf.Lerp(currentCornerWeight, cornerWeight, cornerWeightSmooth * Time.deltaTime);
            if ((currentCornerWeight >= .9f || (shooterInput.isAimingByInput && !isInRightCornerPositionRange && !isInLeftCornerPositionRange)) && !wasAiming)
            {
                wasAiming = true;
            }
            else if (currentCornerWeight < .9f && wasAiming && !shooterInput.isAimingByInput)
            {
                wasAiming = false;
            }
            var localPositionX = localCharPosition.x;
            if (currentCornerWeight > 0.1f)
            {
                localPositionX = Mathf.Lerp(localPositionX, helper.InverseTransformPoint(CornerAimingPosition).x, currentCornerWeight);
                localCharPosition = helper.InverseTransformPoint(transform.position);
                localCharPosition.x = localPositionX;
                transform.position = helper.TransformPoint(localCharPosition);
            }

            transform.position = Vector3.Lerp(transform.position, helper.TransformPoint(localCharPosition), (localCharPosition.z > PositionOffsetZ ? 20 : 5f) * Time.deltaTime);
            float minDirection = Mathf.Abs(localMovementDirection.x) * 0.25f;
            localMovementDirection.z = Mathf.Clamp(localMovementDirection.z, -minDirection, minDirection);
            canMove = shooterInput.IsAiming ? true : localMovementDirection.magnitude >= minDirection * 4f && localMovementDirection.x > 0 && side == Side.left || (localMovementDirection.x < 0 && side == Side.right);
            shooterInput.cc.moveDirection = helper.TransformDirection(localMovementDirection) * (canMove ? 1f : 0);
            Debug.DrawRay(transform.position + Vector3.up * 0.2f, shooterInput.cc.moveDirection, Color.green);
            if (!canMove || localMovementDirection.magnitude == 0)
            {
                shooterInput.cc.inputMagnitude = 0;
            }
        }

        /// <summary>
        /// Control current state of the Thirdperson camera when in cover
        /// </summary>
        protected virtual void ControlCameraState()
        {
            float switchRight = (int)side;

            if (isInRightCornerPositionRange)
            {
                cameraSwtichLeft = 0;
                cameraSwtichRight = Mathf.Clamp(cameraSwtichRight + Time.deltaTime * 10, 0, 1);
                switchRight += Mathf.Lerp(0, isAimingInCorner ? cornerRightAiming_OffsetCameraRight : cornerRight_OffsetCameraRight, cameraSwtichRight);
            }
            else if (isInLeftCornerPositionRange)
            {
                cameraSwtichRight = 0;
                cameraSwtichLeft = Mathf.Clamp(cameraSwtichLeft + Time.deltaTime * 10, 0, 1);
                switchRight -= Mathf.Lerp(0, isAimingInCorner ? cornerLeftAiming_OffsetCameraRight : cornerLeft_OffsetCameraRight, cameraSwtichLeft);
            }
            else
            {
                cameraSwtichLeft = 0;
                cameraSwtichRight = 0;
            }
            shooterInput.tpCamera.switchRight = switchRight;
            if (shooterInput.cc.isCrouching)
            {
                SetCameraState(shooterInput.IsAiming ? aimingOverBarrier ? aimingOverBarrierCameraState : crouchAimingCameraState : crouchCameraState);
            }
            else
            {
                SetCameraState(shooterInput.isAimingByInput ? standAimingCameraState : standCameraState);
            }
        }

        /// <summary>
        /// Control Shooter IK Weapon poses using <seealso cref="vShooterMeleeInput.CustomIKAdjustState"/>
        /// </summary>
        protected virtual void ControlIKState()
        {
            if (!inCover) return;
            var _currentIKPose = TargetIKPose;

            if (string.IsNullOrEmpty(_currentIKPose) && !string.IsNullOrEmpty(shooterInput.CustomIKAdjustState))
            {
                currentIKPose = _currentIKPose;
                shooterInput.ResetCustomIKAdjustState();
            }
            else if (!string.IsNullOrEmpty(_currentIKPose) && _currentIKPose != currentIKPose)
            {
                currentIKPose = _currentIKPose;
                shooterInput.SetCustomIKAdjustState(currentIKPose);
            }
        }

        /// <summary>
        /// Check if Input direction is in exit angle
        /// </summary>
        protected virtual void ControlExitCoverByAngle()
        {
            if (!autoExitCover || shooterInput.cc.input.magnitude < 0.1f || goingToCornerPoint)
            {
                timeToExitByDirection = 0;
                return;
            }

            float angle = Vector3.Angle(inputDirection, -helper.transform.forward);
            if (angle > exitInputDirectionAngle)
            {
                timeToExitByDirection += Time.deltaTime;
                if (timeToExitByDirection > exitDirectionTimer)
                {
                    if (shooterInput.cc.IsAnimatorTag("CoverLocomotion"))
                    {
                        onExitCover.Invoke();
                    }

                    ExitCover();
                }
            }
            else
            {
                timeToExitByDirection = 0;
            }
        }
        #endregion

        #region Cover Helper Methods        
        /// <summary>
        /// Trigger Stay to check interactions with <see cref="vCoverPoint"/>
        /// </summary>
        /// <param name="other"></param>
        /// 
        protected virtual void OnTriggerStay(Collider other)
        {
            if (!coverTag.Contains(other.gameObject.tag))
            {
                return;
            }

            CheckCoverTrigger(other);
        }

        /// <summary>
        /// Trigger Exit to Check if needs to remove a <seealso cref="vCoverPoint"/> of the <see cref="coverPoints"/> List
        /// </summary>
        /// <param name="other"></param>
        protected virtual void OnTriggerExit(Collider other)
        {
            if (!coverTag.Contains(other.gameObject.tag))
            {
                return;
            }

            var newCp = other.gameObject.GetComponent<vCoverPoint>();
            if (newCp)
            {
                if (coverPoints.Contains(newCp) && (!inCover || coverPoints.Count > 0))
                {
                    coverPoints.Remove(newCp);
                }
            }
        }

        /// <summary>
        /// Check if collider is a <see cref="vCoverPoint"/> and add to the <see cref="coverPoints"/> List
        /// </summary>
        /// <param name="other"></param>
        protected virtual void CheckCoverTrigger(Collider other)
        {
            if (currentCoverPoint && other.gameObject == currentCoverPoint.gameObject)
            {
                return;
            }

            var newCp = other.gameObject.GetComponent<vCoverPoint>();
            if (newCp)
            {
                if (!coverPoints.Contains(newCp))
                {
                    coverPoints.Add(newCp);
                }
            }
        }

        /// <summary>
        /// Set state to thirdperson camera
        /// </summary>
        /// <param name="state">target state</param>
        protected virtual void SetCameraState(string state)
        {
            if (shooterInput.customCameraState != state)
            {
                shooterInput.ChangeCameraState(state);
            }
        }

        /// <summary>
        /// Get target ik pose based on Cover state
        /// </summary>
        protected virtual string TargetIKPose
        {
            get
            {
                if (inCover)
                {
                    if (shooterInput.cc.isCrouching)
                    {
                        return shooterInput.IsAiming ? side == Side.left ? (isAimingInCorner ? ikLeftCrouchingCornerAiming : aimingOverBarrier ? ikLeftCrouchingBarrierAiming : ikLeftCrouchingAiming) : (isAimingInCorner ? ikRightCrouchingCornerAiming : aimingOverBarrier ? ikRightCrouchingBarrierAiming : ikRightCrouchingAiming) :
                                                       side == Side.left ? ikLeftCrouching : ikRightCrouching;
                    }
                    else
                    {
                        return shooterInput.IsAiming ? side == Side.left ? (isAimingInCorner ? ikLeftStandingCornerAiming : ikLeftStandingAiming) : (isAimingInCorner ? ikRightStandingCornerAiming : ikRightStandingAiming) :
                                                      side == Side.left ? ikLeftStanding : ikRightStanding;
                    }
                }
                else return string.Empty;
            }
        }

        /// <summary>
        /// Get near cover point inside a the <see cref="coverPoints"/> list
        /// </summary>
        protected virtual void ControlNearCoverPoint()
        {
            if (goingToCornerPoint || (currentCornerWeight > 0.1f))
            {
                return;
            }

            vCoverPoint _coverPoint = GetNearCoverPoint();
            if (_coverPoint)
            {
                currentCoverPoint = _coverPoint;
            }
            else if (currentCoverPoint && Vector3.Distance(transform.position, currentCoverPoint.boxCollider.ClosestPoint(transform.position)) > 0.5f && inCover)
            {
                if (shooterInput.cc.IsAnimatorTag("CoverLocomotion"))
                {
                    onExitCover.Invoke();
                }

                ExitCover();
            }
        }

        private vCoverPoint GetNearCoverPoint()
        {
            var dist = currentCoverPoint ? (currentCoverPoint.posePosition - transform.position).magnitude : 10f;
            vCoverPoint _coverPoint = currentCoverPoint;

            coverPoints = coverPoints.FindAll(c => (c.posePosition - transform.position).magnitude < 10);
            for (int i = 0; i < coverPoints.Count; i++)
            {

                var d = (transform.position - coverPoints[i].posePosition).magnitude;

                if (d < dist)
                {
                    _coverPoint = coverPoints[i];
                    dist = d + 0.1f;
                }
            }

            return _coverPoint;
        }

        /// <summary>
        /// Set all controller properties to make controller move on path 
        /// </summary>
        /// <param name="pathLenght">Lenght of the path used to decide if needs to sprint</param>
        protected virtual void PrepareToMoveOnPath(float pathLenght, bool crouching = false)
        {
            shooterInput.animator.SetLayerWeight(animatorCoverCornerLayer, 0);
            shooterInput.cc.lockSetMoveSpeed = false;
            shooterInput.SetLockUpdateMoveDirection(true);
            shooterInput.SetLockAllInput(true);
            shooterInput.SetWalkByDefault(false);
            shooterInput.CancelAiming();
            shooterInput.cc.isStrafing = false;
            shooterInput.lockCameraInput = true;
            shooterInput.lockUpdateMoveDirection = true;
            shooterInput.lockMoveInput = true;
            shooterInput.cc.isSprinting = pathLenght > 3f;
            shooterInput.cc.isCrouching = crouching;
            shooterInput.cc.lockRotation = false;
        }

        /// <summary>
        /// Check is Aiming over barrier
        /// </summary>
        protected virtual void CheckOverBarrier()
        {
            enableHipFireOverBarrier = Mathf.Abs(cameraAngle) < 90;
            if (enableHipFireOverBarrier)
            {
                shooterInput.shooterManager.hipfireShot = shooterInput.IsCrouching && !isInLeftCornerPositionRange && !isInRightCornerPositionRange && useHipFireOverBarrier;
                shooterInput.shooterManager.hipfireAimTime = hipFireTimer;
            }
            else
            {
                shooterInput.shooterManager.hipfireAimTime = lastHipFireTimer;
                shooterInput.shooterManager.hipfireShot = lastUseHipFire;
            }
            aimingOverBarrier = useHipFireOverBarrier && enableHipFireOverBarrier && !shooterInput.isAimingByInput && shooterInput.isAimingByHipFire;

            overBarrierWeight = Mathf.Lerp(overBarrierWeight, aimingOverBarrier ? 1 : 0f, Time.deltaTime * (aimingOverBarrier ? 50 : 5f));

            shooterInput.animator.SetLayerWeight(animatorCoverOverBarrierLayer, overBarrierWeight * (1 - currentCornerWeight));
        }

        private void UpdateCheckAimPoints(ref float startX, ref float endX, ref float startY, ref float endY)
        {
            if (enableHipFireOverBarrier && shooterInput.IsCrouching)
            {
                startX += checkAimOverBarrierOffsetStartX;
                startY += checkAimOverBarrierOffsetStartY;
                endX += checkAimOverBarrierOffsetEndX;
                endY += checkAimOverBarrierOffsetEndY;
            }
        }

        /// <summary>
        /// Disable all corner UI 
        /// </summary>
        protected virtual void ResetCoverCornerUI()
        {
            if (currentTimeToEnterCorner > 0)
            {
                currentTimeToEnterCorner = 0;
            }

            if (cornerRightUI && cornerRightUI.gameObject.activeSelf)
            {
                cornerRightUI.gameObject.SetActive(false);
            }

            if (cornerLeftUI && cornerLeftUI.gameObject.activeSelf)
            {
                cornerLeftUI.gameObject.SetActive(false);
            }

            if (cornerInputUI && cornerInputUI.gameObject.activeSelf)
            {
                cornerInputUI.SetActive(false);
            }

            isInCornerInputAction = false;
        }

        /// <summary>
        /// Target position while Aiming and in a Corner 
        /// </summary>
        protected virtual Vector3 CornerAimingPosition
        {
            get
            {
                float zOffset = PositionOffsetZ;
                var localX = XLocalPositionAimingInCorner;
                return helper.position + helper.forward * zOffset + (helper.right * (-localX) * (int)side);
            }
        }

        /// <summary>
        /// Max position X inside the <see cref="currentCoverPoint"/> collider;
        /// </summary>
        protected float XLocalClampedInCorner
        {
            get
            {
                float clampX = (currentCoverPoint.boxCollider.size.x * 0.5f) - shooterInput.cc.colliderRadiusDefault;

                return (float)System.Math.Round(clampX, 2);
            }
        }

        /// <summary>
        /// Max position X inside the <see cref="currentCoverPoint"/> collider while Aiming;
        /// </summary>
        protected float XLocalPositionAimingInCorner
        {
            get
            {
                var localX = XLocalClampedInCorner;
                localX += side == Side.right ? cornerRightAiming_OffsetPosition : cornerLeftAiming_OffsetPosition;

                return Mathf.Max(0, localX);
            }
        }

        /// <summary>
        /// Get UI height based on camera look direction
        /// </summary>
        public float CornerUIHeight
        {
            get
            {
                var dist = Vector3.Distance(transform.position, shooterInput.cameraMain.transform.position);
                var p = shooterInput.cameraMain.transform.position + shooterInput.cameraMain.transform.forward * dist;

                p = transform.InverseTransformPoint(p);
                p.x = 0;
                p.z = 0;

                return Mathf.Clamp(p.y, 0, shooterInput.cc._capsuleCollider.height);
            }
        }

        /// <summary>
        /// Target Local position Z to use to get distance from the cover cover;
        /// </summary>
        protected virtual float PositionOffsetZ => (inCover ? shooterInput.cc._capsuleCollider.radius : capsuleRadiusInCover) + positionOffsetZ;

        /// <summary>
        /// Calculate the lenght of the path
        /// </summary>
        /// <param name="movePath">target path</param>
        /// <returns></returns>
        protected virtual float PathLenght(List<Vector3> movePath)
        {
            float remainingDistance = 0;
            Vector3 lastP = transform.position;
            for (int i = 0; i < movePath.Count; i++)
            {
                var distance = Vector3.Distance(lastP, movePath[i]);
                remainingDistance += distance;
                if (debug)
                {
                    Debug.DrawLine(lastP, movePath[i], Color.green, 10);
                }

                lastP = movePath[i];
            }

            return remainingDistance;
        }

        /// <summary>
        /// Check Height of the Cover point
        /// </summary>
        /// <param name="point">Target Cover Point</param>
        /// <returns></returns>
        protected virtual bool CheckCoverHeight(vCoverPoint coverPoint, bool checkAimingHeight)
        {
            var entrance = !inCover;
            var aimingDirection = shooterInput.cameraMain.transform.forward;
            aimingDirection.y = 0;
            var position = entrance ? coverPoint.posePosition : helper.position;
            var direction = coverPoint.transform.forward;

            var rayOrigin = entrance ? (position + Vector3.up * crouchHeight) : position + Vector3.up * crouchHeight + direction * 0.5f;

            var ray = new Ray(rayOrigin, -direction.normalized);

            int checkHeightFailCount = 0;
            bool checkHeightCenter = Physics.SphereCast(ray, crouchRayRadius, out checkHeightHit, crouchRayDistance, obstaclesLayer, QueryTriggerInteraction.Ignore) && !CheckIgnoreHeight(checkHeightHit.collider);
            bool checkHeightRight = Physics.SphereCast(rayOrigin + coverPoint.transform.right * (shooterInput.cc.colliderRadiusDefault * 0.5f), crouchRayRadius, -direction.normalized, out checkHeightHit, crouchRayDistance, obstaclesLayer, QueryTriggerInteraction.Ignore) && !CheckIgnoreHeight(checkHeightHit.collider);
            bool checkHeightLeft = Physics.SphereCast(rayOrigin - coverPoint.transform.right * (shooterInput.cc.colliderRadiusDefault * 0.5f), crouchRayRadius, -direction.normalized, out checkHeightHit, crouchRayDistance, obstaclesLayer, QueryTriggerInteraction.Ignore) && !CheckIgnoreHeight(checkHeightHit.collider);

            if (!checkHeightCenter)
            {
                checkHeightFailCount++;
                if (debug) Debug.DrawRay(rayOrigin, -direction.normalized * crouchRayDistance, Color.red);
            }
            else if (debug) Debug.DrawRay(rayOrigin, -direction.normalized * crouchRayDistance, Color.green);
            if (!checkHeightRight)
            {
                checkHeightFailCount++;
                if (debug) Debug.DrawRay(rayOrigin + coverPoint.transform.right * (shooterInput.cc.colliderRadiusDefault * 0.5f), -direction.normalized * crouchRayDistance, Color.red);
            }
            else if (debug) Debug.DrawRay(rayOrigin + coverPoint.transform.right * (shooterInput.cc.colliderRadiusDefault * 0.5f), -direction.normalized * crouchRayDistance, Color.green);
            if (!checkHeightLeft)
            {
                checkHeightFailCount++;
                if (debug) Debug.DrawRay(rayOrigin - coverPoint.transform.right * (shooterInput.cc.colliderRadiusDefault * 0.5f), -direction.normalized * crouchRayDistance, Color.red);
            }
            else if (debug) Debug.DrawRay(rayOrigin - coverPoint.transform.right * (shooterInput.cc.colliderRadiusDefault * 0.5f), -direction.normalized * crouchRayDistance, Color.green);


            var crouchPose = checkHeightFailCount > 1;

            if (crouchPose && checkAimingHeight)
            {
                var center = transform.position;
                var offset = -helper.transform.right * ((int)side) * (shooterInput.cc.colliderRadiusDefault + crouchRayOffsetInCorner);

                ray = new Ray(center + Vector3.up * crouchHeightAiming + offset, aimingDirection.normalized);
                var right = Quaternion.AngleAxis(90, Vector3.up) * ray.direction;
                if (Physics.Linecast(center, ray.origin + right.normalized * crouchRayRadius * 2f, out checkHeightHit, obstaclesLayer))
                {
                    ray.origin = checkHeightHit.point - right.normalized * crouchRayRadius * 2f;
                }
                if (debug)
                {
                    Debug.DrawRay(ray.origin + right.normalized * crouchRayRadius, ray.direction * crouchRayDistance);
                }

                if (debug)
                {
                    Debug.DrawRay(ray.origin - right.normalized * crouchRayRadius, ray.direction * crouchRayDistance);
                }

                crouchPose = !Physics.SphereCast(ray, crouchRayRadius, crouchRayDistance, obstaclesLayer, QueryTriggerInteraction.Ignore);
            }
            if (!entrance)
            {
                if (!shooterInput.IsAiming && !crouchPose && shooterInput.cc.isCrouching)
                {
                    if (currentTimeToGetUp >= timeToStandUp)
                    {
                        currentTimeToGetUp = 0;
                    }
                    else
                    {
                        crouchPose = true;
                    }

                    currentTimeToGetUp += Time.deltaTime;
                }
                else
                {
                    currentTimeToGetUp = 0;
                }
            }
            return crouchPose;
        }

        /// <summary>
        /// Check if collider is included in Height ignore Tags or names
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        protected virtual bool CheckIgnoreHeight(Collider target)
        {
            return checkHeightIgnoreTags.Contains(target.gameObject.tag) || checkHeightIgnoreNames.Contains(target.gameObject.name);
        }
        /// <summary>
        /// Check if Cover Point is a Left Corner
        /// </summary>
        /// <param name="coverPoint">Target Cover Point</param>
        /// <returns></returns>
        protected virtual bool IsLeftCoverCorner(vCoverPoint coverPoint)
        {
            var isLeftCorner = (coverPoint.corner & vCoverPoint.Corner.Left) != 0;
            return isLeftCorner;
        }

        /// <summary>
        /// Check if Cover Point is a Right Corner
        /// </summary>
        /// <param name="coverPoint">Target Cover Point</param>
        /// <returns></returns>
        protected virtual bool IsRightCoverCorner(vCoverPoint coverPoint)
        {
            var isRightCorner = (coverPoint.corner & vCoverPoint.Corner.Right) != 0;
            return isRightCorner;
        }

        /// <summary>
        /// Check if Cover Point is Corner
        /// </summary>
        /// <param name="coverPoint">Target Cover Point</param>
        /// <returns></returns>
        protected virtual bool IsCoverCorner(vCoverPoint coverPoint)
        {
            return IsRightCoverCorner(coverPoint) || IsLeftCoverCorner(coverPoint);
        }

        /// <summary>
        /// Check if Directions is inside angle
        /// </summary>
        /// <param name="directionA">Direction A</param>
        /// <param name="directionB">Direction B</param>
        /// <param name="value">Angle to Check (Only positive Values)</param>
        /// <returns></returns>
        protected virtual bool CheckAngleOfDirections(Vector3 directionA, Vector3 directionB, float value)
        {
            return Vector3.Angle(directionA, directionB) <= value;
        }

        /// <summary>
        /// Check if is Looking To Corner, including angle and input direction
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsLookingToCorner()
        {
            var dirX = System.Math.Round(helper.InverseTransformDirection(inputDirection).x * -1, 1);
            var forwardAngle = Vector3.Angle(-currentCoverPoint.transform.right * (int)side, transform.forward);


            var inputToCorner = forwardAngle < minAngleToStartMoveToCorner && ((isInLeftCornerPositionRange && dirX < -0.2f) || (isInRightCornerPositionRange && dirX > 0.2f)) && cameraAngle * (int)side < angleToEnterCorner;
            return inputToCorner;
        }

        /// <summary>
        /// Conditions to enter in Cover State
        /// </summary>
        /// <returns></returns>
        protected virtual bool EnterCoverConditions()
        {
            return !shooterInput.cc.animator.IsInTransition(shooterInput.cc.baseLayer) && !shooterInput.cc.customAction && !shooterInput.cc.isJumping && !shooterInput.isAimingByInput && !isRolling;
        }

        /// <summary>
        /// Conditions to Force Exit Cover
        /// </summary>
        /// <returns></returns>
        protected virtual bool ExitByActions()
        {
            return !shooterInput.animator.IsInTransition(0) && ((shooterInput.cc.customAction && !goingToCoverPoint) || isRolling || shooterInput.cc.isJumping || shooterInput.cc.ragdolled || shooterInput.cc.isDead);
        }
        #endregion

        #endregion
    }
}