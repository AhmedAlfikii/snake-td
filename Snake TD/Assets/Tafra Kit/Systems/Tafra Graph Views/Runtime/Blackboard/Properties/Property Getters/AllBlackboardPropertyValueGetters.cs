using System;
using TafraKit.Healthies;
using TafraKit.Internal.CharacterControls;
using TafraKit.Internal.GraphViews;
using UnityEngine;

namespace TafraKit.GraphViews
{
    [Serializable]
    public class BlackboardFloatGetter : BlackboardPropertyValueGetter<float>
    {
        public BlackboardFloatGetter() { }
        public BlackboardFloatGetter(float defaultValue) : base(defaultValue) { }
        public BlackboardFloatGetter(BlackboardFloatGetter other) : base(other) { }

        protected override GenericExposableProperty<float> GetProperty()
        {
            return blackboardCollection.TryGetFloatProperty(blackboardPropertyNameHash, internalBlackboardPropertyID, targetBlackboard);
        }

        protected override bool TryGetValueFromSecondaryBlackboard(out float value)
        {
            return secondaryBlackboard.TryGetFloatProperty(blackboardPropertyNameHash, out value);
        }
    }
    [Serializable]
    public class BlackboardAdvancedFloatGetter : BlackboardPropertyValueGetter<TafraAdvancedFloat>
    {
        public BlackboardAdvancedFloatGetter() { }
        public BlackboardAdvancedFloatGetter(TafraAdvancedFloat defaultValue) : base(defaultValue) { }
        public BlackboardAdvancedFloatGetter(float defaultValue) : base(new TafraAdvancedFloat(defaultValue)) { }
        public BlackboardAdvancedFloatGetter(BlackboardAdvancedFloatGetter other) : base(other) { }

        protected override GenericExposableProperty<TafraAdvancedFloat> GetProperty()
        {
            return blackboardCollection.TryGetAdvancedFloatProperty(blackboardPropertyNameHash, internalBlackboardPropertyID, targetBlackboard);
        }
        protected override bool TryGetValueFromSecondaryBlackboard(out TafraAdvancedFloat value)
        {
            return secondaryBlackboard.TryGetAdvancedFloatProperty(blackboardPropertyNameHash, out value);
        }
    }

    [Serializable]
    public class BlackboardIntGetter : BlackboardPropertyValueGetter<int>
    {
        public BlackboardIntGetter() { }
        public BlackboardIntGetter(int defaultValue) : base(defaultValue) { }
        public BlackboardIntGetter(BlackboardIntGetter other) : base(other) { }

        protected override GenericExposableProperty<int> GetProperty()
        {
            return blackboardCollection.TryGetIntProperty(blackboardPropertyNameHash, internalBlackboardPropertyID, targetBlackboard);
        }
        protected override bool TryGetValueFromSecondaryBlackboard(out int value)
        {
            return secondaryBlackboard.TryGetIntProperty(blackboardPropertyNameHash, out value);
        }
    }

    [Serializable]
    public class BlackboardBoolGetter : BlackboardPropertyValueGetter<bool>
    {
        public BlackboardBoolGetter() { }
        public BlackboardBoolGetter(bool defaultValue) : base(defaultValue) { }
        public BlackboardBoolGetter(BlackboardBoolGetter other) : base(other) { }

        protected override GenericExposableProperty<bool> GetProperty()
        {
            return blackboardCollection.TryGetBoolProperty(blackboardPropertyNameHash, internalBlackboardPropertyID, targetBlackboard);
        }
        protected override bool TryGetValueFromSecondaryBlackboard(out bool value)
        {
            return secondaryBlackboard.TryGetBoolProperty(blackboardPropertyNameHash, out value);
        }
    }

    [Serializable]
    public class BlackboardStringGetter : BlackboardPropertyValueGetter<string>
    {
        public BlackboardStringGetter() { }
        public BlackboardStringGetter(string defaultValue) : base(defaultValue) { }
        public BlackboardStringGetter(BlackboardStringGetter other) : base(other) { }

        protected override GenericExposableProperty<string> GetProperty()
        {
            return blackboardCollection.TryGetStringProperty(blackboardPropertyNameHash, internalBlackboardPropertyID, targetBlackboard);
        }
        protected override bool TryGetValueFromSecondaryBlackboard(out string value)
        {
            return secondaryBlackboard.TryGetStringProperty(blackboardPropertyNameHash, out value);
        }
    }

    [Serializable]
    public class BlackboardVector3Getter : BlackboardPropertyValueGetter<Vector3>
    {
        public BlackboardVector3Getter() { }
        public BlackboardVector3Getter(Vector3 defaultValue) : base(defaultValue) { }
        public BlackboardVector3Getter(BlackboardVector3Getter other) : base(other) { }

