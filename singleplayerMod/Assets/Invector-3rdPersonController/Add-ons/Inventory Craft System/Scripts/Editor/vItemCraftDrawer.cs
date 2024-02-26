using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Invector.vItemManager
{
    public partial class vItemDrawer
    {
        Rect buttonRect;
        public List<vItemType> filter = new List<vItemType>();

        [vItemDrawerToolBar("Items to Craft")]
        public virtual void DrawCraftProperties(ref List<vItem> items, bool showObject, bool editName)
        {
            //item.canCraft = EditorGUILayout.Toggle("Can Craft", item.canCraft);
            //item.price = EditorGUILayout.IntField("Price", item.price);
            GUILayout.BeginHorizontal();

            GUILayout.Box("Items to Craft");

            if (Event.current.type == EventType.Repaint)
            {
                buttonRect = GUILayoutUtility.GetLastRect();
                buttonRect.x += buttonRect.width * 0.5f;
            }

            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                PopupWindow.Show(buttonRect, new vItemSelector
                    (items, ref filter, (vItem item) =>//OnSelectItem
                    {
                        vItemCraft craft = new vItemCraft();
                        craft.item = item;
                        craft.amount = 1;
                        this.item.itemsToCraft.Add(craft);
                        EditorUtility.SetDirty(this.item);
                    }
                    ));
            }

            GUILayout.EndHorizontal();
            if (item.itemsToCraft == null) item.itemsToCraft = new List<vItemCraft>();
            else
            {
                for (int i = 0; i < item.itemsToCraft.Count; i++)
                {
                    GUILayout.BeginHorizontal();

                    var name = " ID " + item.itemsToCraft[i].item.id.ToString("00") +
                        "\n - " + item.itemsToCraft[i].item.name + "\n - " +
                        item.itemsToCraft[i].item.type.ToString();

                    var texture = item.itemsToCraft[i].item.iconTexture;
                    var tooltip = item.itemsToCraft[i].item.description;
                    var content = new GUIContent(name, texture, tooltip);

                    if (GUILayout.Button(content, "box", GUILayout.Height(60), GUILayout.MinWidth(60)))
                    {
                        vItemListWindow.SetCurrentSelectedItem(items.IndexOf(item.itemsToCraft[i].item));
                    }

                    GUILayout.BeginVertical();
                    GUILayout.Label("Amount");
                    item.itemsToCraft[i].amount = EditorGUILayout.IntField(item.itemsToCraft[i].amount, GUILayout.Width(40));
                    GUILayout.EndVertical();
                    if (GUILayout.Button("X", GUILayout.Height(50), GUILayout.Width(20)))
                    {
                        item.itemsToCraft.RemoveAt(i);
                        GUILayout.EndHorizontal();
                        break;
                    }

                    GUILayout.EndHorizontal();
                }
            }
        }
    }
}