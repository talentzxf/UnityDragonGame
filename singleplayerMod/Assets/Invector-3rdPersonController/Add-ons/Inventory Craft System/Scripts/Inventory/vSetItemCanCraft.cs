using System.Collections.Generic;
using UnityEngine;

namespace Invector.vItemManager
{
    [vClassHeader("Set Item CanCraft")]
    public class vSetItemCanCraft : vMonoBehaviour
    {
        public List<CheckItemID> itemIDEvents;
        vItemCraftControl craftControl;

        public void SetItemCanCraft(Collider collider)
        {
            if (collider != null)
            {
                craftControl = collider.gameObject.GetComponent<vItemCraftControl>();

                if (craftControl)
                {
                    FindItemID();
                }
            }
        }

        public void SetItemCanCraft(GameObject gameObject)
        {
            if (gameObject != null)
            {
                craftControl = gameObject.GetComponent<vItemCraftControl>();

                if (craftControl)
                {
                    FindItemID();
                }
            }
        }

        private void FindItemID()
        {
            for (int i = 0; i < itemIDEvents.Count; i++)
            {
                CheckItemID check = itemIDEvents[i];
                check.Check(craftControl);
            }
        }

        [System.Serializable]
        public class CheckItemID
        {
            public string name;
            public List<int> _itemsID;
            public bool canCraft;

            public void Check(vItemCraftControl craftControl)
            {
                for (int i = 0; i < _itemsID.Count; i++)
                {
                    craftControl.SetActiveCanCraftItem(_itemsID[i], canCraft);
                }
            }
        }
    }
}