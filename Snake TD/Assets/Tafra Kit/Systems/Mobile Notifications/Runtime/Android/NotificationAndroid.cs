#if TAFRA_MOBILE_NOTIFICATIONS
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit;

namespace TafraKit.MobileNotifications
{
    public enum NotificationTimeStyle { None, TimeStamp, Stopwatch }
    [Serializable]
    public class Notification 
    {
        public string Title;
        public string Body;
        public string Subtitle;
        public string Channel;
        public TimeSpanSimple DeliverAfter;
        public string SmallIcon;
        public string LargeIcon;
        public NotificationTimeStyle TimeStyle;
        public bool SetColor;
        public Color Color;
        public bool Repeat;
        public TimeSpanSimple RepeatInterval;
        public string IntentData;
    }
}
#endif