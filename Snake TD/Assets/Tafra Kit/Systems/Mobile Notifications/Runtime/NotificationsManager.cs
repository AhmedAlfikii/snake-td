#if TAFRA_MOBILE_NOTIFICATIONS
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

namespace TafraKit.MobileNotifications
{
    public static class NotificationsManager
    {
        private static MobileNotificationsSettings settings;
        private static bool isEnabled;

        private static bool muteNotifications;
        private static int closingNotificationsStartID = 10000;
        private static List<Notification> extraClosingNotifications = new List<Notification>();
        private static Dictionary<Notification, int> extraClosingNotificationsIDs = new Dictionary<Notification, int>();

        public static bool IsEnabled => isEnabled;
        public static bool IsMuted 
        { 
            get 
            { 
                return muteNotifications; 
            } 
            set
            {
                muteNotifications = value;
                PlayerPrefs.SetInt("TAFRAKIT_NOTIFICATIONS_ISMUTED", muteNotifications? 1 : 0);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            settings = TafraSettings.GetSettings<MobileNotificationsSettings>();

            if (settings)
            {
                if (settings.Enabled)
                {
                    isEnabled = true;

                    GameObject monoBridge = new GameObject("Notifications Bridge", typeof(NotificationsMonoBridge));
                    
                    monoBridge.GetComponent<NotificationsMonoBridge>().OnApplicationFocusAction += OnApplicationFocus;

                    GameObject.DontDestroyOnLoad(monoBridge);

                    #if UNITY_ANDROID
                    for (int i = 0; i < settings.Channels.Length; i++)
                    {
                        NotificationChannelAndroid andChannel = settings.Channels[i];

                        AndroidNotificationChannel channel = new AndroidNotificationChannel(andChannel.Id, andChannel.Name, andChannel.Description, Importance.Default);

                        AndroidNotificationCenter.RegisterNotificationChannel(channel);
                    }
                    #endif

                    muteNotifications = PlayerPrefs.GetInt("TAFRAKIT_NOTIFICATIONS_ISMUTED", settings.MutedByDefault? 1 : 0) == 1 ? true : false;

                    if(settings.AutoAskForPermission)
                    {
                        #if UNITY_ANDROID
                        if(!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
                        {
                            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
                        }
                        #endif
                    }
                }
            }
        }

        private static void OnApplicationFocus(bool inFocus)
        {
            #if UNITY_EDITOR
            return;
            #endif

            if (inFocus)
            {
                #if UNITY_IOS
                iOSNotification[] scheduledIOSNotifications = iOSNotificationCenter.GetScheduledNotifications();
                for (int j = 0; j < scheduledIOSNotifications.Length; j++)
                {
                    int iOSNotifID = int.Parse(scheduledIOSNotifications[j].Identifier);

                    //If this scheduled notification's id proves that it's one of the closing notifications, then cancel it.
                    if (iOSNotifID >= closingNotificationsStartID)
                    {
                        iOSNotificationCenter.RemoveScheduledNotification(iOSNotifID.ToString());

                        TafraDebugger.Log("Notifications Manager", $"Found a closing notification that is scheduled ({scheduledIOSNotifications[j].Title}) and canceled it.", TafraDebugger.LogType.Info);
                    }
                }
                #endif

                #if UNITY_ANDROID
                for(int i = 0; i < settings.ClosingNotifications.Length; i++)
                {
                    int notifID = closingNotificationsStartID + i;

                    NotificationStatus status = AndroidNotificationCenter.CheckScheduledNotificationStatus(notifID);

                    if (status == NotificationStatus.Scheduled)
                    {
                        AndroidNotificationCenter.CancelScheduledNotification(notifID);
                        
                        TafraDebugger.Log("Notifications Manager", $"Found a closing notification that is scheduled ({settings.ClosingNotifications[i].Title}) and canceled it.", TafraDebugger.LogType.Info);
                    }
                }
                for(int i = 0; i < extraClosingNotifications.Count; i++)
                {
                    int notifID = extraClosingNotificationsIDs[extraClosingNotifications[i]];

                    NotificationStatus status = AndroidNotificationCenter.CheckScheduledNotificationStatus(notifID);

                    if (status == NotificationStatus.Scheduled)
                    {
                        AndroidNotificationCenter.CancelScheduledNotification(notifID);
                        
                        TafraDebugger.Log("Notifications Manager", $"Found an extra closing notification that is scheduled ({extraClosingNotifications[i].Title}) and canceled it.", TafraDebugger.LogType.Info);
                    }
                }
                #endif
            }
            else if (!muteNotifications)
            {
                for(int i = 0; i < settings.ClosingNotifications.Length; i++)
                {
                    int notifID = closingNotificationsStartID + i;

                    ScheduleNotification(settings.ClosingNotifications[i], notifID);

                    TafraDebugger.Log("Notifications Manager", $"Scheduled a closing notification ({settings.ClosingNotifications[i].Title})", TafraDebugger.LogType.Info);
                }
                for (int i = 0; i < extraClosingNotifications.Count; i++)
                {
                    int notifID = extraClosingNotificationsIDs[extraClosingNotifications[i]];

                    ScheduleNotification(extraClosingNotifications[i], notifID);

                    TafraDebugger.Log("Notifications Manager", $"Scheduled an extra closing notification ({extraClosingNotifications[i].Title})", TafraDebugger.LogType.Info);
                }
            }
        }

        public static int ScheduleNotification(Notification notification, int? customNotificationID = null)
        {
            #if UNITY_ANDROID
            AndroidNotification androidNotification = new AndroidNotification();

            androidNotification.Title = notification.Title;
            androidNotification.Text = notification.Body;
            androidNotification.FireTime = DateTime.Now.Add(notification.DeliverAfter.TimeSpan());
            androidNotification.SmallIcon = notification.SmallIcon;
            androidNotification.LargeIcon = notification.LargeIcon;
            if (notification.SetColor)
                androidNotification.Color = notification.Color;
            if (notification.Repeat)
                androidNotification.RepeatInterval = notification.RepeatInterval.TimeSpan();
            androidNotification.IntentData = notification.IntentData;
            switch (notification.TimeStyle)
            {
                case NotificationTimeStyle.TimeStamp:
                    androidNotification.ShowTimestamp = true;
                    break;
                case NotificationTimeStyle.Stopwatch:
                    androidNotification.UsesStopwatch = true;
                    break;
            }
            
            if (customNotificationID == null)
                return AndroidNotificationCenter.SendNotification(androidNotification, notification.Channel);
            else
                AndroidNotificationCenter.SendNotificationWithExplicitID(androidNotification, notification.Channel, customNotificationID.Value);
            #endif

            #if UNITY_IOS
            iOSNotification iosNotification = new iOSNotification();

            iOSNotificationTimeIntervalTrigger timeTrigger = new iOSNotificationTimeIntervalTrigger()
            {
                TimeInterval = notification.DeliverAfter.TimeSpan(),
                Repeats = false
            };

            iosNotification.Title = notification.Title;
            iosNotification.Body = notification.Body;
            iosNotification.Subtitle = notification.Subtitle;
            iosNotification.Data = notification.IntentData;
            iosNotification.CategoryIdentifier = notification.Channel;
            iosNotification.ThreadIdentifier = notification.Channel;
            iosNotification.Trigger = timeTrigger;

            iosNotification.ShowInForeground = false;

            if (customNotificationID != null)
                iosNotification.Identifier = customNotificationID.Value.ToString();

            iOSNotificationCenter.ScheduleNotification(iosNotification);
                #endif

            return -1;
        }
        public static void UnscheduleNotification(Notification notification, int notificationID)
        {
            #if UNITY_IOS
            iOSNotificationCenter.RemoveScheduledNotification(notificationID.ToString());
            TafraDebugger.Log("Notifications Manager", $"Unscheduled a notification with the ID ({notification.Title}).", TafraDebugger.LogType.Verbose);
            #endif

            #if UNITY_ANDROID
            NotificationStatus status = AndroidNotificationCenter.CheckScheduledNotificationStatus(notificationID);

            if(status == NotificationStatus.Scheduled)
            {
                AndroidNotificationCenter.CancelScheduledNotification(notificationID);

                TafraDebugger.Log("Notifications Manager", $"Unscheduled a notification with the ID: {notificationID}. Title: {notification.Title}.", TafraDebugger.LogType.Info);
            }
            #endif
        }
        /// <summary>
        /// Schedule a notification that would be registered once the game is unfocused.
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="customNotificationID">The ID of this notification (can start at 0, and the final ID will be closingNotificationsStartID + settings.ClosingNotifications.Length + 1).</param>
        public static void ScheduleClosingNotification(Notification notification, int customNotificationID)
        {
            int id = customNotificationID + closingNotificationsStartID + settings.ClosingNotifications.Length + 1;
            extraClosingNotifications.Add(notification);
            extraClosingNotificationsIDs.Add(notification, id);

            TafraDebugger.Log("Notifications Manager", $"Registered an extra closing notification with the ID: {customNotificationID}. Title: {notification.Title}.", TafraDebugger.LogType.Info);
        }
        /// <summary>
        /// Remove the scheduled closing notification.
        /// </summary>
        /// <param name="notification"></param>
        public static void UnscheduleClosingNotification(Notification notification)
        {
            int index = extraClosingNotifications.IndexOf(notification);

            if (index > -1)
                extraClosingNotifications.RemoveAt(index);

            if(extraClosingNotificationsIDs.ContainsKey(notification))
                extraClosingNotificationsIDs.Remove(notification);

            TafraDebugger.Log("Notifications Manager", $"Anregistered an extra closing notification with the Title: {notification.Title}.", TafraDebugger.LogType.Info);
        }
    }
}
#endif