using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;

namespace Invector.vCharacterController.AI
{
    [CustomPropertyDrawer(typeof(vAITarget),true)]
    public class vAITargetDrawer : PropertyDrawer
    {        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

           
            label = EditorGUI.BeginProperty(position, label, property);
           
            Rect rect = position;
            rect.width = EditorGUIUtility.labelWidth;
            rect.height = EditorGUIUtility.singleLineHeight;


            if (!property.propertyPath.Contains("Array"))
            {
                GUI.Label(rect, label);
                rect.x += rect.width;
                rect.width = position.width - rect.width;
            }
            else
            {
                rect.width = position.width;               
            }

            if (property.hasVisibleChildren)
            {
                var oldWidth = rect.width;
                rect.width = EditorGUIUtility.singleLineHeight;
                property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, "");
                rect.width = oldWidth;
            }
            SerializedProperty transformProp = property.FindPropertyRelative("_transform");
            GUI.color =transformProp.objectReferenceValue!=null? ( property.FindPropertyRelative("isFixedTarget").boolValue ? Color.red : Color.green):Color.grey;
            EditorGUI.PropertyField(rect, transformProp, !property.propertyPath.Contains("Array")?GUIContent.none:label);
            GUI.color = Color.white;
            rect.y += EditorGUIUtility.singleLineHeight;
            GUI.enabled = true;
            if (property.hasVisibleChildren && property.isExpanded)
            {
                var childEnum = property.GetEnumerator();
               
                while (childEnum.MoveNext())
                {
                    var current = childEnum.Current as SerializedProperty;
                  
                    if (property.name!=("_transform"))
                    {
                     
                        rect.height = EditorGUI.GetPropertyHeight(current);
                        if (property.name == "_tag")
                        {
                            EditorGUI.LabelField(rect, "Tag", property.stringValue,EditorStyles.linkLabel);
                        }
                        else EditorGUI.PropertyField(rect, current);
                        rect.y += EditorGUI.GetPropertyHeight(current);
                    }
                   
                }
            }
        
            if (GUI.changed) property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
          
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = base.GetPropertyHeight(property, label);
            if (property.hasVisibleChildren && property.isExpanded)
            {
                var childEnum = property.GetEnumerator();
                while (childEnum.MoveNext())
                {
                    var current = childEnum.Current as SerializedProperty;
                    if (property.name != ("_transform"))
                    {
                        height += EditorGUI.GetPropertyHeight(current);
                    }

                }
            }
               
            return height;
        }
    } 

}