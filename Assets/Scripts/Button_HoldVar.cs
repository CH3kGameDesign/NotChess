using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(Button_Hold))]
    public class Button_HoldVar : MonoBehaviour
    {
        public Button.ButtonClickedEvent onClickSingle;
        public Button.ButtonClickedEvent onHold;
        public Button.ButtonClickedEvent onRelease;


        public float holdDelay = 0.1f;
        public float holdRepeat = 0.05f;

        public void Start()
        {
            Button_Hold _button = GetComponent<Button_Hold>();
            _button.onClickSingle = onClickSingle;
            _button.onHold = onHold;
            _button.onRelease = onRelease;
            _button.holdDelay = holdDelay;
            _button.holdRepeat = holdRepeat;
        }
    }
}
