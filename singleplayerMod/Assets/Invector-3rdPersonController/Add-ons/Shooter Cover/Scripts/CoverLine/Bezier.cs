using System.Collections.Generic;
using UnityEngine;

public static class Bezier
{

    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * p0 +
            2f * oneMinusT * t * p1 +
            t * t * p2;
    }

    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return
            2f * (1f - t) * (p1 - p0) +
            2f * t * (p2 - p1);
    }

    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float OneMinusT = 1f - t;
        return
            OneMinusT * OneMinusT * OneMinusT * p0 +
            3f * OneMinusT * OneMinusT * t * p1 +
            3f * OneMinusT * t * t * p2 +
            t * t * t * p3;
    }

    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            3f * oneMinusT * oneMinusT * (p1 - p0) +
            6f * oneMinusT * t * (p2 - p1) +
            3f * t * t * (p3 - p2);
    }
    public static Vector3 CalculateBezierPoint(Vector3 startPoint, Vector3 startTangent, Vector3 endTangent, Vector3 endPoint, float t)
    {
        return (((-startPoint + 3 * (startTangent - endTangent) + endPoint) * t + (3 * (startPoint + endTangent) - 6 * startTangent)) * t + 3 * (startTangent - startPoint)) * t + startPoint;
    }  
 
   
    public static Vector3[] GetPoints(Vector3 startPoint, Vector3 startTangent, Vector3 endTangent, Vector3 endPoint, float space, float accuracy)
    {
        Vector3 last_spawn = startPoint;
        Vector3 last_spawn1 = last_spawn;
        List<Vector3> allPoints = new List<Vector3>();
        allPoints.Add(last_spawn);
        float lenght = 0;
        for (float t = accuracy; t <= 1.0f; t += accuracy)
        {
            Vector3 trial = Bezier.GetPoint(startPoint, startTangent, endTangent, endPoint, t);
            lenght += Vector3.Distance(trial, last_spawn1);
            last_spawn1 = trial;
        }
        lenght += Vector3.Distance(endPoint, last_spawn1);
        int countF = Mathf.FloorToInt(lenght / space);

        var count = (lenght / space);
        if (count - ((int)count) > 0.5f) count = (int)count + 1;
        else
            count = (int)count;
        var _distance = lenght / count;

        for (float t = 0f ; t <= 1.0f; t += accuracy)
        {
            Vector3 trial = Bezier.GetPoint(startPoint, startTangent, endTangent, endPoint, t);
            if (Vector3.Distance(trial, last_spawn) >= _distance)
            {
                last_spawn =last_spawn + (trial-last_spawn).normalized*_distance;
                allPoints.Add(last_spawn);
            }
        }
      
        return allPoints.ToArray();
        
    }
  
}