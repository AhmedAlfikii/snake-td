using TafraKit.Healthies;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit
{
    public static class HealthiesTracker
    {
        private static UnityEvent<Healthy, HitInfo> onHealthyDeath = new UnityEvent<Healthy, HitInfo>();

        public static UnityEvent<Healthy, HitInfo> OnHealthyDeath => onHealthyDeath;

        public static void SignalHealthyDeath(Healthy healthy, HitInfo killHit)
        {
            onHealthyDeath?.Invoke(healthy, killHit);
        }
    }
}