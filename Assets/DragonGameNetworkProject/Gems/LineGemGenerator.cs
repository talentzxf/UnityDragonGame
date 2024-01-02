using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LineGemGenerator : AbstractGemGenerator
{
    [CustomEditor(typeof(LineGemGenerator))]
    class LineGemGeneratorEditor: GemGeneratorEditor{}

    [SerializeField] private float length = 5.0f;
    [SerializeField] private float interval = 1.0f;
    [SerializeField] private Vector3 dir = Vector3.forward;

    public override void GenerateGems()
    {
        
    }
    
    private Vector3 endPoint = Vector3.forward; // 初始方向设为Z轴方向
    private bool isDragging = false;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        // 绘制可调整方向的线条
        Gizmos.DrawLine(transform.position, transform.position + endPoint);

        // 检测鼠标事件
        Event currentEvent = Event.current;
        if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
            Plane plane = new Plane(transform.forward, transform.position);

            float distance;
            if (plane.Raycast(ray, out distance))
            {
                endPoint = ray.GetPoint(distance) - transform.position;
                endPoint = Vector3.ProjectOnPlane(endPoint, transform.forward).normalized;
            }
        }
    }
}
