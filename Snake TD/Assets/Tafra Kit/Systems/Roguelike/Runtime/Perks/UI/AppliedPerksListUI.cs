using System;
using System.Collections.Generic;
using TafraKit.Internal.Roguelike;
using TafraKit.UI;
using UnityEngine;
using UnityEngine.UI;

namespace TafraKit.Roguelike
{
    public class AppliedPerksListUI : MonoBehaviour
    {
        [SerializeField] private bool updateOnEnable = true;
        [SerializeField] private bool autoUpdateOnChange = true;
        [SerializeField] private bool resetScrollOnEnable = true;
        [SerializeField] private DynamicPool<PerkCard> cardsPool = new DynamicPool<PerkCard>();
        [SerializeField] private ScrollRect scrollRect;

        private List<PerkCard> activeCards = new List<PerkCard>();
        private Dictionary<Perk, PerkCard> cardByPerk = new System.Collections.Generic.Dictionary<Perk, PerkCard>();

        private void Awake()
        {
            cardsPool.Initialize();
        }
        private void OnEnable()
        {
            if (updateOnEnable)
                FullyUpdateList();

            if(resetScrollOnEnable && scrollRect != null)
                scrollRect.normalizedPosition = new Vector2(0, 1);

            if(autoUpdateOnChange)
            {
                PerksHandler.OnPerkAppliedFirstTimeInSession.AddListener(OnNewPerkAdded);
                PerksHandler.OnPerkReapplied.AddListener(OnPerkUpdated);
            }
        }
        private void OnDisable()
        {
            if(autoUpdateOnChange)
            {
                PerksHandler.OnPerkAppliedFirstTimeInSession.RemoveListener(OnNewPerkAdded);
                PerksHandler.OnPerkReapplied.RemoveListener(OnPerkUpdated);
            }
        }

        private void OnNewPerkAdded(Perk perk)
        {
            AddCard(perk);
        }
        private void OnPerkUpdated(Perk perk)
        {
            if (cardByPerk.TryGetValue(perk, out PerkCard card))
                card.Populate(perk, false, () => { OnPerkCardClicked(card); });
        }

        public void FullyUpdateList()
        {
            List<Perk> appliedPerks = PerksHandler.AppliedPerks;
            
            int difference = appliedPerks.Count - activeCards.Count;

            if(difference > 0)
            {
                for (int i = 0; i < difference; i++)
                {
                    PerkCard card = cardsPool.RequestUnit(activateUnit: false);
                    activeCards.Add(card);
                }
            }
            else if (difference < 0)
            {
                int toBeRemoved = difference * -1;

                for(int i = 0; i < toBeRemoved; i++)
                {
                    int lastCard = activeCards.Count - 1;
                    PerkCard card = activeCards[lastCard];

                    cardByPerk.Remove(card.DisplayedPerk);

                    activeCards.RemoveAt(lastCard);

                    cardsPool.ReleaseUnit(card);
                }
            }

            for (int i = 0; i < appliedPerks.Count; i++)
            {
                var perk = appliedPerks[i];
                var card = activeCards[i];

                card.Populate(perk, false, ()=> { OnPerkCardClicked(card); });
                
                cardByPerk.TryAdd(perk, card);

                if (!card.gameObject.activeSelf)
                    card.gameObject.SetActive(true);
            }
        }
        private void AddCard(Perk perk)
        {
            PerkCard card = cardsPool.RequestUnit(activateUnit: false);

            card.Populate(perk, false, () => { OnPerkCardClicked(card); });

            activeCards.Add(card);

            card.gameObject.SetActive(true);

            cardByPerk.Add(perk, card);
        }

        private void OnPerkCardClicked(PerkCard card)
        {
            InfoBubbleHandler.Show(card.GetComponent<RectTransform>(), Side.Bottom, card.DisplayedPerk.AppliedDescription, card.DisplayedPerk.AppliedDisplayName);
        }
    }
}