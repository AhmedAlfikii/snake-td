using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using TafraKit.UI;
using TafraKit.Tasks;
using TafraKit.MotionFactory;
using System.Text;
using TafraKit.SceneManagement;
using ZUI;

namespace TafraKit.MetaGame
{
    public class DailyChallengesCalendarPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TafraCalendar calendar;
        [SerializeField] private DailyChallengesMonthPanel monthPanel;
        [SerializeField] private List<DailyChallengesSlot> slots;
        [SerializeField] private UIElement playBtnUIE;
        [SerializeField] private TextMeshProUGUI playBtnText;
        [SerializeField] private SimpleMotionController playBtnChangeMotion;

        [Header("Properties")]
        [SerializeField] private string normalPlayBtnPrefix;

        private int selectedMonthChallengesCount;
        private int[] challengeStates = new int[31];
        private int selectedSlotIndex = -1;
        private DateTime selectedMonthFirstDay;
        private CancellationTokenSource requestPlayCts;
        private StringBuilder playTextSB = new StringBuilder();
        private ControlReceiver playButtonDisablers;
        private int selectedMonthCompletedChallengesCount;

        private void Awake()
        {
            playButtonDisablers = new ControlReceiver(OnPlayButtonDisablerAdded, null, OnAllPlayButtonDisablersCleared);

            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].OnSelect.AddListener(OnSlotSelected);
            }
        }
        private void Start()
        {
            DateTime today = TafraDateTime.Today;

            DisplayMonth(today.Year, today.Month);
        }

        private void OnDisable()
        {
            if (requestPlayCts != null)
            {
                requestPlayCts.Cancel();
                requestPlayCts.Dispose();
            }
        }

        #region Callbacks
        private void OnAllPlayButtonDisablersCleared()
        {
            playBtnUIE.ChangeVisibility(true);
        }

        private void OnPlayButtonDisablerAdded()
        {
            playBtnChangeMotion.Stop();
            playBtnUIE.ChangeVisibility(false);
        }
        #endregion

        #region Private Functions
        private void OnSlotSelected(int slotDay)
        {
            SelectSlot(slotDay, false);
        }
        #endregion

        #region Public Functions
        public void DisplayMonth(int year, int month)
        {
            calendar.DisplayMonth(year, month);

            selectedMonthFirstDay = new DateTime(year, month, 1);

            DailyChallenges.GetMonthChallengesState(year, month, challengeStates, out selectedMonthChallengesCount);

            int latestPlayableSlotIndex = -1;
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

                if (i < selectedMonthChallengesCount && (challengeStates[i] == 0 || challengeStates[i] == 1))
                    latestPlayableSlotIndex = i;
            }

            selectedMonthCompletedChallengesCount = DailyChallenges.GetMonthCompletedChallengesCount(year, month);

            if (monthPanel)
                monthPanel.DisplayMonth(year, month);

            SelectSlot(latestPlayableSlotIndex + 1, true);
        }
        public void SelectSlot(int slotDay, bool instant)
        {
            if (selectedSlotIndex > -1)
            {
                slots[selectedSlotIndex].SetSelectedState(false, instant);
            }

            selectedSlotIndex = slotDay - 1;

            if (slotDay > 0)
            {
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
                else
                    playTextSB.Append(normalPlayBtnPrefix);

                playTextSB.Append("Play ");
                playTextSB.Append(selectedMonthFirstDay.ToString("MMM"));
                playTextSB.Append(" ");
                playTextSB.Append(slotDay);

                playBtnText.text = playTextSB.ToString();

                playButtonDisablers.RemoveController("NoAvailableChallenges");
                playBtnChangeMotion.Play();
            }
            else
            {
                playButtonDisablers.AddController("NoAvailableChallenges");
            }
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

                Task<BoolOperationResult> requestTask = DailyChallenges.RequestPlayChallenge(selectedMonthFirstDay.Year, selectedMonthFirstDay.Month, selectedSlotIndex + 1, requestPlayCts.Token);
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
        public void AddPlayButtonDisabler(string disabler)
        {
            playButtonDisablers.AddController(disabler);
        }
        public void RemovePlayButtonDisabler(string disabler)
        {
            playButtonDisablers.RemoveController(disabler);
        }

        public List<DailyChallengesSlot> GetSlots()
        {
            return slots;
        }
        public int GetSelectedSlotIndex()
        { 
            return selectedSlotIndex;
        }
        public int GetSelectedMonthChallengesCount()
        {
            return selectedMonthChallengesCount;
        }
        public int GetSelectedMonthCompletedChallengesCount()
        {
            return selectedMonthCompletedChallengesCount;
        }
        #endregion
    }
}