#if TAFRA_MOBILE_NOTIFICATIONS
using System;

namespace TafraKit.MobileNotifications
{
    [Serializable]
    public struct NotificationChannelAndroid
    {
        public string Id;
        public string Name;
        public string Description;
    }
}
#endif