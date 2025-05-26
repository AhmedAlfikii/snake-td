using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.SceneManagement;
using UnityEngine;
using ZUI;

namespace TafraKit.MetaGame
{
    public class DCCompletedPanelAnimation : MonoBehaviour
    {
        [SerializeField] private DailyChallengesCalendarPanel dailyChallengesCalendarPanel;
        [SerializeField] private DailyChallengesMonthPanel dailyChallengesMonthPanel;

        [Header("Challenge Completed Animation")]
        [SerializeField] private UIElement completedAnimationCheckMark;
        [SerializeField] private float checkMarkMovementDuration = 0.75f;

        private bool isPlaying;

        private void Start()
        {
            Dictionary<string, object> loadParams = TafraSceneManager.GetLoadParameters();

            if (loadParams.TryGetValue("challenge_completed", out object newlyCompletedChallengeObject))
            {
                DateTime completedChallenge = (DateTime)newlyCompletedChallengeObject;

                StartCoroutine(PlayNewlyCompletedChallengeAnimation(completedChallenge.Day));
            }
        }

        private void OnDisable()
        {
            if (isPlaying)
            { 
                InputBlocker.UnblockTouches("DCCCompletedPanelAnimation");
            }
        }

        private IEnumerator PlayNewlyCompletedChallengeAnimation(int day)
        {
            isPlaying = true;

            int selectedSlotIndex = dailyChallengesCalendarPanel.GetSelectedSlotIndex();
            int selectedMonthChallengesCount = dailyChallengesCalendarPanel.GetSelectedMonthChallengesCount();
            int selectedMonthCompletedChallengesCount = dailyChallengesCalendarPanel.GetSelectedMonthCompletedChallengesCount();
            List<DailyChallengesSlot> slots = dailyChallengesCalendarPanel.GetSlots();

            InputBlocker.BlockTouches("DCCCompletedPanelAnimation");

            dailyChallengesCalendarPanel.AddPlayButtonDisabler("CompletedChallenge");

            if (dailyChallengesMonthPanel)
                dailyChallengesMonthPanel.SetMonthProgress(selectedMonthCompletedChallengesCount - 1, selectedMonthChallengesCount, true, false);

            DailyChallengesSlot completedSlot = slots[day - 1];
            DailyChallengesSlot selectedSlot = null;

            if (selectedSlotIndex > -1)
                selectedSlot = slots[selectedSlotIndex];

            completedSlot.SetSelectedState(true, true);
            completedSlot.SetState(DailyChallenges.ChallengeState.PlayableAtCost);

            if (selectedSlot)
                selectedSlot.SetSelectedState(false, true);

            yield return Yielders.GetWaitForSeconds(0.6f);

            Vector3 startPosition = slots[day - 1].transform.position;

            completedAnimationCheckMark.transform.position = startPosition;

            completedAnimationCheckMark.ChangeVisibility(true);

            yield return Yielders.GetWaitForSeconds(0.5f);

            float startTime = Time.time;
            float endTime = startTime + checkMarkMovementDuration;
            Vector3 targetPos = dailyChallengesMonthPanel.GetMonthProgressTXTTransform().position;

            Vector3 midPoint = (startPosition + targetPos) / 2f + (Vector3.up * (targetPos - startPosition).magnitude / 2f);

            while (Time.time < endTime)
            {
                float t = (Time.time - startTime) / checkMarkMovementDuration;
                t = MotionEquations.EaseIn(t);

                completedAnimationCheckMark.transform.position = ZBezier.GetPointOnQuadraticCurve(t, startPosition, midPoint, targetPos);
                yield return null;
            }

            completedAnimationCheckMark.transform.position = targetPos;

            completedAnimationCheckMark.ChangeVisibility(false);

            if (dailyChallengesMonthPanel)
                dailyChallengesMonthPanel.SetMonthProgress(selectedMonthCompletedChallengesCount, selectedMonthChallengesCount, false, true);

            yield return Yielders.GetWaitForSeconds(0.5f);

            completedSlot.SetState(DailyChallenges.ChallengeState.Completed);

            yield return Yielders.GetWaitForSeconds(0.35f);

            completedSlot.SetSelectedState(false, false);

            if (selectedSlot)
                selectedSlot.SetSelectedState(true, false);

            dailyChallengesCalendarPanel.RemovePlayButtonDisabler("CompletedChallenge");

            InputBlocker.UnblockTouches("DCCCompletedPanelAnimation");
            isPlaying = false;
        }
    }
}