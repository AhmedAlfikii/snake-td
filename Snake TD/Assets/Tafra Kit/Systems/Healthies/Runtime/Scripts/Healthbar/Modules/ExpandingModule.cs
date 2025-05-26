using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using TafraKit.MotionFactory;

namespace TafraKit.Healthies
{
    [SearchMenuItem("Expanding")]
    public class ExpandingModule : HealthBarModule
    {
        [SerializeField] private RectTransform expandingBar;
        [SerializeField] private float widthPerOneHP;
        [SerializeField] private float maxWidth;
        [SerializeField] private float expandingDuration = 0.5f;
        [SerializeField] private EasingType expandingType;

        [Header("References")]
        [SerializeField] private VisibilityMotionController expandingMotionController;

        private IEnumerator expandingEnum;

        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        protected override void OnEnable()
        {
            if(healthBar.Healthy.IsInitialized)
                OnHealthyInitialized();
            else
                healthBar.Healthy.Events.OnInitialize.AddListener(OnHealthyInitialized);
        }
        protected override void OnDisable()
        {
            healthBar.Healthy.Events.OnMaxHealthChange.RemoveListener(OnMaxHealthChange);
        }
        private void OnHealthyInitialized()
        {
            healthBar.Healthy.Events.OnInitialize.RemoveListener(OnHealthyInitialized);

            healthBar.Healthy.Events.OnMaxHealthChange.AddListener(OnMaxHealthChange);

            UpdateWidth(true);
        }

        private void OnMaxHealthChange(float newMaxHealth)
        {
            UpdateWidth(false);
        }

        private void UpdateWidth(bool instant)
        {
            if (expandingEnum != null)
            {
                healthBar.StopCoroutine(expandingEnum);
                expandingEnum = null;
            }

            float targetWidth = Mathf.Clamp(healthBar.Healthy.CurrentMaxHealth * widthPerOneHP, 0, maxWidth);

            if (instant)
            {
                expandingBar.sizeDelta = new Vector2(targetWidth, expandingBar.sizeDelta.y);
            }
            else
            {
                expandingMotionController.Show(false);

                float startWidth = expandingBar.sizeDelta.x;

                expandingEnum = CompactCouroutines.CompactCoroutine(0, expandingDuration, false, (t) =>
                {
                    t = MotionEquations.GetEaseFloat(t, expandingType);
                    expandingBar.sizeDelta = new Vector2(Mathf.LerpUnclamped(startWidth, targetWidth, t), expandingBar.sizeDelta.y);
                },
                onEnd: () => {
                    expandingMotionController.Hide(false);
                });

                healthBar.StartCoroutine(expandingEnum);
            }
        }
    }
}