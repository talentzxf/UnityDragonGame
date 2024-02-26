using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Invector.vItemManager
{
    [CustomPropertyDrawer(typeof(vItemCraftControl.CraftableItem), true)]
    public class vCraftableItemDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label.tooltip = "Can craft";
            label = EditorGUI.BeginProperty(position, label, property);

            var itemRect = new Rect(position.x, position.y, position.width - (position.height * 1.5f), position.height);
            GUI.enabled = false;
            EditorGUI.PropertyField(itemRect, property.FindPropertyRelative("item"), GUIContent.none);
            GUI.enabled = true;

            var toogleRect = new Rect(position.x + (position.width - (position.height)), position.y, position.height, position.height);
            var canCraft = property.FindPropertyRelative("canCraft");
            canCraft.boolValue = EditorGUI.Toggle(toogleRect, new GUIContent("", "Can Craft"), canCraft.boolValue);

            EditorGUI.EndProperty();
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}