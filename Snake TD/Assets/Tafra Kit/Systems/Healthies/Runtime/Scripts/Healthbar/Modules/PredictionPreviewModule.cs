using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.UI;

namespace TafraKit.Healthies
{
    [SearchMenuItem("Prediction Preview")]
    public class PredictionPreviewModule : HealthBarModule
    {
        [Header("References")]
        [SerializeField] private Slider bar;
        [SerializeField] private Image barFill;
        [SerializeField] private Slider barCatcher;
        [SerializeField] private Image barCatcherFill;
        [SerializeField] private TextMeshProUGUI number;
        [SerializeField] private TextMeshProUGUI changeNumber;

        [Header("Damage Colors")]
        [SerializeField] private Color barDamagePreviewColor;
        [SerializeField] private Color barCatcherDamagePreviewColor;
        [SerializeField] private Color numberDamagePreviewColor;

        [Header("Heal Colors")]
        [SerializeField] private Color barHealPreviewColor;
        [SerializeField] private Color barCatcherHealPreviewColor;
        [SerializeField] private Color numberHealPreviewColor;

        [NonSerialized] private Color barNormalColor;
        [NonSerialized] private Color barCatcherNormalColor;
        [NonSerialized] private Color numberNormalColor;
        [NonSerialized] private bool isPreviewing;

        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        protected override void OnInitialize()
        {
            barNormalColor = barFill.color;
            barCatcherNormalColor = barCatcherFill.color;
            numberNormalColor = number.color;
        }
        protected override void OnEnable()
        {

        }
        protected override void OnDisable()
        {
            if(isPreviewing)
                EndPreview();
        }

        public void StartDamagePreview(float damage)
        {
            float previewHealth = healthBar.Healthy.CurrentHealth - damage;
            float barPreviewPercentage = previewHealth / healthBar.Healthy.CurrentMaxHealth;
            float catcherPreviewPercentage = healthBar.Healthy.NormalizedHealth;

            bar.value = barPreviewPercentage;
            barCatcher.value = catcherPreviewPercentage;
            number.text = Mathf.Max(Mathf.CeilToInt(previewHealth), 0).ToString();

            barFill.color = barDamagePreviewColor;
            barCatcherFill.color = barCatcherDamagePreviewColor;
            number.color = numberDamagePreviewColor;

            changeNumber.text = $"-{damage}";
            changeNumber.color = numberDamagePreviewColor;
            changeNumber.gameObject.SetActive(true);

            isPreviewing = true;
        }
        public void StartHealPreview(float heal)
        { 
            //TODO: Make sure the number caps at max health.
            throw new NotImplementedException();
        }
        public void EndPreview()
        {
            barFill.color = barNormalColor;
            barCatcherFill.color = barCatcherNormalColor;
            number.color = numberNormalColor;

            float currentNormalizedHealth = healthBar.Healthy.NormalizedHealth;

            bar.value = currentNormalizedHealth;
            barCatcher.value = currentNormalizedHealth;
            number.text = Mathf.CeilToInt(healthBar.Healthy.CurrentHealth).ToString();
           
            changeNumber.gameObject.SetActive(false);

            isPreviewing = false;
        }
    }
}