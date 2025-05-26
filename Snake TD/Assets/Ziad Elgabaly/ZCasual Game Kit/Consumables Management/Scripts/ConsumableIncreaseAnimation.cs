using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TafraKit;
using TMPro;

namespace ZCasualGameKit
{
    [Serializable]
    public class ConsumableIncreaseAnimation : ConsumableAnimation
    {
        #region Private Fields
        private TafraKit.EasingEquationsParameters easingParameters = new TafraKit.EasingEquationsParameters(
            new TafraKit.EasingEquationsParameters.CustomParameters(),
            new TafraKit.EasingEquationsParameters.EaseInOutParameters(),
            new TafraKit.EasingEquationsParameters.EaseInOutBackParameters(),
            new TafraKit.EasingEquationsParameters.EaseInOutElasticParameters());

        private bool unitJustHitBar;
        private float cachedShowingUnitRadius = -1f;
        private float cachedShowingUnitRadiusPercentage = -1;
        private float cachedShowingUnitRadiusDimension = -1;
        #endregion

        public void Play(ConsumableIncreaseAnimationData data, ConsumablesManager.Consumable consumable, float addedUnits, List<RectTransform> alreadyHeldVisualUnits = null)
        {
            playingEnum = Playing(data, consumable, addedUnits, alreadyHeldVisualUnits);

            player.StartCoroutine(playingEnum);
        }

