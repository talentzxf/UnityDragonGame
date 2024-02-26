using Invector;
using Invector.IK;
using Invector.vCharacterController;
using System.Collections;
using UnityEngine;

[vClassHeader("Push Action Controller")]
public class vPushActionController : vMonoBehaviour, vIAnimatorMoveReceiver
{
    [vEditorToolbar("Settings")]
    public GenericInput startPushPullInput = new GenericInput("E", "A", "A");
    public Vector2 movementSpeed = Vector2.one;

    public float groundMaxDistance = 0.5f;
    public float groundMinDistance = 0.1f;

    [Range(0, 1)]
    public float minMovementMagnitude = 0.1f;
    public float animAcceleration = 8f;
    public bool useLeftHandIK = true;
    public bool useRightHandIK = true;
    public float strength = 1;
    public AnimationCurve enterCurve;
    public LayerMask pushpullLayer;
    public float breakActionDistance = 0.25f;
    public float enterPositionSpeed = 0.1f;
    public float handForwardOffset;
    public float handSideOffset;
    public float handSpaceDistanceX = 0.25f;
    public float distanceBetweenObject = 0.5f;
    [vEditorToolbar("Events")]
    public UnityEngine.Events.UnityEvent onStart, onFinish;
    public UnityEngine.Events.UnityEvent onFindObject, onLostObject;


    protected RaycastHit hit;
    protected Transform cameraTransform;
    protected Vector3 startDirection;
    protected Vector3 startLocalPosition;
    protected vIKSolver leftHand, rightHand;
    protected float weight;
    protected float startDistance;

    protected bool canUpdateIK;
    protected Vector3 inputDirection;
    protected Vector3 lastBodyPosition;
    protected Vector3 movementDirection;
    protected vThirdPersonInput tpInput;
    [vEditorToolbar("Debug")]
    [vReadOnly(false), SerializeField] protected vPushObjectPoint pushPoint;
    [vReadOnly(false), SerializeField] protected bool isStarted;
    [vReadOnly(false), SerializeField] protected bool isStoping;
    [vReadOnly(false), SerializeField] protected bool isPushingPulling;
    [vReadOnly(false), SerializeField] protected bool isMoving;
    [vReadOnly(false), SerializeField] protected bool isCollidingLeft;
    [vReadOnly(false), SerializeField] protected bool isCollidingRight;
    [vReadOnly(false), SerializeField] protected bool isCollidingBack;
    [vReadOnly(false), SerializeField] protected float inputVertical;
    [vReadOnly(false), SerializeField] protected float inputHorizontal;
    [vReadOnly(false), SerializeField] protected float inputWeight;


    protected virtual void Start()
    {
        tpInput = GetComponent<vThirdPersonInput>();
        cameraTransform = Camera.main.transform;
        leftHand = new vIKSolver(tpInput.animator, AvatarIKGoal.LeftHand);
        rightHand = new vIKSolver(tpInput.animator, AvatarIKGoal.RightHand);
    }

    protected virtual void Update()
    {
        UpdateInput();
    }

    protected virtual void LateUpdate()
    {
        UpdateIK();
    }

    protected virtual void FixedUpdate()
    {
        canUpdateIK = true;
    }

