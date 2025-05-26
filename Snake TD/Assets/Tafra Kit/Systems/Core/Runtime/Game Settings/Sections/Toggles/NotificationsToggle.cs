using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.MobileNotifications;

namespace TafraKit.Internal
{
    public class NotificationsToggle : GameSettingsToggle
    {
        #if TAFRA_MOBILE_NOTIFICATIONS
        protected override bool IsManagerOn => !NotificationsManager.IsMuted;
        #else
        protected override bool IsManagerOn => false;
        #endif

        public override bool AreConditionsSatisfied()
        {
            #if TAFRA_MOBILE_NOTIFICATIONS
            return NotificationsManager.IsEnabled;
            #else
            return false;
            #endif
        }

        protected override void OnValueChange(bool on)
        {
            #if TAFRA_MOBILE_NOTIFICATIONS
            NotificationsManager.IsMuted = !on;
            #endif
        }
    }
}