        IEnumerator Playing(ConsumableIncreaseAnimationData data, ConsumablesManager.Consumable consumable, float addedUnits, List<RectTransform> alreadyHeldVisualUnits = null)
        {
            animationsPlayed++;
            int curAnimationNumber = animationsPlayed;
            isUnitAnimationPlaying = true;

            #region Spawn and place units
            List<RectTransform> heldUnits = new List<RectTransform>();

            //Determine the number of visual units that will be used to represent the added units.
            int visualUnitsCount = Mathf.CeilToInt((addedUnits / 100f) * data.VisualsPer100Units);

            if (visualUnitsCount < data.MinimumVisualUnits)
                visualUnitsCount = data.MinimumVisualUnits;
            else if (data.MaximumVisualUnits > 0 && visualUnitsCount > data.MaximumVisualUnits)
                visualUnitsCount = data.MaximumVisualUnits;

            if (alreadyHeldVisualUnits != null)
            {
                int countDiff = alreadyHeldVisualUnits.Count - visualUnitsCount;

                //If there're more units held than needed, then take what you need and put the rest back in the pool.
                if (countDiff > 0)
                {
                    int unusedStartIndex = alreadyHeldVisualUnits.Count - countDiff;

                    consumable.AnimationUnitsPool.ReleaseUnits(alreadyHeldVisualUnits.GetRange(unusedStartIndex, countDiff));

                    alreadyHeldVisualUnits.RemoveRange(unusedStartIndex, countDiff);

                    heldUnits.AddRange(alreadyHeldVisualUnits);
                }
                //If there're less units held than needed, then take them and get the rest from the pool.
                else if (countDiff < 0)
                {
                    heldUnits.AddRange(alreadyHeldVisualUnits);

                    int neededUnits = visualUnitsCount - alreadyHeldVisualUnits.Count;

                    heldUnits.AddRange(consumable.AnimationUnitsPool.RequestUnits(neededUnits));
                }
                //If there're exactly the needed units, then take them.
                else
                    heldUnits = new List<RectTransform>(alreadyHeldVisualUnits);
            }
            else
                heldUnits = consumable.AnimationUnitsPool.RequestUnits(visualUnitsCount);

            heldVisualUnits.AddRange(heldUnits);

            if (heldUnits.Count > 0 && canvasRT == null)
                canvasRT = heldUnits[0].root.GetComponent<RectTransform>();

            //Hide, and position all the units.
            float radius = GetShowingUnitsRadius(data);

            for (int i = 0; i < heldUnits.Count; i++)
            {
                heldUnits[i].localScale = Vector3.zero;

                Vector3 spawnPosition = new Vector3();

                if (data.ShowingUnitsCustomPosition != null)
                {
                    Vector2 pos = (Vector2)data.ShowingUnitsCustomPosition;
                    spawnPosition = new Vector3(pos.x, pos.y, canvasRT.position.z);
                }
                else
                    spawnPosition = canvasRT.position;

                if (radius > 0.0001f)
                    heldUnits[i].position = spawnPosition + (Vector3)(UnityEngine.Random.insideUnitCircle * radius);
                else
                    heldUnits[i].position = spawnPosition;
            }
            #endregion

            if (data.ShowingAnimationStartDelay > 0.0001f)
                yield return Yielders.GetWaitForSeconds(data.ShowingAnimationStartDelay);

            float unitsReachBarDelay = 0;
            float unitsContactWithBarDuration = 0;  //From the time the first unit hit the bar, until the time the last unit hits it.
            float unitsAnimationTotalDuration = 0;

            if (visualUnitsCount > 0)
            {
                if (data.UnitsHideStartBehaviour == ConsumableIncreaseAnimationData.HideStartBehaviour.AfterSelfShow)
                {
                    unitsReachBarDelay = data.ShowingSignleUnitDuration + data.HidingUnitsDelay + (data.MoveToBar ? data.HidingUnitsMovementDuration : 0);
                    unitsContactWithBarDuration = data.ShowingUnitsInterval * (visualUnitsCount - 1);

                    //Delay = SD + HD + HMD
                    //Total Duration = SI(U - 1) + SD + HD + HMD
                    //Duration - Delay = SI(U - 1)
                }
                else if (data.UnitsHideStartBehaviour == ConsumableIncreaseAnimationData.HideStartBehaviour.AfterAllShow)
                {
                    unitsReachBarDelay = data.ShowingSignleUnitDuration + data.ShowingUnitsInterval * (visualUnitsCount - 1) + data.HidingUnitsDelay + (data.MoveToBar? data.HidingUnitsMovementDuration : 0);
                    unitsContactWithBarDuration = data.HidingUnitsInterval * (visualUnitsCount - 1);

                    //Delay = SD + SI(U - 1) + HD +  HMD
                    //Total Duration = SI(U - 1) + SD + HD + HI(U - 1) + HMD
                    //Duration - Delay = HI(U - 1)
                }
            }

            unitsAnimationTotalDuration = unitsReachBarDelay + unitsContactWithBarDuration;

            #region Fire units animation
            Vector3? hideDestination = data.MoveToBar? (Vector3?)consumable.Bar.IconRectTransform().position : null;

            int lastAudioUnitIndex = -9999;
            for (int i = 0; i < heldUnits.Count; i++)
            {
                bool playAudio = i - lastAudioUnitIndex > data.SingleUnitSoundSkips;

                if (playAudio)
                    lastAudioUnitIndex = i;

                int firstOrLastValue = -1;

                if (i == 0)
                    firstOrLastValue = 0;
                else if (i == heldUnits.Count - 1)
                    firstOrLastValue = 1;

                float hideDelay = 0;

                if (data.UnitsHideStartBehaviour == ConsumableIncreaseAnimationData.HideStartBehaviour.AfterSelfShow)
                    hideDelay = data.HidingUnitsDelay;
                else
                    hideDelay =  data.HidingUnitsDelay + data.ShowingUnitsInterval * (visualUnitsCount - i - 1) + data.HidingUnitsInterval * i;

                IEnumerator unitEnum = UnitAnimation(heldUnits[i], data, playAudio, firstOrLastValue, data.ShowingUnitsInterval * i, hideDelay, hideDestination);

                player.StartCoroutine(unitEnum);

                runningUnitCoroutines.Add(unitEnum);
            }
            #endregion

            #region Bar Animation
            if (!isBarPulseAnimationPlaying)
            {
                StopBarAnimationAndResetIt();

                IEnumerator barCoinAnimCo = BarPulseAnimation(consumable, data, visualUnitsCount);

                player.StartCoroutine(barCoinAnimCo);

                runningBarCoroutines.Add(barCoinAnimCo);
            }

            //if (!isBarCountAnimationPlaying)
            {
                //if (runningBarIncrementCoroutines != null)
                //    player.StopCoroutine(runningBarIncrementCoroutines);

                runningBarIncrementCoroutines = BarIncreaseAnimation(runningBarIncrementCoroutines, consumable, unitsReachBarDelay, isBarCountAnimationPlaying? 0 : (unitsContactWithBarDuration + 0.25f));
                player.StartCoroutine(runningBarIncrementCoroutines);
            }
            #endregion

            lastConsumable = consumable;

            //Get the time the entire units animation will finish, so we can release the held units later.
            float unitsAnimationFinishTime = Time.time + unitsAnimationTotalDuration;

            //Check if all the units finished animating.
            while (Time.time < unitsAnimationFinishTime)
            {
                yield return null;
            }

            //Release all held units.
            for (int i = 0; i < heldUnits.Count; i++)
            {
                heldVisualUnits.Remove(heldUnits[i]);
            }

            consumable.AnimationUnitsPool.ReleaseUnits(heldUnits, true);

            //Mark the animation's unit animation as not playing, only if this is the last animation played, otherwise the last animation will take care of marking the class as not playing unit animation.
            if (curAnimationNumber == animationsPlayed)
                isUnitAnimationPlaying = false;
        }

