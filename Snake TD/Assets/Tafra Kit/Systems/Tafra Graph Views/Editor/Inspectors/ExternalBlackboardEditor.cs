using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TafraKit.GraphViews;
using UnityEngine.UIElements;
using TafraKit;
using System.Linq;

namespace TafraKitEditor.GraphViews
{
    [CustomEditor(typeof(ExternalBlackboard))]
    public class ExternalBlackboardEditor : Editor
    {
        private VisualElement propertiesContainer;
        private ExternalBlackboard externalBlackboard;
        private VisualElement root;
        private Button addButton;

        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();
           
            VisualTreeAsset mainUxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Tafra Kit/Systems/Artificial Intelligence V3/Editor/USS & UXML/ExternalBlackboardEditor.uxml");
            mainUxml.CloneTree(root);

            propertiesContainer = root.Q<VisualElement>("properties-container");

            addButton = root.Q<Button>("add-button");
            addButton.clicked += AddItemRequest;

            externalBlackboard = (ExternalBlackboard)target;
            
            PopulateBlackboard();

            return root;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
        private void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndoRedoPerformed;
        }
        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
        }
        private void OnUndoRedoPerformed()
        {
            root.schedule.Execute(() =>
            {
                PopulateBlackboard();
            });
        }

        private void PopulateBlackboard(Type selectLastItemOfType = null)
        {
            propertiesContainer.Clear();

            for (int typeIndex = 0; typeIndex < GraphBlackboard.SupportedTypes.Count; typeIndex++)
            {
                Type type = GraphBlackboard.SupportedTypes[typeIndex];
                object obj = ZHelper.CallGenericMethod<object>(typeof(ExternalBlackboard), "GetAllPropertiesOfGenericType", externalBlackboard, type);
                List<object> propertyObjects = (obj as IEnumerable<object>).Cast<object>().ToList();

                for (int i = 0; i < propertyObjects.Count; i++)
                {
                    Type propertyType = propertyObjects[i].GetType();
                    Type propertyValueType = propertyType.GetTypeInfo().GenericTypeArguments[0];

                    var field = AddPropertyField(propertyValueType, propertyObjects[i], i);

                    if (i == propertyObjects.Count - 1 && propertyValueType == selectLastItemOfType)
                    {
                        root.schedule.Execute(() =>
                        {
                            field.FocusName();
                        }).ExecuteLater(120);
                    }
                }
            }
        }
        private void AddItemRequest()
        {
            GenericMenu menu = new GenericMenu();

            List<Type> supportedTypes = GraphBlackboard.SupportedTypes;

            for (int i = 0; i < supportedTypes.Count; i++)
            {
                Type type = supportedTypes[i];
                string typeName = ZHelper.GetNiceTypeName(type);
                menu.AddItem(new GUIContent(typeName), false, () =>
                {
                    CreateItemOfType(type, typeName);
                });
            }

            Vector2 menuPosition = addButton.worldTransform.GetPosition();
            Rect menuRect = new Rect(menuPosition, Vector2.zero);
            menu.DropDown(menuRect);
        }
        private void CreateItemOfType(Type type, string itemName)
        {
            Undo.RecordObject(serializedObject.targetObject, "Add Item To Blackboard");

            externalBlackboard.AddProperty(type, itemName, out itemName);

            EditorUtility.SetDirty(serializedObject.targetObject);

            PopulateBlackboard(type);
        }
        private ExternalBlackboardFieldView AddPropertyField(Type propertyValueType, object propertyObject, int indexInPropertiesList)
        {
            SerializedProperty propertySerializedProperty = GetPropertySerializedProperty(propertyValueType, indexInPropertiesList);

            ExternalBlackboardFieldView field = new ExternalBlackboardFieldView(propertySerializedProperty, propertyObject as ExposableProperty);
            propertiesContainer.Add(field);
            
            field.OnRenameRequest = OnPropertyFieldRenameRequest;
            field.OnDeleted = OnPropertyFieldDeleted;

            return field;
        }
        private SerializedProperty GetPropertySerializedProperty(Type valueType, int indexInPropertiesList)
        {
            FieldInfo[] blackboardPropertyFields = externalBlackboard.Blackboard.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            string propertyFieldName = null;
            for (int i = 0; i < blackboardPropertyFields.Length; i++)
            {
                var field = blackboardPropertyFields[i];
                Type propertyValueType = field.FieldType.GetTypeInfo().GenericTypeArguments[0].GetTypeInfo().GenericTypeArguments[0];

                if (propertyValueType == valueType)
                {
                    propertyFieldName = field.Name;
                    break;
                }
            }

            if (propertyFieldName == null)
            {
                Debug.LogError($"Couldn't find a property list with the type {valueType} in {externalBlackboard}");
                return null;
            }

            serializedObject.Update();

            return serializedObject.FindProperty("blackboard").FindPropertyRelative(propertyFieldName).GetArrayElementAtIndex(indexInPropertiesList);
        }

        private void OnPropertyFieldRenameRequest(ExternalBlackboardFieldView field, string previousName, string newName)
        {
            Undo.RecordObject(target, "Property Name Change");
            string validatedName = externalBlackboard.RenameProperty(field.ExposableProperty, newName);
            field.SetName(validatedName);
        }
        private void OnPropertyFieldDeleted()
        {
            PopulateBlackboard();
        }
    }
}