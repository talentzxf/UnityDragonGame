using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Invector;
[CustomEditor(typeof(vCoverLine)),CanEditMultipleObjects]
public class vCoverLineEditor : Editor
{
  
    public enum SelectionState
    {
        Left, Right, Center, None
    }
    Tool LastTool;
    SelectionState selectionState;
    public vCoverLinePoint currentPoint;
    vCoverLine coverLine;
    void OnEnable()
    {
        coverLine = (vCoverLine)target;
        LastTool = Tools.current;
        Tools.current = Tool.None;
        selectionState = SelectionState.None;
       
    }   
    Vector3[] segments = new Vector3[0];
    List<vCoverLinePoint> points = new List<vCoverLinePoint>();

    [MenuItem("GameObject/Invector/Cover/New Cover Line", false, -100)]
    [MenuItem("Invector/Cover Add-On/Create New Cover Line")]
    static void CreateCoverLine()
    {
        var cA = new GameObject("Cover Line", typeof(vCoverLine));
      

        SceneView view = SceneView.lastActiveSceneView;
        if (SceneView.lastActiveSceneView == null)
            throw new UnityException("The Scene View can't be access");

        Vector3 spawnPos = view.camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 5f));
        if (Selection.activeGameObject)
        {
            cA.transform.parent = Selection.activeGameObject.transform;
            spawnPos = Selection.activeGameObject.transform.position;
        }
        cA.transform.position = spawnPos;
        
        Selection.activeGameObject = cA.gameObject;
    }

    void OnDisable()
    {
        Tools.current = LastTool;
       
    }
    private void OnSceneGUI()
    {     
        var spawnPoints = new List<Vector3>();
        points = coverLine.CtrlPTransform.GetComponentsInChildren<vCoverLinePoint>().vToList();
        var e = Event.current;
        var positions = new List<Vector3>();
        var center = Vector3.zero;
       
        for (int i = 0; i < points.Count; i++)
        {           
            var point = points[i];
            if(i+1<points.Count)
            {
                point.conectedRight = points[i+1];
            }
            if(i-1>=0)
            {              
                point.conectedLeft = points[i-1];
            }
          
            var tangentLeft = point.transform.TransformPoint(point.tangentLeft);
            var tangentRight = point.transform.TransformPoint(point.tangentRight);           
          
            if (point.conectedLeft && point.curveType!= vCoverLine.CurveType.Linear && point.curveType!= vCoverLine.CurveType.LinearLeft)
            {
                Handles.color = Color.green;
                Handles.DrawLine(tangentLeft, point.transform.position);
                if (e.shift || selectionState == SelectionState.Left && currentPoint == point)
                {
                    EditorGUI.BeginChangeCheck();
                    if ((e.shift && !e.control))
                        { var fmh_87_75_638445639729440474 = Quaternion.identity; tangentLeft = Handles.FreeMoveHandle(tangentLeft, HandleUtility.GetHandleSize(tangentLeft) * 0.15f, Vector3.zero, Handles.RectangleHandleCap); }
                    else
                    {
                        Handles.CubeHandleCap(0, tangentLeft, Quaternion.identity, HandleUtility.GetHandleSize(tangentLeft) * 0.2f, EventType.Repaint);
                        tangentLeft = Handles.DoPositionHandle(tangentLeft, Quaternion.identity);
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        CalculateTangetByMode(ref tangentLeft, ref tangentRight, point.transform.position, point.curveType);
                        Undo.RecordObject(point, "Change Tangent");
                        point.tangentRight = point.transform.InverseTransformPoint(tangentRight);
                        point.tangentLeft = point.transform.InverseTransformPoint(tangentLeft);
                        EditorUtility.SetDirty(point);
                    }
                }
                else
                {
                    if (Handles.Button(tangentLeft, Quaternion.identity, HandleUtility.GetHandleSize(tangentLeft) * 0.1f, HandleUtility.GetHandleSize(tangentLeft) * 0.1f, Handles.CubeHandleCap))
                    {
                        selectionState = SelectionState.Left;
                        currentPoint = point;
                    }
                }
                Handles.color = Color.white;
            }
            if (point.conectedRight && point.curveType != vCoverLine.CurveType.Linear && point.curveType != vCoverLine.CurveType.LinearRight)
            {
                Handles.color = Color.blue;
                Handles.DrawLine(tangentRight, point.transform.position);

                if ((e.shift && !e.control) || selectionState == SelectionState.Right && currentPoint == point)
                {
                    EditorGUI.BeginChangeCheck();
                    if ((e.shift && !e.control))
                        { var fmh_121_77_638445639729475484 = Quaternion.identity; tangentRight = Handles.FreeMoveHandle(tangentRight, HandleUtility.GetHandleSize(tangentRight) * 0.15f, Vector3.zero, Handles.RectangleHandleCap); }
                    else
                    {
                        Handles.CubeHandleCap(0, tangentRight, Quaternion.identity, HandleUtility.GetHandleSize(tangentRight) * 0.2f, EventType.Repaint);
                        tangentRight = Handles.DoPositionHandle(tangentRight, Quaternion.identity);
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        CalculateTangetByMode(ref tangentRight, ref tangentLeft, point.transform.position, point.curveType);
                        Undo.RecordObject(point, "Change Tangent");
                        point.tangentLeft = point.transform.InverseTransformPoint(tangentLeft);
                        point.tangentRight = point.transform.InverseTransformPoint(tangentRight);
                        EditorUtility.SetDirty(point);
                    }
                }
                else
                {
                    if (Handles.Button(tangentRight, Quaternion.identity, HandleUtility.GetHandleSize(tangentRight) * 0.1f, HandleUtility.GetHandleSize(tangentRight) * 0.1f, Handles.CubeHandleCap))
                    {
                        selectionState = SelectionState.Right;
                        currentPoint = point;
                    }
                }
                Handles.color = Color.white;
            }
            if (point.conectedRight)
            {
                var p1 = point.transform.position;
                var p2 = point.conectedRight.transform.position;

                var t2 = point.conectedRight.transform.TransformPoint(point.conectedRight.tangentLeft);
                var t3 = point.conectedRight.transform.TransformPoint(point.conectedRight.tangentRight);

                float t = point.tangentsLenght > 1 ? point.tangentsLenght : 1f;
                segments = Bezier.GetPoints(p1, tangentRight, t2, p2, coverLine.width, 0.01f/   t);
               

                for (int a = 0; a < segments.Length; a++)
                {
                   
                    if (spawnPoints.Count==0||(spawnPoints[spawnPoints.Count-1]-segments[a]).magnitude>coverLine.width*0.5f)
                    {
                        spawnPoints.Add(segments[a]);                      
                    }
                                 
                }             
            }



            var position = point.transform.position;
            var rotation = point.transform.rotation;
            Handles.color = Color.red;
            if ((e.shift && !e.control)  || selectionState == SelectionState.Center && currentPoint == point)
            {
                EditorGUI.BeginChangeCheck();
                if ((e.shift && !e.control))
                {
                    var fmh_179_65_638445639729484358 = Quaternion.identity; position = Handles.FreeMoveHandle(position, HandleUtility.GetHandleSize(position) * 0.15f, Vector3.zero, Handles.RectangleHandleCap);
                }

                else
                {
                    Handles.CubeHandleCap(0, position, Quaternion.identity, HandleUtility.GetHandleSize(position) * 0.2f, EventType.Repaint);
                    position = Handles.PositionHandle(position, Quaternion.identity);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(point.transform, "Change Position");                  
                    point.transform.position = position;
                    EditorUtility.SetDirty(point);

                }
            }
            else
            {
                if (Handles.Button(position, Quaternion.identity, HandleUtility.GetHandleSize(position) * 0.1f, HandleUtility.GetHandleSize(position) * 0.1f, Handles.CubeHandleCap))
                {
                    selectionState = SelectionState.Center;
                    currentPoint = point;
                }
            }
            Handles.color = Color.white;
            switch (point.curveType)
            {
                case vCoverLine.CurveType.Linear:
                    if (point.conectedRight)
                    {
                        point.tangentRight = point.transform.InverseTransformDirection((point.conectedRight.transform.position - point.transform.position) * 0.5f);
                    }
                    if (point.conectedLeft)
                    {
                        point.tangentLeft = point.transform.InverseTransformDirection((point.conectedLeft.transform.position - point.transform.position) * 0.5f);
                    }
                    break;
                case vCoverLine.CurveType.LinearRight:
                    if (point.conectedRight)
                    {
                        point.tangentRight =point.transform.InverseTransformDirection((point.conectedRight.transform.position - point.transform.position) * 0.5f);
                    }

                    break;
                case vCoverLine.CurveType.LinearLeft:
                    if (point.conectedLeft)
                    {
                        point.tangentLeft = point.transform.InverseTransformDirection((point.conectedLeft.transform.position - point.transform.position) * 0.5f);
                    }
                    break;
            }
            center += points[i].transform.position;
            positions.Add(points[i].transform.position);
            string pointName = $"P-{i+1}";
            point.gameObject.name = pointName;
        }

        if (coverLine.close && points.Count > 1)
        {
            bool needToApply = false;
            if (points[0].conectedLeft != points[points.Count - 1])
            {
                points[0].conectedLeft = points[points.Count - 1];
                needToApply = true;

            }
            if (points[points.Count - 1].conectedRight != points[0])
            {
                points[points.Count - 1].conectedRight = points[0];
                needToApply = true;
            }
            if (needToApply)
            {                
                Repaint();
                EditorUtility.SetDirty(coverLine);
            }

        }
        else if (points.Count > 1)
        {
            bool needToApply = false;
            if (points[0].conectedLeft != null)
            {
                points[0].conectedLeft = null;
                needToApply = true;

            }
            if (points[points.Count - 1].conectedRight != null)
            {
                points[points.Count - 1].conectedRight = null;
                needToApply = true;
            }
            if (needToApply)
            {
                EditorUtility.SetDirty(coverLine);
                Repaint();
            }
        }
        if (points.Count > 0)
        {
            center = center / positions.Count;
            if (coverLine.transform.position != center)
            {
                Undo.RecordObject(coverLine.transform, "Change Position");
                coverLine.transform.position = center;
                for (int i = 0; i < positions.Count; i++)
                {
                    if (points[i].transform.position != positions[i])
                    {
                        Undo.RecordObject(points[i].transform, "Change Position");
                        points[i].transform.position = positions[i];

                    }

                }
            }   

           
        }
        if (Application.isPlaying) return;
        List<CPInfo> cpInfos = new List<CPInfo>();
        if (spawnPoints.Count>0)
        {
          
            var lastPoint = spawnPoints[0];
         
            if (coverLine.close) spawnPoints.Add(points[0].transform.position);
            else spawnPoints.Add(points[points.Count - 1].transform.position);
            var matrix = Handles.matrix;
           
            for (int i = 1; i < spawnPoints.Count; i++)
            {
                var cpInfo = new CPInfo();
                var next = spawnPoints[i];              
                var middle = (next+ lastPoint) /2;
                var dir = (next - lastPoint);
                if (dir.magnitude == 0) continue;
                var dP1 = next;
                var dP2 = lastPoint;
                dP1.y = 0;
                dP2.y = 0;
                var width = Vector3.Distance(dP1, dP2);
                dir.y = 0;
                Handles.color = Color.green*0.5f;
                Handles.ConeHandleCap(0, middle, Quaternion.LookRotation(dir), 0.1f, EventType.Repaint);
                dir = Quaternion.AngleAxis(-90, Vector3.up) * dir.normalized;
                cpInfo.position = middle+dir.normalized*coverLine.offsetZ;
                cpInfo.rotation = dir.normalized.magnitude>0? Quaternion.LookRotation(dir.normalized,Vector3.up):Quaternion.identity;
                cpInfo.colliderSize = new Vector3(width, coverLine.height, coverLine.depht);
                cpInfo.colliderCenter = Vector3.up * (coverLine.height*0.5f) + Vector3.forward * coverLine.depht * 0.5f;
                cpInfos.Add(cpInfo);
                Handles.matrix = Matrix4x4.TRS(cpInfo.position + Vector3.up * (coverLine.height * 0.5f) + dir.normalized * coverLine.depht * 0.5f, cpInfo.rotation, cpInfo.colliderSize);
                Handles.color = Color.green*0.5f;
                Handles.DrawWireCube(Vector3.zero, Vector3.one);
              
                Handles.matrix = matrix;
                Handles.color = Color.red * 0.5f;
                Handles.DrawLine(lastPoint, next);
                Handles.DrawLine(lastPoint + Vector3.up * (coverLine.height), next + Vector3.up * (coverLine.height ));

                lastPoint =next;               
            }
            Handles.color = Color.white;
            Handles.matrix = matrix;
        }

       
        var cps = coverLine.CPTransform.GetComponentsInChildren<Invector.vCharacterController.vCoverPoint>().vToList();
        if (cps.Count > cpInfos.Count)
        {
            while (cps.Count > cpInfos.Count)
            {
                DestroyImmediate(cps[cps.Count - 1].gameObject);
                cps.RemoveAt(cps.Count - 1);
            }
        }

        if (cps.Count < cpInfos.Count)
        {
            while (cps.Count < cpInfos.Count)
            {
                var cp = new GameObject("");
                cp.transform.parent = coverLine.CPTransform;
                cps.Add(cp.AddComponent<Invector.vCharacterController.vCoverPoint>());
            }
        }

        for(int i =0;i<cps.Count;i++)
        {
            var cp = cps[i];
            cp.gameObject.name = "CP-"+ (i+1).ToString("00");
            var cpInfo = cpInfos[i];
            cp.boxCollider.center = cpInfo.colliderCenter;
            cp.boxCollider.size = cpInfo.colliderSize;
            cp.transform.position = cpInfo.position;
            cp.transform.rotation = cpInfo.rotation;
        
            if (!string.IsNullOrEmpty(coverLine.coverPointTag)) cp.gameObject.tag = coverLine.coverPointTag;
            if (!string.IsNullOrEmpty(coverLine.coverPointLayer)) cp.gameObject.layer = LayerMask.NameToLayer(coverLine.coverPointLayer);
        }


    }

    struct CPInfo
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 colliderSize;
        public Vector3 colliderCenter;
    }
    

    void CalculateTangetByMode(ref Vector3 t1, ref Vector3 t2, Vector3 center, vCoverLine.CurveType curveType)
    {
        if(curveType == vCoverLine.CurveType.Mirror)
        {           
            var dir = center - t1;
            t2 = center + dir;
           
        }
        else if (curveType == vCoverLine.CurveType.Align)
        {
            var dir = center - t1;
            var dist = (center - t2).magnitude;
            t2 = center + dir.normalized * dist;
        }   
    }

    public override void OnInspectorGUI()
    {

        EditorGUILayout.HelpBox("Please don't insert objects inside this gameObject. If you reset the component all childs will be destroyed, included your object", MessageType.Warning);

        serializedObject.Update();
        DrawSettingsGUI();
        DrawCurrentPointGUI();
        serializedObject.ApplyModifiedProperties();    
    }

    private void DrawSettingsGUI()
    {
        
        var close = serializedObject.FindProperty("close");
        var depht = serializedObject.FindProperty("depht");
        var height = serializedObject.FindProperty("height");
        var width = serializedObject.FindProperty("width");
        var offsetZ = serializedObject.FindProperty("offsetZ");
        var coverPointTag = serializedObject.FindProperty("coverPointTag");
        var coverPointLayer = serializedObject.FindProperty("coverPointLayer");
        EditorGUILayout.PropertyField(close);
        EditorGUILayout.PropertyField(offsetZ);
        EditorGUILayout.PropertyField(width);
        EditorGUILayout.PropertyField(height);
        EditorGUILayout.PropertyField(depht);
        EditorGUILayout.PropertyField(coverPointTag);
        EditorGUILayout.PropertyField(coverPointLayer);


        if (points.Count > 0)
        {
            GUILayout.Box("Align All Points", GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();
          
            if (GUILayout.Button("Align X", EditorStyles.miniButton))
            {

                for (int i = 0; i < points.Count; i++)
                {

                    var point = points[i];
                    Undo.RecordObject(point.transform, "Change Position");
                    var localPosition = point.transform.localPosition;
                    localPosition.x = 0;
                    point.transform.localPosition = localPosition;

                }
                SceneView.RepaintAll();
                Repaint();
            }
            if (GUILayout.Button("Align Y", EditorStyles.miniButton))
            {
                for (int i = 0; i < points.Count; i++)
                {
                    var point = points[i];
                    Undo.RecordObject(point.transform, "Change Position");
                    var localPosition = point.transform.localPosition;
                    localPosition.y = 0;
                    point.transform.localPosition = localPosition;

                }
                SceneView.RepaintAll();
                Repaint();
            }
          
            if (GUILayout.Button("Align Z",EditorStyles.miniButton))
            {
                for (int i = 0; i < points.Count; i++)
                {
                    var point = points[i];
                    Undo.RecordObject(point.transform, "Change Position");
                    var localPosition = point.transform.localPosition;
                    localPosition.z = 0;
                    point.transform.localPosition = localPosition;

                }
                SceneView.RepaintAll();
                Repaint();
            }
            GUILayout.EndHorizontal();
            if (points.Count == 2)
            {
                GUILayout.Box("", GUILayout.ExpandWidth(true));
                if (GUILayout.Button("Make Circle", EditorStyles.miniButton))
                {
                    var p1 = points[0];
                    var p2 = points[1];
                    Undo.RecordObject(p1, "Change Tangent");
                    Undo.RecordObject(p2, "Change Tangent");
                    Undo.RecordObject(coverLine, "Change Close");
                    p1.curveType = vCoverLine.CurveType.Mirror;
                    p2.curveType = vCoverLine.CurveType.Mirror;
                    coverLine.close = true;
                    var dir = Quaternion.AngleAxis(-90, Vector3.up) * (p2.transform.position - p1.transform.position);

                    p1.tangentRight = dir * 0.67f;
                    p1.tangentLeft = -dir * 0.67f;
                    p2.tangentLeft = p1.tangentRight;
                    p2.tangentRight = p1.tangentLeft;
                    SceneView.RepaintAll();
                    Repaint();

                }
            }
        }
        GUILayout.Box("", GUILayout.ExpandWidth(true));
        if (GUILayout.Button("Add New Point", EditorStyles.miniButton))
        {
            Undo.RecordObject(coverLine.CtrlPTransform, "Point");
            Undo.RecordObject(coverLine, "Point");
            var point = points[points.Count - 1];
            var lP = point.transform.localPosition;
            if (point.conectedLeft)
            {
                var localDir = (point.transform.InverseTransformDirection(point.transform.position- point.conectedLeft.transform.position)).normalized;
                lP += localDir;
            }
            else lP += Vector3.right;          
            currentPoint= coverLine.NewPoint(lP);
            SceneView.RepaintAll();
            Repaint();
            
        }
      
    }

    private void DrawCurrentPointGUI()
    {       
        if (currentPoint)
        {
            GUILayout.Box("Selected Point ", GUILayout.ExpandWidth(true));
            GUILayout.BeginVertical( "box");
            GUILayout.Box("Point " + points.IndexOf(currentPoint), GUILayout.ExpandWidth(true));
           
            var value = currentPoint.curveType;
            EditorGUI.BeginChangeCheck();
            
            value = (vCoverLine.CurveType)EditorGUILayout.EnumPopup("Curve Type", value,EditorStyles.toolbarPopup);
            if (EditorGUI.EndChangeCheck())
            {
                if (!(currentPoint.curveType == vCoverLine.CurveType.Mirror || currentPoint.curveType == vCoverLine.CurveType.Align))
                {

                    if (value == vCoverLine.CurveType.Mirror || value == vCoverLine.CurveType.Align)
                    {
                        var p1 = currentPoint.transform.position;
                        var p2 = currentPoint.conectedLeft ? currentPoint.conectedLeft.transform.position - p1 : Vector3.zero;
                        var p3 = currentPoint.conectedRight ? currentPoint.conectedRight.transform.position - p1 : Vector3.zero;
                        var lenght = (p2.magnitude * 0.5f + p3.magnitude * 0.5f) * 0.5f;
                        var dir = ((-p2.normalized + p3.normalized) * 0.5f);
                        var pResult = dir.normalized * lenght;
                        currentPoint.tangentRight = currentPoint.transform.InverseTransformDirection(pResult);
                        currentPoint.tangentLeft = -currentPoint.transform.InverseTransformDirection(pResult);
                    }
                }            
                currentPoint.curveType = value;
                SceneView.RepaintAll();
                Repaint();
            }
            GUILayout.Box("Tangents", GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Align X", EditorStyles.miniButton))
            {
                var leftMagnitude = new Vector3(currentPoint.tangentLeft.x, 0, currentPoint.tangentLeft.z).magnitude;
                var rightMagnitude = new Vector3(currentPoint.tangentRight.x, 0, currentPoint.tangentRight.z).magnitude;
                var magnitude = (leftMagnitude + rightMagnitude) * 0.5f;
                currentPoint.tangentLeft = Vector3.back * magnitude;               
                currentPoint.tangentRight = Vector3.forward* magnitude;
                SceneView.RepaintAll();
                Repaint();
            }
            if (GUILayout.Button("Align Y", EditorStyles.miniButton))
            {
                currentPoint.tangentLeft.y = 0;
                currentPoint.tangentRight.y = 0;
                SceneView.RepaintAll();
                Repaint();
            }
            if (GUILayout.Button("Align Z", EditorStyles.miniButton))
            {
                var leftMagnitude = new Vector3(currentPoint.tangentLeft.x,0, currentPoint.tangentLeft.z).magnitude;
                var rightMagnitude = new Vector3(currentPoint.tangentRight.x, 0, currentPoint.tangentRight.z).magnitude;
                var magnitude = (leftMagnitude + rightMagnitude) * 0.5f;
                currentPoint.tangentLeft = Vector3.left * magnitude;
                currentPoint.tangentRight = Vector3.forward * magnitude;
                SceneView.RepaintAll();
                Repaint();
            }
            GUILayout.EndHorizontal();
            GUILayout.Box("", GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();
            GUI.enabled = currentPoint.conectedLeft;
            if ( GUILayout.Button("<< ADD", EditorStyles.miniButton))
            {
                AddBack();
            }
          
            GUI.enabled = currentPoint.conectedRight;
      
            if (GUILayout.Button("ADD >>", EditorStyles.miniButton))
            {
                AddForward();
            }
            GUILayout.EndHorizontal();
            GUI.enabled = true;
            GUILayout.Box("", GUILayout.ExpandWidth(true));
            if (GUILayout.Button("DELETE POINT", EditorStyles.miniButton))
            {
                DestroyImmediate(currentPoint.gameObject); SceneView.RepaintAll();
                Repaint();
            }

            GUILayout.EndVertical();
        }
    }

    private void AddForward()
    {
        Undo.RecordObject(coverLine.CtrlPTransform, "Point");
        Undo.RecordObject(coverLine, "Point");

       coverLine.NewPoint((currentPoint.conectedRight.transform.localPosition + currentPoint.transform.localPosition) * 0.5f, points.IndexOf(currentPoint) + 1);
        SceneView.RepaintAll();
        Repaint();
    }

    private void AddBack()
    {
        Undo.RecordObject(coverLine.CtrlPTransform, "Point");
        Undo.RecordObject(coverLine, "Point");
        coverLine.NewPoint((currentPoint.conectedLeft.transform.localPosition + currentPoint.transform.localPosition) * 0.5f, points.IndexOf(currentPoint));
        SceneView.RepaintAll();
        Repaint();
    }
}
