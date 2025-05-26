using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using TafraKit.UI;
using ZUI;

namespace TafraKit.MetaGame
{
    public class DailyChallengesSlot : TafraCalnderSlot
    {
        [SerializeField] private ZUIElementBase selectedUIE;
        [SerializeField] private GameObject activeGO;
        [SerializeField] private GameObject todayActiveGO;
        [SerializeField] private GameObject notAvailableGO;
        [SerializeField] private ZUIElementBase completedZUIE;

        [SerializeField] private TextMeshProUGUI numberTXT;
        [SerializeField] private Color activeNumberColor;
        [SerializeField] private Color todayNumberColor;
        [SerializeField] private Color completedNumberColor;
        [SerializeField] private Color notAvailableNumberColor;

        public UnityEvent<int> OnSelect = new UnityEvent<int>();

        private ZButton myButton;
        private DailyChallenges.ChallengeState myState;

        private void Awake()
        {
            myButton = GetComponent<ZButton>();

            myButton.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            OnSelect?.Invoke(day);
        }

        public override void SetDate(int year, int month, int day)
        {
            base.SetDate(year, month, day);

            if(selectedUIE.Visible)
                selectedUIE.ChangeVisibilityImmediate(false);
        }
        public void SetState(DailyChallenges.ChallengeState state)
        {
            myState = state;

            bool isActive = state != DailyChallenges.ChallengeState.NotAvailable;
            bool isCompleted = state == DailyChallenges.ChallengeState.Completed;
            bool isTodayActive = state == DailyChallenges.ChallengeState.Playable;
            
            if(activeGO.activeSelf != isActive)
                activeGO.SetActive(isActive);

            if (notAvailableGO.activeSelf != !isActive)
                notAvailableGO.SetActive(!isActive);

            if(completedZUIE.Visible != isCompleted)
                completedZUIE.ChangeVisibility(isCompleted);

            if(todayActiveGO.activeSelf != isTodayActive)
                todayActiveGO.SetActive(isTodayActive);

            myButton.interactable = isActive && !isCompleted;

            switch(state)
            {
                case DailyChallenges.ChallengeState.NotAvailable:
                    numberTXT.color = notAvailableNumberColor;
                    break;
                case DailyChallenges.ChallengeState.Playable:
                    numberTXT.color = todayNumberColor;
                    break;
                case DailyChallenges.ChallengeState.PlayableAtCost:
                    numberTXT.color = activeNumberColor;
                    break;
                case DailyChallenges.ChallengeState.Completed:
                    numberTXT.color = completedNumberColor;
                    break;
                default:
                    numberTXT.color = activeNumberColor;
                    break;
            }
        }
        public void SetSelectedState(bool isSelected, bool instant)
        {
            if(!instant)
                selectedUIE.ChangeVisibility(isSelected);
            else
                selectedUIE.ChangeVisibilityImmediate(isSelected);

            if (isSelected)
                myButton.interactable =  false;
            else if (myState == DailyChallenges.ChallengeState.Playable || myState == DailyChallenges.ChallengeState.PlayableAtCost)
                myButton.interactable =  true;
        }
    }
}