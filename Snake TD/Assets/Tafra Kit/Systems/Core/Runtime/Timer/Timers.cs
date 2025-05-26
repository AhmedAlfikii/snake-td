using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    public static class Timers
    {
        private static Dictionary<string, ITimer> timersById = new Dictionary<string, ITimer>();

        public static void RegisterTimer(ITimer timer)
        {
            timersById.TryAdd(timer.ID, timer);
        }

        public static void TryGetTimer(string timerID, out ITimer timer)
        {
            timersById.TryGetValue(timerID, out timer);
        }
    }
}