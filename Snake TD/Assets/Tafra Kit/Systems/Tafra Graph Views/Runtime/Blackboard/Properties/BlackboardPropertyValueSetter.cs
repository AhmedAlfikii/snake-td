using System;

namespace TafraKit.GraphViews
{
    [Serializable]
    public abstract class BlackboardPropertyValueSetter<T> : BlackboardGenericPropertyReference<T>
    {
        public BlackboardPropertyValueSetter() { }
        public BlackboardPropertyValueSetter(BlackboardPropertyValueSetter<T> other) : base(other)
        {
        }

        /// <summary>
        /// Sets the value of the target blackboard property with the provided name if found.
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(T value)
        {
            if(targetBlackboard == TargetBlackboard.None)
                return;

            if(blackboardCollection == null)
            {
                TafraDebugger.Log("Blackboard Property Setter", "Setter is not initialized. Call Initialize() on the variable.", TafraDebugger.LogType.Error);
                return;
            }

            if(property == null)
            {
                property = GetProperty();

                if(property == null) 
                    return;
            }

            property.value = value;
        }
        protected abstract void SetValueOnSecondaryBlackboard(T value);

        public T Value
        {
            get
            {
                if(targetBlackboard == TargetBlackboard.None)
                    return default;

                if(blackboardCollection == null)
                {
                    TafraDebugger.Log("Blackboard Property Setter", "Setter is not initialized. Call SetBlackboardCollection on the variable.", TafraDebugger.LogType.Error);
                    return default;
                }

                if(property == null)
                {
                    property = GetProperty();

                    if(property == null)
                        return default;
                }

                return property.value;
            }
        }
    }
}