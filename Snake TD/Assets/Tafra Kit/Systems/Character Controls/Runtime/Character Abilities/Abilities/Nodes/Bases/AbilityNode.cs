using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.GraphViews;
using TafraKit.CharacterControls;
using System;
using TafraKit.Internal.GraphViews;

namespace TafraKit.Internal.CharacterControls
{
    [Serializable]
    public abstract class AbilityNode : BTNode
    {
        [NonSerialized] protected Ability ability;
        [NonSerialized] protected CharacterAbilities characterAbilities;
        [NonSerialized] protected TafraActor actor;
        /// <summary>
        /// The blackboard that gets set by trigger nodes, if any.
        /// </summary>
        [NonSerialized] protected SecondaryBlackboard triggerBlackboard;

        public AbilityNode(AbilityNode other) : base(other)
        {
            ability = other.ability;
            characterAbilities = other.characterAbilities;
            actor = other.actor;
            triggerBlackboard = other.triggerBlackboard;
        }
        public AbilityNode()
        {

        }

        protected virtual void OnTriggerBlackboardSet() { }

        public void Initialize(Ability ability)
        {
            this.ability = ability;
            characterAbilities = ability.CharacterAbilities;
            actor = ability.Actor;

            OnInitialize();
        }
        public void LateInitialize()
        {
            OnLateInitialize();
        }
        public void SetTriggerBlackboard(SecondaryBlackboard triggerBlackboard, bool setOnChildren = true)
        {
            this.triggerBlackboard = triggerBlackboard;

            if(setOnChildren)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    var child = children[i] as AbilityNode;
                    
                    child.SetTriggerBlackboard(triggerBlackboard, true);
                }
            }

            OnTriggerBlackboardSet();
        }

        /// <summary>
        /// Gets invoked after initialize has been called on all nodes.
        /// </summary>
        public virtual void OnLateInitialize() { }
    }
}