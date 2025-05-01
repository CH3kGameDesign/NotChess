using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(Button_HoldVar))]
    public class Button_Hold : Button
    {
        public Button.ButtonClickedEvent onClickSingle { get; set; }
        public ButtonClickedEvent onHold { get; set; }
        public ButtonClickedEvent onRelease { get; set; }


        public float holdDelay = 0.1f;
        public float holdRepeat = 0.05f;
        private float holdTimer = 0f;
        private bool holdLoop = false;

        private bool isPressed = false;

        public void Update()
        {
            if (isPressed)
            {
                if (holdLoop)
                {
                    if (holdTimer >= holdRepeat && holdRepeat > 0)
                    {
                        holdTimer = 0;
                        onHold.Invoke();
                    }
                }
                else
                {
                    if (holdTimer >= holdDelay)
                    {
                        holdTimer = 0;
                        holdLoop = true;
                        onHold.Invoke();
                    }
                }
                holdTimer += Time.unscaledDeltaTime;
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            isPressed = true;
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (!holdLoop)
                onClickSingle.Invoke();
            else
                onRelease.Invoke();
            isPressed = false;
            holdLoop = false;
            holdTimer = 0;
        }
    }
}
