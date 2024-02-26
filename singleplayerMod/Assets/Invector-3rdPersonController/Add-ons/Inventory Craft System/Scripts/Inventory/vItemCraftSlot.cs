using UnityEngine.EventSystems;

namespace Invector.vItemManager
{
    /// <summary>
    /// Slot of the craftables items
    /// </summary>
    public class vItemCraftSlot : vItemSlot
    {
        /// <summary>
        /// Update all slot Displays
        /// </summary>
        public override void UpdateDisplays()
        {
            if (item != null && this.gameObject.activeSelf && displayAmountText)
            {
                var text = "";
                var inventory = GetComponentInParent<vInventory>();
                if (inventory)
                {
                    var count = inventory.GetAllAmount(item.id);
                    text = "x" + count.ToString();
                }
                else
                {
                    text = "";
                }
                displayAmountText.text = text;
            }
        }

        protected override void ChangeDisplayAmount(vItem item)
        {

        }

        #region UnityEngine.EventSystems Implementation
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (isValid)
                {
                    EventSystem.current.SetSelectedGameObject(this.gameObject);
                    onClick.Invoke();
                    if (onSubmitSlotCallBack != null)
                        onSubmitSlotCallBack(this);
                }
            }
        }
        #endregion
    }
}
