using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TafraKit.MotionFactory;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace TafraKitEditor.MotionFactory
{
    public abstract class BaseMotionDrawer : PropertyDrawer
    {
        protected static Dictionary<string, string> propertyTypeReferencesPropertyName = new Dictionary<string, string>();
        protected static Dictionary<string, Dictionary<string, Type>> propertyTypeReferencesTypesByName = new Dictionary<string, Dictionary<string, Type>>();
        protected static Dictionary<string, string> propertyTypeTargetsPropertyName = new Dictionary<string, string>();
        protected static HashSet<string> cachedPropertyTypes = new HashSet<string>();

        public VisualTreeAsset uxml;

        protected virtual string Name => "Motion";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();

            uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Tafra Kit/UI Toolkit Assets/UXML/Motion Factory/BaseMotionEditor.uxml");
            uxml.CloneTree(root);
            VisualElement motionPropertiesBox = root.Q<VisualElement>("MotionPropertiesBox");
            VisualElement targetsBox = root.Q<VisualElement>("TargetsBox");

            Label titleLabel = root.Q<Label>("Title");
            titleLabel.text = Name;
            Label propertyNameLabel = root.Q<Label>("PropertyName");
            propertyNameLabel.text = property.displayName;

            SerializedProperty targetInitialStateProperty = property.FindPropertyRelative("targetInitialState");

            string actualPropertyTypeName = property.GetTypeActualName();

            if(!cachedPropertyTypes.Contains(actualPropertyTypeName))
                ExtractPropertyReferencesAndTargetsFieldsNames(actualPropertyTypeName);

            propertyTypeReferencesPropertyName.TryGetValue(actualPropertyTypeName, out string referencesPropertyName);
            propertyTypeTargetsPropertyName.TryGetValue(actualPropertyTypeName, out string targetsPropertyName);

            SerializedProperty referencesProperty = null;
            SerializedProperty targetsProperty = null;

            bool foundTargetInitialStateProperty = false;

            IEnumerable<SerializedProperty> propertyChildren = property.GetVisibleChildren();
            for(int i = 0; i < propertyChildren.Count(); i++)
            {
                SerializedProperty p = propertyChildren.ElementAt(i);
                PropertyField pf = new PropertyField(p);

                if(p.name == referencesPropertyName)
                    referencesProperty = p;
                else if(p.name == targetsPropertyName)
                    targetsProperty = p;
                else
                {
                    motionPropertiesBox.Add(pf);

                    if(!foundTargetInitialStateProperty && p.name == "targetInitialState")
                    {
                        pf.RegisterCallback<ChangeEvent<bool>>((on) => 
                        {
                            if(targetsProperty == null || on.newValue)
                                targetsBox.style.display = DisplayStyle.None;
                            else
                                targetsBox.style.display = DisplayStyle.Flex;
                        });
                    }
                }
            }

            if (targetsProperty == null || targetInitialStateProperty.boolValue)
                targetsBox.style.display = DisplayStyle.None;
            else
                targetsBox.style.display = DisplayStyle.Flex;

            ProcessTargets(targetsProperty, targetsBox);
            ProcessReferences(actualPropertyTypeName, referencesProperty, root.Q<Foldout>("References"), root.Q<VisualElement>("UnassignedReferencesBox"));
            
            OnCreated(property);

            return root;
        }

        protected void ProcessReferences(string propertyTypeName, SerializedProperty referencesProperty, VisualElement assignedReferencesVE, VisualElement unassignedReferencesVE)
        {
            if(referencesProperty == null)
            {
                assignedReferencesVE.style.display = DisplayStyle.None;
                unassignedReferencesVE.style.display = DisplayStyle.None;

                return;
            }

            Dictionary<string, Type> referencePropertiesTypesByName = propertyTypeReferencesTypesByName[propertyTypeName];
            int referencesCount = referencePropertiesTypesByName.Count;

            List<SerializedProperty> assignedProperties = null;
            List<SerializedProperty> unassignedProperties = null;
            List<string> unassignedPropertiesNames = null;

            for(int i = 0; i < referencesCount; i++)
            {
                KeyValuePair<string, Type> referencePropertyNameType = referencePropertiesTypesByName.ElementAt(i);

                string referencePropertyName = referencePropertyNameType.Key;
                Type referencePropertyType = referencePropertyNameType.Value;
                SerializedProperty referenceProperty = referencesProperty.FindPropertyRelative(referencePropertyName);

                if(referenceProperty.propertyType != SerializedPropertyType.ObjectReference || referenceProperty.objectReferenceValue == null)
                {
                    //Only create the list if there's at least one unassigend reference to avoid unnecessarily creating a list.
                    if(unassignedProperties == null)
                    {
                        unassignedProperties = new List<SerializedProperty>();
                        unassignedPropertiesNames = new List<string>();
                    }

                    unassignedProperties.Add(referenceProperty);
                    unassignedPropertiesNames.Add(referencePropertyName);
                }
                else
                {
                    //Only create the list if there's at least one assigend reference to avoid unnecessarily creating a list.
                    if(assignedProperties == null)
                        assignedProperties = new List<SerializedProperty>();

                    assignedProperties.Add(referenceProperty);
                }
            }

            SerializedObject serializedObject = referencesProperty.serializedObject;
            Object controllerObject = serializedObject.targetObject;
            MonoBehaviour controllerBehaviour = controllerObject as MonoBehaviour;

            if(controllerBehaviour && unassignedProperties != null && unassignedProperties.Count > 0)
            {
                for(int i = 0; i < referencesCount; i++)
                {
                    KeyValuePair<string, Type> referencePropertyNameType = referencePropertiesTypesByName.ElementAt(i);
                  
                    int propertyIndex = unassignedPropertiesNames.IndexOf(referencePropertyNameType.Key);

                    if(propertyIndex > -1)
                    {
                        Type referenceType = referencePropertyNameType.Value;
                        bool isComponent = typeof(Component).IsAssignableFrom(referenceType);
                        
                        if (isComponent)
                        {
                            SerializedProperty unassignedProperty = unassignedProperties[propertyIndex];

                            unassignedProperty.objectReferenceValue = controllerBehaviour.GetComponent(referenceType);

                            unassignedProperties.RemoveAt(propertyIndex);
                            unassignedPropertiesNames.RemoveAt(propertyIndex);

                            //Only create the list if there's at least one assigend reference to avoid unnecessarily creating a list.
                            if(assignedProperties == null)
                                assignedProperties = new List<SerializedProperty>();

                            assignedProperties.Add(unassignedProperty);
                        }
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }

            if(assignedProperties != null)
            {
                for(int i = 0; i < assignedProperties.Count; i++)
                {
                    PropertyField assignedPF = new PropertyField(assignedProperties[i]);
                    AddPropertyFieldToAssignedPropertiesBox(assignedPF);
                }
            }
            if(unassignedProperties != null)
            {
                for(int i = 0; i < unassignedProperties.Count; i++)
                {
                    PropertyField unassignedPF = new PropertyField(unassignedProperties[i]);
                    AddPropertyFieldToUnssignedPropertiesBox(unassignedPF);
                }
            }

            bool foundUnassignedReferences = unassignedProperties != null && unassignedProperties.Count > 0;
            bool foundAssignedReferences = referencesCount > 0 && (unassignedProperties == null || (referencesCount > unassignedProperties.Count));

            UpdateReferenceContainers();

            void AddPropertyFieldToAssignedPropertiesBox(PropertyField pf)
            {
                pf.style.unityFontStyleAndWeight = FontStyle.Normal;
                assignedReferencesVE.Add(pf);

                pf.RegisterCallback<ChangeEvent<Object>>(OnAssignedPFValueChange);
                assignedReferencesVE.Add(pf);

                void OnAssignedPFValueChange(ChangeEvent<Object> objEv)
                {
                    if(objEv.newValue != null)
                        return;

                    AddPropertyFieldToUnssignedPropertiesBox(pf);
                    pf.UnregisterCallback<ChangeEvent<Object>>(OnAssignedPFValueChange);

                    UpdateReferenceContainers();
                }
            }
            void AddPropertyFieldToUnssignedPropertiesBox(PropertyField pf)
            {
                pf.style.unityFontStyleAndWeight = FontStyle.Bold;

                pf.RegisterCallback<ChangeEvent<Object>>(OnUnassignedPFValueChange);
                unassignedReferencesVE.Add(pf);

                void OnUnassignedPFValueChange(ChangeEvent<Object> objEv)
                {
                    if(objEv.newValue == null)
                        return;

                    AddPropertyFieldToAssignedPropertiesBox(pf);
                    pf.UnregisterCallback<ChangeEvent<Object>>(OnUnassignedPFValueChange);

                    UpdateReferenceContainers();
                }
            }

            void UpdateReferenceContainers()
            {
                assignedReferencesVE.style.display = assignedReferencesVE.childCount > 0 ? DisplayStyle.Flex : DisplayStyle.None;
                unassignedReferencesVE.style.display = unassignedReferencesVE.childCount > 1 ? DisplayStyle.Flex : DisplayStyle.None;

            }
        }
        protected void ProcessTargets(SerializedProperty targetsProperty, VisualElement targetsVE)
        {
            if(targetsProperty == null)
                return;

            IEnumerable<SerializedProperty> targetPropertyChildren = targetsProperty.GetVisibleChildren();

            for(int i = 0; i < targetPropertyChildren.Count(); i++)
            {
                SerializedProperty p = targetPropertyChildren.ElementAt(i);
                PropertyField pf = new PropertyField(p);

                targetsVE.Add(pf);
            }

        }
        protected void ExtractPropertyReferencesAndTargetsFieldsNames(string propertyTypeName)
        {
            bool foundReferencesProperty = false;
            bool foundTargetsProperty = false;

            Type referencesPropertyType = null;

            Type propertyType = Type.GetType($"TafraKit.MotionFactory.{propertyTypeName}, Assembly-CSharp");
            List<FieldInfo> propertyFields = new List<FieldInfo>(propertyType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic));
            for(int j = 0; j < propertyFields.Count; j++)
            {
                if(!foundReferencesProperty && typeof(MotionSelfReferences).IsAssignableFrom(propertyFields[j].FieldType))
                {
                    if(!propertyTypeReferencesPropertyName.TryAdd(propertyTypeName, propertyFields[j].Name))
                        propertyTypeReferencesPropertyName[propertyTypeName] = propertyFields[j].Name;

                    referencesPropertyType = propertyFields[j].FieldType;
                    foundReferencesProperty = true;
                }

                if(!foundTargetsProperty && typeof(MotionTargets).IsAssignableFrom(propertyFields[j].FieldType))
                {
                    if(!propertyTypeTargetsPropertyName.TryAdd(propertyTypeName, propertyFields[j].Name))
                        propertyTypeTargetsPropertyName[propertyTypeName] = propertyFields[j].Name;

                    foundTargetsProperty = true;
                }
            }

            if (foundReferencesProperty)
            {
                List<FieldInfo> referenceFields = new List<FieldInfo>(referencesPropertyType.GetFields());
                
                propertyTypeReferencesTypesByName.TryAdd(propertyTypeName, new Dictionary<string, Type>() { });

                for(int i = 0; i < referenceFields.Count; i++)
                {
                    if (!propertyTypeReferencesTypesByName[propertyTypeName].TryAdd(referenceFields[i].Name, referenceFields[i].FieldType)) 
                    {
                        propertyTypeReferencesTypesByName[propertyTypeName][referenceFields[i].Name] = referenceFields[i].FieldType;
                    }
                }
            }

            cachedPropertyTypes.Add(propertyTypeName);
        }

        protected virtual void OnCreated(SerializedProperty property)
        { 

        }
    }
}

