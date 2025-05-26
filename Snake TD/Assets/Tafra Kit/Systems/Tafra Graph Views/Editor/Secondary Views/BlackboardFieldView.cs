using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.GraphViews;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor.GraphViews
{
    public class BlackboardFieldView : BlackboardField
    {
        public Action<BlackboardFieldView> OnFieldSelected;
        public Action<BlackboardFieldView> OnFieldUnselected;
        public Action<BlackboardFieldView> OnFieldDeleteRequest;

        private ExposableProperty exposableProperty;
        private SerializedProperty fieldSerializedProperty;
        private Type valueType;

        public ExposableProperty ExposableProperty => exposableProperty;
        public SerializedProperty FieldSerializedProperty => fieldSerializedProperty;
        public Type ValueType => valueType;

        public BlackboardFieldView(string text, string typeText, ExposableProperty exposableProperty, SerializedProperty fieldSerializedProperty, Type valueType) : base(null, text, typeText)
        {
            this.exposableProperty = exposableProperty;
            this.fieldSerializedProperty = fieldSerializedProperty;
            this.valueType = valueType;

            style.marginLeft = style.marginRight = 7f;
        }
        protected override void BuildFieldContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildFieldContextualMenu(evt);

            evt.menu.AppendAction("Delete", (a) =>
            {
                OnFieldDeleteRequest?.Invoke(this);
            });

        }
        public override void OnSelected()
        {
            base.OnSelected();

            OnFieldSelected?.Invoke(this);
        }
        public override void OnUnselected()
        {
            base.OnUnselected();

            OnFieldUnselected?.Invoke(this);
        }
    }
}