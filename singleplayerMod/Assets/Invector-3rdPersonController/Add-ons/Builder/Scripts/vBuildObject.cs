using Invector;
using Invector.vCharacterController.vActions;
using Invector.vItemManager;
using System.Collections.Generic;
using UnityEngine;

[vClassHeader("Build Object", "Make sure to replace the ItemID to actually get the correct SpawnPrefab, you can assign the SpawnPrefab on your ItemListData", openClose = false)]
public class vBuildObject : vMonoBehaviour
{
    #region Variables

    [Tooltip("Layers to create on")]
    public LayerMask layer = 1 << 0;
    public bool alignToSurface = true;
    [vHideInInspector("alignToSurface")]
    public bool simpleSurfaceAlignment;
    [Tooltip("Tags that will be ignored")]
    public List<string> tagsToExclude = new List<string>() { "Player", "Action", "IgnoreRagdoll", "Weapon" };

    public Color validColor = new Color(0, 1, 0, 0.3f);
    public Color invalidColor = new Color(1, 0, 0, 0.3f);

    public Renderer _renderer;
    public BoxCollider _collider;
    public bool alwaysShowGizmos;
    [Header("Optional")]
    public bool strafeWhileCreate;
    public Transform target;
    public string customCameraState;
    public float rotateSpeed = 3f;

    public UnityEngine.Events.UnityEvent onEnterBuilding, onExitBuilding, onSpawnBuild;
    public OnValidateTrapCreation onValidateTrap;
    [System.Serializable]
    public class OnValidateTrapCreation : UnityEngine.Events.UnityEvent<bool> { }

    [vHelpBox("Use a TriggerGenerecAction to play a custom animation or leave it unassign to spawn the prefab instantly.")]
    public vTriggerGenericAction trigger;
    public GameObject spawnPrefab;
    Transform parent;


    Vector3 lastValidPos;
    bool inBuildMode;
    bool isValid;

    [vHelpBox("Make sure your Item is assign with a SpawnObject Prefab in the ItemListData")]
    public int id;

    [SerializeField] protected bool _canCreate = true;
    public float creationDistance = 1;
    public float animationDistance;
    public float offsetY;
    public float offsetAngle;
    public float offsetAnglePreview;
    [vHelpBox("Used to detect surface height and alignment")]
    [Range(0.001f, 1f)]
    public float rayRadius = 0.2f;
    public float rayHeight = 0.5f;
    [vReadOnly]
    public bool inCollision, canUpdateBuild, isInSurface;

    /// <summary>
    /// Set if can be created. If <see cref="equipedItemInfo"/> == null or item is not is equiped the values aways will be false;
    /// </summary>
    public bool canCreate
    {
        get
        {
            return _canCreate && equipedItemInfo != null && equipedItemInfo.item != null;
        }
        set
        {
            _canCreate = value;
        }
    }

    public bool canSpawn
    {
        get { return !inCollision && isInSurface; }
    }

    public vItemListOperations.EquipedItemInfo equipedItemInfo;
    public vBuilderCollisionSensor collisionSensor;
    public float collisionSensorHeight = 0.5f;
    public float collisionSensorRadius = 0.25f;

    public BoxPoints boxPoints = new BoxPoints();

    public bool debugMode;
    protected BoxCollider referenceCollider;
    #endregion


