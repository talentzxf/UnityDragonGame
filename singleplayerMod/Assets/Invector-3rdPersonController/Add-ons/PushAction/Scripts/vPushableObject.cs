using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class vPushableObject : MonoBehaviour
{
    [System.Serializable]
    public class FloatEvent : UnityEngine.Events.UnityEvent<float>
    { }
    public bool controlBodyKinematics = true;
    public bool controlBodyConstriants = true;
    [SerializeField] protected bool _isBlocked;

    public bool isStoppig { get; protected set; }
    protected Rigidbody _body;
    public Rigidbody body
    {
        get
        {
            if (_body == null)
            {
                _body = GetComponent<Rigidbody>();
            }

            return _body;
        }
    }

    public UnityEngine.Events.UnityEvent onStartMove, onStopMove;
    public UnityEngine.Events.UnityEvent onEnterPushMode, onExitPushMode, onRigidodySleep;
    public FloatEvent onMovimentSpeedChanged;

    public bool isBlocked { get { return _isBlocked; } set { _isBlocked = value; } }
    protected RigidbodyConstraints startConstraints;

    protected virtual void Awake()
    {
        startConstraints = body.constraints;
        StartCoroutine(Stop());
    }

    internal virtual void StartPushAndPull()
    {
        if (controlBodyKinematics)
        {
            body.isKinematic = false;
        }

        if (controlBodyConstriants)
        {
            body.constraints = RigidbodyConstraints.FreezeRotation;
        }

        onEnterPushMode.Invoke();

    }

    internal virtual void FinishPushAndPull()
    {
        if (controlBodyKinematics)
        {
            body.isKinematic = true;
        }

        if (controlBodyConstriants)
        {
            body.constraints = startConstraints;
        }

        onExitPushMode.Invoke();
        if (!isStoppig)
        {
            StartCoroutine(Stop());
        }
    }

    protected virtual IEnumerator Stop()
    {
        isStoppig = true;
        yield return new WaitForEndOfFrame();
        if (controlBodyKinematics)
        {
            body.isKinematic = false;
        }

        float sleepingTimer = .1f;

        while (!body.IsSleeping() && !body.isKinematic && sleepingTimer > 0)
        {
            if (body.velocity.magnitude < 0.1f)
            {
                sleepingTimer -= Time.deltaTime;
            }
            else
            {
                sleepingTimer = .1f;
            }
            yield return null;
        }
        isStoppig = false;
        if (controlBodyKinematics)
        {
            body.isKinematic = true;
        }

        onRigidodySleep.Invoke();
    }
}
