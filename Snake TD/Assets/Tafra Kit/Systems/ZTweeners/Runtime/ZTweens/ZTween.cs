using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit.ZTweeners
{
    public class ZTween
    {
        public ZTween() { }

        protected float duration;
        protected float delay;
        protected bool useUnscaledTime;
        protected MonoBehaviour player;
        protected MonoBehaviour previousPlayer;
        protected EasingType easingType = new EasingType(MotionType.Linear, new EasingEquationsParameters());

        protected UnityEvent onActualStart = new UnityEvent();
        protected UnityEvent onUpdated = new UnityEvent();
        protected UnityEvent onCompleted = new UnityEvent();
        protected UnityEvent onStopped = new UnityEvent();

        protected bool isPlaying;
        protected IEnumerator tweeningEnum;

        public UnityEvent OnActualStart { get { return onActualStart; } }
        public UnityEvent OnUpdated { get { return onUpdated; } }
        public UnityEvent OnCompleted { get { return onCompleted; } }
        public UnityEvent OnTerminated { get { return onStopped; } }

        public MonoBehaviour Player
        {
            get { return player; }
            set { player = value; }
        }
        public MonoBehaviour PreviousPlayer
        {
            get { return previousPlayer; }
            set { previousPlayer = value; }
        }
        public bool IsPlaying
        {
            get { return isPlaying; }
        }

        protected virtual void Scrub(float t) { }

        public virtual IEnumerator Tweening()
        {
            isPlaying = true;

            //This is here to wait until properties are set after construction.
            yield return Yielders.EndOfFrame;

            if(delay > 0.001f)
            {
                if(!useUnscaledTime)
                    yield return Yielders.GetWaitForSeconds(delay);
                else
                    yield return Yielders.GetWaitForSecondsRealtime(delay);
            }

            if (duration > 0.001f)
            {
                float time = useUnscaledTime ? Time.unscaledTime : Time.time;
                float startTime = time;
                float endTime = startTime + duration;

                while (time < endTime)
                {
                    float t = (time - startTime) / duration;
                    t = MotionEquations.GetEaseFloat(t, easingType);

                    Scrub(t);

                    onUpdated?.Invoke();

                    yield return null;

                    time = useUnscaledTime ? Time.unscaledTime : Time.time;
                }
            }

            Scrub(MotionEquations.GetEaseFloat(1, easingType));

            onUpdated?.Invoke();

            onCompleted?.Invoke();

            isPlaying = false;
        }

        public void Play()
        {
            if (player)
            {
                if (!player.gameObject.activeInHierarchy) return;

                tweeningEnum = Tweening();
                player.StartCoroutine(tweeningEnum);
                previousPlayer = player;
            }
        }

        public void Stop()
        {
            if (previousPlayer && tweeningEnum != null)
            {
                previousPlayer.StopCoroutine(tweeningEnum);
                tweeningEnum = null;
                isPlaying = false;
            }

            onStopped?.Invoke();
        }

        public void ResetEvents()
        {
            onActualStart.RemoveAllListeners();
            onUpdated.RemoveAllListeners();
            onCompleted.RemoveAllListeners();
            onStopped.RemoveAllListeners();
        }
    }
}
