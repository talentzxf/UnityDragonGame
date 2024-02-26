using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class vPushObjectPoint : MonoBehaviour
{
    public Rigidbody targetBody => pushableObject.body;
    [SerializeField]
    protected bool _canPushForward = true;
    [SerializeField]
    protected bool _canPushBack = true;
    [SerializeField]
    protected bool _canPushLeft = true;
    [SerializeField]
    protected bool _canPushRight = true;

    [Tooltip("Move Character to center of PushObject")]
    public bool pushPointInCenter = true;
    public Transform handsAdjustReference;
    public bool canPushForward { get { return _canPushForward && !pushableObject.isBlocked; } set { _canPushForward = value; } }
    public bool canPushBack { get { return _canPushBack && !pushableObject.isBlocked; } set { _canPushBack = value; } }
    public bool canPushLeft { get { return _canPushLeft && !pushableObject.isBlocked; } set { _canPushLeft = value; } }
    public bool canPushRight { get { return _canPushRight && !pushableObject.isBlocked; } set { _canPushRight = value; } }
    public bool canUse { get => !pushableObject.isStoppig && targetBody.velocity.magnitude == 0; }
    public vPushableObject pushableObject { get; protected set; }
    public BoxCollider boxCollider { get; protected set; }

    public virtual Vector3 targetBodyPosition
    {
        get
        {
            return targetBody.transform.position;
        }
        set
        {
            targetBody.transform.position = value;
        }
    }

    protected virtual void OnDrawGizmos()
    {
        if (!boxCollider)
        {
            boxCollider = GetComponent<BoxCollider>();
        };
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, (transform.lossyScale));
        Vector3 position = boxCollider.center;
        Vector3 size = Vector3.one;
        size.x *= boxCollider.size.x;
        size.y *= boxCollider.size.y;
        size.z *= boxCollider.size.z;
        Gizmos.color = Color.green * 0.8f;
        Gizmos.DrawWireCube(position, size);


        Color green = new Color(0, 1, 0, 0.2f);
        Gizmos.color = green;
        Gizmos.DrawCube(position, size);
    }

    protected virtual void Awake()
    {
        pushableObject = GetComponentInParent<vPushableObject>();
        boxCollider = GetComponent<BoxCollider>();
        var colliders = transform.root.GetComponentsInChildren<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            var otherCollider = colliders[i];
            if (otherCollider != boxCollider)
            {
                Physics.IgnoreCollision(otherCollider, boxCollider);
            }
        }
    }

}
