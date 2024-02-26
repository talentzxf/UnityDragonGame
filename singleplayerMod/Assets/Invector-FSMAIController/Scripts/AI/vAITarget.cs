using UnityEngine;
namespace Invector.vCharacterController.AI
{
    [System.Serializable]
    public partial class vAITarget
    {
        [SerializeField] protected string _tag;
        [SerializeField] protected Transform _transform;
        [SerializeField, HideInInspector] protected Collider _collider { get; set; }
        public TargetPriority targetPriority => targetInfo ? targetInfo.priority : TargetPriority.None;
        public vAITargetInfo targetInfo { get; protected set; }
        public Transform transform { get { return _transform; } protected set { _transform = value; } }

        public GameObject gameObject { get { return _transform ? _transform.gameObject : null; } }
        public Collider collider { get { return _collider; } protected set { _collider = value; } }
        public vIHealthController healthController => targetInfo != null ? targetInfo.healthController : null;

        public bool isFixedTarget = false;
        [HideInInspector]
        public bool isLost;
        public string tag => _tag;

        public static implicit operator Transform(vAITarget m)
        {
            try
            {
                return m.transform;
            }
            catch { return null; }
        }
        public static bool operator !=(vAITarget target, Transform target2)
        {
            if (target2 is null) return !(target.transform is null);
            return !target.transform == target2.transform;
        }
        public static bool operator ==(vAITarget target, Transform target2)
        {
            if (target2 is null) return (target.transform is null);
            return target.transform == target2.transform;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj is Transform)
            {
                return (transform == (obj as Transform));
            }
            return base.Equals(obj);
        }
        public bool isDead
        {
            get
            {
                var value = true;
                if (healthController != null) return healthController.isDead;
                else if (!transform.gameObject.activeInHierarchy) value = true;
                else if (_collider) value = !_collider.enabled;
                return value;
            }
        }
        public float currentHealth
        {
            get
            {
                if (healthController != null) return healthController.currentHealth;
                return 0;
            }
        }

        public vAITarget() { }
        public vAITarget(GameObject target) => InitTarget(target);
        public vAITarget(Collider target) => InitTarget(target);
        public vAITarget(Transform target) => InitTarget(target);
        public void InitTarget(GameObject target)
        {
            if (target)
            {                
                transform = target.transform;
                collider = transform.GetComponent<Collider>();
                targetInfo = transform.GetComponent<vAITargetInfo>();
                _tag = targetInfo ? targetInfo.targetTag : target.gameObject.tag;
            }
        }
        public void InitTarget(Collider target)
        {
            if (target)
            {
                transform = target.transform;
                collider = target;
                targetInfo = transform.GetComponent<vAITargetInfo>();
                _tag = targetInfo ? targetInfo.targetTag : target.gameObject.tag;
            }
        }
        public void InitTarget(Transform target)
        {
            if (target)
            {
                transform = target;
                collider = transform.GetComponent<Collider>();
                targetInfo = transform.GetComponent<vAITargetInfo>();
                _tag = targetInfo ? targetInfo.targetTag : target.gameObject.tag;
            }
        }
        public void ClearTarget()
        {
            transform = null;
            collider = null;

        }
    }
}