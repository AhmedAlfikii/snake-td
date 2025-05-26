using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit;
using TafraKit.AI3;
using TafraKit.GraphViews;

namespace TafraKit.Internal.AI3
{
    [SearchMenuItem("General/Wait"), GraphNodeName("Wait", "Wait")]
    public class WaitState : ActionState
    {
        [SerializeField] private BlackboardFloatGetter waitDuration = new BlackboardFloatGetter(1);
        [SerializeField] private string msgPrefix;

        private float endTime;

        protected override void OnInitialize()
        {
            waitDuration.Initialize(agent.BlackboardCollection);
        }
        protected override void OnPlay()
        {
            float value = waitDuration.Value;

            endTime = Time.time + value;
        }

        public override void Update()
        {
            if(Time.time > endTime)
                Complete();
        }
    }
}
