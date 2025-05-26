using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Conditions
{
    [Serializable]
    public class ConditionsGroup : Condition
    {
        [Tooltip("Which conditions should be satisfied for this group to be considered satisfied?")]
        [SerializeField] private ConditionsGroupSatisfactionRequirement satisfactionRequirement;
        [SerializeReference] private Condition[] conditions = new Condition[] { };

        [NonSerialized] private int conditionsCount;
        [NonSerialized] private int remainingConditionsCount;

        public Condition[] Conditions => conditions;

        protected override void OnInitialize()
        {
            conditionsCount = conditions.Length;
        }
        protected override void OnActivate()
        {
            remainingConditionsCount = conditionsCount;

            for(int i = 0; i < conditionsCount; i++)
            {
                conditions[i].OnSatisfied = OnConditionSatisfied;
                conditions[i].Activate(checkPerformedOnActivation);
            }
        }
        protected override void OnDeactivate()
        {
            for(int i = 0; i < conditionsCount; i++)
            {
                conditions[i].Deactivate();
                conditions[i].OnSatisfied = null;
            }
        }

        private void OnConditionSatisfied()
        {
            remainingConditionsCount--;

            if(satisfactionRequirement == ConditionsGroupSatisfactionRequirement.All && remainingConditionsCount <= 0)
                Satisfy();
            else if(satisfactionRequirement == ConditionsGroupSatisfactionRequirement.Any && remainingConditionsCount < conditionsCount)
                Satisfy();
        }
        
        protected override bool PerformCheck()
        {
            if(conditionsCount == 0)
                return true;

            int satisfiedConditions = 0;
            for(int i = 0; i < conditionsCount; i++)
            {
                if(conditions[i].Check())
                    satisfiedConditions++;
            }

            bool satisfied;
            switch(satisfactionRequirement)
            {
                case ConditionsGroupSatisfactionRequirement.All:
                    satisfied = satisfiedConditions >= conditionsCount;
                    break;
                case ConditionsGroupSatisfactionRequirement.Any:
                    satisfied = satisfiedConditions > 0;
                    break;
                case ConditionsGroupSatisfactionRequirement.None:
                    satisfied = satisfiedConditions == 0;
                    break;
                default:
                    satisfied = true;
                    break;
            }

            return satisfied;
        }
    }
}