        protected override GenericExposableProperty<Vector3> GetProperty()
        {
            return blackboardCollection.TryGetVector3Property(blackboardPropertyNameHash, internalBlackboardPropertyID, targetBlackboard);
        }
        protected override bool TryGetValueFromSecondaryBlackboard(out Vector3 value)
        {
            return secondaryBlackboard.TryGetVector3Property(blackboardPropertyNameHash, out value);
        }
    }

    [Serializable]
    public class BlackboardGameObjectGetter : BlackboardPropertyValueGetter<GameObject>
    {
        public BlackboardGameObjectGetter() { }
        public BlackboardGameObjectGetter(GameObject defaultValue) : base(defaultValue) { }
        public BlackboardGameObjectGetter(BlackboardGameObjectGetter other) : base(other) { }

        protected override GenericExposableProperty<GameObject> GetProperty()
        {
            return blackboardCollection.TryGetGameObjectProperty(blackboardPropertyNameHash, internalBlackboardPropertyID, targetBlackboard);
        }
        protected override bool TryGetValueFromSecondaryBlackboard(out GameObject value)
        {
            return secondaryBlackboard.TryGetGameObjectProperty(blackboardPropertyNameHash, out value);
        }
    }

    [Serializable]
    public class BlackboardScriptableObjectGetter : BlackboardPropertyValueGetter<ScriptableObject>
    {
        public BlackboardScriptableObjectGetter() { }
        public BlackboardScriptableObjectGetter(ScriptableObject defaultValue) : base(defaultValue) { }
        public BlackboardScriptableObjectGetter(BlackboardScriptableObjectGetter other) : base(other) { }

        protected override GenericExposableProperty<ScriptableObject> GetProperty()
        {
            return blackboardCollection.TryGetScriptableObjectProperty(blackboardPropertyNameHash, internalBlackboardPropertyID, targetBlackboard);
        }
        protected override bool TryGetValueFromSecondaryBlackboard(out ScriptableObject value)
        {
            return secondaryBlackboard.TryGetScriptableObjectProperty(blackboardPropertyNameHash, out value);
        }
    }

    [Serializable]
    public class BlackboardActorGetter : BlackboardPropertyValueGetter<TafraActor>
    {
        [SerializeField] private bool targetIsThisActor;

        public override TafraActor Value
        {
            get
            {
                if(targetIsThisActor)
                    return blackboardCollection.Actor;
                else
                    return base.Value;
            }
        }

        public BlackboardActorGetter() { }
        public BlackboardActorGetter(TafraActor defaultValue) : base(defaultValue) { }
        public BlackboardActorGetter(BlackboardActorGetter other) : base(other)
        {
            targetIsThisActor = other.targetIsThisActor;
        }

        protected override GenericExposableProperty<TafraActor> GetProperty()
        {
            return blackboardCollection.TryGetActorProperty(blackboardPropertyNameHash, internalBlackboardPropertyID, targetBlackboard);
        }
        protected override bool TryGetValueFromSecondaryBlackboard(out TafraActor value)
        {
            return secondaryBlackboard.TryGetTafraActorProperty(blackboardPropertyNameHash, out value);
        }
    }

    [Serializable]
    public class BlackboardObjectGetter : BlackboardPropertyValueGetter<UnityEngine.Object>
    {
        public BlackboardObjectGetter() { }
        public BlackboardObjectGetter(UnityEngine.Object defaultValue) : base(defaultValue) { }
        public BlackboardObjectGetter(BlackboardObjectGetter other) : base(other) { }

        protected override GenericExposableProperty<UnityEngine.Object> GetProperty()
        {
            return blackboardCollection.TryGetObjectProperty(blackboardPropertyNameHash, internalBlackboardPropertyID, targetBlackboard);
        }
        protected override bool TryGetValueFromSecondaryBlackboard(out UnityEngine.Object value)
        {
            return secondaryBlackboard.TryGetObjectProperty(blackboardPropertyNameHash, out value);
        }
    }

    [Serializable]
    public class BlackboardSystemObjectGetter : BlackboardPropertyValueGetter<object>
    {
        public BlackboardSystemObjectGetter() { }
        public BlackboardSystemObjectGetter(object defaultValue) : base(defaultValue) { }
        public BlackboardSystemObjectGetter(BlackboardSystemObjectGetter other) : base(other) { }

        protected override GenericExposableProperty<object> GetProperty()
        {
            return blackboardCollection.TryGetSystemObjectProperty(blackboardPropertyNameHash, internalBlackboardPropertyID, targetBlackboard);
        }
        protected override bool TryGetValueFromSecondaryBlackboard(out object value)
        {
            return secondaryBlackboard.TryGetSystemObjectProperty(blackboardPropertyNameHash, out value);
        }
    }
    [Serializable]
    public class BlackboardSystemObjectGetter<T> : BlackboardGenericPropertyReference<object>
    {
        [Tooltip("This value will be used if the target blackboard is none or if fetching the property from the blackboard failed.")]
        [SerializeField] protected T defaultValue;
        [Tooltip("If enabled, the default value will be used as a fallback in case fetching the target blackboard proprety failed.")]
        [SerializeField] protected bool enableDefault;

