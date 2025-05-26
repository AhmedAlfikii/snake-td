using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TafraKit.UI
{
    public class ZButton : ZSelectable, IPointerClickHandler, ISubmitHandler
    {
        [Serializable]
        public class ButtonClickedEvent : UnityEvent { }

        [SerializeField]
        private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();
        [SerializeField] 
        private SFXClip m_OnClickSFX;

        protected ZButton() 
        {
            m_NodgeContent = true;
            m_NodgeBlock.defaultNodgeDuration = 0.5f;
            m_NodgeBlock.defaultNodgeEasing = new EasingType(MotionType.EaseOutElastic, new EasingEquationsParameters(new EasingEquationsParameters.EaseInOutElasticParameters(0.5f)));
            m_NodgeBlock.customPressedNodgeProperties = true;
            m_NodgeBlock.pressedNodgeEasing = new EasingType(MotionType.Linear, new EasingEquationsParameters());
            m_NodgeBlock.pressedNodgeDuration = 0.05f;
        }

        public ButtonClickedEvent onClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }

        private void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;

            UISystemProfilerApi.AddMarker("ZButton.onClick", this);
            m_OnClick.Invoke();

            if (m_OnClickSFX.Clip != null)
                SFXPlayer.Play(m_OnClickSFX);
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            Press();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            Press();

            // if we get set disabled during the press
            // don't run the coroutine.
            if (!IsActive() || !IsInteractable())
                return;

            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(OnFinishSubmit());
        }

        //TODO: Check this.
        private IEnumerator OnFinishSubmit()
        {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }
    }
}