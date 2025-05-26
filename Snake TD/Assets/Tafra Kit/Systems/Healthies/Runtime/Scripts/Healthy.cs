using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TafraKit.Internal.Healthies;
using TafraKit.ModularSystem;

namespace TafraKit.Healthies
{
    public class Healthy : InternallyModularComponent<HealthyModule>, ITargetPoint, IResettable
    {
        [SerializeField] private TafraFloat maxHealth = new TafraFloat(100);

        [Header("References")]
        [Tooltip("The point of which attackers will aim at while attacking.")]
        [SerializeField] private Transform damagePoint;

        [Header("Extra")]
        [SerializeField] private bool changeLayerOnDeath = true;
        [SerializeField] private int deathLayer = 0;
        [SerializeField] private bool listenToMaxHealthChanges = true;

        //Saving & Loading
        [Tooltip("Should the current health be saved/loaded?")]
        [SerializeField] private bool enableSaveLoad;
        [Tooltip("The ID that will be used to save the PlayerPrefs keys.")]
        [SerializeField] private string saveLoadID;

        [SerializeField] private HealthyEvents events;

        [SerializeReferenceListContainer("modules", true, "Module", "Modules")]
        [SerializeField] private HealthyModulesContainer modulesContainer = new HealthyModulesContainer();

        private float curHealth;
        private float curMaxHealth;
        private bool isDead;
        private string currentHealthPrefsKey;
        private bool prefsKeysInitialized;
        private int originalLayer;
        private bool isInitialized;

        protected override List<HealthyModule> InternalModules => modulesContainer.Modules;
        private string CurrentHealthPrefsKey
        {
            get
            {
                if(!prefsKeysInitialized)
                    InitializePrefsKeys();

                return currentHealthPrefsKey;
            }
        }

        public bool IsInitialized => isInitialized;
        public float CurrentHealth
        {
            get
            {
                if(!isInitialized)
                    TafraDebugger.Log("Healthy", "Healthy is not initialized, you should wait until it's initialized before attempting to get its health.", TafraDebugger.LogType.Error);

                return curHealth;
            }
            protected set
            {
                curHealth = Mathf.Clamp(value, 0, curMaxHealth);

                if(enableSaveLoad)
                    PlayerPrefs.SetFloat(CurrentHealthPrefsKey, value);
            }
        }
        public float CurrentMaxHealth
        {
            get
            {
                if(!isInitialized)
                    TafraDebugger.Log("Healthy", "Healthy is not initialized, you should wait until it's initialized before attempting to get its max health.", TafraDebugger.LogType.Error);

                return curMaxHealth;
            }
        }
        public float NormalizedHealth
        {
            get
            {
                if(!isInitialized)
                    TafraDebugger.Log("Healthy", "Healthy is not initialized, you should wait until it's initialized before attempting to get its normalized health.", TafraDebugger.LogType.Error);

                return curHealth / curMaxHealth;
            }
        }
        public bool IsDead
        {
            get
            {
                if(!isInitialized)
                    TafraDebugger.Log("Healthy", "Healthy is not initialized, you should wait until it's initialized before attempting to check if its dead.", TafraDebugger.LogType.Error);

                return isDead;
            }
        }
        public Transform TargetPoint => damagePoint ? damagePoint : transform;
        public HealthyEvents Events => events;