        public BlackboardSystemObjectGetter() { }
        public BlackboardSystemObjectGetter(T defaultValue) 
        {
            this.defaultValue = defaultValue;
        }
        public BlackboardSystemObjectGetter(BlackboardSystemObjectGetter<T> other) : base(other)
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
                if(targetBlackboard == TargetBlackboard.None)
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

                    if(property == null)
                    {
                        if(!enableDefault)
                            TafraDebugger.Log("Blackboard Property Getter", $"Couldn't find a property with the given name {blackboardPropertyName} in the target blackboard.", TafraDebugger.LogType.Error);

                        return defaultValue;
                    }
                }

                return (T)property.value;
            }
        }

        protected override GenericExposableProperty<object> GetProperty()
        {
            return blackboardCollection.TryGetSystemObjectProperty(blackboardPropertyNameHash, internalBlackboardPropertyID, targetBlackboard);
        }

        protected bool TryGetValueFromSecondaryBlackboard(out T value)
        {
            object obj;

            bool foundIt = secondaryBlackboard.TryGetGeneralObjectProperty<T>(blackboardPropertyNameHash, out obj);

            if(!foundIt)
            {
                value = default;
                return false;
            }

            value = (T)obj;

            return true;
        }
    }


    [Serializable]
    public class BlackboardDynamicFloatGetter
    {
        public enum FloatType
        {
            NormalFloat,
            TafraAdavncedFloat
        }
        [SerializeField] private FloatType floatType;
        [SerializeField] private BlackboardFloatGetter normalFloatGetter;
        [SerializeField] private BlackboardAdvancedFloatGetter advancedFloatGetter;

        public BlackboardDynamicFloatGetter() { }
        public BlackboardDynamicFloatGetter(float value)
        {
            normalFloatGetter = new BlackboardFloatGetter(value);
            advancedFloatGetter = new BlackboardAdvancedFloatGetter(value);
        }
        public BlackboardDynamicFloatGetter(BlackboardDynamicFloatGetter other)
        {
            floatType = other.floatType;
            normalFloatGetter = new BlackboardFloatGetter(other.normalFloatGetter);
            advancedFloatGetter = new BlackboardAdvancedFloatGetter(other.advancedFloatGetter);
        }

        public float Value
        {
            get
            {
                switch(floatType)
                {
                    case FloatType.NormalFloat:
                        return normalFloatGetter.Value;
                    case FloatType.TafraAdavncedFloat:
                            return advancedFloatGetter.Value.Value;
                }

                return 0;
            }
        }

        public void Initialize(BlackboardCollection blackboardCollection)
        {
            switch(floatType)
            {
                case FloatType.NormalFloat:
                    normalFloatGetter.Initialize(blackboardCollection);
                    break;
                case FloatType.TafraAdavncedFloat:
                    advancedFloatGetter.Initialize(blackboardCollection);
                    break;
            }
        }
        public void SetSecondaryBlackboard(SecondaryBlackboard secondaryBlackboard)
        {
            switch(floatType)
            {
                case FloatType.NormalFloat:
                    normalFloatGetter.SetSecondaryBlackboard(secondaryBlackboard);
                    break;
                case FloatType.TafraAdavncedFloat:
                    advancedFloatGetter.SetSecondaryBlackboard(secondaryBlackboard);
                    break;
            }
        }
    }
    [Serializable]
    public class BlackboardDynamicIntGetter
    {
        public enum IntType
        {
            NormalInt,
            TafraAdavncedFloat
        }
        [SerializeField] private IntType intType;
        [SerializeField] private BlackboardIntGetter normalIntGetter;
        [SerializeField] private BlackboardAdvancedFloatGetter advancedFloatGetter;

        public BlackboardDynamicIntGetter() { }
        public BlackboardDynamicIntGetter(int value)
        {
            normalIntGetter = new BlackboardIntGetter(value);
            advancedFloatGetter = new BlackboardAdvancedFloatGetter(value);
        }
        public BlackboardDynamicIntGetter(BlackboardDynamicIntGetter other)
        {
            intType = other.intType;
            normalIntGetter = new BlackboardIntGetter(other.normalIntGetter);
            advancedFloatGetter = new BlackboardAdvancedFloatGetter(other.advancedFloatGetter);
        }

        public int Value
        {
            get
            {
                switch(intType)
                {
                    case IntType.NormalInt:
                        return normalIntGetter.Value;
                    case IntType.TafraAdavncedFloat:
                        return advancedFloatGetter.Value.ValueInt;
                }

                return 0;
            }
        }

        public void Initialize(BlackboardCollection blackboardCollection)
        {
            switch(intType)
            {
                case IntType.NormalInt:
                    normalIntGetter.Initialize(blackboardCollection);
                    break;
                case IntType.TafraAdavncedFloat:
                    advancedFloatGetter.Initialize(blackboardCollection);
                    break;
            }
        }
        public void SetSecondaryBlackboard(SecondaryBlackboard secondaryBlackboard)
        {
            switch(intType)
            {
                case IntType.NormalInt:
                    normalIntGetter.SetSecondaryBlackboard(secondaryBlackboard);
                    break;
                case IntType.TafraAdavncedFloat:
                    advancedFloatGetter.SetSecondaryBlackboard(secondaryBlackboard);
                    break;
            }
        }
    }

    [Serializable]
    public class BlackboardDynamicPointGetter
    {
        public enum PointType
        {
            Actor,
            ThisAgent,
            GameObject,
            Vector3
        }
        [SerializeField] private PointType pointType;
        [SerializeField] private BlackboardActorGetter actorGetter;
        [SerializeField] private BlackboardGameObjectGetter gameObjectGetter;
        [SerializeField] private BlackboardVector3Getter vector3Getter;
        [Tooltip("If true, the actor's healthy component will be fetched from the actor's cached components, " +
            "and its Damage Point's position will be returned. If false, the actor's position will be returned")]
        [SerializeField] private bool getHealthyTargetPoint = false;

        private TafraActor actor;

        public BlackboardDynamicPointGetter() { }
        public BlackboardDynamicPointGetter(BlackboardDynamicPointGetter other)
        {
            pointType = other.pointType;
            actorGetter = new BlackboardActorGetter(other.actorGetter);
            gameObjectGetter = new BlackboardGameObjectGetter(other.gameObjectGetter);
            vector3Getter = new BlackboardVector3Getter(other.vector3Getter);
            getHealthyTargetPoint = other.getHealthyTargetPoint;
        }

        public Vector3 Value
        {
            get
            {
                switch(pointType)
                {
                    case PointType.Actor:
                        {
                            TafraActor actor = actorGetter.Value;

                            if(getHealthyTargetPoint)
                            {
                                Healthy healthy = actor.GetCachedComponent<Healthy>();
                                if(healthy != null)
                                    return healthy.TargetPoint.position;
                                else
                                    return actor.transform.position;
                            }
                            else
                                return actor.transform.position;
                        }
                    case PointType.GameObject:
                        return gameObjectGetter.Value.transform.position;
                    case PointType.ThisAgent:
                        {
                            if(getHealthyTargetPoint)
                            {
                                Healthy healthy = actor.GetCachedComponent<Healthy>();
                                if(healthy != null)
                                    return healthy.TargetPoint.position;
                                else
                                    return actor.transform.position;
                            }
                            else
                                return actor.transform.position;
                        }
                    case PointType.Vector3:
                        return vector3Getter.Value;
                }

                return Vector3.zero;
            }
        }
        public int PropertyNameHash
        {
            get
            {
                switch(pointType)
                {
                    case PointType.Actor:
                        return actorGetter.PropertyNameHash;
                    case PointType.GameObject:
                        return gameObjectGetter.PropertyNameHash;
                    case PointType.ThisAgent:
                        return -1;
                    case PointType.Vector3:
                        return vector3Getter.PropertyNameHash;
                }

                return -1;
            }
        }
        public PointType MyPointType => pointType;
        public BlackboardActorGetter ActorGetter => actorGetter;
        public BlackboardGameObjectGetter GameObjectGetter => gameObjectGetter;
        public BlackboardVector3Getter Vector3Getter => vector3Getter;

        public void Initialize(BlackboardCollection blackboardCollection)
        {
            actor = blackboardCollection.Actor;

            switch(pointType)
            {
                case PointType.Actor:
                    actorGetter.Initialize(blackboardCollection);
                    break;
                case PointType.GameObject:
                    gameObjectGetter.Initialize(blackboardCollection);
                    break;
                case PointType.Vector3:
                    vector3Getter.Initialize(blackboardCollection);
                    break;
            }
        }
        public void SetSecondaryBlackboard(SecondaryBlackboard secondaryBlackboard)
        {
            switch(pointType)
            {
                case PointType.Actor:
                    actorGetter.SetSecondaryBlackboard(secondaryBlackboard);
                    break;
                case PointType.GameObject:
                    gameObjectGetter.SetSecondaryBlackboard(secondaryBlackboard);
                    break;
                case PointType.Vector3:
                    vector3Getter.SetSecondaryBlackboard(secondaryBlackboard);
                    break;
            }
        }
    }
}