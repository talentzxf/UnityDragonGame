using Invector.vCharacterController;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Invector.vItemManager
{
    [RequireComponent(typeof(Selectable))]
    public class ButtonTimer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public GenericInput craftInput = new GenericInput("Mouse0", "A", "A");
        public GenericInput increaseAmountInput = new GenericInput("HorizontalArrow", "D-Pad Horizontal", "D-Pad Horizontal");
        public GenericInput decreaseAmountInput = new GenericInput("HorizontalArrow", "D-Pad Horizontal", "D-Pad Horizontal");

        public float timer = 1f;
        public UnityEngine.Events.UnityEvent onStart, onCancel, onComplete;
        public Slider.SliderEvent onChangeTimerNormalized;
        protected bool pressed;
        protected Selectable selectable;
        public vItemCraftAmountWindow amount;
        public vItemsCraftableDisplay craftDisplay;

        void Awake()
        {
            selectable = GetComponent<Selectable>();
            amount = GetComponentInChildren<vItemCraftAmountWindow>();
            craftDisplay = GetComponentInParent<vItemsCraftableDisplay>();
        }

        private void Update()
        {
            if(craftInput.GetButtonDown() && selectable && craftDisplay.CanCraftItem())
            {
                if (selectable && selectable.interactable)
                    StartCoroutine(InitTime());
            }

            if(craftInput.GetButtonUp())
            {
                pressed = false;
            }

            if(increaseAmountInput.GetButtonDown() && amount)
            {
                amount.ChangeAmount(1);
            }

            if (decreaseAmountInput.GetButtonDown() && amount)
            {
                amount.ChangeAmount(-1);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(selectable && selectable.interactable)
                StartCoroutine(InitTime());
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            pressed = false;
        }

        IEnumerator InitTime()
        {
            if (!pressed)
            {
                pressed = true;
                onStart.Invoke();
                float startTime = Time.unscaledTime;
                float currentTimer = 0;
                onChangeTimerNormalized.Invoke(currentTimer);
                while (currentTimer < 1)
                {
                    yield return null;
                    if (!pressed || (selectable && !selectable.interactable))
                    {
                        onCancel.Invoke();
                        break;
                    }
                    var current = Time.unscaledTime;
                    currentTimer = (current - startTime) / (timer);
                    onChangeTimerNormalized.Invoke(currentTimer);
                }
                if (currentTimer >= 1)
                {
                    onComplete.Invoke();
                }
            }
        }
    }
}