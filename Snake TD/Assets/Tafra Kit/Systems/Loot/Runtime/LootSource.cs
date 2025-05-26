using System.Collections;
using System.Collections.Generic;
using TafraKit;
using UnityEngine;

namespace TafraKit.Loot
{
    public class LootSource : MonoBehaviour
    {
        #region Private Serialized Fields
        [SerializeField] private ListenToEvent dropLootAction;

        [Header("Loot Spawning Fields")]
        [SerializeField] private DropLootAnimationData dropLootAnimationData;
        [Header("Loot Items")]
        [SerializeField] private LootLayersData layersData;
        [Tooltip("If no custom spawn point was assigned, this transform will be used as the spawn point.")]
        [SerializeField] private Transform customSpawnPoint;
        #endregion

        #region Private Fields
        private List<LootItem> pickedItems = new List<LootItem>();
        private List<LootData> tempDroppedItems = new List<LootData>();
        #endregion

        private Transform SpawnPoint 
        {
            get 
            {
                if (customSpawnPoint != null)
                    return customSpawnPoint;
                else
                    return transform;
            }
        }

        #region Monobehaviour Messages
        protected virtual void Awake()
        {
            dropLootAction.Initialize(DropLoot);
        }
        protected virtual void OnEnable()
        {
            dropLootAction.StartListening();
        }
        protected virtual void OnDisable()
        {
            dropLootAction.StopListening();
        }
        #endregion

        #region Private Methods
        private void PickItems()
        {
            if(layersData == null)
                return;

            pickedItems.Clear();

            LootLayer[] layers = layersData.Layers;

            for (int i = 0; i < layers.Length; i++)
            {
                LootLayer layer = layers[i];

                float layerProp = layer.Probability;

                if (Random.value > layerProp)
                    continue;

                LootItem[] items = layer.Items;

                LootItem item = items[Random.Range(0, items.Length)];

                pickedItems.Add(item);
            }
        }
        #endregion

        #region Public Methods
        public void DropLoot()
        {
            PickItems();

            if (pickedItems.Count == 0) return;

            tempDroppedItems.Clear();

            for (int i = 0; i < pickedItems.Count; i++)
            {
                LootItem item = pickedItems[i];

                int amount = item.GetValue();

                if (amount == 0) continue;

                for (int j = 0; j < amount; j++)
                {
                    tempDroppedItems.Add(item.lootData);
                }
            }

            LootHandler.DropLoot(tempDroppedItems, SpawnPoint, dropLootAnimationData);
        }
        #endregion
    }
}