    protected virtual void UpdateIK()
    {
        if (tpInput.enabled || !isStarted || !pushPoint || !canUpdateIK) return;

        var leftHandP = leftHand.endBone.position;
        var rightHandP = rightHand.endBone.position;

        leftHandP = transform.InverseTransformPoint(leftHandP);
        rightHandP = transform.InverseTransformPoint(rightHandP);

        leftHandP.z = 0;
        rightHandP.z = 0;

        leftHandP.x = -handSpaceDistanceX;
        rightHandP.x = handSpaceDistanceX;

        leftHandP.x += handSideOffset;
        rightHandP.x += handSideOffset;

        if (!pushPoint.handsAdjustReference)
        {
            leftHandP = transform.TransformPoint(leftHandP);
            rightHandP = transform.TransformPoint(rightHandP);
            leftHandP = pushPoint.boxCollider.ClosestPoint(leftHandP);
            rightHandP = pushPoint.boxCollider.ClosestPoint(rightHandP);
        }
        else
        {
            var referenceAdjust = transform.InverseTransformPoint(pushPoint.handsAdjustReference.position);
            leftHandP.z = referenceAdjust.z;
            rightHandP.z = referenceAdjust.z;
            leftHandP.y = referenceAdjust.y;
            rightHandP.y = referenceAdjust.y;
            leftHandP = transform.TransformPoint(leftHandP);
            rightHandP = transform.TransformPoint(rightHandP);
        }

        leftHandP += pushPoint.transform.forward * handForwardOffset;
        rightHandP += pushPoint.transform.forward * handForwardOffset;
        leftHand.SetIKWeight(useLeftHandIK ? weight : 0);
        leftHand.SetIKPosition(leftHandP);
        rightHand.SetIKWeight(useRightHandIK ? weight : 0);
        rightHand.SetIKPosition(rightHandP);
        canUpdateIK = false;
    }

    protected virtual void UpdateInput()
    {
        EnterExitInput();
        MoveInput();
    }

    protected void EnterExitInput()
    {
        if (tpInput.enabled || !isStarted || !pushPoint)
        {
            Ray ray = new Ray(transform.position + Vector3.up * 0.5f, transform.forward);

            if (Physics.Raycast(ray, out hit, 1.5f, pushpullLayer))
            {
                var _object = hit.collider.gameObject.GetComponent<vPushObjectPoint>();
                if (_object && pushPoint != _object && _object.canUse)
                {
                    pushPoint = _object;
                    onFindObject.Invoke();
                }
                else if (_object == null && pushPoint)
                {
                    pushPoint = null;
                    onLostObject.Invoke();
                }
            }
            else if (pushPoint)
            {
                pushPoint = null;
                onLostObject.Invoke();
            }

            if (pushPoint && pushPoint.canUse && startPushPullInput.GetButtonDown())
            {
                StartCoroutine(StartPushAndPull());

            }

        }
        else if (isPushingPulling && startPushPullInput.GetButtonDown())
        {
            StartCoroutine(StopPushAndPull());
        }
    }

    protected virtual void MoveInput()
    {
        if (tpInput.enabled || !isPushingPulling || !pushPoint || isStoping)
        {
            return;
        }
        tpInput.CameraInput();

        inputHorizontal = tpInput.horizontalInput.GetAxis();
        inputVertical = tpInput.verticalInput.GetAxis();
        if (Mathf.Abs(inputHorizontal) > 0.5f)
        {
            inputVertical = 0;
        }
        else if (Mathf.Abs(inputVertical) > 0.5f)
        {
            inputHorizontal = 0;
        }

        if (Mathf.Abs(inputHorizontal) < 0.8f)
        {
            inputHorizontal = 0;
        }

        if (Mathf.Abs(inputVertical) < 0.8f)
        {
            inputVertical = 0;
        }

        Vector3 cameraRight = cameraTransform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();
        Vector3 cameraForward = Quaternion.AngleAxis(-90, Vector3.up) * cameraRight;

        inputDirection = cameraForward * inputVertical + cameraRight * inputHorizontal;
        inputDirection = pushPoint.transform.InverseTransformDirection(inputDirection);

        if (inputDirection.z > 0 && !pushPoint.canPushForward)
        {
            inputDirection.z = 0;
        }
        else if (inputDirection.z < 0 && (!pushPoint.canPushBack || isCollidingBack))
        {
            inputDirection.z = 0;
        }

        if (inputDirection.x > 0 && (!pushPoint.canPushRight || isCollidingRight))
        {
            inputDirection.x = 0;
        }
        else if (inputDirection.x < 0 && (!pushPoint.canPushLeft || isCollidingLeft))
        {
            inputDirection.x = 0;
        }

        inputDirection.y = 0;

        if (inputDirection.magnitude > 0.1f)
        {
            inputWeight = Mathf.Lerp(inputWeight, 1, Time.deltaTime * animAcceleration);
        }
        else
        {
            inputWeight = Mathf.Lerp(inputWeight, 0, Time.deltaTime * animAcceleration);
        }
    }

