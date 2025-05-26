using TafraKit.GraphViews;
using TafraKit.Mathematics;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Number/Advanced Float")]
    public class AbilityUpgradeableAdvancedFloatProperty : AbilityUpgradeableGenericProperty<TafraAdvancedFloat>
    {
        [SerializeField] private FormulasContainer valueAtLevel = new FormulasContainer();

        public override void OnInitialize(GraphBlackboard blackboard)
        {
            base.OnInitialize(blackboard);
           
            property.value.MyType = TafraAdvancedFloat.FloatType.Value;
        }

        protected override GenericExposableProperty<TafraAdvancedFloat> GetPropertyFromBlackboard(GraphBlackboard blackboard)
        {
            return blackboard.TryGetAdvancedFloatProperty(propertyNameHash, -1);
        }

        public override void UpdateValue(int level)
        {
            property.value.Value = valueAtLevel.Evaluate(level);
            property.SignalValueChange();
        }
    }
}