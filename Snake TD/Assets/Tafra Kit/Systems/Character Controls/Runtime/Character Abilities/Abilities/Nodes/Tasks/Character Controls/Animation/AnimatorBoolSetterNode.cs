using System.Collections;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using TafraKit.GraphViews;
using UnityEngine;

namespace TafraKit.Internal.CharacterControls
{
    [SearchMenuItem("Tasks/Character Controls/Animation/Animator Bool Setter"), GraphNodeName("Animator Bool Setter", "Animator Bool Setter")]
    public class AnimatorBoolSetterNode : AbilityTaskNode
    {
        [SerializeField] private string boolName;
        [SerializeField] private bool targetValue = true;

        private CharacterAnimator characterAnimator;
        private Animator animator;
        private int boolNameHash;

        public AnimatorBoolSetterNode(AnimatorBoolSetterNode other) : base(other)
        {
            boolName = other.boolName;
            targetValue = other.targetValue;
            characterAnimator = other.characterAnimator;
            animator = other.animator;
            boolNameHash = other.boolNameHash;
        }
        public AnimatorBoolSetterNode()
        {

        }

        protected override void OnInitialize()
        {
            CharacterAnimator characterAnimator = actor.GetCachedComponent<CharacterAnimator>();

            if(characterAnimator != null)
            {
                animator = characterAnimator.Animator;
                boolNameHash = Animator.StringToHash(boolName);
            }
        }
        protected override void OnStart()
        {
            animator.SetBool(boolNameHash, targetValue);
        }
        protected override void OnEnd()
        {
            animator.SetBool(boolNameHash, !targetValue);
        }
        protected override BTNodeState OnUpdate()
        {
            return BTNodeState.Running;
        }
        protected override BTNode CloneContent()
        {
            AnimatorBoolSetterNode clonedNode = new AnimatorBoolSetterNode(this);

            return clonedNode;
        }
    }
}