using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.ModularSystem;

namespace TafraKit.CharacterControls
{
    public class CharacterAnimator : InternallyModularComponent<CharacterAnimatorModule>
    {
        [SerializeField] protected Animator animator;
        [SerializeReferenceListContainer("modules", false, "Module", "Modules")]
        [SerializeField] protected CharacterAnimatorModulesContainer animatorModules;

        protected TafraActor actor;
        protected InfluenceReceiver<float> animatorSpeedInfluenceReceiver;

        public TafraActor Actor => actor;
        public Animator Animator => animator;
        protected override List<CharacterAnimatorModule> InternalModules => animatorModules.Modules;

        protected override void Awake()
        {
            base.Awake();
            
            actor = GetComponent<TafraActor>();

            animatorSpeedInfluenceReceiver = new InfluenceReceiver<float>(ShouldReplaceAnimatorSpeed, OnActiveAnimatorSpeedInfluenceUpdated, null, OnAllAnimatorSpeedInfluencesCleared);

            for(int i = 0; i < modulesCount; i++)
            {
                var module = animatorModules.Modules[i];

                if(module == null)
                    continue;

                module.Initialize(this);
            }
        }

        #region Callbacks
        private bool ShouldReplaceAnimatorSpeed(float newSpeed, float oldSpeed)
        {
            return newSpeed < oldSpeed;
        }
        private void OnActiveAnimatorSpeedInfluenceUpdated(float speed)
        {
            animator.speed = speed;
        }
        private void OnAllAnimatorSpeedInfluencesCleared()
        {
            animator.speed = 1;
        }
        #endregion

        #region Public Functions
        public void SetAnimatorSpeed(string influencer, float speed)
        {
            animatorSpeedInfluenceReceiver.AddInfluence(influencer, speed);
        }
        public void ResetAnimatorSpeed(string influencer)
        {
            animatorSpeedInfluenceReceiver.RemoveInfluence(influencer);
        }
        #endregion
    }
}