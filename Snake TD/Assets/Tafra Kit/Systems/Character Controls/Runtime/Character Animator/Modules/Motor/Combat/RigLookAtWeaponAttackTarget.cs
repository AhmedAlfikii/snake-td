using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.Healthies;
using TafraKit.Weaponry;
using UnityEngine;

namespace TafraKit.CharacterControls
{
    [SearchMenuItem("Combat/Rig Look At Weapon Attack Target")]
    public class RigLookAtWeaponAttackTarget : CharacterAnimatorModule
    {
        private CharacterCombat characterCombat;
        private WeaponAutoAttacking curWeapon;
        #if TAFRA_ANIMATION_RIGGING
        private AnimationRigLookAtController rigLookAtController;
        #endif
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
                TafraDebugger.Log("Rig Look At Weapon Attack Target", "No \"Character Combat\" component assigned to the game object. Will not work.", TafraDebugger.LogType.Error);
                return;
            }

            #if TAFRA_ANIMATION_RIGGING
            rigLookAtController = characterAnimator.GetComponent<AnimationRigLookAtController>();

            if (rigLookAtController == null)
            {
                TafraDebugger.Log("Rig Look At Weapon Attack Target", "No \"Animation Rig Look At Controller\" component assigned to the game object. Will not work.", TafraDebugger.LogType.Error);
                return;
            }
            #endif
        }
        protected override void OnEnable()
        {
            if (characterCombat.MainWeapon != null)
                OnWeaponEquipped(characterCombat.MainWeapon);

            characterCombat.OnWeaponEquip.AddListener(OnWeaponEquipped);
            characterCombat.OnWeaponUnequip.AddListener(OnWeaponUnquipped);

            OnAttackTargetUpdated();
        }
        protected override void OnDisable()
        {
            characterCombat.OnWeaponEquip.RemoveListener(OnWeaponEquipped);
            characterCombat.OnWeaponUnequip.RemoveListener(OnWeaponUnquipped);

            #if TAFRA_ANIMATION_RIGGING
            rigLookAtController.RemoveLookAtTarget(idHash);
            #endif
        }

        private void OnWeaponEquipped(Weapon weapon)
        {
            curWeapon = weapon as WeaponAutoAttacking;

            if(curWeapon == null)
                return;

            curWeapon.OnAttackTargetUpdated.AddListener(OnAttackTargetUpdated);

            OnAttackTargetUpdated();
        }
        private void OnWeaponUnquipped(Weapon weapon)
        {
            WeaponAutoAttacking autoWeapon = weapon as WeaponAutoAttacking;
            
            if(autoWeapon == null)
                return;

            if(curWeapon == autoWeapon)
                curWeapon = null;

            autoWeapon.OnAttackTargetUpdated.RemoveListener(OnAttackTargetUpdated);

            OnAttackTargetUpdated();
        }

        private void OnAttackTargetUpdated()
        {
        #if TAFRA_ANIMATION_RIGGING
            bool hasTarget = false;
            if(curWeapon != null)
            {
                Healthy attackTarget = curWeapon.AttackTargetHealthy;
                if(attackTarget != null)
                {
                    rigLookAtController.SetLookAtTarget(idHash, attackTarget.TargetPoint);
                    hasTarget = true;
                }
            }

            if(!hasTarget)
            {
                rigLookAtController.RemoveLookAtTarget(idHash);
            }
            #endif
        }
    }
}