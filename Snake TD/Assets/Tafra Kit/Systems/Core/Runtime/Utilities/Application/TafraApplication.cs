using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public static class TafraApplication
    {
        public static RuntimePlatform Platform
        {
            get
            {
                //#if UNITY_EDITOR
                //#if UNITY_ANDROID
                //return RuntimePlatform.Android;
                //#elif UNITY_IOS
                //return RuntimePlatform.IPhonePlayer;
                //#endif
                //#endif

                return Application.platform;
            }
        }
    }
}