        IEnumerator UnitAnimation(RectTransform unit, ConsumableIncreaseAnimationData data, bool playAudio, int firstOrLast, float spawnDelay, float hideDelay, Vector3? hidePosition)
        {
            //Wait for the spawn delay duration to pass.
            if (spawnDelay > 0.0001f)
                yield return Yielders.GetWaitForSeconds(spawnDelay);

            #region Show the unit (increase it in scale)
            //Play the unit's show sound effect if found.
            if (playAudio && data.SingleUnitShowAC.Clip != null)
                SFXPlayer.Play(data.SingleUnitShowAC.Clip, data.SingleUnitShowAC.VolumeRange.GetRandomValue(), data.SingleUnitShowAC.PitchRange.GetRandomValue());

            //Play the group's show sound effect if this is first unit and there's a sound effect.
            if (firstOrLast  == 0 && data.GroupShowAC.Clip != null)
                SFXPlayer.Play(data.GroupShowAC.Clip, data.GroupShowAC.VolumeRange.GetRandomValue(), data.GroupShowAC.PitchRange.GetRandomValue());

            float startTime = Time.time;
            float duration = data.ShowingSignleUnitDuration;

            if (duration > 0.0001f)
            {
                while (Time.time < startTime + duration)
                {
                    float t = (Time.time - startTime) / duration;

                    t = MotionEquations.GetEaseFloat(t, data.ShowEasingType.Easing, data.ShowEasingType.Parameters);

                    unit.localScale = Vector3.LerpUnclamped(Vector3.zero, data.ShowingUnitsScale, t);

                    yield return null;
                }
            }

            unit.localScale = Vector3.one;
            #endregion

            //Wait for the hide delay duration to pass.
            if (hideDelay > 0.0001f)
                yield return Yielders.GetWaitForSeconds(hideDelay);

            #region Move the unit if it has a destination
            if (hidePosition != null)
            {
                //Play the unit's move sound effect if found.
                if (playAudio && data.SingleUnitMoveAC.Clip != null)
                    SFXPlayer.Play(data.SingleUnitMoveAC.Clip, data.SingleUnitMoveAC.VolumeRange.GetRandomValue(), data.SingleUnitMoveAC.PitchRange.GetRandomValue());

                //Play the group's move sound effect if this is first unit and there's a sound effect.
                if (firstOrLast == 0 && data.GroupMoveAC.Clip != null)
                    SFXPlayer.Play(data.GroupMoveAC.Clip, data.GroupMoveAC.VolumeRange.GetRandomValue(), data.GroupMoveAC.PitchRange.GetRandomValue());

                startTime = Time.time;
                duration = data.HidingUnitsMovementDuration;

                Vector3 startPos = unit.position;
                Vector3 hidingPos = (Vector3)hidePosition;

                if (duration > 0.0001f)
                {
                    while (Time.time < startTime + duration)
                    {
                        float t = (Time.time - startTime) / duration;

                        t = MotionEquations.GetEaseFloat(t, data.MoveEasingType.Easing, data.MoveEasingType.Parameters);

                        unit.position = Vector3.LerpUnclamped(startPos, hidingPos, t);
                        unit.localScale = Vector3.LerpUnclamped(data.ShowingUnitsScale, data.ReachBarUnitsScale, t);

                        yield return null;
                    }
                }

                unit.position = hidingPos;

                UnitHitBar();
            }
            #endregion

            #region Hide the unit
            //Play the unit's move sound effect if found.
            if (playAudio && data.SingleUnitHideAC.Clip != null)
                SFXPlayer.Play(data.SingleUnitHideAC.Clip, data.SingleUnitHideAC.VolumeRange.GetRandomValue(), data.SingleUnitHideAC.PitchRange.GetRandomValue());

            //Play the group's hide sound effect if this is first unit and there's a sound effect.
            if (firstOrLast == 0 && data.GroupHideAC.Clip != null)
                SFXPlayer.Play(data.GroupHideAC.Clip, data.GroupHideAC.VolumeRange.GetRandomValue(), data.GroupHideAC.PitchRange.GetRandomValue());

            //Hide the unit
            startTime = Time.time;
            duration = data.HidingSignleUnitDuration;

            if (duration > 0.0001f)
            {
                while (Time.time < startTime + duration)
                {
                    float t = (Time.time - startTime) / duration;

                    t = MotionEquations.GetEaseFloat(t, data.HideEasingType.Easing, data.HideEasingType.Parameters);

                    unit.localScale = Vector3.LerpUnclamped(Vector3.one, Vector3.zero, t);

                    yield return null;
                }
            }

            unit.localScale = Vector3.zero;
            #endregion
        }

