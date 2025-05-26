using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TafraKit.RPG;

namespace TafraKit.Loot
{
    public interface ILootCollector
    {
        Transform CollectionPoint { get; }
        Inventory LootInventory { get; }

        void CollectLoot(LootData lootData)
        {
            switch (lootData.RewardType)
            {
                case LootData.LootRewardType.AddToScriptableFloat:
                    if(lootData.FloatChange.Float == null)
                        return;

                    lootData.FloatChange.Add();
                    break;
                case LootData.LootRewardType.SubtractFromScriptableFloat:
                    if(lootData.FloatChange.Float == null)
                        return;

                    lootData.FloatChange.Deduct();
                    break;
                case LootData.LootRewardType.SetScriptableFloat:
                    if(lootData.FloatChange.Float == null)
                        return;

                    lootData.FloatChange.Set();
                    break;
                case LootData.LootRewardType.AddToInventory:
                    if(LootInventory != null && lootData.Item != null && lootData.Item is StorableScriptableObject storableItem)
                    {
                        StorableScriptableObject storableInstance = storableItem.InstancableSO.CreateInstance() as StorableScriptableObject;
                        
                        storableInstance.Quantity = Mathf.RoundToInt(lootData.FloatChange.ChangeAmount);

                        LootInventory.AddItem(storableInstance);
                    }
                    break;
                case LootData.LootRewardType.Internal:
                    lootData.OnCollected();
                    break;
            }

            OnLootCollected(lootData);
        }
        void OnLootCollected(LootData lootData);
    }
}