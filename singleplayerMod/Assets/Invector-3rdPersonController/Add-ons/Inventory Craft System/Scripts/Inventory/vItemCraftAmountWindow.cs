using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Invector.vItemManager
{
    /// <summary>
    /// Window that control the craftable item amount
    /// </summary>
    public class vItemCraftAmountWindow : MonoBehaviour
    {
        public vItemsCraftMaterialDisplay itemWindowDisplay;
        public Text amountDisplay;

        /// <summary>
        /// Event called when the item amount is changed
        /// </summary>
        public InputField.OnChangeEvent onChangeAmount;

        /// <summary>
        /// Event called when the item can be crafted
        /// </summary>
        public UnityEvent onEnableCraft;

        /// <summary>
        /// Event called when the item can't be crafted
        /// </summary>
        public UnityEvent onDisableCraft;

        protected void OnEnable()
        {
            ResetAmount();
        }

        private void Start()
        {
            ResetAmount();
        }

        /// <summary>
        /// Reset the <seealso cref="vItemWindowDisplay.amount"/>
        /// </summary>
        public void ResetAmount()
        {
            if (itemWindowDisplay)
            {
                itemWindowDisplay.amount = 1;
                if (amountDisplay)
                    amountDisplay.text = "01";
                onChangeAmount.Invoke("01");
            }
        }
        /// <summary>
        /// Increase or decrease <seealso cref="vItemWindowDisplay.amount"/>
        /// </summary>
        /// <param name="value">amount value</param>
        public virtual void ChangeAmount(int value)
        {
            if (itemWindowDisplay)
            {
                itemWindowDisplay.amount += value;
                if (amountDisplay)
                    amountDisplay.text = itemWindowDisplay.amount.ToString("00");
                onChangeAmount.Invoke(itemWindowDisplay.amount.ToString("00"));

                if (itemWindowDisplay.CanCraft())
                    onEnableCraft.Invoke();
                else
                    onDisableCraft.Invoke();
            }
        }
    }
}