    private void Awake()
    {
        parent = transform.parent;
        if (trigger)
        {
            ///Add event to <seealso cref="vTriggerGenericAction"/>
            trigger.OnPressActionInput.AddListener(() => { SetActiveUpdateTrap(false); });
            trigger.OnCancelActionInput.AddListener(() => { SetActiveUpdateTrap(true); });
            if (trigger.inputType == vTriggerGenericAction.InputType.GetButtonTimer)
            {
                trigger.OnFinishActionInput.AddListener(() => { CreateBuild(); SetActiveUpdateTrap(true); });
            }
            else
            {
                trigger.OnEndAnimation.AddListener(() => { CreateBuild(); SetActiveUpdateTrap(true); });
            }

            trigger.gameObject.SetActive(false);
        }

        var colliders = transform.root.GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != _collider.gameObject)
            {
                Physics.IgnoreCollision(colliders[i], _collider, true);
            }
        }
        referenceCollider = new GameObject("ReferenceCollider", typeof(BoxCollider)).GetComponent<BoxCollider>();

        referenceCollider.size = _collider.size;
        referenceCollider.center = _collider.center;
        referenceCollider.transform.parent = _collider.transform.parent;
        referenceCollider.transform.localPosition = _collider.transform.localPosition;
        referenceCollider.transform.localEulerAngles = _collider.transform.localEulerAngles;
        referenceCollider.gameObject.SetActive(false);
        //_collider = GetComponentInParent<BoxCollider>();
    }

    public struct BoxPoints
    {
        public bool simple;
        public Vector3 pR_F, pL_F, pR_B, pL_B;
        public Vector3 center, right, forward;
        RaycastHit hR_F, hR_B, hL_F, hL_B, hCenter;

        public void RayCastPoints(out Vector3 position, out Vector3 normal, Vector3 center, Vector3 right, Vector3 forward, BoxCollider _collider, LayerMask mask, float radius = 0.1f, float height = 2f)
        {
            UpdateValues(center, right, forward, _collider, radius);
            position = this.center;
            Vector3 p0 = this.center + Vector3.up * height;
            Vector3 p1 = pR_F + Vector3.up * height;
            Vector3 p2 = pR_B + Vector3.up * height;
            Vector3 p3 = pL_F + Vector3.up * height;
            Vector3 p4 = pL_B + Vector3.up * height;

            if (Physics.Linecast(this.center, p0, out hCenter, mask))
            {
                p0.y = hCenter.point.y;
            }
            if (!simple)
            {
                if (Physics.Linecast(pR_F, p1, out hR_F, mask))
                {
                    p1.y = hR_F.point.y;
                }

                if (Physics.Linecast(pR_B, p2, out hR_B, mask))
                {
                    p2.y = hR_B.point.y;
                }

                if (Physics.Linecast(pL_F, p3, out hL_F, mask))
                {
                    p3.y = hL_F.point.y;
                }

                if (Physics.Linecast(pL_B, p4, out hL_B, mask))
                {
                    p4.y = hL_B.point.y;
                }
            }
            if (Physics.Raycast(p0, Vector3.down, out hCenter, 4, mask))
            {
                position.y = hCenter.point.y;
                UpdateValues(position, right, forward, _collider, radius);
            }
            if (!simple)
            {
                if (Physics.SphereCast(p1, radius, Vector3.down, out hR_F, height * 2, mask))
                {
                    pR_F.y = hR_F.point.y;
                }

                if (Physics.SphereCast(p2, radius, Vector3.down, out hR_B, height * 2, mask))
                {
                    pR_B.y = hR_B.point.y;
                }

                if (Physics.SphereCast(p3, radius, Vector3.down, out hL_F, height * 2, mask))
                {
                    pL_F.y = hL_F.point.y;
                }

                if (Physics.SphereCast(p4, radius, Vector3.down, out hL_B, height * 2, mask))
                {
                    pL_B.y = hL_B.point.y;
                }
            }

            Vector3 meddlePoint = simple ? position : (pR_F + pR_B + pL_F + pL_B) / 4;
            if (meddlePoint.y > position.y)
            {
                position = meddlePoint;
            }

            Vector3 upDir = simple ? hCenter.normal : (-GetNormal(pL_F, pL_B, pR_F) +
                             -GetNormal(pR_F, pL_F, pR_B) +
                             -GetNormal(pR_B, pR_F, pL_B) +
                             -GetNormal(pL_B, pR_B, pL_F)).normalized;
            normal = upDir;

        }
        Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 c)
        {
            // Find vectors corresponding to two of the sides of the triangle.
            Vector3 side1 = b - a;
            Vector3 side2 = c - a;

            // Cross the vectors to get a perpendicular vector, then normalize it.
            return Vector3.Cross(side1, side2).normalized;
        }
        public void UpdateValues(Vector3 center, Vector3 right, Vector3 forward, BoxCollider _collider, float radius = 0.1f)
        {
            this.center = center;
            this.right = right * ((_collider.size.x * 0.5f) - radius);
            this.forward = forward * ((_collider.size.z * 0.5f) - radius);
            pR_F = center + this.right + this.forward;
            pL_F = center - this.right + this.forward;
            pR_B = center + this.right - this.forward;
            pL_B = center - this.right - this.forward;
        }
    }
