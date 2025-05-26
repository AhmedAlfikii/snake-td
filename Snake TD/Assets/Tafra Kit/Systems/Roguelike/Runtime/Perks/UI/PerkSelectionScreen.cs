using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.Roguelike;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ZUI;

namespace TafraKit.Internal.Roguelike
{
    public class PerkSelectionScreen : MonoBehaviour
    {
        [Serializable]
        public class CardReferences
        {
            [Tooltip("The actual card that will be used to display the perk.")]
            public PerkCard card;
            [Tooltip("Will be disable if this card isn't needed.")]
            public GameObject holder;
            [Tooltip("Will be used to show and hide the card, should be on the card itself, not the holder.")]
            public UIElement uie;
        }

        [SerializeField] private UIElementsGroup myUIEG;
        [SerializeField] private List<CardReferences> cards;
        [Tooltip("The perks group that this screen should display the offers of. If empty, it will display the offers from any perks group.")]
        [SerializeField] private PerksGroup perksGroup;
        [SerializeField] private float blockTouchOnShowDuration = 2f;

        [Header("SFX")]
        [SerializeField] private SFXClips showAudio;
        [SerializeField] private SFXClips hideAudio;
        [SerializeField] private SFXClips changeCardsAudio;

        [Header("Music")]
        [SerializeField] private AudioClip[] tracks;
        [SerializeField, Range(0f, 1f)] private float trackVolume = 0.5f;

        private bool isVisible;
        /// <summary>
        /// Is true as long as the UIEG isn't completley hidden.
        /// </summary>
        private bool isUIEGVisible;
        private GraphicRaycaster graphicRaycaster;
        private PerksGroup displayedOfferGroup;
        private int nextTrackIndex;
        private UnityEvent<List<Perk>> onAboutToShow = new UnityEvent<List<Perk>>();
        private UnityEvent<List<Perk>> onShow = new UnityEvent<List<Perk>>();
        private UnityEvent onHide = new UnityEvent();

        public UnityEvent<List<Perk>> OnAboutToShow => onAboutToShow;
        public UnityEvent<List<Perk>> OnShow => onShow;
        public UnityEvent OnHide => onHide;

        private void Awake()
        {
            graphicRaycaster = GetComponent<GraphicRaycaster>();
        }
        private void OnEnable()
        {
            myUIEG.OnHideComplete.AddListener(OnHideComplete);
            PerksHandler.OnOfferDisplayOrder.AddListener(OnOfferDisplayOrder);
        }
        private void OnDisable()
        {
            myUIEG.OnHideComplete.RemoveListener(OnHideComplete);
            PerksHandler.OnOfferDisplayOrder.RemoveListener(OnOfferDisplayOrder);
        }

        private void OnCardClicked(int perkIndex)
        {
            ConcludeDisplayedOffer(perkIndex);
        }
        private void OnHideComplete()
        {
            isUIEGVisible = false;
        }
        private void OnOfferDisplayOrder(List<Perk> perks, PerksGroup perksGroup)
        {
            if(this.perksGroup != null && perksGroup != this.perksGroup)
                return;

            displayedOfferGroup = perksGroup;

            if(!isVisible)
                Show(perks);
            else
                ChangeVisibleCards(perks);
        }
        private void Show(List<Perk> perks)
        {
            if(perks == null || perks.Count == 0)
            {
                TafraDebugger.Log("Perks Selection Screen", "Perks list received is empty, will not display the screen.", TafraDebugger.LogType.Info);
                return;
            }

            StartCoroutine(Showing(perks));
        }
        private IEnumerator Showing(List<Perk> perks)
        {
            TimeScaler.SetTimeScale("perksScren", 0);

            //Wait until the screen hides before displaying the new offer.
            while(isUIEGVisible)
                yield return null;

            isVisible = true;
            isUIEGVisible = true;

            PopulateCards(perks);

            onAboutToShow?.Invoke(perks);

            myUIEG.ChangeVisibility(true);

            SFXPlayer.Play(showAudio);

            if(tracks.Length > 0)
            {
                MusicPlayer.PlayOverlay(tracks[nextTrackIndex], trackVolume, 0.5f, 1);
                nextTrackIndex = (nextTrackIndex + 1) % tracks.Length;
            }

            if(graphicRaycaster)
                graphicRaycaster.enabled = true;

            TouchBlocker.Block("perkSelectionScreenIntro");

            CompactCouroutines.StartCompactCoroutine(this, blockTouchOnShowDuration, 0, true, null, () =>
            {
                TouchBlocker.Unblock("perkSelectionScreenIntro");
            });

            onShow?.Invoke(perks);
        }
        private void ChangeVisibleCards(List<Perk> perks)
        {
            StartCoroutine(ChangingVisibleCards(perks));
        }
        private IEnumerator ChangingVisibleCards(List<Perk> perks)
        {
            TouchBlocker.Block("perkSelectionScreen");

            float totalHidingDuration = 0;
            for(int i = 0; i < cards.Count; i++)
            {
                var card = cards[i];
                var cardUIE = card.uie;

                cardUIE.ChangeVisibility(false);

                if(cardUIE.HidingDuration > totalHidingDuration)
                    totalHidingDuration = cardUIE.HidingDuration;
            }

            yield return Yielders.GetWaitForSecondsRealtime(totalHidingDuration);

            PopulateCards(perks);

            SFXPlayer.Play(changeCardsAudio);

            float totalShowingDuration = 0;
            for(int i = 0; i < cards.Count; i++)
            {
                var card = cards[i];
                var cardUIE = card.uie;

                cardUIE.ChangeVisibility(true);

                if(cardUIE.ShowingDuration > totalShowingDuration)
                    totalShowingDuration = cardUIE.ShowingDuration;
            }

            TouchBlocker.Unblock("perkSelectionScreen");
        }
        private void PopulateCards(List<Perk> perks)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                var card = cards[i];

                if(i < perks.Count)
                {
                    Perk perk = perks[i];
                    int perkIndex = i;

                    card.card.Populate(perks[i], true, () => { OnCardClicked(perkIndex); });

                    card.holder.SetActive(true);
                }
                else
                { 
                    card.holder.SetActive(false);
                }
            }
        }
        private void Hide()
        {
            if(graphicRaycaster)
                graphicRaycaster.enabled = false;

            myUIEG.ChangeVisibility(false);

            SFXPlayer.Play(hideAudio);
            MusicPlayer.StopOverlay(1f);

            TimeScaler.RemoveTimeScaleControl("perksScren");

            isVisible = false;

            displayedOfferGroup = null;

            onHide?.Invoke();
        }

        public void ConcludeDisplayedOffer(int selectedPerkIndex)
        {
            Hide();

            PerksHandler.ConcludeOffer(selectedPerkIndex);
        }
    }
}