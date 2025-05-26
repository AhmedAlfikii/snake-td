using TafraKit.Healthies;
using UnityEngine.Events;

namespace TafraKit.Internal.Healthies
{
    [System.Serializable]
    public class HealthyEvents 
    {
        public bool EnableOnInitializeEvent = true;
        public UnityEvent OnInitialize;
        public bool EnableOnAboutToDieEvent = true;
        public UnityEvent<Healthy, AboutToDieEventArgs> OnAboutToDie;
        public bool EnableOnDeathEvent = true;
        public UnityEvent<Healthy, HitInfo> OnDeath;
        public bool EnableAboutToTakeDamageEvent = true;
        /// <summary>
        /// Fires before the received hit is applied, listeners can make adjustments to that hit before it applies through manipulating HitEventArgs.HitInfo.
        /// </summary>
        public UnityEvent<Healthy, HitEventArgs> OnAboutToTakeDamage;
        public bool EnableOnTakenDamageEvent = true;
        public UnityEvent<Healthy, HitInfo> OnTakenDamage;
        public bool EnableOnAboutToHealEvent = true;
        public UnityEvent<Healthy, HealEventArgs> OnAboutToHeal;
        public bool EnableOnHealEvent = true;
        public UnityEvent<Healthy, float> OnHeal;
        public bool EnableOnHealthChangeEvent = true;
        public UnityEvent<Healthy, float> OnHealthChange;
        public bool EnableOnMaxHealthChangeEvent = true;
        public UnityEvent<float> OnMaxHealthChange;
        public bool EnableOnReviveEvent = true;
        public UnityEvent OnRevive;
    }
}