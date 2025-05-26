#if TAFRA_MOBILE_NOTIFICATIONS
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.MobileNotifications
{
    /// <summary>
    /// This class's job is to send monobehaviour messages to the notifications manager (OnApplicationPaused and the likes).
    /// </summary>
    public class NotificationsMonoBridge : MonoBehaviour
    {
        #region Private Fields
        public Action<bool> OnApplicationFocusAction;
        #endregion

        #region MonoBehaviour Messages
        private void OnApplicationFocus(bool focus)
        {
            OnApplicationFocusAction?.Invoke(focus);
        }
        #endregion
    }
}
#endif