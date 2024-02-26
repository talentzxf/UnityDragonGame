using Invector;
using UnityEngine;
using UnityEngine.Events;

[vClassHeader("vTURRET", openClose = false)]
public class vTurret : vMonoBehaviour
{
    [vEditorToolbar("Settings")]
    public bool unlimitedUsage = false;
    public float maxUsageTime;
    [vBarDisplay("maxUsageTime")]
    public float currentUsageTime;
    public bool startOff;
    public vHealthController target;
    [Tooltip("Transform to align to Aim target")]
    public Transform aimReference;
    [Tooltip("Fixed Transform to calc max angle")]
    public Transform angleReference;
    public vTagMask targetTag = new vTagMask("Enemy");
    //public string targetTag = "Enemy";
    public float targetOffSetY = 0.5f;
    public float range = 3;
    public float smoothFollow = 2f;
    public bool limitAngle = true;
    public float turnSmooth = 8f;
    [Range(0.0f, 180.0f)]
    public float maxAngle = 60.0f;
    public float minStartAngleToShot = 45f;
    public UnityEvent onFindTarget, onLostTarget, onShot;
    public bool normalizeTimeResult;
    public bool invertTimeResult;
    public OnChangeValue onUpdateCurrentUsageTime;
    public OnChangeValue onUpdateMaxUsageTime;
    [vHelpBox("Event called when usage time is finish or usage time is increase when turret is Off")]
    public UnityEvent onTurnOn;
    public UnityEvent onTurnOff;
    [System.Serializable]
    public class OnChangeValue : UnityEngine.Events.UnityEvent<float> { }
    SphereCollider _collider;
    private float _shotFrequency;
    private Quaternion defaultRotation;
    [vEditorToolbar("Detection")]
    public bool useObstacles;
    [vHideInInspector("useObstacles")]
    public LayerMask obstacles;
    [vReadOnly(false)]
    protected bool isOn;

    void Start()
    {
        _collider = GetComponent<SphereCollider>();
        _collider.radius = range;
        defaultRotation = transform.rotation;
        if (!startOff)
        {
            currentUsageTime = maxUsageTime;
            onTurnOn.Invoke();
        }
        else onTurnOff.Invoke();

        isOn = !startOff;
        onUpdateMaxUsageTime.Invoke(maxUsageTime);
        UpdateUsageTime();
    }

    void Update()
    {
        FollowTarget();
    }

    private void UpdateUsageTime()
    {
        if (normalizeTimeResult)
        {
            onUpdateCurrentUsageTime.Invoke(invertTimeResult ? 1 - (currentUsageTime / maxUsageTime) : currentUsageTime / maxUsageTime);
        }
        else
        {
            onUpdateCurrentUsageTime.Invoke(invertTimeResult ? maxUsageTime - currentUsageTime : currentUsageTime);
        }
    }

    private void FollowTarget()
    {
        CheckOnOff();
        if (target && target.currentHealth > 0 && currentUsageTime > 0)
        {
            var _target = new Vector3(target.gameObject.transform.position.x, target.gameObject.transform.position.y + targetOffSetY, target.gameObject.transform.position.z);
            var v3Target = (_target - aimReference.position);

            var angleOfTarget = Vector3.Angle(v3Target, angleReference.forward);
            var angleOfAim = Vector3.Angle(v3Target, aimReference.forward);
            // out of range            
            if (angleOfTarget >= maxAngle || CheckObtacles(_target))
            {
                var v3Axis = Vector3.Cross(angleReference.forward, v3Target);
                v3Target = Quaternion.AngleAxis(maxAngle, v3Axis) * angleReference.forward;
                ResetTarget();
            }
            else if (angleOfAim < minStartAngleToShot)
            {
                OnShot();
                if (!unlimitedUsage)
                {
                    currentUsageTime -= Time.deltaTime;
                    UpdateUsageTime();
                }
            }

            var qTarget = Quaternion.LookRotation(v3Target);
            transform.rotation = Quaternion.Slerp(transform.rotation, qTarget, Time.deltaTime * turnSmooth);
        }
        else if(target)
        {            
            ResetTarget();
            transform.rotation = Quaternion.Slerp(transform.rotation, defaultRotation, Time.deltaTime * turnSmooth);
        }
    }

    bool CheckObtacles(Vector3 targetPos)
    {
        if (!useObstacles) return false;
        var _target = targetPos;
        return Physics.Linecast(angleReference.position, _target, obstacles);
    }

    private void CheckOnOff()
    {
        if (currentUsageTime <= 0 && isOn)
        {
            onTurnOff.Invoke();
            isOn = false;
        }
        else if (currentUsageTime > 0 && !isOn)
        {
            onTurnOn.Invoke();
            isOn = true;
        }
    }

    private void OnShot()
    {
        onShot.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        if (targetTag.Contains(other.tag) && target == null)
        {
            var _target = new Vector3(other.transform.position.x, other.transform.position.y + targetOffSetY, other.transform.position.z);
            var dir = (_target - aimReference.position);
            var angleOfTarget = Vector3.Angle(dir, angleReference.forward);
            if (angleOfTarget < maxAngle && !CheckObtacles(_target))
            {
                //Debug.Log("pegou target");
                target = other.GetComponent<vHealthController>();
                onFindTarget.Invoke();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (targetTag.Contains(other.tag) && target != null && target.gameObject == other.gameObject)
        {           
            ResetTarget();
        }
    }

    public void AddCurrentUsageTime(float time)
    {
        currentUsageTime += time;
        UpdateUsageTime();
    }

    public void SetCurrentUsageTimeToMax()
    {
        currentUsageTime = maxUsageTime;
        UpdateUsageTime();
    }

    public void ChangeCurrentUsageTime(float time)
    {
        currentUsageTime = time;
        UpdateUsageTime();
    }

    public void AddMaxUsageTime(float time)
    {
        maxUsageTime += time;
        onUpdateMaxUsageTime.Invoke(normalizeTimeResult ? 1 : maxUsageTime);
        UpdateUsageTime();
    }

    public void ChangeMaxUsageTime(float time)
    {
        maxUsageTime = time;
        onUpdateMaxUsageTime.Invoke(normalizeTimeResult ? 1 : maxUsageTime);
        UpdateUsageTime();
    }

    void ResetTarget()
    {        
        target = null;
        onLostTarget.Invoke();
    }
}
