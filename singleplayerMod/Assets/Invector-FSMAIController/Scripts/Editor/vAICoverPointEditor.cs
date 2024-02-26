using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Invector.vCharacterController.AI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(vAICoverPoint),true)]
    public class vAICoverPointEditor : vEditorBase
    {
        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
        static void DrawHandles(vAICoverPoint coverPoint, GizmoType gizmoType)
        {
            if (!coverPoint) return;
            Transform transform = coverPoint.transform;
            BoxCollider boxCollider = coverPoint.boxCollider;
            float posePositionZ = coverPoint.posePositionZ;
            bool selected = gizmoType == (GizmoType.Active | GizmoType.InSelectionHierarchy | GizmoType.Selected);
            vAICoverPoint left = coverPoint.left;
            vAICoverPoint leftCorner = coverPoint.leftCorner;
            vAICoverPoint right = coverPoint.right;
            vAICoverPoint rightCorner = coverPoint.rightCorner;
            Vector3 posePosition = coverPoint.posePosition;
            Vector3 leftCornerP = coverPoint.leftCornerP;
            Vector3 rightCornerP = coverPoint.rightCornerP;           
           
            if (left)
            {
                Handles.color = Color.green * 0.5f;
                Vector3 normal = posePosition - left.posePosition;
                normal.y = 0;
                Handles.DrawAAPolyLine(EditorGUIUtility.whiteTexture,5, posePosition, (posePosition + left.posePosition) * 0.5f);
                Handles.DrawSolidArc(posePosition,Vector3.up,Quaternion.AngleAxis(90f,Vector3.up)*normal.normalized,180, 0.1f);
                Handles.color = Color.green;
                Handles.DrawWireArc(posePosition, Vector3.up, Quaternion.AngleAxis(90f, Vector3.up) * normal.normalized, 180, 0.1f);
            }

           
            if (right)
            {
                Handles.color = Color.green * 0.5f;
                Vector3 normal = posePosition - right.posePosition;
                normal.y = 0;
                Handles.DrawAAPolyLine(EditorGUIUtility.whiteTexture, 5, posePosition, (posePosition + right.posePosition) * 0.5f);
                Handles.DrawSolidArc(posePosition, Vector3.up, Quaternion.AngleAxis(90f, Vector3.up) * normal.normalized, 180, 0.1f);
                Handles.color = Color.green;
                Handles.DrawWireArc(posePosition, Vector3.up, Quaternion.AngleAxis(90f, Vector3.up) * normal.normalized, 180, 0.1f);
            }
          

            if (leftCorner)
            {
                Handles.color = Color.green * 0.5f;
                Vector3 normal = posePosition - leftCornerP;
                normal.y = 0;
                Handles.DrawAAPolyLine(EditorGUIUtility.whiteTexture, 5, posePosition, leftCornerP);
              
                Handles.DrawSolidArc(posePosition, Vector3.up, Quaternion.AngleAxis(90f, Vector3.up) * normal.normalized, 180, 0.1f);
                Handles.color = Color.green;
                Handles.DrawWireArc(posePosition, Vector3.up, Quaternion.AngleAxis(90f, Vector3.up) * normal.normalized, 180, 0.1f);

                Handles.color = Color.yellow*0.5f;
                Handles.DrawSolidArc(leftCornerP, Vector3.up, Quaternion.AngleAxis(90f, Vector3.up) * -normal.normalized, 180, 0.1f);
                Handles.color = Color.yellow;
                Handles.DrawWireArc(leftCornerP, Vector3.up, Quaternion.AngleAxis(90f, Vector3.up) * -normal.normalized, 180, 0.1f);

            }
           
            if (rightCorner)
            {
                Handles.color = Color.green * 0.5f;
                Vector3 normal = posePosition - rightCornerP;
                normal.y = 0;
                Handles.DrawAAPolyLine(EditorGUIUtility.whiteTexture, 5, posePosition, rightCornerP);
              
                Handles.DrawSolidArc(posePosition, Vector3.up, Quaternion.AngleAxis(90f, Vector3.up) * normal.normalized, 180, 0.1f);
                Handles.color = Color.green;
                Handles.DrawWireArc(posePosition, Vector3.up, Quaternion.AngleAxis(90f, Vector3.up) * normal.normalized, 180, 0.1f);

                Handles.color = Color.yellow*0.5f;
                Handles.DrawSolidArc(rightCornerP, Vector3.up, Quaternion.AngleAxis(90f, Vector3.up) * -normal.normalized, 180, 0.1f);
                Handles.color = Color.yellow;
                Handles.DrawWireArc(rightCornerP, Vector3.up, Quaternion.AngleAxis(90f, Vector3.up) * -normal.normalized, 180, 0.1f);
            }


            
        }

        private void OnSceneGUI()
        {
            vAICoverPoint coverPoint = (target) as vAICoverPoint;
            Transform transform = coverPoint.transform;
            Vector3 posePosition = coverPoint.posePosition;
            float size = .03f;
            float snap = 0.05f;
            Handles.color = Color.yellow * 0.8f;
            EditorGUI.BeginChangeCheck();
            Handles.ConeHandleCap(0, posePosition - transform.right * 0.04f, Quaternion.LookRotation(-transform.right), 0.02f, EventType.Repaint);
            Handles.ConeHandleCap(0, posePosition + transform.right * 0.04f, Quaternion.LookRotation(transform.right), 0.02f, EventType.Repaint);
            Vector3 newTargetPosition = Handles.Slider(posePosition, -transform.right, size, Handles.SphereHandleCap, snap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(coverPoint, "PosePosition");
                coverPoint.offsetPosePositionX = transform.InverseTransformPoint(newTargetPosition).x;
                coverPoint.offsetPosePositionX = Mathf.Clamp(coverPoint.offsetPosePositionX,-(coverPoint.boxCollider.BoxSize().x * 0.6f), coverPoint.boxCollider.BoxSize().x*0.6f);
            }
            Vector3 pointR = transform.position + transform.TransformDirection(coverPoint.boxCollider.center) + transform.right  * coverPoint.rayCastNeighborOffsetX +transform.forward*0.01f;
            Vector3 pointL = transform.position + transform.TransformDirection(coverPoint.boxCollider.center) - transform.right  * coverPoint.rayCastNeighborOffsetX  - transform.forward * 0.01f; ;
            float distance = coverPoint.boxCollider.size.x * (1.1f + coverPoint.rayCastNeighborOffsetX );
            Handles.color = coverPoint.right?  Color.green * 0.8f:Color.grey;
            Handles.SphereHandleCap(0, pointR, Quaternion.identity, 0.02f, EventType.Repaint); 
            Handles.ConeHandleCap(0, pointR - transform.right * (distance-0.025f), Quaternion.LookRotation(-transform.right), 0.05f, EventType.Repaint);
            Handles.DrawLine(pointR, pointR - transform.right * distance);
            Handles.color = coverPoint.left ? Color.green * 0.8f : Color.grey;
            Handles.SphereHandleCap(0, pointL, Quaternion.identity, 0.02f, EventType.Repaint);
            Handles.ConeHandleCap(0, pointL + transform.right * (distance - 0.025f), Quaternion.LookRotation(transform.right), 0.05f, EventType.Repaint);
            Handles.DrawLine(pointL, pointL + transform.right * distance);


            EditorGUI.BeginChangeCheck();
            Handles.ConeHandleCap(0, pointR - transform.right * 0.04f, Quaternion.LookRotation(-transform.right), 0.02f, EventType.Repaint);
            Handles.ConeHandleCap(0, pointR + transform.right * 0.04f, Quaternion.LookRotation(transform.right), 0.02f, EventType.Repaint);
            Vector3 newPointR = Handles.Slider(pointR, -transform.right, size, Handles.SphereHandleCap, snap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(coverPoint, "PosePosition");
                coverPoint.rayCastNeighborOffsetX  = transform.InverseTransformPoint(newPointR).x;
                coverPoint.rayCastNeighborOffsetX  = Mathf.Clamp(coverPoint.rayCastNeighborOffsetX , -(coverPoint.boxCollider.BoxSize().x * 0.4f), coverPoint.boxCollider.BoxSize().x * 0.4f);
            }


            EditorGUI.BeginChangeCheck();
            Handles.ConeHandleCap(0, pointL - transform.right * 0.04f, Quaternion.LookRotation(-transform.right), 0.02f, EventType.Repaint);
            Handles.ConeHandleCap(0, pointL + transform.right * 0.04f, Quaternion.LookRotation(transform.right), 0.02f, EventType.Repaint);
            Vector3 newPointL = Handles.Slider(pointL, transform.right, size, Handles.SphereHandleCap, snap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(coverPoint, "PosePosition");
                coverPoint.rayCastNeighborOffsetX  = -transform.InverseTransformPoint(newPointL).x;
                coverPoint.rayCastNeighborOffsetX  = Mathf.Clamp(coverPoint.rayCastNeighborOffsetX , -(coverPoint.boxCollider.BoxSize().x * 0.6f), coverPoint.boxCollider.BoxSize().x * 0.6f);
            }
        }
    }
}