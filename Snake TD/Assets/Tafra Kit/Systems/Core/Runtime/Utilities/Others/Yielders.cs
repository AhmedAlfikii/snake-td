using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TafraKit
{
    /// <summary>
    /// This class is meant to minimize garbage collection.
    /// It is the output of this thread here: https://forum.unity.com/threads/c-coroutine-waitforseconds-garbage-collection-tip.224878/
    /// </summary>
    public static class Yielders
    {
        static Dictionary<float, WaitForSeconds> timeInterval = new Dictionary<float, WaitForSeconds>(100);
        static Dictionary<float, WaitForSecondsRealtime> timeIntervalRealtime = new Dictionary<float, WaitForSecondsRealtime>(100);

        static WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
        public static WaitForEndOfFrame EndOfFrame
        {
            get { return endOfFrame; }
        }

        static WaitForFixedUpdate _fixedUpdate = new WaitForFixedUpdate();
        public static WaitForFixedUpdate FixedUpdate
        {
            get { return _fixedUpdate; }
        }

        public static WaitForSeconds GetWaitForSeconds(float seconds)
        {
            if (!timeInterval.ContainsKey(seconds))
                timeInterval.Add(seconds, new WaitForSeconds(seconds));
            return timeInterval[seconds];
        }

        public static WaitForSecondsRealtime GetWaitForSecondsRealtime(float seconds)
        {
            //TODO: WaitForSecondsRealtime can't be cached, as they will wait for an inaccurate duration when reused, need to reset their internal timer...
            //...so to do that you'll have to create your own CustomYieldInstruction https://docs.unity3d.com/ScriptReference/CustomYieldInstruction.html.

            //if (!timeIntervalRealtime.ContainsKey(seconds))
            //    timeIntervalRealtime.Add(seconds, new WaitForSecondsRealtime(seconds));
            //return timeIntervalRealtime[seconds];

            return new WaitForSecondsRealtime(seconds);
        }
    }
}