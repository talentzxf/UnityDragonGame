using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomPropertyDrawer(typeof(vLayerSelectorAttribute))]
public class vLayerSelectorDrawer : PropertyDrawer
{

    static GUIContent[] _layers;
    public static GUIContent[] layers
    {
        get
        {
            if (_layers == null)
            {
                _layers = new GUIContent[UnityEditorInternal.InternalEditorUtility.layers.Length];
                for (int i = 0; i < _layers.Length; i++)
                {
                    _layers[i] = new GUIContent(UnityEditorInternal.InternalEditorUtility.layers[i]);
            
                }
            }

            return _layers;
        }
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.String)
        {
            var index = System.Array.IndexOf(UnityEditorInternal.InternalEditorUtility.layers, property.stringValue);

            EditorGUI.BeginChangeCheck();
            index = EditorGUI.Popup(position, label, index, layers);
            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = UnityEditorInternal.InternalEditorUtility.layers[index];
            }

        }
        else base.OnGUI(position, property, label);
    }
}
