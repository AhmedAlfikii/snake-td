using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKitEditor.GraphViews;
using TafraKit.CharacterControls;
using System;
using TafraKit.GraphViews;
using TafraKit.Internal.CharacterControls;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace TafraKitEditor.CharacterControls
{
    public class AbilityBTGraphView : BehaviourTreeGraphView<Ability, AbilityNode>
    {
        protected override Type DefaultNodeEditor => typeof(AbilityNodeEditor);

        public AbilityBTGraphView(SerializedObject serializedObject, SerializedProperty behaviourTreeProperty, 
            IBTNodesContainer<AbilityNode> behaviourTree, string graphName, GraphWindow<Ability> window) :
            base(serializedObject, behaviourTreeProperty, behaviourTree, graphName, window)
        {
        }

        protected override bool CanNodeHaveMultipleChildren(GraphNode node)
        {
            return true;
        }
        protected override void DisplaySearchWindowForGhostEdge()
        {
            if(ghostEdge.output != null)
            {
                //We are looking for a child.

                AbilityNode node = ((BTNodeEditor)ghostEdge.output.node).Node as AbilityNode;

                //Active nodes and root nodes can only have composite nodes as children.
                if(node is ActiveAbilityNode || node is RootNode || node is TriggerAbilityNode)
                    searchProvider.SetNextShowBaseType(typeof(AbilityCompositeNode));
            }
            else if(ghostEdge.input != null)
            {
                //We are looking for a parent.

                AbilityNode node = ((BTNodeEditor)ghostEdge.input.node).Node as AbilityNode;

                //Task nodes can't be the parent of any nodes.
                searchProvider.AddNextShowExcludedType(typeof(AbilityTaskNode));
            }

            base.DisplaySearchWindowForGhostEdge();
        }
        protected override void OnGraphElementOpenRequest(GraphElement element)
        {
            if(element is BTNodeEditor btNodeEditor && btNodeEditor.Node is SubBehaviourTreeTaskNode subBehaviourTree)
            {
                SerializedProperty nodeProperty = GetNodeProperty(subBehaviourTree);
                SerializedProperty externalTreeProperty = nodeProperty.FindPropertyRelative("externalBehaviourTree");
                SerializedProperty internalTreeProperty = nodeProperty.FindPropertyRelative("internalBehaviourTree");

                SerializedProperty availableTreeProperty;
                SerializedObject availableSerializedObject;
                if(externalTreeProperty.objectReferenceValue != null)
                {
                    availableSerializedObject = new SerializedObject(externalTreeProperty.objectReferenceValue);
                    availableTreeProperty = availableSerializedObject.FindProperty("behaviourTree");
                }
                else
                {
                    availableSerializedObject = serializedObject;
                    availableTreeProperty = internalTreeProperty;
                }

                AbilityBTGraphView graph = new AbilityBTGraphView(availableSerializedObject, availableTreeProperty, subBehaviourTree.AvailableBehaviourTree, subBehaviourTree.Name, window);

                OnOpenNestedViewRequest?.Invoke(graph);
            }
        }
    }
}