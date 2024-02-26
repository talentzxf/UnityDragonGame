using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Invector.vItemManager
{
    [CustomEditor(typeof(vItemCraftControl), true)]
    public class vItemCraftControlEditor : vEditorBase
    {
        string search;
        Vector2 scroll;
        protected override void AdditionalGUI()
        {
            SerializedProperty craftItemList = serializedObject.FindProperty("craftableItems");

            vItemCraftControl craftControl = (vItemCraftControl)target;

            GUILayout.BeginVertical("box");
            GUILayout.Box("Can Craft items", EditorStyles.centeredGreyMiniLabel);
            search = EditorGUILayout.TextField(search, EditorStyles.toolbarSearchField);
            int array = craftItemList.arraySize;
            scroll = GUILayout.BeginScrollView(scroll, GUILayout.Height(Mathf.Min(EditorGUIUtility.singleLineHeight * 11, EditorGUIUtility.singleLineHeight * ((string.IsNullOrEmpty(search) ? array : craftControl.craftableItems.FindAll(c => c.item != null && c.item.name.ToLower().Trim().Contains(search.ToLower().Trim())).Count) + 1))));

            for (int i = 0; i < array; i++)
            {
                var cItem = craftItemList.GetArrayElementAtIndex(i);
                var item = cItem.FindPropertyRelative("item");
                var canDraw = string.IsNullOrEmpty(search) || item.objectReferenceValue == null || item.objectReferenceValue.name.ToLower().Trim().Contains(search.ToLower().Trim());
                if (canDraw) EditorGUILayout.PropertyField(craftItemList.GetArrayElementAtIndex(i));
            }

            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }
    }
}