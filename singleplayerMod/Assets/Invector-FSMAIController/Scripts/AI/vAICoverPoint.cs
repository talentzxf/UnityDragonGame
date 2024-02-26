using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace Invector.vCharacterController.AI
{
    [SelectionBase]
    [RequireComponent(typeof(BoxCollider))]
    [vClassHeader("AI Cover Point", openClose = false)]
    public class vAICoverPoint : vMonoBehaviour
    {
       
        [System.Serializable]        
        public class CoverEvent : UnityEngine.Events.UnityEvent<vAICoverPoint,GameObject> {   };

        [Tooltip("Auto assign the properties : Corner enum, LeftCorner and RightCorner ")]
        public bool autoDetectCorner = true;
        public LayerMask mask = 1 << 0;
        [System.Flags]
        public enum Corner
        {
            Left =1,Right=2
        }
        [vEnumFlag]
        public Corner corner ;
        public float offsetPosePositionX;
        public float rayCastNeighborOffsetX =.15f;
        public vAICoverPoint leftCorner, rightCorner;
        public vAICoverPoint left, right;
        public CoverEvent onEnterCover;
        public bool showRays;
        RaycastHit hit;
        float space => ((boxCollider.size.z ));
        PhysicsScene physics;
        public bool isValid;
     
        private void Awake()
        {
            if (boxCollider) boxCollider.isTrigger = true;
        }

        private IEnumerator Start()
        {
            gameObject.SetActive(isValid =CheckPosePositionInNavMesh());
            yield return new WaitForEndOfFrame();           
            CheckConnections();
            
        }
        public virtual bool CheckPosePositionInNavMesh()
        {
            NavMeshHit hit;
            return NavMesh.SamplePosition(posePosition, out hit, .1f, NavMesh.AllAreas);
        }
        public void CheckConnections()
        {
            if (autoDetectCorner) corner = (Corner)(0);
            mask = 1 << 0 | 1 << gameObject.layer;
            if (gameObject.scene == null) return;
            physics =  PhysicsSceneExtensions.GetPhysicsScene(gameObject.scene);
            CheckLeftConnection();
          
            CheckRightConnection();
        }

        private void CheckRightConnection()
        {
            bool isRight = false;
            vAICoverPoint _possibleRight = null;
           
            if (RayCastConnections(transform.position + transform.TransformDirection(boxCollider.center) + transform.right*boxCollider.size.x* rayCastNeighborOffsetX , -transform.right, out hit, boxCollider.size.x * (1.1f+rayCastNeighborOffsetX ), mask))
            {
                _possibleRight = hit.transform.gameObject.GetComponent<vAICoverPoint>();               
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    var dir = Quaternion.AngleAxis((i * 15), Vector3.up) * -transform.right;
                    var rayOrigin = boxCollider.bounds.center - transform.right * boxCollider.size.x * 0.45f;

                    if (RayCastConnections(rayOrigin, dir, out hit, space, mask))
                    {
                        _possibleRight = hit.transform.gameObject.GetComponent<vAICoverPoint>();
                        if (showRays) Debug.DrawLine(rayOrigin, hit.point, Color.red);
                        if (_possibleRight) break;
                    }
                    else if (showRays) Debug.DrawRay(rayOrigin, dir * space, Color.green);

                }
                if (!_possibleRight)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        var dir = Quaternion.AngleAxis(-(i * 15), Vector3.up) * -transform.right;
                        var rayOrigin = boxCollider.bounds.center - transform.right * boxCollider.size.x * 0.45f;
                       

                        if (RayCastConnections(rayOrigin, dir, out hit, space, mask))
                        {
                            _possibleRight = hit.transform.gameObject.GetComponent<vAICoverPoint>();
                            if (showRays) Debug.DrawLine(rayOrigin, hit.point, Color.red);
                            if (_possibleRight) break;
                        }
                        else if (showRays) Debug.DrawRay(rayOrigin, dir * space, Color.yellow);

                    }
                }
            }
            if (_possibleRight)
            {
                var angle = (_possibleRight.transform.eulerAngles - transform.eulerAngles).NormalizeAngle();
               
                right = angle.y > -75? _possibleRight : null;
                isRight = right == null;
                rightCorner = right == null ? _possibleRight : null;

            }
            else
            {
                right = null;
                rightCorner = null;
                isRight = true;
            }
            if (isRight && autoDetectCorner) corner |= Corner.Right;
        }

        private void CheckLeftConnection()
        {
            bool isLeft = false;
            vAICoverPoint _possibleLeft = null;
           
            if (RayCastConnections(transform.position+transform.TransformDirection(boxCollider.center) - transform.right * boxCollider.size.x * rayCastNeighborOffsetX , transform.right, out hit, boxCollider.size.x * (1.1f + rayCastNeighborOffsetX ), mask))
            {
                _possibleLeft = hit.transform.gameObject.GetComponent<vAICoverPoint>();
               
            }
            else
            {

                for (int i = 0; i < 6; i++)
                {

                    var dir = Quaternion.AngleAxis(-(i * 15), Vector3.up) * transform.right;
                    var rayOrigin = boxCollider.bounds.center + transform.right * boxCollider.size.x * 0.45f;
                   
                    if (RayCastConnections(rayOrigin, dir, out hit, space, mask))
                    {
                        _possibleLeft = hit.transform.gameObject.GetComponent<vAICoverPoint>();
                        if (showRays) Debug.DrawLine(rayOrigin, hit.point, Color.red);
                        if (_possibleLeft) break;
                    }
                    else if (showRays) Debug.DrawRay(rayOrigin, dir*space, Color.green);

                }
                if (!_possibleLeft)
                {
                    for (int i = 0; i < 6; i++)
                    {

                        var dir = Quaternion.AngleAxis((i * 15), Vector3.up) * transform.right;
                        var rayOrigin = boxCollider.bounds.center + transform.right * boxCollider.size.x * 0.45f;
                
                        if (RayCastConnections(rayOrigin, dir, out hit, space, mask))
                        {
                            _possibleLeft = hit.transform.gameObject.GetComponent<vAICoverPoint>();
                            if (showRays) Debug.DrawLine(rayOrigin, hit.point, Color.red);
                            if (_possibleLeft) break;
                        }
                        else if (showRays) Debug.DrawRay(rayOrigin, dir * space, Color.yellow);

                    }
                }
            }

            if (_possibleLeft)
            {
                var angle = (_possibleLeft.transform.eulerAngles - transform.eulerAngles).NormalizeAngle();              
                left = angle.y < 75 ? _possibleLeft : null;
                isLeft = left == null;
                leftCorner = left == null ? _possibleLeft : null;

            }
            else
            {
                left = null;
                leftCorner = null;
                isLeft = true;
            }
            if (isLeft && autoDetectCorner) corner |= Corner.Left;
        }

      
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) isValid = true;
            CheckConnections();           
            var matrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(transform.position + transform.TransformDirection(boxCollider.center) + transform.forward * posePositionZ * 0.45f, transform.rotation, new Vector3(boxCollider.BoxSize().x, boxCollider.BoxSize().y, 0.01f));
            Gizmos.color = isValid? Color.white * 0.8f:Color.red*0.8f;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(1, 1, 0));
            Gizmos.color = isValid ? Color.white * 0.25f : Color.red * 0.25f;
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
            Gizmos.matrix = matrix;

        }


        private void OnDrawGizmosSelected()
        {
            
        }

        protected virtual bool RayCastConnections(Vector3 rayOrigin,Vector3 dir,out RaycastHit hit,float distance,LayerMask mask)
        {
            if(Application.isPlaying)
            {
                return Physics.Raycast(rayOrigin, dir, out hit, distance, mask, QueryTriggerInteraction.Collide);
            }
            else
            {
                return physics.Raycast(rayOrigin, dir, out hit, distance, mask, QueryTriggerInteraction.Collide);
            }
        }

        public float posePositionZ => boxCollider? boxCollider.size.z:0f;

        public BoxCollider boxCollider
        {
            get
            {
                if (_boxCollider == null) _boxCollider = GetComponent<BoxCollider>();
                if (_boxCollider == null) _boxCollider = gameObject.AddComponent<BoxCollider>();
                return _boxCollider;
            }
            set
            {
                _boxCollider = value;

            }

        }

        protected BoxCollider _boxCollider;

        public Vector3 posePosition
        {
            get
            {
                return transform.position + transform.forward * posePositionZ + transform.right*offsetPosePositionX;
            }
        }
   
        public bool isOccuped;       
        
        public Vector3 rightCornerP
        {
            get
            {
                var corners = new Vector3[2];
                var p = posePosition;
                corners[0] = p - transform.right * (boxCollider.size.z * 1.5f);
                corners[1] = rightCorner? rightCorner.posePosition + rightCorner.transform.right * (rightCorner.boxCollider.size.z * 1.5f):corners[0];

                return (corners[0]+ corners[1])/2;
            }
        }

        public Vector3 leftCornerP
        {
            get
            {
                var corners = new Vector3[2];
                var p = posePosition;
                corners[0] = p + transform.right * (boxCollider.size.z * 1.5f);
                corners[1] = leftCorner ? leftCorner.posePosition - leftCorner.transform.right * (leftCorner.boxCollider.size.z * 1.5f) : corners[0];

                return (corners[0] + corners[1]) / 2;
            }
        }

        public void EnterCover(GameObject visitor)
        {
            onEnterCover.Invoke(this, visitor);
        }
    }
}