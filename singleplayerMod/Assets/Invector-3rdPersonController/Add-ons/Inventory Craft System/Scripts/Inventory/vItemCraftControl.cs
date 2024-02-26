using System.Collections.Generic;
using UnityEngine;
namespace Invector.vItemManager
{
    /// <summary>
    /// Controls which crafting item can be crafted
    /// </summary>
    [vClassHeader("Item Craft Control", openClose = false)]
    public class vItemCraftControl : vMonoBehaviour
    {
        [vHelpBox("Open the ItemListData in the ItemManager component, select the item you want to craft and add an item to the ItemToCraft list. " +
            "Then click in the button 'Refresh Craftable Items' to enable/disable in the Inventory Craft List Window", vHelpBoxAttribute.MessageType.Info)]

        [vButton("Refresh Craftable Items", "RefreshCraftableItems", typeof(vItemCraftControl), false)]

        public vItemManager itemManager;

        /// <summary>
        /// List of All craftable items
        /// </summary>
        [HideInInspector]
        public List<CraftableItem> craftableItems;

        private void OnValidate()
        {
            GetItemManager();
        }

        private void Reset()
        {
            GetItemManager();
        }

        private void GetItemManager()
        {
            if (itemManager == null && gameObject.TryGetComponent<vItemManager>(out vItemManager _itemManager)) itemManager = _itemManager;
        }

        /// <summary>
        /// Get All Craftable items inside <seealso cref="vItemManager.itemListData"/> 
        /// </summary>
        public void RefreshCraftableItems()
        {
            GetItemManager();

            var craftableItems = itemManager.itemListData.items.FindAll(i => i.itemsToCraft.Count > 0);
            if (this.craftableItems == null)
            {
                this.craftableItems = new List<CraftableItem>();
            }
            else
            {
                this.craftableItems = this.craftableItems.FindAll(c => c.item != null && craftableItems.Exists(i => i == c.item));
            }
            for (int i = 0; i < craftableItems.Count; i++)
            {
                if (!this.craftableItems.Exists(c => c.item == craftableItems[i]))
                {
                    this.craftableItems.Add(new CraftableItem(craftableItems[i]));
                }
            }
        }

        /// <summary>
        /// Enable of disable item to be craftable
        /// </summary>
        /// <param name="itemId">Id of the item</param>
        /// <param name="value">Can Craft</param>
        public void SetActiveCanCraftItem(int itemId, bool value)
        {
            var craftableToggle = craftableItems.Find(c => c.item != null && c.item.id == itemId);

            if (craftableToggle != null)
            {
                craftableToggle.canCraft = value;
            }
        }

        /// <summary>
        /// Enable of disable item to be craftable
        /// </summary>
        /// <param name="itemName">Name of the item</param>
        /// <param name="value">Can Craft</param>
        public void SetActiveCanCraftItem(string itemName, bool value)
        {
            var craftableToggle = craftableItems.Find(c => c.item != null && c.item.name == itemName);

            if (craftableToggle != null)
            {
                craftableToggle.canCraft = value;
            }
        }

        /// <summary>
        /// Enable item to be craftable
        /// </summary>
        /// <param name="itemID">Id of the item</param>
        public void EnableCanCraftItem(int itemID)
        {
            SetActiveCanCraftItem(itemID, true);
        }

        /// <summary>
        /// Disable item to be craftable
        /// </summary>
        /// <param name="itemID">Id of the item</param>
        public void DisableCanCraftItem(int itemID)
        {
            SetActiveCanCraftItem(itemID, false);
        }

        /// <summary>
        ///  Enable item to be craftable
        /// </summary>
        /// <param name="itemName">Name of the item</param>
        public void EnableCanCraftItem(string itemName)
        {
            SetActiveCanCraftItem(itemName, true);
        }

        /// <summary>
        /// Disable item to be craftable
        /// </summary>
        /// <param name="itemName">Name of the item</param>
        public void DisableCanCraftItem(string itemName)
        {
            SetActiveCanCraftItem(itemName, false);
        }

        /// <summary>
        /// Class that defines if a Craftable item can be crafted
        /// </summary>
        [System.Serializable]
        public class CraftableItem
        {
            public vItem item;
            public bool canCraft;
            public CraftableItem(vItem item)
            {
                this.item = item;
                this.canCraft = true;
            }
            public static implicit operator vItem(CraftableItem craftableItem)
            {
                return craftableItem.item;
            }
        }
    }
}