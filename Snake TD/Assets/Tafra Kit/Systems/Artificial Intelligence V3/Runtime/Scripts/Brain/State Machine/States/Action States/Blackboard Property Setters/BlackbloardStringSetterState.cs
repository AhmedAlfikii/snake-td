using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Blackboard Property Setters/String Setter"), GraphNodeName("Blackboard String Setter", "String Setter")]
    public class BlackbloardStringSetterState : ActionState
    {
        [SerializeField] private BlackboardStringSetter propertySetter;
        [SerializeField] private BlackboardStringGetter value;
        [SerializeField] private bool completeOnPlay = true;

        protected override void OnInitialize()
        {
            propertySetter.Initialize(agent.BlackboardCollection);
            value.Initialize(agent.BlackboardCollection);
        }

        protected override void OnPlay()
        {
            propertySetter.SetValue(value.Value);

            if (completeOnPlay)
                Complete();
        }
        public override void Update()
        {
            propertySetter.SetValue(value.Value);
        }
    }
}