        protected override void Awake()
        {
            base.Awake();

            originalLayer = gameObject.layer;

            curMaxHealth = maxHealth.Value;

            //This is only added to make sure that the curHealth isn't 0 in the first frame to avoid the healthbar looking like it's empty in the first frame.
            //It will get replaced with the actual value in start anyway.
            curHealth = curMaxHealth;
            
            maxHealth.LoadAsset();

            for(int i = 0; i < modulesCount; i++)
            {
                var module = allModules[i];

                if (module == null) 
                    continue;
                
                module.Initialize(this);
            }
        }
        protected override void Start()
        {
            base.Start();

            StartCoroutine(LateInitialize());
        }
        protected virtual IEnumerator LateInitialize()
        {
            //To make sure whoever is setting the max health on load gets to set it before this function invokes.
            yield return Yielders.EndOfFrame;

            //Inform the user if they forgot to set an ID while saving and loading are enabled.
            if(enableSaveLoad && string.IsNullOrEmpty(saveLoadID))
                TafraDebugger.Log("Healthy", $"Saving and loading was enabled on game object \"{name}\" but no ID was set. Please set an ID or disable saving and loading.", TafraDebugger.LogType.Error, gameObject);

            if(!listenToMaxHealthChanges)
                curMaxHealth = maxHealth.Value;

            CurrentHealth = enableSaveLoad ? PlayerPrefs.GetFloat(CurrentHealthPrefsKey, curMaxHealth) : curMaxHealth;

            if(curHealth <= 0)
                Die(new HitInfo());

            isInitialized = true;

            events.OnInitialize?.Invoke();

            if(events.EnableOnHealthChangeEvent)
                events.OnHealthChange?.Invoke(this, curHealth);
        }
        protected override void Update()
        {
            base.Update();
        }
        protected override void OnEnable()
        {
            base.OnEnable();

            if (maxHealth.ScriptableVariable != null && listenToMaxHealthChanges)
                maxHealth.ScriptableVariable.OnValueChange.AddListener(SetMaxHealth);
        }
        protected override void OnDisable()
        {
            base.OnDisable();

            if(maxHealth.ScriptableVariable != null && listenToMaxHealthChanges)
                maxHealth.ScriptableVariable.OnValueChange.RemoveListener(SetMaxHealth);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

            maxHealth.UnloadAsset();
        }
        #region Private Functions
        private void InitializePrefsKeys()
        {
            currentHealthPrefsKey = $"{saveLoadID}_HEALTHY_CURRENT_HEALTH";
            prefsKeysInitialized = true;
        }
        private IEnumerator HealingOverTime(float amountToHeal, float duration)
        {
            float remainingTime = duration;

            float healAmount = 0;

            while(remainingTime > 0)
            {
                float delteTime = Time.deltaTime;

                if(delteTime < remainingTime)
                    remainingTime -= delteTime;
                else
                {
                    delteTime = remainingTime;
                    remainingTime = 0;
                }

                float frameTimePercentage = delteTime / duration;
                float frameHeal = frameTimePercentage * amountToHeal;

                healAmount += frameHeal;

                if(healAmount >= 1)
                {
                    int intHealAmount = Mathf.FloorToInt(healAmount);

                    healAmount -= intHealAmount;

                    Heal(intHealAmount);
                }

                yield return null;
            }

            healAmount = Mathf.Round(healAmount);

            if(healAmount > 0)
                Heal(healAmount);
        }
        private void Die(HitInfo killerHit)
        {
            if(isDead)
                return;

            if(events.EnableOnAboutToDieEvent)
            {
                AboutToDieEventArgs evArgs = new AboutToDieEventArgs();
                events.OnAboutToDie?.Invoke(this, evArgs);

                if(evArgs.PreventDeath)
                    return;
            }

            isDead = true;

            if(changeLayerOnDeath)
                gameObject.layer = deathLayer;

            if(events.EnableOnDeathEvent)
                events.OnDeath?.Invoke(this, killerHit);

            for (int i = 0; i < modulesCount; i++)
            {
                var module = modulesContainer.Modules[i];

                if(module.DisableOnDeath)
                    module.Disable();
            }
        }
        #endregion

