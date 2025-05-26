using System;
using System.Collections.Generic;
using UnityEngine;

namespace TafraKit.Consumables
{
    public static class ConsumableMerchant
    {
        private static ConsumablesSettings consumablesSettings;
        private static OutOfConsumablePopup outOfConsumablesPopup;
        private static Dictionary<Consumable, ConsumableUIBar> consumableBarByConsumable = new Dictionary<Consumable, ConsumableUIBar>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            consumablesSettings = TafraSettings.GetSettings<ConsumablesSettings>();

            if(consumablesSettings == null)
                return;

            if(consumablesSettings.OutOfConsumablesPopup != null)
            {
                GameObject popupGO = GameObject.Instantiate(consumablesSettings.OutOfConsumablesPopup.gameObject);
                GameObject.DontDestroyOnLoad(popupGO);

                outOfConsumablesPopup = popupGO.GetComponent<OutOfConsumablePopup>();
            }

            for (int i = 0; i < consumablesSettings.consumableBarByConsumable.Length; i++)
            {
                var cbbc = consumablesSettings.consumableBarByConsumable[i];

                consumableBarByConsumable.TryAdd(cbbc.consumable, cbbc.barPrefab);
            }
        }

        public static void DisplayOutOfConsumablesPopup(Consumable consumable, float consumptionAmount,Action onConsumedAction = null)
        {
            outOfConsumablesPopup.Show(consumable, consumptionAmount,onConsumedAction);
        }

        public static bool Consume(Consumable consumable, float consumptionAmount,Action onConsumed = null, bool displayUnAffordablePopup = true)
        {
            if(consumable == null)
                return false;

            if(consumable.Value < consumptionAmount)
            {
                if(displayUnAffordablePopup)
                    DisplayOutOfConsumablesPopup(consumable, consumptionAmount,onConsumed);

                return false;
            }

            onConsumed?.Invoke();

            return consumable.Deduct(consumptionAmount);
        }
        public static bool Consume(ConsumableChange consumableChange,Action onConsumed = null, bool displayUnAffordablePopup = true)
        {
            if(consumableChange == null || consumableChange.consumable == null)
                return false;

            Consumable consumable = consumableChange.consumable;

            if(consumable.Value < consumableChange.changeAmount)
            {
                if(displayUnAffordablePopup)
                    DisplayOutOfConsumablesPopup(consumable, consumableChange.changeAmount,onConsumed);

                return false;
            }
            
            onConsumed?.Invoke();

            return consumable.Deduct(consumableChange.changeAmount);
        }
        public static bool Consume(ConsumableChange[] consumableChanges,Action onConsumed = null, bool displayUnAffordablePopup = true)
        {
            if(consumableChanges == null)
                return false;

            if(consumableChanges.Length == 0)
                return true;

            for(int i = 0; i < consumableChanges.Length; i++)
            {
                Consumable consumable = consumableChanges[i].consumable;

                if(consumable.Value < consumableChanges[i].changeAmount)
                {
                    
                    //TODO: loop breaks when the consumable not affordable.
                    if(displayUnAffordablePopup)
                        DisplayOutOfConsumablesPopup(consumable, consumableChanges[i].changeAmount,onConsumed);

                    return false;
                }
            }

            for(int i = 0; i < consumableChanges.Length; i++)
            {
                consumableChanges[i].consumable.Deduct(consumableChanges[i].changeAmount);
            }
            
            onConsumed?.Invoke();
            
            return true;
        }

        public static bool IsAffordable(Consumable consumable, float consumptionAmount, bool displayUnAffordablePopup = false, Action onConsumed = null)
        {
            if (consumable == null)
                return false;

            if (consumable.Value < consumptionAmount)
            {
                if (displayUnAffordablePopup)
                    DisplayOutOfConsumablesPopup(consumable, consumptionAmount, onConsumed);

                return false;
            }

            return true;
        }
        public static bool IsAffordable(ConsumableChange[] consumableChanges, bool displayUnAffordablePopup = false)
        {
            if(consumableChanges == null)
                return false;

            if(consumableChanges.Length == 0)
                return true;

            for(int i = 0; i < consumableChanges.Length; i++)
            {
                Consumable consumable = consumableChanges[i].consumable;

                if(consumable.Value < consumableChanges[i].changeAmount)
                {
                    if (displayUnAffordablePopup)
                        DisplayOutOfConsumablesPopup(consumable, consumableChanges[i].changeAmount);

                    return false;
                }
            }

            return true;
        }

        public static Consumable GetConsumableByName(string name)
        { 
            return Resources.Load<Consumable>($"Consumables/{name}");
        }

        public static bool TryGetConsumableBarPrefab(Consumable consumable, out ConsumableUIBar barPrefab)
        {
            if (consumableBarByConsumable.TryGetValue(consumable, out barPrefab) && barPrefab != null)
                return true;
            else
                return false;
        }
    }
}