using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit.Roguelike
{
    public abstract class PerksGroup : ScriptableObject, IResettable
    {
        [SerializeField] protected string id;

        [NonSerialized] protected bool isInitialized;
        [NonSerialized] protected bool savedOfferChecked;
        [NonSerialized] protected List<Perk> activePerksOffer = new List<Perk>();
        [NonSerialized] protected List<Perk> appliedPerks = new List<Perk>();
        [NonSerialized] protected List<Perk> tempActivePerksOffer = new List<Perk>();
        [NonSerialized] protected List<Perk> tempExcludedPerks = new List<Perk>();
        [NonSerialized] protected StringBuilder appliedPerksSB = new StringBuilder();
        [NonSerialized] protected Dictionary<string, Perk> perksById = new Dictionary<string, Perk>();
        [NonSerialized] private List<string> tempSavedOfferPerks = new List<string>();
        [NonSerialized] private StringBuilder savedOfferSB = new StringBuilder();
        /// <summary>
        /// The total number of offers that were created in this perk.
        /// </summary>
        [NonSerialized] private int totalCreatedOffersCount;

        [NonSerialized] protected string savedOfferPerksCountSaveKey;
        [NonSerialized] protected string savedOfferPerksSaveKey;
        [NonSerialized] protected string appliedPerksSaveKey;
        [NonSerialized] protected string totalOffersCountSaveKey;

        [NonSerialized] private UnityEvent<Perk> onPerkApplied = new UnityEvent<Perk>();
        [NonSerialized] private UnityEvent<Perk> onPerkReapplied = new UnityEvent<Perk>();
        [NonSerialized] private UnityEvent<Perk> onPerkAppliedFirstTimeInSession = new UnityEvent<Perk>();
        [NonSerialized] private UnityEvent<Perk> onPerkAppliedFirstTimeEver = new UnityEvent<Perk>();

        /// <summary>
        /// The total number of offers that were created in this perks group.
        /// </summary>
        public int TotalCreatedOffersCount => totalCreatedOffersCount;
        /// <summary>
        /// Gets fired whenever a perk is applied, whether it's the first time or otherwise.
        /// </summary>
        public UnityEvent<Perk> OnPerkApplied => onPerkApplied;
        /// <summary>
        /// Gets fired when a perk was applied before in this session, and gets applied again (increases its applies count).
        /// </summary>
        public UnityEvent<Perk> OnPerkReapplied => onPerkReapplied;
        /// <summary>
        /// Gets fired when a perk is applied for the first time in this session (could be a loaded perk).
        /// </summary>
        public UnityEvent<Perk> OnPerkAppliedFirstTimeInSession => onPerkAppliedFirstTimeInSession;
        /// <summary>
        /// Gets fired the very first time a perk is applied.
        /// </summary>
        public UnityEvent<Perk> OnPerkAppliedFirstTimeEver => onPerkAppliedFirstTimeEver;

        public void Initialize()
        {
            if(string.IsNullOrEmpty(id))
                TafraDebugger.Log("Perks Group", "The perks group must have an ID.", TafraDebugger.LogType.Error, this);

            savedOfferPerksCountSaveKey = $"{id}_SAVED_OFFER_PERKS_COUNT";
            savedOfferPerksSaveKey = $"{id}_SAVED_OFFER_PERKS";
            appliedPerksSaveKey = $"{id}_APPLIED_PERKS";
            totalOffersCountSaveKey = $"{id}_TOTAL_OFFERS_COUNT";
            
            isInitialized = true;

            OnInitialize();

            LoadAndApplySavedPerks();
        }

        /// <summary>
        /// Gets a number of perks from the group, and fills the list you sent with them.
        /// </summary>
        /// <param name="count">The number of perks to get, if available.</param>
        /// <param name="emptyListToFill">The list that you've made sure to clear before sendint it. Will contain the required perks.</param>
        /// <param name="mustHaveAllPerks">If true, the offer will fail if it couldn't find the required number of perks, otherwise, it will succeed if it found at least 1.</param>
        /// <param name="forceNewOffer">Offers are saved until concluded. So if you don't to conclude an offer and still want to get a new one (reroll) set this to true.</param>
        /// <returns>Whether or not the offer was successfully created.</returns>
        public bool GetOffer(int count, List<Perk> emptyListToFill, List<Perk> excludedPerks = null, bool mustHaveAllPerks = false, bool forceNewOffer = false)
        {
            if(!isInitialized)
            {
                TafraDebugger.Log("Perks Group", "Group isn't initialized. Can't get an offer. Please add the group to the \"Perks Handler\" settings.", TafraDebugger.LogType.Error, this);
                return false;
            }

            bool foundPreviousOffer = false;

            //If the last offer displayed this session is not concluded, then it should be returned.
            if(activePerksOffer.Count == count)
                foundPreviousOffer = true;

            //If there were no unconcluded offers, then look in the save data (only needs to happen once for the first offer request this session).
            if(!forceNewOffer && !savedOfferChecked && !foundPreviousOffer)
            {
                int savedOfferPerksCount = TafraSaveSystem.LoadInt(savedOfferPerksCountSaveKey, 0);

                if(savedOfferPerksCount == count)
                {
                    string savedOfferString = TafraSaveSystem.LoadString(savedOfferPerksSaveKey, "");

                    if(!string.IsNullOrEmpty(savedOfferString))
                    {
                        savedOfferString.SplitNonAlloc(',', tempSavedOfferPerks);

                        activePerksOffer.Clear();

                        bool corruptedSave = false;
                        for(int i = 0; i < tempSavedOfferPerks.Count; i++)
                        {
                            string savedPerkId = tempSavedOfferPerks[i];

                            if(!perksById.TryGetValue(savedPerkId, out Perk savedPerk))
                            {
                                corruptedSave = true;
                                break;
                            }

                            activePerksOffer.Add(savedPerk);
                        }

                        if(!corruptedSave)
                            foundPreviousOffer = true;
                    }
                }
            }
            savedOfferChecked = true;

            //If one of the perks in the previous offer is no longer available, then expire the offer.
            if(foundPreviousOffer)
            {
                bool expireOffer = false;
                for(int i = 0; i < activePerksOffer.Count; i++)
                {
                    var perk = activePerksOffer[i];

                    if(perk == null || !perk.CanBeOffered())
                    {
                        expireOffer = true;
                        break;
                    }
                }

                if(activePerksOffer.Count == 0)
                {
                    expireOffer = true;
                }

                if(!expireOffer)
                {
                    emptyListToFill.AddRange(activePerksOffer);
                    return true;
                }
            }

            //If we reach this point, then we didn't find a suitable previous offer. Create a new one.
            bool success = CreateOffer(count, emptyListToFill, excludedPerks, mustHaveAllPerks);

            if(success)
            {
                SaveOffer(emptyListToFill);

                totalCreatedOffersCount++;
                TafraSaveSystem.SaveInt(savedOfferPerksCountSaveKey, totalCreatedOffersCount);
            }

            return success;
        }
        public bool ExpandActiveOffer(int extraPerksCount, List<Perk> emptyListToFill, List<Perk> excludedPerks = null, bool mustHaveAllPerks = false)
        {
            if(!isInitialized)
            {
                TafraDebugger.Log("Perks Group", "Group isn't initialized. Can't expand offer. Please add the group to the \"Perks Handler\" settings.", TafraDebugger.LogType.Error, this);
                return false;
            }

            if(activePerksOffer.Count == 0)
            {
                TafraDebugger.Log("Perks Group", "There is no displayed offer to expand.", TafraDebugger.LogType.Error);
                return false;
            }

            tempExcludedPerks.Clear();
            tempExcludedPerks.AddRange(activePerksOffer);

            if (excludedPerks != null)
                tempExcludedPerks.AddRange(excludedPerks);

            tempActivePerksOffer.Clear();
            tempActivePerksOffer.AddRange(activePerksOffer);

            bool succes = CreateOffer(extraPerksCount, emptyListToFill, tempExcludedPerks, mustHaveAllPerks);
         
            activePerksOffer.InsertRange(0, tempActivePerksOffer);

            if(!succes)
                return false;

            return true;
        }
        /// <summary>
        /// Call this once the player has picked a perk from the displayed offer.
        /// </summary>
        /// <param name="selectedPerkIndex"></param>
        public void ConcludeOffer(int selectedPerkIndex)
        {
            Perk selectedPerk = activePerksOffer[selectedPerkIndex];
            
            ApplyPerk(selectedPerk);
            
            OnConcludeOffer(selectedPerkIndex);
          
            activePerksOffer.Clear();

            TafraSaveSystem.DeleteKey(savedOfferPerksCountSaveKey);
            TafraSaveSystem.DeleteKey(savedOfferPerksSaveKey);
        }
        public void ApplyPerk(Perk perk)
        {
            bool perkAlreadyApplied = perk.IsApplied;
          
            perk.Apply();

            //Only add the perk to the applied perks list and save it if it wasn't already applied by this group before and it wasn't applied by something else before.
            if(!appliedPerks.Contains(perk) && !perkAlreadyApplied)
            {
                appliedPerks.Add(perk);
                SaveAppliedPerks();
                OnPerkApplyFirstTime(perk);
                onPerkAppliedFirstTimeEver?.Invoke(perk);
                onPerkAppliedFirstTimeInSession?.Invoke(perk);
            }
            else
                onPerkReapplied?.Invoke(perk);

            OnPerkApply(perk);
            onPerkApplied?.Invoke(perk);
        }
        public void SceneLoaded()
        {
            for (int i = 0; i < appliedPerks.Count; i++)
            {
                appliedPerks[i].SceneLoaded();
            }
        }
        public virtual void ResetSavedData()
        {
            for(int i = 0; i < appliedPerks.Count; i++)
            {
                var perk = appliedPerks[i];

                perk.ResetSavedData();
            }
            
            appliedPerks.Clear();
            appliedPerksSB.Clear();

            TafraSaveSystem.DeleteKey(savedOfferPerksCountSaveKey);
            TafraSaveSystem.DeleteKey(savedOfferPerksSaveKey);
            TafraSaveSystem.DeleteKey(appliedPerksSaveKey);
        }

        protected void LoadAndApplySavedPerks()
        {
            totalCreatedOffersCount = TafraSaveSystem.LoadInt(savedOfferPerksCountSaveKey);

            string loadedPerkIdsString = TafraSaveSystem.LoadString(appliedPerksSaveKey);

            if(string.IsNullOrEmpty(loadedPerkIdsString))
                return;

            string[] loadedPerkIds = loadedPerkIdsString.Split(',', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < loadedPerkIds.Length; i++)
            {
                var id = loadedPerkIds[i];
                
                if(perksById.TryGetValue(id, out var perk))
                {
                    perk.PhantomApply();
                    appliedPerks.Add(perk);
                    onPerkAppliedFirstTimeInSession?.Invoke(perk);
                }
            }
        }
        protected void SaveAppliedPerks()
        {
            appliedPerksSB.Clear();

            for (int i = 0; i < appliedPerks.Count; i++)
            {
                var perk = appliedPerks[i];

                appliedPerksSB.Append(perk.ID);

                if (i <  appliedPerks.Count - 1)
                    appliedPerksSB.Append(',');
            }

            TafraSaveSystem.SaveString(appliedPerksSaveKey, appliedPerksSB.ToString());
        }
        protected void SaveOffer(List<Perk> offer)
        {
            TafraSaveSystem.SaveInt(savedOfferPerksCountSaveKey, offer.Count);

            savedOfferSB.Clear();

            for(int i = 0; i < offer.Count; i++)
            {
                var perk = offer[i];

                savedOfferSB.Append(perk.ID);

                if (i < offer.Count - 1)
                    savedOfferSB.Append(',');
            }

            TafraSaveSystem.SaveString(savedOfferPerksSaveKey, savedOfferSB.ToString());
        }
        protected abstract bool CreateOffer(int count, List<Perk> emptyListToFill, List<Perk> excludedPerks = null, bool mustHaveAllPerks = false);
        protected virtual void OnInitialize() { }
        protected virtual void OnConcludeOffer(int selectedPerkIndex) { }
        protected virtual void OnPerkApplyFirstTime(Perk perk) { }
        protected virtual void OnPerkApply(Perk perk) { }

        public abstract bool ContainsPerk(Perk perk);
    }
}