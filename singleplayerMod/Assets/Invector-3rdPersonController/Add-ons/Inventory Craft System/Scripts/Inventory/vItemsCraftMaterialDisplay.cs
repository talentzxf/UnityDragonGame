using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Invector.vItemManager
{
    /// <summary>
    /// Display of the Craftable item Materials
    /// </summary>
    public class vItemsCraftMaterialDisplay : MonoBehaviour
    {
        public vItemCraftMaterialSlot slotPrefab;
        public RectTransform contentWindow;
        public Text displayDescriptionText;
        [vHelpBox("Special text format for item description \n(NAME) = Name of the Item\n(DESC) = Description of the Item (ATTR) = Attributes of the Item\n***Keep empty to use default format***")]
        [Multiline]
        public string descriptionTextFormat = "(NAME)\n(DESC)\n(ATTR)";
        public InputField.OnChangeEvent onChangeDescription;
        public List<vItemCraftMaterialSlot> slots;
        private readonly vItem currentItem;
        private StringBuilder text;

        public bool updateSlotCount = true;
        private int _amount;
        public delegate void OnChangeAmount(int value);
        public delegate bool CanCraftDelegate();
        public OnChangeAmount onChangeAmount;
        public CanCraftDelegate CanCraft;

        public int amount
        {
            get
            {
                if (_amount < 1)
                {
                    _amount = 1;
                    onChangeAmount?.Invoke(1);
                }
                return _amount;
            }
            set
            {
                _amount = value;
                onChangeAmount?.Invoke(value);
            }
        }

        /// <summary>
        /// Create a List of <seealso cref="vItemCraftMaterialSlot"/>
        /// </summary>
        /// <param name="requiredItems">List of  materials that craftable item needs</param>
        /// <param name="currentItems">List of  all items</param>
        /// <param name="destroyAdditionalSlots">Destroy or not additional slots</param>
        public void CreateEquipmentWindow(List<vItemCraft> requiredItems, List<vItem> currentItems, bool destroyAdditionalSlots = true)
        {
            var _craftable = requiredItems;
            if (_craftable.Count == 0)
            {
                if (displayDescriptionText) displayDescriptionText.text = "";
                if (slots.Count > 0 && destroyAdditionalSlots)
                {
                    for (int i = 0; i < slots.Count; i++)
                    {
                        Destroy(slots[i].gameObject);
                    }
                    slots.Clear();
                }
                return;
            }

            if (slots == null) slots = new List<vItemCraftMaterialSlot>();
            var count = slots.Count;
            if (updateSlotCount)
            {
                if (count < _craftable.Count)
                {
                    for (int i = count; i < _craftable.Count; i++)
                    {
                        var slot = Instantiate(slotPrefab) as vItemCraftMaterialSlot;
                        slots.Add(slot);
                        var rectTranform = slot.GetComponent<RectTransform>();
                        rectTranform.SetParent(contentWindow);
                        rectTranform.localPosition = Vector3.zero;
                        rectTranform.localScale = Vector3.one;

                    }
                }
                else if (count > _craftable.Count)
                {
                    for (int i = count - 1; i > _craftable.Count - 1; i--)
                    {
                        Destroy(slots[slots.Count - 1].gameObject);
                        slots.RemoveAt(slots.Count - 1);
                    }
                }
            }

            count = slots.Count;
            for (int i = 0; i < _craftable.Count; i++)
            {
                vItemCraftMaterialSlot slot = null;
                if (i < count)
                {
                    slot = slots[i];
                    int required = _craftable[i].amount * amount;
                    int contained = GetAllAmount(currentItems.FindAll(item => item.id.Equals(_craftable[i].item.id)));
                    slot.SetSlotIcon(_craftable[i].item.icon);
                    slot.SetSlotAmountText(required.ToString("00") + " / " +
                        (contained < required ? "<color=red>" : "") +
                         contained.ToString("00") +
                        (contained < required ? "</color>" : ""));
                }
            }
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

        /// <summary>
        /// Set <seealso cref="EventSystem.current.currentSelectedGameObject"/> with a delay frame
        /// </summary>
        /// <param name="target">target object</param>
        protected virtual IEnumerator SetSelectableHandle(GameObject target)
        {
            if (this.enabled)
            {
                yield return new WaitForEndOfFrame();
                SetSelectable(target);
            }
        }

        /// <summary>
        /// Set <seealso cref="EventSystem.current.currentSelectedGameObject"/>
        /// </summary>
        /// <param name="target">target object</param>
        protected virtual void SetSelectable(GameObject target)
        {
            var pointer = new PointerEventData(EventSystem.current);
            ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, pointer, ExecuteEvents.pointerExitHandler);
            EventSystem.current.SetSelectedGameObject(target, new BaseEventData(EventSystem.current));
            ExecuteEvents.Execute(target, pointer, ExecuteEvents.selectHandler);
        }

        /// <summary>
        /// Event called when an slot is selected
        /// </summary>
        /// <param name="slot"></param>
        public virtual void OnSelect(vItemSlot slot)
        {
            CreateItemDescription(slot);
        }

        /// <summary>
        /// Create item description to displayDescriptionText
        /// </summary>
        /// <param name="slot">target slot</param>
        protected virtual void CreateItemDescription(vItemSlot slot)
        {
            text = new StringBuilder();
            if (slot.item)
            {
                if (string.IsNullOrEmpty(descriptionTextFormat))
                {
                    text.AppendLine(slot.item.name);
                    text.AppendLine(slot.item.description);
                    text.AppendLine(slot.item.GetItemAttributesText());
                }
                else
                {
                    string value = descriptionTextFormat;
                    if (value.Contains("(NAME)")) value = value.Replace("(NAME)", slot.item.name);
                    if (value.Contains("(DESC)")) value = value.Replace("(DESC)", slot.item.description);
                    if (value.Contains("(ATTR)")) value = value.Replace("(ATTR)", slot.item.GetItemAttributesText());
                    text.Append(value);
                }
            }
            if (displayDescriptionText) displayDescriptionText.text = text.ToString();
            onChangeDescription.Invoke(text.ToString());
        }

        /// <summary>
        /// Inset Space in text Befour Upper case.
        /// </summary>
        /// <param name="input">text</param>
        /// <returns></returns>
        protected virtual string InsertSpaceBeforeUpperCase(string input)
        {
            var result = "";

            foreach (char c in input)
            {
                if (char.IsUpper(c))
                {
                    // if not the first letter, insert space before uppercase
                    if (!string.IsNullOrEmpty(result))
                    {
                        result += " ";
                    }
                }
                // start new word
                result += c;
            }

            return result;
        }

        /// <summary>
        /// Event called when actions is canceled
        /// </summary>
        public virtual void OnCancel()
        {
            ///TODO
        }
    }
}