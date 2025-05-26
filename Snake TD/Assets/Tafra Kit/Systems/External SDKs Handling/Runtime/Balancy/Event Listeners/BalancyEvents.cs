#if TAFRA_BALANCY
using Balancy;
using Balancy.Data.SmartObjects;
using Balancy.Interfaces;
using Balancy.Models.LiveOps.Store;
using Balancy.Models.SmartObjects;
using Balancy.SmartObjects;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace TafraKit.ExternalSDKs.BalancySDK
{
    public class BalancyEvents : ISmartObjectsEvents, ILiveOpsEvents, IStoreEvents
    {
        private static bool debugLogs = false;
        private bool smartObjectsInitialized;
        private bool isPreProfileSynced;
        private bool isProfileSynced;

        public bool AreSmartObjectsInitialized => smartObjectsInitialized;
        public bool IsPreProfileSynced => isPreProfileSynced;
        public bool IsProfileSynced => isProfileSynced;
       
        public UnityEvent<LiveOps.ABTests.TestData> AbTestEnded = new UnityEvent<LiveOps.ABTests.TestData>();
        public UnityEvent<LiveOps.ABTests.TestData> AbTestStarted = new UnityEvent<LiveOps.ABTests.TestData>();
        public UnityEvent<EventInfo> EventDeactivated = new UnityEvent<EventInfo>();
        public UnityEvent<EventInfo> NewEventActivated = new UnityEvent<EventInfo>();
        public UnityEvent<OfferInfo> NewOfferActivated = new UnityEvent<OfferInfo>();
        public UnityEvent<OfferGroupInfo> NewOfferGroupActivated = new UnityEvent<OfferGroupInfo>();
        public UnityEvent<OfferInfo, bool> OfferDeactivated = new UnityEvent<OfferInfo, bool>();
        public UnityEvent<OfferInfo, string> OfferFailedToPurchase = new UnityEvent<OfferInfo, string>();
        public UnityEvent<OfferGroupInfo, bool> OfferGroupDeactivated = new UnityEvent<OfferGroupInfo, bool>();
        public UnityEvent<OfferGroupInfo, StoreItem> OfferGroupPurchased = new UnityEvent<OfferGroupInfo, StoreItem>();
        public UnityEvent<OfferInfo> OfferPurchased = new UnityEvent<OfferInfo>();
        public UnityEvent<SegmentInfo> SegmentUpdated = new UnityEvent<SegmentInfo>();
        public UnityEvent SmartObjectsInitialized = new UnityEvent();
        public UnityEvent<StoreItem, string> StoreItemFailedToPurchase = new UnityEvent<StoreItem, string>();
        public UnityEvent SystemProfileConflictAppeared = new UnityEvent();
        public UnityEvent UserProfilesLoaded = new UnityEvent();
        public UnityEvent PreProfileSync = new UnityEvent();
        public UnityEvent ProfileSynced = new UnityEvent();

        public UnityEvent<Reward, int> DailyRewardAvailable = new UnityEvent<Reward, int>();

        public UnityEvent<GameStoreBase, Page> StorePageUpdated = new UnityEvent<GameStoreBase, Page>();
        public UnityEvent<float> StoreResourcesMultiplierChanged = new UnityEvent<float>();
        public UnityEvent<GameStoreBase> StoreUpdated = new UnityEvent<GameStoreBase>();


        #region Callbacks
        public void OnAbTestEnded(LiveOps.ABTests.TestData abTestInfo)
        {
            if (debugLogs)
                Debug.Log($"<color=green><b>Balancy - Smart Objects Event - AB Test Ended - Test name = {abTestInfo?.AbTest?.Name}, variant = {abTestInfo?.Variant?.Name}.</b></color>");
            
            AbTestEnded?.Invoke(abTestInfo);
        }
        public void OnAbTestStarted(LiveOps.ABTests.TestData abTestInfo)
        {
            if (debugLogs)
                Debug.Log($"<color=green><b>Balancy - Smart Objects Event - AB Test Started - Test name = {abTestInfo?.AbTest?.Name}, variant = {abTestInfo?.Variant?.Name}.</b></color>");

            AbTestStarted?.Invoke(abTestInfo);
        }
        public void OnEventDeactivated(EventInfo eventInfo)
        {
            if (debugLogs)
                Debug.Log($"<color=green><b>Balancy - Smart Objects Event - Event Deactivated - {eventInfo?.GameEvent?.Name}.</b></color>");

            EventDeactivated?.Invoke(eventInfo);
        }
        public void OnNewEventActivated(EventInfo eventInfo)
        {
            if (debugLogs)
                Debug.Log($"<color=green><b>Balancy - Smart Objects Event - New Event Activated - {eventInfo?.GameEvent?.Name}.</b></color>");

            NewEventActivated?.Invoke(eventInfo);
        }
        public void OnNewOfferActivated(OfferInfo offerInfo)
        {
            if (debugLogs)
                Debug.Log($"<color=green><b>Balancy - Smart Objects Event - New Offer Activated - {offerInfo?.GameEvent?.Name}.</b></color>");

            NewOfferActivated?.Invoke(offerInfo);
        }
        public void OnNewOfferGroupActivated(OfferGroupInfo offerInfo)
        {
            if (debugLogs)
                Debug.Log($"<color=green><b>Balancy - Smart Objects Event - New Offer Group Activated - {offerInfo?.GameEvent?.Name}.</b></color>");

            NewOfferGroupActivated?.Invoke(offerInfo);
        }
        public void OnOfferDeactivated(OfferInfo offerInfo, bool wasPurchased)
        {
            if (debugLogs)
                Debug.Log($"<color=green><b>Balancy - Smart Objects Event - Offer Deactivated - {offerInfo?.GameEvent?.Name}, was purchased? {wasPurchased}.</b></color>");

            OfferDeactivated?.Invoke(offerInfo, wasPurchased);
        }
        public void OnOfferFailedToPurchase(OfferInfo offerInfo, string error)
        {
            if (debugLogs)
                Debug.Log($"<color=green><b>Balancy - Smart Objects Event - Offer Failed to Purchase - {offerInfo?.GameEvent?.Name}, error: {error}.</b></color>");

            OfferFailedToPurchase?.Invoke(offerInfo, error);
        }
        public void OnOfferGroupDeactivated(OfferGroupInfo offerInfo, bool wasPurchased)
        {
            if (debugLogs)
                Debug.Log($"<color=green><b>Balancy - Smart Objects Event - Offer Group Deactivated - {offerInfo?.GameEvent?.Name}, was purchased? {wasPurchased}.</b></color>");

            OfferGroupDeactivated?.Invoke(offerInfo, wasPurchased);
        }
        public void OnOfferGroupPurchased(OfferGroupInfo offerInfo, StoreItem storeItem)
        {
            if (debugLogs)
                Debug.Log($"<color=green><b>Balancy - Smart Objects Event - Offer Group Purchased - {offerInfo?.GameEvent?.Name}, store item: {storeItem?.Name}.</b></color>");

            OfferGroupPurchased?.Invoke(offerInfo, storeItem);
        }
        public void OnOfferPurchased(OfferInfo offerInfo)
        {
            if (debugLogs)
                Debug.Log($"<color=green><b>Balancy - Smart Objects Event - Offer Purchased - {offerInfo?.GameEvent?.Name}.</b></color>");

            OfferPurchased?.Invoke(offerInfo);
        }
        public void OnSegmentUpdated(SegmentInfo segmentInfo)
        {
            if (debugLogs)
                Debug.Log($"<color=green><b>Balancy - Smart Objects Event - Segment Updated - {segmentInfo?.Segment?.Name}.</b></color>");

            SegmentUpdated?.Invoke(segmentInfo);
        }
        public void OnSmartObjectsInitialized()
        {
            if (debugLogs)
                Debug.Log($"<color=green><b>Balancy - Smart Objects Event - Smart Objects Initialized.</b></color>");

            smartObjectsInitialized = true;

            SmartObjectsInitialized?.Invoke();
        }
        public void OnStoreItemFailedToPurchase(StoreItem storeItem, string error)
        {
            if (debugLogs)
                Debug.Log($"<color=green><b>Balancy - Smart Objects Event - Store Item Failed to Purchase - {storeItem?.Name}, error: {error}.</b></color>");

            StoreItemFailedToPurchase?.Invoke(storeItem, error);
        }
        public void OnSystemProfileConflictAppeared()
        {
            if (debugLogs)
                Debug.Log($"<color=green><b>Balancy - Smart Objects Event - System Profile Conflict Appeared.</b></color>");

            SystemProfileConflictAppeared?.Invoke();
        }
        public void OnUserProfilesLoaded()
        {
            if (debugLogs)
                Debug.Log($"<color=green><b>Balancy - Smart Objects Event - User Profiles Loaded.</b></color>");

            UserProfilesLoaded?.Invoke();
        }
        public void OnProfileSynced()
        {
            if (debugLogs)
                Debug.Log($"<color=green><b>Balancy - User Profile Synced.</b></color>");

            isProfileSynced = true;

            ProfileSynced?.Invoke();
        }
        public void OnPreProfileSynced()
        {
            if (debugLogs)
                Debug.Log($"<color=green><b>Balancy - User Pre Profile Synced.</b></color>");

            isPreProfileSynced = true;

            PreProfileSync?.Invoke();
        }

        public void OnDailyRewardAvailable(Reward reward, int dayNumber)
        {
            if (debugLogs)
                Debug.Log($"<color=green><b>Balancy - LiveOps Event - Daily Reward Available.</b></color>");

            DailyRewardAvailable?.Invoke(reward, dayNumber);
        }

        public void OnStorePageUpdated(GameStoreBase storeConfig, Page page)
        {
            if (debugLogs)
                Debug.Log($"<color=green><b>Balancy - Store Event - Store Page Updated - Page {page?.Name}.</b></color>");

            StorePageUpdated?.Invoke(storeConfig, page);
        }
        public void OnStoreResourcesMultiplierChanged(float multiplier)
        {
            if (debugLogs)
                Debug.Log($"<color=green><b>Balancy - Store Event - Store Resources Multiplier Changed - Multiplier {multiplier}.</b></color>");

            StoreResourcesMultiplierChanged?.Invoke(multiplier);
        }
        public void OnStoreUpdated(GameStoreBase storeConfig)
        {
            if (debugLogs)
                Debug.Log($"<color=green><b>Balancy - Store Event - Store Updated.</b></color>");

            StoreUpdated?.Invoke(storeConfig);
        }
        #endregion
    }
}
#endif