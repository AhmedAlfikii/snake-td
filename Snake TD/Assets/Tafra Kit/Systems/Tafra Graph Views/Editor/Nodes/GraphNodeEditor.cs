using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TafraKit.GraphViews;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TafraKitEditor.GraphViews
{
    public class GraphNodeEditor : Node
    {
        protected GraphNode node;
        protected VisualElement nameChangeFieldContainer;
        protected Label typeName;
        protected TextField nameChangeField;
        protected string typeDisplayName;
        protected string typeShortDisplayName;

        public Action<GraphNodeEditor> OnNodeSelected;
        public Action<GraphNodeEditor> OnNodeUnselected;
        public Action<GraphNodeEditor> OnOpenRequest;
        public Action<Edge> OnEdgeDropperOutsidePort;
        public Action<GraphNodeEditor, string, string> OnRenameRequest;

        public GraphNode Node => node;

        public GraphNodeEditor(GraphNode node)
        {
            Initialize(node);
        }
        public GraphNodeEditor(GraphNode node, string uiFile) : base(uiFile)
        {
            Initialize(node);
        }

        private void Initialize(GraphNode node)
        {
            this.node = node;

            GetTypeDisplayNames();

            if(string.IsNullOrEmpty(node.Name))
                node.Name = typeDisplayName;

            typeName = this.Q<Label>("type-name");
            RefreshDisplayName();

            RegisterCallback<MouseDownEvent>(OnMouseDown);

            CreateNameChangeField();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Edit Script", (a) =>
            {
                TafraEditorUtility.OpenClassScript(node.GetType());
            });
            evt.menu.AppendAction("Select Script", (a) =>
            {
                TafraEditorUtility.SelectClassScript(node.GetType());
            });
        }
        private void CreateNameChangeField()
        {
            nameChangeFieldContainer = this.Q<VisualElement>("name-change-container");
            nameChangeFieldContainer.style.display = DisplayStyle.None;

            nameChangeField = this.Q<TextField>("name-change-field");
            nameChangeField.isDelayed = true;

            nameChangeField.RegisterCallback<FocusOutEvent>((e) =>
            {
                //text = myLayerName;
                nameChangeFieldContainer.style.display = DisplayStyle.None;
            });
            nameChangeField.RegisterCallback<KeyDownEvent>(e =>
            {
                if(e.keyCode == KeyCode.Escape)
                {
                    //text = myLayerName;
                    nameChangeFieldContainer.style.display = DisplayStyle.None;
                }
            });
            nameChangeField.RegisterCallback<ChangeEvent<string>>((ev) =>
            {
                string newName = ev.newValue;
                
                if(string.IsNullOrEmpty(newName))
                    newName = typeDisplayName;

                OnRenameRequest?.Invoke(this, ev.previousValue, newName);
                nameChangeFieldContainer.style.display = DisplayStyle.None;
            });
        }
        private void OnMouseDown(MouseDownEvent evt)
        {
            if(evt.clickCount == 2)
                OnOpenRequest?.Invoke(this);
        }
        protected void EdgeDroppedOutsidePort(Edge edge)
        {
            OnEdgeDropperOutsidePort?.Invoke(edge);
        }
        protected void Rename()
        {
            nameChangeFieldContainer.style.display = DisplayStyle.Flex;
            nameChangeField.SetValueWithoutNotify(node.Name);
            nameChangeField.Focus();
        }
        protected virtual void GetTypeDisplayNames()
        {
            if(node.GetType().GetCustomAttribute<GraphNodeName>() is GraphNodeName graphNodeName)
            {
                typeDisplayName = graphNodeName.Name;
                typeShortDisplayName = graphNodeName.ShortName;
            }
            else
                typeDisplayName = typeShortDisplayName = ObjectNames.NicifyVariableName(node.GetType().Name);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }
        public override void OnUnselected()
        {
            base.OnUnselected();
            OnNodeUnselected?.Invoke(this);
        }
        public void RefreshDisplayName()
        {
            if(typeName != null)
                typeName.text = typeShortDisplayName;

            if(node.Name == typeDisplayName)
                title = node.Name;
            else if(!string.IsNullOrEmpty(node.Name))
            {
                if(typeName != null)
                {
                    typeName.text = typeShortDisplayName;
                    title = node.Name;
                }
                else
                    title = $"{node.Name} ({typeShortDisplayName})";
            }
            else
                title = typeDisplayName;
        }
    }
}
