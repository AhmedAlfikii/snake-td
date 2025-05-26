using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Conditions
{
    public class EventListenerCondition : Condition
    {
        [SerializeField] private ListenToEvent trackEvent;
        
        protected override void OnInitialize()
        {
            trackEvent.Initialize(OnTrackedEventFired);
        }
        protected override void OnActivate()
        {
            trackEvent.StartListening();
        }
        protected override void OnDeactivate()
        {
            trackEvent.StopListening();
        }

        private void OnTrackedEventFired()
        {
            Satisfy();
        }

        protected override bool PerformCheck()
        {
            return isSatisfied;
        }
    }
}