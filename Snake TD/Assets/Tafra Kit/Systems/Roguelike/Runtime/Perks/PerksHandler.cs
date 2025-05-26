using System;
using System.Collections;
using System.Collections.Generic;
using TafraKit.Internal;
using TafraKit.Internal.Roguelike;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace TafraKit.Roguelike
{
    public static class PerksHandler
    {
        private class OfferRequestData
        {
            public PerksGroup perksGroup;
            public int count;
            public List<Perk> excludedPerks;
            public bool mustHaveAllPerks;
            public bool forceNewOffer = false;
            public Action<Perk> onConcluded = null;
        }

        private static PerksHandlerSettings settings;

        private static List<PerksGroup> perkGroups;
        private static bool initializedGroups;
        private static List<Perk> displayedOfferPerks = new List<Perk>();
        private static List<Perk> appliedPerks = new List<Perk>();
        private static PerksGroup displayedOfferPerksGroup;
        private static List<OfferRequestData> queuedOfferRequests = new List<OfferRequestData>();
        private static UnityEvent<List<Perk>, PerksGroup> onOfferDisplayOrder = new UnityEvent<List<Perk>, PerksGroup>();
        private static UnityEvent<Perk> onPerkApplied = new UnityEvent<Perk>();
        private static UnityEvent<Perk> onPerkReapplied = new UnityEvent<Perk>();
        private static UnityEvent<Perk> onPerkAppliedFirstTimeInSession = new UnityEvent<Perk>();
        private static UnityEvent<Perk> onPerkAppliedFirstTimeEver = new UnityEvent<Perk>();
        private static Action<Perk> onOfferConcluded;

        public static List<PerksGroup> PerkGroups => perkGroups;
        public static List<Perk> AppliedPerks => appliedPerks;

        /// <summary>
        /// Gets fired whenever an offer should be displayed. Contains the list of perks, and the perks group that the offer belongs to.
        /// </summary>
        public static UnityEvent<List<Perk>, PerksGroup> OnOfferDisplayOrder => onOfferDisplayOrder;
        /// <summary>
        /// Gets fired whenever a perk is applied, whether it's the first time or otherwise.
        /// </summary>
        public static UnityEvent<Perk> OnPerkApplied => onPerkApplied;
        /// <summary>
        /// Gets fired when a perk was applied before in this session, and gets applied again (increases its applies count).
        /// </summary>
        public static UnityEvent<Perk> OnPerkReapplied => onPerkReapplied;
        /// <summary>
        /// Gets fired when a perk is applied for the first time in this session (could be a loaded perk).
        /// </summary>
        public static UnityEvent<Perk> OnPerkAppliedFirstTimeInSession => onPerkAppliedFirstTimeInSession;
        /// <summary>
        /// Gets fired the very first time a perk is applied.
        /// </summary>
        public static UnityEvent<Perk> OnPerkAppliedFirstTimeEver => onPerkAppliedFirstTimeEver;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            settings = TafraSettings.GetSettings<PerksHandlerSettings>();

            if(settings == null || !settings.Enabled)
                return;

            perkGroups = settings.PerkGroups;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            GeneralCoroutinePlayer.StartCoroutine(LateOnSceneLoaded(scene, loadMode));
        }
        private static IEnumerator LateOnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            //We're only concerned about full scene loads.
            if(loadMode == LoadSceneMode.Additive)
                yield break;

            //The first frame of the game is always 0. And in frame 0, WaitForEndOfFrame actually skips a frame then waits for the end of that new frame.
            //So, to guarantee that the perks will be applied at the end of frame 1, we will skip frame 0.
            if(Time.frameCount == 0)
                yield return null;

            //We want to wait for the end of the frame to make sure that the player equipment were equipped, since this happens on start, and some perks depend on equipment.
            yield return Yielders.EndOfFrame;

            //Initialize the groups if this is the first time a scene load occurs.
            if(!initializedGroups)
            {
                for(int i = 0; i < perkGroups.Count; i++)
                {
                    var group = perkGroups[i];

                    if(group == null)
                        continue;

                    group.OnPerkApplied.AddListener(OnPerkApply);
                    group.OnPerkReapplied.AddListener(OnPerkReapply);
                    group.OnPerkAppliedFirstTimeInSession.AddListener(OnPerkApplyFirstTimeInSession);
                    group.OnPerkAppliedFirstTimeEver.AddListener(OnPerkApplyFirstTimeEver);

                    group.Initialize();
                }

                initializedGroups = true;
            }
            else
            {
                //Inform all the groups that a new scene has been loaded so that they can inform their applied perks.
                //It's important to do this only if the groups were initialized before, since we don't want to double apply their perks (by loading them and by telling it that the a scene was loaded).
                for(int i = 0; i < perkGroups.Count; i++)
                {
                    var group = perkGroups[i];

                    if(group == null)
                        continue;

                    group.SceneLoaded();
                }
            }
        }

        private static void OnPerkApply(Perk perk)
        {
            onPerkApplied?.Invoke(perk);
        }
        private static void OnPerkReapply(Perk perk)
        {
            onPerkReapplied?.Invoke(perk);
        }
        private static void OnPerkApplyFirstTimeInSession(Perk perk)
        {
            appliedPerks.Add(perk);

            onPerkAppliedFirstTimeInSession?.Invoke(perk);
        }
        private static void OnPerkApplyFirstTimeEver(Perk perk)
        {
            onPerkAppliedFirstTimeEver?.Invoke(perk);
        }

        /// <summary>
        /// Gets a number of perks from the group, and fills the list you sent with them.
        /// </summary>
        /// <param name="perksGroupIndex">The index of the perks group in the perks handler settings.</param>
        /// <param name="count">The number of perks to get, if available.</param>
        /// <param name="emptyListToFill">The list that you've made sure to clear before sendint it. Will contain the required perks.</param>
        /// <param name="excludedPerks">The list of perks that you don't want to appear in the offer.</param>
        /// <param name="mustHaveAllPerks">If true, the offer will fail if it couldn't find the required number of perks, otherwise, it will succeed if it found at least 1.</param>
        /// <param name="forceNewOffer">Offers are saved until concluded. So if you don't to conclude an offer and still want to get a new one (reroll) set this to true.</param>
        /// <returns>Whether or not the offer was successfully created.</returns>
        public static bool GetOffer(int perksGroupIndex, int count, List<Perk> emptyListToFill, List<Perk> excludedPerks = null, bool mustHaveAllPerks = false, bool forceNewOffer = false)
        {
            return GetOffer(perkGroups[perksGroupIndex], count, excludedPerks, emptyListToFill, mustHaveAllPerks, forceNewOffer);
        }
        /// <summary>
        /// Gets a number of perks from the group, and fills the list you sent with them.
        /// </summary>
        /// <param name="perksGroup">The perks group you want to get the perks from.</param>
        /// <param name="count">The number of perks to get, if available.</param>
        /// <param name="emptyListToFill">The list that you've made sure to clear before sendint it. Will contain the required perks.</param>
        /// <param name="excludedPerks">The list of perks that you don't want to appear in the offer.</param>
        /// <param name="mustHaveAllPerks">If true, the offer will fail if it couldn't find the required number of perks, otherwise, it will succeed if it found at least 1.</param>
        /// <param name="forceNewOffer">Offers are saved until concluded. So if you don't to conclude an offer and still want to get a new one (reroll) set this to true.</param>
        /// <returns>Whether or not the offer was successfully created.</returns>
        public static bool GetOffer(PerksGroup perksGroup, int count, List<Perk> emptyListToFill, List<Perk> excludedPerks = null, bool mustHaveAllPerks = false, bool forceNewOffer = false)
        {
            return perksGroup.GetOffer(count, emptyListToFill, excludedPerks, mustHaveAllPerks, forceNewOffer);
        }
        /// <summary>
        /// Display an offer containing the required number of perks if found.
        /// </summary>
        /// <param name="perksGroupIndex">The index of the perks group in the perks handler settings.</param>
        /// <param name="count">The number of perks to get, if available.</param>
        /// <param name="excludedPerks">The list of perks that you don't want to appear in the offer.</param>
        /// <param name="mustHaveAllPerks">If true, the offer will fail if it couldn't find the required number of perks, otherwise, it will succeed if it found at least 1.</param>
        /// <param name="forceNewOffer">Offers are saved until concluded. So if you don't to conclude an offer and still want to get a new one (reroll) set this to true.</param>
        /// <returns>Whether or not the offer was successfully created.</returns>
        public static bool DisplayOffer(int perksGroupIndex, int count, List<Perk> excludedPerks = null, bool mustHaveAllPerks = false, bool forceNewOffer = false, Action<Perk> onConcluded = null)
        {
            return DisplayOffer(perkGroups[perksGroupIndex], count, excludedPerks, mustHaveAllPerks, forceNewOffer);
        }
        /// <summary>
        /// Display an offer containing the required number of perks if found.
        /// </summary>
        /// <param name="perksGroup">The perks group you want to get the perks from.</param>
        /// <param name="count">The number of perks to get, if available.</param>
        /// <param name="excludedPerks">The list of perks that you don't want to appear in the offer.</param>
        /// <param name="mustHaveAllPerks">If true, the offer will fail if it couldn't find the required number of perks, otherwise, it will succeed if it found at least 1.</param>
        /// <param name="forceNewOffer">Offers are saved until concluded. So if you don't to conclude an offer and still want to get a new one (reroll) set this to true.</param>
        /// <returns>Whether or not the offer was successfully created.</returns>
        public static bool DisplayOffer(PerksGroup perksGroup, int count, List<Perk> excludedPerks = null, bool mustHaveAllPerks = false, bool forceNewOffer = false, Action<Perk> onConcluded = null)
        {
            if(displayedOfferPerksGroup != null)
            {
                //Queue offer.
                OfferRequestData offerRequestData = new OfferRequestData();
                
                offerRequestData.perksGroup = perksGroup;
                offerRequestData.count = count;
                offerRequestData.excludedPerks = excludedPerks;
                offerRequestData.mustHaveAllPerks = mustHaveAllPerks;
                offerRequestData.forceNewOffer = forceNewOffer;
                offerRequestData.onConcluded = onConcluded;

                queuedOfferRequests.Add(offerRequestData);

                return true;
            }

            onOfferConcluded = onConcluded;

            displayedOfferPerks.Clear();
            displayedOfferPerksGroup = null;

            bool foundOffer = perksGroup.GetOffer(count, displayedOfferPerks, excludedPerks, mustHaveAllPerks, forceNewOffer);

            if(!foundOffer)
                return false;

            displayedOfferPerksGroup = perksGroup;

            onOfferDisplayOrder?.Invoke(displayedOfferPerks, perksGroup);

            return true;
        }
        public static bool ExpandDisplayedOffer(int extraPerksCount, List<Perk> emptyListToFill, List<Perk> excludedPerks = null, bool mustHaveAllPerks = false)
        {
            if(displayedOfferPerks.Count == 0 || displayedOfferPerksGroup == null)
            {
                TafraDebugger.Log("Perks Handler", "There is no displayed offer to expand.", TafraDebugger.LogType.Error);
                return false;
            }

            bool success = displayedOfferPerksGroup.ExpandActiveOffer(extraPerksCount, emptyListToFill, excludedPerks, mustHaveAllPerks);

            if(success)
                displayedOfferPerks.AddRange(emptyListToFill);

            return success;
        }
        /// <summary>
        /// Call this once the player has picked a perk from the displayed offer. So that the group applies the perk, and unsaves the offer (among other things).
        /// </summary>
        /// <param name="perksGroup"></param>
        /// <param name="selectedPerkIndex"></param>
        public static void ConcludeOffer(int selectedPerkIndex)
        {
            onOfferConcluded?.Invoke(displayedOfferPerks[selectedPerkIndex]);

            displayedOfferPerks.Clear();

            displayedOfferPerksGroup.ConcludeOffer(selectedPerkIndex);

            displayedOfferPerksGroup = null;

            if(queuedOfferRequests.Count > 0)
            {
                var offerRequestData = queuedOfferRequests[0];

                DisplayOffer(offerRequestData.perksGroup, offerRequestData.count, offerRequestData.excludedPerks, offerRequestData.mustHaveAllPerks, offerRequestData.forceNewOffer, offerRequestData.onConcluded);

                queuedOfferRequests.RemoveAt(0);
            }
        }

        public static void ResetSavedData()
        {
            for(int i = 0; i < perkGroups.Count; i++)
            {
                perkGroups[i].ResetSavedData();
            }

            appliedPerks.Clear();
        }
    }
}