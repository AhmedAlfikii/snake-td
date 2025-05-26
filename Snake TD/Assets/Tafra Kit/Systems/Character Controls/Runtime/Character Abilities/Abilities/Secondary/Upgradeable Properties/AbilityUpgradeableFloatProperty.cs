using TafraKit.GraphViews;
using TafraKit.Mathematics;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Number/Float")]
    public class AbilityUpgradeableFloatProperty : AbilityUpgradeableGenericProperty<float>
    {
        [SerializeField] private FormulasContainer valueAtLevel = new FormulasContainer();

        protected override GenericExposableProperty<float> GetPropertyFromBlackboard(GraphBlackboard blackboard)
        {
            return blackboard.TryGetFloatProperty(propertyNameHash, -1);
        }

        public override void UpdateValue(int level)
        {
            property.value = valueAtLevel.Evaluate(level);
            property.SignalValueChange();
        }
    }
}