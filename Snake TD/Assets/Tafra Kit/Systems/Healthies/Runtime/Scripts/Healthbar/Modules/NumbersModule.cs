using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

namespace TafraKit.Healthies
{
    [SearchMenuItem("Numbers")]
    public class NumbersModule : HealthBarModule
    {
        [SerializeField] private bool displayMaxHealth;
        [SerializeField] private bool roundToInt = true;

        [Header("References")]
        [SerializeField] private TMP_Text numberTXT;

        private StringBuilder sb = new StringBuilder();

        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        protected override void OnEnable()
        {
            healthBar.HealthUpdated += OnHealthUpdated;
        }
        protected override void OnDisable()
        {
            healthBar.HealthUpdated -= OnHealthUpdated;
        }

        private void OnHealthUpdated(float health, float maxHealth)
        {
            if (roundToInt)
            { 
                health = Mathf.CeilToInt(health);
                maxHealth = Mathf.CeilToInt(maxHealth);
            }

            sb.Clear();

            sb.Append(health);

            if (displayMaxHealth)
            {
                sb.Append('/');
                sb.Append(maxHealth);
            }

            numberTXT.text = sb.ToString();
        }
    }
}