#if UNITY_EDITOR

    protected void DraArrow(Vector3 position, float size = 0.1f)
    {
        Gizmos.DrawRay(position, Vector3.forward * size);
        Gizmos.DrawRay(position, Vector3.back * size);
        Gizmos.DrawRay(position, Vector3.right * size);
        Gizmos.DrawRay(position, Vector3.left * size);
    }

    private void OnDrawGizmos()
    {
        if (_collider && ((Application.isPlaying && inBuildMode) || (alwaysShowGizmos || UnityEditor.Selection.activeGameObject == gameObject)))
        {
            if (!Application.isPlaying)
            {
                var forward = Quaternion.AngleAxis(rotationY + offsetAngle, transform.up) * _collider.transform.forward;
                Vector3 right = Quaternion.AngleAxis(90, transform.up) * forward;
                boxPoints.UpdateValues(transform.position, right, forward, _collider, rayRadius);
            }
            else if (!inBuildMode)
            {
                return;
            }

            Gizmos.color = (isValid ? Color.green : Color.red) * 0.5f;
            var position = transform.position;
            position.y = (parent ? parent : transform.parent).position.y;
            Debug.DrawRay(position + Vector3.up * rayHeight, Vector3.down * (rayHeight * 2f));
            DraArrow(position + Vector3.up * rayHeight);
            DraArrow(position + Vector3.down * rayHeight);
            if (simpleSurfaceAlignment || !alignToSurface)
            {
                Gizmos.DrawSphere(transform.position, 0.2f);
            }
            else
            {
                Gizmos.DrawSphere(boxPoints.pR_F, rayRadius);
                Gizmos.DrawSphere(boxPoints.pL_F, rayRadius);
                Gizmos.DrawSphere(boxPoints.pR_B, rayRadius);
                Gizmos.DrawSphere(boxPoints.pL_B, rayRadius);
            }

            Gizmos.matrix = Matrix4x4.TRS(_collider.transform.position, _collider.transform.rotation, (_collider.transform.lossyScale));
            Gizmos.DrawCube(_collider.center, _collider.size);
        }

    }