    protected virtual void MoveObject()
    {
        var strengthFactor = Mathf.Clamp(strength / pushPoint.targetBody.mass, 0, 1);
        var direction = ClampDirection(pushPoint.transform.TransformDirection(inputDirection));
        movementDirection = direction;

        Vector3 targetPosition = pushPoint.targetBody.position + direction * strengthFactor * vTime.fixedDeltaTime;
        Vector3 targetDirection = (targetPosition - pushPoint.targetBody.position) / vTime.fixedDeltaTime;

        targetDirection.y = pushPoint.targetBody.velocity.y;
        pushPoint.targetBody.velocity = targetDirection * inputWeight;
        bool _isMoving = (pushPoint.targetBodyPosition - lastBodyPosition).magnitude > 0.001f && inputWeight > 0f;

        if (_isMoving != isMoving)
        {
            isMoving = _isMoving;

            if (isMoving)
            {
                pushPoint.pushableObject.onStartMove.Invoke();
            }
            else
            {
                pushPoint.pushableObject.onMovimentSpeedChanged.Invoke(0);
                pushPoint.pushableObject.onStopMove.Invoke();
            }
        }
        if (isMoving)
        {
            pushPoint.pushableObject.onMovimentSpeedChanged.Invoke(Mathf.Clamp(pushPoint.targetBody.velocity.magnitude, 0, 1f));
        }
    }

    protected virtual Vector3 ClampDirection(Vector3 direction)
    {
        if (direction.magnitude < 0.01f) return Vector3.zero;
        var angle = direction.AngleFormOtherDirection(pushPoint.transform.forward).y;
        if (Mathf.Abs(angle) <= 45) direction = pushPoint.transform.forward;
        else if (Mathf.Abs(angle) > 45 && Mathf.Abs(angle) < 135)
        {
            direction = pushPoint.transform.right * (angle >= 0 ? -1 : 1);
        }
        else
        {
            direction = -pushPoint.transform.forward;
        }

        return direction;
    }

    protected virtual void MoveCharacter()
    {
        var movementMagnitude = Mathf.Clamp(pushPoint.targetBody.velocity.magnitude, minMovementMagnitude, 1f) * inputWeight;
        movementDirection = transform.InverseTransformDirection(movementDirection);
        tpInput.cc.animator.SetFloat(vAnimatorParameters.InputVertical, isMoving ? movementDirection.z * movementMagnitude : 0f, 0.2f, Time.deltaTime);
        tpInput.cc.animator.SetFloat(vAnimatorParameters.InputHorizontal, isMoving ? movementDirection.x * movementMagnitude : 0, 0.2f, Time.deltaTime);
        tpInput.cc.animator.SetFloat(vAnimatorParameters.InputMagnitude, inputDirection.magnitude > 0.1f ? Mathf.Clamp(movementMagnitude, 0, 1f) : 1, 0.2f, vTime.fixedDeltaTime);
        tpInput.cc._rigidbody.velocity = pushPoint.targetBody.velocity;
        Vector3 realPosition = GetCharacterPosition();
        transform.position = Vector3.Lerp(transform.position, realPosition, inputDirection.magnitude);
        transform.rotation = Quaternion.LookRotation(startDirection, Vector3.up);
    }

    protected virtual Vector3 GetCharacterPosition()
    {
        var localPosition = pushPoint.transform.InverseTransformPoint(pushPoint.transform.position - transform.transform.forward * distanceBetweenObject);
        localPosition.x = startLocalPosition.x;
        var realPosition = pushPoint.transform.TransformPoint(localPosition);
        realPosition.y = transform.position.y;
        return realPosition;
    }

