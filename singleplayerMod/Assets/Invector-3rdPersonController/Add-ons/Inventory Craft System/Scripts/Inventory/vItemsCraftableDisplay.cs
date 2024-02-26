using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Invector.vItemManager
{
    /// <summary>
    /// Display the craftable items
    /// </summary>
    public class vItemsCraftableDisplay : MonoBehaviour
    {
        public vInventory inventory;
        public vItemCraftControl craftControl;
        public vItemWindow itemWindow;
        public vItemsCraftMaterialDisplay itemToCraftDisplay;
        [HideInInspector]
        public vItemSlot currentSelectedSlot;
        [HideInInspector]
        public int amount;

        public UnityEvent onEnableCraft;
        public UnityEvent onDisableCraft;
        public UnityEvent onSubmit;
        public UnityEvent onSelect;

        /// <summary>
        /// Craftable Items
        /// </summary>
        public List<vItem> items
        {
            get
            {
                return inventory.allItems.FindAll(i => craftControl.craftableItems.Exists(c => c.item.Equals(i) && c.canCraft));
            }
        }

        public virtual void OnEnable()
        {
            if (inventory == null)
            {
                inventory = GetComponentInParent<vInventory>();
            }

            if (craftControl == null)
            {
                craftControl = GetComponentInParent<vItemCraftControl>();
            }

            if (inventory && itemWindow)
            {
                itemWindow.CreateEquipmentWindow(items, OnSubmit, OnSelectSlot);
                itemToCraftDisplay.onChangeAmount = OnChangeCraftAmount;
                itemToCraftDisplay.CanCraft = CanCraftItem;
            }
        }

        /// <summary>
        /// Event called when the amount of the craftable item is changed
        /// </summary>
        /// <param name="value"></param>
        public void OnChangeCraftAmount(int value)
        {
            amount = value;
        }

        /// <summary>
        /// Create the craftable item
        /// </summary>
        public void CreateCurrentItem()
        {
            StartCoroutine(CraftItemRoutine());
        }

        /// <summary>
        /// Event called when an slot is selected
        /// </summary>
        /// <param name="slot">target slot</param>
        public virtual void OnSelectSlot(vItemSlot slot)
        {
            currentSelectedSlot = slot;
            CheckCanCraft();
            itemToCraftDisplay.CreateEquipmentWindow(slot.item.itemsToCraft, inventory.items);
            onSelect.Invoke();
        }

        /// <summary>
        /// Set <seealso cref="EventSystem.current.currentSelectedGameObject"/>
        /// </summary>
        /// <param name="target">target object</param>
        public virtual void SetSelectable(GameObject target)
        {
            try
            {
                var pointer = new PointerEventData(EventSystem.current);
                ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, pointer, ExecuteEvents.pointerExitHandler);
                EventSystem.current.SetSelectedGameObject(target, new BaseEventData(EventSystem.current));
                ExecuteEvents.Execute(target, pointer, ExecuteEvents.selectHandler);
            }
            catch { }
        }

        /// <summary>
        /// Craft item routine
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator CraftItemRoutine()
        {
            vItem item = currentSelectedSlot.item;

            for (int i = 0; i < item.itemsToCraft.Count; i++)
            {
                var count = item.itemsToCraft[i].amount * amount;
                while (count > 0)
                {
                    var itemToLeave = inventory.items.Find(_item => _item.id.Equals(item.itemsToCraft[i].item.id) && _item.amount > 0);
                    if (itemToLeave)
                    {
                        inventory.OnDestroyItem(itemToLeave, 1);
                    }

                    count--;
                    yield return null;
                }
            }
            ItemReference itemReference = new ItemReference(item.id);
            itemReference.amount = amount;
            inventory.AddItemsHandler(itemReference);
            itemToCraftDisplay.CreateEquipmentWindow(currentSelectedSlot.item.itemsToCraft, inventory.items, false);
            CheckCanCraft();
        }

        /// <summary>
        /// Event called when an slot is Submited
        /// </summary>
        /// <param name="slot">target slot </param>
        protected virtual void OnSubmit(vItemSlot slot)
        {
            currentSelectedSlot = slot;
            itemToCraftDisplay.CreateEquipmentWindow(slot.item.itemsToCraft, inventory.items);
            onSubmit.Invoke();
            if (CanCraftItem(slot.item))
            {

            }
        }

        /// <summary>
        /// Check if the item of the <seealso cref="currentSelectedSlot"/> can be craft or not
        /// </summary>
        protected virtual void CheckCanCraft()
        {
            bool can = false;
            if (currentSelectedSlot)
            {
                can = CanCraftItem(currentSelectedSlot.item);
            }
            if (can)
            {
                onEnableCraft.Invoke();
            }
            else
            {
                onDisableCraft.Invoke();
            }
        }

        /// <summary>
        /// Check if can craft the item of the <seealso cref="currentSelectedSlot"/>
        /// </summary>
        /// <returns>Can craft</returns>
        public virtual bool CanCraftItem()
        {
            if (currentSelectedSlot)
            {
                return CanCraftItem(currentSelectedSlot.item);
            }
            return false;
        }

        /// <summary>
        /// Check if can craft the item
        /// </summary>
        /// <param name="item">target item</param>
        /// <returns></returns>
        protected virtual bool CanCraftItem(vItem item)
        {
            for (int i = 0; i < item.itemsToCraft.Count; i++)
            {
                int required = item.itemsToCraft[i].amount * amount;
                int contained = GetAllAmount(inventory.items.FindAll(_item => _item.id.Equals(item.itemsToCraft[i].item.id)));
                if (contained < required)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Get all amount of the items in list
        /// </summary>
        /// <param name="Items">list of items</param>
        /// <returns></returns>
        protected virtual int GetAllAmount(List<vItem> Items)
        {
            int amount = 0;
            for (int i = 0; i < Items.Count; i++)
            {
                amount += Items[i].amount;
            }
            return amount;
        }

    }
}