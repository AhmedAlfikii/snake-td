using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.ModularSystem;

namespace TafraKit.CharacterControls
{
    [System.Serializable]
    public abstract class CharacterAnimatorModule : InternalModule
    {
        protected CharacterAnimator characterAnimator;
        protected Animator animator;
        protected TafraActor actor;

        public virtual void Initialize(CharacterAnimator characterAnimator)
        {
            this.characterAnimator = characterAnimator;
            
            animator = characterAnimator.Animator;
            actor = characterAnimator.Actor;

            OnInitialize();
        }
    }
}