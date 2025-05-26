using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.ModularSystem;
using TafraKit.CharacterControls;

namespace TafraKit.AI3
{
    [System.Serializable]
    public abstract class AIAnimatorModule : CharacterAnimatorModule
    {
        protected AIAnimator aiAnimator;

        public override void Initialize(CharacterAnimator characterAnimator)
        {
            this.characterAnimator = characterAnimator;

            animator = characterAnimator.Animator;
            actor = characterAnimator.Actor;
            aiAnimator = characterAnimator as AIAnimator;

            OnInitialize();
        }
    }
}