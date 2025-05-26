using System;
using TafraKit.GraphViews;
using TafraKit.Mathematics;
using UnityEngine;

namespace TafraKit.CharacterControls
{
    [System.Serializable]
    public class AbilityUpgradeablePropertyOld
    {
        public enum BBPropertyType
        { 
            Float,
            AdvancedFloat,
            Int
        }

        [Tooltip("Must match the ability's internal blackboard name. Works with floats, Advanced Floats and ints.")]
        [SerializeField] private string propertyName;
        [Tooltip("The value of the property per ability level.")]
        [SerializeField] private FormulasContainer formula;

        [NonSerialized] private BBPropertyType propertyType;
        [NonSerialized] private GenericExposableProperty<float> floatProperty;
        [NonSerialized] private GenericExposableProperty<TafraAdvancedFloat> advancedFloatProperty;
        [NonSerialized] private GenericExposableProperty<int> intProperty;

        public BBPropertyType PropertyType => propertyType;
        public GenericExposableProperty<float> FloatProperty => floatProperty;
        public GenericExposableProperty<TafraAdvancedFloat> AdvancedFloatProperty => advancedFloatProperty;
        public GenericExposableProperty<int> IntProperty => intProperty;

        public string PropertyName => propertyName;
        public FormulasContainer Formula => formula;

        public void SetFloatProperty(GenericExposableProperty<float> floatProperty)
        { 
            this.floatProperty = floatProperty;
            propertyType = BBPropertyType.Float;
        }
        public void SetAdvancedFloatProperty(GenericExposableProperty<TafraAdvancedFloat> advancedProperty)
        { 
            this.advancedFloatProperty = advancedProperty;
            propertyType = BBPropertyType.AdvancedFloat;
        }
        public void SetIntProperty(GenericExposableProperty<int> intProperty)
        { 
            this.intProperty = intProperty;
            propertyType = BBPropertyType.Int;
        }
    }
}