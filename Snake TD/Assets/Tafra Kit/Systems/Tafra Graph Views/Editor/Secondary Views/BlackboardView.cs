using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using TafraKit;
using TafraKit.GraphViews;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace TafraKitEditor.GraphViews
{
    public class BlackboardView : Blackboard
    {
        public Action<SerializedProperty, string> OnFieldSelected;
        public Action<SerializedProperty> OnFieldUnselected;
       
        private GraphBlackboard blackboardObject;
        private Button addButton;
        private ScrollView fieldsContainer;
        private SerializedObject mainSerializedObject;
        private SerializedProperty blackboardSerializedProperty;

        public BlackboardView(GraphBlackboard blackboard, SerializedObject mainSerializedObject, SerializedProperty blackboardSerializedProperty, GraphView graphView, string title, string subtitle, Rect position) : base(graphView)
        {
            Initialize(title, subtitle, position);
            
            AssignBlackboardObject(blackboard, mainSerializedObject, blackboardSerializedProperty);
        }
        public BlackboardView(string title, string subTitle, Rect position)
        {
            Initialize(title, subTitle, position);
        }
        private void Initialize(string title, string subTitle, Rect position)
        {
            this.title = title;
            this.subTitle = subTitle;

            SetPosition(position);

            this.Q<VisualElement>("header").style.minHeight = 52;

            addButton = this.Q<Button>("addButton");

            fieldsContainer = new ScrollView();
            fieldsContainer.style.flexGrow = 1;
            contentContainer.Add(fieldsContainer);

            addItemRequested = OnAddItemRequest;
            editTextRequested = OnEditTextRequested;
        }

        public void AssignBlackboardObject(GraphBlackboard blackboard, SerializedObject mainSerializedObject, SerializedProperty blackboardSerializedProperty)
        {
            blackboardObject = blackboard;
            this.mainSerializedObject = mainSerializedObject;
            this.blackboardSerializedProperty = blackboardSerializedProperty;

            PopulateBlackboard();
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if(blackboardObject == null)
                return graphViewChange;

            if(graphViewChange.elementsToRemove != null)
            {
                Undo.RecordObject(mainSerializedObject.targetObject, "Removed Elements");

                for(int i = 0; i < graphViewChange.elementsToRemove.Count; i++)
                {
                    var element = graphViewChange.elementsToRemove[i];

                    if(element is BlackboardFieldView field)
                        blackboardObject.RemoveProperty(field.ValueType, field.ExposableProperty.name);
                }

                EditorUtility.SetDirty(mainSerializedObject.targetObject);
            }

            return graphViewChange;
        }

        public void OnUndoRedoPerformed()
        {
            if(blackboardObject == null)
                return;

            PopulateBlackboard();
        }

        private void OnAddItemRequest(Blackboard blackboard)
        {
            if(blackboardObject == null)
                return;

            GenericMenu menu = new GenericMenu();

            List<Type> supportedTypes = GraphBlackboard.SupportedTypes;

            for(int i = 0; i < supportedTypes.Count; i++)
            {
                Type type = supportedTypes[i];
                string typeName = ZHelper.GetNiceTypeName(type);

                menu.AddItem(new GUIContent(typeName), false, () => 
                {
                    CreateItemOfType(type, typeName);
                });
            }

            Vector2 menuPosition = new Vector2(addButton.layout.center.x, addButton.layout.center.y);
            menuPosition = this.LocalToWorld(menuPosition);
            Rect menuRect = new Rect(menuPosition, Vector2.zero);

            menu.DropDown(menuRect);
        }
        private void OnEditTextRequested(Blackboard blackboard, VisualElement element, string newName)
        {
            if(string.IsNullOrEmpty(newName))
                return;

            BlackboardFieldView field = element as BlackboardFieldView;

            Undo.RecordObject(mainSerializedObject.targetObject, "Rename Property");

            string validatedName = this.blackboardObject.RenameProperty(field.ExposableProperty, newName);

            EditorUtility.SetDirty(mainSerializedObject.targetObject);

            field.text = validatedName;
        }

        private void PopulateBlackboard()
        {
            graphView.graphViewChanged -= OnGraphViewChanged;

            fieldsContainer.Clear();

            for(int typeIndex = 0; typeIndex < GraphBlackboard.SupportedTypes.Count; typeIndex++)
            {
                Type type = GraphBlackboard.SupportedTypes[typeIndex];

                object obj = ZHelper.CallGenericMethod<object>(typeof(GraphBlackboard), "GetAllPropertiesOfGenericType", blackboardObject, type);
                List<object> propertyObjects = (obj as IEnumerable<object>).Cast<object>().ToList();

                for(int i = 0; i < propertyObjects.Count; i++)
                {
                    AddPropertyField(propertyObjects[i], i);
                }
            }
            graphView.graphViewChanged += OnGraphViewChanged;
        }
        private void CreateItemOfType(Type type, string itemName)
        {
            Undo.RecordObject(mainSerializedObject.targetObject, "Add Item To Blackboard");

            ExposableProperty exposableProperty = blackboardObject.AddProperty(type, itemName, out itemName);

            EditorUtility.SetDirty(mainSerializedObject.targetObject);

            BlackboardFieldView field = AddPropertyField(itemName, type, blackboardObject.GetPropertiesCountOfType(type) - 1, exposableProperty);

            ClearSelection();
            AddToSelection(field);

            field.OpenTextEditor();
        }
        private BlackboardFieldView AddPropertyField(object propertyObject, int indexInPropertiesList)
        {
            Type propertyType = propertyObject.GetType();
            Type propertyValueType = propertyType.GetTypeInfo().GenericTypeArguments[0];

            string propertyName = propertyType.GetField("name").GetValue(propertyObject) as string;

            return AddPropertyField(propertyName, propertyValueType, indexInPropertiesList, propertyObject as ExposableProperty);
        }
        private BlackboardFieldView AddPropertyField(string propertyName, Type valueType, int indexInPropertiesList, ExposableProperty exposableProperty)
        {
            SerializedProperty propertySerializedProperty = GetPropertySerializedProperty(valueType, indexInPropertiesList);

            BlackboardFieldView field = new BlackboardFieldView(propertyName, ZHelper.GetNiceTypeName(valueType), exposableProperty, propertySerializedProperty, valueType);

            field.OnFieldSelected = (f) => {  };
            field.OnFieldUnselected = (f) => {  };

            field.OnFieldSelected = OnPropertyFieldSelected;
            field.OnFieldUnselected = OnPropertyFieldUnselected;
            field.OnFieldDeleteRequest = OnPropertyFieldDeleteRequest;

            fieldsContainer.Add(field);
            return field;
        }
        private void OnPropertyFieldSelected(BlackboardFieldView field)
        {
            OnFieldSelected?.Invoke(field.FieldSerializedProperty, field.text);
        }
        private void OnPropertyFieldUnselected(BlackboardFieldView field)
        {
            OnFieldUnselected?.Invoke(field.FieldSerializedProperty);
        }
        private void OnPropertyFieldDeleteRequest(BlackboardFieldView field)
        {
            IEnumerable<GraphElement> elements = new GraphElement[] { field };
            graphView.DeleteElements(elements);
        }

        private SerializedProperty GetPropertySerializedProperty(Type valueType, int indexInPropertiesList)
        {
            FieldInfo[] blackboardPropertyFields = blackboardObject.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            string propertyFieldName = null;
            for(int i = 0; i < blackboardPropertyFields.Length; i++)
            {
                var field = blackboardPropertyFields[i];
                Type propertyValueType = field.FieldType.GetTypeInfo().GenericTypeArguments[0].GetTypeInfo().GenericTypeArguments[0];

                if(propertyValueType == valueType)
                {
                    propertyFieldName = field.Name;
                    break;
                }
            }

            if(propertyFieldName == null)
            {
                Debug.LogError($"Couldn't find a property list with the type {valueType} in {blackboardObject}");
                return null;
            }

            mainSerializedObject.Update();

            if(blackboardSerializedProperty != null)
                return blackboardSerializedProperty.FindPropertyRelative(propertyFieldName).GetArrayElementAtIndex(indexInPropertiesList);
            else if(mainSerializedObject != null)
                return mainSerializedObject.FindProperty(propertyFieldName).GetArrayElementAtIndex(indexInPropertiesList);

            return null;
        }
    }
}