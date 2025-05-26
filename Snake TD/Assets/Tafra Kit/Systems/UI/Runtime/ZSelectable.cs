using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TafraKit.ZTweeners;

namespace TafraKit.UI
{
    public class ZSelectable : Selectable
    {
        #region Private Serialized Fields
        [Tooltip("Enable controls on visuals when the button changes its interactable state")]
        [SerializeField] protected bool m_EnableInteractableValuesSwapping;
        [Tooltip("Control how the visuals will change when the button changes its interactable state")]
        [SerializeField] protected UIValuesSwapper m_InteractableValuesSwapper;
        [Tooltip("Should the content of the button be nodged depending on the button's state?")]
        [SerializeField] protected bool m_NodgeContent;
        [SerializeField] protected RectTransform m_Content;
        [SerializeField] protected NodgeBlock m_NodgeBlock;
        #endregion

        #region Private Fields
        private ZTweenVector3 nodgeTween;
        private bool mimic;
        private SelectionState mimicState;
        #endregion

        #if UNITY_EDITOR
        private void Update()
        {
            if (mimic)
                DoStateTransition(mimicState, true);
        }
        #endif

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            
            if (!m_NodgeContent || !m_Content) return;

            if (nodgeTween == null)
                nodgeTween = new ZTweenVector3();

            Vector2 nodge = Vector2.zero;
            float nodgeDuration = m_NodgeBlock.defaultNodgeDuration;
            EasingType nodgeEasing = m_NodgeBlock.defaultNodgeEasing;

            switch (state)
            {
                case SelectionState.Normal:
                    nodge = m_NodgeBlock.normalNodge;
                    break;
                case SelectionState.Highlighted:
                    nodge = m_NodgeBlock.highlightedNodge;
                    break;
                case SelectionState.Pressed:
                    nodge = m_NodgeBlock.pressedNodge;
                    if (m_NodgeBlock.customPressedNodgeProperties)
                    {
                        nodgeDuration = m_NodgeBlock.pressedNodgeDuration;
                        nodgeEasing = m_NodgeBlock.pressedNodgeEasing;
                    }
                    break;
                case SelectionState.Selected:
                    nodge = m_NodgeBlock.selectedNodge;
                    break;
                case SelectionState.Disabled:
                    nodge = m_NodgeBlock.disabledNodge;
                    break;
            }

            if(m_EnableInteractableValuesSwapping && m_InteractableValuesSwapper != null)
            {
                if(state == SelectionState.Disabled)
                    m_InteractableValuesSwapper.SetFalse();
                else
                    m_InteractableValuesSwapper.SetTrue();
            }

            m_Content.ZTweenMoveLocal(nodgeTween, nodge, instant ? 0 : nodgeDuration, this).SetEasingType(nodgeEasing).SetUnscaledTimeUsage(true);
        }

        #if UNITY_EDITOR
        public void MimicNormal()
        {
            DoStateTransition(SelectionState.Normal, true);
            mimicState = SelectionState.Normal;
            mimic = true;
        }
        public void MimicHighlighted()
        {
            DoStateTransition(SelectionState.Highlighted, true);
            mimicState = SelectionState.Highlighted;
            mimic = true;
        }
        public void MimicPressed()
        {
            DoStateTransition(SelectionState.Pressed, true);
            mimicState = SelectionState.Pressed;
            mimic = true;
        }
        public void MimicSelected()
        {
            DoStateTransition(SelectionState.Selected, true);
            mimicState = SelectionState.Selected;
            mimic = true;
        }
        public void MimicDisabled()
        {
            DoStateTransition(SelectionState.Disabled, true);
            mimicState = SelectionState.Disabled;
            mimic = true;
        }

        public void RevertToNormal()
        {
            DoStateTransition(currentSelectionState, true);
            mimic = false;
        }
        #endif
    }
}