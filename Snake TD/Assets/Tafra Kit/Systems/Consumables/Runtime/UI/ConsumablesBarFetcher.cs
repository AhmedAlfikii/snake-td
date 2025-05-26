using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Consumables
{
    public static class ConsumablesBarFetcher
    {
        private static Dictionary<Consumable, ConsumableUIBar> visibleConsumableBars = new Dictionary<Consumable, ConsumableUIBar>();
        private static Dictionary<Consumable, ConsumableUIBar> consumablesFetchedBars = new Dictionary<Consumable, ConsumableUIBar>();
        private static Dictionary<ConsumableUIBar, HashSet<string>> fetchedBarsByFetcher = new Dictionary<ConsumableUIBar, HashSet<string>>();
        private static ConsumableBarFetcherPool consumableBarFetcherPool;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void FillPool()
        {
            consumableBarFetcherPool = GameObject.FindAnyObjectByType<ConsumableBarFetcherPool>();
            if (consumableBarFetcherPool != null)
                consumableBarFetcherPool.Initialize();
        }

        public static void SignalBarVisible(ConsumableUIBar bar)
        { 
            //visibleConsumableBars.TryAdd(bar.Consumable, bar);
            
            if (visibleConsumableBars.ContainsKey(bar.Consumable))
                visibleConsumableBars[bar.Consumable] = bar;
            else
                visibleConsumableBars.Add(bar.Consumable, bar);
        }
        public static void SignalBarInvisible(ConsumableUIBar bar)
        { 
            if (visibleConsumableBars.ContainsKey(bar.Consumable)) 
                visibleConsumableBars.Remove(bar.Consumable);
        }
        public static ConsumableUIBar TryGetVisibleBar(Consumable consumable)
        {
            return visibleConsumableBars.TryGetValue(consumable, out ConsumableUIBar bar) ? bar : null;
        }
        /// <summary>
        /// Returns the currently visible consumable bar, or gets one from the pool if there isn't any.
        /// </summary>
        /// <param name="consumable">The consumable to get a bar for.</param>
        /// <param name="controller">The id of the fetcher that should abandon this bar later.</param>
        /// <returns></returns>
        public static ConsumableUIBar Fetch(Consumable consumable, string controller)
        {
            ConsumableUIBar bar = null;

            //First check if this bar is already fetched and visible, if so, use it.
            consumablesFetchedBars.TryGetValue(consumable, out bar);

            //If it wasn't already fetched, then see if it is already visible in the scene.
            if (bar == null)
                visibleConsumableBars.TryGetValue(consumable, out bar);

            //If we didn't get a bar until now, then we should get it from the pool.
            if (bar == null && consumableBarFetcherPool != null)
            {
                if(consumableBarFetcherPool.TryGetBar(consumable, out bar))
                {
                    consumableBarFetcherPool.ShowBar(consumable);
                    bar.transform.SetAsLastSibling();
                }
            }

            //If we still didn't get a bar. Then I don't know what else to do...
            if (bar == null)
            {
                TafraDebugger.Log("Consumable Bar Fetcher", "Couldn't find a bar for the given consumable, make sure to add a bar in the pool.", TafraDebugger.LogType.Info);
                return null;
            }

            //If we reached this point, then we have a bar.

            if (fetchedBarsByFetcher.ContainsKey(bar))
            {
                if (!fetchedBarsByFetcher[bar].Contains(controller))
                    fetchedBarsByFetcher[bar].Add(controller);
            }
            else
            {
                HashSet<string> hSet = new HashSet<string> { controller };

                fetchedBarsByFetcher.Add(bar, hSet);
            }

            consumablesFetchedBars.TryAdd(consumable, bar);

            return bar;
        }
        public static void Abandon(Consumable consumable, string controller)
        {
            ConsumableUIBar bar = null;

            if (consumablesFetchedBars.TryGetValue(consumable, out bar))
            {
                HashSet<string> controllers;
                if (fetchedBarsByFetcher.TryGetValue(bar, out controllers))
                { 
                    if (controllers.Contains(controller))
                        controllers.Remove(controller);

                    if (controllers.Count == 0)
                    { 
                        fetchedBarsByFetcher.Remove(bar);
                        consumablesFetchedBars.Remove(consumable);
                        //Hide the bar from the pool if it was visible from there.
                        //consumableBarFetcherPool.HideBar(consumable);
                    }
                }
            }
        }

        public static void MarkBarAsSuperior(Consumable consumable)
        {
            ConsumableUIBar bar = null;

            if(consumableBarFetcherPool != null)
                consumableBarFetcherPool.TryGetBar(consumable, out bar);

            if(bar == null)
                return;

            //TODO: Optimize.
            Canvas barCanvas = bar.GetComponent<Canvas>();

            if(barCanvas == null)
                return;

            barCanvas.overrideSorting = true;
            barCanvas.sortingOrder = 49;
        }
        public static void RemoveBarSuperiority(Consumable consumable)
        {
            ConsumableUIBar bar = null;

            if(consumableBarFetcherPool != null)
                consumableBarFetcherPool.TryGetBar(consumable, out bar);

            if(bar == null)
                return;

            //TODO: Optimize.
            Canvas barCanvas = bar.GetComponent<Canvas>();

            if(barCanvas == null)
                return;

            barCanvas.overrideSorting = false;
        }
    }
}