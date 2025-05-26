using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.ModularSystem;
using UnityEngine;
using UnityEngine.UI;

namespace TafraKit.Healthies
{
    public class HealthBar : InternallyModularComponent<HealthBarModule>
    {
        [SerializeField] protected Healthy healthy;
        [SerializeField] private Slider bar;

        [Header("Bar Update Animation")]
        [SerializeField] private float damageTakenUpdateDuration;
        [SerializeField] private float healUpdateDuration = 0.5f;
        [SerializeField] private float maxHealthUpdateDuration = 1f;
        [SerializeField] private float reviveUpdateDuration = 1f;

        [SerializeReferenceListContainer("modules", true, "Module", "Modules")]
        [SerializeField] private HealthBarModulesContainer healthBarModules;

        private bool waitingHealthyInitialization;
        private float displayedHealth;
        private float displayedMaxHealth;
        private IEnumerator updatingEnum;

        protected override List<HealthBarModule> InternalModules => healthBarModules.Modules;

        public Healthy Healthy
        {
            get
            {
                return healthy;
            }
            set
            {
                //If we are alread hooked to this healthy, no need to do anything.
                if(value == healthy)
                    return;

                //If we're hooked to a different healthy, unhook from it.
                if(healthy != null)
                    UnhookFromHealthy(healthy);

                healthy = value;

                HookToHealthy(healthy);
            }
        }
        public float DamageTakenUpdateDuration => damageTakenUpdateDuration;
        public float HealUpdateDuration => healUpdateDuration;
        public float MaxHealthUpdateDuration => maxHealthUpdateDuration;
        public float ReviveUpdateDuration => reviveUpdateDuration;

        public event Action<float, float> HealthUpdated;

        protected override void Awake()
        {
            base.Awake();

            for (int i = 0; i < healthBarModules.Modules.Count; i++)
            {
                healthBarModules.Modules[i].Initialize(this);
            }
        }
        protected override void OnEnable()
        {
            base.OnEnable();

            if(healthy == null)
                return;

            HookToHealthy(healthy);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
           
            if(healthy == null)
                return;

            UnhookFromHealthy(healthy);
        }

        #region Callbacks
        private void OnHealthyInitialize()
        {
            healthy.Events.OnInitialize.RemoveListener(OnHealthyInitialize);
            
            waitingHealthyInitialization = false;

            UpdateBar();
        }
        private void OnTakenDamage(Healthy healthy, HitInfo hitInfo)
        {
            UpdateBar(damageTakenUpdateDuration);
        }
        private void OnHeal(Healthy healthy, float healing)
        {
            UpdateBar(healUpdateDuration);
        }
        private void OnMaxHealthChange(float newMaxHealth)
        {
            if (!healthy.IsInitialized)
                return;

            UpdateBar(maxHealthUpdateDuration);
        }
        private void OnRevive()
        {
            UpdateBar(reviveUpdateDuration);
        }
        #endregion

        private void UpdateBar(float duration = 0)
        {
            //If an update is already in progress, terminate it.
            if (updatingEnum != null)
            {
                StopCoroutine(updatingEnum);
                updatingEnum = null;
            }

            float curHealth = healthy.CurrentHealth;
            float curMaxHealth = healthy.CurrentMaxHealth;

            if (duration < 0.001f)
            {
                displayedHealth = curHealth;
                displayedMaxHealth = curMaxHealth;

                bar.value = displayedHealth / displayedMaxHealth;
                HealthUpdated?.Invoke(displayedHealth, displayedMaxHealth);
            }
            else
            { 
                float startHealth = displayedHealth;
                float targetHealth = healthy.CurrentHealth;
                float startMaxHealth = displayedMaxHealth;
                float targetMaxHealth = healthy.CurrentMaxHealth;

                updatingEnum = CompactCouroutines.CompactCoroutine(0, duration, false, (t) =>
                {
                    displayedHealth = Mathf.LerpUnclamped(startHealth, targetHealth, t);
                    displayedMaxHealth = Mathf.LerpUnclamped(startMaxHealth, targetMaxHealth, t);

                    bar.value = displayedHealth / displayedMaxHealth;
                    HealthUpdated?.Invoke(displayedHealth, displayedMaxHealth);
                });

                StartCoroutine(updatingEnum);
            }
        }
        private void HookToHealthy(Healthy healthy)
        {
            if(healthy.IsInitialized)
            {
                UpdateBar();
                waitingHealthyInitialization = false;
            }
            else
            {
                waitingHealthyInitialization = true;
                healthy.Events.OnInitialize.AddListener(OnHealthyInitialize);
            }

            healthy.Events.OnTakenDamage.AddListener(OnTakenDamage);
            healthy.Events.OnHeal.AddListener(OnHeal);
            healthy.Events.OnMaxHealthChange.AddListener(OnMaxHealthChange);
            healthy.Events.OnRevive.AddListener(OnRevive);
        }
        private void UnhookFromHealthy(Healthy healthy)
        {
            if (waitingHealthyInitialization)
                healthy.Events.OnInitialize.RemoveListener(OnHealthyInitialize);

            healthy.Events.OnTakenDamage.RemoveListener(OnTakenDamage);
            healthy.Events.OnHeal.RemoveListener(OnHeal);
            healthy.Events.OnMaxHealthChange.RemoveListener(OnMaxHealthChange);
            healthy.Events.OnRevive.RemoveListener(OnRevive);

            waitingHealthyInitialization = false;
        }
    }
}