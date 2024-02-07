using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class LineGemGenerator : AbstractGemGenerator
{
#if UNITY_EDITOR
    [CustomEditor(typeof(LineGemGenerator))]
    class LineGemGeneratorEditor: GemGeneratorEditor{}

    [SerializeField] private float lineLength = 5.0f;
    [SerializeField] private float interval = 1.0f;

    [SerializeField] private float horizontalAngle = 0.0f;
    [SerializeField] private float verticalAngle = 0.0f;

    private Vector3 dir = Vector3.forward;
    
    public override void GenerateGems()
    {
        float curLength = 0.0f;
        while(curLength < lineLength)
        {
            GenerateGemAtPosition(transform.position + dir.normalized * curLength);
            curLength += interval;
        }
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.blue;

        Quaternion horizonalRotation = Quaternion.Euler(0f, horizontalAngle, 0f);
        Quaternion verticalRotation = Quaternion.Euler(-verticalAngle, 0f, 0f);

        Quaternion finalRotation = horizonalRotation * verticalRotation;
        dir = finalRotation * Vector3.forward;
        Handles.DrawAAPolyLine(5f, transform.position, transform.position + dir.normalized * lineLength);
    }
#endif
}
