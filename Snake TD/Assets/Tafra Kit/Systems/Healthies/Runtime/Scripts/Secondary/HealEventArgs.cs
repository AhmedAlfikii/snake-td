using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Healthies
{
    public class HealEventArgs : EventArgs
    {
        private float manipulatedHeal;
        private float healthBeforeHealing;

        public float HealthBeforeHealing => healthBeforeHealing;
        public float OriginalHeal { get; }
        public float ManipulatedHeal
        {
            get => manipulatedHeal;
            set => manipulatedHeal = value;
        }

        public HealEventArgs(float heal, float healthBeforeHealing)
        {
            OriginalHeal = heal;
            manipulatedHeal = heal;
            this.healthBeforeHealing = healthBeforeHealing;
        }
    }
}