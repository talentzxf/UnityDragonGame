using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomPropertyDrawer(typeof(vTagSelectorAttribute))]
public class vTagSelectorDrawer : PropertyDrawer
{
    static GUIContent[] _tags;
    public static GUIContent[] tags
    {
        get
        {
            if(_tags == null)
            {
                _tags = new GUIContent[UnityEditorInternal.InternalEditorUtility.tags.Length];
                for (int i = 0; i < _tags.Length; i++)
                {
                    _tags[i] = new GUIContent(UnityEditorInternal.InternalEditorUtility.tags[i]);
                }

            }

            return _tags;
        }
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.String)
        {
            var index = System.Array.IndexOf(UnityEditorInternal.InternalEditorUtility.tags, property.stringValue);

            EditorGUI.BeginChangeCheck();
            index = EditorGUI.Popup(position, label, index, tags);
            if(EditorGUI.EndChangeCheck())
            {
                property.stringValue = UnityEditorInternal.InternalEditorUtility.tags[index];
            }

        }
        else base.OnGUI(position, property, label);
    }

}
