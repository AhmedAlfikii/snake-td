using System;
using System.Collections.Generic;
using TafraKit.ModularSystem;
using TafraKit.Weaponry;
using Unity.VisualScripting;
using UnityEngine;

namespace TafraKit.RPG
{
    [System.Serializable]
    public class WeaponRigProfileModule : CharacterEquipmentModule
    {
        [System.Serializable]
        public class WeaponContstraintProfile
        {
            public Equipment[] Weapons;
            public WeaponRigConstraintsProfile RigConstraintsProfile;
        }

        [SerializeField] private WeaponContstraintProfile[] weaponConstraintProfiles;

        private Dictionary<string, WeaponRigConstraintsProfile> weaponProfile = new Dictionary<string, WeaponRigConstraintsProfile>();
        #if TAFRA_ANIMATION_RIGGING
        private AnimationRigLookAtController animationRigLookAtController;
        #endif
    
        public override bool UseUpdate => false;
        public override bool UseLateUpdate => false;
        public override bool UseFixedUpdate => false;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            #if TAFRA_ANIMATION_RIGGING
            animationRigLookAtController = characterEquipment.GetComponent<AnimationRigLookAtController>();
            #endif

            for(int i = 0; i < weaponConstraintProfiles.Length; i++)
            {
                for(int j = 0; j < weaponConstraintProfiles[i].Weapons.Length; j++)
                {
                    weaponProfile.Add(weaponConstraintProfiles[i].Weapons[j].ID, weaponConstraintProfiles[i].RigConstraintsProfile);
                }
            }
        }
        protected override void OnEnable()
        {
            characterEquipment.OnWeaponEquipped.AddListener(OnWeaponEquipped);
        }
        protected override void OnDisable()
        {
            characterEquipment.OnWeaponEquipped.RemoveListener(OnWeaponEquipped);
        }

        private void OnWeaponEquipped(Weapon weapon, Equipment equipment)
        {
            if(weaponProfile.TryGetValue(equipment.OriginalID, out WeaponRigConstraintsProfile rigConstraintsProfile))
            {
                #if TAFRA_ANIMATION_RIGGING
                animationRigLookAtController.SetConstraintsProperties(rigConstraintsProfile);
                #endif
            }
        }
    }
}