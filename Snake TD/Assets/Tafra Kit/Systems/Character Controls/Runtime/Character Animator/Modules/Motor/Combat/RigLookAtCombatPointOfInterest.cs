using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.CharacterControls
{
    [SearchMenuItem("Combat/Rig Look At Combat Point Of Interest")]
    public class RigLookAtCombatPointOfInterest : CharacterAnimatorModule
    {
        [Tooltip("The combat point of interset has to be assigned to this transform's position. Use \"Point Of Interest Transform\" module on the character combat component to do so.")]
        [SerializeField] private Transform pointOfInterestTransform;
        [SerializeField] private bool onlyLookWhileAggressive = true;

        private CharacterCombat characterCombat;
        #if TAFRA_ANIMATION_RIGGING
        private AnimationRigLookAtController rigLookAtController;
        #endif
        private bool isAggressive;
        private int idHash = Animator.StringToHash(nameof(RigLookAtCombatPointOfInterest));

        public override bool UseUpdate => false;
        public override bool UseLateUpdate => true;
        public override bool UseFixedUpdate => false;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            characterCombat = characterAnimator.GetComponent<CharacterCombat>();

            if (characterCombat == null)
            {
                TafraDebugger.Log("Rig Look At Combat Point Of Interest", "No \"Character Combat\" component assigned to the game object. Will not work.", TafraDebugger.LogType.Error);
                return;
            }

            #if TAFRA_ANIMATION_RIGGING
            rigLookAtController = characterAnimator.GetComponent<AnimationRigLookAtController>();

            if (rigLookAtController == null)
            {
                TafraDebugger.Log("Rig Look At Combat Point Of Interest", "No \"Animation Rig Look At Controller\" component assigned to the game object. Will not work.", TafraDebugger.LogType.Error);
                return;
            }
            #endif
        }
        protected override void OnEnable()
        {
            #if TAFRA_ANIMATION_RIGGING
            if (onlyLookWhileAggressive)
                characterCombat.OnAggressiveStateChanged.AddListener(OnAggressiveStateChanged);
            else
                rigLookAtController.SetLookAtTarget(idHash, pointOfInterestTransform);
            #endif
        }
        protected override void OnDisable()
        {
            if (onlyLookWhileAggressive)
                characterCombat.OnAggressiveStateChanged.RemoveListener(OnAggressiveStateChanged);

            #if TAFRA_ANIMATION_RIGGING
            rigLookAtController.RemoveLookAtTarget(idHash, pointOfInterestTransform);
            #endif
        }
        private void OnAggressiveStateChanged(bool isAggressive)
        {
            this.isAggressive = isAggressive;

            #if TAFRA_ANIMATION_RIGGING
            if (isAggressive)
                rigLookAtController.SetLookAtTarget(idHash, pointOfInterestTransform);
            else
                rigLookAtController.RemoveLookAtTarget(idHash, pointOfInterestTransform);
            #endif
        }
    }
}