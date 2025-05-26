using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.GraphViews
{
    public static class BlackboardUtilities
    {
        public static bool RefreshExposedProperties(GraphBlackboard referenceBlackboard, GraphBlackboard exposedPropertiesHolder) 
        {
            List<GenericExposableProperty<float>> referenceFloatProperties = referenceBlackboard.GetAllPropertiesOfGenericType<float>();
            List<GenericExposableProperty<TafraAdvancedFloat>> referenceAdvancedFloatProperties = referenceBlackboard.GetAllPropertiesOfGenericType<TafraAdvancedFloat>();
            List<GenericExposableProperty<int>> referenceIntProperties = referenceBlackboard.GetAllPropertiesOfGenericType<int>();
            List<GenericExposableProperty<bool>> referenceBoolProperties = referenceBlackboard.GetAllPropertiesOfGenericType<bool>();
            List<GenericExposableProperty<string>> referenceStringProperties = referenceBlackboard.GetAllPropertiesOfGenericType<string>();
            List<GenericExposableProperty<Vector3>> referenceVector3Properties = referenceBlackboard.GetAllPropertiesOfGenericType<Vector3>();
            List<GenericExposableProperty<GameObject>> referenceGameObjectProperties = referenceBlackboard.GetAllPropertiesOfGenericType<GameObject>();
            List<GenericExposableProperty<ScriptableObject>> referenceScriptableObjectProperties = referenceBlackboard.GetAllPropertiesOfGenericType<ScriptableObject>();
            List<GenericExposableProperty<TafraActor>> referenceActorProperties = referenceBlackboard.GetAllPropertiesOfGenericType<TafraActor>();
            List<GenericExposableProperty<UnityEngine.Object>> referenceObjectProperties = referenceBlackboard.GetAllPropertiesOfGenericType<UnityEngine.Object>();

            List<GenericExposableProperty<float>> exposedFloatProperties = exposedPropertiesHolder.GetAllPropertiesOfGenericType<float>();
            List<GenericExposableProperty<TafraAdvancedFloat>> exposedAdvancedFloatProperties = exposedPropertiesHolder.GetAllPropertiesOfGenericType<TafraAdvancedFloat>();
            List<GenericExposableProperty<int>> exposedIntProperties = exposedPropertiesHolder.GetAllPropertiesOfGenericType<int>();
            List<GenericExposableProperty<bool>> exposedBoolProperties = exposedPropertiesHolder.GetAllPropertiesOfGenericType<bool>();
            List<GenericExposableProperty<string>> exposedStringProperties = exposedPropertiesHolder.GetAllPropertiesOfGenericType<string>();
            List<GenericExposableProperty<Vector3>> exposedVector3Properties = exposedPropertiesHolder.GetAllPropertiesOfGenericType<Vector3>();
            List<GenericExposableProperty<GameObject>> exposedGameObjectProperties = exposedPropertiesHolder.GetAllPropertiesOfGenericType<GameObject>();
            List<GenericExposableProperty<ScriptableObject>> exposedScriptableObjectProperties = exposedPropertiesHolder.GetAllPropertiesOfGenericType<ScriptableObject>();
            List<GenericExposableProperty<TafraActor>> exposedActorProperties = exposedPropertiesHolder.GetAllPropertiesOfGenericType<TafraActor>();
            List<GenericExposableProperty<UnityEngine.Object>> exposedObjectProperties = exposedPropertiesHolder.GetAllPropertiesOfGenericType<UnityEngine.Object>();

            bool ch1 = RefreshExposedPropertiesList(referenceFloatProperties, exposedFloatProperties);
            bool ch2 = RefreshExposedPropertiesList(referenceAdvancedFloatProperties, exposedAdvancedFloatProperties);
            bool ch3 = RefreshExposedPropertiesList(referenceIntProperties, exposedIntProperties);
            bool ch4 = RefreshExposedPropertiesList(referenceBoolProperties, exposedBoolProperties);
            bool ch5 = RefreshExposedPropertiesList(referenceStringProperties, exposedStringProperties);
            bool ch6 = RefreshExposedPropertiesList(referenceVector3Properties, exposedVector3Properties);
            bool ch7 = RefreshExposedPropertiesList(referenceGameObjectProperties, exposedGameObjectProperties);
            bool ch8 = RefreshExposedPropertiesList(referenceScriptableObjectProperties, exposedScriptableObjectProperties);
            bool ch9 = RefreshExposedPropertiesList(referenceActorProperties, exposedActorProperties);
            bool ch10 = RefreshExposedPropertiesList(referenceObjectProperties, exposedObjectProperties);

            return ch1 || ch2 || ch3 || ch4 || ch5 || ch6 || ch7 || ch8 || ch9 || ch10;
        }

        private static bool RefreshExposedPropertiesList<T>(List<GenericExposableProperty<T>> referenceList, List<GenericExposableProperty<T>> exposedList)
        {
            bool changed = false;

            for(int i = 0; i < exposedList.Count; i++)
            {
                var exposedProperty = exposedList[i];

                bool foundMatch = false;
                for(int j = 0; j < referenceList.Count; j++)
                {
                    var referenceProperty = referenceList[j];

                    if(referenceProperty.expose && referenceProperty.ID == exposedProperty.ID)
                    {
                        if(exposedProperty.name != referenceProperty.name)
                        {
                            exposedProperty.name = referenceProperty.name;
                            changed = true;
                        }
                        else if(exposedProperty.tooltip != referenceProperty.tooltip)
                        {
                            exposedProperty.tooltip = referenceProperty.tooltip;
                            changed = true;
                        }

                        foundMatch = true;
                        break;
                    }
                }

                if(!foundMatch)
                {
                    exposedList.RemoveAt(i);
                    i--;
                    changed = true;
                }
            }

            for (int i = 0; i < referenceList.Count; i++)
            {
                var referenceProperty = referenceList[i];

                if(!referenceProperty.expose)
                    continue;

                bool foundMatch = false;
                for (int j = 0; j < exposedList.Count; j++)
                {
                    var exposedProperty = exposedList[j];

                    if(exposedProperty.ID == referenceProperty.ID)
                    {
                        foundMatch = true;
                        break;
                    }
                }

                if(!foundMatch)
                {
                    exposedList.Add(new GenericExposableProperty<T>(referenceProperty.name, referenceProperty.tooltip, referenceProperty.value, referenceProperty.ID));
                    changed = true;
                }
            }

            return changed;
        }
    }
}