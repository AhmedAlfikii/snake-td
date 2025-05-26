using System.Collections.Generic;
using TafraKit.Roguelike;
using UnityEngine;

namespace TafraKit.Internal.Roguelike
{
    public class PerkCardWithStars : PerkCard
    {
        [Header("Stars")]
        [SerializeField] private List<GameObject> dimmedStarHolders;
        [SerializeField] private List<CanvasGroup> litStarHolders;
        [SerializeField] private List<CanvasGroup> comingStarHolders;

        protected override void OnPopulate(Perk perk, bool displayAsOffer)
        {
            base.OnPopulate(perk, displayAsOffer);

            bool infiniteStars = perk.UnlimitedApplies;
            int totalStars = perk.MaxAppliesCount;
            int activeStars = perk.AppliesCount;

            for(int i = 0; i < dimmedStarHolders.Count; i++)
            {
                dimmedStarHolders[i].SetActive(!infiniteStars && i < totalStars);
            }
            for(int i = 0; i < litStarHolders.Count; i++)
            {
                var litStarCG = litStarHolders[i];
                litStarCG.gameObject.SetActive(!infiniteStars && i < totalStars);

                if(i < activeStars)
                    litStarCG.alpha = 1f;
                else
                    litStarCG.alpha = 0f;
            }
            for(int i = 0; i < comingStarHolders.Count; i++)
            {
                var comingStarCG = comingStarHolders[i];
                comingStarCG.gameObject.SetActive(!infiniteStars && i < totalStars);

                if(displayAsOffer && i == activeStars)
                {
                    comingStarCG.alpha = 1;
                }
                else
                {
                    comingStarCG.alpha = 0;
                }
            }
        }
    }
}