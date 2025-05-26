using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TafraKit
{
    public class GenericTimerDisplayer : MonoBehaviour
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

        [Header("Timer Fetching Approaches")]
        [SerializeField] private Object timerObject;
        [SerializeField] private string timerID;

        [Header("Display Options")]
        [SerializeField] private TimerTypes timerType;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI timeTXT;

        private ITimer timer;

        private void Start()
        {
            if(timerObject != null)
                timer = timerObject as ITimer;
            else
                Timers.TryGetTimer(timerID, out timer);
        }
        private void Update()
        {
            if(timer == null) return;

            UpdateTime(timer.CurrentTime);
        }
        public void UpdateTime(double totalSeconds)
        {
            switch(timerType)
            {
                case TimerTypes.HoursMinutesSeconds:
                    {
                        double fractionHours = totalSeconds / 3600f;
                        int wholeHours = Mathf.FloorToInt((float)fractionHours);
                        double hoursFraction = fractionHours - wholeHours;

                        double fractionMinutes = hoursFraction * 60;
                        int wholeMinutes = Mathf.FloorToInt((float)fractionMinutes);
                        double minutesFraction = fractionMinutes - wholeMinutes;

                        int wholeSeconds = timer.IsCountingDown ? Mathf.CeilToInt((float)minutesFraction * 60) : Mathf.FloorToInt((float)minutesFraction * 60);

                        if (wholeSeconds >= 60)
                        {
                            wholeSeconds -= 60;
                            wholeMinutes++;
                        }

                        timeTXT.text = $"{wholeHours:00}:{wholeMinutes:00}:{wholeSeconds:00}";
                    }
                    break;
                case TimerTypes.MinutesSeconds:
                    {
                        double fractionMinutes = totalSeconds / 60f;
                        int wholeMinutes = Mathf.FloorToInt((float)fractionMinutes);
                        double minutesFraction = fractionMinutes - wholeMinutes;

                        int wholeSeconds = timer.IsCountingDown ? Mathf.CeilToInt((float)minutesFraction * 60) : Mathf.FloorToInt((float)minutesFraction * 60);

                        if (wholeSeconds >= 60)
                        {
                            wholeSeconds -= 60;
                            wholeMinutes++;
                        }

                        timeTXT.text = $"{wholeMinutes:00}:{wholeSeconds:00}";
                    }
                    break;
                case TimerTypes.Seconds:
                    {
                        int wholeSeconds = Mathf.CeilToInt((float)totalSeconds);

                        timeTXT.text = $"{wholeSeconds:0}";
                    }
                    break;
                case TimerTypes.Minutes:
                    {

                    }
                    break;
                case TimerTypes.Hours:
                    {

                    }
                    break;
                default:
                    break;
            }
        }

        public void SetTimer(ITimer timer)
        {
            this.timer = timer;
        }
    }
}