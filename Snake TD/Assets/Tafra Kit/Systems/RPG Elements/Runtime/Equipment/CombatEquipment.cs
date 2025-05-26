using System;
using System.Collections.Generic;
using TafraKit.CharacterControls;
using TafraKit.Roguelike;
using UnityEngine;

namespace TafraKit.RPG
{
    [CreateAssetMenu(menuName = "Tafra Kit/RPG/Combat Equipment", fileName = "Combat Equipment")]
    public class CombatEquipment : Equipment
    {
        [Serializable]
        public class RarityCombatProperties
        {
            [Tooltip("Serves no purpose but to nicefy the array element names.")]
            [SerializeField] private string rarityName;
            [SerializeField] private List<Ability> abilities;
            [SerializeField] private List<Perk> perks;

            public List<Ability> Abilities => abilities;
            public List<Perk> Perks => perks;
        }

        [Header("Combat Properties")]
        [SerializeField] private List<RarityCombatProperties> combatPropertiesPerRarity;

        /// <summary>
        /// Fills a list with the abilities assigned to this equipment based on its current rarity.
        /// </summary>
        /// <param name="listToFill"></param>
        /// <param name="includePreviousRarities"></param>
        public void GetAbilities(List<Ability> listToFill, bool includePreviousRarities = true)
        {
            listToFill.Clear();

            if(includePreviousRarities)
            {
                int availableRaritiesCount = Mathf.Min(combatPropertiesPerRarity.Count, rarity + 1);

                for(int i = 0; i < availableRaritiesCount; i++)
                {
                    var combatProperties = combatPropertiesPerRarity[i];

                    listToFill.AddRange(combatProperties.Abilities);
                }
            }
            else if (rarity < combatPropertiesPerRarity.Count)
            {
                listToFill.AddRange(combatPropertiesPerRarity[rarity].Abilities);
            }
        }
        /// <summary>
        /// Fills a list with the perks assigned to this equipment based on its current rarity.
        /// </summary>
        /// <param name="listToFill"></param>
        /// <param name="includePreviousRarities"></param>
        public void GetPerks(List<Perk> listToFill, bool includePreviousRarities = true)
        {
            listToFill.Clear();

            if(includePreviousRarities)
            {
                int availableRaritiesCount = Mathf.Min(combatPropertiesPerRarity.Count, rarity + 1);

                for(int i = 0; i < availableRaritiesCount; i++)
                {
                    var combatProperties = combatPropertiesPerRarity[i];

                    listToFill.AddRange(combatProperties.Perks);
                }
            }
            else if (rarity < combatPropertiesPerRarity.Count)
            {
                listToFill.AddRange(combatPropertiesPerRarity[rarity].Perks);
            }
        }
        
        /// <summary>
        /// Fills a list with the all perks assigned to this equipment.
        /// </summary>
        /// <param name="listToFill"></param>
        /// <param name="includePreviousRarities"></param>
        public void GetAllPerks(List<List<Perk>> listToFill)
        {
            listToFill.Clear();

            for(int i = 0; i < combatPropertiesPerRarity.Count; i++)
            {
                var combatProperties = combatPropertiesPerRarity[i];

                listToFill.Add(combatProperties.Perks);
            }
        }
        
        /// <summary>
        /// Fills a list with the all perks assigned to this equipment.
        /// </summary>
        /// <param name="listToFill"></param>
        /// <param name="includePreviousRarities"></param>
        public void GetAllAbilities(List<List<Ability>> listToFill)
        {
            listToFill.Clear();

            for(int i = 0; i < combatPropertiesPerRarity.Count; i++)
            {
                var combatProperties = combatPropertiesPerRarity[i];

                listToFill.Add(combatProperties.Abilities);
            }
        }
    }
}
