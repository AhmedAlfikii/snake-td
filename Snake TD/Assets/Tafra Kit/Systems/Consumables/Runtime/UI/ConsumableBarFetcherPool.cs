using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZUI;

namespace TafraKit.Consumables
{
    public class ConsumableBarFetcherPool : MonoBehaviour
    {
        [SerializeField] private List<ConsumableUIBar> barsPool;

        private Dictionary<Consumable, ConsumableUIBar> consumableBars = new Dictionary<Consumable, ConsumableUIBar>();
        private Dictionary<Consumable, UIElement> consumableBarUIEs = new Dictionary<Consumable, UIElement>();

        public void Initialize()
        {
            for(int i = 0; i < barsPool.Count; i++)
            {
                ConsumableUIBar bar = barsPool[i];

                consumableBars.TryAdd(bar.Consumable, bar);

                UIElement consumableBarUIE = bar.GetComponent<UIElement>();

                if(consumableBarUIE != null)
                {
                    consumableBarUIEs.Add(bar.Consumable, consumableBarUIE);
                    consumableBarUIE.ChangeVisibilityImmediate(false);
                }
            }
        }
        public bool TryGetBar(Consumable consumable, out ConsumableUIBar bar)
        {
            return consumableBars.TryGetValue(consumable, out bar);
        }

        public void ShowBar(Consumable barConsumable)
        {
            if (consumableBarUIEs.TryGetValue(barConsumable, out UIElement consumableBarUIE))
                consumableBarUIE.ChangeVisibility(true);
        }
        public void HideBar(Consumable barConsumable)
        {
            if (consumableBarUIEs.TryGetValue(barConsumable, out UIElement consumableBarUIE))
                consumableBarUIE.ChangeVisibility(false);
        }
    }
}