        IEnumerator BarPulseAnimation(ConsumablesManager.Consumable consumable, ConsumableIncreaseAnimationData data, int units)
        {
            isBarPulseAnimationPlaying = true;

            float startAmount = consumable.Bar.Amount;
            float finalAmount = consumable.Units;

            RectTransform barIcon = consumable.Bar.IconRectTransform();
            TextMeshProUGUI barTXT = consumable.Bar.AmountTXT();
            barTXTOriginalColor = barTXT.color;

            //Wait for the first unit to hit the bar.
            if (units > 0)
            {
                while (!unitJustHitBar)
                {
                    yield return null;
                }
            }

            float startTime = Time.time;
            float iconStartTime = startTime;
            float duration = 0.25f;

            while (isUnitAnimationPlaying || (!isUnitAnimationPlaying && Time.time < iconStartTime + duration * 2f))
            {
                if (unitJustHitBar)
                {
                    if (Time.time > iconStartTime + (duration / 2f))
                        iconStartTime = Time.time - (duration * 0.1f);

                    unitJustHitBar = false;
                }

                float t = (Time.time - startTime) / duration;
                float iconT = (Time.time - iconStartTime) / duration;

                if (iconT > 1)
                {
                    iconT = (2 - iconT);
                    t = iconT;
                }

                barIcon.localScale = Vector3.Lerp(Vector3.one, data.BarUnitIconIncreaseScale, iconT);
                barTXT.color = Color.Lerp(barTXTOriginalColor, data.BarInreasingColor, t);
                yield return null;
            }

            isBarPulseAnimationPlaying = false;

            //Get back to normal.
            //startTime = Time.time;
            //duration = 0.5f;
            //while (Time.time < startTime + duration)
            //{
            //    float t = (Time.time - startTime) / duration;

            //    barIcon.localScale = Vector3.Lerp(new Vector3(1.2f, 1.2f, 1.2f), Vector3.one, t);

            //    yield return null;
            //}
            //barIcon.localScale = Vector3.one;
        }

        IEnumerator BarIncreaseAnimation(IEnumerator me, ConsumablesManager.Consumable consumable, float delay, float duration)
        {
            float targetValue = consumable.Units;

            if (delay > 0.0001f)
                yield return Yielders.GetWaitForSeconds(delay);
             
            if (lastBarIncrementCoroutines != null)
                player.StopCoroutine(lastBarIncrementCoroutines);

            lastBarIncrementCoroutines = me;

            isBarCountAnimationPlaying = true;

            TextMeshProUGUI barTXT = consumable.Bar.AmountTXT();
            float startNumber = consumable.Bar.Amount;
            float startTime = Time.time;

            while (Time.time < startTime + duration)
            {
                float t = (Time.time - startTime) / duration;

                int curValue = (int)Mathf.Lerp(startNumber, targetValue, t);

                consumable.Bar.Amount = curValue;

                yield return null;
            }

            consumable.Bar.Amount = targetValue;

            isBarCountAnimationPlaying = false;
        }

        void UnitHitBar()
        {
            unitJustHitBar = true;
        }

        /// <summary>
        /// Calculate or return the previously calculated showing unit radius based on the determined percentage.
        /// </summary>
        /// <param name="forceRefresh">Should the calculation be made even if there's a cached value? (use this if the screen/canvas resolution has changed)</param>
        /// <returns></returns>
        float GetShowingUnitsRadius(ConsumableIncreaseAnimationData data, bool forceRefresh = false)
        {
            #region Calculating Dimensions
            if (cachedShowingUnitRadiusDimension < 0 || forceRefresh)
            {
                if (canvasRT != null)
                    cachedShowingUnitRadiusDimension = (canvasRT.rect.width > canvasRT.rect.height ? canvasRT.rect.height * canvasRT.lossyScale.x : canvasRT.rect.width * canvasRT.lossyScale.x) / 2f;
                else
                    cachedShowingUnitRadiusDimension = (Screen.width > Screen.height ? Screen.height : Screen.width) / 2f;
            }
            #endregion

            #region Calculating Radius
            if (cachedShowingUnitRadius < 0 || forceRefresh || Mathf.Abs(cachedShowingUnitRadiusPercentage - data.ShowingUnitsRadiusPercentage) > 0.001f)
            {
                if (data.ShowingUnitsRadiusPercentage > 0.0001f)
                    cachedShowingUnitRadius = cachedShowingUnitRadiusDimension * data.ShowingUnitsRadiusPercentage;
                else
                    cachedShowingUnitRadius = 0;

                cachedShowingUnitRadiusPercentage = data.ShowingUnitsRadiusPercentage;
            }
            #endregion

            return cachedShowingUnitRadius;
        }
    }
}