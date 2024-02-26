using UnityEditor;
using UnityEngine;

namespace Invector.vCharacterController.AI
{
    [CustomEditor(typeof(vControlAI), true)]
    public class vControlAIEditor : vEditorBase
    {
        public SerializedProperty fov;
        public SerializedProperty minDist, maxDist;
        public SerializedProperty lostDist;
        public SerializedProperty eyes;
        public SerializedProperty debug;
        public Color minDistColor = new Color(0, 0, 0, 1f);
        public Color maxDistColor = new Color(1, 1, 0, 1f);
        public Color lostDistColor = new Color(1f, 0, 0, 1f);
        public Color combatColor = new Color(0, 0, 1, 1f);
        public GUIStyle labelStyle;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (serializedObject != null)
            {
                debug = serializedObject.FindProperty("_debugVisualDetection");
                minDist = serializedObject.FindProperty("_minDistanceToDetect");
                maxDist = serializedObject.FindProperty("_maxDistanceToDetect");
                fov = serializedObject.FindProperty("_fieldOfView");
                lostDist = serializedObject.FindProperty("_lostTargetDistance");
                eyes = serializedObject.FindProperty("detectionPointReference");
            }
            labelStyle = new GUIStyle(skin.label);
            labelStyle.normal.textColor = Color.white;
        }

        protected virtual void OnSceneGUI()
        {
            if (debug == null || !debug.boolValue||target ==null) return;
            vIControlAICombat combatControl = null;
            if (target is vIControlAICombat)
            {
                combatControl = target as vIControlAICombat;
            }
            if (combatControl == null ) return;
            DrawGizmos(combatControl);
            DrawDebugWindow(combatControl);
        }

        private void DrawGizmos(vIControlAICombat combatControl)
        {
            minDistColor.a = .1f;
            maxDistColor.a = .1f;
            lostDistColor.a = .1f;
            combatColor.a = .1f;

            var transform = (eyes != null && eyes.objectReferenceValue != null ? (eyes.objectReferenceValue as Transform) : (target as MonoBehaviour).transform);
            float _fov = fov != null ? fov.floatValue : 0;

            if (combatControl != null)
            {
                Handles.color = combatColor;
                Handles.DrawSolidDisc(combatControl.transform.position, Vector3.up, combatControl.combatRange);
                combatColor.a = 1;
                Handles.color = combatColor;
                Handles.DrawWireDisc(combatControl.transform.position, Vector3.up, combatControl.combatRange);
            }

            if (maxDist != null)
            {

                var forward = transform.forward;
                forward.y = 0;
                Handles.color = lostDistColor;
                Handles.DrawSolidDisc(transform.position, Vector3.up, maxDist.floatValue + lostDist.floatValue);
                lostDistColor.a = 1;
                Handles.color = lostDistColor;
                Handles.DrawWireDisc(transform.position, Vector3.up, maxDist.floatValue + lostDist.floatValue);
                Handles.color = maxDistColor;
                Handles.DrawSolidArc(transform.position, Vector3.up, forward, _fov * 0.5f, maxDist.floatValue);
                Handles.DrawSolidArc(transform.position, Vector3.up, forward, -(_fov * 0.5f), maxDist.floatValue);
                maxDistColor.a = 1;
                Handles.color = maxDistColor;
                Quaternion leftRayRotation = Quaternion.AngleAxis(-_fov * 0.5f, Vector3.up);
                Quaternion rightRayRotation = Quaternion.AngleAxis(_fov * 0.5f, Vector3.up);
                Vector3 leftRayDirection = leftRayRotation * transform.forward;
                Vector3 rightRayDirection = rightRayRotation * transform.forward;
                Handles.DrawLine(transform.position, transform.position + leftRayDirection * maxDist.floatValue);
                Handles.DrawLine(transform.position, transform.position+ rightRayDirection * maxDist.floatValue);
              
                Handles.DrawWireDisc(transform.position, Vector3.up, maxDist.floatValue);
               
            }

            if (minDist != null)
            {
                Handles.color = minDistColor;
                Handles.DrawSolidDisc(transform.position, Vector3.up, minDist.floatValue);
                minDistColor.a = 1;
                Handles.color = minDistColor;
                Handles.DrawWireDisc(transform.position, Vector3.up, minDist.floatValue);
            }
        }

        GUIContent content;
        private void DrawDebugWindow(vIControlAICombat combatControl)
        {
            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(Screen.width - 170, Screen.height - 195, 170, 195));
            minDistColor.a = .8f;
            maxDistColor.a = .8f;
            lostDistColor.a = .8f;
            combatColor.a = .8f;
            var color = GUI.color;

            GUILayout.BeginVertical("VISUAL DEBUG", skin.window, GUILayout.Width(150));
            GUILayout.Label(m_Logo, skin.label, GUILayout.MaxHeight(25));
            GUILayout.Space(10);

            if (content == null) content = new GUIContent(EditorGUIUtility.whiteTexture);
            GUILayout.BeginHorizontal("box");
            {
                GUI.color = minDistColor;
                content.text = "Min Distance To Detect";
                GUILayout.Label(content, labelStyle);
            }
            GUILayout.EndHorizontal();

            
            GUILayout.BeginHorizontal("box");
            {
                GUI.color = maxDistColor;
                content.text = "Max Distance To Detect";
            
                GUILayout.Label(content, labelStyle);
            }
            GUILayout.EndHorizontal();

          
            GUILayout.BeginHorizontal("box");
            {
                GUI.color = lostDistColor;
                content.text = "Lost Target Distance";              
                GUILayout.Label(content, labelStyle);
            }
            GUILayout.EndHorizontal();

            if (combatControl != null)
            {
               
                GUILayout.BeginHorizontal("box");
                {
                    GUI.color = combatColor;
                    content.text = "Combat Range";                  
                    GUILayout.Label(content, labelStyle);
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUI.color = color;
            GUILayout.EndArea();
            Handles.EndGUI();
        }
    }
}