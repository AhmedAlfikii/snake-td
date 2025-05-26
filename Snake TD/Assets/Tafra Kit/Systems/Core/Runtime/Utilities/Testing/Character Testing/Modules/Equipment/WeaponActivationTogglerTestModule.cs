using TafraKit.CharacterControls;
using TafraKit.Healthies;
using TafraKit.RPG;
using UnityEngine;

namespace TafraKit
{
    [SearchMenuItem("Equipment/Weapon Activation Toggler")]
    public class WeaponActivationTogglerTestModule : ActionOnInputTestingModule
    {
        [SerializeField] private bool startActive = true;

        private bool isActive = true;
        private CharacterCombat characterCombat;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            characterCombat = characterTesting.GetComponent<CharacterCombat>();

            if(!startActive)
            {
                isActive = false;
                characterCombat.AddWeaponAttackDisabler("testModule");
            }
        }

        protected override void OnInputReceived()
        {
            if(isActive)
            {
                isActive = false;
                characterCombat.AddWeaponAttackDisabler("testModule");
            }
            else
            {
                isActive = true;
                characterCombat.RemoveWeaponAttackDisabler("testModule");
            }
        }
    }
}