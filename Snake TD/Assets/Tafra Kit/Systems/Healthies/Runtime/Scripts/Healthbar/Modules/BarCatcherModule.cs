using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.UI;

namespace TafraKit.Healthies
{
    [SearchMenuItem("Bar Catcher")]
    public class BarCatcherModule : HealthBarModule
    {
        [Header("Properties")]
        [SerializeField] private float catchingDuration = 0.25f;
        [SerializeField] private EasingType catchEasing = new EasingType(MotionType.EaseIn, new EasingEquationsParameters());

        [Header("References")]
        [SerializeField] private Slider catcherSlider;

        private IEnumerator catchingEnum;

        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        protected override void OnEnable()
        {
            healthBar.HealthUpdated += OnHealthUpdated;

            if(catchingEnum != null)
            {
                healthBar.StopCoroutine(catchingEnum);
                catchingEnum = null;
            }

            if(healthBar.Healthy.IsInitialized)
                ResetToHealthy();
            else
                healthBar.Healthy.Events.OnInitialize.AddListener(OnHealthyInitialize);
        }
        protected override void OnDisable()
        {
            healthBar.HealthUpdated -= OnHealthUpdated;
        }
        private void OnHealthyInitialize()
        {
            healthBar.Healthy.Events.OnInitialize.RemoveListener(OnHealthyInitialize);
            ResetToHealthy();
        }
        private void ResetToHealthy()
        {
            catcherSlider.value = healthBar.Healthy.NormalizedHealth;
        }
        private void OnHealthUpdated(float health, float maxHealth)
        {
            if (catchingEnum != null)
                healthBar.StopCoroutine(catchingEnum);

            catchingEnum = Catching(health, maxHealth);
            healthBar.StartCoroutine(catchingEnum);
        }

        private IEnumerator Catching(float health, float maxHealth)
        {
            float target = health / maxHealth;

            float startTime = Time.time;
            float endTime = startTime + catchingDuration;
            float startValue = catcherSlider.value;

            while(Time.time < endTime)
            {
                float t = (Time.time - startTime) / catchingDuration;

                t = MotionEquations.GetEaseFloat(t, catchEasing);

                catcherSlider.value = Mathf.Lerp(startValue, target, t);

                yield return null;
            }
            catcherSlider.value = target;

            yield break;
        }
    }
}