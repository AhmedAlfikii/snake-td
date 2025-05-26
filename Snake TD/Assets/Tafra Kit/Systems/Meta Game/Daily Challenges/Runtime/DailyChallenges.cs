using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TafraKit;
using TafraKit.Tasks;
using UnityEditor;
using TafraKit.SceneManagement;
using TafraKit.Consumables;
using UnityEngine.SearchService;

namespace TafraKit.MetaGame
{
    public static class DailyChallenges
    {
        public enum ChallengeState
        {
            /// <summary>
            /// Not available to play yet (date is yet to come).
            /// </summary>
            NotAvailable = -1,
            /// <summary>
            /// Today's challenge and is available to play (hasn't been played before).
            /// </summary>
            Playable = 0,
            /// <summary>
            /// Passed, or played at least once but not completed.
            /// </summary>
            PlayableAtCost = 1,
            /// <summary>
            /// Played and completed.
            /// </summary>
            Completed = 2
        }

        private const string ChallengeStatePFPrefix = "TAFRA_KIT_DC_CHALLENGE_STATE_";
        private const string MonthCompletedChallengesCountPFPrefix = "TAFRA_KIT_DC_MONTH_COMPLETED_CHALLENGES_COUNT_";

        private static DailyChallengesSettings settings;
        private static DailyChallengePlayCost playCost = new DailyChallengeRVPlayCost();
        private static bool isThereOpenedChallenge;
        private static DateTime openedChallenge;
        private static List<KeyValuePair<string, object>> onChallengeCloseLoadParameters = new List<KeyValuePair<string, object>>();
        public static DateTime OpenedChallenge
        {
            get { return openedChallenge; }
        }

        /// <summary>
        /// Fires once a challenge is requested to play. Parameters: year, month, day of the challenge.
        /// </summary>
        public static UnityEvent<int, int, int> OnChallengePlayRequest = new UnityEvent<int, int, int>();
        /// <summary>
        /// Fires once a challenge should be played, make sure to listen to this and load the challenge scene when it fires. Parameters: year, month, day of the challenge.
        /// </summary>
        public static UnityEvent<int, int, int> OnChallengePlay = new UnityEvent<int, int, int>();
        /// <summary>
        /// Fires once a challenge has been won. Parameters: year, month, day of the challenge.
        /// </summary>
        public static UnityEvent<int, int, int> OnChallengeCompleted = new UnityEvent<int, int, int>();
        /// <summary>
        /// Fires once a challenge is failed. Parameters: year, month, day of the challenge.
        /// </summary>
        public static UnityEvent<int, int, int> OnChallengeFailed = new UnityEvent<int, int, int>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Initialize()
        {
            settings = TafraSettings.GetSettings<DailyChallengesSettings>();

            if (!settings && !settings.Enabled)
                return;
        }

        public static async Task<BoolOperationResult> RequestPlayChallenge(int year, int month, int day, CancellationToken ct)
        {
            int challengeState = GetChallengeState(year, month, day);

            ChallengeState state = (ChallengeState)challengeState;

            //If the challenge wasn't already won before, then mark it as played.
            if (state == ChallengeState.Completed)
                return BoolOperationResult.Fail;

            OnChallengePlayRequest?.Invoke(year, month, day);

            if (state == ChallengeState.Playable)
            {
                PlayChallenge(year, month, day);
                return BoolOperationResult.Success;
            }
            if (state == ChallengeState.PlayableAtCost)
            {
                if (playCost == null)
                {
                    PlayChallenge(year, month, day);
                    return BoolOperationResult.Success;
                }
                else
                {
                    Task<BoolOperationResult> costPaying = playCost.PayCost(year, month, day, ct);

                    await costPaying;

                    if (costPaying.Result == BoolOperationResult.Success)
                        PlayChallenge(year, month, day);

                    return costPaying.Result;
                }
            }

            return BoolOperationResult.Fail;
        }
        private static void PlayChallenge(int year, int month, int day)
        {
            int challengeState = GetChallengeState(year, month, day);

            //If the challenge wasn't already won before, then mark it as played.
            if ((ChallengeState)challengeState != ChallengeState.Completed)
                PlayerPrefs.SetInt($"{ChallengeStatePFPrefix}_{year}_{month}_{day}", 1);

            OnChallengePlay?.Invoke(year, month, day);
            
            TafraSceneManager.LoadScene(settings.ChallengeSceneIndicesRange.GetRandomValue());
            MarkChallengeOpened(year, month, day);
        }
        public static void CompleteOpenedChallenge()
        {
            if (!isThereOpenedChallenge)
            {
                TafraDebugger.Log("Daily Challenges", "There's no opened challenge to complete.", TafraDebugger.LogType.Error);
                return;
            }

            PlayerPrefs.SetInt($"{ChallengeStatePFPrefix}_{openedChallenge.Year}_{openedChallenge.Month}_{openedChallenge.Day}", 2);

            int monthPorgress = GetMonthCompletedChallengesCount(openedChallenge.Year, openedChallenge.Month);

            monthPorgress++;
            
            PlayerPrefs.SetInt($"{MonthCompletedChallengesCountPFPrefix}_{openedChallenge.Year}_{openedChallenge.Month}", monthPorgress);

            OnChallengeCompleted?.Invoke(openedChallenge.Year, openedChallenge.Month, openedChallenge.Day);

            CloseOpenedChallenge();
        }
        public static void FailOpenedChallenge()
        {
            if (!isThereOpenedChallenge)
            {
                TafraDebugger.Log("Daily Challenges", "There's no opened challenge to fail.", TafraDebugger.LogType.Error);
                return;
            }

            OnChallengeFailed?.Invoke(openedChallenge.Year, openedChallenge.Month, openedChallenge.Day);

            CloseOpenedChallenge();
        }
        public static void CloseOpenedChallenge()
        {
            TafraSceneManager.LoadScene(settings.ExistSceneIndex, UnityEngine.SceneManagement.LoadSceneMode.Single, null, onChallengeCloseLoadParameters.ToArray());
            onChallengeCloseLoadParameters.Clear();

            MarkChallengeClosed();
        }
        public static void MarkChallengeOpened(int year, int month, int day)
        { 
            isThereOpenedChallenge = true;
            openedChallenge = new DateTime(year, month, day);
        }
        public static void MarkChallengeClosed()
        { 
            isThereOpenedChallenge = false;
        }

