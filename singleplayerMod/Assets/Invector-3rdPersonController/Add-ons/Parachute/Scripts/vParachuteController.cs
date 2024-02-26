using Invector;
using Invector.vCharacterController;
using System.Collections;
using UnityEngine;

[vClassHeader("Parachute Controller")]
public class vParachuteController : vMonoBehaviour
{
    [vEditorToolbar("Input/Movement")]
    [vReadOnly] public bool canMove;
    [vReadOnly] public bool inForwardCollidion;
    [vReadOnly] public bool usingParachute;
    [vReadOnly, SerializeField] protected float currentDrag;
    [SerializeField] protected bool _canUseParachute = true;
    public Rigidbody parachutePivot;
    public GameObject parachuteTilt;
    public GenericInput openCloseParachute = new GenericInput("Space", "A", "A");
    [Range(2f, 100f)]
    public float minHeightToOpenParachute = 2;
    [Range(0.5f, 5f)]
    public float minTimeToReOpen = 0.5f;

    [Header("Vertical Movement")]
    [Range(1, 10)]
    public float dragPitchBack = 9f;
    [Range(1, 10)]
    public float dragPitchForward = 5f;

    [Header("Horizontal Movement")]
    public float acceleration = 2f;
    public float speedPitchBack = 20;
    public float speedPitchForward = 10;

    [vHelpBox("Roll(z) uses Horizontal Input, Pitch uses Vertical Input (Back (-1) and Forward (1))")]
    [Header("Roll(Z)/Pitch(X) Rotation")]
    public float rollSpeed = 2f;
    public float rollAngle = 35;
    public float pitchSpeed = 1f;
    [vMinMax(minLimit = -45, maxLimit = 80)]
    public Vector2 pitchAngle = new Vector2(-20, 60);

    [Header("Yaw(Y) Rotation")]
    public float rotationYSpeed = 60;
    [vEditorToolbar("Camera")]
    [Header("Camera")]
    public string cameraState = "Parachute";
    public Transform cameraTarget;
    public bool autoAlignCamera = true;
    public float autoAlignSpeed = 0.1f;
    [vEditorToolbar("Animator")]
    [Tooltip("Used to check when is in parachute state")]
    public string parachuteStateName = "Parachute";
    [Tooltip("Used to play the first animation of the parachute state")]
    public string anim_OpenParachute = "OpenParachute";
    [Tooltip("Used to play the first animation of the parachute state")]
    public string anim_ExitParachute = "ExitParachute";
    [vEditorToolbar("Events")]
    public UnityEngine.Events.UnityEvent onCanUseParachuteEnable, onCanUseParachuteDisable;
    public UnityEngine.Events.UnityEvent onStartOpen, onOpen, onClose;

    protected Quaternion forwardBackTiltRotation, sidesTiltRotation;
    protected Vector3 startPivotPosition, startPivotEuler, horizontalForce;
    protected float currentHorizontalSpeed;
    protected float originalFreeSpeedRotation;
    protected int actionStateToExit;
    protected vThirdPersonInput tpInput;
    protected float autoAlignWeight;
    protected float timerToReOpen;
    protected int _startAnimationHash;

