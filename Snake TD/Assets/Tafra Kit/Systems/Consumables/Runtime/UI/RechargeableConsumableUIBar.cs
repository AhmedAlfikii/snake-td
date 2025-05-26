using System;
using System.Collections;
using TafraKit.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZUI;

namespace TafraKit.Consumables.UI
{
    public class RechargeableConsumableBar : ConsumableUIBar
    {
        #region Serialized Fields
        [Header("Slider")]
        [SerializeField] private Slider fillSlider;

        [Header("Timer")]
        [SerializeField] private bool showTimer = true;
        [SerializeField] private GameObject timerGO;
        [SerializeField] private TextMeshProUGUI timerTxt;

        [Header("Decorations")]
        [SerializeField] private string maxValuePrefix = "<size=80%>";
        #endregion

        #region Private Fields
        private RechargeableConsumable consumable;
        #endregion
        
        #region Monobehaviour Messages
        private void Awake()
        {
            consumable = (RechargeableConsumable)scriptableFloat;

            if (timerGO != null)
                timerGO.SetActive(false);
        }
        protected override void Start()
        {
            base.Start();
            
            if(consumable.IsRecharging)
                OnRechargeStarted();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            
            consumable.OnRechargeStarted.AddListener(OnRechargeStarted);
            consumable.OnRechargeTimerUpdated.AddListener(OnRechargeTimerUpdated);
            consumable.OnRechargeCompleted.AddListener(OnRechargeCompleted);
            consumable.OnRechargeCapChanged.AddListener(OnAutoRechargeCapValueChanged);

            if(autoUpdate && fillSlider != null)
                fillSlider.value = consumable.DisplayValue / consumable.CurrentRechargeCap;
        }
        protected override void OnDisable()
        {
            base.OnDisable();

            consumable.OnRechargeStarted.RemoveListener(OnRechargeStarted);
            consumable.OnRechargeTimerUpdated.RemoveListener(OnRechargeTimerUpdated);
            consumable.OnRechargeCompleted.RemoveListener(OnRechargeCompleted);
            consumable.OnRechargeCapChanged.RemoveListener(OnAutoRechargeCapValueChanged);
        }
        #endregion

        #region Private Functions
        protected override void UpdateAmountTxt()
        {
            amountTXT.text = displayAsCompact ? $"{displayedValue.ToCompactNumberString()}{maxValuePrefix} / {consumable.CurrentRechargeCap.ToCompactNumberString()}"
                : $"{displayedValue}{maxValuePrefix} / {consumable.CurrentRechargeCap}";
        }
        #endregion

        #region Callbacks
        private void OnRechargeStarted()
        {
            if(showTimer)
                timerGO.SetActive(true);
        }
        private void OnRechargeTimerUpdated(TimeSpan remainingTime)
        {
            if(showTimer) 
                timerTxt.text = $"{remainingTime.Minutes:00}:{remainingTime.Seconds:00}";
        }
        private void OnRechargeCompleted()
        {
            if(showTimer)
                timerGO.SetActive(false);
        }
        private void OnAutoRechargeCapValueChanged()
        {
            UpdateAmountTxt();
        }
        protected override void OnDisplayValueChange(float value)
        {
            base.OnDisplayValueChange(value);

            if(fillSlider != null)
                fillSlider.value = value / consumable.CurrentRechargeCap;
        }
        #endregion
    }
}
