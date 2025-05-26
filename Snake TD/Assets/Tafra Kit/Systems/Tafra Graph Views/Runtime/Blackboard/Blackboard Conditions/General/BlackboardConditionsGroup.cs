using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.Conditions;

namespace TafraKit.Internal.GraphViews
{
    [System.Serializable, SearchMenuItem("General/Conditions Group")]
    public class BlackboardConditionsGroup : BlackboardCondition
    {
        [Tooltip("Which conditions should be satisfied for this group to be considered satisfied?")]
        [SerializeField] private ConditionsGroupSatisfactionRequirement satisfactionRequirement;
        [SerializeReference] private List<BlackboardCondition> conditions = new List<BlackboardCondition>();

        private int conditionsCount;

        public List<BlackboardCondition> Conditions => conditions;

        protected override void OnInitialize()
        {
            conditionsCount = conditions.Count;

            for(int i = 0; i < conditionsCount; i++)
            {
                conditions[i].SetDependencies(actor, blackboardCollection);
            }
        }
        protected override void OnActivate()
        {
            for(int i = 0; i < conditionsCount; i++)
            {
                conditions[i].Activate();
            }
        }
        protected override void OnDeactivate()
        {
            for(int i = 0; i < conditionsCount; i++)
            {
                conditions[i].Deactivate();
            }
        }

        protected override bool PerformCheck()
        {
            if(conditionsCount == 0)
                return true;

            int satisfiedConditions = 0;
            for(int i = 0; i < conditionsCount; i++)
            {
                if(conditions[i].Check())
                {
                    satisfiedConditions++;

                    //No need to continue checking the rest of the conditions if we just want at least one of them to satisfy.
                    if(satisfactionRequirement == ConditionsGroupSatisfactionRequirement.Any)
                        return true;
                }
                //No need to continue checking the rest of the conditions if one failed and we want all of them to satisfy.
                else if(satisfactionRequirement == ConditionsGroupSatisfactionRequirement.All)
                    return false;
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