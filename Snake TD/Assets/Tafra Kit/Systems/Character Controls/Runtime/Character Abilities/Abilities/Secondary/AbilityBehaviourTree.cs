using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.AI3;
using System;

namespace TafraKit.Internal.CharacterControls
{
    [CreateAssetMenu(menuName = "Tafra Kit/Character Controls/Abilities/Ability Behaviour Tree", fileName = "Ability Behaviour Tree")]
    public class AbilityBehaviourTree : ScriptableObject
    {
        [SerializeField] private InternalBehaviourTree behaviourTree;

        public InternalBehaviourTree Tree => behaviourTree;

        public void Clone()
        {
            for(int i = 0; i < behaviourTree.Nodes.Count; i++)
            {
                var node = behaviourTree.Nodes[i];

                if(node is SubBehaviourTreeTaskNode subBehaviourTreeNode)
                    subBehaviourTreeNode.Clone();
            }
        }
    }
}