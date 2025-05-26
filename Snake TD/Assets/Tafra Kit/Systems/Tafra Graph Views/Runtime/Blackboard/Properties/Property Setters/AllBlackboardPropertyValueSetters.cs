using System;
using UnityEngine;

namespace TafraKit.GraphViews
{
    [Serializable]
    public class BlackboardFloatSetter : BlackboardPropertyValueSetter<float>
    {
        public BlackboardFloatSetter() { }
        public BlackboardFloatSetter(BlackboardFloatSetter other) : base(other) { }

        protected override GenericExposableProperty<float> GetProperty()
        {
            return blackboardCollection.TryGetFloatProperty(blackboardPropertyNameHash, internalBlackboardPropertyID, targetBlackboard);
        }
        protected override void SetValueOnSecondaryBlackboard(float value)
        {
            secondaryBlackboard.SetFloatProperty(blackboardPropertyNameHash, value);
        }
    }
    [Serializable]
    public class BlackboardAdvancedFloatSetter : BlackboardPropertyValueSetter<TafraAdvancedFloat>
    {
        public BlackboardAdvancedFloatSetter() { }
        public BlackboardAdvancedFloatSetter(BlackboardAdvancedFloatSetter other) : base(other) { }

        protected override GenericExposableProperty<TafraAdvancedFloat> GetProperty()
        {
            return blackboardCollection.TryGetAdvancedFloatProperty(blackboardPropertyNameHash, internalBlackboardPropertyID, targetBlackboard);
        }
        protected override void SetValueOnSecondaryBlackboard(TafraAdvancedFloat value)
        {
            secondaryBlackboard.SetAdvancedFloatProperty(blackboardPropertyNameHash, value);
        }
    }

    [Serializable]
    public class BlackboardIntSetter : BlackboardPropertyValueSetter<int>
    {
        public BlackboardIntSetter() { }
        public BlackboardIntSetter(BlackboardIntSetter other) : base(other) { }

        protected override GenericExposableProperty<int> GetProperty()
        {
            return blackboardCollection.TryGetIntProperty(blackboardPropertyNameHash, internalBlackboardPropertyID, targetBlackboard);
        }
        protected override void SetValueOnSecondaryBlackboard(int value)
        {
            secondaryBlackboard.SetIntProperty(blackboardPropertyNameHash, value);
        }
    }

    [Serializable]
    public class BlackboardBoolSetter : BlackboardPropertyValueSetter<bool>
    {
        public BlackboardBoolSetter() { }
        public BlackboardBoolSetter(BlackboardBoolSetter other) : base(other) { }

        protected override GenericExposableProperty<bool> GetProperty()
        {
            return blackboardCollection.TryGetBoolProperty(blackboardPropertyNameHash, internalBlackboardPropertyID, targetBlackboard);
        }
        protected override void SetValueOnSecondaryBlackboard(bool value)
        {
            secondaryBlackboard.SetBoolProperty(blackboardPropertyNameHash, value);
        }
    }

    [Serializable]
    public class BlackboardStringSetter : BlackboardPropertyValueSetter<string>
    {
        public BlackboardStringSetter() { }
        public BlackboardStringSetter(BlackboardStringSetter other) : base(other) { }

        protected override GenericExposableProperty<string> GetProperty()
        {
            return blackboardCollection.TryGetStringProperty(blackboardPropertyNameHash, internalBlackboardPropertyID, targetBlackboard);
        }
        protected override void SetValueOnSecondaryBlackboard(string value)
        {
            secondaryBlackboard.SetStringProperty(blackboardPropertyNameHash, value);
        }
    }

    [Serializable]
    public class BlackboardVector3Setter : BlackboardPropertyValueSetter<Vector3>
    {
        public BlackboardVector3Setter() { }
        public BlackboardVector3Setter(BlackboardVector3Setter other) : base(other) { }

        protected override GenericExposableProperty<Vector3> GetProperty()
        {
            return blackboardCollection.TryGetVector3Property(blackboardPropertyNameHash, internalBlackboardPropertyID, targetBlackboard);
        }
        protected override void SetValueOnSecondaryBlackboard(Vector3 value)
        {
            secondaryBlackboard.SetVector3Property(blackboardPropertyNameHash, value);
        }
    }

    [Serializable]
    public class BlackboardGameObjectSetter : BlackboardPropertyValueSetter<GameObject>
    {
        public BlackboardGameObjectSetter() { }
        public BlackboardGameObjectSetter(BlackboardGameObjectSetter other) : base(other) { }

        protected override GenericExposableProperty<GameObject> GetProperty()
        {
            return blackboardCollection.TryGetGameObjectProperty(blackboardPropertyNameHash, internalBlackboardPropertyID, targetBlackboard);
        }
        protected override void SetValueOnSecondaryBlackboard(GameObject value)
        {
            secondaryBlackboard.SetGameObjectProperty(blackboardPropertyNameHash, value);
        }
    }

    [Serializable]
    public class BlackboardScriptableObjectSetter : BlackboardPropertyValueSetter<ScriptableObject>
    {
        public BlackboardScriptableObjectSetter() { }
        public BlackboardScriptableObjectSetter(BlackboardScriptableObjectSetter other) : base(other) { }

        protected override GenericExposableProperty<ScriptableObject> GetProperty()
        {
            return blackboardCollection.TryGetScriptableObjectProperty(blackboardPropertyNameHash, internalBlackboardPropertyID, targetBlackboard);
        }
        protected override void SetValueOnSecondaryBlackboard(ScriptableObject value)
        {
            secondaryBlackboard.SetScriptableObjectProperty(blackboardPropertyNameHash, value);
        }
    }

    [Serializable]
    public class BlackboardActorSetter : BlackboardPropertyValueSetter<TafraActor>
    {
        public BlackboardActorSetter() { }
        public BlackboardActorSetter(BlackboardActorSetter other) : base(other) { }

        protected override GenericExposableProperty<TafraActor> GetProperty()
        {
            return blackboardCollection.TryGetActorProperty(blackboardPropertyNameHash, internalBlackboardPropertyID, targetBlackboard);
        }
        protected override void SetValueOnSecondaryBlackboard(TafraActor value)
        {
            secondaryBlackboard.SetTafraActorProperty(blackboardPropertyNameHash, value);
        }
    }

    [Serializable]
    public class BlackboardObjectSetter : BlackboardPropertyValueSetter<UnityEngine.Object>
    {
        public BlackboardObjectSetter() { }
        public BlackboardObjectSetter(BlackboardObjectSetter other) : base(other) { }

        protected override GenericExposableProperty<UnityEngine.Object> GetProperty()
        {
            return blackboardCollection.TryGetObjectProperty(blackboardPropertyNameHash, internalBlackboardPropertyID, targetBlackboard);
        }
        protected override void SetValueOnSecondaryBlackboard(UnityEngine.Object value)
        {
            secondaryBlackboard.SetObjectProperty(blackboardPropertyNameHash, value);
        }
    }
    [Serializable]
    public class BlackboardSystemObjectSetter : BlackboardPropertyValueSetter<object>
    {
        public BlackboardSystemObjectSetter() { }
        public BlackboardSystemObjectSetter(BlackboardSystemObjectSetter other) : base(other) { }

        protected override GenericExposableProperty<object> GetProperty()
        {
            return blackboardCollection.TryGetSystemObjectProperty(blackboardPropertyNameHash, internalBlackboardPropertyID, targetBlackboard);
        }
        protected override void SetValueOnSecondaryBlackboard(object value)
        {
            secondaryBlackboard.SetSystemObjectProperty(blackboardPropertyNameHash, value);
        }
    }
}