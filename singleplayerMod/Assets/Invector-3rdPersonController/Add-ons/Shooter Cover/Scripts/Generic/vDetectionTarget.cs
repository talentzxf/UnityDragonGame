using Invector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[vClassHeader("Detection Target", "This is a target to be detected from vDetectionController", useHelpBox = true, openClose = false)]
public class vDetectionTarget : vMonoBehaviour
{
    public enum TargetPriority
    {
        None = 0, Minimal = 1, Medium = 2, High = 3, Maximum = 4,
    }
    public vIHealthController healthController;
    [FormerlySerializedAs("center"), SerializeField, Tooltip("The center of Target used to detect")]
    protected Transform centerPoint;
    public Transform center => centerPoint ? centerPoint : transform;
    [SerializeField] protected List<DetectionPoint> detectionPoints;
    [SerializeField] protected float detectionReduction = 0f;

    /// <summary>
    /// This is used to make it harder to detect <see cref="vDetectionController"/>, the higher the more difficult to detect this target
    /// </summary>
    public virtual float DetectionReductionValue
    {
        get
        {
            return detectionReduction;
        }
        set
        {
            detectionReduction = value;
        }
    }

    /// <summary>
    /// Get the detection points
    /// </summary>
    public virtual List<DetectionPoint> DetectionPoints => detectionPoints;
    [System.Serializable]
    public partial class DetectionPoint
    {
        /// <summary>
        /// The name of detection Point
        /// </summary>
        [Tooltip("The name of detection Point, this is just for list order")]
        public string name;
        /// <summary>
        /// The detection point reference, Used to check if the point is seen for someone
        /// </summary>
        [Tooltip("The detection point reference, Used to check if the point is seen for someone")]
        public Transform center;
        /// <summary>
        /// The score of this point used to compare with <see cref="vDetectionController.requiredDetectionValue"/>  of the observer
        /// </summary>
        [Tooltip("The score of this point used to compare with required detection value of observer")]
        public float value;
        /// <summary>
        /// Priority value of this detection point
        /// </summary>
        [Tooltip("Priority value of this detection point")]
        public TargetPriority targetPriority = TargetPriority.Minimal;
    }

    [ContextMenu("Order Points by Value")]
    protected virtual void OrdePointByValue()
    {
        detectionPoints = detectionPoints.OrderByDescending(p => p.value).ToList();

    }

    [ContextMenu("Find Points")]
    protected virtual void AutoFindDetectionPoints()
    {
        var ps = GetComponentsInChildren<vIDamageReceiver>();

        foreach (var p in ps)
        {
            if (!detectionPoints.Exists(d => d.center.gameObject.Equals(p.gameObject)))
            {
                detectionPoints.Add(new DetectionPoint() { name = p.gameObject.name, center = p.transform, value = 1, targetPriority = TargetPriority.Minimal });
            }
        }
        if (detectionPoints.Count > 0)
        {
            detectionPoints = detectionPoints.FindAll(d => d.center != null);
        }
    }

    protected virtual void Start()
    {
        healthController = GetComponent<vIHealthController>();
    }

    /// <summary>
    /// Check if target is valid
    /// </summary>
    public virtual bool IsValid => healthController != null && !healthController.isDead;

    /// <summary>
    /// Add or Remove value from <see cref="DetectionReductionValue"/>
    /// </summary>
    /// <param name="value"></param>
    public virtual void AddDetectionReduction(float value)
    {
        detectionReduction += value;
    }
}
