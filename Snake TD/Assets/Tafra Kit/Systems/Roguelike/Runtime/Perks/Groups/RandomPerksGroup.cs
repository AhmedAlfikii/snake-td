using System;
using System.Collections.Generic;
using TafraKit.Roguelike;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TafraKit.Internal.Roguelike
{
    [CreateAssetMenu(menuName = "Tafra Kit/Roguelike/Perk Groups/Random Perks Group", fileName = "Random Perks Group", order = 0)]
    public class RandomPerksGroup : PerksGroup
    {
        [SerializeField] private List<PerkBase> perks;

        [NonSerialized] private List<Perk> allPerks = new List<Perk>();
        [NonSerialized] private List<Perk> availablePerks = new List<Perk>();
        [NonSerialized] private List<Perk> availableSubGroupsPerks = new List<Perk>();
        [NonSerialized] private List<Perk> tempAvailablePerks = new List<Perk>();
        [NonSerialized] private Dictionary<Perk, PerksSubGroup> subGroupByPerk = new Dictionary<Perk, PerksSubGroup>();
        [NonSerialized] private HashSet<Perk> tempExcludedPerksHashSet = new HashSet<Perk>();

        protected override void OnInitialize()
        {
            for (int i = 0; i < perks.Count; i++)
            {
                var perkBase = perks[i];

                if(perkBase is Perk perk)
                {
                    perksById.Add(perk.ID, perk);
                    allPerks.Add(perk);
                }
                else if(perkBase is PerksSubGroup subGroup)
                {
                    List<Perk> subGroupPerks = subGroup.Perks;

                    for(int j = 0; j < subGroupPerks.Count; j++)
                    {
                        var subPerk = subGroupPerks[j];

                        perksById.Add(subPerk.ID, subPerk);

                        subGroupByPerk.Add(subPerk, subGroup);
                        allPerks.Add(subPerk);
                    }
                }
            }
        }

        protected void RefreshAvailabilePerks(List<Perk> excludedPerks = null)
        {
            availablePerks.Clear();
            availableSubGroupsPerks.Clear();
            tempExcludedPerksHashSet.Clear();

            if(excludedPerks != null)
            {
                for (int i = 0; i < excludedPerks.Count; i++)
                {
                    tempExcludedPerksHashSet.Add(excludedPerks[i]);
                }
            }

            for (int i = 0; i < perks.Count; i++)
            {
                var perkBase = perks[i];

                if(perkBase is Perk perk)
                {
                    if(tempExcludedPerksHashSet.Contains(perk))
                        continue;

                    if(perk.CanBeOffered())
                        availablePerks.Add(perk);
                }
                else if(perkBase is PerksSubGroup subGroup)
                {
                    subGroup.RefreshAvailabilePerks(excludedPerks);
                    availableSubGroupsPerks.AddRange(subGroup.AvailablePerks);
                }
            }
        }
        protected override bool CreateOffer(int count, List<Perk> emptyListToFill, List<Perk> excludedPerks = null, bool mustHaveAllPerks = false)
        {
            activePerksOffer.Clear();

            RefreshAvailabilePerks(excludedPerks);

            tempAvailablePerks.Clear();
            tempAvailablePerks.AddRange(availablePerks);
            tempAvailablePerks.AddRange(availableSubGroupsPerks);

            int remainingPerks = count;

            while(remainingPerks > 0 && tempAvailablePerks.Count > 0)
            {
                int randomPerkIndex = Random.Range(0, tempAvailablePerks.Count);

                Perk perk = tempAvailablePerks[randomPerkIndex];
                    
                activePerksOffer.Add(perk);

                tempAvailablePerks.RemoveAt(randomPerkIndex);

                remainingPerks--;
            }

            if(activePerksOffer.Count == 0)
                return false;
            else if(mustHaveAllPerks && activePerksOffer.Count < count)
            {
                activePerksOffer.Clear();
                return false;
            }
            else
            {
                emptyListToFill.AddRange(activePerksOffer);
                return true;
            }
        }
        protected override void OnPerkApply(Perk perk)
        {
            if(perk == null)
                return;

            if(subGroupByPerk.TryGetValue(perk, out var subGroup))
                subGroup.PerkApplied(perk);
        }

        public override bool ContainsPerk(Perk perk)
        {
            return allPerks.Contains(perk);
        }
    }
}