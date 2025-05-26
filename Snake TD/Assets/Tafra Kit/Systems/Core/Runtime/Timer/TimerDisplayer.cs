using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.Data;

namespace TafraKit
{
    public class TimerDisplayer : MonoBehaviour
    {
        #region Classes, Structs & Enums
        public enum TimerTypes
        {
            HoursMinutesSeconds,
            MinutesSeconds,
            Seconds,
            Minutes,
            Hours
        }
        #endregion

        [SerializeField] private Timer timer;

        [Header("Display Options")]
        [SerializeField] private TimeUnit minUnit = TimeUnit.Second;
        [SerializeField] private TimeUnit maxUnit = TimeUnit.Hour;
        [SerializeField] private bool lettersFormat;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI timeTXT;

        private StringBuilder sb = new StringBuilder();
        private bool timerActive;
        private bool timerIsCountingDown;
        private double lastUpdatedTotalSeconds;

        private void OnEnable()
        {
            if (timer == null)
                return;

            timer.OnStart.AddListener(OnTimerStart);
            timer.OnPause.AddListener(OnTimerPause);
            timer.OnResume.AddListener(OnTimerResume);
            timer.OnEnd.AddListener(OnTimerEnd);

            if (timer.IsActive())
            {
                timerActive = true;
                UpdateTime(timer.GetTime());
            }

            timerIsCountingDown = timer.IsCountingDown();
        }
        private void OnDisable()
        {
            if (timer == null)
                return;

            timer.OnStart.RemoveListener(OnTimerStart);
            timer.OnPause.RemoveListener(OnTimerPause);
            timer.OnResume.RemoveListener(OnTimerResume);
            timer.OnEnd.RemoveListener(OnTimerEnd);
        }

        void OnTimerStart()
        {
            timerActive = true;

            UpdateTime(timer.GetTime());
        }
        void OnTimerPause()
        {
            UpdateTime(timer.GetTime());

            timerActive = false;
        }
        void OnTimerResume()
        {
            UpdateTime(timer.GetTime());

            timerActive = true;
        }
        void OnTimerEnd()
        {
            timerActive = false;
        }
        private void Update()
        {
            if(timer == null || !timerActive) return;

            double totalSeconds = timer.GetTime();

            if(!timerIsCountingDown && totalSeconds < lastUpdatedTotalSeconds + 1)
                return;
            else if(timerIsCountingDown && totalSeconds > lastUpdatedTotalSeconds - 1)
                return;

            UpdateTime(totalSeconds);
        }
        public void UpdateTime(double totalSeconds)
        {
            TafraDateTime.GetTimeString(sb, totalSeconds, minUnit, maxUnit, timerIsCountingDown, lettersFormat);
            timeTXT.text = sb.ToString();

            lastUpdatedTotalSeconds = timerIsCountingDown ? Mathf.CeilToInt((float)totalSeconds) : Mathf.FloorToInt((float)totalSeconds);
        }

        public void SetTimer(Timer timer)
        {
            if (this.timer != null)
                OnDisable();

            this.timer = timer;

            OnEnable();

            //this.timer = timer;

            //if(timer.IsActive())
            //    timerActive = true;

            //timerIsCountingDown = timer.IsCountingDown();
            //lastUpdatedTotalSeconds = timerIsCountingDown ? Mathf.CeilToInt((float)timer.GetTime()) : Mathf.FloorToInt((float)timer.GetTime());
        }
    }
}