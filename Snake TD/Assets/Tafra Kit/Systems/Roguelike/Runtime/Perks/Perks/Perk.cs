using System;
using TafraKit.Conditions;
using UnityEngine;
using TafraKit.Internal.Roguelike;
using TafraKit.UI;
using System.Collections.Generic;

namespace TafraKit.Roguelike
{
    public abstract class Perk : PerkBase
    {
        [Header("Unlocking")]
        [SerializeField] protected bool unlockedByDefault;
        [Tooltip("The conditions that must be met for this perk to be unlocked if it's not unlocked by default.")]
        [SerializeField] protected ConditionsGroup canBeUnlockedConditions;

        [Header("Run Availability")]
        [SerializeField] protected ConditionsGroup availabilityConditions;
        [Tooltip("If true, this perk can have a chance to always appear to players, no matter how many times it was applied.")]
        [SerializeField] protected bool unlimitedApplies;
        [Tooltip("If unlimited applies isn't enabled, how many times can players select and apply this perk?")]
        [SerializeField] protected int maxAppliesCount = 1;

        [Header("Skin")]
        [SerializeField] private List<UISkin> skinPerLevel = new List<UISkin>();

        [NonSerialized] protected bool isUnlocked;
        /// <summary>
        /// Whether or not this perk is currently applied on the player.
        /// </summary>
        [NonSerialized] protected bool isApplied;
        /// <summary>
        /// Holds the value indicating how many times the player has chosen to apply this perk.
        /// </summary>
        [NonSerialized] protected int appliesCount;
        [NonSerialized] protected bool iconsLoaded;
        [NonSerialized] protected int iconRequesters;
        [NonSerialized] protected string perkUnlockedSaveKey;
        [NonSerialized] protected string perkAppliesSaveKey;

        public bool UnlimitedApplies => unlimitedApplies;
        public int MaxAppliesCount => maxAppliesCount;
        public int AppliesCount => appliesCount;
        public bool IsApplied => appliesCount > 0;
        public UISkin OfferSkin
        {
            get
            {
                int index = appliesCount;

                int count = skinPerLevel.Count;

                if(index >= count)
                    index = count - 1;

                return skinPerLevel[index];
            }
        }
        public UISkin AppliedSkin
        {
            get
            {
                int index = appliesCount - 1;

                int count = skinPerLevel.Count;

                if(index >= count)
                    index = count - 1;

                return skinPerLevel[index];
            }
        }
        /// <summary>
        /// The display name to show to players when this perk is offered to them (use AppliedDisplayName for the display name of the already applied perk).
        /// </summary>
        public abstract string OfferDisplayName { get; }
        /// <summary>
        /// The description to show to players when this perk is offered to them (use AppliedDescription for the description of the already applied perk).
        /// </summary>
        public abstract string OfferDescription { get; }
        /// <summary>
        /// The display name to show to players when they are checking the perks they have.
        /// </summary>
        public abstract string AppliedDisplayName { get; }
        /// <summary>
        /// The description to show to players when they are checking the perks they have.
        /// </summary>
        public abstract string AppliedDescription { get; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            if(string.IsNullOrEmpty(id))
                TafraDebugger.Log("Perk", "Perk ID can't be empty.", TafraDebugger.LogType.Error, this);

            perkUnlockedSaveKey = $"PERK_UNLOCKED_{id}";
            perkAppliesSaveKey = $"PERK_APPLIES_COUNT_{id}";

            Load();
        }
        protected virtual void OnDestroy()
        {
            if(iconsLoaded)
                ReleaseIcons();
        }
        private void Load()
        {
            isUnlocked = unlockedByDefault || TafraSaveSystem.LoadInt(perkUnlockedSaveKey) == 1;
            appliesCount = TafraSaveSystem.LoadInt(perkAppliesSaveKey, 0);

            OnLoad();
        }
        protected override void OnResetSavedData()
        {
            isApplied = false;

            appliesCount = 0;

            TafraSaveSystem.DeleteKey(perkAppliesSaveKey);
        }

        /// <summary>
        /// Use this to apply the perk for the first time of this "apply" number (e.g. once the player selects the perk card in the perk selection screen).
        /// </summary>
        public void Apply()
        {
            isApplied = true;
            
            appliesCount++;
            TafraSaveSystem.SaveInt(perkAppliesSaveKey, appliesCount);

            OnApplied();
        }
        /// <summary>
        /// Apply the perk without counting it as a usage or add an active copy. Use this when activating a loaded perk (previously used in a past game session).
        /// </summary>
        public void PhantomApply()
        {
            isApplied = true;

            OnApplied();
        }
        /// <summary>
        /// Let the perk know that the a new scene is loaded in case it needs to re-apply some of its stuff 
        /// (for example, a perk that needs to re-add an ability to the player's combat component, since it's a fresh one)
        /// </summary>
        public void SceneLoaded()
        {
            OnSceneLoad();
        }
        /// <summary>
        /// The icon to show to players when this perk is offered to them (use AppliedDescription for the description of the already applied perk) - Call ReleaseIcons() when you no longer need the icon.
        /// </summary>
        public abstract Sprite GetLoadedOfferIcon();
        /// <summary>
        /// The icon to show to players when they are checking the perks they have - Call ReleaseIcons() when you no longer need the icon.
        /// </summary>
        public abstract Sprite GetLoadedAppliedIcon();
        public virtual void LoadIcons()
        {
            iconRequesters++;

            iconsLoaded = true;
        }
        public virtual void ReleaseIcons()
        {
            iconRequesters--;

            iconsLoaded = false;
        }
        public bool CanBeOffered()
        {
            if(!isUnlocked)
                return false;

            if(!unlimitedApplies && appliesCount >= maxAppliesCount)
                return false;

            availabilityConditions.Activate(true);
            bool conditionsSatisfied =  availabilityConditions.WasSatisfied();
            availabilityConditions.Deactivate();

            return conditionsSatisfied;
        }
        public bool CanBeUnlocked()
        {
            if(isUnlocked)
                return true;

            canBeUnlockedConditions.Activate(true);
            bool conditionsSatisfied = canBeUnlockedConditions.WasSatisfied();
            canBeUnlockedConditions.Deactivate();

            return conditionsSatisfied;
        }

        protected virtual void OnLoad() { }
        protected abstract void OnApplied();
        /// <summary>
        /// Gets called when a new scene is loaded in case the perk needs to re-apply some of its stuff 
        /// (for example, a perk that needs to re-add an ability to the player's combat component, since it's a fresh one)
        /// </summary>
        protected abstract void OnSceneLoad();
    }
}