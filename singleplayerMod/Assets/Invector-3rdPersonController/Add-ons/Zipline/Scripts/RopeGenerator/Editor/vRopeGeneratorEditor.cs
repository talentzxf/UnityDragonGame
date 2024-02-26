using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Invector
{
    [CustomEditor(typeof(vRope))]
    public class vRopeGeneratorEdtiro : vEditorBase
    {
        vRope rope;
        [MenuItem("GameObject/Invector/Procedural Rope")]

        static void Create()
        {
            GameObject gameObject = new GameObject("Rope");
            vRope c = gameObject.AddComponent<vRope>();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            rope = target as vRope;
        }
        private void OnSceneGUI()
        {
           
            if (rope)
            {
                if (rope.endPoint)
                {
                   
                    if (Handles.Button(rope.endPoint.position, rope.ancorReference.rotation, rope.colliderSizeY * 0.5f, rope.colliderSizeY * 0.5f, Handles.SphereHandleCap))
                    {
                        Selection.activeObject = rope.endPoint;
                    }
                    Handles.color = rope.gizmosColor* 0.5f;
                    if(rope.Length>0)
                    {
                        vZiplineAnchorPoints anchorPoints = rope.GetComponentInChildren<vZiplineAnchorPoints>();
                        if(anchorPoints)
                        {
                            var movementDirection = anchorPoints.movementDirection;
                            var direction = rope.ancorReference.forward;
                            var position = rope.ancorReference.position;
                            for (float i = 0; i < rope.Length; i += 1f)
                            {
                                Handles.ConeHandleCap(0, position + direction * i, Quaternion.LookRotation(movementDirection), rope.colliderSizeX * 0.5f, EventType.Repaint);
                            }
                        }
                       
                    }
                }

            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(GUI.changed)
            {
                rope.Rebuild();
            }
        }
    }
}