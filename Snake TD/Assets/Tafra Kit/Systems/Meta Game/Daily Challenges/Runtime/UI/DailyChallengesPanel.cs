using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using System;
using TafraKit.UI;
using TafraKit.Tasks;
using System.Text;

namespace TafraKit.MetaGame
{
    public class DailyChallengesPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TafraCalender calender;
        [SerializeField] private List<DailyChallengesSlot> slots;
        [SerializeField] private TextMeshProUGUI playBtnText;

        private int selectedMonthChallengesCount;
        private int[] challengeStates = new int[31];
        private int selectedSlotIndex = -1;
        private DateTime selectedMonthFirstDay;
        private CancellationTokenSource requestPlayCts;
        private StringBuilder playTextSB = new StringBuilder();

        private void Awake()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].OnSelect.AddListener(OnSlotSelected);
            }
        }
        private void Start()
        {
            DateTime today = TafraDateTime.Today;

            DisplayMonth(today.Year, today.Month);

            SelectSlot(today.Day, true);
        }

        private void OnDisable()
        {
            if (requestPlayCts != null)
            {
                requestPlayCts.Cancel();
                requestPlayCts.Dispose();
            }
        }

        private void OnSlotSelected(int slotDay)
        {
            SelectSlot(slotDay, false);
        }

        public void DisplayMonth(int year, int month)
        {
            calender.DisplayMonth(year, month);

            selectedMonthFirstDay = new DateTime(year, month, 1);

            DailyChallenges.GetCurrentMonthChallengesState(challengeStates, out selectedMonthChallengesCount);

            for (int i = 0; i < slots.Count; i++)
            {
                DailyChallengesSlot slot = slots[i];

                if (i < selectedMonthChallengesCount)
                {
                    slot.SetDate(year, month, i + 1);
                    slot.SetState((DailyChallenges.ChallengeState)challengeStates[i]);

                    slot.gameObject.SetActive(true);
                }
                else
                    slot.gameObject.SetActive(false);
            }
        }
        public void SelectSlot(int slotDay, bool instant)
        {
            if (selectedSlotIndex > -1)
            {
                slots[selectedSlotIndex].SetSelectedState(false, instant);
            }

            selectedSlotIndex = slotDay -1;
            slots[selectedSlotIndex].SetSelectedState(true, instant);

            playTextSB.Clear();

            DailyChallenges.ChallengeState state = (DailyChallenges.ChallengeState)challengeStates[selectedSlotIndex];

            if (state == DailyChallenges.ChallengeState.PlayableAtCost)
            {
                DailyChallengePlayCost playCost = DailyChallenges.GetActivePlayCost();
                if (playCost != null)
                {
                    playTextSB.Append(playCost.GetCostString());
                    playTextSB.Append(" ");
                }
            }
            playTextSB.Append("Play ");
            playTextSB.Append(selectedMonthFirstDay.ToString("MMM"));
            playTextSB.Append(" ");
            playTextSB.Append(slotDay);

            playBtnText.text = playTextSB.ToString();
        }

        public async void PlaySelectedChallenge()
        { 
            if (selectedSlotIndex >= 0 && selectedSlotIndex < selectedMonthChallengesCount) 
            {
                if (requestPlayCts != null)
                { 
                    requestPlayCts?.Cancel();
                    requestPlayCts?.Dispose();
                }

                requestPlayCts = new CancellationTokenSource();

                Task<BoolOperationResult> requestTask =  DailyChallenges.RequestPlayChallenge(selectedMonthFirstDay.Year, selectedMonthFirstDay.Month, selectedSlotIndex + 1, requestPlayCts.Token);
                await requestTask;

                switch (requestTask.Result)
                {
                    case BoolOperationResult.Success:
                        break;
                    case BoolOperationResult.Fail:
                        Debug.Log("Failed to start the challenge");
                        break;
                    case BoolOperationResult.Canceled:
                        Debug.Log("Starting the challenge was canceled");
                        break;
                }
            }
        }
    }
}