#endif
    /// <summary>
    /// Enter Build Mode 
    /// </summary>
    public virtual void EnterBuild()
    {
        if (debugMode)
        {
            Debug.Log("Enter Build");
        }

        if (collisionSensor)
        {
            collisionSensor.onTriggerStay.AddListener(OnTriggerStay);
            collisionSensor.onTriggerExit.AddListener(OnTriggerExit);
        }
        if (trigger != null)
        {
            trigger.gameObject.SetActive(true);
        }

        onEnterBuilding.Invoke();                                  // call onEnterBuildMove Event
        canUpdateBuild = true;
        transform.parent = null;
        _collider.gameObject.SetActive(true);
        inBuildMode = true;
    }

    /// <summary>
    /// Exit Build Mode 
    /// </summary>
    public virtual void ExitBuild()
    {
        if (debugMode)
        {
            Debug.Log("Exit Build");
        }

        if (collisionSensor)
        {
            collisionSensor.onTriggerStay.RemoveListener(OnTriggerStay);
            collisionSensor.onTriggerExit.RemoveListener(OnTriggerExit);
        }
        onExitBuilding.Invoke();
        _collider.gameObject.SetActive(false);
        if (trigger != null)
        {
            trigger.gameObject.SetActive(false);
        }

        if (transform.parent == null)
        {
            transform.parent = parent;
        }

        inBuildMode = false;
    }

    /// <summary>
    /// Update the Build Object position, rotation and check if can spawn the BuildObject prefab
    /// </summary>
    /// <param name="bm"></param>
    public virtual void HandleBuild(vBuildManager bm)
    {
        if (isValid != canSpawn)
        {
            isValid = canSpawn;
            onValidateTrap.Invoke(isValid);
        }
        if (canUpdateBuild)
        {
            boxPoints.simple = simpleSurfaceAlignment;
            ///BuildObject preview color
            _renderer.sharedMaterial.color = isValid ? validColor : invalidColor;

            Debug.DrawRay(parent.position, Vector3.up, Color.red, 0.01f);

            ///Calc the position of the BuildObject
            var dir = Quaternion.AngleAxis(-90, Vector3.up) * Camera.main.transform.right;
            dir.y = 0;
            var position = parent.position + dir.normalized * creationDistance;

            ///Calc diference between position and closest point to recalc position.
            var referenceP = referenceCollider.transform.position;
            referenceP.y = parent.transform.position.y;
            referenceCollider.transform.position = referenceP;
            var closestPoint = referenceCollider.ClosestPoint(parent.position);
            var distance = Vector3.Distance(position, closestPoint);
            position = position + dir.normalized * distance;

            ///Update Collision Sensor position, rotation and height to prevent Walls and other obstacles
            if (collisionSensor)
            {
                collisionSensor.transform.localPosition = Vector3.up * ((collisionSensor._capsuleCollider.radius * 2f) + collisionSensorHeight);
                var dist = Vector3.Distance(position, parent.position);
                collisionSensor._capsuleCollider.height = dist;
                collisionSensor._capsuleCollider.radius = collisionSensorRadius;
                collisionSensor._capsuleCollider.center = new Vector3(0f, 0f, (dist * 0.5f));
                collisionSensor.transform.rotation = Quaternion.LookRotation(_collider.bounds.center - collisionSensor.transform.position);
            }

            Vector3 up = Vector3.up;
            ///Check Surface to plant BuildObject to recalc position if necessary
            RaycastHit hit;
            if (Physics.Raycast(position + Vector3.up * rayHeight, Vector3.down, out hit, rayHeight * 2, layer) ||
                Physics.SphereCast(position + Vector3.up * rayHeight, rayRadius, Vector3.down, out hit, rayHeight * 2, layer))
            {
                position.y = hit.point.y;
                if (alignToSurface)
                {
                    up = hit.normal;
                }

                isInSurface = true;
            }
            else
            {
                isInSurface = false;
            }

            ///Update Position and rotation of the BuildObject
            if (canSpawn && isInSurface)
            {
                lastValidPos = position;
            }

            transform.position = position;
            AlignToSurface(position, dir.normalized, up);

            ///Update Match Target of the <seealso cref="vTriggerGenericAction"/> to correct position and rotation
            if (target && trigger)
            {
                target.position = position - dir.normalized * (animationDistance);
                target.LookAt(transform.position, Vector3.up);
                if (alwaysShowGizmos)
                {
                    Debug.DrawRay(target.position, target.up);
                    Debug.DrawRay(target.position, target.right);
                    Debug.DrawRay(target.position, -target.right);
                    Debug.DrawRay(target.position, target.forward);
                    Debug.DrawRay(target.position, -target.forward);
                }
            }
        }
    }

    public void CreateObjectInput(vBuildManager bm)
    {
        /// Instantly instantiate the prebab without an trigger  <seealso cref="vGenericAction"/> 
        if (!trigger && bm.createBuildInput.GetButtonDown() && isValid)
        {
            CreateBuild();
        }
        else if (trigger && (isValid || bm.genericAction.playingAnimation))
        {
            ///Fix Position of build object to last valid position befoure animation started 
            if (bm.genericAction.playingAnimation)
            {
                transform.position = lastValidPos;
            }

            ///Simulate <seealso cref="vGenericAction"/>  Input Behaviour
            bm.genericAction.triggerAction = trigger;
            bm.genericAction.TriggerActionInput();
        }
    }

    /// <summary>
    /// Align the collider to the surface
    /// </summary>
    public virtual void AlignToSurface(Vector3 center, Vector3 forward, Vector3 up)
    {
        if (!_collider)
        {
            return;
        }

        forward = Quaternion.AngleAxis(rotationY + offsetAngle, transform.up) * forward;
        Vector3 right = Quaternion.AngleAxis(90, transform.up) * forward;


        Vector3 position = center;
        Vector3 normal = up;
        if (alignToSurface && !simpleSurfaceAlignment)
        {
            boxPoints.RayCastPoints(out position, out normal, center, right, forward, _collider, layer, rayRadius, rayHeight);
        }

        var myForward = Vector3.Cross(right, normal);
        var lookFwd = Quaternion.AngleAxis(offsetAnglePreview, normal) * myForward;
        var referenceFwd = lookFwd;
        referenceFwd.y = 0;
        referenceCollider.transform.rotation = Quaternion.LookRotation(referenceFwd.normalized, Vector3.up);
        if (lookFwd != Vector3.zero)
        {
            var rotation = Quaternion.LookRotation(lookFwd, normal);
            _collider.transform.rotation = Quaternion.Lerp(_collider.transform.rotation, rotation, 20 * Time.deltaTime);
        }

        Vector3 p = position + normal.normalized * offsetY;
        _collider.transform.position = Vector3.Lerp(_collider.transform.position, p, 20 * Time.deltaTime);
    }

    /// <summary>
    /// Instantiate the Prefab of the buildObject
    /// </summary>
    public void CreateBuild()
    {
        Instantiate(spawnPrefab, _collider.transform.position, _collider.transform.rotation);
        ExitBuild();
        onSpawnBuild.Invoke();
    }

    /// <summary>
    /// BuildObject Rotation Y
    /// </summary>
    public float rotationY { get; set; }

    /// <summary>
    /// Enable or disable Update 
    /// </summary>
    /// <param name="value"></param>
    public void SetActiveUpdateTrap(bool value)
    {
        canUpdateBuild = value;
    }

    private void OnTriggerStay(Collider other)
    {
        if (canUpdateBuild && !tagsToExclude.Contains(other.gameObject.tag))
        {
            if (_renderer.enabled)
            {
                inCollision = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!tagsToExclude.Contains(other.gameObject.tag))
        {
            if (_renderer.enabled)
            {
                inCollision = false;
            }
        }
    }
}