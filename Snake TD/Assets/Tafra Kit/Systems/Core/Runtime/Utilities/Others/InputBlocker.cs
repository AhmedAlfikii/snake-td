using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    /// <summary>
    /// TODO: Actully block things.
    /// </summary>
    public static class InputBlocker
    {
        private static ControlReceiver touchBlockers = new ControlReceiver(OnFirstTouchBlockerAdded, null, OnAllTouchBlockersCleared);

        private static bool isTouchBlocked;

        public static bool IsTouchBlocked => isTouchBlocked;

        private static void OnFirstTouchBlockerAdded()
        {
            isTouchBlocked = true;
        }

        private static void OnAllTouchBlockersCleared()
        {
            isTouchBlocked = false;
        }

        public static void BlockTouches(string blockerID)
        {
            touchBlockers.AddController(blockerID);
            Debug.LogError("Handle blocking");
        }
        public static void UnblockTouches(string blockerID)
        {
            touchBlockers.RemoveController(blockerID);
        }
    }
}