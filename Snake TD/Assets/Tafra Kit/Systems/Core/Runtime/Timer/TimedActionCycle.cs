using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit
{
    public class TimedActionCycle : MonoBehaviour, ITimer
    {
        [SerializeField] private string id;
        [SerializeField] private TimeSpanSimple cycleDuration;
        [SerializeField] private bool useGameFirstOpenDate = true;

        public UnityEvent OnNewCycleStarted;

        private DateTime cycleStartDate;
        private DateTime cycleEndDate;

        private const string prefsKeyCycleStartDatePrefix = "TAFRA_TIMED_ACTION_CYCLE_START_DATE_";

        public string ID => id;
        public double CurrentTime => (cycleEndDate - TafraDateTime.Now).TotalSeconds;
        public bool IsCountingDown => true;

        private void Awake()
        {
            Timers.RegisterTimer(this);

            if(!PlayerPrefs.HasKey(prefsKeyCycleStartDatePrefix + id))
            {
                if(useGameFirstOpenDate)
                    cycleStartDate = TafraDateTime.FirstOpen;
                else
                    cycleStartDate = TafraDateTime.Now;

                PlayerPrefs.SetString(prefsKeyCycleStartDatePrefix + id, cycleStartDate.ToString());
            }
            else
                cycleStartDate = DateTime.Parse(PlayerPrefs.GetString(prefsKeyCycleStartDatePrefix + id));
            
            cycleEndDate = cycleStartDate.Add(cycleDuration.TimeSpan());
        }

        private void OnEnable()
        {
            if(CurrentTime < 0)
                StartNewCycle();
        }

        private void Update()
        {
            if(TafraDateTime.Now > cycleEndDate)
                StartNewCycle();
        }

        private void StartNewCycle()
        {
            TimeSpan passedTime = TafraDateTime.Now - cycleStartDate;

            int passedCycles = Mathf.FloorToInt((float)passedTime.TotalSeconds / (float)cycleDuration.TimeSpan().TotalSeconds);
            
            cycleStartDate = cycleStartDate.AddSeconds(passedCycles * cycleDuration.TimeSpan().TotalSeconds);

            cycleEndDate = cycleStartDate.Add(cycleDuration.TimeSpan());

            OnNewCycleStarted?.Invoke();
        }
    }
}