using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.GraphViews;

namespace TafraKit.Internal.GraphViews
{
    [SearchMenuItem("Logical/Int")]
    public class BlackboardIntCondition : BlackboardCondition
    {
        [SerializeField] private BlackboardDynamicIntGetter targetInt;
        [SerializeField] private BlackboardDynamicIntGetter value;
        [SerializeField] private NumberRelation relation;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            targetInt.Initialize(blackboardCollection);
            value.Initialize(blackboardCollection);
        }
        protected override bool PerformCheck()
        {
            return ZHelper.IsNumberRelationValid(targetInt.Value, value.Value, relation);
        }
    }
}