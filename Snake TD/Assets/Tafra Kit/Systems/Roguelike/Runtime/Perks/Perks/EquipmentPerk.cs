using System;
using TafraKit.Internal.Roguelike;
using TafraKit.RPG;
using UnityEngine;

namespace TafraKit.Roguelike
{
    [CreateAssetMenu(menuName = "Tafra Kit/Roguelike/Perks/Equipment Perk", fileName = "Equipment Perk", order = 2)]
    public class EquipmentPerk : IdentifiablePerk
    {
        public enum AppliesEffect
        { 
            DoNothing,
            IncreaseLevel,
            IncreaseRarity
        }

        [Header("Equipment")]
        [SerializeField] private Equipment equipment;
        [Tooltip("What should happen when the perk gets applied multiple times?")]
        [SerializeField] private AppliesEffect appliesEffect = AppliesEffect.IncreaseLevel;

        [NonSerialized] private Equipment equippedEquipment;
        [NonSerialized] private bool waitingForPlayerToInitialize;

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if(waitingForPlayerToInitialize)
            {
                CharacterEquipment playerEquipment = SceneReferences.PlayerEquipment;

                if(playerEquipment != null)
                {
                    playerEquipment.OnInitialized.RemoveListener(OnPlayerEquipmentInitialized);
                    waitingForPlayerToInitialize = false;
                }
            }
        }

        protected override void OnApplied()
        {
            Equip();
        }
        protected override void OnSceneLoad()
        {
            equippedEquipment = null;

            Equip();
        }

        private void Equip()
        {
            CharacterEquipment playerEquipment = SceneReferences.PlayerEquipment;

            if(playerEquipment == null)
            {
                TafraDebugger.Log("Equipment Perk", "There's no Character Equipment component found on the player. Will not work.", TafraDebugger.LogType.Info, this);
                return;
            }

            if(playerEquipment.IsInitialized)
            {
                if(equippedEquipment != null)
                {
                    if(appliesEffect == AppliesEffect.IncreaseLevel)
                        equippedEquipment.Level = appliesCount;
                    else if(appliesEffect == AppliesEffect.IncreaseRarity)
                        equippedEquipment.Rarity = appliesCount;
                }
                else
                {
                    Equipment equipmentInstance = equipment.InstancableSO.GetOrCreateInstance() as Equipment;

                    if (appliesEffect == AppliesEffect.IncreaseLevel)
                        equipmentInstance.Level = appliesCount;
                    else if (appliesEffect == AppliesEffect.IncreaseRarity)
                        equipmentInstance.Rarity = appliesCount;

                    equippedEquipment = playerEquipment.Equip(equipmentInstance, -1, false);
                }
            }
            else
            {
                waitingForPlayerToInitialize = true;
                playerEquipment.OnInitialized.AddListener(OnPlayerEquipmentInitialized);
            }
        }

        private void OnPlayerEquipmentInitialized()
        {
            CharacterEquipment playerEquipment = SceneReferences.PlayerEquipment;

            playerEquipment.OnInitialized.RemoveListener(OnPlayerEquipmentInitialized);

            waitingForPlayerToInitialize = false;

            Equip();
        }

        protected override void OnResetSavedData()
        {
            base.OnResetSavedData();

            equippedEquipment = null;

            if(waitingForPlayerToInitialize)
            {
                CharacterEquipment playerEquipment = SceneReferences.PlayerEquipment;

                if(playerEquipment != null)
                {
                    playerEquipment.OnInitialized.RemoveListener(OnPlayerEquipmentInitialized);
                    waitingForPlayerToInitialize = false;
                }
            }
        }
    }
}