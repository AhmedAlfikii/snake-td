using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Blackboard Property Setters/Vector3 Setter"), GraphNodeName("Blackboard Vector3 Setter", "Vector3 Setter")]
    public class BlackbloardVector3SetterState : ActionState
    {
        [SerializeField] private BlackboardVector3Setter propertySetter;
        [SerializeField] private BlackboardVector3Getter value;
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