using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.GraphViews
{
    [SearchMenuItem("Logical/Bool")]
    public class BlackboardBoolCondition : BlackboardCondition
    {
        [SerializeField] private BlackboardBoolGetter target;
        [SerializeField] private bool targetState;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            target.Initialize(blackboardCollection);
        }
        protected override bool PerformCheck()
        {
            bool value = target.Value;

            return value == targetState;
        }
    }
}