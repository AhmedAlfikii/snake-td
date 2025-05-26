using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using TafraKit.Internal.AI3;

namespace TafraKit.UI
{
    [RequireComponent(typeof(LayoutElement))]
    public class ChainCell : MonoBehaviour
    {
        [Tooltip("The preferred width that a cell should change to once it's selected.")]
        [SerializeField] private float selectedWidth;
        [Tooltip("The preferred height that a cell should change to once it's selected.")]
        [SerializeField] private float selectedHeight;

        [Space()]

        [Tooltip("The button that will cause this cell to be selected when clicked.")]
        [SerializeField] private ZButton myButton;

        [Header("Animation")]
        [SerializeField] private bool useUnscaledTime = true;
        [SerializeField] private float selectAnimationDuration = 0.75f;
        [SerializeField] private EasingType selectAnimationEasing;
        [SerializeField] private float deselectAnimationDuration = 0.35f;
        [SerializeField] private EasingType deselectAnimationEasing;

        #region Public Events
        public UnityEvent OnSelect;
        public UnityEvent OnDeselect;
        #endregion

        #region Private Fields
        private SelectableCellChain chain;
        private LayoutElement layoutElement;
        private int myIndex;
        private bool isHorizontal;
        private float normalWidth;
        private float normalHeight;
        private IEnumerator changingSizeEnum;
        #endregion

        #region Public Properties
        public bool IsSelected { get; private set; }
        public ZButton MyButton => myButton;
        #endregion

        #region Private Properties
        private float Width {
            get
            {
                if (isHorizontal)
                    return layoutElement.preferredWidth;
                else
                    return layoutElement.minWidth;
            }
            set
            {
                if (isHorizontal)
                    layoutElement.preferredWidth = value;
                else
                {
                    if (layoutElement.preferredWidth > -1)
                        layoutElement.minWidth = value;
                }
            }
        }
        private float Height
        {
            get
            {
                if (isHorizontal)
                    return layoutElement.minHeight;
                else
                    return layoutElement.preferredHeight;
            }
            set
            {
                if (isHorizontal)
                {
                    if (layoutElement.preferredHeight > -1)
                        layoutElement.minHeight = value;
                }
                else
                    layoutElement.preferredHeight = value;
            }
        }
        #endregion

        #region MonoBehaviour Messages
        private void OnEnable()
        {
            if (myButton != null)
                myButton.onClick.AddListener(OnButtonClick);
        }


        private void OnDisable()
        {
            if (myButton != null)
                myButton.onClick.RemoveListener(OnButtonClick);
        }
        #endregion

        #region Callbacks
        private void OnButtonClick()
        {
            if (chain == null)
                return;

            Select();
        }
        #endregion

        #region Private Functions
        void ChangeSize(float width, float height, float duration, EasingType easing)
        {
            if (changingSizeEnum != null)
                StopCoroutine(changingSizeEnum);

            changingSizeEnum = ChangingSize(width, height, duration, easing);
            StartCoroutine(changingSizeEnum);
        }

        IEnumerator ChangingSize(float width, float height, float duration, EasingType easing)
        {
            float startTime = !useUnscaledTime ? Time.time : Time.unscaledTime;

            float startWidth = Width;
            float startHeight = Height;

            float time = !useUnscaledTime ? Time.time : Time.unscaledTime;
            while(time < startTime + selectAnimationDuration)
            {
                time = !useUnscaledTime ? Time.time : Time.unscaledTime;

                float t = (time - startTime) / selectAnimationDuration;
                t = MotionEquations.GetEaseFloat(t, selectAnimationEasing);

                Width = Mathf.LerpUnclamped(startWidth, width, t);
                Height = Mathf.LerpUnclamped(startHeight, height, t);

                yield return null;
            }

            Width = width;
            Height = height;
        }
        #endregion

        #region Public Functions
        public void Initialize(SelectableCellChain chain, bool isHorizontal, int myIndex)
        {
            layoutElement = GetComponent<LayoutElement>();

            this.chain = chain;
            this.isHorizontal = isHorizontal;
            this.myIndex = myIndex;

            normalWidth = layoutElement.preferredWidth;
            normalHeight = layoutElement.preferredHeight;

            Width = normalWidth;
            Height = normalHeight;
        }

        public void Select()
        {
            chain.SelectCell(myIndex);
        }
        public void SetAsSelected()
        {
            ChangeSize(selectedWidth, selectedHeight, selectAnimationDuration, selectAnimationEasing);
            IsSelected = true;
            OnSelect?.Invoke();
        }

        public void SetAsDeselected()
        {
            ChangeSize(normalWidth, normalHeight, deselectAnimationDuration, deselectAnimationEasing);
            IsSelected = false;
            OnDeselect?.Invoke();
        }
        #endregion
    }
}
