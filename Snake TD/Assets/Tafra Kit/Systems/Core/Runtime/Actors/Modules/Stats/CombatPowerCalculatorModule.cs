using System;
using System.Collections.Generic;
using TafraKit.Loot;
using TafraKit.Mathematics;
using TafraKit.RPG;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit
{
    [SearchMenuItem("Stats/Combat Power Calculator")]
    public class CombatPowerCalculatorModule : TafraActorModule
    {
        [System.Serializable]
        public class Contributer
        {
            public ScriptableFloat scriptableFloat;
            public FormulasContainer formula;
        }

        [SerializeField] private ScriptableFloat output;
        [SerializeField] private List<Contributer> contributers = new List<Contributer>();

        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        protected override void OnEnable()
        {
            base.OnEnable();

            Recalculate();

            for (int i = 0; i < contributers.Count; i++)
            {
                var contributer = contributers[i];

                contributer.scriptableFloat.OnValueChange.AddListener(OnContributerValueChange);
            }
        }
        protected override void OnDisable()
        {
            base.OnDisable();

            for(int i = 0; i < contributers.Count; i++)
            {
                var contributer = contributers[i];

                contributer.scriptableFloat.OnValueChange.RemoveListener(OnContributerValueChange);
            }
        }

        private void OnContributerValueChange(float newValue)
        {
            Recalculate();
        }

        private void Recalculate()
        {
            float sum = 0;

            for (int i = 0; i < contributers.Count; i++)
            {
                var contributer = contributers[i];

                sum += contributer.formula.Evaluate(contributer.scriptableFloat.Value);
            }

            output.Set(sum);
        }
    }
}