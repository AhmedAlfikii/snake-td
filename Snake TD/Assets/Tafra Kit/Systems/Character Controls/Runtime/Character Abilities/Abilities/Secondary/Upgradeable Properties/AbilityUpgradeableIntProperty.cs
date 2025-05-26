using TafraKit.GraphViews;
using TafraKit.Mathematics;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Number/Int")]
    public class AbilityUpgradeableIntProperty : AbilityUpgradeableGenericProperty<int>
    {
        [SerializeField] private FormulasContainer valueAtLevel = new FormulasContainer();

        protected override GenericExposableProperty<int> GetPropertyFromBlackboard(GraphBlackboard blackboard)
        {
            return blackboard.TryGetIntProperty(propertyNameHash, -1);
        }

        public override void UpdateValue(int level)
        {
            property.value = Mathf.RoundToInt(valueAtLevel.Evaluate(level));
            property.SignalValueChange();
        }
    }
}