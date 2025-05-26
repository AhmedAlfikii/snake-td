using System;
using UnityEngine;

namespace TafraKit.GraphViews
{
    [Serializable]
    public abstract class BlackboardPropertyValueGetter<T> : BlackboardGenericPropertyReference<T>
    {
        [Tooltip("This value will be used if the target blackboard is none or if fetching the property from the blackboard failed.")]
        [SerializeField] protected T defaultValue;
        [Tooltip("If enabled, the default value will be used as a fallback in case fetching the target blackboard proprety failed.")]
        [SerializeField] protected bool enableDefault;

        public BlackboardPropertyValueGetter() { }
        public BlackboardPropertyValueGetter(T defaultValue) 
        {
            this.defaultValue = defaultValue;
        }
        public BlackboardPropertyValueGetter(BlackboardPropertyValueGetter<T> other) : base(other) 
        {
            defaultValue = other.defaultValue;
            enableDefault = other.enableDefault;
        }

        /// <summary>
        /// Returns the value of the variable depending on the target blackboard. If not found in the target blackboard, the default value will be returned.
        /// </summary>
        public virtual T Value
        {
            get
            {
                if (targetBlackboard == TargetBlackboard.None)
                    return defaultValue;

                if(targetBlackboard == TargetBlackboard.Secondary && secondaryBlackboard == null)
                    TafraDebugger.Log("Blackboard Property Getter", "Secondary blackboard is not set. Did you forget to call SetSecondaryBlackboard on the property, in the node's OnTriggerBlackboardSet function?", TafraDebugger.LogType.Error);

                //Prioritize getting the value from the secondary black board if it exists.
                if(secondaryBlackboard != null && targetBlackboard == TargetBlackboard.Secondary && TryGetValueFromSecondaryBlackboard(out T secondaryBBValue))
                    return secondaryBBValue;

                if(blackboardCollection == null)
                {
                    TafraDebugger.Log("Blackboard Property Getter", "Getter is not initialized. Call Initialize() on the variable.", TafraDebugger.LogType.Error);
                    return defaultValue;
                }

                if(property == null)
                {
                    property = GetProperty();

                    if (property == null)
                    {
                        if (!enableDefault)
                            TafraDebugger.Log("Blackboard Property Getter", $"Couldn't find a property with the given name {blackboardPropertyName} in the target blackboard.", TafraDebugger.LogType.Error);

                        return defaultValue;
                    }
                }

                return property.value;
            }
        }

        protected abstract bool TryGetValueFromSecondaryBlackboard(out T value);
    }
}