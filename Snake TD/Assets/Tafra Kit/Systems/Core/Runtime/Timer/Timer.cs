using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit
{
    //TODO:
    //1- Invoke events upon reaching certain seconds.
    public class Timer : MonoBehaviour
    {
        [SerializeField] private bool countdown;
        [SerializeField] private bool useUnscaledTime;

        public UnityEvent OnStart;
        public UnityEvent OnPause;
        public UnityEvent OnResume;
        public UnityEvent OnEnd;
        public UnityEvent OnReachedLimit;

        private bool active;
        private double seconds;
        private double limit;

        void Update()
        {
            if(!active) return;

            if(!countdown && limit > 0 && seconds >= limit)
            {
                seconds = limit;

                PauseTimer();

                OnReachedLimit?.Invoke();
                return;
            }
            else if(countdown && seconds <= limit)
            {
                seconds = limit;

                PauseTimer();
               
                OnReachedLimit?.Invoke();
                return;
            }

            if(!useUnscaledTime)
            {
                if(!countdown)
                    seconds += Time.deltaTime;
                else
                    seconds -= Time.deltaTime;
            }
            else
            {
                if(!countdown)
                    seconds += Time.unscaledDeltaTime;
                else
                    seconds -= Time.unscaledDeltaTime;
            }
        }

        public void StartTimer()
        {
            active = true;

            OnStart?.Invoke();
        }

        public void PauseTimer()
        {
            active = false;
            
            OnPause?.Invoke();
        }
        public void ResumeTimer()
        {
            active = true;

            OnResume?.Invoke();
        }

        public void EndTimer()
        {
            active = false;
            seconds = 0;

            OnEnd?.Invoke();
        }

        public void SetLimit(double limit)
        {
            this.limit = limit;
        }
        public void SetTime(double time)
        {
            seconds = time;
        }
        public double GetTime()
        {
            return seconds;
        }
        public bool IsActive()
        {
            return active;
        }
        public bool IsCountingDown()
        {
            return countdown;
        }
    }
}