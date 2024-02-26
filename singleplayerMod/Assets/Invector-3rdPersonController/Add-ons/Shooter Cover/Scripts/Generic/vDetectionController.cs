using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Invector
{  
    [vClassHeader("Detection")]
    public class vDetectionController : vMonoBehaviour
    {
        [System.Serializable]
        public class DetectedvTargetEvent : UnityEngine.Events.UnityEvent<vDetectionTarget> { }
        [System.Serializable]
        public class DetectedTransformEvent : UnityEngine.Events.UnityEvent<Transform> { }
        [System.Serializable]
        public class DetectedTarget
        {
            [vReadOnly(false)] public vDetectionTarget target;
            [vReadOnly(false)] public Transform detectedPoint;
            [vReadOnly(false)] public bool targetIsInSight;
            [vReadOnly(false)] public float detectionValue;
            [vReadOnly(false)] public bool targetIsLosted;

            public void Reset()
            {
                target = null;
                detectedPoint = null;
                targetIsInSight = false;
                targetIsLosted = true;
            }
        }

        [vEditorToolbar("Detection")]
        public bool autoUpdate = true;
        public Collider sensorCollider;
        public Transform eyesCenter;
        public bool useDetectionPoints = true;
        public bool useObstacles;
        [vHideInInspector("useObstacles")]
        public LayerMask obstacles;
        public float requiredDetectionValue = 4;
        public float timeToLostTarget;
        public float detectionFrequency = 1f;
        public float timeToTriggerFindTargetEvent = 0;
        public vDetectionTarget.TargetPriority minPriority = vDetectionTarget.TargetPriority.None;
        public vDetectionTarget.TargetPriority maxPriority = vDetectionTarget.TargetPriority.Maximum;
        public vTagMask targetTag = new vTagMask("Enemy");
        [Range(0.0f, 180.0f)]
        public float maxAngleVertical = 60.0f;
        [Range(0.0f, 180.0f)]
        public float maxAngleHorizontal = 60.0f;

        [vEditorToolbar("Events")]
        public DetectedTransformEvent onFindTransformTarget;
        public DetectedTransformEvent onLostTransformTarget;
        public DetectedvTargetEvent onFindvTarget;
        public DetectedvTargetEvent onLostvTarget;

        [vEditorToolbar("Debug")]
        public bool debugRays;
        public bool debugTargetInAngle;
        public bool debugTargetObstacle;
        public List<vDetectionTarget> targetsInRange;
        public DetectedTarget detectedTarget = new DetectedTarget();
        [vReadOnly(false), SerializeField] protected float currentTimeToFindTarget = 0;
        [vReadOnly(false), SerializeField] protected float currentTimeToTriggerFindTargetEvent = 0;
        [vReadOnly(false), SerializeField] protected float currentTimeToLostTarget = 0;
        [vReadOnly(false), SerializeField] protected float targetAngleHorizontal = 0;
        [vReadOnly(false), SerializeField] protected float targetAngleVertical = 0;

        RaycastHit hit;
        protected int indexOfDetection;
        protected Vector3 targetDirection;

        protected virtual void Start()
        {
            if (!eyesCenter)
            {
                eyesCenter = transform;
            }

            targetsInRange = new List<vDetectionTarget>();

        }

        protected virtual void LateUpdate()
        {
            
            if (autoUpdate)
            {
                UpdatedeDetection();
            }
        }

        /// <summary>
        /// Update the detection behaviour
        /// </summary>
        public virtual void UpdatedeDetection()
        {
            if (this.enabled == false) return;

            if (targetsInRange.Count > 0)
            {
                if (detectedTarget.target == null || targetsInRange.Exists(_target => Vector3.Distance(detectedTarget.target.center.position, eyesCenter.position) > Vector3.Distance(_target.center.position, eyesCenter.position)))
                {
                    if (currentTimeToFindTarget <= 0)
                    {
                        vDetectionTarget _target = null;
                        currentTimeToFindTarget = detectionFrequency;
                        if (targetsInRange.Count > 0)
                        {
                            if (indexOfDetection >= targetsInRange.Count)
                            {
                                indexOfDetection = 0;
                            }
                            _target = targetsInRange.OrderBy(t => Vector3.Distance(eyesCenter.position, t.transform.position)).ToList()[indexOfDetection];
                        }
                        if (_target)
                        {
                            if (detectedTarget.target != _target)
                            {
                                detectedTarget.targetIsLosted = false;
                                detectedTarget.target = _target;
                                currentTimeToLostTarget = timeToLostTarget;
                                HandleLineOfSight();
                                if (detectedTarget.targetIsInSight)
                                {
                                    OnFindTarget();
                                }
                                else
                                {
                                    detectedTarget.Reset();
                                    indexOfDetection++;
                                    return;
                                }
                            }
                            else
                            {
                                indexOfDetection++;
                                return;
                            }
                        }
                        else
                        {
                            indexOfDetection++;
                            return;
                        }
                    }
                    else if (currentTimeToFindTarget > 0)
                    {
                        currentTimeToFindTarget -= Time.deltaTime;
                    }
                }
                else
                {
                    currentTimeToFindTarget = detectionFrequency;
                }

                if (detectedTarget.target)
                {
                    HandleLostTarget();
                }
            }
            else if (detectedTarget.target)
            {
                HandleLostTargetTime();
            }
        }

        /// <summary>
        /// Control when  Lost  a target
        /// </summary>
        protected virtual void HandleLostTarget()
        {
            HandleLineOfSight();
            if (!detectedTarget.targetIsInSight || detectedTarget.target.healthController == null || detectedTarget.target.healthController.isDead)
            {
                HandleLostTargetTime();
            }
            else
            {
                currentTimeToLostTarget = timeToLostTarget;
            }
        }

        /// <summary>
        /// Control the time to lost target
        /// </summary>
        protected virtual void HandleLostTargetTime()
        {
            if (detectedTarget.target.healthController == null || detectedTarget.target.healthController.isDead || (currentTimeToLostTarget <= 0))
            {
                onLostvTarget.Invoke(detectedTarget.target);
                onLostTransformTarget.Invoke(detectedTarget.detectedPoint);
                detectedTarget.Reset();
            }
            else if (currentTimeToLostTarget > 0)
            {
                currentTimeToLostTarget -= Time.deltaTime;
            }
        }

        /// <summary>
        /// Trigger find target events
        /// </summary>
        protected virtual void OnFindTarget()
        {
            currentTimeToTriggerFindTargetEvent = 0;
            StopAllCoroutines();
            StartCoroutine(FindTargetEvent());
        }

        /// <summary>
        /// Routine to trigger find a target events with deleay
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator FindTargetEvent()
        {
            currentTimeToTriggerFindTargetEvent = timeToTriggerFindTargetEvent;
            while (currentTimeToTriggerFindTargetEvent > 0)
            {
                currentTimeToTriggerFindTargetEvent -= Time.deltaTime;
                yield return null;
            }
            if (detectedTarget != null && detectedTarget.target != null)
            {
                onFindTransformTarget.Invoke(detectedTarget.target.center.transform);

                onFindvTarget.Invoke(detectedTarget.target);
            }
        }

        /// <summary>
        /// Control the Line of sight of the <see cref="detectedTarget"/>
        /// </summary>
        protected virtual void HandleLineOfSight()
        {
            if (useDetectionPoints)
            {
                CheckTargetPointsInSight(detectedTarget);
                detectedTarget.targetIsInSight = detectedTarget.detectedPoint && detectedTarget.detectionValue >= requiredDetectionValue;
            }
            else
            {
                detectedTarget.targetIsInSight = IsInFOV(detectedTarget.target.center.transform) && !IsObstructed(detectedTarget.target.center.transform);
                detectedTarget.detectedPoint = detectedTarget.target.center;
            }
        }

        /// <summary>
        /// Check if a transform is in Fild of view 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        protected virtual bool IsInFOV(Transform target)
        {
            var position = target.position;
            targetDirection = (position - eyesCenter.position);
            var localTargetDirectionHorizontal = eyesCenter.InverseTransformDirection(targetDirection.normalized);
            localTargetDirectionHorizontal.y = 0;
            var localTargetDirectionVertical = eyesCenter.InverseTransformDirection(targetDirection.normalized);
            localTargetDirectionVertical.x = 0;
            float magnitude = targetDirection.magnitude;
            var targetDirectionH = eyesCenter.TransformDirection(localTargetDirectionHorizontal);
            var targetDirectionV = eyesCenter.TransformDirection(localTargetDirectionVertical);
            var eyesForward = eyesCenter.forward;
            //Debug.DrawRay(eyesCenter.position, eyesForward * 10);
            var angleHorizontal = Vector3.Angle(eyesForward, targetDirectionH.normalized);
            var angleVertical = Vector3.Angle(eyesForward, targetDirectionV.normalized);
            var isValid = angleHorizontal <= maxAngleHorizontal && angleVertical <= maxAngleVertical;

            if (debugTargetInAngle)
            {
                Debug.Log($"{target.gameObject.name} angle V: {(angleVertical <= maxAngleVertical).ToStringColor() } angle H: {(angleHorizontal <= maxAngleHorizontal).ToStringColor()}", target.gameObject);
            }

            if (debugRays)
            {
                Debug.DrawRay(eyesCenter.position, targetDirectionH.normalized * magnitude, angleHorizontal <= maxAngleHorizontal ? Color.green : Color.blue, 2);
                Debug.DrawRay(eyesCenter.position, targetDirectionV.normalized * magnitude, angleVertical <= maxAngleVertical ? Color.yellow : Color.red, 2);
            }

            return isValid;
        }
        /// <summary>
        /// Set de <see cref="detectedTarget"/> if the target transform has one <see cref="vDetectionTarget"/>
        /// </summary>
        /// <param name="target"></param>
        public void SetTarget(Transform target)
        {
            if (target == null)
            {
                return;
            }

            var _vTarget = target.GetComponent<vDetectionTarget>();
            if (_vTarget && this.detectedTarget.target != _vTarget)
            {
                SetTarget(_vTarget);
            }
        }

        /// <summary>
        /// Set the <seealso cref="detectedTarget"/>
        /// </summary>
        /// <param name="target"></param>
        public void SetTarget(vDetectionTarget target)
        {
            if (target == null)
            {
                return;
            }

            if (target && this.detectedTarget.target != target)
            {
                if (this.detectedTarget.target && !this.detectedTarget.targetIsLosted)
                {
                    onLostTransformTarget.Invoke(this.detectedTarget.target.transform);
                    onLostvTarget.Invoke(this.detectedTarget.target);
                }
                this.detectedTarget.target = target;
                this.detectedTarget.targetIsInSight = true;
                this.detectedTarget.targetIsLosted = false;
                currentTimeToLostTarget = timeToLostTarget;
                onFindTransformTarget.Invoke(target.center.transform);
                onFindvTarget.Invoke(target);
            }
        }

        /// <summary>
        /// Check targets  inside  Collider range
        /// </summary>
        /// <param name="other"></param>
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (this.enabled && targetTag.Contains(other.tag))
            {
                var target = other.GetComponent<vDetectionTarget>();

                if (target && target.IsValid && !targetsInRange.Contains(target))
                {
                    targetsInRange.Add(target);
                }
            }
        }
        /// <summary>
        /// Check targets  outside  Collider range
        /// </summary>
        /// <param name="other"></param>
        protected virtual void OnTriggerExit(Collider other)
        {
            if (this.enabled && targetTag.Contains(other.tag) && targetsInRange.Exists(t => t.gameObject.Equals(other.gameObject)))
            {
                targetsInRange.Remove(targetsInRange.Find(t => t.gameObject.Equals(other.gameObject)));                
            }
        }

        /// <summary>
        /// Check target detection point in line of sight
        /// </summary>
        /// <param name="targetToCheck"></param>
        protected virtual void CheckTargetPointsInSight(DetectedTarget targetToCheck)
        {
            detectedTarget.detectionValue = 0;
            float detectionValue = 0;
            Transform point = null;
            int lastPriority = 0;
            for (int i = 0; i < targetToCheck.target.DetectionPoints.Count; i++)
            {
                var detectionPoint = targetToCheck.target.DetectionPoints[i];
                if (detectionPoint.center)
                {
                    if (IsInFOV(detectionPoint.center) && !IsObstructed(detectionPoint.center) && (int)detectionPoint.targetPriority >= (int)minPriority && (int)detectionPoint.targetPriority <= (int)maxPriority)
                    {
                        if (debugRays)
                        {
                            Debug.Log($"{detectionPoint.center.gameObject.name} was detected ", detectionPoint.center);
                        }
                        detectionValue += detectionPoint.value;
                        if (lastPriority < (int)detectionPoint.targetPriority)
                        {
                            point = detectionPoint.center;
                            targetToCheck.detectedPoint = point;
                            lastPriority = (int)detectionPoint.targetPriority;
                        }
                        if ((detectionValue - targetToCheck.target.DetectionReductionValue) >= requiredDetectionValue)
                        {
                            break;
                        }
                    }
                }
            }

            detectedTarget.detectionValue = detectionValue - targetToCheck.target.DetectionReductionValue;
            detectedTarget.detectedPoint = point;
        }

        /// <summary>
        /// Check if someone is obstructing the line of sight of a transform
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        protected virtual bool IsObstructed(Transform target)
        {
            if (!useObstacles)
            {
                return false;
            }

            var _target = target.position;
            var obstructed = Physics.Linecast(eyesCenter.position, _target, out hit, obstacles);
            if (obstructed && debugTargetObstacle)
            {
                Debug.Log($"{target.gameObject.name} is Obstructed by {hit.collider.gameObject.name}", hit.collider.gameObject);
            }

            return obstructed;
        }
    }
}