using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if TAFRA_MOBILE_NOTIFICATIONS
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif
#endif

namespace TafraKit.MobileNotifications
{
    public class MobileNotificationsSettings : SettingsModule
    {
        public bool Enabled;
        public bool MutedByDefault;
        public bool AutoAskForPermission = true;

        #if TAFRA_MOBILE_NOTIFICATIONS
        public NotificationChannelAndroid[] Channels;
        public Notification[] ClosingNotifications;
        #endif

        public override int Priority => 9;

        public override string Name => "Mobile/Mobile Notifications";

        public override string Description => "Handles mobile notifications through Unity's Mobile Notifications package. Make sure to install it.";
    }
}
