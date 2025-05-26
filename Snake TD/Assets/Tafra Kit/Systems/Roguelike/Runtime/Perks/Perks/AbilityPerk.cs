using System;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using UnityEngine;
using UnityEngine.UI;

namespace TafraKit.Roguelike
{
    [CreateAssetMenu(menuName = "Tafra Kit/Roguelike/Perks/Ability Perk", fileName = "Ability Perk", order = 0)]
    public class AbilityPerk : Perk
    {
        [Header("Ability")]
        [SerializeField] private Ability ability;

        [NonSerialized] private bool waitingForPlayerToInitialize;

        public override string OfferDisplayName => ability.GetDisplayName(appliesCount + 1);
        public override string OfferDescription => ability.GetDescription(appliesCount + 1);
        public override string AppliedDisplayName => ability.GetDisplayName(appliesCount);
        public override string AppliedDescription => ability.GetDescription(appliesCount);

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if(waitingForPlayerToInitialize)
            {
                CharacterAbilities playerAbilities = SceneReferences.PlayerAbilities;

                if(playerAbilities != null)
                {
                    playerAbilities.OnInitialized.RemoveListener(OnPlayerReadyToReceiveAbilities);
                    waitingForPlayerToInitialize = false;
                }
            }
        }

        protected override void OnApplied()
        {
            AddAbilityToPlayer();
        }
        protected override void OnSceneLoad()
        {
            AddAbilityToPlayer();
        }
        private void AddAbilityToPlayer()
        {
            CharacterAbilities playerAbilities = SceneReferences.PlayerAbilities;

            if(playerAbilities == null)
            {
                TafraDebugger.Log("Ability Perk", "There's no Character Ability component found on the player. Will not work.", TafraDebugger.LogType.Info, this);
                return;
            }

            if(playerAbilities.IsInitialized)
            {
                if(playerAbilities.TryGetEquippedAbility(ability, out Ability equippedInstance))
                {
                    equippedInstance.Level = appliesCount;
                }
                else
                {
                    Ability instance = playerAbilities.Equip(ability, null, false);
                    instance.Level = appliesCount;
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
        public override void LoadIcons()
        {
            if (iconRequesters <= 0)
                ability.LoadIcons();

            base.LoadIcons();
        }
        public override void ReleaseIcons()
        {
            base.ReleaseIcons();

            if (iconRequesters <= 0)
                ability.ReleaseIcons();
        }

        public override Sprite GetLoadedOfferIcon()
        {
            if(!iconsLoaded)
                TafraDebugger.Log("Ability Perk", "Icons are not loaded, load them first by calling LoadIcons(). And make sure to release them when you no longer need them by calling ReleaseIcons()", TafraDebugger.LogType.Error);

            return ability.GetLoadedIcon(appliesCount + 1);
        }
        public override Sprite GetLoadedAppliedIcon()
        {
            if(!iconsLoaded)
                TafraDebugger.Log("Ability Perk", "Icons are not loaded, load them first by calling LoadIcons(). And make sure to release them when you no longer need them by calling ReleaseIcons()", TafraDebugger.LogType.Error);

            return ability.GetLoadedIcon(appliesCount);
        }
    }
}