using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TafraKit.GraphViews
{
    [CreateAssetMenu(menuName = "Tafra Kit/AI3/Blackboard", fileName = "Blackboard")]
    public class ExternalBlackboard : ScriptableObject
    {
        [SerializeField] private GraphBlackboard blackboard;

        [NonSerialized] private bool isInitialized;

        public GraphBlackboard Blackboard => blackboard;

        public void Initialize()
        {
            if(isInitialized)
                return;

            blackboard.Initialize();

            isInitialized = true;
        }

        public ExposableProperty AddProperty(Type propertyType, string propertyName, out string validatedName)
        {
            return blackboard.AddProperty(propertyType, propertyName, out validatedName);
        }

        public List<GenericExposableProperty<T>> GetAllPropertiesOfGenericType<T>()
        {
            return blackboard.GetAllPropertiesOfGenericType<T>();
        }

        public object GetAllPropertiesOfType(Type type)
        {
            return blackboard.GetAllPropertiesOfType(type);
        }
        public int GetPropertiesCountOfType(Type type)
        {
            return blackboard.GetPropertiesCountOfType(type);
        }
        public void RemoveAllProperties()
        {
            blackboard.RemoveAllProperties();
        }
        public void RemoveProperty(Type propertyType, string propertyName)
        {
            blackboard.RemoveProperty(propertyType, propertyName);
        }
        public string RenameProperty(ExposableProperty property, string newName)
        {
            return blackboard.RenameProperty(property, newName);
        }
        public void GetAllProperties(List<ExposableProperty> listToFill)
        {
            blackboard.GetAllProperties(listToFill);
        }

        public GenericExposableProperty<float> TryGetFloatProperty(int propertyNameHash, int propretyID)
        {
            if(!isInitialized)
            {
                TafraDebugger.Log("External Blackboard", "You're trying to get a property without initializing the blackboard, make sure to call Initialize() first.", TafraDebugger.LogType.Error);
                return null;
            }

            return blackboard.TryGetFloatProperty(propertyNameHash, propretyID);
        }
        public GenericExposableProperty<TafraAdvancedFloat> TryGetAdvancedFloatProperty(int propertyNameHash, int propretyID)
        {
            if(!isInitialized)
            {
                TafraDebugger.Log("External Blackboard", "You're trying to get a property without initializing the blackboard, make sure to call Initialize() first.", TafraDebugger.LogType.Error);
                return null;
            }

            return blackboard.TryGetAdvancedFloatProperty(propertyNameHash, propretyID);
        }
        public GenericExposableProperty<int> TryGetIntProperty(int propertyNameHash, int propretyID)
        {
            if(!isInitialized)
            {
                TafraDebugger.Log("External Blackboard", "You're trying to get a property without initializing the blackboard, make sure to call Initialize() first.", TafraDebugger.LogType.Error);
                return null;
            }

            return blackboard.TryGetIntProperty(propertyNameHash, propretyID);
        }
        public GenericExposableProperty<bool> TryGetBoolProperty(int propertyNameHash, int propretyID)
        {
            if(!isInitialized)
            {
                TafraDebugger.Log("External Blackboard", "You're trying to get a property without initializing the blackboard, make sure to call Initialize() first.", TafraDebugger.LogType.Error);
                return null;
            }

            return blackboard.TryGetBoolProperty(propertyNameHash, propretyID);
        }
        public GenericExposableProperty<string> TryGetStringProperty(int propertyNameHash, int propretyID)
        {
            if(!isInitialized)
            {
                TafraDebugger.Log("External Blackboard", "You're trying to get a property without initializing the blackboard, make sure to call Initialize() first.", TafraDebugger.LogType.Error);
                return null;
            }

            return blackboard.TryGetStringProperty(propertyNameHash, propretyID);
        }
        public GenericExposableProperty<Vector3> TryGetVector3Property(int propertyNameHash, int propretyID)
        {
            if(!isInitialized)
            {
                TafraDebugger.Log("External Blackboard", "You're trying to get a property without initializing the blackboard, make sure to call Initialize() first.", TafraDebugger.LogType.Error);
                return null;
            }

            return blackboard.TryGetVector3Property(propertyNameHash, propretyID);
        }
        public GenericExposableProperty<GameObject> TryGetGameObjectProperty(int propertyNameHash, int propretyID)
        {
            if(!isInitialized)
            {
                TafraDebugger.Log("External Blackboard", "You're trying to get a property without initializing the blackboard, make sure to call Initialize() first.", TafraDebugger.LogType.Error);
                return null;
            }

            return blackboard.TryGetGameObjectProperty(propertyNameHash, propretyID);
        }
        public GenericExposableProperty<ScriptableObject> TryGetScriptableObjectProperty(int propertyNameHash, int propretyID)
        {
            if(!isInitialized)
            {
                TafraDebugger.Log("External Blackboard", "You're trying to get a property without initializing the blackboard, make sure to call Initialize() first.", TafraDebugger.LogType.Error);
                return null;
            }

            return blackboard.TryGetScriptableObjectProperty(propertyNameHash, propretyID);
        }
        public GenericExposableProperty<TafraActor> TryGetActorProperty(int propertyNameHash, int propretyID)
        {
            if(!isInitialized)
            {
                TafraDebugger.Log("External Blackboard", "You're trying to get a property without initializing the blackboard, make sure to call Initialize() first.", TafraDebugger.LogType.Error);
                return null;
            }

            return blackboard.TryGetActorProperty(propertyNameHash, propretyID);
        }
        public GenericExposableProperty<UnityEngine.Object> TryGetObjectProperty(int propertyNameHash, int propretyID)
        {
            if(!isInitialized)
            {
                TafraDebugger.Log("External Blackboard", "You're trying to get a property without initializing the blackboard, make sure to call Initialize() first.", TafraDebugger.LogType.Error);
                return null;
            }

            return blackboard.TryGetObjectProperty(propertyNameHash, propretyID);
        }
    }
}