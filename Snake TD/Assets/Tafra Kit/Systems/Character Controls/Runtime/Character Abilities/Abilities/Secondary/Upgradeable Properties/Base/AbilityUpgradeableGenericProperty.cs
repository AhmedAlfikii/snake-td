using TafraKit.GraphViews;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    public abstract class AbilityUpgradeableGenericProperty<T> : AbilityUpgradeableProperty
    {
        protected GenericExposableProperty<T> property;

        public override void OnInitialize(GraphBlackboard blackboard)
        {
            property = GetPropertyFromBlackboard(blackboard);
        }

        protected abstract GenericExposableProperty<T> GetPropertyFromBlackboard(GraphBlackboard blackboard);
    }
}