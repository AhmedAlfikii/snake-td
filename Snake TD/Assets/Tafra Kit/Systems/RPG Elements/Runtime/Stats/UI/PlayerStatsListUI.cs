using System.Collections.Generic;
using TafraKit.RPG;
using TafraKit.UI;
using UnityEngine;

namespace TafraKit.Internal.RPGElements
{
    public class PlayerStatsListUI : MonoBehaviour
    {
        [SerializeField] private DynamicPool<StatSlotUI> slotsPool = new DynamicPool<StatSlotUI>();

        private bool isInitialized;
        private PlayerStatsSettings settings;
        private List<StatSlotUI> activeSlots = new List<StatSlotUI>();

        private void Start()
        {
            if(!isInitialized)
                Initialize();
        }
        private void OnDestroy()
        {
            if(settings == null)
                return;

            for(int i = 0; i < settings.UsedSats.Count; i++)
            {
                settings.UsedSats[i].stat.Release();
            }
        }

        private void OnSlotClicked(StatSlotUI slot)
        {
            InfoBubbleHandler.Show(slot.GetComponent<RectTransform>(), Side.Top, slot.Stat.Description, slot.Stat.DisplayName);
        }

        public void Initialize()
        {
            if(isInitialized)
                return;

            settings = TafraSettings.GetSettings<PlayerStatsSettings>();

            if(settings == null)
                return;

            slotsPool.Initialize();

            for(int i = 0; i < settings.UsedSats.Count; i++)
            {
                var statSettings = settings.UsedSats[i];

                StatSlotUI slot = slotsPool.RequestUnit(activateUnit: false);
                Stat stat = statSettings.stat.Load();

                slot.SetRounding(statSettings.rounding);
                slot.SetDisplayAsPercentage(stat.IsPercentage);
                slot.SetStat(stat);
                slot.Initialize(() => { 
                    OnSlotClicked(slot);
                });

                slot.gameObject.SetActive(true);

                activeSlots.Add(slot);
            }

            isInitialized = true;
        }
    }
}