    protected virtual void OnCollisionStay(Collision collision)
    {
        bool _isCollidingLeft = false;
        bool _isCollidingRight = false;
        bool _isCollidingBack = false;

        for (int i = 0; i < collision.contactCount; i++)
        {
            Debug.DrawRay(transform.position + Vector3.up, collision.contacts[i].normal);

            var normal = -collision.contacts[i].normal;
            if (Vector3.Angle(transform.right, normal) < 20)
            {
                _isCollidingRight = true;
            }
            if (Vector3.Angle(-transform.right, normal) < 20)
            {
                _isCollidingLeft = true;
            }
            if (Vector3.Angle(-transform.forward, normal) < 20)
            {
                _isCollidingBack = true;
            }
        }

        isCollidingRight = _isCollidingRight;
        isCollidingLeft = _isCollidingLeft;
        isCollidingBack = _isCollidingBack;
    }

    protected virtual IEnumerator StartPushAndPull()
    {
        onStart.Invoke();
        tpInput.cc._rigidbody.velocity = Vector3.zero;
        tpInput.cc.enabled = false;
        tpInput.enabled = false;
        tpInput.cc.isJumping = false;
        tpInput.cc.animator.SetFloat(vAnimatorParameters.InputHorizontal, 0);
        tpInput.cc.animator.SetFloat(vAnimatorParameters.InputVertical, 0);
        tpInput.cc.animator.SetFloat(vAnimatorParameters.InputMagnitude, 0);
        startDirection = pushPoint.transform.forward;
        startDirection.y = 0;
        startDirection.Normalize();
        isStarted = true;
        var positionA = transform.position;
        Vector3 positionB = GetCharacterTargetPoisiton();
        positionB.y = transform.position.y;
        var rotationA = transform.rotation;
        var rotationB = Quaternion.LookRotation(startDirection, Vector3.up);
        tpInput.animator.PlayInFixedTime("StartPushAndPull");
        float _weight = 0;
        while (_weight < 1)
        {
            _weight += enterPositionSpeed * Time.deltaTime;
            weight = enterCurve.Evaluate(_weight);

            transform.position = Vector3.Lerp(positionA, positionB, weight);
            transform.rotation = Quaternion.Lerp(rotationA, rotationB, weight);
            yield return null;
        }
        lastBodyPosition = pushPoint.targetBodyPosition;
        startLocalPosition = pushPoint.transform.InverseTransformPoint(positionB);
        startDistance = Vector3.Distance(transform.position, pushPoint.transform.position);
        weight = 1;
        transform.position = Vector3.Lerp(positionA, positionB, 1);
        transform.rotation = Quaternion.Lerp(rotationA, rotationB, 1);
        isPushingPulling = true;
        pushPoint.pushableObject.StartPushAndPull();
    }

    protected virtual Vector3 GetCharacterTargetPoisiton()
    {
        var positionB = pushPoint.transform.position - transform.transform.forward * distanceBetweenObject;
        if (!pushPoint.pushPointInCenter)
        {
            var p = pushPoint.transform.InverseTransformPoint(positionB);
            var p2 = pushPoint.transform.InverseTransformPoint(transform.position);
            p.x = p2.x;
            positionB = pushPoint.transform.TransformPoint(p);
        }

        return positionB;
    }

