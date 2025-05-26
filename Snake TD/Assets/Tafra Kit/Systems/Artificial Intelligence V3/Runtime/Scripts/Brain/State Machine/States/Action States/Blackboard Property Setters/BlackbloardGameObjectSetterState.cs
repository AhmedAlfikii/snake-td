using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("Blackboard Property Setters/GameObject Setter"), GraphNodeName("Blackboard GameObject Setter", "GO Setter")]
    public class BlackbloardGameObjectSetterState : ActionState
    {
        [SerializeField] private BlackboardGameObjectSetter propertySetter;
        [SerializeField] private BlackboardGameObjectGetter value;
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