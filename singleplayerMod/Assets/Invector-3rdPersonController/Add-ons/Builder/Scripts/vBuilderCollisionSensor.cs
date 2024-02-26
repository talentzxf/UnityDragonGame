using UnityEngine;

public class vBuilderCollisionSensor : MonoBehaviour
{
    public TriggerEvent onTriggerStay;
    public TriggerEvent onTriggerExit;
    public CapsuleCollider _capsuleCollider;
    private void Awake()
    {
        _capsuleCollider = GetComponent<CapsuleCollider>();
        if(_capsuleCollider)
        {
            _capsuleCollider.gameObject.tag = "Action";
            _capsuleCollider.isTrigger = true;

            var colliders = transform.root.GetComponentsInChildren<Collider>(true);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != _capsuleCollider.gameObject)
                    Physics.IgnoreCollision(colliders[i], _capsuleCollider, true);
            }
        }
           
    }
    public void OnTriggerStay(Collider other)
    {
        onTriggerStay.Invoke(other);
    }

    public void OnTriggerExit(Collider other)
    {
        onTriggerExit.Invoke(other);
    }

    [System.Serializable]
    public class TriggerEvent : UnityEngine.Events.UnityEvent<Collider>
    {
        
    }
}