        #region Public Functions
        public void TakeDamage(HitInfo hitInfo)
        {
            if(isDead)
                return;

            //Broadcast an event to tell whoever is listening that this healthy is about to take damage. They can call InterceptAboutToBeTakenDamage, to manipulat the damage received.
            if(events.EnableAboutToTakeDamageEvent)
            {
                HitEventArgs args = new HitEventArgs(hitInfo);

                events.OnAboutToTakeDamage?.Invoke(this, args);
                hitInfo = args.ManipulatedHitInfo;
            }

            //In case listeners removed the damage entirely.
            if(hitInfo.damage <= 0)
                return;

            if(hitInfo.isMissed)
            {
                events.OnTakenDamage?.Invoke(this, hitInfo);
                return;
            }

            CurrentHealth = Mathf.Max(CurrentHealth - hitInfo.damage, 0);

            if(events.EnableOnTakenDamageEvent)
                events.OnTakenDamage?.Invoke(this, hitInfo);

            if(events.EnableOnHealthChangeEvent)
                events.OnHealthChange?.Invoke(this, curHealth);

            if(CurrentHealth <= 0)
                Die(hitInfo);
        }
        public void Heal(float healingAmount)
        {
            if(isDead)
                return;

            HealEventArgs healEventArgs = new HealEventArgs(healingAmount, CurrentHealth);

            if (events.EnableOnAboutToHealEvent)
                events.OnAboutToHeal?.Invoke(this, healEventArgs);

            healingAmount = healEventArgs.ManipulatedHeal;

            if(healingAmount == 0)
                return;

            float appliedHeal = Mathf.Min(CurrentMaxHealth - CurrentHealth, healingAmount);
            CurrentHealth = Mathf.Min(CurrentHealth + healingAmount, curMaxHealth);

            if(events.EnableOnHealEvent)
                events.OnHeal?.Invoke(this, appliedHeal);

            if(events.EnableOnHealthChangeEvent)
                events.OnHealthChange?.Invoke(this, curHealth);
        }
        public void HealOverTime(float healingAmount, float duration)
        {
            StartCoroutine(HealingOverTime(healingAmount, duration));
        }
        /// <summary>
        /// Increases the maximum health of this healthy. Note: If you're applying loaded objects that increase max health 
        /// (e.g. a gear that has health stats) make sure to do it before this object's Start() funciton.
        /// </summary>
        /// <param name="newMaxHealth"></param>
        public void SetMaxHealth(float newMaxHealth)
        {
            float addedMaxHealth = newMaxHealth - curMaxHealth;
            
            curMaxHealth = newMaxHealth;

            if(!isInitialized)
                return;

            //If the healthy isn't initialized (which means saved data aren't yet loaded) then we don't need to adjust its current health.
            if(isInitialized && addedMaxHealth > 0)
                CurrentHealth = Mathf.Clamp(curHealth + addedMaxHealth, 0, curMaxHealth);

            if (curHealth > curMaxHealth)
                CurrentHealth = curMaxHealth;

            if(events.EnableOnMaxHealthChangeEvent)
                events.OnMaxHealthChange?.Invoke(newMaxHealth);

            if(events.EnableOnHealthChangeEvent)
                events.OnHealthChange?.Invoke(this, curHealth);
        }
        /// <summary>
        /// Revive this healthy with the desired amount of health (if "reviveHealth" is 0 or less, healthy will revive at full hp)
        /// </summary>
        /// <param name="reviveHealth"></param>
        public void Revive(float reviveHealth = 0)
        {
            if(!gameObject.activeInHierarchy)
            {
                TafraDebugger.Log("Healthy", "Healthy game object is disabled, enabled it before reviving.", TafraDebugger.LogType.Error, gameObject);
                return;
            }

            if(!isDead)
                return;

            if(changeLayerOnDeath)
                gameObject.layer = originalLayer;

            //Enable the modules that were disabled on death.
            for(int i = 0; i < modulesCount; i++)
            {
                var module = modulesContainer.Modules[i];

                if(module.DisableOnDeath)
                    module.Enable();
            }

            if(reviveHealth <= 0)
                CurrentHealth = curMaxHealth;
            else
            {
                float reviveHealthValue = curMaxHealth * reviveHealth;

                if (reviveHealthValue > curMaxHealth)
                    reviveHealthValue = curMaxHealth;

                CurrentHealth = reviveHealthValue;
            }

            isDead = false;

            if(events.EnableOnReviveEvent)
                events.OnRevive?.Invoke();

            if(events.EnableOnHealthChangeEvent)
                events.OnHealthChange?.Invoke(this, curHealth);
        }
        public void ResetSavedData()
        {
            CurrentHealth = curMaxHealth;
        }
        #endregion
    }
}