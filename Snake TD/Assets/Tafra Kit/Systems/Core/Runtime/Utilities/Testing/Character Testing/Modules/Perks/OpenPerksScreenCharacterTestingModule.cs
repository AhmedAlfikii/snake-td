using System.Collections.Generic;
using TafraKit.Roguelike;
using UnityEngine;

namespace TafraKit.Internal
{
    [SearchMenuItem("Perks/Open Perks Screen")]
    public class OpenPerksScreenCharacterTestingModule : ActionOnInputTestingModule
    {
        [SerializeField] private PerksGroup perksGroup;
        [SerializeField] private int perksCount = 3;

        protected override void OnInputReceived()
        {
            PerksHandler.DisplayOffer(perksGroup, perksCount);
        }
    }
}