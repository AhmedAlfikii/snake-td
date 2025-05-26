using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Conditions
{
    [Serializable]
    public class ScriptableFloatCondition : Condition
    {
        [SerializeField] private ScriptableFloat scriptableFloat;
        [SerializeField] private float value;
        [SerializeField] private NumberRelation relation;
        [SerializeField] private bool activelyListen;

        public ScriptableFloat Float => scriptableFloat;
        public float Value => value;

        protected override void OnActivate()
        {
            if(scriptableFloat == null)
                return;

            if(activelyListen)
                scriptableFloat.OnValueChange.AddListener(OnSFValueChange);
        }
        protected override void OnDeactivate()
        {
            if(scriptableFloat == null)
                return;

            if(activelyListen)
                scriptableFloat.OnValueChange.RemoveListener(OnSFValueChange);
        }

        private void OnSFValueChange(float sfVal)
        {
            Check();
        }

        protected override bool PerformCheck()
        {
            if(scriptableFloat == null)

                return true;

            bool valid = ZHelper.IsNumberRelationValid(scriptableFloat.Value, value, relation);

            return valid;
        }
    }
}