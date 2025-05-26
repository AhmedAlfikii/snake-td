using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.Mathematics;

namespace TafraKit
{
    [System.Serializable]
    public class ScriptableFloatChangeEquation
    {
        [SerializeField] private ScriptableFloat scriptableFloat;
        [SerializeField] private ScriptableFloat equationInput;
        [SerializeField] private FormulasContainer changeAmount;

        public ScriptableFloat Float
        {
            get
            {
                return scriptableFloat;
            }
            set
            {
                scriptableFloat = value;
            }
        }

        #region Public Fields
        /// <summary>
        /// Get the change amount based on the assigned equation input scriptable float.
        /// </summary>
        /// <returns></returns>
        public float GetChangeAmount()
        {
            return changeAmount.Evaluate(equationInput.Value);
        }
        /// <summary>
        /// Get the change amount based on the sent input.
        /// </summary>
        /// <returns></returns>
        public float GetChangeAmount(float input)
        {
            return changeAmount.Evaluate(input);
        }
        public void Add(bool hidden = false)
        {
            scriptableFloat.Add(changeAmount.Evaluate(equationInput.Value), hidden);
        }
        public void Deduct(bool hidden = false)
        {
            scriptableFloat.Deduct(changeAmount.Evaluate(equationInput.Value), hidden);
        }
        public bool IsAffordable()
        {
            bool affordable = scriptableFloat.Value >= Mathf.Abs(changeAmount.Evaluate(equationInput.Value));

            return affordable;
        }
        #endregion
    }
}
