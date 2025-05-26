using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    [System.Serializable]
    public class ScriptableFloatChange
    {
        [SerializeField] private ScriptableFloat scriptableFloat;
        [SerializeField] private float changeAmount;

        public float ChangeAmount 
        {
            get
            {
                return changeAmount;
            }
            set
            {
                changeAmount = value;
            }
        }
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
        public void Add(bool hidden = false)
        {
            scriptableFloat.Add(ChangeAmount, hidden);
        }
        public void Deduct(bool hidden = false)
        {
            scriptableFloat.Deduct(ChangeAmount, hidden);
        }
        public void Set(bool hidden = false)
        {
            scriptableFloat.Set(ChangeAmount, hidden);
        }
        public bool IsAffordable()
        {
            bool affordable = scriptableFloat.Value >= Mathf.Abs(ChangeAmount);

            return affordable;
        }
        #endregion
    }
}
