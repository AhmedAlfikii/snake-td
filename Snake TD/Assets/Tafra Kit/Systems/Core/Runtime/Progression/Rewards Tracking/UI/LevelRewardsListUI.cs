using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Internal
{
    public class LevelRewardsListUI : MonoBehaviour
    {
        [SerializeField] private bool updateOnEnable = true;
        [SerializeField] private DynamicPool<ItemSlot> slotsPool;

        private LevelRewardsTracker tracker;
        private List<ItemSlot> activeSlots = new List<ItemSlot>();
        private bool isInitialized;
      
        private void Awake()
        {
            if(!isInitialized)
                Initialize();
        }

        private void OnEnable()
        {
            if(updateOnEnable)
                Refresh();
        }

        private void Initialize()
        {
            if(isInitialized)
                return;

            tracker = GameObject.FindAnyObjectByType<LevelRewardsTracker>();

            slotsPool.Initialize();

            isInitialized = true;
        }

        public void Refresh()
        {
            if(!isInitialized)
                Initialize();

            for(int i = 0; i < activeSlots.Count; i++)
            {
                slotsPool.ReleaseUnit(activeSlots[i]);
            }

            var trackers = tracker.ConsumableTrackers;

            for(int i = 0; i < trackers.Count; i++)
            {
                var tracker = trackers[i];

                if(tracker.ChangeValue <= 0)
                    continue;

                ItemSlot slot = slotsPool.RequestUnit(activateUnit: false);

                slot.Populate(tracker.Consumable, Mathf.RoundToInt(tracker.ChangeValue));

                slot.gameObject.SetActive(true);

                activeSlots.Add(slot);
            }
        }
    }
}