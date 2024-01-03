using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

public class SplineGemGenerator : AbstractGemGenerator
{
#if UNITY_EDITOR
    [CustomEditor(typeof(SplineGemGenerator))]
    class BezierGemGeneratorEditor : GemGeneratorEditor
    {
    }

    [SerializeField] private int controlPointCount = 5;
    [SerializeField] private float interval = 1.0f;

    private static readonly string CONTROL_PREFIX = "Control_";
    
    private float controlPointRange = 5;
    [SerializeField] private List<Transform> controlPoints = new();

    protected override bool CanDelete(GameObject go)
    {
        if (go.name.StartsWith(CONTROL_PREFIX))
        {
            return false;
        }

        return true;
    }
    
    private void GenerateControlPoints()
    {
        // Remove all invalid points.
        controlPoints.RemoveAll(item => item == null);

        for (int i = 0; i < controlPoints.Count; i++)
        {
            controlPoints[i].name = CONTROL_PREFIX + i;
        }

        while (controlPoints.Count < controlPointCount)
        {
            GameObject newGO = new GameObject(CONTROL_PREFIX + controlPoints.Count);
            newGO.transform.parent = transform;
            newGO.transform.localPosition = Random.insideUnitSphere * controlPointRange;
            controlPoints.Add(newGO.transform);
        }

        while (controlPoints.Count > controlPointCount)
        {
            controlPoints.RemoveAt(controlPoints.Count - 1);
        }
    }

    private void OnDrawGizmos()
    {
        GenerateControlPoints();

        Handles.color = Color.red;
        for (int i = 0; i < controlPoints.Count - 1; i++)
        {
            Vector3 p0 = controlPoints[Mathf.Max(i - 1, 0)].position;
            Vector3 p1 = controlPoints[i].position;
            Vector3 p2 = controlPoints[i + 1].position;
            Vector3 p3 = controlPoints[Mathf.Min(i + 2, controlPoints.Count - 1)].position;

            Handles.DrawAAPolyLine(5f, CalculateSplinePoints(p0, p1, p2, p3));
        }
    }

    Vector3[] CalculateSplinePoints(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3[] splinePoints = new Vector3[10];

        for (int i = 0; i < splinePoints.Length; i++)
        {
            float t = i / (float) (splinePoints.Length - 1);
            splinePoints[i] = CalculateCatmullRomPoint(t, p0, p1, p2, p3);
        }

        return splinePoints;
    }

    Vector3 CalculateCatmullRomPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float tt = t * t;
        float ttt = tt * t;

        Vector3 point =
            0.5f * ((2.0f * p1) +
                    (-p0 + p2) * t +
                    (2.0f * p0 - 5.0f * p1 + 4.0f * p2 - p3) * tt +
                    (-p0 + 3.0f * p1 - 3.0f * p2 + p3) * ttt);

        return point;
    }
    
    void SampleCurvePoints(Vector3[] curvePoints, Action<Vector3> processor)
    {
        float segmentStartDistance = 0f;
        float distance = 0.0f;
        
        for (int i = 0; i < curvePoints.Length - 1; i++)
        {
            float segmentLength = Vector3.Distance(curvePoints[i], curvePoints[i + 1]);

            while (segmentStartDistance + segmentLength >= distance)
            {
                float ratio = (distance - segmentStartDistance) / segmentLength;
                Vector3 position = Vector3.Lerp(curvePoints[i], curvePoints[i + 1], ratio);
                processor(position);
                distance += interval;
            }
            
            segmentStartDistance += segmentLength;
        }
    }
    
    public override void GenerateGems()
    {
        float calculatedLength = 0.0f;

        Vector3[] points = new Vector3[0];

        for (int i = 0; i < controlPoints.Count - 1; i++)
        {
            Vector3 p0 = controlPoints[Mathf.Max(i - 1, 0)].position;
            Vector3 p1 = controlPoints[i].position;
            Vector3 p2 = controlPoints[i + 1].position;
            Vector3 p3 = controlPoints[Mathf.Min(i + 2, controlPoints.Count - 1)].position;

            points = points.Concat(CalculateSplinePoints(p0, p1, p2, p3)).ToArray();
        }
        
        SampleCurvePoints(points, position =>
        {
            GenerateGemAtPosition(position);
        });
    }
#endif
}