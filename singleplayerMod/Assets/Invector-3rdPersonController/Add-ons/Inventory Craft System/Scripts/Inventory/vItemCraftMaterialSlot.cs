using UnityEngine;
using UnityEngine.UI;

namespace Invector.vItemManager
{
    /// <summary>
    /// Slot of craftable materials
    /// </summary>
    public class vItemCraftMaterialSlot : MonoBehaviour
    {
        [SerializeField] protected Image icon;
        [SerializeField] protected Text amountText;

        /// <summary>
        /// Event called when the amount is changed
        /// </summary>
        [SerializeField] protected InputField.OnChangeEvent onChangeAmount;

        /// <summary>
        /// Set the slot icon
        /// </summary>
        /// <param name="iconSprite">sprite icon</param>
        public virtual void SetSlotIcon(Sprite iconSprite)
        {
            if (icon) icon.sprite = iconSprite;
        }

        /// <summary>
        /// Set the amount text
        /// </summary>
        /// <param name="amount">amount value</param>
        public virtual void SetSlotAmountText(string amount)
        {
            if (amountText) amountText.text = amount;
            onChangeAmount.Invoke(amount);
        }
    }
}