using System.Collections.Generic;
using TafraKit.Roguelike;
using UnityEngine;

namespace TafraKit.Internal
{
    [SearchMenuItem("Perks/Apply Perks")]
    public class ApplyPerksCharacterTestingModule : ActionOnInputTestingModule
    {
        [SerializeField] private List<Perk> perks;

        private List<Perk> tempPerks = new List<Perk>();

        protected override void OnInputReceived()
        {
            if(perks.Count == 0)
                return;

            tempPerks.AddRange(perks);

            List<PerksGroup> allPerkGroups = PerksHandler.PerkGroups;
            for (int i = 0; i < allPerkGroups.Count; i++)
            {
                var perkGroup = allPerkGroups[i];

                for (int j = 0; j < tempPerks.Count; j++)
                {
                    var perk = perks[j];

                    if(perkGroup.ContainsPerk(perk))
                    {
                        perkGroup.ApplyPerk(perk);

                        tempPerks.RemoveAt(j);
                        j--;
                    }

                }
            }

            for (int i = 0; i < tempPerks.Count; i++)
            {
                TafraDebugger.Log("Character Testing - Apply Perks", $"Couldn't find the perk {tempPerks[i]} in any of the existing perk groups. Will not apply it.", TafraDebugger.LogType.Error, tempPerks[i]);
            }

            tempPerks.Clear();
        }
    }
}