    protected virtual IEnumerator StopPushAndPull(bool playAnimation = true)
    {
        if (isMoving)
        {
            pushPoint.pushableObject.onStopMove.Invoke();
            isMoving = false;
        }
        isStoping = true;
        isStarted = false;
        pushPoint.pushableObject.FinishPushAndPull();
        pushPoint.targetBody.velocity = Vector3.zero;
        pushPoint = null;
        onLostObject.Invoke();
        tpInput.cc.animator.SetFloat(vAnimatorParameters.InputHorizontal, 0);
        tpInput.cc.animator.SetFloat(vAnimatorParameters.InputVertical, 0);
        tpInput.cc.animator.SetFloat(vAnimatorParameters.InputMagnitude, 0);
        isPushingPulling = false;
        if (playAnimation)
        {
            tpInput.animator.PlayInFixedTime("StopPushAndPull");
        }

        yield return new WaitForEndOfFrame();

        weight = 0;
        tpInput.cc.ResetInputAnimatorParameters();
        tpInput.cc.inputSmooth = Vector3.zero;
        tpInput.cc.input = Vector3.zero;
        tpInput.cc.inputMagnitude = 0;
        tpInput.cc.StopCharacter();
        tpInput.cc._rigidbody.velocity = Vector3.zero;
        tpInput.cc.moveDirection = Vector3.zero;
        tpInput.cc.animator.SetFloat(vAnimatorParameters.InputHorizontal, 0);
        tpInput.cc.animator.SetFloat(vAnimatorParameters.InputVertical, 0);
        tpInput.cc.animator.SetFloat(vAnimatorParameters.InputMagnitude, 0);
        tpInput.cc.enabled = true;
        tpInput.enabled = true;
        onFinish.Invoke();
        isStoping = false;
    }

    protected virtual void CheckBreakActionConditions()
    {
        // radius of the SphereCast
        float radius = tpInput.cc._capsuleCollider.radius * 0.9f;
        var dist = 10f;
        // ray for RayCast
        Ray ray2 = new Ray(transform.position + new Vector3(0, tpInput.cc.colliderHeight / 2, 0), Vector3.down);
        // raycast for check the ground distance
        if (Physics.Raycast(ray2, out tpInput.cc.groundHit, (tpInput.cc.colliderHeight / 2) + dist, tpInput.cc.groundLayer) && !tpInput.cc.groundHit.collider.isTrigger)
        {
            dist = transform.position.y - tpInput.cc.groundHit.point.y;
        }
        // sphere cast around the base of the capsule to check the ground distance
        if (dist >= groundMinDistance)
        {
            Vector3 pos = transform.position + Vector3.up * (tpInput.cc._capsuleCollider.radius);
            Ray ray = new Ray(pos, -Vector3.up);
            if (Physics.SphereCast(ray, radius, out tpInput.cc.groundHit, tpInput.cc._capsuleCollider.radius + groundMaxDistance, tpInput.cc.groundLayer) && !tpInput.cc.groundHit.collider.isTrigger)
            {
                Physics.Linecast(tpInput.cc.groundHit.point + (Vector3.up * 0.1f), tpInput.cc.groundHit.point + Vector3.down * 0.15f, out tpInput.cc.groundHit, tpInput.cc.groundLayer);
                float newDist = transform.position.y - tpInput.cc.groundHit.point.y;
                if (dist > newDist)
                {
                    dist = newDist;
                }
            }
        }

        if (dist > groundMaxDistance || Vector3.Distance(transform.position, pushPoint.transform.TransformPoint(startLocalPosition)) > (breakActionDistance))
        {
            bool falling = dist > groundMaxDistance;
            if (falling)
            {
                tpInput.cc.isGrounded = false;
                tpInput.cc.animator.SetBool(vAnimatorParameters.IsGrounded, false);
                tpInput.cc.animator.PlayInFixedTime("Falling");
            }
            StartCoroutine(StopPushAndPull(!falling));
        }
    }

    public virtual void OnAnimatorMoveEvent()
    {

        if (tpInput.enabled || !isPushingPulling || !pushPoint || isStoping)
        {
            return;
        }

        MoveObject();
        MoveCharacter();

        lastBodyPosition = pushPoint.targetBodyPosition;

        CheckBreakActionConditions();
    }

    public virtual bool IsPushingPulling => isPushingPulling;

    public virtual bool IsMoving => isMoving;

    public virtual bool IsCollidingLeft => isMoving;

    public virtual bool IsCollidingRight => isMoving;

    public virtual bool IsCollidingBack => isMoving;

    public virtual vPushObjectPoint PushPoint => pushPoint;
}