    const int actState_default = 1;
    const int actState_exitOnGround = 1;
    const int actState_exitImmediate = 0;

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        tpInput = GetComponentInParent<vThirdPersonInput>();
        if (tpInput)
        {
            tpInput.onUpdate -= UpdateParachute;
            tpInput.onUpdate += UpdateParachute;

            tpInput.onFixedUpdate -= FixedUpdateParachute;
            tpInput.onFixedUpdate += FixedUpdateParachute;

            originalFreeSpeedRotation = tpInput.cc.freeSpeed.rotationSpeed;

            startPivotPosition = parachutePivot.transform.localPosition;
            startPivotEuler = parachutePivot.transform.localEulerAngles;

            parachuteTilt.transform.localPosition = Vector3.zero;
            parachuteTilt.transform.localEulerAngles = Vector3.zero;
            parachuteTilt.SetActive(false);
            parachutePivot.centerOfMass = parachutePivot.transform.InverseTransformPoint(tpInput.transform.position);
        }
        _startAnimationHash = Animator.StringToHash(anim_OpenParachute);
        if (canUseParachute)
        {
            onCanUseParachuteEnable.Invoke();
        }
        else
        {
            onCanUseParachuteDisable.Invoke();
        }
    }

    /// <summary>
    /// Open Parachute if <seealso cref="openParachuteConditions"/> is TRUE
    /// </summary>
    public virtual void OpenParachute()
    {
        if (openParachuteConditions)
        {
            StartCoroutine(OpenParachuteRoutine());
        }
    }

    /// <summary>
    /// Check if Can open parachute
    /// </summary>
    public bool openParachuteConditions
    {
        get
        {
            return canUseParachute && !usingParachute && !tpInput.cc.ragdolled && !tpInput.cc.isRolling && !tpInput.cc.customAction && tpInput.cc.groundDistance > minHeightToOpenParachute && timerToReOpen < Time.time;
        }
    }

    /// <summary>
    /// Exit Parachute State without <seealso cref="anim_ExitParachute"/>
    /// </summary>
    public virtual void ExitParachuteImmedite()
    {
        if (!usingParachute)
        {
            return;
        }

        actionStateToExit = actState_exitImmediate;
        CloseParachute();
    }

    /// <summary>
    /// Exit Parachute State playing <seealso cref="anim_ExitParachute"/>
    /// </summary>
    public virtual void ExitParachute()
    {
        if (!usingParachute)
        {
            return;
        }

        actionStateToExit = actState_exitOnGround;
        CloseParachute();
    }

    /// <summary>
    /// Enable or disable parachute usage. If the parachute is already open this variable will only take effect after closing it
    /// </summary>
    public bool canUseParachute
    {
        get
        {
            return _canUseParachute;
        }
        set
        {
            if (_canUseParachute != value)
            {
                if (value)
                {
                    onCanUseParachuteEnable.Invoke();
                }
                else
                {
                    onCanUseParachuteDisable.Invoke();
                }

                _canUseParachute = value;
            }

        }
    }

    protected virtual void UpdateParachute()
    {
        if (openCloseParachute.GetButtonDown())
        {
            if (openParachuteConditions)
            {
                OpenParachute();
            }
            else if (usingParachute)
            {
                ExitParachuteImmedite();
            }
        }
        if (usingParachute && tpInput.cc.ragdolled)
        {
            ExitParachuteImmedite();
        }
    }

    protected virtual void FixedUpdateParachute()
    {
        if (canMove)
        {
            var inputX = tpInput.horizontalInput.GetAxis();
            var inputY = tpInput.verticalInput.GetAxis();
            var input = new Vector3(inputX, 0, inputY);
            var rotationInput = input;
            var inputRelativeToCamera = tpInput.cameraMain.transform.TransformDirection(input);

            tpInput.animator.SetFloat("InputHorizontal", input.x, input.magnitude > 0.1f ? 1f : 2f, Time.deltaTime);
            tpInput.animator.SetFloat("InputVertical", input.z, input.magnitude > 0.1f ? 1f : 2f, Time.deltaTime);
            var forward = parachutePivot.transform.forward;
            forward.y = 0;
            currentHorizontalSpeed = Mathf.Lerp(currentHorizontalSpeed, Mathf.Lerp(speedPitchBack, speedPitchForward, input.z), acceleration * Time.deltaTime);
            horizontalForce = (parachutePivot.transform.forward).normalized * currentHorizontalSpeed;
            currentDrag = Mathf.Lerp(dragPitchBack, dragPitchForward, input.z) * (input.z < 0f ? Mathf.Lerp(1, -parachutePivot.velocity.y, -input.z) : 1);
            parachutePivot.AddForceAtPosition(Vector3.up * currentDrag, transform.position);

            tpInput.cc.heightReached = tpInput.transform.position.y;
            tpInput.cc.verticalVelocity = parachutePivot.velocity.y;
            float angleSide = rollAngle * -rotationInput.x;
            float angleForward = rotationInput.z > 0 ? pitchAngle.y * rotationInput.z : pitchAngle.x * (-rotationInput.z);

            forwardBackTiltRotation = Quaternion.Lerp(forwardBackTiltRotation, Quaternion.AngleAxis(angleForward, Vector3.right), pitchSpeed * Time.deltaTime);
            sidesTiltRotation = Quaternion.Lerp(sidesTiltRotation, Quaternion.AngleAxis(angleSide, Vector3.forward), rollSpeed * Time.deltaTime);
            parachuteTilt.transform.localRotation = sidesTiltRotation * forwardBackTiltRotation;

            parachutePivot.transform.Rotate(Vector3.up, rotationInput.x * rotationYSpeed * Time.deltaTime);

            if (autoAlignCamera)
            {
                if (input.magnitude > 0.5 && Mathf.Abs(tpInput.rotateCameraXInput.GetAxis()) < .5f && Mathf.Abs(tpInput.rotateCameraYInput.GetAxis()) < .5f)
                {
                    if (autoAlignWeight < 1)
                    {
                        autoAlignWeight += Time.deltaTime * autoAlignSpeed;
                    }

                    tpInput.tpCamera.mouseY = Mathf.LerpAngle(tpInput.tpCamera.mouseY, parachuteTilt.transform.eulerAngles.NormalizeAngle().x, autoAlignWeight);
                    tpInput.tpCamera.mouseX = Mathf.LerpAngle(tpInput.tpCamera.mouseX, parachutePivot.transform.eulerAngles.NormalizeAngle().y, autoAlignWeight);
                }
                else
                {
                    autoAlignWeight = 0;
                }
            }

            if (!inForwardCollidion)
            {
                var vel = parachutePivot.velocity;
                vel.Set(horizontalForce.x, vel.y, horizontalForce.z);
                parachutePivot.velocity = vel;
            }
            else
            {
                currentHorizontalSpeed = 0;
            }
        }
    }

    protected virtual IEnumerator OpenParachuteRoutine()
    {
        onStartOpen.Invoke();

        usingParachute = true;
        parachuteTilt.transform.localPosition = Vector3.zero;
        parachuteTilt.transform.localEulerAngles = Vector3.zero;
        forwardBackTiltRotation = Quaternion.identity;
        sidesTiltRotation = Quaternion.identity;
        tpInput.cc.heightReached = transform.position.y;
        tpInput.cc.disableCheckGround = true;
        tpInput.SetLockAllInput(true);
        tpInput.cc.isCrouching = false;
        tpInput.cc.lockMovement = true;
        tpInput.cc.lockRotation = true;
        tpInput.animator.SetInteger("ActionState", actState_default);
        tpInput.animator.CrossFadeInFixedTime(_startAnimationHash, .25f);
        var vel = tpInput.cc._rigidbody.velocity;
        while (!tpInput.cc.baseLayerInfo.IsName(parachuteStateName))
        {
            yield return null;
        }

        onOpen.Invoke();
        vel.y = 0;
        horizontalForce = vel;
        currentDrag = 0;
        currentHorizontalSpeed = 0;
        tpInput.cc._rigidbody.isKinematic = true;
        parachutePivot.transform.parent = null;
        parachuteTilt.SetActive(true);
        tpInput.transform.parent = parachuteTilt.transform;
        tpInput.transform.localEulerAngles = Vector3.zero;
        tpInput.transform.localPosition = new Vector3(0, tpInput.transform.localPosition.y, 0);
        parachutePivot.isKinematic = false;

        tpInput.cc.disableAnimations = true;
        tpInput.cc.lockSetMoveSpeed = true;
        tpInput.cc.moveSpeed = speedPitchBack;
        tpInput.cc.freeSpeed.rotationSpeed = rotationYSpeed;
        tpInput.cc._rigidbody.useGravity = false;
        tpInput.cc._capsuleCollider.isTrigger = false;
        canMove = true;

        yield return new WaitForEndOfFrame();
        if (cameraTarget)
        {
            tpInput.tpCamera.SetTarget(cameraTarget);
        }

        if (!string.IsNullOrEmpty(cameraState))
        {
            tpInput.ChangeCameraState(cameraState);
        }
    }

    protected virtual void CloseParachute()
    {
        if (!usingParachute)
        {
            return;
        }

        timerToReOpen = Time.time + minTimeToReOpen;
        usingParachute = false;
        canMove = false;
        tpInput.transform.parent = null;
        parachuteTilt.SetActive(false);

        parachutePivot.isKinematic = true;
        parachutePivot.transform.parent = tpInput.transform;
        parachutePivot.transform.localPosition = startPivotPosition;
        parachutePivot.transform.localEulerAngles = startPivotEuler;
        parachuteTilt.transform.localPosition = Vector3.zero;
        parachuteTilt.transform.localEulerAngles = Vector3.zero;
        var forward = tpInput.transform.forward;
        forward.y = 0;

        tpInput.transform.rotation = Quaternion.LookRotation(forward);

        tpInput.cc.disableAnimations = false;

        tpInput.cc.lockSetMoveSpeed = false;
        if (actionStateToExit == actState_exitOnGround)
        {
            tpInput.animator.CrossFadeInFixedTime(anim_ExitParachute, 0.1f);
        }

        tpInput.animator.SetInteger("ActionState", 0);
        tpInput.cc._rigidbody.drag = 0;

        tpInput.cc.disableCheckGround = false;
        if (actionStateToExit != actState_exitOnGround)
        {
            tpInput.cc.isGrounded = false;
            tpInput.cc.animator.SetBool("IsGrounded", false);
        }

        if (!tpInput.cc.ragdolled)
        {
            tpInput.cc.verticalVelocity = 0;
            tpInput.cc._rigidbody.isKinematic = false;
            tpInput.cc._rigidbody.useGravity = true;
        }

        tpInput.cc.freeSpeed.rotationSpeed = originalFreeSpeedRotation;
        tpInput.cc.lockMovement = false;
        tpInput.cc.lockRotation = false;
        tpInput.ResetCameraState();
        tpInput.SetLockAllInput(false);
        tpInput.tpCamera.SetTarget(tpInput.transform);
        onClose.Invoke();
    }

    protected void OnCollisionEnter(Collision collision)
    {
        CheckCollision(collision);
    }

    protected void OnCollisionStay(Collision collision)
    {
        CheckCollision(collision);
    }

    protected virtual void CheckCollision(Collision collision)
    {
        if (!usingParachute)
        {
            return;
        }

        if (Vector3.Angle(Vector3.up, collision.contacts[0].normal) < tpInput.cc.slopeLimit)
        {
            ExitParachute();
        }
        if (Vector3.Angle(-transform.forward, collision.contacts[0].normal) < 45)
        {
            inForwardCollidion = true;
        }
        else
        {
            inForwardCollidion = false;
        }
    }

    protected void OnCollisionExit(Collision collision)
    {
        inForwardCollidion = false;
    }
}
