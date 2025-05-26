using System;
using System.Collections.Generic;
using TafraKit.Internal.Roguelike;
using UnityEngine;

namespace TafraKit.Roguelike
{
    [CreateAssetMenu(menuName = "Tafra Kit/Roguelike/Perk Groups/Perks Sub-Group", fileName = "Perks Sub-Group", order = 99999)]
    public class PerksSubGroup : PerkBase
    {
        [SerializeField] private List<Perk> perks = new List<Perk>();

        [Header("Limitation")]
        [Tooltip("Enable this to limit the number of perks that can be applied for the first time in this sub-group.")]
        [SerializeField] private bool limitAppliedPerksCount;
        [Tooltip("Limit the number of the applied (for the first time) perks.")]
        [SerializeField] private int maxAppliedPerks = 5;

        [NonSerialized] private List<Perk> availablePerks = new List<Perk>();
        [NonSerialized] private List<Perk> appliedPerks = new List<Perk>();
        [NonSerialized] private HashSet<Perk> tempExcludedPerksHashSet = new HashSet<Perk>();

        public List<Perk> Perks => perks;
        public List<Perk> AvailablePerks => availablePerks;
        public List<Perk> AppliedPerks => appliedPerks;
        public int AppliedPerksCount => appliedPerks.Count;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            for (int i = 0; i < perks.Count; i++)
            {
                var perk = perks[i];

                perk.Initialize();

                if(perk.IsApplied)
                {
                    appliedPerks.Add(perk);
                }
            }
        }

        public void RefreshAvailabilePerks(List<Perk> excludedPerks = null)
        {
            availablePerks.Clear();
            tempExcludedPerksHashSet.Clear();

            if(excludedPerks != null)
            {
                for(int i = 0; i < excludedPerks.Count; i++)
                {
                    tempExcludedPerksHashSet.Add(excludedPerks[i]);
                }
            }

            if(limitAppliedPerksCount && appliedPerks.Count >= maxAppliedPerks)
            {
                //If we reached the maximum number of applied perks, then only the applied perks should be available. Players shouldn't be able to apply new perks.
                for(int i = 0; i < appliedPerks.Count; i++)
                {
                    Perk perk = appliedPerks[i];

                    if(tempExcludedPerksHashSet.Contains(perk))
                        continue;

                    if(perk.CanBeOffered())
                        availablePerks.Add(perk);
                }
            }
            else
            {
                for(int i = 0; i < perks.Count; i++)
                {
                    Perk perk = perks[i];

                    if(tempExcludedPerksHashSet.Contains(perk))
                        continue;

                    if(perk.CanBeOffered())
                        availablePerks.Add(perk);
                }
            }
        }
        public void PerkApplied(Perk perk)
        { 
            if (perk.AppliesCount == 1)
                appliedPerks.Add(perk);
        }
    }
}