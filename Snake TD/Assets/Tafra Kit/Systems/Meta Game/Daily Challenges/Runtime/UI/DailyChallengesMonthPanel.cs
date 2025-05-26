using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
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
    public class DailyChallengesMonthPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SectionedSlider monthProgressSlider;
        [SerializeField] private TextMeshProUGUI monthProgressTXT;
        [SerializeField] private GameObject[] closedChests;
        [SerializeField] private GameObject[] openedChests;
        [SerializeField] private TextMeshProUGUI[] chestsRequiredCount;

        [Header("Properties")]
        [SerializeField] private string monthProgressTotalDaysPrefix;

        [Header("Animation")]
        [SerializeField] private float monthProgressAnimationDuration = 0.75f;
        [SerializeField] private EasingType motionProgressEasing;

        /// <summary>
        /// Fires when a reward day is reached.
        /// Parameters: day index.
        /// </summary>
        public UnityEvent<int> OnRewardDayReached = new UnityEvent<int>();

        private int monthChallengesCount;
        private int monthCompletedChallengesCount;
        private DateTime selectedMonthFirstDay;
        private StringBuilder monthProgressSB = new StringBuilder();
        private List<SectionedSlider.Separator> progressSliderSeparators = new List<SectionedSlider.Separator>();

        #region Private Functions
        private IEnumerator SettingMonthProgress(int completedChallenges, int totalDays, int claimDayIndexReward = -1)
        {
            float startTime = Time.time;
            float endTime = startTime + monthProgressAnimationDuration;

            float sliderStartValue = monthProgressSlider.Value;
            float sliderEndValue = (completedChallenges / (float)totalDays) - 0.00f;  //The deducted 0.001f is to make sure that the slider doens't
                                                                                        //jump to the next secion if it's exactly at a section's end.

            while (Time.time < endTime)
            {
                float t = (Time.time - startTime) / monthProgressAnimationDuration;

                t = MotionEquations.GetEaseFloat(t, motionProgressEasing);

                monthProgressSlider.Value = Mathf.Lerp(sliderStartValue, sliderEndValue, t);

                yield return null;
            }
            monthProgressSlider.Value = sliderEndValue;

            if (claimDayIndexReward > -1)
            {
                closedChests[claimDayIndexReward].SetActive(false);
                openedChests[claimDayIndexReward].SetActive(true);

                yield return Yielders.GetWaitForSeconds(0.25f);

                Debug.LogError("You should give the player the reward here");
                //ConsumableRewarder.Instance.AddRewards(DailyChallenges.GetMonthDayReward(claimDayIndexReward));
                //ConsumableRewarder.Instance.ShowScreen();
            }
        }
        #endregion

        #region Public Functions
        public void DisplayMonth(int year, int month)
        {
            selectedMonthFirstDay = new DateTime(year, month, 1);

            monthChallengesCount = DateTime.DaysInMonth(year, month);
            monthCompletedChallengesCount = DailyChallenges.GetMonthCompletedChallengesCount(year, month);

            SetMonthProgress(monthCompletedChallengesCount, monthChallengesCount, true, false);
        }
        public void SetMonthProgress(int completedChallenges, int totalDays, bool instant, bool giveRewardIfFound)
        {
            monthProgressSB.Clear();
            monthProgressSB.Append(completedChallenges);
            monthProgressSB.Append(monthProgressTotalDaysPrefix);
            monthProgressSB.Append("/");
            monthProgressSB.Append(totalDays);

            monthProgressTXT.text = monthProgressSB.ToString();

            int rewardDayIndex = -1;
            int totalMonthDays = DateTime.DaysInMonth(selectedMonthFirstDay.Year, selectedMonthFirstDay.Month);

            int[] rewardDays = DailyChallenges.GetMonthRewardDays();

            for (int i = 0; i < rewardDays.Length; i++)
            {
                int rewardDay = Mathf.Clamp(rewardDays[i], 1, totalMonthDays);

                bool openedChest = rewardDay <= completedChallenges;
                bool isRewardDay = rewardDay == completedChallenges;

                if(!instant && isRewardDay)
                {
                    closedChests[i].SetActive(true);
                    openedChests[i].SetActive(false);
                }
                else
                {
                    closedChests[i].SetActive(!openedChest);
                    openedChests[i].SetActive(openedChest);
                }

                chestsRequiredCount[i].text = rewardDay.ToString();

                float placement = rewardDay / (float)totalMonthDays;

                monthProgressSlider.SetSeparatorPlacement(i, placement);

                if(isRewardDay)
                    rewardDayIndex = i;
            }
            
            monthProgressSlider.Initialize();

            if(instant)
            {
                monthProgressSlider.Value = (completedChallenges / (float)totalDays) - 0.00f;  //The deducted 0.001f is to make sure that the slider doens't
                                                                                                 //jump to the next secion if it's exactly at a section's end. 
            }
            else
                StartCoroutine(SettingMonthProgress(completedChallenges, totalDays, rewardDayIndex));
        }

        public Transform GetMonthProgressTXTTransform()
        {
            return monthProgressTXT.transform;
        }
        #endregion
    }
}