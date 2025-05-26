using System;
using TafraKit.CharacterControls;
using TafraKit.Internal.CharacterControls;
using UnityEngine;

namespace TafraKit.RPG
{
    [SearchMenuItem("Ability")]
    public class NormalAbilityUpgradeModule : NormalUpgradeModule
    {
        [SerializeField] private Ability ability;
        [Tooltip("If true, no new instance of the ability will be equipped, instead, it will look for already equipped instances and level it up.")]
        [SerializeField] private bool levelUpExisting;
      
        [NonSerialized] private bool waitingForPlayerToInitialize;

        public override Sprite LoadedIcon => ability.GetLoadedIcon(1);
        public override string DisplayName => ability.GetDisplayName(1);
        public override string Description => ability.GetDescription(1);

        public override Sprite LoadIcon()
        {
            ability.LoadIcons();
            return ability.GetLoadedIcon(1);
        }
        public override void ReleaseIcon()
        {
            ability.ReleaseIcons();
        }

        protected override void OnApply()
        {
            AddAbilityToPlayer();
        }
        protected override void OnSceneLoaded()
        {
            AddAbilityToPlayer();
        }

        private void AddAbilityToPlayer()
        {
            CharacterAbilities playerAbilities = SceneReferences.PlayerAbilities;

            if(playerAbilities == null)
            {
                TafraDebugger.Log("Ability Upgrade", "There's no Character Ability component found on the player. Will not work.", TafraDebugger.LogType.Info, path);
                return;
            }

            if(playerAbilities.IsInitialized)
            {
                if(levelUpExisting)
                {
                    if(playerAbilities.TryGetEquippedAbility(ability, out Ability equippedInstance))
                    {
                        equippedInstance.Level++;
                    }
                    else
                        TafraDebugger.Log("Ability Upgrade", "Can't level up existing ability because there's this ability isn't equipped..", TafraDebugger.LogType.Info, path);
                }
                else
                {
                    Ability instance = playerAbilities.Equip(ability, null, false);
                    instance.Level = 1;
                    instance.Perform();
                }
            }
            else
            {
                waitingForPlayerToInitialize = true;
                playerAbilities.OnInitialized.AddListener(OnPlayerReadyToReceiveAbilities);
            }
        }
        private void OnPlayerReadyToReceiveAbilities()
        {
            CharacterAbilities playerAbilities = SceneReferences.PlayerAbilities;

            playerAbilities.OnInitialized.RemoveListener(OnPlayerReadyToReceiveAbilities);

            waitingForPlayerToInitialize = false;

            AddAbilityToPlayer();
        }
    }
}