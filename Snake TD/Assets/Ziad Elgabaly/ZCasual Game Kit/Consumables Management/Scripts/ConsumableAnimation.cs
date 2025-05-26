using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZUtilities;

namespace ZCasualGameKit
{
    public class ConsumableAnimation
    {
        protected MonoBehaviour player;
        protected bool isUnitAnimationPlaying;
        protected bool isBarPulseAnimationPlaying;
        protected bool isBarCountAnimationPlaying;
        protected List<RectTransform> heldVisualUnits = new List<RectTransform>();
        protected ConsumablesManager.Consumable lastConsumable;
        protected IEnumerator playingEnum;
        protected List<IEnumerator> runningUnitCoroutines = new List<IEnumerator>();
        protected List<IEnumerator> runningBarCoroutines = new List<IEnumerator>();
        protected IEnumerator runningBarIncrementCoroutines;
        protected IEnumerator lastBarIncrementCoroutines;
        protected RectTransform canvasRT;
        protected int animationsPlayed;
        protected Color barTXTOriginalColor;

        public void Initialize(MonoBehaviour _player)
        {
            player = _player;
        }

        public List<RectTransform> GetAndClearHeldVisualUnits()
        {
            List<RectTransform> held = new List<RectTransform>(heldVisualUnits);

            heldVisualUnits.Clear();

            isUnitAnimationPlaying = false;

            return held;
        }

        public bool IsUnitAnimationPlaying()
        {
            return isUnitAnimationPlaying;
        }

        public void StopMainCoroutine()
        {
            if (player != null && playingEnum != null)
                player.StopCoroutine(playingEnum);
        }
        public void StopUnitAnimations()
        {
            if (player == null)
                return;

            for (int i = 0; i < runningUnitCoroutines.Count; i++)
            {
                if (runningUnitCoroutines[i] != null)
                    player.StopCoroutine(runningUnitCoroutines[i]);
            }
            

            runningUnitCoroutines.Clear();
        }

        public void StopBarAnimationAndResetIt()
        {
            isBarPulseAnimationPlaying = false;

            if (player != null)
            {
                for (int i = 0; i < runningBarCoroutines.Count; i++)
                {
                    if (runningBarCoroutines[i] != null)
                        player.StopCoroutine(runningBarCoroutines[i]);
                }
                runningBarCoroutines.Clear();

                if (lastConsumable != null)
                {
                    lastConsumable.Bar.AmountTXT().color = barTXTOriginalColor;
                    lastConsumable.Bar.IconRectTransform().localScale = Vector3.one;
                }
            }
        }
    }
}