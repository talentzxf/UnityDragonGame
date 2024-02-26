using UnityEngine;
using UnityEditor;
using Invector.vCharacterController.AI.FSMBehaviour;

[CustomEditor(typeof(vFSMBehaviour))]
public class vFSMBehaviourEditor : Editor
{
    private void OnEnable()
    {
        ((vFSMBehaviour)target).ReloadChilds();
    }
    public override void OnInspectorGUI()
    {
        GUILayout.BeginVertical(); 
        if(!vFSMNodeEditorWindow.curWindow)
        if(GUILayout.Button("Open in FSM Editor Window"))
        {
                vFSMNodeEditorWindow.InitEditorWindow(target as vFSMBehaviour);
        }

        base.OnInspectorGUI();       
        GUILayout.EndVertical();
    }
    public override bool UseDefaultMargins()
    {
        return false;
    }
}