        public static void RegisterOnChallengeCloseLoadParam(string key, object value)
        {
            onChallengeCloseLoadParameters.Add(new KeyValuePair<string, object>(key, value));
        }

        /// <summary>
        /// Fills up the array sent with the states of the challenges of this month. Make sure the array sent has 31 elements.
        /// </summary>
        /// <param name="challengeStatesHolder"></param>
        public static void GetCurrentMonthChallengesState(int[] challengeStatesHolder, out int totalDays)
        {
            DateTime today = TafraDateTime.Today;

            GetMonthChallengesState(today.Year, today.Month, challengeStatesHolder, out totalDays);
        }
        /// <summary>
        /// Fills up the array sent with the states of the challenges of the given month. Make sure the array sent has 31 elements.
        /// States:
        /// <c>-1</c> : not available to play yet (date is yet to come).
        /// <c>0</c> : not played.
        /// <c>1</c> : played at least once but not completed.
        /// <c>2</c> : played and completed. 
        /// </summary>
        /// <param name="challengeStatesHolder"></param>
        public static void GetMonthChallengesState(int year, int month, int[] challengeStatesHolder, out int totalDays)
        {
            totalDays = DateTime.DaysInMonth(year, month);

            for (int i = 0; i < totalDays; i++)
            {
                challengeStatesHolder[i] = GetChallengeState(year, month, i + 1);
            }
        }
        public static int GetChallengeState(int year, int month, int day)
        {
            int state = -1;

            bool dateIsYetToCome = false;

            DateTime today = TafraDateTime.Today;
            DateTime challengeDay = new DateTime(year, month, day);

            if (challengeDay.Year > today.Year
                || (challengeDay.Year == today.Year && challengeDay.Month > today.Month)
                || (challengeDay.Year == today.Year && challengeDay.Month == today.Month && challengeDay.Day > today.Day))
                dateIsYetToCome = true;

            if (!dateIsYetToCome)
            {
                bool isToday = today == challengeDay;

                if (isToday)
                    state = PlayerPrefs.GetInt($"{ChallengeStatePFPrefix}_{challengeDay.Year}_{challengeDay.Month}_{challengeDay.Day}", 0);
                else
                    state = PlayerPrefs.GetInt($"{ChallengeStatePFPrefix}_{challengeDay.Year}_{challengeDay.Month}_{challengeDay.Day}", 1);
            }

            return state;
        }
        public static int GetCurrentMonthChallengesCount()
        { 
            DateTime date = TafraDateTime.Today;

            return DateTime.DaysInMonth(date.Year, date.Month);
        }
        public static string GetCurrentMonthName()
        {
            return TafraDateTime.Today.ToString("MMMM");
        }
        public static string GetMonthName(int year, int month)
        {
            DateTime date = new DateTime(year, month, 1);

            return date.ToString("MMMM");
        }
        public static int GetMonthCompletedChallengesCount(int year, int month)
        { 
            return PlayerPrefs.GetInt($"{MonthCompletedChallengesCountPFPrefix}_{year}_{month}", 0);
        }
        public static DailyChallengePlayCost GetActivePlayCost()
        {
            return playCost;
        }
        public static int[] GetMonthRewardDays()
        {
            return settings.MonthRewardDays;
        }
        public static List<ConsumableChange> GetMonthDayReward(int dayIndex)
        {
            return settings.MonthRewards[dayIndex].group;
        }
    }
}
