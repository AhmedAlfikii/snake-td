using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Blackboard Property Setters/Float Setter"), GraphNodeName("Blackboard Float Setter", "Float Setter")]
    public class BlackbloardFloatSetterState : ActionState
    {
        [SerializeField] private BlackboardFloatSetter propertyToSet;
        [SerializeField] private BlackboardFloatGetter value;
        [SerializeField] private bool completeOnPlay = true;

        protected override void OnInitialize()
        {
            propertyToSet.Initialize(agent.BlackboardCollection);
            value.Initialize(agent.BlackboardCollection);
        }

        protected override void OnPlay()
        {
            propertyToSet.SetValue(value.Value);

            if (completeOnPlay)
                Complete();
        }
        public override void Update()
        {
            propertyToSet.SetValue(value.Value);
        }
    }
}