using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.GraphViews
{
    public class BlackboardCollection
    {
        private GraphBlackboard internalBlackboard;
        private List<GraphBlackboard> externalBlackboards = new List<GraphBlackboard>();
        private TafraActor actor;

        public TafraActor Actor => actor;

        public void SetDependencies(TafraActor actor)
        {
            this.actor = actor;
        }
        public void SetInternalBlackboard(GraphBlackboard blackboard)
        {
            internalBlackboard = blackboard;
        }
        public void AddExternalBlackboard(GraphBlackboard blackboard)
        {
            externalBlackboards.Add(blackboard);
        }

        #region Property Getter Functions
        public GenericExposableProperty<float> TryGetFloatProperty(int propertyNameHash, int internalBBPropretyID, TargetBlackboard target)
        {
            switch(target)
            {
                case TargetBlackboard.Internal:
                    return internalBlackboard.TryGetFloatProperty(propertyNameHash, internalBBPropretyID);
                case TargetBlackboard.External:
                    {
                        for(int i = 0; i < externalBlackboards.Count; i++)
                        {
                            var blackbaord = externalBlackboards[i];
                            var prop = blackbaord.TryGetFloatProperty(propertyNameHash, -1);

                            if (prop != null)
                                return prop;
                        }
                    }
                    break;
                case TargetBlackboard.InternalOrExternal:
                    {
                        var internalProp = internalBlackboard.TryGetFloatProperty(propertyNameHash, internalBBPropretyID);

                        if(internalProp != null)
                            return internalProp;

                        for(int i = 0; i < externalBlackboards.Count; i++)
                        {
                            var blackbaord = externalBlackboards[i];
                            var prop = blackbaord.TryGetFloatProperty(propertyNameHash, -1);

                            if(prop != null)
                                return prop;
                        }
                    }
                    break;
            }

            return null;
        }
        public GenericExposableProperty<TafraAdvancedFloat> TryGetAdvancedFloatProperty(int propertyNameHash, int internalBBPropretyID, TargetBlackboard target)
        {
            switch(target)
            {
                case TargetBlackboard.Internal:
                    return internalBlackboard.TryGetAdvancedFloatProperty(propertyNameHash, internalBBPropretyID);
                case TargetBlackboard.External:
                    {
                        for(int i = 0; i < externalBlackboards.Count; i++)
                        {
                            var blackbaord = externalBlackboards[i];
                            var prop = blackbaord.TryGetAdvancedFloatProperty(propertyNameHash, -1);

                            if (prop != null)
                                return prop;
                        }
                    }
                    break;
                case TargetBlackboard.InternalOrExternal:
                    {
                        var internalProp = internalBlackboard.TryGetAdvancedFloatProperty(propertyNameHash, internalBBPropretyID);

                        if(internalProp != null)
                            return internalProp;

                        for(int i = 0; i < externalBlackboards.Count; i++)
                        {
                            var blackbaord = externalBlackboards[i];
                            var prop = blackbaord.TryGetAdvancedFloatProperty(propertyNameHash, -1);

                            if(prop != null)
                                return prop;
                        }
                    }
                    break;
            }

            return null;
        }
        public GenericExposableProperty<int> TryGetIntProperty(int propertyNameHash, int internalBBPropretyID, TargetBlackboard target)
        {
            switch(target)
            {
                case TargetBlackboard.Internal:
                    return internalBlackboard.TryGetIntProperty(propertyNameHash, internalBBPropretyID);
                case TargetBlackboard.External:
                    {
                        for(int i = 0; i < externalBlackboards.Count; i++)
                        {
                            var blackbaord = externalBlackboards[i];
                            var prop = blackbaord.TryGetIntProperty(propertyNameHash, -1);

                            if(prop != null)
                                return prop;
                        }
                    }
                    break;
                case TargetBlackboard.InternalOrExternal:
                    {
                        var internalProp = internalBlackboard.TryGetIntProperty(propertyNameHash, internalBBPropretyID);

                        if(internalProp != null)
                            return internalProp;

                        for(int i = 0; i < externalBlackboards.Count; i++)
                        {
                            var blackbaord = externalBlackboards[i];
                            var prop = blackbaord.TryGetIntProperty(propertyNameHash, -1);

                            if(prop != null)
                                return prop;
                        }
                    }
                    break;
            }

            return null;
        }
        public GenericExposableProperty<bool> TryGetBoolProperty(int propertyNameHash, int internalBBPropretyID, TargetBlackboard target)
        {
            switch(target)
            {
                case TargetBlackboard.Internal:
                    return internalBlackboard.TryGetBoolProperty(propertyNameHash, internalBBPropretyID);
                case TargetBlackboard.External:
                    {
                        for(int i = 0; i < externalBlackboards.Count; i++)
                        {
                            var blackbaord = externalBlackboards[i];
                            var prop = blackbaord.TryGetBoolProperty(propertyNameHash, -1);

                            if(prop != null)
                                return prop;
                        }
                    }
                    break;
                case TargetBlackboard.InternalOrExternal:
                    {
                        var internalProp = internalBlackboard.TryGetBoolProperty(propertyNameHash, internalBBPropretyID);

                        if(internalProp != null)
                            return internalProp;

                        for(int i = 0; i < externalBlackboards.Count; i++)
                        {
                            var blackbaord = externalBlackboards[i];
                            var prop = blackbaord.TryGetBoolProperty(propertyNameHash, -1);

                            if(prop != null)
                                return prop;
                        }
                    }
                    break;
            }

            return null;
        }
        public GenericExposableProperty<string> TryGetStringProperty(int propertyNameHash, int internalBBPropretyID, TargetBlackboard target)
        {
            switch(target)
            {
                case TargetBlackboard.Internal:
                    return internalBlackboard.TryGetStringProperty(propertyNameHash, internalBBPropretyID);
                case TargetBlackboard.External:
                    {
                        for(int i = 0; i < externalBlackboards.Count; i++)
                        {
                            var blackbaord = externalBlackboards[i];
                            var prop = blackbaord.TryGetStringProperty(propertyNameHash, -1);

                            if(prop != null)
                                return prop;
                        }
                    }
                    break;
                case TargetBlackboard.InternalOrExternal:
                    {
                        var internalProp = internalBlackboard.TryGetStringProperty(propertyNameHash, internalBBPropretyID);

                        if(internalProp != null)
                            return internalProp;

                        for(int i = 0; i < externalBlackboards.Count; i++)
                        {
                            var blackbaord = externalBlackboards[i];
                            var prop = blackbaord.TryGetStringProperty(propertyNameHash, -1);

                            if(prop != null)
                                return prop;
                        }
                    }
                    break;
            }

            return null;
        }
        public GenericExposableProperty<Vector3> TryGetVector3Property(int propertyNameHash, int internalBBPropretyID, TargetBlackboard target)
        {
            switch (target)
            {
                case TargetBlackboard.Internal:
                    return internalBlackboard.TryGetVector3Property(propertyNameHash, internalBBPropretyID);
                case TargetBlackboard.External:
                    {
                        for (int i = 0; i < externalBlackboards.Count; i++)
                        {
                            var blackbaord = externalBlackboards[i];
                            var prop = blackbaord.TryGetVector3Property(propertyNameHash, -1);

                            if(prop != null)
                                return prop;
                        }
                    }
                    break;
                case TargetBlackboard.InternalOrExternal:
                    {
                        var internalProp = internalBlackboard.TryGetVector3Property(propertyNameHash, internalBBPropretyID);

                        if(internalProp != null)
                            return internalProp;

                        for(int i = 0; i < externalBlackboards.Count; i++)
                        {
                            var blackbaord = externalBlackboards[i];
                            var prop = blackbaord.TryGetVector3Property(propertyNameHash, -1);

                            if(prop != null)
                                return prop;
                        }
                    }
                    break;
            }

            return null;
        }
        public GenericExposableProperty<GameObject> TryGetGameObjectProperty(int propertyNameHash, int internalBBPropretyID, TargetBlackboard target)
        {
            switch(target)
            {
                case TargetBlackboard.Internal:
                    return internalBlackboard.TryGetGameObjectProperty(propertyNameHash, internalBBPropretyID);
                case TargetBlackboard.External:
                    {
                        for(int i = 0; i < externalBlackboards.Count; i++)
                        {
                            var blackbaord = externalBlackboards[i];
                            var prop = blackbaord.TryGetGameObjectProperty(propertyNameHash, -1);

                            if(prop != null)
                                return prop;
                        }
                    }
                    break;
                case TargetBlackboard.InternalOrExternal:
                    {
                        var internalProp = internalBlackboard.TryGetGameObjectProperty(propertyNameHash, internalBBPropretyID);

                        if(internalProp != null)
                            return internalProp;

                        for(int i = 0; i < externalBlackboards.Count; i++)
                        {
                            var blackbaord = externalBlackboards[i];
                            var prop = blackbaord.TryGetGameObjectProperty(propertyNameHash, -1);

                            if(prop != null)
                                return prop;
                        }
                    }
                    break;
            }

            return null;
        }
        public GenericExposableProperty<ScriptableObject> TryGetScriptableObjectProperty(int propertyNameHash, int internalBBPropretyID, TargetBlackboard target)
        {
            switch(target)
            {
                case TargetBlackboard.Internal:
                    return internalBlackboard.TryGetScriptableObjectProperty(propertyNameHash, internalBBPropretyID);
                case TargetBlackboard.External:
                    {
                        for(int i = 0; i < externalBlackboards.Count; i++)
                        {
                            var blackbaord = externalBlackboards[i];
                            var prop = blackbaord.TryGetScriptableObjectProperty(propertyNameHash, -1);

                            if(prop != null)
                                return prop;
                        }
                    }
                    break;
                case TargetBlackboard.InternalOrExternal:
                    {
                        var internalProp = internalBlackboard.TryGetScriptableObjectProperty(propertyNameHash, internalBBPropretyID);

                        if(internalProp != null)
                            return internalProp;

                        for(int i = 0; i < externalBlackboards.Count; i++)
                        {
                            var blackbaord = externalBlackboards[i];
                            var prop = blackbaord.TryGetScriptableObjectProperty(propertyNameHash, -1);

                            if(prop != null)
                                return prop;
                        }
                    }
                    break;
            }

            return null;
        }
        public GenericExposableProperty<TafraActor> TryGetActorProperty(int propertyNameHash, int internalBBPropretyID, TargetBlackboard target)
        {
            switch(target)
            {
                case TargetBlackboard.Internal:
                    return internalBlackboard.TryGetActorProperty(propertyNameHash, internalBBPropretyID);
                case TargetBlackboard.External:
                    {
                        for(int i = 0; i < externalBlackboards.Count; i++)
                        {
                            var blackbaord = externalBlackboards[i];
                            var prop = blackbaord.TryGetActorProperty(propertyNameHash, -1);

                            if(prop != null)
                                return prop;
                        }
                    }
                    break;
                case TargetBlackboard.InternalOrExternal:
                    {
                        var internalProp = internalBlackboard.TryGetActorProperty(propertyNameHash, internalBBPropretyID);

                        if(internalProp != null)
                            return internalProp;

                        for(int i = 0; i < externalBlackboards.Count; i++)
                        {
                            var blackbaord = externalBlackboards[i];
                            var prop = blackbaord.TryGetActorProperty(propertyNameHash, -1);

                            if(prop != null)
                                return prop;
                        }
                    }
                    break;
            }

            return null;
        }
        public GenericExposableProperty<UnityEngine.Object> TryGetObjectProperty(int propertyNameHash, int internalBBPropretyID, TargetBlackboard target)
        {
            switch(target)
            {
                case TargetBlackboard.Internal:
                    return internalBlackboard.TryGetObjectProperty(propertyNameHash, internalBBPropretyID);
                case TargetBlackboard.External:
                    {
                        for(int i = 0; i < externalBlackboards.Count; i++)
                        {
                            var blackbaord = externalBlackboards[i];
                            var prop = blackbaord.TryGetObjectProperty(propertyNameHash, -1);

                            if(prop != null)
                                return prop;
                        }
                    }
                    break;
                case TargetBlackboard.InternalOrExternal:
                    {
                        var internalProp = internalBlackboard.TryGetObjectProperty(propertyNameHash, internalBBPropretyID);

                        if(internalProp != null)
                            return internalProp;

                        for(int i = 0; i < externalBlackboards.Count; i++)
                        {
                            var blackbaord = externalBlackboards[i];
                            var prop = blackbaord.TryGetObjectProperty(propertyNameHash, -1);

                            if(prop != null)
                                return prop;
                        }
                    }
                    break;
            }

            return null;
        }
        public GenericExposableProperty<object> TryGetSystemObjectProperty(int propertyNameHash, int internalBBPropretyID, TargetBlackboard target)
        {
            switch(target)
            {
                case TargetBlackboard.Internal:
                    return internalBlackboard.TryGetSystemObjectProperty(propertyNameHash, internalBBPropretyID);
                case TargetBlackboard.External:
                    {
                        for(int i = 0; i < externalBlackboards.Count; i++)
                        {
                            var blackbaord = externalBlackboards[i];
                            var prop = blackbaord.TryGetSystemObjectProperty(propertyNameHash, -1);

                            if(prop != null)
                                return prop;
                        }
                    }
                    break;
                case TargetBlackboard.InternalOrExternal:
                    {
                        var internalProp = internalBlackboard.TryGetSystemObjectProperty(propertyNameHash, internalBBPropretyID);

                        if(internalProp != null)
                            return internalProp;

                        for(int i = 0; i < externalBlackboards.Count; i++)
                        {
                            var blackbaord = externalBlackboards[i];
                            var prop = blackbaord.TryGetSystemObjectProperty(propertyNameHash, -1);

                            if(prop != null)
                                return prop;
                        }
                    }
                    break;
            }

            return null;
        }
        #endregion
    }
}