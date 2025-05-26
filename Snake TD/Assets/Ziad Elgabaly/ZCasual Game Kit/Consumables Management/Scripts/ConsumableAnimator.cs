using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZUtilities;

namespace ZCasualGameKit
{
    public class ConsumableAnimator
    {
        private ConsumableIncreaseAnimation increaseAnimation = new ConsumableIncreaseAnimation();
        private ConsumableIncreaseAnimation decreaseAnimation = new ConsumableIncreaseAnimation();

        public void Initialize(MonoBehaviour player)
        {
            increaseAnimation.Initialize(player);
            decreaseAnimation.Initialize(player);
        }

        public void PlayIncrease(ConsumableIncreaseAnimationData animationData, ConsumablesManager.Consumable consumable, float unitDifference, bool reuseUnits)
        {
            List<RectTransform> heldUnits = null;

            if (increaseAnimation.IsUnitAnimationPlaying())
            {
                if (reuseUnits)
                {
                    heldUnits = increaseAnimation.GetAndClearHeldVisualUnits();
                    increaseAnimation.StopMainCoroutine();
                    increaseAnimation.StopUnitAnimations();
                }

            }
            else if (decreaseAnimation.IsUnitAnimationPlaying())
            {
                heldUnits = increaseAnimation.GetAndClearHeldVisualUnits();
                decreaseAnimation.StopMainCoroutine();
                decreaseAnimation.StopUnitAnimations();
            }

            increaseAnimation.Play(animationData, consumable, unitDifference, heldUnits);
        }
    }
}